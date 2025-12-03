using Microsoft.AspNetCore.Mvc;
using ChatbotWhatsApp.Services;
using Newtonsoft.Json.Linq;

namespace ChatbotWhatsApp.Controllers
{
    /// <summary>
    /// Controlador encargado de gestionar la comunicación directa con
    /// WhatsApp Cloud API a través del Webhook configurado en Meta.
    /// 
    /// Expone dos endpoints:
    ///   - GET /webhook   → Validación inicial del webhook
    ///   - POST /webhook  → Recepción y procesamiento de mensajes entrantes
    /// </summary>
    [ApiController]
    [Route("webhook")]
    public class WebhookController : ControllerBase
    {
        /// <summary>
        /// Token personalizado que Meta utilizará para validar
        /// la autenticidad del webhook durante la configuración.
        /// </summary>
        private const string VERIFY_TOKEN = "miprimertoken";

        private readonly MessageProcessor _processor;
        private readonly WhatsAppSender _sender;

        /// <summary>
        /// Constructor que inyecta las dependencias necesarias para procesar mensajes
        /// y enviar respuestas automáticas utilizando WhatsApp Cloud API.
        /// </summary>
        public WebhookController(MessageProcessor processor, WhatsAppSender sender)
        {
            _processor = processor;
            _sender = sender;
        }

        // --------------------------------------------------------------------
        //                     VALIDACIÓN INICIAL DEL WEBHOOK
        // --------------------------------------------------------------------

        /// <summary>
        /// Endpoint utilizado por Meta durante la fase de verificación del webhook.
        /// Meta enviará parámetros obligatorios para confirmar que el servidor
        /// acepta la suscripción.
        /// 
        /// Si un usuario ingresa manualmente a /webhook sin parámetros,
        /// se mostrará un mensaje informativo en vez de un error 400.
        /// </summary>
        /// <param name="mode">Indica la intención de Meta (ej. "subscribe").</param>
        /// <param name="token">Token de verificación enviado por Meta.</param>
        /// <param name="challenge">Cadena que Meta necesita recibir como respuesta.</param>
        [HttpGet]
        public IActionResult Verify(
            [FromQuery(Name = "hub.mode")] string mode,
            [FromQuery(Name = "hub.verify_token")] string token,
            [FromQuery(Name = "hub.challenge")] string challenge)
        {
            // Si el GET viene vacío (acceso manual), devolvemos un mensaje amigable
            if (string.IsNullOrEmpty(mode) &&
                string.IsNullOrEmpty(token) &&
                string.IsNullOrEmpty(challenge))
            {
                return Ok("Webhook activo. Esperando validación o eventos desde Meta.");
            }

            // Si Meta envió modo y token correctos, completamos la verificación
            if (mode == "subscribe" && token == VERIFY_TOKEN)
            {
                return Ok(challenge);
            }

            // Cualquier otro caso es una verificación fallida
            return Unauthorized();
        }

        // --------------------------------------------------------------------
        //                         RECEPCIÓN DE MENSAJES
        // --------------------------------------------------------------------

        /// <summary>
        /// Endpoint principal utilizado por WhatsApp para enviar eventos
        /// en tiempo real, incluyendo mensajes entrantes de los usuarios.
        /// 
        /// Aquí se extrae el mensaje recibido, se procesa mediante el
        /// motor MessageProcessor y se envía una respuesta automática
        /// al usuario utilizando WhatsAppSender.
        /// </summary>
        /// <param name="body">JSON completo enviado por Meta con la información del mensaje.</param>
        [HttpPost]
        public async Task<IActionResult> Receive([FromBody] JObject body)
        {
            Console.WriteLine("📥 JSON recibido desde WhatsApp:");
            Console.WriteLine(body.ToString());

            try
            {
                // Navegación segura dentro del JSON hasta llegar al mensaje
                var message = body["entry"]?[0]?["changes"]?[0]?
                                   ["value"]?["messages"]?[0];

                if (message != null)
                {
                    string from = message["from"]?.ToString();
                    string text = message["text"]?["body"]?.ToString();

                    // Procesar mensaje recibido
                    string respuesta = _processor.ProcesarMensaje(text);

                    // Enviar respuesta automática
                    await _sender.EnviarMensajeAsync(from, respuesta);

                    Console.WriteLine($"📤 Respuesta enviada a {from}: {respuesta}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Error al procesar el mensaje:");
                Console.WriteLine(ex.Message);
            }

            // WhatsApp SIEMPRE requiere un 200 OK
            return Ok();
        }
    }
}
