using Personnel.Tracker.Model.Base;
using System;

namespace Personnel.Tracker.Model.Action
{

    public class Query
    {
        public Query()
        {
            PageSize = 10;
        }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string OrderBy { get; set; }
        public OrderDir OrderDir { get; set; }
        public Guid UserId { get; set; } 
    }

    public class Query<T> : Query
    {
        public Query()
        {

        }
        public Query(T param)
        {
            Parameter = param;
        }
        public T Parameter { get; set; }
    }
}
