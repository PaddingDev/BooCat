#!/bin/bash

sudo systemctl stop boocat.service
mkdir -p /var/boocat/bin
cd /var/boocat
rm -fr *
git clone https://github.com/KevinZonda/BooCat src
cd src
dotnet publish KevinZonda.BooCat.AspNetWebApi -c Release -o /var/boocat/bin/
sudo cp boocat.service /etc/systemd/system/
sudo systemctl enable boocat.service
sudo systemctl start boocat.service