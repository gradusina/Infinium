using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;

using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;

namespace Infinium.Classes
{
    class PictureToExcel
    {
        public FileManager FM = new FileManager();
        static HSSFWorkbook hssfworkbook;
        DataTable TechStoreDocumentsDT;

        public PictureToExcel()
        {
            InitializeWorkbook();

            HSSFSheet sheet1 = hssfworkbook.CreateSheet("PictureSheet");
            TechStoreDocumentsDT = new DataTable();
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT TOP (2000) TechStoreName, TechStoreDocumentID FROM TechStore 
                LEFT JOIN TechStoreDocuments ON TechStore.TechStoreID = TechStoreDocuments.TechID AND DocType = 0
                WHERE TechStoreSubGroupID = 30 ORDER BY TechStoreName", ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    DA.Fill(TechStoreDocumentsDT);
                }
            }

            HSSFPatriarch patriarch = sheet1.CreateDrawingPatriarch();
            //create the anchor
            HSSFClientAnchor anchor;
            HSSFCell Cell1;

            HSSFFont HeaderF1 = hssfworkbook.CreateFont();
            HeaderF1.FontHeightInPoints = 11;
            HeaderF1.Boldweight = 11 * 256;
            HeaderF1.FontName = "Calibri";

            HSSFCellStyle ReportCS1 = hssfworkbook.CreateCellStyle();
            ReportCS1.BorderBottom = HSSFCellStyle.BORDER_MEDIUM;
            ReportCS1.BottomBorderColor = HSSFColor.BLACK.index;
            ReportCS1.SetFont(HeaderF1);

            int RowIndex = 0;
            for (int i = 0; i < TechStoreDocumentsDT.Rows.Count; i++)
            {
                string TechStoreName = TechStoreDocumentsDT.Rows[i]["TechStoreName"].ToString();

                Cell1 = sheet1.CreateRow(RowIndex).CreateCell(0);
                Cell1.SetCellValue(TechStoreName);
                Cell1.CellStyle = ReportCS1;

                if (TechStoreDocumentsDT.Rows[i]["TechStoreDocumentID"] != DBNull.Value)
                {
                    int TechStoreDocumentID = Convert.ToInt32(TechStoreDocumentsDT.Rows[i]["TechStoreDocumentID"]);
                    anchor = new HSSFClientAnchor(0, 0, 0, 255, 2, RowIndex, 5, RowIndex + 7)
                    {
                        AnchorType = 2
                    };
                    HSSFPicture picture = patriarch.CreatePicture(anchor, GetTechStoreImage(TechStoreDocumentID, hssfworkbook));
                    //picture.Resize();
                    picture.LineStyle = HSSFPicture.LINESTYLE_DASHDOTGEL;
                }
                RowIndex = RowIndex + 8;
            }

            WriteToFile();
        }

        public int GetTechStoreImage(int TechStoreDocumentID, HSSFWorkbook wb)
        {
            using (SqlDataAdapter DA = new SqlDataAdapter(@"SELECT * FROM TechStoreDocuments
                WHERE TechStoreDocumentID = " + TechStoreDocumentID, ConnectionStrings.CatalogConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) == 0)
                        return -1;

                    if (DT.Rows[0]["FileName"] == DBNull.Value)
                        return -1;

                    string FileName = DT.Rows[0]["FileName"].ToString();

                    try
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            byte[] buffer = FM.ReadFile(Configs.DocumentsPath + FileManager.GetPath("TechStoreDocuments") + "/" + FileName,
                                Convert.ToInt64(DT.Rows[0]["FileSize"]), Configs.FTPType);
                            return wb.AddPicture(buffer, HSSFWorkbook.PICTURE_TYPE_JPEG);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        return -1;
                    }
                }
            }
        }

        public static int LoadImage(string path, HSSFWorkbook wb)
        {
            FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[file.Length];
            file.Read(buffer, 0, (int)file.Length);
            return wb.AddPicture(buffer, HSSFWorkbook.PICTURE_TYPE_JPEG);

        }

        static void WriteToFile()
        {
            //Write the stream data of workbook to the root directory
            FileStream file = new FileStream(@"test.xls", FileMode.Create);
            hssfworkbook.Write(file);
            file.Close();
        }

        static void InitializeWorkbook()
        {
            hssfworkbook = new HSSFWorkbook();

            //create a entry of DocumentSummaryInformation
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "NPOI Team";
            hssfworkbook.DocumentSummaryInformation = dsi;

            //create a entry of SummaryInformation
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NPOI SDK Example";
            hssfworkbook.SummaryInformation = si;
        }
    }
}
