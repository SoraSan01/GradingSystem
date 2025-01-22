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

public class BulkInsertCourseViewModel : INotifyPropertyChanged
{
    public ICommand SaveCommand { get; set; }
    private readonly ApplicationDbContext _context;
    public ObservableCollection<Subject> Subject { get; set; }

    public BulkInsertCourseViewModel(ApplicationDbContext context)
    {
        _context = context;
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

            using (var package = new ExcelPackage(new FileInfo(filepath)))
            {
                Subject.Clear();



                foreach (var worksheet in package.Workbook.Worksheets)
                {
                    int rowCount = worksheet.Dimension.Rows;

                    string currentYear = "";
                    string currentSemester = "";
                    string program = "";

                    // Loop through all rows
                    for (int row = 1; row <= rowCount; row++)
                    {
                        var cellValue = worksheet.Cells[row, 1].Text?.Trim();

                        // Identify Program
                        if (cellValue?.Contains("BACHELOR OF SCIENCE IN", StringComparison.OrdinalIgnoreCase) == true)
                        {
                            program = cellValue;
                            continue; // Move to the next row after identifying the program
                        }

                        // Identify Year Level
                        if (cellValue?.ToLowerInvariant().Contains("first year") == true ||
    cellValue?.ToLowerInvariant().Contains("second year") == true ||
    cellValue?.ToLowerInvariant().Contains("third year") == true ||
    cellValue?.ToLowerInvariant().Contains("fourth year") == true)
                        {
                            currentYear = cellValue;
                            continue; // Move to the next row after identifying the year level
                        }


                        // Identify Semester
                        if (cellValue?.Contains("First Semester", StringComparison.OrdinalIgnoreCase) == true ||
                            cellValue?.Contains("Second Semester", StringComparison.OrdinalIgnoreCase) == true)
                        {
                            currentSemester = cellValue;
                            continue; // Move to the next row after identifying the semester
                        }

                        // Add subject details only when all required fields are identified
                        if (!string.IsNullOrWhiteSpace(currentYear) && !string.IsNullOrWhiteSpace(currentSemester) && !string.IsNullOrWhiteSpace(program))
                        {
                            var courseCode = worksheet.Cells[row, 1].Text?.Trim();
                            var descriptiveTitle = worksheet.Cells[row, 2].Text?.Trim();
                            var unitsText = worksheet.Cells[row, 4].Text?.Trim();

                            // Skip rows with invalid course data
                            if (string.IsNullOrWhiteSpace(courseCode) || string.IsNullOrWhiteSpace(descriptiveTitle))
                                continue;

                            // Skip headers or irrelevant data
                            if (courseCode.Contains("Course Code") || descriptiveTitle.Contains("Descriptive Title") || descriptiveTitle.Contains("UNITS"))
                                continue;

                            // Parse units
                            int.TryParse(unitsText, out int units);

                            // Add subject to the ObservableCollection
                            Subject.Add(new Subject
                            {
                                CourseCode = courseCode,
                                SubjectName = descriptiveTitle,
                                Units = units,
                                ProgramId = program,
                                YearLevel = currentYear,
                                Semester = currentSemester
                            });
                        }
                    }
                }
            }

            MessageBox.Show("Data extraction completed successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private async void SaveCoursesToDatabase()
    {
        try
        {
            foreach (var course in Subject)
            {
                // Check if the subject already exists in the database (optional)
                var existingCourse = await _context.Subjects
                    .FirstOrDefaultAsync(s => s.SubjectId == course.SubjectId);

                if (existingCourse != null)
                {
                    continue; // Skip if the course already exists
                }

                // Add the new course to the DbSet
                _context.Subjects.Add(course);
            }

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Optionally, update the UI or show a message after saving
            MessageBox.Show("Courses saved successfully!");
        }
        catch (DbUpdateException ex)
        {
            // Log the full exception details
            string errorMessage = ex.InnerException?.Message ?? ex.Message;
            MessageBox.Show($"An error occurred while saving the courses: {errorMessage}");
            // Optionally log the stack trace or log the error to a file for better debugging
            Console.WriteLine(ex.StackTrace);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An unexpected error occurred: {ex.Message}");
        }
    }

    private void SaveData(object parameter)
    {
        // Call a method to save data to your database
        SaveCoursesToDatabase();
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
