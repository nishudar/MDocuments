using Common.DomainEvents;
using Common.IntegrationEvents.Infrastructure;
using Common.Mediatr;
using Common.Middleware;
using Common.Seq;
using Documents.Api;
using Documents.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var seqUrl = builder.Configuration["Seq:Server"] ??
             throw new ArgumentException("Seq server url needs to be configured");
var kafkaUrl = builder.Configuration["Kafka:Server"] ??
               throw new ArgumentException("Kafka server url needs to be configured");
var storageServiceUrl = builder.Configuration["Storage:BaseUrl"] ??
                        throw new ArgumentException("Services storage url needs to be configured");

Log.Logger = Seq.CreateSeqLogger(seqUrl);
builder.Host.UseSerilog();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatrWithPipelines(typeof(Program).Assembly);
builder.Services.AddKafkaIntegrationEvents(kafkaUrl);

builder.Services.AddDomainEventHandlers();
builder.Services.AddInfrastructure(storageServiceUrl, kafkaUrl);

var app = builder.Build();

app.UseCommonMiddleware();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
var operationTimeout = new TimeSpan(0, 0, 1, 0);

app.MapProcessEndpoints(operationTimeout);
app.MapDocumentsEndpoints(operationTimeout);

await app.RunAsync();