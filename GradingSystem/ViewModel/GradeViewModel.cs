using GradingSystem.Data;
using GradingSystem.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace GradingSystem.ViewModel
{
    public class GradeViewModel
    {
        private readonly ApplicationDbContext _context;
        public ObservableCollection<Grade> Grades { get; set; }

        public GradeViewModel(ApplicationDbContext context)
        {
            _context = context;
            Grades = new ObservableCollection<Grade>();
        }

        // Asynchronous method to load grades
        public async Task LoadGradeAsync()
        {
            try
            {
                var gradeList = await _context.Grades.ToListAsync();
                Grades.Clear();
                foreach (var grade in gradeList)
                {
                    Grades.Add(grade);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading grades: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
