using DataProcessing.Api.Services.DataProcessor;
using DataProcessing.Api.Services.DataReceiver;
using DataProcessing.Api.Services.DataSender;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DataProcessing.Api.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/data")]
public class DataController : ControllerBase
{
    #region Constructor and Dependencies

    private readonly IDataProcessorService dataProcessorService;
    private readonly IDataReceiverService dataReceiverService;
    private readonly IDataSenderService dataSenderService;
    private readonly ILogger<DataController> logger;

    public DataController(
        IDataProcessorService dataProcessorService,
        IDataReceiverService dataReceiverService,
        IDataSenderService dataSenderService,
        ILogger<DataController> logger)
    {
        this.dataProcessorService = dataProcessorService ?? throw new ArgumentNullException(nameof(dataProcessorService));
        this.dataReceiverService = dataReceiverService ?? throw new ArgumentNullException(nameof(dataReceiverService));
        this.dataSenderService = dataSenderService ?? throw new ArgumentNullException(nameof(dataSenderService));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    #endregion

    #region File Handling

    [HttpPost("read-data-from-file")]
    public async Task<IActionResult> ReadDataFromFileAsync([Required] IFormFile file)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest("File is empty or null.");
        }

        using (var memoryStream = new MemoryStream())
        {
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var persons = await dataProcessorService.ReadCsvDataAsync(memoryStream).ConfigureAwait(false);
            await dataProcessorService.SetDataToMemoryAsync(persons).ConfigureAwait(false);

            return Ok(persons);
        }
    }

    #endregion

    #region Message Queue Operations

    [HttpPost("send-data-to-queue")]
    public async Task<IActionResult> SendDataToQueueAsync()
    {
        var processedPersons = await dataProcessorService.GetDataFromMemoryAsync().ConfigureAwait(false);
        await dataSenderService.SendDataAsync(processedPersons).ConfigureAwait(false);
        return Ok("Data sent to the message queue successfully");
    }

    [HttpPost("receive-data-from-queue")]
    public async Task<IActionResult> ReceiveDataFromQueueAsync()
    {
        var receivedPersons = await dataReceiverService.ReceiveDataAsync().ConfigureAwait(false);
        return Ok(receivedPersons);
    }

    #endregion
}
