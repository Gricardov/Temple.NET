IF DB_ID('BDTEMPLE') IS NOT NULL
BEGIN
DROP DATABASE BDTEMPLE
END
GO

CREATE DATABASE BDTEMPLE
GO

USE BDTEMPLE
GO

-- LAS TABLAS

IF OBJECT_ID('TB_ROL') IS NOT NULL
BEGIN
DROP TABLE TB_ROL;
END

CREATE TABLE TB_ROL(
ID_ROL INT NOT NULL,
DES_ROL VARCHAR(30) NOT NULL
)

ALTER TABLE TB_ROL
ADD PRIMARY KEY (ID_ROL)

IF OBJECT_ID('TB_USUARIO') IS NOT NULL
BEGIN
DROP TABLE TB_USUARIO;
END

CREATE TABLE TB_USUARIO(
COD_USU INT IDENTITY(1001,1) NOT NULL,
NOM_USU VARCHAR(60) NOT NULL,
APA_USU VARCHAR(30) NOT NULL,
AMA_USU VARCHAR(30) NOT NULL,
CORREO_USU VARCHAR(50) NOT NULL,
LOG_USU VARCHAR(50) NOT NULL,
CLA_USU VARCHAR(50) NOT NULL,
ID_ROL INT NOT NULL,
)

ALTER TABLE TB_USUARIO
ADD PRIMARY KEY (COD_USU),
FOREIGN KEY (ID_ROL) REFERENCES TB_ROL(ID_ROL)

IF OBJECT_ID('TB_PERFIL_USUARIO') IS NOT NULL
BEGIN
DROP TABLE TB_PERFIL_USUARIO;
END

CREATE TABLE TB_PERFIL_USUARIO(
ID_PER INT IDENTITY(1001,1) NOT NULL,
COD_USU INT NOT NULL,
SOBRE_MI VARCHAR(500) NULL DEFAULT 'NO ESPECIFICADO',
BUSCANDO VARCHAR(200) NULL DEFAULT 'NO ESPECIFICADO',
ESPECIALIDAD VARCHAR(50) NULL DEFAULT 'NO ESPECIFICADO',
DIR_CV VARCHAR(50) NULL DEFAULT 'NO ESPECIFICADO',
VERIFICADO BIT NULL DEFAULT 0
)

ALTER TABLE TB_PERFIL_USUARIO
ADD PRIMARY KEY (ID_PER),
FOREIGN KEY (COD_USU) REFERENCES TB_USUARIO (COD_USU)

IF OBJECT_ID('TB_UBICACION_USUARIO') IS NOT NULL
BEGIN
DROP TABLE TB_UBICACION_USUARIO;
END

CREATE TABLE TB_UBICACION_USUARIO(
COD_USU INT NOT NULL,
LAT_USU DECIMAL(8,6) NULL,
LONG_USU DECIMAL(8,6) NULL
)

ALTER TABLE TB_UBICACION_USUARIO
ADD FOREIGN KEY (COD_USU) REFERENCES TB_USUARIO (COD_USU)

IF OBJECT_ID('TB_CATEGORIA_CURSO') IS NOT NULL
BEGIN
DROP TABLE TB_CATEGORIA_CURSO;
END

CREATE TABLE TB_CATEGORIA_CURSO(
ID_CAT INT NOT NULL,
DES_CAT VARCHAR(60) NOT NULL
)

ALTER TABLE TB_CATEGORIA_CURSO
ADD PRIMARY KEY (ID_CAT)

IF OBJECT_ID('TB_SUBCATEGORIAS_CURSO') IS NOT NULL
BEGIN
DROP TABLE TB_SUBCATEGORIAS_CURSO;
END

CREATE TABLE TB_SUBCATEGORIAS_CURSO(
ID_SUB INT NOT NULL,
ID_CAT INT NOT NULL,
DES_SUB VARCHAR(60) NOT NULL
)

ALTER TABLE TB_SUBCATEGORIAS_CURSO
ADD PRIMARY KEY (ID_SUB),
UNIQUE (ID_SUB,ID_CAT),
FOREIGN KEY (ID_CAT) REFERENCES TB_CATEGORIA_CURSO (ID_CAT)

IF OBJECT_ID('TB_PREFERENCIA_APRENDIZAJE') IS NOT NULL
BEGIN
DROP TABLE TB_PREFERENCIA_APRENDIZAJE;
END

