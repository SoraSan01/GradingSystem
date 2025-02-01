using System.Windows;
using System.Windows.Input;

namespace GradingSystem.View.Admin.Dialogs
{
    public partial class EnrollStudent : Window
    {
        public EnrollStudent()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Minimize(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CancelBtn(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void EnrollStudentBtn(object sender, RoutedEventArgs e)
        {
            // Implement the logic to enroll the student here
            // For example, you can validate the input fields and save the data to the database

            MessageBox.Show("Student enrolled successfully!");
            this.Close();
        }

        private void IdTxt_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Ensure that only numeric input is allowed for the Student ID field
            e.Handled = !IsTextNumeric(e.Text);
        }

        private static bool IsTextNumeric(string text)
        {
            foreach (char c in text)
            {
                if (!char.IsDigit(c))
                    return false;
            }
            return true;
        }
    }
}
