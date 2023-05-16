using Identity.Helpers;
using Identity.Models.Response;

namespace Identity.Extensions
{
    public static class ResultExtensions
    {
        public static (APIResponse,int) MapToResponse<T>(this Result<T> result)
        {
            var code = result.IsSuccess ? 200 : result.IsNotFound ? 404 : result.IsForbid ? 401 : result.IsError ? 400 : 500;

            return (new APIResponse
            {
                Ok = result.IsSuccess,
                Error = result.ErrorMessage,
                Data = result.Data,
            }, code);
        }
    }
}