using GradingSystem.Data;
using GradingSystem.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace GradingSystem.View.Admin
{
    public partial class ManageSubjects : UserControl
    {
        private readonly ApplicationDbContext _context;
        public SubjectViewModel Subjects { get; set; }

        public ManageSubjects(ApplicationDbContext context)
        {
            InitializeComponent();

            _context = context;
            DataContext = new SubjectViewModel(_context);
        }

        private void AddSubject(object sender, RoutedEventArgs e)
        {

                BulkInsertCourse bulkInsert = new BulkInsertCourse(_context);
                bulkInsert.ShowDialog();
        }
    }
}
