CREATE TABLE `PriceStatsSecond` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(32) DEFAULT NULL,
  `LastPrice` decimal(10,2) DEFAULT 0 NOT NULL,
  `Bid` decimal(10,2) DEFAULT 0 NOT NULL,
  `BidQuantity` decimal(10,2) DEFAULT 0 NOT NULL,
  `Ask` decimal(10,2) DEFAULT 0 NOT NULL,
  `AskQuantity` decimal(10,2) DEFAULT 0 NOT NULL,
  `CreateTime` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`, `CreateTime`)
);

CREATE TABLE `PriceStatsMinute` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(32) DEFAULT NULL,
  `LastPrice` decimal(10,2) DEFAULT 0 NOT NULL,
  `Diff` decimal(10,2) DEFAULT 0 NOT NULL,
  `Bid` decimal(10,2) DEFAULT 0 NOT NULL,
  `BidQuantity` decimal(10,2) DEFAULT 0 NOT NULL,
  `Ask` decimal(10,2) DEFAULT 0 NOT NULL,
  `AskQuantity` decimal(10,2) DEFAULT 0 NOT NULL,
  `CreateTime` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`, `CreateTime`)
);

CREATE TABLE `Price` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(32) DEFAULT NULL,
  `OpenPirce` decimal(10,2) DEFAULT 0 NOT NULL,
  `HighPrice` decimal(10,2) DEFAULT 0 NOT NULL,
  `LowPrice` decimal(10,2) DEFAULT 0 NOT NULL,
  `CreateTime` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`)
);
