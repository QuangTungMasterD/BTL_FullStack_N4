using System.Net;
using System.Net.Mail;

namespace PaymentReportService.API.Services;

public class EmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendAccountEmailAsync(
        string email,
        string fullName,
        string username,
        string password)
    {
        var smtpServer =
            _configuration["EmailSettings:SmtpServer"];

        var port =
            int.Parse(
                _configuration["EmailSettings:Port"]!);

        var senderEmail =
            _configuration["EmailSettings:SenderEmail"];

        var senderPassword =
            _configuration["EmailSettings:Password"];

        using var client =
            new SmtpClient(smtpServer, port);

        client.EnableSsl = true;

        client.Credentials =
            new NetworkCredential(
                senderEmail,
                senderPassword);

        var message = new MailMessage();

        message.From =
            new MailAddress(
                senderEmail!,
                "EduTrack");

        message.To.Add(email);

        message.Subject =
            "Tài khoản giảng viên EduTrack";

        message.Body = $@"
Xin chào {fullName},

Admin đã tạo tài khoản cho bạn.

=================================

Username: {username}

Password: {password}

=================================

Link đăng nhập:

http://localhost:5173/login

Vui lòng đổi mật khẩu sau lần đăng nhập đầu tiên.

EduTrack Team
";

        await client.SendMailAsync(message);
    }
}