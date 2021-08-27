namespace Personnel.Tracker.Model.Action
{
    public class SysException : System.Exception
    {
        public string Code { get; }

        public SysException()
        {
        }
        public SysException(string code)
        {
            Code = code;
        }

        public SysException(string message, params object[] args)
            : this(string.Empty, message, args)
        {
        }

        public SysException(string code, string message, params object[] args)
            : this(null, code, message, args)
        {
        }

        public SysException(System.Exception innerException, string message, params object[] args)
            : this(innerException, string.Empty, message, args)
        {
        }

        public SysException(System.Exception innerException, string code, string message, params object[] args)
            : base(string.Format(message, args), innerException)
        {
            Code = code;
        }
    }
}
