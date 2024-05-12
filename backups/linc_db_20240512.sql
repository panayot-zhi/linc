-- MySQL dump 10.13  Distrib 8.0.34, for Win64 (x86_64)
--
-- Host: localhost    Database: linc_db
-- ------------------------------------------------------
-- Server version	8.0.36-0ubuntu0.22.04.1

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
-- Table structure for table `__EFMigrationsHistory`
--

DROP TABLE IF EXISTS `__EFMigrationsHistory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `__EFMigrationsHistory` (
  `migration_id` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `product_version` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`migration_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `__EFMigrationsHistory`
--

LOCK TABLES `__EFMigrationsHistory` WRITE;
/*!40000 ALTER TABLE `__EFMigrationsHistory` DISABLE KEYS */;
INSERT INTO `__EFMigrationsHistory` VALUES ('20240119215117_InitialDatabase','6.0.23'),('20240119215159_SeedDatabase','6.0.23'),('20240126073240_StringResourceEditedTrace','6.0.23'),('20240210195101_UserSubscribedFlag','6.0.23'),('20240225162646_IssueAndDossier','6.0.23'),('20240225220943_ElaborateDescriptions','6.0.23'),('20240317145229_IndexOnSourceNames','6.0.23'),('20240317164223_LastSourcePage','6.0.23'),('20240404195053_AddSourcePdf','6.0.23'),('20240409042419_MandatoryPdfId','6.0.23'),('20240409045220_RobustSources','6.0.23'),('20240409050957_CorrectName','6.0.23'),('20240413175738_EntityJournals','6.0.23'),('20240413181944_IndexAndUserTracing','6.0.23'),('20240413200445_UseIntegerForStatus','6.0.23'),('20240413211608_DossierAssignment','6.0.23'),('20240414073329_RemoveEditedBy','6.0.23'),('20240414080210_UserFlags','6.0.23'),('20240414080528_IncludeAutoProperties','6.0.23'),('20240414100638_JournalMessageArguments','6.0.23'),('20240414131644_DossierReviews','6.0.23'),('20240420071454_RemoveAuthorNames','6.0.23'),('20240420074336_ComputedAuthorNamesColumn','6.0.23'),('20240420074843_IndexSourceColumns','6.0.23'),('20240420093056_LastUserLoginTime','6.0.23'),('20240420215129_DossierAuthorAssignment','6.0.23'),('20240430165530_ReleaseDate','6.0.23');
/*!40000 ALTER TABLE `__EFMigrationsHistory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `application_document_application_dossier`
--

DROP TABLE IF EXISTS `application_document_application_dossier`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `application_document_application_dossier` (
  `documents_id` int NOT NULL,
  `dossiers_id` int NOT NULL,
  PRIMARY KEY (`documents_id`,`dossiers_id`),
  KEY `ix_application_document_application_dossier_dossiers_id` (`dossiers_id`),
  CONSTRAINT `fk_application_document_application_dossier_documents_documents` FOREIGN KEY (`documents_id`) REFERENCES `documents` (`id`) ON DELETE CASCADE,
  CONSTRAINT `fk_application_document_application_dossier_dossiers_dossiers_id` FOREIGN KEY (`dossiers_id`) REFERENCES `dossiers` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `application_document_application_dossier`
--

LOCK TABLES `application_document_application_dossier` WRITE;
/*!40000 ALTER TABLE `application_document_application_dossier` DISABLE KEYS */;
/*!40000 ALTER TABLE `application_document_application_dossier` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `application_document_application_issue`
--

DROP TABLE IF EXISTS `application_document_application_issue`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `application_document_application_issue` (
  `files_id` int NOT NULL,
  `issues_id` int NOT NULL,
  PRIMARY KEY (`files_id`,`issues_id`),
  KEY `ix_application_document_application_issue_issues_id` (`issues_id`),
  CONSTRAINT `fk_application_document_application_issue_documents_files_id` FOREIGN KEY (`files_id`) REFERENCES `documents` (`id`) ON DELETE CASCADE,
  CONSTRAINT `fk_application_document_application_issue_issues_issues_id` FOREIGN KEY (`issues_id`) REFERENCES `issues` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `application_document_application_issue`
--

LOCK TABLES `application_document_application_issue` WRITE;
/*!40000 ALTER TABLE `application_document_application_issue` DISABLE KEYS */;
INSERT INTO `application_document_application_issue` VALUES (1,2),(2,2),(3,2),(4,2),(5,2),(6,2),(7,2),(8,2);
/*!40000 ALTER TABLE `application_document_application_issue` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `asp_net_role_claims`
--

DROP TABLE IF EXISTS `asp_net_role_claims`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `asp_net_role_claims` (
  `id` int NOT NULL AUTO_INCREMENT,
  `role_id` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `claim_type` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `claim_value` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`id`),
  KEY `ix_asp_net_role_claims_role_id` (`role_id`),
  CONSTRAINT `fk_asp_net_role_claims_asp_net_roles_role_id` FOREIGN KEY (`role_id`) REFERENCES `asp_net_roles` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `asp_net_role_claims`
--

LOCK TABLES `asp_net_role_claims` WRITE;
/*!40000 ALTER TABLE `asp_net_role_claims` DISABLE KEYS */;
/*!40000 ALTER TABLE `asp_net_role_claims` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `asp_net_roles`
--

DROP TABLE IF EXISTS `asp_net_roles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `asp_net_roles` (
  `id` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `name` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `normalized_name` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `concurrency_stamp` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`id`),
  UNIQUE KEY `RoleNameIndex` (`normalized_name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `asp_net_roles`
--

LOCK TABLES `asp_net_roles` WRITE;
/*!40000 ALTER TABLE `asp_net_roles` DISABLE KEYS */;
INSERT INTO `asp_net_roles` VALUES ('00000000-0000-0000-0000-000000000000','ADMINISTRATOR','ADMINISTRATOR','000000000000-0000-0000-0000-00000000'),('05cbe4c7-108e-40bc-bee7-65438875026e','EDITOR','EDITOR','e62057883456-7eeb-cb04-e801-7c4ebc50'),('5e1199d7-7725-4900-aa34-5496365bf5a0','HEAD_EDITOR','HEAD_EDITOR','0a5fb5636945-43aa-0094-5277-7d9911e5'),('6b1acea8-2d26-4c82-b6ad-7281b7d621ae','USER_PLUS','USER_PLUS','ea126d7b1827-da6b-28c4-62d2-8aeca1b6'),('90667439-9058-4956-96e6-d23bac481443','USER','USER','344184cab32d-6e69-6594-8509-93476609');
/*!40000 ALTER TABLE `asp_net_roles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `asp_net_user_claims`
--

DROP TABLE IF EXISTS `asp_net_user_claims`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `asp_net_user_claims` (
  `id` int NOT NULL AUTO_INCREMENT,
  `user_id` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `claim_type` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `claim_value` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`id`),
  KEY `ix_asp_net_user_claims_user_id` (`user_id`),
  CONSTRAINT `fk_asp_net_user_claims_asp_net_users_user_id` FOREIGN KEY (`user_id`) REFERENCES `asp_net_users` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `asp_net_user_claims`
--

LOCK TABLES `asp_net_user_claims` WRITE;
/*!40000 ALTER TABLE `asp_net_user_claims` DISABLE KEYS */;
/*!40000 ALTER TABLE `asp_net_user_claims` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `asp_net_user_logins`
--

DROP TABLE IF EXISTS `asp_net_user_logins`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `asp_net_user_logins` (
  `login_provider` varchar(127) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `provider_key` varchar(127) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `provider_display_name` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `user_id` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`login_provider`,`provider_key`),
  KEY `ix_asp_net_user_logins_user_id` (`user_id`),
  CONSTRAINT `fk_asp_net_user_logins_asp_net_users_user_id` FOREIGN KEY (`user_id`) REFERENCES `asp_net_users` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `asp_net_user_logins`
--

LOCK TABLES `asp_net_user_logins` WRITE;
/*!40000 ALTER TABLE `asp_net_user_logins` DISABLE KEYS */;
/*!40000 ALTER TABLE `asp_net_user_logins` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `asp_net_user_roles`
--

DROP TABLE IF EXISTS `asp_net_user_roles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `asp_net_user_roles` (
  `user_id` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `role_id` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`user_id`,`role_id`),
  KEY `ix_asp_net_user_roles_role_id` (`role_id`),
  CONSTRAINT `fk_asp_net_user_roles_asp_net_roles_role_id` FOREIGN KEY (`role_id`) REFERENCES `asp_net_roles` (`id`) ON DELETE CASCADE,
  CONSTRAINT `fk_asp_net_user_roles_asp_net_users_user_id` FOREIGN KEY (`user_id`) REFERENCES `asp_net_users` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `asp_net_user_roles`
--

LOCK TABLES `asp_net_user_roles` WRITE;
/*!40000 ALTER TABLE `asp_net_user_roles` DISABLE KEYS */;
INSERT INTO `asp_net_user_roles` VALUES ('00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000000'),('2c63936e-0ada-4dd8-a844-d516c21913f1','90667439-9058-4956-96e6-d23bac481443'),('66a47e24-4673-486b-be52-f42ab252e308','90667439-9058-4956-96e6-d23bac481443'),('84f14f53-c633-40af-aaf6-a1c3a9d199b9','90667439-9058-4956-96e6-d23bac481443');
/*!40000 ALTER TABLE `asp_net_user_roles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `asp_net_user_tokens`
--

DROP TABLE IF EXISTS `asp_net_user_tokens`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `asp_net_user_tokens` (
  `user_id` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `login_provider` varchar(127) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `name` varchar(127) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `value` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`user_id`,`login_provider`,`name`),
  CONSTRAINT `fk_asp_net_user_tokens_asp_net_users_user_id` FOREIGN KEY (`user_id`) REFERENCES `asp_net_users` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `asp_net_user_tokens`
--

LOCK TABLES `asp_net_user_tokens` WRITE;
/*!40000 ALTER TABLE `asp_net_user_tokens` DISABLE KEYS */;
/*!40000 ALTER TABLE `asp_net_user_tokens` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `asp_net_users`
--

DROP TABLE IF EXISTS `asp_net_users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `asp_net_users` (
  `id` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `description` varchar(1024) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `first_name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `last_name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `display_name_type` int NOT NULL,
  `display_email` tinyint(1) NOT NULL,
  `avatar_type` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `facebook_avatar_path` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `twitter_avatar_path` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `google_avatar_path` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `internal_avatar_path` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `last_updated` datetime(6) NOT NULL,
  `date_created` datetime(6) NOT NULL,
  `user_name` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `normalized_user_name` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `email` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `normalized_email` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `email_confirmed` tinyint(1) NOT NULL,
  `password_hash` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `security_stamp` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `concurrency_stamp` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `phone_number` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `phone_number_confirmed` tinyint(1) NOT NULL,
  `two_factor_enabled` tinyint(1) NOT NULL,
  `lockout_end` datetime(6) DEFAULT NULL,
  `lockout_enabled` tinyint(1) NOT NULL,
  `access_failed_count` int NOT NULL,
  `subscribed` tinyint(1) NOT NULL DEFAULT '0',
  `is_author` tinyint(1) NOT NULL DEFAULT '0',
  `is_reviewer` tinyint(1) NOT NULL DEFAULT '0',
  `last_login` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `UserNameIndex` (`normalized_user_name`),
  KEY `EmailIndex` (`normalized_email`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `asp_net_users`
--

LOCK TABLES `asp_net_users` WRITE;
/*!40000 ALTER TABLE `asp_net_users` DISABLE KEYS */;
INSERT INTO `asp_net_users` VALUES ('00000000-0000-0000-0000-000000000000','System administrator. / Администратор на системата.','Panayot','Ivanov',2,1,'Gravatar',NULL,NULL,NULL,NULL,'2024-05-08 18:00:06.237597','2024-01-01 00:00:00.000000','p.ivanov','P.IVANOV','admin-linc@uni-plovdiv.bg','ADMIN-LINC@UNI-PLOVDIV.BG',1,'AQAAAAEAACcQAAAAEHfAOnTfE/c7aXuqmS20qR+ZBJhqk54z7mswTFVGlRLUaltQdbya8rr2QSdVW44Ixg==','BETIC7D2TSZ6RCJWB4F75AQ7DAY5QL7O','9611777a-14d8-460e-a747-7f09ab0df0e3',NULL,0,0,NULL,0,0,0,0,0,'2024-05-08 18:00:05.935333'),('2c63936e-0ada-4dd8-a844-d516c21913f1',NULL,'Richmond','Mathewson',0,0,'Default',NULL,NULL,NULL,NULL,'2024-04-24 11:12:06.770088','2024-04-23 05:07:54.260242','richmond','RICHMOND','richmondmathewson@gmail.com','RICHMONDMATHEWSON@GMAIL.COM',1,'AQAAAAEAACcQAAAAEBWM/V5u03ZI/+8u7gPfs135RwWb/BsGSzXQS2qUkhhsU/JpSJ45sYJW54br0NG18w==','Y73OAUGYIQGR4B7Z3O5Y2DQ55PT5Z6ZG','91bf9945-34c2-4829-8659-5a4c189f84c5',NULL,0,0,NULL,1,0,0,0,0,'2024-04-24 11:12:06.766025'),('66a47e24-4673-486b-be52-f42ab252e308',NULL,'Panayot','Ivanov',0,0,'Default',NULL,NULL,NULL,NULL,'2024-04-21 15:06:33.130254','2024-04-21 15:06:04.758719','panayot.zhi','PANAYOT.ZHI','panayot.zhi@gmail.com','PANAYOT.ZHI@GMAIL.COM',1,'AQAAAAEAACcQAAAAEKT7RLc4jKh2QdRggvP8M8tsB3z9LAnSX7JEXpqdgilNwMRxvPx4vaAqog5RJyTVwA==','IRUGI5KDYCVPNETY3B2TNMZYF26V4VDZ','bd504e3a-bb85-42b8-bb79-e09dafcf1610',NULL,0,0,NULL,1,0,0,0,0,'2024-04-21 15:06:33.126577'),('84f14f53-c633-40af-aaf6-a1c3a9d199b9',NULL,'Snezha','Tsoneva-Mathewson',0,0,'Default',NULL,NULL,NULL,NULL,'2024-04-24 11:19:23.629316','2024-04-24 11:18:30.331641','admin','ADMIN','mathewson@uni-plovdiv.bg','MATHEWSON@UNI-PLOVDIV.BG',1,'AQAAAAEAACcQAAAAEGrWiOwRurTKn3pxNM9R4yRjwZEfIBkq4vzM3KQh7bvADWoXzmDZS26+8MbriPTyww==','JFSER2GPLOZEL2O5LXBUGYJFKTCIYDRA','3c5ccbc8-665b-48d4-a27e-2683cf8f2707',NULL,0,0,NULL,1,0,0,0,0,NULL);
/*!40000 ALTER TABLE `asp_net_users` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `documents`
--

DROP TABLE IF EXISTS `documents`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `documents` (
  `id` int NOT NULL AUTO_INCREMENT,
  `original_file_name` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `file_name` varchar(127) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `extension` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `mime_type` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `document_type` varchar(128) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `relative_path` varchar(512) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `last_updated` datetime(6) NOT NULL,
  `date_created` datetime(6) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=34 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `documents`
--

LOCK TABLES `documents` WRITE;
/*!40000 ALTER TABLE `documents` DISABLE KEYS */;
INSERT INTO `documents` VALUES (1,'2024-001-issue-pdf.pdf','2024-001-issue-pdf','pdf','application/pdf','IssuePdf','issues/2024/2024-001-issue-pdf.pdf','2024-04-21 13:25:00.685165','2024-04-21 13:25:00.685165'),(2,'2024-001-cover-page.png','2024-001-cover-page','png','image/png','CoverPage','issues/2024/2024-001-cover-page.png','2024-04-21 13:25:00.850727','2024-04-21 13:25:00.850727'),(3,'2024-001-index-page-01.png','2024-001-index-page-01','png','image/png','IndexPage','issues/2024/2024-001-index-page-01.png','2024-04-21 13:25:00.916605','2024-04-21 13:25:00.916605'),(4,'2024-001-index-page-02.png','2024-001-index-page-02','png','image/png','IndexPage','issues/2024/2024-001-index-page-02.png','2024-04-21 13:25:01.177652','2024-04-21 13:25:01.177652'),(5,'2024-001-index-page-03.png','2024-001-index-page-03','png','image/png','IndexPage','issues/2024/2024-001-index-page-03.png','2024-04-21 13:25:01.275833','2024-04-21 13:25:01.275833'),(6,'2024-001-index-page-04.png','2024-001-index-page-04','png','image/png','IndexPage','issues/2024/2024-001-index-page-04.png','2024-04-21 13:25:01.375870','2024-04-21 13:25:01.375870'),(7,'2024-001-index-page-05.png','2024-001-index-page-05','png','image/png','IndexPage','issues/2024/2024-001-index-page-05.png','2024-04-21 13:25:01.477604','2024-04-21 13:25:01.477604'),(8,'2024-001-index-page-06.png','2024-001-index-page-06','png','image/png','IndexPage','issues/2024/2024-001-index-page-06.png','2024-04-21 13:25:01.543530','2024-04-21 13:25:01.543530'),(9,'2024-001-source-pdf-16','2024-001-source-pdf-16','pdf','application/pdf','SourcePdf','issues/2024/2024-001-source-pdf-16.pdf','2024-04-21 13:59:42.600150','2024-04-21 13:59:42.600150'),(10,'2024-001-source-pdf-47','2024-001-source-pdf-47','pdf','application/pdf','SourcePdf','issues/2024/2024-001-source-pdf-47.pdf','2024-04-21 14:00:18.739554','2024-04-21 14:00:18.739554'),(11,'2024-001-source-pdf-72','2024-001-source-pdf-72','pdf','application/pdf','SourcePdf','issues/2024/2024-001-source-pdf-72.pdf','2024-04-21 14:00:50.900634','2024-04-21 14:00:50.900634'),(12,'2024-001-source-pdf-93','2024-001-source-pdf-93','pdf','application/pdf','SourcePdf','issues/2024/2024-001-source-pdf-93.pdf','2024-04-21 14:01:27.011867','2024-04-21 14:01:27.011867'),(14,'2024-001-source-pdf-114','2024-001-source-pdf-114','pdf','application/pdf','SourcePdf','issues/2024/2024-001-source-pdf-114.pdf','2024-04-21 14:06:02.682823','2024-04-21 14:06:02.682823'),(15,'2024-001-source-pdf-135','2024-001-source-pdf-135','pdf','application/pdf','SourcePdf','issues/2024/2024-001-source-pdf-135.pdf','2024-04-21 14:07:21.065702','2024-04-21 14:07:21.065702'),(16,'2024-001-source-pdf-179','2024-001-source-pdf-179','pdf','application/pdf','SourcePdf','issues/2024/2024-001-source-pdf-179.pdf','2024-04-21 14:08:30.992366','2024-04-21 14:08:30.992366'),(17,'2024-001-source-pdf-201','2024-001-source-pdf-201','pdf','application/pdf','SourcePdf','issues/2024/2024-001-source-pdf-201.pdf','2024-04-21 14:09:35.833918','2024-04-21 14:09:35.833918'),(18,'2024-001-source-pdf-210','2024-001-source-pdf-210','pdf','application/pdf','SourcePdf','issues/2024/2024-001-source-pdf-210.pdf','2024-04-21 14:10:24.846194','2024-04-21 14:10:24.846194'),(19,'2024-001-source-pdf-216','2024-001-source-pdf-216','pdf','application/pdf','SourcePdf','issues/2024/2024-001-source-pdf-216.pdf','2024-04-21 14:11:23.224204','2024-04-21 14:11:23.224204'),(20,'2024-001-source-pdf-220','2024-001-source-pdf-220','pdf','application/pdf','SourcePdf','issues/2024/2024-001-source-pdf-220.pdf','2024-04-21 14:12:57.403974','2024-04-21 14:12:57.403974'),(21,'2024-001-source-pdf-235','2024-001-source-pdf-235','pdf','application/pdf','SourcePdf','issues/2024/2024-001-source-pdf-235.pdf','2024-04-21 14:18:12.481674','2024-04-21 14:18:12.481674'),(22,'2024-001-source-pdf-16','2024-001-source-pdf-16','pdf','application/pdf','SourcePdf','issues/2024/2024-001-source-pdf-16.pdf','2024-04-21 14:19:10.056412','2024-04-21 14:19:10.056412'),(23,'2024-001-source-pdf-47','2024-001-source-pdf-47','pdf','application/pdf','SourcePdf','issues/2024/2024-001-source-pdf-47.pdf','2024-04-21 14:20:33.380373','2024-04-21 14:20:33.380373'),(24,'2024-001-source-pdf-72','2024-001-source-pdf-72','pdf','application/pdf','SourcePdf','issues/2024/2024-001-source-pdf-72.pdf','2024-04-21 14:21:17.413036','2024-04-21 14:21:17.413036'),(25,'2024-001-source-pdf-93','2024-001-source-pdf-93','pdf','application/pdf','SourcePdf','issues/2024/2024-001-source-pdf-93.pdf','2024-04-21 14:22:04.830379','2024-04-21 14:22:04.830379'),(26,'2024-001-source-pdf-114','2024-001-source-pdf-114','pdf','application/pdf','SourcePdf','issues/2024/2024-001-source-pdf-114.pdf','2024-04-21 14:53:44.863296','2024-04-21 14:53:44.863296'),(27,'2024-001-source-pdf-135','2024-001-source-pdf-135','pdf','application/pdf','SourcePdf','issues/2024/2024-001-source-pdf-135.pdf','2024-04-21 14:54:21.783942','2024-04-21 14:54:21.783942'),(28,'2024-001-source-pdf-179','2024-001-source-pdf-179','pdf','application/pdf','SourcePdf','issues/2024/2024-001-source-pdf-179.pdf','2024-04-21 14:56:26.910268','2024-04-21 14:56:26.910268'),(29,'2024-001-source-pdf-201','2024-001-source-pdf-201','pdf','application/pdf','SourcePdf','issues/2024/2024-001-source-pdf-201.pdf','2024-04-21 14:57:20.481098','2024-04-21 14:57:20.481098'),(30,'2024-001-source-pdf-210','2024-001-source-pdf-210','pdf','application/pdf','SourcePdf','issues/2024/2024-001-source-pdf-210.pdf','2024-04-21 14:58:02.439863','2024-04-21 14:58:02.439863'),(31,'2024-001-source-pdf-216','2024-001-source-pdf-216','pdf','application/pdf','SourcePdf','issues/2024/2024-001-source-pdf-216.pdf','2024-04-21 14:58:44.993526','2024-04-21 14:58:44.993526'),(32,'2024-001-source-pdf-220','2024-001-source-pdf-220','pdf','application/pdf','SourcePdf','issues/2024/2024-001-source-pdf-220.pdf','2024-04-21 14:59:38.658084','2024-04-21 14:59:38.658084'),(33,'2024-001-source-pdf-235','2024-001-source-pdf-235','pdf','application/pdf','SourcePdf','issues/2024/2024-001-source-pdf-235.pdf','2024-04-21 15:00:22.645779','2024-04-21 15:00:22.645779');
/*!40000 ALTER TABLE `documents` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `dossier_journals`
--

DROP TABLE IF EXISTS `dossier_journals`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `dossier_journals` (
  `id` int NOT NULL AUTO_INCREMENT,
  `dossier_id` int NOT NULL,
  `type` int NOT NULL,
  `message` varchar(512) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `performed_by_id` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `last_updated` datetime(6) NOT NULL,
  `date_created` datetime(6) NOT NULL,
  `message_arguments` varchar(512) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `ix_dossier_journals_dossier_id` (`dossier_id`),
  KEY `ix_dossier_journals_performed_by_id` (`performed_by_id`),
  KEY `ix_dossier_journals_message` (`message`),
  CONSTRAINT `fk_dossier_journals_dossiers_dossier_id` FOREIGN KEY (`dossier_id`) REFERENCES `dossiers` (`id`) ON DELETE CASCADE,
  CONSTRAINT `fk_dossier_journals_users_performed_by_id` FOREIGN KEY (`performed_by_id`) REFERENCES `asp_net_users` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `dossier_journals`
--

LOCK TABLES `dossier_journals` WRITE;
/*!40000 ALTER TABLE `dossier_journals` DISABLE KEYS */;
/*!40000 ALTER TABLE `dossier_journals` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `dossier_reviews`
--

DROP TABLE IF EXISTS `dossier_reviews`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `dossier_reviews` (
  `id` int NOT NULL AUTO_INCREMENT,
  `first_name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `last_name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `email` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `reviewer_id` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `dossier_id` int NOT NULL,
  `review_id` int NOT NULL,
  PRIMARY KEY (`id`),
  KEY `ix_dossier_reviews_dossier_id` (`dossier_id`),
  KEY `ix_dossier_reviews_review_id` (`review_id`),
  KEY `ix_dossier_reviews_reviewer_id` (`reviewer_id`),
  CONSTRAINT `fk_dossier_reviews_documents_review_id` FOREIGN KEY (`review_id`) REFERENCES `documents` (`id`) ON DELETE CASCADE,
  CONSTRAINT `fk_dossier_reviews_dossiers_dossier_id` FOREIGN KEY (`dossier_id`) REFERENCES `dossiers` (`id`) ON DELETE CASCADE,
  CONSTRAINT `fk_dossier_reviews_users_reviewer_id` FOREIGN KEY (`reviewer_id`) REFERENCES `asp_net_users` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `dossier_reviews`
--

LOCK TABLES `dossier_reviews` WRITE;
/*!40000 ALTER TABLE `dossier_reviews` DISABLE KEYS */;
/*!40000 ALTER TABLE `dossier_reviews` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `dossiers`
--

DROP TABLE IF EXISTS `dossiers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `dossiers` (
  `id` int NOT NULL AUTO_INCREMENT,
  `title` varchar(1024) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `first_name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `last_name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `email` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `status` int NOT NULL,
  `created_by_id` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT '',
  `assigned_to_id` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `date_created` datetime(6) NOT NULL DEFAULT '0001-01-01 00:00:00.000000',
  `last_updated` datetime(6) NOT NULL DEFAULT '0001-01-01 00:00:00.000000',
  `author_id` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `ix_dossiers_created_by_id` (`created_by_id`),
  KEY `ix_dossiers_assigned_to_id` (`assigned_to_id`),
  KEY `ix_dossiers_author_id` (`author_id`),
  CONSTRAINT `fk_dossiers_users_assigned_to_id` FOREIGN KEY (`assigned_to_id`) REFERENCES `asp_net_users` (`id`),
  CONSTRAINT `fk_dossiers_users_author_id` FOREIGN KEY (`author_id`) REFERENCES `asp_net_users` (`id`),
  CONSTRAINT `fk_dossiers_users_created_by_id` FOREIGN KEY (`created_by_id`) REFERENCES `asp_net_users` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `dossiers`
--

LOCK TABLES `dossiers` WRITE;
/*!40000 ALTER TABLE `dossiers` DISABLE KEYS */;
/*!40000 ALTER TABLE `dossiers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `issue_journals`
--

DROP TABLE IF EXISTS `issue_journals`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `issue_journals` (
  `id` int NOT NULL AUTO_INCREMENT,
  `issue_id` int NOT NULL,
  `type` int NOT NULL,
  `message` varchar(512) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `performed_by_id` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `last_updated` datetime(6) NOT NULL,
  `date_created` datetime(6) NOT NULL,
  `message_arguments` varchar(512) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `ix_issue_journals_issue_id` (`issue_id`),
  KEY `ix_issue_journals_performed_by_id` (`performed_by_id`),
  KEY `ix_issue_journals_message` (`message`),
  CONSTRAINT `fk_issue_journals_issues_issue_id` FOREIGN KEY (`issue_id`) REFERENCES `issues` (`id`) ON DELETE CASCADE,
  CONSTRAINT `fk_issue_journals_users_performed_by_id` FOREIGN KEY (`performed_by_id`) REFERENCES `asp_net_users` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `issue_journals`
--

LOCK TABLES `issue_journals` WRITE;
/*!40000 ALTER TABLE `issue_journals` DISABLE KEYS */;
/*!40000 ALTER TABLE `issue_journals` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `issues`
--

DROP TABLE IF EXISTS `issues`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `issues` (
  `id` int NOT NULL AUTO_INCREMENT,
  `is_available` tinyint(1) NOT NULL,
  `issue_number` int NOT NULL,
  `description` varchar(1024) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `last_updated` datetime(6) NOT NULL,
  `date_created` datetime(6) NOT NULL,
  `release_date` date NOT NULL DEFAULT '0001-01-01',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `issues`
--

LOCK TABLES `issues` WRITE;
/*!40000 ALTER TABLE `issues` DISABLE KEYS */;
INSERT INTO `issues` VALUES (2,1,1,'© Дизайн на корицата: Димитър Келбечев\r\n© Cover design: Dimitar Kelbechev\r\n© Пловдивско университетско издателство, 2024\r\n© Plovdiv University Press, 2024\r\n\r\nISSN 3033-0181 (Print)','2024-04-21 13:25:01.715621','2024-04-21 13:25:01.715621','2024-01-01');
/*!40000 ALTER TABLE `issues` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `languages`
--

DROP TABLE IF EXISTS `languages`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `languages` (
  `id` int NOT NULL AUTO_INCREMENT,
  `culture` varchar(12) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `ix_languages_culture` (`culture`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `languages`
--

LOCK TABLES `languages` WRITE;
/*!40000 ALTER TABLE `languages` DISABLE KEYS */;
INSERT INTO `languages` VALUES (1,'bg'),(2,'en');
/*!40000 ALTER TABLE `languages` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `source_journals`
--

DROP TABLE IF EXISTS `source_journals`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `source_journals` (
  `id` int NOT NULL AUTO_INCREMENT,
  `source_id` int NOT NULL,
  `type` int NOT NULL,
  `message` varchar(512) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `performed_by_id` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `last_updated` datetime(6) NOT NULL,
  `date_created` datetime(6) NOT NULL,
  `message_arguments` varchar(512) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `ix_source_journals_performed_by_id` (`performed_by_id`),
  KEY `ix_source_journals_source_id` (`source_id`),
  KEY `ix_source_journals_message` (`message`),
  CONSTRAINT `fk_source_journals_sources_source_id` FOREIGN KEY (`source_id`) REFERENCES `sources` (`id`) ON DELETE CASCADE,
  CONSTRAINT `fk_source_journals_users_performed_by_id` FOREIGN KEY (`performed_by_id`) REFERENCES `asp_net_users` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `source_journals`
--

LOCK TABLES `source_journals` WRITE;
/*!40000 ALTER TABLE `source_journals` DISABLE KEYS */;
/*!40000 ALTER TABLE `source_journals` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sources`
--

DROP TABLE IF EXISTS `sources`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `sources` (
  `id` int NOT NULL AUTO_INCREMENT,
  `first_name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `last_name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `title` varchar(512) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `author_notes` varchar(1024) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `title_notes` varchar(1024) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `starting_page` int NOT NULL,
  `language_id` int NOT NULL,
  `issue_id` int NOT NULL DEFAULT '0',
  `author_id` varchar(127) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `last_updated` datetime(6) NOT NULL,
  `date_created` datetime(6) NOT NULL,
  `last_page` int NOT NULL DEFAULT '0',
  `pdf_id` int NOT NULL DEFAULT '0',
  `author_names` varchar(512) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci GENERATED ALWAYS AS (concat(`first_name`,_utf8mb4' ',`last_name`)) VIRTUAL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `ix_sources_pdf_id` (`pdf_id`),
  KEY `ix_sources_author_id` (`author_id`),
  KEY `ix_sources_issue_id` (`issue_id`),
  KEY `ix_sources_language_id` (`language_id`),
  KEY `ix_sources_first_name` (`first_name`),
  KEY `ix_sources_last_name` (`last_name`),
  KEY `ix_sources_title` (`title`),
  KEY `ix_sources_author_names` (`author_names`),
  CONSTRAINT `fk_sources_documents_pdf_id` FOREIGN KEY (`pdf_id`) REFERENCES `documents` (`id`) ON DELETE CASCADE,
  CONSTRAINT `fk_sources_issues_issue_id` FOREIGN KEY (`issue_id`) REFERENCES `issues` (`id`) ON DELETE CASCADE,
  CONSTRAINT `fk_sources_languages_language_id` FOREIGN KEY (`language_id`) REFERENCES `languages` (`id`) ON DELETE CASCADE,
  CONSTRAINT `fk_sources_users_author_id` FOREIGN KEY (`author_id`) REFERENCES `asp_net_users` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=26 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sources`
--

LOCK TABLES `sources` WRITE;
/*!40000 ALTER TABLE `sources` DISABLE KEYS */;
INSERT INTO `sources` (`id`, `first_name`, `last_name`, `title`, `author_notes`, `title_notes`, `starting_page`, `language_id`, `issue_id`, `author_id`, `last_updated`, `date_created`, `last_page`, `pdf_id`) VALUES (1,'Благовест','ЗЛАТАНОВ','ТВОРЦИ БОРЦИ, РОМАНСИ, КАФЕНЕТА: ЧАСТИ ОТ ПОЛИТИЧЕСКАТА ПРЕДИСТОРИЯ НА АНГЛИЙСКИЯ ПРЕВОД НА РОМАНА „ПОД ИГОТО“','(Хайделбергски университет)',NULL,16,1,2,NULL,'2024-04-21 13:59:42.762018','2024-04-21 13:59:42.762018',46,9),(2,'Клео','ПРОТОХРИСТОВА','БЪЛГАРСКИТЕ 1922-ра – 1925-а – ЛИТЕРАТУРА, КРИТИКА, ПОЛИТИКИ','(Пловдивски университет „Паисий Хилендарски“)',NULL,47,1,2,NULL,'2024-04-21 14:00:18.744667','2024-04-21 14:00:18.744667',71,10),(3,'Якуб','МИКУЛЕЦКИ','САНСТЕФАНСКИЯТ МИТ В БЪЛГАРСКАТА ЕМИГРАНТСКА ЛИТЕРАТУРА СЛЕД 1944 г.','(Славянски институт при Чешката академия на науките, Прага)',NULL,72,1,2,NULL,'2024-04-21 14:00:50.904702','2024-04-21 14:00:50.904702',92,11),(4,'Милена','КИРОВА','ЛИТЕРАТУРА, ВЛАСТ И ВЪОБРАЖЕНИЕ: ПОТРЕБНОСТТА ОТ ДИСТОПИЯ В БЪЛГАРСКИЯ РОМАН ПРЕЗ ХХІ ВЕК','(Софийски университет „Св. Климент Охридски“)',NULL,93,1,2,NULL,'2024-04-21 14:01:27.015947','2024-04-21 14:01:27.015947',113,12),(6,'Витана','КОСТАДИНОВА','ФРАНКЕНЩАЙН И ВЛАСТОВАТА ДИНАМИКА: ОТ ТЕКСТА КЪМ КОНТЕКСТА','(Пловдивски университет „Паисий Хилендарски“)',NULL,114,1,2,NULL,'2024-04-21 14:06:02.687956','2024-04-21 14:06:02.687956',132,14),(7,'Guglielmo','CINQUE','THE HIDDEN RULES OF WORD ORDER VARIATIONS','(Ca’ Foscari University, Venice)',NULL,135,1,2,NULL,'2024-04-21 14:07:21.069706','2024-04-21 14:07:21.069706',161,15),(8,'Христо','САЛДЖИЕВ','ЗА ЕТИМОЛОГИЯТА НА СТАРОБЪЛГАРСКАТА ТИТЛА ЦѢСАРЬ / ЦЬСАРЬ','(Тракийски университет – Стара Загора)',NULL,179,1,2,NULL,'2024-04-21 14:08:30.995876','2024-04-21 14:08:30.995876',198,16),(9,'Светла','ЧЕРПОКОВА','ЗА ЕДНО КОСМОПОЛИТНО ПЪТЕШЕСТВИЕ В РАЗБИРАНЕТО ЗА СВЕТОВНА ЛИТЕРАТУРА','(Пловдивски университет „Паисий Хилендарски“)','(Галин Тиханов, Световна литература. Космополитизъм.\r\nИзгнание. Избрани статии и интервюта.\r\nСофия: Кралица Маб, 2022. ISBN 978-954-533-205-0)',201,1,2,NULL,'2024-04-21 14:09:35.836971','2024-04-21 14:09:35.836971',209,17),(10,'Запрян','КОЗЛУДЖОВ','ЗА АКТУАЛНИТЕ ПРОБЛЕМИ И АСПЕКТИ НА ЛИТЕРАТУРНАТА СОЦИОЛОГИЯ','(Пловдивски университет „Паисий Хилендарски“)','(Жизел Сапиро, Социология на литературата. София: СОНМ, 2023.\r\nISBN 9786197500417)',210,1,2,NULL,'2024-04-21 14:10:24.849549','2024-04-21 14:10:24.849549',215,18),(11,'Димитър','КРЪСТЕВ','КАК ДЕЙСТВАТ ЛИТЕРАТУРНИТЕ ТЕКСТОВЕ?','(Пловдивски университет „Паисий Хилендарски“)','(Александър Панов, Текстът като социално действие. София:\r\nИЦ „Боян Пенев“, УЦ „Диоген.бг“, 2022. ISBN 978-619-7372-53-3)',216,1,2,NULL,'2024-04-21 14:11:23.227754','2024-04-21 14:11:23.227754',219,19),(12,'Yana','ROWLAND','METAXIS: ANTONY ROWLAND BETWEEN COMMITMENT AND AUTONOMY','(Paisii Hilendarski University of Plovdiv)','(Antony Rowland, Metamodernism and Contemporary British Poetry.\r\nCambridge: Cambridge University Press, 2021, pp. xi, 240.\r\nISBN 9781108841979 https://doi.org/10.1017/9781108895286)',220,1,2,NULL,'2024-04-21 14:12:57.407098','2024-04-21 14:12:57.407098',234,20),(13,'Boris','MINKOV','VIENNA IN 1900 AS THE FOCUS OF BULGARIAN LITERARY HISTORY','(Krastyo Sarafov National Academy of Theatre and Film Arts, Sofia)','(Mladen Vlashki, Materialien zur Rezeption der Wiener Moderne\r\nin Bulgarien bis 1944. Hermann Bahr, Hugo von Hofmannsthal,\r\nAtrhur Schnitzler. Berlin: Peter Lang, 2022. ISBN 9783631830666)',235,1,2,NULL,'2024-04-21 14:18:12.485117','2024-04-21 14:18:12.485117',238,21),(14,'Blagovest','ZLATANOV','AUTHORS AND FIGHTERS, ROMANCES, COFFEE-HOUSES: SOME POLITICAL DEVELOPMENTS PRECEDING THE ENGLISH TRANSLATION OF THE NOVEL “UNDER THE YOKE”','(University of Heidelberg)',NULL,16,2,2,NULL,'2024-04-21 14:19:10.059122','2024-04-21 14:19:10.059122',46,22),(15,'Kleo','PROTOHRISTOVA','1922 – 1925 in BULGARIA – LITERATURE, LITERARY CRITICISM and POLITICS','(Paisii Hilendarski University of Plovdiv)',NULL,47,2,2,NULL,'2024-04-21 14:20:33.383268','2024-04-21 14:20:33.383268',71,23),(16,'Jakub','MIKULECKÝ','THE MYTH OF SAN STEFANO IN BULGARIAN EXILE LITERATURE AFTER 1944','(Institute of Slavonic Studies of the CAS, Prague)',NULL,72,2,2,NULL,'2024-04-21 14:21:17.415894','2024-04-21 14:21:17.415894',92,24),(17,'Мilena','KIROVA','LITERATURE, POWER AND IMAGINATION: THE QUEST FOR DYSTOPIA IN THE BULGARIAN NOVEL OF THE 21st CENTURY','(Sofia University St. Kliment Ohridski)',NULL,93,2,2,NULL,'2024-04-21 14:22:04.833156','2024-04-21 14:22:04.833156',113,25),(18,'Vitana','KOSTADINOVA','FRANKENSTEIN AND POWER DYNAMICS: FROM TEXT TO CONTEXT','(Paisii Hilendarski University of Plovdiv)',NULL,114,2,2,NULL,'2024-04-21 14:53:44.865886','2024-04-21 14:53:44.865886',132,26),(19,'Guglielmo','CINQUE','THE HIDDEN RULES OF WORD ORDER VARIATIONS','(Ca’ Foscari University, Venice)',NULL,135,2,2,NULL,'2024-04-21 14:54:21.786581','2024-04-21 14:54:21.786581',161,27),(20,'Hristo','SALDZHIEV','THE ETYMOLOGY OF THE OLD BULGARIAN TITLE ЦѢСАРЬ / ЦЬСАРЬ','(Trakia University – Stara Zagora)',NULL,179,2,2,NULL,'2024-04-21 14:56:26.912637','2024-04-21 14:56:26.912637',198,28),(21,'Svetla','CHERPOKOVA','ABOUT A COSMOPOLITAN JOURNEY THROUGH THE UNDERSTANDING OF WORLD LITERATURE','(Paisii Hilendarski University of Plovdiv)','(Galin Tihanov, World Literature. Cosmopolitanism. Exile.\r\nSelected Articles and Interviews. Sofia: Kralitsa Mab, 2022.\r\nISBN 978-954-533-205-0)',201,2,2,NULL,'2024-04-21 14:57:20.483556','2024-04-21 14:57:20.483556',209,29),(22,'Zapryan','KOZLUDZHOV','ON CURRENT PROBLEMS AND ASPECTS OF LITERARY SOCIOLOGY','(Paisii Hilendarski University of Plovdiv)','(Gisѐle Sapiro, Sociology of Literature. Sofia: SONM, 2023.\r\nISBN 9786197500417)',210,2,2,NULL,'2024-04-21 14:58:02.442785','2024-04-21 14:58:02.442785',215,30),(23,'Dimitar','KRASTEV','HOW DO LITERARY TEXTS WORK?','(Paisii Hilendarski University of Plovdiv)','(Aleksandar Panov, The Text as a Social Action. Sofia:\r\nBoyan Penev, Diogen.bg, 2022. ISBN 978-619-7372-53-3)',216,2,2,NULL,'2024-04-21 14:58:44.996296','2024-04-21 14:58:44.996296',219,31),(24,'Yana','ROWLAND','METAXIS: ANTONY ROWLAND BETWEEN COMMITMENT AND AUTONOMY','(Paisii Hilendarski University of Plovdiv)','(Antony Rowland, Metamodernism and Contemporary British Poetry.\r\nCambridge: Cambridge University Press, 2021. ISBN 9781108841979\r\nhttps://doi.org/10.1017/9781108895286)',220,2,2,NULL,'2024-04-21 14:59:38.660825','2024-04-21 14:59:38.660825',234,32),(25,'Boris','MINKOV','VIENNA IN 1900 AS THE FOCUS OF BULGARIAN LITERARY HISTORY','(Krastyo Sarafov National Academy of Theatre and Film Arts, Sofia)','(Mladen Vlashki, Materialien zur Rezeption der Wiener Moderne\r\nin Bulgarien bis 1944. Hermann Bahr, Hugo von Hofmannsthal,\r\nAtrhur Schnitzler. Berlin: Peter Lang, 2022. ISBN 9783631830666)',235,2,2,NULL,'2024-04-21 15:00:22.648906','2024-04-21 15:00:22.648906',238,33);
/*!40000 ALTER TABLE `sources` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `string_resources`
--

DROP TABLE IF EXISTS `string_resources`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `string_resources` (
  `id` int NOT NULL AUTO_INCREMENT,
  `key` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `value` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `language_id` int NOT NULL,
  `edited_by_id` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT '',
  `last_edited` datetime(6) NOT NULL DEFAULT '0001-01-01 00:00:00.000000',
  PRIMARY KEY (`id`),
  KEY `ix_string_resources_key` (`key`),
  KEY `ix_string_resources_language_id` (`language_id`),
  KEY `ix_string_resources_edited_by_id` (`edited_by_id`),
  CONSTRAINT `fk_string_resources_languages_language_id` FOREIGN KEY (`language_id`) REFERENCES `languages` (`id`) ON DELETE CASCADE,
  CONSTRAINT `fk_string_resources_users_edited_by_id` FOREIGN KEY (`edited_by_id`) REFERENCES `asp_net_users` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `string_resources`
--

LOCK TABLES `string_resources` WRITE;
/*!40000 ALTER TABLE `string_resources` DISABLE KEYS */;
/*!40000 ALTER TABLE `string_resources` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping events for database 'linc_db'
--

--
-- Dumping routines for database 'linc_db'
--
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2024-05-12 12:30:25
