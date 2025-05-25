#!/bin/bash

# Check if Mono is installed
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

# Run the C# application using Mono
mono liberty-pre.exe
