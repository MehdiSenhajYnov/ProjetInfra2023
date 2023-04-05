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
