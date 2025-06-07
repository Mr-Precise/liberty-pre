#!/bin/bash

echo "[Info]: Stop nfqws and removing iptables rules..."

sudo kill -TERM $(pidof nfqws)

sudo iptables -D OUTPUT -p udp --dport 443 -j NFQUEUE --queue-num 0
sudo iptables -D OUTPUT -p udp --dport 50000:50099 -j NFQUEUE --queue-num 0
sudo iptables -D OUTPUT -p tcp --dport 80 -j NFQUEUE --queue-num 0
sudo iptables -D OUTPUT -p tcp --dport 443 -j NFQUEUE --queue-num 0

echo "[Info]: Ok!"
