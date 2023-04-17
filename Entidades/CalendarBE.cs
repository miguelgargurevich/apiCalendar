namespace apiCalendar.Entidades
{
    public class CalendarBE
    {
            public int id { get; set; }
            public string? title { get; set; }
            public DateTime? start { get; set; }
            public DateTime? end { get; set; }
            public bool? allDay { get; set; }
            public string? color { get; set; }
            public int? EventTypeId { get; set; }
            public string? type { get; set; }
            public int? CalendarTypeId { get; set; }
            public string? CalendarTypeName { get; set; }
            public string? description { get; set; }
            public int? UserCreate { get; set; }
            public DateTime? DateCreate { get; set; }

    }
}
