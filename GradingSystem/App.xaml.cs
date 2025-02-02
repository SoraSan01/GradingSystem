using GradingSystem.Data;
using GradingSystem.View;
using GradingSystem.View.Admin;
using GradingSystem.View.Encoder;
using GradingSystem.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using System.Windows;
using GradingSystem.View.Admin.Dialogs;
using System.Configuration;

namespace GradingSystem
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Set up the service collection and configuration
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            // Build the service provider
            ServiceProvider = serviceCollection.BuildServiceProvider();

            // Create and show the LoginWindow with dependency injection
            var loginWindow = ServiceProvider.GetRequiredService<Login>();
            loginWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Register configuration from appsettings.json
            services.AddSingleton<IConfiguration>(provider =>
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();
                return configuration;
            });

            // Register ApplicationDbContext with SQL Server connection string from configuration
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                var connectionString = ServiceProvider.GetRequiredService<IConfiguration>()
                    .GetConnectionString("MyDbConnectionString");
                options.UseSqlServer(connectionString);
            }, ServiceLifetime.Scoped);

            // Register Views
            RegisterViews(services);

            // Register ViewModels
            RegisterViewModels(services);

            // Register other services (e.g., business logic, etc.)
            RegisterOtherServices(services);
        }

        private void RegisterViews(IServiceCollection services)
        {
            services.AddTransient<Login>();
            services.AddTransient<MainWindow>();
            services.AddTransient<ManageUser>();
            services.AddTransient<ManageSubjects>();
            services.AddTransient<ManageStudents>();
            services.AddTransient<ManageGrades>();
            services.AddTransient<ManageEnrollment>();
            services.AddTransient<ManageCourse>();
            services.AddTransient<View.Dashboard>();
            services.AddTransient<ChangePassword>();
            services.AddTransient<ForgotPassword>();
            services.AddTransient<EncoderMainWindow>();
            services.AddTransient<Grades>();
            services.AddTransient<View.Encoder.Dashboard>();
            services.AddTransient<EditStudent>();
            services.AddTransient<EditSubject>();
            services.AddTransient<ShowGrade>();
            services.AddTransient<EditUser>();
            services.AddTransient<EditProgram>();
            services.AddTransient<AddProgram>();
            services.AddTransient<AddStudent>();
            services.AddTransient<AddSubject>();
            services.AddTransient<AddUser>();
            services.AddTransient<EnrollStudent>();
        }

        private void RegisterViewModels(IServiceCollection services)
        {
            services.AddTransient<DashboardViewModel>();
            services.AddTransient<EnrollmentViewModel>();
            services.AddTransient<ForgotPasswordViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<GradeViewModel>();
            services.AddTransient<ProgramViewModel>();
            services.AddTransient<StudentsViewModel>();
            services.AddTransient<SubjectViewModel>();
            services.AddTransient<UserViewModel>();
            services.AddTransient<StudentSubjectViewModel>();
        }

        private void RegisterOtherServices(IServiceCollection services)
        {
            // Register any other application services or business logic here
            // For example, if you have services for handling file I/O, external APIs, etc.
        }
    }
}
