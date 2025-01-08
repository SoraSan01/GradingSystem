using GradingSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GradingSystem
{
    /// <summary>
    /// Interaction logic for FogotPassword.xaml
    /// </summary>
    public partial class ForgotPassword : Window
    {

        private readonly ForgotPasswordViewModel _viewModel;  // Declare _viewModel as a private field

        public ForgotPassword()
        {
            InitializeComponent();

            // Initialize _viewModel with the ApplicationDbContext
            _viewModel = new ForgotPasswordViewModel();

            // Set the DataContext for data binding
            DataContext = _viewModel;
        }

        private void closeBtn(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to exit?", "Close", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();  // Close the application
            }
        }

        private void sendBtn(object sender, RoutedEventArgs e)
        {
            string email = emailTxt.Text;

            // Validate email format before sending OTP
            if (string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Please enter your email.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Validate email format using a simple regex pattern
            if (!IsValidEmail(email))
            {
                MessageBox.Show("Please enter a valid email address.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Set email for OTP sending and disable the email textbox
            _viewModel.Email = email;
            emailTxt.IsReadOnly = true; // Disable email input after OTP is sent

            _viewModel.SendOtp();
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void verifyBtn(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
            {
                string enteredOtp = codeTxt.Text;
                string email = emailTxt.Text;

                // Validate OTP and email before verifying
                if (string.IsNullOrWhiteSpace(enteredOtp))
                {
                    MessageBox.Show("Please enter the OTP.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (string.IsNullOrWhiteSpace(email))
                {
                    MessageBox.Show("Please enter your email address.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Set entered OTP and email for verification
                _viewModel.EnteredOtp = enteredOtp;
                _viewModel.Email = email;

                _viewModel.VerifyOtp();
                this.Close();
            }
        }
    }
}
