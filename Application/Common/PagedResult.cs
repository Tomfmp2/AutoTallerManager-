using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Common;

public class PagedResult<T>
{
    public IReadOnlyCollection<T> Items { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalCount;
    public PagedResult(IReadOnlyCollection<T> items, int totalConunt, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = totalConunt;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}