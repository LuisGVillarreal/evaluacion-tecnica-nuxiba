-- Ejercicio 2.2: Usuario que MENOS tiempo ha estado logueado
-- Es la misma consulta que el ejercicio 2.1 pero se ordena de menor a mayor (ASC)

USE NuxibaDB;
GO

-- Paso 1: Se numeran los logins y logouts por separado para cada usuario
WITH Numerados AS (
    SELECT 
        User_id,
        TipoMov,
        fecha,
        ROW_NUMBER() OVER (PARTITION BY User_id, TipoMov ORDER BY fecha) AS Numero
    FROM ccloglogin
),

-- Paso 2: Se une cada login con su logout
Sesiones AS (
    SELECT 
        logins.User_id,
        logins.fecha AS FechaLogin,
        logouts.fecha AS FechaLogout,
        DATEDIFF(SECOND, logins.fecha, logouts.fecha) AS Segundos
    FROM Numerados logins
    INNER JOIN Numerados logouts 
        ON logins.User_id = logouts.User_id 
        AND logins.Numero = logouts.Numero
    WHERE logins.TipoMov = 1   -- login
      AND logouts.TipoMov = 0  -- logout
)

-- Paso 3: Se suman los segundos por usuario y se muestra el que tiene MENOS tiempo
SELECT TOP 1
    User_id,
    CAST(SUM(Segundos) / 86400 AS VARCHAR) + ' dias, ' +
    CAST((SUM(Segundos) % 86400) / 3600 AS VARCHAR) + ' horas, ' +
    CAST((SUM(Segundos) % 3600) / 60 AS VARCHAR) + ' minutos, ' +
    CAST(SUM(Segundos) % 60 AS VARCHAR) + ' segundos' AS TiempoTotal
FROM Sesiones
GROUP BY User_id
ORDER BY SUM(Segundos) ASC;  -- ASC para obtener el menor
GO
