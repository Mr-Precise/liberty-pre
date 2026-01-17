#!/bin/bash

BASE_DIR="$(cd "$(dirname "$0")" && pwd)"
LAUNCHER_EXE="liberty-pre.exe"
SCRIPT_BASENAME="$(basename "$0")"
SCRIPT_FULL_PATH="$BASE_DIR/$SCRIPT_BASENAME"
ICON_FILE="$BASE_DIR/liberty-pre.png"
DESKTOP_FILE="$HOME/.local/share/applications/liberty-pre.desktop"

# Detect system language for localization
# Определение языка системы для локализации
if [ -n "$LANG" ] && [[ "$LANG" == *"ru"* ]]; then
    LANG_RU=true
else
    LANG_RU=false
fi

# Localization function
# Функция локализации
localize_msg() {
    local key="$1"
    shift
    
    if [ "$LANG_RU" = true ]; then
        case "$key" in
            # Error messages
            # Сообщения об ошибках
            "error_mono_not_found")
                echo "Ошибка: Mono не установлен"
                ;;
            "error_dependency_missing")
                echo "Ошибка: Отсутствуют зависимости: $*"
                ;;
            "error_file_not_found")
                echo "Ошибка: Файл $LAUNCHER_EXE не найден в $BASE_DIR"
                ;;
            "error_start_failed")
                echo "Ошибка: Не удалось запустить $LAUNCHER_EXE"
                ;;
            "error_icon_not_found")
                echo "Ошибка: Файл иконки liberty-pre.png не найден"
                ;;
            
            # Warning messages
            # Предупреждения
            "warn_desktop_file_exists")
                echo "Предупреждение: Файл .desktop уже существует, будет перезаписан"
                ;;
            
            # Info messages
            # Информационные сообщения
            "info_starting_app")
                echo "Запуск $LAUNCHER_EXE с аргументами: $*"
                ;;
            "info_creating_desktop")
                echo "Создание файла .desktop..."
                ;;
            "info_desktop_created")
                echo "Файл .desktop создан в: $DESKTOP_FILE"
                ;;
            "info_desktop_install_hint")
                echo "Подсказка: Приложение появится в меню после перезагрузки сеанса (должно сразу)"
                ;;
            
            # Success messages
            # Сообщения об успехе
            "success_started")
                echo "Все успешно запущено"
                ;;
            "success_desktop_installed")
                echo "Ярлык приложения успешно установлен"
                ;;
            
            # Default - return key as is
            # По умолчанию - возвращаем ключ как есть
            *)
                echo "$key"
                ;;
        esac
    else
        case "$key" in
            # Error messages
            # Сообщения об ошибках
            "error_mono_not_found")
                echo "Error: Mono is not installed"
                ;;
            "error_dependency_missing")
                echo "Error: Missing dependencies: $*"
                ;;
            "error_file_not_found")
                echo "Error: File $LAUNCHER_EXE not found in $BASE_DIR"
                ;;
            "error_start_failed")
                echo "Error: Failed to start $LAUNCHER_EXE"
                ;;
            "error_icon_not_found")
                echo "Error: Icon file liberty-pre.png not found"
                ;;
            
            # Warning messages
            # Предупреждающие сообщения
            "warn_desktop_file_exists")
                echo "Warning: .desktop file already exists, will be overwritten"
                ;;
            
            # Info messages
            # Информационные сообщения
            "info_starting_app")
                echo "Starting $LAUNCHER_EXE with arguments: $*"
                ;;
            "info_creating_desktop")
                echo "Creating .desktop file..."
                ;;
            "info_desktop_created")
                echo ".desktop file created at: $DESKTOP_FILE"
                ;;
            "info_desktop_install_hint")
                echo "Hint: Application will appear in menu after session restart (should be immediate)"
                ;;
            
            # Success messages
            # Сообщений об успехе
            "success_started")
                echo "Everything started successfully"
                ;;
            "success_desktop_installed")
                echo "Application shortcut successfully installed"
                ;;
            
            # Default - return key as is
            # По умолчанию - возвращаем ключ как есть
            *)
                echo "$key"
                ;;
        esac
    fi
}

# Print colored messages
# Вывод цветных сообщений
print_status() {
    local color="$1"
    local msg_type="$2"
    local message="$3"
    local reset='\033[0m'
    
    echo -e "${color}[${msg_type}]${reset} ${message}"
}

print_info()    { print_status '\033[0;36m' "Info" "$1"; }
print_warning() { print_status '\033[1;33m' "Warning" "$1"; }
print_error()   { print_status '\033[0;31m' "Error" "$1"; }
print_success() { print_status '\033[0;32m' "Success" "$1"; }

