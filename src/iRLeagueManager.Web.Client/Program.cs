using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// This project currently hosts no components itself: it exists so that the server
// (iRLeagueManager.Web) can register the InteractiveWebAssembly/InteractiveAuto render
// modes and serve a real WebAssembly runtime (Blazor.web.js, dotnet.wasm, etc.) to the
// browser. Components that should actually execute client-side via WebAssembly need to be
// moved into this project (or a Razor class library referenced by it) in a future phase,
// since only assemblies referenced by this project are downloaded to the browser.
await builder.Build().RunAsync();
