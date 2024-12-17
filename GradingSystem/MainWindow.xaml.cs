using GradingSystem.View;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GradingSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainContent.Content = new Dashboard();
        }

        private void dashboardBtn(object sender, RoutedEventArgs e)
        {
            if (MainContent.Content is not Dashboard) {
                MainContent.Content = new Dashboard();
            }
        }

        private void logoutBtn(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to Log out?", "Close", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                Login login = new Login();
                login.Show();

                this.Close();
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                // Call DragMove to allow the window to be dragged
                this.DragMove();
            }
        }

        private void ProfessorBtn(object sender, RoutedEventArgs e)
        {
            if (MainContent.Content is not ProfessorForm)
            {
                MainContent.Content = new ProfessorForm();
            }
        }

        private void studentsBtn(object sender, RoutedEventArgs e)
        {
            if (MainContent.Content is not Students)
            {
                MainContent.Content = new Students();
            }
        }

        private void Minimize(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.WindowState = WindowState.Minimized; // Minimize the window
        }

        private void Maximize(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window.WindowState == WindowState.Maximized)
                window.WindowState = WindowState.Normal; // Restore window to normal
            else
                window.WindowState = WindowState.Maximized; // Maximize the window
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to exit?", "Close", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();  // Close the application
            }
        }
    }
}