namespace LeninSearch.Script.Scripts.Models
{
    public class Fb2Line
    {
        public string Text { get; set; }
        public Fb2LineType Type { get; set; }
    }

    public enum Fb2LineType
    {
        Paragraph, Image
    }
}