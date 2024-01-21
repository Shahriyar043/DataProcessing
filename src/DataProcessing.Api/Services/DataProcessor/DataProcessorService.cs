using CsvHelper;
using DataProcessing.Api.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;

namespace DataProcessing.Api.Services.DataProcessor;

public class DataProcessorService : IDataProcessorService
{
    #region Constructor and Dependencies

    private readonly ILogger<DataProcessorService> logger;
    private readonly IMemoryCache memoryCache;

    public DataProcessorService(ILogger<DataProcessorService> logger, IMemoryCache memoryCache)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
    }

    #endregion

    #region IDataProcessorService Implementation

    public Task<List<Person>> ReadCsvDataAsync(Stream fileStream)
    {
        using (var reader = new StreamReader(fileStream))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var persons = csv.GetRecords<Person>().ToList();

            if (!persons.Any())
            {
                logger.LogError("No persons found in the CSV file.");
                throw new ArgumentNullException(nameof(persons));
            }

            return Task.FromResult(persons);
        }
    }

    public async Task SetDataToMemoryAsync(IEnumerable<Person> persons)
    {
        if (persons is null || !persons.Any())
        {
            throw new ArgumentNullException(nameof(persons), "The collection of persons cannot be null or empty.");
        }

        var activePersons = persons.Where(r => r.IsActive);
        var sortedPersons = activePersons.OrderByDescending(r => r.DateOfBirth);

        var cacheEntryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
            SlidingExpiration = TimeSpan.FromMinutes(5)
        };

        memoryCache.Set("CsvData", sortedPersons, cacheEntryOptions);

        await Task.CompletedTask;
    }

    public Task<IEnumerable<Person>> GetDataFromMemoryAsync()
    {
        if (!memoryCache.TryGetValue("CsvData", out IEnumerable<Person>? cachedPersons))
        {
            return Task.FromResult(Enumerable.Empty<Person>());
        }

        if (cachedPersons is null)
        {
            throw new ArgumentNullException(nameof(cachedPersons), "The cachedPersons collection is unexpectedly null.");
        }

        return Task.FromResult(cachedPersons);
    }

    #endregion
}
