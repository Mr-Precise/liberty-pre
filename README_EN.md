### liberty-pre - zapret Launcher (DPI bypass)

Читать на [Русском](README.md) языке

### What is this?
Implementation of a launcher for winws.exe or nfqws (DPI bypass tool) in C#, with profile support.  
Helps you bypass blocking Youtube, Discord, etc.  
Currently in development.

### How does it work?
Checks for everything necessary, the presence of an existing process, OS.  
Reads the specified configuration file with startup arguments, then runs winws.exe with administrator rights in a minimized window.  
If we are on Linux, we also check for sudo, it asks for a password and nfqws starts.  
By default, reads the configuration (profile) `default.cfg`  
When you first launch the program, it `automatically` creates the necessary shortcuts with configurations in the same directory.  
To run with an alternative configuration, run liberty-pre with the argument `-c config_name.cfg`  
For example, create a shortcut to liberty-pre.exe with the argument `liberty-pre.exe -c discord.cfg` to use only for Discord.

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

### How to use?
Download the latest [release](https://github.com/Mr-Precise/liberty-pre/releases/latest), unpack and run.  
If your provider intercepts or replaces DNS requests, use encrypted DNS (DoT/DoH).  
A shortcut named `liberty-pre stop` stops the process and terminates the WinDivert driver.  
Linux (for advanced users): install mono, run liberty-pre-linux.sh  
Linux: Additional iptables/nftables configuration may be required.

### Caution:
The executable file that is in the bin directory, for example winws.exe, is taken from the original repository [github.com/bol-van/zapret](https://github.com/bol-van/zapret). It is not safe to use from other places / authors.

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
* [github.com/Flowseal/zapret-discord-youtube](https://github.com/Flowseal/zapret-discord-youtube) - idea and lists of domains / ip networks
* [github.com/basil00/WinDivert](https://github.com/basil00/WinDivert) - for the great driver
