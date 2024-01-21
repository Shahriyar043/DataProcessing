using DataProcessing.Api.Models;

namespace DataProcessing.Api.Services.DataSender;

public interface IDataSenderService
{
    Task SendDataAsync(IEnumerable<Person> persons);
}
