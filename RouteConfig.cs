namespace WorldDominion
{
    public class RouteConfig
    {
        public static void ConfigureRoutes(IEndpointRouteBuilder routes)
        {
            routes.MapControllerRoute(
                name: "about",
                pattern: "about",
                defaults: new { controller = "Home", action = "About" });

            routes.MapControllerRoute(
                name: "contact",
                pattern: "contact",
                defaults: new { controller = "Home", action = "Contact" });

            routes.MapControllerRoute(
                name: "privacy",
                pattern: "privacy",
                defaults: new { controller = "Home", action = "Privacy" });

            routes.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        }
    }
}
