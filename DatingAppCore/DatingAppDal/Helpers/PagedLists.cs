using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DatingAppWebApi.Helpers
{
    public class PagedLists<T>:List<T>
    {
        public int CurrentPage { get; set; }
        public int TotalPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        private static int _pageNumber{get;set;}
        private static int _pageSize{get;set;}

        public PagedLists(List<T> items,int Count, int pageSize, int pageNumber)
        {
          this.AddRange(items);  
          this.TotalCount=Count;
          this.PageSize= pageSize;
          this.CurrentPage= pageNumber;
          this.TotalPage= (int)Math.Ceiling(Count/(double)pageSize);
        }
        public static async Task<PagedLists<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = await source.CountAsync();
            _pageNumber=pageNumber==0?1:pageNumber;
            _pageSize= pageSize==0?5:pageSize;
            var items = await source.Skip((_pageNumber-1)*pageSize).Take(_pageSize).ToListAsync();
            return new PagedLists<T>(items,count,pageSize,pageNumber);
        }
    }
}