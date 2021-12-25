namespace LeninSearch.Standard.Core.Corpus
{
    public class VideoDataItem
    {
        public VideoDataItem() {}

        public VideoDataItem(string videoId, ushort firstParagraphIndex, ushort lastParagraphIndex)
        {
            VideoId = videoId;
            FirstParagraphIndex = firstParagraphIndex;
            LastParagraphIndex = lastParagraphIndex;
        }

        public string VideoId { get; set; }
        public ushort FirstParagraphIndex { get; set; }
        public ushort LastParagraphIndex { get; set; }
    }
}