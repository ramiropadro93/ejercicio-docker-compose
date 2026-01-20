using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/process-file", async (ILogger<Program> logger) =>
{
    try
    {
        var filePath = "/app/shared/processed-file.txt";
        
        if (!File.Exists(filePath))
        {
            logger.LogWarning("Archivo no encontrado en {FilePath}", filePath);
            return Results.NotFound("Archivo no encontrado");
        }
        
        var existingContent = await File.ReadAllTextAsync(filePath);
        var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
        var newContent = $"{existingContent}Procesado por Proyecto2 en: {timestamp}\n";
        
        await File.WriteAllTextAsync(filePath, newContent);
        
        logger.LogInformation("Archivo procesado exitosamente en {FilePath}", filePath);
        
        using var httpClient = new HttpClient();
        var notificationData = new { message = "Archivo procesado", timestamp = timestamp };
        var json = System.Text.Json.JsonSerializer.Serialize(notificationData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await httpClient.PostAsync("http://proyecto3/finalize-file", content);
        
        if (response.IsSuccessStatusCode)
        {
            logger.LogInformation("Notificación enviada exitosamente a proyecto3");
            return Results.Ok(new { 
                message = "Archivo procesado y notificación enviada", 
                timestamp = timestamp,
                nextService = "proyecto3"
            });
        }
        else
        {
            logger.LogWarning("Error al notificar a proyecto3: {StatusCode}", response.StatusCode);
            return Results.Ok(new { 
                message = "Archivo procesado pero error en notificación", 
                timestamp = timestamp 
            });
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error al procesar archivo");
        return Results.Problem("Error interno del servidor");
    }
});

app.MapGet("/health", () => Results.Ok(new { service = "Proyecto2", status = "Healthy", timestamp = DateTime.UtcNow }));

app.Run();
