using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;

namespace iLabTicketMgr;

public static class Extensions
{
    public static Ticket GetTicket(this SQLiteDataReader reader)
    {
        return new Ticket
        {
            Number = reader.GetInt32(0),
            Status = reader.GetString(1),
            DateCreated = reader.GetDateTime(2),
            DateModified = reader.GetDateTime(3),
            Contacts = reader.GetString(4),
            Subject = reader.GetString(5),
            Type = reader.GetString(6),
            SubType = reader.GetString(7),
            DateClosed = reader.IsDBNull(8) ? null : reader.GetDateTime(8),
        };
    }
}