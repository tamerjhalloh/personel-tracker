using System.Collections.Generic;

namespace Personnel.Tracker.Model.Action
{
    public class OperationResult
    {
        public bool Result { get; set; }
        public string Message { get; set; }
        public string ErrorCategory { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string SysErrorMessage { get; set; }
    }

    public class OperationResult<TResponse> : OperationResult
    {
        public OperationResult() { }
        public OperationResult(TResponse response)
        {
            Response = response;
        }

        public OperationResult(OperationResult result)
        {
            Result = result.Result;
            Message = result.Message;
            ErrorCode = result.ErrorCode;
            ErrorMessage = result.ErrorMessage;
            SysErrorMessage = result.SysErrorMessage;
            ErrorCategory = result.ErrorCategory;
        }
        public TResponse Response { get; set; }
    }

    public class PaggedOperationResult<TResponse> : OperationResult<IEnumerable<TResponse>>
    {
        public PaggedOperationResult()
        {
            Response = new List<TResponse>();
        }
        public PaggedOperationResult(IEnumerable<TResponse> responses)
        {
            Response = responses;
        }
        public PaggedOperationResult(OperationResult result)
        {
            Result = result.Result;
            Message = result.Message;
            ErrorCode = result.ErrorCode;
            ErrorMessage = result.ErrorMessage;
            SysErrorMessage = result.SysErrorMessage;
            ErrorCategory = result.ErrorCategory;
            Response = new List<TResponse>();
        }
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
