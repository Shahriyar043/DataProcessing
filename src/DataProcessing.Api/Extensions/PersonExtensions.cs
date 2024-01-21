using DataProcessing.Api.Models;
using System.Text.Json;

namespace DataProcessing.Api.Extensions;

public static class PersonExtensions
{
    public static string ToJsonString(this IEnumerable<Person> persons)
    {
        var jsonList = persons.Select(person =>
            new
            {
                person.Id,
                person.Name,
                person.DateOfBirth,
                person.IsActive
            });

        return JsonSerializer.Serialize(jsonList);
    }
}
