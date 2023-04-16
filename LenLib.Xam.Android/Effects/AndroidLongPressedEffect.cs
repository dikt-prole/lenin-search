using LenLib.Xam.Droid.Effects;
using LenLib.Xam.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ResolutionGroupName("LeninSearch.Xam")]
[assembly: ExportEffect(typeof(AndroidLongPressedEffect), "LongPressedEffect")]
namespace LenLib.Xam.Droid.Effects
{
        public class AndroidLongPressedEffect : PlatformEffect
        {
            private bool _attached;
            public static void Initialize() { }

            public AndroidLongPressedEffect() { }

            protected override void OnAttached()
            {
                //because an effect can be detached immediately after attached (happens in listview), only attach the handler one time.
                if (!_attached)
                {
                    if (Control != null)
                    {
                        Control.LongClickable = true;
                        Control.LongClick += Control_LongClick;
                    }
                    else
                    {
                        Container.LongClickable = true;
                        Container.LongClick += Control_LongClick;
                    }
                    _attached = true;
                }
            }

            private void Control_LongClick(object sender, Android.Views.View.LongClickEventArgs e)
            {
                var command = LongPressedEffect.GetCommand(Element);
                command?.Execute(LongPressedEffect.GetCommandParameter(Element));
            }

            protected override void OnDetached()
            {
                if (_attached)
                {
                    if (Control != null)
                    {
                        Control.LongClickable = true;
                        Control.LongClick -= Control_LongClick;
                    }
                    else
                    {
                        Container.LongClickable = true;
                        Container.LongClick -= Control_LongClick;
                    }
                    _attached = false;
                }
            }
        }
    }