DROP SCHEMA IF EXISTS gymxanhpt;
CREATE SCHEMA gymxanhpt;
USE gymxanhpt;

DROP TABLE IF EXISTS `member`;
CREATE TABLE `member` (
  `MemberId` int(11) NOT NULL AUTO_INCREMENT,
  `MemberUUId` varchar(36) NOT NULL,
  `UserName` varchar(20) NOT NULL,
  `FullName` varchar(45) NOT NULL,
  `DateOfBirth` date NOT NULL,
  `Email` varchar(45) NOT NULL,
  `PhoneNumber` varchar(11) NOT NULL,
  `Gender` tinyint(4) NOT NULL,
  `Address` varchar(45) NOT NULL,
  `JoinedDate` date NOT NULL,
  `ExpiredDate` date DEFAULT NULL,
  `Status` varchar(45) DEFAULT NULL,
  `CreatedDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedDate` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `IsDeleted` tinyint(4) NOT NULL DEFAULT '0',
  `ImageUrl` blob,
  `StudioID` int(11) NOT NULL,
  PRIMARY KEY (`MemberId`),
  UNIQUE KEY `Email_UNIQUE` (`Email`),
  KEY `fk_studioid` (`StudioID`),
  CONSTRAINT `fk_studioid` FOREIGN KEY (`StudioID`) REFERENCES `studio` (`studioid`)
) ENGINE=InnoDB AUTO_INCREMENT=44 DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `studio`;
CREATE TABLE `studio` (
  `StudioID` int(11) NOT NULL AUTO_INCREMENT,
  `StudioName` varchar(45) NOT NULL,
  `StudioAddress` varchar(100) NOT NULL,
  `StudioPhone` varchar(13) NOT NULL,
  `StudioPic` blob,
  PRIMARY KEY (`StudioID`)
) ENGINE=InnoDB AUTO_INCREMENT=18 DEFAULT CHARSET=utf8;

