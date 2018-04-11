using CC.Common;
using CC.Enums;
using CC.Helpers;
using CC.Model;
using CC.Services;
using CC.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.ViewManagement;

namespace CC.ViewModel
{
    /// <summary>
    /// Модель представления страницы заполнения кроссворда.
    /// </summary>
    public class FillingViewModel : BaseViewModel
    {
        /// <summary>
        /// Имя файла по умолчанию.
        /// </summary>
        private readonly string _defaultFileName = StringHelper.ToString("UntitledGrid");

        #region Private fields
        private StorageFile _gridFile;
        private FillingGridWordModel _selectedItem;
        private string _gridFileName, _filledAnswer;
        private bool _isFileReady, _isEmptyVisible, _hasListItems, _hasGridItems, _isCtrlKeyPressed, _hasSelectedItem, _isCheckEnabled;
        private PrintHelper printHelper;
        #endregion

        #region Constructor
        /// <summary>
        /// Конструктор по умолчанию.
        /// </summary>
        public FillingViewModel()
        {
            CheckCommand = new DelegateCommand(async () =>
            {
                List<ushort> mistakes = new List<ushort>();

                foreach (FillingGridWordModel item in Items)
                    if (item.Answer.Filled != item.Answer.Needed)
                        mistakes.Add((ushort)(item.ID - 1));

                if (mistakes.Count > 0)
                {
                    MessageDialogResult result = await Messages.CrosswordFilledIsNotCorrectlyAsync();

                    if (result == MessageDialogResult.Yes)
                    {
                        for (ushort i = 0; i < Items.Count; i++)
                            Items[i].IsSelected = false;

                        for (ushort i = 0; i < mistakes.Count; i++)
                            Items[mistakes[i]].IsSelected = true;
                    }
                }
                else
                    Messages.CrosswordFilledIsCorrectly();
            });

            GoToQuestionsViewCommand = new DelegateCommand(() =>
            {
                Locator.NavigationHelper.CurrentFrame.Navigate(typeof(QuestionsView));
            });

            OpenCommand = new DelegateCommand(async () =>
            {
                FileOpenPicker picker = new FileOpenPicker();
                picker.FileTypeFilter.Add(".cwgf");

                StorageFile file = await picker.PickSingleFileAsync();
                OpenGrid(file);

                WordList.CollectionChanged += WordList_CollectionChanged;
                Items.CollectionChanged += Items_CollectionChanged;
            });

            PrintCommand = new DelegateCommand(async () =>
            {
                await printHelper.ShowPrintUIAsync();
            });
        }
        #endregion
        
        #region Commands
        /// <summary>
        /// Команда проверки правильности заполнения кроссворда.
        /// </summary>
        public ICommand CheckCommand { get; private set; }
        /// <summary>
        /// Команда навигции на страницу вопросов.
        /// </summary>
        public ICommand GoToQuestionsViewCommand { get; private set; }
        /// <summary>
        /// Команда открытия файла.
        /// </summary>
        public ICommand OpenCommand { get; private set; }
        /// <summary>
        /// Команда печати файла.
        /// </summary>
        public ICommand PrintCommand { get; private set; }
        #endregion

        #region Properties
        /// <summary>
        /// Указывает на наличие элементов в списке.
        /// </summary>
        public bool HasListItems
        {
            get { return _hasListItems; }
            private set
            {
                Set(ref _hasListItems, value);
                IsEmptyVisible = !value;
            }
        }

        /// <summary>
        /// Возвращает или задает значение, указывающее на готовность файла к печати и не только.
        /// </summary>
        public bool IsFileReady
        {
            get { return _isFileReady; }
            private set { Set(ref _isFileReady, value); }
        }

        /// <summary>
        /// Указывает на видимость информации об отсутствии элементов в списке.
        /// </summary>
        public bool IsEmptyVisible
        {
            get { return _isEmptyVisible; }
            private set { Set(ref _isEmptyVisible, value); }
        }

        /// <summary>
        /// Название открытого файла сетки.
        /// </summary>
        public string GridFileName
        {
            get
            {
                if (!string.IsNullOrEmpty(_gridFileName))
                    return _gridFileName;
                else
                    return _defaultFileName;
            }
            private set { Set(ref _gridFileName, value); ApplicationView.GetForCurrentView().Title = value; }
        }

        /// <summary>
        /// Текущий открытый файл сетки в приложении.
        /// <see cref="null"/> указывает на его отсутствие.
        /// </summary>
        private StorageFile GridFile
        {
            get { return _gridFile; }
            set
            {
                Set(ref _gridFile, value);
                GridFileName = value != null ? value.DisplayName : _defaultFileName;
                HasGridItems = HasListItems = value != null;
                IsFileReady = value != null;
                FilledAnswers.Clear();
                SelectedItem = null;
                IsCheckEnabled = false;
            }
        }

        /// <summary>
        /// Обозначает выделенный элемент в сетке.
        /// </summary>
        public FillingGridWordModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                Set(ref _selectedItem, value);
                HasSelectedItem = value != null;
                
