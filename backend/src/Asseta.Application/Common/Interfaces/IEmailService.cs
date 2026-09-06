namespace Asseta.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendMasterKeyEmailAsync(string toEmail, string fullName, string masterKey, CancellationToken cancellationToken = default);
}
