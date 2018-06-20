using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;

namespace AutoLoadPackageManager
{
    class AutoLoadPackageEnumerator
    {
        const string _autoLoadPackagesPath = "AutoLoadPackages";
        const string _uIContextRulesPath = "UIContextRules";

        private SettingsStore _configurationStore;
        private static Dictionary<Guid, string> _uIContextNameMapping;

        static AutoLoadPackageEnumerator()
        {
            _uIContextNameMapping = new Dictionary<Guid, string>
            {
                [Guid.Parse("fbcae063-e2c0-4ab1-a516-996ea3dafb72")] = "SQL Server Object ToolWindow",
                [Guid.Parse("f6819a78-a205-47b5-be1c-675b3c7f0b8e")] = "XML Language Service",
                [Guid.Parse("efba0ad7-5a72-4c68-af49-83d382785dcf")] = "Xamarin XAML Language Service",
                [Guid.Parse("e80ef1cb-6d64-4609-8faa-feacfd3bc89f")] = "UICONTEXT_ShellInitialized",
                [Guid.Parse("e24c65dc-7377-472b-9aba-bc803b73c61a")] = "Website Project Service",
                [Guid.Parse("e16ec553-041c-403a-a275-958469fcde74")] = "Winexpress UI Context",
                [Guid.Parse("de9f6b31-c1e5-b965-95f3-1885af956fc9")] = "UICONTEXT_SolutionHasWindowsPhone80Native",
                [Guid.Parse("de039a0e-c18f-490c-944a-888b8e86da4b")] = "UICONTEXT_ProjectRetargeting",
                [Guid.Parse("d8cdd15a-d1f0-4ad5-b0f4-2de654546d5b")] = "UICONTEXT_RepositoryOpen",
                [Guid.Parse("de039a0e-c18f-490c-944a-888b8e86da4b")] = "UICONTEXT_ProjectRetargeting",
                [Guid.Parse("d78612c7-9962-4b83-95d9-268046dad23a")] = "Error List",
                [Guid.Parse("c22bcf10-e1eb-42c6-95a5-e01418c08a29")] = "Cloud Debugging",
                [Guid.Parse("8b1141ab-519d-4c1e-a86c-510e5a56bf64")] = "Powershell Project",
                [Guid.Parse("862a616d-e4be-a6b0-59b8-b3affd6f1fbc")] = "IsAndroidApplication",
                [Guid.Parse("7cac4ae1-2e6b-4b02-a91c-71611e86f273")] = "UICONTEXT_SolutionHasAppContainerProject",
                [Guid.Parse("781d1330-8de9-429d-bf73-c74f19e4fcb1")] = "Windows Phone Project",
                [Guid.Parse("65b1d035-27a5-4bba-bab9-5f61c1e2bc4a")] = "Auto Load Nuget (manually set)",
                [Guid.Parse("4646b819-1ae0-4e79-97f4-8a8176fdd664")] = "UICONTEXT_OpenFolder",
                [Guid.Parse("349c5852-65df-11da-9384-00065b846f21")] = "ASPNETWapProject",
                [Guid.Parse("310d9a74-0a72-4b83-8c5b-4e75f035214c")] = "Powershell Repl Creation UI",
                [Guid.Parse("27f27371-b70c-4b7b-ba28-9edf8a3f0538")] = "SQL Object Explorer",
                [Guid.Parse("11b8e6d7-c08b-4385-b321-321078cdd1f8")] = "GIT SCC Provider",
                [Guid.Parse("03bdeac4-7186-458b-a2b0-941605d9917f")] = "UICONTEXT_ProjectCreating",
                [Guid.Parse("27f27371-b70c-4b7b-ba28-9edf8a3f0538")] = "SQL Object Explorer",
                [Guid.Parse("02e62848-c147-6f63-57b7-446a314e7f59")] = "Linux Application Type",
                [Guid.Parse("00d1a9c2-b5f0-4af3-8072-f6c62b433612")] = "SQL Project Service"
            };
        }

        public AutoLoadPackageEnumerator(SettingsStore configurationStore)
        {
            _configurationStore = configurationStore;
        }

