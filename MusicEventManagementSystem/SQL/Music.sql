-- MySQL dump 10.13  Distrib 8.0.43, for Win64 (x86_64)
--
-- Host: localhost    Database: musiceventdb
-- ------------------------------------------------------
-- Server version	9.4.0

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `__efmigrationshistory`
--

DROP TABLE IF EXISTS `__efmigrationshistory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProductVersion` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `__efmigrationshistory`
--

LOCK TABLES `__efmigrationshistory` WRITE;
/*!40000 ALTER TABLE `__efmigrationshistory` DISABLE KEYS */;
INSERT INTO `__efmigrationshistory` VALUES ('20251105092715_InitialModelSyncWithDB','8.0.4'),('20251105094054_InitialSetup','8.0.4'),('20251105100035_AddEndDateToMusicEvent','8.0.4'),('20251111135030_AddFirstLastNameToUser','8.0.4');
/*!40000 ALTER TABLE `__efmigrationshistory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetroleclaims`
--

DROP TABLE IF EXISTS `aspnetroleclaims`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetroleclaims` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `RoleId` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ClaimType` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ClaimValue` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  KEY `IX_AspNetRoleClaims_RoleId` (`RoleId`),
  CONSTRAINT `FK_AspNetRoleClaims_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `aspnetroles` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetroleclaims`
--

LOCK TABLES `aspnetroleclaims` WRITE;
/*!40000 ALTER TABLE `aspnetroleclaims` DISABLE KEYS */;
/*!40000 ALTER TABLE `aspnetroleclaims` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetroles`
--

DROP TABLE IF EXISTS `aspnetroles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetroles` (
  `Id` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Name` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `NormalizedName` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ConcurrencyStamp` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `RoleNameIndex` (`NormalizedName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetroles`
--

LOCK TABLES `aspnetroles` WRITE;
/*!40000 ALTER TABLE `aspnetroles` DISABLE KEYS */;
INSERT INTO `aspnetroles` VALUES ('a1a2a3a4-a1a2-a1a2-a1a2-a1a2a3a4a5a6','Admin','ADMIN','baedae43-ba32-11f0-86a8-40c2ba8a20b4');
/*!40000 ALTER TABLE `aspnetroles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetuserclaims`
--

DROP TABLE IF EXISTS `aspnetuserclaims`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetuserclaims` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `UserId` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ClaimType` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ClaimValue` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  KEY `IX_AspNetUserClaims_UserId` (`UserId`),
  CONSTRAINT `FK_AspNetUserClaims_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetuserclaims`
--

LOCK TABLES `aspnetuserclaims` WRITE;
/*!40000 ALTER TABLE `aspnetuserclaims` DISABLE KEYS */;
/*!40000 ALTER TABLE `aspnetuserclaims` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetuserlogins`
--

DROP TABLE IF EXISTS `aspnetuserlogins`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetuserlogins` (
  `LoginProvider` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProviderKey` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProviderDisplayName` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `UserId` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`LoginProvider`,`ProviderKey`),
  KEY `IX_AspNetUserLogins_UserId` (`UserId`),
  CONSTRAINT `FK_AspNetUserLogins_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetuserlogins`
--

LOCK TABLES `aspnetuserlogins` WRITE;
/*!40000 ALTER TABLE `aspnetuserlogins` DISABLE KEYS */;
/*!40000 ALTER TABLE `aspnetuserlogins` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetuserroles`
--

DROP TABLE IF EXISTS `aspnetuserroles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetuserroles` (
  `UserId` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `RoleId` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`UserId`,`RoleId`),
  KEY `IX_AspNetUserRoles_RoleId` (`RoleId`),
  CONSTRAINT `FK_AspNetUserRoles_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `aspnetroles` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_AspNetUserRoles_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetuserroles`
--

LOCK TABLES `aspnetuserroles` WRITE;
/*!40000 ALTER TABLE `aspnetuserroles` DISABLE KEYS */;
INSERT INTO `aspnetuserroles` VALUES ('06447d89-1cb1-49f8-81c5-7920589a54fc','a1a2a3a4-a1a2-a1a2-a1a2-a1a2a3a4a5a6');
/*!40000 ALTER TABLE `aspnetuserroles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetusers`
--

DROP TABLE IF EXISTS `aspnetusers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetusers` (
  `Id` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `UserName` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `NormalizedUserName` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Email` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `NormalizedEmail` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `EmailConfirmed` tinyint(1) NOT NULL,
  `PasswordHash` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `SecurityStamp` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ConcurrencyStamp` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `PhoneNumber` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `PhoneNumberConfirmed` tinyint(1) NOT NULL,
  `TwoFactorEnabled` tinyint(1) NOT NULL,
  `LockoutEnd` datetime(6) DEFAULT NULL,
  `LockoutEnabled` tinyint(1) NOT NULL,
  `AccessFailedCount` int NOT NULL,
  `FirstName` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT '',
  `LastName` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UserNameIndex` (`NormalizedUserName`),
  KEY `EmailIndex` (`NormalizedEmail`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetusers`
--

LOCK TABLES `aspnetusers` WRITE;
/*!40000 ALTER TABLE `aspnetusers` DISABLE KEYS */;
INSERT INTO `aspnetusers` VALUES ('06447d89-1cb1-49f8-81c5-7920589a54fc','thanhkien2900@gmail.com','THANHKIEN2900@GMAIL.COM','thanhkien2900@gmail.com','THANHKIEN2900@GMAIL.COM',0,'AQAAAAIAAYagAAAAEF4UvhHXTwPFt/DKzZKI1xfQi0u6EKTsZoMu8P4gU1EKpC/cqNoLibWhWkQBzPkDyQ==','5FBGF4ME4VIUQGSDZDABWVDCNAKJMCIF','55917c54-9831-41b5-8b79-941cb39d0d60',NULL,0,0,NULL,1,0,'Kiên','Nguyễn '),('1a85c05e-243b-4885-908e-64e6bb184723','nguyenkhoi060318@gmail.com','NGUYENKHOI060318@GMAIL.COM','nguyenkhoi060318@gmail.com','NGUYENKHOI060318@GMAIL.COM',1,'AQAAAAIAAYagAAAAEJhG/OJDVguCKZ8N7enOPGJYc8jfSaes5zFlxDSdiqx4kVsCJvojfH5MM7RtBjoghg==','RPM6I4QOHPPWL5JS7SU676GVY7SAFDCK','b9f6ae6b-3141-42e5-8b12-528368650551',NULL,0,0,NULL,1,0,'Nguyên','Lê'),('a0464764-bf43-401a-866b-ce94783360ac','kienvn2k6@gmail.com','KIENVN2K6@GMAIL.COM','kienvn2k6@gmail.com','KIENVN2K6@GMAIL.COM',0,'AQAAAAIAAYagAAAAEGW1K6IDsznNCX6hTnymoS61GCuqtojJoDFaw7kJJ9J8t1K1xS5fQ6xh9wxbTa13pg==','W5G6SZJH2AL3CB2YPLW3KN6J2ALXCKB2','cb4e1af8-b850-4226-884f-5e07948183e6',NULL,0,0,NULL,1,0,'',''),('f4b1251f-3e28-4c55-9215-38976a405f24','thanhkien2883@gmail.com','THANHKIEN2883@GMAIL.COM','thanhkien2883@gmail.com','THANHKIEN2883@GMAIL.COM',0,'AQAAAAIAAYagAAAAEMYRy7JquKaCD7AeB4QmzrRiKGtev/BRUFUdqTcwXjFfy1Gr7sBAgwZMNwpiC7Yjlw==','AILN4BZERYN4DWGBLZZUJMQX5CYJWUHL','b242570f-2f9e-4191-b330-e5e596d12949',NULL,0,0,NULL,1,0,'','');
/*!40000 ALTER TABLE `aspnetusers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspnetusertokens`
--

DROP TABLE IF EXISTS `aspnetusertokens`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `aspnetusertokens` (
  `UserId` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `LoginProvider` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Value` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`UserId`,`LoginProvider`,`Name`),
  CONSTRAINT `FK_AspNetUserTokens_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspnetusertokens`
--

LOCK TABLES `aspnetusertokens` WRITE;
/*!40000 ALTER TABLE `aspnetusertokens` DISABLE KEYS */;
INSERT INTO `aspnetusertokens` VALUES ('06447d89-1cb1-49f8-81c5-7920589a54fc','[AspNetUserStore]','AuthenticatorKey','6SIWUKS5ZGNGEHCTJPYVIMVK56ZEYB4H');
/*!40000 ALTER TABLE `aspnetusertokens` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `eventregistrations`
--

DROP TABLE IF EXISTS `eventregistrations`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `eventregistrations` (
  `RegistrationID` int NOT NULL AUTO_INCREMENT,
  `RegistrationDate` datetime(6) NOT NULL,
  `Status` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `PaymentStatus` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `TotalPrice` decimal(18,2) NOT NULL,
  `EventID` int NOT NULL,
  `UserID` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `PricingTierID` int NOT NULL,
  `ConfirmationCode` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  PRIMARY KEY (`RegistrationID`),
  KEY `IX_EventRegistrations_EventID` (`EventID`),
  KEY `IX_EventRegistrations_PricingTierID` (`PricingTierID`),
  KEY `IX_EventRegistrations_UserID` (`UserID`),
  CONSTRAINT `FK_EventRegistrations_AspNetUsers_UserID` FOREIGN KEY (`UserID`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_EventRegistrations_MusicEvents_EventID` FOREIGN KEY (`EventID`) REFERENCES `musicevents` (`EventID`) ON DELETE RESTRICT,
  CONSTRAINT `FK_EventRegistrations_PricingTiers_PricingTierID` FOREIGN KEY (`PricingTierID`) REFERENCES `pricingtiers` (`PricingTierID`) ON DELETE RESTRICT
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `eventregistrations`
--

LOCK TABLES `eventregistrations` WRITE;
/*!40000 ALTER TABLE `eventregistrations` DISABLE KEYS */;
INSERT INTO `eventregistrations` VALUES (1,'2025-11-05 15:26:10.087683','Pending','Unpaid',500000.00,5,'06447d89-1cb1-49f8-81c5-7920589a54fc',6,NULL),(2,'2025-11-05 15:32:16.509070','Pending','Unpaid',500000.00,5,'06447d89-1cb1-49f8-81c5-7920589a54fc',6,NULL),(3,'2025-11-05 15:33:43.946307','Pending','Unpaid',525000.00,5,'06447d89-1cb1-49f8-81c5-7920589a54fc',6,NULL),(4,'2025-11-06 05:55:36.074279','Pending','Unpaid',50000.00,5,'a0464764-bf43-401a-866b-ce94783360ac',5,NULL),(5,'2025-11-06 06:06:53.321010','Pending','Confirmed',2000000.00,4,'06447d89-1cb1-49f8-81c5-7920589a54fc',4,'0ac301c4-0f09-40cf-b4b2-d49e86ed01d4'),(6,'2025-11-06 06:19:32.352155','Pending','Unpaid',2000000.00,4,'a0464764-bf43-401a-866b-ce94783360ac',4,NULL),(7,'2025-11-11 09:36:10.885785','Confirmed','Unpaid',2000000.00,4,'f4b1251f-3e28-4c55-9215-38976a405f24',4,NULL),(8,'2025-11-11 13:55:10.712654','Pending','Confirmed',0.00,4,'1a85c05e-243b-4885-908e-64e6bb184723',3,'0a69ef47-c02d-4368-8ac9-e69803836287'),(9,'2025-11-13 03:54:18.585413','Confirmed','Confirmed',4000000.00,4,'06447d89-1cb1-49f8-81c5-7920589a54fc',4,'a9c594a9-c1a0-418c-9bda-08234ece4cf8'),(10,'2025-11-13 04:01:45.026774','Confirmed','Confirmed',7000000.00,4,'06447d89-1cb1-49f8-81c5-7920589a54fc',3,'3de57e9d-7975-4fe6-bcfd-e6d0dfa1b461'),(11,'2025-11-13 04:06:19.377763','Confirmed','Confirmed',13500000.00,3,'06447d89-1cb1-49f8-81c5-7920589a54fc',2,'95981e87-3718-4a06-8259-f2d7ff35f9c7'),(12,'2025-11-13 04:14:54.167410','Confirmed','Confirmed',15000000.00,3,'06447d89-1cb1-49f8-81c5-7920589a54fc',2,'e2275122-268e-4cc9-a3a8-c436aa71c3ab'),(13,'2025-11-18 08:43:31.771556','Confirmed','Confirmed',5600000.00,4,'06447d89-1cb1-49f8-81c5-7920589a54fc',3,'fb4093f3-bb00-4c5b-89e1-da57fd283144');
/*!40000 ALTER TABLE `eventregistrations` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `musicevents`
--

DROP TABLE IF EXISTS `musicevents`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `musicevents` (
  `EventID` int NOT NULL AUTO_INCREMENT,
  `EventName` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `EventDate` datetime(6) NOT NULL,
  `Location` varchar(300) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Genre` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Description` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `MaxAttendees` int DEFAULT NULL,
  `CurrentAttendees` int NOT NULL,
  `RegistrationDeadline` datetime(6) DEFAULT NULL,
  `ImageUrl` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `CreatedByUserID` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `CreatedDate` datetime(6) NOT NULL,
  `LastModifiedDate` datetime(6) DEFAULT NULL,
  `IsPublished` tinyint(1) NOT NULL,
  `EndDate` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`EventID`),
  KEY `IX_MusicEvents_CreatedByUserID` (`CreatedByUserID`),
  CONSTRAINT `FK_MusicEvents_AspNetUsers_CreatedByUserID` FOREIGN KEY (`CreatedByUserID`) REFERENCES `aspnetusers` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `musicevents`
--

LOCK TABLES `musicevents` WRITE;
/*!40000 ALTER TABLE `musicevents` DISABLE KEYS */;
INSERT INTO `musicevents` VALUES (2,'Testing','2025-11-22 17:50:14.100000','1121 Ngo Quyen','EDM','The boat was like a pea floating in a great bowl of blue soup. ',NULL,0,NULL,'https://plus.unsplash.com/premium_photo-1664474619075-644dd191935f?ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxzZWFyY2h8MXx8aW1hZ2V8ZW58MHx8MHx8fDA%3D&fm=jpg&q=60&w=3000','06447d89-1cb1-49f8-81c5-7920589a54fc','2025-11-05 10:46:47.000000','2025-11-05 10:47:13.406291',1,NULL),(3,'BIGGEST EDM MUSIC FESTIVAL - LIGHT DANCE','2025-12-20 20:00:00.000000','My Dinh Stadium, Hanoi','EDM','The biggest EDM event of the year featuring world-renowned DJs. An explosive night with spectacular sound and laser lights. Don\'t miss the opportunity to meet Martin Garrix and Alan Walker!',20000,0,NULL,'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcStcYt1yuor-sTThbPLJzTBJPS1QmcggZJMig&s','06447d89-1cb1-49f8-81c5-7920589a54fc','2025-11-05 21:56:49.000000','2025-11-13 06:07:34.932969',1,NULL),(4,'ACOUSTIC NIGHT - CHILL WITH STARS','2025-11-25 19:30:00.000000','We Tea House, Ho Chi Minh City','Acoustic, Pop','A warm and intimate music night with famous V-Pop singers such as Ha Anh Tuan and Vu. Enjoy natural and familiar music.',200,0,NULL,'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTHoMev2VPeG81wrzcm0l7YanrC8N8rkApsCQ&s','06447d89-1cb1-49f8-81c5-7920589a54fc','2025-11-05 21:56:58.000000','2025-11-13 06:04:54.048140',1,NULL),(5,'Testing','2025-11-12 22:02:06.466000','11221 Ngo Quyen','EDM','A test event',NULL,0,NULL,NULL,'06447d89-1cb1-49f8-81c5-7920589a54fc','2025-11-05 15:04:01.000000','2025-11-05 15:33:32.825608',1,NULL),(7,'Drifting Dance','2025-11-28 15:42:42.500000','128 Hải Phòng','EDM',NULL,1500,0,NULL,NULL,'06447d89-1cb1-49f8-81c5-7920589a54fc','2025-11-18 08:45:40.315664',NULL,1,NULL);
/*!40000 ALTER TABLE `musicevents` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `pricingtiers`
--

DROP TABLE IF EXISTS `pricingtiers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `pricingtiers` (
  `PricingTierID` int NOT NULL AUTO_INCREMENT,
  `TierName` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Price` decimal(18,2) NOT NULL,
  `Description` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `AvailableTickets` int DEFAULT NULL,
  `SoldTickets` int NOT NULL,
  `EventID` int NOT NULL,
  PRIMARY KEY (`PricingTierID`),
  KEY `IX_PricingTiers_EventID` (`EventID`),
  CONSTRAINT `FK_PricingTiers_MusicEvents_EventID` FOREIGN KEY (`EventID`) REFERENCES `musicevents` (`EventID`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `pricingtiers`
--

LOCK TABLES `pricingtiers` WRITE;
/*!40000 ALTER TABLE `pricingtiers` DISABLE KEYS */;
INSERT INTO `pricingtiers` VALUES (1,'Standard Ticket (GA)',500000.00,'Free standing area.',15000,0,3),(2,'VIP Ticket',1500000.00,'Private area near the stage, includes 1 complimentary drink.',5000,19,3),(3,'Standard Ticket',700000.00,'Includes 1 beverage of your choice.',150,18,4),(4,'Sofa Ticket (2 people)',2000000.00,'Sofa seating at the best location, includes 2 beverages and 1 fruit platter.',25,5,4),(5,'Standard',50000.00,NULL,150,1,5),(6,'SVIP',525000.00,NULL,20,1,5),(7,'Standard Ticket',100000.00,NULL,1400,0,7),(8,'VIP',10000000.00,NULL,100,0,7);
/*!40000 ALTER TABLE `pricingtiers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `registrationdata`
--

DROP TABLE IF EXISTS `registrationdata`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `registrationdata` (
  `DataID` bigint NOT NULL AUTO_INCREMENT,
  `FieldValue` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `RegistrationID` int NOT NULL,
  `FieldID` int NOT NULL,
  PRIMARY KEY (`DataID`),
  KEY `IX_RegistrationData_FieldID` (`FieldID`),
  KEY `IX_RegistrationData_RegistrationID` (`RegistrationID`),
  CONSTRAINT `FK_RegistrationData_EventRegistrations_RegistrationID` FOREIGN KEY (`RegistrationID`) REFERENCES `eventregistrations` (`RegistrationID`) ON DELETE CASCADE,
  CONSTRAINT `FK_RegistrationData_RequiredFields_FieldID` FOREIGN KEY (`FieldID`) REFERENCES `requiredfields` (`FieldID`) ON DELETE RESTRICT
) ENGINE=InnoDB AUTO_INCREMENT=41 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `registrationdata`
--

LOCK TABLES `registrationdata` WRITE;
/*!40000 ALTER TABLE `registrationdata` DISABLE KEYS */;
INSERT INTO `registrationdata` VALUES (1,'Kiên',1,8),(2,'0932423423',1,12),(3,'32',1,9),(4,'Idk',1,10),(5,'Trưng',2,8),(6,'0932423423',2,12),(7,'32',2,9),(8,'Idk',2,10),(9,'Kiên',3,8),(10,'0932423423',3,12),(11,'Hiếu ',4,8),(12,'0932423423',4,12),(13,'32',4,9),(14,'Idk',4,10),(15,'Kiên Nguyễn ',5,5),(16,'thanhkien2900@gmail.com',5,6),(17,'Ko có',5,7),(18,'Kiên Nguyễn ',6,5),(19,'kienvn2k6@gmail.com',6,6),(20,'ko',6,7),(21,'Kiên Nguyễn ',7,5),(22,'thanhkien2883@gmail.com',7,6),(23,'None',7,7),(24,'Kiên Nguyễn ',9,5),(25,'thanhkien2883@gmail.com',9,6),(26,'None',9,7),(27,'Huy Ngo',10,5),(28,'kienvn2k6@gmail.com',10,6),(29,'None',10,7),(30,'Dang Phan',11,1),(31,'thanhkien2900@gmail.com',11,2),(32,'0934234234',11,3),(33,'true',11,4),(34,'Dang Phang',12,1),(35,'thanhkien2900@gmail.com',12,2),(36,'0934233324',12,3),(37,'true',12,4),(38,'Kien ',13,5),(39,'thanhkien2883@gmail.com',13,6),(40,'None',13,7);
/*!40000 ALTER TABLE `registrationdata` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `requiredfields`
--

DROP TABLE IF EXISTS `requiredfields`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `requiredfields` (
  `FieldID` int NOT NULL AUTO_INCREMENT,
  `FieldName` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `FieldType` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `IsRequired` tinyint(1) NOT NULL,
  `DisplayOrder` int NOT NULL,
  `EventID` int NOT NULL,
  PRIMARY KEY (`FieldID`),
  KEY `IX_RequiredFields_EventID` (`EventID`),
  CONSTRAINT `FK_RequiredFields_MusicEvents_EventID` FOREIGN KEY (`EventID`) REFERENCES `musicevents` (`EventID`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=17 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `requiredfields`
--

LOCK TABLES `requiredfields` WRITE;
/*!40000 ALTER TABLE `requiredfields` DISABLE KEYS */;
INSERT INTO `requiredfields` VALUES (1,'Full Name','Text',1,1,3),(2,'Email','Email',1,2,3),(3,'Phone Number','Tel',1,3,3),(4,'Would you like to receive information from the organizers?','Checkbox',0,4,3),(5,'Full Name (Representative)','Text',1,1,4),(6,'Confirmation Email','Email',1,2,4),(7,'Special Requests (vegetarian, allergies...)','TextArea',0,3,4),(8,'Name','Text',1,0,5),(9,'Age','Text',0,1,5),(10,'Size','Tel',0,2,5),(12,'Phone Number','Tel',1,0,5),(15,'Full Name','Text',1,1,7),(16,'Email','Email',1,2,7);
/*!40000 ALTER TABLE `requiredfields` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `userpreferences`
--

DROP TABLE IF EXISTS `userpreferences`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `userpreferences` (
  `PreferenceID` int NOT NULL AUTO_INCREMENT,
  `PreferredGenres` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `PreferredLocations` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `MaxPriceRange` decimal(18,2) DEFAULT NULL,
  `NotificationEnabled` tinyint(1) NOT NULL,
  `EmailReminderEnabled` tinyint(1) NOT NULL,
  `ReminderHoursBefore` int NOT NULL,
  `LastUpdated` datetime(6) NOT NULL,
  `UserID` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`PreferenceID`),
  UNIQUE KEY `IX_UserPreferences_UserID` (`UserID`),
  CONSTRAINT `FK_UserPreferences_AspNetUsers_UserID` FOREIGN KEY (`UserID`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `userpreferences`
--

LOCK TABLES `userpreferences` WRITE;
/*!40000 ALTER TABLE `userpreferences` DISABLE KEYS */;
/*!40000 ALTER TABLE `userpreferences` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-11-24 19:56:16
