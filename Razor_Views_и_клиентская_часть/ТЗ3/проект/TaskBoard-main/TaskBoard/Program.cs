using System.Diagnostics;
using TaskBoard.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// ===== Задание 1 (TaskBoard). Сервис задач =====
builder.Services.AddSingleton<ITaskService, InMemoryTaskService>();

// ===== Задание 2 (Витрина). Сервисы товаров и корзины =====
builder.Services.AddSingleton<IProductService, InMemoryProductService>();
builder.Services.AddSingleton<ICartService, InMemoryCartService>();

// ===== НОВОЕ. Задание 1.2. Session для AJAX-корзины =====
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// ===== Шаг 2. Замер времени =====
app.Use(async (context, next) =>
{
    var sw = Stopwatch.StartNew();
    await next(context);
    sw.Stop();

    if (!context.Response.HasStarted)
    {
        context.Response.Headers.Append("X-Response-Time-ms",
            sw.ElapsedMilliseconds.ToString());
    }
});

// ===== Шаг 1. Заголовок + логирование =====
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-App-Name", "TaskBoard");

    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("--> {Method} {Path}",
        context.Request.Method, context.Request.Path);

    await next(context);

    logger.LogInformation("<-- {StatusCode}", context.Response.StatusCode);
});

// ===== Шаг 1 (усложнение). /health =====
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/health")
    {
        context.Response.StatusCode = 200;
        await context.Response.WriteAsync("healthy");
        return;
    }
    await next(context);
});

// ===== Уровень 2 (Задание 1). X-Api-Key для /tasks/api/* =====
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/tasks/api"))
    {
        if (!context.Request.Headers.TryGetValue("X-Api-Key", out var key)
            || key != "secret123")
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync("{\"error\":\"Unauthorized\"}");
            return;
        }
    }
    await next(context);
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ===== НОВОЕ. Задание 1.2. Session middleware =====
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();