using BusinessLayer.InterFace;
using BusinessLayer.Repository;
using DataAccessLayer.DataContext;
using Humanizer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using ServiceStack.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ILogin, Login>();    
builder.Services.AddScoped<IPatientRequest,PatientRequest>();    
builder.Services.AddScoped<IOtherRequest,OtherRequest>(); 
builder.Services.AddScoped<IAdmin,AdminRepository>();
builder.Services.AddScoped<IJwtAuth, JwtAuthRepo>();
RotativaConfiguration.Setup(builder.Environment.WebRootPath, "Rotativa");


builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = long.MaxValue;
});
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(1); // Set your desired timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();
var hostingEnvironment = app.Services.GetRequiredService<IHostEnvironment>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}



app.UseHttpsRedirection();
app.UseRotativa();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
