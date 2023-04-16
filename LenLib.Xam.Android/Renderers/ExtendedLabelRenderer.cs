using System.ComponentModel;
using System.IO;
using System.Reflection;
using Android.Content;
using Android.Graphics;
using Android.Text;
using Android.Text.Style;
using Java.Lang;
using LenLib.Xam.Controls;
using LenLib.Xam.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ExtendedLabel), typeof(ExtendedLabelRenderer))]
namespace LenLib.Xam.Droid.Renderers
{
    public class ExtendedLabelRenderer : LabelRenderer
    {
        public ExtendedLabelRenderer(Context context) : base(context) { }

        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            if (Element is ExtendedLabel el)
            {
                if (el.JustifyText)
                {
                    if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
                    {
                        Control.JustificationMode = JustificationMode.InterWord;
                    }
                }

                UpdateFormattedText();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (Element is ExtendedLabel el && el.JustifyText)
            {
                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
                {
                    Control.JustificationMode = Android.Text.JustificationMode.InterWord;
                }
            }
        }

        private void UpdateFormattedText()
        {
            if (Element.FormattedText == null) return;

            var extensionType = typeof(FormattedStringExtensions);
            var type = extensionType.GetNestedType("FontSpan", BindingFlags.NonPublic);
            var ss = new SpannableString(Control.TextFormatted);
            var ssText = ss.ToString();
            var spans = ss.GetSpans(0, ss.ToString().Length, Class.FromType(type));
            foreach (var span in spans)
            {
                var start = ss.GetSpanStart(span);
                var end = ss.GetSpanEnd(span);
                var imageFile = ssText.Substring(start, end - start);
                
                if (imageFile.EndsWith(".jpeg") && File.Exists(imageFile))
                {
                    var flags = ss.GetSpanFlags(span);
                    ss.RemoveSpan(span);
                    using (var bitmap = BitmapFactory.DecodeFile(imageFile))
                    {
                        var width = bitmap.Width;
                        var height = bitmap.Height;
                        var newHeight = 3 * Settings.UI.Fonts.ReadingFontSize * Settings.UI.ScreenDensity;
                        var newWidth = newHeight * width / height;
                        var scaleWidth = (float)newWidth / width;
                        var scaleHeight = (float)newHeight / height;
                        var matrix = new Matrix();
                        matrix.PostScale(scaleWidth, scaleHeight);
                        var resized = Bitmap.CreateBitmap(bitmap, 0, 0, width, height, matrix, false);
                        var imageSpan = new ImageSpan(Context, resized, SpanAlign.Baseline);
                        ss.SetSpan(imageSpan, start, end, flags);
                    }
                }
            }
            Control.TextFormatted = ss;
        }
    }
}