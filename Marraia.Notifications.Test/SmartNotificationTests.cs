using FluentAssertions;
using Marraia.Notifications;
using Marraia.Notifications.Handlers;
using Marraia.Notifications.Interfaces;
using Marraia.Notifications.Models;
using MediatR;
using NSubstitute;
using System;
using Xunit;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Marraia.Notifications.Test
{
    public class SmartNotificationTests
    {
        private ILogger<SmartNotification> subLogger;
        private INotificationHandler<DomainNotification> subNotificationHandler;
        private DomainNotificationHandler domainNotificationHandler;
        public SmartNotificationTests()
        {
            this.subLogger = Substitute.For<ILogger<SmartNotification>>();
            domainNotificationHandler = new DomainNotificationHandler();
            this.subNotificationHandler = domainNotificationHandler;
        }

        private ISmartNotification CreateSmartNotification()
        {
            return new SmartNotification(this.subLogger,
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
                .BeFalse();

            domainNotificationHandler
                .GetNotifications()
                .Should()
                .HaveCount(1);

            domainNotificationHandler
                .GetNotifications()
                .FirstOrDefault()
                .Value
                .Should()
                .Be("Conflict");
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
                .BeFalse();

            domainNotificationHandler
                .GetNotifications()
                .Should()
                .HaveCount(1);

            domainNotificationHandler
                .GetNotifications()
                .FirstOrDefault()
                .Value
                .Should()
                .Be("BadRequest");
        }

        [Fact]
        public void NewNotificationError_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var smartNotification = this.CreateSmartNotification();
            string message = "Error";

            // Act
            smartNotification.NewNotificationError(message);

            // Assert
            smartNotification
                .IsValid()
                .Should()
                .BeFalse();

            domainNotificationHandler
                .GetNotifications()
                .Should()
                .HaveCount(1);

            domainNotificationHandler
                .GetNotifications()
                .FirstOrDefault()
                .Value
                .Should()
                .Be("Error");
        }
    }
}
