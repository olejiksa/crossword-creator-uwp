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
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.ViewManagement;

namespace CC.ViewModel
{
    /// <summary>
    /// Модель представления редактора списков.
    /// </summary>
    public class ListEditorViewModel : BaseViewModel
    {
        /// <summary>
        /// Имя файла по умолчанию.
        /// </summary>
        private readonly string _defaultFileName = StringHelper.ToString("UntitledList");

        #region Private fields
        private bool _isAddEnabled, _isCtrlKeyPressed, _isDirty, _isNewEnabled, _isEditVisible, _isShiftKeyPressed;
        private ContentStates _state;
        private ObservableCollection<ListWordViewModel> _items;
        private string _fileName, _currentAnswer, _currentQuestion;
        private StorageFile _currentFile;
        private object _selectedItem;
        #endregion

        /// <summary>
        /// Конструктор.
        /// </summary>
        public ListEditorViewModel()
        {
            FileName = _defaultFileName;
            Items = new ObservableCollection<ListWordViewModel>();
            Items.CollectionChanged += Items_CollectionChanged;

            _state = ContentStates.NoData;
            OnPropertyChanged(nameof(State));

            IsEditVisible = false;

            DeleteCommand = new DelegateCommand<ushort>((ushort id) =>
            {
                if (SelectedItem != null && id == ((ListWordViewModel)SelectedItem).ID)
                    CurrentQuestion = CurrentAnswer = string.Empty;

                Items.RemoveAt(id - 1);

                IsNewEnabled = (IsDirty && FileName != _defaultFileName || FileName == _defaultFileName && Items.Count > 0);

                if (Items.Count == 0)
                {
                    _state = ContentStates.NoData;
                    OnPropertyChanged(nameof(State));
                }
                else
                    UpdateElementsID();
            });
        }

        #region Commands
        /// <summary>
        /// Команда удаления из списка.
        /// </summary>
        public DelegateCommand<ushort> DeleteCommand { get; private set; }
        #endregion

        #region Properties
        /// <summary>
        /// Состояние кнопки создания нового файла.
        /// </summary>
        public bool IsNewEnabled
        {
            get { return _isNewEnabled; }
            set { Set(ref _isNewEnabled, value); }
        }

        /// <summary>
        /// Состояние кнопки добавления.
        /// </summary>
        public bool IsAddEnabled
        {
            get { return _isAddEnabled; }
            set { Set(ref _isAddEnabled, value); }
        }

        /// <summary>
        /// Определяет, отображена ли на экране кнопка добавления.
        /// </summary>
        public bool IsAddVisible { get; set; }

        /// <summary>
        /// Определяет, отображены ли на экране кнопки сохранения и удаления.
        /// Действительно к странице редактируемого элемента.
        /// </summary>
        public bool IsEditVisible
        {
            get { return _isEditVisible; }
            set
            {
                if (value)
                {
                    _isEditVisible = true;
                    IsAddVisible = !_isEditVisible;
                }
                else
                {
                    _isEditVisible = false;
                    IsAddVisible = !_isEditVisible;
                }
                OnPropertyChanged(nameof(IsEditVisible));
                OnPropertyChanged(nameof(IsAddVisible));
            }
        }

        /// <summary>
        /// Состояние страницы.
        /// </summary>
        public string State
        {
            get { return _state.ToString(); }
        }

        /// <summary>
        /// Название открытого файла.
        /// </summary>
        public string FileName
        {
            get { return _fileName; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    _fileName = ApplicationView.GetForCurrentView().Title = value;
                OnPropertyChanged(nameof(FileName));
            }
        }

        /// <summary>
        /// Текущий вопрос.
        /// </summary>
        public string CurrentQuestion
        {
            get { return _currentQuestion; }
            set
            {
                if (value != null)
                    _currentQuestion = value.Trim();
                OnPropertyChanged(nameof(CurrentQuestion));
                Check();
            }
        }

        /// <summary>
        /// Текущий ответ.
        /// </summary>
        public string CurrentAnswer
        {
            get { return _currentAnswer; }
            set
            {
                if (value != null)
                    _currentAnswer = value.Trim().ToLower();
                OnPropertyChanged(nameof(CurrentAnswer));
                Check();
            }
        }

