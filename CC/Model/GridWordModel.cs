using Windows.UI.Xaml.Controls;

namespace CC.Model
{
    /// <summary>
    /// Класс слова сетки кроссворда.
    /// </summary>
    public sealed class GridWordModel : BaseWordModel
    {
        /// <summary>
        /// Возвращает или задает позицию слова по оси X относительно верхнего левого угла.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Возвращает или задает позицию слова по оси Y относительно верхнего левого угла.
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Возвращает или задает ориентацию слова.
        /// </summary>
        public Orientation Orientation { get; set; }

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="GridWordModel"/> с заданными значениями.
        /// </summary>
        /// <param name="answer">Ответ.</param>
        /// <param name="question">Вопрос.</param>
        /// <param name="id">Числовой идентификатор.</param>
        /// <param name="x">Позиция по оси X относительно верхнего левого угла.</param>
        /// <param name="y">Позиция по оси Y относительно верхнего левого угла.</param>
        /// <param name="orientation">Ориентация слова.</param>
        public GridWordModel(string answer, string question, ushort id,
            double x, double y, Orientation orientation)
        {
            Answer = answer;
            Question = question;
            ID = id;
            X = x;
            Y = y;
            Orientation = orientation;
        }

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="GridWordModel"/>.
        /// </summary>
        public GridWordModel() { }
    }
}