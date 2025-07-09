using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.StaticFiles;
using MongoDB.Driver;
using WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.MaxDepth = 64;
    });  

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 100_000_000; // 100 MB
    options.ValueLengthLimit = 2048; //  По умолчанию
    options.MemoryBufferThreshold = 131072;
});

builder.Services.AddSingleton<IMongoClient>(s =>
    new MongoClient(builder.Configuration.GetValue<string>("MongoDb:ConnectionString")));

builder.Services.AddScoped<VideoService>(s =>
    new VideoService(s.GetRequiredService<IMongoClient>(), builder.Configuration.GetValue<string>("MongoDb:DatabaseName")));

builder.Services.AddSingleton<IContentTypeProvider, FileExtensionContentTypeProvider>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseStaticFiles();
app.Run();
