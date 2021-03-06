﻿using CC.Common;
using CC.ViewModel;
using Windows.UI.Xaml.Controls;

namespace CC.View
{
    /// <summary>
    /// Представляет страницу для печати.
    /// </summary>
    public sealed partial class PrintGridView : Page, IBindableView<FillingViewModel>
    {
        /// <summary>
        /// Конструктор по умолчанию.
        /// </summary>
        public PrintGridView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Модель представления.
        /// </summary>
        public FillingViewModel ViewModel
        {
            get { return Locator.FillingVM; }
            set
            {
                Locator.FillingVM = value;
            }
        }
    }
}