using Android.Text;
using LeninSearch.Xam.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(TextMeasureAndroid))]
namespace LeninSearch.Xam.Droid
{
    public class TextMeasureAndroid : ITextMeasure
    {
        public float Width(string text, string font, float textSize)
        {
            var paint = new TextPaint {TextSize = textSize};
            return paint.MeasureText(text);
        }
    }
}