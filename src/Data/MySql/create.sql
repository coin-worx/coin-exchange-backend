/*
SQLyog Enterprise - MySQL GUI v6.5
MySQL - 5.6.14 : Database - coinexchangedev
*********************************************************************
*/

/*!40101 SET NAMES utf8 */;

/*!40101 SET SQL_MODE=''*/;

/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;

create database if not exists `coinexchangedev`;

USE `coinexchangedev`;

/*Table structure for table `balance` */

DROP TABLE IF EXISTS `balance`;

CREATE TABLE `balance` (
  `Id` int(10) NOT NULL AUTO_INCREMENT,
  `Currency` varchar(10) DEFAULT NULL,
  `AccountId` varchar(30) DEFAULT NULL,
  `AvailableBalance` double DEFAULT NULL,
  `CurrentBalance` double DEFAULT NULL,
  `PendingBalance` double DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*Table structure for table `currencypair` */

DROP TABLE IF EXISTS `currencypair`;

CREATE TABLE `currencypair` (
  `CurrencyPairName` varchar(25) NOT NULL,
  `BaseCurrency` varchar(10) DEFAULT NULL,
  `QuoteCurrency` varchar(10) DEFAULT NULL,
  PRIMARY KEY (`CurrencyPairName`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 CHECKSUM=1 DELAY_KEY_WRITE=1 ROW_FORMAT=DYNAMIC;

/*Table structure for table `deposit` */

DROP TABLE IF EXISTS `deposit`;

CREATE TABLE `deposit` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Currency` varchar(7) DEFAULT NULL,
  `AccountId` varchar(15) DEFAULT NULL,
  `DepositId` varchar(20) DEFAULT NULL,
  `Date` datetime(6) DEFAULT NULL,
  `Amount` double DEFAULT NULL,
  `Fee` double DEFAULT NULL,
  `Confirmations` int(11) DEFAULT NULL,
  `Status` varchar(15) DEFAULT NULL,
  `Type` varchar(15) DEFAULT NULL,
  `TransactionId` varchar(30) DEFAULT NULL,
  `BitcoinAddress` varchar(30) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*Table structure for table `depositaddress` */

DROP TABLE IF EXISTS `depositaddress`;

CREATE TABLE `depositaddress` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `AccountId` varchar(20) DEFAULT NULL,
  `BitcoinAddress` varchar(30) DEFAULT NULL,
  `Status` varchar(20) DEFAULT NULL,
  `CreationDateTime` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*Table structure for table `depositlimit` */

DROP TABLE IF EXISTS `depositlimit`;

CREATE TABLE `depositlimit` (
  `Id` int(10) NOT NULL AUTO_INCREMENT,
  `TierLevel` varchar(15) DEFAULT NULL,
  `DailyLimit` double DEFAULT NULL,
  `MonthlyLimit` double DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*Table structure for table `fee` */

DROP TABLE IF EXISTS `fee`;

CREATE TABLE `fee` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `CurrencyPair` varchar(15) DEFAULT NULL,
  `PercentageFee` double DEFAULT NULL,
  `Amount` double DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=26 DEFAULT CHARSET=latin1;

/*Table structure for table `ledger` */

DROP TABLE IF EXISTS `ledger`;

CREATE TABLE `ledger` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `LedgerId` varchar(20) DEFAULT NULL,
  `DateTime` datetime(6) DEFAULT NULL,
  `LedgerType` varchar(15) DEFAULT NULL,
  `Currency` varchar(15) DEFAULT NULL,
  `AccountId` varchar(20) DEFAULT NULL,
  `Amount` double DEFAULT NULL,
  `Fee` double DEFAULT NULL,
  `Balance` double DEFAULT NULL,
  `TradeId` varchar(20) DEFAULT NULL,
  `OrderId` varchar(20) DEFAULT NULL,
  `WithdrawId` varchar(20) DEFAULT NULL,
  `DepositId` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*Table structure for table `ohlc` */

DROP TABLE IF EXISTS `ohlc`;

CREATE TABLE `ohlc` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `DateTime` datetime DEFAULT NULL,
  `Open` decimal(10,5) DEFAULT NULL,
  `High` decimal(10,5) DEFAULT NULL,
  `Low` decimal(10,5) DEFAULT NULL,
  `Close` decimal(10,5) DEFAULT NULL,
  `Volume` decimal(10,5) DEFAULT NULL,
  `CurrencyPair` varchar(100) DEFAULT NULL,
  `AveragePrice` decimal(10,5) DEFAULT NULL,
  `TotalWeight` decimal(10,5) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*Table structure for table `order` */

DROP TABLE IF EXISTS `order`;

CREATE TABLE `order` (
  `OrderId` varchar(100) NOT NULL,
  `OrderType` varchar(100) DEFAULT NULL,
  `OrderSide` varchar(100) DEFAULT NULL,
  `Price` decimal(10,5) DEFAULT NULL,
  `VolumeExecuted` decimal(10,5) DEFAULT NULL,
  `Status` varchar(100) DEFAULT NULL,
  `TraderId` varchar(100) DEFAULT NULL,
  `CurrencyPair` varchar(25) DEFAULT NULL,
  `OrderDateTime` datetime DEFAULT NULL,
  `Volume` decimal(10,5) DEFAULT NULL,
  `OpenQuantity` decimal(10,5) DEFAULT NULL,
  `AveragePrice` decimal(10,5) DEFAULT NULL,
  `ClosingDateTime` datetime DEFAULT NULL,
  PRIMARY KEY (`OrderId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*Table structure for table `passwordcoderecord` */

DROP TABLE IF EXISTS `passwordcoderecord`;

CREATE TABLE `passwordcoderecord` (
  `Id` int(11) NOT NULL,
  `ForgotPasswordCode` varchar(100) DEFAULT NULL,
  `ExpirationDateTime` datetime DEFAULT NULL,
  `CreationDateTime` datetime DEFAULT NULL,
  `IsUsed` tinyint(1) DEFAULT NULL,
  `UserId` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*Table structure for table `permission` */

DROP TABLE IF EXISTS `permission`;

CREATE TABLE `permission` (
  `PermissionId` varchar(10) NOT NULL,
  `PermissionName` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`PermissionId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*Table structure for table `securitykeyspair` */

DROP TABLE IF EXISTS `securitykeyspair`;

CREATE TABLE `securitykeyspair` (
  `ApiKey` varchar(100) NOT NULL,
  `SecretKey` varchar(100) DEFAULT NULL,
  `KeyDescription` varchar(50) DEFAULT NULL,
  `ExpirationDate` datetime DEFAULT NULL,
  `StartDate` datetime DEFAULT NULL,
  `EndDate` datetime DEFAULT NULL,
  `LastModified` datetime DEFAULT NULL,
  `SystemGenerated` tinyint(1) DEFAULT NULL,
  `UserId` int(11) DEFAULT NULL,
  `CreationDateTime` datetime DEFAULT NULL,
  `EnableExpirationDate` tinyint(1) DEFAULT NULL,
  `EnableStartDate` tinyint(1) DEFAULT NULL,
  `EnableEndDate` tinyint(1) DEFAULT NULL,
  `Deleted` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`ApiKey`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*Table structure for table `securitykeyspermission` */

DROP TABLE IF EXISTS `securitykeyspermission`;

CREATE TABLE `securitykeyspermission` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ApiKey` varchar(100) DEFAULT NULL,
  `PermissionId` varchar(10) DEFAULT NULL,
  `IsAllowed` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_apikeypermission` (`PermissionId`),
  CONSTRAINT `FK_apikeypermission` FOREIGN KEY (`PermissionId`) REFERENCES `permission` (`PermissionId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*Table structure for table `tickerinfo` */

DROP TABLE IF EXISTS `tickerinfo`;

CREATE TABLE `tickerinfo` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `AskPrice` decimal(10,5) DEFAULT NULL,
  `AskVolume` decimal(10,5) DEFAULT NULL,
  `BidPrice` decimal(10,5) DEFAULT NULL,
  `BidVolume` decimal(10,5) DEFAULT NULL,
  `OpeningPrice` decimal(10,5) DEFAULT NULL,
  `TradePrice` decimal(10,5) DEFAULT NULL,
  `TradeVolume` decimal(10,5) DEFAULT NULL,
  `TodaysVolumeWeight` decimal(10,5) DEFAULT NULL,
  `Last24HourVolumeWeight` decimal(10,5) DEFAULT NULL,
  `TodaysTrades` bigint(20) DEFAULT NULL,
  `Last24HourTrades` bigint(20) DEFAULT NULL,
  `TodaysLow` decimal(10,5) DEFAULT NULL,
  `Last24HoursLow` decimal(10,5) DEFAULT NULL,
  `TodaysHigh` decimal(10,5) DEFAULT NULL,
  `Last24HoursHigh` decimal(10,5) DEFAULT NULL,
  `TodaysVolume` decimal(10,5) DEFAULT NULL,
  `Last24HourVolume` decimal(10,5) DEFAULT NULL,
  `CurrencyPair` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 CHECKSUM=1 DELAY_KEY_WRITE=1 ROW_FORMAT=DYNAMIC;

/*Table structure for table `tier` */

DROP TABLE IF EXISTS `tier`;

CREATE TABLE `tier` (
  `TierLevel` varchar(30) NOT NULL,
  `TierName` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`TierLevel`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*Table structure for table `trade` */

DROP TABLE IF EXISTS `trade`;

CREATE TABLE `trade` (
  `TradeId` varchar(100) NOT NULL,
  `Price` decimal(10,5) DEFAULT NULL,
  `Volume` decimal(10,5) DEFAULT NULL,
  `ExecutionDateTime` datetime DEFAULT NULL,
  `CurrencyPair` varchar(50) DEFAULT NULL,
  `BuyOrderId` varchar(100) DEFAULT NULL,
  `BuyTraderId` varchar(100) DEFAULT NULL,
  `SellOrderId` varchar(100) DEFAULT NULL,
  `SellTraderId` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`TradeId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*Table structure for table `user` */

DROP TABLE IF EXISTS `user`;

CREATE TABLE `user` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `UserName` varchar(50) DEFAULT NULL,
  `Password` varchar(100) DEFAULT NULL,
  `PublicKey` varchar(100) DEFAULT NULL,
  `Email` varchar(50) DEFAULT NULL,
  `Language` varchar(100) DEFAULT NULL,
  `TimeZone` mediumblob,
  `AutoLogout` bigint(20) DEFAULT NULL,
  `LastLogin` datetime DEFAULT NULL,
  `ActivationKey` varchar(50) DEFAULT NULL,
  `Country` varchar(50) DEFAULT NULL,
  `State` varchar(50) DEFAULT NULL,
  `PhoneNumber` varchar(50) DEFAULT NULL,
  `Address1` varchar(100) DEFAULT NULL,
  `Address2` varchar(100) DEFAULT NULL,
  `IsActivationKeyUsed` tinyint(1) DEFAULT NULL,
  `IsUserBlocked` tinyint(1) DEFAULT NULL,
  `Deleted` tinyint(1) DEFAULT NULL,
  `ForgotPasswordCode` varchar(100) DEFAULT NULL,
  `ForgotPasswordCodeExpiration` datetime DEFAULT NULL,
  `City` varchar(50) DEFAULT NULL,
  `ZipCode` int(11) DEFAULT NULL,
  `SocialSecurityNumber` varchar(50) DEFAULT NULL,
  `NationalIdentificationNumber` varchar(50) DEFAULT NULL,
  `FullName` varchar(50) DEFAULT NULL,
  `DateOfBirth` date DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `NewIndex1` (`UserName`,`Email`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*Table structure for table `userdocument` */

DROP TABLE IF EXISTS `userdocument`;

CREATE TABLE `userdocument` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `DocumentType` varchar(50) DEFAULT NULL,
  `DocumentPath` varchar(200) DEFAULT NULL,
  `UserId` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*Table structure for table `usertierlevelstatus` */

DROP TABLE IF EXISTS `usertierlevelstatus`;

CREATE TABLE `usertierlevelstatus` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `TierLevel` varchar(50) DEFAULT NULL,
  `Status` varchar(50) DEFAULT NULL,
  `UserId` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*Table structure for table `withdraw` */

DROP TABLE IF EXISTS `withdraw`;

CREATE TABLE `withdraw` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `WithdrawId` varchar(15) DEFAULT NULL,
  `Currency` varchar(10) DEFAULT NULL,
  `Amount` double DEFAULT NULL,
  `Fee` double DEFAULT NULL,
  `DateTime` datetime DEFAULT NULL,
  `Type` varchar(15) DEFAULT NULL,
  `Status` varchar(15) DEFAULT NULL,
  `AccountId` varchar(15) DEFAULT NULL,
  `BankName` varchar(20) DEFAULT NULL,
  `BankAccountAddress` varchar(20) DEFAULT NULL,
  `BankAccountSwiftCode` varchar(20) DEFAULT NULL,
  `TransactionId` varchar(30) DEFAULT NULL,
  `BitcoinAddress` varchar(30) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*Table structure for table `withdrawaddress` */

DROP TABLE IF EXISTS `withdrawaddress`;

CREATE TABLE `withdrawaddress` (
  `Id` int(10) NOT NULL AUTO_INCREMENT,
  `AccountId` varchar(25) DEFAULT NULL,
  `BitcoinAddress` varchar(30) DEFAULT NULL,
  `Description` varchar(100) DEFAULT NULL,
  `CreationDateTime` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*Table structure for table `withdrawfees` */

DROP TABLE IF EXISTS `withdrawfees`;

CREATE TABLE `withdrawfees` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Currency` varchar(15) DEFAULT NULL,
  `MinimumAmount` double DEFAULT NULL,
  `Fee` double DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*Table structure for table `withdrawlimit` */

DROP TABLE IF EXISTS `withdrawlimit`;

CREATE TABLE `withdrawlimit` (
  `Id` int(10) NOT NULL AUTO_INCREMENT,
  `TierLevel` varchar(15) DEFAULT NULL,
  `DailyLimit` double DEFAULT NULL,
  `MonthlyLimit` double DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
