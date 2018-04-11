using CC.Enums;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CC.Controls
{
    /// <summary>
    /// Представляет <see cref="TextBlock"/> с поддержкой преобразования текста элемента в заглавные или прописные символы.
    /// </summary>
    public sealed class FlexibleTextBlock : Control
    {
        private string _text;

        /// <summary>
        /// Конструктор по умолчанию.
        /// </summary>
        public FlexibleTextBlock()
        {
            DefaultStyleKey = typeof(FlexibleTextBlock);
        }

        /// <summary>
        /// Текст, отображаемый на экране.
        /// </summary>
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                SetValue(TextProperty, value);
            }
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text),
            typeof(string), typeof(FlexibleTextBlock), new PropertyMetadata(null));

        /// <summary>
        /// Преобразует регистр текста в верхний или нижний.
        /// </summary>
        public TextTransform TextTransform
        {
            get { return (TextTransform)GetValue(TextTransformProperty); }
            set
            {
                switch (value)
                {
                    case TextTransform.Lowercase: SetValue(TextProperty, Text.ToLower()); break;
                    case TextTransform.None: SetValue(TextProperty, Text); break;
                    case TextTransform.Uppercase: SetValue(TextProperty, Text.ToUpper()); break;
                }

                SetValue(TextTransformProperty, value);
            }
        }

        public static readonly DependencyProperty TextTransformProperty = DependencyProperty.Register(nameof(TextTransform),
            typeof(TextTransform), typeof(FlexibleTextBlock), new PropertyMetadata(TextTransform.None));

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }
    }
}