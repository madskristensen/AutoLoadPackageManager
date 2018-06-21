# VS Package Load Explorer

[![Build status](https://ci.appveyor.com/api/projects/status/0bqwktptj7ux1imq?svg=true)](https://ci.appveyor.com/project/madskristensen/autoloadpackagemanager)

**Requires Visual Studio 2017.6 or newer**

This extension makes it easy to diagnose how packages are being loaded in Visual Studio. Both build-in as well as packages comming from external extensions are supported.

## The Package Load Explorer window
Display it by going to **View -> Other Windows -> Package Load Explroer**.

![Context menu](art/context-menu.png)

Shows information about how the VS packages was loaded in the IDE.

![Tool window](art/tool-window.png)

The columns shown are:

* PackageGuid
* PackageName
* ModuleName
* AutoLoadContextGuid
* AutoLoadContextName
* IsRuleBasedUIContext
* UIContextTerms
* IsAsyncPackage
* IsAsyncForUIContext

## License
[Apache 2.0](LICENSE) 