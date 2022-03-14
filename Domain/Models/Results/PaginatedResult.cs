using System.Collections.Generic;

namespace Domain.Models.Results
{
    public class PaginatedResult<T>
    {
        public int TotalCount { get; set; }
        public IReadOnlyCollection<T> Results { get; set; }
    }
}