using Microsoft.AspNetCore.Identity.UI.Services;

namespace EmployeeManagementSystem.Services
{
    public class EmailSender : IEmailSender    // Creating a dummy email-sender to keep it simple
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Task.CompletedTask;
        }
    }
}
