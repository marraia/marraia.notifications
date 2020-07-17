using FluentAssertions;
using Marraia.Notifications;
using Marraia.Notifications.Handlers;
using Marraia.Notifications.Interfaces;
using Marraia.Notifications.Models;
using MediatR;
using NSubstitute;
using System;
using Xunit;

namespace Marraia.Notifications.Test
{
    public class SmartNotificationTests
    {
        private INotificationHandler<DomainNotification> subNotificationHandler;

        public SmartNotificationTests()
        {
            this.subNotificationHandler = Substitute.For<DomainNotificationHandler>();
        }

        private ISmartNotification CreateSmartNotification()
        {
            return new SmartNotification(
                this.subNotificationHandler);
        }

        [Fact]
        public void IsValid_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var smartNotification = this.CreateSmartNotification();

            // Act
            var result = smartNotification.IsValid();

            // Assert
            result
                .Should()
                .BeTrue();
        }

        [Fact]
        public void NewNotificationConflict_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var smartNotification = this.CreateSmartNotification();
            string message = "Conflict";

            // Act
            smartNotification.NewNotificationConflict(message);

            // Assert
            smartNotification
                .IsValid()
                .Should()
                .BeTrue();
        }

        [Fact]
        public void NewNotificationBadRequest_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var smartNotification = this.CreateSmartNotification();
            string message = "BadRequest";

            // Act
            smartNotification.NewNotificationBadRequest(message);

            // Assert
            smartNotification
                .IsValid()
                .Should()
                .BeTrue();
        }
    }
}
