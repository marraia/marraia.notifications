using System.Text.Json;
using Marraia.Notifications.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Marraia.Notifications.Test
{
    public class ModelStateResponseTests
    {
        private static IActionResult Invoke(ModelStateDictionary modelState, params string[] parameterNames)
        {
            var services = new ServiceCollection();
            services.AddControllers().AddSmartNotificationModelStateResponse();
            var options = services.BuildServiceProvider().GetRequiredService<IOptions<ApiBehaviorOptions>>().Value;

            var descriptor = new ActionDescriptor
            {
                Parameters = parameterNames.Select(n => new Microsoft.AspNetCore.Mvc.Abstractions.ParameterDescriptor { Name = n }).ToList()
            };
            var context = new ActionContext(new DefaultHttpContext(), new RouteData(), descriptor, modelState);

            return options.InvalidModelStateResponseFactory(context);
        }

        private static string Body(IActionResult result)
            => JsonSerializer.Serialize(((ObjectResult)result).Value, new JsonSerializerOptions(JsonSerializerDefaults.Web));

        [Fact]
        public void Json_invalido_vira_badrequest_com_campo_sem_o_aviso_generico_do_parametro()
        {
            var modelState = new ModelStateDictionary();
            modelState.AddModelError("input", "The input field is required.");
            modelState.AddModelError("$.role", "Role nao reconhecido");

            var result = Invoke(modelState, "input");

            Assert.Equal(StatusCodes.Status400BadRequest, ((ObjectResult)result).StatusCode);
            Assert.Equal("""{"error":{"code":"BadRequest","message":[{"value":"Role nao reconhecido","field":"role"}]}}""", Body(result));
        }

        [Fact]
        public void Corpo_ausente_mantem_o_erro_do_parametro()
        {
            var modelState = new ModelStateDictionary();
            modelState.AddModelError("input", "The input field is required.");

            var result = Invoke(modelState, "input");

            Assert.Equal("""{"error":{"code":"BadRequest","message":[{"value":"The input field is required.","field":"input"}]}}""", Body(result));
        }
    }
}
