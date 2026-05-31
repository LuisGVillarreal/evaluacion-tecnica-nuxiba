-- Ejercicio 2.3: Promedio de tiempo logueado por usuario en cada mes

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

-- Paso 2: Se une cada login con su logout y se extrae el año y mes
Sesiones AS (
    SELECT 
        logins.User_id,
        logins.fecha AS FechaLogin,
        logouts.fecha AS FechaLogout,
        DATEDIFF(SECOND, logins.fecha, logouts.fecha) AS Segundos,
        YEAR(logins.fecha) AS Anio,
        MONTH(logins.fecha) AS Mes
    FROM Numerados logins
    INNER JOIN Numerados logouts 
        ON logins.User_id = logouts.User_id 
        AND logins.Numero = logouts.Numero
    WHERE logins.TipoMov = 1   -- login
      AND logouts.TipoMov = 0  -- logout
)

-- Paso 3: Se calcula el promedio de segundos por usuario, año y mes
SELECT 
    User_id,
    Anio,
    Mes,
    COUNT(*) AS TotalSesiones,
    CAST(AVG(Segundos) / 86400 AS VARCHAR) + ' dias, ' +
    CAST((AVG(Segundos) % 86400) / 3600 AS VARCHAR) + ' horas, ' +
    CAST((AVG(Segundos) % 3600) / 60 AS VARCHAR) + ' minutos, ' +
    CAST(AVG(Segundos) % 60 AS VARCHAR) + ' segundos' AS PromedioTiempo
FROM Sesiones
GROUP BY User_id, Anio, Mes
ORDER BY User_id, Anio, Mes;
GO
