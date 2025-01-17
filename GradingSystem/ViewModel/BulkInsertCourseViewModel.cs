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

    public ObservableCollection<Subject> Courses { get; set; }

    private readonly ApplicationDbContext _context;

    public BulkInsertCourseViewModel(ApplicationDbContext context)
    {
        _context = context;
        Courses = new ObservableCollection<Subject>();
        SaveCommand = new RelayCommand(SaveData);

    }

    public async Task ChooseFileAsync()
    {
        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Title = "Select an Excel File.",
            Filter = "Excel Files (*.xlsx; *.xls)|*.xlsx;*.xls"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            string filepath = openFileDialog.FileName;

            try
            {
                using (var package = new ExcelPackage(new FileInfo(filepath)))
                {
                    Courses.Clear();

                    foreach (var worksheet in package.Workbook.Worksheets)
                    {
                        if (worksheet.Dimension == null) continue;

                        int rowCount = worksheet.Dimension.Rows;
                        int colCount = worksheet.Dimension.Columns;

                        string program = "";
                        string currentYear = "";
                        string currentSemester = "";

                        for (int row = 1; row <= rowCount; row++)
                        {
                            string cellValue = worksheet.Cells[row, 1].Text?.Trim();

                            // Extract Program Name
                            if (!string.IsNullOrEmpty(cellValue) &&
                                (cellValue.Contains("BACHELOR OF SCIENCE", StringComparison.OrdinalIgnoreCase) ||
                                 cellValue.Contains("BACHELOR OF SECONDARY EDUCATION", StringComparison.OrdinalIgnoreCase)))
                            {
                                program = cellValue;
                                continue;
                            }

                            // Extract Year Level
                            if (!string.IsNullOrEmpty(cellValue) &&
                                (cellValue.Contains("First Year", StringComparison.OrdinalIgnoreCase) ||
                                 cellValue.Contains("Second Year", StringComparison.OrdinalIgnoreCase) ||
                                 cellValue.Contains("Third Year", StringComparison.OrdinalIgnoreCase) ||
                                 cellValue.Contains("Fourth Year", StringComparison.OrdinalIgnoreCase)))
                            {
                                currentYear = cellValue;
                                continue;
                            }

                            // Extract Semester
                            if (!string.IsNullOrEmpty(cellValue) &&
                                (cellValue.Contains("First Semester", StringComparison.OrdinalIgnoreCase) ||
                                 cellValue.Contains("Second Semester", StringComparison.OrdinalIgnoreCase)))
                            {
                                currentSemester = cellValue;
                                continue;
                            }

                            // Parse Course Data
                            if (!string.IsNullOrWhiteSpace(currentYear) && !string.IsNullOrWhiteSpace(currentSemester))
                            {
                                string courseCode1 = worksheet.Cells[row, 1].Text?.Trim();
                                string descriptiveTitle1 = worksheet.Cells[row, 2].Text?.Trim();
                                string unitsText1 = worksheet.Cells[row, 4].Text?.Trim();

                                string courseCode2 = worksheet.Cells[row, 5].Text?.Trim();
                                string descriptiveTitle2 = worksheet.Cells[row, 6].Text?.Trim();
                                string unitsText2 = worksheet.Cells[row, 7].Text?.Trim();

                                // First Semester Data
                                if (!string.IsNullOrWhiteSpace(courseCode1) && !string.IsNullOrWhiteSpace(descriptiveTitle1))
                                {
                                    if (!courseCode1.Contains("Course Code", StringComparison.OrdinalIgnoreCase))
                                    {
                                        int.TryParse(unitsText1?.Replace("(", "").Replace(")", ""), out int units1);

                                        if (!Courses.Any(c => c.CourseCode == courseCode1 && c.YearLevel == currentYear && c.Semester == "First Semester"))
                                        {
                                            Courses.Add(new Subject
                                            {
                                                SubjectId = Subject.GenerateSubjectId(descriptiveTitle1, Courses.Select(c => c.SubjectId).ToList()),
                                                CourseCode = courseCode1,
                                                SubjectName = descriptiveTitle1,
                                                Units = units1,
                                                Program = program,
                                                YearLevel = currentYear,
                                                Semester = "First Semester"
                                            });
                                        }
                                    }
                                }

                                // Second Semester Data
                                if (!string.IsNullOrWhiteSpace(courseCode2) && !string.IsNullOrWhiteSpace(descriptiveTitle2))
                                {
                                    if (!courseCode2.Contains("Course Code", StringComparison.OrdinalIgnoreCase))
                                    {
                                        int.TryParse(unitsText2?.Replace("(", "").Replace(")", ""), out int units2);

                                        if (!Courses.Any(c => c.CourseCode == courseCode2 && c.YearLevel == currentYear && c.Semester == "Second Semester"))
                                        {
                                            Courses.Add(new Subject
                                            {
                                                SubjectId = Subject.GenerateSubjectId(descriptiveTitle2, Courses.Select(c => c.SubjectId).ToList()),
                                                CourseCode = courseCode2,
                                                SubjectName = descriptiveTitle2,
                                                Units = units2,
                                                Program = program,
                                                YearLevel = currentYear,
                                                Semester = "Second Semester"
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while processing the file: {ex.Message}");
            }
        }
    }

    private void SaveData(object parameter)
    {
        // Call a method to save data to your database
        SaveCoursesToDatabase();
    }
    private async void SaveCoursesToDatabase()
    {
        try
        {
            foreach (var course in Courses)
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

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}