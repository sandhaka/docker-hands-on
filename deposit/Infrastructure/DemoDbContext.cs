using Newtonsoft.Json;

namespace Deposit.Infrastructure;

internal class DemoDbContext : IDbContext
{
    public HashSet<Entities.Deposit> Deposits => new HashSet<Entities.Deposit>(Load());

    private IEnumerable<Entities.Deposit> Load()
    {
        var json = File.ReadAllText("sampleDb.json");
        return JsonConvert.DeserializeObject<List<Entities.Deposit>>(json) ?? Enumerable.Empty<Entities.Deposit>();
    }
}

public interface IDbContext
{
    HashSet<Entities.Deposit> Deposits { get; }
}