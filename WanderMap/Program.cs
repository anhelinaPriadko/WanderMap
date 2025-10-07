using Microsoft.EntityFrameworkCore;
using WanderMap.Data;

var builder = WebApplication.CreateBuilder(args);

// 1?? ������ MVC ���������� + Views
builder.Services.AddControllersWithViews();

// 2?? �������� ��� �������� � PostgreSQL
builder.Services.AddDbContext<WanderMapDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3?? ��������� ����������
var app = builder.Build();

// 4?? Middleware
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// 5?? �������
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//for each entity create a slug route
//app.MapControllerRoute(
//    name: "article",
//    pattern: "articles/{slug}",
//    defaults: new { controller = "Article", action = "Details" });

app.Run();
