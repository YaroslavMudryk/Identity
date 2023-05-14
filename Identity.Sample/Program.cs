
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
                options.Features.IsAvailableRestorePassword = true;
                options.Features.IsAvailableMfa = true;
                options.Features.IsAvailableSessions = true;
                options.Features.IsAvailableConfirm = true;
                options.Features.IsAvailableRefreshToken = true;
                options.Features.IsAvailableRestorePassword = true;

                options.Routes.IsAvailableToDisplayRoutes = true;

                options.Token.Issuer = "Identity ID";
                options.Token.Audience = "Identity Client";
                options.Token.LifeTimeInMinutes = 180;

            }, new Options.IdentityDatabase
            {
                ConnectionString = "Server=(localdb)\\mssqllocaldb;Database=IdentityDb;Trusted_Connection=True;MultipleActiveResultSets=true",
                Provider = new SqlServerProvider()
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

            app.MapControllers();

            app.UseIdentityHandler();

            app.Run();
        }
    }
}