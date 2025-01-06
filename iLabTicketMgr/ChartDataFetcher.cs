using System.Data.SQLite;
using System.Globalization;
using System.Numerics;
using ScottPlot;

namespace iLabTicketMgr;

public class ChartDataFetcher
{
    private SQLiteConnection _conn;
    public DataWindow CurrentWindow { get; set; } = DataWindow.Year;
    public DateTime CustomWindowStart { get; set; } = DateTime.Today;
    public DateTime CustomWindowEnd { get; set; } = DateTime.Today;

    public ChartDataFetcher()
    {
        _conn = new SQLiteConnection(@"Data Source=Databases\iLabTicketMgr.db");
        _conn.Open();
    }

    public enum DataWindow
    {
        Day,
        Week,
        Month,
        Year,
        AllTime,
        Custom
    }

    private (DateTime Start, DateTime End) GetDataWindowDates(DataWindow window)
    {
        switch (window)
        {
            case DataWindow.Day:
                return (DateTime.Now.AddDays(-1).Date, DateTime.Now.Date);
            case DataWindow.Week:
                return (DateTime.Now.AddDays(-7).Date, DateTime.Now.Date);
            case DataWindow.Month:
                return (DateTime.Now.AddMonths(-1).Date, DateTime.Now.Date);
            case DataWindow.Year:
                return (DateTime.Now.AddYears(-1).Date, DateTime.Now.Date);
            case DataWindow.AllTime:
                return (DateTime.UnixEpoch.Date, DateTime.Now.Date);
            case DataWindow.Custom:
                return (CustomWindowStart, CustomWindowEnd);
            default:
                throw new ArgumentOutOfRangeException(nameof(window), window, null);
        }
    }

    public Dictionary<string, int> GetTicketDistribution(string column)
    {
        var query = _conn.CreateCommand();
        query.CommandText = column switch
        {
            "status" =>
                "SELECT status, count(status) FROM tickets WHERE datetime(date_created) BETWEEN @start AND @end GROUP BY status ORDER BY count(status) DESC ",
            "subtype" =>
                "SELECT subtype, count(subtype) FROM tickets WHERE datetime(date_created) BETWEEN @start AND @end GROUP BY subtype ORDER BY count(subtype) DESC",
            "type" =>
                "SELECT type, count(type) FROM tickets WHERE datetime(date_created) BETWEEN @start AND @end GROUP BY type ORDER BY count(type) DESC",
            _ => throw new ArgumentOutOfRangeException(nameof(column), column, "ChartDataFetcher: Invalid column")
        };
        var dateRange = GetDataWindowDates(CurrentWindow);
        query.Parameters.AddWithValue("@start", dateRange.Start);
        query.Parameters.AddWithValue("@end", dateRange.End);
        
        var result = query.ExecuteReader();
        
        var data = new Dictionary<string, int>();
        while (result.Read())
        {
            var col = result.GetString(0);
            var count = result.GetInt32(1);
            
            data.Add(col, count);
        }
        return data;
    }
    
