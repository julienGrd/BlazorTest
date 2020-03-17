using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using blazorWebassembly3._2Preview1.Shared;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace blazorWebassembly3._2Preview1.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.Services.AddBaseAddressHttpClient();
            builder.RootComponents.Add<App>("app");

            await builder.Build().RunAsync();
        }
    }
}
