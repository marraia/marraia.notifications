using FluentAssertions;
using Marraia.Notifications.Handlers;
using Marraia.Notifications.Models;
using System.Threading.Tasks;
using Xunit;

namespace Marraia.Notifications.Test.Handlers
{
    public class DomainNotificationHandlerTests
    {
        public DomainNotificationHandlerTests()
        {

        }

        private DomainNotificationHandler CreateDomainNotificationHandler()
        {
            return new DomainNotificationHandler();
        }

        [Fact]
        public async Task Handle_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var domainNotificationHandler = this.CreateDomainNotificationHandler();
            var notification = new DomainNotification("TESTE", Models.Enum.DomainNotificationType.BadRequest);
            var cancellationToken = default(global::System.Threading.CancellationToken);

            // Act
            await domainNotificationHandler.Handle(
                notification,
                cancellationToken);

            var result = domainNotificationHandler.GetNotifications();

            // Assert
            result
            .Should()
            .HaveCount(1);
        }

        [Fact]
        public void GetNotifications_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var domainNotificationHandler = this.CreateDomainNotificationHandler();

            // Act
            var result = domainNotificationHandler.GetNotifications();

            // Assert
            result
                .Should()
                .HaveCount(0);
        }

        [Fact]
        public void HasNotifications_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var domainNotificationHandler = this.CreateDomainNotificationHandler();

            // Act
            var result = domainNotificationHandler.HasNotifications();

            // Assert
            result
                .Should()
                .BeFalse();
        }

        [Fact]
        public void Dispose_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var domainNotificationHandler = this.CreateDomainNotificationHandler();

            // Act
            domainNotificationHandler.Dispose();

            // Assert
            domainNotificationHandler
                .GetNotifications()
                .Should()
                .HaveCount(0);
        }
    }
}
