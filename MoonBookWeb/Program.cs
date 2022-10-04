using Microsoft.EntityFrameworkCore;
using MoonBookWeb.Midelewere;
using MoonBookWeb.Services;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
//include Database
builder.Services.AddDbContext<MoonBookWeb.AddDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("MoonBookDb")));
builder.Services.AddSingleton<Hesher>();
builder.Services.AddScoped<ISessionLogin, SessionLoginServices>();
builder.Services.AddScoped<ChekUser>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(72);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();
//Include Middelwere session login
app.UseSessionLogin();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
