using Microsoft.AspNetCore.Mvc;
using task8.DTOs;
using task8.Exceptions;
using task8.Services;

namespace task8.Controllers;

[ApiController]
[Route("api/submissions")]
public class SubmissionsController : ControllerBase
{
    private readonly ISubmissionService _submissionService;

    public SubmissionsController(ISubmissionService submissionService)
    {
        _submissionService = submissionService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateSubmission(CreateSubmissionDto request)
    {
        try
        {
            var submission = await _submissionService.CreateSubmissionAsync(request);
            return Created("", submission);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (BadRequestException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{idSubmission:int}/grade")]
    public async Task<IActionResult> GradeSubmission(
        int idSubmission,
        GradeSubmissionDto request)
    {
        try
        {
            await _submissionService.GradeSubmissionAsync(idSubmission, request);
            return Ok();
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (BadRequestException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{idSubmission:int}")]
    public async Task<IActionResult> DeleteSubmission(int idSubmission)
    {
        try
        {
            await _submissionService.DeleteSubmissionAsync(idSubmission);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (BadRequestException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}