using Microsoft.AspNetCore.ResponseCompression;
using Newtonsoft.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
//var MyAllowSpecificOrigins = "MyPolicy";
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    // Use the default property (Pascal) casing
    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
});
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy(name: MyAllowSpecificOrigins,
//                        builder => {
//                            builder.AllowAnyOrigin()
//                              .AllowAnyMethod()
//                              .AllowAnyHeader();
//                        });
//});

builder.Services.AddMemoryCache();
builder.Services.AddEndpointsApiExplorer();

builder.Services.Configure<GzipCompressionProviderOptions>(options => options.Level = System.IO.Compression.CompressionLevel.Optimal);
builder.Services.AddResponseCompression();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
//app.UseCors(MyAllowSpecificOrigins);
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
