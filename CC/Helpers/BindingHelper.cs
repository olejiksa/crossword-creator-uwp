using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace CC.Helpers
{
    /// <summary>
    /// Класс-помощник по работе с привязками.
    /// </summary>
    public class BindingHelper
    {
        public static readonly DependencyProperty CanvasTopBindingPathProperty = DependencyProperty.RegisterAttached(
                "CanvasTopBindingPath", typeof(string), typeof(BindingHelper), new PropertyMetadata(null, CanvasBindingPathPropertyChanged));

        public static readonly DependencyProperty CanvasLeftBindingPathProperty = DependencyProperty.RegisterAttached(
                "CanvasLeftBindingPath", typeof(string), typeof(BindingHelper), new PropertyMetadata(null, CanvasBindingPathPropertyChanged));

        public static string GetCanvasTopBindingPath(DependencyObject obj)
        {
            return (string)obj.GetValue(CanvasTopBindingPathProperty);
        }

        public static void SetCanvasTopBindingPath(DependencyObject obj, string value)
        {
            obj.SetValue(CanvasTopBindingPathProperty, value);
        }

        public static string GetCanvasLeftBindingPath(DependencyObject obj)
        {
            return (string)obj.GetValue(CanvasLeftBindingPathProperty);
        }

        public static void SetCanvasLeftBindingPath(DependencyObject obj, string value)
        {
            obj.SetValue(CanvasLeftBindingPathProperty, value);
        }

        private static void CanvasBindingPathPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            string propertyPath = (string)e.NewValue;

            if (!string.IsNullOrEmpty(propertyPath))
            {
                DependencyProperty canvasProperty = e.Property == CanvasTopBindingPathProperty ? Canvas.TopProperty : Canvas.LeftProperty;
                BindingOperations.SetBinding(obj, canvasProperty, new Binding { Path = new PropertyPath(propertyPath) });
            }
        }
    }
}