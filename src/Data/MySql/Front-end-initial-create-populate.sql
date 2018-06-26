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
  `BalanceId` int(10) NOT NULL AUTO_INCREMENT,
  `Currency` varchar(10) DEFAULT NULL,
  `AccountId` int(11) DEFAULT NULL,
  `AvailableBalance` double DEFAULT NULL,
  `CurrentBalance` double DEFAULT NULL,
  `PendingBalance` double DEFAULT NULL,
  `IsFrozen` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`BalanceId`)
) ENGINE=InnoDB AUTO_INCREMENT=18 DEFAULT CHARSET=latin1;

/*Data for the table `balance` */

insert  into `balance`(`BalanceId`,`Currency`,`AccountId`,`AvailableBalance`,`CurrentBalance`,`PendingBalance`,`IsFrozen`) values (6,'BTC',1,2000,2000,0,0),(7,'LTC',1,2000,2000,0,0),(8,'BTC',2,2000,2000,0,0),(9,'LTC',2,2000,2000,0,0),(10,'BTC',3,2000,2000,0,0),(11,'LTC',3,2000,2000,0,0),(12,'BTC',4,2000,2000,0,0),(13,'LTC',4,2000,2000,0,0),(14,'BTC',5,2000,2000,0,0),(15,'LTC',5,2000,2000,0,0),(16,'BTC',6,2000,2000,0,0),(17,'LTC',6,2000,2000,0,0);

/*Table structure for table `currencypair` */

DROP TABLE IF EXISTS `currencypair`;

