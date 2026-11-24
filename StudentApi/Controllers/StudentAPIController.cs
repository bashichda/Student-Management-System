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

    }
}
