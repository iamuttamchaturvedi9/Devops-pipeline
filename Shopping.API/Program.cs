using Shopping.API.Data;

namespace Shopping.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped<ProductContext>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            // Enable Swagger in all environments for API documentation
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Shopping API V1");
            });

            app.UseAuthorization();

            // Add a simple root endpoint to show API is running
            app.MapGet("/", () => "Shopping API is running! Visit /swagger for API documentation or /api/Product for products.");

            app.MapControllers();

            app.Run();
        }
    }
}
