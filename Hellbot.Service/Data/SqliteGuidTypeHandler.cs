using System.Data;
using Dapper;

namespace Hellbot.Service.Data;

// SQLite TEXT UUIDs come back as string from the reader; Dapper needs this to map to Guid.
public sealed class SqliteGuidTypeHandler : SqlMapper.TypeHandler<Guid>
{
    public override Guid Parse(object value) => value switch
    {
        Guid g => g,
        string s => Guid.Parse(s),
        byte[] b when b.Length == 16 => new Guid(b),
        _ => throw new InvalidOperationException($"Cannot convert {value?.GetType().FullName ?? "null"} to Guid."),
    };

    public override void SetValue(IDbDataParameter parameter, Guid value) =>
        parameter.Value = value.ToString();
}
