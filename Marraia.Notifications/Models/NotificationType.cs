namespace Marraia.Notifications.Models
{
    /// <summary>
    /// Tipo da notificação. O valor é o status HTTP devolvido pela API.
    /// </summary>
    public enum NotificationType
    {
        /// <summary>Requisição inválida: dados obrigatórios, formato, validação de campos.</summary>
        BadRequest = 400,

        /// <summary>Não autenticado: credenciais inválidas ou sessão expirada.</summary>
        Unauthorized = 401,

        /// <summary>Autenticado, mas sem permissão para a operação.</summary>
        Forbidden = 403,

        /// <summary>O recurso solicitado não existe.</summary>
        NotFound = 404,

        /// <summary>Regra de negócio violada ou conflito com o estado atual (ex.: registro duplicado).</summary>
        Conflict = 409,

        /// <summary>Falha inesperada ao processar a requisição.</summary>
        Error = 500
    }
}