        /// <summary>
        /// Обозреваемая коллекция слов.
        /// </summary>
        public ObservableCollection<ListWordViewModel> Items
        {
            get { return _items; }
            set
            {
                if (value != null)
                    _items = value;
                OnPropertyChanged(nameof(Items));
            }
        }

        /// <summary>
        /// Возвращает или задает значение, указывающее на наличие изменений.
        /// </summary>
        public bool IsDirty
        {
            get { return _isDirty; }
            set { Set(ref _isDirty, value); }
        }

        /// <summary>
        /// Возвращает или задает значение, отвечающее за выделенный в UI-списке элемент.
        /// </summary>
        public object SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (value == _selectedItem)
                    return;
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        /// <summary>
        /// Текущий открытый файл в приложении.
        /// <see cref="null"/> указывает на его отсутствие.
        /// </summary>
        private StorageFile CurrentFile
        {
            get { return _currentFile; }
            set
            {
                _currentFile = value;
                OnPropertyChanged(nameof(CurrentFile));
            }
        }
        #endregion

        #region Events
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
            }

            if (_isCtrlKeyPressed && _isShiftKeyPressed)
                switch (e.Key)
                {
                    case VirtualKey.S: await SaveAsButtonClick(); break;
                }
            else if (_isCtrlKeyPressed)
                switch (e.Key)
                {
                    case VirtualKey.N: NewButtonClick(); break;
                    case VirtualKey.O: OpenButtonClick(); break;
                    case VirtualKey.S: await SaveButtonClick(); break;
                }
            else if (e.Key == VirtualKey.Delete && SelectedItem != null)
            {
                ushort id = ((ListWordViewModel)SelectedItem).ID;
                DeleteCommand.Execute(id);
                if (id <= Items.Count)
                    SelectedItem = Items[id - 1];
                else if (id > 1)
                    SelectedItem = Items[id - 2];
                else
                {
                    IsEditVisible = false;
                    return;
                }

                CurrentQuestion = ((ListWordViewModel)SelectedItem).Question;
                CurrentAnswer = ((ListWordViewModel)SelectedItem).Answer;
                IsEditVisible = true;
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

                if (IsDirty && FileName != _defaultFileName
            || FileName == _defaultFileName && Items.Count > 0)
                {
                    MessageDialogResult result = await Messages.SaveTheChangesAsync(FileName);

                    switch (result)
                    {
                        case MessageDialogResult.Yes: await SaveButtonClick(); break;
                        case MessageDialogResult.No: break;
                        case MessageDialogResult.Cancel: return;
                    }
                }

                StorageFile file = (StorageFile)storageItems[0];
                FileExtension extension = new FileExtensionHelper(file.FileType).GetFileExtension;

                if (extension == FileExtension.CrosswordListFile)
                    OpenListFile(file);
                else
                    Messages.NotSupportedDroppedFile(new FileExtensionHelper(FileExtension.CrosswordListFile).GetFileType);
            }
        }

        /// <summary>
        /// Показывает контекстное меню с командой удаления.
        /// </summary>
        /// <param name="sender">Объект-отправитель.</param>
        /// <param name="parameter">Передаваемые данные.</param>
        public void OpenMenuFlyout(object sender, object parameter)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            FlyoutBase.GetAttachedFlyout(senderElement).ShowAt(senderElement);
        }

        /// <summary>
        /// Пересчитывает индексы в списке после завершения операций перемещения.
        /// </summary>
        public void DragItemsCompleted()
        {
            UpdateElementsID();
        }

        /// <summary>
        /// Обрабатывает выбор элемента в списке.
        /// </summary>
        /// <param name="sender">Объект-отправитель.</param>
        /// <param name="e">Передаваемые данные.</param>
        public void ItemClick(object sender, ItemClickEventArgs e)
        {
            IsEditVisible = true;
            CurrentQuestion = ((ListWordViewModel)e.ClickedItem).Question;
            CurrentAnswer = ((ListWordViewModel)e.ClickedItem).Answer;

            if (Locator.NavigationHelper.CurrentFrame.ActualWidth < 976 && !Locator.ShellVM.IsSplitViewPaneOpen)
            {
                SelectedItem = (ListWordViewModel)e.ClickedItem;
                Locator.NavigationHelper.CurrentFrame.Navigate(typeof(AddListWordView));
            }
        }

        /// <summary>
        /// Обрабатывает изменение размера окна.
        /// </summary>
        /// <param name="sender">Объект-отправитель.</param>
        /// <param name="e">Передаваемые данные.</param>
        public void VMSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Locator.NavigationHelper.CurrentFrame.ActualWidth >= 976
                && e.PreviousSize.Width < 976 && IsEditVisible
                && !Locator.ShellVM.IsSplitViewPaneOpen)
            {
                IsEditVisible = false;
                CurrentQuestion = CurrentAnswer = string.Empty;
            }
        }

        /// <summary>
        /// Обработчик события изменения коллекции.
        /// Устанавливает значение наличия изменений в <see cref="true"/>.
        /// </summary>
        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IsDirty = true;
        }
        #endregion

        #region Click events
        /// <summary>
        /// Вызывает добавление.
        /// </summary>
        public void AddButtonClick()
        {
            Items.Add(new ListWordViewModel(new ListWordModel
            {
                ID = (ushort)(Items.Count + 1),
                Question = CurrentQuestion,
                Answer = CurrentAnswer
            }));

            _state = ContentStates.Normal;

            CurrentQuestion = CurrentAnswer = string.Empty;
            IsAddEnabled = false;
            IsNewEnabled = true;

            if (Locator.NavigationHelper.CurrentFrame.CurrentSourcePageType == typeof(AddListWordView))
                Locator.NavigationHelper.CurrentFrame.GoBack();
            else
                OnPropertyChanged(nameof(State));
        }

        /// <summary>
        /// Вызывает редактирование.
        /// </summary>
        public void EditButtonClick()
        {
            if (Locator.NavigationHelper.CurrentFrame.ActualWidth >= 976
                || Locator.ShellVM.IsSplitViewPaneOpen)
            {
                Edit();
                OnPropertyChanged(nameof(State));
            }
            else
            {
                Edit();
                Locator.NavigationHelper.CurrentFrame.GoBack();
            }
        }

        /// <summary>
        /// Вызывает удаление.
        /// </summary>
        public void DeleteButtonClick()
        {
            DeleteCommand.Execute(((ListWordViewModel)SelectedItem).ID);
            Locator.NavigationHelper.CurrentFrame.GoBack();

            IsAddEnabled = IsEditVisible = false;
            IsNewEnabled = true;

            CurrentQuestion = CurrentAnswer = string.Empty;
            SelectedItem = null;
        }

        /// <summary>
        /// Обновляет визуальные состояния при навигации на страницу.
        /// </summary>
        public void UpdateAddingState()
        {
            if (Locator.NavigationHelper.CurrentFrame.CanGoForward)
                OnPropertyChanged(nameof(State));
        }

        /// <summary>
        /// Вызывает навигацию к странице добавления.
        /// </summary>
        public void AddNavigateButtonClick()
        {
            CurrentQuestion = CurrentAnswer = string.Empty;
            SelectedItem = null;
            IsEditVisible = false;

            Locator.NavigationHelper.CurrentFrame.Navigate(typeof(AddListWordView));
        }

        /// <summary>
        /// Вызывает создание файла.
        /// </summary>
        public async void NewButtonClick()
        {
            if (IsDirty && FileName != _defaultFileName
                || FileName == _defaultFileName && Items.Count > 0)
            {
                MessageDialogResult result = await Messages.SaveTheChangesAsync(FileName);

                switch (result)
                {
                    case MessageDialogResult.Yes: await SaveButtonClick(); break;
                    case MessageDialogResult.No: break;
                    case MessageDialogResult.Cancel: return;
                }
            }

            Items.Clear();
            IsNewEnabled = IsAddEnabled = IsEditVisible = IsDirty = false;
            FileName = _defaultFileName;
            CurrentFile = null;
            CurrentQuestion = CurrentAnswer = string.Empty;
            SelectedItem = null;

            _state = ContentStates.NoData;
            OnPropertyChanged(nameof(State));
        }

        /// <summary>
        /// Вызывает открытие файла.
        /// </summary>
        public async void OpenButtonClick()
        {
            if (IsDirty && FileName != _defaultFileName
                || FileName == _defaultFileName && Items.Count > 0)
            {
                MessageDialogResult result = await Messages.SaveTheChangesAsync(FileName);

                switch (result)
                {
                    case MessageDialogResult.Yes: await SaveButtonClick(); break;
                    case MessageDialogResult.No: break;
                    case MessageDialogResult.Cancel: return;
                }
            }

            FileOpenPicker picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".cwtf");

            CurrentFile = await picker.PickSingleFileAsync();
            if (CurrentFile != null)
                OpenListFile(CurrentFile);
        }

        /// <summary>
        /// Вызывает сохранение файла.
        /// </summary>
        public async Task SaveButtonClick()
        {
            if (CurrentFile == null)
                await SaveAsButtonClick();
            else
                Save(CurrentFile);
        }

        /// <summary>
        /// Вызывает сохранение строго вместе с созданием файла.
        /// </summary>
        public async Task SaveAsButtonClick()
        {
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.FileTypeChoices.Add(StringHelper.ToString("CrosswordCreatorListFile"), new List<string>() { ".cwtf" });
            savePicker.SuggestedFileName = FileName;

            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
                Save(file);
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Общий метод для всех способов открытия файла.
        /// </summary>
        /// <param name="file">Файл, который требуется открыть.</param>
        public async void OpenListFile(StorageFile file)
        {
            List<ListWordModel> data = new List<ListWordModel>();

            try
            {
                data = new XmlService().ReadList(await FileIO.ReadTextAsync(file));
                if (data == null)
                    return;
            }
            catch (Exception ex)
            {
                Messages.OpenFileError(ex.Message);
                return;
            }

            CurrentQuestion = CurrentAnswer = string.Empty;
            IsAddEnabled = IsEditVisible = false;
            IsNewEnabled = true;
            SelectedItem = null;

            FileName = file.DisplayName;

            _state = data?.Count > 0 ? ContentStates.Normal : ContentStates.NoData;
            OnPropertyChanged(nameof(State));

            CurrentFile = file;
            new AppHelper().AddFileToMRU(CurrentFile);

            ObservableCollection<ListWordViewModel> readed
                = new ObservableCollection<ListWordViewModel>(data?.Select(e => new ListWordViewModel(e)));
            Items.Clear();
            foreach (ListWordViewModel item in readed)
                Items.Add(item);

            IsDirty = false;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Проверяет поля на возможность добавления элемента.
        /// </summary>
        private void Check()
        {
            IsAddEnabled = (!string.IsNullOrWhiteSpace(CurrentQuestion) && !string.IsNullOrWhiteSpace(CurrentAnswer)) ? true : false;
        }

        private void Edit()
        {
            Items.Insert(((ListWordViewModel)SelectedItem).ID, new ListWordViewModel(new ListWordModel
            {
                ID = ((ListWordViewModel)SelectedItem).ID,
                Question = CurrentQuestion,
                Answer = CurrentAnswer
            }));
            Items.RemoveAt(((ListWordViewModel)SelectedItem).ID - 1);

            _state = ContentStates.Normal;

            CurrentQuestion = CurrentAnswer = string.Empty;
            IsAddEnabled = IsEditVisible = false;
            IsNewEnabled = true;
            SelectedItem = null;
        }

        /// <summary>
        /// Выполняет сохранение списка.
        /// </summary>
        /// <param name="file">Файл, используемый для сохранения.</param>
        private async void Save(StorageFile file)
        {
            if (file == null)
                return;

            CachedFileManager.DeferUpdates(file);
            FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);

            try
            {
                if (status == FileUpdateStatus.Complete)
                {
                    await FileIO.WriteTextAsync(file, new XmlService().WriteList(Items.ToList()));

                    CurrentFile = file;
                    FileName = file.DisplayName;
                    IsDirty = false;
                }
            }
            catch (Exception ex)
            {
                Messages.SaveFileError(ex.Message);
            }
        }

        /// <summary>
        /// Обновляет идентификаторы всех элементов коллекции.
        /// </summary>
        private void UpdateElementsID()
        {
            for (int i = 1; i <= Items.Count; i++)
                Items[i - 1].ID = (ushort)i;
        }
        #endregion
    }
}