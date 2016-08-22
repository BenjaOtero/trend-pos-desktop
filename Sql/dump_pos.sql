/*
SQLyog Ultimate v9.02 
MySQL - 5.5.0-m2-community : Database - dump_pos
*********************************************************************
*/

/*!40101 SET NAMES utf8 */;

/*!40101 SET SQL_MODE=''*/;

/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;
CREATE DATABASE /*!32312 IF NOT EXISTS*/`dump_pos` /*!40100 DEFAULT CHARACTER SET utf8 */;

USE `dump_pos`;

/*Table structure for table `exportar_fondo_caja` */

DROP TABLE IF EXISTS `exportar_fondo_caja`;

CREATE TABLE `exportar_fondo_caja` (
  `IdFondoFONP` int(11) DEFAULT NULL,
  `FechaFONP` date NOT NULL,
  `IdPcFONP` int(11) NOT NULL,
  `ImporteFONP` double DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/*Data for the table `exportar_fondo_caja` */

LOCK TABLES `exportar_fondo_caja` WRITE;

UNLOCK TABLES;

/*Table structure for table `exportar_tesoreria_movimientos` */

DROP TABLE IF EXISTS `exportar_tesoreria_movimientos`;

CREATE TABLE `exportar_tesoreria_movimientos` (
  `IdMovTESM` int(11) NOT NULL,
  `FechaTESM` datetime DEFAULT NULL,
  `IdPcTESM` int(11) DEFAULT NULL,
  `DetalleTESM` varchar(200) DEFAULT NULL,
  `ImporteTESM` double DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/*Data for the table `exportar_tesoreria_movimientos` */

LOCK TABLES `exportar_tesoreria_movimientos` WRITE;

UNLOCK TABLES;

/*Table structure for table `exportar_ventas` */

DROP TABLE IF EXISTS `exportar_ventas`;

CREATE TABLE `exportar_ventas` (
  `IdVentaVEN` int(11) NOT NULL,
  `IdPCVEN` int(11) DEFAULT NULL,
  `FechaVEN` datetime DEFAULT NULL,
  `IdClienteVEN` int(11) DEFAULT NULL,
  `NroCuponVEN` varchar(12) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/*Data for the table `exportar_ventas` */

LOCK TABLES `exportar_ventas` WRITE;

UNLOCK TABLES;

/*Table structure for table `exportar_ventas_detalle` */

DROP TABLE IF EXISTS `exportar_ventas_detalle`;

CREATE TABLE `exportar_ventas_detalle` (
  `IdDVEN` int(11) NOT NULL,
  `IdVentaDVEN` int(11) DEFAULT NULL,
  `IdLocalDVEN` int(3) DEFAULT NULL,
  `IdArticuloDVEN` varchar(50) DEFAULT NULL,
  `DescripcionDVEN` varchar(50) DEFAULT NULL,
  `CantidadDVEN` int(11) DEFAULT NULL,
  `PrecioPublicoDVEN` double DEFAULT NULL,
  `PrecioCostoDVEN` double DEFAULT NULL,
  `PrecioMayorDVEN` double DEFAULT NULL,
  `IdFormaPagoDVEN` int(11) DEFAULT NULL,
  `NroCuponDVEN` int(11) DEFAULT NULL,
  `NroFacturaDVEN` int(11) DEFAULT NULL,
  `IdEmpleadoDVEN` int(11) DEFAULT NULL,
  `LiquidadoDVEN` bit(1) DEFAULT NULL,
  `EsperaDVEN` bit(1) DEFAULT NULL,
  `DevolucionDVEN` smallint(1) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

/*Data for the table `exportar_ventas_detalle` */

LOCK TABLES `exportar_ventas_detalle` WRITE;

UNLOCK TABLES;

/* Procedure structure for procedure `DatosPos_Borrar` */

/*!50003 DROP PROCEDURE IF EXISTS  `DatosPos_Borrar` */;

DELIMITER $$

/*!50003 CREATE DEFINER=`ncsoftwa_re`@`%` PROCEDURE `DatosPos_Borrar`()
BEGIN
DELETE FROM exportar_fondo_caja;
DELETE FROM exportar_tesoreria_movimientos;
DELETE FROM exportar_ventas;
delete from exportar_ventas_detalle;
END */$$
DELIMITER ;

/* Procedure structure for procedure `DatosPos_ControlarExport` */

/*!50003 DROP PROCEDURE IF EXISTS  `DatosPos_ControlarExport` */;

DELIMITER $$

/*!50003 CREATE DEFINER=`ncsoftwa_re`@`%` PROCEDURE `DatosPos_ControlarExport`()
BEGIN
select count(*) FROM `exportar_fondo_caja`;
END */$$
DELIMITER ;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;
