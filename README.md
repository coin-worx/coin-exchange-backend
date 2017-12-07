# Coin Exchange Backend - README

## Technologies required:
1. Visual Studio 2012
2. .NET Framework 4.5
3. Databases: 
- MySql (preferably the latest version, e.g. 5.6), 
- RavenDB
4. IIS

## Code Base
1. Download and install Git.
2. Right the folder which you want to be the home for the codebase, and choose Git Bash.
3. Paste git clone git@bitbucket.org:abaziz/coin-exchange-backend.git
4. Give the password when prompted.

## Install IIS
- Follow this guide: http://www.howtogeek.com/112455/how-to-install-iis-8-on-windows-8/

## Project Structure
- Working code directory is coin-exchange-backend\src.

## Setting up RavenDB
1. Download the RavenDB from here: http://hibernatingrhinos.com/downloads/RavenDB%20Installer/2879
2. Run the Setup installation.
3. While installing the RavenDB, please select "Development" in Tragetting environment.
4. Install the RavenDB as a service and chose enter port 8081.
5. After the installtion completes, open "http://localhost:8081/raven/studio.html" link in the browser.
(install silver light if prompted) you might have to use IE for opening the ravendb studio
6. In the main screen,click on "New Database", type "InputEventStore" in Name Field and click next.
7. In the main screen,click on "New Database", type "OutputEventStore" in Name Field and click next.
This all was needed to configure RavenDB. If the link "http://localhost:8081/raven/studio.html" make sure that RavenDB service is running.

## MySql
1. Download latest version of MySql community server.
2. Run .exe or .msi file and follow the instructions.
3. Select Developer default.
4. There should be no password for root user.
5. Execute the "create" and "populate" scripts in the src\data\MySql folder.

open command line
go to location where MySQL is installed e.g. C:\Program Files\MySQL\MySQL Server 5.7\bin
then run this command:
mysql -u root -h localhost -p
give password when prompted
then run this command to create the database (plz change path):
SOURCE C:\Users\rasheed-pc\Source\Repos\coin-exchange-backend\src\Data\MySql\create.sql
then populate the database
SOURCE C:\Users\rasheed-pc\Source\Repos\coin-exchange-backend\src\Data\MySql\populate.sql

## Database Setup
1. Create a new database named coinexchange.
2. MySql Settings:
3. username = root,
4. host = localhost,
5. port = 3306.

## MySQL Connector for .NET:
Download and install the MySQL connector for .NET from here: http://dev.mysql.com/downloads/connector/net/
