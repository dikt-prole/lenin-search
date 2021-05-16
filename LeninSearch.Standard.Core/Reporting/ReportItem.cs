using System;

namespace LeninSearch.Standard.Core.Reporting
{
    public class ReportItem
    {
        public static string GlobalDeviceId { get; set; }
        public string Device { get; set; }
        public string File { get; set; }
        public ushort Index { get; set; }
        public ReportType Type { get; set; }
        public DateTime DateTime { get; set; }

        public static ReportItem Construct(ReportType type, string file, ushort index)
        {
            return new ReportItem
            {
                Device = GlobalDeviceId,
                File = file,
                Index = index,
                Type = type,
                DateTime = DateTime.UtcNow
            };
        }
    }
}