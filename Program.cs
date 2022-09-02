using ProjectsWebApi.Repositories;
using ProjectsWebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Build DatabaseConnectionConfig

DatabaseConnectionConfig connectionConfig;
if (builder.Environment.IsDevelopment() == true)
{
    connectionConfig = builder.Configuration.GetSection("MySql").Get<DatabaseConnectionConfig>();
    Environment.SetEnvironmentVariable("ASPNETCORE_PASSWORD", builder.Configuration["Password"]);
}
else
{
    connectionConfig = new()
    {
        Server = Environment.GetEnvironmentVariable("ASPNETCORE_MYSQL_SERVER") ?? "",
        Port = Int32.Parse(Environment.GetEnvironmentVariable("ASPNETCORE_MYSQL_PORT") ?? "0"),
        Database = Environment.GetEnvironmentVariable("ASPNETCORE_MYSQL_DATABASE") ?? "",
        User = Environment.GetEnvironmentVariable("ASPNETCORE_MYSQL_USER") ?? "",
        Password = Environment.GetEnvironmentVariable("ASPNETCORE_MYSQL_PASSWORD") ?? ""
    };
}

// Build TableSchemasConfiguration

TableSchemasConfiguration schemasConfiguration = builder.Configuration.GetSection("TableSchemasConfiguration").Get<TableSchemasConfiguration>();

var services = builder.Services;

services.AddSingleton(connectionConfig);
services.AddSingleton(schemasConfiguration);

services.AddScoped<IProjectService, ProjectService>();
services.AddScoped<IProjectRepository, ProjectRepository>();

services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddCors(options =>
{
    options.AddPolicy(name: "All", builder =>
    {
        builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("All");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
