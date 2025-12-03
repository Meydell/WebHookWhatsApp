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
    /// Actualmente permite enviar mensajes de texto, pero puede ampliarse para:
    /// - Plantillas (templates)
    /// - Imágenes, audio, documentos
    /// - Botones interactivos
    /// - Mensajes con contexto
    /// </summary>
    public class WhatsAppSender
    {
        /// <summary>
        /// Cliente HTTP reutilizable para optimizar conexiones.
        /// </summary>
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Token utilizado para autenticar solicitudes contra la API de WhatsApp Cloud.
        /// </summary>
        private readonly string _token;

        /// <summary>
        /// Identificador del número de teléfono registrado en Meta.
        /// </summary>
        private readonly string _phoneNumberId;

        /// <summary>
        /// Inicializa una nueva instancia del servicio utilizando la configuración del sistema.
        /// </summary>
        /// <param name="config">Proveedor de configuración que contiene las claves:
        /// WhatsApp:Token y WhatsApp:PhoneNumberId.</param>
        public WhatsAppSender(IConfiguration config)
        {
            _httpClient = new HttpClient();

            _token = config["WhatsApp:Token"]
                ?? throw new ArgumentNullException(nameof(_token),
                    "El token de acceso para WhatsApp no está configurado.");

            _phoneNumberId = config["WhatsApp:PhoneNumberId"]
                ?? throw new ArgumentNullException(nameof(_phoneNumberId),
                    "El PhoneNumberId de WhatsApp no está configurado.");
        }

        /// <summary>
        /// Envía un mensaje de texto simple hacia un usuario específico mediante la API
        /// de WhatsApp Cloud.
        /// </summary>
        /// <param name="to">Número destino en formato internacional (Ej: 50584593041).</param>
        /// <param name="message">Contenido textual del mensaje a enviar.</param>
        public async Task EnviarMensajeAsync(string to, string message)
        {
            if (string.IsNullOrWhiteSpace(to))
                throw new ArgumentException("El número de destino no puede estar vacío.", nameof(to));

            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("El mensaje no puede estar vacío.", nameof(message));

            // Construcción del payload requerido por Meta
            var payload = new
            {
                messaging_product = "whatsapp",
                to = to,
                type = "text",
                text = new
                {
                    body = message
                }
            };

            string json = JsonConvert.SerializeObject(payload);

            // Configurar encabezado de autorización
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _token);

            string url = $"https://graph.facebook.com/v22.0/{_phoneNumberId}/messages";

            try
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                Console.WriteLine($"📨 Enviando mensaje a {to}...");

                var response = await _httpClient.PostAsync(url, content);
                string apiResponse = await response.Content.ReadAsStringAsync();

                Console.WriteLine("📤 Respuesta de WhatsApp API:");
                Console.WriteLine(apiResponse);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("⚠️ Advertencia: Meta devolvió un error al enviar el mensaje.");
                    Console.WriteLine($"Código: {response.StatusCode}");
                }
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine("❌ Error HTTP al comunicarse con WhatsApp API:");
                Console.WriteLine(httpEx.Message);
                throw; // Re-lanzamos para permitir logging externo si existe
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error inesperado al enviar mensaje a WhatsApp:");
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
