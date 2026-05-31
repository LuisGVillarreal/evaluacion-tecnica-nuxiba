-- =============================================
-- Script para insertar datos de prueba
-- Basado en el archivo CCenterRIA.xlsx
-- =============================================

USE NuxibaDB;
GO

-- Insertar Areas
SET IDENTITY_INSERT ccRIACat_Areas ON;

INSERT INTO ccRIACat_Areas (IDArea, AreaName, StatusArea, CreateDate) VALUES
(1, 'Default', 1, '2021-09-03 17:32:30'),
(2, 'BBVA', 1, '2022-10-03 17:32:30'),
(3, 'Banamex', 1, '2024-09-30 17:32:30');

SET IDENTITY_INSERT ccRIACat_Areas OFF;
GO

-- Insertar Users
SET IDENTITY_INSERT ccUsers ON;

INSERT INTO ccUsers (User_id, Login, Nombres, ApellidoPaterno, ApellidoMaterno, Password, TipoUser_id, Status, fCreate, IDArea, LastLoginAttempt) VALUES
(70, 'jperez', 'Juan', 'Perez', 'Lopez', '123456', 1, 1, '2022-01-10 08:00:00', 2, NULL),
(90, 'mgarcia', 'Maria', 'Garcia', 'Martinez', '123456', 1, 1, '2022-03-15 09:00:00', 3, NULL),
(92, 'crodriguez', 'Carlos', 'Rodriguez', 'Sanchez', '123456', 1, 1, '2022-05-20 10:00:00', 1, NULL);

SET IDENTITY_INSERT ccUsers OFF;
GO

-- Insertar registros de login/logout
INSERT INTO ccloglogin (User_id, Extension, TipoMov, fecha) VALUES
(92, 100, 1, '2023-01-02 08:00:00'),
(92, 100, 0, '2023-01-02 17:30:00'),
(92, 100, 1, '2023-01-03 08:00:00'),
(92, 100, 0, '2023-01-03 18:00:00'),
(92, 100, 1, '2023-02-01 07:30:00'),
(92, 100, 0, '2023-02-01 16:45:00'),
(92, 100, 1, '2023-02-02 08:00:00'),
(92, 100, 0, '2023-02-02 17:00:00'),

-- User 90: el que menos tiempo logueado (segun ejemplo del README)
(90, 200, 1, '2023-01-05 09:00:00'),
(90, 200, 0, '2023-01-05 14:00:00'),
(90, 200, 1, '2023-01-06 10:00:00'),
(90, 200, 0, '2023-01-06 13:30:00'),
(90, 200, 1, '2023-02-10 09:00:00'),
(90, 200, 0, '2023-02-10 12:00:00'),

-- User 70: datos para promedio por mes
(70, 300, 1, '2023-01-10 08:00:00'),
(70, 300, 0, '2023-01-10 16:00:00'),
(70, 300, 1, '2023-01-11 07:30:00'),
(70, 300, 0, '2023-01-11 15:30:00'),
(70, 300, 1, '2023-01-12 08:00:00'),
(70, 300, 0, '2023-01-12 17:00:00'),
(70, 300, 1, '2023-02-06 08:30:00'),
(70, 300, 0, '2023-02-06 16:30:00'),
(70, 300, 1, '2023-02-07 09:00:00'),
(70, 300, 0, '2023-02-07 17:30:00');

GO

PRINT 'Datos de prueba insertados correctamente.';
GO
