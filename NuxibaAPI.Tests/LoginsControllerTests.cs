using Microsoft.EntityFrameworkCore;
using NuxibaAPI.Controllers;
using NuxibaAPI.Data;
using NuxibaAPI.DTOs;
using NuxibaAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace NuxibaAPI.Tests;

public class LoginsControllerTests
{
    // Crea una base de datos falsa en memoria para cada test.
    // Asi cada test es independiente y no afecta a los demas.
    private AppDbContext GetInMemoryContext()
    {
        // Guid.NewGuid() genera un nombre unico para que cada test tenga su propia BD
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);

        // Insertamos datos base que necesitan los tests:
        // un area y un usuario (User_id=92) para que las validaciones funcionen
        context.Areas.Add(new Area
        {
            IDArea = 1,
            AreaName = "Default",
            StatusArea = 1,
            CreateDate = DateTime.Now
        });

        context.Users.Add(new User
        {
            User_id = 92,
            Login = "crodriguez",
            Nombres = "Carlos",
            ApellidoPaterno = "Rodriguez",
            ApellidoMaterno = "Sanchez",
            Password = "123456",
            TipoUser_id = 1,
            Status = 1,
            fCreate = DateTime.Now,
            IDArea = 1
        });

        context.SaveChanges();
        return context;
    }

    // ===================== GET =====================

    // Verifica que el GET devuelva 200 OK cuando no hay logins en la BD
    [Fact]
    public async Task GetAll_DebeRetornarListaVacia_CuandoNoHayRegistros()
    {
        var context = GetInMemoryContext(); // BD vacia (sin logins)
        var controller = new LoginsController(context);

        var result = await controller.GetAll(); // Llama al endpoint

        var okResult = Assert.IsType<OkObjectResult>(result); // Verifica que sea 200
        Assert.Equal(200, okResult.StatusCode);
    }

    // Verifica que el GET devuelva registros cuando si existen logins
    [Fact]
    public async Task GetAll_DebeRetornarRegistros_CuandoExistenLogins()
    {
        var context = GetInMemoryContext();
        // Insertamos un login de prueba
        context.Logins.Add(new Login { User_id = 92, Extension = 100, TipoMov = 1, Fecha = DateTime.Now.AddHours(-2) });
        context.SaveChanges();

        var controller = new LoginsController(context);

        var result = await controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }

    // ===================== POST =====================

    // Verifica que se cree un login correctamente cuando los datos son validos
    [Fact]
    public async Task Create_DebeCrearLogin_CuandoDatosValidos()
    {
        var context = GetInMemoryContext();
        var controller = new LoginsController(context);

        // DTO con datos validos: usuario existe, fecha pasada, primer login
        var dto = new LoginCreateDto
        {
            User_id = 92,
            Extension = 100,
            TipoMov = 1,
            Fecha = DateTime.Now.AddHours(-1)
        };

        var result = await controller.Create(dto);

        // Debe retornar 201 Created y el registro debe existir en la BD
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(201, createdResult.StatusCode);
        Assert.Equal(1, context.Logins.Count());
    }

    // Verifica que retorne error cuando el User_id no existe en la BD
    [Fact]
    public async Task Create_DebeRetornarBadRequest_CuandoUserIdNoExiste()
    {
        var context = GetInMemoryContext();
        var controller = new LoginsController(context);

        // User_id 999 no existe en nuestra BD de prueba
        var dto = new LoginCreateDto
        {
            User_id = 999,
            Extension = 100,
            TipoMov = 1,
            Fecha = DateTime.Now.AddHours(-1)
        };

        var result = await controller.Create(dto);

        // Debe retornar 400 BadRequest
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    // Verifica que retorne error cuando la fecha es futura
    [Fact]
    public async Task Create_DebeRetornarBadRequest_CuandoFechaEsFutura()
    {
        var context = GetInMemoryContext();
        var controller = new LoginsController(context);

        // Fecha 5 dias en el futuro = invalida
        var dto = new LoginCreateDto
        {
            User_id = 92,
            Extension = 100,
            TipoMov = 1,
            Fecha = DateTime.Now.AddDays(5)
        };

        var result = await controller.Create(dto);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    // Verifica que no se pueda hacer login si el ultimo movimiento fue un login (sin logout)
    [Fact]
    public async Task Create_DebeRetornarBadRequest_CuandoLoginSinLogoutPrevio()
    {
        var context = GetInMemoryContext();

        // Ya existe un login previo sin su logout correspondiente
        context.Logins.Add(new Login { User_id = 92, Extension = 100, TipoMov = 1, Fecha = DateTime.Now.AddHours(-2) });
        context.SaveChanges();

        var controller = new LoginsController(context);

        // Intenta hacer otro login sin haber hecho logout primero
        var dto = new LoginCreateDto
        {
            User_id = 92,
            Extension = 100,
            TipoMov = 1,
            Fecha = DateTime.Now.AddHours(-1)
        };

        var result = await controller.Create(dto);

        // Debe retornar 400 porque falta el logout
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    // ===================== PUT =====================

    // Verifica que se actualice correctamente un registro existente
    [Fact]
    public async Task Update_DebeActualizar_CuandoDatosValidos()
    {
        var context = GetInMemoryContext();
        // Insertamos un login que vamos a actualizar
        context.Logins.Add(new Login { Id = 1, User_id = 92, Extension = 100, TipoMov = 1, Fecha = DateTime.Now.AddHours(-2) });
        context.SaveChanges();

        var controller = new LoginsController(context);

        // Cambiamos la extension de 100 a 200
        var dto = new LoginUpdateDto
        {
            User_id = 92,
            Extension = 200,
            TipoMov = 1,
            Fecha = DateTime.Now.AddHours(-1)
        };

        var result = await controller.Update(1, dto);

        // Debe retornar 200 OK y el campo Extension debe ser 200
        var okResult = Assert.IsType<OkObjectResult>(result);
        var loginActualizado = context.Logins.Find(1);
        Assert.Equal(200, loginActualizado!.Extension);
    }

    // Verifica que retorne 404 cuando se intenta actualizar un registro que no existe
    [Fact]
    public async Task Update_DebeRetornarNotFound_CuandoIdNoExiste()
    {
        var context = GetInMemoryContext();
        var controller = new LoginsController(context);

        var dto = new LoginUpdateDto
        {
            User_id = 92,
            Extension = 100,
            TipoMov = 1,
            Fecha = DateTime.Now.AddHours(-1)
        };

        // Id 999 no existe en la BD
        var result = await controller.Update(999, dto);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    // ===================== DELETE =====================

    // Verifica que se elimine correctamente un registro existente
    [Fact]
    public async Task Delete_DebeEliminar_CuandoIdExiste()
    {
        var context = GetInMemoryContext();
        context.Logins.Add(new Login { Id = 1, User_id = 92, Extension = 100, TipoMov = 1, Fecha = DateTime.Now.AddHours(-2) });
        context.SaveChanges();

        var controller = new LoginsController(context);

        var result = await controller.Delete(1);

        // Debe retornar 204 NoContent y la tabla debe quedar vacia
        Assert.IsType<NoContentResult>(result);
        Assert.Equal(0, context.Logins.Count());
    }

    // Verifica que retorne 404 cuando se intenta eliminar un registro que no existe
    [Fact]
    public async Task Delete_DebeRetornarNotFound_CuandoIdNoExiste()
    {
        var context = GetInMemoryContext();
        var controller = new LoginsController(context);

        var result = await controller.Delete(999);

        Assert.IsType<NotFoundObjectResult>(result);
    }
}
