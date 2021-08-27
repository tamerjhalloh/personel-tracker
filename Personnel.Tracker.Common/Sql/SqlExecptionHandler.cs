using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Personnel.Tracker.Model.Action;
using Personnel.Tracker.Model.Base;
using System;
using System.Text.RegularExpressions;

namespace Personnel.Tracker.Common.Sql
{
    public class SqlExecptionHandler
    {
        private readonly ILogger<SqlExecptionHandler> _logger;

        private const int SqlServerViolationOfUniqueIndex = 2601;
        private const int SqlServerViolationOfUniqueConstraint = 2627;
        public SqlExecptionHandler(ILogger<SqlExecptionHandler> logger)
        {
            _logger = logger;
        }

        public void HandelException
            (Exception e, OperationResult result)
        {
            result.Result = false;
            result.ErrorCategory = ErrorCategory.Exception;
            result.ErrorCode = ErrorCode.Exception; 
            result.ErrorMessage = ErrorCode.Exception;
            result.SysErrorMessage = e.Message;
            _logger.LogError(e, "SqlExecption");
            if (e.InnerException != null && e.InnerException is SqlException)
            {
                var sqlEx = e.InnerException as SqlException;
                //This is a DbUpdateException on a SQL database

                if (sqlEx.Number == SqlServerViolationOfUniqueIndex ||
                    sqlEx.Number == SqlServerViolationOfUniqueConstraint)
                {
                    //We have an error we can process
                    UniqueErrorFormatter(sqlEx, result);
                }
                else
                {
                    result.SysErrorMessage = $"{e.Message}  - {e.InnerException.Message}";
                }
            }
            else
            {
                result.SysErrorMessage = $"{e.Message}";
            }
        }

        private readonly Regex UniqueConstraintRegex =
                    new Regex("'UniqueError_([a-zA-Z0-9]*)_([a-zA-Z0-9]*)'", RegexOptions.Compiled);

        private void UniqueErrorFormatter(SqlException ex, OperationResult result)
        {
            var message = ex.Errors[0].Message;
            var matches = UniqueConstraintRegex.Matches(message);

            if (matches.Count == 0)
                return;

            var entityDisplayName = matches[0].Groups[1].Value; 

            _logger.LogError("Cannot have a duplicate {value} in  {entity}", matches[0].Groups[2].Value, entityDisplayName);

            var openingBadValue = message.IndexOf("(");
            if (openingBadValue > 0)
            {
                var dupPart = message.Substring(openingBadValue + 1,
                    message.Length - openingBadValue - 3); 
                _logger.LogError("Duplicate value was  {value}", dupPart);
            }

            result.ErrorCode = ErrorCode.UsedBefore;
            result.ErrorCategory = ErrorCategory.Exception;
            result.ErrorMessage = matches[0].Groups[2].Value; 
        }
    }
}
