using Microsoft.EntityFrameworkCore;
using NuxibaAPI.Controllers;
using NuxibaAPI.Data;
using NuxibaAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace NuxibaAPI.Tests;

public class ReportsControllerTests
{
    // Se crea una BD en memoria con datos de prueba para cada test
    private AppDbContext GetContextConDatos()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);

        // Se inserta un area de prueba
        context.Areas.Add(new Area
        {
            IDArea = 1,
            AreaName = "Soporte",
            StatusArea = 1,
            CreateDate = DateTime.Now
        });

        // Se inserta un usuario de prueba
        context.Users.Add(new User
        {
            User_id = 1,
            Login = "jperez",
            Nombres = "Juan",
            ApellidoPaterno = "Perez",
            ApellidoMaterno = "Lopez",
            Password = "123456",
            TipoUser_id = 1,
            Status = 1,
            fCreate = DateTime.Now,
            IDArea = 1
        });

        // Se insertan pares de login/logout para calcular horas
        // Login: 08:00 → Logout: 16:00 = 8 horas
        context.Logins.Add(new Login { User_id = 1, Extension = 100, TipoMov = 1, Fecha = new DateTime(2023, 1, 10, 8, 0, 0) });
        context.Logins.Add(new Login { User_id = 1, Extension = 100, TipoMov = 0, Fecha = new DateTime(2023, 1, 10, 16, 0, 0) });

        // Login: 09:00 → Logout: 17:00 = 8 horas (total: 16 horas)
        context.Logins.Add(new Login { User_id = 1, Extension = 100, TipoMov = 1, Fecha = new DateTime(2023, 1, 11, 9, 0, 0) });
        context.Logins.Add(new Login { User_id = 1, Extension = 100, TipoMov = 0, Fecha = new DateTime(2023, 1, 11, 17, 0, 0) });

        context.SaveChanges();
        return context;
    }

    // Verifica que el CSV se descargue correctamente como archivo
    [Fact]
    public async Task DownloadCsv_DebeRetornarArchivoCsv()
    {
        var context = GetContextConDatos();
        var controller = new ReportsController(context);

        var result = await controller.DownloadCsv();

        // Debe retornar un FileContentResult (archivo descargable)
        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal("text/csv", fileResult.ContentType);
        Assert.Equal("reporte_horas.csv", fileResult.FileDownloadName);
    }

    // Verifica que el CSV contenga los encabezados correctos
    [Fact]
    public async Task DownloadCsv_DebeContenerEncabezados()
    {
        var context = GetContextConDatos();
        var controller = new ReportsController(context);

        var result = await controller.DownloadCsv();

        var fileResult = Assert.IsType<FileContentResult>(result);
        var contenido = System.Text.Encoding.UTF8.GetString(fileResult.FileContents);

        // Se verifica que la primera linea tenga los encabezados
        Assert.Contains("NombreUsuario,NombreCompleto,Area,TotalHorasTrabajadas", contenido);
    }

    // Verifica que el CSV contenga los datos del usuario
    [Fact]
    public async Task DownloadCsv_DebeContenerDatosDelUsuario()
    {
        var context = GetContextConDatos();
        var controller = new ReportsController(context);

        var result = await controller.DownloadCsv();

        var fileResult = Assert.IsType<FileContentResult>(result);
        var contenido = System.Text.Encoding.UTF8.GetString(fileResult.FileContents);

        // Se verifica que el CSV contenga el usuario y su area
        Assert.Contains("jperez", contenido);
        Assert.Contains("Juan Perez Lopez", contenido);
        Assert.Contains("Soporte", contenido);
    }

    // Verifica que el total de horas sea correcto (8 + 8 = 16 horas)
    [Fact]
    public async Task DownloadCsv_DebeCalcularHorasCorrectamente()
    {
        var context = GetContextConDatos();
        var controller = new ReportsController(context);

        var result = await controller.DownloadCsv();

        var fileResult = Assert.IsType<FileContentResult>(result);
        var contenido = System.Text.Encoding.UTF8.GetString(fileResult.FileContents);

        // 8 horas + 8 horas = 16.00 horas
        Assert.Contains("16.00", contenido);
    }

    // Verifica que funcione con una BD sin logins (0 horas)
    [Fact]
    public async Task DownloadCsv_DebeRetornarCeroHoras_CuandoNoHayLogins()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);

        // Se inserta usuario sin logins
        context.Areas.Add(new Area { IDArea = 1, AreaName = "IT", StatusArea = 1, CreateDate = DateTime.Now });
        context.Users.Add(new User
        {
            User_id = 1, Login = "test", Nombres = "Test", ApellidoPaterno = "User",
            ApellidoMaterno = "One", Password = "123", TipoUser_id = 1, Status = 1,
            fCreate = DateTime.Now, IDArea = 1
        });
        context.SaveChanges();

        var controller = new ReportsController(context);

        var result = await controller.DownloadCsv();

        var fileResult = Assert.IsType<FileContentResult>(result);
        var contenido = System.Text.Encoding.UTF8.GetString(fileResult.FileContents);

        // Sin logins, las horas deben ser 0.00
        Assert.Contains("0.00", contenido);
    }
}
