namespace Sav.Common.Models
{
    public class PagedResult<T> where T : class
    {
        public int Page { get; set; }
        
        public int Count { get; set; }
        
        public int TotalCount { get; set; }
        
        public IEnumerable<T> Items { get; set; }
    }
}
