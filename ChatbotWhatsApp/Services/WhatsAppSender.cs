using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace ChatbotWhatsApp.Services
{
    /// <summary>
    /// Servicio responsable de comunicarse con la API oficial de WhatsApp Cloud (Meta).
    /// 
    /// Esta clase encapsula la lógica necesaria para:
    /// 1. Construcción del payload requerido por Meta.
    /// 2. Autenticación mediante token Bearer.
    /// 3. Ejecución de solicitudes HTTP hacia el endpoint de envío de mensajes.
    /// 
    /// Compatible tanto con ejecución local (appsettings.json)
    /// como con despliegue en Railway (variables de entorno).
    /// </summary>
    public class WhatsAppSender
    {
        private readonly HttpClient _httpClient;
        private readonly string _token;
        private readonly string _phoneNumberId;

        /// <summary>
        /// Inicializa el servicio cargando las credenciales desde:
        /// - Variables de entorno (Railway)
        /// - appsettings.json (modo local)
        /// </summary>
        public WhatsAppSender(IConfiguration config)
        {
            _httpClient = new HttpClient();

            // 1. TOKEN → intentar primero desde variable de entorno
            _token =
                Environment.GetEnvironmentVariable("WHATSAPP_TOKEN") ??
                config["WhatsApp:Token"] ??
                throw new Exception("❌ Token de WhatsApp no configurado ni en ENV ni en appsettings.json.");

            // 2. PHONE NUMBER ID → igual que arriba
            _phoneNumberId =
                Environment.GetEnvironmentVariable("WHATSAPP_PHONE_ID") ??
                config["WhatsApp:PhoneNumberId"] ??
                throw new Exception("❌ PhoneNumberId no configurado ni en ENV ni en appsettings.json.");
        }

        /// <summary>
        /// Envía un mensaje de texto simple mediante la API de WhatsApp Cloud.
        /// </summary>
        public async Task EnviarMensajeAsync(string to, string message)
        {
            if (string.IsNullOrWhiteSpace(to))
                throw new ArgumentException("El número de destino no puede estar vacío.", nameof(to));

            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("El mensaje no puede estar vacío.", nameof(message));

            var payload = new
            {
                messaging_product = "whatsapp",
                to,
                type = "text",
                text = new { body = message }
            };

            string json = JsonConvert.SerializeObject(payload);

            // Configurar token Bearer
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _token);

            string url = $"https://graph.facebook.com/v22.0/{_phoneNumberId}/messages";

            try
            {
                Console.WriteLine($"📨 Enviando mensaje a {to}...");

                var response = await _httpClient.PostAsync(
                    url,
                    new StringContent(json, Encoding.UTF8, "application/json")
                );

                string apiResponse = await response.Content.ReadAsStringAsync();

                Console.WriteLine("📤 Respuesta de WhatsApp API:");
                Console.WriteLine(apiResponse);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"⚠ Meta devolvió error HTTP: {response.StatusCode}");
                }
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine("❌ Error de red al comunicarse con WhatsApp API:");
                Console.WriteLine(httpEx.Message);
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error inesperado al enviar mensaje:");
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
