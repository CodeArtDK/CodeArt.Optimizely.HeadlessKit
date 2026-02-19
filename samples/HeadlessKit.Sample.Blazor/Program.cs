using CodeArt.Optimizely.HeadlessKit.Mvc;
using CodeArt.Optimizely.HeadlessKit.TypeBuilder;
using HeadlessKit.Sample.Blazor.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // HeadlessKit services
        builder.Services.AddSaaSCMSTypeBuilder(builder.Configuration);
        builder.Services.AddOptimizelyGraph(builder.Configuration);

        // Portal services
        builder.Services.AddSingleton<PortalUserService>();

        // Authentication
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/login";
                options.LogoutPath = "/api/auth/logout";
            });
        builder.Services.AddAuthorization();
        builder.Services.AddCascadingAuthenticationState();

        // Blazor Server
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseAntiforgery();

        app.UseAuthentication();
        app.UseAuthorization();

        // Minimal API endpoints for cookie auth (Blazor Server can't set cookies from components)
        app.MapPost("/api/auth/login", async (HttpContext context, PortalUserService userService) =>
        {
            var form = await context.Request.ReadFormAsync();
            var username = form["username"].ToString();
            var password = form["password"].ToString();

            var user = userService.Authenticate(username, password);
            if (user == null)
            {
                return Results.Redirect("/login?error=invalid");
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Username),
                new("DisplayName", user.DisplayName),
                new(ClaimTypes.Email, user.Email)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await context.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            var returnUrl = form["returnUrl"].ToString();
            return Results.Redirect(string.IsNullOrEmpty(returnUrl) ? "/portal" : returnUrl);
        });

        app.MapPost("/api/auth/logout", async (HttpContext context) =>
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Results.Redirect("/login");
        });

        app.MapRazorComponents<HeadlessKit.Sample.Blazor.Components.App>()
            .AddInteractiveServerRenderMode();

        // Initialize HeadlessKit services (type scanning, etc.)
        await app.InitializeServicesAsync();

        app.Run();
    }
}
