using Identity.Helpers;

namespace Identity.Seeder
{
    public interface ISeederService
    {
        Task<Result<bool>> SeedSystemAsync();
    }
}
