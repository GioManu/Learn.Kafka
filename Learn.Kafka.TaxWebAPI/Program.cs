using FluentValidation;
using Learn.Kafka.Taxi.Shared;
using Learn.Kafka.Taxi.Shared.Interfaces;
using Learn.Kafka.Taxi.Shared.Models;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IVehicleProducer, VehicleCoordsProducerService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IValidator<VehicleSignalRequest>, VehicleSignalRequestValidator>();
builder.Services.AddFluentValidationAutoValidation();

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