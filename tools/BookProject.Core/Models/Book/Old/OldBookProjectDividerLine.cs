using System.Drawing;
using Newtonsoft.Json;

namespace BookProject.Core.Models.Book.Old
{
    public class OldBookProjectDividerLine
    {
        private const int DragPointSize = 20;
        public OldBookProjectDividerLine() { }

        public OldBookProjectDividerLine(int y, int leftX, int rightX)
        {
            Y = y;
            LeftX = leftX;
            RightX = rightX;
        }

        [JsonProperty("t")]
        public int Y { get; set; }

        [JsonProperty("lx")]
        public int LeftX { get; set; }

        [JsonProperty("rx")]
        public int RightX { get; set; }

        [JsonIgnore]
        public int Length => RightX - LeftX;

        [JsonIgnore]
        public Rectangle DragRectangle => new Rectangle(RightX - DragPointSize / 2, Y - DragPointSize / 2, DragPointSize, DragPointSize);
    }
}