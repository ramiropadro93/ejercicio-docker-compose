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

app.MapPost("/finalize-file", async (ILogger<Program> logger) =>
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
        var finalContent = $"{existingContent}Finalizado por Proyecto3 en: {timestamp}\n";
        
        await File.WriteAllTextAsync(filePath, finalContent);
        
        logger.LogInformation("Archivo finalizado exitosamente en {FilePath}", filePath);
        
        return Results.Ok(new { 
            message = "Archivo finalizado exitosamente", 
            timestamp = timestamp,
            service = "proyecto3",
            status = "completed"
        });
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error al finalizar archivo");
        return Results.Problem("Error interno del servidor");
    }
});

app.MapGet("/read-file", async (ILogger<Program> logger) =>
{
    try
    {
        var filePath = "/app/shared/processed-file.txt";
        
        if (!File.Exists(filePath))
        {
            return Results.NotFound("Archivo no encontrado");
        }
        
        var content = await File.ReadAllTextAsync(filePath);
        return Results.Ok(new { 
            content = content,
            service = "proyecto3",
            timestamp = DateTime.UtcNow
        });
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error al leer archivo");
        return Results.Problem("Error interno del servidor");
    }
});

app.MapGet("/health", () => Results.Ok(new { service = "Proyecto3", status = "Healthy", timestamp = DateTime.UtcNow }));

app.Run();
