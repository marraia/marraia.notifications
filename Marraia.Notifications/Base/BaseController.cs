using System.Collections;
using Marraia.Notifications.Interfaces;
using Marraia.Notifications.Models;
using Microsoft.AspNetCore.Mvc;

namespace Marraia.Notifications.Base
{
    /// <summary>
    /// Base para controllers que respondem conforme as notificações da requisição.
    /// Sucesso: <c>{ "success": true, "data": ... }</c>. Erro: <see cref="ErrorResponse"/>,
    /// com o status da notificação mais grave.
    /// </summary>
    public abstract class BaseController : ControllerBase
    {
        protected BaseController(ISmartNotification notification)
        {
            Notification = notification;
        }

        protected ISmartNotification Notification { get; }

        protected bool HasNotification() => Notification.HasNotifications;

        /// <summary>Resposta de erro com as notificações atuais.</summary>
        protected IActionResult NotificationResult()
        {
            var status = (int)(Notification.MostSevereType ?? NotificationType.BadRequest);
            return new ObjectResult(ErrorResponse.From(Notification.Notifications)) { StatusCode = status };
        }

        /// <summary>200 com os dados; 404 se <paramref name="result"/> for nulo; erro se houver notificação.</summary>
        protected IActionResult OkOrNotFound(object? result = null)
        {
            if (HasNotification())
                return NotificationResult();

            if (result is null)
                return NotFound();

            return result is bool ? Ok() : Ok(Success(result));
        }

        /// <summary>200 com os dados; 204 se nulo ou lista vazia; erro se houver notificação.</summary>
        protected IActionResult OkOrNoContent(object? result = null)
        {
            if (HasNotification())
                return NotificationResult();

            if (result is null || IsEmptyEnumerable(result))
                return NoContent();

            return Ok(Success(result));
        }

        /// <summary>202 com os dados; 404 se nulo; erro se houver notificação.</summary>
        protected IActionResult AcceptedContent(object? result = null)
        {
            if (HasNotification())
                return NotificationResult();

            return result is null ? NotFound() : Accepted(Success(result));
        }

        /// <summary>202 com os dados, ou 202 sem corpo se nulo; erro se houver notificação.</summary>
        protected IActionResult AcceptedOrContent(object? result = null)
        {
            if (HasNotification())
                return NotificationResult();

            return result is null ? Accepted() : Accepted(Success(result));
        }

        /// <summary>201 com os dados; erro se houver notificação.</summary>
        protected IActionResult CreatedContent(string route, object? result = null)
        {
            if (HasNotification())
                return NotificationResult();

            return result is null
                ? Created(route, new { success = true })
                : Created(route, Success(result));
        }

        /// <summary>204 sem corpo (comandos sem retorno); erro se houver notificação.</summary>
        protected IActionResult NoContentOrNotification()
            => HasNotification() ? NotificationResult() : NoContent();

        private static object Success(object data) => new { success = true, data };

        private static bool IsEmptyEnumerable(object result)
        {
            if (result is string || result is not IEnumerable enumerable)
                return false;

            if (result is ICollection collection)
                return collection.Count == 0;

            var enumerator = enumerable.GetEnumerator();
            try
            {
                return !enumerator.MoveNext();
            }
            finally
            {
                (enumerator as IDisposable)?.Dispose();
            }
        }
    }
}
