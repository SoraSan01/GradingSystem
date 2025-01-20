using GradingSystem.Model;
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
    /// Interaction logic for EditStudent.xaml
    /// </summary>
    public partial class EditStudent : Window
    {
        public Student SelectedStudent { get; set; }
        public ProgramViewModel ProgramViewModel { get; set; }

        public EditStudent(Student student, ProgramViewModel Programs)
        {
            InitializeComponent();

            ProgramViewModel = Programs;
            this.SelectedStudent = student;
            this.DataContext = this;

            programCmb.ItemsSource = ProgramViewModel.Programs;

        }

        private void Close(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to exit?", "Close", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void saveBtn(object sender, RoutedEventArgs e)
        {
            if (SelectedStudent != null)
            {
                try
                {
                    // Call the ViewModel's EditStudent method
                    var viewModel = (StudentsViewModel)this.DataContext;
                    _ = viewModel.EditStudentAsync(SelectedStudent); // Update the student in the database

                    // Close the window after editing
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while saving: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("No student selected to edit.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed && e.OriginalSource is not TextBox && e.OriginalSource is not Button)
            {
                this.DragMove();
            }
        }
    }
}
