using Hangfire;
using Streetcode.BLL.Services.BlobStorageService;
using Streetcode.BLL.Services.Tokens;
using Streetcode.WebApi.Extensions;
using Streetcode.WebApi.Utils;
using Streetcode.WebApi.Middlewares;
using Streetcode.WebApi.HttpClients.Configuration;
var builder = WebApplication.CreateBuilder(args);
builder.Host.ConfigureApplication();

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddSwaggerServices();
builder.Services.AddHttpClients(builder.Configuration);
builder.Services.AddCustomServices();
builder.Services.AddIdentityService();
builder.Services.AddPipelineBehaviors();
builder.Services.ConfigureBlob(builder);
builder.Services.ConfigurePayment(builder);
builder.Services.ConfigureInstagram(builder);
builder.Services.ConfigureSerilog(builder);
builder.Services.AddCachingService(builder.Configuration);
builder.Services.AddTokensConfiguration(builder.Configuration);
builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.EnvironmentName == "Local")
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPIv5 v1"));
}
else
{
    app.UseHsts();
}

app.UseMiddleware<GenericExceptionHandlerMiddleware>();

await app.ApplyMigrations();

//await app.SeedDataAsync(); // uncomment for seeding data in local
app.UseCors();
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseHangfireDashboard("/dash");

if (app.Environment.EnvironmentName != "Local")
{
    BackgroundJob.Schedule<WebParsingUtils>(
    wp => wp.ParseZipFileFromWebAsync(), TimeSpan.FromMinutes(1));
    RecurringJob.AddOrUpdate<WebParsingUtils>(
        wp => wp.ParseZipFileFromWebAsync(), Cron.Monthly);
    RecurringJob.AddOrUpdate<BlobService>(
        b => b.CleanBlobStorage(), Cron.Monthly);
    RecurringJob.AddOrUpdate<TokenService>(
        ts => ts.RemoveExpiredRefreshToken(), Cron.Weekly);
}

app.MapControllers();

app.Run();
public partial class Program
{
}