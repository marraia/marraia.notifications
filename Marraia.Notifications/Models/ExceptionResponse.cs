using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;

namespace Marraia.Notifications.Models
{
    public class ExceptionResponse
    {
        private ExceptionResponse()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        public ExceptionResponse(Exception exception)
        {
            var response = GetCompleteException(exception);
            Error = response.Error;
            InnerError = response.InnerError;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        public ExceptionResponse(List<DomainNotification> exception)
        {
            var response = GetCompleteException(exception);
            Error = response.Error;
            InnerError = response.InnerError;
        }

        /// <summary>
        ///     Response error
        /// </summary>
        [JsonProperty("error", NullValueHandling = NullValueHandling.Ignore)]
        public Errors Error { get; private set; }

        /// <summary>
        ///     Response inner error
        /// </summary>
        [JsonProperty("innererror", NullValueHandling = NullValueHandling.Ignore)]
        public ExceptionResponse InnerError { get; private set; }

        private ExceptionResponse GetCompleteException(Exception exception)
        {
            var response = new ExceptionResponse
            {
                Error = new Errors
                {
                    Message = exception.Message,
                    Code = HttpStatusCode.BadRequest.ToString()
                },
                InnerError = exception.InnerException != null
                    ? GetCompleteException(exception.InnerException)
                    : default(ExceptionResponse)
            };

            return response;
        }

        private ExceptionResponse GetCompleteException(List<DomainNotification> validationErrors)
        {
            var response = new ExceptionResponse
            {
                Error = new Errors
                {
                    Message = validationErrors,
                    Code = HttpStatusCode.BadRequest.ToString()
                }
            };

            return response;
        }
    }
}
