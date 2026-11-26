using System;
using System.Data;
using System.Diagnostics.Tracing;
using Microsoft.Data.SqlClient;

namespace StudentDataAccessLayer
{
    public class StudentDTO
    {
        public StudentDTO(int id,string name,int age,int grade)
        {
            this.Id = id;
            this.Name = name;
            this.Age = age;
            this.Grade = grade;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public int Grade { get; set; }
    }
    public class StudentData
    {
        static string _connectionString = 
            "Server=localhost;Database=StudentsDB;User Id=sa;Password=sa;TrustServerCertificate=True;";
        public static List<StudentDTO> GetAllStudents()
        {
            var StudentsList = new List<StudentDTO>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_GetAllStudents",connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            StudentsList.Add(new StudentDTO
                                (
                                    reader.GetInt32(reader.GetOrdinal("Id")),
                                    reader.GetString(reader.GetOrdinal("Name")),
                                    reader.GetInt32(reader.GetOrdinal("Age")),
                                    reader.GetInt32(reader.GetOrdinal("Grade"))
                                ));
                        }
                    }
                }
            }

            return StudentsList;
        }
        public static List<StudentDTO> GetPassedStudents()
        {
            List<StudentDTO> passedStudentsList = new List<StudentDTO>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_GetPassedStudents", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            passedStudentsList.Add(new StudentDTO(
                                    reader.GetInt32(reader.GetOrdinal("Id")),
                                    reader.GetString(reader.GetOrdinal("Name")),
                                    reader.GetInt32(reader.GetOrdinal("Age")),
                                    reader.GetInt32(reader.GetOrdinal("Grade"))
                                ));
                        }
                    }
                }

                return passedStudentsList;
            }
        }
        public static double GetAverageGrades()
        {
            double avgGrades = 0;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_GetAverageGrade",connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();

                    object result = command.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        avgGrades = Convert.ToDouble(result);
                    }
                    else
                    {
                        avgGrades = 0;
                    }
                }
            }

            return avgGrades;
        }
        public static StudentDTO GetStudentById(int StudentId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SP_GetStudentById",connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@StudentId", StudentId);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new StudentDTO(
                                    reader.GetInt32(reader.GetOrdinal("Id")),
                                    reader.GetString(reader.GetOrdinal("Name")),
                                    reader.GetInt32(reader.GetOrdinal("Age")),
                                    reader.GetInt32(reader.GetOrdinal("Grade"))
                                );
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }
        public static int AddStudent(StudentDTO studentDTO)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("SP_AddStudent", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                
                command.Parameters.AddWithValue("@Name", studentDTO.Name);
                command.Parameters.AddWithValue("@Age", studentDTO.Age);
                command.Parameters.AddWithValue("@Grade", studentDTO.Grade);
                var outputIdParam = new SqlParameter("@NewStudentId", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(outputIdParam);
                
                connection.Open();
                command.ExecuteNonQuery();
                
                return (int)outputIdParam.Value;
            }
        }
        public static bool UpdateStudent(StudentDTO studentDTO)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("SP_UpdateStudent",connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@StudentId", studentDTO.Id);
                command.Parameters.AddWithValue("@Name", studentDTO.Name);
                command.Parameters.AddWithValue("@Age", studentDTO.Age);
                command.Parameters.AddWithValue("@Grade", studentDTO.Grade);

                connection.Open();

                try
                {
                    command.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }
    }
}
