using CodeArt.Optimizely.HeadlessKit.Mvc;
using CodeArt.Optimizely.HeadlessKit.Mvc.Infrastructure;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddSaaSCMSTypeBuilder(builder.Configuration);
        builder.Services.AddOptimizelyGraph(builder.Configuration);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseCmsPreview();
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapDynamicPageRoute<ContentRouteTransformer>("{**path}");
        app.MapRazorPages();

        // Initialize services (TemplateCoordinator, etc.)
        await app.InitializeServicesAsync();

        app.Run();
    }
}
