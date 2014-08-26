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

/*Data for the table `balance` */

/*Data for the table `currencypair` */

insert  into `currencypair`(`CurrencyPairName`,`BaseCurrency`,`QuoteCurrency`) values ('XBTUSD','XBT','USD');

/*Data for the table `deposit` */

/*Data for the table `depositaddress` */

/*Data for the table `depositlimit` */

insert  into `depositlimit`(`Id`,`TierLevel`,`DailyLimit`,`MonthlyLimit`) values (1,'Tier 0',0,0),(2,'Tier 1',1000,5000);

/*Data for the table `fee` */

insert  into `fee`(`Id`,`CurrencyPair`,`PercentageFee`,`Amount`) values (1,'XBTUSD',0.3,1000),(2,'XBTUSD',0.29,2000),(3,'XBTUSD',0.28,3500),(4,'XBTUSD',0.27,5000),(5,'XBTUSD',0.26,6500),(6,'XBTUSD',0.25,8000),(7,'XBTUSD',0.24,10000),(8,'XBTUSD',0.23,12500),(9,'XBTUSD',0.22,15000),(10,'XBTUSD',0.21,17500),(11,'XBTUSD',0.2,20000),(12,'XBTUSD',0.19,25000),(13,'XBTUSD',0.18,30000),(14,'XBTUSD',0.17,40000),(15,'XBTUSD',0.16,50000),(16,'XBTUSD',0.15,60000),(17,'XBTUSD',0.14,80000),(18,'XBTUSD',0.13,100000),(19,'XBTUSD',0.12,125000),(20,'XBTUSD',0.11,150000),(21,'XBTUSD',0.1,200000),(22,'XBTUSD',0.09,350000),(23,'XBTUSD',0.08,500000),(24,'XBTUSD',0.07,750000),(25,'XBTUSD',0.06,1000000),(26,'BTCUSD',0.3,1000),(27,'BTCUSD',0.29,2000),(28,'BTCUSD',0.28,3500),(29,'BTCUSD',0.27,5000),(30,'BTCUSD',0.26,6500),(31,'BTCUSD',0.25,8000),(32,'BTCUSD',0.24,10000),(33,'BTCUSD',0.23,12500),(34,'BTCUSD',0.22,15000),(35,'BTCUSD',0.21,17500),(36,'BTCUSD',0.2,20000),(37,'BTCUSD',0.19,25000),(38,'BTCUSD',0.18,30000),(39,'BTCUSD',0.17,40000),(40,'BTCUSD',0.16,50000),(41,'BTCUSD',0.15,60000),(42,'BTCUSD',0.14,80000),(43,'BTCUSD',0.13,100000),(44,'BTCUSD',0.12,125000),(45,'BTCUSD',0.11,150000),(46,'BTCUSD',0.1,200000),(47,'BTCUSD',0.09,350000),(48,'BTCUSD',0.08,500000),(49,'BTCUSD',0.07,750000),(50,'BTCUSD',0.06,1000000);

/*Data for the table `ledger` */

/*Data for the table `ohlc` */

/*Data for the table `order` */

/*Data for the table `passwordcoderecord` */

/*Data for the table `pendingtransaction` */

/*Data for the table `permission` */

insert  into `permission`(`PermissionId`,`PermissionName`) values ('CO','Cancel Order'),('DF','Deposit Funds'),('PO','Place Order'),('QCOT','Query Closed Orders and Trades'),('QF','Query Funds'),('QLT','Query Ledger Entries'),('QOOT','Query Open Orders and Trades'),('WF','Withdraw Funds');

/*Data for the table `securitykeyspair` */

/*Data for the table `securitykeyspermission` */

/*Data for the table `tickerinfo` */

/*Data for the table `tier` */

insert  into `tier`(`TierLevel`,`TierName`) values ('Tier 0','Tier 0'),('Tier 1','Tier 1'),('Tier 2','Tier 2'),('Tier 3','Tier 3'),('Tier 4','Tier 4');

/*Data for the table `trade` */

/*Data for the table `user` */

/*Data for the table `userdocument` */

/*Data for the table `usertierlevelstatus` */

/*Data for the table `withdraw` */

/*Data for the table `withdrawaddress` */

/*Data for the table `withdrawfees` */

insert  into `withdrawfees`(`Id`,`Currency`,`MinimumAmount`,`Fee`) values (3,'BTC',0.01,0.001),(4,'USD',1,0.01);

/*Data for the table `withdrawlimit` */

insert  into `withdrawlimit`(`Id`,`TierLevel`,`DailyLimit`,`MonthlyLimit`) values (1,'Tier 0',0,0),(2,'Tier 1',1000,5000);

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
