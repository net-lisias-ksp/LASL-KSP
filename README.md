# L Aerospace Shared Libraries / KSP Division

**LASL-KSP** is a bag of tricks and stunts used (or not!) by Lisias' Add'Ons.


## In a Hurry

* [Latest Release](https://github.com/net-lisias-ksp/LASL-KSP/releases)
    + [Binaries](https://github.com/net-lisias-ksp/LASL-KSP/tree/Archive)
* [Source](https://github.com/net-lisias-ksp/LASL-KSP)
* Documentation
    + [Project's README](https://github.com/net-lisias-ksp/LASL-KSP/blob/master/README.md)
    + [Install Instructions](https://github.com/net-lisias-ksp/LASL-KSP/blob/master/INSTALL.md)
    + [Change Log](./CHANGE_LOG.md)


## Description

**LASL-KSP** is a (crescent) bag of tricks and stunts used (or not!) by Lisias' Add'Ons.

The objective is to remove from the main distribution (common) code that are subject to be revisited (as supporting 3rd party add'ons) or to share common pieces of code that can be useful elsewhere (as the Morse Code!), and shove them on a single easily maintainable and deployable package.

Currently available:

### Morse Code

A class for "compiling" morse code into signals. Currently supports input in Morse Code itself, or in human readable form.

Decoding Morse into human is not implemented yet, but it's on the works.

Decoding signals into Morse Code is not planned, but it's under consideration.

Based on the specification found [MorseCode.World](https://morsecode.world/international/morse.html).

### `SwitchLights`

Loosely inspired by the source code from [CrewLight](https://github.com/Li0n-0/CrewLight), it's an extensible and configurable framework to handle Lights in KSP. No more Reflection, calls are native and fast - and the code was updated for restore functionality for the supported 3rd parties add'ons.

Lights are segmented into Beacons, Strobes and Navigation for easy specialised support. Additionally, you can detect "slow response lights" by querying the response time from on to off and back, so you can ignore devices that are not going to respond under your criteria.

Fully supports:

* Stock
* [Aviation Lights](https://forum.kerbalspaceprogram.com/index.php?/topic/211781-*/).
* [Kerbal Electric](https://forum.kerbalspaceprogram.com/index.php?/topic/165449-*/)
* [WildBlue Tools](https://forum.kerbalspaceprogram.com/index.php?/topic/163889-wild-blue-industries-crafts/)
* And more to come, as they became available and needed.


## Installation

Detailed installation instructions are now on its own file (see the [In a Hurry](#in-a-hurry) section) and on the distribution file.

### Licensing

* **LASL-KSPs** is double licensed as follows:
	+ [SKL 1.0](https://ksp.lisias.net/SKL-1_0.txt). See [here](./LICENSE.KSPe.SKL-1_0)
		+ You are free to:
			- Use : unpack and use the material in any computer or device
			- Redistribute : redistribute the original package in any medium
		+ Under the following terms:
			- You agree to use the material only on (or to) KSP
			- You don't alter the package in any form or way (but you can embedded it)
			- You don't change the material in any way, and retain any copyright notices
			- You must explicitly state the author's Copyright, as well an Official Site for downloading the original and new versions (the one you used to download is good enough)
	+ [GPL 2.0](https://www.gnu.org/licenses/gpl-2.0.txt). See [here](./LICENSE.KSPe.GPL-2_0)
		+ You are free to:
			- Use : unpack and use the material in any computer or device
			- Redistribute : redistribute the original package in any medium
			- Adapt : Reuse, modify or incorporate source code into your works (and redistribute it!) 
		+ Under the following terms:
			- You retain any copyright notices
			- You recognise and respect any trademarks
			- You don't impersonate the authors, neither redistribute a derivative that could be misrepresented as theirs.
			- You credit the author and republish the copyright notices on your works where the code is used.
			- You relicense (and fully comply) your works using GPL 2.0
				- Please note that upgrading the license to GPLv3 **IS NOT ALLOWED** for this work, as the author **DID NOT** added the "or (at your option) any later version" on the license.
			- You don't mix your work with GPL incompatible works.
	* If by some reason the GPL would be invalid for you, rest assured that you still retain the right to Use the Work under SKL 1.0. 

Please note the copyrights and trademarks in [NOTICE](./NOTICE).


## Upstream

There's no upstream, **I am (g)ROOT** :)
