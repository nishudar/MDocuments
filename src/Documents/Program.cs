using Common.DomainEvents;
using Common.IntegrationEvents;
using Common.Mediatr;
using Common.Middleware;
using Common.Seq;
using Documents.Api;
using Documents.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var seqUrl = builder.Configuration["Seq:Server"] ?? throw new ArgumentException("Seq server url needs to be configured");
var kafkaUrl = builder.Configuration["Kafka:Server"] ?? throw new ArgumentException("Kafka server url needs to be configured");
var storageServiceUrl = builder.Configuration["Storage:BaseUrl"] ?? throw new ArgumentException("Services storage url needs to be configured");

Log.Logger = Seq.CreateSeqLogger(seqUrl);
builder.Host.UseSerilog();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatrWithPipelines(typeof(Program).Assembly);
builder.Services.AddKafkaIntegrationEvents(kafkaUrl);

builder.Services.AddDomainEventHandlers();
builder.Services.AddInfrastructure(storageServiceUrl);

builder.Services.AddAntiforgery(options =>
{
    options.SuppressXFrameOptionsHeader = true;
});

var app = builder.Build();

app.UseCommonMiddleware();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
var operationTimeout = new TimeSpan(0, 0, 1, 0);

app.MapUserEndpoints(operationTimeout);
app.MapConsumerEndpoints(operationTimeout);
app.MapProcessEndpoints(operationTimeout);
app.MapDocumentsEndpoints(operationTimeout);

app.Run();