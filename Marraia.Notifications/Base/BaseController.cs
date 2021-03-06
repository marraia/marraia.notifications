﻿using Marraia.Notifications.Handlers;
using Marraia.Notifications.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Linq;
using System.Net;

namespace Marraia.Notifications.Base
{
    public abstract class BaseController : ControllerBase
    {
        private readonly DomainNotificationHandler _messageHandler;

        protected BaseController(INotificationHandler<DomainNotification> notification)
        {
            _messageHandler = (DomainNotificationHandler)notification;
        }

        protected bool HasNotification()
        {
            return _messageHandler.HasNotifications();
        }

        protected IActionResult NotificationBusiness()
        {
            var notifications = _messageHandler.GetNotifications();
            var domainNotificationType = notifications?.FirstOrDefault()?.DomainNotificationType;

            return new JsonResult(new ExceptionResponse(notifications?.ToList()))
            {
                StatusCode = (int?)domainNotificationType
            };
        }

        protected IActionResult HttpResponse(object response, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            if (!_messageHandler.HasNotifications()) return new JsonResult(response)
            {
                StatusCode = (int)statusCode
            };

            var notifications = _messageHandler.GetNotifications();

            var domainNotificationType = notifications?.FirstOrDefault()?.DomainNotificationType;
            if (domainNotificationType != null)
            {
                return new JsonResult(new ExceptionResponse(notifications.ToList()))
                {
                    StatusCode = (int)domainNotificationType
                };
            }

            return new JsonResult(response)
            {
                StatusCode = (int)statusCode
            };
        }

        protected IActionResult OkOrNotFound(object result = null)
        {

            if (!HasNotification())
            {
                if (result != null)
                {
                    if (result is bool)
                        return Ok();

                    return Ok(new
                    {
                        success = true,
                        data = result
                    });
                }

                return NotFound();
            }

            return NotificationBusiness();
        }

        protected IActionResult OkOrNoContent(object result = null)
        {
            if (!HasNotification() && result != null)
            {
                if (result is IEnumerable)
                {
                    if (((ICollection)result).Count > 0)
                    {
                        return Ok(new
                        {
                            success = true,
                            data = result
                        });
                    }

                    return NoContent();
                }

                return Ok(new
                {
                    success = true,
                    data = result
                });
            }

            if (!HasNotification() && result == null)
            {
                return NoContent();
            }

            return NotificationBusiness();
        }

        protected IActionResult AcceptedContent(object result = null)
        {
            if (!HasNotification())
            {
                if (result != null)
                    return Accepted(new
                    {
                        success = true,
                        data = result
                    });

                return NotFound();
            }

            return NotificationBusiness();
        }

        protected IActionResult AcceptedOrContent(object result = null)
        {
            if (!HasNotification())
            {
                if (result != null)
                    return Accepted(new
                    {
                        success = true,
                        data = result
                    });

                return Accepted();
            }

            return NotificationBusiness();
        }

        protected IActionResult CreatedContent(string rota, object result = null)
        {
            if (!HasNotification())
            {
                if (result != null)
                    return Created(rota, new
                    {
                        success = true,
                        data = result
                    });

                return Created(rota, new
                {
                    success = true
                });
            }

            return NotificationBusiness();
        }
    }
}