

namespace Domain.Constants
{
    public static class ErrorMessage
    {
        public static readonly string Internal = "something went wrong";
        public static readonly string NotFoundMessage = "Could not find";
        public static readonly string AppConfigurationMessage = "Can not get appsetting variables";
        public static readonly string TransactionNotCommit = "Transaction can not commit";
        public static readonly string TransactionNotExecute = "Transaction can not execute";
    }
}
