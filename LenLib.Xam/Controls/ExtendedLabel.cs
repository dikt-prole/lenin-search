using System;
using Xamarin.Forms;

namespace LenLib.Xam.Controls
{
    public class ExtendedLabel : Label
    {
        public static readonly BindableProperty JustifyTextProperty =
            BindableProperty.Create(
                propertyName: nameof(JustifyText),
                returnType: typeof(Boolean),
                declaringType: typeof(ExtendedLabel),
                defaultValue: false,
                defaultBindingMode: BindingMode.OneWay
            );

        public bool JustifyText
        {
            get { return (Boolean)GetValue(JustifyTextProperty); }
            set { SetValue(JustifyTextProperty, value); }
        }
    }
}