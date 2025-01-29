using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using OfficeOpenXml; // Ensure you have the EPPlus package installed
using System.Linq;
using GradingSystem.Model;
using Microsoft.Win32;
using System.Windows;
using GradingSystem.Command;
using GradingSystem.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

public class BulkInsertCourseViewModel : INotifyPropertyChanged
{
    public ICommand SaveCommand { get; set; }
    private readonly ApplicationDbContext _context;
    private readonly StudentSubjectViewModel _studentSubjectViewModel;
    public ObservableCollection<Subject> Subject { get; set; }

    public BulkInsertCourseViewModel(ApplicationDbContext context)
    {
        _context = context;
        _studentSubjectViewModel = new StudentSubjectViewModel(context);
        Subject = new ObservableCollection<Subject>();
        SaveCommand = new RelayCommand(SaveData);
    }

    public async Task ChooseFileAsync()
    {
        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Title = "Select an Excel file.",
            Filter = "Excel Files (.xlsx;.xls)|*.xlsx;*.xls"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            string filepath = openFileDialog.FileName;

            try
            {
                using (var package = new ExcelPackage(new FileInfo(filepath)))
                {
                    Subject.Clear();

                    // Load programs from the database and create a dictionary to map program names to program IDs
                    var programs = await _context.Programs.ToListAsync();
                    var programDictionary = programs.ToDictionary(p => p.ProgramName, p => p.ProgramId);

                    foreach (var worksheet in package.Workbook.Worksheets)
                    {
                        int rowCount = worksheet.Dimension.Rows;

                        string currentYear = "";
                        string currentSemester = "";
                        string programName = "";

                        for (int row = 1; row <= rowCount; row++)
                        {
                            var cellValue = worksheet.Cells[row, 1].Text?.Trim();

                            // Check for program name
                            if (cellValue?.Contains("BACHELOR OF SCIENCE IN", StringComparison.OrdinalIgnoreCase) == true)
                            {
                                programName = cellValue;
                                continue;
                            }

                            // Check for year level
                            if (cellValue?.ToLowerInvariant().Contains("first year") == true ||
                                cellValue?.ToLowerInvariant().Contains("second year") == true ||
                                cellValue?.ToLowerInvariant().Contains("third year") == true ||
                                cellValue?.ToLowerInvariant().Contains("fourth year") == true)
                            {
                                currentYear = cellValue;
                                continue;
                            }

                            // Check for semester
                            if (cellValue?.Contains("First Semester", StringComparison.OrdinalIgnoreCase) == true ||
                                cellValue?.Contains("Second Semester", StringComparison.OrdinalIgnoreCase) == true)
                            {
                                currentSemester = cellValue;
                                continue;
                            }

                            // Extract course details
                            if (!string.IsNullOrWhiteSpace(currentYear) && !string.IsNullOrWhiteSpace(currentSemester) && !string.IsNullOrWhiteSpace(programName))
                            {
                                var courseCode = worksheet.Cells[row, 1].Text?.Trim();
                                var descriptiveTitle = worksheet.Cells[row, 2].Text?.Trim();
                                var unitsText = worksheet.Cells[row, 4].Text?.Trim();

                                // Validate course code and descriptive title
                                if (string.IsNullOrWhiteSpace(courseCode) || string.IsNullOrWhiteSpace(descriptiveTitle))
                                    continue;

                                // Skip header rows
                                if (courseCode.Contains("Course Code") || descriptiveTitle.Contains("Descriptive Title") || descriptiveTitle.Contains("UNITS"))
                                    continue;

                                // Parse units
                                int.TryParse(unitsText, out int units);

                                // Generate a unique subject ID
                                var existingIds = Subject.Select(s => s.SubjectId).ToList();
                                var subjectId = GenerateSubjectId(courseCode, existingIds);

                                // Map program name to program ID
                                if (!programDictionary.TryGetValue(programName, out var programId))
                                {
                                    MessageBox.Show($"Program name '{programName}' does not exist in the database. Please ensure all program names are valid.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                    return;
                                }

                                // Add the subject to the list
                                Subject.Add(new Subject
                                {
                                    SubjectId = subjectId,
                                    CourseCode = courseCode,
                                    SubjectName = descriptiveTitle,
                                    Units = units,
                                    ProgramId = programId,
                                    YearLevel = currentYear,
                                    Semester = currentSemester
                                });
                            }
                        }
                    }
                }

                MessageBox.Show("Data extraction completed successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while processing the file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
    private async void SaveCoursesToDatabase()
    {
        try
        {
            foreach (var course in Subject)
            {
                var existingCourse = await _context.Subjects
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.SubjectId == course.SubjectId);

                if (existingCourse != null)
                {
                    _context.Entry(existingCourse).State = EntityState.Detached;
                    _context.Entry(course).State = EntityState.Modified;
                }
                else
                {
                    _context.Subjects.Add(course);
                }
            }

            await _context.SaveChangesAsync();
            MessageBox.Show("Courses saved successfully!");
        }
        catch (DbUpdateException ex)
        {
            string errorMessage = ex.InnerException?.Message ?? ex.Message;
            MessageBox.Show($"An error occurred while saving the courses: {errorMessage}");
            Console.WriteLine(ex.StackTrace);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An unexpected error occurred: {ex.Message}");
        }
    }

    private void SaveData(object parameter)
    {
        SaveCoursesToDatabase();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private string GenerateSubjectId(string courseCode, List<string> existingIds)
    {
        string prefix = courseCode.Length >= 3 ? courseCode.Substring(0, 3).ToUpper() : courseCode.ToUpper().PadRight(3, 'X');
        var matchingIds = existingIds
            .Where(id => id.StartsWith(prefix + "-"))
            .Select(id => id.Substring(prefix.Length + 1))
            .Where(num => int.TryParse(num, out _))
            .Select(int.Parse);

        int nextNumber = matchingIds.Any() ? matchingIds.Max() + 1 : 1;
        return $"{prefix}-{nextNumber:000}";
    }
}

