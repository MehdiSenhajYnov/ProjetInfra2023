##ONLINE GAME PROJECT

###DATABASE

- Upload the content of the `/utils/db` directory on your dedicated server
- Execute `db-server_init.sh` as sudoer
- You have access to the db with the command `mongosh`

###WEB SERVER

- Upload the content of `/utils/web` on your dedicated server
- Execute `web-server_init.sh` as sudoer
- Setup the connection to your database with the command `ssh-copy-id [db-server_user]@[db-server_IP]`
- Connect via ssh tunnel to the database with the command `ssh -L 4321:localhost:27017 [db-server_user]@[db-server_IP] -f -N`
- Extract the `web_server` archive and upload its content on your server
- From the `web_server` directory, you can launch the web server with the command `node server.js`

##WAF with ModSecurity-Nginx Module:

- Make the script runnable with `chmod +x install.sh`
- Execute `./install.sh` as **sudoer**
- Check the installation with `/usr/local/nginx/sbin/nginx -t`

#Nginx configuration:

- Go to configuration file `/usr/local/nginx/conf/nginx.conf`
- Replace the variable `<NOM_DU_SERVEUR_WEB>` with Web server hostname/IP adress
- Replace the variable `<IP_DE_InterfaceWeb>` with the proxy target IP adress
- reload Nginx with `sudo systemctl restart nginx`

#ModSecurity Test:

- You can test if ModSecurity is working by using the following command:
  `curl -d "id=1 AND 1=1" http://yourserver.com/index.php`

If you get an 403 Forbidden response, ModSecurity is working !
