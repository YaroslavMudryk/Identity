using Newtonsoft.Json;

namespace Identity.Extensions
{
    public static class StreamExtensions
    {
        public async static Task<T> GetBodyAsync<T>(this Stream stream)
        {
            using var reader = new StreamReader(stream);

            var requestBody = await reader.ReadToEndAsync();

            return JsonConvert.DeserializeObject<T>(requestBody);
        }
    }
}