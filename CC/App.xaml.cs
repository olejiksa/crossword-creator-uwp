using CC.Common;
using CC.Enums;
using CC.Helpers;
using CC.View;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Foundation.Metadata;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CC
{
    /// <summary>
    /// Корневой класс приложения.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Конструктор по умолчанию.
        /// </summary>
        public App()
        {
            InitializeComponent();
        }

        private Frame Frame => Locator.NavigationHelper.CurrentFrame;

        /// <summary>
        /// Срабатывает при прямом запуске приложения.
        /// </summary>
        /// <param name="e">Передаваемые данные.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
            {
                rootFrame = new Frame();
                Window.Current.Content = rootFrame;
                rootFrame.Navigate(typeof(ShellView));

                Frame.Navigated += OnNavigated;

                SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
                if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
                    HardwareButtons.BackPressed += OnBackPressed;

                UpdateBackButtonVisibility();

                if (e.PrelaunchActivated == false)
                    Window.Current.Activate();

                AppHelper app = new AppHelper();
                app.CheckTheme();
                app.GetJumpList();

                UpdateBackButtonVisibility();
            }
        }

        /// <summary>
        /// Срабатывает при открытии файла.
        /// </summary>
        /// <param name="args">Передаваемые данные.</param>
        protected override async void OnFileActivated(FileActivatedEventArgs args)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
            {
                rootFrame = new Frame();
                Window.Current.Content = rootFrame;
                rootFrame.Navigate(typeof(ShellView));

                Frame.Navigated += OnNavigated;

                SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
                if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
                    HardwareButtons.BackPressed += OnBackPressed;

                UpdateBackButtonVisibility();

                Window.Current.Activate();

                AppHelper app = new AppHelper();
                app.CheckTheme();
                app.GetJumpList();

                UpdateBackButtonVisibility();

                await Task.Delay(1);
                ChoosingNextPage((IStorageFile)args.Files[0]);

                Frame.BackStack.Clear();
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility
                    = AppViewBackButtonVisibility.Collapsed;
            }
            else
                ChoosingNextPage((IStorageFile)args.Files[0]);
        }

        /// <summary>
        /// Срабатывает при навигации нажатием на аппаратную или экранную кнопку Back.
        /// </summary>
        /// <param name="sender">Объект-отправитель.</param>
        /// <param name="e">Передаваемые данные.</param>
        void OnBackPressed(object sender, BackPressedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                e.Handled = true;
                Frame.GoBack();
            }
        }

        /// <summary>
        /// Срабатывает при навигации нажатием на аппаратную или экранную кнопку Back.
        /// </summary>
        /// <param name="sender">Объект-отправитель.</param>
        /// <param name="e">Передаваемые данные.</param>
        void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                e.Handled = true;
                Frame.GoBack();
            }
        }

        /// <summary>
        /// Срабатывает при любой навигации.
        /// </summary>
        /// <param name="sender">Объект-отправитель.</param>
        /// <param name="e">Передаваемые данные.</param>
        void OnNavigated(object sender, NavigationEventArgs e)
        {
            UpdateBackButtonVisibility();
        }

        /// <summary>
        /// Управляет видимостью кнопки Back в панели заголовка окна.
        /// </summary>
        private void UpdateBackButtonVisibility()
        {
            AppViewBackButtonVisibility visibility = AppViewBackButtonVisibility.Collapsed;
            if (Frame.CanGoBack)
                visibility = AppViewBackButtonVisibility.Visible;

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = visibility;
        }

        /// <summary>
        /// Выбирает следующую страницу для навигации при открытии файла.
        /// </summary>
        /// <param name="file">Полученный файл.</param>
        /// <param name="file">Открывалось ли приложение вместе с открытием файла.</param>
        private async void ChoosingNextPage(IStorageFile file, bool isFileActivated = false)
        {
            FileExtension extension = new FileExtensionHelper(file.FileType).GetFileExtension;

            switch (extension)
            {
                case FileExtension.CrosswordListFile:
                    if (Frame.CurrentSourcePageType == typeof(ListEditorView))
                    {
                        Frame.Navigate(typeof(ListEditorView), file);
                        Frame.BackStack.RemoveAt(Frame.BackStack.Count - 1);
                    }
                    Frame.Navigate(typeof(ListEditorView), file);
                    break;
                case FileExtension.CrosswordGridFile:
                    MessageDialogResult result = await Messages.SelectOperationWithLoadedGridFileAsync();
                    switch (result)
                    {
                        case MessageDialogResult.Yes:
                            if (Frame.CurrentSourcePageType == typeof(FillingView))
                            {
                                Frame.Navigate(typeof(FillingView), file);
                                Frame.BackStack.RemoveAt(Frame.BackStack.Count - 1);
                            }
                            Frame.Navigate(typeof(FillingView), file);
                            break;
                        case MessageDialogResult.No:
                            if (Frame.CurrentSourcePageType == typeof(GridEditorView))
                            {
                                Frame.Navigate(typeof(GridEditorView), file);
                                Frame.BackStack.RemoveAt(Frame.BackStack.Count - 1);
                            }
                            Frame.Navigate(typeof(GridEditorView), file);
                            break;
                        case MessageDialogResult.Cancel: break;
                    }
                    break;
            }
        }
    }
}