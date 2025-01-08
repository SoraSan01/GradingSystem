﻿using GradingSystem.Data;
using GradingSystem.Model;
using GradingSystem.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GradingSystem.View.Admin
{
    public partial class AddStudent : Window
    {
        public StudentsViewModel ViewModel { get; set; }

        public event Action StudentAdded;

        public AddStudent(StudentsViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DataContext = ViewModel;
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to exit?", "Close", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void addStudentBtn(object sender, RoutedEventArgs e)
        {
            if (ViewModel == null)
            {
                MessageBox.Show("ViewModel is not initialized.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(FnameTxt.Text?.Trim()) ||
                string.IsNullOrWhiteSpace(LnameTxt.Text?.Trim()) ||
                courseCmb.SelectedItem == null ||
                yearCmb.SelectedItem == null)
            {
                MessageBox.Show("Please fill in all fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var newStudent = new Student
                    {
                        StudentId = context.GenerateStudentId(),
                        FirstName = FnameTxt.Text.Trim(),
                        LastName = LnameTxt.Text.Trim(),
                        Course = courseCmb.SelectedValue?.ToString(),
                        YearLevel = yearCmb.SelectedValue?.ToString(),
                    };

                    ViewModel.AddStudent(newStudent);

                    StudentAdded?.Invoke();
                    clear();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void clear()
        {
            FnameTxt.Clear();
            LnameTxt.Clear();
            courseCmb.SelectedIndex = -1;
            yearCmb.SelectedIndex = -1;
            FnameTxt.Focus();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed && e.OriginalSource is not TextBox && e.OriginalSource is not Button)
            {
                this.DragMove();
            }
        }
    }
}
