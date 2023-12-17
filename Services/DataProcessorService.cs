namespace DataProcesser.Services;

public class DataProcessorService : IDataProcessorService
{
    private readonly List<DataJobDTO> _dataJobs = new();

    public IEnumerable<DataJobDTO> GetAllDataJobs()
    {
        return _dataJobs;
    }

    public IEnumerable<DataJobDTO> GetDataJobsByStatus(DataJobStatus status)
    {
        return _dataJobs.Where(dj => dj.Status == status);
    }

    public DataJobDTO GetDataJob(Guid id)
    {
        return _dataJobs.FirstOrDefault(dj => dj.Id == id);
    }

    public DataJobDTO Create(DataJobDTO dataJob)
    {
        _dataJobs.Add(dataJob);
        return dataJob;
    }

    public DataJobDTO Update(DataJobDTO dataJob)
    {
        var existingDataJob = _dataJobs.FirstOrDefault(dj => dj.Id == dataJob.Id);
        if (existingDataJob == null)
            return null;

        existingDataJob.Name = dataJob.Name;
        existingDataJob.FilePathToProcess = dataJob.FilePathToProcess;
        existingDataJob.Status = dataJob.Status;
        existingDataJob.Results = dataJob.Results;
        existingDataJob.Links = dataJob.Links;

        return existingDataJob;
    }

    public void Delete(Guid dataJobId)
    {
        var dataJob = _dataJobs.First(dj => dj.Id == dataJobId);
        _dataJobs.Remove(dataJob);
    }

    public bool StartBackgroundProcess(Guid dataJobId)
    {
        var dataJob = _dataJobs.First(dj => dj.Id == dataJobId);

        //It has started already or finalized already.
        if (dataJob.Status != DataJobStatus.New)
            return false;

        // Simulate background processing
        dataJob.Status = DataJobStatus.Processing;
        // Simulate some processing result
        dataJob.Results = new List<string> { "Processing started" };
        return true;
    }

    public DataJobStatus GetBackgroundProcessStatus(Guid dataJobId)
    {
        var dataJob = _dataJobs.First(dj => dj.Id == dataJobId);
        return dataJob.Status;
    }

    public List<string> GetBackgroundProcessResults(Guid dataJobId)
    {
        var dataJob = _dataJobs.First(dj => dj.Id == dataJobId);
        return dataJob.Results.ToList();
    }
}