CREATE TABLE TB_PREFERENCIA_APRENDIZAJE(
ID_PREF INT IDENTITY (1,1) NOT NULL,
COD_USU INT NOT NULL,
ID_CAT INT NOT NULL,
ID_SUB INT NOT NULL
)

ALTER TABLE TB_PREFERENCIA_APRENDIZAJE
ADD PRIMARY KEY(ID_PREF),
UNIQUE (COD_USU,ID_CAT,ID_SUB),
FOREIGN KEY (COD_USU) REFERENCES TB_USUARIO(COD_USU),
FOREIGN KEY (ID_CAT) REFERENCES TB_CATEGORIA_CURSO(ID_CAT),
FOREIGN KEY (ID_SUB) REFERENCES TB_SUBCATEGORIAS_CURSO(ID_SUB)

IF OBJECT_ID('TB_PREFERENCIA_ENSENANZA') IS NOT NULL
BEGIN
DROP TABLE TB_PREFERENCIA_ENSENANZA;
END

CREATE TABLE TB_PREFERENCIA_ENSENANZA(
ID_PREF INT IDENTITY (1,1) NOT NULL,
COD_USU INT NOT NULL,
ID_CAT INT NOT NULL,
ID_SUB INT NOT NULL
)

ALTER TABLE TB_PREFERENCIA_ENSENANZA
ADD PRIMARY KEY(ID_PREF),
UNIQUE (COD_USU,ID_CAT,ID_SUB),
FOREIGN KEY (COD_USU) REFERENCES TB_USUARIO(COD_USU),
FOREIGN KEY (ID_CAT) REFERENCES TB_CATEGORIA_CURSO(ID_CAT),
FOREIGN KEY (ID_SUB) REFERENCES TB_SUBCATEGORIAS_CURSO(ID_SUB)

-- LOS TIGRES

IF OBJECT_ID('UTR_PERFIL_USUARIO') IS NOT NULL
BEGIN
DROP TRIGGER UTR_PERFIL_USUARIO;
END
GO

CREATE TRIGGER UTR_PERFIL_USUARIO
ON TB_USUARIO
AFTER INSERT
AS
BEGIN
INSERT INTO TB_PERFIL_USUARIO
SELECT I.COD_USU,'NO ESPECIFICADO','NO ESPECIFICADO','NO ESPECIFICADO','NO ESPECIFICADO',0 FROM INSERTED I
END
GO

-- LAS FUNCIONES
IF OBJECT_ID('UFN_CORRELATIVO_USUARIO') IS NOT NULL
BEGIN
DROP FUNCTION UFN_CORRELATIVO_USUARIO;
END
GO

CREATE FUNCTION UFN_CORRELATIVO_USUARIO ()
RETURNS INT
AS
BEGIN
	DECLARE @CODIGO INT;	
	SET @CODIGO=(SELECT MAX(COD_USU) FROM TB_USUARIO)
	SET @CODIGO=@CODIGO+1;
	RETURN @CODIGO;
END
GO

-- LOS PROCEDIMIENTOS
/*
IF OBJECT_ID('USP_REGISTRAR_USUARIO') IS NOT NULL
BEGIN
DROP PROCEDURE USP_REGISTRAR_USUARIO;
END
GO

CREATE PROCEDURE USP_REGISTRAR_USUARIO
AS
BEGIN

END
GO

*/

IF OBJECT_ID('USP_LOGIN') IS NOT NULL
BEGIN
DROP PROCEDURE USP_LOGIN;
END
GO

CREATE PROCEDURE USP_LOGIN (@USUARIO VARCHAR(50), @CONTRASENA VARCHAR(50))
AS
BEGIN
	SELECT U.COD_USU, U.NOM_USU, U.APA_USU, U.AMA_USU, U.LOG_USU, U.CLA_USU, U.ID_ROL, R.DES_ROL
	FROM TB_USUARIO U
	JOIN TB_ROL R
	ON R.ID_ROL=U.ID_ROL
	WHERE U.LOG_USU=@USUARIO AND U.CLA_USU=@CONTRASENA
END
GO

IF OBJECT_ID('USP_OBTENER_PREFERENCIAS_APRENDIZAJE') IS NOT NULL
BEGIN
DROP PROCEDURE USP_OBTENER_PREFERENCIAS_APRENDIZAJE;
END
GO

