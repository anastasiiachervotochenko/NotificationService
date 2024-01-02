using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NotificationService.Data;
using NotificationService.Domain.Contracts.Interfaces;
using NotificationService.Domain.Contracts.Models.RequestModel;
using NotificationService.Domain.Services.App;
using NotificationService.Domain.Services.Email;
using NotificationService.Manager;
using NotificationService.Validators;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddScoped<IAppManager, AppManager>();
services.AddScoped<IAppService, AppService>();
services.AddScoped<IMailService, MailService>();
services.AddScoped<IValidator<CreateUserRequestModel>, CreateUserRequestModelValidator>();
services.AddScoped<IValidator<CreateNotificationRequestModel>, CreateNotificationRequestModelValidator>();

services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();