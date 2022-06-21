#!/bin/bash

apt-get update & apt-get upgrade -y
apt-get install nginx nano curl -y
systemctl enable nginx
curl https://ssl-config.mozilla.org/ffdhe2048.txt > /var/lib/nginx/dhparam.pem
chmod +r /var/lib/nginx/dhparam.pem

mkdir -p /data/nginx/cache
mkdir -p /www/wwwlogs/
mkdir -p /var/www/cert/

cp cache.conf /etc/nginx/sites-enabled/boocat.org.conf

if [ -z $input ]; then
    read -r -p "Install API or Function edtion [A/F] " input
fi    

case $input in
    [aA])
        echo "You selected API"
        cp nginx-api.conf /etc/nginx/sites-enabled/api.boocat.org.conf
        ;;
    [fF])
        echo "You selected Function"
        cp nginx-func.conf /etc/nginx/sites-enabled/api.boocat.org.conf
        ;;

    *)
        echo "Invalid input..."
        exit 1
        ;;
esac

echo 'TLS Cert' > /var/www/cert/bc.pem
echo 'TLS Key' > /var/www/cert/bc.key
nano /var/www/cert/bc.pem
nano /var/www/cert/bc.key
nano /etc/nginx/sites-enabled/api.boocat.org.conf
nginx -t
systemctl reload nginx