CREATE PROCEDURE USP_OBTENER_PREFERENCIAS_APRENDIZAJE (@CODUSU INT)
AS
BEGIN
SELECT P.ID_CAT, C.DES_CAT, P.ID_SUB, S.DES_SUB
FROM TB_PREFERENCIA_APRENDIZAJE P
INNER JOIN TB_CATEGORIA_CURSO C
ON P.ID_CAT=C.ID_CAT
INNER JOIN TB_SUBCATEGORIAS_CURSO S
ON S.ID_SUB=P.ID_SUB
INNER JOIN TB_USUARIO U
ON P.COD_USU=U.COD_USU
WHERE P.COD_USU=@CODUSU
END
GO

IF OBJECT_ID('USP_OBTENER_INSTRUCTORES_RECOMENDADOS_PREFERENCIA') IS NOT NULL
BEGIN
DROP PROCEDURE USP_OBTENER_INSTRUCTORES_RECOMENDADOS_PREFERENCIA;
END
GO

CREATE PROCEDURE USP_OBTENER_INSTRUCTORES_RECOMENDADOS_PREFERENCIA (@IDSUB INT)
AS
BEGIN
SELECT P.COD_USU, U.NOM_USU, U.APA_USU, U.AMA_USU, (SELECT PU.ESPECIALIDAD FROM TB_PERFIL_USUARIO PU WHERE PU.COD_USU=P.COD_USU),
		'10KM',5,4,(SELECT PU.VERIFICADO FROM TB_PERFIL_USUARIO PU WHERE PU.COD_USU=P.COD_USU)
FROM TB_PREFERENCIA_ENSENANZA P
INNER JOIN
TB_USUARIO U
ON U.COD_USU=P.COD_USU
WHERE U.ID_ROL=1 AND P.ID_SUB=@IDSUB;
END
GO

IF OBJECT_ID('USP_OBTENER_INSTRUCTORES_BUSQUEDA') IS NOT NULL
BEGIN
DROP PROCEDURE USP_OBTENER_INSTRUCTORES_BUSQUEDA;
END
GO

CREATE PROCEDURE USP_OBTENER_INSTRUCTORES_BUSQUEDA (@IDCAT INT, @IDSUB INT)
AS
BEGIN
SELECT P.COD_USU, U.NOM_USU, U.APA_USU, U.AMA_USU, (SELECT PU.ESPECIALIDAD FROM TB_PERFIL_USUARIO PU WHERE PU.COD_USU=P.COD_USU),
		'10KM',5,4,(SELECT PU.VERIFICADO FROM TB_PERFIL_USUARIO PU WHERE PU.COD_USU=P.COD_USU)
FROM TB_PREFERENCIA_ENSENANZA P
INNER JOIN
TB_USUARIO U
ON U.COD_USU=P.COD_USU
WHERE U.ID_ROL=1 AND P.ID_CAT=@IDCAT AND P.ID_SUB=@IDSUB;
END
GO

IF OBJECT_ID('USP_OBTENER_CATEGORIAS') IS NOT NULL
BEGIN
DROP PROCEDURE USP_OBTENER_CATEGORIAS;
END
GO

CREATE PROCEDURE USP_OBTENER_CATEGORIAS
AS
BEGIN
SELECT ID_CAT, DES_CAT FROM TB_CATEGORIA_CURSO
END
GO

IF OBJECT_ID('USP_OBTENER_SUBCATEGORIAS') IS NOT NULL
BEGIN
DROP PROCEDURE USP_OBTENER_SUBCATEGORIAS;
END
GO

CREATE PROCEDURE USP_OBTENER_SUBCATEGORIAS (@IDCAT INT)
AS
BEGIN
SELECT ID_SUB, DES_SUB FROM TB_SUBCATEGORIAS_CURSO
WHERE ID_CAT=@IDCAT
END
GO

IF OBJECT_ID('USP_OBTENER_UBICACION_USUARIO') IS NOT NULL
BEGIN
DROP PROCEDURE USP_OBTENER_UBICACION_USUARIO;
END
GO

CREATE PROCEDURE USP_OBTENER_UBICACION_USUARIO (@CODUSU INT)
AS
BEGIN
SELECT LAT_USU, LONG_USU FROM TB_UBICACION_USUARIO
WHERE COD_USU=@CODUSU
END
GO

