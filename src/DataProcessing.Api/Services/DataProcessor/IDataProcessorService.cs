using DataProcessing.Api.Models;

namespace DataProcessing.Api.Services.DataProcessor;

public interface IDataProcessorService
{
    Task<List<Person>> ReadCsvDataAsync(Stream fileStream);
    Task SetDataToMemoryAsync(IEnumerable<Person> persons);
    Task<IEnumerable<Person>> GetDataFromMemoryAsync();
}
