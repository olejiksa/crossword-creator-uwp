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
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.ViewManagement;

namespace CC.ViewModel
{
    /// <summary>
    /// Модель представления редактора сетки.
    /// </summary>
    public class GridEditorViewModel : BaseViewModel
    {
        /// <summary>
        /// Имя файла по умолчанию.
        /// </summary>
        private readonly string _defaultFileName = StringHelper.ToString("UntitledGrid");

        #region Private fields
        private StorageFile _listFile, _gridFile;
        private GridWordViewModel _selectedItem;
        private string _gridFileName;
        private bool _isDirty, _isFileReady, _isEmptyVisible, _hasListItems, _hasGridItems, _isCtrlKeyPressed, _isShiftKeyPressed, _isNewEnabled;
        #endregion

        /// <summary>
        /// Конструктор по умолчанию.
        /// </summary>
        public GridEditorViewModel()
        {
            IsEmptyVisible = true;
            WordList.CollectionChanged += WordList_CollectionChanged;
            Items.CollectionChanged += Items_CollectionChanged;

            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;

            AddCommand = new DelegateCommand(() =>
            {
                Locator.NavigationHelper.CurrentFrame.Navigate(typeof(AddGridWordView));
            });

            AddGridWordCommand = new DelegateCommand<ItemClickEventArgs>((ItemClickEventArgs e) =>
            {
                ListWordViewModel item = (ListWordViewModel)e.ClickedItem;

                Items.Add(new GridWordViewModel(new GridWordModel
                {
                    Answer = item.Answer,
                    ID = (ushort)Items.Count,
                    Orientation = Orientation.Horizontal,
                    Question = item.Question,
                    X = 0,
                    Y = 25
                }));

                Locator.NavigationHelper.CurrentFrame.GoBack();
                IsDirty = true;
            });

            DeleteCommand = new DelegateCommand<ushort>((ushort id) =>
            {
                Items.RemoveAt(id);
                UpdateElementsID();

                IsDirty = true;
            });

            OpenListCommand = new DelegateCommand(async () =>
            {
                FileOpenPicker picker = new FileOpenPicker();
                picker.FileTypeFilter.Add(".cwtf");

                StorageFile file = await picker.PickSingleFileAsync();
                OpenList(file);
            });

            OpenGridCommand = new DelegateCommand(async () =>
            {
                FileOpenPicker picker = new FileOpenPicker();
                picker.FileTypeFilter.Add(".cwgf");

                StorageFile file = await picker.PickSingleFileAsync();
                OpenGrid(file);
            });

            SaveGridCommand = new DelegateCommand(async () =>
            {
                if (GridFile == null)
                    await SaveAsAsync();
                else
                    await SaveAsync(GridFile);
            });

            SaveAsGridCommand = new DelegateCommand(async () =>
            {
                await SaveAsAsync();
            });

            ShareCommand = new DelegateCommand(() =>
            {
                DataTransferManager.ShowShareUI();
            });

            NewCommand = new DelegateCommand(async () => { await NewAsync(); });
        }

