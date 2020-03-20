using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Infinium
{
    public class Excel
    {
        string[] colNames = { "", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH" };

        private object _excel = null;
        private object _workbooks = null;
        private object _workbook = null;
        private object _sheets = null;
        private object _sheet = null;
        private object _sheetsInNewWorkbook = null;
        private object _range = null;
        private object _interior = null;
        private object _cell = null;
        private object _borders = null;
        private object _font = null;

        #region Конструктор
        /// <summary>
        /// Запуск Excel
        /// </summary>
        public Excel()
        {
            string sAppProgID = "Excel.Application";
            // Получаем ссылку на интерфейс IDispatch
            Type tExcelObj = Type.GetTypeFromProgID(sAppProgID);
            // Запускаем Excel
            _excel = Activator.CreateInstance(tExcelObj);
        }
        #endregion

        #region
        /// <summary>
        /// Закрываем приложение Excel
        /// </summary>
        public void Dispose()
        {
            if (_excel == null)
                return;
            //Если параметр DisplayAlerts = false, то при закрытии Excel не будет выскакивать окно Сохранить изменения в файле...
            _excel.GetType().InvokeMember("DisplayAlerts", BindingFlags.SetProperty, null, _excel, new object[] { false });
            _excel.GetType().InvokeMember("Quit", BindingFlags.InvokeMethod, null, _excel, null);
            Marshal.ReleaseComObject(_excel);
            _excel = null;
            GC.GetTotalMemory(true);
        }
        #endregion

        #region Отображать полосы прокрутки
        /// <summary>
        /// Видимость полос прокрутки
        /// </summary>
        public bool DisplayScrollBars
        {
            set
            {
                _excel.GetType().InvokeMember("DisplayScrollBars", BindingFlags.SetProperty, null, _excel, new object[] { value });
            }
            get
            {
                return Convert.ToBoolean(_excel.GetType().InvokeMember("DisplayScrollBars", BindingFlags.GetProperty, null, _excel, null));
            }
        }
        #endregion

        #region Отображать строку состояния
        /// <summary>
        /// Видимость строки состояния Excel
        /// </summary>
        public bool DisplayStatusBar
        {
            set
            {
                _excel.GetType().InvokeMember("DisplayStatusBar", BindingFlags.SetProperty, null, _excel, new object[] { value });
            }
            get
            {
                return Convert.ToBoolean(_excel.GetType().InvokeMember("DisplayStatusBar", BindingFlags.GetProperty, null, _excel, null));
            }
        }
        #endregion

        #region Заголовок окна
        public string Caption
        {
            set
            {
                _excel.GetType().InvokeMember("Caption", BindingFlags.SetProperty, null, _excel, new object[] { value });
            }
            get
            {
                return Convert.ToString(_excel.GetType().InvokeMember("Caption", BindingFlags.GetProperty, null, _excel, null));
            }
        }
        #endregion

        #region Видимость Excel
        /// <summary>
        /// Делаем приложение Excel видимым
        /// </summary>
        public bool Visible
        {
            set
            {
                _excel.GetType().InvokeMember("Visible", BindingFlags.SetProperty, null, _excel, new object[] { value });
            }
            get
            {
                return Convert.ToBoolean(_excel.GetType().InvokeMember("Visible", BindingFlags.GetProperty, null, _excel, null));
            }
        }
        #endregion

        #region Открыть документ
        /// <summary>
        /// Открытие документа Excel
        /// </summary>
        /// <param name="name">Имя открываемого документа</param>
        public void OpenDocument(string name)
        {
            _workbooks = _excel.GetType().InvokeMember("Workbooks", BindingFlags.GetProperty, null, _excel, null);
            _workbook = _workbooks.GetType().InvokeMember("Open", BindingFlags.InvokeMethod, null, _workbooks, new object[] { name, true });
            _sheets = _workbook.GetType().InvokeMember("Worksheets", BindingFlags.GetProperty, null, _workbook, null);
            _sheet = _sheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, _sheets, new object[] { 1 });
            // Range = WorkSheet.GetType().InvokeMember("Range",BindingFlags.GetProperty,null,WorkSheet,new object[1] { "A1" });
        }
        #endregion

        #region Новый документ
        /// <summary>
        /// Создание документа excel
        /// </summary>
        /// <param name="numberOfSheets">Количество листов в документе excel</param>
        public void NewDocument(int numberOfSheets)
        {
            //Получаем коллекции рабочих книг
            _workbooks = _excel.GetType().InvokeMember("Workbooks", BindingFlags.GetProperty, null, _excel, null);
            //Задаем количество листов в создаваемой книге
            _sheetsInNewWorkbook = _excel.GetType().InvokeMember("SheetsInNewWorkbook", BindingFlags.SetProperty, null, _excel, new object[] { numberOfSheets });
            //Добавляем новую рабочую книгу с количеством листов = NumberOfSheets
            _workbook = _workbooks.GetType().InvokeMember("Add", BindingFlags.InvokeMethod, null, _workbooks, null);
            _sheets = _workbook.GetType().InvokeMember("Worksheets", BindingFlags.GetProperty, null, _workbook, null);
            _sheet = _sheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, _sheets, new object[] { 1 });
            //_range = _sheet.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, _sheet, new object[1] { "A10" });
        }
        #endregion

        #region Закрыть документ
        /// <summary>
        /// Закрыть рабочую книгу
        /// </summary>
        public void Close()
        {
            if (_workbook == null)
                return;
            //Если параметр DisplayAlerts = false, то при закрытии Excel не будет выскакивать окно Сохранить изменения в файле...
            _excel.GetType().InvokeMember("DisplayAlerts", BindingFlags.SetProperty, null, _excel, new object[] { false });
            //Если параметр Saved = false, то рабочая книга будет закрываться сразу и не показывать окно на Сохранить как
            _workbook.GetType().InvokeMember("Saved", BindingFlags.SetProperty, null, _workbook, new object[] { false });
            _workbook.GetType().InvokeMember("Close", BindingFlags.InvokeMethod, null, _workbook, new object[] { true });

            _workbook = null;
        }
        #endregion

        #region Сохранить документ
        /// <summary>
        /// Сохранение документа
        /// </summary>
        /// <param name="name">Имя сохраняемого документа</param>
        public void SaveDocument(string name)
        {
            object[] args = new object[2];
            args[0] = name;
            args[1] = 39;

            if (File.Exists(name))
                _workbook.GetType().InvokeMember("Save", BindingFlags.InvokeMethod, null,
                    _workbook, null);
            else
                _workbook.GetType().InvokeMember("SaveAs", BindingFlags.InvokeMethod, null,
                    _workbook, args);
        }
        #endregion

        #region Добавить лист
        /// <summary>
        /// Добавление листа
        /// </summary>
        /// <param name="position">Позиция, в которую добавиться лист</param>
        /// <param name="Name">Имя листа</param>
        public void AddNewPage(int position, string Name)
        {
            //Name - Название листа
            _sheets = _workbook.GetType().InvokeMember("Worksheets", BindingFlags.GetProperty, null, _workbook, null);
            _sheet = _sheets.GetType().InvokeMember("Add", BindingFlags.GetProperty, null, _sheets, null);
            object Page = _sheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, _sheets, new object[] { position });
            Page.GetType().InvokeMember("Name", BindingFlags.SetProperty, null, Page, new object[] { Name });
        }
        #endregion

        public void SetTopMargin(int position, string Name)
        {
            //Name - Название листа
            _sheets = _workbook.GetType().InvokeMember("Worksheets", BindingFlags.GetProperty, null, _workbook, null);
            _sheet = _sheets.GetType().InvokeMember("Add", BindingFlags.GetProperty, null, _sheets, null);
            object Page = _sheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, _sheets, new object[] { position });
            Page.GetType().InvokeMember("Name", BindingFlags.SetProperty, null, Page, new object[] { Name });
        }


        #region Переименовать лист
        /// <summary>
        /// Переименование листа
        /// </summary>
        /// <param name="sheet">Номер листа, который будет переименован</param>
        /// <param name="Name">Новое имя листа</param>
        public void ReNamePage(int sheet, string Name)
        {
            //Range.Interior.ColorIndex
            object Page = _sheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, _sheets, new object[] { sheet });

            Page.GetType().InvokeMember("Name", BindingFlags.SetProperty,
                null, Page, new object[] { Name });
        }
        #endregion

        #region Шрифт ячейки

        #endregion
        public void SetFont(int sheet, int row, int column, bool bold, bool italic, bool underline, int size)
        {
            string range = colNames[column] + row;
            //Получаем ссылку на нужную страницу
            _sheet = _sheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, _sheets, new object[] { sheet });
            _range = _sheet.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, _sheet, new object[] { range });
            _font = _range.GetType().InvokeMember("Font", BindingFlags.GetProperty, null, _range, null);
            _range.GetType().InvokeMember("Size", BindingFlags.SetProperty, null, _font, new object[] { size });
            _font.GetType().InvokeMember("Bold", BindingFlags.SetProperty, null, _font, new object[] { bold });
            _font.GetType().InvokeMember("Italic", BindingFlags.SetProperty, null, _font, new object[] { italic });
            _font.GetType().InvokeMember("Underline", BindingFlags.SetProperty, null, _font, new object[] { underline });
        }

        #region Запись значения в ячейку
        /// <summary>
        /// Запись значения в ячейку с применением шрифта и выравнивания
        /// </summary>
        /// <param name="sheet">Номер рабочего листа</param>
        /// <param name="value">Записываемое значение</param>
        /// <param name="range">Адрес ячейки</param>
        /// <param name="bold">Полужирный шрифт</param>
        /// <param name="italic">Курсивный шрифт</param>
        /// <param name="underline">Подчеркнутый шрифт</param>
        /// <param name="hAlignment">Горизонтальное выравнивание в ячейке</param>
        /// <param name="vAlignment">Вертикальное выравнивание в ячейке</param>
        /// <param name="size">Размер шрифта</param>
        public void WriteCell(int sheet, object value, string range, bool bold, bool italic, bool underline,
            AlignHorizontal hAlignment, AlignVertical vAlignment, int size)
        {
            //Получаем ссылку на нужную страницу
            _sheet = _sheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, _sheets, new object[] { sheet });
            _range = _sheet.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, _sheet, new object[] { range });
            _range.GetType().InvokeMember("Value", BindingFlags.SetProperty, null, _range, new object[] { value });
            _font = _range.GetType().InvokeMember("Font", BindingFlags.GetProperty, null, _range, null);
            _range.GetType().InvokeMember("Size", BindingFlags.SetProperty, null, _font, new object[] { size });
            _font.GetType().InvokeMember("Bold", BindingFlags.SetProperty, null, _font, new object[] { bold });
            _font.GetType().InvokeMember("Italic", BindingFlags.SetProperty, null, _font, new object[] { italic });
            _font.GetType().InvokeMember("Underline", BindingFlags.SetProperty, null, _font, new object[] { underline });
            _font.GetType().InvokeMember("HorizontalAlignment", BindingFlags.SetProperty, null, _range, new object[] { hAlignment });
            _font.GetType().InvokeMember("VerticalAlignment", BindingFlags.SetProperty, null, _range, new object[] { vAlignment });
            _font.GetType().InvokeMember("Value", BindingFlags.SetProperty, null, _range, new object[] { value });

        }
        /// <summary>
        /// Запись значения в ячейку 
        /// </summary>
        /// <param name="sheet">Номер рабочего листа</param>
        /// <param name="value">Записываемое значение</param>
        /// <param name="row">Номер строки ячейки</param>
        /// <param name="column">Номер столбца ячейки</param>
        /// <param name="bold">Полужирный шрифт</param>
        /// <param name="italic">Курсивный шрифт</param>
        /// <param name="underline">Подчеркнутый шрифт</param>
        /// <param name="hAlignment">Горизонтальное выравнивание в ячейке</param>
        /// <param name="vAlignment">Вертикльное выравнивание в ячейке</param>
        /// <param name="size">Размер шрифта</param>
        public void WriteCell(int sheet, object value, int row, int column, bool bold, bool italic, bool underline,
            AlignHorizontal hAlignment, AlignVertical vAlignment, int size)
        {
            if (value == null)
                return;
            string range = colNames[column] + row;
            //Получаем ссылку на нужную страницу
            _sheet = _sheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, _sheets, new object[] { sheet });
            _range = _sheet.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, _sheet, new object[] { range });
            _range.GetType().InvokeMember("Value", BindingFlags.SetProperty, null, _range, new object[] { value });
            _font = _range.GetType().InvokeMember("Font", BindingFlags.GetProperty, null, _range, null);
            _range.GetType().InvokeMember("Size", BindingFlags.SetProperty, null, _font, new object[] { size });
            _font.GetType().InvokeMember("Bold", BindingFlags.SetProperty, null, _font, new object[] { bold });
            _font.GetType().InvokeMember("Italic", BindingFlags.SetProperty, null, _font, new object[] { italic });
            _font.GetType().InvokeMember("Underline", BindingFlags.SetProperty, null, _font, new object[] { underline });
            _font.GetType().InvokeMember("HorizontalAlignment", BindingFlags.SetProperty, null, _range, new object[] { hAlignment });
            _font.GetType().InvokeMember("VerticalAlignment", BindingFlags.SetProperty, null, _range, new object[] { vAlignment });
            _font.GetType().InvokeMember("Value", BindingFlags.SetProperty, null, _range, new object[] { value });
        }
        /// <summary>
        /// Стандартная запись значения в ячейку
        /// </summary>
        /// <param name="sheet">Номер рабочего листа</param>
        /// <param name="value">Записываемое значение</param>
        /// <param name="row">Номер строки, в кот. находится ячейка</param>
        /// <param name="column">Номер столбца</param>
        public void WriteCell(int sheet, object value, int row, int column, int size, bool bold)
        {
            if (value == null || value == DBNull.Value)
                return;

            string range = colNames[column] + row;
            //Получаем ссылку на нужную страницу
            _sheet = _sheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, _sheets, new object[] { sheet });
            _range = _sheet.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, _sheet, new object[] { range });

            _range.GetType().InvokeMember("NumberFormat", BindingFlags.SetProperty, null, _range, new object[] { "@" });
            _range.GetType().InvokeMember("Value", BindingFlags.SetProperty, null, _range, new object[] { value });
            _font = _range.GetType().InvokeMember("Font", BindingFlags.GetProperty, null, _range, null);
            _range.GetType().InvokeMember("Size", BindingFlags.SetProperty, null, _font, new object[] { size });
            _font.GetType().InvokeMember("Bold", BindingFlags.SetProperty, null, _font, new object[] { bold });
        }
        public void WriteCell(int sheet, object value, int row, int column, int size, bool bold, Color Color)
        {
            if (value == null || value == DBNull.Value)
                return;

            string range = colNames[column] + row;
            //Получаем ссылку на нужную страницу
            _sheet = _sheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, _sheets, new object[] { sheet });
            _range = _sheet.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, _sheet, new object[] { range });

            _range.GetType().InvokeMember("NumberFormat", BindingFlags.SetProperty, null, _range, new object[] { "@" });
            _range.GetType().InvokeMember("Value", BindingFlags.SetProperty, null, _range, new object[] { value });
            _font = _range.GetType().InvokeMember("Font", BindingFlags.GetProperty, null, _range, null);
            _font.GetType().InvokeMember("ColorIndex", BindingFlags.SetProperty, null, _font, new object[] { Color });
            _range.GetType().InvokeMember("Size", BindingFlags.SetProperty, null, _font, new object[] { size });
            _font.GetType().InvokeMember("Bold", BindingFlags.SetProperty, null, _font, new object[] { bold });
        }

        public void WriteCell(int sheet, object value, int row, int column, int size, bool bold, string format,
            AlignHorizontal hAlignment = AlignHorizontal.xlLeft, AlignVertical vAlignment = AlignVertical.xlCenter)
        {
            if (value == null || value == DBNull.Value)
                return;

            string range = colNames[column] + row;
            //Получаем ссылку на нужную страницу
            _sheet = _sheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, _sheets, new object[] { sheet });
            _range = _sheet.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, _sheet, new object[] { range });

            _range.GetType().InvokeMember("NumberFormat", BindingFlags.SetProperty, null, _range, new object[] { format });
            _range.GetType().InvokeMember("Value", BindingFlags.SetProperty, null, _range, new object[] { value });
            _font = _range.GetType().InvokeMember("Font", BindingFlags.GetProperty, null, _range, null);
            _font.GetType().InvokeMember("HorizontalAlignment", BindingFlags.SetProperty, null, _range, new object[] { hAlignment });
            _font.GetType().InvokeMember("VerticalAlignment", BindingFlags.SetProperty, null, _range, new object[] { vAlignment });
            _range.GetType().InvokeMember("Size", BindingFlags.SetProperty, null, _font, new object[] { size });
            _font.GetType().InvokeMember("Bold", BindingFlags.SetProperty, null, _font, new object[] { bold });
        }
        #endregion

        #region Форматированный вывод

        public void SetFormatValue(int sheet, string range, string value, string format, System.Globalization.CultureInfo culture)
        {
            _sheet = _sheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, _sheets, new object[] { sheet });
            _range = _sheet.GetType().InvokeMember("Range", BindingFlags.GetProperty,
                null, _sheet, new object[] { range });
            _range.GetType().InvokeMember("NumberFormat", BindingFlags.SetProperty, null, _range, new object[] { format }, culture);
            _range.GetType().InvokeMember("Value", BindingFlags.SetProperty, null, _range, new object[] { value });
        }
        #endregion

        #region Установка цвета фона ячейки
        /// <summary>
        /// Установка цвета фона ячейки
        /// </summary>
        /// <param name="sheet">Номер рабочего листа</param>
        /// <param name="row">Номер строки</param>
        /// <param name="column">Номер столбца</param>
        /// <param name="color">Цвет фона</param>
        /// <returns></returns>
        public bool SetColor(int sheet, int row, int column, Color color)
        {
            string range = colNames[column] + row;
            //Получаем ссылку на нужную страницу
            _sheet = _sheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, _sheets, new object[] { sheet });
            _range = _sheet.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, _sheet, new object[] { range });
            _interior = _range.GetType().InvokeMember("Interior", BindingFlags.GetProperty, null, _range, null);
            _range.GetType().InvokeMember("ColorIndex", BindingFlags.SetProperty, null, _interior, new object[] { color });
            return true;
        }
        /// <summary>
        /// Установка цвета фона ячейки
        /// </summary>
        /// <param name="sheet">Номер рабочего листа</param>
        /// <param name="cell">Адрес ячейки</param>
        /// <param name="color">Цвет фона</param>
        /// <returns></returns>
        public bool SetColor(int sheet, string cell, Color color, Pattern pattern)
        {
            //Получаем ссылку на нужную страницу
            _sheet = _sheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, _sheets, new object[] { sheet });
            _range = _sheet.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, _sheet, new object[] { cell });
            _interior = _range.GetType().InvokeMember("Interior", BindingFlags.GetProperty, null, _range, null);
            _range.GetType().InvokeMember("ColorIndex", BindingFlags.SetProperty, null, _interior, new object[] { color });
            _interior.GetType().InvokeMember("Pattern", BindingFlags.SetProperty, null, _interior, new object[] { pattern });
            return true;
        }
        #endregion

        #region Форматирование выбранного текста в ячейке
        public void SelectText(int sheet, string range, int Start, int Length, Color Color, int FontSize)
        {
            //Получаем ссылку на нужную страницу
            _sheet = _sheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, _sheets, new object[] { sheet });
            _range = _sheet.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, _sheet, new object[] { range });
            object[] args = new object[] { Start, Length };
            object Characters = _range.GetType().InvokeMember("Characters", BindingFlags.GetProperty, null, _range, args);
            object Font = Characters.GetType().InvokeMember("Font", BindingFlags.GetProperty, null, Characters, null);
            Font.GetType().InvokeMember("ColorIndex", BindingFlags.SetProperty, null, Font, new object[] { Color });
            //Font.GetType().InvokeMember("FontStyle", BindingFlags.SetProperty, null, Font, new object[] { FontStyle });
            Font.GetType().InvokeMember("Size", BindingFlags.SetProperty, null, Font, new object[] { FontSize });
        }
        #endregion

        #region Перенос слов в ячейке
        public void SetWrapText(int sheet, string range, bool Value)
        {
            //Получаем ссылку на нужную страницу
            _sheet = _sheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, _sheets, new object[] { sheet });
            _range = _sheet.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, _sheet, new object[] { range });
            _range.GetType().InvokeMember("WrapText", BindingFlags.SetProperty, null, _range, new object[] { Value });
        }

        public void SetWrapText(int sheet, int row, int column, bool Value)
        {
            string range = colNames[column] + row;
            //Получаем ссылку на нужную страницу
            _sheet = _sheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, _sheets, new object[] { sheet });
            _range = _sheet.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, _sheet, new object[] { range });
            _range.GetType().InvokeMember("WrapText", BindingFlags.SetProperty, null, _range, new object[] { Value });
        }
        #endregion

        #region Чтение значения из ячейки
        /// <summary>
        /// Чтение значения в ячейке
        /// </summary>
        /// <param name="sheet">Номер рабочего листа</param>
        /// <param name="row">Номер строки, в кот. расположена ячейка</param>
        /// <param name="column">Номер столбца</param>
        /// <returns>Значение ячейки</returns>
        public string ReadCell(int sheet, int row, int column)
        {
            _sheet = _sheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, _sheets, new object[] { sheet });
            _cell = _sheet.GetType().InvokeMember("Cells", BindingFlags.GetProperty, null, _sheet, new object[] { row, column });
            object Value = _cell.GetType().InvokeMember("Value", BindingFlags.GetProperty, null, _cell, null).ToString();
            if (Value == null)
                return "";
            return Value.ToString();
        }
        /// <summary>
        /// Чтение значения в ячейке
        /// </summary>
        /// <param name="sheet">Номер рабочего листа</param>
        /// <param name="range">Адрес ячейки</param>
        /// <returns>Значение ячейки</returns>
        public string ReadCell(int sheet, string range)
        {
            _sheet = _sheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, _sheets, new object[] { sheet });
            _range = _sheet.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, _sheet, new object[] { range });
            object Value = _range.GetType().InvokeMember("Value", BindingFlags.GetProperty, null, _range, null);
            if (Value == null)
                return "";
            return Value.ToString();
        }
        #endregion

        #region Установка ориентации страницы
        /// <summary>
        /// Установка ориентации страницы
        /// </summary>
        /// <param name="Orientation">тип ориентации страницы</param>
        public void SetOrientation(PageOrientation Orientation)
        {
            //Range.Interior.ColorIndex
            object PageSetup = _sheet.GetType().InvokeMember("PageSetup", BindingFlags.GetProperty, null, _sheet, null);

            PageSetup.GetType().InvokeMember("Orientation", BindingFlags.SetProperty, null, PageSetup, new object[] { Orientation });
        }
        #endregion

        // Вертикальный разрыв страницы
        public void PageBreakerVertical(int sheet, int row, int column)
        {
            string range = colNames[column] + row;
            //Получаем ссылку на нужную страницу
            _sheet = _sheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, _sheets, new object[] { sheet });
            _range = _sheet.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, _sheet, new object[] { range });

            object pageBreak = _sheet.GetType().InvokeMember("HPageBreaks", BindingFlags.GetProperty, null, _sheet, null);
            pageBreak.GetType().InvokeMember("Add", BindingFlags.InvokeMethod, null, pageBreak, new object[] { _range });

            pageBreak = null;
        }

        public void SetMargins(int sheet, double Left, double Top, double Right, double Bottom)
        {
            //Range.Interior.ColorIndex
            _sheet = _sheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, _sheets, new object[] { sheet });
            object PageSetup = _sheet.GetType().InvokeMember("PageSetup", BindingFlags.GetProperty, null, _sheet, null);

            PageSetup.GetType().InvokeMember("LeftMargin", BindingFlags.SetProperty, null, PageSetup, new object[] { Left });
            PageSetup.GetType().InvokeMember("RightMargin", BindingFlags.SetProperty, null, PageSetup, new object[] { Right });
            PageSetup.GetType().InvokeMember("TopMargin", BindingFlags.SetProperty, null, PageSetup, new object[] { Top });
            PageSetup.GetType().InvokeMember("BottomMargin", BindingFlags.SetProperty, null, PageSetup, new object[] { Bottom });
        }

        public void AddBarcode(Image BarCodeImage, int Row, bool Left)
        {
            System.Windows.Forms.Clipboard.SetDataObject(BarCodeImage, true);

            string range = "";

            if (Left)
                range = "A" + Row.ToString();
            else
                range = "H" + Row.ToString();

            _range = _sheet.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, _sheet, new object[] { range });

            _sheet.GetType().InvokeMember("Paste", BindingFlags.InvokeMethod, null, _sheet, new object[] { _range });



            object _shapes = _sheet.GetType().InvokeMember("Shapes", BindingFlags.GetProperty, null, _sheet, null);

            int Count = Convert.ToInt32(_shapes.GetType().InvokeMember("Count", BindingFlags.GetProperty, null, _shapes, null));

            object _shape = _shapes.GetType().InvokeMember("Item", BindingFlags.InvokeMethod, null, _shapes, new object[] { Count });



            int top = Convert.ToInt32(_shape.GetType().InvokeMember("Top", BindingFlags.GetProperty, null, _shape, null));

            int left = Convert.ToInt32(_shape.GetType().InvokeMember("Left", BindingFlags.GetProperty, null, _shape, null));

            _shape.GetType().InvokeMember("Top", BindingFlags.SetProperty, null, _shape, new object[] { top + 4 });

            if (Left == false)
                _shape.GetType().InvokeMember("Left", BindingFlags.SetProperty, null, _shape, new object[] { left - 35 });
            else
                _shape.GetType().InvokeMember("Left", BindingFlags.SetProperty, null, _shape, new object[] { left + 2 });

            _shape.GetType().InvokeMember("Height", BindingFlags.SetProperty, null, _shape, new object[] { 50 });

            _shape.GetType().InvokeMember("Width", BindingFlags.SetProperty, null, _shape, new object[] { 100 });

        }

        public void AddBarcode(Image BarCodeImage, int Row, string ColumnName)
        {
            System.Windows.Forms.Clipboard.SetDataObject(BarCodeImage, true);

            string range = "";

            range = ColumnName + Row.ToString();

            _range = _sheet.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, _sheet, new object[] { range });

            _sheet.GetType().InvokeMember("Paste", BindingFlags.InvokeMethod, null, _sheet, new object[] { _range });



            object _shapes = _sheet.GetType().InvokeMember("Shapes", BindingFlags.GetProperty, null, _sheet, null);

            int Count = Convert.ToInt32(_shapes.GetType().InvokeMember("Count", BindingFlags.GetProperty, null, _shapes, null));

            object _shape = _shapes.GetType().InvokeMember("Item", BindingFlags.InvokeMethod, null, _shapes, new object[] { Count });



            int top = Convert.ToInt32(_shape.GetType().InvokeMember("Top", BindingFlags.GetProperty, null, _shape, null));

            int left = Convert.ToInt32(_shape.GetType().InvokeMember("Left", BindingFlags.GetProperty, null, _shape, null));

            _shape.GetType().InvokeMember("Top", BindingFlags.SetProperty, null, _shape, new object[] { top + 4 });

            _shape.GetType().InvokeMember("Left", BindingFlags.SetProperty, null, _shape, new object[] { left - 17 });


            _shape.GetType().InvokeMember("Height", BindingFlags.SetProperty, null, _shape, new object[] { 50 });

            _shape.GetType().InvokeMember("Width", BindingFlags.SetProperty, null, _shape, new object[] { 110 });

        }

        public void AddBarcode(Image BarCodeImage, int Row)
        {
            System.Windows.Forms.Clipboard.SetDataObject(BarCodeImage, true);

            string range = "H" + Row;

            _range = _sheet.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, _sheet, new object[] { range });

            _sheet.GetType().InvokeMember("Paste", BindingFlags.InvokeMethod, null, _sheet, new object[] { _range });

            object _shapes = _sheet.GetType().InvokeMember("Shapes", BindingFlags.GetProperty, null, _sheet, null);

            int Count = Convert.ToInt32(_shapes.GetType().InvokeMember("Count", BindingFlags.GetProperty, null, _shapes, null));

            object _shape = _shapes.GetType().InvokeMember("Item", BindingFlags.InvokeMethod, null, _shapes, new object[] { Count });

            int left = Convert.ToInt32(_shape.GetType().InvokeMember("Left", BindingFlags.GetProperty, null, _shape, null));

            int top = Convert.ToInt32(_shape.GetType().InvokeMember("Top", BindingFlags.GetProperty, null, _shape, null));

            _shape.GetType().InvokeMember("Left", BindingFlags.SetProperty, null, _shape, new object[] { left - 45 });

            _shape.GetType().InvokeMember("Top", BindingFlags.SetProperty, null, _shape, new object[] { top - 8 });

            _shape.GetType().InvokeMember("Height", BindingFlags.SetProperty, null, _shape, new object[] { 110 });

            _shape.GetType().InvokeMember("Width", BindingFlags.SetProperty, null, _shape, new object[] { 110 });
        }

        public decimal GetRowHeight(int sheet, int row)
        {
            decimal RowHeight = 0;
            _sheet = _sheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, _sheets, new object[] { sheet });
            object _row = _sheet.GetType().InvokeMember("Rows", BindingFlags.GetProperty, null, _sheet, new object[] { row });
            RowHeight = Convert.ToDecimal(_row.GetType().InvokeMember("RowHeight", BindingFlags.GetProperty, null, _row, null));
            return RowHeight;
        }

        #region Объединение ячеек
        /// <summary>
        /// Объединение ячеек
        /// </summary>
        /// <param name="sheet">Номер рабочего листа</param>
        /// <param name="row1">Номер строки левого верхнего угла</param>
        /// <param name="column1">Номер столбца левого верхнего угла</param>
        /// <param name="row2">Номер строки правого нижнего угла</param>
        /// <param name="column2">Номер столбца правого нижнег угла</param>
        /// <param name="MergeCells">объединить</param>
        public void SetMerge(int sheet, int row1, int column1, int row2, int column2, AlignHorizontal HAlignment, AlignVertical VAlignment, bool MergeCells)
        {
            //левая верхняя ячейка
            string range1 = colNames[column1] + row1;
            //правая нижняя ячейка
            string range2 = colNames[column2] + row2;
            //Получаем ссылку на нужную страницу
            _sheet = _sheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, _sheets, new object[] { sheet });
            _range = _sheet.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, _sheet, new object[] { range1, range2 });
            object[] argsH = new object[] { HAlignment };
            _range.GetType().InvokeMember("HorizontalAlignment", BindingFlags.SetProperty, null, _range, argsH);
            object[] argsV = new object[] { VAlignment };
            _range.GetType().InvokeMember("VerticalAlignment", BindingFlags.SetProperty, null, _range, argsV);
            _range.GetType().InvokeMember("MergeCells", BindingFlags.SetProperty, null, _range, new object[] { MergeCells });
        }
        /// <summary>
        /// Объединение ячеек
        /// </summary>
        /// <param name="sheet">Номер рабочего листа</param>
        /// <param name="range">Объединяемые ячейки</param>
        /// <param name="MergeCells">объединить</param>
        public void SetMerge(int sheet, string range, bool MergeCells)
        {
            //Получаем ссылку на нужную страницу
            _sheet = _sheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, _sheets, new object[] { sheet });
            _range = _sheet.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, _sheet, new object[] { range });
            _range.GetType().InvokeMember("MergeCells", BindingFlags.SetProperty, null, _range, new object[] { MergeCells });
        }
        #endregion

        #region Возвращает ширину столбца
        /// <summary>
        /// Установка ширины столбца
        /// </summary>
        /// <param name="sheet">Номер рабочего листа</param>
        /// <param name="range">Номер столбца в буквенном обозначении, например, A1</param>
        /// <param name="width">Ширина столбца</param>
        public decimal GetColumnWidth(int sheet, string range)
        {
            decimal ColumnWidth = 0;
            //Получаем ссылку на нужную страницу
            _sheet = _sheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, _sheets, new object[] { sheet });
            _range = _sheet.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, _sheet, new object[] { range });
            ColumnWidth = Convert.ToDecimal(_range.GetType().InvokeMember("ColumnWidth", BindingFlags.GetProperty, null, _range, null));
            return ColumnWidth;
        }
        /// <summary>
        /// Установка ширины столбца
        /// </summary>
        /// <param name="sheet">Номер рабочего листа</param>
        /// <param name="column">Номер столбца по порядку</param>
        /// <param name="width">Ширина столбца</param>
        public decimal GetColumnWidth(int sheet, int column)
        {
            decimal ColumnWidth = 0;
            _sheet = _sheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, _sheets, new object[] { sheet });
            _cell = _sheet.GetType().InvokeMember("Cells", BindingFlags.GetProperty, null, _sheet, new object[] { column });
            ColumnWidth = Convert.ToDecimal(_cell.GetType().InvokeMember("ColumnWidth", BindingFlags.GetProperty, null, _cell, null));
            return ColumnWidth;
        }
        #endregion

        #region Установка ширины столбца
        /// <summary>
        /// Установка ширины столбца
        /// </summary>
        /// <param name="sheet">Номер рабочего листа</param>
        /// <param name="range">Номер столбца в буквенном обозначении, например, A1</param>
        /// <param name="width">Ширина столбца</param>
        public void SetColumnWidth(int sheet, string range, double width)
        {
            //Получаем ссылку на нужную страницу
            _sheet = _sheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, _sheets, new object[] { sheet });
            _range = _sheet.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, _sheet, new object[] { range });
            _range.GetType().InvokeMember("ColumnWidth", BindingFlags.SetProperty, null, _range, new object[] { width });
        }
        /// <summary>
        /// Установка ширины столбца
        /// </summary>
        /// <param name="sheet">Номер рабочего листа</param>
        /// <param name="column">Номер столбца по порядку</param>
        /// <param name="width">Ширина столбца</param>
        public void SetColumnWidth(int sheet, int column, double width)
        {
            _sheet = _sheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, _sheets, new object[] { sheet });
            _cell = _sheet.GetType().InvokeMember("Cells", BindingFlags.GetProperty, null, _sheet, new object[] { column });
            _cell.GetType().InvokeMember("ColumnWidth", BindingFlags.SetProperty, null, _cell, new object[] { width });
        }
        #endregion

        #region Автовыравнивание ячеек
        /// <summary>
        /// Автовыравнивание ячеек
        /// </summary>
        /// <param name="sheet">Номер рабочего листа</param>
        /// <param name="range">Номер столбца в буквенном обозначении, например, A1</param>
        public void AutoFit(int sheet, string range)
        {
            _sheet = _sheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, _sheets, new object[] { sheet });
            _range = _sheet.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, _sheet, new object[] { range });
            _range = _sheet.GetType().InvokeMember("Columns", BindingFlags.GetProperty, null, _range, null);
            _range.GetType().InvokeMember("AutoFit", BindingFlags.InvokeMethod, null, _range, null);
        }
        /// <summary>
        /// Автовыравнивание ячеек
        /// </summary>
        /// <param name="sheet">Номер рабочего листа</param>
        /// <param name="row">Номер строки, в кот. находится ячейка</param>
        /// <param name="column">Номер столбца</param>
        public void AutoFit(int sheet, int row1, int column1, int row2, int column2)
        {
            Object[] Parameters = new Object[2];
            _sheet = _sheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, _sheets, new object[] { sheet });

            Parameters[0] = _sheet.GetType().InvokeMember("Cells", BindingFlags.GetProperty, null, _sheet, new object[2] { row1, column1 });
            Parameters[1] = _sheet.GetType().InvokeMember("Cells", BindingFlags.GetProperty, null, _sheet, new object[2] { row2, column2 });

            _range = _sheet.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, _sheet, Parameters);
            _range = _sheet.GetType().InvokeMember("Columns", BindingFlags.GetProperty, null, _range, null);
            _range.GetType().InvokeMember("AutoFit", BindingFlags.InvokeMethod, null, _range, null);
        }
        #endregion

        #region Установка высоты строки
        /// <summary>
        /// Установка высоты строки
        /// </summary>
        /// <param name="sheet">Номер рабочего листа</param>
        /// <param name="row">Номер строки поп порядку</param>
        /// <param name="Height">Высота строки</param>
        public void SetRowHeight(int sheet, int row, double Height)
        {
            string range = String.Format("{0}:{0}", row);
            //Получаем ссылку на нужную страницу
            _sheet = _sheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, _sheets, new object[] { sheet });
            object _row = _range.GetType().InvokeMember("Rows", BindingFlags.GetProperty, null, _sheet, new object[] { range });
            _row.GetType().InvokeMember("RowHeight", BindingFlags.SetProperty, null, _row, new object[] { Height });
        }
        #endregion

        #region Установка границ вокруг ячейки
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheet">Номер рабочего листа</param>
        /// <param name="row">Номер строки</param>
        /// <param name="column">Номер столбца</param>
        /// <param name="left">Провести линию слева</param>
        /// <param name="right">Провести линию справа</param>
        /// <param name="top">Провести линию сверху</param>
        /// <param name="bottom">Провести линию снизу</param>
        /// <param name="linestyle">Стиль линии</param>
        /// <param name="weight">Вес линии</param>
        public void SetBorderStyle(int sheet, int row, int column, bool left, bool right, bool top, bool bottom, LineStyle linestyle, BorderWeight weight)
        {
            _sheet = _sheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, _sheets, new object[] { sheet });
            _cell = _sheet.GetType().InvokeMember("Cells", BindingFlags.GetProperty, null, _sheet, new object[] { row, column });
            if (left)
            {
                _borders = _cell.GetType().InvokeMember("Borders", BindingFlags.GetProperty, null, _cell, new object[] { BordersIndex.xlEdgeLeft });
                _borders.GetType().InvokeMember("LineStyle", BindingFlags.SetProperty, null, _borders, new object[] { linestyle });
                _borders.GetType().InvokeMember("Weight", BindingFlags.SetProperty, null, _borders, new object[] { weight });
            }
            if (top)
            {
                _borders = _cell.GetType().InvokeMember("Borders", BindingFlags.GetProperty, null, _cell, new object[] { BordersIndex.xlEdgeTop });
                _borders.GetType().InvokeMember("LineStyle", BindingFlags.SetProperty, null, _borders, new object[] { linestyle });
                _borders.GetType().InvokeMember("Weight", BindingFlags.SetProperty, null, _borders, new object[] { weight });
            }
            if (bottom)
            {
                _borders = _cell.GetType().InvokeMember("Borders", BindingFlags.GetProperty, null, _cell, new object[] { BordersIndex.xlEdgeBottom });
                _borders.GetType().InvokeMember("LineStyle", BindingFlags.SetProperty, null, _borders, new object[] { linestyle });
                _borders.GetType().InvokeMember("Weight", BindingFlags.SetProperty, null, _borders, new object[] { weight });
            }
            if (right)
            {
                _borders = _cell.GetType().InvokeMember("Borders", BindingFlags.GetProperty, null, _cell, new object[] { BordersIndex.xlEdgeRight });
                _borders.GetType().InvokeMember("LineStyle", BindingFlags.SetProperty, null, _borders, new object[] { linestyle });
                _borders.GetType().InvokeMember("Weight", BindingFlags.SetProperty, null, _borders, new object[] { weight });
            }
        }
        #endregion

        #region Выравнивания в ячейке
        ///Вертикальное
        /// <summary>
        /// Вертикальное выравнивание
        /// </summary>
        /// <param name="sheet">Номер рабочего листа</param>
        /// <param name="range">Адрес ячейки</param>
        /// <param name="Alignment">Тип выравнивания</param>
        public void SetVerticalAlignment(int sheet, string range, AlignVertical Alignment)
        {
            //Получаем ссылку на нужную страницу
            _sheet = _sheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, _sheets, new object[] { sheet });
            _range = _sheet.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, _sheet, new object[] { range });
            object[] args = new object[] { Alignment };
            _range.GetType().InvokeMember("VerticalAlignment", BindingFlags.SetProperty, null, _range, args);
        }
        /// <summary>
        /// Вертикальное выравнивание
        /// </summary>
        /// <param name="sheet">Номер рабочего листа</param>
        /// <param name="row">Номер строки ячейки</param>
        /// <param name="column">Номер столбца ячейки</param>
        /// <param name="Alignment">Тип выравнивания</param>
        public void SetVerticalAlignment(int sheet, int row, int column, AlignVertical Alignment)
        {
            string range = colNames[column] + row;
            //Получаем ссылку на нужную страницу
            _sheet = _sheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, _sheets, new object[] { sheet });
            _range = _sheet.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, _sheet, new object[] { range });
            object[] args = new object[] { Alignment };
            _range.GetType().InvokeMember("VerticalAlignment", BindingFlags.SetProperty, null, _range, args);
        }

        ///Горизонтальное
        /// <summary>
        /// Горизонтальное выравнивание
        /// </summary>
        /// <param name="sheet">Номер рабочего листа</param>
        /// <param name="range">Адрес ячейки</param>
        /// <param name="Alignment">Тип выравнивания</param>
        public void SetHorisontalAlignment(int sheet, string range, AlignHorizontal Alignment)
        {
            //Получаем ссылку на нужную страницу
            _sheet = _sheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, _sheets, new object[] { sheet });
            _range = _sheet.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, _sheet, new object[] { range });
            object[] args = new object[] { Alignment };
            _range.GetType().InvokeMember("HorizontalAlignment", BindingFlags.SetProperty, null, _range, args);
        }
        /// <summary>
        /// Горизонтальное выравнивание
        /// </summary>
        /// <param name="sheet">Номер рабочего листа</param>
        /// <param name="row">Номер строки ячейки</param>
        /// <param name="column">Номер столбца ячейки</param>
        /// <param name="Alignment">Тип выравнивания</param>
        public void SetHorisontalAlignment(int sheet, int row, int column, AlignHorizontal Alignment)
        {
            string range = colNames[column] + row;
            //Получаем ссылку на нужную страницу
            _sheet = _sheets.GetType().InvokeMember("Item", BindingFlags.GetProperty, null, _sheets, new object[] { sheet });
            _range = _sheet.GetType().InvokeMember("Range", BindingFlags.GetProperty, null, _sheet, new object[] { range });
            object[] args = new object[] { Alignment };
            _range.GetType().InvokeMember("HorizontalAlignment", BindingFlags.SetProperty, null, _range, args);
        }
        #endregion

        #region Перечисления
        // Выравнивание по горизонтали 
        public enum AlignHorizontal
        {
            xlGeneral = 1, // По значению 
            xlLeft = -4131, // По левому краю (отступ) 
            xlCenter = -4108, // По центру 
            xlRight = -4152, // По правому краю (отступ) 
            xlFill = 5, // С заполнением 
            xlJustify = -4130, // По ширине 
            xlCenterAcrossSelection = 7, // По центру выделения 
            xlDistributed = -4117 // Распределенный (отступ) 
        }

        // Выравнивание по вертикали 
        public enum AlignVertical
        {
            xlTop = 1, // По верхнему краю 
            xlCenter = -4108, // По центру 
            xlBottom = -4107, // По нижнему краю 
            xlJustify = -4130, // По высоте 
            xlDistributed = -4117 // Распределенный 
        }

        // Ориентация страницы 
        public enum PageOrientation
        {
            xlPortrait = 1, //Книжный 
            xlLandscape = 2 // Альбомный 
        }

        // Вид границы 
        public enum BordersIndex
        {
            xlDiagonalDown = 5, // Border running from the upper left-hand corner to the lower right of each cell in the range. 
            xlDiagonalUp = 6, // Border running from the lower left-hand corner to the upper right of each cell in the range. 
            xlEdgeBottom = 9, // Border at the bottom of the range. 
            xlEdgeLeft = 7, // Border at the left-hand edge of the range. 
            xlEdgeRight = 10, // Border at the right-hand edge of the range. 
            xlEdgeTop = 8, // Border at the top of the range. 
            xlInsideHorizontal = 12, // Horizontal borders for all cells in the range except borders on the outside of the range. 
            xlInsideVertical = 11 // Vertical borders for all the cells in the range except borders on the outside of the range. 
        }

        // Стиль линии для границы 
        public enum LineStyle
        {
            xlContinuous = 1, // Continuous line. 
            xlDash = -4115, // Dashed line. 
            xlDashDot = 4, // Alternating dashes and dots. 
            xlDashDotDot = 5, // Dash followed by two dots. 
            xlDot = -4118, // Dotted line. 
            xlDouble = -4119, // Double line. 
            xlLineStyleNone = -4142, // No line. 
            xlSlantDashDot = 13 // Slanted dashes. 
        }

        // Толщина линии границы 
        public enum BorderWeight
        {
            xlHairline = 1, // Hairline (thinnest border). 
            xlMedium = -4138, // Medium. 
            xlThick = 4, // Thick (widest border). 
            xlThin = 2 // Thin. 
        }

        //Заливка
        public enum Pattern
        {
            xlPatternAutomatic = -4105,
            xlPatternChecker = 9,
            xlPatternCrissCross = 16,
            xlPatternDown = -4121,
            xlPatternGray16 = 17,
            xlPatternGray25 = -4124,
            xlPatternGray50 = -4125,
            xlPatternGray75 = -4126,
            xlPatternGray8 = 18,
            xlPatternGrid = 15,
            xlPatternHorizontal = -4128,
            xlPatternLightDown = 13,
            xlPatternLightHorizontal = 11,
            xlPatternLightUp = 14,
            xlPatternLightVertical = 12,
            xlPatternNone = -4142,
            xlPatternSemiGray75 = 10,
            xlPatternSolid = 1,
            xlPatternUp = -4162,
            xlPatternVertical = -4166
        }

        //Цвет
        public enum Color
        {
            Aqua = 42,
            Black = 1,
            Blue = 5,
            BlueGray = 47,
            BrightGreen = 4,
            Brown = 53,
            Cream = 19,
            DarkBlue = 11,
            DarkGreen = 51,
            DarkPurple = 21,
            DarkRed = 9,
            DarkTeal = 49,
            DarkYellow = 12,
            Gold = 44,
            Gray25 = 15,
            Gray40 = 48,
            Gray50 = 16,
            Gray80 = 56,
            Green = 10,
            Indigo = 55,
            Lavender = 39,
            LightBlue = 41,
            LightGreen = 35,
            LightLavender = 24,
            LightOrange = 45,
            LightTurquoise = 20,
            LightYellow = 36,
            Lime = 43,
            NavyBlue = 23,
            OliveGreen = 52,
            Orange = 46,
            PaleBlue = 37,
            Pink = 7,
            Plum = 18,
            PowderBlue = 17,
            Red = 3,
            Rose = 38,
            Salmon = 22,
            SeaGreen = 50,
            SkyBlue = 33,
            Tan = 40,
            Teal = 14,
            Turquoise = 8,
            Violet = 13,
            White = 2,
            Yellow = 6
        }
        #endregion
    }

}
