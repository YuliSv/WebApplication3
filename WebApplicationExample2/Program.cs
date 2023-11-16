﻿using InvertedSoftware.PLogger.Core;
using WebApplicationExample2.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IMailService, MailService>();

var settings = new PLoggerSettings(builder.Configuration);
builder.Logging.ClearProviders();
builder.Logging.AddPLogger(settings);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.Use((context, next) =>
{
    var request = context.Request;
    var ipAddress = context.Connection.RemoteIpAddress;
    var requestTime = DateTime.Now;

    var logMessage = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString} [:] " +
                     $"T:{requestTime}, " +
                     $"IP:{ipAddress}";

    context.RequestServices.GetRequiredService<ILogger<Program>>().LogInformation(logMessage);

    return next();
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
