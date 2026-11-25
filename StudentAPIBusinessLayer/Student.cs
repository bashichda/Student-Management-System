using StudentDataAccessLayer;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;

namespace StudentAPIBusinessLayer
{
    public class Student
    {
        public enum enMode { AddNew = 0, Update = 1 }
        public enMode Mode = enMode.AddNew;
        public StudentDTO SDTO
        {
            get { return (new StudentDTO(this.ID, this.Name, this.Age, this.Grade)); }
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public int Grade { get; set; }

        public Student (StudentDTO SDTO, enMode cMode = enMode.AddNew)
        {
            this.ID = SDTO.Id;
            this.Name = SDTO.Name;
            this.Age = SDTO.Age;
            this.Grade = SDTO.Grade;
            this.Mode = cMode;
        }
        public static List<StudentDTO> GetAllStudents()
        {
            return StudentData.GetAllStudents();
        }
        public static List<StudentDTO> GetPassedStudents()
        {
            return StudentData.GetPassedStudents();
        }
        public static double GetAverageGrades()
        {
            return StudentData.GetAverageGrades();
        }
        public static Student Find(int Id)
        {
            StudentDTO SDTO = StudentData.GetStudentById(Id);

            if (SDTO != null)
            {
                return new Student(SDTO, enMode.Update);
            }
            else
            {
                return null;
            }
        }
        private bool _AddNewStudent()
        {
            this.ID = StudentData.AddStudent(SDTO);

            return this.ID != -1;
        }
        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewStudent())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case enMode.Update:
                    return false;
            }

            return false;
        }

    }
}