                if (value != null)
                    FilledAnswer = SelectedItem.Answer.Filled;
            }
        }

        public string FilledAnswer
        {
            get { return _filledAnswer; }
            set
            {
                Set(ref _filledAnswer, value);
                Items[SelectedItem.ID - 1].Answer = new FillingAnswerModel()
                {
                    Filled = value,
                    Needed = Items[SelectedItem.ID - 1].Answer.Needed
                };
                IsCheckEnabled = true;
            }
        }

        /// <summary>
        /// Указывает на наличие элементов в сетке.
        /// </summary>
        public bool HasGridItems
        {
            get { return _hasGridItems; }
            private set { Set(ref _hasGridItems, value); }
        }

        /// <summary>
        /// Указывает на наличие выделенного элемента в списке и на сетке.
        /// </summary>
        public bool HasSelectedItem
        {
            get { return _hasSelectedItem; }
            private set { Set(ref _hasSelectedItem, value); }
        }

        /// <summary>
        /// Возвращает или задает значение, указывающее на возможность проверки правильности заполнения кроссворда.
        /// </summary>
        public bool IsCheckEnabled
        {
            get { return _isCheckEnabled; }
            private set { Set(ref _isCheckEnabled, value); }
        }

        /// <summary>
        /// Словарь заполненных ответов.
        /// </summary>
        public Dictionary<ushort, string> FilledAnswers { get; private set; } = new Dictionary<ushort, string>();

        /// <summary>
        /// Коллекция элементов списка слов.
        /// </summary>
        public ObservableCollection<ListWordViewModel> WordList { get; set; } = new ObservableCollection<ListWordViewModel>();

        /// <summary>
        /// Коллекция элементов сетки.
        /// </summary>
        public ObservableCollection<FillingGridWordModel> Items { get; set; } = new ObservableCollection<FillingGridWordModel>();
        #endregion

        #region Methods
        /// <summary>
        /// Обрабатывает начало перемещения элемента (drag'n'drop) в сетке.
        /// </summary>
        /// <param name="sender">Объект-отправитель.</param>
        /// <param name="e">Передаваемые данные.</param>
        public void OnGridDragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
        }

        /// <summary>
        /// Обрабатывает конец перемещения элемента (drag'n'drop) в сетке.
        /// </summary>
        /// <param name="sender">Объект-отправитель.</param>
        /// <param name="e">Передаваемые данные.</param>
        public async void OnGridDrop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                IReadOnlyList<IStorageItem> storageItems = await e.DataView.GetStorageItemsAsync();

                StorageFile file = (StorageFile)storageItems[0];
                FileExtension extension = new FileExtensionHelper(file.FileType).GetFileExtension;

                if (extension == FileExtension.CrosswordGridFile)
                    OpenGrid(file);
                else
                    Messages.NotSupportedDroppedFile(new FileExtensionHelper(FileExtension.CrosswordGridFile).GetFileType);
            }
        }

        /// <summary>
        /// Общий метод для всех способов открытия файла списка.
        /// </summary>
        /// <param name="file">Файл списка, который требуется открыть.</param>
        private async void OpenList(StorageFile file)
        {
            if (file == null)
                return;

            List<ListWordModel> data = new List<ListWordModel>();

            try
            {
                data = new XmlService().ReadList(await FileIO.ReadTextAsync(file), true);
                if (data == null)
                    return;
            }
            catch (Exception ex)
            {
                Messages.OpenFileError(ex.Message);
                return;
            }

            WordList.Clear();

            ObservableCollection<ListWordViewModel> readed
                = new ObservableCollection<ListWordViewModel>(data?.Select(e => new ListWordViewModel(e)));

            for (int i = 1; i <= readed.Count; i++)
                readed[i - 1].ID = (ushort)i;

            foreach (ListWordViewModel item in readed)
                WordList.Add(item);
        }

        /// <summary>
        /// Общий метод для всех способов открытия файла сетки.
        /// </summary>
        /// <param name="file">Файл сетки, который требуется открыть.</param>
        public async void OpenGrid(StorageFile file)
        {
            if (file == null)
                return;

            List<FillingGridWordModel> data = new List<FillingGridWordModel>();

            try
            {
                data = new XmlService().ReadGridForFilling(await FileIO.ReadTextAsync(file));
                if (data == null)
                    return;
            }
            catch (Exception ex)
            {
                Messages.OpenFileError(ex.Message);
                return;
            }

            Items.Clear();
            GridFile = file;
            new AppHelper().AddFileToMRU(GridFile);

            ObservableCollection<FillingGridWordModel> readed = new ObservableCollection<FillingGridWordModel>(data);

            for (ushort i = 1; i <= readed.Count; i++)
                readed[i - 1].ID = i;

            foreach (FillingGridWordModel item in readed)
                Items.Add(item);

            for (ushort i = 1; i <= readed.Count; i++)
                FilledAnswers.Add(i, string.Empty);

            OpenList(GridFile);
        }

        /// <summary>
        /// Обрабатывает изменение коллекции списка слов.
        /// </summary>
        private void WordList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            HasListItems = WordList.Count > 0;
        }

        /// <summary>
        /// Обрабатывает изменение коллекции сетки.
        /// </summary>
        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            HasGridItems = Items.Count > 0;
        }

        public void OnNavigatedTo(FillingView view)
        {
            // Initalize common helper class and register for printing.
            printHelper = new PrintHelper(view);
            printHelper.RegisterForPrinting();

            // Initialize print content for this scenario.
            printHelper.PreparePrintContent(new PrintGridView());
        }

        public void OnNavigatedFrom()
        {
            if (printHelper != null)
                printHelper.UnregisterForPrinting();
        }

        /// <summary>
        /// Обрабатывает нажатие клавиши.
        /// </summary>
        /// <param name="sender">Объект-отправитель.</param>
        /// <param name="e">Передаваемые данные.</param>
        public void OnPageKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Control)
                _isCtrlKeyPressed = true;

            if (_isCtrlKeyPressed)
                switch (e.Key)
                {
                    case VirtualKey.O: OpenCommand.Execute(null); break;
                    case VirtualKey.P: PrintCommand.Execute(null); break;
                }
        }

        /// <summary>
        /// Обрабатывает снятие нажатия с клавиши.
        /// </summary>
        /// <param name="sender">Объект-отправитель.</param>
        /// <param name="e">Передаваемые данные.</param>
        public void OnPageKeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Control)
                _isCtrlKeyPressed = false;
        }
        #endregion
    }
}