CREATE TABLE `currencypair` (
  `CurrencyPairName` varchar(25) NOT NULL,
  `BaseCurrency` varchar(10) DEFAULT NULL,
  `QuoteCurrency` varchar(10) DEFAULT NULL,
  PRIMARY KEY (`CurrencyPairName`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 CHECKSUM=1 DELAY_KEY_WRITE=1 ROW_FORMAT=DYNAMIC;

/*Data for the table `currencypair` */

insert  into `currencypair`(`CurrencyPairName`,`BaseCurrency`,`QuoteCurrency`) values ('BTCLTC','BTC','LTC');

/*Table structure for table `deposit` */

DROP TABLE IF EXISTS `deposit`;

CREATE TABLE `deposit` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Currency` varchar(7) DEFAULT NULL,
  `IsCryptoCurrency` tinyint(1) DEFAULT NULL,
  `AccountId` int(11) DEFAULT NULL,
  `DepositId` varchar(100) DEFAULT NULL,
  `Date` datetime(6) DEFAULT NULL,
  `Amount` double DEFAULT NULL,
  `Fee` double DEFAULT NULL,
  `Confirmations` int(11) DEFAULT NULL,
  `Status` varchar(15) DEFAULT NULL,
  `Type` varchar(15) DEFAULT NULL,
  `TransactionId` varchar(100) DEFAULT NULL,
  `BitcoinAddress` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=latin1;

/*Data for the table `deposit` */

insert  into `deposit`(`Id`,`Currency`,`IsCryptoCurrency`,`AccountId`,`DepositId`,`Date`,`Amount`,`Fee`,`Confirmations`,`Status`,`Type`,`TransactionId`,`BitcoinAddress`) values (3,'BTC',1,1,'3271cee7-0792-4494-a2da-ab28a4e671f8','2014-09-25 12:17:09.000000',2000,0,0,'1','0',NULL,NULL),(4,'LTC',1,1,'6c914e26-8f5f-4f53-bf35-b8dceac6c42d','2014-09-25 12:17:09.000000',2000,0,0,'1','0',NULL,NULL),(5,'BTC',1,2,'a12a1dfa-1b8b-42fb-a6d1-0a1e3d7a4083','2014-09-25 12:19:35.000000',2000,0,0,'1','0',NULL,NULL),(6,'LTC',1,2,'23e20e43-9981-4a39-9aca-f9e9bbf3a272','2014-09-25 12:19:35.000000',2000,0,0,'1','0',NULL,NULL),(7,'BTC',1,3,'eb64b775-8441-43ac-ba54-4636b20c3b6d','2014-09-25 12:21:53.000000',2000,0,0,'1','0',NULL,NULL),(8,'LTC',1,3,'fe488799-9fa8-4f87-8c9d-7da64b224350','2014-09-25 12:21:54.000000',2000,0,0,'1','0',NULL,NULL),(9,'BTC',1,4,'452ad151-e244-44c0-9640-6ee9dd5ada4d','2014-09-25 12:23:23.000000',2000,0,0,'1','0',NULL,NULL),(10,'LTC',1,4,'aa866c68-9873-47a3-9069-d6fa1f5dcec0','2014-09-25 12:23:23.000000',2000,0,0,'1','0',NULL,NULL),(11,'BTC',1,5,'5e81241f-e448-4419-aa62-a5a36c89549a','2014-09-25 12:25:19.000000',2000,0,0,'1','0',NULL,NULL),(12,'LTC',1,5,'a14d232b-c9c9-4223-a464-c7e94dbac211','2014-09-25 12:25:19.000000',2000,0,0,'1','0',NULL,NULL),(13,'BTC',1,6,'791fe90a-313f-4afe-9579-b129d0ff154c','2014-09-25 12:27:52.000000',2000,0,0,'1','0',NULL,NULL),(14,'LTC',1,6,'3ed15afb-d5e2-467c-aa72-05dc84437f68','2014-09-25 12:27:52.000000',2000,0,0,'1','0',NULL,NULL);

/*Table structure for table `depositaddress` */

DROP TABLE IF EXISTS `depositaddress`;

CREATE TABLE `depositaddress` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Currency` varchar(15) DEFAULT NULL,
  `IsCryptoCurrency` tinyint(1) DEFAULT NULL,
  `AccountId` int(11) DEFAULT NULL,
  `BitcoinAddress` varchar(50) DEFAULT NULL,
  `Status` varchar(20) DEFAULT NULL,
  `CreationDateTime` datetime DEFAULT NULL,
  `DateUsed` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=latin1;

/*Data for the table `depositaddress` */

/*Table structure for table `depositlimit` */

DROP TABLE IF EXISTS `depositlimit`;

CREATE TABLE `depositlimit` (
  `Id` int(10) NOT NULL AUTO_INCREMENT,
  `TierLevel` varchar(15) DEFAULT NULL,
  `DailyLimit` double DEFAULT NULL,
  `MonthlyLimit` double DEFAULT NULL,
  `LimitsCurrency` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=latin1;

/*Data for the table `depositlimit` */

insert  into `depositlimit`(`Id`,`TierLevel`,`DailyLimit`,`MonthlyLimit`,`LimitsCurrency`) values (1,'Tier 0',0,0,'Default'),(2,'Tier 1',2,10,'Default'),(3,'Tier 2',4,20,'Default'),(4,'Tier 3',40,200,'Default'),(5,'Tier 4',200,1000,'Default');

/*Table structure for table `fee` */

DROP TABLE IF EXISTS `fee`;

CREATE TABLE `fee` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `CurrencyPair` varchar(15) DEFAULT NULL,
  `PercentageFee` double DEFAULT NULL,
  `Amount` double DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=51 DEFAULT CHARSET=latin1;

/*Data for the table `fee` */

insert  into `fee`(`Id`,`CurrencyPair`,`PercentageFee`,`Amount`) values (1,'BTCLTC',0.1,1000),(2,'BTCLTC',0.9,2000),(3,'BTCLTC',0.8,3000),(4,'BTCLTC',0.7,5000),(5,'BTCLTC',0.6,8000),(6,'BTCLTC',0.5,10000),(7,'BTCLTC',0.4,15000);

/*Table structure for table `ledger` */

DROP TABLE IF EXISTS `ledger`;

CREATE TABLE `ledger` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `LedgerId` varchar(50) DEFAULT NULL,
  `DateTime` datetime(6) DEFAULT NULL,
  `LedgerType` varchar(15) DEFAULT NULL,
  `Currency` varchar(15) DEFAULT NULL,
  `AccountId` int(11) DEFAULT NULL,
  `Amount` double DEFAULT NULL,
  `AmountInUsd` double DEFAULT NULL,
  `Fee` double DEFAULT NULL,
  `Balance` double DEFAULT NULL,
  `TradeId` varchar(50) DEFAULT NULL,
  `OrderId` varchar(50) DEFAULT NULL,
  `WithdrawId` varchar(50) DEFAULT NULL,
  `DepositId` varchar(50) DEFAULT NULL,
  `IsBaseCurrencyInTrade` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=latin1;

/*Data for the table `ledger` */

/*Table structure for table `mfasubscription` */

DROP TABLE IF EXISTS `mfasubscription`;

CREATE TABLE `mfasubscription` (
  `MfaSubscriptionId` varchar(11) NOT NULL,
  `MfaSubscriptionName` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`MfaSubscriptionId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*Data for the table `mfasubscription` */

insert  into `mfasubscription`(`MfaSubscriptionId`,`MfaSubscriptionName`) values ('CO','CancelOrder'),('DEP','Deposit'),('LOG','Login'),('PO','PlaceOrder'),('WD','Withdraw');

/*Table structure for table `mfasubscriptionstatus` */

DROP TABLE IF EXISTS `mfasubscriptionstatus`;

CREATE TABLE `mfasubscriptionstatus` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ApiKey` varchar(100) DEFAULT NULL,
  `UserId` int(11) DEFAULT NULL,
  `Enabled` tinyint(1) DEFAULT NULL,
  `MfaSubscriptionId` varchar(11) DEFAULT NULL,
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

/*Data for the table `ohlc` */

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

/*Data for the table `order` */

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

/*Data for the table `passwordcoderecord` */

/*Table structure for table `pendingtransaction` */

DROP TABLE IF EXISTS `pendingtransaction`;

CREATE TABLE `pendingtransaction` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `BalanceId` int(10) DEFAULT NULL,
  `Currency` varchar(20) DEFAULT NULL,
  `InstanceId` varchar(50) DEFAULT NULL,
  `PendingTransactionType` int(11) DEFAULT NULL,
  `Amount` double DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=latin1;

/*Data for the table `pendingtransaction` */

/*Table structure for table `permission` */

DROP TABLE IF EXISTS `permission`;

CREATE TABLE `permission` (
  `PermissionId` varchar(10) NOT NULL,
  `PermissionName` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`PermissionId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*Data for the table `permission` */

insert  into `permission`(`PermissionId`,`PermissionName`) values ('CO','Cancel Order'),('DF','Deposit Funds'),('PO','Place Order'),('QCOT','Query Closed Orders and Trades'),('QF','Query Funds'),('QLT','Query Ledger Entries'),('QOOT','Query Open Orders and Trades'),('WF','Withdraw Funds');

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
  `MfaCode` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`ApiKey`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*Data for the table `securitykeyspair` */

insert  into `securitykeyspair`(`ApiKey`,`SecretKey`,`KeyDescription`,`ExpirationDate`,`StartDate`,`EndDate`,`LastModified`,`SystemGenerated`,`UserId`,`CreationDateTime`,`EnableExpirationDate`,`EnableStartDate`,`EnableEndDate`,`Deleted`) values ('8X8XlypsT0SlZJav1UGabg','cN6dom8f6kiWBJfFl6f6FA','25/09/2014 12:23:22',NULL,NULL,NULL,'2014-09-25 12:23:23',1,4,'2014-09-25 12:23:22',0,0,0,0),('G0OiQnVV0WYmAskZQbQQg','Z4JjUs4qkenfrg2oddSFQ','25/09/2014 12:27:51',NULL,NULL,NULL,'2014-09-25 12:27:52',1,6,'2014-09-25 12:27:51',0,0,0,0),('HoWxVSGcUEiI3u1dogHjpw','o1xpduSiES40eprWGZuWw','25/09/2014 12:21:53',NULL,NULL,NULL,'2014-09-25 12:21:53',1,3,'2014-09-25 12:21:53',0,0,0,0),('qaNxIvWXJkmfpU8JKDZmlw','brVCYrZZM06FXf0Y2IXAA','25/09/2014 12:19:34',NULL,NULL,NULL,'2014-09-25 12:19:35',1,2,'2014-09-25 12:19:34',0,0,0,0),('qb05nrXn9kSdgEghrYTbg','K9JOb4INnUeNqH01hpxX0A','25/09/2014 12:17:08',NULL,NULL,NULL,'2014-09-25 12:17:09',1,1,'2014-09-25 12:17:08',0,0,0,0),('r71kaX0CcUyHPBGLIMCZA','SKCOgBAakUuf6gnZZ4u4Q','25/09/2014 12:25:18',NULL,NULL,NULL,'2014-09-25 12:25:19',1,5,'2014-09-25 12:25:18',0,0,0,0);

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

/*Data for the table `securitykeyspermission` */

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

/*Data for the table `tickerinfo` */

/*Table structure for table `tier` */

DROP TABLE IF EXISTS `tier`;

CREATE TABLE `tier` (
  `TierLevel` varchar(30) NOT NULL,
  `TierName` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`TierLevel`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*Data for the table `tier` */

insert  into `tier`(`TierLevel`,`TierName`) values ('Tier 0','Tier 0'),('Tier 1','Tier 1'),('Tier 2','Tier 2'),('Tier 3','Tier 3'),('Tier 4','Tier 4');

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

/*Data for the table `trade` */

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
  `AdminEmailsSubscribed` tinyint(1) DEFAULT NULL,
  `NewsletterEmailsSubscribed` tinyint(1) DEFAULT NULL,
  `MfaCode` varchar(60) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `NewIndex1` (`UserName`,`Email`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=latin1;

/*Data for the table `user` */

insert  into `user`(`Id`,`UserName`,`Password`,`PublicKey`,`Email`,`Language`,`TimeZone`,`AutoLogout`,`LastLogin`,`ActivationKey`,`Country`,`State`,`PhoneNumber`,`Address1`,`Address2`,`IsActivationKeyUsed`,`IsUserBlocked`,`Deleted`,`ForgotPasswordCode`,`ForgotPasswordCodeExpiration`,`City`,`ZipCode`,`SocialSecurityNumber`,`NationalIdentificationNumber`,`FullName`,`DateOfBirth`,`AdminEmailsSubscribed`,`NewsletterEmailsSubscribed`) values (1,'rodholt','$2a$12$AbpwfdY1NbQv2YjGahR44e5jNIMca4UodPVFfDU9uv39rwgd7LYaK','','rodholt@mclaren.com','0',NULL,6000000000,'2014-09-25 12:17:08','sJLbzNhr60KmeC0OUG7uPg','pakistan',NULL,'+1244322222',NULL,NULL,1,0,0,NULL,NULL,NULL,0,NULL,NULL,'Rod Holt','2071-09-25',0,0),(2,'bruce','$2a$12$1AoBuHUrBhz/e/e6Os5SpO2HxLzGZQRPPFxNjFkjCUJ2eyparDj1y','','bruce@wayne.com','0',NULL,6000000000,'2014-09-25 12:19:35','hXDgvQ77KkyxOv7SFPGzGw','pakistan',NULL,'+1244322222',NULL,NULL,1,0,0,NULL,NULL,NULL,0,NULL,NULL,'Rod Holt','2071-09-25',0,0),(3,'thejoker','$2a$12$Z9K1foK9BK3XCcVp5U7dsetf6hb16SpZ0FOVA7dg4leQB.AF4Vfsa','','thejoker@gotham.com','0',NULL,6000000000,'2014-09-25 12:21:53','BfGaI2W7km4CIbv2CsQ','pakistan',NULL,'+1244322222',NULL,NULL,1,0,0,NULL,NULL,NULL,0,NULL,NULL,'Rod Holt','2071-09-25',0,0),(4,'gordon','$2a$12$8xuq6lkWUJefWY210cy0E.nubDAmQNJzhPkw00PG8iJS22DKu4fG6','','gordon@gotham.com','0',NULL,6000000000,'2014-09-25 12:23:22','8irjbYGboUW4BTgeMkHQUQ','pakistan',NULL,'+1244322222',NULL,NULL,1,0,0,NULL,NULL,NULL,0,NULL,NULL,'Rod Holt','2071-09-25',0,0),(5,'bane','$2a$12$4jLZUuNxVS2xwAKOOv4m..UfPEO6XUhUuNev1ytOQM5.3AziLcTIy','','bane@gotham.com','0',NULL,6000000000,'2014-09-25 12:25:19','QwED5Vq7X0KE0kLASb6g','pakistan',NULL,'+1244322222',NULL,NULL,1,0,0,NULL,NULL,NULL,0,NULL,NULL,'Rod Holt','2071-09-25',0,0),(6,'jonsnow','$2a$12$8NJNaUYMjFowEoVoIJNRU.Yf5589PmGVmi4PxXu3p6mKcvYICpDPm','','jonsnow@winter.com','0',NULL,6000000000,'2014-09-25 12:27:51','72eHniVx02K2gQVnYLtkQ','pakistan',NULL,'+1244322222',NULL,NULL,1,0,0,NULL,NULL,NULL,0,NULL,NULL,'Rod Holt','2071-09-25',0,0);

/*Table structure for table `userdocument` */

DROP TABLE IF EXISTS `userdocument`;

CREATE TABLE `userdocument` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `DocumentType` varchar(50) DEFAULT NULL,
  `DocumentPath` varchar(200) DEFAULT NULL,
  `UserId` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*Data for the table `userdocument` */

/*Table structure for table `usertierlevelstatus` */

DROP TABLE IF EXISTS `usertierlevelstatus`;

CREATE TABLE `usertierlevelstatus` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `TierLevel` varchar(50) DEFAULT NULL,
  `Status` varchar(50) DEFAULT NULL,
  `UserId` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=31 DEFAULT CHARSET=latin1;

/*Data for the table `usertierlevelstatus` */

insert  into `usertierlevelstatus`(`Id`,`TierLevel`,`Status`,`UserId`) values (1,'Tier 0','1',1),(2,'Tier 1','1',1),(3,'Tier 2','0',1),(4,'Tier 3','0',1),(5,'Tier 4','0',1),(6,'Tier 0','1',2),(7,'Tier 1','1',2),(8,'Tier 2','0',2),(9,'Tier 3','0',2),(10,'Tier 4','0',2),(11,'Tier 0','1',3),(12,'Tier 1','1',3),(13,'Tier 2','0',3),(14,'Tier 3','0',3),(15,'Tier 4','0',3),(16,'Tier 0','1',4),(17,'Tier 1','1',4),(18,'Tier 2','0',4),(19,'Tier 3','0',4),(20,'Tier 4','0',4),(21,'Tier 0','1',5),(22,'Tier 1','1',5),(23,'Tier 2','0',5),(24,'Tier 3','0',5),(25,'Tier 4','0',5),(26,'Tier 0','1',6),(27,'Tier 1','1',6),(28,'Tier 2','0',6),(29,'Tier 3','0',6),(30,'Tier 4','0',6);

/*Table structure for table `withdraw` */

DROP TABLE IF EXISTS `withdraw`;

CREATE TABLE `withdraw` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `WithdrawId` varchar(50) DEFAULT NULL,
  `Currency` varchar(10) DEFAULT NULL,
  `IsCryptoCurrency` tinyint(1) DEFAULT NULL,
  `Amount` double DEFAULT NULL,
  `AmountInUsd` double DEFAULT NULL,
  `Fee` double DEFAULT NULL,
  `DateTime` datetime DEFAULT NULL,
  `Type` varchar(15) DEFAULT NULL,
  `Status` varchar(15) DEFAULT NULL,
  `AccountId` int(11) DEFAULT NULL,
  `BankName` varchar(20) DEFAULT NULL,
  `BankAccountAddress` varchar(20) DEFAULT NULL,
  `BankAccountSwiftCode` varchar(20) DEFAULT NULL,
  `TransactionId` varchar(50) DEFAULT NULL,
  `BitcoinAddress` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*Data for the table `withdraw` */

/*Table structure for table `withdrawaddress` */

DROP TABLE IF EXISTS `withdrawaddress`;

CREATE TABLE `withdrawaddress` (
  `Id` int(10) NOT NULL AUTO_INCREMENT,
  `Currency` varchar(15) DEFAULT NULL,
  `IsCryptoCurrency` tinyint(1) DEFAULT NULL,
  `AccountId` int(11) DEFAULT NULL,
  `BitcoinAddress` varchar(50) DEFAULT NULL,
  `Description` varchar(140) DEFAULT NULL,
  `CreationDateTime` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*Data for the table `withdrawaddress` */

/*Table structure for table `withdrawfees` */

DROP TABLE IF EXISTS `withdrawfees`;

CREATE TABLE `withdrawfees` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Currency` varchar(15) DEFAULT NULL,
  `MinimumAmount` double DEFAULT NULL,
  `Fee` double DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=latin1;

/*Data for the table `withdrawfees` */

insert  into `withdrawfees`(`Id`,`Currency`,`MinimumAmount`,`Fee`) values (3,'BTC',0.001,0.0001),(4,'LTC',0.001,0.0001);

/*Table structure for table `withdrawlimit` */

DROP TABLE IF EXISTS `withdrawlimit`;

CREATE TABLE `withdrawlimit` (
  `Id` int(10) NOT NULL AUTO_INCREMENT,
  `TierLevel` varchar(15) DEFAULT NULL,
  `DailyLimit` double DEFAULT NULL,
  `MonthlyLimit` double DEFAULT NULL,
  `LimitsCurrency` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=latin1;

/*Data for the table `withdrawlimit` */

insert  into `withdrawlimit`(`Id`,`TierLevel`,`DailyLimit`,`MonthlyLimit`,`LimitsCurrency`) values (1,'Tier 0',0,0,'Default'),(2,'Tier 1',2,10,'Default'),(3,'Tier 2',4,20,'Default'),(4,'Tier 3',40,200,'Default'),(5,'Tier 4',200,1000,'Default');

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
