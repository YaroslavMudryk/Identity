
using Identity.Db.Providers;

namespace Identity.Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddIdentityServices(options =>
            {
                options.Routes.RefreshRoute = "/api/v1/refresh";
                options.Features.IsAvailableRefreshToken = true;

                options.Token.Issuer = "Identity ID";
                options.Token.Audience = "Identity Client";
                options.Token.LifeTimeInMinutes = 180;

            }, new Options.IdentityDatabase
            {
                ConnectionString = "database.db3",
                Provider = new SqliteProvider()
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseIdentityHandler();

            app.MapControllers();

            app.Run();
        }
    }
}