using Applied_WebApplication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddRazorPages().AddRazorRuntimeCompilation(); ;
builder.Services.AddAuthentication("MyCookieAuth").AddCookie("MyCookieAuth", options =>
{
    options.Cookie.Name = "MyCookieAuth";
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/AccessDenied";
    
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", Policy => Policy.RequireClaim("Admin"));
    options.AddPolicy("AccountsManager", Policy => Policy.RequireClaim("Accounts_Manager"));
    options.AddPolicy("AccountsClark", Policy => Policy.RequireClaim("Accounts_Clark"));
    options.AddPolicy("MustBelongToHRPolicy", policy => policy.RequireClaim("Department", "HR"));
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCreateDatabase();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
