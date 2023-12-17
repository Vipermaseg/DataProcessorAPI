using DataProcesser.Services;
using Microsoft.AspNetCore.Mvc;

namespace DataProcesser.Controllers;

[ApiController]
[Route("api/datajobs")]
public class DataProcessorController : ControllerBase
{
    private readonly IDataProcessorService _service;

    public DataProcessorController(IDataProcessorService service)
    {
        _service = service;
    }

    /// <summary>
    /// Retrieves all data jobs.
    /// </summary>
    /// <returns>A list of all data jobs.</returns>
    /// <response code="200">Returns the list of data jobs.</response>
    [HttpGet]
    public IActionResult GetAllDataJobs()
    {
        return Ok(_service.GetAllDataJobs());
    }

    /// <summary>
    /// Retrieves a specific data job by its ID.
    /// </summary>
    /// <param name="id">The ID of the data job.</param>
    /// <returns>The data job with the specified ID.</returns>
    /// <response code="200">Returns the data job with the specified ID.</response>
    /// <response code="404">If the data job is not found.</response>
    [HttpGet("{id}")]
    public IActionResult GetDataJob(Guid id)
    {
        var dataJob = _service.GetDataJob(id);
        if (dataJob == null)
            return NotFound();

        return Ok(dataJob);
    }

    /// <summary>
    /// Retrieves data jobs filtered by their status.
    /// </summary>
    /// <param name="status">The status to filter the data jobs. 2- New, 1- Processing, 0- Completed</param>
    /// <returns>A list of data jobs filtered by the specified status.</returns>
    /// <response code="200">Returns the filtered list of data jobs.</response>
    /// <response code="404">If no data jobs are found with the specified status.</response>
    [HttpGet("status/{status}")]
    public IActionResult GetDataJobsByStatus(DataJobStatus status)
    {
        var dataJobs = _service.GetDataJobsByStatus(status);
        if (!dataJobs.Any())
            return NotFound("No data jobs found with the specified status.");

        return Ok(dataJobs);
    }

    /// <summary>
    /// Creates a new data job.
    /// </summary>
    /// <param name="dataJob">The data job to create.</param>
    /// <returns>The created data job.</returns>
    /// <response code="201">Returns the newly created data job.</response>
    [HttpPost]
    public IActionResult CreateDataJob([FromBody] DataJobDTO dataJob)
    {
        var createdDataJob = _service.Create(dataJob);
        return CreatedAtAction(nameof(GetDataJob), new { id = createdDataJob.Id }, createdDataJob);
    }

    /// <summary>
    /// Updates an existing data job.
    /// </summary>
    /// <param name="dataJob">The updated data job information.</param>
    /// <returns>The updated data job.</returns>
    /// <response code="200">Returns the updated data job.</response>
    /// <response code="404">If the data job is not found.</response>
    [HttpPut]
    public IActionResult UpdateDataJob([FromBody] DataJobDTO dataJob)
    {
        var updatedDataJob = _service.Update(dataJob);
        if (updatedDataJob == null)
            return NotFound();

        return Ok(updatedDataJob);
    }

    /// <summary>
    /// Deletes a data job by its ID.
    /// </summary>
    /// <param name="id">The ID of the data job to delete.</param>
    /// <response code="204">If the data job is successfully deleted.</response>
    /// <response code="404">If the data job is not found.</response>
    [HttpDelete("{id}")]
    public IActionResult DeleteDataJob(Guid id)
    {
        try
        {
            _service.Delete(id);
        }
        catch (ArgumentNullException)
        {
            return NotFound("No data jobs found.");
        }
        return NoContent();
    }

    /// <summary>
    /// Starts the background process for a specific data job.
    /// </summary>
    /// <param name="id">The ID of the data job for which to start the process.</param>
    /// <returns>A confirmation message that the process has started.</returns>
    /// <response code="200">If the background process is successfully started.</response>
    /// <response code="400">If the data job is not in state "new".</response>
    /// <response code="404">If the data job is not found.</response>
    [HttpPost("{id}/startprocess")]
    public IActionResult StartBackgroundProcess(Guid id)
    {
        try
        {
            var started = _service.StartBackgroundProcess(id);
            if (!started)
                return BadRequest("Data job already started or finished.");
        }
        catch (ArgumentNullException)
        {
            return NotFound("No data jobs found.");
        }

        return Ok($"Background process started for data job {id}");
    }

    /// <summary>
    /// Gets the status of the background process for a specific data job.
    /// </summary>
    /// <param name="id">The ID of the data job to check the status.</param>
    /// <returns>The status of the background process.</returns>
    /// <response code="200">Returns the status of the background process.</response>
    /// <response code="404">If the data job is not found.</response>
    [HttpGet("{id}/status")]
    public IActionResult GetBackgroundProcessStatus(Guid id)
    {
        try
        {
            var status = _service.GetBackgroundProcessStatus(id);
            return Ok(status);
        }
        catch (ArgumentNullException)
        {
            return NotFound("No data jobs found.");
        }
    }

    /// <summary>
    /// Retrieves the results of the background process for a specific data job.
    /// </summary>
    /// <param name="id">The ID of the data job to get results for.</param>
    /// <returns>The results of the background process.</returns>
    /// <response code="200">Returns the results of the background process.</response>
    /// <response code="404">If the data job is not found.</response>
    [HttpGet("{id}/results")]
    public IActionResult GetBackgroundProcessResults(Guid id)
    {
        try
        {
            var results = _service.GetBackgroundProcessResults(id);
            return Ok(results);
        }
        catch (ArgumentNullException)
        {
            return NotFound("No data jobs found.");
        }
    }
}
