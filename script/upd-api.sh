#!/bin/bash

echo "Fetching new version..."
cd /var/boocat/src
git fetch --all
git reset --hard origin/master
git pull

echo "Stopping current services..."
sudo systemctl stop boocat.service

echo "Building new version..."
dotnet publish KevinZonda.BooCat.AspNetWebApi -c Release -o /var/boocat/bin-new/

echo "Creating bin backup..."
sudo mv /var/boocat/bin /var/boocat/bin-bak

echo "Moving new version..."
sudo mv /var/boocat/bin-new /var/boocat/bin


if [ $? -ne 0 ]; then
    echo "Failed to publish, falling back..."
    sudo rm -fr /var/boocat/bin
    sudo mv /var/boocat/bin-bak /var/boocat/bin
    sudo systemctl start boocat.service
    echo "Upgrade failed."
    exit 1
fi

echo "Creating service backup..."
sudo mv /etc/systemd/system/boocat.service /etc/systemd/system/boocat.service.bak

echo "Updating service file..."
sudo cp boocat.service /etc/systemd/system/

echo "Try to start service..."
sudo systemctl start boocat.service

if [ $? -ne 0 ]; then
    echo "Failed to start with new service, falling back service..."
    sudo mv /etc/systemd/system/boocat.service.bak /etc/systemd/system/boocat.service
    echo "Try to start new bin with old service..."
    sudo systemctl start boocat.service
    if [ $? -ne 0 ]; then
        echo "Failed to start with old service, falling back bin..."
        sudo mv /var/boocat/bin-bak /var/boocat/bin
        echo "Upgrade failed."
        exit 1
    fi
fi

echo "Cleaning..."
sudo rm -fr /var/boocat/bin-bak

echo "Upgrade complete."