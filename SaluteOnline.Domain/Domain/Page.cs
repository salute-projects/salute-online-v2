using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SaluteOnline.Domain.Domain
{
    public class Page<T>
    {
        public Page() {}

        public Page(int currentPage, int pageSize, long total, IEnumerable<T> items)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            Total = total;
            var enumerable = items as T[] ?? items.ToArray();
            Items = enumerable.ToList();
            TotalItems = enumerable.Length;
            TotalPages = (int) Math.Ceiling((double) total / pageSize);
        }

        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public long Total { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public List<T> Items { get; set; }
    }
}
