using GradingSystem.Model;
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
    /// Interaction logic for AddUser.xaml
    /// </summary>
    public partial class AddUser : Window
    {
        public AddUser()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {

        }

        private void Minimize(object sender, RoutedEventArgs e)
        {

        }

        private void CancelBtn(object sender, RoutedEventArgs e)
        {

        }

        private void SaveBtn(object sender, RoutedEventArgs e)
        {

        }

        //private Subject CreateStudent()
        //{
        //    return new Subject
        //    {
        //        SubjectId = SubjectIdText.Text.Trim(),
        //        CourseCode = CourseCodeTxt.Text.Trim(),
        //        SubjectName = TitleTxt.Text.Trim(),
        //        Units = int.Parse(UnitTxt.Text.Trim()),
        //        ProgramId = ProgramCmb.SelectedValue?.ToString(),
        //        YearLevel = yearCmb.SelectedValue?.ToString(),
        //        Semester = SemesterCmb.SelectedValue?.ToString(),
        //        Schedule = ScheduleTxt.Text.Trim(),
        //        ProfessorName = ProfessorTxt.Text.Trim(),
        //    };
        //}
    }
}
