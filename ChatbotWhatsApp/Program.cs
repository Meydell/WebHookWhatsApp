using ChatbotWhatsApp.Services;

var builder = WebApplication.CreateBuilder(args);

// ============================================================================
//                        CONFIGURACIÓN DE SERVICIOS
// ============================================================================
// Este bloque registra todos los servicios que el sistema necesita para
// funcionar en producción, incluyendo controladores, servicios propios
// y herramientas de documentación.

// ---------------------------------------------------------------------------
// Controladores Web API
// ---------------------------------------------------------------------------
builder.Services.AddControllers();


// ---------------------------------------------------------------------------
// OpenAPI / Swagger (solo para desarrollo por seguridad)
// ---------------------------------------------------------------------------
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddOpenApi();
}


// ---------------------------------------------------------------------------
// Servicios personalizados del Chatbot
// ---------------------------------------------------------------------------
builder.Services.AddSingleton<MessageProcessor>();
builder.Services.AddSingleton<WhatsAppSender>();


// ============================================================================
//                        CONSTRUCCIÓN DE LA APLICACIÓN
// ============================================================================
var app = builder.Build();


// ============================================================================
//                     CONFIGURACIÓN DEL PIPELINE DE MIDDLEWARE
// ============================================================================

// ---------------------------------------------------------------------------
// Documentación OpenAPI solo en desarrollo
// ---------------------------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    Console.WriteLine("📘 OpenAPI habilitado (solo Development).");
}


// ---------------------------------------------------------------------------
// Railway NO usa HTTPS en plan gratuito
// Desactivamos redirección HTTPS para evitar errores 308 / bucles
// ---------------------------------------------------------------------------
// app.UseHttpsRedirection();


// ---------------------------------------------------------------------------
// Autorización (no necesaria ahora pero útil para futuro)
// ---------------------------------------------------------------------------
app.UseAuthorization();


// ---------------------------------------------------------------------------
// Registro automático de controladores
// ---------------------------------------------------------------------------
app.MapControllers();


// ============================================================================
//                     CONFIGURACIÓN ESPECIAL PARA RAILWAY
// ============================================================================
// Railway asigna dinámicamente el puerto a través de la variable de entorno
// PORT. Si no se usa este puerto, el contenedor no funcionará.

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";

app.Urls.Add($"http://0.0.0.0:{port}");

Console.WriteLine("🚀 Servidor iniciado:");
Console.WriteLine($"    ➤ Entorno: {app.Environment.EnvironmentName}");
Console.WriteLine($"    ➤ Escuchando en puerto: {port}");
Console.WriteLine($"    ➤ URL completa: http://0.0.0.0:{port}");


// ============================================================================
//                           EJECUCIÓN DE LA APLICACIÓN
// ============================================================================
app.Run();
