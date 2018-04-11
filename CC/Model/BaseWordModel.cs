namespace CC.Model
{
    /// <summary>
    /// Базовая абстрактная модель слова.
    /// </summary>
    public abstract class BaseWordModel
    {
        /// <summary>
        /// Возвращает или задает значение ответа.
        /// </summary>
        public string Answer { get; set; }

        /// <summary>
        /// Возвращает или задает значение вопроса.
        /// </summary>
        public string Question { get; set; }

        /// <summary>
        /// Возвращает или задает значение идентификатора.
        /// </summary>
        public ushort ID { get; set; }

        /// <summary>
        /// Возвращает значение длины <see cref="Answer"/>.
        /// </summary>
        public byte Length { get { return (byte)Answer.Length; } }
    }
}