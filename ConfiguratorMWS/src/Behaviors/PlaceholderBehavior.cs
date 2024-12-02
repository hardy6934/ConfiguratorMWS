using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace ConfiguratorMWS.src.Behaviors
{
    public static class PlaceholderBehavior
    {
        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.RegisterAttached(
                "Placeholder",
                typeof(string),
                typeof(PlaceholderBehavior),
                new PropertyMetadata(string.Empty, OnPlaceholderChanged));

        public static string GetPlaceholder(TextBox textBox) =>
            (string)textBox.GetValue(PlaceholderProperty);

        public static void SetPlaceholder(TextBox textBox, string value) =>
            textBox.SetValue(PlaceholderProperty, value);

        private static void OnPlaceholderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                textBox.GotFocus -= RemovePlaceholder;
                textBox.LostFocus -= ShowPlaceholder;

                if (!string.IsNullOrEmpty((string)e.NewValue))
                {
                    textBox.LostFocus += ShowPlaceholder;
                    textBox.GotFocus += RemovePlaceholder;
                    ShowPlaceholder(textBox, null);
                }
            }
        }

        private static void ShowPlaceholder(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Foreground = Brushes.Gray;
                textBox.Text = GetPlaceholder(textBox);
            }
        }

        private static void RemovePlaceholder(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.Text == GetPlaceholder(textBox))
            {
                textBox.Foreground = Brushes.Black;
                textBox.Text = string.Empty;
            }
        }
    }
}
