using Xamarin.Forms;

namespace LeninSearch.Xam.Controls
{
    public class ImageJoystick : Grid
    {
        private const int ButtonSize = 24;
        public ImageJoystick()
        {
            ColumnSpacing = 0;
            RowSpacing = 0;
            HeightRequest = ButtonSize * 2;
            WidthRequest = ButtonSize * 3;
            RowDefinitions = new RowDefinitionCollection
            {
                new RowDefinition {Height = ButtonSize},
                new RowDefinition {Height = ButtonSize}
            };
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition {Width = ButtonSize},
                new ColumnDefinition {Width = ButtonSize},
                new ColumnDefinition {Width = ButtonSize}
            };

            var upButton = new Button {Text = "U"};
            Children.Add(upButton);
            SetRow(upButton, 0);
            SetColumn(upButton, 1);

            var leftButton = new Button { Text = "L" };
            Children.Add(leftButton);
            SetRow(leftButton, 0);
            SetColumn(leftButton, 0);

            var rightButton = new Button { Text = "R" };
            Children.Add(rightButton);
            SetRow(rightButton, 0);
            SetColumn(rightButton, 2);

            var downButton = new Button { Text = "D" };
            Children.Add(downButton);
            SetRow(downButton, 1);
            SetColumn(downButton, 1);
        }
    }
}