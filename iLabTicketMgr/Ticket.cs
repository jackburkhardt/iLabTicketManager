using System.IO;

namespace iLabTicketMgr;

public class Ticket
{
    public int Number { get; set; }
    public string Status { get; set; }
    public string Subject { get; set; }
    public string Type { get; set; }
    public string SubType { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DateModified { get; set; }
    public DateTime? DateClosed { get; set; }
    public string Contacts { get; set; }
}

public static class TicketReader
{
    public static IEnumerable<Ticket> ParseTickets(string path)
    {
        if (!path.EndsWith(".csv"))
            throw new FileFormatException($"TicketReader: Expected a .csv file, instead got path {path}");
        string[] lines = File.ReadAllLines(path);

        var parsedTickets = new List<Ticket>();
        
        for (int i = 1; i < lines.Length; i++){
            
            // splitting by commas alone is erroneous, but all fields have quotes
           var fields = lines[i].Split("\",\"");
           fields = fields.Select(f => f.Trim('\"')).ToArray();
           
           parsedTickets.Add( new Ticket
           {
               Status = fields[0],
               DateCreated = DateTime.Parse(fields[1]),
               DateModified = DateTime.Parse(fields[2]),
               Contacts = fields[4],
               Number = int.Parse(fields[5]),
               Subject = fields[6],
               Type = fields[7],
               SubType = fields[8],
               DateClosed = fields[9] == "" ? null : DateTime.Parse(fields[9]),
           });
        };
        
        return parsedTickets; 
    }
}