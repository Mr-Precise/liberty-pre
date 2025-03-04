### liberty-pre - zapret Launcher (DPI bypass)

Читать на [Русском](README.md) языке
### What is this?
Implementation of a launcher for winws.exe in C#, with profile support.  
Currently in development.

### How does it work?
Checks for everything necessary, the presence of an existing process, OS.  
Reads the specified configuration file with startup arguments, then runs winws.exe with administrator rights in a minimized window.  
By default, reads the configuration (profile) `default.cfg`  
To change this behavior, you need to run liberty-pre with the argument `-c config_name.cfg`  
For example, create a shortcut to liberty-pre.exe with the argument `liberty-pre.exe -c discord.cfg`

### What for?
Convenient and why not.

### How to use?
Download the latest [release](https://github.com/Mr-Precise/liberty-pre/releases/latest), unpack and run.  
If your provider intercepts or replaces DNS requests, use encrypted DNS (DoT/DoH).

### Caution:
The executable file that is in the bin directory, for example winws.exe, is taken from the original repository [github.com/bol-van/zapret](https://github.com/bol-van/zapret). It is not safe to use from other places / authors.

### How to compile?
Use Visual Studio or Monodevelop/Dotdevelop  
Requirements: .net framework or Mono if on Linux/macOS.

### Used components from repositories and thanks:
* [github.com/bol-van/zapret](https://github.com/bol-van/zapret) - components
* [github.com/Flowseal/zapret-discord-youtube](https://github.com/Flowseal/zapret-discord-youtube) - idea and lists of domains / ip networks
* [github.com/basil00/WinDivert](https://github.com/basil00/WinDivert) - for the great driver
