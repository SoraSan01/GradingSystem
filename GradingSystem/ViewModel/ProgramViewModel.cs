using GradingSystem.Data;
using GradingSystem.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace GradingSystem.ViewModel
{
    public class ProgramViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Program> _programs;
        private ObservableCollection<Program> _filteredPrograms;
        private readonly ApplicationDbContext _context;
        private bool _isLoading;

        public ProgramViewModel(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            Programs = new ObservableCollection<Program>();
            FilteredPrograms = new ObservableCollection<Program>();
            _ = LoadProgramsAsync();
        }

        public ObservableCollection<Program> Programs
        {
            get => _programs;
            set
            {
                _programs = value;
                OnPropertyChanged(nameof(Programs));
                ApplySearch();
            }
        }

        public ObservableCollection<Program> FilteredPrograms
        {
            get => _filteredPrograms;
            set
            {
                _filteredPrograms = value;
                OnPropertyChanged(nameof(FilteredPrograms));
            }
        }

        public string? SearchText { get; set; }

        private Program _selectedProgram;
        public Program SelectedProgram
        {
            get => _selectedProgram;
            set
            {
                _selectedProgram = value;
                OnPropertyChanged(nameof(SelectedProgram));
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public async Task LoadProgramsAsync()
        {
            IsLoading = true;
            try
            {
                using (var context = new ApplicationDbContext())  // Create a new DbContext instance
                {
                    var programs = await context.Programs.ToListAsync();
                    Programs = new ObservableCollection<Program>(programs);
                }
                ApplySearch();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading programs: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public void ApplySearch()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredPrograms = new ObservableCollection<Program>(Programs);
            }
            else
            {
                var lowerSearch = SearchText.ToLower();
                var filtered = Programs
                    .Where(p => (p.ProgramName ?? string.Empty).ToLower().Contains(lowerSearch) ||
                                (p.ProgramId ?? string.Empty).ToLower().Contains(lowerSearch) ||
                                (p.Major ?? string.Empty).ToLower().Contains(lowerSearch) ||
                                (p.Description ?? string.Empty).ToLower().Contains(lowerSearch))
                    .ToList();
                FilteredPrograms = new ObservableCollection<Program>(filtered);
            }
        }

        public async Task AddProgramAsync(Program newProgram)
        {
            if (newProgram == null) throw new ArgumentNullException(nameof(newProgram));

            try
            {
                _context.Programs.Add(newProgram);
                await _context.SaveChangesAsync();

                Programs.Add(newProgram);
                ApplySearch();

                MessageBox.Show("Program added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding program: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async Task EditProgramAsync(Program program)
        {
            if (program == null) throw new ArgumentNullException(nameof(program));

            try
            {
                var programToUpdate = await _context.Programs.FindAsync(program.ProgramId);

                if (programToUpdate == null)
                {
                    MessageBox.Show("The program was not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                programToUpdate.ProgramName = program.ProgramName;
                programToUpdate.Description = program.Description;
                programToUpdate.Major = program.Major;
                await _context.SaveChangesAsync();

                var index = Programs.IndexOf(SelectedProgram);
                if (index >= 0)
                {
                    Programs[index] = programToUpdate;  // Update the existing item in the collection
                }

                MessageBox.Show("Program updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Refresh the program list after editing
                await LoadProgramsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while editing the program: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async Task DeleteProgramAsync(Program program)
        {
            if (program == null)
            {
                MessageBox.Show("Program cannot be null.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var confirmResult = MessageBox.Show(
                    $"Are you sure you want to delete the program '{program.ProgramName}'?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (confirmResult == MessageBoxResult.Yes)
                {
                    var programToDelete = await _context.Programs.FindAsync(program.ProgramId);

                    if (programToDelete == null)
                    {
                        MessageBox.Show("The program no longer exists in the database.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    _context.Programs.Remove(programToDelete);
                    await _context.SaveChangesAsync();

                    Programs.Remove(program);
                    ApplySearch();

                    MessageBox.Show("Program deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting program: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async Task RefreshPrograms()
        {
            await LoadProgramsAsync();
        }
    }
}
