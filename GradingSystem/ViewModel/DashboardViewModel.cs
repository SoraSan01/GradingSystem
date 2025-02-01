using GradingSystem.Data;
using GradingSystem.Model;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace GradingSystem.ViewModel
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        private readonly ApplicationDbContext _context;
        private SeriesCollection _chartValues;
        private List<string> _timeLabels;

        public DashboardViewModel(ApplicationDbContext context)
        {
            _context = context;
            LoadChartData();
        }

        public SeriesCollection ChartValues
        {
            get => _chartValues;
            set
            {
                _chartValues = value;
                OnPropertyChanged();
            }
        }

        public List<string> TimeLabels
        {
            get => _timeLabels;
            set
            {
                _timeLabels = value;
                OnPropertyChanged();
            }
        }

        private async void LoadChartData()
        {
            // Fetch student data from the database
            var students = await _context.Students.ToListAsync();

            // Group students by month of creation
            var studentGroups = students.GroupBy(s => s.CreatedAt.ToString("MMM"))
                                        .OrderBy(g => g.Key)
                                        .ToList();

            // Prepare chart values and labels
            var values = new ChartValues<int>();
            var labels = new List<string>();

            foreach (var group in studentGroups)
            {
                values.Add(group.Count());
                labels.Add(group.Key);
            }

            ChartValues = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Students",
                    Values = values
                }
            };

            TimeLabels = labels;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
