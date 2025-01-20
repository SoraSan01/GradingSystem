using GradingSystem.Data;
using GradingSystem.Model;
using GradingSystem.View;
using GradingSystem.View.Admin;
using GradingSystem.View.Admin.Dialogs;
using System.Windows;
using System.Windows.Input;

namespace GradingSystem
{
    public partial class MainWindow : Window
    {
        private readonly ApplicationDbContext _context;

        private Dashboard _dashboard;
        private ManageStudents? _manageStudents;
        private ManageGrades? _manageGrades;
        private ManageUser? _manageUser;
        private ManageCourse? _manageCourse;
        private ManageSubjects? _manageSubjects;

        public MainWindow(ApplicationDbContext context)
        {
            InitializeComponent();
            _context = context;

            _dashboard = new Dashboard();
            MainContent.Content = _dashboard;
        }

        public void SwitchContent<T>(ref T contentPage) where T : class
        {
            if (contentPage == null)
            {
                contentPage = Activator.CreateInstance(typeof(T), _context) as T;
            }

            if (contentPage != null && MainContent.Content != contentPage)
            {
                MainContent.Content = contentPage;
            }
        }

        private void DashboardBtn(object sender, RoutedEventArgs e) => SwitchContent(ref _dashboard);

        private void LogoutBtn(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to log out?", "Logout", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                var login = new Login(_context);
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

        private void Minimize(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void Maximize(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to exit?", "Close", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        private void ManageStudentsBtn(object sender, RoutedEventArgs e) => SwitchContent(ref _manageStudents);

        private void ManageGradesBtn(object sender, RoutedEventArgs e) => SwitchContent(ref _manageGrades);

        private void ManageUserBtn(object sender, RoutedEventArgs e) => SwitchContent(ref _manageUser);

        private void ManageCourseBtn(object sender, RoutedEventArgs e) => SwitchContent(ref _manageCourse);

        private void ManageSubjectsBtn(object sender, RoutedEventArgs e) => SwitchContent(ref _manageSubjects);
    }
}