        public IEnumerable<AutoLoadPackage> GetAutoLoadPackages()
        {
            foreach (string autoLoadContextGuidString in _configurationStore.GetSubCollectionNames(_autoLoadPackagesPath))
            {
                //Each path here is a context GUID
                Guid contextGuid = Guid.Empty;
                if (!Guid.TryParse(autoLoadContextGuidString, out contextGuid))
                {
                    continue;
                }

                string packagesPath = Path.Combine(_autoLoadPackagesPath, autoLoadContextGuidString);
                string contextName = GetUIContextName(autoLoadContextGuidString);
                bool isRuleBasedUIContext = IsRuleBasedUIContext(autoLoadContextGuidString);
                string uiContextExpression = string.Empty;

                if (isRuleBasedUIContext)
                {
                    string rulePath = Path.Combine(_uIContextRulesPath, autoLoadContextGuidString);
                    var expression = new StringBuilder();
                    foreach (string expressionName in _configurationStore.GetPropertyNames(rulePath))
                    {
                        if (expressionName != "" && expressionName != "Expression" && expressionName != "Delay")
                        {
                            expression.Append(_configurationStore.GetString(rulePath, expressionName) + "|");
                        }
                    }

                    uiContextExpression = expression.ToString();
                }

                foreach (string packageGuidString in _configurationStore.GetPropertyNames(packagesPath))
                {
                    Guid packageGuid = Guid.Empty;
                    if (Guid.TryParse(packageGuidString, out packageGuid))
                    {
                        yield return new AutoLoadPackage(packageGuid, contextGuid, contextName, isRuleBasedUIContext, uiContextExpression, _configurationStore);
                    }
                }
            }
        }

        public string GetUIContextName(string uiContextString)
        {
            string autoLoadUIContextPath = Path.Combine(_autoLoadPackagesPath, uiContextString);
            string rulePath = Path.Combine(_uIContextRulesPath, uiContextString);
            var uiContextGuid = Guid.Parse(uiContextString);
            string contextName = _uIContextNameMapping.ContainsKey(uiContextGuid) ? _uIContextNameMapping[uiContextGuid] : string.Empty;
            
            if (string.IsNullOrEmpty(contextName))
            {
                contextName = _configurationStore.GetString(autoLoadUIContextPath, string.Empty, string.Empty);
            }

            if (string.IsNullOrEmpty(contextName))
            {
                contextName = _configurationStore.GetString(rulePath, string.Empty, string.Empty);
            }

            if (string.IsNullOrEmpty(contextName))
            {
                contextName = _configurationStore.GetString(rulePath, "Expression", string.Empty);
            }

            return contextName;
        }

        private bool IsRuleBasedUIContext(string uiContextString)
        {
            return _configurationStore.CollectionExists(Path.Combine(_uIContextRulesPath, uiContextString));
        }
    }

    class AutoLoadPackage
    {
        const string _packagesPath = "Packages";
        const string _autoLoadPackagesPath = "AutoLoadPackages";

        public Guid PackageGuid { get; private set; }

        public string PackageName { get; private set; }
        
        public string ModuleName { get; private set; }

        public Guid AutoLoadContextGuid { get; private set; }

        public string AutoLoadContextName { get; private set; }

        public bool IsRuleBasedUIContext { get; private set; }
        
        public string UIContextTerms { get; private set; }

        public bool IsAsyncPackage { get; private set; }

        public bool IsAsyncForUIContext { get; private set; }

        public AutoLoadPackage(Guid packageGuid, Guid contextGuid, string contextName, bool isRuleBasedUIContext, string contextTerms, SettingsStore configurationStore)
        {
            PackageGuid = packageGuid;
            AutoLoadContextGuid = contextGuid;
            AutoLoadContextName = contextName;
            IsRuleBasedUIContext = isRuleBasedUIContext;
            UIContextTerms = contextTerms;

            string packageInfoPath = Path.Combine(_packagesPath, packageGuid.ToString("B"));
            string autoLoadConfigurationPath = Path.Combine(_autoLoadPackagesPath, contextGuid.ToString("B"));
            if (configurationStore.CollectionExists(packageInfoPath))
            {
                PackageName = configurationStore.GetString(packageInfoPath, string.Empty, "Unknown").Split(',')[0];
                string moduleName = configurationStore.GetString(packageInfoPath, "Class", null) ?? Path.GetFileName(configurationStore.GetString(packageInfoPath, "InProcServer32", string.Empty));
                ModuleName = moduleName.Split(',')[0];
                IsAsyncPackage = configurationStore.GetBoolean(packageInfoPath, "AllowsBackgroundLoad", false);
            }

            if (configurationStore.CollectionExists(autoLoadConfigurationPath))
            {
                uint autoLoadFlags = (uint)PackageAutoLoadFlags.None;
                try
                {
                    autoLoadFlags = configurationStore.GetUInt32(autoLoadConfigurationPath, packageGuid.ToString("B"), 0);
                }
                catch (Exception)
                {
                    //Do not do anyting. Use none as flag
                    // Apparently user feed package "bb4bf712-fcf7-4d17-83bb-93e6478b4e5d" specified a string in the pkgdef..
                }

                bool backgroundLoad = ((autoLoadFlags & (uint)PackageAutoLoadFlags.BackgroundLoad) == (uint)PackageAutoLoadFlags.BackgroundLoad);

                IsAsyncForUIContext = IsAsyncPackage && backgroundLoad;
            }
        }

    }
}
