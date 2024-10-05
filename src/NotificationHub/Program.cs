using Common.IntegrationEvents.Kafka;
using Common.Mediatr;
using Common.Middleware;
using Common.Seq;
using NotificationHub.Consumer;
using NotificationHub.Notifications;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var seqUrl = builder.Configuration["Seq:Server"] ?? throw new ArgumentException("Seq server url needs to be configured");
var kafkaUrl = builder.Configuration["Kafka:Server"] ?? throw new ArgumentException("Kafka server url needs to be configured");

Log.Logger = Seq.CreateSeqLogger(seqUrl);
builder.Host.UseSerilog();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatrWithPipelines(typeof(Program).Assembly);
builder.Services.AddKafkaIntegrationEvents(kafkaUrl);

builder.Services.AddNotificatioNHub();
builder.Services.AddConsumerBackgroundService(builder.Configuration);

var app = builder.Build();

app.UseNotificationHub();
app.UseCommonMiddleware();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();


app.Run();