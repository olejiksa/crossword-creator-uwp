using CC.ViewModel;
using System;

namespace CC.Model
{
    /// <summary>
    /// Представляет элемент меню.
    /// </summary>
    public class MenuItem : BaseViewModel
    {
        private string icon;
        private string title;
        private Type pageType;

        /// <summary>
        /// Иконка.
        /// </summary>
        public string Icon
        {
            get { return icon; }
            set { Set(ref icon, value); }
        }

        /// <summary>
        /// Название страницы.
        /// </summary>
        public string Title
        {
            get { return title; }
            set { Set(ref title, value); }
        }

        /// <summary>
        /// Страница.
        /// </summary>
        public Type PageType
        {
            get { return pageType; }
            set { Set(ref pageType, value); }
        }
    }
}