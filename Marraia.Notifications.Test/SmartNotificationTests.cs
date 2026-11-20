using FluentValidation;
using Marraia.Notifications.Models;
using Marraia.Notifications.Validations;
using Microsoft.Extensions.Logging.Abstractions;

namespace Marraia.Notifications.Test
{
    public class SmartNotificationTests
    {
        private static SmartNotification Create() => new(NullLogger<SmartNotification>.Instance);

        [Fact]
        public void Sem_notificacoes_e_valido()
        {
            var notification = Create();

            Assert.True(notification.IsValid);
            Assert.False(notification.HasNotifications);
            Assert.Null(notification.MostSevereType);
        }

        [Theory]
        [InlineData(NotificationType.BadRequest)]
        [InlineData(NotificationType.Unauthorized)]
        [InlineData(NotificationType.Forbidden)]
        [InlineData(NotificationType.NotFound)]
        [InlineData(NotificationType.Conflict)]
        [InlineData(NotificationType.Error)]
        public void Cada_metodo_registra_o_tipo_correspondente(NotificationType type)
        {
            var notification = Create();
            Action<string> add = type switch
            {
                NotificationType.BadRequest => m => notification.BadRequest(m),
                NotificationType.Unauthorized => notification.Unauthorized,
                NotificationType.Forbidden => notification.Forbidden,
                NotificationType.NotFound => notification.NotFound,
                NotificationType.Conflict => notification.Conflict,
                _ => notification.Error
            };

            add("mensagem");

            var single = Assert.Single(notification.Notifications);
            Assert.Equal(new Notification("mensagem", type), single);
            Assert.False(notification.IsValid);
        }

        [Fact]
        public void Mensagem_vazia_e_ignorada()
        {
            var notification = Create();

            notification.Conflict("");
            notification.BadRequest("   ");

            Assert.True(notification.IsValid);
        }

        [Fact]
        public void Tipo_mais_grave_vence_independente_da_ordem()
        {
            var notification = Create();

            notification.BadRequest("campo");
            notification.Conflict("duplicado");
            notification.NotFound("não existe");

            Assert.Equal(NotificationType.NotFound, notification.MostSevereType);

            notification.Unauthorized("sem login");
            Assert.Equal(NotificationType.Unauthorized, notification.MostSevereType);
        }

        [Fact]
        public void Validation_converte_erros_do_FluentValidation_com_campo_em_camelCase()
        {
            var notification = Create();
            var result = new PersonValidator().Validate(new Person { Name = "", Address = new Address { ZipCode = "" } });

            notification.Validation(result);

            Assert.Contains(new Notification("Nome obrigatório", NotificationType.BadRequest, "name"), notification.Notifications);
            Assert.Contains(new Notification("CEP obrigatório", NotificationType.BadRequest, "address.zipCode"), notification.Notifications);
        }

        [Fact]
        public void EntityValidator_notifica_FieldValidation()
        {
            var notification = Create();
            var service = new SampleService(notification);
            var fieldValidation = new FieldValidation(false);
            fieldValidation.AddValidation(new PersonValidator().Validate(new Person { Address = new Address { ZipCode = "1" } }));

            service.NotifyValidationErrors(fieldValidation);

            var single = Assert.Single(notification.Notifications);
            Assert.Equal("name", single.Field);
        }

        [Fact]
        public void Clear_remove_todas()
        {
            var notification = Create();
            notification.Error("falha");

            notification.Clear();

            Assert.True(notification.IsValid);
        }

        private sealed class SampleService(SmartNotification notification) : EntityValidator(notification);

        private sealed class Person
        {
            public string Name { get; set; } = string.Empty;
            public Address Address { get; set; } = new();
        }

        private sealed class Address
        {
            public string ZipCode { get; set; } = string.Empty;
        }

        private sealed class PersonValidator : AbstractValidator<Person>
        {
            public PersonValidator()
            {
                RuleFor(p => p.Name).NotEmpty().WithMessage("Nome obrigatório");
                RuleFor(p => p.Address.ZipCode).NotEmpty().WithMessage("CEP obrigatório");
            }
        }
    }
}
