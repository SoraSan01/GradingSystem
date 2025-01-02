using GradingSystem.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradingSystem.ViewModel
{
    public class StudentsViewModel
    {
        public ObservableCollection<Student> Students { get; set; }

        public StudentsViewModel()
        {
            Students = new ObservableCollection<Student>
            {
                new Student { Name = "Sora", ID = "2024-0001", Course = "Computer Science", ImagePath = "/Resources/teacher.png" },
                new Student { Name = "Riku", ID = "2024-0002", Course = "Information Technology", ImagePath = "/Resources/teacher.png" },
                new Student { Name = "Sora", ID = "2024-0001", Course = "Computer Science", ImagePath = "/Resources/teacher.png" },
                new Student { Name = "Sora", ID = "2024-0001", Course = "Computer Science", ImagePath = "/Resources/teacher.png" },
                new Student { Name = "Sora", ID = "2024-0001", Course = "Computer Science", ImagePath = "/Resources/teacher.png" },

            };
        }

        public class Student
        {
            public string Name { get; set; }
            public string ID { get; set; }
            public string Course { get; set; }
            public string ImagePath { get; set; }
        }

    }
}
