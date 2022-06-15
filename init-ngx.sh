#!/bin/bash

apt-get update & apt-get upgrade -y
apt install nginx nano -y
systemctl enable nginx
curl https://ssl-config.mozilla.org/ffdhe2048.txt > /var/lib/nginx/dhparam.pem
chmod +r /var/lib/nginx/dhparam.pem
mkdir -p /data/nginx/cache
mkdir -p /www/wwwlogs/
mkdir -p /var/www/cert/
mv nginx.conf /etc/nginx/sites-enabled/boocat.org.conf

echo 'TLS Cert' > /var/www/cert/bc.pem
echo 'TLS Key' > /var/www/cert/bc.key
nano /var/www/cert/bc.pem
nano /var/www/cert/bc.key
nano /etc/nginx/sites-enabled/boocat.org.conf
nginx -t
systemctl reload nginx