# Check for required dependencies
# Проверка необходимых зависимостей
check_requirements() {
    local missing_tools=()
    
    # Check for Mono
    # Проверка Mono
    if ! command -v mono &> /dev/null; then
        print_error "$(localize_msg "error_mono_not_found")"
        echo ""
        if [ "$LANG_RU" = true ]; then
            echo "Для установки Mono следуйте инструкциям:"
            echo ""
            echo "Debian/Ubuntu:"
            echo "  sudo apt update && sudo apt install mono-complete"
            echo ""
            echo "Fedora/RHEL/CentOS:"
            echo "  sudo dnf install mono-complete"
            echo ""
            echo "Arch/Manjaro Linux:"
            echo "  sudo pacman -S mono"
        else
            echo "To install Mono, follow these instructions:"
            echo ""
            echo "For Debian/Ubuntu:"
            echo "  sudo apt update && sudo apt install mono-complete"
            echo ""
            echo "For Fedora/RHEL/CentOS:"
            echo "  sudo dnf install mono-complete"
            echo ""
            echo "For Arch/Manjaro Linux:"
            echo "  sudo pacman -S mono"
        fi
        echo ""
        return 1
    fi
    
    # Check for nftables
    # Проверка nftables
    local required_tools=("nft" "pgrep" "pkill")
    for tool in "${required_tools[@]}"; do
        if ! command -v "$tool" &> /dev/null; then
            missing_tools+=("$tool")
        fi
    done
    
    if [ ${#missing_tools[@]} -gt 0 ]; then
        print_error "$(localize_msg "error_dependency_missing" "${missing_tools[*]}")"
        echo ""
        if [ "$LANG_RU" = true ]; then
            echo "Установите недостающие инструменты:"
            echo ""
            echo "Debian/Ubuntu: sudo apt install nftables procps"
            echo "Fedora/RHEL: sudo dnf install nftables procps-ng"
            echo "Arch/Manjaro: sudo pacman -S nftables procps-ng"
        else
            echo "Install missing tools with:"
            echo ""
            echo "Debian/Ubuntu: sudo apt install nftables procps"
            echo "Fedora/RHEL: sudo dnf install nftables procps-ng"
            echo "Arch/Manjaro: sudo pacman -S nftables procps-ng"
        fi
        echo ""
        return 1
    fi
    
    return 0
}

# Install .desktop file for application menu
# Установка .desktop файла для меню приложений
install_desktop_file() {
    print_info "$(localize_msg "info_creating_desktop")"
    
    # Check if icon file exists
    # Проверка наличия файла иконки
    if [ ! -f "$ICON_FILE" ]; then
        print_warning "$(localize_msg "error_icon_not_found")"
        ICON_PATH=""
    else
        ICON_PATH="$ICON_FILE"
    fi
    
    # Create applications directory if it doesn't exist (??)
    # Создание директории applications если она не существует (??)
    mkdir -p "$HOME/.local/share/applications"
    
    # Check if desktop file already exists
    # Проверка существования .desktop файла
    if [ -f "$DESKTOP_FILE" ]; then
        print_warning "$(localize_msg "warn_desktop_file_exists")"
    fi
    
    # Create .desktop file
    # Создание .desktop файла
    cat > "$DESKTOP_FILE" << EOF
[Desktop Entry]
Type=Application
Name=liberty-pre
Comment=liberty-pre zapret launcher (DPI bypass) with nftables
Exec=$SCRIPT_FULL_PATH
Icon=$ICON_PATH
Terminal=true
Categories=Network;Utility;
StartupNotify=true
Keywords=dpi;bypass;zapret;liberty-pre;
EOF
    
    # Make .desktop file executable
    # Делаем .desktop файл исполняемым
    chmod +x "$DESKTOP_FILE"
    
    print_info "$(localize_msg "info_desktop_created")"
    print_success "$(localize_msg "success_desktop_installed")"
    print_info "$(localize_msg "info_desktop_install_hint")"
    
    # Just in case: update desktop database
    # На всякий случай: обновить базу данных desktop
    if command -v update-desktop-database &> /dev/null; then
        update-desktop-database "$HOME/.local/share/applications" 2>/dev/null && \
            print_info "Desktop database updated"
    fi
    
    exit 0
}

# Display help information for the script
# Отображение справки для скрипта
show_usage() {
    if [ "$LANG_RU" = true ]; then
        echo "Использование: $SCRIPT_BASENAME [опции]"
        echo ""
        echo "Опции:"
        echo "  -h, --help          Показать эту справку"
        echo "  --install           Установить ярлык в меню приложений"
        echo "  Все остальные аргументы передаются в $LAUNCHER_EXE"
        echo ""
        echo "Примеры:"
        echo "  $SCRIPT_BASENAME -c default.cfg"
        echo "  $SCRIPT_BASENAME -c discord.cfg"
        echo "  $SCRIPT_BASENAME --install"
    else
        echo "Usage: $SCRIPT_BASENAME [options]"
        echo ""
        echo "Options:"
        echo "  -h, --help          Show this help message"
        echo "  --install           Install application menu shortcut"
        echo "  Any other arguments are passed to $LAUNCHER_EXE"
        echo ""
        echo "Examples:"
        echo "  $SCRIPT_BASENAME -c default.cfg"
        echo "  $SCRIPT_BASENAME -c discord.cfg"
        echo "  $SCRIPT_BASENAME --extended-ports"
        echo "  $SCRIPT_BASENAME --install"
    fi
    exit 0
}

# Main function
# Основная функция
execute_launcher() {
    local app_arguments=()
    
    # Parse command line arguments
    # Разбор аргументов командной строки
    while [[ $# -gt 0 ]]; do
        case "$1" in
            --help|-h)
                show_usage
                ;;
            --install)
                install_desktop_file
                ;;
            *)
                app_arguments+=("$1")
                shift
                ;;
        esac
    done
    
    # Verify requirements
    # Проверка требований
    if ! check_requirements; then
        exit 1
    fi
    
    # Verify executable exists
    # Проверка существования исполняемого файла
    if [ ! -f "$BASE_DIR/$LAUNCHER_EXE" ]; then
        print_error "$(localize_msg "error_file_not_found")"
        exit 1
    fi

    # Launch the application
    # Запуск приложения
    print_info "$(localize_msg "info_starting_app" "${app_arguments[*]}")"
    if ! mono "$BASE_DIR/$LAUNCHER_EXE" "${app_arguments[@]}"; then
        print_error "$(localize_msg "error_start_failed")"
        exit 1
    fi

    # Wait for application initialization
    # Ожидание инициализации приложения
    sleep 2

    print_success "$(localize_msg "success_started")"
    exit 0
}

# Start the launcher
# Запуск лаунчера
execute_launcher "$@"