        #region Commands
        /// <summary>
        /// Команда добавления элемента (навигация на страницу).
        /// </summary>
        public ICommand AddCommand { get; private set; }
        /// <summary>
        /// Команда добавления элемента на сетку.
        /// </summary>
        public ICommand AddGridWordCommand { get; private set; }
        /// <summary>
        /// Команда удаления элемента.
        /// </summary>
        public ICommand DeleteCommand { get; private set; }
        /// <summary>
        /// Команда открытия файла списка.
        /// </summary>
        public ICommand OpenListCommand { get; private set; }
        /// <summary>
        /// Команда открытия файла сетки.
        /// </summary>
        public ICommand OpenGridCommand { get; private set; }
        /// <summary>
        /// Команда сохранения файла сетки.
        /// </summary>
        public ICommand SaveGridCommand { get; private set; }
        /// <summary>
        /// Команда сохранения файла сетки как нового файла.
        /// </summary>
        public ICommand SaveAsGridCommand { get; private set; }
        /// <summary>
        /// Команда предоставления общего доступа к файлу.
        /// </summary>
        public ICommand ShareCommand { get; private set; }
        /// <summary>
        /// Команда создания нового файла.
        /// </summary>
        public ICommand NewCommand { get; private set; }
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
        /// Возвращает или задает значение, указывающее на наличие изменений.
        /// </summary>
        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                Set(ref _isDirty, value);
                IsNewEnabled = true;
            }
        }

        /// <summary>
        /// Возвращает или задает значение, указывающее на готовность файла к функции общего доступа.
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
        /// Текущий открытый файл списка в приложении.
        /// <see cref="null"/> указывает на его отсутствие.
        /// </summary>
        private StorageFile ListFile
        {
            get { return _listFile; }
            set
            {
                if (value != null)
                    IsNewEnabled = true;
                Set(ref _listFile, value);
            }
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
                IsFileReady = value != null;
                IsDirty = false;
            }
        }

        /// <summary>
        /// Обозначает выделенный элемент в сетке.
        /// </summary>
        public GridWordViewModel SelectedItem
        {
            get { return _selectedItem; }
            set { Set(ref _selectedItem, value); }
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
        /// Указывает на возможность создания нового файла.
        /// </summary>
        public bool IsNewEnabled
        {
            get { return _isNewEnabled; }
            private set { Set(ref _isNewEnabled, value); }
        }

        /// <summary>
        /// Коллекция элементов списка слов.
        /// </summary>
        public ObservableCollection<ListWordViewModel> WordList { get; set; } = new ObservableCollection<ListWordViewModel>();

        /// <summary>
        /// Коллекция элементов сетки.
        /// </summary>
        public ObservableCollection<GridWordViewModel> Items { get; set; } = new ObservableCollection<GridWordViewModel>();
        #endregion

        #region Public methods
        /// <summary>
        /// Обрабатывает начало перемещения элемента (drag'n'drop).
        /// </summary>
        /// <param name="sender">Объект-отправитель.</param>
        /// <param name="e">Передаваемые данные.</param>
        public void OnDragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
        }

        /// <summary>
        /// Обрабатывает конец перемещения элемента (drag'n'drop).
        /// </summary>
        /// <param name="sender">Объект-отправитель.</param>
        /// <param name="e">Передаваемые данные.</param>
        public async void OnDrop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                IReadOnlyList<IStorageItem> storageItems = await e.DataView.GetStorageItemsAsync();

                StorageFile file = (StorageFile)storageItems[0];
                FileExtension extension = new FileExtensionHelper(file.FileType).GetFileExtension;

                if (extension == FileExtension.CrosswordListFile)
                    OpenList(file);
                else
                    Messages.NotSupportedDroppedFile(new FileExtensionHelper(FileExtension.CrosswordListFile).GetFileType);
            }
        }

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
        /// Обрабатывает нажатие клавиши.
        /// </summary>
        /// <param name="sender">Объект-отправитель.</param>
        /// <param name="e">Передаваемые данные.</param>
        public async void OnPageKeyDown(object sender, KeyRoutedEventArgs e)
        {
            switch (e.Key)
            {
                case VirtualKey.Control: _isCtrlKeyPressed = true; break;
                case VirtualKey.Shift: _isShiftKeyPressed = true; break;

                case VirtualKey.Delete: DeleteCommand.Execute(SelectedItem.ID); break;
                case VirtualKey.Space: Items[SelectedItem.ID].Orientation
                        = Items[SelectedItem.ID].Orientation == Orientation.Horizontal
                        ? Orientation.Vertical : Orientation.Horizontal; break;
                case VirtualKey.Left: Items[SelectedItem.ID].X -= 25; break;
                case VirtualKey.Right: Items[SelectedItem.ID].X += 25; break;
                case VirtualKey.Up: Items[SelectedItem.ID].Y -= 25; break;
                case VirtualKey.Down: Items[SelectedItem.ID].Y += 25; break;
            }

            if (_isCtrlKeyPressed && _isShiftKeyPressed)
                switch (e.Key)
                {
                    case VirtualKey.S: await SaveAsAsync(); break;
                }
            else if (_isCtrlKeyPressed)
                switch (e.Key)
                {
                    case VirtualKey.N: NewCommand.Execute(null); break;
                    case VirtualKey.S: SaveGridCommand.Execute(null); break;
                }
        }

        /// <summary>
        /// Обрабатывает снятие нажатия с клавиши.
        /// </summary>
        /// <param name="sender">Объект-отправитель.</param>
        /// <param name="e">Передаваемые данные.</param>
        public void OnPageKeyUp(object sender, KeyRoutedEventArgs e)
        {
            switch (e.Key)
            {
                case VirtualKey.Control: _isCtrlKeyPressed = false; break;
                case VirtualKey.Shift: _isShiftKeyPressed = false; break;
            }
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
            else if (e.DataView.Contains(StandardDataFormats.Text))
            {
                e.Handled = true;
                string text = await e.DataView.GetTextAsync();

                Items.Add(new GridWordViewModel(new GridWordModel
                {
                    Answer = WordList[ushort.Parse(text)].Answer,
                    ID = (ushort)Items.Count,
                    Orientation = Orientation.Horizontal,
                    Question = WordList[ushort.Parse(text)].Question,
                    X = (long)(e.GetPosition(e.OriginalSource as UIElement).X / 25) * 25,
                    Y = (long)(e.GetPosition(e.OriginalSource as UIElement).Y / 25) * 25
                }));

                IsDirty = true;
            }
        }

        /// <summary>
        /// Обрабатывает начало перемещения элемента (drag'n'drop) из списка.
        /// </summary>
        /// <param name="sender">Объект-отправитель.</param>
        /// <param name="e">Передаваемые данные.</param>
        public void OnDragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            e.Data.RequestedOperation = DataPackageOperation.Copy;
            ushort id = ((ListWordViewModel)(e.Items[0])).ID;
            e.Data.SetText(id.ToString());
        }

        /// <summary>
        /// Общий метод для всех способов открытия файла сетки.
        /// </summary>
        /// <param name="file">Файл сетки, который требуется открыть.</param>
        public async void OpenGrid(StorageFile file)
        {
            if (file == null)
                return;

            List<GridWordModel> data = new List<GridWordModel>();

            try
            {
                data = new XmlService().ReadGrid(await FileIO.ReadTextAsync(file));
                if (data == null)
                    return;
            }
            catch (Exception ex)
            {
                Messages.OpenFileError(ex.Message);
                return;
            }

            if (await SaveTheChangesAsync() != null)
            {
                Items.Clear();
                GridFile = file;
                new AppHelper().AddFileToMRU(GridFile);

                ObservableCollection<GridWordViewModel> readed
                    = new ObservableCollection<GridWordViewModel>(data?.Select(e => new GridWordViewModel(e)));

                foreach (GridWordViewModel item in readed)
                    Items.Add(item);

                OpenList(GridFile, false);
            }
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Выполняет создание файла.
        /// </summary>
        private async Task NewAsync()
        {
            if (await SaveTheChangesAsync() != null)
            {
                GridFile = ListFile = null;
                IsNewEnabled = false;
                Items.Clear();
                WordList.Clear();
            }
        }

        /// <summary>
        /// Выполняет сохранение файла.
        /// </summary>
        /// <param name="file">Файл, используемый для сохранения.</param>
        private async Task SaveAsync(StorageFile file)
        {
            if (file == null)
                return;

            CachedFileManager.DeferUpdates(file);
            FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);

            try
            {
                if (status == FileUpdateStatus.Complete)
                {
                    await FileIO.WriteTextAsync(file, new XmlService().WriteGrid(Items.ToList()));
                    GridFile = file;
                }
            }
            catch (Exception ex)
            {
                Messages.SaveFileError(ex.Message);
            }
        }

        /// <summary>
        /// Сохраняет файл, создавая его.
        /// </summary>
        private async Task SaveAsAsync()
        {
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.FileTypeChoices.Add(StringHelper.ToString("CrosswordCreatorGridFile"), new List<string>() { ".cwgf" });
            savePicker.SuggestedFileName = GridFileName;

            StorageFile file = await savePicker.PickSaveFileAsync();
            await SaveAsync(file);
        }

        /// <summary>
        /// Обновляет идентификаторы всех элементов коллекции.
        /// </summary>
        private void UpdateElementsID()
        {
            for (int i = 0; i < Items.Count; i++)
                Items[i].ID = (ushort)i;
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

        /// <summary>
        /// Обрабатывает запрос к отправке данных в общий доступ.
        /// </summary>
        private void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;
            request.Data.SetStorageItems(new List<IStorageFile>() { GridFile } );

            request.Data.Properties.Title = GridFileName;
            request.Data.Properties.Description = StringHelper.ToString("CrosswordCreatorGridFile");
        }

        /// <summary>
        /// Общий метод для всех способов открытия файла списка.
        /// </summary>
        /// <param name="file">Файл списка, который требуется открыть.</param>
        /// <param name="askSelectAction">Требуется ли запрашивать выбор действия.</param>
        private async void OpenList(StorageFile file, bool askSelectAction = true)
        {
            if (file == null)
                return;

            List<ListWordModel> data = new List<ListWordModel>();

            try
            {
                data = new XmlService().ReadList(await FileIO.ReadTextAsync(file), !askSelectAction);
                if (data == null)
                    return;
            }
            catch (Exception ex)
            {
                Messages.OpenFileError(ex.Message);
                return;
            }

            if (askSelectAction && WordList.Count > 0)
            {
                MessageDialogResult result = await Messages.SelectOperationWithLoadedListFileAsync();

                switch (result)
                {
                    case MessageDialogResult.Yes:
                        WordList.Clear();
                        new AppHelper().AddFileToMRU(file); break;
                    case MessageDialogResult.No: break;
                    case MessageDialogResult.Cancel: return;
                }
            }
            else
                WordList.Clear();

            ListFile = file;

            ObservableCollection<ListWordViewModel> readed
                = new ObservableCollection<ListWordViewModel>(data?.Select(e => new ListWordViewModel(e)));

            foreach (ListWordViewModel item in readed)
                WordList.Add(item);

            for (int i = 0; i < WordList.Count; i++)
                WordList[i].ID = (ushort)i;
        }

        /// <summary>
        /// Предлагает сохранение изменений.
        /// </summary>
        private async Task<bool?> SaveTheChangesAsync()
        {
            if (IsDirty)
            {
                MessageDialogResult result = await Messages.SaveTheChangesAsync(GridFileName);

                switch (result)
                {
                    case MessageDialogResult.Yes:
                        if (GridFile == null)
                            await SaveAsAsync();
                        else
                            await SaveAsync(GridFile);
                        return true;
                    case MessageDialogResult.No: return false;
                    case MessageDialogResult.Cancel: return null;
                    default: return null;
                }
            }
            else
                return false;
        }
        #endregion
    }
}