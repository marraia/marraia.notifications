using System.Text.Json;
using Marraia.Notifications.Base;
using Marraia.Notifications.Interfaces;
using Marraia.Notifications.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace Marraia.Notifications.Test
{
    public class BaseControllerTests
    {
        private static readonly JsonSerializerOptions WebJson = new(JsonSerializerDefaults.Web);

        private readonly SmartNotification _notification = new(NullLogger<SmartNotification>.Instance);
        private TestController Controller => new(_notification);

        private static int? StatusOf(IActionResult result) => result switch
        {
            ObjectResult o => o.StatusCode,
            StatusCodeResult s => s.StatusCode,
            _ => null
        };

        private static string BodyOf(IActionResult result) => JsonSerializer.Serialize(((ObjectResult)result).Value, WebJson);

        [Fact]
        public void OkOrNotFound_retorna_200_com_success_e_data()
        {
            var result = Controller.CallOkOrNotFound(new { id = 1 });

            Assert.Equal(StatusCodes.Status200OK, StatusOf(result));
            Assert.Equal("""{"success":true,"data":{"id":1}}""", BodyOf(result));
        }

        [Fact]
        public void OkOrNotFound_com_resultado_nulo_retorna_404()
            => Assert.Equal(StatusCodes.Status404NotFound, StatusOf(Controller.CallOkOrNotFound(null)));

        [Fact]
        public void OkOrNoContent_com_IEnumerable_nao_materializado_nao_quebra()
        {
            var empty = Enumerable.Range(0, 0).Select(i => i);
            var filled = Enumerable.Range(0, 2).Select(i => i);

            Assert.Equal(StatusCodes.Status204NoContent, StatusOf(Controller.CallOkOrNoContent(empty)));
            Assert.Equal(StatusCodes.Status200OK, StatusOf(Controller.CallOkOrNoContent(filled)));
        }

        [Theory]
        [InlineData(NotificationType.BadRequest, 400)]
        [InlineData(NotificationType.Unauthorized, 401)]
        [InlineData(NotificationType.Forbidden, 403)]
        [InlineData(NotificationType.NotFound, 404)]
        [InlineData(NotificationType.Conflict, 409)]
        [InlineData(NotificationType.Error, 500)]
        public void Notificacao_define_status_e_code(NotificationType type, int status)
        {
            Add(_notification, type, "mensagem");

            var result = Controller.CallCreatedContent(new { id = 1 });

            Assert.Equal(status, StatusOf(result));
            Assert.Equal($$$"""{"error":{"code":"{{{type}}}","message":[{"value":"mensagem"}]}}""", BodyOf(result));
        }

        [Fact]
        public void Erro_de_campo_inclui_field_no_json()
        {
            _notification.BadRequest("Nome obrigatório", "name");

            var result = Controller.CallAcceptedContent(new { });

            Assert.Equal("""{"error":{"code":"BadRequest","message":[{"value":"Nome obrigatório","field":"name"}]}}""",
                         BodyOf(result).Replace("\\u00F3", "ó"));
        }

        [Fact]
        public void NoContentOrNotification_retorna_204_ou_erro()
        {
            Assert.Equal(StatusCodes.Status204NoContent, StatusOf(Controller.CallNoContent()));

            _notification.Unauthorized("Sessão expirada");
            Assert.Equal(StatusCodes.Status401Unauthorized, StatusOf(Controller.CallNoContent()));
        }

        private static void Add(ISmartNotification notification, NotificationType type, string message)
        {
            switch (type)
            {
                case NotificationType.BadRequest: notification.BadRequest(message); break;
                case NotificationType.Unauthorized: notification.Unauthorized(message); break;
                case NotificationType.Forbidden: notification.Forbidden(message); break;
                case NotificationType.NotFound: notification.NotFound(message); break;
                case NotificationType.Conflict: notification.Conflict(message); break;
                default: notification.Error(message); break;
            }
        }

        private sealed class TestController(ISmartNotification notification) : BaseController(notification)
        {
            public IActionResult CallOkOrNotFound(object? result) => OkOrNotFound(result);
            public IActionResult CallOkOrNoContent(object? result) => OkOrNoContent(result);
            public IActionResult CallAcceptedContent(object? result) => AcceptedContent(result);
            public IActionResult CallCreatedContent(object? result) => CreatedContent("api/test/1", result);
            public IActionResult CallNoContent() => NoContentOrNotification();
        }
    }
}
