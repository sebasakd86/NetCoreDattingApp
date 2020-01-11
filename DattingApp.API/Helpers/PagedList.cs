using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DattingApp.API.Helpers
{
    public class PagedList<T> : List<T>
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public PagedList(List<T> items, int count, int pNumber, int pSize)
        {
            this.TotalCount = count;
            this.CurrentPage = pNumber;
            this.PageSize = pSize;
            this.TotalPages = (int)Math.Ceiling(this.TotalCount / (double)this.PageSize);
            this.AddRange(items);
        }

        public static async Task<PagedList<T>> CreateAsync(
            IQueryable<T> src
            , int pNumber
            , int pSize
        )
        {
            int count = await src.CountAsync();
            var items = await src.Skip((pNumber - 1) * pSize).Take(pSize).ToListAsync();
            return new PagedList<T>(items, count, pNumber, pSize);
        }
    }
}