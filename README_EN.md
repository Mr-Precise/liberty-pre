### liberty-pre - zapret Launcher (DPI bypass)

Читать на [Русском](README.md) языке

### What is this?
Implementation of a launcher for winws.exe or nfqws (DPI bypass tool) in C#, with profile support.  
Helps you bypass blocking Youtube, Discord, etc.  
Currently in development.

### How does it work?
Checks for everything necessary, the presence of an existing process, the interface language and OS.  
Reads the specified configuration file with startup arguments, then runs winws.exe with administrator rights in a minimized window.  
If we are on Linux, we also check for sudo, it asks for a password and nfqws starts.  
By default, reads the configuration (profile) `default.cfg`  
When you first launch the program, it `automatically` creates the necessary shortcuts with configurations in the same directory.  
To run with an custom configuration, run liberty-pre with the argument `-c config_name.cfg`  
For example, create a shortcut to liberty-pre.exe with the argument `liberty-pre.exe -c discord.cfg` to use only for Discord.

### Localization
The program automatically detects OS language:  
- If the system language is **Russian**, messages are shown in **Russian**.  
- For any other system language messages are shown in **English** only.

### OS compatibility
#### Full Windows support:
- 8.1 - 11 64bit
- separate build for outdated Windows 7 SP1 64bit (build 7601) and [WMF 5.1 (PowerShell 5.1)](https://download.microsoft.com/download/6/F/5/6F5FF66C-6775-42B0-86C4-47D41F2DA187/Win7AndW2K8R2-KB3191566-x64.zip)

#### Experimental Linux / Mono support (in development):
- *ubuntu 20.04+ amd64
- Debian 11+ amd64
- actual Arch/Manjaro

### What for?
Convenient and why not.  
Just an alternative implementation.

### How to use?
Download the latest [release](https://github.com/Mr-Precise/liberty-pre/releases/latest), unpack and run, shortcuts to other profiles will be created automatically.  
If your provider intercepts or replaces DNS requests, use encrypted DNS (DoT/DoH).  
A shortcut named `liberty-pre - STOP` stops the process and terminates the WinDivert driver.  
The shortcut named `liberty-pre - SWITCH ipset mode` is used to switch the ipset file. Command-line argument `--ipset`  
This ipset switch has two states: full (with a valid network list) and stub (IPs from the TEST-NET-3 range).  
The shortcut named `liberty-pre SWITCH - Extended Ports` is used to switch the extended port filtering mode (1024-65535).  
The shortcut named `liberty-pre SWITCH - hidden modes` used to switch to the invisible/hidden mode of operation.  
Linux (for advanced users): install mono, run liberty-pre-linux.sh  
Linux: run `liberty-pre-linux.sh` command line option `--install` to install a .desktop file for application menu integration.  
Linux: Additional iptables/nftables configuration may be required.

### Caution:
The executable files that is in the bin directory, for example winws.exe, is taken from the original repository [github.com/bol-van/zapret](https://github.com/bol-van/zapret). It is not safe to use from other places / authors.  
WinDivert driver is not a virus but a tool, read the [details](https://github.com/bol-van/zapret-win-bundle?tab=readme-ov-file#antivirus-warning) of why this happens.

### How to compile?
Install git:  
Ubuntu / Debian: `sudo apt install git` / Windows: [git-scm.com/downloads/win](https://git-scm.com/downloads/win)  
Recursively clone the source code:  
`git clone --recursive https://github.com/Mr-Precise/liberty-pre`  
Use Visual Studio, Visual Studio Code + C# extension or Monodevelop/Dotdevelop for build  
Requirements: .net framework (msbuild) or Mono (xbuild) if on Linux/macOS.  
For successful build and use on Linux, Ubuntu 20.04/22.04 LTS and mono nightly (6.13) versions are required.  
You can look into [.github/workflows/build.yml](.github/workflows/build.yml#L24)  
Example:
```sh
sudo gpg --homedir /tmp --no-default-keyring --keyring /usr/share/keyrings/mono-official-archive-keyring.gpg --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF
echo "deb [arch=amd64 signed-by=/usr/share/keyrings/mono-official-archive-keyring.gpg] https://download.mono-project.com/repo/ubuntu nightly-focal main" | sudo tee /etc/apt/sources.list.d/mono-official-nightly.list
echo "deb [arch=amd64 signed-by=/usr/share/keyrings/mono-official-archive-keyring.gpg] https://download.mono-project.com/repo/ubuntu preview-focal main" | sudo tee /etc/apt/sources.list.d/mono-official-preview.list
sudo apt install mono-complete nuget git
git clone --recursive https://github.com/Mr-Precise/liberty-pre
cd liberty-pre
nuget restore
xbuild /p:Configuration=Release liberty-pre.sln
```

### Used components from repositories and thanks:
* [github.com/bol-van/zapret](https://github.com/bol-van/zapret) - components
* [github.com/Flowseal/zapret-discord-youtube](https://github.com/Flowseal/zapret-discord-youtube) - idea and lists of domains / IP networks
* [github.com/basil00/WinDivert](https://github.com/basil00/WinDivert) - for the great driver
