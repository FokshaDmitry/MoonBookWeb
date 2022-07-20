using MoonBookWeb.Services;

namespace MoonBookWeb.Midelewere
{
    public class SessionLoginMiddelwere
    {
        private readonly RequestDelegate _next;

        public SessionLoginMiddelwere(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ISessionLogin sessionLogin)
        {
            String userId = context.Session.GetString("UserId");
            if (userId != null)
            {
                sessionLogin.Set(userId);
            }
            await _next(context);
        }
    }
}
