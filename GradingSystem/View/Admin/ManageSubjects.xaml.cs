using GradingSystem.Data;
using GradingSystem.Model;
using GradingSystem.View.Admin.Dialogs;
using GradingSystem.ViewModel;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;

namespace GradingSystem.View.Admin
{
    public partial class ManageSubjects : UserControl
    {
        private Subject _selectedSubject;  // Declare the selectedSubject here
        private readonly SubjectViewModel _subjectViewModel;
        public ApplicationDbContext _context;

        public ManageSubjects(ApplicationDbContext context)
        {
            InitializeComponent();
            _context = context;
            _subjectViewModel = new SubjectViewModel(context);
            DataContext = _subjectViewModel;
        }

        private void AddSubject(object sender, RoutedEventArgs e)
        {
            var programViewModel = new ProgramViewModel();
            var addSubject = new AddSubject(_context, programViewModel);
            addSubject.Show();
        }

        private async void DeleteSubjectBtn(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Subject subject)
            {
                await _subjectViewModel.DeleteSubjectAsync(subject);
            }
        }

        private void EditSubjectBtn_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected subject
            var selectedSubject = (Subject)subjectsDataGrid.SelectedItem;

            if (selectedSubject != null)
            {
                // Create the view model here with the context and selected subject
                var viewModel = new SubjectViewModel(_context)
                {
                    SelectedSubject = selectedSubject
                };

                // Create and show the EditSubject window
                var editWindow = new EditSubject(selectedSubject, new ProgramViewModel())
                {
                    DataContext = viewModel  // Bind the data context to the view model
                };
                editWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select a subject to edit.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
