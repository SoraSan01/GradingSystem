using GradingSystem.Data;
using GradingSystem.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;

namespace GradingSystem
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Create and show the LoginWindow first
            Login loginWindow = new Login();
            loginWindow.Show();
        }
    }
}