    Func<DateTime, int> weekProjector = 
        d => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
            d,
            CalendarWeekRule.FirstFullWeek,
            DayOfWeek.Sunday);

    private Dictionary<DateTime, T> SortTicketsByWindow<T>(IList<(DateTime, T)> sqlData, DataWindow groupBy, bool useGroupAverage = false) where T : INumber<T>
    {
        // converts a given DateTime to a numerical calendar grouping
        // relative to current group (past month, week, etc.)
        Func<DateTime, DateTime> evalGroup = date => throw new Exception("SortTicketsByWindow: Invalid data grouping window. How did you get here??");
        if (groupBy == DataWindow.Day)
        {
            evalGroup = date => date.Date;
        } else if (groupBy == DataWindow.Week)
        {
            evalGroup = date =>
            {
                var week = ISOWeek.GetWeekOfYear(date);
                var weekStart = ISOWeek.ToDateTime(date.Year, week, DateTimeFormatInfo.InvariantInfo.FirstDayOfWeek);
                return weekStart;
            };
        } else if (groupBy == DataWindow.Month)
        {
            evalGroup = date => new DateTime(date.Year, date.Month, 1);
        } else if (groupBy == DataWindow.Year)
        {
            evalGroup = date => new DateTime(date.Year, 1, 1);
        } else if (groupBy == DataWindow.AllTime)
        {
            evalGroup = date => DateTime.Today;
        }

        var sortedData = new Dictionary<DateTime, T>();
        var occurrences = new Dictionary<DateTime, int>();
        foreach (var (date, value) in sqlData)
        {
            var calendarGroup = evalGroup(date);
            if (useGroupAverage)
            {
                occurrences.TryAdd(calendarGroup, 0);
                occurrences[calendarGroup]++;
                
                sortedData.TryAdd(calendarGroup, value);
                sortedData[calendarGroup] += (value - sortedData[calendarGroup]) / (T)Convert.ChangeType(occurrences[calendarGroup], typeof(T));
            }
            else
            {
                if (!sortedData.TryAdd(calendarGroup, value))
                {
                    sortedData[calendarGroup] += value;
                }
            }
        }

        return sortedData;
    }
    

    public Dictionary<DateTime, int> GetTicketCreationData(DataWindow groupBy = DataWindow.Week)
    {
        var query = _conn.CreateCommand();
        query.CommandText = "SELECT date_created, COUNT(*) FROM tickets WHERE datetime(date_created) BETWEEN @start AND @end GROUP BY datetime(date_created) ORDER BY datetime(date_created) ASC ";
        
        var dateRange = GetDataWindowDates(CurrentWindow);
        query.Parameters.AddWithValue("@start", dateRange.Start);
        query.Parameters.AddWithValue("@end", dateRange.End);
        
        var result = query.ExecuteReader();
        
        var data = new List<(DateTime, int)>();
        while (result.Read())
        {
            var col = result.GetDateTime(0);
            var count = result.GetInt32(1);
            
            data.Add((col, count));
        }
        
        return SortTicketsByWindow(data, groupBy);
    }
    
    public Dictionary<DateTime, double> GetTicketClosureData(DataWindow groupBy = DataWindow.Week)
    {
        var query = _conn.CreateCommand();
        query.CommandText = "SELECT date_created, date_closed FROM tickets WHERE date_closed NOT NULL AND datetime(date_created) BETWEEN @start AND @end";
        
        var dateRange = GetDataWindowDates(CurrentWindow);
        query.Parameters.AddWithValue("@start", dateRange.Start);
        query.Parameters.AddWithValue("@end", dateRange.End);
        
        var result = query.ExecuteReader();
        
        var data = new List<(DateTime created, double closeTime)>();
        while (result.Read())
        {
            var created = result.GetDateTime(0);
            var closed = result.GetDateTime(1);
            var closeTime = (closed - created).TotalHours;
            data.Add((created, closeTime));
        }
        
        return SortTicketsByWindow(data, groupBy, useGroupAverage: true);
    }
    
    public List<(string subtype, double hours)> GetTicketClosureBySubtype(DataWindow groupBy = DataWindow.Week)
    {
        var query = _conn.CreateCommand();
        query.CommandText = "SELECT subtype, avg(unixepoch(date_closed) - unixepoch(date_created)) FROM tickets WHERE date_closed NOT NULL AND datetime(date_created) BETWEEN @start AND @end GROUP BY subtype";
        
        var dateRange = GetDataWindowDates(CurrentWindow);
        query.Parameters.AddWithValue("@start", dateRange.Start);
        query.Parameters.AddWithValue("@end", dateRange.End);
        
        var result = query.ExecuteReader();
        
        var data = new List<(string, double)>();
        while (result.Read())
        {
            var subtype = result.GetString(0);
            var secs = result.GetDouble(1);
            var hours = secs / 3600; 
            data.Add((subtype, hours));
        }

        return data;
    }
}