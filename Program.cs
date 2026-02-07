using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/", () =>
{
    return "Hello World! Welcome to my API! I am a software developer with a passion for creating " +
    "innovative solutions. I have experience in various programming languages and frameworks, " +
    "and I enjoy tackling complex problems. Feel free to explore my API and see what I have to offer!";
});

app.MapGet("/test", () =>
{
    var currentServerTime = DateTime.Now;

    //return $"Current server's time is {currentServerTime}. I can pull myself together! ";
    return $"PUSHED FROM GITHUB! TEST VERSION OF MY API! Current server's time is {currentServerTime}. I can pull myself together! ";
})
.WithName("test");

app.MapGet("/db", (IConfiguration config, ILogger<Program> logger) =>
{
    logger.LogInformation("Received request to /db endpoint at {Time}", DateTime.Now);

    var connectionString = config.GetConnectionString("AZURE_SQL_CONNECTION");

    if (string.IsNullOrEmpty(connectionString))
    {
        return Results.NotFound("Connection string 'AZURE_SQL_CONNECTION' not found.");
    }

    var sqlConnection = new SqlConnection(connectionString);
    sqlConnection.Open();

    var sqlCommand = new SqlCommand("SELECT VillageId, Name, Rating FROM Villages", sqlConnection);


    var results = new List<string>();

    using (SqlDataReader reader = sqlCommand.ExecuteReader())
    {
        while (reader.Read())
        {
            var villageId = reader["VillageId"].ToString();
            var name = reader["Name"].ToString();
            var rating = reader["Rating"].ToString();

            results.Add($"VillageId: {villageId}, Name: {name}, Rating: {rating}");
        }
    }

    return Results.Ok(results);
})
.WithName("db");

app.Run();

