using Microsoft.Xaml.Interactivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace CC.Common
{
    /// <summary>
    /// Отображает панель меню.
    /// </summary>
    public class OpenMenuFlyoutAction : DependencyObject, IAction
    {
        public object Execute(object sender, object parameter)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            FlyoutBase flyoutBase = FlyoutBase.GetAttachedFlyout(sender as FrameworkElement);

            flyoutBase.ShowAt(senderElement);

            return null;
        }
    }
}