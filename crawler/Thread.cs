using System;


namespace crawler
{
    public partial class crawler
    {
        public class SelfThread
        {
            public int ThreadNO { get; set; }
            public DateTime Started { get; set; }
            public DateTime Ended { get; set; }
            public string TimeDiff { get; set; }
            public string Status { get; set; }
            public string Keyword { get; set; }
            public int Page { get; set; }
            public int Row { get; set; }
            public string Url { get; set; }
            public string Exeption { get; set; }
            public string Browser { get; set; }
            public string PublicIpAddress { get; set; }
            public bool Google { get; set; }
        }

    }
}
