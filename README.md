### liberty-pre - zapret Launcher (DPI bypass)

Read in [English](README_EN.md) language

### Что это?
Реализация лаунчера для winws.exe (инструмента для обхода DPI) на языке C#, с поддержкой профилей.  
Поможет Вам обойти блокировки Youtube, Discord и т. д.  
В настоящее время находится в разработке.

### Как работает?
Делает проверки на всё необходимое, наличие существующего процесса, ОС.  
Читает указанный конфигурационный файл с аргументами запуска, затем запускает winws.exe с правами администратора в свёрнутом окне.  
По умолчанию читает конфигурацию (профиль) `default.cfg`  
Чтобы изменить ситуацию, нужно запускать liberty-pre с аргументом `-c config_name.cfg`  
К примеру, создайте ярлык на liberty-pre.exe с аргументом `liberty-pre.exe -c discord.cfg` чтобы использовать только для discord.

### Зачем?
Удобно и почему нет.

### Как использовать?
Скачать последний [релиз](https://github.com/Mr-Precise/liberty-pre/releases/latest), распаковать, запустить.  
Если провайдер перехватывает или подменяет DNS запросы - используйте шифрованный DNS (DoT/DoH).

### Осторожность:
Исполняемый файл который находится в каталоге bin к примеру winws.exe взят из оригинального репозитория [github.com/bol-van/zapret](https://github.com/bol-van/zapret). Из других мест / авторов использовать небезопасно.

### Как скомпилировать?
Установить git:  
Ubuntu/Debian: `sudo apt install git` / Windows: [git-scm.com/downloads/win](https://git-scm.com/downloads/win)  
Рекурсивно склонировать исходный код:  
`git clone --recursive https://github.com/Mr-Precise/liberty-pre`  
Использовать Visual Studio, Visual Studio Code + C# дополнение или Monodevelop/Dotdevelop для сборки  
Требования: .net framework (msbuild) или Mono (xbuild), если в Linux/macOS.  
Для успешной сборки на linux требуется ubuntu 20.04/22.04 LTS и mono nightly (6.13) версии.  
Можете изучить [.github/workflows/build.yml](.github/workflows/build.yml#L24)  
Пример:
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

### Использованы компоненты из репозиториев и благодарности:
* [github.com/bol-van/zapret](https://github.com/bol-van/zapret) - компоненты
* [github.com/Flowseal/zapret-discord-youtube](https://github.com/Flowseal/zapret-discord-youtube) - идея и списки доменов / ip сетей
* [github.com/basil00/WinDivert](https://github.com/basil00/WinDivert) - за замечательный драйвер
