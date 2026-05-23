using System.Collections.Generic;

namespace PCBuilder.Application;

public class PaginatedResponse<T>
{
    public List<T> Data { get; set; } = new List<T>();
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => PageSize > 0 ? (TotalCount + PageSize - 1) / PageSize : 0;
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageSize > 0 && PageNumber < TotalPages;
}
