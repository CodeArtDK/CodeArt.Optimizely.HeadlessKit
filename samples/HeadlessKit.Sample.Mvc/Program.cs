using CodeArt.Optimizely.HeadlessKit.Mvc;
using CodeArt.Optimizely.HeadlessKit.Mvc.Infrastructure;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        builder.Services.AddSaaSCMSTypeBuilder(builder.Configuration);
        builder.Services.AddOptimizelyGraph(builder.Configuration);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseCmsPreview();
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "search",
            pattern: "search",
            defaults: new { controller = "Search", action = "Index" });

        app.MapControllerRoute(
            name: "query",
            pattern: "query",
            defaults: new { controller = "Query", action = "Index" });

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.MapContentControllerRoute();

        // Initialize services (TemplateCoordinator, etc.)
        await app.InitializeServicesAsync();

        app.Run();
    }
}
