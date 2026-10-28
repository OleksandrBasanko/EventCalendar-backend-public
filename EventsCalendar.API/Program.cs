using EventsCalendar.Application;
using EventsCalendar.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi;
using System.Reflection;
using EventsCalendar.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "EventsCalendar API",
        Version = "v1",
        Description = "This is a sample EventsCalendar server. You can search for scheduled events, view calendar dates, and manage subscriptions. Authorized users can subscribe to events."
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Введи токен у форматі: Bearer {твій токен}"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });

    var apiXmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var apiXmlPath = Path.Combine(AppContext.BaseDirectory, apiXmlFile);
    if (File.Exists(apiXmlPath))
    {
        options.IncludeXmlComments(apiXmlPath);
    }

    var applicationXmlPath = Path.Combine(AppContext.BaseDirectory, "EventsCalendar.Application.xml");
    if (File.Exists(applicationXmlPath))
    {
        options.IncludeXmlComments(applicationXmlPath);
    }
});

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

// Проблема: Сервіс RoleManager (і DbContext, який лежить під ним) зареєстровані в Dependency Injection як Scoped
// Це означає, що вони мають жити тільки в межах одного HTTP-запиту від користувача
// Рішення: Оскільки під час запуску програми (Program.cs) жодного HTTP-запиту ще немає, ми вручну створюємо тимчасовий контейнер (Scope)
// Конструкція using (...) автоматично закриє та знищить цей тимчасовий scope (і закриє з'єднання з базою даних) одразу після виходу з фігурних дужок
using (var scope = app.Services.CreateScope())
{
    // Витягує з контейнера залежностей готовий об'єкт RoleManager, який вміє працювати з таблицею ролей у вашій базі даних (використовуючи Guid як тип первинного ключа)
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    // перевіряє чи існують базові ролі "user", "admin"  якщо ні то створить їх
    foreach (var role in new[] { "user", "admin" })
    {
        // перевіряє чи роль є
        if (!await roleManager.RoleExistsAsync(role))
            // створює її
            await roleManager.CreateAsync(new IdentityRole<Guid>(role));
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// хто робить запит
// перехоплює JWT-токен та перевіряє його підпис та термін придатності, розшифровує його і записує дані користувача (ID, Email, Role) у об'єкт HttpContext.User
app.UseAuthentication(); 
// перевіряє чи дозволено користувачу робити запит ( наприклад [Authorize(Roles = "admin")] )
app.UseAuthorization();
// Перенаправляє перевірений запит до конкретного C# контролера та його методу (наприклад, зіставляє URL /api/Events/123 з методом GetEventById(string id) у EventsController)
app.MapControllers();

app.Run();