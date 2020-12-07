using Infinium.Modules.CabFurnitureModule;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Infinium.Modules.CabFurnitureAssignments
{
    class CabFurStorage
    {
        DataTable workShopsDt = null;
        DataTable racksDt = null;
        DataTable cellsDt = null;

        public BindingSource workShopsBs = null;
        public BindingSource racksBs = null;
        public BindingSource cellsBs = null;

        SqlDataAdapter workShopsDa = null;
        SqlCommandBuilder workShopsCb = null;

        SqlDataAdapter racksDa = null;
        SqlCommandBuilder racksCb = null;

        SqlDataAdapter cellsDa = null;
        SqlCommandBuilder cellsCb = null;

        public CabFurStorage()
        {
            CreateObjects();
            //ПОПРОБОВАТЬ НАОБОРОТ ВЫЗВАТЬ ФУНКЦИИ
            FillTables();
            BindingTables();
        }

        private void CreateObjects()
        {
            workShopsDt = new DataTable();
            racksDt = new DataTable();
            cellsDt = new DataTable();

            workShopsBs = new BindingSource();
            racksBs = new BindingSource();
            cellsBs = new BindingSource();
        }

        private void BindingTables()
        {
            workShopsBs = new BindingSource()
            {
                DataSource = workShopsDt
            };
            racksBs = new BindingSource()
            {
                DataSource = racksDt
            };
            cellsBs = new BindingSource()
            {
                DataSource = cellsDt
            };
        }

        private void FillTables()
        {
            string selectCommand = @"SELECT * FROM WorkShops ORDER BY Name";
            workShopsDa = new SqlDataAdapter(selectCommand, ConnectionStrings.StorageConnectionString);
            workShopsCb = new SqlCommandBuilder(workShopsDa);
            workShopsDa.Fill(workShopsDt);

            selectCommand = @"SELECT * FROM Racks ORDER BY Name";
            racksDa = new SqlDataAdapter(selectCommand, ConnectionStrings.StorageConnectionString);
            racksCb = new SqlCommandBuilder(racksDa);
            racksDa.Fill(racksDt);

            selectCommand = @"SELECT * FROM Cells ORDER BY Name";
            cellsDa = new SqlDataAdapter(selectCommand, ConnectionStrings.StorageConnectionString);
            cellsCb = new SqlCommandBuilder(cellsDa);
            cellsDa.Fill(cellsDt);
        }

        #region WorkShops

        public bool HasWorkShops
        {
            get { return workShopsBs.Count > 0; }
        }

        public string CurrentWorkShopName
        {
            get
            {
                string name = string.Empty;
                if (workShopsBs.Current != null)
                    name = ((DataRowView)workShopsBs.Current).Row["Name"].ToString();
                return name;
            }
        }

        public int CurrentWorkShopId
        {
            get
            {
                int id = -1;
                if (workShopsBs.Current != null)
                    id = Convert.ToInt32(((DataRowView)workShopsBs.Current).Row["WorkShopID"]);
                return id;
            }
        }

        public int MaxWorkShopId
        {
            get { return Convert.ToInt32(workShopsDt.Compute("max([WorkShopID])", string.Empty)); }
        }

        public void SetWorkShopPosition(int Id)
        {
            workShopsBs.Position = workShopsBs.Find("WorkShopID", Id);
        }

        public void AddWorkShop(string name)
        {
            DataRow NewRow = workShopsDt.NewRow();
            NewRow["Name"] = name;
            workShopsDt.Rows.Add(NewRow);
        }

        public void EditWorkShop(int id, string name)
        {
            DataRow[] rows = workShopsDt.Select("WorkShopID = " + id);
            if (rows.Count() > 0)
                rows[0]["Name"] = name;
        }

        public void RemoveWorkShop(int id)
        {
            foreach (DataRow row in workShopsDt.Select("WorkShopID = " + id))
                row.Delete();
        }

        public void UpdateWorkShops()
        {
            workShopsDt.Clear();
            workShopsDa.Fill(workShopsDt);
        }

        public void SaveWorkShops()
        {
            workShopsDa.Update(workShopsDt);
        }

        #endregion

        #region Racks

        public bool HasRacks
        {
            get { return racksBs.Count > 0; }
        }

        public string CurrentRackName
        {
            get
            {
                string name = string.Empty;
                if (racksBs.Current != null)
                    name = ((DataRowView)racksBs.Current).Row["Name"].ToString();
                return name;
            }
        }

        public int CurrentRackId
        {
            get
            {
                int id = -1;
                if (racksBs.Current != null)
                    id = Convert.ToInt32(((DataRowView)racksBs.Current).Row["RackID"]);
                return id;
            }
        }

        public int MaxRackId
        {
            get { return Convert.ToInt32(racksDt.Compute("max([RackID])", string.Empty)); }
        }

        public void SetRackPosition(int Id)
        {
            racksBs.Position = racksBs.Find("RackID", Id);
        }

        public void FilterRacksByWorkShop(int workShopId)
        {
            racksBs.Filter = "WorkShopID=" + workShopId;
            racksBs.MoveFirst();
        }

        public void AddRack(string name, int workShopId)
        {
            DataRow NewRow = racksDt.NewRow();
            NewRow["Name"] = name;
            NewRow["WorkShopID"] = workShopId;
            racksDt.Rows.Add(NewRow);
        }

        public void EditRack(int id, string name)
        {
            DataRow[] rows = racksDt.Select("RackID = " + id);
            if (rows.Count() > 0)
                rows[0]["Name"] = name;
        }

        public void RemoveRack(int id)
        {
            foreach (DataRow row in racksDt.Select("RackID = " + id))
                row.Delete();
        }

        public void UpdateRacks()
        {
            racksDt.Clear();
            racksDa.Fill(racksDt);
            racksBs.MoveFirst();
        }

        public void SaveRacks()
        {
            racksDa.Update(racksDt);
        }

        #endregion

        #region Cells

        public bool HasCells
        {
            get { return cellsBs.Count > 0; }
        }

        public string CurrentCellName
        {
            get
            {
                string name = string.Empty;
                if (cellsBs.Current != null)
                    name = ((DataRowView)cellsBs.Current).Row["Name"].ToString();
                return name;
            }
        }

        public int CurrentCellId
        {
            get
            {
                int id = -1;
                if (cellsBs.Current != null)
                    id = Convert.ToInt32(((DataRowView)cellsBs.Current).Row["CellID"]);
                return id;
            }
        }

        public int MaxCellId
        {
            get { return Convert.ToInt32(cellsDt.Compute("max([CellID])", string.Empty)); }
        }

        public void FilterCellsByRack(int rackId)
        {
            cellsBs.Filter = "RackID=" + rackId;
            cellsBs.MoveFirst();
        }

        public void AddCell(string name, int rackId)
        {
            DataRow NewRow = cellsDt.NewRow();
            NewRow["Name"] = name;
            NewRow["RackID"] = rackId;
            cellsDt.Rows.Add(NewRow);
        }

        public void EditCell(int id, string name)
        {
            DataRow[] rows = cellsDt.Select("CellID = " + id);
            if (rows.Count() > 0)
                rows[0]["Name"] = name;
        }

        public void RemoveCell(int id)
        {
            foreach (DataRow row in cellsDt.Select("CellID = " + id))
                row.Delete();
        }

        public void UpdateCells()
        {
            cellsDt.Clear();
            cellsDa.Fill(cellsDt);
        }

        public void SaveCells()
        {
            cellsDa.Update(cellsDt);
        }

        public List<CellLabelInfo> CreateCellLabels(int[] cells)
        {
            string filter = string.Empty;
            foreach (int item in cells)
                filter += item.ToString() + ",";
            if (filter.Length > 0)
                filter = " WHERE Cells.CellID IN (" + filter.Substring(0, filter.Length - 1) + ")";

            List<CellLabelInfo> Labels = new List<CellLabelInfo>();
            string SelectCommand = @"SELECT Cells.*, Racks.Name AS RackName, WorkShops.Name AS WorkShopName FROM Cells
                INNER JOIN Racks ON Cells.RackID=Racks.RackID
                INNER JOIN WorkShops ON Racks.WorkShopID=WorkShops.WorkShopID" + filter;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        for (int i = 0; i < DT.Rows.Count; i++)
                        {
                            string WorkShopName = DT.Rows[i]["WorkShopName"].ToString();
                            string RackName = DT.Rows[i]["RackName"].ToString();
                            string CellName = DT.Rows[i]["Name"].ToString();
                            int CellID = Convert.ToInt32(DT.Rows[i]["CellID"]);
                            string BarcodeNumber = GetBarcodeNumber(23, Convert.ToInt32(DT.Rows[i]["CellID"]));

                            CellLabelInfo LabelInfo = new CellLabelInfo();
                            LabelInfo.WorkShopName = WorkShopName;
                            LabelInfo.RackName = RackName;
                            LabelInfo.CellName = CellName;
                            LabelInfo.CellID = CellID;
                            LabelInfo.BarcodeNumber = BarcodeNumber;
                            LabelInfo.FactoryType = 1;

                            Labels.Add(LabelInfo);
                        }
                    }
                }
            }

            return Labels;
        }

        public string GetBarcodeNumber(int BarcodeType, int PackNumber)
        {
            string Type = "";
            if (BarcodeType.ToString().Length == 1)
                Type = "00" + BarcodeType.ToString();
            if (BarcodeType.ToString().Length == 2)
                Type = "0" + BarcodeType.ToString();
            if (BarcodeType.ToString().Length == 3)
                Type = BarcodeType.ToString();

            string Number = "";
            if (PackNumber.ToString().Length == 1)
                Number = "00000000" + PackNumber.ToString();
            if (PackNumber.ToString().Length == 2)
                Number = "0000000" + PackNumber.ToString();
            if (PackNumber.ToString().Length == 3)
                Number = "000000" + PackNumber.ToString();
            if (PackNumber.ToString().Length == 4)
                Number = "00000" + PackNumber.ToString();
            if (PackNumber.ToString().Length == 5)
                Number = "0000" + PackNumber.ToString();
            if (PackNumber.ToString().Length == 6)
                Number = "000" + PackNumber.ToString();
            if (PackNumber.ToString().Length == 7)
                Number = "00" + PackNumber.ToString();
            if (PackNumber.ToString().Length == 8)
                Number = "0" + PackNumber.ToString();

            StringBuilder BarcodeNumber = new StringBuilder(Type);
            BarcodeNumber.Append(Number);

            return BarcodeNumber.ToString();
        }

        #endregion
    }

    public struct CellLabelInfo
    {
        public string WorkShopName;
        public string RackName;
        public string CellName;
        public int CellID;
        public string BarcodeNumber;
        public int FactoryType;
    }

    public class CellLabel
    {
        Barcode Barcode;
        public PrintDocument PD;

        public int PaperHeight = 488;
        public int PaperWidth = 394;

        public int CurrentLabelNumber = 0;

        public int PrintedCount = 0;

        public bool Printed = false;

        SolidBrush FontBrush;

        Font ClientFont;
        Font DocFont;
        Font InfoFont;

        Pen Pen;

        Image ZTTPS;
        Image ZTProfil;
        Image STB;
        Image RST;

        public ArrayList LabelInfo;

        public CellLabel()
        {

            Barcode = new Barcode();

            InitializeFonts();
            InitializePrinter();

            ZTTPS = new Bitmap(Properties.Resources.ZOVTPS);
            ZTProfil = new Bitmap(Properties.Resources.ZOVPROFIL);
            STB = new Bitmap(Properties.Resources.eac);
            RST = new Bitmap(Properties.Resources.RST);

            LabelInfo = new System.Collections.ArrayList();
        }

        private void InitializePrinter()
        {
            PD = new PrintDocument();
            PD.DefaultPageSettings.PaperSize = new PaperSize("Label", PaperWidth, PaperHeight);
            PD.DefaultPageSettings.Landscape = true;
            PD.DefaultPageSettings.Margins.Bottom = 0;
            PD.DefaultPageSettings.Margins.Top = 0;
            PD.DefaultPageSettings.Margins.Left = 0;
            PD.DefaultPageSettings.Margins.Right = 0;
        }

        private void InitializeFonts()
        {
            FontBrush = new System.Drawing.SolidBrush(Color.Black);
            ClientFont = new Font("Arial", 20, FontStyle.Regular);
            DocFont = new Font("Arial", 14, FontStyle.Regular);
            InfoFont = new Font("Arial", 5.0f, FontStyle.Regular);
            Pen = new Pen(FontBrush)
            {
                Width = 1
            };
        }

        public void ClearLabelInfo()
        {
            CurrentLabelNumber = 0;
            PrintedCount = 0;
            LabelInfo.Clear();
            GC.Collect();
        }

        public void AddLabelInfo(ref CellLabelInfo LabelInfoItem)
        {
            LabelInfo.Add(LabelInfoItem);
        }

        public void PrintPage(object sender, PrintPageEventArgs ev)
        {
            if (PrintedCount >= LabelInfo.Count)
                return;
            else
                PrintedCount++;

            ev.Graphics.Clear(Color.White);

            ev.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

            int HorizLineClientBot = 18;
            ev.Graphics.DrawLine(Pen, 11, HorizLineClientBot, 467, HorizLineClientBot);

            ev.Graphics.DrawString("Цех: " + ((CellLabelInfo)LabelInfo[CurrentLabelNumber]).WorkShopName, ClientFont, FontBrush, 19, HorizLineClientBot + 82);

            ev.Graphics.DrawString("Стеллаж: " + ((CellLabelInfo)LabelInfo[CurrentLabelNumber]).RackName, ClientFont, FontBrush, 19, HorizLineClientBot + 142);

            ev.Graphics.DrawString("Ячейка: " + ((CellLabelInfo)LabelInfo[CurrentLabelNumber]).CellName, ClientFont, FontBrush, 19, HorizLineClientBot + 202);

            int HorizLinOrderBot = 75;
            int HorizLinSmallBarcode = 338;
            ev.Graphics.DrawLine(Pen, HorizLinSmallBarcode, HorizLinOrderBot, 467, HorizLinOrderBot);

            int HorizLinTableTop = 115;

            ev.Graphics.DrawString("№" + ((CellLabelInfo)LabelInfo[CurrentLabelNumber]).CellID.ToString(), DocFont, FontBrush, HorizLinSmallBarcode + 4, HorizLineClientBot + 4);
            ev.Graphics.DrawLine(Pen, HorizLinSmallBarcode, HorizLineClientBot, HorizLinSmallBarcode, HorizLinOrderBot);

            ev.Graphics.DrawImage(Barcode.GetBarcode(Barcode.BarcodeLength.Medium, 46, ((CellLabelInfo)LabelInfo[CurrentLabelNumber]).BarcodeNumber), 10, 317);

            Barcode.DrawBarcodeText(Barcode.BarcodeLength.Medium, ev.Graphics, ((CellLabelInfo)LabelInfo[CurrentLabelNumber]).BarcodeNumber, 9, 366);

            ev.Graphics.DrawImage(Barcode.GetBarcode(Barcode.BarcodeLength.Short, 15, ((CellLabelInfo)LabelInfo[CurrentLabelNumber]).BarcodeNumber), HorizLinSmallBarcode + 2, 54, 130, 15);

            if (((CellLabelInfo)LabelInfo[CurrentLabelNumber]).FactoryType == 2)
            {
                ev.Graphics.DrawImage(ZTTPS, 249, 320, 37, 45);
            }
            else
            {
                ev.Graphics.DrawImage(ZTProfil, 249, 320, 37, 45);
            }

            ev.Graphics.DrawImage(STB, 418, 319, 39, 27);
            //ev.Graphics.DrawImage(RST, 423, 357, 34, 27);

            ev.Graphics.DrawLine(Pen, 11, 315, 467, 315);
            ev.Graphics.DrawLine(Pen, 235, 315, 235, 385);

            ev.Graphics.DrawString("ГОСТ 16371-2014", InfoFont, FontBrush, 305, 318);

            if (((CellLabelInfo)LabelInfo[CurrentLabelNumber]).FactoryType == 2)
                ev.Graphics.DrawString("СООО \"ЗОВ-ТПС\"", InfoFont, FontBrush, 305, 328);
            else
                ev.Graphics.DrawString("СООО \"ЗОВ-Профиль\"", InfoFont, FontBrush, 305, 328);

            ev.Graphics.DrawString("Республика Беларусь, 230011", InfoFont, FontBrush, 305, 338);
            ev.Graphics.DrawString("г. Гродно, ул. Герасимовича, 1", InfoFont, FontBrush, 305, 348);
            ev.Graphics.DrawString("тел\\факс: (0152) 52-14-70", InfoFont, FontBrush, 305, 358);
            //ev.Graphics.DrawString("Изготовлено: " + ((PackageLabelInfo)LabelInfo[CurrentLabelNumber]).AddToStorageDateTime, InfoFont, FontBrush, 305, 368);
            ev.Graphics.DrawString("Распечатено: " + Security.GetCurrentDate().ToString(), InfoFont, FontBrush, 305, 378);
            //ev.Graphics.DrawString(((PackageLabelInfo)LabelInfo[CurrentLabelNumber]).DispatchDate, DispatchFont, FontBrush, 237, 374);

            if (CurrentLabelNumber == LabelInfo.Count - 1 || PrintedCount >= LabelInfo.Count)
                ev.HasMorePages = false;

            if (CurrentLabelNumber < LabelInfo.Count - 1 && PrintedCount < LabelInfo.Count)
            {
                ev.HasMorePages = true;
                CurrentLabelNumber++;
            }
        }

        public void Print()
        {
            PD.DefaultPageSettings.PaperSize = new PaperSize("Label", PaperWidth + 40, PaperHeight);
            PD.DefaultPageSettings.Margins.Bottom = 0;
            PD.DefaultPageSettings.Margins.Top = 0;
            PD.DefaultPageSettings.Margins.Right = 0;
            PD.DefaultPageSettings.Margins.Left = 0;

            if (!Printed)
            {
                Printed = true;
                PD.PrintPage += new PrintPageEventHandler(PrintPage);
            }

            PD.Print();
        }
    }

    public class StoragePackagesManager
    {
        DataTable PackageLabelsDT = null;
        DataTable BindPackageLabelsDT = null;
        DataTable TempPackageLabelsDT = null;
        public BindingSource PackageLabelsBS = null;
        public BindingSource BindPackageLabelsBS = null;
        SqlDataAdapter PackageLabelsDA;
        SqlDataAdapter BindPackageLabelsDA;

        DataTable PackageDetailsDT = null;
        public BindingSource PackageDetailsBS = null;
        SqlDataAdapter PackageDetailsDA;

        public StoragePackagesManager()
        {
            PackageLabelsDT = new DataTable();
            BindPackageLabelsDT = new DataTable();
            PackageDetailsDT = new DataTable();
            TempPackageLabelsDT = new DataTable();

            PackageLabelsBS = new BindingSource();
            BindPackageLabelsBS = new BindingSource();
            PackageDetailsBS = new BindingSource();

            string SelectCommand = @"SELECT TOP 0 CabFurniturePackageID, PackNumber, AddToStorageDateTime, RemoveFromStorageDateTime, QualityControlInDateTime, QualityControlOutDateTime, QualityControl, Cells.Name FROM CabFurniturePackages 
                INNER JOIN Cells ON CabFurniturePackages.CellID=Cells.CellID";
            PackageLabelsDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString);
            PackageLabelsDA.Fill(PackageLabelsDT);
            PackageLabelsDT.Columns.Add(new DataColumn("Index", Type.GetType("System.Int32")));

            SelectCommand = @"SELECT TOP 0 CabFurniturePackageID, PackNumber, AddToStorageDateTime, RemoveFromStorageDateTime, QualityControlInDateTime, QualityControlOutDateTime, QualityControl, CabFurniturePackages.CellID, Cells.Name FROM CabFurniturePackages 
                INNER JOIN Cells ON CabFurniturePackages.CellID=Cells.CellID";
            BindPackageLabelsDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString);
            BindPackageLabelsDA.Fill(BindPackageLabelsDT);
            BindPackageLabelsDA.Fill(TempPackageLabelsDT);

            SelectCommand = @"SELECT TOP 0 C.PackNumber, C.TechStoreSubGroupID, C.TechStoreID AS CTechStoreID, C.CoverID, C.PatinaID, C.InsetColorID, 
                CabFurniturePackageDetails.* FROM CabFurniturePackageDetails 
                INNER JOIN CabFurniturePackages AS C ON CabFurniturePackageDetails.CabFurniturePackageID=C.CabFurniturePackageID";
            PackageDetailsDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString);
            PackageDetailsDA.Fill(PackageDetailsDT);

            PackageLabelsBS.DataSource = PackageLabelsDT;
            BindPackageLabelsBS.DataSource = BindPackageLabelsDT;
            PackageDetailsBS.DataSource = PackageDetailsDT;
        }

        public void FilterPackagesLabels(int CellID)
        {
            PackageDetailsDT.Clear();
            string SelectCommand = @"SELECT C.PackNumber, C.TechStoreSubGroupID, C.TechStoreID AS CTechStoreID, C.CoverID, C.PatinaID, C.InsetColorID,
                CabFurniturePackageDetails.* FROM CabFurniturePackageDetails 
                INNER JOIN CabFurniturePackages AS C ON CabFurniturePackageDetails.CabFurniturePackageID=C.CabFurniturePackageID
                WHERE CellID=" + CellID;
            PackageDetailsDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString);
            PackageDetailsDA.Fill(PackageDetailsDT);

            PackageLabelsDT.Clear();
            SelectCommand = @"SELECT CabFurniturePackageID, PackNumber, AddToStorageDateTime, RemoveFromStorageDateTime, QualityControlInDateTime, QualityControlOutDateTime, QualityControl, CabFurniturePackages.CellID, Cells.Name FROM CabFurniturePackages 
                LEFT JOIN Cells ON CabFurniturePackages.CellID=Cells.CellID WHERE CabFurniturePackages.CellID=" + CellID;
            PackageLabelsDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString);
            PackageLabelsDA.Fill(PackageLabelsDT);

            for (int i = 0; i < PackageLabelsDT.Rows.Count; i++)
            {
                PackageLabelsDT.Rows[i]["Index"] = i + 1;
            }
        }

        public void Clear()
        {
            BindPackageLabelsDT.Clear();
        }

        public bool GetPackagesLabels(int CabFurniturePackageID)
        {
            TempPackageLabelsDT.Clear();
            string SelectCommand = @"SELECT CabFurniturePackageID, PackNumber, AddToStorageDateTime, RemoveFromStorageDateTime, QualityControlInDateTime, QualityControlOutDateTime, QualityControl, CabFurniturePackages.CellID, Cells.Name FROM CabFurniturePackages 
                LEFT JOIN Cells ON CabFurniturePackages.CellID=Cells.CellID WHERE CabFurniturePackageID=" + CabFurniturePackageID;
            PackageLabelsDA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString);
            PackageLabelsDA.Fill(TempPackageLabelsDT);

            if (TempPackageLabelsDT.Rows.Count > 0)
                BindPackageLabelsDT.Rows.Add(TempPackageLabelsDT.Rows[0].ItemArray);
            return TempPackageLabelsDT.Rows.Count > 0;
        }

        public void FilterPackagesDetails(int CabFurniturePackageID)
        {
            PackageDetailsBS.Filter = "CabFurniturePackageID =" + CabFurniturePackageID;
            PackageDetailsBS.MoveFirst();
        }

        public void ClearPackges()
        {
            PackageLabelsDT.Clear();
            PackageDetailsDT.Clear();
        }

        public bool IsCellExist(int cellId)
        {
            bool bExist = false;
            string SelectCommand = @"SELECT CellID FROM Cells WHERE CellID=" + cellId;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        bExist = true;
                    }
                }
            }
            return bExist;
        }
        public bool IsPackageExist(int packageId)
        {
            bool bExist = false;
            string SelectCommand = @"SELECT CabFurniturePackageID FROM CabFurniturePackages WHERE CabFurniturePackageID=" + packageId;
            using (SqlDataAdapter DA = new SqlDataAdapter(SelectCommand, ConnectionStrings.StorageConnectionString))
            {
                using (DataTable DT = new DataTable())
                {
                    if (DA.Fill(DT) > 0)
                    {
                        bExist = true;
                    }
                }
            }
            return bExist;
        }

        /// <summary>
        /// привязать упаковки к ячейке
        /// </summary>
        /// <param name="cellID"></param>
        /// <param name="packageIds"></param>
        public void BindPackagesToCell(int cellID, int[] packageIds)
        {
            string filter = string.Empty;
            foreach (int item in packageIds)
                filter += item.ToString() + ",";
            if (filter.Length > 0)
                filter = "SELECT CabFurniturePackageID, CellID, BindToCellUserID, BindToCellDateTime FROM CabFurniturePackages " +
                    "WHERE CabFurniturePackageID IN (" + filter.Substring(0, filter.Length - 1) + ")";

            using (SqlDataAdapter DA = new SqlDataAdapter(filter, ConnectionStrings.StorageConnectionString))
            {
                using (new SqlCommandBuilder(DA))
                {
                    using (DataTable DT = new DataTable())
                    {
                        if (DA.Fill(DT) > 0)
                        {
                            DateTime dateTime = Security.GetCurrentDate();

                            for (int i = 0; i < DT.Rows.Count; i++)
                            {
                                DT.Rows[i]["CellID"] = cellID;
                                DT.Rows[i]["BindToCellUserID"] = Security.CurrentUserID;
                                DT.Rows[i]["BindToCellDateTime"] = dateTime;
                            }
                            DA.Update(DT);
                        }
                    }
                }
            }
        }

    }
}
