using Attendance.Api.Configurations;
using Attendance.Api.Initializer;
using Attendance.Api.Middlewares;
using Attendance.Common.Constants;

var builder = WebApplication.CreateBuilder(args);

// Configure the application's configuration settings
builder.Configuration.SetBasePath(AppDomain.CurrentDomain.BaseDirectory);
builder.Configuration.AddJsonFile("appsettings.json", false, true);
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true);
builder.Configuration.AddEnvironmentVariables();
// Map AppSettings section in appsettings.json file value to AppSetting model
builder.Configuration.GetSection("AppSettings").Get<AppSettings>(options => options.BindNonPublicProperties = true);

// Add services to the container.
builder.Services
    .AddAutoMapper()
    .AddDatabase()
    .AddService()
    .AddSwagger()
    .AddCORS();

builder.Services.AddControllers().AddNewtonsoftJson(config => {
    config.SerializerSettings.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Final v1"));
}

app.UseRouting();

app.UseCors("AllowAll");

// global error handler
app.UseMiddleware<ErrorHandlerMiddleware>();

// custom jwt auth middleware
app.UseMiddleware<JwtMiddleware>();

app.MapControllers();

DbInitializer.Initialize(app);

app.Run();

