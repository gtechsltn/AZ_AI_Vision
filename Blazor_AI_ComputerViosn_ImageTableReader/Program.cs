using Blazor_AI_ComputerViosn_ImageTableReader;
using Blazor_AI_ComputerViosn_ImageTableReader.AIProcessor;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<AIImageProcessor>();

// Read the JSON file and add it to the configuration
using var http = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
using var response = await http.GetAsync("config.json");
using var stream = await response.Content.ReadAsStreamAsync();
builder.Configuration.AddJsonStream(stream);
await builder.Build().RunAsync();
