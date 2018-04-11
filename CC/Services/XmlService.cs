using CC.Model;
using CC.ViewModel;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Windows.UI.Xaml.Controls;

namespace CC.Services
{
    /// <summary>
    /// Представляет службу для работы с XML.
    /// </summary>
    public class XmlService
    {
        /// <summary>
        /// Конструктор по умолчанию.
        /// </summary>
        public XmlService() { }

        /// <summary>
        /// Читает данные списка из XML.
        /// </summary>
        /// <param name="xmlData">Код XML-файла.</param>
        /// <param name="isGridWord">Вызывается ли чтение списка после чтения сетки.</param>
        public List<ListWordModel> ReadList(string xmlData, bool isGridWord = false)
        {
            List<ListWordModel> words = new List<ListWordModel>();

            XDocument xDocument = XDocument.Parse(xmlData);
            string subroot = isGridWord ? "gridWord" : "word";

            foreach (XElement xElement in xDocument.Element("head").Elements(subroot))
            {
                ListWordModel word = new ListWordModel();

                XElement id = xElement.Element(nameof(id).ToUpper());
                XElement answer = xElement.Element(nameof(answer));
                XElement question = xElement.Element(nameof(question));

                word.ID = ushort.Parse(id?.Value);
                word.Answer = answer?.Value;
                word.Question = question?.Value;

                words.Add(word);
            }

            return words;
        }

        /// <summary>
        /// Записывает данные списка в XML.
        /// </summary>
        /// <param name="items">Данные списка.</param>
        public string WriteList(List<ListWordViewModel> items)
        {
            XDocument xDoc = new XDocument();

            XElement head = new XElement(nameof(head));
            xDoc.Add(head);

            foreach (ListWordViewModel item in items)
            {
                XElement word = new XElement(nameof(word));

                XElement id = new XElement(nameof(id).ToUpper(), item.ID);
                XElement answer = new XElement(nameof(answer), item.Answer);
                XElement question = new XElement(nameof(question), item.Question);

                word.Add(id, answer, question);
                head.Add(word);
            }

            return $"<?xml version=\"1.0\" encoding=\"utf-8\"?>{Environment.NewLine}{xDoc.ToString()}";
        }

        /// <summary>
        /// Читает данные сетки из XML.
        /// </summary>
        /// <param name="xmlData">Код XML-файла.</param>
        public List<GridWordModel> ReadGrid(string xmlData)
        {
            List<GridWordModel> gridWords = new List<GridWordModel>();

            XDocument xDocument = XDocument.Parse(xmlData);

            foreach (XElement xElement in xDocument.Element("head").Elements("gridWord"))
            {
                GridWordModel gridWord = new GridWordModel();

                XElement id = xElement.Element(nameof(id).ToUpper());
                XElement x = xElement.Element(nameof(x).ToUpper());
                XElement y = xElement.Element(nameof(y).ToUpper());
                XElement orientation = xElement.Element(nameof(orientation));
                XElement answer = xElement.Element(nameof(answer));
                XElement question = xElement.Element(nameof(question));

                gridWord.ID = ushort.Parse(id?.Value);
                gridWord.X = double.Parse(x?.Value);
                gridWord.Y = double.Parse(y?.Value);

                Orientation orientationValue;
                Enum.TryParse(orientation?.Value, out orientationValue);
                gridWord.Orientation = orientationValue;

                gridWord.Question = question?.Value;
                gridWord.Answer = answer?.Value;

                gridWords.Add(gridWord);
            }

            return gridWords;
        }

        /// <summary>
        /// Читает данные сетки из XML.
        /// </summary>
        /// <param name="xmlData">Код XML-файла.</param>
        public List<FillingGridWordModel> ReadGridForFilling(string xmlData)
        {
            List<FillingGridWordModel> gridWords = new List<FillingGridWordModel>();

            XDocument xDocument = XDocument.Parse(xmlData);

            foreach (XElement xElement in xDocument.Element("head").Elements("gridWord"))
            {
                FillingGridWordModel gridWord = new FillingGridWordModel();

                XElement id = xElement.Element(nameof(id).ToUpper());
                XElement x = xElement.Element(nameof(x).ToUpper());
                XElement y = xElement.Element(nameof(y).ToUpper());
                XElement orientation = xElement.Element(nameof(orientation));
                XElement answer = xElement.Element(nameof(answer));
                XElement question = xElement.Element(nameof(question));

                gridWord.ID = ushort.Parse(id?.Value);
                gridWord.X = double.Parse(x?.Value);
                gridWord.Y = double.Parse(y?.Value);

                Orientation orientationValue;
                Enum.TryParse(orientation?.Value, out orientationValue);
                gridWord.Orientation = orientationValue;

                gridWord.Question = question?.Value;
                gridWord.Answer = new FillingAnswerModel() { Needed = answer?.Value };

                gridWords.Add(gridWord);
            }

            return gridWords;
        }

        /// <summary>
        /// Записывает данные сетки в XML.
        /// </summary>
        /// <param name="listWords">Данные сетки.</param>
        public string WriteGrid(List<GridWordViewModel> items)
        {
            XDocument xDoc = new XDocument();

            XElement head = new XElement(nameof(head));
            xDoc.Add(head);

            foreach (GridWordViewModel item in items)
            {
                XElement gridWord = new XElement(nameof(gridWord));

                XElement id = new XElement(nameof(id).ToUpper(), item.ID);
                XElement x = new XElement(nameof(x).ToUpper(), item.X);
                XElement y = new XElement(nameof(y).ToUpper(), item.Y);
                XElement orientation = new XElement(nameof(orientation), item.Orientation);
                XElement answer = new XElement(nameof(answer), item.Answer);
                XElement question = new XElement(nameof(question), item.Question);

                gridWord.Add(id, x, y, orientation, answer, question);
                head.Add(gridWord);
            }

            return $"<?xml version=\"1.0\" encoding=\"utf-8\"?>{Environment.NewLine}{xDoc.ToString()}";
        }
    }
}