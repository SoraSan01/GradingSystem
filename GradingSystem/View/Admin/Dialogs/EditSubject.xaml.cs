﻿using GradingSystem.Model;
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

namespace GradingSystem.View.Admin.Dialogs
{
    /// <summary>
    /// Interaction logic for EditSubject.xaml
    /// </summary>
    public partial class EditSubject : Window
    {
        public Subject SelectedSubject { get; set; }
        public ProgramViewModel ProgramViewModel { get; set; }
        public EditSubject(Subject subject, ProgramViewModel programViewModel)
        {
            InitializeComponent();
            SelectedSubject = subject;
            this.DataContext = SelectedSubject; // Binding to Subject

            // Initialize ProgramViewModel if not passed in the constructor
            ProgramViewModel = programViewModel ?? new ProgramViewModel();

            // Bind Programs to the ComboBox
            ProgramCmb.ItemsSource = ProgramViewModel.Programs;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove(); // Enables dragging
            }
        }

        private void SaveBtn(object sender, RoutedEventArgs e)
        {
            DialogResult = true; // Indicates success
            Close();
        }

        private void CancelBtn(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Minimize(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }
    }
}
