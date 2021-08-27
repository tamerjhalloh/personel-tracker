namespace Personnel.Tracker.Model.Base
{
    public class ErrorCategory
    {
        public const string Exception = "Exception";
        public const string Authorization = "Authorization";
        public const string Service = "Service";
        public const string WebService = "WebService";
        public const string Preperation = "Preperation";
        public const string Validation = "Validation";
        public const string Error = "Error";
    }


    public class ErrorCode
    {

        //General 
        public const string Validation = "System.General.Validation";
        public const string Exception = "System.General.Exception";
        public const string WebService = "System.General.WebService";
        public const string Service = "System.General.Service"; 
        public const string NotFound = "System.General.NotFound";
        public const string UsedBefore = "System.General.UsedBefore";
        public const string MissingParameter = "System.General.MissingParameter";

        //Validation
        public const string MaxLengthExceeded = "System.Validation.MaxLengthExceeded";
        public const string ValueIsEmpy = "System.Validation.EmptyValue";
        public const string ExistsBefore = "System.Validation.ExistsBefore"; 
        public const string NotAllowed = "System.Validation.NotAllowed"; 

    }
}
