sudo dnf update
sudo dnf install nodejs -y
sudo firewall-cmd --add-port=4321/tcp --permanent
sudo firewall-cmd --add-port=7777/tcp --permanent
sudo firewall-cmd --restart
