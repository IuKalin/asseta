namespace Asseta.Application.Common.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
    public NotFoundException(string entityName, object key) : base($"Entity '{entityName}' ({key}) was not found.") { }
}

public class ConcurrencyException : Exception
{
    public ConcurrencyException(string message) : base(message) { }
}

public class SensitiveDataDetectedException : Exception
{
    public SensitiveDataDetectedException(string fieldName, string patternDetected)
        : base($"Sensitive data pattern '{patternDetected}' was detected in field '{fieldName}'. Under Zero-Knowledge policy, sensitive data must be encrypted client-side.")
    {
        FieldName = fieldName;
        PatternDetected = patternDetected;
    }

    public string FieldName { get; }
    public string PatternDetected { get; }
}

public class UnauthorizedResourceAccessException : Exception
{
    public UnauthorizedResourceAccessException(string message = "You do not have permission to access this resource.")
        : base(message) { }
}

public class AuthException : Exception
{
    public AuthException(string message = "Invalid email or password.")
        : base(message) { }
}

public class ConflictException : Exception
{
    public ConflictException(string message) : base(message) { }
}

public class PairingAttemptsExceededException : Exception
{
    public PairingAttemptsExceededException(string message = "Bạn đã nhập sai mã ghép đôi quá 3 lần. Vui lòng thử lại sau 15 phút.")
        : base(message) { }
}

public class PairingCodeExpiredOrInvalidException : Exception
{
    public PairingCodeExpiredOrInvalidException(string message = "Mã ghép đôi không hợp lệ hoặc đã hết hạn (48h).")
        : base(message) { }
}

public class DuplicateContactException : Exception
{
    public DuplicateContactException(string message = "Số điện thoại hoặc email đã tồn tại trong danh sách Người Ủy Thác của bạn.")
        : base(message) { }
}

public class MaxTrustedPeopleExceededException : Exception
{
    public MaxTrustedPeopleExceededException(string message = "Bạn chỉ có thể thiết lập tối đa 5 Người Ủy Thác trong phạm vi tài khoản.")
        : base(message) { }
}

public class SelfDelegationProhibitedException : Exception
{
    public SelfDelegationProhibitedException(string message = "Không thể sử dụng chính tài khoản chủ tài sản để ghép đôi ủy thác.")
        : base(message) { }
}

public class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException(string message = "Bạn không có quyền thực hiện hành động này.")
        : base(message) { }
}


