using MasaQSTemplate.AuthModule.Entities;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Newtonsoft.Json;
using System.Diagnostics;

namespace MasaQSTemplate.Api.Middlewares
{
    public class RequestLogMiddleware
    {
        private readonly RequestDelegate _next;
        public RequestLogMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var logger = context.RequestServices.GetRequiredService<ILogger<RequestLogMiddleware>>();
            var eventBus = context.RequestServices.GetRequiredService<IEventBus>();
            var stopWatch = new Stopwatch();
            string body = null;
            if (context.Request.HasFormContentType)
            {
                body = JsonConvert.SerializeObject(context.Request.Form.Keys.Select(v => new { Key = v, Value = context.Request.Form[v] }));
            }
            if (context.Request.HasJsonContentType())
            {
                context.Request.EnableBuffering();
                using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true))
                {
                    body = await reader.ReadToEndAsync();
                    context.Request.Body.Seek(0, SeekOrigin.Begin);
                }
            }
            var log = new LogCreateDto()
            {
                TraceId = context.TraceIdentifier ?? Guid.NewGuid().ToString(),
                Url = context.Request.GetEncodedPathAndQuery(),
                HttpMethod = context.Request.Method,
                HttpBody = body,
                ClientIpAddress = context.Connection.RemoteIpAddress?.ToString(),
                BrowserInfo = context.Request?.Headers?["User-Agent"],
                ExecutionTime = DateTime.Now,
            };
            try
            {
                stopWatch.Start();
                await _next(context);
            }
            catch (Exception ex)
            {
                var message = $"服务器内部错误..({log.TraceId})";
                if (ex is MasaValidatorException || ex is UserFriendlyException)
                {
                    message = ex.Message;
                    context.Response.StatusCode = ex is MasaValidatorException ? 298 : 299;
                }
                else
                {
                    log.Exception = ex.Message;
                    logger.LogInformation($"【request.exception】TraceId：{log.TraceId}|exception：{ex}");
                }
                //context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(message);
            }
            finally
            {
                stopWatch.Stop();
                var handler = context.GetEndpoint()?
                                        .Metadata
                                        .GetMetadata<ControllerActionDescriptor>();
                //.GetMetadata<MethodInfo>();
                log.ServiceName = handler?.ControllerName; //handler?.DeclaringType?.Name;
                log.MethodName = handler?.ActionName; //handler?.Name;
                var currentUser = context.RequestServices.GetService<IUserContext>();
                log.UserId = currentUser?.UserId != null && Guid.TryParse(currentUser.UserId, out var userid) ? userid : null;
                log.UserName = currentUser?.UserName;
                log.ExecutionDuration = Convert.ToInt32(stopWatch.ElapsedMilliseconds);
                log.HttpStatusCode = context.Response.StatusCode;
                await eventBus.PublishAsync(new LogCreateCommand(log));
            }
        }
    }
}
