using System.Collections.Generic;
using System.Linq;

namespace SaluteOnline.Domain.Domain
{
    public class Page<T>
    {
        public Page() {}

        public Page(int currentPage, int pageSize, long total, int totalPages, IEnumerable<T> items)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            Total = total;
            TotalPages = totalPages;
            Items = items?.ToList() ?? new List<T>();
        }

        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public long Total { get; set; }
        public int TotalPages { get; set; }
        public List<T> Items { get; set; }
    }
}
