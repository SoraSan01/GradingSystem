using GradingSystem.Data;
using GradingSystem.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace GradingSystem.ViewModel
{
    public class ProgramViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Program> _programs;
        public ObservableCollection<Program> Programs
        {
            get => _programs;
            set
            {
                _programs = value;
                OnPropertyChanged();
            }
        }

        private Program _selectedProgram;
        public Program SelectedProgram
        {
            get => _selectedProgram;
            set
            {
                _selectedProgram = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ProgramViewModel()
        {
            Programs = new ObservableCollection<Program>();
            _ = LoadProgramsAsync(); // Initial load of programs
        }

        // Load programs asynchronously from the database
        public async Task LoadProgramsAsync()
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var programList = await Task.Run(() => context.Programs.ToList());
                    Programs.Clear();
                    foreach (var program in programList)
                    {
                        Programs.Add(program);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        // Add a new program
        public void AddProgram(Program newProgram)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var existingProgram = context.Programs
                        .FirstOrDefault(c => c.ProgramName == newProgram.ProgramName && c.Description == newProgram.Description);

                    if (existingProgram != null)
                    {
                        MessageBox.Show("A program with the same name already exists.", "Duplicate Program", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    context.Programs.Add(newProgram);
                    context.SaveChanges();

                    Programs.Add(newProgram); // Directly update the ObservableCollection
                    MessageBox.Show("Program added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Refresh the program list after adding
                    _ = LoadProgramsAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while adding the program: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Edit an existing program
        public void EditProgram(Program program)
        {
            try
            {
                using (var context = new ApplicationDbContext())
                {
                    var programToUpdate = context.Programs.Find(program.ProgramId);

                    if (programToUpdate == null)
                    {
                        MessageBox.Show("The program was not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    programToUpdate.ProgramName = program.ProgramName;
                    programToUpdate.Description = program.Description;
                    programToUpdate.Major = program.Major;
                    context.SaveChanges();

                    var index = Programs.IndexOf(SelectedProgram);
                    if (index >= 0)
                    {
                        Programs[index] = programToUpdate;  // Update the existing item in the collection
                    }

                    MessageBox.Show("Program updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Refresh the program list after editing
                    _ = LoadProgramsAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while editing the program: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Delete a program
        public void DeleteProgram(Program program)
        {
            if (MessageBox.Show($"Are you sure you want to delete the program '{program.ProgramName}'?",
                                "Confirm Delete",
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new ApplicationDbContext())
                    {
                        var programToDelete = context.Programs.Find(program.ProgramId);

                        if (programToDelete == null)
                        {
                            MessageBox.Show("The program was not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        context.Programs.Remove(programToDelete);
                        context.SaveChanges();

                        Programs.Remove(program); // Directly update the ObservableCollection
                        MessageBox.Show("Program deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Refresh the program list after deletion
                        _ = LoadProgramsAsync();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while deleting the program: {ex.Message}");
                }
            }
        }
    }
}
