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
            var AddSubject = new AddSubject(_context, programViewModel);
            AddSubject.Show();
        }

        private async void DeleteSubjectBtn(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Subject subject)
            {
                await _subjectViewModel.DeleteSubjectAsync(subject);
            }
        }
    }
}
