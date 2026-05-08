using Microsoft.Data.Sqlite;

namespace Hellbot.Service.Data;

internal static class SqliteErrors
{
    private const int SqliteConstraint = 19; // SQLITE_CONSTRAINT

    public static bool IsConstraintViolation(SqliteException ex) =>
        ex.SqliteErrorCode == SqliteConstraint;
}
