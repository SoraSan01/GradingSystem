﻿using GradingSystem.Data;
using GradingSystem.Model;
using GradingSystem.View;
using GradingSystem.View.Admin;
using GradingSystem.View.Admin.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using System.Windows.Input;

namespace GradingSystem
{
    public partial class MainWindow : Window
    {
        private readonly ApplicationDbContext _context;

        // Lazy-loaded views to avoid unnecessary instantiations
        private Dashboard _dashboard;
        private ManageStudents? _manageStudents;
        private ManageGrades? _manageGrades;
        private ManageUser? _manageUser;
        private ManageCourse? _manageCourse;
        private ManageSubjects? _manageSubjects;
        private ManageEnrollment? _manageEnrollment;
        private readonly IServiceProvider _serviceProvider;

        public MainWindow(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;

            // Initialize Dashboard on startup
            _dashboard = _serviceProvider.GetRequiredService<Dashboard>();
            MainContent.Content = _dashboard;
        }

        private void SwitchContent<T>(ref T? contentPage) where T : class
        {
            try
            {
                // Lazy loading for views that require dependencies
                if (contentPage == null)
                {
                    // Resolve views from DI container instead of creating them manually
                    contentPage = (T)(_serviceProvider.GetRequiredService(typeof(T)));
                }

                if (contentPage != null && MainContent.Content != contentPage)
                {
                    MainContent.Content = contentPage;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while switching to {nameof(T)}: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        // Event Handlers for Button Clicks
        private void DashboardBtn(object sender, RoutedEventArgs e) => SwitchContent(ref _dashboard);
        private void ManageStudentsBtn(object sender, RoutedEventArgs e) => SwitchContent(ref _manageStudents);
        private void ManageGradesBtn(object sender, RoutedEventArgs e) => SwitchContent(ref _manageGrades);
        private void ManageUserBtn(object sender, RoutedEventArgs e) => SwitchContent(ref _manageUser);
        private void ManageCourseBtn(object sender, RoutedEventArgs e) => SwitchContent(ref _manageCourse);
        private void ManageSubjectsBtn(object sender, RoutedEventArgs e) => SwitchContent(ref _manageSubjects);
        private void EnrollmentBtn(object sender, RoutedEventArgs e) => SwitchContent(ref _manageEnrollment);

        private void LogoutBtn(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to log out?", "Logout", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var login = _serviceProvider.GetRequiredService<Login>();
                login.Show();
                Close();
            }
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        private void Minimize(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
        private void Maximize(object sender, RoutedEventArgs e) => WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to exit?", "Exit", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }
    }
}
