using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Http.Features;
using WebApi.Services; 

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews() 
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.MaxDepth = 64;
    });

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 100_000_000;
    options.ValueLengthLimit = 2048; //  По умолчанию
    options.MemoryBufferThreshold = 131072;
});

builder.Services.AddScoped<VideoService>(); // Простая регистрация, IConfiguration передается автоматически


builder.Services.AddSingleton<IContentTypeProvider, FileExtensionContentTypeProvider>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
