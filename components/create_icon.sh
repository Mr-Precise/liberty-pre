#!/bin/bash

# This script creates the .ico files for the resource file from the .png files.
# It requires ImageMagick to be installed.
# Этот скрипт создает .ico файлы для ресурсного файла из .png файлов.
# Для его работы требуется установленный ImageMagick.

# Optimize the .png files using optipng if necessary.
# Оптимизируем .png файлы с помощью optipng, если это необходимо.
# sudo apt install optipng
#optipng -o7 *.png

# Create the .ico files from the .png files using ImageMagick's convert command.
# Создаем .ico файлы из .png файлов с помощью команды convert из ImageMagick.
# sudo apt install imagemagick
convert liberty-pre-64x64.png liberty-pre-128x128.png liberty-pre-256x256.png liberty-pre.ico
convert liberty-pre-green-64x64.png liberty-pre-green-128x128.png liberty-pre-green-256x256.png liberty-pre-green.ico
convert liberty-pre-red-64x64.png liberty-pre-red-128x128.png liberty-pre-red-256x256.png liberty-pre-red.ico
