[![GitHub release](https://img.shields.io/github/release/coin-worx/coin-exchange-backend.svg?style=flat-square)](https://github.com/coin-worx/coin-exchange-backend/releases) [![GitHub (pre-)release](https://img.shields.io/github/release/coin-worx/coin-exchange-backend/all.svg?style=flat-square)](https://github.com/coin-worx/coin-exchange-backend/releases/tag/v0.3) [![GitHub issues](https://img.shields.io/github/issues/coin-worx/coin-exchange-backend.svg?style=flat-square)](https://github.com/coin-worx/coin-exchange-backend/issues) [![GitHub top language](https://img.shields.io/github/languages/top/coin-worx/coin-exchange-backend.svg?style=flat-square)](https://github.com/coin-worx/coin-exchange-backend) [![GitHub watchers](https://img.shields.io/github/watchers/coin-worx/coin-exchange-backend.svg?style=flat-square&label=Watch)](https://github.com/coin-worx/coin-exchange-backend/watchers) [![GitHub stars](https://img.shields.io/github/stars/coin-worx/coin-exchange-backend.svg?style=flat-square&label=Stars)](https://github.com/coin-worx/coin-exchange-backend/stargazers) [![GitHub forks](https://img.shields.io/github/forks/coin-worx/coin-exchange-backend.svg?style=flat-square&label=Fork)](https://github.com/coin-worx/coin-exchange-backend/network/members)

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

#### Use Cases
###### Market Data
- Limit Order Book management.
- Currency Depth details.
- Currency Spread details.

###### Orders
- Place Limit Orders.
- Place Market Orders.
- Manage existing Open Orders.
- View Closed Orders.
- View Individual Order Details.

###### Trades
- Overview of users all executed trades.
- User Ledger details.
- Individual Trade details.

###### Deposit & Withdraw
- Deposit crypto currency funds into user account.
- Withdraw funds from user account into cyrpto currency wallet.

[Screenshots](https://github.com/coin-worx/coin-exchange-frontend#screen-shots) are available in the frontend repository.

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
- Startup project is *CoinExchange.Rest.Webhost*.

#### Install IIS
- Follow this guide to setup IIS on windows machine: https://msdn.microsoft.com/en-us/Library/ms181052(v=vs.80).aspx

#### Setting up RavenDB
1. Download the RavenDB from their website: https://ravendb.net/download
2. Run the Setup installation using the provided instructions: https://ravendb.net/docs/article-page/4.0/csharp/start/installation/manual
3. While installing the RavenDB, please select "Development" in Tragetting environment.
4. Install the RavenDB as a service and chose enter port 8081.
5. After the installtion completes, open "http://localhost:8081/studio/index.html" link in the browser.
6. In the main screen,click on "New Database", type "InputEventStoreDev" in Name Field and click next.
7. In the main screen,click on "New Database", type "OutputEventStoreDev" in Name Field and click next.

If the link "http://localhost:8081/studio/index.html" is inaccessible make sure that RavenDB service is running. The following link can be used to start or stop RavenDB service: https://ravendb.net/docs/article-page/4.0/csharp/start/installation/running-as-service

*NOTE*: RavenDB and Event Sourcing has been disabled for now as the library is no longer functional.

#### MySql
1. Download latest version of MySql community server.
2. Run .exe or .msi file and follow the instructions.
3. Select Developer default.
4. There should be no password for root user.
5. Execute the "create" and "populate" scripts in the src\data\MySql folder.

#### Database Setup
1. Create a new database named **coinexchangedev**.
2. MySql Settings:
3. username = root,
4. host = localhost,
5. port = 3306.

#### MySQL Connector for .NET:
Download and install the MySQL connector for .NET from here: http://dev.mysql.com/downloads/connector/net/

#### Bitcoin:
Download and install bitcoin core from bitcoin [website](https://bitcoin.org/en/download).

If you want to run bitcoin in testnet, edit the configuration file named **bitcoin.conf** located in %appdata% under bitcoin directory and add the following propteris:
- testnet=1
- server=1
- rpcport=[Same as the value provided in Web.config file]
- rpcuser=[Same as the value provided in Web.config file]
- rpcpassword=[Same as the value provided in Web.config file]

#### Litecoin:
Similar to Bitcoin download and install litecoin core from litecoin [website](https://litecoin.org/).

If you want to run bitcoin in testnet, edit the configuration file named **litecoin.conf** located in %appdata% under litecoin directory and add the following propteris:
- testnet=1
- server=1
- rpcport=[Same as the value provided in Web.config file]
- rpcuser=[Same as the value provided in Web.config file]
- rpcpassword=[Same as the value provided in Web.config file]

Once done, go to the litecoin installaion directory and perform the following tasks:
- Copy the executable file and make a shortcut on the desktop.
- Right click the shortcut and go to properties and in the shortcut tab edit the target location by adding **-testnet** at the of the existing value after which your target location will look something like this: "C:\Program Files\Litecoin\litecoin-qt.exe" -testnet

#### Dogecoin:
Download and install dogecoin core from dogecoin [website](http://dogecoin.com/).

Setting up the testnet for dogecoin is similar to the tasks performed for the litecoin setup.

#### Building
- Clone the repo: git clone https://github.com/coin-worx/coin-exchange-backend.git
- open command line and go to location where MySQL is installed e.g. C:\Program Files\MySQL\MySQL Server 5.7\bin
- Execute command: **mysql -u root -h localhost -p** and provide password when prompted.
- Then execute command to create the database (plz change path): **SOURCE ..\coin-exchange-backend\src\Data\MySql\create.sql**
- After that populate the database: **SOURCE ..\coin-exchange-backend\src\Data\MySql\populate.sql**
- NAnt and ccnet build scripts are available in **..\coin-exchange-backend\src\Build**

### Logs
Log files are created under the directory: **C:\Logs**
  
## Help

**Got a question?** 
File a GitHub [issue](https://github.com/coin-worx/coin-exchange-frontend/issues), send us an email at info@aurorasolutions.io or reach out to us on social media [Twitter](https://twitter.com/aurora__sol?lang=en), [Facebook](https://www.facebook.com/AuroraSolutions/)

## Contributing 

### Bug Reports & Feature Requests

Please use the [issue tracker](https://github.com/coin-worx/coin-exchange-backend/issues) to report any bugs or file feature requests.

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
