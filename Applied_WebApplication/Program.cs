global using Applied_WebApplication.Data;
global using static Applied_WebApplication.Data.TableValidationClass;
using Applied_WebApplication;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSession();
builder.Services.AddSingleton<IAppliedDependency, AppliedDependency>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddRazorPages();
builder.Services.AddAuthentication("MyCookieAuth").AddCookie("MyCookieAuth", options =>
{
    options.Cookie.Name = "MyCookieAuth";
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.LogoutPath = "/Account/Logout";
    

});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", Policy => Policy.RequireClaim("Admin"));
    options.AddPolicy("AccountsManager", Policy => Policy.RequireClaim("AccountsManager"));
    options.AddPolicy("AccountsAssistant", policy => policy.RequireClaim("AccountAssistants"));
    options.AddPolicy("MustBelongToHRPolicy", policy => policy.RequireClaim("Department", "HR"));
    options.AddPolicy("StoreOnly", policy => policy.RequireClaim("Stock", "Store"));
    options.AddPolicy("Client", policy => policy.RequireClaim("Client", "Customer"));

});
builder.Services.AddHttpContextAccessor();
builder.Services.AddSwaggerGen();



var app = builder.Build();
var Rootpath = app.Environment.ContentRootPath;

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "SwaggerAPI");
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseCreateDatabase();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.Run();
