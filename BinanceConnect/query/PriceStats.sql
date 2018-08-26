CREATE TABLE `PriceStats2` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(32) DEFAULT NULL,
  `Ask` decimal(10,2) DEFAULT 0 NOT NULL,
  `Bid` decimal(10,2) DEFAULT 0 NOT NULL,
  `CreateTime` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`, `CreateTime`)
);

CREATE TABLE `PriceStatsMinutes` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(32) DEFAULT NULL,
  `Price` decimal(10,2) DEFAULT 0 NOT NULL,
  `Diff` decimal(10,3) DEFAULT 0 NOT NULL,
  `CreateTime` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`, `CreateTime`)
);

