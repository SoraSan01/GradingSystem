using GradingSystem.Data;
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

namespace GradingSystem.View.Admin
{
    /// <summary>
    /// Interaction logic for BulkInsertCourse.xaml
    /// </summary>
    public partial class BulkInsertCourse : Window
    {
        private BulkInsertCourseViewModel _viewModel;

        public BulkInsertCourse(ApplicationDbContext context)
        {
            InitializeComponent();

            _viewModel = new BulkInsertCourseViewModel(context);
            DataContext = _viewModel;
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
