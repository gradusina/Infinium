using System;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.HPSF;
using System.Xml;
using System.Net;

namespace Infinium
{
    public class AdminModulesJournalToExcel
    {
        HSSFWorkbook hssfworkbook;

        HSSFFont HeaderF;
        HSSFFont SimpleF;
        HSSFCellStyle MainCS;
        HSSFCellStyle HeaderCS;

        HSSFCellStyle SimpleCS;
        HSSFCellStyle SimpleLCS;
        HSSFCellStyle SimpleLBCS;
        HSSFCellStyle SimpleRCS;
        HSSFCellStyle SimpleRBCS;
        HSSFCellStyle SimpleBCS;

        public AdminModulesJournalToExcel()
        {
            Create();
        }

        private void Create()
        {
            hssfworkbook = new HSSFWorkbook();

            ////create a entry of DocumentSummaryInformation
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "NPOI Team";
            hssfworkbook.DocumentSummaryInformation = dsi;

            ////create a entry of SummaryInformation
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NPOI SDK Example";
            hssfworkbook.SummaryInformation = si;

            HeaderF = hssfworkbook.CreateFont();
            HeaderF.FontHeightInPoints = 12;
            HeaderF.Boldweight = 10 * 256;
            HeaderF.FontName = "Times New Roman";

            SimpleF = hssfworkbook.CreateFont();
            SimpleF.FontHeightInPoints = 11;
            SimpleF.FontName = "Times New Roman";

            MainCS = hssfworkbook.CreateCellStyle();
            MainCS.SetFont(HeaderF);

            SimpleCS = hssfworkbook.CreateCellStyle();
            SimpleCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            SimpleCS.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SimpleCS.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            SimpleCS.RightBorderColor = HSSFColor.BLACK.index;
            SimpleCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            SimpleCS.TopBorderColor = HSSFColor.BLACK.index;
            SimpleCS.SetFont(SimpleF);

            SimpleLCS = hssfworkbook.CreateCellStyle();
            SimpleLCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            SimpleLCS.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleLCS.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            SimpleLCS.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleLCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            SimpleLCS.RightBorderColor = HSSFColor.BLACK.index;
            SimpleLCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            SimpleLCS.TopBorderColor = HSSFColor.BLACK.index;
            SimpleLCS.SetFont(SimpleF);

            SimpleLBCS = hssfworkbook.CreateCellStyle();
            SimpleLBCS.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            SimpleLBCS.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleLBCS.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            SimpleLBCS.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleLBCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            SimpleLBCS.RightBorderColor = HSSFColor.BLACK.index;
            SimpleLBCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            SimpleLBCS.TopBorderColor = HSSFColor.BLACK.index;
            SimpleLBCS.SetFont(SimpleF);

            SimpleRCS = hssfworkbook.CreateCellStyle();
            SimpleRCS.BorderBottom = HSSFCellStyle.BORDER_THIN;
            SimpleRCS.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleRCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SimpleRCS.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleRCS.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            SimpleRCS.RightBorderColor = HSSFColor.BLACK.index;
            SimpleRCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            SimpleRCS.TopBorderColor = HSSFColor.BLACK.index;
            SimpleRCS.SetFont(SimpleF);

            SimpleRBCS = hssfworkbook.CreateCellStyle();
            SimpleRBCS.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            SimpleRBCS.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleRBCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SimpleRBCS.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleRBCS.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            SimpleRBCS.RightBorderColor = HSSFColor.BLACK.index;
            SimpleRBCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            SimpleRBCS.TopBorderColor = HSSFColor.BLACK.index;
            SimpleRBCS.SetFont(SimpleF);

            SimpleBCS = hssfworkbook.CreateCellStyle();
            SimpleBCS.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            SimpleBCS.BottomBorderColor = HSSFColor.BLACK.index;
            SimpleBCS.BorderLeft = HSSFCellStyle.BORDER_THIN;
            SimpleBCS.LeftBorderColor = HSSFColor.BLACK.index;
            SimpleBCS.BorderRight = HSSFCellStyle.BORDER_THIN;
            SimpleBCS.RightBorderColor = HSSFColor.BLACK.index;
            SimpleBCS.BorderTop = HSSFCellStyle.BORDER_THIN;
            SimpleBCS.TopBorderColor = HSSFColor.BLACK.index;
            SimpleBCS.SetFont(SimpleF);

            HeaderCS = hssfworkbook.CreateCellStyle();
            HeaderCS.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            HeaderCS.BottomBorderColor = HSSFColor.BLACK.index;
            HeaderCS.BorderLeft = HSSFCellStyle.BORDER_MEDIUM;
            HeaderCS.LeftBorderColor = HSSFColor.BLACK.index;
            HeaderCS.BorderRight = HSSFCellStyle.BORDER_MEDIUM;
            HeaderCS.RightBorderColor = HSSFColor.BLACK.index;
            HeaderCS.BorderTop = HSSFCellStyle.BORDER_MEDIUM;
            HeaderCS.TopBorderColor = HSSFColor.BLACK.index;
            HeaderCS.SetFont(HeaderF);
        }

        public void SaveOpenReport(DateTime DateFrom, DateTime DateTo)
        {
            string FileName = "Журнал посещения модулей Infinium " + DateFrom.ToString("dd.MM.yyyy");
            if (DateFrom != DateTo)
                FileName = "Журнал посещения модулей Infinium за период с " + DateFrom.ToString("dd.MM.yyyy") + " по " + DateTo.ToString("dd.MM.yyyy");

            string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");
            FileInfo file = new FileInfo(tempFolder + @"\" + FileName + ".xls");
            int j = 1;
            while (file.Exists == true)
            {
                file = new FileInfo(tempFolder + @"\" + FileName + "(" + j++ + ").xls");
            }

            FileStream NewFile = new FileStream(file.FullName, FileMode.Create);
            hssfworkbook.Write(NewFile);
            NewFile.Close();

            System.Diagnostics.Process.Start(file.FullName);
        }

        public void CreateSheet1(DataTable DT1)
        {
            if (DT1.Rows.Count == 0)
                return;
            int DisplayIndex = 0;
            int pos = 1;

            HSSFSheet sheet1;
            sheet1 = hssfworkbook.CreateSheet("Журнал входа в Infinium");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            sheet1.SetColumnWidth(DisplayIndex++, 15 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 15 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 20 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 45 * 256);

            DisplayIndex = 0;

            HSSFCell Cell1;

            Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Дата входа");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Дата выхода");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Пользователь");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Модуль");
            Cell1.CellStyle = HeaderCS;

            pos++;

            for (int i = 0; i < DT1.Rows.Count; i++)
            {
                DisplayIndex = 0;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                if (DT1.Rows[i]["DateEnter"] != DBNull.Value)
                    Cell1.SetCellValue(Convert.ToDateTime(DT1.Rows[i]["DateEnter"]).ToString("dd.MM.yyyy HH:mm"));
                Cell1.CellStyle = SimpleCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                if (DT1.Rows[i]["DateExit"] != DBNull.Value)
                    Cell1.SetCellValue(Convert.ToDateTime(DT1.Rows[i]["DateExit"]).ToString("dd.MM.yyyy HH:mm"));
                Cell1.CellStyle = SimpleCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue(DT1.Rows[i]["UserName"].ToString());
                Cell1.CellStyle = SimpleCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue(DT1.Rows[i]["ModuleName"].ToString());
                Cell1.CellStyle = SimpleCS;

                pos++;
            }

            //int DispInd = DisplayIndex;
            //DispInd++;
            //DispInd++;
            //pos = 1;

            //Cell1 = sheet1.CreateRow(pos++).CreateCell(DispInd);
            //Cell1.SetCellValue("Сводка по пользователям:");
            //Cell1.CellStyle = MainCS;

            //Cell1 = sheet1.CreateRow(pos).CreateCell(DispInd++);
            //Cell1.SetCellValue("Пользователь");
            //Cell1.CellStyle = HeaderCS;

            //Cell1 = sheet1.CreateRow(pos).CreateCell(DispInd++);
            //Cell1.SetCellValue("Модуль");
            //Cell1.CellStyle = HeaderCS;

            //Cell1 = sheet1.CreateRow(pos).CreateCell(DispInd++);
            //Cell1.SetCellValue("Рейтинг");
            //Cell1.CellStyle = HeaderCS;

            //pos++;

            //for (int i = 0; i < DT2.Rows.Count; i++)
            //{
            //    DispInd = DisplayIndex;
            //    DispInd++;
            //    DispInd++;

            //    Cell1 = sheet1.CreateRow(pos).CreateCell(DispInd++);
            //    Cell1.SetCellValue(DT2.Rows[i]["UserName"].ToString());
            //    if (DT2.Rows.Count - 1 != i && DT2.Rows[i]["UserName"].ToString() == DT2.Rows[i + 1]["UserName"].ToString())
            //        Cell1.CellStyle = SimpleLCS;
            //    else
            //        Cell1.CellStyle = SimpleLBCS;

            //    Cell1 = sheet1.CreateRow(pos).CreateCell(DispInd++);
            //    Cell1.SetCellValue(DT2.Rows[i]["ModuleName"].ToString());
            //    if (DT2.Rows.Count - 1 != i && DT2.Rows[i]["UserName"].ToString() == DT2.Rows[i + 1]["UserName"].ToString())
            //        Cell1.CellStyle = SimpleCS;
            //    else
            //        Cell1.CellStyle = SimpleBCS;

            //    Cell1 = sheet1.CreateRow(pos).CreateCell(DispInd++);
            //    Cell1.SetCellValue(Convert.ToInt32(DT2.Rows[i]["Rating"]));
            //    if (DT2.Rows.Count - 1 != i && DT2.Rows[i]["UserName"].ToString() == DT2.Rows[i + 1]["UserName"].ToString())
            //        Cell1.CellStyle = SimpleRCS;
            //    else
            //        Cell1.CellStyle = SimpleRBCS;

            //    pos++;
            //}

            //DispInd = DisplayIndex;
            //DispInd++;
            //DispInd++;
            //pos++;

            //Cell1 = sheet1.CreateRow(pos++).CreateCell(DispInd);
            //Cell1.SetCellValue("Сводка по модулям:");
            //Cell1.CellStyle = MainCS;

            //Cell1 = sheet1.CreateRow(pos).CreateCell(DispInd++);
            //Cell1.SetCellValue("Модуль");
            //Cell1.CellStyle = HeaderCS;

            //Cell1 = sheet1.CreateRow(pos).CreateCell(DispInd++);
            //Cell1.SetCellValue("Пользователь");
            //Cell1.CellStyle = HeaderCS;

            //Cell1 = sheet1.CreateRow(pos).CreateCell(DispInd++);
            //Cell1.SetCellValue("Рейтинг");
            //Cell1.CellStyle = HeaderCS;

            //pos++;

            //for (int i = 0; i < DT3.Rows.Count; i++)
            //{
            //    DispInd = DisplayIndex;
            //    DispInd++;
            //    DispInd++;

            //    Cell1 = sheet1.CreateRow(pos).CreateCell(DispInd++);
            //    Cell1.SetCellValue(DT3.Rows[i]["ModuleName"].ToString());
            //    if (DT3.Rows.Count - 1 != i && DT3.Rows[i]["ModuleName"].ToString() == DT3.Rows[i + 1]["ModuleName"].ToString())
            //        Cell1.CellStyle = SimpleLCS;
            //    else
            //        Cell1.CellStyle = SimpleLBCS;

            //    Cell1 = sheet1.CreateRow(pos).CreateCell(DispInd++);
            //    Cell1.SetCellValue(DT3.Rows[i]["UserName"].ToString());
            //    if (DT3.Rows.Count - 1 != i && DT3.Rows[i]["ModuleName"].ToString() == DT3.Rows[i + 1]["ModuleName"].ToString())
            //        Cell1.CellStyle = SimpleCS;
            //    else
            //        Cell1.CellStyle = SimpleBCS;

            //    Cell1 = sheet1.CreateRow(pos).CreateCell(DispInd++);
            //    Cell1.SetCellValue(Convert.ToInt32(DT3.Rows[i]["Rating"]));
            //    if (DT3.Rows.Count - 1 != i && DT3.Rows[i]["ModuleName"].ToString() == DT3.Rows[i + 1]["ModuleName"].ToString())
            //        Cell1.CellStyle = SimpleRCS;
            //    else
            //        Cell1.CellStyle = SimpleRBCS;

            //    pos++;
            //}
        }

        public void CreateSheet2(DataTable DT1, DataTable DT2)
        {
            if (DT1.Rows.Count == 0)
                return;
            int DisplayIndex = 0;
            int pos = 1;

            HSSFSheet sheet1;
            sheet1 = hssfworkbook.CreateSheet("Сводка по пользователям");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            sheet1.SetColumnWidth(DisplayIndex++, 20 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 25 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 19 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 15 * 256);
            DisplayIndex++;
            DisplayIndex++;
            sheet1.SetColumnWidth(DisplayIndex++, 20 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 45 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 10 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 15 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 15 * 256);

            DisplayIndex = 0;

            HSSFCell Cell1;

            Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Пользователь");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Компьютер");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Среднее время");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Общее время");
            Cell1.CellStyle = HeaderCS;

            pos++;

            for (int i = 0; i < DT1.Rows.Count; i++)
            {
                DisplayIndex = 0;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue(DT1.Rows[i]["UserName"].ToString());
                Cell1.CellStyle = SimpleCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue(DT1.Rows[i]["ComputerName"].ToString());
                Cell1.CellStyle = SimpleCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue(DT1.Rows[i]["AVGTime"].ToString());
                Cell1.CellStyle = SimpleCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue(DT1.Rows[i]["TotalTime"].ToString());
                Cell1.CellStyle = SimpleCS;

                pos++;
            }

            int DispInd = DisplayIndex;
            DispInd++;
            DispInd++;
            pos = 1;

            Cell1 = sheet1.CreateRow(pos).CreateCell(DispInd++);
            Cell1.SetCellValue("Пользователь");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet1.CreateRow(pos).CreateCell(DispInd++);
            Cell1.SetCellValue("Модуль");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet1.CreateRow(pos).CreateCell(DispInd++);
            Cell1.SetCellValue("Рейтинг");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet1.CreateRow(pos).CreateCell(DispInd++);
            Cell1.SetCellValue("Общее время");
            Cell1.CellStyle = HeaderCS;

            pos++;

