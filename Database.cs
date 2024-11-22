using System.Data.SQLite;

namespace iLabTicketMgr;

public class Database
{
    private SQLiteConnection _conn;

    public Database()
    {
        _conn = new SQLiteConnection(@"Data Source=C:\Projects\iLabTicketManager\Databases\iLabTicketMgr.db");
        _conn.Open();
    }

    public Ticket? GetTicketById(int ticketNum)
    {
        var query = _conn.CreateCommand();
        query.CommandText = "SELECT * FROM tickets WHERE number = @number";
        query.Parameters.AddWithValue("@number", ticketNum);
        var result = query.ExecuteReader();
        var tickets = GetTicketsFromReader(result);
        return tickets.Count > 0 ? tickets[0] : null;
    }

    /// <summary>
    /// Will insert a ticket into the database or replace it if it exists.
    /// </summary>
    public void UpsertTicket(Ticket ticket)
    {
        var query = _conn.CreateCommand();
        query.CommandText = "INSERT OR REPLACE INTO tickets VALUES (@num, @status, @created, @modified, @contacts, @subject, @type, @subtype, @closed)";
        query.Parameters.AddWithValue("@num", ticket.Number);
        query.Parameters.AddWithValue("@status", ticket.Status);
        query.Parameters.AddWithValue("@created", ticket.DateCreated);
        query.Parameters.AddWithValue("@modified", ticket.DateModified);
        query.Parameters.AddWithValue("@contacts", ticket.Contacts);
        query.Parameters.AddWithValue("@subject", ticket.Subject);
        query.Parameters.AddWithValue("@type", ticket.Type);
        query.Parameters.AddWithValue("@subtype", ticket.SubType);
        query.Parameters.AddWithValue("@closed", ticket.DateClosed);
        
        query.ExecuteNonQuery();
    }

    /// <summary>
    /// Selects the N latest tickets starting from <paramref name="offset"/>
    /// </summary>
    public IEnumerable<Ticket> SelectNLatestTickets(int n, int offset = 0)
    {
        var query = _conn.CreateCommand();
        query.CommandText = "SELECT * FROM tickets ORDER BY datetime(date_modified) DESC LIMIT @n OFFSET @offset";
        query.Parameters.AddWithValue("@n", n);
        query.Parameters.AddWithValue("@offset", offset);
        
        var result = query.ExecuteReader();
        return GetTicketsFromReader(result);
    }

    private List<Ticket> GetTicketsFromReader(SQLiteDataReader? reader)
    {
        if (reader == null || !reader.HasRows) return [];

        List<Ticket> tickets = [];
        while (reader.Read())
        {
            tickets.Add(reader.GetTicket());
        }
        
        reader.Close();
        return tickets; 
    }

    ~Database()
    {
        _conn.Close();
    }
}