IF OBJECT_ID('USP_OBTENER_UBICACIONES_USUARIOS') IS NOT NULL
BEGIN
DROP PROCEDURE USP_OBTENER_UBICACIONES_USUARIOS;
END
GO

CREATE PROCEDURE USP_OBTENER_UBICACIONES_USUARIOS (@CODUSUORIG INT, @CODUSUOBJ INT)
AS
BEGIN
SELECT U1.LAT_USU AS LAT_ORIG, U1.LONG_USU AS LONG_ORIG,
(SELECT U3.LAT_USU FROM TB_UBICACION_USUARIO U3 WHERE U3.COD_USU=@CODUSUOBJ) AS LAT_OBJ,
(SELECT U2.LONG_USU FROM TB_UBICACION_USUARIO U2 WHERE U2.COD_USU=@CODUSUOBJ) AS LONG_OBJ
FROM TB_UBICACION_USUARIO U1 WHERE U1.COD_USU=@CODUSUORIG
END
GO

IF OBJECT_ID('USP_OBTENER_PERFIL_INSTRUCTOR') IS NOT NULL
BEGIN
DROP PROCEDURE USP_OBTENER_PERFIL_INSTRUCTOR;
END
GO

CREATE PROCEDURE USP_OBTENER_PERFIL_INSTRUCTOR (@CODUSU INT)
AS
BEGIN
SELECT U.COD_USU, U.NOM_USU, U.APA_USU, U.AMA_USU, PU.ESPECIALIDAD, PU.SOBRE_MI, PU.DIR_CV, 5, PU.VERIFICADO,CAST(1 AS BIT),UU.LAT_USU, UU.LONG_USU
FROM TB_USUARIO U 
INNER JOIN TB_PERFIL_USUARIO PU
ON U.COD_USU=PU.COD_USU
INNER JOIN TB_UBICACION_USUARIO UU
ON U.COD_USU=UU.COD_USU
WHERE U.ID_ROL=1 AND U.COD_USU=@CODUSU
END
GO

-- LAS INSERCIONES

INSERT INTO TB_ROL VALUES	(1,'Instructor'),
							(2,'Alumno'),
							(3,'Admin')
GO

INSERT INTO TB_USUARIO VALUES	('Arena Liset','Rojas','Rojas','arenita@gmail.com','arenita','bella',2),
								('Mila Luna','Rojas','Rojas','mila@gmail.com','mila','luna',2),
								('D�maso D�maso','L�pez','L�pez','damaso@gmail.com','damaso','damaso',1),
								('Jos� Ant�car','Ant�car','Ant�car','antucar@gmail.com','antucar','antucar',1),
								('Gricardov','Coraz�n','De Le�n','gricardov@gmail.com','gricardov','gricardov',3),
								('Juanito','Perez','Perez','jperez@gmail.com','jperez','jperez',2),
								('Bruno','Mars','Mars','bmars@gmail.com','bmars','bmars',2),
								('Chico','Aledan','Aledan','caledan@gmail.com','caledan','caledan',2),
								('Lidia','S�nchez','S�nchez','plsanche@gmail.com','plsanche','plsanche',1),
								('Ysa','Bella','Bella','ysabella@gmail.com','ysabella','ysabella',2),
								('Ra�l','Jim�nez','Drago','drago@gmail.com','drago','drago',1)
GO

INSERT INTO TB_UBICACION_USUARIO	VALUES	(1001,-12.162446,-76.997595),
											(1002,-12.164681,-76.986371),
											(1003,-12.164031,-76.991758),
											(1004,-12.160638,-76.972015),
											(1005,-12.164744,-76.988309),
											(1006,-12.177455,-77.019825),
											(1007,-12.193731,-77.002575),
											(1008,-12.181062,-76.938708),
											(1009,-12.183481,-70.548154),
											(1010,-12.187578,-69.415741),
											(1011,-11.999847,-72.154871)

