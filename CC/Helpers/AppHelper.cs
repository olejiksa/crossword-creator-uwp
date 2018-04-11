using System;
using System.Globalization;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.System.Profile;
using Windows.UI;
using Windows.UI.StartScreen;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace CC.Helpers
{
    /// <summary>
    /// Класс-помощник для работы с приложением.
    /// </summary>
    public class AppHelper
    {
        /// <summary>
        /// Статусная строка.
        /// </summary>
        private StatusBar status;

        /// <summary>
        /// Operating system build number.
        /// </summary>
        private ulong SystemBuildNumber
        {
            get
            {
                string sv = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
                ulong v = ulong.Parse(sv);
                /*Using for getting full operationg system version number.
                ulong v1 = (v & 0xFFFF000000000000L) >> 48;
                ulong v2 = (v & 0x0000FFFF00000000L) >> 32;
                ulong v3 = (v & 0x00000000FFFF0000L) >> 16;
                ulong v4 = (v & 0x000000000000FFFFL);*/
                return (v & 0x00000000FFFF0000L) >> 16;
            }
        }

        /// <summary>
        /// Returns app version.
        /// </summary>
        /// <returns>Current app version.</returns>
        public string GetAppVersion()
        {
            PackageVersion version = Package.Current.Id.Version;
            return $"{version.Major}.{version.Minor}.{version.Build}";
        }

        /// <summary>
        /// Returns date as string.
        /// </summary>
        /// <param name="date">Date.</param>
        /// <returns>String that includes date only.</returns>
        public string ToShortDateString(DateTime date)
        {
            return date.ToString(DateTimeFormatInfo.CurrentInfo.ShortDatePattern);
        }

        /// <summary>
        /// Проверяет и устанавливает тему оформления.
        /// </summary>
        public void CheckTheme()
        {
            // Определяет цветовую гамму заголовка окна или статусбара.
            if (AnalyticsInfo.VersionInfo.DeviceFamily != "Windows.Mobile")
                if (Application.Current.RequestedTheme == ApplicationTheme.Light)
                {
                    ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
                    titleBar.BackgroundColor = titleBar.ButtonBackgroundColor = titleBar.InactiveBackgroundColor = titleBar.ButtonInactiveBackgroundColor =
                        Color.FromArgb(255, 230, 230, 230);
                    titleBar.ButtonHoverForegroundColor = titleBar.ButtonPressedForegroundColor = Colors.Black;
                    titleBar.ButtonHoverBackgroundColor = Color.FromArgb(255, 207, 207, 207);
                    titleBar.ButtonPressedBackgroundColor = Color.FromArgb(255, 184, 184, 184);
                }
                else
                {
                    ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
                    titleBar.BackgroundColor = titleBar.ButtonBackgroundColor = titleBar.InactiveBackgroundColor = titleBar.ButtonInactiveBackgroundColor =
                        Color.FromArgb(255, 31, 31, 31);
                    titleBar.ButtonHoverForegroundColor = titleBar.ButtonPressedForegroundColor = Colors.White;
                    titleBar.ButtonHoverBackgroundColor = Color.FromArgb(255, 53, 53, 53);
                    titleBar.ButtonPressedBackgroundColor = Color.FromArgb(255, 76, 76, 76);
                }
            else if (Application.Current.RequestedTheme == ApplicationTheme.Light)
            {
                status = StatusBar.GetForCurrentView();
                status.BackgroundColor = Color.FromArgb(255, 230, 230, 230);
                status.ForegroundColor = Color.FromArgb(255, 102, 102, 102);
                status.BackgroundOpacity = 1;
            }
            else if (Application.Current.RequestedTheme == ApplicationTheme.Dark)
            {
                status = StatusBar.GetForCurrentView();
                status.BackgroundColor = Color.FromArgb(255, 31, 31, 31);
                status.ForegroundColor = Color.FromArgb(255, 191, 191, 191);
                status.BackgroundOpacity = 1;
            }
        }

        /// <summary>
        /// Получает список переходов для приложения в меню "Пуск" и панель задач.
        /// </summary>
        public async void GetJumpList()
        {
            if (SystemBuildNumber < 10586)
                return;
            JumpList jumpList = await JumpList.LoadCurrentAsync();
            jumpList.SystemGroupKind = JumpListSystemGroupKind.Recent;
            await jumpList.SaveAsync();
        }

        /// <summary>
        /// Добавляет файл в список последних открытых файлов.
        /// </summary>
        /// <param name="file">Файл, который требуется добавить.</param>
        public void AddFileToMRU(StorageFile file)
        {
            if (SystemBuildNumber < 10586)
                return;
            string mruToken = StorageApplicationPermissions.MostRecentlyUsedList.Add(file,
                string.Empty, RecentStorageItemVisibility.AppAndSystem);
        }
    }
}