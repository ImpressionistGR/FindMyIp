using FindMyIp.Configuration;
using FindMyIp.Domain.Providers;
using FindMyIp.Domain.Services;
using FindMyIp.Infrastructure.Data;
using Hangfire;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddFindMyIp(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHangfireDashboard();

RecurringJob.AddOrUpdate<IpUpdateService>(
    "UpdateIpInformationJob", s => s.UpdateIpInformation(),
    Cron.Hourly);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
