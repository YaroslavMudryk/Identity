namespace Identity.Models.Response
{
    public class APIResponse
    {
        public bool Ok { get; set; }
        public string Error { get; set; }
        public object Data { get; set; }

        public APIResponse()
        {
            
        }

        public APIResponse(bool ok, string error, object data)
        {
            Ok = ok;
            Error = error;
            Data = data;
        }

        public static APIResponse OK(string error = null, object data = null)
        {
            return new APIResponse(true, error, data);
        }

        public static APIResponse BadRequest(string error = null, object data = null)
        {
            return new APIResponse(false, error, data);
        }

        public static APIResponse Custom(bool ok, string error, object data)
        {
            return new APIResponse(ok, error, data);
        }
    }
}