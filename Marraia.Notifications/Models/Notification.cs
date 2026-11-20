namespace Marraia.Notifications.Models
{
    /// <summary>
    /// Uma notificação de regra de negócio.
    /// </summary>
    /// <param name="Message">Mensagem para o usuário.</param>
    /// <param name="Type">Tipo, que define o status HTTP.</param>
    /// <param name="Field">Campo relacionado (validação), em camelCase; nulo quando não se aplica.</param>
    public sealed record Notification(string Message, NotificationType Type, string? Field = null);
}
