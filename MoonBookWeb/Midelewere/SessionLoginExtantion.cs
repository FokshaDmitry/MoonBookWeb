namespace MoonBookWeb.Midelewere
{
    public static class SessionLoginExtantion
    {
        public static IApplicationBuilder UseSessionLogin(
        this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SessionLoginMiddelwere>();
        }
    }
}
