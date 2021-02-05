namespace LeninSearch.Core
{
    public class Heading
    {
        public ushort Index { get; set; }
        public ushort StartPage { get; set; }
        public ushort EndPage { get; set; }
        public byte Level { get; set; }
        public string Text { get; set; }
    }
}