/*
SQLyog Enterprise - MySQL GUI v6.5
MySQL - 5.6.14 : Database - coinexchange
*********************************************************************
*/

/*!40101 SET NAMES utf8 */;

/*!40101 SET SQL_MODE=''*/;

/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;

create database if not exists `coinexchange`;

USE `coinexchange`;

/*Table structure for table `currencypair` */

DROP TABLE IF EXISTS `currencypair`;

CREATE TABLE `currencypair` (
  `CurrencyPairName` varchar(25) NOT NULL,
  `BaseCurrency` varchar(10) DEFAULT NULL,
  `QuoteCurrency` varchar(10) DEFAULT NULL,
  PRIMARY KEY (`CurrencyPairName`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 CHECKSUM=1 DELAY_KEY_WRITE=1 ROW_FORMAT=DYNAMIC;

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
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=21 DEFAULT CHARSET=latin1;

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
  PRIMARY KEY (`OrderId`)
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
) ENGINE=InnoDB AUTO_INCREMENT=20 DEFAULT CHARSET=latin1 CHECKSUM=1 DELAY_KEY_WRITE=1 ROW_FORMAT=DYNAMIC;

/*Table structure for table `trade` */

DROP TABLE IF EXISTS `trade`;

CREATE TABLE `trade` (
  `TradeId` varchar(100) NOT NULL,
  `Price` decimal(10,0) DEFAULT NULL,
  `Volume` decimal(10,0) DEFAULT NULL,
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
  `UserName` varchar(50) NOT NULL,
  `Password` varchar(100) DEFAULT NULL,
  `PublicKey` varchar(100) DEFAULT NULL,
  `Email` varchar(50) DEFAULT NULL,
  `Language` varchar(100) DEFAULT NULL,
  `TimeZone` varchar(100) DEFAULT NULL,
  `AutoLogout` bigint(20) DEFAULT NULL,
  `LastLogin` datetime DEFAULT NULL,
  `ActivationKey` varchar(50) DEFAULT NULL,
  `Country` varchar(50) DEFAULT NULL,
  `State` varchar(50) DEFAULT NULL,
  `PhoneNumber` varchar(50) DEFAULT NULL,
  `Address1` varchar(100) DEFAULT NULL,
  `Address2` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`UserName`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
