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

                    // Iterate through all worksheets in the Excel package
                    foreach (var worksheet in package.Workbook.Worksheets)
                    {
                        if (worksheet.Dimension == null) continue;

                        int rowCount = worksheet.Dimension.Rows;

                        Program program = null;
                        string currentYear = "";
                        string currentSemester = "";

                        for (int row = 1; row <= rowCount; row++)
                        {
                            string cellValue = worksheet.Cells[row, 1].Text?.Trim();

                            // Extract Program Name
                            if (IsProgramName(cellValue))
                            {
                                program = await GetProgramByNameAsync(cellValue);
                                currentYear = ""; // Reset year/semester after program
                                currentSemester = "";
                                continue;
                            }

                            // Extract Year Level
                            if (IsYearLevel(cellValue))
                            {
                                currentYear = cellValue;
                                continue;
                            }

                            // Extract Semester
                            if (IsSemester(cellValue))
                            {
                                currentSemester = cellValue;
                                continue;
                            }

                            // Parse Course Data if Year, Semester, and Program are set
                            if (!string.IsNullOrWhiteSpace(currentYear) && !string.IsNullOrWhiteSpace(currentSemester) && program != null)
                            {
                                ParseCourseData(worksheet, row, program, currentYear, currentSemester);
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

    private bool IsProgramName(string cellValue)
    {
        return !string.IsNullOrEmpty(cellValue) &&
               (cellValue.Contains("BACHELOR OF SCIENCE", StringComparison.OrdinalIgnoreCase) ||
                cellValue.Contains("BACHELOR OF SECONDARY EDUCATION", StringComparison.OrdinalIgnoreCase));
    }

    private bool IsYearLevel(string cellValue)
    {
        return !string.IsNullOrEmpty(cellValue) &&
               (cellValue.Contains("First Year", StringComparison.OrdinalIgnoreCase) ||
                cellValue.Contains("Second Year", StringComparison.OrdinalIgnoreCase) ||
                cellValue.Contains("Third Year", StringComparison.OrdinalIgnoreCase) ||
                cellValue.Contains("Fourth Year", StringComparison.OrdinalIgnoreCase));
    }

    private bool IsSemester(string cellValue)
    {
        return !string.IsNullOrEmpty(cellValue) &&
               (cellValue.Contains("First Semester", StringComparison.OrdinalIgnoreCase) ||
                cellValue.Contains("Second Semester", StringComparison.OrdinalIgnoreCase));
    }

    private async Task<Program> GetProgramByNameAsync(string programName)
    {
        return await _context.Programs
            .FirstOrDefaultAsync(p => EF.Functions.Like(p.ProgramName, programName));
    }

    private void ParseCourseData(ExcelWorksheet worksheet, int row, Program program, string yearLevel, string semester)
    {
        string courseCode1 = worksheet.Cells[row, 1].Text?.Trim();
        string descriptiveTitle1 = worksheet.Cells[row, 2].Text?.Trim();
        string unitsText1 = worksheet.Cells[row, 4].Text?.Trim();

        string courseCode2 = worksheet.Cells[row, 5].Text?.Trim();
        string descriptiveTitle2 = worksheet.Cells[row, 6].Text?.Trim();
        string unitsText2 = worksheet.Cells[row, 7].Text?.Trim();

        // Process first course
        AddCourse(courseCode1, descriptiveTitle1, unitsText1, program, yearLevel, semester, "First Semester");

        // Process second course
        AddCourse(courseCode2, descriptiveTitle2, unitsText2, program, yearLevel, semester, "Second Semester");
    }

    private void AddCourse(string courseCode, string descriptiveTitle, string unitsText, Program program, string yearLevel, string semester, string semesterType)
    {
        if (!string.IsNullOrWhiteSpace(courseCode) && !string.IsNullOrWhiteSpace(descriptiveTitle) && !courseCode.Contains("Course Code", StringComparison.OrdinalIgnoreCase))
        {
            int.TryParse(unitsText?.Replace("(", "").Replace(")", ""), out int units);

            if (!Courses.Any(c => c.CourseCode == courseCode && c.YearLevel == yearLevel && c.Semester == semesterType))
            {
                Courses.Add(new Subject
                {
                    SubjectId = Subject.GenerateSubjectId(descriptiveTitle, Courses.Select(c => c.SubjectId).ToList()),
                    CourseCode = courseCode,
                    SubjectName = descriptiveTitle,
                    Units = units,
                    Program = program,
                    YearLevel = yearLevel,
                    Semester = semesterType
                });
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