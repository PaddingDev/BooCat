#!/bin/bash

echo "Fetching new version..."
cd /usr/local/boocat/src
git fetch --all
git reset --hard origin/master
git pull

echo "Stopping current services..."
sudo systemctl stop boocat.service

echo "Building new version..."
dotnet publish KevinZonda.BooCat.AspNetWebApi -c Release -o /usr/local/boocat/bin-new/

echo "Creating bin backup..."
sudo mv /usr/local/boocat/bin /usr/local/boocat/bin-bak

echo "Moving new version..."
sudo mv /usr/local/boocat/bin-new /usr/local/boocat/bin


if [ $? -ne 0 ]; then
    echo "Failed to publish, falling back..."
    sudo rm -fr /usr/local/boocat/bin
    sudo mv /usr/local/boocat/bin-bak /usr/local/boocat/bin
    sudo systemctl start boocat.service
    echo "Upgrade failed."
    exit 1
fi

echo "Creating service backup..."
sudo mv /etc/systemd/system/boocat.service /etc/systemd/system/boocat.service.bak

echo "Updating service file..."
sudo cp ./script/boocat.service /etc/systemd/system/
sudo systemctl daemon-reload

echo "Try to start service..."
sudo systemctl start boocat.service

if [ $? -ne 0 ]; then
    echo "Failed to start with new service, falling back service..."
    sudo mv /etc/systemd/system/boocat.service.bak /etc/systemd/system/boocat.service
    sudo systemctl daemon-reload

    echo "Try to start new bin with old service..."
    sudo systemctl start boocat.service
    if [ $? -ne 0 ]; then
        echo "Failed to start with old service, falling back bin..."
        sudo mv /usr/local/boocat/bin-bak /usr/local/boocat/bin
        echo "Upgrade failed."
        exit 1
    fi
fi

echo "Cleaning..."
sudo rm -fr /usr/local/boocat/bin-bak

echo "Upgrade complete."