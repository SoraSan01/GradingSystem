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
using GradingSystem.View.Encoder;
using Microsoft.Extensions.DependencyInjection;

namespace GradingSystem.View.Encoder
{
    public partial class EncoderMainWindow : Window
    {
        private readonly ApplicationDbContext _context;
        private Dashboard _dashboard;

        public EncoderMainWindow(ApplicationDbContext context)
        {
            InitializeComponent();
            _context = context;

            _dashboard = new Dashboard();
            MainContent.Content = _dashboard;
        }

        private void ManageGradesBtn(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new Grades(_context);
        }

        private void DashboardBtn(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new Dashboard();
        }

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
