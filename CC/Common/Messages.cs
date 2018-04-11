using CC.Enums;
using CC.Helpers;
using System;
using System.Threading.Tasks;
using Windows.System.Profile;
using Windows.UI.Popups;

namespace CC.Common
{
    /// <summary>
    /// Статический класс, предназначенный для вывода диалоговых окон.
    /// </summary>
    public static class Messages
    {
        /// <summary>
        /// Отображает окно, предлагающее сохранить изменения в файле.
        /// </summary>
        /// <param name="fileName">Имя файла.</param>
        /// <returns>Результат работы окна.</returns>
        public static async Task<MessageDialogResult> SaveTheChangesAsync(string fileName)
        {
            string title = StringHelper.ToString("SaveChangesTitle");
            string content = $"{StringHelper.ToString("SaveChanges")} \"{fileName}\"?";

            return await ShowAsync(content, title, MessageDialogCommands.YesNoCancel);
        }

        /// <summary>
        /// Отображает окно, сообщающее о том, что перенесенный на поле файл не поддерживается.
        /// </summary>
        /// <param name="displayType">Информация о типе файла.</param>
        public static void NotSupportedDroppedFile(string displayType)
        {
            string title = StringHelper.ToString("NotSupportedDroppedFileTitle");
            string content = $"{StringHelper.ToString("NotSupportedDroppedFile")} {displayType}.";

            Show(content, title);
        }

        /// <summary>
        /// Отображает окно, сообщающее об ошибке открытия файла.
        /// </summary>
        /// <param name="ex">Текст ошибки.</param>
        public static void OpenFileError(string ex)
        {
            string title = StringHelper.ToString("OpenFileErrorTitle");
            string message = StringHelper.ToString(nameof(OpenFileError));
            string content = $"{message}\n\n{ex.Trim()}";

            Show(content, title);
        }

        /// <summary>
        /// Отображает окно, сообщающее об ошибке сохранения файла.
        /// </summary>
        /// <param name="ex">Текст ошибки.</param>
        public static void SaveFileError(string ex)
        {
            string title = StringHelper.ToString("SaveFileErrorTitle");
            string message = StringHelper.ToString(nameof(SaveFileError));
            string content = $"{message}\n\n{ex}";

            Show(content, title);
        }

        /// <summary>
        /// Отображает окно, сообщающее об ошибке печати файла.
        /// </summary>
        /// <param name="ex">Текст ошибки.</param>
        public static void PrintFileError()
        {
            string title = StringHelper.ToString("PrintFileErrorTitle");
            string content = StringHelper.ToString(nameof(PrintFileError));

            Show(content, title);
        }

        /// <summary>
        /// Отображает окно, сообщающее об ошибке печати файла.
        /// </summary>
        /// <param name="ex">Текст ошибки.</param>
        public static void PrintFileError(string ex)
        {
            string title = StringHelper.ToString("PrintFileErrorTitle");
            string message = StringHelper.ToString(nameof(PrintFileError));
            string content = $"{message}\n\n{ex}";

            Show(content, title);
        }

        /// <summary>
        /// Отображает окно, спрашивающее о действии, которое требуется совершить с загруженным списком терминов.
        /// </summary>
        /// <returns>Результат работы окна.</returns>
        public static async Task<MessageDialogResult> SelectOperationWithLoadedListFileAsync()
        {
            string title = StringHelper.ToString("SelectOperationWithLoadedListFileTitle");
            string content = StringHelper.ToString("SelectOperationWithLoadedListFile");

            return await ShowAsync(content, title, MessageDialogCommands.YesNoCancel);
        }

        /// <summary>
        /// Отображает окно, спрашивающее о действии, которое требуется выполнить с загруженным файлом сетки кроссворда.
        /// </summary>
        /// <returns>Результат работы окна.</returns>
        public static async Task<MessageDialogResult> SelectOperationWithLoadedGridFileAsync()
        {
            string title = StringHelper.ToString("SelectOperationWithLoadedGridFileTitle");
            string content = StringHelper.ToString("SelectOperationWithLoadedGridFile");

            return await ShowAsync(content, title, MessageDialogCommands.YesNoCancel);
        }

        /// <summary>
        /// Отображает окно, которое сообщает, что кроссворд заполнен неверно, и предлагает показать ошибки.
        /// </summary>
        /// <returns>Результат работы окна.</returns>
        public static async Task<MessageDialogResult> CrosswordFilledIsNotCorrectlyAsync()
        {
            string title = StringHelper.ToString("CrosswordFilledIsNotCorrectlyTitle");
            string content = StringHelper.ToString("CrosswordFilledIsNotCorrectly");

            return await ShowAsync(content, title, MessageDialogCommands.YesNo);
        }

        /// <summary>
        /// Отображает окно, сообщающее о том, что кроссворд заполнен верно.
        /// </summary>
        public static void CrosswordFilledIsCorrectly()
        {
            string title = StringHelper.ToString("CrosswordFilledIsCorrectlyTitle");
            string content = StringHelper.ToString("CrosswordFilledIsCorrectly");

            Show(content, title);
        }

        /// <summary>
        /// Отображает окно сообщения.
        /// </summary>
        /// <param name="content">Текст, который требуется отобразить в окне.</param>
        /// <param name="title">Заголовок окна.</param>
        /// <param name="commands">Кнопки, отображаемые в окне сообщения.</param>
        /// <returns>Результат работы окна.</returns>
        private static async Task<MessageDialogResult> ShowAsync(string content, string title, MessageDialogCommands commands)
        {
            MessageDialog messageDialog = new MessageDialog(content, title);

            switch (commands)
            {
                case MessageDialogCommands.YesNo:
                    messageDialog.Commands.Add(new UICommand { Label = StringHelper.ToString(nameof(MessageDialogResult.Yes)), Id = MessageDialogResult.Yes });
                    messageDialog.Commands.Add(new UICommand { Label = StringHelper.ToString(nameof(MessageDialogResult.No)), Id = MessageDialogResult.No });
                    break;
                case MessageDialogCommands.YesNoCancel:
                    messageDialog.Commands.Add(new UICommand { Label = StringHelper.ToString(nameof(MessageDialogResult.Yes)), Id = MessageDialogResult.Yes });
                    messageDialog.Commands.Add(new UICommand { Label = StringHelper.ToString(nameof(MessageDialogResult.No)), Id = MessageDialogResult.No });
                    if (AnalyticsInfo.VersionInfo.DeviceFamily != "Windows.Mobile")
                        messageDialog.Commands.Add(new UICommand { Label = StringHelper.ToString(nameof(MessageDialogResult.Cancel)), Id = MessageDialogResult.Cancel });
                    break;
            }

            messageDialog.DefaultCommandIndex = 0;
            messageDialog.CancelCommandIndex = 1;

            IUICommand result = await messageDialog.ShowAsync();
            return (MessageDialogResult)result?.Id;
        }

        /// <summary>
        /// Отображает окно сообщения.
        /// </summary>
        /// <param name="content">Текст, который требуется отобразить в окне.</param>
        /// <param name="title">Заголовок окна.</param>
        private static async void Show(string content, string title)
        {
            await new MessageDialog(content, title).ShowAsync();
        }

        /// <summary>
        /// Отображает окно сообщения.
        /// </summary>
        /// <param name="content">Текст, который требуется отобразить в окне.</param>
        private static async void Show(string content)
        {
            await new MessageDialog(content).ShowAsync();
        }
    }
}