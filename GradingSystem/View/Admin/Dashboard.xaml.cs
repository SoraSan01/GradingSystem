using GradingSystem.Data;
using GradingSystem.ViewModel;
using System.Windows.Controls;

namespace GradingSystem.View
{
    public partial class Dashboard : UserControl
    {

        public Dashboard(ApplicationDbContext context)
        {
            InitializeComponent();
            DataContext = new DashboardViewModel(context);
        }
    }
}
