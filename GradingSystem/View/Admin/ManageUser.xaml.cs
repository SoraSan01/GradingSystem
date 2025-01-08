﻿using GradingSystem.Data;
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
    /// Interaction logic for ManageUser.xaml
    /// </summary>
    public partial class ManageUser : UserControl
    {
        private readonly ApplicationDbContext _context;
        public ManageUser(ApplicationDbContext context)
        {
            InitializeComponent();
            _context = context;

            DataContext = new UserViewModel(_context);
        }
    }
}
