using DataProcessing.Api.Models;
using DataProcessing.Api.Services.DataProcessor;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;

namespace DataProcessing.Tests;

public class DataProcessorServiceTests
{
    [Fact]
    public async Task ReadCsvDataAsync_ValidStream_ReturnsListOfPersons()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<DataProcessorService>>();
        var memoryCacheMock = new Mock<IMemoryCache>();
        var dataProcessorService = new DataProcessorService(loggerMock.Object, memoryCacheMock.Object);

        var validCsvData = "Id,Name,DateOfBirth,IsActive\n1,John Doe,1992-01-01,true";
        using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(validCsvData)))
        {
            // Act
            var result = await dataProcessorService.ReadCsvDataAsync(stream);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("John Doe", result.First().Name);
            Assert.Equal(1, result.First().Id);
            Assert.True(result.First().IsActive);
            Assert.Equal(new DateTime(1992, 1, 1), result.First().DateOfBirth);
        }
    }

    [Fact]
    public async Task ReadCsvDataAsync_EmptyStream_ThrowsArgumentNullException()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<DataProcessorService>>();
        var memoryCacheMock = new Mock<IMemoryCache>();
        var dataProcessorService = new DataProcessorService(loggerMock.Object, memoryCacheMock.Object);

        using (var emptyStream = new MemoryStream())
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => dataProcessorService.ReadCsvDataAsync(emptyStream));
        }
    }


    [Fact]
    public async Task SetDataToMemoryAsync_NullPersons_ThrowsArgumentNullException()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<DataProcessorService>>();
        var memoryCacheMock = new Mock<IMemoryCache>();
        var dataProcessorService = new DataProcessorService(loggerMock.Object, memoryCacheMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => dataProcessorService.SetDataToMemoryAsync(Enumerable.Empty<Person>()));
    }


    [Fact]
    public async Task GetDataFromMemoryAsync_NoCachedData_ReturnsNull()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<DataProcessorService>>();
        var memoryCacheMock = new Mock<IMemoryCache>();
        var dataProcessorService = new DataProcessorService(loggerMock.Object, memoryCacheMock.Object);

        // Act
        var result = await dataProcessorService.GetDataFromMemoryAsync();

        // Assert
        Assert.Null(result);
    }
}

