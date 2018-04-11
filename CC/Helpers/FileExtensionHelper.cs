using CC.Enums;

namespace CC.Helpers
{
    /// <summary>
    /// Класс-помощник для работы с расширением файла.
    /// </summary>
    public class FileExtensionHelper
    {
        private string _fileExtension;
        private FileExtension _fileType;

        /// <summary>
        /// Констркутор для получения объекта перечисления.
        /// </summary>
        /// <param name="fileExtension">Полученное расширение файла.</param>
        public FileExtensionHelper(string fileExtension)
        {
            _fileExtension = fileExtension;
        }

        /// <summary>
        /// Констркутор для получения строки.
        /// </summary>
        /// <param name="fileExtension">Полученное расширение файла.</param>
        public FileExtensionHelper(FileExtension fileType)
        {
            _fileType = fileType;
        }

        /// <summary>
        /// Возвращает расширение файла объектом перечисления.
        /// </summary>
        public FileExtension GetFileExtension
        {
            get
            {
                switch (_fileExtension)
                {
                    case ".cwtf": return FileExtension.CrosswordListFile;
                    case ".cwgf": return FileExtension.CrosswordGridFile;
                    default: return FileExtension.NotSupportedFileExtension;
                }
            }
        }

        /// <summary>
        /// Возвращает расширение файла строкой.
        /// </summary>
        public string GetFileType
        {
            get
            {
                switch (_fileType)
                {
                    case FileExtension.CrosswordListFile: return StringHelper.ToString("CrosswordListFile");
                    case FileExtension.CrosswordGridFile: return StringHelper.ToString("CrosswordGridFile");
                    default: return string.Empty;
                }
            }
        }
    }
}