namespace Nubot.Core.Nancy
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using log4net;
    using Microsoft.Owin;
    using Owin;
    using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;

    public class RequestLogger
    {
        private readonly AppFunc _next;
        private readonly ILog _log;

        public RequestLogger(AppFunc next, ILog log)
        {
            _next = next;
            _log = log;
        }

        public async Task Invoke(IDictionary<string, object> env)
        {
            IOwinContext context = new OwinContext(env);

            if (_log.IsInfoEnabled) _log.InfoFormat("{0} {1}", context.Request.Method, context.Request.Uri);

            if (_log.IsDebugEnabled)
            {
                var stream = context.Request.Body;
                var buffer = new MemoryStream();
                context.Request.Body = buffer;

                await stream.CopyToAsync(buffer);

                buffer.Seek(0, SeekOrigin.Begin);

                var reader = new StreamReader(buffer);
                var requestBody = await reader.ReadToEndAsync();

                _log.Debug(requestBody);

                buffer.Seek(0, SeekOrigin.Begin);
            }

            await _next(env);
        }
    }

    public static class AppBuilderExtensions
    {
        public static IAppBuilder UseSimpleLogger(this IAppBuilder app, ILog log)
        {
            return app.Use<RequestLogger>(log);
        }
    }
}