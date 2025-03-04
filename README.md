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
К примеру создайте ярлык на liberty-pre.exe с аргументом `liberty-pre.exe -c discord.cfg`

### Зачем?
Удобно и почему нет.

### Как использовать?
Скачать последний [релиз](https://github.com/Mr-Precise/liberty-pre/releases/latest), распаковать, запустить.  
Если провайдер перехватывает или подменяет DNS запросы - используйте шифрованный DNS (DoT/DoH).

### Осторожность:
Исполняемый файл который находится в каталоге bin к примеру winws.exe взят из оригинального репозитория [github.com/bol-van/zapret](https://github.com/bol-van/zapret). Из других мест / авторов использовать небезопасно.

### Как скомпилировать?
Рекурсивно склонировать исходный код:  
`git clone --recursive https://github.com/Mr-Precise/liberty-pre`  
Использовать Visual Studio или Monodevelop/Dotdevelop  
Требования: .net framework (msbuild) или Mono (xbuild), если в Linux/macOS.

### Использованы компоненты из репозиториев и благодарности:
* [github.com/bol-van/zapret](https://github.com/bol-van/zapret) - компоненты
* [github.com/Flowseal/zapret-discord-youtube](https://github.com/Flowseal/zapret-discord-youtube) - идея и списки доменов / ip сетей
* [github.com/basil00/WinDivert](https://github.com/basil00/WinDivert) - за замечательный драйвер
