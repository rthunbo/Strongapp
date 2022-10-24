using Blazored.Modal;
using Fluxor;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Strongapp.UI;
using Strongapp.UI.Services;
using Strongapp.UI.Stores;
using System.Reflection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredModal();

builder.Services.AddHttpClient<IWorkoutService, WorkoutService>(client =>
    client.BaseAddress = new Uri("https://localhost:7199/"));
builder.Services.AddHttpClient<IExerciseService, ExerciseService>(client =>
    client.BaseAddress = new Uri("https://localhost:7199/"));
builder.Services.AddHttpClient<IFolderService, FolderService>(client =>
    client.BaseAddress = new Uri("https://localhost:7199/"));
builder.Services.AddHttpClient<ITemplateService, TemplateService>(client =>
    client.BaseAddress = new Uri("https://localhost:7199/"));
builder.Services.AddHttpClient<IAggregateDataService, AggregateDataService>(client =>
    client.BaseAddress = new Uri("https://localhost:7199/"));

Assembly storeAssembly = typeof(AppStore).Assembly;
builder.Services
    .AddFluxor(options =>
        options.ScanAssemblies(storeAssembly));

await builder.Build().RunAsync();
