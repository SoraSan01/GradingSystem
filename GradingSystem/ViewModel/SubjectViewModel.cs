﻿using GradingSystem.Command;
using GradingSystem.Data;
using GradingSystem.Model;
using GradingSystem.View.Admin.Dialogs;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GradingSystem.ViewModel
{
    public class SubjectViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Subject> _subjects = new();
        private ObservableCollection<Subject> _filteredSubjects = new();

        private readonly ApplicationDbContext _context;

        public SubjectViewModel(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _ = LoadSubjectsAsync();
        }

        public ObservableCollection<Subject> Subjects
        {
            get => _subjects;
            private set
            {
                _subjects = value;
                OnPropertyChanged(nameof(Subjects));
                ApplySearch();
            }
        }

        public ObservableCollection<Subject> FilteredSubjects
        {
            get => _filteredSubjects;
            private set
            {
                _filteredSubjects = value;
                OnPropertyChanged(nameof(FilteredSubjects));
            }
        }

        public string? SearchText { get; set; }

        private Subject? _SelectedSubject;
        public Subject? SelectedSubject
        {
            get => _SelectedSubject;
            set
            {
                if (_SelectedSubject != value)
                {
                    _SelectedSubject = value;
                    OnPropertyChanged(nameof(SelectedSubject));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public async Task LoadSubjectsAsync()
        {
            try
            {
                using var context = new ApplicationDbContext(new DbContextOptions<ApplicationDbContext>());
                {
                    var subjects = await context.Subjects
                        .AsNoTracking()
                        .Include(s => s.Program) // Include the related Program entity
                        .ToListAsync();

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Subjects = new ObservableCollection<Subject>(subjects);
                    });
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error loading subjects: {ex.Message}", "Error", MessageBoxImage.Error);
            }
        }


        public void ApplySearch()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredSubjects = new ObservableCollection<Subject>(Subjects);
            }
            else
            {
                var lowerSearch = SearchText.ToLower();
                var filtered = Subjects
                    .Where(s => (s.SubjectName ?? string.Empty).ToLower().Contains(lowerSearch) ||
                                (s.SubjectId ?? string.Empty).ToLower().Contains(lowerSearch) ||
                                (s.CourseCode ?? string.Empty).ToLower().Contains(lowerSearch) ||
                                (s.ProfessorName ?? string.Empty).ToLower().Contains(lowerSearch) ||
                                (s.Semester ?? string.Empty).ToLower().Contains(lowerSearch) ||
                                (s.ProgramName ?? string.Empty).ToLower().Contains(lowerSearch) ||
                                (s.Schedule ?? string.Empty).ToLower().Contains(lowerSearch))
                    .ToList();
                FilteredSubjects = new ObservableCollection<Subject>(filtered);
            }
        }

        public async Task<bool> AddSubjectAsync(Subject newSubject)
        {
            try
            {
                // Check if subject name is empty
                if (string.IsNullOrWhiteSpace(newSubject.SubjectName))
                {
                    ShowMessage("Subject name cannot be empty.", "Validation Error", MessageBoxImage.Warning);
                    return false;
                }

                using (var context = new ApplicationDbContext(new DbContextOptions<ApplicationDbContext>()))
                {
                    // Check if the subject already exists in the same program, year level, and semester
                    bool subjectExists = await context.Subjects
                        .AnyAsync(s =>
                            s.SubjectName == newSubject.SubjectName &&
                            s.ProgramId == newSubject.ProgramId &&
                            s.YearLevel == newSubject.YearLevel &&
                            s.Semester == newSubject.Semester);

                    if (subjectExists)
                    {
                        ShowMessage("A subject with the same name already exists in this program for the same year and semester.", "Validation Error", MessageBoxImage.Warning);
                        return false;
                    }

                    // Validate ProgramId - Ensure the program exists in the database
                    bool programExists = await context.Programs
                        .AnyAsync(p => p.ProgramId == newSubject.ProgramId);

                    if (!programExists)
                    {
                        ShowMessage("Invalid Program ID. Please select a valid program.", "Validation Error", MessageBoxImage.Warning);
                        return false;
                    }

                    // Now generate a unique SubjectId after successful validation
                    var existingIds = await context.Subjects.Select(s => s.SubjectId).ToListAsync();
                    newSubject.SubjectId = ApplicationDbContext.GenerateSubjectId(newSubject.SubjectName, existingIds);

                    // Add the new subject to the context
                    context.Subjects.Add(newSubject);
                    await context.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                ShowMessage($"Error adding subject: {ex.Message}", "Error", MessageBoxImage.Error);
                return false;
            }
        }

        public async Task DeleteSubjectAsync(Subject subject)
        {
            if (subject == null)
            {
                ShowMessage("Subject cannot be null.", "Error", MessageBoxImage.Error);
                return;
            }

            try
            {
                var confirmResult = MessageBox.Show(
                    $"Are you sure you want to delete the subject '{subject.SubjectName}'?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (confirmResult == MessageBoxResult.Yes)
                {
                    using (var context = new ApplicationDbContext(new DbContextOptions<ApplicationDbContext>()))
                    {
                        var subjectToDelete = await context.Subjects.FindAsync(subject.SubjectId);

                        if (subjectToDelete == null)
                        {
                            ShowMessage("The subject no longer exists in the database.", "Error", MessageBoxImage.Warning);
                            return;
                        }

                        context.Subjects.Remove(subjectToDelete);
                        await context.SaveChangesAsync();
                    }

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Subjects.Remove(subject);
                        ApplySearch();
                    });

                    ShowMessage("Subject deleted successfully.", "Success", MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Error deleting subject: {ex.Message}", "Error", MessageBoxImage.Error);
            }
        }

        public async Task EditSubjectAsync(Subject updatedSubject)
        {
            try
            {
                using (var context = new ApplicationDbContext(new DbContextOptions<ApplicationDbContext>()))
                {
                    var subjectToEdit = await context.Subjects.FindAsync(updatedSubject.SubjectId);

                    if (subjectToEdit == null)
                    {
                        ShowMessage("The subject no longer exists in the database.", "Error", MessageBoxImage.Warning);
                        return;
                    }

                    subjectToEdit.CourseCode = updatedSubject.CourseCode;
                    subjectToEdit.SubjectName = updatedSubject.SubjectName;
                    subjectToEdit.Units = updatedSubject.Units;
                    subjectToEdit.ProgramId = updatedSubject.ProgramId;
                    subjectToEdit.YearLevel = updatedSubject.YearLevel;
                    subjectToEdit.Semester = updatedSubject.Semester;
                    subjectToEdit.Schedule = updatedSubject.Schedule;
                    subjectToEdit.ProfessorName = updatedSubject.ProfessorName;

                    await context.SaveChangesAsync();
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    var index = Subjects.IndexOf(updatedSubject);
                    if (index >= 0)
                    {
                        Subjects[index] = updatedSubject;
                    }

                    ApplySearch();
                });

                ShowMessage("Subject edited successfully.", "Success", MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                ShowMessage($"Error editing subject: {ex.Message}", "Error", MessageBoxImage.Error);
            }
        }

        private void ShowMessage(string message, string caption, MessageBoxImage icon) =>
            Application.Current.Dispatcher.Invoke(() =>
                MessageBox.Show(message, caption, MessageBoxButton.OK, icon));
    }
}
