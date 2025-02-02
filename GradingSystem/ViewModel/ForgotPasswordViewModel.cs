using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Windows;
using GradingSystem.Data;

namespace GradingSystem.ViewModel
{
    public class ForgotPasswordViewModel
    {
        private readonly ApplicationDbContext _dbContext;

        public string Email { get; set; }
        public string EnteredOtp { get; set; } // To store the OTP entered by the user

        public int GeneratedOtp { get; private set; } // To store the generated OTP

        public ForgotPasswordViewModel(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void SendOtp()
        {
            try
            {
                // Find the user by email
                var user = _dbContext.Users.FirstOrDefault(u => u.Email == Email);

                if (user == null)
                {
                    MessageBox.Show("No account found with this email address.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Generate an OTP
                GeneratedOtp = GenerateOtp();

                // Send the OTP via email
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("reyveneran01@gmail.com", "jvra oufw osoy eaoc"),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("reyveneran01@gmail.com"),
                    Subject = "OTP for Password Reset",
                    Body = $"Hello {user.Email},\n\nYour OTP for password reset is: {GeneratedOtp}\n\nThis OTP is valid for the current session.\n\nThank you!",
                    IsBodyHtml = false,
                };

                mailMessage.To.Add(user.Email);

                smtpClient.Send(mailMessage);

                MessageBox.Show("An OTP has been sent to your email. Please check your inbox.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to send OTP. Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void VerifyOtp()
        {
            if (string.IsNullOrWhiteSpace(EnteredOtp) || !int.TryParse(EnteredOtp, out int otp))
            {
                MessageBox.Show("Please enter a valid OTP.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (otp == GeneratedOtp)
            {
                MessageBox.Show("OTP verified successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Resolve ChangePassword window from IServiceProvider
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var changePasswordWindow = new ChangePassword(_dbContext, Email, App.ServiceProvider);
                    changePasswordWindow.ShowDialog();
                });
            }
            else
            {
                MessageBox.Show("Invalid OTP. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private int GenerateOtp()
        {
            var random = new Random();
            return random.Next(100000, 999999); // Generates a 6-digit OTP
        }
    }
}
