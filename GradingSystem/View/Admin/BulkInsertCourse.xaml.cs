using GradingSystem.Data;
using GradingSystem.ViewModel;
using System.Windows;
using System.Windows.Input;

namespace GradingSystem.View.Admin
{
    public partial class BulkInsertCourse : Window
    {
        private BulkInsertCourseViewModel _viewModel;

        public BulkInsertCourse(ApplicationDbContext context)
        {
            InitializeComponent();
            _viewModel = new BulkInsertCourseViewModel(context);
            this.DataContext = _viewModel;
        }

        private async void BrowseBtn(object sender, RoutedEventArgs e)
        {
            await _viewModel.ChooseFileAsync();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to exit?", "Close", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }
    }
}
