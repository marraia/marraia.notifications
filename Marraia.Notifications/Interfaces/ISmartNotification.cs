using FluentValidation.Results;
using Marraia.Notifications.Models;
using Marraia.Notifications.Validations;

namespace Marraia.Notifications.Interfaces
{
    /// <summary>
    /// Acumula as notificações de regra de negócio da requisição atual (escopo por requisição).
    /// </summary>
    public interface ISmartNotification
    {
        /// <summary>Verdadeiro quando não há nenhuma notificação.</summary>
        bool IsValid { get; }

        bool HasNotifications { get; }

        IReadOnlyList<Notification> Notifications { get; }

        /// <summary>Tipo mais grave entre as notificações; nulo quando não há nenhuma.</summary>
        NotificationType? MostSevereType { get; }

        /// <summary>400 — requisição inválida. Informe <paramref name="field"/> quando o erro for de um campo.</summary>
        void BadRequest(string message, string? field = null);

        /// <summary>401 — não autenticado.</summary>
        void Unauthorized(string message);

        /// <summary>403 — sem permissão.</summary>
        void Forbidden(string message);

        /// <summary>404 — recurso não encontrado.</summary>
        void NotFound(string message);

        /// <summary>409 — regra de negócio violada ou conflito (ex.: duplicado).</summary>
        void Conflict(string message);

        /// <summary>500 — falha inesperada.</summary>
        void Error(string message);

        /// <summary>Adiciona um BadRequest para cada erro do FluentValidation, com o campo em camelCase.</summary>
        void Validation(ValidationResult validationResult);

        /// <summary>Adiciona um BadRequest para cada erro das validações acumuladas.</summary>
        void Validation(FieldValidation fieldValidation);

        void Clear();
    }
}
