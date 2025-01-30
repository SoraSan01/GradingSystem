﻿using GradingSystem.Data;
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

namespace GradingSystem.View.Admin
{
    /// <summary>
    /// Interaction logic for AddProgram.xaml
    /// </summary>
    public partial class AddProgram : Window
    {
        public ProgramViewModel ViewModel { get; set; }

        public event Action ProgramAdded;

        public AddProgram(ProgramViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DataContext = ViewModel;
        }

        private void addProgramBtn(object sender, RoutedEventArgs e)
        {
            if (ViewModel == null)
            {
                MessageBox.Show("ViewModel is not initialized.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(ProgramIdTxt.Text?.Trim()) ||
                string.IsNullOrWhiteSpace(ProgramNameTxt.Text?.Trim()) ||
                string.IsNullOrWhiteSpace(DescriptionTxt.Text?.Trim()) ||
                string.IsNullOrWhiteSpace(MajorTxt.Text?.Trim()))
            {
                MessageBox.Show("Please fill in all fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var newProgram = new Program
                    {
                        ProgramId = ProgramIdTxt.Text.Trim(),
                        ProgramName = ProgramNameTxt.Text.Trim(),
                        Description = DescriptionTxt.Text.Trim(),
                        Major = MajorTxt.Text.Trim(),
                    };

                    ViewModel.AddProgramAsync(newProgram);

                    ProgramAdded?.Invoke();
                    clear();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed && e.OriginalSource is not TextBox && e.OriginalSource is not Button)
            {
                this.DragMove();
            }
        }

        private void clear()
        {
            ProgramNameTxt.Clear();
            ProgramIdTxt.Clear();
            DescriptionTxt.Clear();
            MajorTxt.Clear();
        }

        private void Minimize(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CancelBtn(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
