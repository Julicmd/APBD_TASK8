using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityTasksDbFirstApi.Data;
using UniversityTasksDbFirstApi.DTOs;
using UniversityTasksDbFirstApi.Models;

namespace UniversityTasksDbFirstApi.Controllers;


[ApiController]
[Route("api/submissions")]
public class SubmissionController :ControllerBase
{
    private readonly UniversityTasksDbContext _dbContext;

    public SubmissionController(UniversityTasksDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    
    //POST /api/submissions Create a new submission after checking business rules. 
    [HttpPost]
    public async Task<ActionResult> CreateSubmission([FromBody] CreateSubmissionDto sub)
    {
        
        var student = await _dbContext.Students.FirstOrDefaultAsync(st => st.StudentId  == sub.StudentId);

        if (student == null)
        {
            return NotFound();
        }

        if (!student.IsActive)
        {
            return BadRequest("Student is not active");
        }

        var assigment = await _dbContext.Assignments
            .FirstOrDefaultAsync(a => a.AssignmentId == sub.AssignmentId);
        
        if (assigment == null)
        {
            return NotFound("Assignment not found");
        }

        if (!assigment.IsPublished)
        {
            return BadRequest("Assignment is not published");
        }
        
        var status = DateTime.UtcNow > assigment.DueDate ? "Late" : "Submitted";
        
        var enrollment = await _dbContext.Enrollments
            .FirstOrDefaultAsync(e => e.StudentId == sub.StudentId 
                                      && e.CourseId == assigment.CourseId);
        if (enrollment == null)
        {
            return BadRequest("Student is not enrolled into this course");
        }

        if (enrollment.Status != "Active" && enrollment.Status != "Completed")
        {
            return BadRequest("Student enrollment is not active or completed");
        }
        if (string.IsNullOrEmpty(sub.RepositoryUrl) || !sub.RepositoryUrl.StartsWith("https://"))
        {
            return BadRequest("Url is required");
        }

        var newSub = new Submission()
        {
            StudentId = sub.StudentId,
            AssignmentId = sub.AssignmentId,
            RepositoryUrl = sub.RepositoryUrl,
            Status = status,
        };
        
        await _dbContext.AddAsync(newSub);
        await _dbContext.SaveChangesAsync();
        
        return CreatedAtAction(nameof(CreateSubmission), new {id = newSub.SubmissionId}, newSub);
    }


    //. DELETE /api/submissions/{idSubmission} Delete a submission
    //only if it is not graded. Return 400when the submission is already graded.
    
    [HttpDelete("{idSubmission:int}")]
    public async Task<ActionResult> DeleteSubmission(int idSubmission)
    {
        var Sub = await _dbContext.Submissions.FirstOrDefaultAsync(s => s.SubmissionId == idSubmission);

        if (Sub == null)
        {
            return NotFound();
        }

        if (Sub.Status == "Graded")
        {
            return Conflict("Submission is graded");
        }

        _dbContext.Remove(Sub);
        await _dbContext.SaveChangesAsync();
        
        return NoContent();
    }
    
    
}