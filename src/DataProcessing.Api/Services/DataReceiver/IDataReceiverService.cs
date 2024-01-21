using DataProcessing.Api.Models;

namespace DataProcessing.Api.Services.DataReceiver;

public interface IDataReceiverService
{
    Task<IEnumerable<Person>> ReceiveDataAsync();
}
