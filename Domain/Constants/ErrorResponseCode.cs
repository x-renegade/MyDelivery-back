

namespace Domain.Constants
{
    public static class ErrorResponseCode
    {
        public static readonly string NOT_FOUND = "not_found";
        public static readonly string VERSION_CONFLICT = "version_conflict";
        public static readonly string ITEM_ALREADY_EXISTS = "item_exists";
        public static readonly string CONFLICT = "conflict";
        public static readonly string BAD_REQUEST = "bad_request";
        public static readonly string UNAUTHORIZED = "unauthorized";
        public static readonly string INTERNAL_ERROR = "internal_error";
        public static readonly string GENERAL_ERROR = "general_error";
        public static readonly string UNPROCESSABLE_ENTITY = "unprocessable_entity";
    }
}
