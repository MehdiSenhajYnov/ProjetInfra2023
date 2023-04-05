sudo dnf update 
sudo dnf install epel-release -y
sudo mv mongodb-org-6.0.repo /etc/yum.repos.d/
sudo yum install -y mongodb-org
sudo firewall-cmd --add-port=27017/tcp --permanent
sudo firewall-cmd --restart
sudo systemctl enable --now mongod
sudo systemctl daemon-reload
sudo systemctl start mongod