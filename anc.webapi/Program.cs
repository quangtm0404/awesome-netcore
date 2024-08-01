using anc.applications;
using anc.repositories;
using anc.webapi;
using anc.webapi.Policy;
using Microsoft.AspNetCore.RateLimiting;
using Scrutor;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCoreServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.Scan(scan => scan
		 .FromAssemblies(anc.applications.AssemblyReference.Assembly,
		 anc.repositories.AssemblyReference.Assembly,
		 ApiAssemblyReference.Assembly)
		 .AddClasses()
		 .UsingRegistrationStrategy(RegistrationStrategy.Skip)
		 .AsMatchingInterface()
		 .WithScopedLifetime());
var app = builder.Build();
app.UseRateLimiter(new RateLimiterOptions()
.AddPolicy("apiKey", new APIRateLimitPolicy()));

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();
app.MapHealthChecks("/healthz");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers()
    .RequireRateLimiting("apiKey");

app.Run();
