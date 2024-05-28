using Microsoft.AspNetCore.Http.Features;
using NTierApplication.Web.ActionHelpers;
using WebTotalComander.Repository.Services;
using WebTotalComander.Service.Services;

namespace WebTotalComander.Server;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers(options => options.Filters.Add<ApiExceptionFilterAttribute>());

        // Add services to the container.

        builder.Services.AddMvc();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle


        //folder dependency injection
        var uploadFolderPath = builder.Configuration["AppSettings:UploadFolderPath"];
        var folderSettings = new FolderSettings(uploadFolderPath);
        builder.Services.AddSingleton<FolderSettings>(folderSettings);

        // azure dependency injection
        var azureConnection = builder.Configuration["ConnectionStrings:AzureBlobStorageConnection"];
        var azureContainer = builder.Configuration["ConnectionStrings:AzureBlobStorageContainer"];
        var azureSettings = new AzureSettings(azureConnection, azureContainer);
        builder.Services.AddSingleton<AzureSettings>(azureSettings);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddScoped<IFileService, FileService>();
        builder.Services.AddScoped<IFileRepository, AzureFileRepository>();
        builder.Services.AddScoped<IFolderRepository, AzureFolderRepository>();
        //builder.Services.AddScoped<IFileRepository, FileRepository>();
        //builder.Services.AddScoped<IFolderRepository, FolderRepository>();
        builder.Services.AddScoped<IFolderService, FolderService>();
        builder.Services.AddScoped<IFilterService, FilterService>();

        builder.Services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = 1024 * 1024 * 2024;
        });

        /*
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin",
                builder => builder.WithOrigins("https://localhost:4200")
                    .AllowAnyHeader()
                    .AllowAnyMethod());
        });
        */

        var allowedOrigins = builder.Configuration.GetSection("Url:AllowedOrigins").Get<string[]>();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin",
                builder => builder.WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod());
        });

        var app = builder.Build();

        app.UseDefaultFiles();
        app.UseStaticFiles();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors("AllowSpecificOrigin");

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.MapFallbackToFile("/index.html");

        app.Run();
    }
}
