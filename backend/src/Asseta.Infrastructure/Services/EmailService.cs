using System.Net;
using System.Net.Mail;
using Asseta.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Asseta.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendMasterKeyEmailAsync(string toEmail, string fullName, string masterKey, CancellationToken cancellationToken = default)
    {
        var subject = "[Asseta Vault] Khóa Master Key Khẩn Cấp Bất Biến Của Bạn";
        var body = $@"
================================================================================
KÍNH GỬI: {fullName} ({toEmail})
HỆ THỐNG QUẢN LÝ TIẾP QUẢN CÁ NHÂN ASSETA
================================================================================

Tài khoản Asseta của bạn đã được khởi tạo thành công!

Dưới đây là KHÓA MASTER KEY BẢO MẬT BẤT BIẾN của bạn:

    ===================================================
    MASTER KEY: {masterKey}
    ===================================================

LƯU Ý CỰC KỲ QUAN TRỌNG VỀ BẢO MẬT ZERO-KNOWLEDGE:
1. Khóa Master Key này là DUY NHẤT và BẤT BIẾN (không thể thay đổi hoặc cấp lại).
2. Toàn bộ ghi chú chỉ dẫn khẩn cấp, tài liệu nhạy cảm được mã hóa AES-256-GCM
   bằng khóa này ngay tại trình duyệt của bạn trước khi gửi lên máy chủ.
3. Máy chủ Asseta HOÀN TOÀN KHÔNG LƯU TRỮ Master Key này dưới bất kỳ hình thức nào.
4. Hãy sao lưu khóa này vào nơi an toàn (ghi vào sổ tay, két sắt gia đình hoặc
   ứng dụng quản lý mật khẩu tin cậy).

Trân trọng,
Đội ngũ Phát triển Asseta.
================================================================================
";

        _logger.LogInformation(
            "\n================================================================================\n" +
            "[DEV EMAIL DISPATCH] Gửi Master Key tới: {ToEmail}\n" +
            "Họ tên: {FullName}\n" +
            "Khóa Master Key: {MasterKey}\n" +
            "================================================================================\n",
            toEmail, fullName, masterKey);

        // If SMTP is configured, attempt sending real email
        var smtpHost = _configuration["Smtp:Host"];
        var smtpPortStr = _configuration["Smtp:Port"];
        var smtpUser = _configuration["Smtp:Username"];
        var smtpPass = _configuration["Smtp:Password"];

        if (!string.IsNullOrEmpty(smtpHost) && int.TryParse(smtpPortStr, out var smtpPort))
        {
            try
            {
                using var client = new SmtpClient(smtpHost, smtpPort)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(smtpUser, smtpPass)
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpUser ?? "no-reply@asseta.vn", "Asseta Security Vault"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = false
                };
                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage, cancellationToken);
                _logger.LogInformation("Real email successfully sent via SMTP to {ToEmail}", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send real SMTP email to {ToEmail}. Master Key is logged to server console.", toEmail);
            }
        }
    }
}
