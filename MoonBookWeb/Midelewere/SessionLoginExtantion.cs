namespace MoonBookWeb.Midelewere
{
    public static class SessionLoginExtantion
    {
        //Bilder Middelwere session login
        public static IApplicationBuilder UseSessionLogin(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SessionLoginMiddelwere>();
        }
    }
}
