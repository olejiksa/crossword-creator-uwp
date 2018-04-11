using CC.Common;
using CC.Helpers;
using Microsoft.Services.Store.Engagement;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;

namespace CC.ViewModel
{
    /// <summary>
    /// Модель представления сведений о приложении.
    /// </summary>
    public class AboutViewModel : BaseViewModel
    {
        private AppHelper appHelper = new AppHelper();

        /// <summary>
        /// Пустой конструктор.
        /// </summary>
        public AboutViewModel()
        {
            var dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;
        }

        #region Commands
        public ICommand SendMailCommand => new DelegateCommand(async () =>
        {
            var mailtoPairs = new Dictionary<string, string>
            {
                { "to", "quillaur@outlook.com" },
                { "subject", $"Crossword Creator ({appHelper.GetAppVersion()})" }
            };
            string mailto = "mailto:";
            byte iterator = 0;
            foreach (var pair in mailtoPairs)
            {
                if (iterator == 0)
                    mailto += '?';

                mailto += $"{pair.Key}={pair.Value}&";

                if (iterator + 1 == mailtoPairs.Count)
                    mailto.Remove(mailto.Length - 1);

                iterator++;
            }

            await Launcher.LaunchUriAsync(new Uri(mailto));
        });

        public ICommand RateCommand => new DelegateCommand(async () =>
        {
            await Launcher.LaunchUriAsync(new Uri(@"ms-windows-store:REVIEW?PFN=" + Package.Current.Id.FamilyName));
        });

        public ICommand ShareLinkCommand => new DelegateCommand(() =>
        {
            DataTransferManager.ShowShareUI();
        });

        public ICommand GoToFeedbackHubCommand => new DelegateCommand(async () =>
        {
            await StoreServicesFeedbackLauncher.GetDefault().LaunchAsync();
        });
        #endregion

        #region Properties
        public bool IsFeedbackHyperlinkVisible => StoreServicesFeedbackLauncher.IsSupported();

        public string Version => $"{StringHelper.ToString("Version")} {appHelper.GetAppVersion()}";

        public string LastUpdateDate => appHelper.ToShortDateString(new DateTime(2017, 5, 5));
        #endregion

        /// <summary>
        /// Обрабатывает запрос к отправке данных в общий доступ.
        /// </summary>
        private void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var request = args.Request;
            request.Data.SetWebLink(new Uri($"https://www.microsoft.com/ru-ru/store/p/crossword-creator-uwp/9nblggh513s3"));

            request.Data.Properties.Title = "Crossword Creator";
            request.Data.Properties.Description = StringHelper.ToString("ShareWebsiteLink");
        }
    }
}
