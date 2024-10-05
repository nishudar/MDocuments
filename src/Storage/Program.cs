using Common.IntegrationEvents.Kafka;
using Common.Mediatr;
using Common.Middleware;
using Common.Seq;
using Serilog;
using Storage.Api;
using Storage.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
var seqUrl = builder.Configuration["Seq:Server"] ?? throw new ArgumentException("Seq server url needs to be configured");
var kafkaUrl = builder.Configuration["Kafka:Server"] ?? throw new ArgumentException("Kafka server url needs to be configured");
    
Log.Logger = Seq.CreateSeqLogger(seqUrl);
builder.Host.UseSerilog();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatrWithPipelines(typeof(Program).Assembly);
builder.Services.AddKafkaIntegrationEvents(kafkaUrl);

builder.Services.AddInfrastructure("./file-storage");

var app = builder.Build();

app.UseCommonMiddleware();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
var operationTimeout = new TimeSpan(0, 0, 1, 0);
app.MapFileEndpoints(operationTimeout);

app.Run();
