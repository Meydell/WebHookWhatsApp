using Newtonsoft.Json;

namespace ChatbotWhatsApp.Models
{
    /// <summary>
    /// Modelo raíz del Webhook enviado por WhatsApp Cloud API.
    /// Representa exactamente el JSON recibido por el endpoint POST.
    /// </summary>
    public class WhatsAppWebhook
    {
        /// <summary>
        /// Tipo de evento reportado (generalmente "whatsapp_business_account").
        /// </summary>
        [JsonProperty("object")]
        public string Object { get; set; }

        /// <summary>
        /// Lista de entradas asociadas al evento.
        /// </summary>
        [JsonProperty("entry")]
        public List<Entry> Entry { get; set; }
    }

    /// <summary>
    /// Entrada completa que representa un cambio detectado.
    /// </summary>
    public class Entry
    {
        /// <summary>
        /// Identificador de la cuenta de WhatsApp Business asociada.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Lista de cambios detectados (ej: mensajes recibidos).
        /// </summary>
        [JsonProperty("changes")]
        public List<Change> Changes { get; set; }
    }

    /// <summary>
    /// Representa un tipo de evento específico (ej: mensaje entrante).
    /// </summary>
    public class Change
    {
        [JsonProperty("field")]
        public string Field { get; set; }

        [JsonProperty("value")]
        public ChangeValue Value { get; set; }
    }

    /// <summary>
    /// Datos completos del mensaje recibido, contactos involucrados,
    /// y metadatos asociados.
    /// </summary>
    public class ChangeValue
    {
        [JsonProperty("messaging_product")]
        public string MessagingProduct { get; set; }

        [JsonProperty("metadata")]
        public Metadata Metadata { get; set; }

        [JsonProperty("contacts")]
        public List<Contact> Contacts { get; set; }

        [JsonProperty("messages")]
        public List<Message> Messages { get; set; }
    }

    /// <summary>
    /// Información del número de WhatsApp Business receptor.
    /// </summary>
    public class Metadata
    {
        [JsonProperty("display_phone_number")]
        public string DisplayPhoneNumber { get; set; }

        [JsonProperty("phone_number_id")]
        public string PhoneNumberId { get; set; }
    }

    /// <summary>
    /// Información del usuario que envió el mensaje.
    /// </summary>
    public class Contact
    {
        [JsonProperty("profile")]
        public ContactProfile Profile { get; set; }

        [JsonProperty("wa_id")]
        public string WaId { get; set; }
    }

    /// <summary>
    /// Datos del perfil del usuario que envió el mensaje.
    /// </summary>
    public class ContactProfile
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    /// <summary>
    /// Mensaje recibido desde WhatsApp Cloud API.
    /// Contiene texto, imágenes, audios u otros tipos de contenido.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Identificador del usuario que envió el mensaje.
        /// </summary>
        [JsonProperty("from")]
        public string From { get; set; }

        /// <summary>
        /// ID único del mensaje.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Marca de tiempo Unix.
        /// </summary>
        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }

        /// <summary>
        /// Tipo de mensaje (text, image, audio, document, etc.)
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Contenido del mensaje si es de tipo "text".
        /// </summary>
        [JsonProperty("text")]
        public MessageText Text { get; set; }
    }

    /// <summary>
    /// Representa solo el campo "text" del mensaje.
    /// </summary>
    public class MessageText
    {
        [JsonProperty("body")]
        public string Body { get; set; }
    }
}
