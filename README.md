### liberty-pre - zapret Launcher (DPI bypass)

Read in [English](README_EN.md) language

### Что это?
Реализация лаунчера для winws.exe или nfqws (инструмента для обхода DPI) на языке C#, с поддержкой профилей.  
Поможет Вам обойти блокировки Youtube, Discord и т. д.  
В настоящее время находится в разработке.

### Как работает?
При запуске программа делает все необходимые для правильной работы проверки, наличие существующего процесса, язык интерфейса и ОС.  
Читает указанный конфигурационный файл (профиль) с аргументами запуска, затем запускает winws.exe с правами администратора в свёрнутом окне. 
Если мы на linux, ещё проверяем наличие sudo, запрашиваем пароль и запускается nfqws.  
По умолчанию читается конфигурация (профиль) `default.cfg`  
При певом запуске программа сама `автоматически` создаст нужные ярлыки с конфигурациями возле себя.  
Чтобы запустить с пользовательской конфигурацией, запускайте liberty-pre с аргументом `-c config_name.cfg`  
К примеру, создайте ярлык на liberty-pre.exe с аргументом `liberty-pre.exe -c discord.cfg` чтобы использовать только для discord.

### Локализация
Программа автоматически определяет язык ОС:  
- Если язык системы **русский**, то все сообщения выводятся на **русском языке**.  
- Для любого другого системного языка сообщения выводятся только на **английском языке**.

### Совместимость с ОС
#### Полная поддержка Windows:
- 8.1 - 11 64bit
- отдельная сборка под устаревшую Windows 7 SP1 64bit (build 7601) и [WMF 5.1 (PowerShell 5.1)](https://download.microsoft.com/download/6/F/5/6F5FF66C-6775-42B0-86C4-47D41F2DA187/Win7AndW2K8R2-KB3191566-x64.zip)

#### Экспериментальная поддержка Linux / Mono (в разработке):
- *ubuntu 20.04+ amd64
- Debian 11+ amd64
- актуальный Arch/Manjaro

### Зачем?
Удобно и почему нет.  
Просто альтернативная реализация.

### Как использовать?
Скачать последний [релиз](https://github.com/Mr-Precise/liberty-pre/releases/latest), распаковать, запустить, ярлыки на остальные профили создадутся автоматически.  
Есть невидимый / скрытый режим работы, создайте пустой файл без расширения с именем `hidden_mode` в папке с программой.  
Если провайдер перехватывает или подменяет DNS запросы - используйте шифрованный DNS (DoT/DoH).  
Ярлык с именем `liberty-pre STOP` останавливает процесс и завершает работу драйвера WinDivert.  
Ярлык с именем `liberty-pre SWITCH ipset` предназначен для переключения ipset файла. Агрумент командной строки `--ipset`  
У этого переключателя ipset есть два состояния: полный (с валидным списком сетей) и заглушка (IP из диапазона TEST-NET-3).  
Linux (для продвинутых): установить mono, запускать liberty-pre-linux.sh  
Linux: Возможно потребуется дополнительная настройка iptables/nftables.

### Осторожно:
Исполняемые файлы которые находятся в каталоге bin, к примеру winws.exe взяты из оригинального репозитория [github.com/bol-van/zapret](https://github.com/bol-van/zapret). Из других мест / авторов использовать небезопасно.  
Драйвер WinDivert не вирус а инструмент, читайте [подробности](https://github.com/bol-van/zapret-win-bundle?tab=readme-ov-file#антивирусы) почему антивирусы иногда ругаются и почему так происходит.

### Как скомпилировать?
Установить git:  
Ubuntu/Debian: `sudo apt install git` / Windows: [git-scm.com/downloads/win](https://git-scm.com/downloads/win)  
Рекурсивно склонировать исходный код:  
`git clone --recursive https://github.com/Mr-Precise/liberty-pre`  
Использовать Visual Studio, Visual Studio Code + C# дополнение или Monodevelop/Dotdevelop для сборки  
Требования: .net framework (msbuild) или Mono (xbuild), если в Linux/macOS.  
Для успешной сборки и использования на linux требуется ubuntu 20.04/22.04 LTS и mono nightly (6.13) версии.  
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
* [github.com/Flowseal/zapret-discord-youtube](https://github.com/Flowseal/zapret-discord-youtube) - идея и списки доменов / IP сетей
* [github.com/basil00/WinDivert](https://github.com/basil00/WinDivert) - за замечательный драйвер
