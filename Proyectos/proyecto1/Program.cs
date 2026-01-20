using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/create-file", async (ILogger<Program> logger) =>
{
    try
    {
        var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
        var content = $"Archivo creado por Proyecto1 en: {timestamp}\n";
        
        var filePath = "/app/shared/processed-file.txt";
        await File.WriteAllTextAsync(filePath, content);
        
        logger.LogInformation("Archivo creado exitosamente en {FilePath}", filePath);
        
        using var httpClient = new HttpClient();
        var notificationData = new { message = "Archivo creado", timestamp = timestamp };
        var json = System.Text.Json.JsonSerializer.Serialize(notificationData);
        var content2 = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await httpClient.PostAsync("http://proyecto2/process-file", content2);
        
        if (response.IsSuccessStatusCode)
        {
            logger.LogInformation("Notificación enviada exitosamente a proyecto2");
            return Results.Ok(new { 
                message = "Archivo creado y notificación enviada", 
                timestamp = timestamp,
                nextService = "proyecto2"
            });
        }
        else
        {
            logger.LogWarning("Error al notificar a proyecto2: {StatusCode}", response.StatusCode);
            return Results.Ok(new { 
                message = "Archivo creado pero error en notificación", 
                timestamp = timestamp 
            });
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error al crear archivo");
        return Results.Problem("Error interno del servidor");
    }
});

app.MapGet("/health", () => Results.Ok(new { service = "Proyecto1", status = "Healthy", timestamp = DateTime.UtcNow }));

app.Run();
