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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GradingSystem.View.Admin
{
    /// <summary>
    /// Interaction logic for ManageGrades.xaml
    /// </summary>
    public partial class ManageGrades : UserControl
    {
        public StudentsViewModel ViewModel { get; set; }

        public ManageGrades()
        {
            InitializeComponent();
            // Initialize the ViewModel
            ViewModel = new StudentsViewModel();

            // Set the DataContext for binding, if required
            DataContext = ViewModel;
        }
    }
}
