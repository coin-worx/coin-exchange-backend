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

insert  into `currencypair`(`CurrencyPairName`,`BaseCurrency`,`QuoteCurrency`) values ('BTCLTC','BTC','LTC');

/*Data for the table `deposit` */

/*Data for the table `depositaddress` */

/*Data for the table `depositlimit` */

insert  into `depositlimit`(`Id`,`TierLevel`,`DailyLimit`,`MonthlyLimit`) values (1,'Tier 0',0,0),(2,'Tier 1',2,10),(3,'Tier 2',4,20),(4,'Tier 3',40,200),(5,'Tier 4',200,1000);

/*Data for the table `fee` */

insert  into `fee`(`Id`,`CurrencyPair`,`PercentageFee`,`Amount`) values (1,'BTCLTC',0.1,1000),(2,'BTCLTC',0.9,2000),(3,'BTCLTC',0.8,3000),(4,'BTCLTC',0.7,5000),(5,'BTCLTC',0.6,8000),(6,'BTCLTC',0.5,10000),(7,'BTCLTC',0.4,15000);

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

insert  into `withdrawfees`(`Id`,`Currency`,`MinimumAmount`,`Fee`) values (3,'BTC',0.001,0.0001),(4,'LTC',0.001,0.0001);

/*Data for the table `withdrawlimit` */

insert  into `withdrawlimit`(`Id`,`TierLevel`,`DailyLimit`,`MonthlyLimit`) values (1,'Tier 0',0,0),(2,'Tier 1',2,10),(3,'Tier 2',4,20),(4,'Tier 3',40,200),(5,'Tier 4',200,1000);

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
