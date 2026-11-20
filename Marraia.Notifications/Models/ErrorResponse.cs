using System.Text.Json.Serialization;

namespace Marraia.Notifications.Models
{
    /// <summary>
    /// Corpo das respostas de erro:
    /// <code>{ "error": { "code": "BadRequest", "message": [ { "value": "...", "field": "name" } ] } }</code>
    /// </summary>
    public sealed class ErrorResponse
    {
        [JsonPropertyName("error")]
        public ErrorDetail Error { get; init; } = new();

        /// <summary>Monta a resposta a partir das notificações, usando o tipo mais grave como código.</summary>
        public static ErrorResponse From(IEnumerable<Notification> notifications)
        {
            var list = notifications.ToList();
            var type = NotificationSeverity.MostSevere(list) ?? NotificationType.BadRequest;

            return new ErrorResponse
            {
                Error = new ErrorDetail
                {
                    Code = type.ToString(),
                    Message = list.Select(n => new ErrorMessage { Value = n.Message, Field = n.Field }).ToList()
                }
            };
        }
    }

    public sealed class ErrorDetail
    {
        /// <summary>Nome do status: BadRequest, Unauthorized, Forbidden, NotFound, Conflict ou Error.</summary>
        [JsonPropertyName("code")]
        public string Code { get; init; } = string.Empty;

        [JsonPropertyName("message")]
        public IReadOnlyList<ErrorMessage> Message { get; init; } = Array.Empty<ErrorMessage>();
    }

    public sealed class ErrorMessage
    {
        [JsonPropertyName("value")]
        public string Value { get; init; } = string.Empty;

        [JsonPropertyName("field")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Field { get; init; }
    }
}
