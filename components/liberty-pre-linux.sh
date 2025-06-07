#!/bin/bash

DIR="$(cd "$(dirname "$0")" && pwd)"

# Check if Mono is installed
# Проверяем, установлен ли Mono
if ! command -v mono &> /dev/null
then
    echo "Mono is not installed. To install Mono, follow the instructions below:"
    echo "For Debian-based distributions (e.g., Ubuntu):"
    echo "  sudo apt update && sudo apt install mono-complete"
    echo "For RPM-based distributions (e.g., Fedora, CentOS, RHEL):"
    echo "  sudo dnf install mono-complete  # or 'sudo yum install mono-complete' for older systems"
    echo "For Arch Linux and Manjaro:"
    echo "  sudo pacman -S mono"
fi

# Check for active process nfqws
# Проверка на существование процесса nfqws
if pgrep -f "nfqws" >/dev/null; then
    echo "[Warning]: Process nfqws already launched. Exit."
    exit 1
fi

# Run the C# application using Mono
# Запустите приложение C# с помощью Mono
echo "[Info]: Start liberty-pre..."
mono "$DIR/liberty-pre.exe" $1 $2
exit_code=$?

# Checking if the launch was successful
# Проверка, успешен ли запуск
if [ $exit_code -ne 0 ]; then
    echo "[!] liberty-pre ended with an error $exit_code"
    exit $exit_code
fi

# Adding iptables rules
# Добавление iptables правил
echo "[Info]: Adding iptables rules..."
sudo iptables -I OUTPUT -p tcp --dport 80 -j NFQUEUE --queue-num 0
sudo iptables -I OUTPUT -p tcp --dport 443 -j NFQUEUE --queue-num 0
sudo iptables -I OUTPUT -p udp --dport 443 -j NFQUEUE --queue-num 0
sudo iptables -I OUTPUT -p udp --dport 50000:50100 -j NFQUEUE --queue-num 0
echo "[Info]: Ok!"

# --- Uncomment for nftables
# --- Раскомментируйте для nftables

# Create a table
# Создать таблицу
# sudo nft add table inet liberty_pre

# Create OUTPUT chain
# Создать цепочку OUTPUT
# sudo nft add chain inet liberty_pre output { type filter hook output priority 0 \; policy accept \; }

# Add rules
# Добавить правила
# sudo nft add rule inet liberty_pre output tcp dport 80 nfqueue num 0
# sudo nft add rule inet liberty_pre output tcp dport 443 nfqueue num 0
# sudo nft add rule inet liberty_pre output udp dport 443 nfqueue num 0
# sudo nft add rule inet liberty_pre output udp dport 50000-50100 nfqueue num 0

# Delete table:
# Удалить таблицу:
# sudo nft delete table inet myfilter
