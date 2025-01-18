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
    public class StudentsViewModel : INotifyPropertyChanged
    {
        private readonly ApplicationDbContext _context;

        public ObservableCollection<Student> Students { get; set; }
        public Student SelectedStudent { get; set; }

        public StudentsViewModel(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            Students = new ObservableCollection<Student>();
            LoadStudentsAsync();
        }

        // Load all students asynchronously with error handling
        public async Task LoadStudentsAsync()
        {
            try
            {
                var studentList = await _context.Students.ToListAsync();
                Students.Clear();
                foreach (var student in studentList)
                {
                    Students.Add(student);
                }
            }
            catch (Exception ex)
            {
                HandleError("loading students", ex);
            }
        }

        // Add a new student with async handling
        public async Task AddStudentAsync(Student newStudent, string year, string semester, string program)
        {
            if (newStudent == null) throw new ArgumentNullException(nameof(newStudent));

            try
            {
                if (await IsStudentDuplicateAsync(newStudent.StudentId))
                {
                    ShowMessage("Duplicate student found.", "Warning", MessageBoxImage.Warning);
                    return;
                }

                _context.Students.Add(newStudent);
                await _context.SaveChangesAsync();

                // Assign grade info to the student
                await AddStudentGradeInfoAsync(newStudent.StudentId, program, year, semester, newStudent.FirstName, newStudent.LastName);

                // Assign subjects to the student
                await AssignSubjectsToStudentAsync(newStudent.StudentId, year, semester, program);

                Students.Add(newStudent);
                ShowMessage("Student added successfully.", "Success", MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                HandleError("adding student", ex);
            }
        }

        // Check if the student is already registered
        private async Task<bool> IsStudentDuplicateAsync(string studentId)
        {
            return await _context.Students.AnyAsync(s => s.StudentId == studentId);
        }

        // Assign subjects to student
        private async Task AssignSubjectsToStudentAsync(string studentId, string year, string semester, string program)
        {
            var subjects = await _context.Subjects
                .Where(s => s.YearLevel == year && s.Semester == semester && s.Program == program)
                .ToListAsync();

            foreach (var subject in subjects)
            {
                var studentSubjectId = $"{studentId}_{subject.SubjectId}";
                _context.StudentSubjects.Add(new StudentSubject
                {
                    StudentId = studentId,
                    SubjectId = subject.SubjectId,
                    Id = studentSubjectId
                });
            }

            await _context.SaveChangesAsync();
        }

        // Add student's grade info
        public async Task AddStudentGradeInfoAsync(string studentId, string program, string year, string semester, string Fname, string Lname)
        {
            try
            {
                // Check if grade information already exists for this combination of student, program, year, and semester
                if (await _context.Grades.AnyAsync(g => g.StudentId == studentId && g.Program == program && g.YearLevel == year && g.Semester == semester))
                {
                    ShowMessage("Grade information already exists for this student, program, year, and semester.", "Warning", MessageBoxImage.Warning);
                    return;
                }

                var grade = new Grade
                {
                    GradeId = Guid.NewGuid().ToString(),
                    StudentId = studentId,
                    Program = program,
                    YearLevel = year,
                    Semester = semester,
                    FirstName = Fname,
                    LastName = Lname
                };

                _context.Grades.Add(grade);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                HandleError("adding grade information", ex);
            }
        }

        // Edit student details
        public async Task EditStudentAsync(Student updatedStudent)
        {
            if (updatedStudent == null) throw new ArgumentNullException(nameof(updatedStudent));

            try
            {
                var studentToUpdate = await _context.Students.FindAsync(updatedStudent.StudentId);
                if (studentToUpdate == null)
                {
                    ShowMessage("Student not found.", "Error", MessageBoxImage.Error);
                    return;
                }

                if (await IsStudentDuplicateAsync(updatedStudent.StudentId))
                {
                    ShowMessage("Duplicate student detected.", "Error", MessageBoxImage.Error);
                    return;
                }

                studentToUpdate.FirstName = updatedStudent.FirstName;
                studentToUpdate.LastName = updatedStudent.LastName;
                studentToUpdate.Email = updatedStudent.Email;
                studentToUpdate.Program = updatedStudent.Program;
                studentToUpdate.YearLevel = updatedStudent.YearLevel;

                await _context.SaveChangesAsync();

                UpdateStudentInUI(updatedStudent);
                ShowMessage("Student updated successfully.", "Success", MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                HandleError("editing student", ex);
            }
        }

        // Update student in the ObservableCollection
        private void UpdateStudentInUI(Student updatedStudent)
        {
            var studentInCollection = Students.FirstOrDefault(s => s.StudentId == updatedStudent.StudentId);
            if (studentInCollection != null)
            {
                studentInCollection.FirstName = updatedStudent.FirstName;
                studentInCollection.LastName = updatedStudent.LastName;
                studentInCollection.Email = updatedStudent.Email;
                studentInCollection.Program = updatedStudent.Program;
                studentInCollection.YearLevel = updatedStudent.YearLevel;
            }
        }

        // Delete student
        public async Task DeleteStudentAsync(Student student)
        {
            if (student == null) throw new ArgumentNullException(nameof(student));

            try
            {
                var confirmResult = MessageBox.Show($"Are you sure you want to delete the student '{student.FirstName} {student.LastName}'?",
                    "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (confirmResult == MessageBoxResult.Yes)
                {
                    _context.Students.Remove(student);
                    await _context.SaveChangesAsync();

                    Students.Remove(student);
                    ShowMessage("Student deleted successfully.", "Success", MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                HandleError("deleting student", ex);
            }
        }

        // Show generic message in MessageBox
        private void ShowMessage(string message, string caption, MessageBoxImage icon)
        {
            MessageBox.Show(message, caption, MessageBoxButton.OK, icon);
        }

        // Show error message in MessageBox
        private void HandleError(string action, Exception ex)
        {
            ShowMessage($"An error occurred while {action}: {ex.Message}", "Error", MessageBoxImage.Error);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
