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
using GradingSystem.Data;
using GradingSystem.View.Admin;
using GradingSystem.View.Encoder;
using Microsoft.Extensions.DependencyInjection;

namespace GradingSystem.View.Encoder
{
    public partial class EncoderMainWindow : Window
    {
        private readonly ApplicationDbContext _context;
        private Dashboard _dashboard;
        private Grades _grades;
        private EnrollmentDashboard _enrollmentDashboard;
        private readonly IServiceProvider _serviceProvider;


        public EncoderMainWindow(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;

            // Initialize Dashboard on startup
            _dashboard = _serviceProvider.GetRequiredService<Dashboard>();
            MainContent.Content = _dashboard;
        }

        private void SwitchContent<T>(ref T? contentPage) where T : class
        {
            try
            {
                // Lazy loading for views that require dependencies
                if (contentPage == null)
                {
                    // Resolve views from DI container instead of creating them manually
                    contentPage = (T)(_serviceProvider.GetRequiredService(typeof(T)));
                }

                if (contentPage != null && MainContent.Content != contentPage)
                {
                    MainContent.Content = contentPage;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while switching to {nameof(T)}: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ManageGradesBtn(object sender, RoutedEventArgs e) => SwitchContent(ref _grades);
        private void DashboardBtn(object sender, RoutedEventArgs e) => SwitchContent(ref _dashboard);
        private void EnrollmentBtn(object sender, RoutedEventArgs e) => SwitchContent(ref _enrollmentDashboard);



        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to exit?", "Close", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        private void Maximize(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void Minimize(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void LogoutBtn(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to log out?", "Logout", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                var login = App.ServiceProvider.GetRequiredService<Login>(); // Resolves Login via DI
                login.Show();
                this.Close();
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}
