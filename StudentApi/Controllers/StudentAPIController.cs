using Microsoft.AspNetCore.Mvc;
using StudentAPIBusinessLayer;
using StudentDataAccessLayer;

namespace StudentApi.Controllers
{
    [ApiController]
    //[Route("api/[controller]")]
    [Route("api/Students")]
    public class StudentAPIController : ControllerBase
    {
        [HttpGet("All", Name = "GetAllStudents")] // Marks this method to respond to HTTP GET requests.
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<StudentDTO>> GetAllStudents()
        {
            List<StudentDTO> studentsList = Student.GetAllStudents();
            if (studentsList.Count == 0)
            {
                return NotFound("No students found.");
            }

            return Ok(studentsList);// return The list of Students.
        }

        [HttpGet("Passed", Name = "GetPassedStudents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<StudentDTO>> GetPassedStudents()
        {
            List<StudentDTO> passedStudents = Student.GetPassedStudents();
            if (passedStudents.Count == 0)
            {
                return NotFound("No students passed.");
            }
            return Ok(passedStudents);

        }

        [HttpGet("AverageGrade", Name = "GetAverageGrades")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<double> GetAverageGrades()
        {
            double avgGrade = Student.GetAverageGrades();
            return Ok(avgGrade);
        }

        [HttpGet("{id}",Name = "GetStudentById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<StudentDTO> GetStudentById(int id)
        {
            if (id < 1)
            {
                return BadRequest($"Not accepted ID {id}");
            }
            Student student = Student.Find(id);
            if (student == null)
            {
                return NotFound($"Student with ID {id} not found.");
            }

            // here we get only the DTO object to send it back:
            StudentDTO SDTO = student.SDTO;

            // we return the DTO not the student object
            return Ok(SDTO);
        }

        [HttpPost(Name = "AddStudent")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<StudentDTO> AddStudent(StudentDTO newStudentDTO)
        {
            if (newStudentDTO == null || string.IsNullOrWhiteSpace(newStudentDTO.Name) || newStudentDTO.Age < 1 ||
                newStudentDTO.Grade < 0 || newStudentDTO.Grade > 100)
            {
                return BadRequest("Not accepted student data");
            }

            if (Student.Find(newStudentDTO.Id) != null)
            {
                return BadRequest($"Student with ID {newStudentDTO.Id} already exists");
            }

            Student student = new Student(new StudentDTO(newStudentDTO.Id,newStudentDTO.Name,newStudentDTO.Age,newStudentDTO.Grade));
            if (student.Save())
            {
                newStudentDTO.Id = student.ID;
                // we return the DTO only not the full object
                // we dont return ok here,we return createdRoute: this will be status code 201 created.
                return CreatedAtRoute("GetStudentById", new { id = newStudentDTO.Id }, newStudentDTO);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to add student");
            }

        }

        [HttpPut("{id}",Name = "UpdateStudent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<StudentDTO> UpdateStudent(int id,StudentDTO updatedStudent)
        {
            if ( id < 1 || updatedStudent == null || string.IsNullOrWhiteSpace(updatedStudent.Name) || updatedStudent.Age <= 0 || updatedStudent.Grade < 0 || updatedStudent.Grade > 100)
            {
                return BadRequest("Not accepted student data;");
            }

            Student student = Student.Find(id);
            if (student == null)
            {
                return NotFound($"No student with ID {id} found");
            }

            student.Name = updatedStudent.Name;
            student.Age = updatedStudent.Age;
            student.Grade = updatedStudent.Grade;
            if (student.Save())
            {
                // we return the DTO not the full student Object:
                return Ok(student.SDTO);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to update student");
            }

        }
    }
}
