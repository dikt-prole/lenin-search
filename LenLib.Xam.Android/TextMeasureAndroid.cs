using Android.Text;
using LenLib.Xam.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(TextMeasureAndroid))]
namespace LenLib.Xam.Droid
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