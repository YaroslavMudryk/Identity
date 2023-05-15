namespace Identity.Models.Response
{
    public class APIResponse
    {
        public bool Ok { get; set; }
        public string Error { get; set; }
        public object Data { get; set; }
    }
}
