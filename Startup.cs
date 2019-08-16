using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WebSocketMiddleware;
using WebSocketService;

namespace dotnet_core_websocet
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            WebSocketService = new NotificationWebSocketService();
        }

        public IConfiguration Configuration { get; }
        public IWebSocketService<Message> WebSocketService { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSingleton(WebSocketService);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            WebSocketOptions options = new WebSocketOptions
            {
                KeepAliveInterval = new TimeSpan(0, 2, 0),
                ReceiveBufferSize = 1024 * 8
            };

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseMvc();
            app.UseWebSockets(options);
            app.UseWebSocketConnection(options, WebSocketService);

        }
    }
}
