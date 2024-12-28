

namespace Application.Common.Exceptions;

//public static class UserException
//{
//    public static UserFriendlyException UserAlreadyExistsException(string field)
//        => new(ErrorCode.BadRequest, string.Format(UserErrorMessage.AlreadyExists, field), string.Format(UserErrorMessage.AlreadyExists, field));

//    public static UserFriendlyException UserUnauthorizedException()
//        => new(ErrorCode.Unauthorized, UserErrorMessage.Unauthorized, UserErrorMessage.Unauthorized);

//    public static UserFriendlyException InternalException(Exception? exception)
//        => new(ErrorCode.Internal, ErrorMessage.Internal, ErrorMessage.Internal, exception);

//    public static UserFriendlyException BadRequestException(string errorMessage)
//        => new(ErrorCode.BadRequest, errorMessage, errorMessage);

//}
public class InvalidTokenException(string message) : Exception(message)
{
}

public class UserNotFoundException(string message) : Exception(message)
{
}

public class PasswordIncorrectException(string message) : Exception(message)
{
}
public class UserAlreadyExistsException(string message) : Exception(message)
{
}

public class UserException(string message) : Exception(message)
{
}

