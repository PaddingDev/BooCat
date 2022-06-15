#!/bin/bash

sudo apt-get update & apt-get upgrade -y
sudo apt install nginx nano -y
sudo systemctl enable nginx
sudo curl https://ssl-config.mozilla.org/ffdhe2048.txt > /var/lib/nginx/dhparam.pem
sudo chmod +w /var/lib/nginx/dhparam.pem
sudo mkdir -p /data/nginx/cache
sudo mkdir -p /www/wwwlogs/
sudo mkdir -p /var/www/cert/
sudo mv nginx.conf /etc/nginx/sites-enabled/boocat.org.conf

sudo echo 'TLS Cert' > /var/www/cert/bc.pem
sudo echo 'TLS Key' > /var/www/cert/bc.Key
sudo nano /var/www/cert/bc.pem
sudo nano /var/www/cert/bc.Key
sudo nano /etc/nginx/sites-enabled/boocat.org.conf
sudo nginx -t
sudo systemctl reload nginx