SELECT*FROM TB_UBICACION_USUARIO
SELECT*FROM TB_USUARIO
INSERT INTO TB_CATEGORIA_CURSO VALUES	(1,'Ciencias naturales'),
										(2,'Ciencias exactas'),
										(3,'Ciencias de la salud'),
										(4,'Ciencias del comportamiento'),
										(5,'Ciencias pol�ticas'),
										(6,'Ciencias de la comunicaci�n'),
										(7,'Ciencias de la computaci�n'),
										(8,'Ciencias sociales'),
										(9,'Artes manuales'),
										(10,'M�sica'),
										(11,'Danzas'),
										(12,'Deportes y entrenamiento'),
										(13,'Juegos de habilidad'),
										(14,'Teatro'),
										(15,'Relajaci�n y calidad de vida'),
										(16,'Idiomas'),
										(17,'Preparaci�n preuniversitaria'),
										(18,'Preparaci�n ex�menes internacionales'),
										(19,'Asesor�a de tesis'),
										(20,'Soluci�n de ex�menes')
GO

INSERT INTO TB_SUBCATEGORIAS_CURSO VALUES	(1,1,'Biolog�a'),
											(2,1,'Zoolog�a'),
											(3,1,'Bot�nica'),
											(4,2,'Matem�ticas'),
											(5,2,'F�sica'),
											(6,2,'Qu�mica'),
											(7,3,'Incisiones'),
											(8,3,'Diagn�stico'),
											(9,3,'Evaluaci�n'),
											(10,3,'Inyectables'),
											(11,13,'Ajedrez'),
											(12,13,'Go'),
											(13,16,'Ingl�s'),
											(14,16,'Franc�s'),
											(15,7,'SQL Server'),
											(16,7,'Programaci�n en Java'),
											(17,7,'Programaci�n .NET'),
											(18,7,'Dise�o web'),
											(19,7,'Base de datos')
GO

INSERT INTO TB_PREFERENCIA_APRENDIZAJE VALUES	(1001,2,4),
												(1001,2,5),
												(1002,1,1),
												(1002,7,16),
												(1002,7,17),
												(1006,7,16),
												(1007,7,17),
												(1008,2,5),
												(1008,2,6),
												(1010,1,3),
												(1010,2,5),
												(1010,3,9)
GO

INSERT INTO TB_PREFERENCIA_ENSENANZA VALUES		(1003,7,15),
												(1003,7,17),
												(1003,7,18),
												(1004,7,16),
												(1009,5,2),
												(1009,3,9),
												(1009,7,16),
												(1009,7,17),
												(1003,5,2),
												(1011,7,15),
												(1011,5,2)
GO

select*from TB_USUARIO
select*from TB_PREFERENCIA_APRENDIZAJE
select*from TB_PREFERENCIA_ENSENANZA
select*from TB_PERFIL_USUARIO

SELECT P.COD_USU, U.NOM_USU, U.APA_USU, U.AMA_USU, (SELECT PU.ESPECIALIDAD FROM TB_PERFIL_USUARIO PU WHERE PU.COD_USU=P.COD_USU),
		'10KM',5,4,(SELECT PU.VERIFICADO FROM TB_PERFIL_USUARIO PU WHERE PU.COD_USU=P.COD_USU),C.ID_CAT, C.DES_CAT, SC.ID_SUB, SC.DES_SUB
FROM TB_PREFERENCIA_ENSENANZA P
INNER JOIN
TB_USUARIO U
ON U.COD_USU=P.COD_USU
INNER JOIN
TB_CATEGORIA_CURSO C
ON P.ID_CAT=C.ID_CAT
INNER JOIN
TB_SUBCATEGORIAS_CURSO SC
ON P.ID_SUB=SC.ID_SUB

IF OBJECT_ID('USP_OBTENER_INSTRUCTORES') IS NOT NULL
BEGIN
DROP PROCEDURE USP_OBTENER_INSTRUCTORES;
END
GO

CREATE PROCEDURE USP_OBTENER_INSTRUCTORES
AS
BEGIN
SELECT P.COD_USU, U.NOM_USU, U.APA_USU, U.AMA_USU, (SELECT PU.ESPECIALIDAD FROM TB_PERFIL_USUARIO PU WHERE PU.COD_USU=P.COD_USU),
		'10KM',5,4,(SELECT PU.VERIFICADO FROM TB_PERFIL_USUARIO PU WHERE PU.COD_USU=P.COD_USU)
FROM TB_PREFERENCIA_ENSENANZA P
INNER JOIN
TB_USUARIO U
ON U.COD_USU=P.COD_USU
WHERE U.ID_ROL=1
END
GO
