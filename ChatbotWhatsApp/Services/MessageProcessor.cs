using System.Linq;
using System.Text.RegularExpressions;

namespace ChatbotWhatsApp.Services
{
    /// <summary>
    /// Motor avanzado de procesamiento de mensajes para el chatbot.
    /// 
    /// Este componente analiza texto entrante y genera respuestas
    /// basadas en reglas simples, clasificación básica e intención.
    /// 
    /// Funcionalidades:
    ///  - Detección de saludos
    ///  - Detección de emociones básicas
    ///  - Reconocimiento de nombre
    ///  - Procesamiento de comandos (/menu, /ayuda, etc.)
    ///  - Identificación de preguntas
    ///  - Análisis de frecuencia de palabras
    /// </summary>
    public class MessageProcessor
    {
        private readonly string _miNombre = "meydell";

        /// <summary>
        /// Procesamiento principal del chatbot.
        /// </summary>
        public string ProcesarMensaje(string mensaje)
        {
            if (string.IsNullOrWhiteSpace(mensaje))
                return "😅 No logré entender el mensaje. ¿Podés repetirlo?";

            string texto = mensaje.ToLower().Trim();

            // 1. Procesar comandos explícitos
            string? comando = ProcesarComandos(texto);
            if (comando != null) return comando;

            // 2. Detección del nombre
            if (ContieneNombre(texto))
                return "👀 Veo que mencionaste mi nombre. ¿En qué puedo ayudarte?";

            // 3. Saludos naturales
            string? saludo = DetectarSaludo(texto);
            if (saludo != null) return saludo;

            // 4. Detección de emociones básicas
            string? emocion = DetectarEmocion(texto);
            if (emocion != null) return emocion;

            // 5. Detección de preguntas
            if (EsPregunta(texto))
                return "🤔 Buena pregunta. Estoy procesando tu consulta...";

            // 6. Frases clave
            string? fraseClave = AnalizarFrasesClave(texto);
            if (fraseClave != null) return fraseClave;

            // 7. Analizar palabras frecuentes
            string top = ObtenerPalabrasFrecuentes(texto);

            return $"🤖 Procesé tu mensaje. Palabras más mencionadas: {top}";
        }

        // --------------------------------------------------------------------
        // -------------------------- COMANDOS ---------------------------------
        // --------------------------------------------------------------------

        private string? ProcesarComandos(string texto)
        {
            if (texto.StartsWith("/ayuda"))
                return "🆘 *Ayuda*\nComandos disponibles:\n/menu – Ver menú\n/hora – Hora actual\n/saludo – Saludo rápido\n/info – Información del bot";

            if (texto.StartsWith("/menu"))
                return "📋 *Menú del Bot*\n1️⃣ Información\n2️⃣ Consultas\n3️⃣ Ayuda\n4️⃣ Contacto";

            if (texto.StartsWith("/hora"))
                return $"⏰ Hora actual: {DateTime.Now:hh:mm tt}";

            if (texto.StartsWith("/saludo"))
                return "👋 ¡Hola! Aquí estoy para ayudarte.";

            if (texto.StartsWith("/info"))
                return "📘 Soy un chatbot creado por Meydell para automatizar WhatsApp.";

            return null;
        }

        // --------------------------------------------------------------------
        // --------------------- DETECCIÓN DE SALUDOS --------------------------
        // --------------------------------------------------------------------

        private string? DetectarSaludo(string texto)
        {
            string[] saludos = { "hola", "buenas", "que tal", "hey", "holi", "saludos" };

            if (saludos.Any(s => texto.StartsWith(s)))
                return "👋 ¡Hola! ¿Cómo estás? ¿En qué puedo ayudarte hoy?";

            return null;
        }

        // --------------------------------------------------------------------
        // --------------------- DETECCIÓN DE EMOCIONES ------------------------
        // --------------------------------------------------------------------

        private string? DetectarEmocion(string texto)
        {
            if (texto.Contains("triste") || texto.Contains("mal") || texto.Contains("deprimido"))
                return "😔 Lamento que te sientas así. Si querés hablar, estoy aquí.";

            if (texto.Contains("feliz") || texto.Contains("contento") || texto.Contains("alegre"))
                return "😄 ¡Qué bueno! Me alegra escuchar eso.";

            if (texto.Contains("enojado") || texto.Contains("molesto"))
                return "😠 Entiendo tu molestia. ¿Querés contarme qué pasó?";

            return null;
        }

        // --------------------------------------------------------------------
        // ------------------- DETECCIÓN DE NOMBRE ------------------------------
        // --------------------------------------------------------------------

        private bool ContieneNombre(string texto)
        {
            return texto.Contains(_miNombre.ToLower());
        }

        // --------------------------------------------------------------------
        // --------------------- DETECCIÓN DE PREGUNTAS -------------------------
        // --------------------------------------------------------------------

        private bool EsPregunta(string texto)
        {
            return texto.EndsWith("?") ||
                   texto.StartsWith("como") ||
                   texto.StartsWith("qué") ||
                   texto.StartsWith("donde") ||
                   texto.StartsWith("cuando") ||
                   texto.StartsWith("por qué");
        }

        // --------------------------------------------------------------------
        // -------------------- FRASES CLAVE PERSONALIZADAS --------------------
        // --------------------------------------------------------------------

        private string? AnalizarFrasesClave(string texto)
        {
            if (texto.Contains("gracias"))
                return "🙏 ¡Con gusto! Estoy aquí para ayudarte.";

            if (texto.Contains("adios") || texto.Contains("bye"))
                return "👋 ¡Hasta luego! Que tengas un excelente día.";

            if (texto.Contains("te quiero") || texto.Contains("te amo"))
                return "🥰 Aprecio tus palabras, pero soy un bot jaja.";

            return null;
        }

        // --------------------------------------------------------------------
        // ------------ ANÁLISIS DE FRECUENCIA DE PALABRAS ---------------------
        // --------------------------------------------------------------------

        private string ObtenerPalabrasFrecuentes(string texto)
        {
            var palabras = texto
                .Split(' ', '.', ',', '!', '?', ';', ':')
                .Where(p => p.Length > 2)
                .GroupBy(p => p)
                .OrderByDescending(g => g.Count())
                .Take(3)
                .Select(g => $"{g.Key} ({g.Count()}×)");

            return string.Join(", ", palabras);
        }
    }
}
