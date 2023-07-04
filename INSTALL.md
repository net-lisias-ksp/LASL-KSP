# L Aerospace Shared Libraries / KSP Division

**LASL-KSP** is a bag of tricks and stunts used (or not!) by Lisias' Add'Ons.

## Installation Instructions

To install, place the GameData folder inside your Kerbal Space Program folder:

* **REMOVE ANY OLD VERSIONS OF THE PRODUCT BEFORE INSTALLING**, including any other fork:
	+ Delete `<KSP_ROOT>/GameData/999_LST-KSP`
* Extract the package's `GameData/999_LST-KSP` folder into your KSP's as follows:
	+ `<PACKAGE>/GameData/999_LST-KSP` --> `<KSP_ROOT>/GameData`

The following file layout must be present after installation:

```
<KSP_ROOT>
	[GameData]
		[000_KSPe]
			...
		[999_LASL-KSP]
			[PluginData]
				...
			[Plugins]
				...
			CHANGE_LOG.md
			LASL.KSP.version
			LICENSE
			NOTICE
			README.md
		000_KSPe.dll
		ModuleManager.dll
		...
	KSP.log
	PartDatabase.cfg
	...
```

### Dependencies

* [KSPe](https://github.com/net-lisias-ksp/KSPAPIExtensions/releases/) (for KSP >= 1.2.2 - yeah, anything goes)
	+ Not Included
