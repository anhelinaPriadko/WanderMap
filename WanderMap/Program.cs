using Microsoft.EntityFrameworkCore;
using WanderMap.Data;
using WanderMap.Services;

var builder = WebApplication.CreateBuilder(args);

// 1?? Додаємо MVC контролери + Views
builder.Services.AddControllersWithViews();

// 2?? Реєструємо наш контекст з PostgreSQL
builder.Services.AddDbContext<WanderMapDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ISlugService, SlugService>();    
// 3?? Створюємо застосунок
var app = builder.Build();

// 4?? Middleware
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// 5?? Роутинг
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//for each entity create a slug route
//app.MapControllerRoute(
//    name: "article",
//    pattern: "articles/{slug}",
//    defaults: new { controller = "Article", action = "Details" });

app.Run();
