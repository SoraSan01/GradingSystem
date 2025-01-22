using GradingSystem.Data;
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

namespace GradingSystem.View.Admin.Dialogs
{
    /// <summary>
    /// Interaction logic for AddSubject.xaml
    /// </summary>
    public partial class AddSubject : Window
    {
        public SubjectViewModel _subject { get; set; }
        public ProgramViewModel Program { get; set; }

        public event Action StudentAdded;
        private readonly ApplicationDbContext _context;
        public AddSubject(ApplicationDbContext context, ProgramViewModel Program)
        {
            InitializeComponent();
            _context = context;
            ProgramCmb.ItemsSource = Program.Programs;
            DataContext = _subject;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed && e.OriginalSource is not TextBox && e.OriginalSource is not Button)
            {
                this.DragMove();
            }
        }

        private void Minimize(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to exit?", "Close", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void CancelBtn(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveBtn(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(SubjectIdText.Text) ||
                    string.IsNullOrWhiteSpace(CourseCodeTxt.Text) ||
                    string.IsNullOrWhiteSpace(TitleTxt.Text) ||
                    string.IsNullOrWhiteSpace(UnitTxt.Text) ||
                    ProgramCmb.SelectedValue == null ||
                    yearCmb.SelectedValue == null ||
                    SemesterCmb.SelectedValue == null ||
                    string.IsNullOrWhiteSpace(ScheduleTxt.Text) ||
                    string.IsNullOrWhiteSpace(ProfessorTxt.Text))
                {
                    MessageBox.Show("Please fill in all fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Ensure Units is a valid integer
                if (!int.TryParse(UnitTxt.Text.Trim(), out int units))
                {
                    MessageBox.Show("Units must be a valid number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Create new subject
                Subject newSubject = CreateStudent();

                // Save to database
                using (var context = new ApplicationDbContext())
                {
                    context.Subjects.Add(newSubject);
                    context.SaveChanges();
                }

                // Notify user
                MessageBox.Show("Subject added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Trigger event
                StudentAdded?.Invoke();

                // Close the window
                this.Close();
            }
            catch (Exception ex)
            {
                // Handle errors
                MessageBox.Show($"An error occurred while saving the subject: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private Subject CreateStudent()
        {
            return new Subject
            {
                SubjectId = SubjectIdText.Text.Trim(),
                CourseCode = CourseCodeTxt.Text.Trim(),
                SubjectName = TitleTxt.Text.Trim(),
                Units = int.Parse(UnitTxt.Text.Trim()),
                ProgramId = ProgramCmb.SelectedValue?.ToString(),
                YearLevel = yearCmb.SelectedValue?.ToString(),
                Semester = SemesterCmb.SelectedValue?.ToString(),
                Schedule = ScheduleTxt.Text.Trim(),
                ProfessorName = ProfessorTxt.Text.Trim(),
            };
        }

        private void BulkInsertBtn(object sender, RoutedEventArgs e)
        {
            var BulkInsert = new BulkInsertCourse(_context);
            BulkInsert.Show();
        }
    }
}
