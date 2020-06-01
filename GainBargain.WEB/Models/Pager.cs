using System;

namespace GainBargain.WEB.Models
{
    public class Pager
    {
        public int TotalItems { get; private set; }
        public int CurrentPage { get; private set; }
        public int PageSize { get; private set; }
        public int TotalPages { get; private set; }
        public int StartPage { get; private set; }
        public int EndPage { get; private set; }
        public int PagerLength { get; private set; }

        public Pager(int totalItems, int? page, int pageSize, int pagerLength)
        {
            if (pageSize <= 0)
            {
                pageSize = 1;
            }

            var totalPages = (int)Math.Ceiling((decimal)totalItems / (decimal)pageSize);
            var currentPage = page != null ? (int)page : 1;
            if (pagerLength <= 0)
            {
                pagerLength = 1;
            }
            ++pagerLength;
            if (pagerLength >= totalItems)
            {
                pagerLength = totalItems - 1;
            }

            var isEven = pagerLength % 2 == 0;
            var startPage = 0;
            var endPage = 0;

            if (isEven)
            {
                startPage = currentPage - (int)(pagerLength / 2) + 1;
                endPage = currentPage + (int)(pagerLength / 2);
            }
            else
            {
                startPage = currentPage - (int)Math.Floor((decimal)pagerLength / 2);
                endPage = currentPage + (int)Math.Ceiling((decimal)pagerLength / 2) - 1;
            }

            if (startPage <= 0)
            {
                startPage = 1;
                endPage = pagerLength;
            }

            if (endPage >= totalPages)
            {
                endPage = totalPages;
                startPage = totalPages - pagerLength + 1;
            }

            TotalItems = totalItems;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = totalPages;
            StartPage = startPage;
            EndPage = endPage;
            PagerLength = pagerLength;
        }
}
}