            for (int i = 0; i < DT2.Rows.Count; i++)
            {
                DispInd = DisplayIndex;
                DispInd++;
                DispInd++;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DispInd++);
                Cell1.SetCellValue(DT2.Rows[i]["UserName"].ToString());
                if (DT2.Rows.Count - 1 != i && DT2.Rows[i]["UserName"].ToString() == DT2.Rows[i + 1]["UserName"].ToString())
                    Cell1.CellStyle = SimpleLCS;
                else
                    Cell1.CellStyle = SimpleLBCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DispInd++);
                Cell1.SetCellValue(DT2.Rows[i]["ModuleName"].ToString());
                if (DT2.Rows.Count - 1 != i && DT2.Rows[i]["UserName"].ToString() == DT2.Rows[i + 1]["UserName"].ToString())
                    Cell1.CellStyle = SimpleCS;
                else
                    Cell1.CellStyle = SimpleBCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DispInd++);
                Cell1.SetCellValue(Convert.ToInt32(DT2.Rows[i]["Rating"]));
                if (DT2.Rows.Count - 1 != i && DT2.Rows[i]["UserName"].ToString() == DT2.Rows[i + 1]["UserName"].ToString())
                    Cell1.CellStyle = SimpleRCS;
                else
                    Cell1.CellStyle = SimpleRBCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DispInd++);
                Cell1.SetCellValue(DT2.Rows[i]["TotalTime"].ToString());
                if (DT2.Rows.Count - 1 != i && DT2.Rows[i]["UserName"].ToString() == DT2.Rows[i + 1]["UserName"].ToString())
                    Cell1.CellStyle = SimpleCS;
                else
                {
                    Cell1.CellStyle = SimpleBCS;
                    Cell1 = sheet1.CreateRow(pos).CreateCell(DispInd++);
                    Cell1.SetCellValue(DT2.Rows[i]["TotalMinutes"].ToString());
                    Cell1.CellStyle = SimpleBCS;
                }

                pos++;
            }
        }

        public void CreateSheet3(DataTable DT1, DataTable DT2)
        {
            if (DT1.Rows.Count == 0)
                return;
            int DisplayIndex = 0;
            int pos = 1;

            HSSFSheet sheet1;
            sheet1 = hssfworkbook.CreateSheet("Сводка по модулям");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            sheet1.SetColumnWidth(DisplayIndex++, 45 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 20 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 10 * 256);
            DisplayIndex++;
            DisplayIndex++;
            sheet1.SetColumnWidth(DisplayIndex++, 45 * 256);
            sheet1.SetColumnWidth(DisplayIndex++, 10 * 256);

            DisplayIndex = 0;

            HSSFCell Cell1;

            Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Модуль");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Пользователь");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
            Cell1.SetCellValue("Рейтинг");
            Cell1.CellStyle = HeaderCS;

            pos++;

            for (int i = 0; i < DT1.Rows.Count; i++)
            {
                DisplayIndex = 0;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue(DT1.Rows[i]["ModuleName"].ToString());
                if (DT1.Rows.Count - 1 != i && DT1.Rows[i]["ModuleName"].ToString() == DT1.Rows[i + 1]["ModuleName"].ToString())
                    Cell1.CellStyle = SimpleLCS;
                else
                    Cell1.CellStyle = SimpleLBCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue(DT1.Rows[i]["UserName"].ToString());
                if (DT1.Rows.Count - 1 != i && DT1.Rows[i]["ModuleName"].ToString() == DT1.Rows[i + 1]["ModuleName"].ToString())
                    Cell1.CellStyle = SimpleCS;
                else
                    Cell1.CellStyle = SimpleBCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DisplayIndex++);
                Cell1.SetCellValue(Convert.ToInt32(DT1.Rows[i]["Rating"]));
                if (DT1.Rows.Count - 1 != i && DT1.Rows[i]["ModuleName"].ToString() == DT1.Rows[i + 1]["ModuleName"].ToString())
                    Cell1.CellStyle = SimpleRCS;
                else
                    Cell1.CellStyle = SimpleRBCS;

                pos++;
            }

            int DispInd = DisplayIndex;
            DispInd++;
            DispInd++;
            pos = 1;

            Cell1 = sheet1.CreateRow(pos).CreateCell(DispInd++);
            Cell1.SetCellValue("Модуль");
            Cell1.CellStyle = HeaderCS;

            Cell1 = sheet1.CreateRow(pos).CreateCell(DispInd++);
            Cell1.SetCellValue("Рейтинг");
            Cell1.CellStyle = HeaderCS;

            pos++;

            for (int i = 0; i < DT2.Rows.Count; i++)
            {
                DispInd = DisplayIndex;
                DispInd++;
                DispInd++;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DispInd++);
                Cell1.SetCellValue(DT2.Rows[i]["ModuleName"].ToString());
                Cell1.CellStyle = SimpleCS;

                Cell1 = sheet1.CreateRow(pos).CreateCell(DispInd++);
                Cell1.SetCellValue(Convert.ToInt32(DT2.Rows[i]["TotalCount"]));
                Cell1.CellStyle = SimpleCS;

                pos++;
            }
        }

    }

    public class AdminModulesJournal
    {
        DataTable ComputerParamsDataTable;
        DataTable ResultUsersTimeDataTable;
        DataTable TotalResultUsersDataTable;
        DataTable ResultModulesDataTable;
        DataTable TotalResultModulesDataTable;
        DataTable ModulesDataTable;
        DataTable LoginJournalDataTable;
        DataTable ModulesJournalDataTable;
        DataTable UsersDataTable;
        public DataTable ChartDataTable;

        PercentageDataGrid JournalDataGrid;

        public BindingSource ResultUsersTimeBingingSource = null;
        public BindingSource TotalResultUsersBingingSource = null;
        public BindingSource ResultModulesBingingSource = null;
        public BindingSource TotalResultModulesBingingSource = null;
        public BindingSource UsersBingingSource = null;
        public BindingSource ModulesJournalBingingSource = null;
        public BindingSource ModulesBingingSource = null;


        private DataGridViewComboBoxColumn UserColumn = null;
        private DataGridViewComboBoxColumn ModuleColumn = null;

        public AdminModulesJournal(ref PercentageDataGrid tJournalDataGrid)
        {
            JournalDataGrid = tJournalDataGrid;

            Initialize();
        }

        public DataTable ResultUsersTimeDT
        {
            get
            {
                DataTable DT = ResultUsersTimeDataTable.Clone();
                using (DataView DV = new DataView(ResultUsersTimeDataTable.Copy()))
                {
                    DV.Sort = "UserName, AVGTime DESC";
                    DT.Clear();
                    DT = DV.ToTable();
                }
                return DT;
            }
        }

        public DataTable TotalResultUsersDT
        {
            get
            {
                DataTable DT = TotalResultUsersDataTable.Clone();
                using (DataView DV = new DataView(TotalResultUsersDataTable.Copy()))
                {
                    DV.Sort = "UserName, Rating DESC, ModuleName";
                    DT.Clear();
                    DT = DV.ToTable();
                }
                return DT;
            }
        }

        public DataTable ResultModulesDT
        {
            get
            {
                DataTable DT = ResultModulesDataTable.Clone();
                using (DataView DV = new DataView(ResultModulesDataTable.Copy()))
                {
                    DV.Sort = "ModuleName, Rating DESC, UserName";
                    DT.Clear();
                    DT = DV.ToTable();
                }
                return DT;
            }
        }

        public DataTable TotalResultModulesDT
        {
            get
            {
                DataTable DT = TotalResultModulesDataTable.Clone();
                using (DataView DV = new DataView(TotalResultModulesDataTable.Copy()))
                {
                    DV.Sort = "TotalCount DESC, ModuleName";
                    DT.Clear();
                    DT = DV.ToTable();
                }
                return DT;
            }
        }

        public DataTable ModulesJournalDT
        {
            get
            {
                DataTable DT = ModulesJournalDataTable.Copy();
                DT.Columns.Add(new DataColumn("UserName", Type.GetType("System.String")));
                DT.Columns.Add(new DataColumn("ModuleName", Type.GetType("System.String")));

                for (int j = 0; j < DT.Rows.Count; j++)
                {
                    DT.Rows[j]["UserName"] = GetUserName(Convert.ToInt32(DT.Rows[j]["UserID"]));
                    DT.Rows[j]["ModuleName"] = GetModuleName(Convert.ToInt32(DT.Rows[j]["ModuleID"]));
                }

                return DT;
            }
        }

        private void Create()
        {
            LoginJournalDataTable = new DataTable();
            ModulesJournalDataTable = new DataTable();
            UsersDataTable = new DataTable();
            ModulesDataTable = new DataTable();

            ResultUsersTimeDataTable = new DataTable();
            ResultUsersTimeDataTable.Columns.Add(new DataColumn("UserName", Type.GetType("System.String")));
            ResultUsersTimeDataTable.Columns.Add(new DataColumn("ComputerName", Type.GetType("System.String")));
            ResultUsersTimeDataTable.Columns.Add(new DataColumn("AVGTime", Type.GetType("System.String")));
            ResultUsersTimeDataTable.Columns.Add(new DataColumn("TotalTime", Type.GetType("System.String")));

            TotalResultUsersDataTable = new DataTable();
            TotalResultUsersDataTable.Columns.Add(new DataColumn("UserName", Type.GetType("System.String")));
            TotalResultUsersDataTable.Columns.Add(new DataColumn("ModuleName", Type.GetType("System.String")));
            TotalResultUsersDataTable.Columns.Add(new DataColumn("Rating", Type.GetType("System.Int32")));
            TotalResultUsersDataTable.Columns.Add(new DataColumn("TotalTime", Type.GetType("System.String")));
            TotalResultUsersDataTable.Columns.Add(new DataColumn("TotalMinutes", Type.GetType("System.String")));

            ResultModulesDataTable = new DataTable();
            ResultModulesDataTable.Columns.Add(new DataColumn("ModuleName", Type.GetType("System.String")));
            ResultModulesDataTable.Columns.Add(new DataColumn("UserName", Type.GetType("System.String")));
            ResultModulesDataTable.Columns.Add(new DataColumn("Rating", Type.GetType("System.Int32")));

            TotalResultModulesDataTable = new DataTable();
            TotalResultModulesDataTable.Columns.Add(new DataColumn("ModuleName", Type.GetType("System.String")));
            TotalResultModulesDataTable.Columns.Add(new DataColumn("TotalCount", Type.GetType("System.Int32")));

            ChartDataTable = new DataTable();
            ChartDataTable.Columns.Add(new DataColumn("Hour", Type.GetType("System.String")));
            ChartDataTable.Columns.Add(new DataColumn("Percent", Type.GetType("System.Decimal")));

            ComputerParamsDataTable = new DataTable()
            {
                TableName = "ComputerParams"
            };
            ComputerParamsDataTable.Columns.Add(new DataColumn("Domain", Type.GetType("System.String")));
            ComputerParamsDataTable.Columns.Add(new DataColumn("ComputerName", Type.GetType("System.String")));
            ComputerParamsDataTable.Columns.Add(new DataColumn("LoginName", Type.GetType("System.String")));
            ComputerParamsDataTable.Columns.Add(new DataColumn("Manufacturer", Type.GetType("System.String")));
            ComputerParamsDataTable.Columns.Add(new DataColumn("Model", Type.GetType("System.String")));
            ComputerParamsDataTable.Columns.Add(new DataColumn("ProcessorName", Type.GetType("System.String")));
            ComputerParamsDataTable.Columns.Add(new DataColumn("ProcessorCores", Type.GetType("System.String")));
            ComputerParamsDataTable.Columns.Add(new DataColumn("TotalRAM", Type.GetType("System.String")));
            ComputerParamsDataTable.Columns.Add(new DataColumn("OSName", Type.GetType("System.String")));
            ComputerParamsDataTable.Columns.Add(new DataColumn("OSVersion", Type.GetType("System.String")));
            ComputerParamsDataTable.Columns.Add(new DataColumn("OSPlatform", Type.GetType("System.String")));

        }

        private void Fill()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ModuleID, ModuleName FROM Modules ORDER BY ModuleName ASC", ConnectionStrings.UsersConnectionString))
            {
                DA.Fill(ModulesDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT UserID, Name, ShortName FROM USERS ORDER BY Name ASC", ConnectionStrings.UsersConnectionString))
            {
                DA.Fill(UsersDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM LoginJournal", ConnectionStrings.UsersConnectionString))
            {
                DA.Fill(LoginJournalDataTable);
                LoginJournalDataTable.Columns.Add(new DataColumn("ComputerName", Type.GetType("System.String")));
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM ModulesJournal", ConnectionStrings.UsersConnectionString))
            {
                DA.Fill(ModulesJournalDataTable);
                ModulesJournalDataTable.Columns.Add(new DataColumn("TotalMinutes", Type.GetType("System.Int32")));
            }
        }

        private void GetHours(int UserID, DateTime DateFrom, DateTime DateTo)
        {
            DataTable CountDataTable = new DataTable();
            CountDataTable.Columns.Add(new DataColumn("Hour", Type.GetType("System.String")));
            CountDataTable.Columns.Add(new DataColumn("Count", Type.GetType("System.Int32")));

            decimal Total = 0;

            string FilterExpr = "";

            if (UserID > 0)
                FilterExpr = " AND UserID = " + UserID;
            else
                FilterExpr = " AND (UserID NOT IN(SELECT UserID FROM Users WHERE Coding = 1))";

            ChartDataTable.Clear();

            for (int i = 0; i < 24; i++)
            {
                if (FilterExpr.Length > 0)
                    FilterExpr += " AND (DateEnter >= '" + DateFrom.ToString("yyyy-MM-dd") + "' AND DateEnter <= '" + DateTo.ToString("yyyy-MM-dd") + " 23:59:59.996')";
                else
                    FilterExpr += "WHERE (DateEnter >= '" + DateFrom.ToString("yyyy-MM-dd") + "' AND DateEnter <= '" + DateTo.ToString("yyyy-MM-dd") + " 23:59:59.996')";

                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT Count(ModulesJournalID) AS Count FROM ModulesJournal WHERE DATEPART(hh,DateEnter) = " + i + FilterExpr, ConnectionStrings.UsersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        DataRow NewRow = CountDataTable.NewRow();
                        NewRow["Hour"] = i;
                        NewRow["Count"] = DT.Rows[0]["Count"];
                        CountDataTable.Rows.Add(NewRow);

                        Total += Convert.ToInt32(DT.Rows[0]["Count"]);
                    }
                }
            }

            for (int i = 0; i < 24; i++)
            {
                DataRow NewRow = ChartDataTable.NewRow();
                NewRow["Hour"] = i;
                if (Total == 0)
                    NewRow["Percent"] = 0;
                else
                    NewRow["Percent"] = Decimal.Round(Convert.ToDecimal(CountDataTable.Rows[i]["Count"]) / (Total / 100), 2, MidpointRounding.AwayFromZero);
                ChartDataTable.Rows.Add(NewRow);
            }


            CountDataTable.Dispose();
        }

        private void Binding()
        {
            ResultUsersTimeBingingSource = new BindingSource()
            {
                DataSource = ResultUsersTimeDataTable
            };
            TotalResultUsersBingingSource = new BindingSource()
            {
                DataSource = TotalResultUsersDataTable
            };
            ResultModulesBingingSource = new BindingSource()
            {
                DataSource = ResultModulesDataTable.DefaultView
            };
            TotalResultModulesBingingSource = new BindingSource()
            {
                DataSource = TotalResultModulesDataTable.DefaultView
            };
            UsersBingingSource = new BindingSource()
            {
                DataSource = UsersDataTable
            };
            ModulesBingingSource = new BindingSource()
            {
                DataSource = ModulesDataTable
            };
            ModulesJournalBingingSource = new BindingSource()
            {
                DataSource = ModulesJournalDataTable
            };
            JournalDataGrid.DataSource = ModulesJournalBingingSource;
        }

        private void CreateColumns()
        {
            UserColumn = new DataGridViewComboBoxColumn()
            {
                Name = "UserColumn",
                HeaderText = "Пользователь",
                DataPropertyName = "UserID",
                DataSource = new DataView(UsersDataTable),
                ValueMember = "UserID",
                DisplayMember = "Name",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            ModuleColumn = new DataGridViewComboBoxColumn()
            {
                Name = "ModuleColumn",
                HeaderText = "Модуль",
                DataPropertyName = "ModuleID",
                DataSource = new DataView(ModulesDataTable),
                ValueMember = "ModuleID",
                DisplayMember = "ModuleName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            JournalDataGrid.Columns.Add(UserColumn);
            JournalDataGrid.Columns.Add(ModuleColumn);
        }

        private void GridSettings()
        {
            if (JournalDataGrid.Columns.Contains("TotalMinutes"))
                JournalDataGrid.Columns["TotalMinutes"].Visible = false;
            JournalDataGrid.Columns["LoginJournalID"].Visible = false;
            JournalDataGrid.Columns["ModulesJournalID"].Visible = false;
            JournalDataGrid.Columns["UserID"].Visible = false;
            JournalDataGrid.Columns["ModuleID"].Visible = false;

            JournalDataGrid.Columns["DateEnter"].HeaderText = "Время входа";
            JournalDataGrid.Columns["DateEnter"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            JournalDataGrid.Columns["DateEnter"].Width = 160;

            JournalDataGrid.Columns["DateExit"].HeaderText = "Время выхода";
            JournalDataGrid.Columns["DateExit"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            JournalDataGrid.Columns["DateExit"].Width = 160;

            JournalDataGrid.Columns["DateEnter"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm:ss";
            JournalDataGrid.Columns["DateExit"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm:ss";

            JournalDataGrid.Columns["ModuleColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            JournalDataGrid.Columns["ModuleColumn"].MinimumWidth = 300;
            JournalDataGrid.Columns["UserColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            JournalDataGrid.Columns["UserColumn"].MinimumWidth = 260;
            JournalDataGrid.Columns["ModuleColumn"].SortMode = DataGridViewColumnSortMode.Automatic;
            JournalDataGrid.Columns["UserColumn"].SortMode = DataGridViewColumnSortMode.Automatic;
        }

        public void Initialize()
        {
            Create();
            Fill();
            Binding();
            CreateColumns();
            GridSettings();
        }

        public void GetResultModulesTable()
        {
            ResultModulesDataTable.Clear();
            TotalResultModulesDataTable.Clear();
            DataTable DT1 = new DataTable();
            DataTable DT2 = new DataTable();
            using (DataView DV = new DataView(ModulesJournalDataTable))
            {
                DT1 = DV.ToTable(true, new string[] { "ModuleID" });
            }
            for (int i = 0; i < DT1.Rows.Count; i++)
            {
                int ModuleID = Convert.ToInt32(DT1.Rows[i]["ModuleID"]);

                using (DataView DV = new DataView(ModulesJournalDataTable, "ModuleID=" + ModuleID, string.Empty, DataViewRowState.CurrentRows))
                {
                    DT2.Clear();
                    DT2 = DV.ToTable(true, new string[] { "UserID" });
                }
                int TotalCount = 0;
                for (int j = 0; j < DT2.Rows.Count; j++)
                {
                    int UserID = Convert.ToInt32(DT2.Rows[j]["UserID"]);
                    DataRow[] Rows = ModulesJournalDataTable.Select("UserID = " + UserID + " AND ModuleID=" + ModuleID);
                    TotalCount += Rows.Count();
                }
                DataRow NewRow1 = TotalResultModulesDataTable.NewRow();
                NewRow1["ModuleName"] = GetModuleName(ModuleID);
                NewRow1["TotalCount"] = TotalCount;
                TotalResultModulesDataTable.Rows.Add(NewRow1);
                for (int j = 0; j < DT2.Rows.Count; j++)
                {
                    int UserID = Convert.ToInt32(DT2.Rows[j]["UserID"]);
                    DataRow[] Rows = ModulesJournalDataTable.Select("UserID = " + UserID + " AND ModuleID=" + ModuleID);
                    DataRow NewRow = ResultModulesDataTable.NewRow();
                    NewRow["UserName"] = GetUserName(UserID);
                    NewRow["ModuleName"] = GetModuleName(ModuleID);
                    NewRow["Rating"] = Rows.Count();
                    ResultModulesDataTable.Rows.Add(NewRow);
                }
            }
            ResultModulesDataTable.DefaultView.Sort = "ModuleName, Rating DESC, UserName";
            ResultModulesBingingSource.MoveFirst();
            TotalResultModulesDataTable.DefaultView.Sort = "TotalCount DESC";
            TotalResultModulesBingingSource.MoveFirst();
        }

        public void GetResultUsersTimeTable(DateTime DateFrom, DateTime DateTo)
        {
            int TotalDays = 0;

            TotalDays = (int)(DateTo - DateFrom).TotalDays + 1;
            ResultUsersTimeDataTable.Clear();
            DataTable DT1 = new DataTable();
            DataTable DT2 = new DataTable();
            using (DataView DV = new DataView(LoginJournalDataTable))
            {
                DT1 = DV.ToTable(true, new string[] { "UserID", "ComputerName" });
            }
            for (int i = 0; i < DT1.Rows.Count; i++)
            {
                int UserID = Convert.ToInt32(DT1.Rows[i]["UserID"]);
                string ComputerName = DT1.Rows[i]["ComputerName"].ToString();

                int AGVMinutes = 0;
                int TotalMinutes = 0;
                int Minutes = 0;
                int Hours = 0;

                DataRow[] Rows = LoginJournalDataTable.Select("UserID = " + UserID + " AND ComputerName='" + ComputerName + "'");
                for (int j = 0; j < Rows.Count(); j++)
                {
                    int LoginJournalID = Convert.ToInt32(Rows[j]["LoginJournalID"]);
                    DataRow[] Rows1 = ModulesJournalDataTable.Select("LoginJournalID = " + LoginJournalID);
                    foreach (DataRow item in Rows1)
                    {
                        TotalMinutes += Convert.ToInt32(item["TotalMinutes"]);
                    }
                }
                Hours = (TotalMinutes / 60);
                Minutes = (TotalMinutes - Hours * 60);
                string TotalTime = Hours.ToString() + " ч " + Minutes.ToString() + " мин";
                if (Hours == 0 && Minutes == 0)
                    TotalTime = "0 ч 1 мин";

                AGVMinutes = TotalMinutes / TotalDays;
                Hours = (AGVMinutes / 60);
                Minutes = (AGVMinutes - Hours * 60);
                string AVGTime = Hours.ToString() + " ч " + Minutes.ToString() + " мин";
                if (Hours == 0 && Minutes == 0)
                    AVGTime = "0 ч 1 мин";

                DataRow NewRow = ResultUsersTimeDataTable.NewRow();
                NewRow["UserName"] = GetUserName(UserID);
                NewRow["ComputerName"] = ComputerName;
                NewRow["TotalTime"] = TotalTime;
                NewRow["AVGTime"] = AVGTime;
                ResultUsersTimeDataTable.Rows.Add(NewRow);
            }
            ResultUsersTimeDataTable.DefaultView.Sort = "UserName, AVGTime DESC";
            ResultUsersTimeBingingSource.MoveFirst();
        }

        public void GetTotalResultUsersTable()
        {
            TotalResultUsersDataTable.Clear();
            DataTable DT1 = new DataTable();
            DataTable DT2 = new DataTable();
            using (DataView DV = new DataView(ModulesJournalDataTable))
            {
                DT1 = DV.ToTable(true, new string[] { "UserID" });
            }
            for (int i = 0; i < DT1.Rows.Count; i++)
            {
                int UserID = Convert.ToInt32(DT1.Rows[i]["UserID"]);

                using (DataView DV = new DataView(ModulesJournalDataTable, "UserID=" + UserID, string.Empty, DataViewRowState.CurrentRows))
                {
                    DT2.Clear();
                    DT2 = DV.ToTable(true, new string[] { "ModuleID" });
                }
                for (int j = 0; j < DT2.Rows.Count; j++)
                {
                    int ModuleID = Convert.ToInt32(DT2.Rows[j]["ModuleID"]);
                    int TotalMinutes = 0;
                    int Minutes = 0;
                    int Hours = 0;
                    DataRow[] Rows = ModulesJournalDataTable.Select("UserID = " + UserID + " AND ModuleID=" + ModuleID);
                    foreach (DataRow item in Rows)
                    {
                        TotalMinutes += Convert.ToInt32(item["TotalMinutes"]);
                        Hours = (TotalMinutes / 60);
                        Minutes = (TotalMinutes - Hours * 60);
                        //if (item["DateExit"] != DBNull.Value)
                        //{
                        //    DateTime DateEnter = Convert.ToDateTime(item["DateEnter"]);
                        //    DateTime DateExit = Convert.ToDateTime(item["DateExit"]);
                        //    TotalMinutes += (int)(DateExit - DateEnter).TotalMinutes;
                        //    Hours = (TotalMinutes / 60);
                        //    Minutes = (TotalMinutes - Hours * 60);
                        //}
                        //else
                        //{
                        //    TotalMinutes += 1;
                        //    Minutes = 1;
                        //}
                    }
                    string TotalTime = Hours.ToString() + " ч " + Minutes.ToString() + " мин";
                    if (Hours == 0 && Minutes == 0)
                        TotalTime = "0 ч 1 мин";
                    DataRow NewRow = TotalResultUsersDataTable.NewRow();
                    NewRow["UserName"] = GetUserName(UserID);
                    NewRow["ModuleName"] = GetModuleName(ModuleID);
                    NewRow["Rating"] = Rows.Count();
                    NewRow["TotalTime"] = TotalTime;
                    NewRow["TotalMinutes"] = TotalMinutes;
                    TotalResultUsersDataTable.Rows.Add(NewRow);
                }
            }

            using (DataView DV = new DataView(TotalResultUsersDataTable))
            {
                DT1 = DV.ToTable(true, new string[] { "UserName" });
            }
            for (int i = 0; i < DT1.Rows.Count; i++)
            {
                string UserName = DT1.Rows[i]["UserName"].ToString();
                DataRow[] Rows = TotalResultUsersDataTable.Select("UserName = '" + UserName + "'");
                int TotalMinutes = 0;
                int Minutes = 0;
                int Hours = 0;
                foreach (DataRow item in Rows)
                {
                    if (item["TotalMinutes"] != DBNull.Value)
                    {
                        TotalMinutes += Convert.ToInt32(item["TotalMinutes"]);
                        Hours = (TotalMinutes / 60);
                        Minutes = (TotalMinutes - Hours * 60);
                    }
                }
                string TotalTime = Hours.ToString() + " ч " + Minutes.ToString() + " мин";
                if (Hours == 0 && Minutes == 0)
                    TotalTime = "0 ч 1 мин";

                foreach (DataRow item in Rows)
                {
                    item["TotalMinutes"] = TotalTime;
                }
            }
            TotalResultUsersDataTable.DefaultView.Sort = "UserName, Rating DESC";
            TotalResultUsersBingingSource.MoveFirst();
        }

        //public void GetTotalResultUsersTable()
        //{
        //    TotalResultUsersDataTable.Clear();
        //    DataTable DT1 = new DataTable();
        //    DataTable DT2 = new DataTable();
        //    using (DataView DV = new DataView(ModulesJournalDataTable))
        //    {
        //        DT1 = DV.ToTable(true, new string[] { "UserID" });
        //    }
        //    for (int i = 0; i < DT1.Rows.Count; i++)
        //    {
        //        int UserID = Convert.ToInt32(DT1.Rows[i]["UserID"]);

        //        using (DataView DV = new DataView(ModulesJournalDataTable, "UserID=" + UserID, string.Empty, DataViewRowState.CurrentRows))
        //        {
        //            DT2.Clear();
        //            DT2 = DV.ToTable(true, new string[] { "ModuleID" });
        //        }
        //        for (int j = 0; j < DT2.Rows.Count; j++)
        //        {
        //            int ModuleID = Convert.ToInt32(DT2.Rows[j]["ModuleID"]);
        //            double TotalMinutes = 0;
        //            double Minutes = 0;
        //            double Hours = 0;
        //            DataRow[] Rows = ModulesJournalDataTable.Select("UserID = " + UserID + " AND ModuleID=" + ModuleID);
        //            foreach (DataRow item in Rows)
        //            {
        //                if (item["DateExit"] != DBNull.Value)
        //                {
        //                    DateTime DateEnter = Convert.ToDateTime(item["DateEnter"]);
        //                    DateTime DateExit = Convert.ToDateTime(item["DateExit"]);
        //                    TotalMinutes += (DateExit - DateEnter).TotalMinutes;
        //                    Hours = (TotalMinutes / 60);
        //                    Minutes = (TotalMinutes - Hours * 60);
        //                }
        //            }
        //            string TotalTime = Convert.ToInt32(Hours).ToString() + " ч " + Convert.ToInt32(Minutes).ToString() + " мин";
        //            if (Hours == 0 && Minutes == 0)
        //                TotalTime = "0 ч 1 мин";
        //            DataRow NewRow = TotalResultUsersDataTable.NewRow();
        //            NewRow["UserName"] = GetUserName(UserID);
        //            NewRow["ModuleName"] = GetModuleName(ModuleID);
        //            NewRow["Rating"] = Rows.Count();
        //            NewRow["TotalTime"] = TotalTime;
        //            NewRow["TotalMinutes"] = TotalMinutes;
        //            TotalResultUsersDataTable.Rows.Add(NewRow);
        //        }
        //    }

        //    using (DataView DV = new DataView(TotalResultUsersDataTable))
        //    {
        //        DT1 = DV.ToTable(true, new string[] { "UserName" });
        //    }
        //    for (int i = 0; i < DT1.Rows.Count; i++)
        //    {
        //        string UserName = DT1.Rows[i]["UserName"].ToString();
        //        DataRow[] Rows = TotalResultUsersDataTable.Select("UserName = '" + UserName + "'");
        //        double TotalMinutes = 0;
        //        double Minutes = 0;
        //        double Hours = 0;
        //        foreach (DataRow item in Rows)
        //        {
        //            if (item["TotalMinutes"] != DBNull.Value)
        //            {
        //                TotalMinutes += Convert.ToDouble(item["TotalMinutes"]);
        //                Hours = (TotalMinutes / 60);
        //                Minutes = (TotalMinutes - Hours * 60);
        //            }
        //        }
        //        string TotalTime = Convert.ToInt32(Hours).ToString() + " ч " + Convert.ToInt32(Minutes).ToString() + " мин";
        //        if (Hours == 0 && Minutes == 0)
        //            TotalTime = "0 ч 1 мин";

        //        foreach (DataRow item in Rows)
        //        {
        //            item["TotalMinutes"] = TotalTime;
        //        }
        //    }
        //    TotalResultUsersDataTable.DefaultView.Sort = "UserName, Rating DESC";
        //    TotalResultUsersBingingSource.MoveFirst();
        //}

        public string GetModuleName(int ModuleID)
        {
            string Name = string.Empty;
            DataRow[] rows = ModulesDataTable.Select("ModuleID=" + ModuleID);
            if (rows.Count() > 0)
                Name = rows[0]["ModuleName"].ToString();
            return Name;
        }

        public string GetUserName(int UserID)
        {
            string Name = string.Empty;
            DataRow[] rows = UsersDataTable.Select("UserID=" + UserID);
            if (rows.Count() > 0)
                Name = rows[0]["ShortName"].ToString();
            return Name;
        }

        public void FillComputerName()
        {
            for (int i = 0; i < LoginJournalDataTable.Rows.Count; i++)
            {
                string XML = LoginJournalDataTable.Rows[i]["ComputerParams"].ToString();
                if (XML.Length == 0)
                    continue;
                ComputerParamsDataTable.Clear();
                using (StringReader SR = new StringReader(XML))
                {
                    try
                    {
                        ComputerParamsDataTable.ReadXml(SR);
                        foreach (DataColumn Column in ComputerParamsDataTable.Columns)
                        {
                            if (Column.ColumnName == "ComputerName")
                                LoginJournalDataTable.Rows[i]["ComputerName"] = ComputerParamsDataTable.Rows[0][Column.ColumnName];
                        }
                    }
                    catch
                    {

                    }
                }
            }
        }

        public void FilterLoginJournal(int UserID, int ModuleID, DateTime DateFrom, DateTime DateTo, bool Coders)
        {
            string FilterExpr = "";


            if (UserID > 0)
                FilterExpr += " WHERE UserID = " + UserID.ToString();

            if (ModuleID > 0)
                if (FilterExpr.Length > 0)
                    FilterExpr += " AND ModuleID = " + ModuleID;
                else
                    FilterExpr += " WHERE ModuleID = " + ModuleID;

            if (!Coders)
                if (FilterExpr.Length > 0)
                    FilterExpr += " AND UserID NOT IN (SELECT UserID FROM Users WHERE Coding = 1)";
                else
                    FilterExpr += "WHERE UserID NOT IN (SELECT UserID FROM Users WHERE Coding = 1)";

            if (FilterExpr.Length > 0)
                FilterExpr += " AND (DateEnter >= '" + DateFrom.ToString("yyyy-MM-dd") + "' AND DateEnter <= '" + DateTo.ToString("yyyy-MM-dd") + " 23:59:59.996')";
            else
                FilterExpr += "WHERE (DateEnter >= '" + DateFrom.ToString("yyyy-MM-dd") + "' AND DateEnter <= '" + DateTo.ToString("yyyy-MM-dd") + " 23:59:59.996')";

            LoginJournalDataTable.Clear();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM LoginJournal WHERE LoginJournalID IN (SELECT LoginJournalID FROM ModulesJournal " + FilterExpr + ") ORDER BY DateEnter ASC", ConnectionStrings.UsersConnectionString))
            {
                DA.Fill(LoginJournalDataTable);
            }
            FillComputerName();
        }

        public void FilterModulesJournal(int UserID, int ModuleID, DateTime DateFrom, DateTime DateTo, bool Coders)
        {
            string FilterExpr = "";


            if (UserID > 0)
                FilterExpr += " WHERE UserID = " + UserID.ToString();


            if (ModuleID > 0)
                if (FilterExpr.Length > 0)
                    FilterExpr += " AND ModuleID = " + ModuleID;
                else
                    FilterExpr += " WHERE ModuleID = " + ModuleID;

            if (!Coders)
                if (FilterExpr.Length > 0)
                    FilterExpr += " AND UserID NOT IN (SELECT UserID FROM Users WHERE Coding = 1)";
                else
                    FilterExpr += "WHERE UserID NOT IN (SELECT UserID FROM Users WHERE Coding = 1)";

            if (FilterExpr.Length > 0)
                FilterExpr += " AND (DateEnter >= '" + DateFrom.ToString("yyyy-MM-dd") + "' AND DateEnter <= '" + DateTo.ToString("yyyy-MM-dd") + " 23:59:59.996')";
            else
                FilterExpr += "WHERE (DateEnter >= '" + DateFrom.ToString("yyyy-MM-dd") + "' AND DateEnter <= '" + DateTo.ToString("yyyy-MM-dd") + " 23:59:59.996')";

            ModulesJournalDataTable.Clear();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM ModulesJournal " + FilterExpr + " ORDER BY DateEnter ASC", ConnectionStrings.UsersConnectionString))
            {
                DA.Fill(ModulesJournalDataTable);
            }

            for (int i = 0; i < ModulesJournalDataTable.Rows.Count; i++)
            {
                int TotalMinutes = 0;
                if (ModulesJournalDataTable.Rows[i]["DateExit"] != DBNull.Value)
                {
                    DateTime DateEnter = Convert.ToDateTime(ModulesJournalDataTable.Rows[i]["DateEnter"]);
                    DateTime DateExit = Convert.ToDateTime(ModulesJournalDataTable.Rows[i]["DateExit"]);
                    TotalMinutes = (int)(DateExit - DateEnter).TotalMinutes;
                    if (TotalMinutes == 0)
                        TotalMinutes = 1;
                }
                else
                    TotalMinutes = 1;
                ModulesJournalDataTable.Rows[i]["TotalMinutes"] = TotalMinutes;
            }

            GetHours(UserID, DateFrom, DateTo);
        }

    }






    public class AdminJournalDetail
    {
        public DataTable UsersDataTable;
        DataTable LoginJournalDataTable;
        DataTable ModulesJournalDataTable;
        DataTable ComputerParamsDataTable;
        DataTable ComputerParamsStuctDataTable;
        DataTable ModulesDataTable;
        DataTable MessagesDataTable;
        public DataTable FullUsersDataTable;
        public DataTable OfficesDataTable;

        public BindingSource UsersBindingSource;
        public BindingSource LoginJournalBindingSource;
        public BindingSource ModulesJournalBindingSource;
        public BindingSource ComputerParamsBindingSource;
        public BindingSource MessagesBindingSource;

        DateTime StartModuleDateTime;

        UsersDataGrid UsersGrid;
        PercentageDataGrid LoginJournalDataGrid;
        PercentageDataGrid ModulesJournalDataGrid;
        PercentageDataGrid ComputerParamsDataGrid;
        PercentageDataGrid MessagesDataGrid;

        Font fUserFont = new Font("Segoe UI", 13.0f, FontStyle.Bold);
        Font fModuleFont = new Font("Segoe UI", 13.0f, FontStyle.Bold);
        Font fTextFont = new Font("Segoe UI", 11.0f, FontStyle.Regular);

        Color cUserFontColor = Color.FromArgb(65, 124, 174);
        Color cModuleColor = Color.FromArgb(255, 15, 60);
        Color cTextFontColor = Color.Black;

        RichTextBox RichTextBox;

        public AdminJournalDetail(ref UsersDataGrid tUsersGrid, ref PercentageDataGrid tLoginJournalDataGrid,
                                  ref PercentageDataGrid tComputerParamsDataGrid, ref PercentageDataGrid tModulesJournalDataGrid,
                                  ref RichTextBox tRichTextBox, ref PercentageDataGrid tMessagesDataGrid)
        {
            UsersGrid = tUsersGrid;
            LoginJournalDataGrid = tLoginJournalDataGrid;
            ComputerParamsDataGrid = tComputerParamsDataGrid;
            ModulesJournalDataGrid = tModulesJournalDataGrid;
            MessagesDataGrid = tMessagesDataGrid;

            RichTextBox = tRichTextBox;

            StartModuleDateTime = DateTime.Now;

            UsersDataTable = new DataTable();
            UsersDataTable.Columns.Add(new DataColumn("OnlineStatus", Type.GetType("System.Boolean")));
            UsersDataTable.Columns.Add(new DataColumn("IdleTime", Type.GetType("System.String")));
            UsersDataTable.Columns.Add(new DataColumn("OnTop", Type.GetType("System.String")));

            ModulesDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ModuleID, ModuleName, FormName FROM Modules", ConnectionStrings.UsersConnectionString))
            {
                DA.Fill(ModulesDataTable);
            }

            {
                DataRow NewRow = ModulesDataTable.NewRow();
                NewRow["ModuleID"] = 0;
                NewRow["ModuleName"] = "Главное меню";
                NewRow["FormName"] = "LightStartForm";
                ModulesDataTable.Rows.Add(NewRow);
            }

            {
                DataRow NewRow = ModulesDataTable.NewRow();
                NewRow["ModuleID"] = -1;
                NewRow["ModuleName"] = "-";
                NewRow["FormName"] = "Unknown form";
                ModulesDataTable.Rows.Add(NewRow);
            }

            {
                DataRow NewRow = ModulesDataTable.NewRow();
                NewRow["ModuleID"] = -2;
                NewRow["ModuleName"] = "Модуль в разработке";
                NewRow["FormName"] = "UnderConstructionForm";
                ModulesDataTable.Rows.Add(NewRow);
            }


            LoginJournalDataTable = new DataTable();
            MessagesDataTable = new DataTable();
            ModulesJournalDataTable = new DataTable();

            ComputerParamsStuctDataTable = new DataTable();
            ComputerParamsStuctDataTable.Columns.Add(new DataColumn("Param", Type.GetType("System.String")));
            ComputerParamsStuctDataTable.Columns.Add(new DataColumn("Value", Type.GetType("System.String")));

            ComputerParamsDataTable = new DataTable()
            {
                TableName = "ComputerParams"
            };
            ComputerParamsDataTable.Columns.Add(new DataColumn("Domain", Type.GetType("System.String")));
            ComputerParamsDataTable.Columns.Add(new DataColumn("ComputerName", Type.GetType("System.String")));
            ComputerParamsDataTable.Columns.Add(new DataColumn("LoginName", Type.GetType("System.String")));
            ComputerParamsDataTable.Columns.Add(new DataColumn("Manufacturer", Type.GetType("System.String")));
            ComputerParamsDataTable.Columns.Add(new DataColumn("Model", Type.GetType("System.String")));
            ComputerParamsDataTable.Columns.Add(new DataColumn("ProcessorName", Type.GetType("System.String")));
            ComputerParamsDataTable.Columns.Add(new DataColumn("ProcessorCores", Type.GetType("System.String")));
            ComputerParamsDataTable.Columns.Add(new DataColumn("TotalRAM", Type.GetType("System.String")));
            ComputerParamsDataTable.Columns.Add(new DataColumn("OSName", Type.GetType("System.String")));
            ComputerParamsDataTable.Columns.Add(new DataColumn("OSVersion", Type.GetType("System.String")));
            ComputerParamsDataTable.Columns.Add(new DataColumn("OSPlatform", Type.GetType("System.String")));


            UsersBindingSource = new BindingSource()
            {
                DataSource = UsersDataTable
            };
            UsersGrid.DataSource = UsersBindingSource;

            LoginJournalBindingSource = new BindingSource()
            {
                DataSource = LoginJournalDataTable
            };
            LoginJournalDataGrid.DataSource = LoginJournalBindingSource;

            ComputerParamsBindingSource = new BindingSource()
            {
                DataSource = ComputerParamsStuctDataTable
            };
            tComputerParamsDataGrid.DataSource = ComputerParamsBindingSource;

            ModulesJournalBindingSource = new BindingSource()
            {
                DataSource = ModulesJournalDataTable
            };
            tModulesJournalDataGrid.DataSource = ModulesJournalBindingSource;

            MessagesBindingSource = new BindingSource()
            {
                DataSource = MessagesDataTable
            };
            tMessagesDataGrid.DataSource = MessagesBindingSource;
        }

        public void FillUsers(DateTime DateFrom, DateTime DateTo)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DISTINCT UserID FROM LoginJournal WHERE DateEnter >= '" + DateFrom.ToString("yyyy-MM-dd") + "' AND DateEnter <= '" + DateTo.ToString("yyyy-MM-dd") + " 23:59:59.996'", ConnectionStrings.UsersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count != UsersDataTable.Rows.Count)
                    {
                        using (SqlDataAdapter uDA = new SqlDataAdapter("SELECT UserID, ShortName, TopModule, TopMost FROM USERS WHERE UserID IN (SELECT UserID FROM LoginJournal WHERE DateEnter >= '" + DateFrom.ToString("yyyy-MM-dd") + "' AND DateEnter <= '" + DateTo.ToString("yyyy-MM-dd") + " 23:59:59.996')", ConnectionStrings.UsersConnectionString))
                        {
                            UsersDataTable.Clear();

                            uDA.Fill(UsersDataTable);
                        }
                    }
                    else
                        return;
                }
            }

            if (FullUsersDataTable != null)
                FullUsersDataTable.Clear();
            else
                FullUsersDataTable = new DataTable();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT UserID, Name, ShortName FROM USERS", ConnectionStrings.UsersConnectionString))
            {
                DA.Fill(FullUsersDataTable);
            }

            if (OfficesDataTable != null)
                OfficesDataTable.Clear();
            else
                OfficesDataTable = new DataTable();

            OfficesDataTable = new DataTable();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM Offices", ConnectionStrings.UsersConnectionString))
            {
                DA.Fill(OfficesDataTable);
            }
        }

        public void FillLoginJournal(int UserID, DateTime DateFrom, DateTime DateTo)
        {
            LoginJournalDataTable.Clear();

            using (SqlDataAdapter uDA = new SqlDataAdapter("SELECT LoginJournalID, DateEnter, DateExit, ComputerParams FROM LoginJournal WHERE UserID = " + UserID + " AND DateEnter >= '" + DateFrom.ToString("yyyy-MM-dd") + "' AND DateEnter <= '" + DateTo.ToString("yyyy-MM-dd") + " 23:59:59.996' ORDER BY LoginJournalID DESC", ConnectionStrings.UsersConnectionString))
            {
                uDA.Fill(LoginJournalDataTable);
            }
        }

        public void FillModulesJournal(int LoginJournalID)
        {
            ModulesJournalDataTable.Clear();

            using (SqlDataAdapter uDA = new SqlDataAdapter("SELECT ModulesJournalID, DateEnter, DateExit, ModuleID FROM ModulesJournal WHERE LoginJournalID = " + LoginJournalID, ConnectionStrings.UsersConnectionString))
            {
                uDA.Fill(ModulesJournalDataTable);
            }
        }

        public void FillMessages(DateTime DateFrom, DateTime DateTo)
        {
            MessagesDataTable.Clear();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MessageID, SendDateTime, SENDERUSERS.Name AS SENDERNAME, RECIPIENTUSERS.Name AS RecipientUser FROM infiniu2_light.dbo.Messages INNER JOIN infiniu2_users.dbo.Users AS SENDERUSERS ON SENDERUSERS.UserID = SenderUserID INNER JOIN infiniu2_users.dbo.Users AS RECIPIENTUSERS ON RECIPIENTUSERS.UserID = RecipientUserID WHERE SendDateTime >= '" + DateFrom.ToString("yyyy-MM-dd") + "' AND SendDateTime <= '" + DateTo.ToString("yyyy-MM-dd") + " 23:59:59.996'", ConnectionStrings.LightConnectionString))
            {
                DA.Fill(MessagesDataTable);
            }
        }

        public void FillComputerParams(int LoginJournalID)
        {
            ComputerParamsDataTable.Clear();
            ComputerParamsStuctDataTable.Clear();

            string XML = LoginJournalDataTable.Select("LoginJournalID = " + LoginJournalID)[0]["ComputerParams"].ToString();

            if (XML.Length == 0)
                return;

            using (StringReader SR = new StringReader(XML))
            {
                try
                {
                    ComputerParamsDataTable.ReadXml(SR);
                }
                catch
                {

                }
            }

            foreach (DataColumn Column in ComputerParamsDataTable.Columns)
            {
                DataRow NewRow = ComputerParamsStuctDataTable.NewRow();
                NewRow["Param"] = Column.ColumnName;
                NewRow["Value"] = ComputerParamsDataTable.Rows[0][Column.ColumnName];
                ComputerParamsStuctDataTable.Rows.Add(NewRow);
            }
        }

        public int GetOnlineUsersCount()
        {
            return UsersDataTable.Select("OnlineStatus = 1").Count();
        }

        public void SetGrid()
        {
            DataGridViewComboBoxColumn FormColumn = new DataGridViewComboBoxColumn()
            {
                Name = "FormColumn",
                HeaderText = "Название модуля",
                DataPropertyName = "TopModule",
                DataSource = new DataView(ModulesDataTable),
                ValueMember = "ModuleID",
                DisplayMember = "ModuleName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                DisplayIndex = 5
            };
            UsersGrid.AutoGenerateColumns = false;

            UsersGrid.Columns.Add(FormColumn);

            UsersGrid.Columns["UserID"].Visible = false;
            UsersGrid.Columns["OnlineStatus"].Visible = false;
            UsersGrid.Columns["TopModule"].Visible = false;
            UsersGrid.Columns["OnlineColumn"].DisplayIndex = 0;
            UsersGrid.Columns["IdleTime"].DisplayIndex = 6;
            UsersGrid.Columns["TopMost"].Visible = false;
            UsersGrid.Columns["OnTop"].DisplayIndex = 5;
            UsersGrid.sOnlineStatusColumnName = "OnlineStatus";

            UsersGrid.Columns["IdleTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            UsersGrid.Columns["IdleTime"].Width = 80;
            UsersGrid.Columns["OnTop"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            UsersGrid.Columns["OnTop"].Width = 40;
            UsersGrid.Columns["ShortName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            UsersGrid.Columns["ShortName"].Width = 190;

            LoginJournalDataGrid.Columns["DateEnter"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm:ss";
            LoginJournalDataGrid.Columns["DateExit"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm:ss";
            LoginJournalDataGrid.Columns["DateEnter"].HeaderText = "Дата входа";
            LoginJournalDataGrid.Columns["DateExit"].HeaderText = "Дата выхода";
            LoginJournalDataGrid.Columns["LoginJournalID"].Visible = false;
            LoginJournalDataGrid.Columns["ComputerParams"].Visible = false;

            ComputerParamsDataGrid.Columns["Param"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            ComputerParamsDataGrid.Columns["Param"].Width = 150;


            DataGridViewComboBoxColumn ModuleColumn = new DataGridViewComboBoxColumn()
            {
                Name = "ModuleColumn",
                HeaderText = "Название модуля",
                DataPropertyName = "ModuleID",
                DataSource = new DataView(ModulesDataTable),
                ValueMember = "ModuleID",
                DisplayMember = "ModuleName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                DisplayIndex = 0
            };
            ModulesJournalDataGrid.Columns.Add(ModuleColumn);
            ModulesJournalDataGrid.Columns["ModulesJournalID"].Visible = false;
            ModulesJournalDataGrid.Columns["ModuleID"].Visible = false;
            ModulesJournalDataGrid.Columns["DateEnter"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm:ss";
            ModulesJournalDataGrid.Columns["DateExit"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm:ss";
            ModulesJournalDataGrid.Columns["DateEnter"].HeaderText = "Дата входа";
            ModulesJournalDataGrid.Columns["DateExit"].HeaderText = "Дата выхода";

            ModulesJournalDataGrid.Columns["DateEnter"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            ModulesJournalDataGrid.Columns["DateEnter"].Width = 180;
            ModulesJournalDataGrid.Columns["DateExit"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            ModulesJournalDataGrid.Columns["DateExit"].Width = 180;

            MessagesDataGrid.Columns["SENDERNAME"].HeaderText = "От кого";
            MessagesDataGrid.Columns["RecipientUser"].HeaderText = "Кому";
            MessagesDataGrid.Columns["SendDateTime"].HeaderText = "Дата отправки";
            MessagesDataGrid.Columns["MessageID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MessagesDataGrid.Columns["MessageID"].Width = 120;
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private extern static IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);


        public void BeginUpdate()
        {
            SendMessage(RichTextBox.Handle, 0xb, (IntPtr)0, IntPtr.Zero);
        }

        public void EndUpdate()
        {
            SendMessage(RichTextBox.Handle, 0xb, (IntPtr)1, IntPtr.Zero);
            RichTextBox.Invalidate();
        }


        public void ClearTopMostAndTopModuleAndIdle()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT UserID, TopMost, TopModule, IdleTime FROM Users WHERE Online = 0 AND (TopMost = 1 OR TopModule > -1 OR IdleTime > -1)", ConnectionStrings.UsersConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        foreach (DataRow Row in DT.Rows)
                        {
                            Row["TopMost"] = false;
                            Row["TopModule"] = -1;
                            Row["IdleTime"] = DBNull.Value;
                        }

                        DA.Update(DT);
                    }
                }
            }
        }

        public void CheckOnline()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT UserID, TopModule, IdleTime, Online, TopMost FROM Users", ConnectionStrings.UsersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    try
                    {
                        DA.Fill(DT);
                    }
                    catch
                    {
                        return;
                    }

                    foreach (DataRow Row in UsersDataTable.Rows)
                    {
                        DataRow[] DRow = DT.Select("UserID = " + Row["UserID"]);


                        Row["OnlineStatus"] = DRow[0]["Online"];
                        Row["TopModule"] = DRow[0]["TopModule"];
                        Row["TopMost"] = DRow[0]["TopMost"];

                        if (DRow[0]["IdleTime"] == DBNull.Value)
                            Row["IdleTime"] = "-";
                        else
                            Row["IdleTime"] = SecondsToTime(Convert.ToInt32(DRow[0]["IdleTime"]));
                    }
                }
            }


        }

        private string SecondsToTime(int Seconds)
        {
            TimeSpan t = TimeSpan.FromSeconds(Seconds);

            string answer = string.Format("{0:D2}:{1:D2}:{2:D2}",
                            t.Hours,
                            t.Minutes,
                            t.Seconds);

            return answer;
        }

        //public string GetName(string FIO)
        //{
        //    return FIO.Substring(0, FIO.IndexOf(' '));
        //}

        public void FillRichTextBox()
        {
            BeginUpdate();

            RichTextBox.Clear();

            using (SqlDataAdapter uDA = new SqlDataAdapter("SELECT UserID, DateEnter, DateExit, ModuleID FROM ModulesJournal WHERE DateEnter >= '" + StartModuleDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'", ConnectionStrings.UsersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    uDA.Fill(DT);



                    foreach (DataRow Row in DT.Rows)
                    {
                        AddUser(FullUsersDataTable.Select("UserID = " + Row["UserID"])[0]["ShortName"].ToString());
                        AddText(" зашел в ");
                        AddModule(ModulesDataTable.Select("ModuleID = " + Row["ModuleID"])[0]["ModuleName"].ToString());
                        AddText(" в " + Convert.ToDateTime(Row["DateEnter"]).ToString("HH:mm:ss") + "\n");
                    }


                }
            }

            RichTextBox.ScrollToCaret();

            EndUpdate();
        }

        public void AddUser(string Sender)
        {
            RichTextBox.SelectionStart = RichTextBox.TextLength;
            RichTextBox.SelectionLength = 0;
            RichTextBox.SelectionFont = fUserFont;

            RichTextBox.SelectionColor = cUserFontColor;

            RichTextBox.AppendText(Sender);
        }

        public void AddText(string Text)
        {
            RichTextBox.SelectionStart = RichTextBox.TextLength;
            RichTextBox.SelectionLength = 0;
            RichTextBox.SelectionFont = fTextFont;

            RichTextBox.SelectionColor = cTextFontColor;

            RichTextBox.AppendText(Text);
        }

        public void AddModule(string Text)
        {
            RichTextBox.SelectionStart = RichTextBox.TextLength;
            RichTextBox.SelectionLength = 0;
            RichTextBox.SelectionFont = fModuleFont;

            RichTextBox.SelectionColor = cModuleColor;

            RichTextBox.AppendText(Text);
        }
    }






    public class AdminModulesManagement
    {
        public DataTable ModulesDataTable;
        public DataTable MainMenuTabsDataTable;

        public BindingSource ModulesBindingSource = null;
        public BindingSource MainMenuTabsBingingSource = null;

        SqlDataAdapter ModulesDataAdapter;
        SqlCommandBuilder ModulesCommandBuilder;

        PercentageDataGrid ModulesDataGrid;

        public AdminModulesManagement(ref PercentageDataGrid tModulesDataGrid)
        {
            ModulesDataGrid = tModulesDataGrid;
            Initialize();
        }

        private void Create()
        {
            ModulesDataTable = new DataTable();
        }

        private void Fill()
        {
            ModulesDataAdapter = new SqlDataAdapter("SELECT * FROM Modules", ConnectionStrings.UsersConnectionString);
            ModulesCommandBuilder = new SqlCommandBuilder(ModulesDataAdapter);
            ModulesDataAdapter.Fill(ModulesDataTable);

            ModulesDataTable.Columns.Add(new DataColumn("Rating", Type.GetType("System.Int32")));
            MainMenuTabsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT* FROM MenuItems", ConnectionStrings.UsersConnectionString))
            {
                DA.Fill(MainMenuTabsDataTable);
            }
        }


        private void Binding()
        {
            ModulesBindingSource = new BindingSource()
            {
                DataSource = ModulesDataTable
            };
            ModulesDataGrid.DataSource = ModulesBindingSource;

            MainMenuTabsBingingSource = new BindingSource()
            {
                DataSource = MainMenuTabsDataTable
            };
        }

        private void GridSettings()
        {
            ModulesDataGrid.Columns["MenuItemID"].Visible = false;

            ModulesDataGrid.Columns["ModuleID"].HeaderText = "ID";
            ModulesDataGrid.Columns["ModuleName"].HeaderText = "Название";
            ModulesDataGrid.Columns["SecurityAccess"].HeaderText = "Ограниченный доступ";
            ModulesDataGrid.Columns["PriceView"].HeaderText = "Содержит цены";
            ModulesDataGrid.Columns["Rating"].HeaderText = "Рейтинг";

            ModulesDataGrid.Columns["ModuleID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            ModulesDataGrid.Columns["ModuleID"].Width = 60;
            ModulesDataGrid.Columns["SecurityAccess"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            ModulesDataGrid.Columns["SecurityAccess"].Width = 195;
            ModulesDataGrid.Columns["PriceView"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            ModulesDataGrid.Columns["PriceView"].Width = 140;
            ModulesDataGrid.Columns["Rating"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            ModulesDataGrid.Columns["Rating"].Width = 100;

            ModulesDataGrid.Columns["SecurityAccess"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ModulesDataGrid.Columns["PriceView"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            ModulesDataGrid.Columns["SecurityAccess"].ReadOnly = false;
        }

        public void Initialize()
        {
            Create();
            Fill();
            Binding();
            GridSettings();
        }

        public void Filter(int MenuItemID)
        {
            ModulesBindingSource.Filter = "MenuItemID = " + MenuItemID;
        }

        public bool IsChecked(string ColumnName, int ID)
        {
            return Convert.ToBoolean(ModulesDataTable.Select("ModuleID = " + ID)[0][ColumnName]);
        }

        public void Check(string ColumnName, int ID, bool Check)
        {
            ModulesDataTable.Select("ModuleID = " + ID)[0][ColumnName] = Check;
        }

        public void Save()
        {
            ModulesDataAdapter.Update(ModulesDataTable);
        }

        public void Rating()
        {
            foreach (DataRow Row in ModulesDataTable.Rows)
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT Count(ModulesJournalID) AS Count FROM ModulesJournal WHERE ModuleID = " + Row["ModuleID"] +
                                                              " AND UserID NOT IN (SELECT UserID FROM Users WHERE Coding = 1)",
                                                             ConnectionStrings.UsersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        Row["Rating"] = DT.Rows[0]["Count"];
                    }
                }
            }
        }

        public void SetPicture(int ModuleID, Image Image)
        {
            if (Image != null)
            {
                MemoryStream ms = new MemoryStream();

                Image.Save(ms, ImageFormat.Png);

                ModulesDataTable.Select("ModuleID = " + ModuleID)[0]["Picture"] = ms.ToArray();

                ModulesDataAdapter.Update(ModulesDataTable);

                ms.Dispose();
            }


        }
    }






    public class AdminUsersManagement
    {
        public DataTable UsersDataTable;

        PercentageDataGrid UsersDataGrid;

        public BindingSource UsersBindingSource;

        SqlDataAdapter UsersDataAdapter;
        SqlCommandBuilder UsersCommandBuilder;

        public AdminUsersManagement(ref PercentageDataGrid tUsersDataGrid)
        {
            UsersDataGrid = tUsersDataGrid;

            Initialize();
        }

        private void Create()
        {
            UsersDataTable = new DataTable();
        }

        private void Fill()
        {
            UsersDataAdapter = new SqlDataAdapter("SELECT UserID, Name, ShortName, FirstName, LastName, MiddleName, Password, PriceAccess, Coding, OnlineRefreshDateTime, AccessToken, Email, Fired FROM Users ORDER BY Name ASC", ConnectionStrings.UsersConnectionString);
            UsersCommandBuilder = new SqlCommandBuilder(UsersDataAdapter);
            UsersDataAdapter.Fill(UsersDataTable);

            UsersDataTable.Columns.Add(new DataColumn("Rating", Type.GetType("System.Int32")));
        }


        private void Binding()
        {
            UsersBindingSource = new BindingSource()
            {
                DataSource = UsersDataTable
            };
            UsersDataGrid.DataSource = UsersBindingSource;
        }

        private void GridSettings()
        {
            UsersDataGrid.Columns["Password"].Visible = false;
            UsersDataGrid.Columns["OnlineRefreshDateTime"].Visible = false;
            UsersDataGrid.Columns["AccessToken"].Visible = false;
            UsersDataGrid.Columns["Email"].Visible = false;

            UsersDataGrid.Columns["UserID"].HeaderText = "ID";
            UsersDataGrid.Columns["Name"].HeaderText = "ФИО";
            UsersDataGrid.Columns["PriceAccess"].HeaderText = "Доступ к ценам";
            UsersDataGrid.Columns["Coding"].HeaderText = "Разработчик";
            UsersDataGrid.Columns["Rating"].HeaderText = "Рейтинг";
            UsersDataGrid.Columns["Fired"].HeaderText = "Отключен";

            UsersDataGrid.Columns["UserID"].ReadOnly = true;
            //UsersDataGrid.Columns["Name"].ReadOnly = true;
            UsersDataGrid.Columns["Rating"].ReadOnly = true;

            UsersDataGrid.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            UsersDataGrid.Columns["UserID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            UsersDataGrid.Columns["UserID"].Width = 60;
            UsersDataGrid.Columns["Fired"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            UsersDataGrid.Columns["Fired"].Width = 90;
            UsersDataGrid.Columns["PriceAccess"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            UsersDataGrid.Columns["PriceAccess"].Width = 170;
            UsersDataGrid.Columns["Coding"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            UsersDataGrid.Columns["Coding"].Width = 170;
            UsersDataGrid.Columns["Rating"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            UsersDataGrid.Columns["Rating"].Width = 100;

            UsersDataGrid.Columns["PriceAccess"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            UsersDataGrid.Columns["Coding"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        public void Initialize()
        {
            Create();
            Fill();
            Binding();
            GridSettings();
        }

        public bool IsChecked(string ColumnName, int ID)
        {
            return Convert.ToBoolean(UsersDataTable.Select("UserID = " + ID)[0][ColumnName]);
        }

        public void Check(string ColumnName, int ID, bool Check)
        {
            UsersDataTable.Select("UserID = " + ID)[0][ColumnName] = Check;
        }

        public void Save()
        {
            DataTable prDT = UsersDataTable.Copy();
            using (SqlDataAdapter prDA = new SqlDataAdapter("SELECT UserID, Name, ShortName, FirstName, LastName, MiddleName, Password, PriceAccess, Coding, AccessToken, Email, Fired FROM Users", ConnectionStrings.UsersConnectionString))
            {
                using (SqlCommandBuilder prCB = new SqlCommandBuilder(prDA))
                {
                    prDA.Update(prDT);
                }
            }
            prDT.Dispose();
            UsersDataTable.Clear();
            UsersDataAdapter.Fill(UsersDataTable);
            TablesManager.FillUsersDataTable();
        }

        private int GetNextUserID()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MAX(UserID) AS NEXT FROM Users",
                ConnectionStrings.UsersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    if (DT.Rows[0]["Next"] == DBNull.Value)
                        return 1;

                    return Convert.ToInt32(DT.Rows[0]["Next"]) + 1;
                }
            }
        }

        private void SeparateFullName()
        {
            for (int i = 0; i < UsersDataTable.Rows.Count; i++)
            {
                string fullName = UsersDataTable.Rows[i]["Name"].ToString();
                string firstName = UsersDataTable.Rows[i]["Name"].ToString();
                string lastName = string.Empty;
                string middleName = string.Empty; ;
                var names = fullName.Split(' ');

                if (names.Count() == 2)
                {
                    firstName = names[0];
                    lastName = names[1];
                }
                if (names.Count() == 3)
                {
                    firstName = names[0];
                    lastName = names[1];
                    middleName = names[2];
                }
                UsersDataTable.Rows[i]["FirstName"] = firstName;
                UsersDataTable.Rows[i]["LastName"] = lastName;
                UsersDataTable.Rows[i]["MiddleName"] = middleName;
            }
            UsersDataAdapter.Update(UsersDataTable);
        }

        public void Add(string Name)
        {
            string fullName = Name;
            string firstName = Name;
            string lastName = string.Empty;
            string middleName = string.Empty;
            var names = fullName.Split(' ');
            if (names.Count() == 1)
                lastName = names[0];
            if (names.Count() == 2)
            {
                lastName = names[0];
                firstName = names[1];
            }
            if (names.Count() == 3)
            {
                lastName = names[0];
                firstName = names[1];
                middleName = names[2];
            }

            string ShortName = Name;
            if (Name.IndexOf(' ') > -1)
            {
                if (lastName.Length > 0)
                    ShortName = lastName;
                if (firstName.Length > 0)
                    ShortName += " " + firstName.Substring(0, 1) + ".";
                if (middleName.Length > 0)
                    ShortName += middleName.Substring(0, 1) + ".";
            }
            DataRow NewRow = UsersDataTable.NewRow();
            NewRow["UserID"] = GetNextUserID();
            NewRow["Name"] = Name;
            NewRow["FirstName"] = firstName;
            NewRow["LastName"] = lastName;
            NewRow["MiddleName"] = middleName;
            NewRow["ShortName"] = ShortName;
            NewRow["OnlineRefreshDateTime"] = DateTime.Now;
            NewRow["Password"] = "4ba29b9f9e5732ed33761840f4ba6c53";
            UsersDataTable.Rows.Add(NewRow);

            UsersDataAdapter.Update(UsersDataTable);
            UsersDataTable.Clear();
            UsersDataAdapter.Fill(UsersDataTable);
            TablesManager.FillUsersDataTable();

            Rating();

            UsersBindingSource.Position = UsersBindingSource.Find("Name", Name);
        }

        public void Delete()
        {
            if (UsersBindingSource.Count == 0)
                return;

            UsersBindingSource.RemoveCurrent();
            UsersDataAdapter.Update(UsersDataTable);
        }

        public void Rating()
        {
            foreach (DataRow Row in UsersDataTable.Rows)
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT Count(ModulesJournalID) AS Count FROM ModulesJournal WHERE UserID = " + Row["UserID"],
                                                             ConnectionStrings.UsersConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        Row["Rating"] = DT.Rows[0]["Count"];
                    }
                }
            }
        }

    }






    public class AdminModulesAccess
    {
        public DataTable UsersDataTable;
        public DataTable ModulesDataTable;
        public DataTable ModulesAccessDataTable;

        PercentageDataGrid ModulesDataGrid;
        PercentageDataGrid UsersDataGrid;

        public BindingSource ModulesBindingSource;
        public BindingSource UsersBindingSource;

        SqlDataAdapter ModulesAccessDataAdapter;
        SqlCommandBuilder ModulesAccessCommandBuilder;

        public AdminModulesAccess(ref PercentageDataGrid tModulesDataGrid, ref PercentageDataGrid tUsersDataGrid)
        {
            ModulesDataGrid = tModulesDataGrid;
            UsersDataGrid = tUsersDataGrid;

            Initialize();
        }

        private void Create()
        {
            UsersDataTable = new DataTable();


            ModulesAccessDataTable = new DataTable();

            ModulesDataTable = new DataTable();

        }

        private void Fill()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ModuleID, ModuleName, SecurityAccess, PriceView FROM Modules", ConnectionStrings.UsersConnectionString))
            {
                DA.Fill(ModulesDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT UserID, Name, PriceAccess FROM Users  WHERE Fired <> 1 ORDER BY Name ASC", ConnectionStrings.UsersConnectionString))
            {
                DA.Fill(UsersDataTable);
            }

            ModulesAccessDataAdapter = new SqlDataAdapter("SELECT * FROM ModulesAccess", ConnectionStrings.UsersConnectionString);
            ModulesAccessCommandBuilder = new SqlCommandBuilder(ModulesAccessDataAdapter);
            ModulesAccessDataAdapter.Fill(ModulesAccessDataTable);

            ModulesDataTable.Columns.Add(new DataColumn("Access", Type.GetType("System.Boolean")));
        }


        private void Binding()
        {
            UsersBindingSource = new BindingSource()
            {
                DataSource = UsersDataTable
            };
            UsersDataGrid.DataSource = UsersBindingSource;

            ModulesBindingSource = new BindingSource()
            {
                DataSource = ModulesDataTable
            };
            ModulesDataGrid.DataSource = ModulesBindingSource;
        }

        private void GridSettings()
        {
            ModulesDataGrid.AutoGenerateColumns = false;
            ModulesDataGrid.Columns["ModuleID"].DisplayIndex = 0;
            ModulesDataGrid.Columns["ModuleName"].DisplayIndex = 1;
            ModulesDataGrid.Columns["Access"].DisplayIndex = 2;
            ModulesDataGrid.Columns["SecurityAccess"].DisplayIndex = 3;
            ModulesDataGrid.Columns["PriceView"].DisplayIndex = 4;

            ModulesDataGrid.Columns["ModuleID"].HeaderText = "ID";
            ModulesDataGrid.Columns["ModuleName"].HeaderText = "Название";
            ModulesDataGrid.Columns["Access"].HeaderText = "Доступ";
            ModulesDataGrid.Columns["SecurityAccess"].HeaderText = "Ограничения";
            ModulesDataGrid.Columns["PriceView"].HeaderText = "Цены";

            ModulesDataGrid.Columns["ModuleID"].ReadOnly = true;
            ModulesDataGrid.Columns["ModuleName"].ReadOnly = true;
            ModulesDataGrid.Columns["SecurityAccess"].ReadOnly = true;
            ModulesDataGrid.Columns["PriceView"].ReadOnly = true;

            ModulesDataGrid.Columns["ModuleID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            ModulesDataGrid.Columns["ModuleID"].Width = 50;
            ModulesDataGrid.Columns["Access"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            ModulesDataGrid.Columns["Access"].Width = 75;
            ModulesDataGrid.Columns["SecurityAccess"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            ModulesDataGrid.Columns["SecurityAccess"].Width = 120;
            ModulesDataGrid.Columns["PriceView"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            ModulesDataGrid.Columns["PriceView"].Width = 70;

            ModulesDataGrid.Columns["Access"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ModulesDataGrid.Columns["SecurityAccess"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            ModulesDataGrid.Columns["PriceView"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;


            UsersDataGrid.AutoGenerateColumns = false;

            UsersDataGrid.Columns["UserID"].HeaderText = "ID";
            UsersDataGrid.Columns["PriceAccess"].HeaderText = "Цены";

            UsersDataGrid.Columns["UserID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            UsersDataGrid.Columns["UserID"].Width = 50;


            UsersDataGrid.Columns["PriceAccess"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            UsersDataGrid.Columns["PriceAccess"].Width = 60;

            UsersDataGrid.Columns["PriceAccess"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        public void Initialize()
        {
            Create();
            Fill();
            Binding();
            GridSettings();
        }

        public bool IsCheckedModule(string ColumnName, int ID)
        {
            return Convert.ToBoolean(ModulesDataTable.Select("ModuleID = " + ID)[0][ColumnName]);
        }

        public void CheckModule(string ColumnName, int ID, bool Check)
        {
            ModulesDataTable.Select("ModuleID = " + ID)[0][ColumnName] = Check;
        }

        public bool IsCheckedUser(string ColumnName, int ID)
        {
            return Convert.ToBoolean(UsersDataTable.Select("UserID = " + ID)[0][ColumnName]);
        }

        public void Save()
        {
            foreach (DataRow Row in ModulesDataTable.Rows)
            {
                int UserID = Convert.ToInt32(((DataRowView)UsersBindingSource.Current)["UserID"]);

                if (Convert.ToBoolean(Row["Access"]) == true)
                {
                    if (ModulesAccessDataTable.Select("UserID = " + UserID + "AND ModuleID = " + Row["ModuleID"]).Count() == 0)
                    {
                        DataRow NewRow = ModulesAccessDataTable.NewRow();
                        NewRow["UserID"] = UserID;
                        NewRow["ModuleID"] = Row["ModuleID"];
                        ModulesAccessDataTable.Rows.Add(NewRow);
                    }
                }
                if (Convert.ToBoolean(Row["Access"]) == false)
                {
                    if (ModulesAccessDataTable.Select("UserID = " + UserID + "AND ModuleID = " + Row["ModuleID"]).Count() > 0)
                    {
                        ModulesAccessDataTable.Select("UserID = " + UserID + "AND ModuleID = " + Row["ModuleID"])[0].Delete();
                    }
                }
            }

            ModulesAccessDataAdapter.Update(ModulesAccessDataTable);
        }

        public void Filter()
        {
            int UserID = Convert.ToInt32(((DataRowView)UsersBindingSource.Current)["UserID"]);

            foreach (DataRow Row in ModulesDataTable.Rows)
            {
                if (ModulesAccessDataTable.Select("UserID = " + UserID + "AND ModuleID = " + Row["ModuleID"]).Count() > 0)
                    Row["Access"] = true;
                else
                    Row["Access"] = false;
            }
        }
    }






    public class AdminLoginJournal
    {
        DataTable LoginJournalDataTable;
        DataTable UsersDataTable;

        PercentageDataGrid JournalDataGrid;

        public BindingSource UsersBingingSource = null;
        public BindingSource LoginJournalBingingSource = null;


        private DataGridViewComboBoxColumn UserColumn = null;


        public AdminLoginJournal(ref PercentageDataGrid tJournalDataGrid)
        {
            JournalDataGrid = tJournalDataGrid;

            Initialize();
        }

        private void Create()
        {
            LoginJournalDataTable = new DataTable();
            UsersDataTable = new DataTable();
        }

        private void Fill()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT UserID, Name, ShortName FROM Users  WHERE Fired <> 1 ORDER BY Name ASC", ConnectionStrings.UsersConnectionString))
            {
                DA.Fill(UsersDataTable);
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 0 * FROM LoginJournal", ConnectionStrings.UsersConnectionString))
            {
                DA.Fill(LoginJournalDataTable);
            }
        }


        private void Binding()
        {
            UsersBingingSource = new BindingSource()
            {
                DataSource = UsersDataTable
            };
            LoginJournalBingingSource = new BindingSource()
            {
                DataSource = LoginJournalDataTable
            };
            JournalDataGrid.DataSource = LoginJournalBingingSource;
        }

        private void CreateColumns()
        {
            UserColumn = new DataGridViewComboBoxColumn()
            {
                Name = "UserColumn",
                HeaderText = "Пользователь",
                DataPropertyName = "UserID",
                DataSource = new DataView(UsersDataTable),
                ValueMember = "UserID",
                DisplayMember = "Name",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
            JournalDataGrid.Columns.Add(UserColumn);
        }

        private void GridSettings()
        {
            foreach (DataGridViewColumn Column in JournalDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            JournalDataGrid.Columns["LoginJournalID"].Visible = false;
            JournalDataGrid.Columns["UserID"].Visible = false;

            JournalDataGrid.Columns["DateEnter"].HeaderText = "Дата входа";
            JournalDataGrid.Columns["DateEnter"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            JournalDataGrid.Columns["DateEnter"].Width = 160;
            JournalDataGrid.Columns["DateExit"].HeaderText = "Дата выхода";
            JournalDataGrid.Columns["DateExit"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            JournalDataGrid.Columns["DateExit"].Width = 160;

            JournalDataGrid.Columns["DateEnter"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm:ss";
            JournalDataGrid.Columns["DateExit"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm:ss";

            JournalDataGrid.Columns["UserColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            JournalDataGrid.Columns["UserColumn"].MinimumWidth = 260;
        }

        public void Initialize()
        {
            Create();
            Fill();
            Binding();
            CreateColumns();
            GridSettings();
        }

        public void Filter(int UserID, DateTime DateFrom, DateTime DateTo, bool Coders)
        {
            string FilterExpr = "";


            if (UserID > 0)
                FilterExpr += " WHERE UserID = " + UserID.ToString();


            if (!Coders)
                if (FilterExpr.Length > 0)
                    FilterExpr += " AND UserID NOT IN (SELECT UserID FROM Users WHERE Coding = 1)";
                else
                    FilterExpr += "WHERE UserID NOT IN (SELECT UserID FROM Users WHERE Coding = 1)";

            if (FilterExpr.Length > 0)
                FilterExpr += " AND (DateEnter >= '" + DateFrom.ToString("yyyy-MM-dd") + "' AND DateEnter <= '" + DateTo.ToString("yyyy-MM-dd") + " 23:59:59.996')";
            else
                FilterExpr += "WHERE (DateEnter >= '" + DateFrom.ToString("yyyy-MM-dd") + "' AND DateEnter <= '" + DateTo.ToString("yyyy-MM-dd") + " 23:59:59.996')";

            LoginJournalDataTable.Clear();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM LoginJournal " + FilterExpr + " ORDER BY DateEnter ASC", ConnectionStrings.UsersConnectionString))
            {
                DA.Fill(LoginJournalDataTable);
            }
        }
    }






    public class AdminDepartmentEdit
    {
        public DataTable UsersDataTable;
        public BindingSource UsersBindingSource;
        SqlDataAdapter DA;
        SqlCommandBuilder CB;

        SqlDataAdapter DepartmentsDA;
        SqlCommandBuilder DepartmentsCB;
        public DataTable DepartmentsDataTable;
        public DataTable PositionsDataTable;

        private DataGridViewComboBoxColumn DepartmentColumn = null;

        public AdminDepartmentEdit()
        {
            UsersDataTable = new DataTable();
            DA = new SqlDataAdapter(@"SELECT StaffList.DepartmentID, Users.Name, Positions.Position, Factory.FactoryName FROM StaffList 
                INNER JOIN Positions ON StaffList.PositionID = Positions.PositionID 
                INNER JOIN infiniu2_users.dbo.Users AS Users ON StaffList.UserID = Users.UserID
                INNER JOIN infiniu2_catalog.dbo.Factory AS Factory ON StaffList.FactoryID = Factory.FactoryID
                ORDER BY Name, Position, FactoryName", ConnectionStrings.LightConnectionString);
            CB = new SqlCommandBuilder(DA);
            DA.Fill(UsersDataTable);

            UsersBindingSource = new BindingSource()
            {
                DataSource = UsersDataTable
            };
            DepartmentsDataTable = new DataTable();

            PositionsDataTable = new DataTable();

            using (SqlDataAdapter pDA = new SqlDataAdapter("SELECT * FROM Positions", ConnectionStrings.LightConnectionString))
            {
                pDA.Fill(PositionsDataTable);
            }


            DepartmentsDA = new SqlDataAdapter("SELECT DepartmentID, DepartmentName FROM Departments ORDER BY DepartmentName", ConnectionStrings.LightConnectionString);
            DepartmentsCB = new SqlCommandBuilder(DepartmentsDA);
            DepartmentsDA.Fill(DepartmentsDataTable);

            DepartmentColumn = new DataGridViewComboBoxColumn()
            {
                Name = "DepartmentColumn",
                HeaderText = "Группа",
                DataPropertyName = "DepartmentID",
                DataSource = new DataView(DepartmentsDataTable),
                ValueMember = "DepartmentID",
                DisplayMember = "DepartmentName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
            };
        }

        public Image GetPhoto(int DepartmentID)
        {
            DataRow Row = TablesManager.DepartmentsDataTable.Select("DepartmentID = " + DepartmentID)[0];

            if (Row["Photo"] == DBNull.Value)
                return null;

            byte[] b = (byte[])Row["Photo"];
            MemoryStream ms = new MemoryStream(b);

            return Image.FromStream(ms);
        }

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        public void SetDepartmentPhoto(Image Photo, int DepartmentID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DepartmentID, Photo FROM Departments WHERE DepartmentID = " + DepartmentID, ConnectionStrings.LightConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (Photo != null)
                        {
                            MemoryStream ms = new MemoryStream();

                            ImageCodecInfo ImageCodecInfo = GetEncoderInfo("image/jpeg");
                            System.Drawing.Imaging.Encoder eEncoder1 = System.Drawing.Imaging.Encoder.Quality;

                            EncoderParameter myEncoderParameter1 = new EncoderParameter(eEncoder1, 100L);
                            EncoderParameters myEncoderParameters = new EncoderParameters(1);

                            myEncoderParameters.Param[0] = myEncoderParameter1;

                            Photo.Save(ms, ImageCodecInfo, myEncoderParameters);

                            DT.Rows[0]["Photo"] = ms.ToArray();
                            ms.Dispose();
                        }
                        else
                            DT.Rows[0]["Photo"] = DBNull.Value;

                        DA.Update(DT);
                    }
                }
            }

            TablesManager.FillDepartmentsDataTable();
        }

        public void AddDepartment(string Name)
        {
            DataRow NewRow = DepartmentsDataTable.NewRow();
            NewRow["DepartmentName"] = Name;
            DepartmentsDataTable.Rows.Add(NewRow);

            DepartmentsDA.Update(DepartmentsDataTable);

            DepartmentsDataTable.Clear();

            DepartmentsDA.Fill(DepartmentsDataTable);

            TablesManager.FillDepartmentsDataTable();
        }

        public void SaveDepartments()
        {
            DepartmentsDA.Update(DepartmentsDataTable);
            DepartmentsDataTable.Clear();
            DepartmentsDA.Fill(DepartmentsDataTable);
        }
    }



    public class AdminPositionsEdit
    {
        public DataTable UsersDataTable;
        public BindingSource UsersBindingSource;
        SqlDataAdapter DA;
        SqlCommandBuilder CB;

        SqlDataAdapter PositionsDA;
        SqlCommandBuilder PositionsCB;
        public DataTable PositionsDataTable;

        public AdminPositionsEdit()
        {
            UsersDataTable = new DataTable();
            DA = new SqlDataAdapter(@"SELECT StaffList.PositionID, Users.Name, Departments.DepartmentName, Factory.FactoryName FROM StaffList 
                INNER JOIN Departments ON StaffList.DepartmentID = Departments.DepartmentID 
                INNER JOIN infiniu2_users.dbo.Users AS Users ON StaffList.UserID = Users.UserID
                INNER JOIN infiniu2_catalog.dbo.Factory AS Factory ON StaffList.FactoryID = Factory.FactoryID
                ORDER BY Name, DepartmentName, FactoryName", ConnectionStrings.LightConnectionString);
            CB = new SqlCommandBuilder(DA);
            DA.Fill(UsersDataTable);

            UsersBindingSource = new BindingSource()
            {
                DataSource = UsersDataTable
            };
            PositionsDataTable = new DataTable();
            PositionsDA = new SqlDataAdapter("SELECT PositionID, Position FROM Positions ORDER BY Position", ConnectionStrings.LightConnectionString);
            PositionsCB = new SqlCommandBuilder(PositionsDA);
            PositionsDA.Fill(PositionsDataTable);
        }

        public void AddPosition(string Name)
        {
            DataRow NewRow = PositionsDataTable.NewRow();
            NewRow["Position"] = Name;
            PositionsDataTable.Rows.Add(NewRow);

            PositionsDA.Update(PositionsDataTable);

            PositionsDataTable.Clear();

            PositionsDA.Fill(PositionsDataTable);

            TablesManager.FillPositionsDataTable();
        }

    }





    public class AdminFunctionsEdit
    {
        public int CurrentDepartmentID = -1;
        public DataTable FunctionsDataTable;
        public DataTable UsersResponsibilitiesDT;
        public BindingSource FunctionsBindingSource;
        SqlDataAdapter FunctionsDA;
        SqlCommandBuilder FunctionsCB;

        SqlDataAdapter DepartmentsDA;
        public DataTable DepartmentsDataTable;

        public AdminFunctionsEdit()
        {
            UsersResponsibilitiesDT = new DataTable();
            string SelectCommand = @"SELECT UserFunctions.*, Functions.FunctionName FROM UserFunctions
                INNER JOIN Functions ON UserFunctions.FunctionID=Functions.FunctionID ORDER BY FunctionName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.LightConnectionString))
            {
                DA.Fill(UsersResponsibilitiesDT);
            }

            FunctionsDataTable = new DataTable();

            SelectCommand = @"SELECT DepartmentID, FunctionID, FunctionName, FunctionDescription FROM Functions
                ORDER BY FunctionName";
            FunctionsDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.LightConnectionString);
            FunctionsCB = new SqlCommandBuilder(FunctionsDA);
            FunctionsDA.Fill(FunctionsDataTable);

            FunctionsBindingSource = new BindingSource()
            {
                DataSource = FunctionsDataTable
            };
            DepartmentsDataTable = new DataTable();
            DepartmentsDA = new SqlDataAdapter("SELECT DepartmentID, DepartmentName FROM Departments ORDER BY DepartmentName", ConnectionStrings.LightConnectionString);
            DepartmentsDA.Fill(DepartmentsDataTable);
            DataRow NewRow = DepartmentsDataTable.NewRow();
            NewRow["DepartmentID"] = -1;
            NewRow["DepartmentName"] = "Общие обязанности";
            DepartmentsDataTable.Rows.InsertAt(NewRow, 0);
        }

        public void FilterFunctions(int DepartmentID)
        {
            FunctionsBindingSource.Filter = "DepartmentID=" + DepartmentID;
            FunctionsBindingSource.MoveFirst();
        }

        public void Save()
        {
            string SelectCommand = @"SELECT DepartmentID, FunctionID, FunctionName, FunctionDescription FROM Functions
                ORDER BY FunctionName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.LightConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(FunctionsDataTable);
                    FunctionsDataTable.Clear();
                    DA.Fill(FunctionsDataTable);
                }
            }
        }

        public bool IsFunctionUsing(int FunctionID)
        {
            DataRow[] Rows = UsersResponsibilitiesDT.Select("FunctionID = " + FunctionID);
            if (Rows.Count() > 0)
                return true;
            else
                return false;
        }

        public void AddFunction(int DepartmentID, string Name, string Description)
        {
            DataRow NewRow = FunctionsDataTable.NewRow();
            NewRow["DepartmentID"] = DepartmentID;
            NewRow["FunctionName"] = Name;
            NewRow["FunctionDescription"] = Description;
            FunctionsDataTable.Rows.Add(NewRow);

            FunctionsDA.Update(FunctionsDataTable);
            FunctionsDataTable.Clear();
            FunctionsDA.Fill(FunctionsDataTable);
        }

        public void EditFunction(int FunctionID, string Name, string Description)
        {
            DataRow[] Rows = FunctionsDataTable.Select("FunctionID = " + FunctionID);
            if (Rows.Count() > 0)
            {
                Rows[0]["FunctionName"] = Name;
                Rows[0]["FunctionDescription"] = Description;
            }
        }

        public void ChangeDepartment(int FunctionID, int DepartmentID)
        {
            DataRow[] Rows = FunctionsDataTable.Select("FunctionID = " + FunctionID);
            if (Rows.Count() > 0)
            {
                Rows[0]["DepartmentID"] = DepartmentID;
            }
        }

        public bool IsFunctionAlreadyAdded(int DepartmentID, string Function, string FunctionDescription)
        {
            DataRow[] Rows = FunctionsDataTable.Select("DepartmentID=" + DepartmentID + " AND FunctionName = '" + Function + "' AND FunctionDescription = '" + FunctionDescription + "'");
            if (Rows.Count() > 0)
                return true;
            else
                return false;
        }

        public void MoveToFunction(int FunctionID)
        {
            FunctionsBindingSource.Position = FunctionsBindingSource.Find("FunctionID", FunctionID);
        }

        public bool AddUsersResponsibility(int StaffListID, int UserID, int FunctionID)
        {
            string SelectCommand = @"SELECT * FROM UserFunctions WHERE StaffListID=" + StaffListID + " AND FunctionID=" + FunctionID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.LightConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable UsersResponsibilitiesDT = new DataTable())
                    {
                        if (DA.Fill(UsersResponsibilitiesDT) > 0)
                        {
                            return false;
                        }
                        DataRow NewRow = UsersResponsibilitiesDT.NewRow();
                        NewRow["StaffListID"] = StaffListID;
                        NewRow["UserID"] = UserID;
                        NewRow["FunctionID"] = FunctionID;
                        UsersResponsibilitiesDT.Rows.Add(NewRow);
                        DA.Update(UsersResponsibilitiesDT);
                    }
                }
            }
            return true;
        }

        public void DeleteFunction(int FunctionID)
        {
            DeleteUserFunction(FunctionID);
            using (SqlDataAdapter DA = new SqlDataAdapter("DELETE FROM Functions WHERE FunctionID = " + FunctionID,
                ConnectionStrings.LightConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                    }
                }
            }
        }

        public void DeleteUserFunction(int FunctionID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("DELETE FROM UserFunctions WHERE FunctionID = " + FunctionID,
                ConnectionStrings.LightConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                    }
                }
            }
        }

    }




    public class DateRatesEdit
    {
        public DataTable DateRatesDT;
        public BindingSource DateRatesBS;
        SqlDataAdapter DateRatesDA;
        SqlCommandBuilder DateRatesCB;

        public DateRatesEdit()
        {
            DateRatesDT = new DataTable();

            string SelectCommand = @"SELECT * FROM DateRates
                ORDER BY DateRateID DESC";
            DateRatesDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingReferenceConnectionString);
            DateRatesCB = new SqlCommandBuilder(DateRatesDA);
            DateRatesDA.Fill(DateRatesDT);

            DateRatesBS = new BindingSource()
            {
                DataSource = DateRatesDT
            };
        }

        public void SaveDateRates()
        {
            string SelectCommand = @"SELECT * FROM DateRates
                ORDER BY DateRateID DESC";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.MarketingReferenceConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(DateRatesDT);
                    DateRatesDT.Clear();
                    DA.Fill(DateRatesDT);
                }
            }
        }

        public void SaveDateRates(DateTime DateTime, decimal USD, decimal RUB, decimal BYN, decimal USDRUB)
        {
            using (DataTable DT = new DataTable())
            {
                using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT TOP 0 * FROM DateRates WHERE CAST(Date AS Date) = 
                    '" + DateTime.ToString("yyyy-MM-dd") + "' ORDER BY DateRateID DESC",
                    ConnectionStrings.MarketingReferenceConnectionString))
                {
                    using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                    {
                        DA.Fill(DT);

                        DataRow Row = DT.NewRow();
                        Row["Date"] = DateTime;
                        Row["USD"] = USD;
                        Row["RUB"] = RUB;
                        Row["BYN"] = BYN;
                        Row["USDRUB"] = USDRUB;
                        DT.Rows.Add(Row);
                        DA.Update(DT);
                    }
                }
            }
            return;
        }

        public bool NBRBDailyRates(DateTime date, ref decimal EURBYRCurrency)
        {
            string EuroXML = "";
            string url = "http://www.nbrb.by/Services/XmlExRates.aspx?ondate=" + date.ToString("MM.dd.yyyy");

            HttpWebRequest myHttpWebRequest;
            HttpWebResponse myHttpWebResponse;

            try
            {
                myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                myHttpWebRequest.KeepAlive = false;
                myHttpWebRequest.AllowAutoRedirect = true;
                CookieContainer cookieContainer = new CookieContainer();
                myHttpWebRequest.CookieContainer = cookieContainer;
                myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
            }
            catch
            {
                return false;
            }

            XmlTextReader reader = new XmlTextReader(myHttpWebResponse.GetResponseStream());
            //XmlTextReader reader = new XmlTextReader("http://www.nbrb.by/Services/XmlExRates.aspx?ondate=" + date.ToString("MM/dd/yyyy"));
            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name == "Currency")
                            {
                                if (reader.HasAttributes)
                                {
                                    while (reader.MoveToNextAttribute())
                                    {
                                        if (reader.Name == "Id")
                                        {
                                            if (reader.Value == "292")
                                            {
                                                reader.MoveToElement();
                                                EuroXML = reader.ReadOuterXml();
                                            }
                                        }
                                        if (reader.Name == "Id")
                                        {
                                            if (reader.Value == "19")
                                            {
                                                reader.MoveToElement();
                                                EuroXML = reader.ReadOuterXml();
                                            }
                                        }
                                    }
                                }
                            }

                            break;
                    }
                }
                XmlDocument euroXmlDocument = new XmlDocument();
                euroXmlDocument.LoadXml(EuroXML);
                XmlNode xmlNode = euroXmlDocument.SelectSingleNode("Currency/Rate");
                bool b = decimal.TryParse(xmlNode.InnerText, out EURBYRCurrency);
                if (!b)
                    EURBYRCurrency = Convert.ToDecimal(xmlNode.InnerText = xmlNode.InnerText.Replace('.', ','));
                else
                    EURBYRCurrency = Convert.ToDecimal(xmlNode.InnerText);
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message + " . КУРСЫ МОЖНО БУДЕТ ВВЕСТИ ВРУЧНУЮ");
                return false;
            }
            return true;
        }

        public bool CBRDailyRates(DateTime date, ref decimal EURRUBCurrency, ref decimal USDRUBCurrency)
        {
            string EuroXML = "";
            string USDXml = "";

            string url = "http://www.cbr.ru/scripts/XML_daily.asp?date_req=" + date.ToString("dd/MM/yyyy");

            HttpWebRequest myHttpWebRequest;
            HttpWebResponse myHttpWebResponse;

            try
            {
                myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
                myHttpWebRequest.KeepAlive = false;
                myHttpWebRequest.AllowAutoRedirect = true;
                CookieContainer cookieContainer = new CookieContainer();
                myHttpWebRequest.CookieContainer = cookieContainer;
                myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
            }
            catch
            {
                return false;
            }

            XmlTextReader reader1 = new XmlTextReader(myHttpWebResponse.GetResponseStream());

            try
            {
                while (reader1.Read())
                {
                    switch (reader1.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader1.Name == "Valute")
                            {
                                if (reader1.HasAttributes)
                                {
                                    while (reader1.MoveToNextAttribute())
                                    {
                                        if (reader1.Name == "ID")
                                        {
                                            if (reader1.Value == "R01239")
                                            {
                                                reader1.MoveToElement();
                                                EuroXML = reader1.ReadOuterXml();
                                            }
                                        }
                                        if (reader1.Name == "ID")
                                        {
                                            if (reader1.Value == "R01235")
                                            {
                                                reader1.MoveToElement();
                                                USDXml = reader1.ReadOuterXml();
                                            }
                                        }
                                    }
                                }
                            }

                            break;
                    }
                }
                XmlDocument euroXmlDocument = new XmlDocument();
                euroXmlDocument.LoadXml(EuroXML);
                XmlDocument usdXmlDocument = new XmlDocument();
                usdXmlDocument.LoadXml(USDXml);

                XmlNode xmlNode = euroXmlDocument.SelectSingleNode("Valute/Value");
                EURRUBCurrency = Convert.ToDecimal(xmlNode.InnerText = xmlNode.InnerText.Replace('.', ','));
                xmlNode = usdXmlDocument.SelectSingleNode("Valute/Value");
                USDRUBCurrency = Convert.ToDecimal(xmlNode.InnerText = xmlNode.InnerText.Replace('.', ','));
            }
            catch (WebException ex)
            {
                MessageBox.Show(ex.Message + " . КУРСЫ МОЖНО БУДЕТ ВВЕСТИ ВРУЧНУЮ");
                return false;
            }
            return true;
        }

        public bool GetDateRates(DateTime ConfirmDate)
        {
            bool CBR = true;
            bool NBRB = true;
            decimal EURRUBCurrency = 1000000;
            decimal USDRUBCurrency = 1000000;
            decimal EURUSDCurrency = 1000000;
            decimal EURBYRCurrency = 1000000;

            CBR = CBRDailyRates(ConfirmDate, ref EURRUBCurrency, ref USDRUBCurrency);
            NBRB = NBRBDailyRates(ConfirmDate, ref EURBYRCurrency);

            if (USDRUBCurrency != 0)
                EURUSDCurrency = Decimal.Round(EURRUBCurrency / USDRUBCurrency, 4, MidpointRounding.AwayFromZero);

            if (EURBYRCurrency == 1000000)
            {
                return false;
            }

            SaveDateRates(Convert.ToDateTime(ConfirmDate), EURUSDCurrency, EURRUBCurrency, EURBYRCurrency, USDRUBCurrency);
            return true;
        }

        public void EditDateRates(int DateRateID, string Name, string Description)
        {
            DataRow[] Rows = DateRatesDT.Select("DateRateID = " + DateRateID);
            if (Rows.Count() > 0)
            {
                Rows[0]["FunctionName"] = Name;
                Rows[0]["FunctionDescription"] = Description;
            }
        }

        public void DeleteDateRate(int DateRateID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("DELETE FROM DateRates WHERE DateRateID = " + DateRateID,
                ConnectionStrings.MarketingReferenceConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                    }
                }
            }
        }
    }




    public class WorkTimeRegister
    {
        private DataTable WorkDaysDataTable;

        public BindingSource WorkDayDetailsBindingSource;
        public BindingSource WorkDaysBindingSource;

        PercentageDataGrid WorkDaysGrid;

        int CurrentUserID = -1;
        int CurrentWorkDayID = -1;
        public int sDayNotStarted = 0;
        public int sDayStarted = 1;
        public int sBreakStarted = 2;
        public int sDayContinued = 3;
        public int sDayEnded = 4;
        public int sDaySaved = 5;

        public WorkTimeRegister(ref PercentageDataGrid tWorkDaysGrid)
        {
            WorkDaysGrid = tWorkDaysGrid;

            Create();
            Fill();
            Binding();
            GridSettings();
        }

        public struct DayStatus
        {
            public int iDayStatus;
            public DateTime DayStarted;
            public DateTime BreakStarted;
            public DateTime BreakEnded;
            public DateTime DayEnded;
            public bool bBreak;
            public bool bOverdued;
        }

        private void Create()
        {
            WorkDaysDataTable = new DataTable();

            WorkDayDetailsBindingSource = new BindingSource();
            WorkDaysBindingSource = new BindingSource();
        }

        private void Fill()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT WorkDays.*, Name FROM WorkDays" +
                " INNER JOIN infiniu2_users.dbo.Users ON WorkDays.UserID = infiniu2_users.dbo.Users.UserID" +
                " WHERE DayStartFactDateTime >= '" +
                GetCurrentDate.ToString("yyyy-MM-dd") + "' AND DayStartFactDateTime <= '" + GetCurrentDate.ToString("yyyy-MM-dd") + " 23:59:59.996'" +
                " ORDER BY Name",
                ConnectionStrings.LightConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(WorkDaysDataTable);
                }
            }
        }

        private void Binding()
        {
            WorkDaysBindingSource.DataSource = WorkDaysDataTable;

            WorkDaysGrid.DataSource = WorkDaysBindingSource;
        }

        private void GridSettings()
        {
            if (WorkDaysGrid != null)
            {
                foreach (DataGridViewColumn Column in WorkDaysGrid.Columns)
                {
                    Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                WorkDaysGrid.Columns["WorkDayID"].Visible = false;
                WorkDaysGrid.Columns["UserID"].Visible = false;
                //WorkDaysGrid.Columns["DayStartDateTime"].Visible = false;
                //WorkDaysGrid.Columns["DayEndDateTime"].Visible = false;
                //WorkDaysGrid.Columns["DayBreakStartDateTime"].Visible = false;
                //WorkDaysGrid.Columns["DayBreakEndDateTime"].Visible = false;
                //WorkDaysGrid.Columns["DayBreakStartFactDateTime"].Visible = false;
                //WorkDaysGrid.Columns["DayBreakEndFactDateTime"].Visible = false;

                WorkDaysGrid.Columns["DayStartNotes"].Visible = false;
                WorkDaysGrid.Columns["DayBreakStartNotes"].Visible = false;
                WorkDaysGrid.Columns["DayContinueNotes"].Visible = false;
                WorkDaysGrid.Columns["DayEndNotes"].Visible = false;
                WorkDaysGrid.Columns["Saved"].Visible = false;

                WorkDaysGrid.Columns["Name"].HeaderText = "Сотрудник";
                WorkDaysGrid.Columns["DayStartDateTime"].HeaderText = "Начало";
                WorkDaysGrid.Columns["DayEndDateTime"].HeaderText = "Завершение";
                WorkDaysGrid.Columns["DayBreakStartDateTime"].HeaderText = " Начало\r\nперерыва";
                WorkDaysGrid.Columns["DayBreakEndDateTime"].HeaderText = "Завершение\r\n  перерыва";
                WorkDaysGrid.Columns["DayStartFactDateTime"].HeaderText = "      Начало\r\n(фактическое)";
                WorkDaysGrid.Columns["DayEndFactDateTime"].HeaderText = "  Завершение\r\n(фактическое)";
                WorkDaysGrid.Columns["DayBreakStartFactDateTime"].HeaderText = "Начало перерыва\r\n    (фактическое)";
                WorkDaysGrid.Columns["DayBreakEndFactDateTime"].HeaderText = "Завершение перерыва\r\n        (фактическое)";

                WorkDaysGrid.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                WorkDaysGrid.Columns["Name"].MinimumWidth = 290;
                WorkDaysGrid.Columns["DayStartDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                WorkDaysGrid.Columns["DayStartDateTime"].Width = 140;
                WorkDaysGrid.Columns["DayEndDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                WorkDaysGrid.Columns["DayEndDateTime"].Width = 140;
                WorkDaysGrid.Columns["DayBreakStartDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                WorkDaysGrid.Columns["DayBreakStartDateTime"].Width = 140;
                WorkDaysGrid.Columns["DayBreakEndDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                WorkDaysGrid.Columns["DayBreakEndDateTime"].Width = 140;
                WorkDaysGrid.Columns["DayStartFactDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                WorkDaysGrid.Columns["DayStartFactDateTime"].Width = 140;
                WorkDaysGrid.Columns["DayEndFactDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                WorkDaysGrid.Columns["DayEndFactDateTime"].Width = 140;
                WorkDaysGrid.Columns["DayBreakStartFactDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                WorkDaysGrid.Columns["DayBreakStartFactDateTime"].Width = 170;
                WorkDaysGrid.Columns["DayBreakEndFactDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                WorkDaysGrid.Columns["DayBreakEndFactDateTime"].Width = 200;

                WorkDaysGrid.Columns["Name"].DisplayIndex = 0;
                WorkDaysGrid.Columns["DayStartDateTime"].DisplayIndex = 1;
                WorkDaysGrid.Columns["DayEndDateTime"].DisplayIndex = 2;
                WorkDaysGrid.Columns["DayBreakStartDateTime"].DisplayIndex = 3;
                WorkDaysGrid.Columns["DayBreakEndDateTime"].DisplayIndex = 4;
                WorkDaysGrid.Columns["DayStartFactDateTime"].DisplayIndex = 4;
                WorkDaysGrid.Columns["DayEndFactDateTime"].DisplayIndex = 5;
                WorkDaysGrid.Columns["DayBreakStartFactDateTime"].DisplayIndex = 6;
                WorkDaysGrid.Columns["DayBreakEndFactDateTime"].DisplayIndex = 7;
            }
        }

        public void FilterWorkDays(DateTime Date)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT WorkDays.*, infiniu2_users.dbo.Users.Name FROM WorkDays" +
                " INNER JOIN infiniu2_users.dbo.Users ON WorkDays.UserID = infiniu2_users.dbo.Users.UserID" +
                " WHERE DayStartFactDateTime >= '" +
                Date.ToString("yyyy-MM-dd") + "' AND DayStartFactDateTime <= '" + Date.ToString("yyyy-MM-dd") + " 23:59:59.996'" +
                " ORDER BY Name",
                ConnectionStrings.LightConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    WorkDaysDataTable.Clear();
                    DA.Fill(WorkDaysDataTable);
                }
            }
        }

        public DayStatus GetDayStatus(DateTime Date)
        {
            DayStatus DS = new DayStatus();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM WorkDays WHERE CAST(DayStartDateTime AS DATE) = '" + Date.ToString("yyyy-MM-dd") +
                "' AND UserID = " + UserID + " ORDER BY WorkDayID DESC", ConnectionStrings.LightConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count != 0)
                    {
                        DS.iDayStatus = sDayStarted;

                        DS.DayStarted = Convert.ToDateTime(DT.Rows[0]["DayStartDateTime"]);

                        if (DT.Rows[0]["DayBreakStartDateTime"] != DBNull.Value)
                        {
                            DS.iDayStatus = sBreakStarted;
                            DS.BreakStarted = Convert.ToDateTime(DT.Rows[0]["DayBreakStartDateTime"]);
                        }

                        if (DT.Rows[0]["DayBreakEndDateTime"] != DBNull.Value)
                        {
                            DS.iDayStatus = sDayContinued;
                            DS.BreakEnded = Convert.ToDateTime(DT.Rows[0]["DayBreakEndDateTime"]);
                        }

                        if (DT.Rows[0]["DayEndDateTime"] != DBNull.Value)
                        {
                            DS.iDayStatus = sDayEnded;

                            if (DT.Rows[0]["DayBreakStartDateTime"] != DBNull.Value)
                                DS.bBreak = true;
                            else
                                DS.bBreak = false;

                            DS.DayEnded = Convert.ToDateTime(DT.Rows[0]["DayEndDateTime"]);
                        }

                        if (Convert.ToBoolean(DT.Rows[0]["Saved"]) == true)
                            DS.iDayStatus = sDaySaved;
                    }
                    else
                        DS.iDayStatus = sDayNotStarted;
                }
            }

            return DS;
        }

        #region Properies

        public int UserID
        {
            get { return CurrentUserID; }
            set { CurrentUserID = value; }
        }

        public int WorkDayID
        {
            get { return CurrentWorkDayID; }
            set { CurrentWorkDayID = value; }
        }

        public DateTime GetCurrentDate
        {
            get
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT GETDATE()", ConnectionStrings.LightConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        return Convert.ToDateTime(DT.Rows[0][0]);
                    }
                }
            }
        }

        public string GetDayStartNotes
        {
            get
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DayStartNotes FROM WorkDays WHERE WorkDayID = " + CurrentWorkDayID,
                    ConnectionStrings.LightConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DT.Rows.Count < 1)
                            return "";

                        return DT.Rows[0][0].ToString();
                    }
                }
            }
        }

        public string GetDayBreakStartNotes
        {
            get
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DayBreakStartNotes FROM WorkDays WHERE WorkDayID = " + CurrentWorkDayID,
                    ConnectionStrings.LightConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DT.Rows.Count < 1)
                            return "";

                        return DT.Rows[0][0].ToString();
                    }
                }
            }
        }

        public string GetDayContinueNotes
        {
            get
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DayContinueNotes FROM WorkDays WHERE WorkDayID = " + CurrentWorkDayID,
                    ConnectionStrings.LightConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DT.Rows.Count < 1)
                            return "";

                        return DT.Rows[0][0].ToString();
                    }
                }
            }
        }

        public string GetDayEndNotes
        {
            get
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DayEndNotes FROM WorkDays WHERE WorkDayID = " + CurrentWorkDayID,
                    ConnectionStrings.LightConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DT.Rows.Count < 1)
                            return "";

                        return DT.Rows[0][0].ToString();
                    }
                }
            }
        }

        public string GetDayStartDateTime
        {
            get
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DayStartDateTime FROM WorkDays WHERE WorkDayID = " + CurrentWorkDayID,
                    ConnectionStrings.LightConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DT.Rows.Count > 0)
                            return DT.Rows[0][0].ToString();

                        return "";
                    }
                }
            }
        }

        public string GetDayEndDateTime
        {
            get
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DayEndDateTime FROM WorkDays WHERE WorkDayID = " + CurrentWorkDayID,
                    ConnectionStrings.LightConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DT.Rows.Count > 0)
                            return DT.Rows[0][0].ToString();

                        return "";
                    }
                }
            }
        }

        public string GetDayBreakStartDateTime
        {
            get
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DayBreakStartDateTime FROM WorkDays WHERE WorkDayID = " + CurrentWorkDayID,
                    ConnectionStrings.LightConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DT.Rows.Count > 0)
                            return DT.Rows[0][0].ToString();

                        return "";
                    }
                }
            }
        }

        public string GetDayBreakEndDateTime
        {
            get
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DayBreakEndDateTime FROM WorkDays WHERE WorkDayID = " + CurrentWorkDayID,
                    ConnectionStrings.LightConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DT.Rows.Count > 0)
                            return DT.Rows[0][0].ToString();

                        return "";
                    }
                }
            }
        }

        public string GetDayStartFactDateTime
        {
            get
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DayStartFactDateTime FROM WorkDays WHERE WorkDayID = " + CurrentWorkDayID,
                    ConnectionStrings.LightConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        if (DT.Rows.Count > 0)
                            return DT.Rows[0][0].ToString();

                        return "";
                    }
                }
            }
        }

        public string GetDayEndFactDateTime
        {
            get
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DayEndFactDateTime FROM WorkDays WHERE WorkDayID = " + CurrentWorkDayID,
                    ConnectionStrings.LightConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                        if (DT.Rows.Count > 0)
                            return DT.Rows[0][0].ToString();

                        return "";
                    }
                }
            }
        }

        public string GetDayBreakStartFactDateTime
        {
            get
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DayBreakStartFactDateTime FROM WorkDays WHERE WorkDayID = " + CurrentWorkDayID,
                    ConnectionStrings.LightConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DT.Rows.Count > 0)
                            return DT.Rows[0][0].ToString();

                        return "";
                    }
                }
            }
        }

        public string GetDayBreakEndFactDateTime
        {
            get
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DayBreakEndFactDateTime FROM WorkDays WHERE WorkDayID = " + CurrentWorkDayID,
                    ConnectionStrings.LightConnectionString))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        if (DT.Rows.Count > 0)
                            return DT.Rows[0][0].ToString();

                        return "";
                    }
                }
            }
        }

        public void SetOverduedColor()
        {
            for (int i = 0; i < WorkDaysGrid.Rows.Count; i++)
            {
                if (WorkDaysGrid.Rows[i].Cells["DayEndFactDateTime"].Value != DBNull.Value)
                {
                    if (Convert.ToDateTime(WorkDaysGrid.Rows[i].Cells["DayEndDateTime"].Value) !=
                        Convert.ToDateTime(WorkDaysGrid.Rows[i].Cells["DayEndFactDateTime"].Value))
                    {
                        WorkDaysGrid.Rows[i].Cells["DayEndDateTime"].Style.BackColor = Color.FromArgb(85, 200, 85);
                        WorkDaysGrid.Rows[i].Cells["DayEndFactDateTime"].Style.BackColor = Color.FromArgb(85, 200, 85);
                    }
                    else
                    {
                        WorkDaysGrid.Rows[i].Cells["DayEndDateTime"].Style.BackColor = Color.White;
                        WorkDaysGrid.Rows[i].Cells["DayEndFactDateTime"].Style.BackColor = Color.White;
                    }
                }
                else
                {
                    WorkDaysGrid.Rows[i].Cells["DayEndDateTime"].Style.BackColor = Color.White;
                    WorkDaysGrid.Rows[i].Cells["DayEndFactDateTime"].Style.BackColor = Color.White;
                }

                if (WorkDaysGrid.Rows[i].Cells["DayStartFactDateTime"].Value != DBNull.Value)
                {
                    if (Convert.ToDateTime(WorkDaysGrid.Rows[i].Cells["DayStartDateTime"].Value) !=
                        Convert.ToDateTime(WorkDaysGrid.Rows[i].Cells["DayStartFactDateTime"].Value))
                    {
                        WorkDaysGrid.Rows[i].Cells["DayStartDateTime"].Style.BackColor = Color.FromArgb(222, 222, 65);
                        WorkDaysGrid.Rows[i].Cells["DayStartFactDateTime"].Style.BackColor = Color.FromArgb(222, 222, 65);
                    }
                    else
                    {
                        WorkDaysGrid.Rows[i].Cells["DayStartDateTime"].Style.BackColor = Color.White;
                        WorkDaysGrid.Rows[i].Cells["DayStartFactDateTime"].Style.BackColor = Color.White;
                    }
                }
                else
                {
                    WorkDaysGrid.Rows[i].Cells["DayStartDateTime"].Style.BackColor = Color.White;
                    WorkDaysGrid.Rows[i].Cells["DayStartFactDateTime"].Style.BackColor = Color.White;
                }

                if (WorkDaysGrid.Rows[i].Cells["DayBreakEndDateTime"].Value != DBNull.Value &&
                    WorkDaysGrid.Rows[i].Cells["DayBreakEndFactDateTime"].Value != DBNull.Value)
                {
                    if (Convert.ToDateTime(WorkDaysGrid.Rows[i].Cells["DayBreakEndDateTime"].Value) !=
                        Convert.ToDateTime(WorkDaysGrid.Rows[i].Cells["DayBreakEndFactDateTime"].Value))
                    {
                        WorkDaysGrid.Rows[i].Cells["DayBreakEndDateTime"].Style.BackColor = Color.FromArgb(75, 120, 210);
                        WorkDaysGrid.Rows[i].Cells["DayBreakEndFactDateTime"].Style.BackColor = Color.FromArgb(75, 120, 210);
                    }
                    else
                    {
                        WorkDaysGrid.Rows[i].Cells["DayBreakEndDateTime"].Style.BackColor = Color.White;
                        WorkDaysGrid.Rows[i].Cells["DayBreakEndFactDateTime"].Style.BackColor = Color.White;
                    }
                }
                else
                {
                    WorkDaysGrid.Rows[i].Cells["DayBreakEndDateTime"].Style.BackColor = Color.White;
                    WorkDaysGrid.Rows[i].Cells["DayBreakEndFactDateTime"].Style.BackColor = Color.White;
                }

                if (WorkDaysGrid.Rows[i].Cells["DayBreakStartDateTime"].Value != DBNull.Value &&
                    WorkDaysGrid.Rows[i].Cells["DayBreakStartFactDateTime"].Value != DBNull.Value)
                {
                    if (Convert.ToDateTime(WorkDaysGrid.Rows[i].Cells["DayBreakStartDateTime"].Value) !=
                        Convert.ToDateTime(WorkDaysGrid.Rows[i].Cells["DayBreakStartFactDateTime"].Value))
                    {
                        WorkDaysGrid.Rows[i].Cells["DayBreakStartDateTime"].Style.BackColor = Color.FromArgb(85, 190, 190);
                        WorkDaysGrid.Rows[i].Cells["DayBreakStartFactDateTime"].Style.BackColor = Color.FromArgb(85, 190, 190);
                    }
                    else
                    {
                        WorkDaysGrid.Rows[i].Cells["DayBreakStartDateTime"].Style.BackColor = Color.White;
                        WorkDaysGrid.Rows[i].Cells["DayBreakStartFactDateTime"].Style.BackColor = Color.White;
                    }
                }
                else
                {
                    WorkDaysGrid.Rows[i].Cells["DayBreakStartDateTime"].Style.BackColor = Color.White;
                    WorkDaysGrid.Rows[i].Cells["DayBreakStartFactDateTime"].Style.BackColor = Color.White;
                }
            }
        }
        #endregion

        public static void EditBreakStartWorkDay(int WorkDayID, DateTime DateTime)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM WorkDays WHERE WorkDayID = " + WorkDayID, ConnectionStrings.LightConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) == 0)
                            return;

                        DT.Rows[0]["DayBreakStartDateTime"] = DateTime;
                        DT.Rows[0]["DayBreakStartFactDateTime"] = DateTime;
                        DA.Update(DT);

                        return;
                    }
                }
            }

        }

        public static void EditBreakEndWorkDay(int WorkDayID, DateTime DateTime)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM WorkDays WHERE WorkDayID = " + WorkDayID, ConnectionStrings.LightConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) == 0)
                            return;

                        DT.Rows[0]["DayBreakEndDateTime"] = DateTime;
                        DT.Rows[0]["DayBreakEndFactDateTime"] = DateTime;
                        DA.Update(DT);

                        return;
                    }
                }
            }

        }

        public static void EditStartWorkDay(int WorkDayID, DateTime DateTime)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM WorkDays WHERE WorkDayID = " + WorkDayID, ConnectionStrings.LightConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) == 0)
                            return;

                        DT.Rows[0]["DayStartDateTime"] = DateTime;
                        DT.Rows[0]["DayStartFactDateTime"] = DateTime;
                        DA.Update(DT);

                        return;
                    }
                }
            }

        }

        public static void EditEndWorkDay(int WorkDayID, DateTime DateTime)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM WorkDays WHERE WorkDayID = " + WorkDayID, ConnectionStrings.LightConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) == 0)
                            return;

                        DT.Rows[0]["DayEndDateTime"] = DateTime;
                        DT.Rows[0]["DayEndFactDateTime"] = DateTime;
                        DA.Update(DT);

                        return;
                    }
                }
            }

        }
    }






    public class WorkTimeSheet
    {
        public DataTable TimeSheetDataTable;
        private DataTable userTable;
        private DataTable dtTimeSheetNew;
        Excel Ex = null;

        public WorkTimeSheet()
        {
            Create();
            Fill();
        }

        private void Create()
        {
            dtTimeSheetNew = new DataTable();
            TimeSheetDataTable = new DataTable();
            userTable = new DataTable();
            userTable.Columns.Add(new DataColumn("Date", Type.GetType("System.DateTime")));
            userTable.Columns.Add(new DataColumn("UserID", Type.GetType("System.Int32")));
            userTable.Columns.Add(new DataColumn("WorkTime", Type.GetType("System.Decimal")));
            userTable.Columns.Add(new DataColumn("BreakTime", Type.GetType("System.Decimal")));
            userTable.Columns.Add(new DataColumn("PlanTime", Type.GetType("System.Decimal")));
            userTable.Columns.Add(new DataColumn("AbsenceTypeID", Type.GetType("System.Int32")));
            userTable.Columns.Add(new DataColumn("AbsenceTime", Type.GetType("System.Decimal")));
        }

        private void Fill()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT infiniu2_users.dbo.Users.Name, infiniu2_light.dbo.WorkDays.DayStartDateTime, infiniu2_light.dbo.WorkDays.DayEndDateTime, infiniu2_light.dbo.WorkDays.DayBreakStartDateTime, infiniu2_light.dbo.WorkDays.DayBreakEndDateTime FROM infiniu2_users.dbo.Users INNER JOIN infiniu2_light.dbo.WorkDays ON infiniu2_users.dbo.Users.UserID = infiniu2_light.dbo.WorkDays.UserID ORDER BY infiniu2_users.dbo.Users.Name",
                ConnectionStrings.LightConnectionString))
            {
                DA.Fill(TimeSheetDataTable);
            }
        }

        public DataTable DayStartDate()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT infiniu2_light.dbo.WorkDays.DayStartDateTime FROM infiniu2_light.dbo.WorkDays",
                ConnectionStrings.LightConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    return DT;
                }
            }
        }

        private DataTable DT_TimeSheet_new_by_YM(int year, int month)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT infiniu2_light.dbo.WorkDays.UserID, infiniu2_users.dbo.Users.Name, infiniu2_light.dbo.WorkDays.DayStartDateTime, infiniu2_light.dbo.WorkDays.DayEndDateTime, infiniu2_light.dbo.WorkDays.DayBreakStartDateTime, infiniu2_light.dbo.WorkDays.DayBreakEndDateTime FROM infiniu2_users.dbo.Users INNER JOIN infiniu2_light.dbo.WorkDays ON infiniu2_users.dbo.Users.UserID = infiniu2_light.dbo.WorkDays.UserID WHERE DATEPART(year,infiniu2_light.dbo.WorkDays.DayStartDateTime)=" + year + " and DATEPART(month,infiniu2_light.dbo.WorkDays.DayStartDateTime)=" + month + " ORDER BY infiniu2_users.dbo.Users.Name",
                ConnectionStrings.LightConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    return DT;
                }
            }
        }

        private DataTable DT_TimeSheet_by_YM(int year, int month)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DISTINCT infiniu2_light.dbo.WorkDays.UserID, infiniu2_users.dbo.Users.Name FROM infiniu2_users.dbo.Users INNER JOIN infiniu2_light.dbo.WorkDays ON infiniu2_users.dbo.Users.UserID = infiniu2_light.dbo.WorkDays.UserID WHERE DATEPART(year,infiniu2_light.dbo.WorkDays.DayStartDateTime)=" + year + " and DATEPART(month,infiniu2_light.dbo.WorkDays.DayStartDateTime)=" + month + " ORDER BY infiniu2_users.dbo.Users.Name",
                ConnectionStrings.LightConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);
                    return DT;
                }
            }
        }

        private DataRow GetAbsenceRow(int userId, DateTime date)
        {
            DataRow[] rows = _dtAbsences.Select("UserID=" + userId);
            if (rows.Any())
            {
                DataRow row = dtTimeSheet.NewRow();

                for (int i = 0; i < rows.Length; i++)
                {
                    DateTime dateStart = Convert.ToDateTime(rows[i]["DateStart"]);
                    DateTime dateFinish = Convert.ToDateTime(rows[i]["DateFinish"]);

                    if (!(date.Date >= dateStart.Date && date.Date <= dateFinish.Date))
                        continue;

                    int planHour = GetPlanHour(date);
                    int absenceTypeId = Convert.ToInt32(rows[i]["AbsenceTypeID"]);
                    decimal absenceHour = Convert.ToDecimal(rows[i]["Hours"]);

                    decimal workHour = 0;

                    string absenceTypeString = string.Empty;


                    if (absenceTypeId == 1)
                        absenceTypeString = "А/" + absenceHour;
                    else if (absenceTypeId == 2)
                        absenceTypeString = "О/" + absenceHour;
                    else if (absenceTypeId == 3)
                        absenceTypeString = "У/" + absenceHour;
                    else if (absenceTypeId == 4)
                        absenceTypeString = "Б/" + absenceHour;
                    else if (absenceTypeId == 5)
                        absenceTypeString = "ДМ/" + absenceHour;
                    else if (absenceTypeId == 6)
                        absenceTypeString = "Д/" + absenceHour;
                    else if (absenceTypeId == 7)
                        absenceTypeString = "ОА/" + absenceHour;
                    else if (absenceTypeId == 8)
                        absenceTypeString = "К/" + absenceHour;
                    else if (absenceTypeId == 9)
                        absenceTypeString = "Г/" + absenceHour;
                    else if (absenceTypeId == 10)
                        absenceTypeString = "ОТ/" + absenceHour;
                    else if (absenceTypeId == 11)
                        absenceTypeString = "МО/" + absenceHour;
                    else if (absenceTypeId == 12)
                        absenceTypeString = "П/" + absenceHour;
                    else if (absenceTypeId == 13)
                        absenceTypeString = "В/" + absenceHour;

                    row[date.Day + 1] = absenceTypeString;
                }

                row[dtTimeSheet.Columns.Count - 1] = "Итог";
                return row;
            }

            return null;
        }

        private Tuple<int, decimal> GetAbsenceTime(int userId, DateTime dayStartDateTime)
        {
            int absenceTypeId = 0;
            decimal absenceHour = 0;

            DataRow[] rows = _dtAbsences.Select("UserID=" + userId);
            if (rows.Any())
            {
                for (int x = 0; x < rows.Length; x++)
                {
                    DateTime dateStart = Convert.ToDateTime(rows[x]["DateStart"]);
                    DateTime dateFinish = Convert.ToDateTime(rows[x]["DateFinish"]);

                    if (dayStartDateTime.Date >= dateStart.Date && dayStartDateTime.Date <= dateFinish.Date)
                    {
                        absenceTypeId = Convert.ToInt32(rows[x]["AbsenceTypeID"]);
                        absenceHour = Convert.ToDecimal(rows[x]["Hours"]);
                    }
                }
            }

            var tuple = new Tuple<int, decimal>(absenceTypeId, absenceHour);
            return tuple;
        }

        public void GetTimeSheet(DataGridView TimeSheetDataGrid, string YearComboBox, string MonthComboBox)
        {
            int monthInt = Convert.ToDateTime(MonthComboBox + " 2013").Month;
            int yearInt = int.Parse(YearComboBox);
            int factoryId = 1;

            if (dtTimeSheet == null)
                dtTimeSheet = new DataTable();
            if (_dtAbsences == null)
                _dtAbsences = new DataTable();
            _dtAbsences = GetAbsences(yearInt, monthInt, factoryId);

            DataTable dtProdShedule = GetShedule(yearInt, monthInt);

            dtTimeSheetNew.Clear();
            dtTimeSheetNew = DT_TimeSheet_new_by_YM(yearInt, monthInt);

            dtTimeSheet = DT_TimeSheet_by_YM(yearInt, monthInt);

            for (int i = 1; i < DateTime.DaysInMonth(yearInt, monthInt) + 1; i++)
                dtTimeSheet.Columns.Add(i.ToString("D2"));

            dtTimeSheet.Columns.Add("Total");

            TimeSpan timeWork = default;
            TimeSpan timeSpan = default;

            for (int i = 0; i < dtTimeSheet.Rows.Count; i++)
            {
                double Total = 0;
                double T = 0;
                int userId = Convert.ToInt32(dtTimeSheet.Rows[i]["UserID"]);

                using (DataView DV = new DataView(dtTimeSheetNew))
                {
                    //DV.RowFilter = "Name = '" + DT_TimeSheet.Rows[i]["Name"].ToString() + "'";
                    DV.RowFilter = "UserID = '" + dtTimeSheet.Rows[i]["UserID"].ToString() + "'";
                    DataTable Table = DV.ToTable(false, new string[] { "UserID", "Name", "DayStartDateTime", "DayEndDateTime",
                        "DayBreakStartDateTime", "DayBreakEndDateTime" });
                    for (int j = 0; j < Table.Rows.Count; j++)
                    {
                        if (Table.Rows[j]["DayEndDateTime"].ToString() != "")
                        {
                            var dayStartDateTime = (DateTime)Table.Rows[j]["DayStartDateTime"];
                            var dayEndDateTime = (DateTime)Table.Rows[j]["DayEndDateTime"];
                            timeWork = dayEndDateTime.TimeOfDay - dayStartDateTime.TimeOfDay;

                            if (Table.Rows[j]["DayBreakStartDateTime"].ToString() != "" && Table.Rows[j]["DayBreakEndDateTime"].ToString() != "")
                            {
                                var dayBreakStartDateTime = (DateTime)Table.Rows[j]["DayBreakStartDateTime"];
                                var dayBreakEndDateTime = (DateTime)Table.Rows[j]["DayBreakEndDateTime"];
                                //TimeWork -= DayBreakEndDateTime.TimeOfDay - DayBreakStartDateTime.TimeOfDay;
                                timeSpan = dayBreakEndDateTime.TimeOfDay - dayBreakStartDateTime.TimeOfDay;
                            }
                            else
                            {
                                //TimeWork -= new TimeSpan(1, 0, 0);
                                timeSpan = new TimeSpan(1, 0, 0);
                            }
                            dtTimeSheet.Rows[i][dayStartDateTime.ToString("dd")] = Math.Round(timeWork.TotalHours, 1) + " (" + Math.Round(timeSpan.TotalHours, 1) + ")";
                            Total += timeWork.TotalHours;
                            T += timeSpan.TotalHours;
                        }
                    }
                }
                dtTimeSheet.Rows[i]["Total"] = Math.Round(Total) + " (" + Math.Round(T, 1) + ")";
            }

            if (dtProdShedule.Rows.Count > 0)
            {
                DataRow row = dtTimeSheet.NewRow();

                for (int j = 1; j < DateTime.DaysInMonth(yearInt, monthInt) + 1; j++)
                    row[j + 1] = dtProdShedule.Rows[j - 1]["Hour"];

                dtTimeSheet.Rows.InsertAt(row, 0);
            }

            for (int i = 1; i < dtTimeSheet.Rows.Count; i++)
            {
                int userId = Convert.ToInt32(dtTimeSheet.Rows[i]["UserID"]);

                if (!_dtAbsences.Select("UserID=" + userId).Any())
                    continue;

                DataRow row = dtTimeSheet.NewRow();

                decimal totalAbsenceTime = 0;
                double totalWorkTime = 0;
                decimal totalPlanTime = 0;
                for (int j = 1; j < DateTime.DaysInMonth(yearInt, monthInt) + 1; j++)
                {
                    DateTime today = new DateTime(yearInt, monthInt, j);

                    int planHour = Convert.ToInt32(dtTimeSheet.Rows[0][j + 1]);
                    var tuple = GetAbsenceTime(userId, today);

                    string absenceTypeString;
                    int absenceTypeId = tuple.Item1;
                    decimal absenceHour = tuple.Item2;
                    timeWork = default;
                    timeSpan = default;

                    totalPlanTime += planHour - absenceHour;

                    if (absenceTypeId == 0)
                        continue;;

                    if (absenceTypeId != 12 && absenceTypeId != 13)
                        totalAbsenceTime += absenceHour;

                    for (int x = 0; x < dtTimeSheetNew.Rows.Count; x++)
                    {
                        if (userId != Convert.ToInt32(dtTimeSheetNew.Rows[x]["UserID"]))
                            continue;

                        if (dtTimeSheetNew.Rows[x]["DayEndDateTime"].ToString() != "")
                        {
                            var dayStartDateTime = (DateTime)dtTimeSheetNew.Rows[x]["DayStartDateTime"];
                            var dayEndDateTime = (DateTime)dtTimeSheetNew.Rows[x]["DayEndDateTime"];

                            var compare = DateTime.Compare(today.Date, dayStartDateTime.Date);
                            if (compare != 0)
                                continue;

                            timeWork = dayEndDateTime.TimeOfDay - dayStartDateTime.TimeOfDay;
                            totalWorkTime += timeWork.TotalHours;
                            if (dtTimeSheetNew.Rows[x]["DayBreakStartDateTime"].ToString() != "" && dtTimeSheetNew.Rows[x]["DayBreakEndDateTime"].ToString() != "")
                            {
                                var dayBreakStartDateTime = (DateTime)dtTimeSheetNew.Rows[x]["DayBreakStartDateTime"];
                                var dayBreakEndDateTime = (DateTime)dtTimeSheetNew.Rows[x]["DayBreakEndDateTime"];
                                timeSpan = dayBreakEndDateTime.TimeOfDay - dayBreakStartDateTime.TimeOfDay;
                            }
                            else
                            {
                                timeSpan = new TimeSpan(1, 0, 0);
                            }
                        }
                    }


                    if (timeWork.TotalHours > 0)
                        absenceTypeString = Decimal.Round(absenceHour, 1, MidpointRounding.AwayFromZero) + " " + Math.Round(timeWork.TotalHours, 1) + "/" + Math.Round(planHour - absenceHour, 1) + "(" + Math.Round(timeSpan.TotalHours, 1) + ")";
                    else
                        absenceTypeString = Decimal.Round(absenceHour, 1, MidpointRounding.AwayFromZero).ToString();

                    if (absenceTypeId == 1)
                        absenceTypeString = "А/" + absenceTypeString;
                    else if (absenceTypeId == 2)
                        absenceTypeString = "О/" + absenceTypeString;
                    else if (absenceTypeId == 3)
                        absenceTypeString = "У/" + absenceTypeString;
                    else if (absenceTypeId == 4)
                        absenceTypeString = "Б/" + absenceTypeString;
                    else if (absenceTypeId == 5)
                        absenceTypeString = "ДМ/" + absenceTypeString;
                    else if (absenceTypeId == 6)
                        absenceTypeString = "Д/" + absenceTypeString;
                    else if (absenceTypeId == 7)
                        absenceTypeString = "ОА/" + absenceTypeString;
                    else if (absenceTypeId == 8)
                        absenceTypeString = "К/" + absenceTypeString;
                    else if (absenceTypeId == 9)
                        absenceTypeString = "Г/" + absenceTypeString;
                    else if (absenceTypeId == 10)
                        absenceTypeString = "ОТ/" + absenceTypeString;
                    else if (absenceTypeId == 11)
                        absenceTypeString = "МО/" + absenceTypeString;
                    else if (absenceTypeId == 12)
                        absenceTypeString = "П/" + absenceTypeString;
                    else if (absenceTypeId == 13)
                        absenceTypeString = "В/" + absenceTypeString;

                    row[j + 1] = absenceTypeString;
                }

                row[dtTimeSheet.Columns.Count - 1] = Decimal.Round(totalAbsenceTime, 1, MidpointRounding.AwayFromZero)+ " " + Math.Round(totalWorkTime, 1)+ "/" + Decimal.Round(totalPlanTime, 1, MidpointRounding.AwayFromZero);
                dtTimeSheet.Rows.InsertAt(row, ++i);
            }

            dtTimeSheet.Columns.RemoveAt(0);
            TimeSheetDataGrid.Columns.Clear();
            TimeSheetDataGrid.DataSource = dtTimeSheet;
            TimeSheetDataGrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            for (int i = 1; i < DateTime.DaysInMonth(yearInt, monthInt) + 1; i++)
            {
                var header = new DateTime(yearInt, monthInt, i).ToString("ddd");
                TimeSheetDataGrid.Columns[i.ToString("D2")].HeaderText += "\r\n" + header;
                TimeSheetDataGrid.Columns[i.ToString("D2")].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                TimeSheetDataGrid.Columns[i.ToString("D2")].Width = 130;
                if (header == "Сб" | header == "Вс")
                    TimeSheetDataGrid.Columns[i.ToString("D2")].DefaultCellStyle.BackColor = Color.Yellow;
                //TimeSheetDataGrid.Columns[i.ToString("D2")].DefaultCellStyle.BackColor = Color.FromArgb(222, 222, 65);
            }

            TimeSheetDataGrid.Columns["Name"].HeaderText = "Сотрудник";
            TimeSheetDataGrid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            TimeSheetDataGrid.Columns["Name"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            TimeSheetDataGrid.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            TimeSheetDataGrid.Columns["Total"].HeaderText = "Итого";
            TimeSheetDataGrid.Columns["Total"].DisplayIndex = TimeSheetDataGrid.Columns.Count - 1;
            TimeSheetDataGrid.Columns["Total"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            TimeSheetDataGrid.Columns["Total"].Width = 130;


            //DataGridViewCellStyle styleUser1 = new DataGridViewCellStyle();
            //styleUser1.BackColor = Color.LightSlateGray;

            //bool needPaint = true;

            //for (int i = 1; i < TimeSheetDataGrid.Rows.Count; i++)
            //{
            //    if ((i + 1) == TimeSheetDataGrid.Rows.Count)
            //        continue;

            //    if (!needPaint)
            //    {
            //        needPaint = true;
            //        continue;
            //    }

            //    if (needPaint)
            //        TimeSheetDataGrid.Rows[i].DefaultCellStyle = styleUser1;

            //    if (TimeSheetDataGrid.Rows[i + 1].Cells["Name"].Value.ToString() == "")
            //    {
            //        TimeSheetDataGrid.Rows[i].DefaultCellStyle = styleUser1;
            //        needPaint = true;
            //    }
            //    else
            //        needPaint = false;
            //}

            //DataGridViewCellStyle style = new DataGridViewCellStyle();

            //style.Font = new Font(TimeSheetDataGrid.Font, FontStyle.Bold);
            //if (TimeSheetDataGrid.Rows.Count > 0)
            //    TimeSheetDataGrid.Rows[0].DefaultCellStyle = style;

            dtTimeSheet.Dispose();
            dtTimeSheetNew.Clear();
            dtTimeSheetNew.Dispose();
        }

        public void GetTimeSheet1(DataGridView TimeSheetDataGrid, string YearComboBox, string MonthComboBox)
        {
            DataTable dtTimeSheetNew;

            DateTime dayStartDateTime;
            DateTime dayEndDateTime;
            DateTime dayBreakStartDateTime;
            DateTime dayBreakEndDateTime;
            TimeSpan timeWork;

            string header;
            int monthInt = Convert.ToDateTime(MonthComboBox + " 2013").Month;
            int yearInt = int.Parse(YearComboBox);
            int factoryId = 1;

            if (dtTimeSheet == null)
                dtTimeSheet = new DataTable();
            if (_dtAbsences == null)
                _dtAbsences = new DataTable();
            _dtAbsences = GetAbsences(yearInt, monthInt, factoryId);

            DataTable dtProdShedule = GetShedule(yearInt, monthInt);

            dtTimeSheetNew = DT_TimeSheet_new_by_YM(yearInt, monthInt);

            dtTimeSheet = DT_TimeSheet_by_YM(yearInt, monthInt);

            for (int i = 1; i < DateTime.DaysInMonth(yearInt, monthInt) + 1; i++)
                dtTimeSheet.Columns.Add(i.ToString("D2"));

            dtTimeSheet.Columns.Add("Total");

            for (int i = 0; i < dtTimeSheet.Rows.Count; i++)
            {
                double Total = 0;
                double T = 0;
                using (DataView DV = new DataView(dtTimeSheetNew))
                {
                    //DV.RowFilter = "Name = '" + DT_TimeSheet.Rows[i]["Name"].ToString() + "'";
                    DV.RowFilter = "UserID = '" + dtTimeSheet.Rows[i]["UserID"].ToString() + "'";
                    DataTable Table = DV.ToTable(false, new string[] { "UserID", "Name", "DayStartDateTime", "DayEndDateTime",
                        "DayBreakStartDateTime", "DayBreakEndDateTime" });
                    for (int j = 0; j < Table.Rows.Count; j++)
                    {
                        if (Table.Rows[j]["DayEndDateTime"].ToString() != "")
                        {
                            dayStartDateTime = (DateTime)Table.Rows[j]["DayStartDateTime"];
                            dayEndDateTime = (DateTime)Table.Rows[j]["DayEndDateTime"];
                            timeWork = dayEndDateTime.TimeOfDay - dayStartDateTime.TimeOfDay;

                            TimeSpan timeSpan;
                            if (Table.Rows[j]["DayBreakStartDateTime"].ToString() != "" && Table.Rows[j]["DayBreakEndDateTime"].ToString() != "")
                            {
                                dayBreakStartDateTime = (DateTime)Table.Rows[j]["DayBreakStartDateTime"];
                                dayBreakEndDateTime = (DateTime)Table.Rows[j]["DayBreakEndDateTime"];
                                //TimeWork -= DayBreakEndDateTime.TimeOfDay - DayBreakStartDateTime.TimeOfDay;
                                timeSpan = dayBreakEndDateTime.TimeOfDay - dayBreakStartDateTime.TimeOfDay;
                            }
                            else
                            {
                                //TimeWork -= new TimeSpan(1, 0, 0);
                                timeSpan = new TimeSpan(1, 0, 0);
                            }
                            dtTimeSheet.Rows[i][dayStartDateTime.ToString("dd")] = Math.Round(timeWork.TotalHours, 1) + " (" + Math.Round(timeSpan.TotalHours, 1) + ")";
                            Total += timeWork.TotalHours;
                            T += timeSpan.TotalHours;
                        }
                    }
                }
                dtTimeSheet.Rows[i]["Total"] = Math.Round(Total) + " (" + Math.Round(T, 1) + ")";
            }

            //for (int i = 0; i < dtTimeSheet.Rows.Count; i++)
            //{
            //    int userId = Convert.ToInt32(dtTimeSheet.Rows[i]["UserID"]);
            //    DataRow absenceRow = GetAbsenceRow(userId);
            //    if (absenceRow != null)
            //    {
            //        dtTimeSheet.Rows.InsertAt(absenceRow, ++i);

            //    }
            //    dtTimeSheet.Rows.InsertAt(dtTimeSheet.NewRow(), ++i);
            //}

            if (dtProdShedule.Rows.Count > 0)
            {
                DataRow row = dtTimeSheet.NewRow();

                for (int i = 1; i < DateTime.DaysInMonth(yearInt, monthInt) + 1; i++)
                    row[i + 1] = dtProdShedule.Rows[i - 1]["Hour"];

                dtTimeSheet.Rows.InsertAt(row, 0);
            }
            dtTimeSheet.Columns.RemoveAt(0);
            TimeSheetDataGrid.Columns.Clear();
            TimeSheetDataGrid.DataSource = dtTimeSheet;
            TimeSheetDataGrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            for (int i = 1; i < DateTime.DaysInMonth(yearInt, monthInt) + 1; i++)
            {
                header = new DateTime(yearInt, monthInt, i).ToString("ddd");
                TimeSheetDataGrid.Columns[i.ToString("D2")].HeaderText += "\r\n" + header;
                TimeSheetDataGrid.Columns[i.ToString("D2")].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                TimeSheetDataGrid.Columns[i.ToString("D2")].Width = 90;
                if (header == "Сб" | header == "Вс")
                    TimeSheetDataGrid.Columns[i.ToString("D2")].DefaultCellStyle.BackColor = Color.Yellow;
                //TimeSheetDataGrid.Columns[i.ToString("D2")].DefaultCellStyle.BackColor = Color.FromArgb(222, 222, 65);
            }

            TimeSheetDataGrid.Columns["Name"].HeaderText = "Сотрудник";
            TimeSheetDataGrid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            TimeSheetDataGrid.Columns["Name"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            TimeSheetDataGrid.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            TimeSheetDataGrid.Columns["Total"].HeaderText = "Итого";
            TimeSheetDataGrid.Columns["Total"].DisplayIndex = TimeSheetDataGrid.Columns.Count - 1;

            //DataGridViewCellStyle style = new DataGridViewCellStyle();

            //style.Font = new Font(TimeSheetDataGrid.Font, FontStyle.Bold);
            //if (TimeSheetDataGrid.Rows.Count > 0)
            //    TimeSheetDataGrid.Rows[0].DefaultCellStyle = style;

            dtTimeSheet.Dispose();
            dtTimeSheetNew.Clear();
            dtTimeSheetNew.Dispose();
        }

        public void ExportToExcel(DataGridView TimeSheetDataGrid)
        {
            ClearReport();
            Ex = new Excel();
            Ex.NewDocument(1);

            for (int i = 0; i < TimeSheetDataGrid.Columns.Count; i++)
            {
                Ex.WriteCell(1, TimeSheetDataGrid.Columns[i].HeaderText, 1, i + 1, 12, true);
                Ex.SetHorisontalAlignment(1, 1, i + 1, Excel.AlignHorizontal.xlCenter);
                Ex.SetBorderStyle(1, 1, i + 1, false, true, false, true, Excel.LineStyle.xlContinuous, Excel.BorderWeight.xlThin);
                if (TimeSheetDataGrid.Columns[i].DefaultCellStyle.BackColor == Color.FromArgb(222, 222, 65))
                    for (int j = 0; j <= TimeSheetDataGrid.Rows.Count; j++)
                        Ex.SetColor(1, j + 1, i + 1, Excel.Color.Yellow);
            }

            for (int i = 1; i < TimeSheetDataGrid.Rows.Count + 1; i++)
            {
                Ex.WriteCell(1, TimeSheetDataGrid.Rows[i - 1].Cells[0].Value, i + 1, 1, 12, true);
                Ex.SetBorderStyle(1, i + 1, 1, false, true, false, true, Excel.LineStyle.xlContinuous, Excel.BorderWeight.xlThin);
            }

            for (int i = 1; i < TimeSheetDataGrid.Rows.Count + 1; i++)
            {
                for (int j = 1; j < TimeSheetDataGrid.Columns.Count - 1; j++)
                {
                    if (TimeSheetDataGrid.Rows[i - 1].Cells[j].Value.ToString() != "")
                    {
                        //Ex.WriteCell(1, double.Parse(TimeSheetDataGrid.Rows[i - 1].Cells[j].Value.ToString()), i + 1, j + 1, 12, false);
                        Ex.WriteCell(1, TimeSheetDataGrid.Rows[i - 1].Cells[j].Value.ToString(), i + 1, j + 1, 12, false);
                    }
                    Ex.SetHorisontalAlignment(1, i + 1, j + 1, Excel.AlignHorizontal.xlCenter);
                    Ex.SetBorderStyle(1, i + 1, j + 1, false, true, false, true, Excel.LineStyle.xlContinuous, Excel.BorderWeight.xlThin);
                }
            }

            for (int i = 1; i < TimeSheetDataGrid.Rows.Count + 1; i++)
            {
                if (TimeSheetDataGrid.Rows[i - 1].Cells[TimeSheetDataGrid.Columns.Count - 1].Value.ToString() != "")
                {
                    //Ex.WriteCell(1, double.Parse(TimeSheetDataGrid.Rows[i - 1].Cells[TimeSheetDataGrid.Columns.Count - 1].Value.ToString()), i + 1, TimeSheetDataGrid.Columns.Count, 12, true);
                    Ex.WriteCell(1, TimeSheetDataGrid.Rows[i - 1].Cells[TimeSheetDataGrid.Columns.Count - 1].Value.ToString(), i + 1, TimeSheetDataGrid.Columns.Count, 12, true);
                }
                Ex.SetHorisontalAlignment(1, i + 1, TimeSheetDataGrid.Columns.Count, Excel.AlignHorizontal.xlCenter);
                Ex.SetBorderStyle(1, i + 1, TimeSheetDataGrid.Columns.Count, false, true, false, true, Excel.LineStyle.xlContinuous, Excel.BorderWeight.xlThin);
            }
            Ex.AutoFit(1, 1, 1, TimeSheetDataGrid.Rows.Count + 1, 1);
            Ex.Visible = true;
        }

        public DataTable GetShedule(int year, int month)
        {
            DataTable dt = new DataTable();
            using (SqlDataAdapter da = new SqlDataAdapter(@"SELECT * FROM ProductionShedule WHERE Year=" + year + " AND Month=" + month, ConnectionStrings.LightConnectionString))
            {
                da.Fill(dt);
            }
            return dt;
        }

        public DataTable GetAbsences(int year, int month, int factoryId)
        {
            DataTable dt = new DataTable();
            using (SqlDataAdapter da = new SqlDataAdapter(@"SELECT * FROM AbsencesJournal WHERE FactoryID=" + factoryId + " AND ((DATEPART(month, DateStart) = " + month + " AND DATEPART(year, DateStart) = " + year + ") OR (DATEPART(month, DateFinish) = " + month + " AND DATEPART(year, DateFinish) = " + year + "))", ConnectionStrings.LightConnectionString))
            {
                da.Fill(dt);
            }
            return dt;
        }

        private DataTable _dtAbsences;
        private DataTable dtTimeSheet;
        private int userTablesCount;
        private DataTable[] userTables;

        private int GetPlanHour(DateTime date)
        {
            int hour = -1;

            using (SqlDataAdapter da = new SqlDataAdapter(@"SELECT * FROM ProductionShedule
                WHERE Year=" + date.Year + " AND Month=" + date.Month + " AND Day=" + date.Day, ConnectionStrings.LightConnectionString))
            {
                using (DataTable dt = new DataTable())
                {
                    if (da.Fill(dt) > 0)
                        hour = Convert.ToInt32(dt.Rows[0]["Hour"]);
                }
            }

            return hour;
        }

        private void ClearReport()
        {
            if (Ex != null)
            {
                Ex.Dispose();
                Ex = null;
            }
        }
    }






    public class StaffListManager
    {
        DataTable RanksDT;
        DataTable TariffCoefsDT;
        DataTable RatesDT;
        DataTable DepartmentsDT;
        DataTable PositionsDT;
        DataTable StaffListDT;
        DataTable StaffListGroupByFullNameDT;
        DataTable FactoryDT;
        DataTable UsersDT;

        public BindingSource RanksBS;
        public BindingSource TariffCoefsBS;
        public BindingSource RatesBS;
        public BindingSource DepartmentsBS;
        public BindingSource StaffListBS;
        public BindingSource StaffListGroupByFullNameBS;
        public BindingSource FactoryBS;

        public StaffListManager()
        {
            Create();
            Fill();
            Binding();
        }

        private void Create()
        {
            RanksDT = new DataTable();
            RanksDT.Columns.Add(new DataColumn("Rank", Type.GetType("System.Int32")));
            TariffCoefsDT = new DataTable();
            TariffCoefsDT.Columns.Add(new DataColumn("TariffCoef", Type.GetType("System.Decimal")));
            RatesDT = new DataTable();
            RatesDT.Columns.Add(new DataColumn("Rate", Type.GetType("System.Decimal")));
            DepartmentsDT = new DataTable();
            PositionsDT = new DataTable();
            StaffListDT = new DataTable();
            StaffListGroupByFullNameDT = new DataTable();
            FactoryDT = new DataTable();
            UsersDT = new DataTable();
        }

        private void Fill()
        {
            string SelectCommand = @"SELECT * FROM Departments ORDER BY DepartmentName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.LightConnectionString))
            {
                DA.Fill(DepartmentsDT);
                DataRow NewRow = DepartmentsDT.NewRow();
                NewRow["DepartmentID"] = -1;
                NewRow["DepartmentName"] = "Все отделы";
                DepartmentsDT.Rows.InsertAt(NewRow, 0);
            }
            SelectCommand = @"SELECT * FROM Positions ORDER BY Position";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.LightConnectionString))
            {
                DA.Fill(PositionsDT);
            }
            SelectCommand = @"SELECT * FROM StaffList";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.LightConnectionString))
            {
                DA.Fill(StaffListDT);
            }
            SelectCommand = @"SELECT TOP 0 FactoryID, DepartmentID, PositionID, Rank, TariffCoef, SUM(Rate) AS Rate, TariffRateFirstRank, IncreasingContract, 
                    IndividualIncrease, IncreaseCoefCategorization, TechnologyIncrease, SurchargeContract, SUM(Salary) AS Salary FROM StaffList
                    GROUP BY FactoryID, DepartmentID, PositionID, Rank, TariffCoef, TariffRateFirstRank, IncreasingContract, IndividualIncrease, 
                    IncreaseCoefCategorization, TechnologyIncrease, SurchargeContract";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.LightConnectionString))
            {
                DA.Fill(StaffListGroupByFullNameDT);
            }
            SelectCommand = @"SELECT FactoryID, FactoryName FROM Factory WHERE FactoryID <> 0";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(FactoryDT);
            }
            SelectCommand = @"SELECT UserID, Name, ShortName FROM Users ORDER BY Name";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.UsersConnectionString))
            {
                DA.Fill(UsersDT);
            }

            for (int i = 1; i < 21; i++)
                RankNewRow(i);
            TariffCoefNewRow(1.16m);
            TariffCoefNewRow(1.35m);
            TariffCoefNewRow(1.57m);
            TariffCoefNewRow(1.73m);
            TariffCoefNewRow(1.9m);
            TariffCoefNewRow(2.03m);
            TariffCoefNewRow(2.17m);
            TariffCoefNewRow(2.32m);
            TariffCoefNewRow(2.48m);
            TariffCoefNewRow(2.65m);
            TariffCoefNewRow(2.84m);
            TariffCoefNewRow(3.04m);
            TariffCoefNewRow(3.25m);
            TariffCoefNewRow(3.48m);
            TariffCoefNewRow(3.72m);
            TariffCoefNewRow(3.98m);
            TariffCoefNewRow(4.26m);
            TariffCoefNewRow(4.56m);
            TariffCoefNewRow(4.88m);

            for (decimal i = 0.125m; i <= 1.5m; i += 0.125m)
                RateNewRow(i);
        }

        private void Binding()
        {
            //RanksBS = new BindingSource();
            //RanksBS.DataSource = RanksDT;
            //TariffCoefsBS = new BindingSource();
            //TariffCoefsBS.DataSource = TariffCoefsDT;
            //RatesBS = new BindingSource();
            //RatesBS.DataSource = RatesDT;
            DepartmentsBS = new BindingSource()
            {
                DataSource = new DataView(DepartmentsDT)
            };
            FactoryBS = new BindingSource()
            {
                DataSource = FactoryDT
            };
            StaffListBS = new BindingSource()
            {
                DataSource = StaffListDT
            };
            StaffListGroupByFullNameBS = new BindingSource()
            {
                DataSource = StaffListGroupByFullNameDT
            };
        }

        public DataGridViewComboBoxColumn RanksColumn
        {
            get
            {
                DataGridViewComboBoxColumn column = new DataGridViewComboBoxColumn()
                {
                    HeaderText = "Разряд",
                    Name = "RanksColumn",
                    DataPropertyName = "Rank",
                    DataSource = new DataView(RanksDT),
                    ValueMember = "Rank",
                    DisplayMember = "Rank",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    //column.DisplayIndex = 0;
                    //column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                return column;
            }
        }

        public DataGridViewComboBoxColumn TariffCoefsColumn
        {
            get
            {
                DataGridViewComboBoxColumn column = new DataGridViewComboBoxColumn()
                {
                    HeaderText = "ТК",
                    Name = "TariffCoefsColumn",
                    DataPropertyName = "TariffCoef",
                    DataSource = new DataView(TariffCoefsDT),
                    ValueMember = "TariffCoef",
                    DisplayMember = "TariffCoef",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    //column.DisplayIndex = 0;
                    //column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                return column;
            }
        }

        public DataGridViewComboBoxColumn RatesColumn
        {
            get
            {
                DataGridViewComboBoxColumn column = new DataGridViewComboBoxColumn()
                {
                    HeaderText = "Ст",
                    Name = "RatesColumn",
                    DataPropertyName = "Rate",
                    DataSource = new DataView(RatesDT),
                    ValueMember = "Rate",
                    DisplayMember = "Rate",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    //column.DisplayIndex = 0;
                    //column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                return column;
            }
        }

        public DataGridViewComboBoxColumn DepartmentNameColumn
        {
            get
            {
                DataGridViewComboBoxColumn column = new DataGridViewComboBoxColumn()
                {
                    HeaderText = "Служба",
                    Name = "DepartmentName",
                    DataPropertyName = "DepartmentID",
                    DataSource = new DataView(DepartmentsDT, "DepartmentID<>-1", string.Empty, DataViewRowState.CurrentRows),
                    ValueMember = "DepartmentID",
                    DisplayMember = "DepartmentName",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    //column.DisplayIndex = 0;
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                return column;
            }
        }

        public DataGridViewComboBoxColumn PositionColumn
        {
            get
            {
                DataGridViewComboBoxColumn column = new DataGridViewComboBoxColumn()
                {
                    HeaderText = "Должность",
                    Name = "Position",
                    DataPropertyName = "PositionID",
                    DataSource = new DataView(PositionsDT),
                    ValueMember = "PositionID",
                    DisplayMember = "Position",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    //column.DisplayIndex = 0;
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                return column;
            }
        }

        public DataGridViewComboBoxColumn UserNameColumn
        {
            get
            {
                DataGridViewComboBoxColumn column = new DataGridViewComboBoxColumn()
                {
                    HeaderText = "Ф.И.О.",
                    Name = "Name",
                    DataPropertyName = "UserID",
                    DataSource = new DataView(UsersDT),
                    ValueMember = "UserID",
                    DisplayMember = "Name",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    //column.DisplayIndex = 0;
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
                return column;
            }
        }

        public void FilterStaffList(bool bGroupByFullName, int DepartmentID, int FactoryID)
        {
            string Filter = "FactoryID=" + FactoryID;
            if (DepartmentID != -1)
                Filter += " AND DepartmentID=" + DepartmentID;
            if (bGroupByFullName)
                StaffListGroupByFullNameBS.Filter = Filter;
            else
                StaffListBS.Filter = Filter;
            StaffListBS.MoveFirst();
        }

        public void UpdateStaffList(bool bGroupByFullName)
        {
            if (bGroupByFullName)
            {
                string SelectCommand = @"SELECT FactoryID, DepartmentID, PositionID, Rank, TariffCoef, SUM(Rate) AS Rate, TariffRateFirstRank, IncreasingContract, 
                    IndividualIncrease, IncreaseCoefCategorization, TechnologyIncrease, SurchargeContract, SUM(Salary) AS Salary FROM StaffList
                    GROUP BY FactoryID, DepartmentID, PositionID, Rank, TariffCoef, TariffRateFirstRank, IncreasingContract, IndividualIncrease, 
                    IncreaseCoefCategorization, TechnologyIncrease, SurchargeContract";
                using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.LightConnectionString))
                {
                    StaffListGroupByFullNameDT.Clear();
                    DA.Fill(StaffListGroupByFullNameDT);
                }
            }
            else
            {
                string SelectCommand = @"SELECT * FROM StaffList";
                using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.LightConnectionString))
                {
                    StaffListDT.Clear();
                    DA.Fill(StaffListDT);
                }
            }
        }

        public void SaveStaffList()
        {
            string SelectCommand = @"SELECT TOP 0 * FROM StaffList";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.LightConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    DA.Update(StaffListDT);
                }
            }
        }

        public void DeleteStaff(int StaffListID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("DELETE FROM StaffList WHERE StaffListID = " + StaffListID,
                ConnectionStrings.LightConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                    }
                }
            }
        }

        public void DeleteStaffFunctions(int StaffListID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("DELETE FROM UserFunctions WHERE StaffListID = " + StaffListID,
                ConnectionStrings.LightConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                    }
                }
            }
        }

        private void RankNewRow(int Rank)
        {
            DataRow NewRow = RanksDT.NewRow();
            NewRow["Rank"] = Rank;
            RanksDT.Rows.Add(NewRow);
        }

        private void TariffCoefNewRow(decimal TariffCoef)
        {
            DataRow NewRow = TariffCoefsDT.NewRow();
            NewRow["TariffCoef"] = TariffCoef;
            TariffCoefsDT.Rows.Add(NewRow);
        }

        private void RateNewRow(decimal Rate)
        {
            DataRow NewRow = RatesDT.NewRow();
            NewRow["Rate"] = Rate;
            RatesDT.Rows.Add(NewRow);
        }

        public void DeletePosition(int StaffListID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("DELETE FROM StaffListDT WHERE StaffListID = " + StaffListID,
                ConnectionStrings.LightConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                    }
                }
            }
        }

        public bool IsPositionAlreadyAdded(string Function, string FunctionDescription)
        {
            DataRow[] Rows = StaffListDT.Select("Function = '" + Function + "' AND FunctionDescription = '" + FunctionDescription + "'");
            if (Rows.Count() > 0)
                return true;
            else
                return false;
        }

        public void MoveToPosition(int StaffListID)
        {
            StaffListBS.Position = StaffListBS.Find("StaffListID", StaffListID);
        }


        public void Calculation()
        {
            for (int i = 0; i < StaffListDT.Rows.Count; i++)
            {
                decimal TariffCoef = 0;
                decimal Rate = 0;
                decimal TariffRateFirstRank = 0;

                decimal IncreasingContract = 0;
                decimal IndividualIncrease = 0;
                decimal IncreaseCoefCategorization = 0;
                decimal TechnologyIncrease = 0;
                decimal SurchargeContract = 0;

                decimal BaseSalary = 0;
                decimal IncreasingContractSum = 0;
                decimal IndividualIncreaseSum = 0;
                decimal IncreaseCoefCategorizationSum = 0;
                decimal TechnologyIncreaseSum = 0;
                decimal InterimSalary = 0;
                decimal Salary = 0;

                if (StaffListDT.Rows[i]["TariffCoef"] != DBNull.Value)
                    TariffCoef = Convert.ToDecimal(StaffListDT.Rows[i]["TariffCoef"]);
                if (StaffListDT.Rows[i]["Rate"] != DBNull.Value)
                    Rate = Convert.ToDecimal(StaffListDT.Rows[i]["Rate"]);
                if (StaffListDT.Rows[i]["TariffRateFirstRank"] != DBNull.Value)
                    TariffRateFirstRank = Convert.ToDecimal(StaffListDT.Rows[i]["TariffRateFirstRank"]);
                if (StaffListDT.Rows[i]["IncreasingContract"] != DBNull.Value)
                    IncreasingContract = Convert.ToDecimal(StaffListDT.Rows[i]["IncreasingContract"]);
                if (StaffListDT.Rows[i]["IndividualIncrease"] != DBNull.Value)
                    IndividualIncrease = Convert.ToDecimal(StaffListDT.Rows[i]["IndividualIncrease"]);
                if (StaffListDT.Rows[i]["IncreaseCoefCategorization"] != DBNull.Value)
                    IncreaseCoefCategorization = Convert.ToDecimal(StaffListDT.Rows[i]["IncreaseCoefCategorization"]);
                if (StaffListDT.Rows[i]["TechnologyIncrease"] != DBNull.Value)
                    TechnologyIncrease = Convert.ToDecimal(StaffListDT.Rows[i]["TechnologyIncrease"]);
                if (StaffListDT.Rows[i]["SurchargeContract"] != DBNull.Value)
                    SurchargeContract = Convert.ToDecimal(StaffListDT.Rows[i]["SurchargeContract"]);

                BaseSalary = TariffCoef * TariffRateFirstRank;
                IncreasingContractSum = BaseSalary * IncreasingContract / 100;
                IndividualIncreaseSum = BaseSalary * IndividualIncrease / 100;
                IncreaseCoefCategorizationSum = BaseSalary * IncreaseCoefCategorization;
                TechnologyIncreaseSum = BaseSalary * TechnologyIncrease / 100;
                InterimSalary = TariffCoef * TariffRateFirstRank + IncreasingContractSum + IndividualIncreaseSum + IncreaseCoefCategorizationSum + TechnologyIncreaseSum;
                Salary = InterimSalary * Rate + SurchargeContract;

                if (BaseSalary != 0)
                    StaffListDT.Rows[i]["BaseSalary"] = BaseSalary;
                if (IncreasingContractSum != 0)
                    StaffListDT.Rows[i]["IncreasingContractSum"] = IncreasingContractSum;
                if (IndividualIncreaseSum != 0)
                    StaffListDT.Rows[i]["IndividualIncreaseSum"] = IndividualIncreaseSum;
                if (IncreaseCoefCategorizationSum != 0)
                    StaffListDT.Rows[i]["IncreaseCoefCategorizationSum"] = IncreaseCoefCategorizationSum;
                if (TechnologyIncreaseSum != 0)
                    StaffListDT.Rows[i]["TechnologyIncreaseSum"] = TechnologyIncreaseSum;
                if (Salary != 0)
                    StaffListDT.Rows[i]["Salary"] = Salary;
            }
        }
    }





    public class UsersResponsibilities
    {
        DataTable DepartmentsDT;
        DataTable PositionsDT;
        DataTable StaffListDT;
        DataTable ProfilPositionsDT;
        DataTable TPSPositionsDT;
        public DataTable ProfilFunctionsDT;
        public DataTable TPSFunctionsDT;
        DataTable UsersResponsibilitiesDT;
        DataTable FactoryDT;
        DataTable UsersDT;

        public BindingSource DepartmentsBS;
        public BindingSource PositionsBS;
        public BindingSource UsersBS;
        public BindingSource UsersResponsibilitiesBS;
        public BindingSource FactoryBS;

        public UsersResponsibilities()
        {
            Create();
            Fill();
            Binding();
        }

        public UsersResponsibilities(int FactoryID, int UserID, int PositionID)
        {
            ProfilPositionsDT = new DataTable();
            TPSPositionsDT = new DataTable();
            ProfilFunctionsDT = new DataTable();
            TPSFunctionsDT = new DataTable();
        }

        private void Create()
        {
            DepartmentsDT = new DataTable();
            PositionsDT = new DataTable();
            StaffListDT = new DataTable();
            UsersResponsibilitiesDT = new DataTable();
            FactoryDT = new DataTable();
            UsersDT = new DataTable();
        }

        private void Fill()
        {
            string SelectCommand = @"SELECT * FROM Departments ORDER BY DepartmentName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.LightConnectionString))
            {
                DA.Fill(DepartmentsDT);
            }
            SelectCommand = @"SELECT * FROM Positions ORDER BY Position";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.LightConnectionString))
            {
                DA.Fill(PositionsDT);
            }
            SelectCommand = @"SELECT * FROM StaffList";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.LightConnectionString))
            {
                DA.Fill(StaffListDT);
            }
            SelectCommand = @"SELECT UserFunctions.*, Functions.FunctionName FROM UserFunctions
                INNER JOIN Functions ON UserFunctions.FunctionID=Functions.FunctionID ORDER BY FunctionName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.LightConnectionString))
            {
                DA.Fill(UsersResponsibilitiesDT);
            }
            SelectCommand = @"SELECT FactoryID, FactoryName FROM Factory WHERE FactoryID <> 0";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.CatalogConnectionString))
            {
                DA.Fill(FactoryDT);
            }
            SelectCommand = @"SELECT UserID, Name, ShortName FROM Users  WHERE Fired <> 1 ORDER BY Name";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.UsersConnectionString))
            {
                DA.Fill(UsersDT);
            }
        }

        private void Binding()
        {
            DepartmentsBS = new BindingSource()
            {
                DataSource = new DataView(DepartmentsDT)
            };
            FactoryBS = new BindingSource()
            {
                DataSource = FactoryDT
            };
            UsersResponsibilitiesBS = new BindingSource()
            {
                DataSource = UsersResponsibilitiesDT
            };
            PositionsBS = new BindingSource()
            {
                DataSource = PositionsDT
            };
            UsersBS = new BindingSource()
            {
                DataSource = UsersDT
            };
        }

        public void FilterDepartments(int FactoryID)
        {
            string filter = string.Empty;
            DataTable Table = new DataTable();
            using (DataView DV = new DataView(StaffListDT, "FactoryID=" + FactoryID, string.Empty, DataViewRowState.CurrentRows))
            {
                Table = DV.ToTable(true, new string[] { "DepartmentID" });
            }
            for (int i = 0; i < Table.Rows.Count; i++)
                filter += Convert.ToInt32(Table.Rows[i]["DepartmentID"]) + ",";
            if (filter.Length > 0)
            {
                filter = filter.Substring(0, filter.Length - 1);
                filter = "DepartmentID IN (" + filter + ")";
            }
            Table.Dispose();
            DepartmentsBS.Filter = filter;
        }

        public void FilterUsers(int FactoryID, int DepartmentID)
        {
            string filter = string.Empty;
            DataTable Table = new DataTable();
            using (DataView DV = new DataView(StaffListDT, "FactoryID=" + FactoryID + " AND DepartmentID=" + DepartmentID, string.Empty, DataViewRowState.CurrentRows))
            {
                Table = DV.ToTable(true, new string[] { "UserID" });
            }
            for (int i = 0; i < Table.Rows.Count; i++)
                filter += Convert.ToInt32(Table.Rows[i]["UserID"]) + ",";
            if (filter.Length > 0)
            {
                filter = filter.Substring(0, filter.Length - 1);
                filter = "UserID IN (" + filter + ")";
            }
            Table.Dispose();
            UsersBS.Filter = filter;
        }

        public void FilterPositions(int FactoryID, int DepartmentID, int UserID)
        {
            string filter = string.Empty;
            DataTable Table = new DataTable();
            using (DataView DV = new DataView(StaffListDT, "FactoryID=" + FactoryID + " AND DepartmentID=" + DepartmentID + " AND UserID=" + UserID, string.Empty, DataViewRowState.CurrentRows))
            {
                Table = DV.ToTable(true, new string[] { "PositionID" });
            }
            for (int i = 0; i < Table.Rows.Count; i++)
                filter += Convert.ToInt32(Table.Rows[i]["PositionID"]) + ",";
            if (filter.Length > 0)
            {
                filter = filter.Substring(0, filter.Length - 1);
                filter = "PositionID IN (" + filter + ")";
            }
            Table.Dispose();
            PositionsBS.Filter = filter;
        }

        public void FilterUsersResponsibilities(int FactoryID, int DepartmentID, int UserID, int PositionID)
        {
            string filter = string.Empty;
            DataTable Table = new DataTable();
            using (DataView DV = new DataView(StaffListDT, "FactoryID=" + FactoryID + " AND DepartmentID=" + DepartmentID + " AND UserID=" + UserID + " AND PositionID=" + PositionID, string.Empty, DataViewRowState.CurrentRows))
            {
                Table = DV.ToTable(true, new string[] { "StaffListID" });
            }
            for (int i = 0; i < Table.Rows.Count; i++)
                filter += Convert.ToInt32(Table.Rows[i]["StaffListID"]) + ",";
            if (filter.Length > 0)
            {
                filter = filter.Substring(0, filter.Length - 1);
                filter = "StaffListID IN (" + filter + ")";
            }
            Table.Dispose();
            UsersResponsibilitiesBS.Filter = filter;
        }

        public DataTable FunctionsToExportDT
        {
            get { return ((DataTable)UsersResponsibilitiesBS.DataSource).DefaultView.ToTable(); }
        }

        public string UserNameByID(int UserID)
        {
            string UserName = "unnamed_user";

            DataRow[] Rows = UsersDT.Select("UserID=" + UserID);
            if (Rows.Count() > 0)
                UserName = Rows[0]["ShortName"].ToString();
            return UserName;
        }

        public bool HasProfilFunctions
        {
            get
            {
                string SelectCommand = @"SELECT UserFunctions.*, StaffList.DepartmentID, StaffList.PositionID, Positions.Position, StaffList.Rate, Functions.FunctionName,Functions.FunctionDescription FROM UserFunctions
                INNER JOIN StaffList ON UserFunctions.StaffListID=StaffList.StaffListID AND (StaffList.FactoryID=1 AND StaffList.UserID=" + Security.CurrentUserID + @")
                INNER JOIN Functions ON UserFunctions.FunctionID=Functions.FunctionID
                INNER JOIN Positions ON StaffList.PositionID=Positions.PositionID ORDER BY FunctionName";
                using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.LightConnectionString))
                {
                    ProfilFunctionsDT.Clear();
                    DA.Fill(ProfilFunctionsDT);
                }
                return ProfilFunctionsDT.Rows.Count > 0;
            }
        }

        public bool HasTPSFunctions
        {
            get
            {
                string SelectCommand = @"SELECT UserFunctions.*, StaffList.DepartmentID, StaffList.PositionID, Positions.Position, StaffList.Rate, Functions.FunctionName, Functions.FunctionDescription FROM UserFunctions
                INNER JOIN StaffList ON UserFunctions.StaffListID=StaffList.StaffListID AND (StaffList.FactoryID=2 AND StaffList.UserID=" + Security.CurrentUserID + @")
                INNER JOIN Functions ON UserFunctions.FunctionID=Functions.FunctionID
                INNER JOIN Positions ON StaffList.PositionID=Positions.PositionID ORDER BY FunctionName";
                using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.LightConnectionString))
                {
                    TPSFunctionsDT.Clear();
                    DA.Fill(TPSFunctionsDT);
                }
                return TPSFunctionsDT.Rows.Count > 0;
            }
        }

        public DataTable GetProfilPositionFunctions(int PositionID)
        {
            DataTable Table = ProfilFunctionsDT.Clone();
            DataRow[] Rows = ProfilFunctionsDT.Select("PositionID=" + PositionID);
            for (int j = 0; j < Rows.Count(); j++)
                Table.Rows.Add(Rows[j].ItemArray);
            return Table;
        }

        public DataTable GetTPSPositionFunctions(int PositionID)
        {
            DataTable Table = TPSFunctionsDT.Clone();
            DataRow[] Rows = TPSFunctionsDT.Select("PositionID=" + PositionID);
            for (int j = 0; j < Rows.Count(); j++)
                Table.Rows.Add(Rows[j].ItemArray);
            return Table;
        }

        public int ProfilPositionsCount
        {
            get
            {
                ProfilPositionsDT.Clear();
                using (DataView DV = new DataView(ProfilFunctionsDT))
                {
                    ProfilPositionsDT = DV.ToTable(true, new string[] { "DepartmentID", "PositionID", "Rate", "Position" });
                }
                return ProfilPositionsDT.Rows.Count;
            }
        }

        public int TPSPositionsCount
        {
            get
            {
                TPSPositionsDT.Clear();
                using (DataView DV = new DataView(TPSFunctionsDT))
                {
                    TPSPositionsDT = DV.ToTable(true, new string[] { "DepartmentID", "PositionID", "Rate", "Position" });
                }
                return TPSPositionsDT.Rows.Count;
            }
        }

        public void GetProfilPositionsInfo(int RowIndex, ref int DepartmentID, ref int PositionID, ref decimal Rate, ref string Position)
        {
            int.TryParse(ProfilPositionsDT.Rows[RowIndex]["DepartmentID"].ToString(), out DepartmentID);
            int.TryParse(ProfilPositionsDT.Rows[RowIndex]["PositionID"].ToString(), out PositionID);
            decimal.TryParse(ProfilPositionsDT.Rows[RowIndex]["Rate"].ToString(), out Rate);
            Position = ProfilPositionsDT.Rows[RowIndex]["Position"].ToString();
        }

        public void GetTPSPositionsInfo(int RowIndex, ref int DepartmentID, ref int PositionID, ref decimal Rate, ref string Position)
        {
            int.TryParse(TPSPositionsDT.Rows[RowIndex]["DepartmentID"].ToString(), out DepartmentID);
            int.TryParse(TPSPositionsDT.Rows[RowIndex]["PositionID"].ToString(), out PositionID);
            decimal.TryParse(TPSPositionsDT.Rows[RowIndex]["Rate"].ToString(), out Rate);
            Position = TPSPositionsDT.Rows[RowIndex]["Position"].ToString();
        }

        public void UpdateProfilFunctions()
        {
            string SelectCommand = @"SELECT UserFunctions.*, StaffList.DepartmentID, StaffList.PositionID, Positions.Position, StaffList.Rate, Functions.FunctionName, Functions.FunctionDescription FROM UserFunctions
                INNER JOIN StaffList ON UserFunctions.StaffListID=StaffList.StaffListID AND (StaffList.FactoryID=1 AND StaffList.UserID=" + Security.CurrentUserID + @")
                INNER JOIN Functions ON UserFunctions.FunctionID=Functions.FunctionID
                INNER JOIN Positions ON StaffList.PositionID=Positions.PositionID ORDER BY FunctionName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.LightConnectionString))
            {
                ProfilFunctionsDT.Clear();
                DA.Fill(ProfilFunctionsDT);
            }
        }

        public void UpdateTPSFunctions()
        {
            string SelectCommand = @"SELECT UserFunctions.*, StaffList.DepartmentID, StaffList.PositionID, Positions.Position, StaffList.Rate, Functions.FunctionName, Functions.FunctionDescription FROM UserFunctions
                INNER JOIN StaffList ON UserFunctions.StaffListID=StaffList.StaffListID AND (StaffList.FactoryID=2 AND StaffList.UserID=" + Security.CurrentUserID + @")
                INNER JOIN Functions ON UserFunctions.FunctionID=Functions.FunctionID
                INNER JOIN Positions ON StaffList.PositionID=Positions.PositionID ORDER BY FunctionName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.LightConnectionString))
            {
                TPSFunctionsDT.Clear();
                DA.Fill(TPSFunctionsDT);
            }
        }

        public void UpdateUsersResponsibilities()
        {
            string SelectCommand = @"SELECT UserFunctions.*, Functions.FunctionName FROM UserFunctions
                INNER JOIN Functions ON UserFunctions.FunctionID=Functions.FunctionID ORDER BY FunctionName";
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.LightConnectionString))
            {
                UsersResponsibilitiesDT.Clear();
                DA.Fill(UsersResponsibilitiesDT);
            }
        }

        public void DeleteUsersResponsibility(int UserFunctionID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("DELETE FROM UserFunctions WHERE UserFunctionID = " + UserFunctionID,
                ConnectionStrings.LightConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);
                    }
                }
            }
        }

        static public int GetStaffListID(int FactoryID, int DepartmentID, int UserID, int PositionID)
        {
            int StaffListID = -1;

            string SelectCommand = @"SELECT * FROM StaffList
                WHERE FactoryID=" + FactoryID + " AND DepartmentID=" + DepartmentID + " AND UserID=" + UserID + " AND PositionID=" + PositionID;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.LightConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                        StaffListID = Convert.ToInt32(DT.Rows[0]["StaffListID"]);
                }
            }

            return StaffListID;
        }

        public void MoveToPosition(int StaffListID)
        {
            UsersResponsibilitiesBS.Position = UsersResponsibilitiesBS.Find("StaffListID", StaffListID);
        }

    }


    public class UsersResponsibilitiesReport
    {
        DataTable FunctionsDT = null;

        public UsersResponsibilitiesReport()
        {
            FunctionsDT = new DataTable();
        }

        public DataTable dFunctionsDT
        {
            set { FunctionsDT = value; }
        }

        public void ClearReport()
        {
            FunctionsDT.Clear();
        }

        public void Report(string FileName, string Firm, string Department, string User, string Responsibility)
        {
            string MainOrdersList = string.Empty;

            int pos = 0;

            //Export to excel
            HSSFWorkbook hssfworkbook = new HSSFWorkbook();

            ////create a entry of DocumentSummaryInformation
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "NPOI Team";
            hssfworkbook.DocumentSummaryInformation = dsi;

            ////create a entry of SummaryInformation
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NPOI SDK Example";
            hssfworkbook.SummaryInformation = si;

            #region Create fonts and styles

            HSSFFont HeaderF3 = hssfworkbook.CreateFont();
            HeaderF3.FontHeightInPoints = 12;
            HeaderF3.Boldweight = 12 * 256;
            HeaderF3.FontName = "Calibri";

            HSSFFont SimpleF = hssfworkbook.CreateFont();
            SimpleF.FontHeightInPoints = 12;
            SimpleF.FontName = "Calibri";

            HSSFCellStyle SimpleCS = hssfworkbook.CreateCellStyle();
            SimpleCS.SetFont(SimpleF);

            HSSFCellStyle SimpleHeaderCS = hssfworkbook.CreateCellStyle();
            SimpleHeaderCS.SetFont(HeaderF3);

            #endregion

            HSSFSheet sheet1 = hssfworkbook.CreateSheet("Обязанности");
            sheet1.PrintSetup.PaperSize = (short)PaperSizeType.A4;

            sheet1.SetMargin(HSSFSheet.LeftMargin, (double).12);
            sheet1.SetMargin(HSSFSheet.RightMargin, (double).07);
            sheet1.SetMargin(HSSFSheet.TopMargin, (double).20);
            sheet1.SetMargin(HSSFSheet.BottomMargin, (double).20);

            sheet1.SetColumnWidth(0, 21 * 256);

            HSSFCell Cell1;

            if (FunctionsDT.Rows.Count > 0)
            {
                //Профиль
                pos += 2;

                Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                Cell1.SetCellValue("Участок:");
                Cell1.CellStyle = SimpleHeaderCS;
                Cell1 = sheet1.CreateRow(pos).CreateCell(1);
                Cell1.SetCellValue(Firm);
                Cell1.CellStyle = SimpleCS;
                pos++;
                Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                Cell1.SetCellValue("Отдел:");
                Cell1.CellStyle = SimpleHeaderCS;
                Cell1 = sheet1.CreateRow(pos).CreateCell(1);
                Cell1.SetCellValue(Department);
                Cell1.CellStyle = SimpleCS;
                pos++;
                Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                Cell1.SetCellValue("Сотрудник:");
                Cell1.CellStyle = SimpleHeaderCS;
                Cell1 = sheet1.CreateRow(pos).CreateCell(1);
                Cell1.SetCellValue(User);
                Cell1.CellStyle = SimpleCS;
                pos++;
                Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                Cell1.SetCellValue("Должность:");
                Cell1.CellStyle = SimpleHeaderCS;
                Cell1 = sheet1.CreateRow(pos).CreateCell(1);
                Cell1.SetCellValue(Responsibility);
                Cell1.CellStyle = SimpleCS;
                pos++;
                Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                Cell1.SetCellValue("Обязанности");
                Cell1.CellStyle = SimpleHeaderCS;
                pos++;

                for (int i = 0; i < FunctionsDT.Rows.Count; i++)
                {
                    Cell1 = sheet1.CreateRow(pos).CreateCell(0);
                    Cell1.SetCellValue(FunctionsDT.Rows[i]["FunctionName"].ToString());
                    Cell1.CellStyle = SimpleCS;
                    pos++;
                }
            }
            string tempFolder = System.Environment.GetEnvironmentVariable("TEMP");
            FileInfo file = new FileInfo(tempFolder + @"\" + FileName + ".xls");
            int j = 1;
            while (file.Exists == true)
            {
                file = new FileInfo(tempFolder + @"\" + FileName + "(" + j++ + ").xls");
            }

            FileStream NewFile = new FileStream(file.FullName, FileMode.Create);
            hssfworkbook.Write(NewFile);
            NewFile.Close();

            System.Diagnostics.Process.Start(file.FullName);
            ClearReport();
        }

    }



    public class RolesAndPermissionsManager
    {
        DataTable ModulesDataTable;
        DataTable RolesDataTable;
        DataTable PermissionsDataTable;
        DataTable RolePermissionsDataTable;
        DataTable UserRolesDataTable;
        DataTable UsersDataTable;

        public BindingSource ModulesBindingSource;
        public BindingSource RolesBindingSource;
        public BindingSource PermissionsBindingSource;
        public BindingSource RolePermissionsBindingSource;
        public BindingSource UserRolesBindingSource;
        public BindingSource UsersBindingSource;

        PercentageDataGrid ModulesDataGrid;
        PercentageDataGrid RolesDataGrid;
        PercentageDataGrid PermissionsDataGrid;
        PercentageDataGrid RolePermissionsDataGrid;
        PercentageDataGrid RolesPermissionsRolesDataGrid;
        PercentageDataGrid RoleUsersDataGrid;
        PercentageDataGrid UserRolesDataGrid;

        SqlDataAdapter RolesDA;
        SqlCommandBuilder RolesCB;

        SqlDataAdapter PermissionsDA;
        SqlCommandBuilder PermissionsCB;

        SqlDataAdapter RolePermissionsDA;
        SqlCommandBuilder RolePermissionsCB;

        SqlDataAdapter UserRolesDA;
        SqlCommandBuilder UserRolesCB;

        private DataGridViewComboBoxColumn PermissionColumn = null;
        private DataGridViewComboBoxColumn UsersColumn = null;

        public RolesAndPermissionsManager(ref PercentageDataGrid tModulesDataGrid, ref PercentageDataGrid tRolesDataGrid,
                                          ref PercentageDataGrid tPermissionsDataGrid, ref PercentageDataGrid tRolePermissionsDataGrid,
                                          ref PercentageDataGrid tRolesPermissionsRolesDataGrid, ref PercentageDataGrid tRoleUsersDataGrid,
                                          ref PercentageDataGrid tUserRolesDataGrid)
        {
            ModulesDataGrid = tModulesDataGrid;
            RolesDataGrid = tRolesDataGrid;
            PermissionsDataGrid = tPermissionsDataGrid;
            RolePermissionsDataGrid = tRolePermissionsDataGrid;
            RolesPermissionsRolesDataGrid = tRolesPermissionsRolesDataGrid;
            RoleUsersDataGrid = tRoleUsersDataGrid;
            UserRolesDataGrid = tUserRolesDataGrid;

            CreateAndFill();
            Binding();
            SetGrids();
        }

        private void CreateAndFill()
        {
            ModulesDataTable = new DataTable();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ModuleID, ModuleName FROM Modules ORDER BY ModuleName ASC", ConnectionStrings.UsersConnectionString))
            {
                DA.Fill(ModulesDataTable);
            }

            UsersDataTable = new DataTable();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT UserID, Name FROM Users  WHERE Fired <> 1 ORDER BY Name ASC", ConnectionStrings.UsersConnectionString))
            {
                DA.Fill(UsersDataTable);
            }


            RolesDA = new SqlDataAdapter("SELECT RoleID, ModuleID, RoleName, RoleDescription FROM Roles", ConnectionStrings.UsersConnectionString);
            RolesCB = new SqlCommandBuilder(RolesDA);
            RolesDataTable = new DataTable();
            RolesDA.Fill(RolesDataTable);

            PermissionsDA = new SqlDataAdapter("SELECT PermissionID, ModuleID, PermissionName, PermissionDescription FROM Permissions", ConnectionStrings.UsersConnectionString);
            PermissionsCB = new SqlCommandBuilder(PermissionsDA);
            PermissionsDataTable = new DataTable();
            PermissionsDA.Fill(PermissionsDataTable);

            RolePermissionsDA = new SqlDataAdapter("SELECT RolePermissionID, RoleID, PermissionID, Granted FROM RolePermissions", ConnectionStrings.UsersConnectionString);
            RolePermissionsCB = new SqlCommandBuilder(RolePermissionsDA);
            RolePermissionsDataTable = new DataTable();
            RolePermissionsDA.Fill(RolePermissionsDataTable);

            UserRolesDA = new SqlDataAdapter("SELECT UserRoleID, RoleID, UserID FROM UserRoles WHERE UserID IN (SELECT UserID FROM Users  WHERE Fired <> 1)", ConnectionStrings.UsersConnectionString);
            UserRolesCB = new SqlCommandBuilder(UserRolesDA);
            UserRolesDataTable = new DataTable();
            UserRolesDA.Fill(UserRolesDataTable);
        }

        private void Binding()
        {
            ModulesBindingSource = new BindingSource()
            {
                DataSource = ModulesDataTable
            };
            ModulesDataGrid.DataSource = ModulesBindingSource;

            RolesBindingSource = new BindingSource()
            {
                DataSource = RolesDataTable
            };
            RolesDataGrid.DataSource = RolesBindingSource;

            PermissionsBindingSource = new BindingSource()
            {
                DataSource = PermissionsDataTable
            };
            PermissionsDataGrid.DataSource = PermissionsBindingSource;

            RolePermissionsBindingSource = new BindingSource()
            {
                DataSource = RolePermissionsDataTable
            };
            RolePermissionsDataGrid.DataSource = RolePermissionsBindingSource;

            RolesPermissionsRolesDataGrid.DataSource = RolesBindingSource;

            RoleUsersDataGrid.DataSource = RolesBindingSource;

            UserRolesBindingSource = new BindingSource()
            {
                DataSource = UserRolesDataTable
            };
            UserRolesDataGrid.DataSource = UserRolesBindingSource;

            UsersBindingSource = new BindingSource()
            {
                DataSource = UsersDataTable
            };
        }

        private void SetGrids()
        {
            ModulesDataGrid.Columns["ModuleID"].Visible = false;

            RolesDataGrid.Columns["RoleID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            RolesDataGrid.Columns["RoleID"].Width = 70;
            RolesDataGrid.Columns["RoleName"].HeaderText = "Роль";
            RolesDataGrid.Columns["ModuleID"].Visible = false;
            RolesDataGrid.Columns["RoleDescription"].Visible = false;

            PermissionsDataGrid.Columns["PermissionID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            PermissionsDataGrid.Columns["PermissionID"].Width = 120;
            PermissionsDataGrid.Columns["PermissionName"].HeaderText = "Разрешение";
            PermissionsDataGrid.Columns["ModuleID"].Visible = false;
            PermissionsDataGrid.Columns["PermissionDescription"].Visible = false;

            RolesPermissionsRolesDataGrid.Columns["RoleID"].Visible = false;
            RolesPermissionsRolesDataGrid.Columns["RoleName"].HeaderText = "Роль";
            RolesPermissionsRolesDataGrid.Columns["ModuleID"].Visible = false;
            RolesPermissionsRolesDataGrid.Columns["RoleDescription"].Visible = false;


            PermissionColumn = new DataGridViewComboBoxColumn()
            {
                Name = "PermissionColumn",
                HeaderText = "Разрешение",
                DataPropertyName = "PermissionID",
                DataSource = new DataView(PermissionsDataTable),
                ValueMember = "PermissionID",
                DisplayMember = "PermissionName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                DisplayIndex = 0
            };
            RolePermissionsDataGrid.Columns.Add(PermissionColumn);
            RolePermissionsDataGrid.Columns["RoleID"].Visible = false;
            RolePermissionsDataGrid.Columns["PermissionID"].Visible = false;
            RolePermissionsDataGrid.Columns["RolePermissionID"].Visible = false;
            RolePermissionsDataGrid.Columns["Granted"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            RolePermissionsDataGrid.Columns["Granted"].Width = 70;

            RoleUsersDataGrid.Columns["RoleID"].Visible = false;
            RoleUsersDataGrid.Columns["ModuleID"].Visible = false;
            RoleUsersDataGrid.Columns["RoleDescription"].Visible = false;

            UsersColumn = new DataGridViewComboBoxColumn()
            {
                Name = "UsersColumn",
                HeaderText = "Пользователь",
                DataPropertyName = "UserID",
                DataSource = new DataView(UsersDataTable),
                ValueMember = "UserID",
                DisplayMember = "Name",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                SortMode = DataGridViewColumnSortMode.Programmatic,
                DisplayIndex = 0
            };
            UserRolesDataGrid.Columns.Add(UsersColumn);
            UserRolesDataGrid.Columns["UserRoleID"].Visible = false;
            UserRolesDataGrid.Columns["RoleID"].Visible = false;
            UserRolesDataGrid.Columns["UserID"].Visible = false;
            UserRolesDataGrid.Sort(UserRolesDataGrid.Columns["UsersColumn"], System.ComponentModel.ListSortDirection.Ascending);
        }

        public void FilterRoles(int ModuleID)
        {
            RolesBindingSource.Filter = "ModuleID = " + ModuleID;
        }

        public void FilterPermissions(int ModuleID)
        {
            PermissionsBindingSource.Filter = "ModuleID = " + ModuleID;
        }

        public void FilterRolePermissions(int RoleID)
        {
            RolePermissionsBindingSource.Filter = "RoleID = " + RoleID;
        }

        public void FilterUserRoles(int RoleID)
        {
            UserRolesBindingSource.Filter = "RoleID = " + RoleID;
        }

        public void RemoveCurrentRole()
        {
            if (RolesBindingSource.Count > -1)
            {
                RolesBindingSource.RemoveCurrent();
            }
        }

        public void RemoveCurrentPermission()
        {
            if (PermissionsBindingSource.Count > -1)
                PermissionsBindingSource.RemoveCurrent();
        }

        public void RemoveCurrentUserRole()
        {
            if (UserRolesBindingSource.Count > -1)
                UserRolesBindingSource.RemoveCurrent();
        }

        public void AddRole(int ModuleID, string RoleName, string Description)
        {
            DataRow NewRow = RolesDataTable.NewRow();
            NewRow["ModuleID"] = ModuleID;
            NewRow["RoleName"] = RoleName;
            NewRow["RoleDescription"] = Description;
            RolesDataTable.Rows.Add(NewRow);
        }

        public void AddPermission(int ModuleID, string PermissionName, string PermissionDescription)
        {
            DataRow NewRow = PermissionsDataTable.NewRow();
            NewRow["ModuleID"] = ModuleID;
            NewRow["PermissionName"] = PermissionName;
            NewRow["PermissionDescription"] = PermissionDescription;
            PermissionsDataTable.Rows.Add(NewRow);
        }

        public void AddUserRole(int RoleID, int UserID)
        {
            if (UserRolesDataTable.Select("UserID = " + UserID + " AND RoleID = " + RoleID).Count() > 0)
                return;

            DataRow NewRow = UserRolesDataTable.NewRow();
            NewRow["RoleID"] = RoleID;
            NewRow["UserID"] = UserID;
            UserRolesDataTable.Rows.Add(NewRow);
        }

        public void SaveRoles(int ModuleID)
        {
            RolesDA.Update(RolesDataTable);
            RolesDataTable.Clear();
            RolesDA.Fill(RolesDataTable);

            UpdateRolesPermissions(ModuleID);
        }

        public void UpdateRoleDescription(string Description)
        {
            if (RolesBindingSource.Count == 0)
                return;

            ((DataRowView)RolesBindingSource.Current).Row["RoleDescription"] = "";
        }

        public void UpdatePermissionsDescription(string Description)
        {
            if (PermissionsBindingSource.Count == 0)
                return;

            ((DataRowView)PermissionsBindingSource.Current).Row["PermissionDescription"] = "";
        }

        public void UpdateRolesPermissions(int ModuleID)
        {
            //remove

            using (SqlDataAdapter prDA = new SqlDataAdapter("DELETE FROM RolePermissions WHERE RoleID NOT IN (SELECT RoleID FROM Roles) OR PermissionID NOT IN (SELECT PermissionID FROM Permissions)", ConnectionStrings.UsersConnectionString))
            {
                using (DataTable prDT = new DataTable())
                {
                    prDA.Fill(prDT);
                }
            }

            using (SqlDataAdapter rDA = new SqlDataAdapter("SELECT * FROM Roles WHERE ModuleID = " + ModuleID, ConnectionStrings.UsersConnectionString))
            {
                using (DataTable rDT = new DataTable())
                {
                    rDA.Fill(rDT);

                    using (SqlDataAdapter pDA = new SqlDataAdapter("SELECT * FROM Permissions WHERE ModuleID = " + ModuleID, ConnectionStrings.UsersConnectionString))
                    {
                        using (DataTable pDT = new DataTable())
                        {
                            pDA.Fill(pDT);

                            using (SqlDataAdapter prDA = new SqlDataAdapter("SELECT * FROM RolePermissions WHERE RoleID IN (SELECT RoleID FROM Roles)", ConnectionStrings.UsersConnectionString))
                            {
                                using (SqlCommandBuilder prCB = new SqlCommandBuilder(prDA))
                                {
                                    using (DataTable prDT = new DataTable())
                                    {
                                        prDA.Fill(prDT);

                                        foreach (DataRow rRow in rDT.Rows)
                                        {
                                            foreach (DataRow pRow in pDT.Rows)
                                            {
                                                if (prDT.Select("RoleID = " + rRow["RoleID"] + " AND PermissionID = " + pRow["PermissionID"]).Count() > 0)
                                                    continue;

                                                DataRow NewRow = prDT.NewRow();
                                                NewRow["RoleID"] = rRow["RoleID"];
                                                NewRow["PermissionID"] = pRow["PermissionID"];
                                                NewRow["Granted"] = false;
                                                prDT.Rows.Add(NewRow);
                                            }
                                        }

                                        prDA.Update(prDT);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            RolePermissionsDataTable.Clear();
            RolePermissionsDA.Fill(RolePermissionsDataTable);
        }

        public void SavePermissions(int ModuleID)
        {
            PermissionsDA.Update(PermissionsDataTable);
            PermissionsDataTable.Clear();
            PermissionsDA.Fill(PermissionsDataTable);

            UpdateRolesPermissions(ModuleID);
        }

        public void SaveRolePermissions()
        {
            RolePermissionsDA.Update(RolePermissionsDataTable);
            RolePermissionsDataTable.Clear();
            RolePermissionsDA.Fill(RolePermissionsDataTable);
        }

        public void SaveUserRoles()
        {
            UserRolesDA.Update(UserRolesDataTable);
            UserRolesDataTable.Clear();
            UserRolesDA.Fill(UserRolesDataTable);
        }

        static public DataTable GetPermissions(int UserID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM RolePermissions" +
                " WHERE RolePermissions.RoleID IN " +
                "(SELECT RoleID FROM UserRoles WHERE UserID = " + UserID + ")", ConnectionStrings.UsersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    return (DataTable)DT;
                }
            }
        }

        static public DataTable GetPermissions(int UserID, int ModuleID)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM RolePermissions" +
                " WHERE RolePermissions.RoleID IN " +
                "(SELECT RoleID FROM UserRoles WHERE UserID = " + UserID + ")", ConnectionStrings.UsersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    return (DataTable)DT;
                }
            }
        }

        static public DataTable GetPermissions(int UserID, string FormName)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM RolePermissions" +
                " WHERE Granted=1 AND RolePermissions.RoleID IN " +
                "(SELECT RoleID FROM UserRoles WHERE UserID = " + UserID + ")" +
                " AND PermissionID IN (SELECT PermissionID FROM Permissions WHERE ModuleID IN " +
                " (SELECT ModuleID FROM Modules WHERE FormName = '" + FormName + "'))", ConnectionStrings.UsersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    return (DataTable)DT;
                }
            }
        }
    }






    public class ScanTasks
    {
        public DataTable TasksDataTable;
        public BindingSource TasksBindingSource;

        private SqlCommandBuilder CB;
        private SqlDataAdapter DA;
        PercentageDataGrid TasksPlanDataGrid;

        public ScanTasks(ref PercentageDataGrid tTasksPlanDataGrid)
        {
            TasksPlanDataGrid = tTasksPlanDataGrid;

            CreateAndFill();
            Binding();
            GridSettings();
        }

        public void CreateAndFill()
        {
            DA = new SqlDataAdapter("SELECT * FROM Tasks", ConnectionStrings.LightConnectionString);
            CB = new SqlCommandBuilder(DA);
            TasksDataTable = new DataTable();
            DA.Fill(TasksDataTable);
        }

        public void Binding()
        {
            TasksBindingSource = new BindingSource()
            {
                DataSource = TasksDataTable
            };
            TasksPlanDataGrid.DataSource = TasksBindingSource;
        }

        public void GridSettings()
        {
            TasksPlanDataGrid.Columns["TaskID"].Visible = false;
            TasksPlanDataGrid.Columns["DirectorID"].Visible = false;
            TasksPlanDataGrid.Columns["TaskDescription"].Visible = false;
            TasksPlanDataGrid.Columns["ReturnDescription"].Visible = false;
            TasksPlanDataGrid.Columns["TaskName"].HeaderText = "Задача";
            TasksPlanDataGrid.Columns["CreationDate"].HeaderText = "Дата постановки\r\nзадачи";
            TasksPlanDataGrid.Columns["ExecutionDate"].HeaderText = "Срок выполнения\r\nзадачи";
            TasksPlanDataGrid.Columns["FactExecutionDate"].HeaderText = "Дата выполнения\r\nзадачи";
            TasksPlanDataGrid.Columns["CompletPercentage"].HeaderText = "Готовность, %";
            TasksPlanDataGrid.Columns["ExecutionStatus"].HeaderText = "Завершено";

            foreach (DataGridViewColumn Column in TasksPlanDataGrid.Columns)
            {
                Column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            TasksPlanDataGrid.Columns["CreationDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            TasksPlanDataGrid.Columns["CreationDate"].Width = 150;
            TasksPlanDataGrid.Columns["ExecutionDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            TasksPlanDataGrid.Columns["ExecutionDate"].Width = 150;
            TasksPlanDataGrid.Columns["FactExecutionDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            TasksPlanDataGrid.Columns["FactExecutionDate"].Width = 150;
            TasksPlanDataGrid.Columns["ExecutionStatus"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            TasksPlanDataGrid.Columns["ExecutionStatus"].Width = 100;
            TasksPlanDataGrid.Columns["CompletPercentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            TasksPlanDataGrid.Columns["CompletPercentage"].Width = 150;

            TasksPlanDataGrid.AddPercentageColumn("CompletPercentage");

            TasksPlanDataGrid.Columns["TaskName"].DisplayIndex = 0;
            TasksPlanDataGrid.Columns["CreationDate"].DisplayIndex = 1;
            TasksPlanDataGrid.Columns["ExecutionDate"].DisplayIndex = 2;
            TasksPlanDataGrid.Columns["CompletPercentage"].DisplayIndex = 7;
            TasksPlanDataGrid.Columns["FactExecutionDate"].DisplayIndex = 6;
            TasksPlanDataGrid.Columns["ExecutionStatus"].DisplayIndex = 8;

        }

        public string GetPerformersName(string TaskID)
        {
            string PerformersName = "";
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT [TaskMembersID],[TaskID] ,[PerformerID] FROM TaskMembers where TaskID = " + TaskID, ConnectionStrings.LightConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    for (int i = 0; i < DT.Rows.Count; i++)
                    {
                        PerformersName += Security.GetUserNameByID(Convert.ToInt32(DT.Rows[i]["PerformerID"])) + ", ";
                    }
                }
            }

            PerformersName = PerformersName.Remove(PerformersName.Length - 2);
            return PerformersName;
        }
    }






    public class AdminClientsJournalDetail
    {
        public DataTable ClientsDataTable;
        DataTable LoginJournalDataTable;
        DataTable ModulesJournalDataTable;
        DataTable ComputerParamsDataTable;
        DataTable ComputerParamsStuctDataTable;
        DataTable MessagesDataTable;
        public DataTable PaymentsDataTable;

        DataTable ClientEventsJournalDT;

        DataTable UsersNameDT;
        public DataTable ClientsNameDT;

        public BindingSource ClientsBindingSource;
        public BindingSource LoginJournalBindingSource;
        public BindingSource ModulesJournalBindingSource;
        public BindingSource ComputerParamsBindingSource;
        public BindingSource MessagesBindingSource;
        public BindingSource ClientEventsJournalBindingSource;

        DateTime StartModuleDateTime;

        UsersDataGrid ClientsGrid;
        PercentageDataGrid LoginJournalDataGrid;
        PercentageDataGrid ModulesJournalDataGrid;
        PercentageDataGrid ComputerParamsDataGrid;
        PercentageDataGrid MessagesDataGrid;
        PercentageDataGrid ClientEventsJournalDataGrid;

        Font fUserFont = new Font("Segoe UI", 13.0f, FontStyle.Bold);
        Font fModuleFont = new Font("Segoe UI", 13.0f, FontStyle.Bold);
        Font fTextFont = new Font("Segoe UI", 11.0f, FontStyle.Regular);

        Color cUserFontColor = Color.FromArgb(65, 124, 174);
        Color cModuleColor = Color.FromArgb(255, 15, 60);
        Color cTextFontColor = Color.Black;

        public AdminClientsJournalDetail(ref UsersDataGrid tClientsGrid, ref PercentageDataGrid tLoginJournalDataGrid,
            ref PercentageDataGrid tComputerParamsDataGrid, ref PercentageDataGrid tModulesJournalDataGrid,
            ref PercentageDataGrid tMessagesDataGrid, ref PercentageDataGrid tClientEventsJournalDataGrid)
        {
            ClientsGrid = tClientsGrid;
            LoginJournalDataGrid = tLoginJournalDataGrid;
            ComputerParamsDataGrid = tComputerParamsDataGrid;
            ModulesJournalDataGrid = tModulesJournalDataGrid;
            MessagesDataGrid = tMessagesDataGrid;
            ClientEventsJournalDataGrid = tClientEventsJournalDataGrid;

            PaymentsDataTable = new DataTable();

            FillPayments(-1);

            StartModuleDateTime = DateTime.Now;

            ClientsDataTable = new DataTable();
            ClientsDataTable.Columns.Add(new DataColumn("OnlineStatus", Type.GetType("System.Boolean")));
            ClientsDataTable.Columns.Add(new DataColumn("IdleTime", Type.GetType("System.String")));
            ClientsDataTable.Columns.Add(new DataColumn("OnTop", Type.GetType("System.String")));

            using (SqlDataAdapter uDA = new SqlDataAdapter("SELECT TOP 0 ClientID, ClientName, TopModule, TopMost FROM Clients", ConnectionStrings.MarketingReferenceConnectionString))
            {
                uDA.Fill(ClientsDataTable);
            }

            UsersNameDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT UserID, Name FROM Users WHERE Fired <> 1 ", ConnectionStrings.UsersConnectionString))
            {
                DA.Fill(UsersNameDT);
            }

            ClientsNameDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientID, ClientName FROM Clients ORDER BY ClientName ASC", ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(ClientsNameDT);
            }

            MessagesDataTable = new DataTable();

            ModulesJournalDataTable = new DataTable();
            using (SqlDataAdapter uDA = new SqlDataAdapter("SELECT TOP 0 ClientsModulesJournalID, DateEnter, DateExit, ModuleName FROM ClientsModulesJournal", ConnectionStrings.MarketingReferenceConnectionString))
            {
                uDA.Fill(ModulesJournalDataTable);
            }

            LoginJournalDataTable = new DataTable();
            using (SqlDataAdapter uDA = new SqlDataAdapter("SELECT TOP 0 ClientsLoginJournalID, DateEnter, DateExit, ComputerParams FROM ClientsLoginJournal", ConnectionStrings.MarketingReferenceConnectionString))
            {
                uDA.Fill(LoginJournalDataTable);
            }

            ClientEventsJournalDT = new DataTable();
            using (SqlDataAdapter uDA = new SqlDataAdapter("SELECT TOP 0 EventDateTime, ModuleName, ClientEvent FROM ClientEventsJournal", ConnectionStrings.MarketingReferenceConnectionString))
            {
                uDA.Fill(ClientEventsJournalDT);
            }

            MessagesDataTable.Columns.Add("SENDERNAME");
            MessagesDataTable.Columns.Add("RecipientUser");

            ComputerParamsStuctDataTable = new DataTable();
            ComputerParamsStuctDataTable.Columns.Add(new DataColumn("Param", Type.GetType("System.String")));
            ComputerParamsStuctDataTable.Columns.Add(new DataColumn("Value", Type.GetType("System.String")));

            ComputerParamsDataTable = new DataTable()
            {
                TableName = "ComputerParams"
            };
            ComputerParamsDataTable.Columns.Add(new DataColumn("Domain", Type.GetType("System.String")));
            ComputerParamsDataTable.Columns.Add(new DataColumn("ComputerName", Type.GetType("System.String")));
            ComputerParamsDataTable.Columns.Add(new DataColumn("LoginName", Type.GetType("System.String")));
            ComputerParamsDataTable.Columns.Add(new DataColumn("Manufacturer", Type.GetType("System.String")));
            ComputerParamsDataTable.Columns.Add(new DataColumn("Model", Type.GetType("System.String")));
            ComputerParamsDataTable.Columns.Add(new DataColumn("ProcessorName", Type.GetType("System.String")));
            ComputerParamsDataTable.Columns.Add(new DataColumn("ProcessorCores", Type.GetType("System.String")));
            ComputerParamsDataTable.Columns.Add(new DataColumn("TotalRAM", Type.GetType("System.String")));
            ComputerParamsDataTable.Columns.Add(new DataColumn("OSName", Type.GetType("System.String")));
            ComputerParamsDataTable.Columns.Add(new DataColumn("OSVersion", Type.GetType("System.String")));
            ComputerParamsDataTable.Columns.Add(new DataColumn("OSPlatform", Type.GetType("System.String")));
            ComputerParamsDataTable.Columns.Add(new DataColumn("Resolution", Type.GetType("System.String")));


            ClientsBindingSource = new BindingSource()
            {
                DataSource = ClientsDataTable
            };
            ClientsGrid.DataSource = ClientsBindingSource;

            LoginJournalBindingSource = new BindingSource()
            {
                DataSource = LoginJournalDataTable
            };
            LoginJournalDataGrid.DataSource = LoginJournalBindingSource;

            ComputerParamsBindingSource = new BindingSource()
            {
                DataSource = ComputerParamsStuctDataTable
            };
            tComputerParamsDataGrid.DataSource = ComputerParamsBindingSource;

            ModulesJournalBindingSource = new BindingSource()
            {
                DataSource = ModulesJournalDataTable
            };
            tModulesJournalDataGrid.DataSource = ModulesJournalBindingSource;

            MessagesBindingSource = new BindingSource()
            {
                DataSource = MessagesDataTable
            };
            tMessagesDataGrid.DataSource = MessagesBindingSource;


            ClientEventsJournalBindingSource = new BindingSource()
            {
                DataSource = ClientEventsJournalDT
            };
            ClientEventsJournalDataGrid.DataSource = ClientEventsJournalBindingSource;
        }

        public void FillClients(DateTime DateFrom, DateTime DateTo)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DISTINCT ClientID FROM ClientsLoginJournal WHERE DateEnter >= '" + DateFrom.ToString("yyyy-MM-dd") + "' AND DateEnter <= '" + DateTo.ToString("yyyy-MM-dd") + " 23:59:59.996'", ConnectionStrings.MarketingReferenceConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count != ClientsDataTable.Rows.Count)
                    {
                        using (SqlDataAdapter uDA = new SqlDataAdapter("SELECT ClientID, ClientName, TopModule, TopMost FROM Clients WHERE ClientID IN (SELECT ClientID FROM ClientsLoginJournal WHERE DateEnter >= '" + DateFrom.ToString("yyyy-MM-dd") + "' AND DateEnter <= '" + DateTo.ToString("yyyy-MM-dd") + " 23:59:59.996')", ConnectionStrings.MarketingReferenceConnectionString))
                        {
                            ClientsDataTable.Clear();

                            uDA.Fill(ClientsDataTable);
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }

        public void FillPayments(int ClientID)
        {
            PaymentsDataTable.Clear();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientPaymentsID, TypePayments, DocNumber, Date, Cost, infiniu2_catalog.dbo.CurrencyTypes.CurrencyType, infiniu2_catalog.dbo.Factory.FactoryName FROM ClientPayments INNER JOIN infiniu2_catalog.dbo.CurrencyTypes ON infiniu2_catalog.dbo.CurrencyTypes.CurrencyTypeID = ClientPayments.CurrencyTypeID INNER JOIN infiniu2_catalog.dbo.Factory ON infiniu2_catalog.dbo.Factory.FactoryID = ClientPayments.FactoryID WHERE ClientID = " + ClientID, ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(PaymentsDataTable);
            }
        }

        public string GetDebt(int ClientID)
        {
            decimal Cost = 0;

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT Cost, TypePayments, infiniu2_catalog.dbo.CurrencyTypes.CurrencyType FROM ClientPayments INNER JOIN infiniu2_catalog.dbo.CurrencyTypes ON infiniu2_catalog.dbo.CurrencyTypes.CurrencyTypeID = ClientPayments.CurrencyTypeID WHERE ClientID = " + ClientID, ConnectionStrings.MarketingReferenceConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) == 0)
                        return "0";

                    foreach (DataRow Row in DT.Rows)
                    {
                        if (Row["TypePayments"].ToString() == "Отгружено")
                            Cost += Convert.ToDecimal(Row["Cost"]);
                        else
                            Cost -= Convert.ToDecimal(Row["Cost"]);
                    }

                    return GetCostWithThousands(Cost) + " " + DT.Rows[0]["CurrencyType"].ToString();
                }
            }


        }

        public static string GetCostWithThousands(decimal iValue)
        {
            if (iValue == 0)
                return "0";

            return String.Format("{0:### ### ### ### ### ###.##}", iValue).TrimStart();
        }


        public void FillLoginJournal(int UserID, DateTime DateFrom, DateTime DateTo)
        {
            LoginJournalDataTable.Clear();

            using (SqlDataAdapter uDA = new SqlDataAdapter("SELECT ClientsLoginJournalID, DateEnter, DateExit, ComputerParams FROM ClientsLoginJournal WHERE ClientID = " + UserID + " AND DateEnter >= '" + DateFrom.ToString("yyyy-MM-dd") + "' AND DateEnter <= '" + DateTo.ToString("yyyy-MM-dd") + " 23:59:59.996'", ConnectionStrings.MarketingReferenceConnectionString))
            {
                uDA.Fill(LoginJournalDataTable);
            }
        }

        public void FillModulesJournal(int LoginJournalID)
        {
            ModulesJournalDataTable.Clear();

            using (SqlDataAdapter uDA = new SqlDataAdapter("SELECT ClientsModulesJournalID, DateEnter, DateExit, ModuleName FROM ClientsModulesJournal WHERE ClientsLoginJournalID = " + LoginJournalID, ConnectionStrings.MarketingReferenceConnectionString))
            {
                uDA.Fill(ModulesJournalDataTable);
            }
        }

        public void FillEventsJournal(int LoginJournalID)
        {
            ClientEventsJournalDT.Clear();

            using (SqlDataAdapter uDA = new SqlDataAdapter("SELECT EventDateTime, ModuleName, ClientEvent FROM ClientEventsJournal WHERE LoginJournalID = " + LoginJournalID, ConnectionStrings.MarketingReferenceConnectionString))
            {
                uDA.Fill(ClientEventsJournalDT);
            }
        }

        public void FillMessages(DateTime DateFrom, DateTime DateTo)
        {
            MessagesDataTable.Clear();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientsMessageID, SendDateTime, SenderID, RecipientID, SenderTypeID, RecipientTypeID FROM ClientsMessage WHERE SendDateTime >= '" + DateFrom.ToString("yyyy-MM-dd") + "' AND SendDateTime <= '" + DateTo.ToString("yyyy-MM-dd") + " 23:59:59.996'", ConnectionStrings.MarketingReferenceConnectionString))
            {
                DA.Fill(MessagesDataTable);
            }

            for (int i = 0; i < MessagesDataTable.Rows.Count; i++)
            {
                if (MessagesDataTable.Rows[i]["SenderTypeID"].ToString() == "True")
                {
                    MessagesDataTable.Rows[i]["SENDERNAME"] = (ClientsNameDT.Select("ClientID = " + MessagesDataTable.Rows[i]["SenderID"]))[0]["ClientName"];
                }
                else
                {
                    MessagesDataTable.Rows[i]["SENDERNAME"] = (UsersNameDT.Select("UserID = " + MessagesDataTable.Rows[i]["SenderID"]))[0]["Name"];
                }

                if (MessagesDataTable.Rows[i]["RecipientTypeID"].ToString() == "True")
                {
                    MessagesDataTable.Rows[i]["RecipientUser"] = (ClientsNameDT.Select("ClientID = " + MessagesDataTable.Rows[i]["RecipientID"]))[0]["ClientName"];
                }
                else
                {
                    MessagesDataTable.Rows[i]["RecipientUser"] = (UsersNameDT.Select("UserID = " + MessagesDataTable.Rows[i]["RecipientID"]))[0]["Name"];
                }
            }
        }

        public void FillComputerParams(int LoginJournalID)
        {
            ComputerParamsDataTable.Clear();
            ComputerParamsStuctDataTable.Clear();

            string XML = LoginJournalDataTable.Select("ClientsLoginJournalID = " + LoginJournalID)[0]["ComputerParams"].ToString();

            if (XML.Length == 0)
                return;

            using (StringReader SR = new StringReader(XML))
            {
                try
                {
                    ComputerParamsDataTable.ReadXml(SR);
                }
                catch
                {

                }
            }

            foreach (DataColumn Column in ComputerParamsDataTable.Columns)
            {
                DataRow NewRow = ComputerParamsStuctDataTable.NewRow();
                NewRow["Param"] = Column.ColumnName;
                NewRow["Value"] = ComputerParamsDataTable.Rows[0][Column.ColumnName];
                ComputerParamsStuctDataTable.Rows.Add(NewRow);
            }
        }

        public int GetOnlineUsersCount()
        {
            return ClientsDataTable.Select("OnlineStatus = 1").Count();
        }

        public void SetGrid()
        {
            ClientsGrid.AutoGenerateColumns = false;

            ClientsGrid.Columns["ClientID"].Visible = false;
            ClientsGrid.Columns["OnlineStatus"].Visible = false;
            ClientsGrid.Columns["OnlineColumn"].DisplayIndex = 0;
            ClientsGrid.Columns["IdleTime"].DisplayIndex = 6;
            ClientsGrid.Columns["TopMost"].Visible = false;
            ClientsGrid.Columns["OnTop"].DisplayIndex = 5;
            ClientsGrid.sOnlineStatusColumnName = "OnlineStatus";

            ClientsGrid.Columns["IdleTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            ClientsGrid.Columns["IdleTime"].Width = 80;
            ClientsGrid.Columns["OnTop"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            ClientsGrid.Columns["OnTop"].Width = 40;
            ClientsGrid.Columns["ClientName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            ClientsGrid.Columns["ClientName"].Width = 190;

            LoginJournalDataGrid.Columns["DateEnter"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm:ss";
            LoginJournalDataGrid.Columns["DateExit"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm:ss";
            LoginJournalDataGrid.Columns["DateEnter"].HeaderText = "Дата входа";
            LoginJournalDataGrid.Columns["DateExit"].HeaderText = "Дата выхода";
            LoginJournalDataGrid.Columns["ClientsLoginJournalID"].Visible = false;
            LoginJournalDataGrid.Columns["ComputerParams"].Visible = false;

            ComputerParamsDataGrid.Columns["Param"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            ComputerParamsDataGrid.Columns["Param"].Width = 150;

            ClientEventsJournalDataGrid.Columns["ModuleName"].HeaderText = "Модуль";
            ClientEventsJournalDataGrid.Columns["EventDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm:ss";
            ClientEventsJournalDataGrid.Columns["EventDateTime"].Width = 180;
            ClientEventsJournalDataGrid.Columns["EventDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            ClientEventsJournalDataGrid.Columns["EventDateTime"].HeaderText = "Дата";
            ClientEventsJournalDataGrid.Columns["ClientEvent"].HeaderText = "Действие";
            ClientEventsJournalDataGrid.Columns["ClientEvent"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ClientEventsJournalDataGrid.Columns["ModuleName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            ModulesJournalDataGrid.Columns["ClientsModulesJournalID"].Visible = false;
            ModulesJournalDataGrid.Columns["ModuleName"].HeaderText = "Название модуля";
            ModulesJournalDataGrid.Columns["DateEnter"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm:ss";
            ModulesJournalDataGrid.Columns["DateExit"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm:ss";
            ModulesJournalDataGrid.Columns["DateEnter"].HeaderText = "Дата входа";
            ModulesJournalDataGrid.Columns["DateExit"].HeaderText = "Дата выхода";

            ModulesJournalDataGrid.Columns["DateEnter"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            ModulesJournalDataGrid.Columns["DateEnter"].Width = 180;
            ModulesJournalDataGrid.Columns["DateExit"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            ModulesJournalDataGrid.Columns["DateExit"].Width = 180;


            MessagesDataGrid.Columns["SenderID"].Visible = false;
            MessagesDataGrid.Columns["RecipientID"].Visible = false;
            MessagesDataGrid.Columns["SenderTypeID"].Visible = false;
            MessagesDataGrid.Columns["RecipientTypeID"].Visible = false;

            MessagesDataGrid.Columns["SENDERNAME"].HeaderText = "От кого";
            MessagesDataGrid.Columns["SENDERNAME"].DisplayIndex = 3;

            MessagesDataGrid.Columns["RecipientUser"].HeaderText = "Кому";
            MessagesDataGrid.Columns["RecipientUser"].DisplayIndex = 4;

            MessagesDataGrid.Columns["SendDateTime"].HeaderText = "Дата отправки";
            MessagesDataGrid.Columns["ClientsMessageID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MessagesDataGrid.Columns["ClientsMessageID"].Width = 180;
        }

        public void ClearTopMostAndTopModuleAndIdle()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientID, TopMost, TopModule, IdleTime FROM Clients WHERE Online = 0 AND (TopMost = 1 OR TopModule != '-1' OR IdleTime > -1)", ConnectionStrings.MarketingReferenceConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        foreach (DataRow Row in DT.Rows)
                        {
                            Row["TopMost"] = false;
                            Row["TopModule"] = -1;
                            Row["IdleTime"] = DBNull.Value;
                        }

                        DA.Update(DT);
                    }
                }
            }
        }

        public void CheckOnline()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ClientID, TopModule, IdleTime, Online, TopMost FROM Clients", ConnectionStrings.MarketingReferenceConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    try
                    {
                        DA.Fill(DT);
                    }
                    catch
                    {
                        return;
                    }

                    foreach (DataRow Row in ClientsDataTable.Rows)
                    {
                        DataRow[] DRow = DT.Select("ClientID = " + Row["ClientID"]);


                        Row["OnlineStatus"] = DRow[0]["Online"];
                        Row["TopModule"] = DRow[0]["TopModule"];
                        Row["TopMost"] = DRow[0]["TopMost"];

                        if (DRow[0]["IdleTime"] == DBNull.Value)
                            Row["IdleTime"] = "-";
                        else
                            Row["IdleTime"] = SecondsToTime(Convert.ToInt32(DRow[0]["IdleTime"]));
                    }
                }
            }


        }

        private string SecondsToTime(int Seconds)
        {
            TimeSpan t = TimeSpan.FromSeconds(Seconds);

            string answer = string.Format("{0:D2}:{1:D2}:{2:D2}",
                            t.Hours,
                            t.Minutes,
                            t.Seconds);

            return answer;
        }
    }






    public class AdminManagersJournalDetail
    {
        public DataTable ManagersDataTable;
        DataTable LoginJournalDataTable;
        DataTable ModulesJournalDataTable;
        DataTable ComputerParamsDataTable;
        DataTable ComputerParamsStuctDataTable;
        DataTable MessagesDataTable;

        DataTable ManagerEventsJournalDT;

        DataTable UsersNameDT;
        DataTable ManagersNameDT;

        public BindingSource ManagersBindingSource;
        public BindingSource LoginJournalBindingSource;
        public BindingSource ModulesJournalBindingSource;
        public BindingSource ComputerParamsBindingSource;
        public BindingSource MessagesBindingSource;
        public BindingSource ManagerEventsJournalBindingSource;

        DateTime StartModuleDateTime;

        UsersDataGrid ClientsGrid;
        PercentageDataGrid LoginJournalDataGrid;
        PercentageDataGrid ModulesJournalDataGrid;
        PercentageDataGrid ComputerParamsDataGrid;
        PercentageDataGrid MessagesDataGrid;
        PercentageDataGrid ClientEventsJournalDataGrid;

        Font fUserFont = new Font("Segoe UI", 13.0f, FontStyle.Bold);
        Font fModuleFont = new Font("Segoe UI", 13.0f, FontStyle.Bold);
        Font fTextFont = new Font("Segoe UI", 11.0f, FontStyle.Regular);

        Color cUserFontColor = Color.FromArgb(65, 124, 174);
        Color cModuleColor = Color.FromArgb(255, 15, 60);
        Color cTextFontColor = Color.Black;

        public AdminManagersJournalDetail(ref UsersDataGrid tClientsGrid, ref PercentageDataGrid tLoginJournalDataGrid,
            ref PercentageDataGrid tComputerParamsDataGrid, ref PercentageDataGrid tModulesJournalDataGrid,
            ref PercentageDataGrid tMessagesDataGrid, ref PercentageDataGrid tClientEventsJournalDataGrid)
        {
            ClientsGrid = tClientsGrid;
            LoginJournalDataGrid = tLoginJournalDataGrid;
            ComputerParamsDataGrid = tComputerParamsDataGrid;
            ModulesJournalDataGrid = tModulesJournalDataGrid;
            MessagesDataGrid = tMessagesDataGrid;
            ClientEventsJournalDataGrid = tClientEventsJournalDataGrid;

            StartModuleDateTime = DateTime.Now;

            ManagersDataTable = new DataTable();
            ManagersDataTable.Columns.Add(new DataColumn("OnlineStatus", Type.GetType("System.Boolean")));
            ManagersDataTable.Columns.Add(new DataColumn("IdleTime", Type.GetType("System.String")));
            ManagersDataTable.Columns.Add(new DataColumn("OnTop", Type.GetType("System.String")));

            using (SqlDataAdapter uDA = new SqlDataAdapter("SELECT TOP 0 ManagerID, Name, TopModule, TopMost FROM Managers", ConnectionStrings.ZOVReferenceConnectionString))
            {
                uDA.Fill(ManagersDataTable);
            }

            UsersNameDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT UserID, Name FROM Users WHERE Fired <> 1 ", ConnectionStrings.UsersConnectionString))
            {
                DA.Fill(UsersNameDT);
            }

            ManagersNameDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ManagerID, Name FROM Managers", ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(ManagersNameDT);
            }

            MessagesDataTable = new DataTable();

            ModulesJournalDataTable = new DataTable();
            using (SqlDataAdapter uDA = new SqlDataAdapter("SELECT TOP 0 ManagersModulesJournalID, DateEnter, DateExit, ModuleName FROM ManagersModulesJournal", ConnectionStrings.ZOVReferenceConnectionString))
            {
                uDA.Fill(ModulesJournalDataTable);
            }

            LoginJournalDataTable = new DataTable();
            using (SqlDataAdapter uDA = new SqlDataAdapter("SELECT TOP 0 ManagersLoginJournalID, DateEnter, DateExit, ComputerParams FROM ManagersLoginJournal", ConnectionStrings.ZOVReferenceConnectionString))
            {
                uDA.Fill(LoginJournalDataTable);
            }

            ManagerEventsJournalDT = new DataTable();
            using (SqlDataAdapter uDA = new SqlDataAdapter("SELECT TOP 0 EventDateTime, ModuleName, ManagerEvent FROM ManagerEventsJournal", ConnectionStrings.ZOVReferenceConnectionString))
            {
                uDA.Fill(ManagerEventsJournalDT);
            }

            MessagesDataTable.Columns.Add("SENDERNAME");
            MessagesDataTable.Columns.Add("RecipientUser");

            ComputerParamsStuctDataTable = new DataTable();
            ComputerParamsStuctDataTable.Columns.Add(new DataColumn("Param", Type.GetType("System.String")));
            ComputerParamsStuctDataTable.Columns.Add(new DataColumn("Value", Type.GetType("System.String")));

            ComputerParamsDataTable = new DataTable()
            {
                TableName = "ComputerParams"
            };
            ComputerParamsDataTable.Columns.Add(new DataColumn("Domain", Type.GetType("System.String")));
            ComputerParamsDataTable.Columns.Add(new DataColumn("ComputerName", Type.GetType("System.String")));
            ComputerParamsDataTable.Columns.Add(new DataColumn("LoginName", Type.GetType("System.String")));
            ComputerParamsDataTable.Columns.Add(new DataColumn("Manufacturer", Type.GetType("System.String")));
            ComputerParamsDataTable.Columns.Add(new DataColumn("Model", Type.GetType("System.String")));
            ComputerParamsDataTable.Columns.Add(new DataColumn("ProcessorName", Type.GetType("System.String")));
            ComputerParamsDataTable.Columns.Add(new DataColumn("ProcessorCores", Type.GetType("System.String")));
            ComputerParamsDataTable.Columns.Add(new DataColumn("TotalRAM", Type.GetType("System.String")));
            ComputerParamsDataTable.Columns.Add(new DataColumn("OSName", Type.GetType("System.String")));
            ComputerParamsDataTable.Columns.Add(new DataColumn("OSVersion", Type.GetType("System.String")));
            ComputerParamsDataTable.Columns.Add(new DataColumn("OSPlatform", Type.GetType("System.String")));
            ComputerParamsDataTable.Columns.Add(new DataColumn("Resolution", Type.GetType("System.String")));


            ManagersBindingSource = new BindingSource()
            {
                DataSource = ManagersDataTable
            };
            ClientsGrid.DataSource = ManagersBindingSource;

            LoginJournalBindingSource = new BindingSource()
            {
                DataSource = LoginJournalDataTable
            };
            LoginJournalDataGrid.DataSource = LoginJournalBindingSource;

            ComputerParamsBindingSource = new BindingSource()
            {
                DataSource = ComputerParamsStuctDataTable
            };
            tComputerParamsDataGrid.DataSource = ComputerParamsBindingSource;

            ModulesJournalBindingSource = new BindingSource()
            {
                DataSource = ModulesJournalDataTable
            };
            tModulesJournalDataGrid.DataSource = ModulesJournalBindingSource;

            MessagesBindingSource = new BindingSource()
            {
                DataSource = MessagesDataTable
            };
            tMessagesDataGrid.DataSource = MessagesBindingSource;


            ManagerEventsJournalBindingSource = new BindingSource()
            {
                DataSource = ManagerEventsJournalDT
            };
            ClientEventsJournalDataGrid.DataSource = ManagerEventsJournalBindingSource;
        }

        public void FillClients(DateTime DateFrom, DateTime DateTo)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT DISTINCT ManagerID FROM ManagersLoginJournal WHERE DateEnter >= '" + DateFrom.ToString("yyyy-MM-dd") + "' AND DateEnter <= '" + DateTo.ToString("yyyy-MM-dd") + " 23:59:59.996'", ConnectionStrings.ZOVReferenceConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    if (DT.Rows.Count != ManagersDataTable.Rows.Count)
                    {
                        using (SqlDataAdapter uDA = new SqlDataAdapter("SELECT ManagerID, Name, TopModule, TopMost FROM Managers WHERE ManagerID IN (SELECT ManagerID FROM ManagersLoginJournal WHERE DateEnter >= '" + DateFrom.ToString("yyyy-MM-dd") + "' AND DateEnter <= '" + DateTo.ToString("yyyy-MM-dd") + " 23:59:59.996')", ConnectionStrings.ZOVReferenceConnectionString))
                        {
                            ManagersDataTable.Clear();

                            uDA.Fill(ManagersDataTable);
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }

        public void FillLoginJournal(int UserID, DateTime DateFrom, DateTime DateTo)
        {
            LoginJournalDataTable.Clear();

            using (SqlDataAdapter uDA = new SqlDataAdapter("SELECT ManagersLoginJournalID, DateEnter, DateExit, ComputerParams FROM ManagersLoginJournal WHERE ManagerID = " + UserID + " AND DateEnter >= '" + DateFrom.ToString("yyyy-MM-dd") + "' AND DateEnter <= '" + DateTo.ToString("yyyy-MM-dd") + " 23:59:59.996'", ConnectionStrings.ZOVReferenceConnectionString))
            {
                uDA.Fill(LoginJournalDataTable);
            }
        }

        public void FillModulesJournal(int LoginJournalID)
        {
            ModulesJournalDataTable.Clear();

            using (SqlDataAdapter uDA = new SqlDataAdapter("SELECT ManagersModulesJournalID, DateEnter, DateExit, ModuleName FROM ManagersModulesJournal WHERE ManagersLoginJournalID = " + LoginJournalID, ConnectionStrings.ZOVReferenceConnectionString))
            {
                uDA.Fill(ModulesJournalDataTable);
            }
        }

        public void FillEventsJournal(int LoginJournalID)
        {
            ManagerEventsJournalDT.Clear();

            using (SqlDataAdapter uDA = new SqlDataAdapter("SELECT EventDateTime, ModuleName, ManagerEvent FROM ManagerEventsJournal WHERE LoginJournalID = " + LoginJournalID, ConnectionStrings.ZOVReferenceConnectionString))
            {
                uDA.Fill(ManagerEventsJournalDT);
            }
        }

        public void FillMessages(DateTime DateFrom, DateTime DateTo)
        {
            MessagesDataTable.Clear();

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT MessageID, SendDateTime, SenderID, RecipientID, SenderTypeID, RecipientTypeID FROM ZOVMessages WHERE SendDateTime >= '" + DateFrom.ToString("yyyy-MM-dd") + "' AND SendDateTime <= '" + DateTo.ToString("yyyy-MM-dd") + " 23:59:59.996'", ConnectionStrings.ZOVReferenceConnectionString))
            {
                DA.Fill(MessagesDataTable);
            }

            for (int i = 0; i < MessagesDataTable.Rows.Count; i++)
            {
                if (MessagesDataTable.Rows[i]["SenderTypeID"].ToString() == "True")
                {
                    MessagesDataTable.Rows[i]["SENDERNAME"] = (ManagersNameDT.Select("ManagerID = " + MessagesDataTable.Rows[i]["SenderID"]))[0]["Name"];
                }
                else
                {
                    MessagesDataTable.Rows[i]["SENDERNAME"] = (UsersNameDT.Select("UserID = " + MessagesDataTable.Rows[i]["SenderID"]))[0]["Name"];
                }

                if (MessagesDataTable.Rows[i]["RecipientTypeID"].ToString() == "True")
                {
                    MessagesDataTable.Rows[i]["RecipientUser"] = (ManagersNameDT.Select("ManagerID = " + MessagesDataTable.Rows[i]["RecipientID"]))[0]["Name"];
                }
                else
                {
                    MessagesDataTable.Rows[i]["RecipientUser"] = (UsersNameDT.Select("UserID = " + MessagesDataTable.Rows[i]["RecipientID"]))[0]["Name"];
                }
            }
        }

        public void FillComputerParams(int LoginJournalID)
        {
            ComputerParamsDataTable.Clear();
            ComputerParamsStuctDataTable.Clear();

            string XML = LoginJournalDataTable.Select("ManagersLoginJournalID = " + LoginJournalID)[0]["ComputerParams"].ToString();

            if (XML.Length == 0)
                return;

            using (StringReader SR = new StringReader(XML))
            {
                try
                {
                    ComputerParamsDataTable.ReadXml(SR);
                }
                catch
                {

                }
            }

            foreach (DataColumn Column in ComputerParamsDataTable.Columns)
            {
                DataRow NewRow = ComputerParamsStuctDataTable.NewRow();
                NewRow["Param"] = Column.ColumnName;
                NewRow["Value"] = ComputerParamsDataTable.Rows[0][Column.ColumnName];
                ComputerParamsStuctDataTable.Rows.Add(NewRow);
            }
        }

        public int GetOnlineUsersCount()
        {
            return ManagersDataTable.Select("OnlineStatus = 1").Count();
        }

        public void SetGrid()
        {
            ClientsGrid.AutoGenerateColumns = false;

            ClientsGrid.Columns["ManagerID"].Visible = false;
            ClientsGrid.Columns["OnlineStatus"].Visible = false;
            ClientsGrid.Columns["OnlineColumn"].DisplayIndex = 0;
            ClientsGrid.Columns["IdleTime"].DisplayIndex = 6;
            ClientsGrid.Columns["TopMost"].Visible = false;
            ClientsGrid.Columns["OnTop"].DisplayIndex = 5;
            ClientsGrid.sOnlineStatusColumnName = "OnlineStatus";

            ClientsGrid.Columns["IdleTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            ClientsGrid.Columns["IdleTime"].Width = 80;
            ClientsGrid.Columns["OnTop"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            ClientsGrid.Columns["OnTop"].Width = 40;
            ClientsGrid.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            ClientsGrid.Columns["Name"].Width = 190;

            LoginJournalDataGrid.Columns["DateEnter"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm:ss";
            LoginJournalDataGrid.Columns["DateExit"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm:ss";
            LoginJournalDataGrid.Columns["DateEnter"].HeaderText = "Дата входа";
            LoginJournalDataGrid.Columns["DateExit"].HeaderText = "Дата выхода";
            LoginJournalDataGrid.Columns["ManagersLoginJournalID"].Visible = false;
            LoginJournalDataGrid.Columns["ComputerParams"].Visible = false;

            ComputerParamsDataGrid.Columns["Param"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            ComputerParamsDataGrid.Columns["Param"].Width = 150;

            ClientEventsJournalDataGrid.Columns["ModuleName"].HeaderText = "Модуль";
            ClientEventsJournalDataGrid.Columns["EventDateTime"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm:ss";
            ClientEventsJournalDataGrid.Columns["EventDateTime"].Width = 180;
            ClientEventsJournalDataGrid.Columns["EventDateTime"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            ClientEventsJournalDataGrid.Columns["EventDateTime"].HeaderText = "Дата";
            ClientEventsJournalDataGrid.Columns["ManagerEvent"].HeaderText = "Действие";
            ClientEventsJournalDataGrid.Columns["ManagerEvent"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            ClientEventsJournalDataGrid.Columns["ModuleName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            ModulesJournalDataGrid.Columns["ManagersModulesJournalID"].Visible = false;
            ModulesJournalDataGrid.Columns["ModuleName"].HeaderText = "Название модуля";
            ModulesJournalDataGrid.Columns["DateEnter"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm:ss";
            ModulesJournalDataGrid.Columns["DateExit"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm:ss";
            ModulesJournalDataGrid.Columns["DateEnter"].HeaderText = "Дата входа";
            ModulesJournalDataGrid.Columns["DateExit"].HeaderText = "Дата выхода";

            ModulesJournalDataGrid.Columns["DateEnter"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            ModulesJournalDataGrid.Columns["DateEnter"].Width = 180;
            ModulesJournalDataGrid.Columns["DateExit"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            ModulesJournalDataGrid.Columns["DateExit"].Width = 180;


            MessagesDataGrid.Columns["SenderID"].Visible = false;
            MessagesDataGrid.Columns["RecipientID"].Visible = false;
            MessagesDataGrid.Columns["SenderTypeID"].Visible = false;
            MessagesDataGrid.Columns["RecipientTypeID"].Visible = false;

            MessagesDataGrid.Columns["SENDERNAME"].HeaderText = "От кого";
            MessagesDataGrid.Columns["SENDERNAME"].DisplayIndex = 3;

            MessagesDataGrid.Columns["RecipientUser"].HeaderText = "Кому";
            MessagesDataGrid.Columns["RecipientUser"].DisplayIndex = 4;

            MessagesDataGrid.Columns["SendDateTime"].HeaderText = "Дата отправки";
            MessagesDataGrid.Columns["MessageID"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            MessagesDataGrid.Columns["MessageID"].Width = 180;
        }

        public void ClearTopMostAndTopModuleAndIdle()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ManagerID, TopMost, TopModule, IdleTime FROM Managers WHERE Online = 0 AND (TopMost = 1 OR TopModule != '-1' OR IdleTime > -1)", ConnectionStrings.ZOVReferenceConnectionString))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        DA.Fill(DT);

                        foreach (DataRow Row in DT.Rows)
                        {
                            Row["TopMost"] = false;
                            Row["TopModule"] = -1;
                            Row["IdleTime"] = DBNull.Value;
                        }

                        DA.Update(DT);
                    }
                }
            }
        }

        public void CheckOnline()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT ManagerID, TopModule, IdleTime, Online, TopMost FROM Managers", ConnectionStrings.ZOVReferenceConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    try
                    {
                        DA.Fill(DT);
                    }
                    catch
                    {
                        return;
                    }

                    foreach (DataRow Row in ManagersDataTable.Rows)
                    {
                        DataRow[] DRow = DT.Select("ManagerID = " + Row["ManagerID"]);


                        Row["OnlineStatus"] = DRow[0]["Online"];
                        Row["TopModule"] = DRow[0]["TopModule"];
                        Row["TopMost"] = DRow[0]["TopMost"];

                        if (DRow[0]["IdleTime"] == DBNull.Value)
                            Row["IdleTime"] = "-";
                        else
                            Row["IdleTime"] = SecondsToTime(Convert.ToInt32(DRow[0]["IdleTime"]));
                    }
                }
            }


        }

        private string SecondsToTime(int Seconds)
        {
            TimeSpan t = TimeSpan.FromSeconds(Seconds);

            string answer = string.Format("{0:D2}:{1:D2}:{2:D2}",
                            t.Hours,
                            t.Minutes,
                            t.Seconds);

            return answer;
        }
    }






    public class AdminWebServiceManager
    {
        public AdminWebServiceManager()
        {

        }

        public bool StartOnlineControl()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 1 * FROM WebServiceControl", "Data Source=v02.bizneshost.by, 32433;Initial Catalog=USERS;Persist Security Info=True;Connection Timeout=15;User ID=infinium;Password=infinium1q2w3e4r"))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        try
                        {
                            if (DA.Fill(DT) == 0)
                                return false;

                            DT.Rows[0]["CurrentCommand"] = true;

                            DA.Update(DT);
                        }
                        catch
                        {
                            return false;
                        }
                    }
                }
            }

            WebService.WebService1 WS = new WebService.WebService1();
            WS.StartUsersControlAsync();

            WS.Dispose();

            return true;
        }

        public bool StopOnlineControl()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 1 * FROM WebServiceControl", "Data Source=v02.bizneshost.by, 32433;Initial Catalog=USERS;Persist Security Info=True;Connection Timeout=15;User ID=infinium;Password=infinium1q2w3e4r"))
            {
                using (SqlCommandBuilder CB = new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        try
                        {
                            if (DA.Fill(DT) == 0)
                                return false;

                            DT.Rows[0]["CurrentCommand"] = false;

                            DA.Update(DT);
                        }
                        catch
                        {
                            return false;
                        }
                    }
                }
            }

            WebService.WebService1 WS = new WebService.WebService1();
            WS.StartUsersControlAsync();

            WS.Dispose();

            return true;
        }

        public bool GetOnlineControlStatus(ref string StartDate, ref string EndDate, ref string Last)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 1 * FROM WebServiceControl", "Data Source=v02.bizneshost.by, 32433;Initial Catalog=USERS;Persist Security Info=True;Connection Timeout=15;User ID=infinium;Password=infinium1q2w3e4r"))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    if (Convert.ToBoolean(DT.Rows[0]["ControlStatus"]) == false)
                        EndDate = DT.Rows[0]["ControlEndDateTime"].ToString();
                    else
                        StartDate = DT.Rows[0]["ControlStartDateTime"].ToString();

                    Last = DT.Rows[0]["ControlLastCycleDateTime"].ToString();

                    return Convert.ToBoolean(DT.Rows[0]["ControlStatus"]);
                }
            }
        }
    }





    public class AdminOptionsClass
    {
        public AdminOptionsClass()
        {

        }

        public int SetModulesSubscribes()
        {
            int iCount = 0;

            using (SqlDataAdapter DA = new SqlDataAdapter("DELETE FROM Subscribers WHERE ModuleID NOT IN (SELECT ModuleID FROM SubscribesItems) OR UserID NOT IN (SELECT UserID FROM infiniu2_users.dbo.Users WHERE Fired <> 1 )", ConnectionStrings.LightConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    iCount += DA.Fill(DT);
                }
            }

            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT * FROM SubscribesItems", ConnectionStrings.LightConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);


                    using (SqlDataAdapter sDA = new SqlDataAdapter("SELECT * FROM Subscribers", ConnectionStrings.LightConnectionString))
                    {
                        using (SqlCommandBuilder CB = new SqlCommandBuilder(sDA))
                        {
                            using (DataTable sDT = new DataTable())
                            {
                                sDA.Fill(sDT);


                                using (SqlDataAdapter uDA = new SqlDataAdapter("SELECT UserID FROM Users WHERE Fired <> 1 ", ConnectionStrings.UsersConnectionString))
                                {
                                    using (DataTable uDT = new DataTable())
                                    {
                                        uDA.Fill(uDT);

                                        foreach (DataRow uRow in uDT.Rows)
                                        {
                                            foreach (DataRow mRow in DT.Rows)
                                            {
                                                if (sDT.Select("UserID = " + uRow["UserID"] + " AND ModuleID = " + mRow["ModuleID"]).Count() == 0)
                                                {
                                                    iCount += 1;

                                                    DataRow NewRow = sDT.NewRow();
                                                    NewRow["UserID"] = uRow["UserID"];
                                                    NewRow["ModuleID"] = mRow["ModuleID"];
                                                    sDT.Rows.Add(NewRow);
                                                }
                                            }
                                        }

                                        sDA.Update(sDT);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return iCount;
        }
    }



    public class AdminWorkDays
    {
        public DataTable UserFunctionsDataTable;
        public DataTable MinutesDataTable;
        public DataTable FunctionsDataTable;
        public DataTable DescriptionWorkDayDataTable;
        public BindingSource UserFunctionsBindingSource;
        public BindingSource CommentsBindingSource;

        PercentageDataGrid UserFunctionsDataGrid;
        public string TotalTime = "";
        public string Comments = "";
        public string TotalMin;

        public AdminWorkDays(ref PercentageDataGrid tUserFunctionsDataGrid)
        {
            UserFunctionsDataGrid = tUserFunctionsDataGrid;

            CreateAndFill();
            Binding();
            GridSettings();
        }

        public void CreateAndFill()
        {
            UserFunctionsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT UserFunctionID, FunctionID, UserID FROM UserFunctions", ConnectionStrings.LightConnectionString))
            {
                DA.Fill(UserFunctionsDataTable);
                UserFunctionsDataTable.Columns.Add("Minutes");
                UserFunctionsDataTable.Columns.Add("Percentage");
            }

            MinutesDataTable = new DataTable();

            FunctionsDataTable = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT FunctionID, FunctionName FROM Functions ORDER BY FunctionName", ConnectionStrings.LightConnectionString))
            {
                DA.Fill(FunctionsDataTable);
            }

            DescriptionWorkDayDataTable = new DataTable();

            DataRow RowOtherWorks = UserFunctionsDataTable.NewRow();
            RowOtherWorks["FunctionID"] = 0;
            RowOtherWorks["UserFunctionID"] = 0;
            UserFunctionsDataTable.Rows.Add(RowOtherWorks);
        }

        public void Binding()
        {
            UserFunctionsBindingSource = new BindingSource()
            {
                DataSource = UserFunctionsDataTable
            };
            UserFunctionsDataGrid.DataSource = UserFunctionsBindingSource;
        }

        public void GridSettings()
        {
            UserFunctionsDataGrid.Columns["FunctionID"].Visible = false;
            UserFunctionsDataGrid.Columns["UserID"].Visible = false;
            UserFunctionsDataGrid.Columns["UserFunctionID"].Visible = false;

            DataGridViewComboBoxColumn FunctionNameColumn = new DataGridViewComboBoxColumn()
            {
                HeaderText = "Обязанность",
                Name = "FunctionName",
                DataPropertyName = "FunctionID",
                DataSource = FunctionsDataTable,
                ValueMember = "FunctionID",
                DisplayMember = "FunctionName",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                DisplayIndex = 0,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };
            UserFunctionsDataGrid.Columns.Add(FunctionNameColumn);

            UserFunctionsDataGrid.Columns["Minutes"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            UserFunctionsDataGrid.Columns["Minutes"].Width = 150;
            UserFunctionsDataGrid.Columns["Minutes"].HeaderText = "Время";

            UserFunctionsDataGrid.AddPercentageColumn("Percentage");

            UserFunctionsDataGrid.Columns["Percentage"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            UserFunctionsDataGrid.Columns["Percentage"].Width = 150;
            UserFunctionsDataGrid.Columns["Percentage"].HeaderText = "Проценты";
        }

        public DataTable TableUsers()
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT UserID, Name FROM Users WHERE Fired=0 ORDER BY Name", ConnectionStrings.UsersConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(DT);

                    return DT;
                }
            }
        }

        public void UpdateUserFunctionsDataGrid(int UserID)
        {
            UserFunctionsDataTable.Select("UserFunctionID = 0")[0]["UserID"] = UserID;
            UserFunctionsBindingSource.Filter = "UserID = " + UserID;
            UserFunctionsDataTable.DefaultView.RowFilter = "UserID = " + UserID;
        }

        public void UpdatePercentageColumns()
        {
            foreach (DataRow Row in UserFunctionsDataTable.DefaultView.ToTable().Rows)
            {
                UserFunctionsDataTable.Select("UserFunctionID = " + Row["UserFunctionID"])[0]["Minutes"] = 0;
                UserFunctionsDataTable.Select("UserFunctionID = " + Row["UserFunctionID"])[0]["Percentage"] = 0;
            }
        }

        public void FillColumnMinutes()
        {
            int TotalMinutes, Minutes, Hours;
            int TotalTimeSheet = 0;

            foreach (DataRow Row in UserFunctionsDataTable.DefaultView.ToTable().Rows)
            {
                TotalMinutes = 0;

                foreach (DataRow RowMinutes in MinutesDataTable.Select("FunctionID = " + Row["UserFunctionID"]))
                {
                    TotalMinutes += Convert.ToInt32(RowMinutes["Minutes"]);
                }

                TotalTimeSheet += TotalMinutes;

                UserFunctionsDataTable.Select("UserFunctionID = " + Row["UserFunctionID"])[0]["Minutes"] = TotalMinutes;
            }

            TotalTime = TotalTimeSheet.ToString();

            foreach (DataRow Row in UserFunctionsDataTable.DefaultView.ToTable().Rows)
            {
                Minutes = 0;
                Hours = 0;
                TotalMin = "";
                if (TotalTimeSheet != 0)
                    UserFunctionsDataTable.Select("UserFunctionID = " + Row["UserFunctionID"])[0]["Percentage"] = Convert.ToInt32(Row["Minutes"]) * 100 / TotalTimeSheet;

                Hours = (Convert.ToInt32(Row["Minutes"]) / 60);
                Minutes = (Convert.ToInt32(Row["Minutes"]) - Hours * 60);
                TotalMin = Hours.ToString() + " ч " + Minutes.ToString() + " мин";

                UserFunctionsDataTable.Select("UserFunctionID = " + Row["UserFunctionID"])[0]["Minutes"] = TotalMin;

                if ((UserFunctionsDataTable.Select("UserFunctionID = " + Row["UserFunctionID"])[0]["Minutes"]).ToString() == "0 ч 0 мин")
                    UserFunctionsDataTable.Select("UserFunctionID = " + Row["UserFunctionID"])[0]["Percentage"] = 0;
            }

            if (MinutesDataTable.Rows.Count > 0)
            {
                foreach (DataRow Row in MinutesDataTable.DefaultView.ToTable().Rows)
                {
                    if (Convert.ToInt32(Row["FunctionID"]) == 0)
                        Comments = Row["Comments"].ToString();

                    if (Row["Comments"].ToString() == "")
                        Comments = "";
                }
            }
            else
                Comments = "";
        }

        public void FilterMinutes(int UserID, DateTime DateFrom, DateTime DateTo, bool OneDay)
        {
            if (OneDay)
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT Minutes, FunctionID, Comments, FactoryID FROM infiniu2_light.dbo.WorkDayDetails where WorkDayID in (SELECT WorkDayID"
                       + " FROM infiniu2_light.dbo.WorkDays where UserID = " + UserID + " and cast(DayStartDateTime as Date) = '" + DateFrom.ToString("yyyy-MM-dd") + "')", ConnectionStrings.LightConnectionString))
                {
                    MinutesDataTable.Clear();
                    DA.Fill(MinutesDataTable);
                }
            }
            else
            {
                using (SqlDataAdapter DA = new SqlDataAdapter("SELECT Minutes, FunctionID, Comments, FactoryID FROM infiniu2_light.dbo.WorkDayDetails where WorkDayID in (SELECT WorkDayID"
                       + " FROM infiniu2_light.dbo.WorkDays where UserID = " + UserID + " and cast(DayStartDateTime as Date) >= '" + DateFrom.ToString("yyyy-MM-dd") + "' and cast(DayStartDateTime as Date) <= '" + DateTo.ToString("yyyy-MM-dd")
                       + "')", ConnectionStrings.LightConnectionString))
                {
                    MinutesDataTable.Clear();
                    DA.Fill(MinutesDataTable);
                }
            }
        }

        public void DescriptionWorkDay(int UserID, DateTime DateFrom)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter("SELECT TOP 1 * FROM WorkDays where UserID = " + UserID + " and cast(DayStartDateTime as Date) = '" + DateFrom.ToString("yyyy-MM-dd") + "'", ConnectionStrings.LightConnectionString))
            {
                DescriptionWorkDayDataTable.Clear();
                DA.Fill(DescriptionWorkDayDataTable);
            }
        }
    }

}
