namespace Identity.Helpers
{
    public class Meta
    {
        public int TotalPages { get; set; }
        public int Page { get; set; }

        public static Meta FromMeta(int totalCount, int page)
        {
            var totalPages = 1;

            var res = (double)(totalCount / Paginations.PerPage);
            if (res % 10 == 0)
                totalPages = (int)res;
            else
                totalPages = (int)Math.Ceiling(res);

            var meta = new Meta();
            meta.Page = page;
            meta.TotalPages = totalPages;
            return meta;
        }
    }
}
