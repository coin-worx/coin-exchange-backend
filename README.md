# Coin Exchange Backend

## Introduction
The platform acts as cryptocurrency exchange server to be used for retail and institutional trading. It can be accessed by any front end or mobile service comsuming the provided REST API.

## Context
The product serves as platform for trading different digital currencies. Currently following trading currencies are supported out of the box.

- Bitcoin
- Litecoin

The platform will mostly be a self contained product with not much integration required with external system(s). However, the billing module will have to support some different mechanisms for depositing and withdrawing money into user accounts that may include:

- Bitcoin deposit / withdrawal
- Litecon deposit / withdrawal

## Problem Statement
Cryptocurrency market has been on rise in the recent years and has become a popular financial entity. The crypto market is decentralized it's not being controlled by any single authority offering more control to the users. Some of the cryptocurrencies can also be used by setting a fixed supply cap making them more imune to inflation. This makes the cryptocurrency market highly profitable.

## Solution
To gain profit from cryptocurrency market apart from investing in trading cryptocurrencies itself one can take advantage from market liquidity by creating their own cryptocurrency exchange. Creating a cryptocurrency exchange offers the advantage of being a part of the cyrpto market itself and one can take advantage of the transaction fees to generate revenue.
 
#### Technology Stack
1. Visual Studio 2012
2. .NET Framework 4.5
3. Databases:
- MySql (preferably the latest version)
- RavenDB
4. IIS

#### Frontend
A web based frontend is already available to be used: [Web-UI](https://github.com/coin-worx/coin-exchange-frontend) 

#### Infrastructure
The project runs as an IIS service.

## Usage 

### Running

#### Project Structure
- Working code directory is coin-exchange-backend\src.

#### Install IIS
- Follow this guide to setup IIS on windows machine: https://msdn.microsoft.com/en-us/Library/ms181052(v=vs.80).aspx

#### Setting up RavenDB
1. Download the RavenDB from here: http://hibernatingrhinos.com/downloads/RavenDB%20Installer/2879
2. Run the Setup installation.
3. While installing the RavenDB, please select "Development" in Tragetting environment.
4. Install the RavenDB as a service and chose enter port 8081.
5. After the installtion completes, open "http://localhost:8081/raven/studio.html" link in the browser.
(install silver light if prompted) you might have to use IE for opening the ravendb studio
6. In the main screen,click on "New Database", type "InputEventStore" in Name Field and click next.
7. In the main screen,click on "New Database", type "OutputEventStore" in Name Field and click next.
This all was needed to configure RavenDB. If the link "http://localhost:8081/raven/studio.html" make sure that RavenDB service is running.

#### MySql
1. Download latest version of MySql community server.
2. Run .exe or .msi file and follow the instructions.
3. Select Developer default.
4. There should be no password for root user.
5. Execute the "create" and "populate" scripts in the src\data\MySql folder.

#### Database Setup
1. Create a new database named coinexchange.
2. MySql Settings:
3. username = root,
4. host = localhost,
5. port = 3306.

#### MySQL Connector for .NET:
Download and install the MySQL connector for .NET from here: http://dev.mysql.com/downloads/connector/net/

#### Building
- Clone the repo: git clone https://github.com/coin-worx/coin-exchange-backend.git
- open command line and go to location where MySQL is installed e.g. C:\Program Files\MySQL\MySQL Server 5.7\bin
- Execute command: **mysql -u root -h localhost -p** and provide password when prompted.
- Then execute command to create the database (plz change path): **SOURCE ..\coin-exchange-backend\src\Data\MySql\create.sql**
- After that populate the database: **SOURCE ..\coin-exchange-backend\src\Data\MySql\populate.sql**
- NAnt and ccnet build scripts are available in **..\coin-exchange-backend\src\Build**

### Logs
  *TODO: Add content*
  
## Help

**Got a question?** 
File a GitHub [issue](https://github.com/coin-worx/coin-exchange-frontend/issues), send us an email at info@aurorasolutions.io or reach out to us on social media [Twitter](https://twitter.com/aurora__sol?lang=en), [Facebook](https://www.facebook.com/AuroraSolutions/)

## Contributing 

### Bug Reports & Feature Requests

Please use the [issue tracker](https://github.com/coin-worx/coin-exchange-frontend/issues) to report any bugs or file feature requests.

#### Developing

PRs are welcome. In general, we follow the "fork-and-pull" Git workflow.

 1. **Fork** the repo on GitHub
 2. **Clone** the project to your own machine
 3. **Commit** changes to your own branch
 4. **Push** your work back up to your fork
 5. Submit a **Pull request** so that we can review your changes

NOTE: Be sure to merge the latest from "upstream" before making a pull request!

## About
The project is created and maintained by [Aurora Solutions](https://www.aurorasolutions.io/). Like it? Please let us know at [info@aurorasolutions.io](info@aurorasolutions.io)

See our [other projects](https://www.aurorasolutions.io/#portfolio)
