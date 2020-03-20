using ComponentFactory.Krypton.Toolkit;

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Infinium
{
    public class PercentageDataGrid : ComponentFactory.Krypton.Toolkit.KryptonDataGridView
    {
        ArrayList ColumnsArray;

        Pen P100;//100 percent only
        Pen P90;//90-99
        Pen P80;//80-89
        Pen P70;//70-79
        Pen P60;//60-69
        Pen P50;//50-59
        Pen P40;//40-49
        Pen P30;//30-39
        Pen P20;//20-29
        Pen P10;//0-20

        SolidBrush S100;
        SolidBrush S90;
        SolidBrush S80;
        SolidBrush S70;
        SolidBrush S60;
        SolidBrush S50;
        SolidBrush S40;
        SolidBrush S30;
        SolidBrush S20;
        SolidBrush S10;

        SolidBrush CommonCellBackBrush;
        SolidBrush SelectedCellBackBrush;

        Pen pCheckedRectPen;

        string sBackText = "Нет данных";

        Rectangle rCheckBoxRect;

        bool NeedRefresh = false;

        Font NoDataLabelFont;
        SolidBrush NoDataLabelBrush;

        Bitmap CheckedBMPCommon = new Bitmap(Properties.Resources.checkblack);
        Bitmap CheckedBMPSelected = new Bitmap(Properties.Resources.checkwhite);

        int iPercentLineWidth;

        int MpX = 0;
        int MpY = 0;

        bool bStandardStyle;

        bool bUseCustomBackColor = false;

        public enum ColorStyle { Green, Blue, Orange };

        public ColorStyle cSelectedColorStyle;

        public ColorStyle SelectedColorStyle
        {
            get { return cSelectedColorStyle; }
            set { cSelectedColorStyle = value; SetStyle(); this.Refresh(); }
        }

        public int PercentLineWidth
        {
            get { return iPercentLineWidth; }
            set
            {
                iPercentLineWidth = value;

                P100.Width = iPercentLineWidth;
                P90.Width = iPercentLineWidth;
                P80.Width = iPercentLineWidth;
                P70.Width = iPercentLineWidth;
                P60.Width = iPercentLineWidth;
                P50.Width = iPercentLineWidth;
                P40.Width = iPercentLineWidth;
                P30.Width = iPercentLineWidth;
                P20.Width = iPercentLineWidth;
                P10.Width = iPercentLineWidth;
            }
        }

        public PercentageDataGrid()
        {
            ColumnsArray = new ArrayList();

            S100 = new SolidBrush(Color.FromArgb(26, 228, 28));
            S90 = new SolidBrush(Color.FromArgb(169, 242, 14));
            S80 = new SolidBrush(Color.FromArgb(255, 242, 0));
            S70 = new SolidBrush(Color.FromArgb(255, 211, 0));
            S60 = new SolidBrush(Color.FromArgb(255, 173, 0));
            S50 = new SolidBrush(Color.FromArgb(255, 133, 0));
            S40 = new SolidBrush(Color.FromArgb(255, 101, 0));
            S30 = new SolidBrush(Color.FromArgb(255, 80, 0));
            S20 = new SolidBrush(Color.FromArgb(255, 52, 0));
            S10 = new SolidBrush(Color.FromArgb(255, 0, 0));
            CommonCellBackBrush = new SolidBrush(Color.FromArgb(255, 230, 230, 230));
            SelectedCellBackBrush = new SolidBrush(Color.FromArgb(255, 230, 230, 230));

            P100 = new Pen(S100, iPercentLineWidth);
            P90 = new Pen(S90, iPercentLineWidth);
            P80 = new Pen(S80, iPercentLineWidth);
            P70 = new Pen(S70, iPercentLineWidth);
            P60 = new Pen(S60, iPercentLineWidth);
            P50 = new Pen(S50, iPercentLineWidth);
            P40 = new Pen(S40, iPercentLineWidth);
            P30 = new Pen(S30, iPercentLineWidth);
            P20 = new Pen(S20, iPercentLineWidth);
            P10 = new Pen(S10, iPercentLineWidth);

            NoDataLabelFont = new System.Drawing.Font("Segoe UI", 12.0f, FontStyle.Regular);
            NoDataLabelBrush = new SolidBrush(Color.FromArgb(140, 140, 140));

            SetStyle();

            pCheckedRectPen = new Pen(new SolidBrush(Color.Black));

            rCheckBoxRect = new Rectangle(0, 0, 14, 14);

            cSelectedColorStyle = ColorStyle.Orange;
        }

        public bool StandardStyle
        {
            get { return bStandardStyle; }
            set { bStandardStyle = value; SetStyle(); }
        }

        private void SetStyle()
        {
            if (bUseCustomBackColor)
                this.StateCommon.Background.Color1 = Security.GridsBackColor;
            else
                this.StateCommon.Background.Color1 = Color.White;

            if (bUseCustomBackColor)
                this.StateCommon.DataCell.Back.Color1 = Security.GridsBackColor;
            else
                this.StateCommon.DataCell.Back.Color1 = Color.White;

            if (bStandardStyle == false)
                return;

            this.ReadOnly = true;

            Font DataCellFont = new Font("SEGOE UI", 15.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            Font HeaderFont = new Font("SEGOE UI", 15.0f, FontStyle.Bold, GraphicsUnit.Pixel);

            this.AllowUserToAddRows = false;
            this.AllowUserToDeleteRows = false;
            this.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.AllowUserToOrderColumns = false;
            this.AllowUserToResizeColumns = false;
            this.AllowUserToResizeRows = false;
            this.ColumnHeadersHeight = 55;
            this.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.RowTemplate.Height = 40;
            this.RowHeadersVisible = false;



            this.StateCommon.Background.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;



            this.StateCommon.DataCell.Content.Font = DataCellFont;
            this.StateCommon.DataCell.Content.Color1 = Color.Black;
            this.StateCommon.DataCell.Border.Color1 = Color.FromArgb(210, 210, 210);
            this.StateCommon.DataCell.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.StateCommon.HeaderColumn.Content.Font = HeaderFont;
            this.StateCommon.HeaderColumn.Content.Color1 = Color.White;
            this.StateCommon.HeaderColumn.Border.Color1 = Color.Black;
            this.StateCommon.HeaderColumn.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.StateCommon.HeaderColumn.Back.Color1 = Color.FromArgb(150, 150, 150);
            this.StateCommon.HeaderColumn.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;

            this.StateSelected.DataCell.Content.Color1 = Color.White;

            if (cSelectedColorStyle == PercentageDataGrid.ColorStyle.Blue)
                this.StateSelected.DataCell.Back.Color1 = Color.FromArgb(121, 177, 229);
            if (cSelectedColorStyle == ColorStyle.Green)
                this.StateSelected.DataCell.Back.Color1 = Color.FromArgb(31, 158, 0);
            if (cSelectedColorStyle == ColorStyle.Orange)
                this.StateSelected.DataCell.Back.Color1 = Color.FromArgb(253, 164, 61);

            this.StateSelected.DataCell.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;


            CommonCellBackBrush.Color = this.StateCommon.DataCell.Back.Color1;
            SelectedCellBackBrush.Color = this.StateSelected.DataCell.Back.Color1;

            this.MultiSelect = false;
        }

        public void AddPercentageColumn(string ColumnName)
        {
            ColumnsArray.Add(ColumnName);
        }

        Pen GetPen(int Value)
        {
            if (Value == 100)
                return P100;
            if (Value >= 90 && Value <= 99)
                return P90;
            if (Value >= 80 && Value <= 89)
                return P80;
            if (Value >= 70 && Value <= 79)
                return P70;
            if (Value >= 60 && Value <= 69)
                return P60;
            if (Value >= 50 && Value <= 59)
                return P50;
            if (Value >= 40 && Value <= 49)
                return P40;
            if (Value >= 30 && Value <= 39)
                return P30;
            if (Value >= 20 && Value <= 29)
                return P20;
            if (Value >= 0 && Value <= 19)
                return P10;

            return null;
        }

        int GetLength(int Value, int CellWidth, int CellHeight)
        {
            return Convert.ToInt32(Convert.ToDecimal(CellWidth - 10) * Convert.ToDecimal(Value) / 100);
        }

        public string BackText
        {
            get { return sBackText; }
            set { sBackText = value; }
        }

        protected override void OnCellPainting(DataGridViewCellPaintingEventArgs e)
        {
            base.OnCellPainting(e);

            if (e.RowIndex == -1)
                return;

            if (e.ColumnIndex == -1)
                return;

            //Checkbox
            if (this.Columns[e.ColumnIndex] is DataGridViewCheckBoxColumn)
            {
                if (this.Rows[e.RowIndex].Selected)
                {
                    SelectedCellBackBrush.Color = StateSelected.DataCell.Back.Color1;
                    if (!this.Enabled)
                        SelectedCellBackBrush.Color = this.StateDisabled.DataCell.Back.Color1;

                    e.Graphics.FillRectangle(SelectedCellBackBrush, e.CellBounds.Left + ((e.CellBounds.Right - e.CellBounds.Left) / 2 - CheckedBMPCommon.Width / 2), e.CellBounds.Top + 1, 25, 25);

                    pCheckedRectPen.Color = Color.White;

                    rCheckBoxRect.Y = e.CellBounds.Top + (e.CellBounds.Height - rCheckBoxRect.Height) / 2;
                    rCheckBoxRect.X = e.CellBounds.Left + (e.CellBounds.Width - rCheckBoxRect.Width) / 2;

                    e.Graphics.DrawRectangle(pCheckedRectPen, rCheckBoxRect);

                    try
                    {
                        if (Convert.ToBoolean(e.Value) == true)
                            e.Graphics.DrawImage(CheckedBMPSelected, e.CellBounds.Left + ((e.CellBounds.Right - e.CellBounds.Left) / 2 - CheckedBMPCommon.Width / 2) + 1, e.CellBounds.Top + ((e.CellBounds.Bottom - e.CellBounds.Top) / 2 - CheckedBMPCommon.Height / 2) - 1, CheckedBMPCommon.Width, CheckedBMPCommon.Height);
                    }
                    catch
                    {
                        //e.Graphics.DrawImage(CheckedBMPCommon, e.CellBounds.Left + ((e.CellBounds.Right - e.CellBounds.Left) / 2 - CheckedBMPCommon.Width / 2) + 1, e.CellBounds.Top + 2, CheckedBMPCommon.Width, CheckedBMPCommon.Height);
                    }
                }
                else
                {
                    CommonCellBackBrush.Color = this.StateCommon.DataCell.Back.Color1;
                    if (!this.Enabled)
                        CommonCellBackBrush.Color = this.StateDisabled.DataCell.Back.Color1;

                    if (!this.Rows[e.RowIndex].DefaultCellStyle.BackColor.IsEmpty)
                        if (this.Rows[e.RowIndex].DefaultCellStyle.BackColor != CommonCellBackBrush.Color)
                            CommonCellBackBrush.Color = this.Rows[e.RowIndex].DefaultCellStyle.BackColor;

                    e.Graphics.FillRectangle(CommonCellBackBrush, e.CellBounds.Left + ((e.CellBounds.Right - e.CellBounds.Left) / 2 - CheckedBMPCommon.Width / 2), e.CellBounds.Top + 1, 25, 25);

                    pCheckedRectPen.Color = Color.FromArgb(121, 121, 121);

                    rCheckBoxRect.Y = e.CellBounds.Top + (e.CellBounds.Height - rCheckBoxRect.Height) / 2;
                    rCheckBoxRect.X = e.CellBounds.Left + (e.CellBounds.Width - rCheckBoxRect.Width) / 2;

                    e.Graphics.DrawRectangle(pCheckedRectPen, rCheckBoxRect);

                    try
                    {
                        if (Convert.ToBoolean(e.Value) == true)
                            e.Graphics.DrawImage(CheckedBMPCommon, e.CellBounds.Left + ((e.CellBounds.Right - e.CellBounds.Left) / 2 - CheckedBMPCommon.Width / 2) + 1, e.CellBounds.Top + ((e.CellBounds.Bottom - e.CellBounds.Top) / 2 - CheckedBMPCommon.Height / 2) - 1, CheckedBMPCommon.Width, CheckedBMPCommon.Height);

                    }
                    catch
                    {

                    }
                }
            }

            //Percents
            if (ColumnsArray.Count == 0)
                return;

            if (ColumnsArray.Contains(Columns[e.ColumnIndex].Name))
            {
                if (e.Value != null && e.Value.ToString().Length > 0 && (Convert.ToInt32(e.Value) >= 0 && Convert.ToInt32(e.Value) <= 100))
                {
                    e.Graphics.DrawLine(GetPen(Convert.ToInt32(e.Value)), e.CellBounds.Left + 4, e.CellBounds.Bottom - 7, (e.CellBounds.Left + 4) +
                        GetLength(Convert.ToInt32(e.Value), e.CellBounds.Width, e.CellBounds.Height), e.CellBounds.Bottom - 7);
                }
            }
        }

        protected override void OnCellClick(DataGridViewCellEventArgs e)
        {
            base.OnCellClick(e);

            if (this.Columns[e.ColumnIndex] is DataGridViewCheckBoxColumn)
            {
                if (this.Columns[e.ColumnIndex].ReadOnly == true)
                    return;

                Rectangle CB = this.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);

                if (MpX < CB.X + (CB.Width - rCheckBoxRect.Width) / 2 || MpX > CB.X + (CB.Width - rCheckBoxRect.Width) / 2 + rCheckBoxRect.Width ||
                    MpY < CB.Y + (CB.Height - rCheckBoxRect.Height) / 2 || MpY > CB.Y + (CB.Height - rCheckBoxRect.Height) / 2 + rCheckBoxRect.Height)
                    return;

                if (e.RowIndex == -1)//header
                    return;

                if (Convert.ToBoolean(this.Rows[e.RowIndex].Cells[e.ColumnIndex].Value) == true)
                    this.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = false;
                else
                    this.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = true;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            MpX = e.X;
            MpY = e.Y;
        }

        protected override void OnScroll(ScrollEventArgs e)
        {
            base.OnScroll(e);

            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
                NeedRefresh = true;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (NeedRefresh)
            {
                this.Refresh();
                NeedRefresh = false;
            }
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);

            SetStyle();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (this.RowCount == 0)
            {
                e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                e.Graphics.DrawString(sBackText, NoDataLabelFont, NoDataLabelBrush,
                            (this.ClientRectangle.Width - e.Graphics.MeasureString(sBackText, NoDataLabelFont).Width) / 2 + 4,
                            (this.ClientRectangle.Height - e.Graphics.MeasureString(sBackText, NoDataLabelFont).Height) / 2);
            }
        }

        public bool UseCustomBackColor
        {
            get { return bUseCustomBackColor; }
            set { bUseCustomBackColor = value; this.Refresh(); }
        }
    }


    public class UsersMessagesDataGrid : ComponentFactory.Krypton.Toolkit.KryptonDataGridView
    {
        ArrayList ColumnsArray;

        Pen P100;//100 percent only
        Pen P90;//90-99
        Pen P80;//80-89
        Pen P70;//70-79
        Pen P60;//60-69
        Pen P50;//50-59
        Pen P40;//40-49
        Pen P30;//30-39
        Pen P20;//20-29
        Pen P10;//0-20

        SolidBrush S100;
        SolidBrush S90;
        SolidBrush S80;
        SolidBrush S70;
        SolidBrush S60;
        SolidBrush S50;
        SolidBrush S40;
        SolidBrush S30;
        SolidBrush S20;
        SolidBrush S10;

        SolidBrush CommonCellBackBrush;
        SolidBrush SelectedCellBackBrush;

        Pen pCheckedRectPen;

        string sBackText = "Нет данных";

        Rectangle rCheckBoxRect;

        bool NeedRefresh = false;

        Font NoDataLabelFont;
        SolidBrush NoDataLabelBrush;

        Bitmap CheckedBMPCommon = new Bitmap(Properties.Resources.checkblack);
        Bitmap CheckedBMPSelected = new Bitmap(Properties.Resources.checkwhite);

        int iPercentLineWidth;

        int MpX = 0;
        int MpY = 0;

        bool bStandardStyle;
        public bool bDrawOnlineImage = true;
        bool bUseCustomBackColor = false;

        public enum ColorStyle { Green, Blue, Orange };

        public ColorStyle cSelectedColorStyle;

        public ColorStyle SelectedColorStyle
        {
            get { return cSelectedColorStyle; }
            set { cSelectedColorStyle = value; SetStyle(); this.Refresh(); }
        }

        public int PercentLineWidth
        {
            get { return iPercentLineWidth; }
            set
            {
                iPercentLineWidth = value;

                P100.Width = iPercentLineWidth;
                P90.Width = iPercentLineWidth;
                P80.Width = iPercentLineWidth;
                P70.Width = iPercentLineWidth;
                P60.Width = iPercentLineWidth;
                P50.Width = iPercentLineWidth;
                P40.Width = iPercentLineWidth;
                P30.Width = iPercentLineWidth;
                P20.Width = iPercentLineWidth;
                P10.Width = iPercentLineWidth;
            }
        }

        public string sOnlineStatusColumnName = "";
        public string sNewMessagesColumnName = "";

        Bitmap OnlineBMP;
        Bitmap OfflineBMP;




        public UsersMessagesDataGrid()
        {
            ColumnsArray = new ArrayList();

            S100 = new SolidBrush(Color.FromArgb(26, 228, 28));
            S90 = new SolidBrush(Color.FromArgb(169, 242, 14));
            S80 = new SolidBrush(Color.FromArgb(255, 242, 0));
            S70 = new SolidBrush(Color.FromArgb(255, 211, 0));
            S60 = new SolidBrush(Color.FromArgb(255, 173, 0));
            S50 = new SolidBrush(Color.FromArgb(255, 133, 0));
            S40 = new SolidBrush(Color.FromArgb(255, 101, 0));
            S30 = new SolidBrush(Color.FromArgb(255, 80, 0));
            S20 = new SolidBrush(Color.FromArgb(255, 52, 0));
            S10 = new SolidBrush(Color.FromArgb(255, 0, 0));
            CommonCellBackBrush = new SolidBrush(Color.FromArgb(255, 230, 230, 230));
            SelectedCellBackBrush = new SolidBrush(Color.FromArgb(255, 230, 230, 230));

            P100 = new Pen(S100, iPercentLineWidth);
            P90 = new Pen(S90, iPercentLineWidth);
            P80 = new Pen(S80, iPercentLineWidth);
            P70 = new Pen(S70, iPercentLineWidth);
            P60 = new Pen(S60, iPercentLineWidth);
            P50 = new Pen(S50, iPercentLineWidth);
            P40 = new Pen(S40, iPercentLineWidth);
            P30 = new Pen(S30, iPercentLineWidth);
            P20 = new Pen(S20, iPercentLineWidth);
            P10 = new Pen(S10, iPercentLineWidth);

            NoDataLabelFont = new System.Drawing.Font("Segoe UI", 12.0f, FontStyle.Regular);
            NoDataLabelBrush = new SolidBrush(Color.FromArgb(140, 140, 140));

            SetStyle();

            pCheckedRectPen = new Pen(new SolidBrush(Color.Black));

            rCheckBoxRect = new Rectangle(0, 0, 14, 14);

            cSelectedColorStyle = ColorStyle.Orange;

            OnlineBMP = new Bitmap(Properties.Resources.Online);
            OfflineBMP = new Bitmap(Properties.Resources.Offline);
        }

        public void AddColumns()
        {
            DataGridViewImageColumn CloseColumn = new DataGridViewImageColumn()
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                Width = 40,
                Name = "CloseColumn",
                Image = global::Infinium.Properties.Resources.CloseGrid
            };
            CloseColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.Columns.Add(CloseColumn);

            DataGridViewTextBoxColumn OnlineColumn = new DataGridViewTextBoxColumn()
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                Width = 40,
                Name = "OnlineColumn"
            };
            OnlineColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.Columns.Add(OnlineColumn);
        }

        public void OnTimedEvent()
        {
            //if (sNewMessagesColumnName == "")
            //    return;

            //foreach (DataGridViewRow Row in this.Rows)
            //{
            //    if (Row.Cells[sNewMessagesColumnName].Value == DBNull.Value)
            //        continue;

            //    if(bDrawUpdatesImage)
            //        if (Convert.ToInt32(Row.Cells[sNewMessagesColumnName].Value) > 0)
            //            G.DrawImage(NewMessageBMP, Row.Cells[sNewMessagesColumnName].ContentBounds.Left, Row.Cells[sNewMessagesColumnName].ContentBounds.Top);
            //}

            //bDrawUpdatesImage = !bDrawUpdatesImage;
        }

        public bool StandardStyle
        {
            get { return bStandardStyle; }
            set { bStandardStyle = value; SetStyle(); }
        }

        private void SetStyle()
        {
            if (bUseCustomBackColor)
                this.StateCommon.Background.Color1 = Security.GridsBackColor;
            else
                this.StateCommon.Background.Color1 = Color.White;

            if (bUseCustomBackColor)
                this.StateCommon.DataCell.Back.Color1 = Security.GridsBackColor;
            else
                this.StateCommon.DataCell.Back.Color1 = Color.White;

            if (bStandardStyle == false)
                return;

            Font DataCellFont = new Font("SEGOE UI", 15.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            Font HeaderFont = new Font("SEGOE UI", 15.0f, FontStyle.Bold, GraphicsUnit.Pixel);

            this.AllowUserToAddRows = false;
            this.AllowUserToDeleteRows = false;
            this.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.AllowUserToOrderColumns = false;
            this.AllowUserToResizeColumns = false;
            this.AllowUserToResizeRows = false;
            this.ColumnHeadersHeight = 55;
            //this.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.RowTemplate.Height = 40;
            this.RowHeadersVisible = false;



            this.StateCommon.Background.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;



            this.StateCommon.DataCell.Content.Font = DataCellFont;
            this.StateCommon.DataCell.Content.Color1 = Color.Black;
            this.StateCommon.DataCell.Border.Color1 = Color.FromArgb(210, 210, 210);
            this.StateCommon.DataCell.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.StateCommon.HeaderColumn.Content.Font = HeaderFont;
            this.StateCommon.HeaderColumn.Content.Color1 = Color.White;
            this.StateCommon.HeaderColumn.Border.Color1 = Color.Black;
            this.StateCommon.HeaderColumn.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.StateCommon.HeaderColumn.Back.Color1 = Color.FromArgb(150, 150, 150);
            this.StateCommon.HeaderColumn.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;

            this.StateSelected.DataCell.Content.Color1 = Color.White;

            if (cSelectedColorStyle == UsersMessagesDataGrid.ColorStyle.Blue)
                this.StateSelected.DataCell.Back.Color1 = Color.FromArgb(121, 177, 229);
            if (cSelectedColorStyle == ColorStyle.Green)
                this.StateSelected.DataCell.Back.Color1 = Color.FromArgb(31, 158, 0);
            if (cSelectedColorStyle == ColorStyle.Orange)
                this.StateSelected.DataCell.Back.Color1 = Color.FromArgb(253, 164, 61);

            this.StateSelected.DataCell.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;


            CommonCellBackBrush.Color = this.StateCommon.DataCell.Back.Color1;
            SelectedCellBackBrush.Color = this.StateSelected.DataCell.Back.Color1;
        }

        public void AddPercentageColumn(string ColumnName)
        {
            ColumnsArray.Add(ColumnName);
        }

        Pen GetPen(int Value)
        {
            if (Value == 100)
                return P100;
            if (Value >= 90 && Value <= 99)
                return P90;
            if (Value >= 80 && Value <= 89)
                return P80;
            if (Value >= 70 && Value <= 79)
                return P70;
            if (Value >= 60 && Value <= 69)
                return P60;
            if (Value >= 50 && Value <= 59)
                return P50;
            if (Value >= 40 && Value <= 49)
                return P40;
            if (Value >= 30 && Value <= 39)
                return P30;
            if (Value >= 20 && Value <= 29)
                return P20;
            if (Value >= 0 && Value <= 19)
                return P10;

            return null;
        }

        int GetLength(int Value, int CellWidth, int CellHeight)
        {
            return Convert.ToInt32(Convert.ToDecimal(CellWidth - 10) * Convert.ToDecimal(Value) / 100);
        }

        [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            typeof(UITypeEditor))]
        public string BackText
        {
            get { return sBackText; }
            set { sBackText = value; }
        }

        protected override void OnCellPainting(DataGridViewCellPaintingEventArgs e)
        {
            base.OnCellPainting(e);

            if (e.RowIndex == -1)
                return;

            if (e.ColumnIndex == -1)
                return;

            //Checkbox
            if (this.Columns[e.ColumnIndex] is DataGridViewCheckBoxColumn)
            {
                if (e.RowIndex == this.SelectedRows[0].Index)
                {
                    e.Graphics.FillRectangle(SelectedCellBackBrush, e.CellBounds.Left + ((e.CellBounds.Right - e.CellBounds.Left) / 2 - CheckedBMPCommon.Width / 2), e.CellBounds.Top + 3, 32, 32);

                    pCheckedRectPen.Color = Color.White;

                    rCheckBoxRect.Y = e.CellBounds.Top + (e.CellBounds.Height - rCheckBoxRect.Height) / 2;
                    rCheckBoxRect.X = e.CellBounds.Left + (e.CellBounds.Width - rCheckBoxRect.Width) / 2;

                    e.Graphics.DrawRectangle(pCheckedRectPen, rCheckBoxRect);

                    if (Convert.ToBoolean(e.Value) == true)
                        e.Graphics.DrawImage(CheckedBMPSelected, e.CellBounds.Left + ((e.CellBounds.Right - e.CellBounds.Left) / 2 - CheckedBMPCommon.Width / 2) + 1, e.CellBounds.Top + 2, CheckedBMPCommon.Width, CheckedBMPCommon.Height);
                }
                else
                {
                    e.Graphics.FillRectangle(CommonCellBackBrush, e.CellBounds.Left + ((e.CellBounds.Right - e.CellBounds.Left) / 2 - CheckedBMPCommon.Width / 2), e.CellBounds.Top + 3, 32, 32);

                    pCheckedRectPen.Color = Color.FromArgb(121, 121, 121);

                    rCheckBoxRect.Y = e.CellBounds.Top + (e.CellBounds.Height - rCheckBoxRect.Height) / 2;
                    rCheckBoxRect.X = e.CellBounds.Left + (e.CellBounds.Width - rCheckBoxRect.Width) / 2;

                    e.Graphics.DrawRectangle(pCheckedRectPen, rCheckBoxRect);

                    if (e.Value != DBNull.Value)
                        if (Convert.ToBoolean(e.Value) == true)
                            e.Graphics.DrawImage(CheckedBMPCommon, e.CellBounds.Left + ((e.CellBounds.Right - e.CellBounds.Left) / 2 - CheckedBMPCommon.Width / 2) + 1, e.CellBounds.Top + 2, CheckedBMPCommon.Width, CheckedBMPCommon.Height);
                }


            }

            //Updates
            if (e.ColumnIndex == Columns["OnlineColumn"].Index)
            {
                if (sOnlineStatusColumnName == "")
                    return;

                if (sNewMessagesColumnName == "")
                    return;

                if (Convert.ToInt32(Rows[e.RowIndex].Cells[sNewMessagesColumnName].Value) > 0)
                {
                    if (bDrawOnlineImage)
                    {

                        if (Rows[e.RowIndex].Cells[sOnlineStatusColumnName].Value != DBNull.Value)
                        {
                            if (e.Value != DBNull.Value)
                            {
                                if (Convert.ToBoolean(Rows[e.RowIndex].Cells[sOnlineStatusColumnName].Value) == true)
                                    e.Graphics.DrawImage(OnlineBMP, e.CellBounds.Left + ((e.CellBounds.Right - e.CellBounds.Left) / 2 - (OnlineBMP.Width - 5) / 2), e.CellBounds.Top + ((e.CellBounds.Bottom - e.CellBounds.Top) / 2 - (OnlineBMP.Height - 5) / 2) - 1, OnlineBMP.Width - 5, OnlineBMP.Height - 5);
                                else
                                    e.Graphics.DrawImage(OfflineBMP, e.CellBounds.Left + ((e.CellBounds.Right - e.CellBounds.Left) / 2 - (OfflineBMP.Width - 5) / 2), e.CellBounds.Top + ((e.CellBounds.Bottom - e.CellBounds.Top) / 2 - (OfflineBMP.Height - 5) / 2) - 1, OfflineBMP.Width - 5, OfflineBMP.Height - 5);
                            }

                        }
                    }
                }
                else
                {
                    if (Rows[e.RowIndex].Cells[sOnlineStatusColumnName].Value != DBNull.Value)
                    {
                        if (e.Value != DBNull.Value)
                        {
                            if (Convert.ToBoolean(Rows[e.RowIndex].Cells[sOnlineStatusColumnName].Value) == true)
                                e.Graphics.DrawImage(OnlineBMP, e.CellBounds.Left + ((e.CellBounds.Right - e.CellBounds.Left) / 2 - (OnlineBMP.Width - 5) / 2), e.CellBounds.Top + ((e.CellBounds.Bottom - e.CellBounds.Top) / 2 - (OnlineBMP.Height - 5) / 2) - 1, OnlineBMP.Width - 5, OnlineBMP.Height - 5);
                            else
                                e.Graphics.DrawImage(OfflineBMP, e.CellBounds.Left + ((e.CellBounds.Right - e.CellBounds.Left) / 2 - (OfflineBMP.Width - 5) / 2), e.CellBounds.Top + ((e.CellBounds.Bottom - e.CellBounds.Top) / 2 - (OfflineBMP.Height - 5) / 2) - 1, OfflineBMP.Width - 5, OfflineBMP.Height - 5);
                        }

                    }
                }
            }


            //Percents
            if (ColumnsArray.Count == 0)
                return;

            if (ColumnsArray.Contains(Columns[e.ColumnIndex].Name))
            {
                if (e.Value.ToString().Length > 0 && (Convert.ToInt32(e.Value) >= 0 && Convert.ToInt32(e.Value) <= 100))
                {
                    e.Graphics.DrawLine(GetPen(Convert.ToInt32(e.Value)), e.CellBounds.Left + 4, e.CellBounds.Bottom - 7, (e.CellBounds.Left + 4) +
                        GetLength(Convert.ToInt32(e.Value), e.CellBounds.Width, e.CellBounds.Height), e.CellBounds.Bottom - 7);
                }
            }
        }

        protected override void OnCellStateChanged(DataGridViewCellStateChangedEventArgs e)
        {
            base.OnCellStateChanged(e);

            if (e.Cell.OwningColumn.Name == "OnlineColumn" || e.Cell.OwningColumn.Name == "CloseColumn")
            {
                e.Cell.Selected = false;
                this.Rows[e.Cell.RowIndex].Cells["Name"].Selected = true;
            }
        }

        protected override void OnCellClick(DataGridViewCellEventArgs e)
        {
            base.OnCellClick(e);

            if (this.Columns[e.ColumnIndex] is DataGridViewCheckBoxColumn)
            {
                if (this.Columns[e.ColumnIndex].ReadOnly == true)
                    return;

                Rectangle CB = this.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);

                if (MpX < CB.X + (CB.Width - rCheckBoxRect.Width) / 2 || MpX > CB.X + (CB.Width - rCheckBoxRect.Width) / 2 + rCheckBoxRect.Width ||
                    MpY < CB.Y + (CB.Height - rCheckBoxRect.Height) / 2 || MpY > CB.Y + (CB.Height - rCheckBoxRect.Height) / 2 + rCheckBoxRect.Height)
                    return;

                if (Convert.ToBoolean(this.Rows[e.RowIndex].Cells[e.ColumnIndex].Value) == true)
                    this.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = false;
                else
                    this.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = true;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            MpX = e.X;
            MpY = e.Y;
        }

        protected override void OnScroll(ScrollEventArgs e)
        {
            base.OnScroll(e);

            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
                NeedRefresh = true;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (NeedRefresh)
            {
                this.Refresh();
                NeedRefresh = false;
            }
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);

            SetStyle();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (this.RowCount == 0)
            {
                e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                e.Graphics.DrawString(sBackText, NoDataLabelFont, NoDataLabelBrush,
                            (this.ClientRectangle.Width - e.Graphics.MeasureString(sBackText, NoDataLabelFont).Width) / 2 + 4,
                            (this.ClientRectangle.Height - e.Graphics.MeasureString(sBackText, NoDataLabelFont).Height) / 2);
            }
        }

        public bool UseCustomBackColor
        {
            get { return bUseCustomBackColor; }
            set { bUseCustomBackColor = value; this.Refresh(); }
        }
    }


    public class ClientsMessagesDataGrid : ComponentFactory.Krypton.Toolkit.KryptonDataGridView
    {
        ArrayList ColumnsArray;

        Pen P100;//100 percent only
        Pen P90;//90-99
        Pen P80;//80-89
        Pen P70;//70-79
        Pen P60;//60-69
        Pen P50;//50-59
        Pen P40;//40-49
        Pen P30;//30-39
        Pen P20;//20-29
        Pen P10;//0-20

        SolidBrush S100;
        SolidBrush S90;
        SolidBrush S80;
        SolidBrush S70;
        SolidBrush S60;
        SolidBrush S50;
        SolidBrush S40;
        SolidBrush S30;
        SolidBrush S20;
        SolidBrush S10;

        SolidBrush CommonCellBackBrush;
        SolidBrush SelectedCellBackBrush;

        Pen pCheckedRectPen;

        string sBackText = "Нет данных";

        Rectangle rCheckBoxRect;

        bool NeedRefresh = false;

        Font NoDataLabelFont;
        SolidBrush NoDataLabelBrush;

        Bitmap CheckedBMPCommon = new Bitmap(Properties.Resources.checkblack);
        Bitmap CheckedBMPSelected = new Bitmap(Properties.Resources.checkwhite);

        int iPercentLineWidth;

        int MpX = 0;
        int MpY = 0;

        bool bStandardStyle;
        public bool bDrawOnlineImage = true;
        bool bUseCustomBackColor = false;

        public enum ColorStyle { Green, Blue, Orange };

        public ColorStyle cSelectedColorStyle;

        public ColorStyle SelectedColorStyle
        {
            get { return cSelectedColorStyle; }
            set { cSelectedColorStyle = value; SetStyle(); this.Refresh(); }
        }

        public int PercentLineWidth
        {
            get { return iPercentLineWidth; }
            set
            {
                iPercentLineWidth = value;

                P100.Width = iPercentLineWidth;
                P90.Width = iPercentLineWidth;
                P80.Width = iPercentLineWidth;
                P70.Width = iPercentLineWidth;
                P60.Width = iPercentLineWidth;
                P50.Width = iPercentLineWidth;
                P40.Width = iPercentLineWidth;
                P30.Width = iPercentLineWidth;
                P20.Width = iPercentLineWidth;
                P10.Width = iPercentLineWidth;
            }
        }

        public string sOnlineStatusColumnName = "";
        public string sNewMessagesColumnName = "";

        Bitmap OnlineBMP;
        Bitmap OfflineBMP;




        public ClientsMessagesDataGrid()
        {
            ColumnsArray = new ArrayList();

            S100 = new SolidBrush(Color.FromArgb(26, 228, 28));
            S90 = new SolidBrush(Color.FromArgb(169, 242, 14));
            S80 = new SolidBrush(Color.FromArgb(255, 242, 0));
            S70 = new SolidBrush(Color.FromArgb(255, 211, 0));
            S60 = new SolidBrush(Color.FromArgb(255, 173, 0));
            S50 = new SolidBrush(Color.FromArgb(255, 133, 0));
            S40 = new SolidBrush(Color.FromArgb(255, 101, 0));
            S30 = new SolidBrush(Color.FromArgb(255, 80, 0));
            S20 = new SolidBrush(Color.FromArgb(255, 52, 0));
            S10 = new SolidBrush(Color.FromArgb(255, 0, 0));
            CommonCellBackBrush = new SolidBrush(Color.FromArgb(255, 230, 230, 230));
            SelectedCellBackBrush = new SolidBrush(Color.FromArgb(255, 230, 230, 230));

            P100 = new Pen(S100, iPercentLineWidth);
            P90 = new Pen(S90, iPercentLineWidth);
            P80 = new Pen(S80, iPercentLineWidth);
            P70 = new Pen(S70, iPercentLineWidth);
            P60 = new Pen(S60, iPercentLineWidth);
            P50 = new Pen(S50, iPercentLineWidth);
            P40 = new Pen(S40, iPercentLineWidth);
            P30 = new Pen(S30, iPercentLineWidth);
            P20 = new Pen(S20, iPercentLineWidth);
            P10 = new Pen(S10, iPercentLineWidth);

            NoDataLabelFont = new System.Drawing.Font("Segoe UI", 12.0f, FontStyle.Regular);
            NoDataLabelBrush = new SolidBrush(Color.FromArgb(140, 140, 140));

            SetStyle();

            pCheckedRectPen = new Pen(new SolidBrush(Color.Black));

            rCheckBoxRect = new Rectangle(0, 0, 14, 14);

            cSelectedColorStyle = ColorStyle.Orange;

            OnlineBMP = new Bitmap(Properties.Resources.Online);
            OfflineBMP = new Bitmap(Properties.Resources.Offline);
        }

        public void AddColumns()
        {
            DataGridViewImageColumn CloseColumn = new DataGridViewImageColumn()
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                Width = 40,
                Name = "CloseColumn",
                Image = global::Infinium.Properties.Resources.CloseGrid
            };
            CloseColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.Columns.Add(CloseColumn);

            DataGridViewTextBoxColumn OnlineColumn = new DataGridViewTextBoxColumn()
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                Width = 40,
                Name = "OnlineColumn"
            };
            OnlineColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.Columns.Add(OnlineColumn);
        }

        public void OnTimedEvent()
        {
            //if (sNewMessagesColumnName == "")
            //    return;

            //foreach (DataGridViewRow Row in this.Rows)
            //{
            //    if (Row.Cells[sNewMessagesColumnName].Value == DBNull.Value)
            //        continue;

            //    if(bDrawUpdatesImage)
            //        if (Convert.ToInt32(Row.Cells[sNewMessagesColumnName].Value) > 0)
            //            G.DrawImage(NewMessageBMP, Row.Cells[sNewMessagesColumnName].ContentBounds.Left, Row.Cells[sNewMessagesColumnName].ContentBounds.Top);
            //}

            //bDrawUpdatesImage = !bDrawUpdatesImage;
        }

        public bool StandardStyle
        {
            get { return bStandardStyle; }
            set { bStandardStyle = value; SetStyle(); }
        }

        private void SetStyle()
        {
            if (bUseCustomBackColor)
                this.StateCommon.Background.Color1 = Security.GridsBackColor;
            else
                this.StateCommon.Background.Color1 = Color.White;

            if (bUseCustomBackColor)
                this.StateCommon.DataCell.Back.Color1 = Security.GridsBackColor;
            else
                this.StateCommon.DataCell.Back.Color1 = Color.White;

            if (bStandardStyle == false)
                return;

            Font DataCellFont = new Font("SEGOE UI", 15.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            Font HeaderFont = new Font("SEGOE UI", 15.0f, FontStyle.Bold, GraphicsUnit.Pixel);

            this.AllowUserToAddRows = false;
            this.AllowUserToDeleteRows = false;
            this.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.AllowUserToOrderColumns = false;
            this.AllowUserToResizeColumns = false;
            this.AllowUserToResizeRows = false;
            this.ColumnHeadersHeight = 55;
            //this.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.RowTemplate.Height = 40;
            this.RowHeadersVisible = false;



            this.StateCommon.Background.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;



            this.StateCommon.DataCell.Content.Font = DataCellFont;
            this.StateCommon.DataCell.Content.Color1 = Color.Black;
            this.StateCommon.DataCell.Border.Color1 = Color.FromArgb(210, 210, 210);
            this.StateCommon.DataCell.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.StateCommon.HeaderColumn.Content.Font = HeaderFont;
            this.StateCommon.HeaderColumn.Content.Color1 = Color.White;
            this.StateCommon.HeaderColumn.Border.Color1 = Color.Black;
            this.StateCommon.HeaderColumn.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.StateCommon.HeaderColumn.Back.Color1 = Color.FromArgb(150, 150, 150);
            this.StateCommon.HeaderColumn.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;

            this.StateSelected.DataCell.Content.Color1 = Color.White;

            if (cSelectedColorStyle == ClientsMessagesDataGrid.ColorStyle.Blue)
                this.StateSelected.DataCell.Back.Color1 = Color.FromArgb(121, 177, 229);
            if (cSelectedColorStyle == ColorStyle.Green)
                this.StateSelected.DataCell.Back.Color1 = Color.FromArgb(31, 158, 0);
            if (cSelectedColorStyle == ColorStyle.Orange)
                this.StateSelected.DataCell.Back.Color1 = Color.FromArgb(253, 164, 61);

            this.StateSelected.DataCell.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;


            CommonCellBackBrush.Color = this.StateCommon.DataCell.Back.Color1;
            SelectedCellBackBrush.Color = this.StateSelected.DataCell.Back.Color1;
        }

        public void AddPercentageColumn(string ColumnName)
        {
            ColumnsArray.Add(ColumnName);
        }

        Pen GetPen(int Value)
        {
            if (Value == 100)
                return P100;
            if (Value >= 90 && Value <= 99)
                return P90;
            if (Value >= 80 && Value <= 89)
                return P80;
            if (Value >= 70 && Value <= 79)
                return P70;
            if (Value >= 60 && Value <= 69)
                return P60;
            if (Value >= 50 && Value <= 59)
                return P50;
            if (Value >= 40 && Value <= 49)
                return P40;
            if (Value >= 30 && Value <= 39)
                return P30;
            if (Value >= 20 && Value <= 29)
                return P20;
            if (Value >= 0 && Value <= 19)
                return P10;

            return null;
        }

        int GetLength(int Value, int CellWidth, int CellHeight)
        {
            return Convert.ToInt32(Convert.ToDecimal(CellWidth - 10) * Convert.ToDecimal(Value) / 100);
        }

        [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            typeof(UITypeEditor))]
        public string BackText
        {
            get { return sBackText; }
            set { sBackText = value; }
        }

        protected override void OnCellPainting(DataGridViewCellPaintingEventArgs e)
        {
            base.OnCellPainting(e);

            if (e.RowIndex == -1)
                return;

            if (e.ColumnIndex == -1)
                return;

            //Checkbox
            if (this.Columns[e.ColumnIndex] is DataGridViewCheckBoxColumn)
            {
                if (this.SelectedRows.Count > 0)
                    if (e.RowIndex == this.SelectedRows[0].Index)
                    {
                        e.Graphics.FillRectangle(SelectedCellBackBrush, e.CellBounds.Left + ((e.CellBounds.Right - e.CellBounds.Left) / 2 - CheckedBMPCommon.Width / 2), e.CellBounds.Top + 3, 32, 32);

                        pCheckedRectPen.Color = Color.White;

                        rCheckBoxRect.Y = e.CellBounds.Top + (e.CellBounds.Height - rCheckBoxRect.Height) / 2;
                        rCheckBoxRect.X = e.CellBounds.Left + (e.CellBounds.Width - rCheckBoxRect.Width) / 2;

                        e.Graphics.DrawRectangle(pCheckedRectPen, rCheckBoxRect);

                        if (Convert.ToBoolean(e.Value) == true)
                            e.Graphics.DrawImage(CheckedBMPSelected, e.CellBounds.Left + ((e.CellBounds.Right - e.CellBounds.Left) / 2 - CheckedBMPCommon.Width / 2) + 1, e.CellBounds.Top + 2, CheckedBMPCommon.Width, CheckedBMPCommon.Height);
                    }
                    else
                    {
                        e.Graphics.FillRectangle(CommonCellBackBrush, e.CellBounds.Left + ((e.CellBounds.Right - e.CellBounds.Left) / 2 - CheckedBMPCommon.Width / 2), e.CellBounds.Top + 3, 32, 32);

                        pCheckedRectPen.Color = Color.FromArgb(121, 121, 121);

                        rCheckBoxRect.Y = e.CellBounds.Top + (e.CellBounds.Height - rCheckBoxRect.Height) / 2;
                        rCheckBoxRect.X = e.CellBounds.Left + (e.CellBounds.Width - rCheckBoxRect.Width) / 2;

                        e.Graphics.DrawRectangle(pCheckedRectPen, rCheckBoxRect);

                        if (e.Value != DBNull.Value)
                            if (Convert.ToBoolean(e.Value) == true)
                                e.Graphics.DrawImage(CheckedBMPCommon, e.CellBounds.Left + ((e.CellBounds.Right - e.CellBounds.Left) / 2 - CheckedBMPCommon.Width / 2) + 1, e.CellBounds.Top + 2, CheckedBMPCommon.Width, CheckedBMPCommon.Height);
                    }


            }

            //Updates
            if (e.ColumnIndex == Columns["OnlineColumn"].Index)
            {
                if (sOnlineStatusColumnName == "")
                    return;

                if (sNewMessagesColumnName == "")
                    return;

                if (Convert.ToInt32(Rows[e.RowIndex].Cells[sNewMessagesColumnName].Value) > 0)
                {
                    if (bDrawOnlineImage)
                    {

                        if (Rows[e.RowIndex].Cells[sOnlineStatusColumnName].Value != DBNull.Value)
                        {
                            if (e.Value != DBNull.Value)
                            {
                                if (Convert.ToBoolean(Rows[e.RowIndex].Cells[sOnlineStatusColumnName].Value) == true)
                                    e.Graphics.DrawImage(OnlineBMP, e.CellBounds.Left + ((e.CellBounds.Right - e.CellBounds.Left) / 2 - (OnlineBMP.Width - 5) / 2), e.CellBounds.Top + ((e.CellBounds.Bottom - e.CellBounds.Top) / 2 - (OnlineBMP.Height - 5) / 2) - 1, OnlineBMP.Width - 5, OnlineBMP.Height - 5);
                                else
                                    e.Graphics.DrawImage(OfflineBMP, e.CellBounds.Left + ((e.CellBounds.Right - e.CellBounds.Left) / 2 - (OfflineBMP.Width - 5) / 2), e.CellBounds.Top + ((e.CellBounds.Bottom - e.CellBounds.Top) / 2 - (OfflineBMP.Height - 5) / 2) - 1, OfflineBMP.Width - 5, OfflineBMP.Height - 5);
                            }

                        }
                    }
                }
                else
                {
                    if (Rows[e.RowIndex].Cells[sOnlineStatusColumnName].Value != DBNull.Value)
                    {
                        if (e.Value != DBNull.Value)
                        {
                            if (Convert.ToBoolean(Rows[e.RowIndex].Cells[sOnlineStatusColumnName].Value) == true)
                                e.Graphics.DrawImage(OnlineBMP, e.CellBounds.Left + ((e.CellBounds.Right - e.CellBounds.Left) / 2 - (OnlineBMP.Width - 5) / 2), e.CellBounds.Top + ((e.CellBounds.Bottom - e.CellBounds.Top) / 2 - (OnlineBMP.Height - 5) / 2) - 1, OnlineBMP.Width - 5, OnlineBMP.Height - 5);
                            else
                                e.Graphics.DrawImage(OfflineBMP, e.CellBounds.Left + ((e.CellBounds.Right - e.CellBounds.Left) / 2 - (OfflineBMP.Width - 5) / 2), e.CellBounds.Top + ((e.CellBounds.Bottom - e.CellBounds.Top) / 2 - (OfflineBMP.Height - 5) / 2) - 1, OfflineBMP.Width - 5, OfflineBMP.Height - 5);
                        }

                    }
                }
            }


            //Percents
            if (ColumnsArray.Count == 0)
                return;

            if (ColumnsArray.Contains(Columns[e.ColumnIndex].Name))
            {
                if (e.Value.ToString().Length > 0 && (Convert.ToInt32(e.Value) >= 0 && Convert.ToInt32(e.Value) <= 100))
                {
                    e.Graphics.DrawLine(GetPen(Convert.ToInt32(e.Value)), e.CellBounds.Left + 4, e.CellBounds.Bottom - 7, (e.CellBounds.Left + 4) +
                        GetLength(Convert.ToInt32(e.Value), e.CellBounds.Width, e.CellBounds.Height), e.CellBounds.Bottom - 7);
                }
            }
        }

        protected override void OnCellStateChanged(DataGridViewCellStateChangedEventArgs e)
        {
            base.OnCellStateChanged(e);

            if (e.Cell.OwningColumn.Name == "OnlineColumn" || e.Cell.OwningColumn.Name == "CloseColumn")
            {
                e.Cell.Selected = false;
                try
                {
                    this.Rows[e.Cell.RowIndex].Cells["Name"].Selected = true;
                }
                catch
                {
                    this.Rows[e.Cell.RowIndex].Cells["ClientName"].Selected = true;
                }
            }
        }

        protected override void OnCellClick(DataGridViewCellEventArgs e)
        {
            base.OnCellClick(e);

            if (this.Columns[e.ColumnIndex] is DataGridViewCheckBoxColumn)
            {
                if (this.Columns[e.ColumnIndex].ReadOnly == true)
                    return;

                Rectangle CB = this.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);

                if (MpX < CB.X + (CB.Width - rCheckBoxRect.Width) / 2 || MpX > CB.X + (CB.Width - rCheckBoxRect.Width) / 2 + rCheckBoxRect.Width ||
                    MpY < CB.Y + (CB.Height - rCheckBoxRect.Height) / 2 || MpY > CB.Y + (CB.Height - rCheckBoxRect.Height) / 2 + rCheckBoxRect.Height)
                    return;

                if (Convert.ToBoolean(this.Rows[e.RowIndex].Cells[e.ColumnIndex].Value) == true)
                    this.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = false;
                else
                    this.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = true;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            MpX = e.X;
            MpY = e.Y;
        }

        protected override void OnScroll(ScrollEventArgs e)
        {
            base.OnScroll(e);

            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
                NeedRefresh = true;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (NeedRefresh)
            {
                this.Refresh();
                NeedRefresh = false;
            }
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);

            SetStyle();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (this.RowCount == 0)
            {
                e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                e.Graphics.DrawString(sBackText, NoDataLabelFont, NoDataLabelBrush,
                            (this.ClientRectangle.Width - e.Graphics.MeasureString(sBackText, NoDataLabelFont).Width) / 2 + 4,
                            (this.ClientRectangle.Height - e.Graphics.MeasureString(sBackText, NoDataLabelFont).Height) / 2);
            }
        }

        public bool UseCustomBackColor
        {
            get { return bUseCustomBackColor; }
            set { bUseCustomBackColor = value; this.Refresh(); }
        }
    }


    public class UsersDataGrid : ComponentFactory.Krypton.Toolkit.KryptonDataGridView
    {
        ArrayList ColumnsArray;

        Pen P100;//100 percent only
        Pen P90;//90-99
        Pen P80;//80-89
        Pen P70;//70-79
        Pen P60;//60-69
        Pen P50;//50-59
        Pen P40;//40-49
        Pen P30;//30-39
        Pen P20;//20-29
        Pen P10;//0-20

        SolidBrush S100;
        SolidBrush S90;
        SolidBrush S80;
        SolidBrush S70;
        SolidBrush S60;
        SolidBrush S50;
        SolidBrush S40;
        SolidBrush S30;
        SolidBrush S20;
        SolidBrush S10;

        SolidBrush CommonCellBackBrush;
        SolidBrush SelectedCellBackBrush;

        Pen pCheckedRectPen;

        string sBackText = "Нет данных";

        Rectangle rCheckBoxRect;

        bool NeedRefresh = false;

        Font NoDataLabelFont;
        SolidBrush NoDataLabelBrush;

        Bitmap CheckedBMPCommon = new Bitmap(Properties.Resources.checkblack);
        Bitmap CheckedBMPSelected = new Bitmap(Properties.Resources.checkwhite);

        int iPercentLineWidth;

        int MpX = 0;
        int MpY = 0;

        bool bStandardStyle;
        public bool bDrawUpdatesImage = true;
        bool bUseCustomBackColor = false;

        public enum ColorStyle { Green, Blue, Orange };

        public ColorStyle cSelectedColorStyle;

        public ColorStyle SelectedColorStyle
        {
            get { return cSelectedColorStyle; }
            set { cSelectedColorStyle = value; SetStyle(); this.Refresh(); }
        }

        public int PercentLineWidth
        {
            get { return iPercentLineWidth; }
            set
            {
                iPercentLineWidth = value;

                P100.Width = iPercentLineWidth;
                P90.Width = iPercentLineWidth;
                P80.Width = iPercentLineWidth;
                P70.Width = iPercentLineWidth;
                P60.Width = iPercentLineWidth;
                P50.Width = iPercentLineWidth;
                P40.Width = iPercentLineWidth;
                P30.Width = iPercentLineWidth;
                P20.Width = iPercentLineWidth;
                P10.Width = iPercentLineWidth;
            }
        }

        public string sOnlineStatusColumnName = "";

        Bitmap OnlineBMP;




        public UsersDataGrid()
        {
            ColumnsArray = new ArrayList();

            S100 = new SolidBrush(Color.FromArgb(26, 228, 28));
            S90 = new SolidBrush(Color.FromArgb(169, 242, 14));
            S80 = new SolidBrush(Color.FromArgb(255, 242, 0));
            S70 = new SolidBrush(Color.FromArgb(255, 211, 0));
            S60 = new SolidBrush(Color.FromArgb(255, 173, 0));
            S50 = new SolidBrush(Color.FromArgb(255, 133, 0));
            S40 = new SolidBrush(Color.FromArgb(255, 101, 0));
            S30 = new SolidBrush(Color.FromArgb(255, 80, 0));
            S20 = new SolidBrush(Color.FromArgb(255, 52, 0));
            S10 = new SolidBrush(Color.FromArgb(255, 0, 0));
            CommonCellBackBrush = new SolidBrush(Color.FromArgb(255, 230, 230, 230));
            SelectedCellBackBrush = new SolidBrush(Color.FromArgb(255, 230, 230, 230));

            P100 = new Pen(S100, iPercentLineWidth);
            P90 = new Pen(S90, iPercentLineWidth);
            P80 = new Pen(S80, iPercentLineWidth);
            P70 = new Pen(S70, iPercentLineWidth);
            P60 = new Pen(S60, iPercentLineWidth);
            P50 = new Pen(S50, iPercentLineWidth);
            P40 = new Pen(S40, iPercentLineWidth);
            P30 = new Pen(S30, iPercentLineWidth);
            P20 = new Pen(S20, iPercentLineWidth);
            P10 = new Pen(S10, iPercentLineWidth);

            NoDataLabelFont = new System.Drawing.Font("Segoe UI", 12.0f, FontStyle.Regular);
            NoDataLabelBrush = new SolidBrush(Color.FromArgb(140, 140, 140));

            SetStyle();

            pCheckedRectPen = new Pen(new SolidBrush(Color.Black));

            rCheckBoxRect = new Rectangle(0, 0, 14, 14);

            cSelectedColorStyle = ColorStyle.Orange;

            OnlineBMP = new Bitmap(Properties.Resources.Online);
        }

        public void AddColumns()
        {
            DataGridViewTextBoxColumn OnlineColumn = new DataGridViewTextBoxColumn()
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                Width = 40,
                Name = "OnlineColumn"
            };
            OnlineColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.Columns.Add(OnlineColumn);
        }

        public void OnTimedEvent()
        {
            //if (sNewMessagesColumnName == "")
            //    return;

            //foreach (DataGridViewRow Row in this.Rows)
            //{
            //    if (Row.Cells[sNewMessagesColumnName].Value == DBNull.Value)
            //        continue;

            //    if(bDrawUpdatesImage)
            //        if (Convert.ToInt32(Row.Cells[sNewMessagesColumnName].Value) > 0)
            //            G.DrawImage(NewMessageBMP, Row.Cells[sNewMessagesColumnName].ContentBounds.Left, Row.Cells[sNewMessagesColumnName].ContentBounds.Top);
            //}

            //bDrawUpdatesImage = !bDrawUpdatesImage;
        }

        public bool StandardStyle
        {
            get { return bStandardStyle; }
            set { bStandardStyle = value; SetStyle(); }
        }

        private void SetStyle()
        {
            if (bUseCustomBackColor)
                this.StateCommon.Background.Color1 = Security.GridsBackColor;
            else
                this.StateCommon.Background.Color1 = Color.White;

            if (bUseCustomBackColor)
                this.StateCommon.DataCell.Back.Color1 = Security.GridsBackColor;
            else
                this.StateCommon.DataCell.Back.Color1 = Color.White;

            if (bStandardStyle == false)
                return;

            Font DataCellFont = new Font("SEGOE UI", 15.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            Font HeaderFont = new Font("SEGOE UI", 15.0f, FontStyle.Bold, GraphicsUnit.Pixel);

            this.AllowUserToAddRows = false;
            this.AllowUserToDeleteRows = false;
            this.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.AllowUserToOrderColumns = false;
            this.AllowUserToResizeColumns = false;
            this.AllowUserToResizeRows = false;
            this.ColumnHeadersHeight = 55;
            //this.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.RowTemplate.Height = 40;
            this.RowHeadersVisible = false;



            this.StateCommon.Background.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;



            this.StateCommon.DataCell.Content.Font = DataCellFont;
            this.StateCommon.DataCell.Content.Color1 = Color.Black;
            this.StateCommon.DataCell.Border.Color1 = Color.FromArgb(210, 210, 210);
            this.StateCommon.DataCell.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.StateCommon.HeaderColumn.Content.Font = HeaderFont;
            this.StateCommon.HeaderColumn.Content.Color1 = Color.White;
            this.StateCommon.HeaderColumn.Border.Color1 = Color.Black;
            this.StateCommon.HeaderColumn.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.StateCommon.HeaderColumn.Back.Color1 = Color.FromArgb(150, 150, 150);
            this.StateCommon.HeaderColumn.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;

            this.StateSelected.DataCell.Content.Color1 = Color.White;

            if (cSelectedColorStyle == UsersDataGrid.ColorStyle.Blue)
                this.StateSelected.DataCell.Back.Color1 = Color.FromArgb(121, 177, 229);
            if (cSelectedColorStyle == ColorStyle.Green)
                this.StateSelected.DataCell.Back.Color1 = Color.FromArgb(31, 158, 0);
            if (cSelectedColorStyle == ColorStyle.Orange)
                this.StateSelected.DataCell.Back.Color1 = Color.FromArgb(253, 164, 61);

            this.StateSelected.DataCell.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;


            CommonCellBackBrush.Color = this.StateCommon.DataCell.Back.Color1;
            SelectedCellBackBrush.Color = this.StateSelected.DataCell.Back.Color1;
        }

        public void AddPercentageColumn(string ColumnName)
        {
            ColumnsArray.Add(ColumnName);
        }

        Pen GetPen(int Value)
        {
            if (Value == 100)
                return P100;
            if (Value >= 90 && Value <= 99)
                return P90;
            if (Value >= 80 && Value <= 89)
                return P80;
            if (Value >= 70 && Value <= 79)
                return P70;
            if (Value >= 60 && Value <= 69)
                return P60;
            if (Value >= 50 && Value <= 59)
                return P50;
            if (Value >= 40 && Value <= 49)
                return P40;
            if (Value >= 30 && Value <= 39)
                return P30;
            if (Value >= 20 && Value <= 29)
                return P20;
            if (Value >= 0 && Value <= 19)
                return P10;

            return null;
        }

        int GetLength(int Value, int CellWidth, int CellHeight)
        {
            return Convert.ToInt32(Convert.ToDecimal(CellWidth - 10) * Convert.ToDecimal(Value) / 100);
        }

        public string BackText
        {
            get { return sBackText; }
            set { sBackText = value; }
        }

        protected override void OnCellPainting(DataGridViewCellPaintingEventArgs e)
        {
            base.OnCellPainting(e);

            if (e.RowIndex == -1)
                return;

            if (e.ColumnIndex == -1)
                return;

            //Checkbox
            if (this.Columns[e.ColumnIndex] is DataGridViewCheckBoxColumn)
            {
                if (e.RowIndex == this.SelectedRows[0].Index)
                {
                    e.Graphics.FillRectangle(SelectedCellBackBrush, e.CellBounds.Left + ((e.CellBounds.Right - e.CellBounds.Left) / 2 - CheckedBMPCommon.Width / 2), e.CellBounds.Top + 3, 32, 32);

                    pCheckedRectPen.Color = Color.White;

                    rCheckBoxRect.Y = e.CellBounds.Top + (e.CellBounds.Height - rCheckBoxRect.Height) / 2;
                    rCheckBoxRect.X = e.CellBounds.Left + (e.CellBounds.Width - rCheckBoxRect.Width) / 2;

                    e.Graphics.DrawRectangle(pCheckedRectPen, rCheckBoxRect);

                    if (e.Value != DBNull.Value)
                        if (Convert.ToBoolean(e.Value) == true)
                            e.Graphics.DrawImage(CheckedBMPSelected, e.CellBounds.Left + ((e.CellBounds.Right - e.CellBounds.Left) / 2 - CheckedBMPCommon.Width / 2) + 1, e.CellBounds.Top + 2, CheckedBMPCommon.Width, CheckedBMPCommon.Height);
                }
                else
                {
                    e.Graphics.FillRectangle(CommonCellBackBrush, e.CellBounds.Left + ((e.CellBounds.Right - e.CellBounds.Left) / 2 - CheckedBMPCommon.Width / 2), e.CellBounds.Top + 3, 32, 32);

                    pCheckedRectPen.Color = Color.FromArgb(121, 121, 121);

                    rCheckBoxRect.Y = e.CellBounds.Top + (e.CellBounds.Height - rCheckBoxRect.Height) / 2;
                    rCheckBoxRect.X = e.CellBounds.Left + (e.CellBounds.Width - rCheckBoxRect.Width) / 2;

                    e.Graphics.DrawRectangle(pCheckedRectPen, rCheckBoxRect);

                    if (e.Value != DBNull.Value)
                        if (Convert.ToBoolean(e.Value) == true)
                            e.Graphics.DrawImage(CheckedBMPCommon, e.CellBounds.Left + ((e.CellBounds.Right - e.CellBounds.Left) / 2 - CheckedBMPCommon.Width / 2) + 1, e.CellBounds.Top + 2, CheckedBMPCommon.Width, CheckedBMPCommon.Height);
                }


            }

            //Updates
            if (bDrawUpdatesImage)
                if (e.ColumnIndex == Columns["OnlineColumn"].Index)
                {
                    if (sOnlineStatusColumnName == "")
                        return;

                    if (Rows[e.RowIndex].Cells[sOnlineStatusColumnName].Value != DBNull.Value)
                    {
                        if (e.Value != DBNull.Value)
                            if (Convert.ToBoolean(Rows[e.RowIndex].Cells[sOnlineStatusColumnName].Value) == true)
                                e.Graphics.DrawImage(OnlineBMP, e.CellBounds.Left + ((e.CellBounds.Right - e.CellBounds.Left) / 2 - (OnlineBMP.Width - 5) / 2), e.CellBounds.Top + ((e.CellBounds.Bottom - e.CellBounds.Top) / 2 - (OnlineBMP.Height - 5) / 2) - 1, OnlineBMP.Width - 5, OnlineBMP.Height - 5);
                    }
                }


            //Percents
            if (ColumnsArray.Count == 0)
                return;

            if (ColumnsArray.Contains(Columns[e.ColumnIndex].Name))
            {
                if (e.Value.ToString().Length > 0 && (Convert.ToInt32(e.Value) >= 0 && Convert.ToInt32(e.Value) <= 100))
                {
                    e.Graphics.DrawLine(GetPen(Convert.ToInt32(e.Value)), e.CellBounds.Left + 4, e.CellBounds.Bottom - 7, (e.CellBounds.Left + 4) +
                        GetLength(Convert.ToInt32(e.Value), e.CellBounds.Width, e.CellBounds.Height), e.CellBounds.Bottom - 7);
                }
            }
        }

        protected override void OnCellStateChanged(DataGridViewCellStateChangedEventArgs e)
        {
            base.OnCellStateChanged(e);

            if (e.Cell.OwningColumn.Name == "OnlineColumn")
            {
                e.Cell.Selected = false;
                this.Rows[e.Cell.RowIndex].Cells["Name"].Selected = true;
            }
        }

        protected override void OnCellClick(DataGridViewCellEventArgs e)
        {
            base.OnCellClick(e);

            if (this.Columns[e.ColumnIndex] is DataGridViewCheckBoxColumn)
            {
                if (this.Columns[e.ColumnIndex].ReadOnly == true)
                    return;

                Rectangle CB = this.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);

                if (MpX < CB.X + (CB.Width - rCheckBoxRect.Width) / 2 || MpX > CB.X + (CB.Width - rCheckBoxRect.Width) / 2 + rCheckBoxRect.Width ||
                    MpY < CB.Y + (CB.Height - rCheckBoxRect.Height) / 2 || MpY > CB.Y + (CB.Height - rCheckBoxRect.Height) / 2 + rCheckBoxRect.Height)
                    return;

                if (Convert.ToBoolean(this.Rows[e.RowIndex].Cells[e.ColumnIndex].Value) == true)
                    this.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = false;
                else
                    this.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = true;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            MpX = e.X;
            MpY = e.Y;
        }

        protected override void OnScroll(ScrollEventArgs e)
        {
            base.OnScroll(e);

            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
                NeedRefresh = true;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (NeedRefresh)
            {
                this.Refresh();
                NeedRefresh = false;
            }
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);

            SetStyle();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (this.RowCount == 0)
            {
                e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                e.Graphics.DrawString(sBackText, NoDataLabelFont, NoDataLabelBrush,
                            (this.ClientRectangle.Width - e.Graphics.MeasureString(sBackText, NoDataLabelFont).Width) / 2 + 4,
                            (this.ClientRectangle.Height - e.Graphics.MeasureString(sBackText, NoDataLabelFont).Height) / 2);
            }
        }

        public bool UseCustomBackColor
        {
            get { return bUseCustomBackColor; }
            set { bUseCustomBackColor = value; this.Refresh(); }
        }
    }


    public class ClientsDataGrid : ComponentFactory.Krypton.Toolkit.KryptonDataGridView
    {
        ArrayList ColumnsArray;

        Pen P100;//100 percent only
        Pen P90;//90-99
        Pen P80;//80-89
        Pen P70;//70-79
        Pen P60;//60-69
        Pen P50;//50-59
        Pen P40;//40-49
        Pen P30;//30-39
        Pen P20;//20-29
        Pen P10;//0-20

        SolidBrush S100;
        SolidBrush S90;
        SolidBrush S80;
        SolidBrush S70;
        SolidBrush S60;
        SolidBrush S50;
        SolidBrush S40;
        SolidBrush S30;
        SolidBrush S20;
        SolidBrush S10;

        SolidBrush CommonCellBackBrush;
        SolidBrush SelectedCellBackBrush;

        Pen pCheckedRectPen;

        string sBackText = "Нет данных";

        Rectangle rCheckBoxRect;

        bool NeedRefresh = false;

        Font NoDataLabelFont;
        SolidBrush NoDataLabelBrush;

        Bitmap CheckedBMPCommon = new Bitmap(Properties.Resources.checkblack);
        Bitmap CheckedBMPSelected = new Bitmap(Properties.Resources.checkwhite);

        int iPercentLineWidth;

        int MpX = 0;
        int MpY = 0;

        bool bStandardStyle;
        public bool bDrawUpdatesImage = true;
        bool bUseCustomBackColor = false;

        public enum ColorStyle { Green, Blue, Orange };

        public ColorStyle cSelectedColorStyle;

        public ColorStyle SelectedColorStyle
        {
            get { return cSelectedColorStyle; }
            set { cSelectedColorStyle = value; SetStyle(); this.Refresh(); }
        }

        public int PercentLineWidth
        {
            get { return iPercentLineWidth; }
            set
            {
                iPercentLineWidth = value;

                P100.Width = iPercentLineWidth;
                P90.Width = iPercentLineWidth;
                P80.Width = iPercentLineWidth;
                P70.Width = iPercentLineWidth;
                P60.Width = iPercentLineWidth;
                P50.Width = iPercentLineWidth;
                P40.Width = iPercentLineWidth;
                P30.Width = iPercentLineWidth;
                P20.Width = iPercentLineWidth;
                P10.Width = iPercentLineWidth;
            }
        }

        public string sOnlineStatusColumnName = "";

        Bitmap OnlineBMP;




        public ClientsDataGrid()
        {
            ColumnsArray = new ArrayList();

            S100 = new SolidBrush(Color.FromArgb(26, 228, 28));
            S90 = new SolidBrush(Color.FromArgb(169, 242, 14));
            S80 = new SolidBrush(Color.FromArgb(255, 242, 0));
            S70 = new SolidBrush(Color.FromArgb(255, 211, 0));
            S60 = new SolidBrush(Color.FromArgb(255, 173, 0));
            S50 = new SolidBrush(Color.FromArgb(255, 133, 0));
            S40 = new SolidBrush(Color.FromArgb(255, 101, 0));
            S30 = new SolidBrush(Color.FromArgb(255, 80, 0));
            S20 = new SolidBrush(Color.FromArgb(255, 52, 0));
            S10 = new SolidBrush(Color.FromArgb(255, 0, 0));
            CommonCellBackBrush = new SolidBrush(Color.FromArgb(255, 230, 230, 230));
            SelectedCellBackBrush = new SolidBrush(Color.FromArgb(255, 230, 230, 230));

            P100 = new Pen(S100, iPercentLineWidth);
            P90 = new Pen(S90, iPercentLineWidth);
            P80 = new Pen(S80, iPercentLineWidth);
            P70 = new Pen(S70, iPercentLineWidth);
            P60 = new Pen(S60, iPercentLineWidth);
            P50 = new Pen(S50, iPercentLineWidth);
            P40 = new Pen(S40, iPercentLineWidth);
            P30 = new Pen(S30, iPercentLineWidth);
            P20 = new Pen(S20, iPercentLineWidth);
            P10 = new Pen(S10, iPercentLineWidth);

            NoDataLabelFont = new System.Drawing.Font("Segoe UI", 12.0f, FontStyle.Regular);
            NoDataLabelBrush = new SolidBrush(Color.FromArgb(140, 140, 140));

            SetStyle();

            pCheckedRectPen = new Pen(new SolidBrush(Color.Black));

            rCheckBoxRect = new Rectangle(0, 0, 14, 14);

            cSelectedColorStyle = ColorStyle.Orange;

            OnlineBMP = new Bitmap(Properties.Resources.Online);
        }

        public void AddColumns()
        {
            DataGridViewTextBoxColumn OnlineColumn = new DataGridViewTextBoxColumn()
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                Width = 40,
                Name = "OnlineColumn"
            };
            OnlineColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.Columns.Add(OnlineColumn);
        }

        public void OnTimedEvent()
        {
            //if (sNewMessagesColumnName == "")
            //    return;

            //foreach (DataGridViewRow Row in this.Rows)
            //{
            //    if (Row.Cells[sNewMessagesColumnName].Value == DBNull.Value)
            //        continue;

            //    if(bDrawUpdatesImage)
            //        if (Convert.ToInt32(Row.Cells[sNewMessagesColumnName].Value) > 0)
            //            G.DrawImage(NewMessageBMP, Row.Cells[sNewMessagesColumnName].ContentBounds.Left, Row.Cells[sNewMessagesColumnName].ContentBounds.Top);
            //}

            //bDrawUpdatesImage = !bDrawUpdatesImage;
        }

        public bool StandardStyle
        {
            get { return bStandardStyle; }
            set { bStandardStyle = value; SetStyle(); }
        }

        private void SetStyle()
        {
            if (bUseCustomBackColor)
                this.StateCommon.Background.Color1 = Security.GridsBackColor;
            else
                this.StateCommon.Background.Color1 = Color.White;

            if (bUseCustomBackColor)
                this.StateCommon.DataCell.Back.Color1 = Security.GridsBackColor;
            else
                this.StateCommon.DataCell.Back.Color1 = Color.White;

            if (bStandardStyle == false)
                return;

            Font DataCellFont = new Font("SEGOE UI", 15.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            Font HeaderFont = new Font("SEGOE UI", 15.0f, FontStyle.Bold, GraphicsUnit.Pixel);

            this.AllowUserToAddRows = false;
            this.AllowUserToDeleteRows = false;
            this.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.AllowUserToOrderColumns = false;
            this.AllowUserToResizeColumns = false;
            this.AllowUserToResizeRows = false;
            this.ColumnHeadersHeight = 55;
            //this.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.RowTemplate.Height = 40;
            this.RowHeadersVisible = false;



            this.StateCommon.Background.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;



            this.StateCommon.DataCell.Content.Font = DataCellFont;
            this.StateCommon.DataCell.Content.Color1 = Color.Black;
            this.StateCommon.DataCell.Border.Color1 = Color.FromArgb(210, 210, 210);
            this.StateCommon.DataCell.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.StateCommon.HeaderColumn.Content.Font = HeaderFont;
            this.StateCommon.HeaderColumn.Content.Color1 = Color.White;
            this.StateCommon.HeaderColumn.Border.Color1 = Color.Black;
            this.StateCommon.HeaderColumn.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.StateCommon.HeaderColumn.Back.Color1 = Color.FromArgb(150, 150, 150);
            this.StateCommon.HeaderColumn.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;

            this.StateSelected.DataCell.Content.Color1 = Color.White;

            if (cSelectedColorStyle == ClientsDataGrid.ColorStyle.Blue)
                this.StateSelected.DataCell.Back.Color1 = Color.FromArgb(121, 177, 229);
            if (cSelectedColorStyle == ColorStyle.Green)
                this.StateSelected.DataCell.Back.Color1 = Color.FromArgb(31, 158, 0);
            if (cSelectedColorStyle == ColorStyle.Orange)
                this.StateSelected.DataCell.Back.Color1 = Color.FromArgb(253, 164, 61);

            this.StateSelected.DataCell.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;


            CommonCellBackBrush.Color = this.StateCommon.DataCell.Back.Color1;
            SelectedCellBackBrush.Color = this.StateSelected.DataCell.Back.Color1;
        }

        public void AddPercentageColumn(string ColumnName)
        {
            ColumnsArray.Add(ColumnName);
        }

        Pen GetPen(int Value)
        {
            if (Value == 100)
                return P100;
            if (Value >= 90 && Value <= 99)
                return P90;
            if (Value >= 80 && Value <= 89)
                return P80;
            if (Value >= 70 && Value <= 79)
                return P70;
            if (Value >= 60 && Value <= 69)
                return P60;
            if (Value >= 50 && Value <= 59)
                return P50;
            if (Value >= 40 && Value <= 49)
                return P40;
            if (Value >= 30 && Value <= 39)
                return P30;
            if (Value >= 20 && Value <= 29)
                return P20;
            if (Value >= 0 && Value <= 19)
                return P10;

            return null;
        }

        int GetLength(int Value, int CellWidth, int CellHeight)
        {
            return Convert.ToInt32(Convert.ToDecimal(CellWidth - 10) * Convert.ToDecimal(Value) / 100);
        }

        public string BackText
        {
            get { return sBackText; }
            set { sBackText = value; }
        }

        protected override void OnCellPainting(DataGridViewCellPaintingEventArgs e)
        {
            base.OnCellPainting(e);

            if (e.RowIndex == -1)
                return;

            if (e.ColumnIndex == -1)
                return;

            //Checkbox
            if (this.Columns[e.ColumnIndex] is DataGridViewCheckBoxColumn)
            {
                if (e.RowIndex == this.SelectedRows[0].Index)
                {
                    e.Graphics.FillRectangle(SelectedCellBackBrush, e.CellBounds.Left + ((e.CellBounds.Right - e.CellBounds.Left) / 2 - CheckedBMPCommon.Width / 2), e.CellBounds.Top + 3, 32, 32);

                    pCheckedRectPen.Color = Color.White;

                    rCheckBoxRect.Y = e.CellBounds.Top + (e.CellBounds.Height - rCheckBoxRect.Height) / 2;
                    rCheckBoxRect.X = e.CellBounds.Left + (e.CellBounds.Width - rCheckBoxRect.Width) / 2;

                    e.Graphics.DrawRectangle(pCheckedRectPen, rCheckBoxRect);

                    if (e.Value != DBNull.Value)
                        if (Convert.ToBoolean(e.Value) == true)
                            e.Graphics.DrawImage(CheckedBMPSelected, e.CellBounds.Left + ((e.CellBounds.Right - e.CellBounds.Left) / 2 - CheckedBMPCommon.Width / 2) + 1, e.CellBounds.Top + 2, CheckedBMPCommon.Width, CheckedBMPCommon.Height);
                }
                else
                {
                    e.Graphics.FillRectangle(CommonCellBackBrush, e.CellBounds.Left + ((e.CellBounds.Right - e.CellBounds.Left) / 2 - CheckedBMPCommon.Width / 2), e.CellBounds.Top + 3, 32, 32);

                    pCheckedRectPen.Color = Color.FromArgb(121, 121, 121);

                    rCheckBoxRect.Y = e.CellBounds.Top + (e.CellBounds.Height - rCheckBoxRect.Height) / 2;
                    rCheckBoxRect.X = e.CellBounds.Left + (e.CellBounds.Width - rCheckBoxRect.Width) / 2;

                    e.Graphics.DrawRectangle(pCheckedRectPen, rCheckBoxRect);

                    if (e.Value != DBNull.Value)
                        if (Convert.ToBoolean(e.Value) == true)
                            e.Graphics.DrawImage(CheckedBMPCommon, e.CellBounds.Left + ((e.CellBounds.Right - e.CellBounds.Left) / 2 - CheckedBMPCommon.Width / 2) + 1, e.CellBounds.Top + 2, CheckedBMPCommon.Width, CheckedBMPCommon.Height);
                }


            }

            //Updates
            if (bDrawUpdatesImage)
                if (e.ColumnIndex == Columns["OnlineColumn"].Index)
                {
                    if (sOnlineStatusColumnName == "")
                        return;

                    if (Rows[e.RowIndex].Cells[sOnlineStatusColumnName].Value != DBNull.Value)
                    {
                        if (e.Value != DBNull.Value)
                            if (Convert.ToBoolean(Rows[e.RowIndex].Cells[sOnlineStatusColumnName].Value) == true)
                                e.Graphics.DrawImage(OnlineBMP, e.CellBounds.Left + ((e.CellBounds.Right - e.CellBounds.Left) / 2 - (OnlineBMP.Width - 5) / 2), e.CellBounds.Top + ((e.CellBounds.Bottom - e.CellBounds.Top) / 2 - (OnlineBMP.Height - 5) / 2) - 1, OnlineBMP.Width - 5, OnlineBMP.Height - 5);
                    }
                }


            //Percents
            if (ColumnsArray.Count == 0)
                return;

            if (ColumnsArray.Contains(Columns[e.ColumnIndex].Name))
            {
                if (e.Value.ToString().Length > 0 && (Convert.ToInt32(e.Value) >= 0 && Convert.ToInt32(e.Value) <= 100))
                {
                    e.Graphics.DrawLine(GetPen(Convert.ToInt32(e.Value)), e.CellBounds.Left + 4, e.CellBounds.Bottom - 7, (e.CellBounds.Left + 4) +
                        GetLength(Convert.ToInt32(e.Value), e.CellBounds.Width, e.CellBounds.Height), e.CellBounds.Bottom - 7);
                }
            }
        }

        protected override void OnCellStateChanged(DataGridViewCellStateChangedEventArgs e)
        {
            base.OnCellStateChanged(e);

            if (e.Cell.OwningColumn.Name == "OnlineColumn")
            {
                e.Cell.Selected = false;
                this.Rows[e.Cell.RowIndex].Cells["ClientName"].Selected = true;
            }
        }

        protected override void OnCellClick(DataGridViewCellEventArgs e)
        {
            base.OnCellClick(e);

            if (this.Columns[e.ColumnIndex] is DataGridViewCheckBoxColumn)
            {
                if (this.Columns[e.ColumnIndex].ReadOnly == true)
                    return;

                Rectangle CB = this.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);

                if (MpX < CB.X + (CB.Width - rCheckBoxRect.Width) / 2 || MpX > CB.X + (CB.Width - rCheckBoxRect.Width) / 2 + rCheckBoxRect.Width ||
                    MpY < CB.Y + (CB.Height - rCheckBoxRect.Height) / 2 || MpY > CB.Y + (CB.Height - rCheckBoxRect.Height) / 2 + rCheckBoxRect.Height)
                    return;

                if (Convert.ToBoolean(this.Rows[e.RowIndex].Cells[e.ColumnIndex].Value) == true)
                    this.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = false;
                else
                    this.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = true;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            MpX = e.X;
            MpY = e.Y;
        }

        protected override void OnScroll(ScrollEventArgs e)
        {
            base.OnScroll(e);

            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
                NeedRefresh = true;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (NeedRefresh)
            {
                this.Refresh();
                NeedRefresh = false;
            }
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);

            SetStyle();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (this.RowCount == 0)
            {
                e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                e.Graphics.DrawString(sBackText, NoDataLabelFont, NoDataLabelBrush,
                            (this.ClientRectangle.Width - e.Graphics.MeasureString(sBackText, NoDataLabelFont).Width) / 2 + 4,
                            (this.ClientRectangle.Height - e.Graphics.MeasureString(sBackText, NoDataLabelFont).Height) / 2);
            }
        }

        public bool UseCustomBackColor
        {
            get { return bUseCustomBackColor; }
            set { bUseCustomBackColor = value; this.Refresh(); }
        }
    }




    public class CropImageEdit : PictureBox
    {
        SolidBrush ResizeRectBrush;

        Pen WhiteFramePen;
        Pen BlackFramePen;


        SizeF InitialFrameSize;
        SizeF CurrentFrameSize;

        bool bMove = false;
        bool bResize = false;

        struct Position
        {
            public int X;
            public int Y;
        }

        float AspectRatio = 1.07f;

        Position FramePosition;
        Position MousePos;

        string RePosX = "";//T,L,R,B
        string RePosY = "";
        bool ReSiz = false;



        public CropImageEdit()
        {
            WhiteFramePen = new Pen(new SolidBrush(Color.White))
            {
                Width = 2
            };
            BlackFramePen = new Pen(new SolidBrush(Color.Black))
            {
                Width = 2
            };
            ResizeRectBrush = new SolidBrush(Color.FromArgb(31, 158, 0));

            InitialFrameSize.Height = 136;
            InitialFrameSize.Width = 146;

            CurrentFrameSize.Height = 136;
            CurrentFrameSize.Width = 146;

            this.SizeMode = PictureBoxSizeMode.Zoom;
        }





        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            FramePosition.X = Convert.ToInt32(this.Width / 2 - CurrentFrameSize.Width / 2);
            FramePosition.Y = Convert.ToInt32(this.Height / 2 - CurrentFrameSize.Height / 2);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            if (Image == null)
                return;

            DrawHorizFrame(pe.Graphics);
            DrawVertFrame(pe.Graphics);

            pe.Graphics.FillRectangle(ResizeRectBrush, FramePosition.X + CurrentFrameSize.Width - 5, FramePosition.Y + CurrentFrameSize.Height - 5, 9, 9);


        }

        void DrawHorizFrame(Graphics G)
        {
            bool b = true;

            int X = 0;
            int ox = 20;

            while (X < CurrentFrameSize.Width)
            {
                if (CurrentFrameSize.Width - X < 20)
                    ox = (int)CurrentFrameSize.Width - X;

                if (b)
                {
                    G.DrawLine(WhiteFramePen, FramePosition.X + X - 1, FramePosition.Y, FramePosition.X + X + ox + 1, FramePosition.Y);
                    G.DrawLine(WhiteFramePen, FramePosition.X + X - 1, FramePosition.Y + CurrentFrameSize.Height, FramePosition.X + X + ox + 1, FramePosition.Y + CurrentFrameSize.Height);
                }
                else
                {
                    G.DrawLine(BlackFramePen, FramePosition.X + X - 1, FramePosition.Y, FramePosition.X + X + ox + 1, FramePosition.Y); ;
                    G.DrawLine(BlackFramePen, FramePosition.X + X - 1, FramePosition.Y + CurrentFrameSize.Height, FramePosition.X + X + ox + 1, FramePosition.Y + CurrentFrameSize.Height);
                }

                X += 20;

                b = !b;
            }
        }

        void DrawVertFrame(Graphics G)
        {
            bool b = false;

            int Y = 0;
            int oy = 20;

            while (Y < CurrentFrameSize.Height)
            {
                if (CurrentFrameSize.Height - Y < 20)
                    oy = (int)CurrentFrameSize.Height - Y - 1;

                if (b)
                {
                    G.DrawLine(WhiteFramePen, FramePosition.X, FramePosition.Y + Y - 1, FramePosition.X, FramePosition.Y + Y + oy);
                    G.DrawLine(WhiteFramePen, FramePosition.X + CurrentFrameSize.Width, FramePosition.Y + Y - 1, FramePosition.X + CurrentFrameSize.Width, FramePosition.Y + Y + oy);
                }
                else
                {
                    G.DrawLine(BlackFramePen, FramePosition.X, FramePosition.Y + Y - 1, FramePosition.X, FramePosition.Y + Y + oy);
                    G.DrawLine(BlackFramePen, FramePosition.X + CurrentFrameSize.Width, FramePosition.Y + Y - 1, FramePosition.X + CurrentFrameSize.Width, FramePosition.Y + Y + oy);
                }

                Y += 20;

                b = !b;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.X >= FramePosition.X + CurrentFrameSize.Width - 5 && e.X <= FramePosition.X + CurrentFrameSize.Width - 5 + 9 &&
                   e.Y >= FramePosition.Y + CurrentFrameSize.Height - 5 && e.Y <= FramePosition.Y + CurrentFrameSize.Height - 5 + 9)
            {
                this.Cursor = Cursors.SizeNWSE;
            }//NWSE
            else
                if (e.X >= FramePosition.X && e.X <= FramePosition.X + CurrentFrameSize.Width &&
                    e.Y >= FramePosition.Y && e.Y <= FramePosition.Y + CurrentFrameSize.Height)
                this.Cursor = Cursors.SizeAll;//MoveCursor
            else
                this.Cursor = Cursors.Default;



            if (bMove)
            {
                if (!(FramePosition.X <= 2 || FramePosition.X >= (this.Width - CurrentFrameSize.Width) - 2))
                    RePosX = "";

                if (!(FramePosition.Y <= 2 || FramePosition.Y >= (this.Height - CurrentFrameSize.Height) - 2))
                    RePosY = "";

                if (FramePosition.X <= 2)
                    if (FramePosition.X > e.X - MousePos.X)
                        RePosX = "L";

                if (FramePosition.Y <= 2)
                    if (FramePosition.Y > e.Y - MousePos.Y)
                        RePosY = "T";

                if (FramePosition.X >= (this.Width - CurrentFrameSize.Width) - 2)
                    if (FramePosition.X < e.X - MousePos.X)
                        RePosX = "R";

                if (FramePosition.Y >= (this.Height - CurrentFrameSize.Height) - 2)
                    if (FramePosition.Y < e.Y - MousePos.Y)
                        RePosY = "B";

                {
                    FramePosition.X = e.X - MousePos.X;
                    FramePosition.Y = e.Y - MousePos.Y;
                }

                this.Refresh();
            }



            if (bResize)
            {
                int pX = e.X - MousePos.X;
                MousePos.X = e.X;

                int pY;

                pY = e.Y - MousePos.Y;

                MousePos.Y = e.Y;

                if (pX < 0 || pY < 0)
                    if ((CurrentFrameSize.Width <= InitialFrameSize.Width || CurrentFrameSize.Height <= InitialFrameSize.Height))
                    {
                        bResize = false;
                        return;
                    }

                if (pX > 0 || pY > 0)
                    if ((FramePosition.X >= (this.Width - CurrentFrameSize.Width) - 2
                         || FramePosition.Y >= (this.Height - CurrentFrameSize.Height) - 2))
                    {
                        bResize = false;
                        ReSiz = true;
                        return;
                    }

                if (InitialFrameSize.Width > InitialFrameSize.Height)
                {
                    CurrentFrameSize.Width = CurrentFrameSize.Width + pY * AspectRatio;
                    CurrentFrameSize.Height = CurrentFrameSize.Height + pY;
                }
                else
                {
                    CurrentFrameSize.Width = CurrentFrameSize.Width + pX;
                    CurrentFrameSize.Height = CurrentFrameSize.Height + pX * AspectRatio;
                }

                this.Refresh();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (this.Cursor == Cursors.SizeAll)
            {
                bMove = true;
                MousePos.X = e.X - FramePosition.X;
                MousePos.Y = e.Y - FramePosition.Y;
                return;
            }

            if (this.Cursor == Cursors.SizeNESW || this.Cursor == Cursors.SizeNWSE)
            {
                bResize = true;
                MousePos.X = e.X;
                MousePos.Y = e.Y;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            bMove = false;
            bResize = false;

            if (RePosX == "L")
                FramePosition.X = 2;

            if (RePosX == "R")
                FramePosition.X = (int)((this.Width - CurrentFrameSize.Width) - 2);

            if (RePosY == "T")
                FramePosition.Y = 2;

            if (RePosY == "B")
                FramePosition.Y = (int)((this.Height - CurrentFrameSize.Height) - 2);

            RePosX = "";
            RePosY = "";

            if (ReSiz)
            {
                int q = 0;

                if ((int)(FramePosition.X + CurrentFrameSize.Width - this.Width) > 0)
                    q = (int)(FramePosition.X + CurrentFrameSize.Width - this.Width) + 3;

                if ((int)(FramePosition.Y + CurrentFrameSize.Height - this.Height) > 0)
                    q = (int)(FramePosition.Y + CurrentFrameSize.Height - this.Height) + 3;

                if (InitialFrameSize.Width > InitialFrameSize.Height)
                {
                    CurrentFrameSize.Width = CurrentFrameSize.Width - q * AspectRatio;
                    CurrentFrameSize.Height = CurrentFrameSize.Height - q;
                }
                else
                {
                    CurrentFrameSize.Width = CurrentFrameSize.Width - q;
                    CurrentFrameSize.Height = CurrentFrameSize.Height - q * AspectRatio;
                }

                ReSiz = false;
            }

            this.Refresh();

            OnFrameMoved(this, e);
        }

        public Bitmap CropImage()
        {
            using (Bitmap B = new Bitmap(this.Width, this.Height))
            {
                this.DrawToBitmap(B, new Rectangle(0, 0, this.Width, this.Height));
                System.Drawing.Imaging.PixelFormat format = PixelFormat.DontCare;

                Bitmap C = new Bitmap((int)InitialFrameSize.Width, (int)InitialFrameSize.Height);

                using (Graphics gr = Graphics.FromImage(C))
                {
                    gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    gr.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                    gr.DrawImage(B.Clone(new Rectangle((int)FramePosition.X + 2, (int)FramePosition.Y + 2, (int)CurrentFrameSize.Width - 7,
                                (int)CurrentFrameSize.Height - 7), format), new Rectangle(0, 0, (int)InitialFrameSize.Width, (int)InitialFrameSize.Height));
                }

                GC.Collect();

                return C;
            }
        }

        public void Clear()
        {
            Graphics g = this.CreateGraphics();
            g.Clear(this.BackColor);
        }

        public event EventHandler FrameMoved;

        protected void OnFrameMoved(object sender, EventArgs e)
        {
            if (this.FrameMoved != null)
                this.FrameMoved(this, e);
        }
    }





    public class PhotoBox : PictureBox
    {
        Font NoImageFont;

        SolidBrush NoImageBrush;

        Pen BorderCommonPen;
        Pen BorderTrackingPen;

        bool bDrawBorder = false;

        bool bTracking = false;

        Color cBorderColorCommon = Color.Black;
        Color cBorderColorTracking = Color.FromArgb(253, 164, 61);

        public PhotoBox()
        {
            NoImageFont = new Font("Segoe UI", 12.5f, FontStyle.Regular, GraphicsUnit.Pixel);
            NoImageBrush = new SolidBrush(Color.Gray);

            BorderCommonPen = new Pen(new SolidBrush(cBorderColorCommon));
            BorderTrackingPen = new Pen(new SolidBrush(cBorderColorTracking));
        }

        public bool DrawBorder
        {
            get { return bDrawBorder; }
            set
            {
                bDrawBorder = value;

                if (value == false)
                    this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                else
                    this.BorderStyle = System.Windows.Forms.BorderStyle.None;

                this.Refresh();
            }
        }

        public Color BorderColorCommon
        {
            get { return cBorderColorCommon; }
            set { cBorderColorCommon = value; BorderCommonPen.Color = cBorderColorCommon; this.Refresh(); }
        }

        public Color BorderColorTracking
        {
            get { return cBorderColorTracking; }
            set { cBorderColorTracking = value; BorderTrackingPen.Color = cBorderColorTracking; this.Refresh(); }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            if (Image == null)
            {
                pe.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                pe.Graphics.DrawString("Нет фото", NoImageFont, NoImageBrush, (this.Width - 66) / 2 + 3, (this.Height - 15) / 2 - 2);
            }

            if (bDrawBorder)
            {
                if (!bTracking)
                    pe.Graphics.DrawRectangle(BorderCommonPen, new Rectangle(0, 0, this.Width - 1, this.Height - 1));
                else
                    pe.Graphics.DrawRectangle(BorderTrackingPen, new Rectangle(0, 0, this.Width - 1, this.Height - 1));
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!bTracking)
            {
                bTracking = true;
                this.Refresh();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (bTracking)
            {
                bTracking = false;
                this.Refresh();
            }
        }
    }





    public class MenuLabel : ComponentFactory.Krypton.Toolkit.KryptonCheckButton
    {
        Color cCommonLineColor = Color.FromArgb(253, 164, 61);
        Color cTrackingLineColor = Color.FromArgb(255, 217, 112);

        int iLineWidth = 3;

        Pen pLineCommonPen;
        Pen pLineTrackingPen;

        bool bTracking = false;

        bool bToWidth = false;

        public MenuLabel()
        {
            this.StateCommon.Back.Color1 = Color.Transparent;
            this.StateCommon.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.StateCommon.Border.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.False;

            this.Cursor = Cursors.Hand;

            pLineCommonPen = new Pen(new SolidBrush(cCommonLineColor), iLineWidth);
            pLineTrackingPen = new Pen(new SolidBrush(cTrackingLineColor), iLineWidth);
        }

        public Color CommonLineColor
        {
            get { return cCommonLineColor; }
            set { cCommonLineColor = value; pLineCommonPen.Color = cCommonLineColor; this.Refresh(); }
        }

        public Color TrackingLineColor
        {
            get { return cTrackingLineColor; }
            set { cTrackingLineColor = value; pLineTrackingPen.Color = cTrackingLineColor; this.Refresh(); }
        }

        public int LineWidth
        {
            get { return iLineWidth; }
            set { iLineWidth = value; pLineCommonPen.Width = iLineWidth; pLineTrackingPen.Width = iLineWidth; this.Refresh(); }
        }

        public bool ToWidth
        {
            get { return bToWidth; }
            set { bToWidth = value; this.Refresh(); }
        }



        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (this.Checked)
            {
                int TextHeight = Convert.ToInt32(e.Graphics.MeasureString(this.Text, this.StateCommon.Content.ShortText.Font).Height);
                int TextWidth = Convert.ToInt32(e.Graphics.MeasureString(this.Text, this.StateCommon.Content.ShortText.Font).Width);

                if (!bToWidth)
                    e.Graphics.DrawLine(pLineCommonPen, ((this.Width - TextWidth) / 2 + 4), ((this.Height - TextHeight) / 2 + TextHeight) + 2,
                                                        ((this.Width - TextWidth) / 2 + 4) + TextWidth - 8, ((this.Height - TextHeight) / 2 + TextHeight) + 2);
                else
                    e.Graphics.DrawLine(pLineCommonPen, 10, ((this.Height - TextHeight) / 2 + TextHeight) + 2, this.Width - 10, ((this.Height - TextHeight) / 2 + TextHeight) + 2);

                return;
            }

            if (bTracking)
            {
                int TextHeight = Convert.ToInt32(e.Graphics.MeasureString(this.Text, this.StateCommon.Content.ShortText.Font).Height);
                int TextWidth = Convert.ToInt32(e.Graphics.MeasureString(this.Text, this.StateCommon.Content.ShortText.Font).Width);

                if (!bToWidth)
                    e.Graphics.DrawLine(pLineTrackingPen, ((this.Width - TextWidth) / 2 + 4), ((this.Height - TextHeight) / 2 + TextHeight) + 2,
                                                          ((this.Width - TextWidth) / 2 + 4) + TextWidth - 8, ((this.Height - TextHeight) / 2 + TextHeight) + 2);
                else
                    e.Graphics.DrawLine(pLineTrackingPen, 10, ((this.Height - TextHeight) / 2 + TextHeight) + 2, this.Width - 10, ((this.Height - TextHeight) / 2 + TextHeight) + 2);

                return;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!bTracking)
                bTracking = true;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            bTracking = false;
        }
    }





    public class LightUsersList : Control
    {
        int iOffset = 0;

        int iItemHeight = 166;
        int iMarginForImageWidth = 160;

        bool bShortView = false;
        int iTracking = -1;

        int ShortItemHeight = 45;

        DataTable ItemsPositions;

        Color cUserNameColor = Color.Black;
        Color cPositionLabelColor = Color.Black;
        Color cPositionItemColor = Color.Gray;
        Color cInfoLabelColor = Color.Black;
        Color cInfoItemColor = Color.Black;
        Color cUserLineColor = Color.Black;
        Color cInOfficeColor = Color.LimeGreen;
        Color cItemLineColor = Color.Gray;
        Color cVerticalScrollCommonShaftBackColor = Color.White;
        Color cVerticalScrollCommonThumbButtonColor = Color.Gray;

        Font fUserNameFont;
        Font fPositionLabelFont;
        Font fPositionItemFont;
        Font fInfoLabelFont;
        Font fInfoItemFont;
        Font fInOfficeFont;

        SolidBrush brUserNameBrush;
        SolidBrush brPositionLabelBrush;
        SolidBrush brPositionItemBrush;
        SolidBrush brInfoLabelBrush;
        SolidBrush brInfoItemBrush;
        SolidBrush brInOfficeBrush;
        SolidBrush brVerticalScrollCommonShaftBackBrush;
        SolidBrush brVerticalScrollCommonThumbButtonBrush;

        Pen pImageRectPen;
        Pen pUserLinePen;
        Pen pItemLinePen;

        Rectangle VerticalScrollShaftRect;

        Rectangle rImageRect;

        DataTable dUsersDataTable;

        Bitmap NoImageBitmap;

        public LightUsersList()
        {
            fUserNameFont = new Font("Segoe UI", 22.5f, FontStyle.Regular, GraphicsUnit.Pixel);
            fPositionLabelFont = new Font("Segoe UI Semilight", 16.25f, FontStyle.Regular, GraphicsUnit.Pixel);
            fPositionItemFont = new Font("Segoe UI Semilight", 15.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            fInfoLabelFont = new Font("Segoe UI Semilight", 16.25f, FontStyle.Regular, GraphicsUnit.Pixel);
            fInfoItemFont = new Font("Segoe UI Semilight", 15.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            fInOfficeFont = new Font("Segoe UI Semilight", 15.0f, FontStyle.Bold, GraphicsUnit.Pixel);

            brUserNameBrush = new SolidBrush(cUserNameColor);
            brPositionLabelBrush = new SolidBrush(cPositionLabelColor);
            brPositionItemBrush = new SolidBrush(cPositionItemColor);
            brInfoLabelBrush = new SolidBrush(cInfoLabelColor);
            brInfoItemBrush = new SolidBrush(cInfoItemColor);
            brInOfficeBrush = new SolidBrush(cInOfficeColor);
            brVerticalScrollCommonShaftBackBrush = new SolidBrush(cVerticalScrollCommonShaftBackColor);
            brVerticalScrollCommonThumbButtonBrush = new SolidBrush(cVerticalScrollCommonThumbButtonColor);

            pImageRectPen = new Pen(new SolidBrush(Color.Black));
            pUserLinePen = new Pen(new SolidBrush(cUserLineColor));
            pItemLinePen = new Pen(new SolidBrush(cItemLineColor));


            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);


            NoImageBitmap = new Bitmap(146, 136);
            Graphics Gr = Graphics.FromImage(NoImageBitmap);
            Font F = new Font("Segoe UI", 10.0f, FontStyle.Regular);
            Gr.TextRenderingHint = TextRenderingHint.AntiAlias;
            Gr.DrawString("Нет фото", F, new SolidBrush(Color.Gray), (146 - Gr.MeasureString("Нет фото", F).Width) / 2, (136 - Gr.MeasureString("Нет фото", F).Height) / 2 - 2);
            Gr.Dispose();
            GC.Collect();
            F.Dispose();

            VerticalScrollShaftRect = new Rectangle(this.Width - 10, 0, 10, this.Height);
            rImageRect = new Rectangle(3, 0, 147, 137);

            ItemsPositions = new DataTable();

            ItemsPositions.Columns.Add(new DataColumn("ID", Type.GetType("System.Int32")));
            ItemsPositions.Columns.Add(new DataColumn("Name", Type.GetType("System.String")));
            ItemsPositions.Columns.Add(new DataColumn("X1", Type.GetType("System.Int32")));
            ItemsPositions.Columns.Add(new DataColumn("X2", Type.GetType("System.Int32")));
            ItemsPositions.Columns.Add(new DataColumn("Y1", Type.GetType("System.Int32")));
            ItemsPositions.Columns.Add(new DataColumn("Y2", Type.GetType("System.Int32")));
        }


        public Color UserNameColor
        {
            get { return cUserNameColor; }
            set { cUserNameColor = value; brUserNameBrush.Color = cUserNameColor; this.Refresh(); }
        }

        public Color PositionLabelColor
        {
            get { return cPositionLabelColor; }
            set { cPositionLabelColor = value; brPositionLabelBrush.Color = cPositionLabelColor; this.Refresh(); }
        }

        public Color PositionItemColor
        {
            get { return cPositionItemColor; }
            set { cPositionItemColor = value; brPositionItemBrush.Color = cPositionItemColor; this.Refresh(); }
        }

        public Color InfoLabelColor
        {
            get { return cInfoLabelColor; }
            set { cInfoLabelColor = value; brInfoLabelBrush.Color = cInfoLabelColor; this.Refresh(); }
        }

        public Color InfoItemColor
        {
            get { return cInfoItemColor; }
            set { cInfoItemColor = value; brInfoItemBrush.Color = cInfoItemColor; this.Refresh(); }
        }

        public Color UserLineColor
        {
            get { return cUserLineColor; }
            set { cUserLineColor = value; pUserLinePen.Color = cUserLineColor; this.Refresh(); }
        }

        public Color InOfficeColor
        {
            get { return cInOfficeColor; }
            set { cInOfficeColor = value; brInOfficeBrush.Color = cInOfficeColor; this.Refresh(); }
        }

        public Color ItemLineColor
        {
            get { return cItemLineColor; }
            set { cItemLineColor = value; pItemLinePen.Color = cItemLineColor; this.Refresh(); }
        }

        public Color VerticalScrollCommonShaftBackColor
        {
            get { return cVerticalScrollCommonShaftBackColor; }
            set { cVerticalScrollCommonShaftBackColor = value; brVerticalScrollCommonShaftBackBrush.Color = cVerticalScrollCommonShaftBackColor; this.Refresh(); }
        }

        public Color VerticalScrollCommonThumbButtonColor
        {
            get { return cVerticalScrollCommonThumbButtonColor; }
            set { cVerticalScrollCommonThumbButtonColor = value; brVerticalScrollCommonThumbButtonBrush.Color = cVerticalScrollCommonThumbButtonColor; this.Refresh(); }
        }



        public Font UserNameFont
        {
            get { return fUserNameFont; }
            set { fUserNameFont = value; this.Refresh(); }
        }

        public Font PositionLabelFont
        {
            get { return fPositionLabelFont; }
            set { fPositionLabelFont = value; this.Refresh(); }
        }

        public Font PositionItemFont
        {
            get { return fPositionItemFont; }
            set { fPositionItemFont = value; this.Refresh(); }
        }

        public Font InfoLabelFont
        {
            get { return fInfoLabelFont; }
            set { fInfoLabelFont = value; this.Refresh(); }
        }

        public Font InfoItemFont
        {
            get { return fInfoItemFont; }
            set { fInfoItemFont = value; this.Refresh(); }
        }

        public Font InOfficeFont
        {
            get { return fInOfficeFont; }
            set { fInOfficeFont = value; this.Refresh(); }
        }


        public DataTable UsersDataTable
        {
            get { return dUsersDataTable; }
            set { dUsersDataTable = value; this.Refresh(); }
        }



        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (dUsersDataTable == null)
                return;

            if (dUsersDataTable.Rows.Count == 0)
                return;

            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            if (dUsersDataTable.DefaultView.Count == 0)
                return;

            if (ItemsPositions.Rows.Count == 0)
                FillTable();

            if (!bShortView)
            {
                ShortItemHeight = iItemHeight;

                for (int i = iOffset; i < dUsersDataTable.DefaultView.Count; i++)
                {
                    if (this.Height <= (i) * ShortItemHeight - iOffset * ShortItemHeight)
                        break;

                    string sName = dUsersDataTable.DefaultView[i]["Name"].ToString();
                    string sProfilPosition = dUsersDataTable.DefaultView[i]["ProfilPosition"].ToString();
                    string sTPSPosition = dUsersDataTable.DefaultView[i]["TPSPosition"].ToString();

                    if (e.Graphics.MeasureString(sProfilPosition, fPositionItemFont).Width > (550 + iMarginForImageWidth + 3 - (iMarginForImageWidth + 3)))
                        sProfilPosition = OneLineToTwo(sProfilPosition, fPositionItemFont, (550 + iMarginForImageWidth + 3 - (iMarginForImageWidth + 3)));
                    if (e.Graphics.MeasureString(sTPSPosition, fPositionItemFont).Width > (550 + iMarginForImageWidth + 3 - (iMarginForImageWidth + 3)))
                        sTPSPosition = OneLineToTwo(sTPSPosition, fPositionItemFont, (550 + iMarginForImageWidth + 3 - (iMarginForImageWidth + 3)));

                    string sDepartment = dUsersDataTable.DefaultView[i]["Department"].ToString();
                    string sPersonalMobilePhone = UsersDataTable.DefaultView[i]["PersonalMobilePhone"].ToString();
                    string sWorkMobilePhone = UsersDataTable.DefaultView[i]["WorkMobilePhone"].ToString();
                    string sWorkExtPhone = UsersDataTable.DefaultView[i]["WorkExtPhone"].ToString();
                    string sICQ = UsersDataTable.DefaultView[i]["ICQ"].ToString();
                    string sOnline = "online";

                    int iNameWidth = Convert.ToInt32(e.Graphics.MeasureString(sName, fUserNameFont).Width);
                    int iNameHeight = Convert.ToInt32(e.Graphics.MeasureString(sName, fUserNameFont).Height);

                    if (iTracking != i)
                        e.Graphics.DrawString(sName, fUserNameFont, brUserNameBrush,
                                            iMarginForImageWidth, i * (iItemHeight) - 5 - iOffset * iItemHeight);
                    else
                        e.Graphics.DrawString(sName, fUserNameFont, brInOfficeBrush,
                                            iMarginForImageWidth, i * (iItemHeight) - 5 - iOffset * iItemHeight);

                    e.Graphics.DrawLine(pUserLinePen, iMarginForImageWidth + 3, i * (iItemHeight) + 28 - iOffset * iItemHeight,
                                        550 + iMarginForImageWidth + 3, i * (iItemHeight) + 28 - iOffset * iItemHeight);

                    DrawImage(i, dUsersDataTable.DefaultView[i], e.Graphics);

                    float y = 2f;
                    if (sProfilPosition.Length > 0 && sTPSPosition.Length > 0)
                    {
                        e.Graphics.DrawString(sProfilPosition, fPositionItemFont, brPositionLabelBrush, iMarginForImageWidth, i * (iItemHeight) + 35 + y - iOffset * iItemHeight);
                        //e.Graphics.DrawString("(" + sProfilRate + " ЗОВ-Профиль)", fPositionLabelFont, brPositionItemBrush, 10 + e.Graphics.MeasureString(sProfilPosition, fPositionItemFont).Width + iMarginForImageWidth, i * (iItemHeight) + 35 - iOffset * iItemHeight);
                        e.Graphics.DrawString(sTPSPosition, fPositionItemFont, brPositionLabelBrush, iMarginForImageWidth, i * (iItemHeight) + 20 + 35 + y - iOffset * iItemHeight);
                        //e.Graphics.DrawString("(" + sTPSRate + " ЗОВ-ТПС)", fPositionLabelFont, brPositionItemBrush, 10 + e.Graphics.MeasureString(sTPSPosition, fPositionItemFont).Width + iMarginForImageWidth, i * (iItemHeight) + 55 - iOffset * iItemHeight);
                    }
                    else
                    {
                        if (sProfilPosition.Length > 0)
                        {
                            e.Graphics.DrawString(sProfilPosition, fPositionItemFont, brPositionLabelBrush, iMarginForImageWidth, i * (iItemHeight) + 35 + y - iOffset * iItemHeight);
                            //e.Graphics.DrawString("(" + sProfilRate + " ЗОВ-Профиль)", fPositionLabelFont, brPositionItemBrush, 10 + e.Graphics.MeasureString(sProfilPosition, fPositionItemFont).Width + iMarginForImageWidth, i * (iItemHeight) + 35 - iOffset * iItemHeight);
                        }
                        if (sTPSPosition.Length > 0)
                        {
                            e.Graphics.DrawString(sTPSPosition, fPositionItemFont, brPositionLabelBrush, iMarginForImageWidth, i * (iItemHeight) + 20 + 35 + y - iOffset * iItemHeight);
                            //e.Graphics.DrawString("(" + sTPSRate + " ЗОВ-ТПС)", fPositionLabelFont, brPositionItemBrush, 10 + e.Graphics.MeasureString(sTPSPosition, fPositionItemFont).Width + iMarginForImageWidth, i * (iItemHeight) + 55 - iOffset * iItemHeight);
                        }
                    }

                    e.Graphics.DrawString("Служба\\отдел:", fPositionLabelFont, brPositionLabelBrush,
                                          iMarginForImageWidth, i * (iItemHeight) + 118 - iOffset * iItemHeight);

                    e.Graphics.DrawString(sDepartment, fPositionItemFont, brPositionItemBrush,
                                          iMarginForImageWidth + 110, i * (iItemHeight) + 118 + y - iOffset * iItemHeight);

                    e.Graphics.DrawString("Личный телефон", fInfoLabelFont, brInfoLabelBrush,
                                          this.Width - 349, i * (iItemHeight) + 35 - iOffset * iItemHeight);

                    e.Graphics.DrawString("Рабочий телефон", fInfoLabelFont, brInfoLabelBrush,
                                          this.Width - 349, i * (iItemHeight) + 85 - iOffset * iItemHeight);

                    e.Graphics.DrawString("Внутренний телефон", fInfoLabelFont, brInfoLabelBrush,
                                          this.Width - 349, i * (iItemHeight) + 60 - iOffset * iItemHeight);

                    e.Graphics.DrawString("ICQ", fInfoLabelFont, brInfoLabelBrush,
                                          this.Width - 349, i * (iItemHeight) + 110 - iOffset * iItemHeight);

                    e.Graphics.DrawString(sPersonalMobilePhone, fInfoItemFont, brInfoItemBrush,
                                          this.Width - 349 + 180, i * (iItemHeight) + 36 - iOffset * iItemHeight);

                    e.Graphics.DrawString(sWorkMobilePhone, fInfoItemFont, brInfoItemBrush,
                                          this.Width - 349 + 180, i * (iItemHeight) + 86 - iOffset * iItemHeight);

                    e.Graphics.DrawString(sWorkExtPhone, fInfoItemFont, brInfoItemBrush,
                                          this.Width - 349 + 180, i * (iItemHeight) + 61 - iOffset * iItemHeight);

                    e.Graphics.DrawString(sICQ, fInfoItemFont, brInfoItemBrush,
                                          this.Width - 349 + 180, i * (iItemHeight) + 111 - iOffset * iItemHeight);

                    if (i > 0)
                        e.Graphics.DrawLine(pItemLinePen, 3, i * (iItemHeight) - 15 - iOffset * iItemHeight,
                                            this.Width - 10, i * (iItemHeight) - 15 - iOffset * iItemHeight);

                    if (Convert.ToBoolean(UsersDataTable.DefaultView[i]["Online"]))
                        e.Graphics.DrawString(sOnline, fInOfficeFont, brInOfficeBrush,
                                                iNameWidth + 5 + iMarginForImageWidth, i * (ShortItemHeight) - 5 - iOffset * ShortItemHeight + 2);
                }
            }
            else
            {
                ShortItemHeight = 45;

                for (int i = 0; i < dUsersDataTable.DefaultView.Count; i++)
                {
                    string sName = dUsersDataTable.DefaultView[i]["Name"].ToString();
                    string sOnline = "online";

                    int iNameWidth = Convert.ToInt32(e.Graphics.MeasureString(sName, fUserNameFont).Width);

                    if (iTracking != i)
                        e.Graphics.DrawString(sName, fUserNameFont, brUserNameBrush,
                                          10, i * (ShortItemHeight) - 5 - iOffset * ShortItemHeight);
                    else
                        e.Graphics.DrawString(sName, fUserNameFont, brInOfficeBrush,
                                          10, i * (ShortItemHeight) - 5 - iOffset * ShortItemHeight);

                    e.Graphics.DrawLine(pUserLinePen, 10, i * (ShortItemHeight) + 28 - iOffset * ShortItemHeight,
                                        this.Width - VerticalScrollShaftRect.Width, i * (ShortItemHeight) + 28 - iOffset * ShortItemHeight);

                    if (Convert.ToBoolean(UsersDataTable.DefaultView[i]["Online"]))
                        e.Graphics.DrawString(sOnline, fInOfficeFont, brInOfficeBrush,
                                                iNameWidth + 10, i * (ShortItemHeight) - 5 - iOffset * ShortItemHeight + 2);
                }
            }


            DrawVerticalScrollShaft(e.Graphics);
            DrawVertScrollThumb(e.Graphics);
        }

        private string OneLineToTwo(string text, Font Font, int MaxWidth)
        {
            if (text.Length == 0)
                return "";

            Graphics G = this.CreateGraphics();

            if (G.MeasureString(text, Font).Width > MaxWidth)
            {
                int LastSpace = GetLastSpace(text);

                text = text.Insert(LastSpace + 1, "\n");
            }

            G.Dispose();

            return text;
        }

        private int GetLastSpace(string Text)
        {
            int LastSpace = 0;

            for (int i = Text.Length - 1; i >= 0; i--)
            {
                if (Text[i] == ' ')
                {
                    LastSpace = i;
                    break;
                }
            }

            if (LastSpace == 0)//no spaces was found
            {
                for (int i = Text.Length - 1; i >= 0; i--)
                {
                    if (Text[i] == '.' || Text[i] == ',' || Text[i] == ';' || Text[i] == ':')
                    {
                        LastSpace = i;
                        break;
                    }
                }
            }

            return LastSpace;
        }

        private void DrawImage(int index, DataRowView Row, Graphics G)
        {
            if (Row["Photo"] != DBNull.Value)
            {
                byte[] b = (byte[])Row["Photo"];
                MemoryStream ms = new MemoryStream(b);
                Bitmap Bmp = new Bitmap(ms);
                Bmp.SetResolution(G.DpiX, G.DpiY);
                G.DrawImage(Bmp, 4, index * (iItemHeight) - iOffset * iItemHeight + 1);

                ms.Dispose();
                Bmp.Dispose();
            }
            else
            {
                G.DrawImage(NoImageBitmap, 4, index * (iItemHeight) - iOffset * iItemHeight + 1);
            }

            rImageRect.Y = index * (iItemHeight) - 1 - iOffset * iItemHeight + 1;

            G.DrawRectangle(pImageRectPen, rImageRect);
        }

        private void DrawVerticalScrollShaft(Graphics G)
        {
            if (!bShortView)
            {
                if (this.Height >= dUsersDataTable.DefaultView.Count * ShortItemHeight)
                    return;
            }

            //Shaft
            G.FillRectangle(brVerticalScrollCommonShaftBackBrush, VerticalScrollShaftRect);
        }

        private void DrawVertScrollThumb(Graphics G)
        {
            if (this.Height >= dUsersDataTable.DefaultView.Count * ShortItemHeight)
            {
                return;
            }

            decimal V = this.Height;
            decimal T = Convert.ToDecimal(dUsersDataTable.DefaultView.Count * ShortItemHeight);

            decimal Rtv = V / (T / 100);

            decimal Th = Convert.ToInt32(Decimal.Round(Rtv * (V / 100), 0, MidpointRounding.AwayFromZero));

            if (Th >= V)
                return;

            decimal Ws = (V - Th) / ((T - V) / ShortItemHeight);

            int posY = iOffset * Convert.ToInt32(Ws);

            G.FillRectangle(brVerticalScrollCommonThumbButtonBrush, new Rectangle(VerticalScrollShaftRect.X + 2, posY,
                                                                                  10 - 4, Convert.ToInt32(Th)));
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            VerticalScrollShaftRect.X = this.Width - 10;
            VerticalScrollShaftRect.Height = this.Height;
            VerticalScrollShaftRect.Width = 10;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            VerticalScrollShaftRect.X = this.Width - 10;
            VerticalScrollShaftRect.Height = this.Height;
            VerticalScrollShaftRect.Width = 10;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!this.Focused)
                this.Focus();

            TrackItem(e.X, e.Y);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            iTracking = -1;
            Cursor = Cursors.Default;
            this.Refresh();
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            if (iTracking > -1)
                OnUserClick();
        }

        private void TrackItem(int X, int Y)
        {
            int iItemH = 0;

            if (bShortView)
                iItemH = ShortItemHeight;
            else
                iItemH = iItemHeight;

            DataRow[] Row = ItemsPositions.Select("X1 <= " + X + " AND X2 >= " + X + " AND Y1 <= " + (Y + iOffset * iItemH) + "AND Y2 >= " + (Y + iOffset * iItemH));

            if (Row.Count() == 0)
            {
                if (iTracking > -1)
                {
                    iTracking = -1;
                    this.Refresh();
                    Cursor = Cursors.Default;
                }

                return;
            }

            if (iTracking == Convert.ToInt32(Row[0]["ID"]))
                return;

            iTracking = Convert.ToInt32(Row[0]["ID"]);
            Cursor = Cursors.Hand;
            this.Refresh();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (e.Delta < 0)
            {
                if (iOffset < (dUsersDataTable.DefaultView.Count - (this.Height / ShortItemHeight)))
                {
                    iOffset++;
                    this.Refresh();
                }
            }

            if (e.Delta > 0)
                if (iOffset > 0)
                {
                    iOffset--;
                    this.Refresh();
                }
        }


        public bool ShortView
        {
            get { return bShortView; }
            set { bShortView = value; iOffset = 0; FillTable(); this.Refresh(); }
        }

        public void Filter(string Filter)
        {
            if (dUsersDataTable == null)
                return;

            dUsersDataTable.DefaultView.RowFilter = Filter;

            iOffset = 0;

            FillTable();

            this.Refresh();
        }

        private void FillTable()
        {
            if (dUsersDataTable == null)
                return;

            Graphics G = this.CreateGraphics();

            ItemsPositions.Clear();

            if (!bShortView)
                for (int i = 0; i < dUsersDataTable.DefaultView.Count; i++)
                {
                    string sName = dUsersDataTable.DefaultView[i]["Name"].ToString();

                    int iNameWidth = Convert.ToInt32(G.MeasureString(sName, fUserNameFont).Width);
                    int iNameHeight = Convert.ToInt32(G.MeasureString(sName, fUserNameFont).Height);

                    DataRow Row = ItemsPositions.NewRow();
                    Row["ID"] = i;
                    Row["Name"] = sName;
                    Row["X1"] = iMarginForImageWidth;
                    Row["X2"] = iNameWidth + iMarginForImageWidth;
                    Row["Y1"] = i * (iItemHeight) - 5 - iOffset * iItemHeight;
                    Row["Y2"] = iNameHeight + Convert.ToInt32(Row["Y1"]);
                    ItemsPositions.Rows.Add(Row);
                }
            else
                for (int i = 0; i < dUsersDataTable.DefaultView.Count; i++)
                {
                    string sName = dUsersDataTable.DefaultView[i]["Name"].ToString();

                    int iNameWidth = Convert.ToInt32(G.MeasureString(sName, fUserNameFont).Width);
                    int iNameHeight = Convert.ToInt32(G.MeasureString(sName, fUserNameFont).Height);

                    int iShortItemHeight = Convert.ToInt32(G.MeasureString("Name", fUserNameFont).Height) + 10;

                    DataRow Row = ItemsPositions.NewRow();
                    Row["ID"] = i;
                    Row["Name"] = sName;
                    Row["X1"] = 10;
                    Row["X2"] = iNameWidth + 10;
                    Row["Y1"] = i * (iShortItemHeight) - 5 - iOffset * iShortItemHeight;
                    Row["Y2"] = iShortItemHeight + Convert.ToInt32(Row["Y1"]);
                    ItemsPositions.Rows.Add(Row);
                }

            G.Dispose();
        }


        public event UserClickEventHandler UserClick;

        public delegate void UserClickEventHandler(object sender, string Name);

        public virtual void OnUserClick()
        {
            if (UserClick != null)
            {
                UserClick(this, ItemsPositions.Rows[iTracking]["Name"].ToString());//Raise the event
            }
        }

    }





    public class LightBackButton : Panel
    {
        Bitmap bForwardImageBlack;
        Bitmap bForwardImageWhite;
        Bitmap bForwardImageBlue;
        Bitmap bBackWardImageWhite;
        Bitmap bBackWardImageBlue;
        Bitmap bBackWardImageBlack;

        Pen Pen;
        Rectangle Rect;
        SolidBrush Brush;

        bool bTracking = false;

        public enum eDirectionAndColorTracking { WhiteForward, BlueForward, BlackForward, WhiteBackward, BlueBackward, BlackBackward };
        public enum eDirectionAndColorCommon { WhiteForward, BlueForward, BlackForward, WhiteBackward, BlueBackward, BlackBackward };

        eDirectionAndColorTracking eDCT;
        eDirectionAndColorCommon eDCC;

        public LightBackButton()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);

            bForwardImageBlack = new Bitmap(Properties.Resources.ArrowForwardBlack);
            bForwardImageWhite = new Bitmap(Properties.Resources.ArrowForwardTrackingWhite);
            bForwardImageBlue = new Bitmap(Properties.Resources.ArrowForwardTrackingBlue);
            bBackWardImageWhite = new Bitmap(Properties.Resources.ArrowBackTrackingWhite);
            bBackWardImageBlue = new Bitmap(Properties.Resources.ArrowBackTrackingBlue);
            bBackWardImageBlack = new Bitmap(Properties.Resources.ArrowBackBlack);

            Rect = new Rectangle(2, 2, this.Width - 4, this.Height - 4);

            Brush = new SolidBrush(Color.Black);
            Pen = new Pen(Brush, 2.4f);
        }

        public eDirectionAndColorTracking DirectionAndColorTracking
        {
            get { return eDCT; }
            set
            {
                eDCT = value; this.Refresh();
            }
        }

        public eDirectionAndColorCommon DirectionAndColorCommon
        {
            get { return eDCC; }
            set { eDCC = value; this.Refresh(); }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            if (!bTracking)
            {
                if (eDCC == eDirectionAndColorCommon.BlackBackward)
                {
                    Pen.Color = Color.Black;
                    e.Graphics.DrawImage(bBackWardImageBlack, 0, 0);
                }
                if (eDCC == eDirectionAndColorCommon.BlackForward)
                {
                    Pen.Color = Color.Black;
                    e.Graphics.DrawImage(bForwardImageBlack, 0, 0);
                }
                if (eDCC == eDirectionAndColorCommon.BlueBackward)
                {
                    Pen.Color = Color.FromArgb(87, 181, 252);
                    e.Graphics.DrawImage(bBackWardImageBlue, 0, 0);
                }
                if (eDCC == eDirectionAndColorCommon.BlueForward)
                {
                    Pen.Color = Color.FromArgb(87, 181, 252);
                    e.Graphics.DrawImage(bForwardImageBlue, 0, 0);
                }
                if (eDCC == eDirectionAndColorCommon.WhiteBackward)
                {
                    Pen.Color = Color.White;
                    e.Graphics.DrawImage(bBackWardImageWhite, 0, 0);
                }
                if (eDCC == eDirectionAndColorCommon.WhiteForward)
                {
                    Pen.Color = Color.White;
                    e.Graphics.DrawImage(bForwardImageWhite, 0, 0);
                }
            }
            else
            {
                if (eDCT == eDirectionAndColorTracking.BlackBackward)
                {
                    Pen.Color = Color.Black;
                    e.Graphics.DrawImage(bBackWardImageBlack, 0, 0);
                }
                if (eDCT == eDirectionAndColorTracking.BlackForward)
                {
                    Pen.Color = Color.Black;
                    e.Graphics.DrawImage(bForwardImageBlack, 0, 0);
                }
                if (eDCT == eDirectionAndColorTracking.BlueBackward)
                {
                    Pen.Color = Color.FromArgb(87, 181, 252);
                    e.Graphics.DrawImage(bBackWardImageBlue, 0, 0);
                }
                if (eDCT == eDirectionAndColorTracking.BlueForward)
                {
                    Pen.Color = Color.FromArgb(87, 181, 252);
                    e.Graphics.DrawImage(bForwardImageBlue, 0, 0);
                }
                if (eDCT == eDirectionAndColorTracking.WhiteBackward)
                {
                    Pen.Color = Color.White;
                    e.Graphics.DrawImage(bBackWardImageWhite, 0, 0);
                }
                if (eDCT == eDirectionAndColorTracking.WhiteForward)
                {
                    Pen.Color = Color.White;
                    e.Graphics.DrawImage(bForwardImageWhite, 0, 0);
                }
            }

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            e.Graphics.DrawEllipse(Pen, Rect);
        }

        protected override void OnResize(EventArgs e)
        {
            this.Width = 73;
            this.Height = 73;

            Rect.Height = this.Height - 4;
            Rect.Width = this.Width - 4;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!bTracking)
            {
                bTracking = true;
                this.Refresh();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            bTracking = false;
            this.Refresh();
        }

    }





    public partial class InfiniumLoginForm : Form
    {
        Bitmap Back;

        bool bDrawImage = true;

        public InfiniumLoginForm()
        {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);


            Back = new Bitmap(Properties.Resources.n2);
        }

        public bool DrawImage
        {
            get { return bDrawImage; }
            set { bDrawImage = value; this.Refresh(); }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);

            if (bDrawImage)
                e.Graphics.DrawImage(Back, 0, 0, this.Width, this.Height);
            else
            {
                base.OnPaintBackground(e);
            }
        }
    }





    public class InfiniumForm : Form
    {
        public InfiniumForm()
        {

        }

        public event ANSUpdateEventHandler ANSUpdate;

        public delegate void ANSUpdateEventHandler(object sender);

        public virtual void OnANSUpdate()
        {
            if (ANSUpdate != null)
            {
                ANSUpdate(this);//Raise the event
            }
        }

    }





    public class GlassPanel : Panel
    {
        public GlassPanel()
        {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
        }
    }





    public class SplashWindow
    {
        public static bool bSmallCreated = false;

        public static void CreateSplash()
        {
            SplashForm SplashForm = new SplashForm();

            SplashForm.ShowDialog();

            SplashForm.Dispose();
            SplashForm = null;
            GC.Collect();
        }

        public static void CreateSmallSplash(ref Form TopForm, string Message)
        {
            SmallWaitForm SmallWaitForm = new SmallWaitForm(Message);

            bSmallCreated = true;

            SmallWaitForm.ShowDialog();

            SmallWaitForm.Dispose();
            bSmallCreated = false;
            SmallWaitForm = null;
            GC.Collect();
        }

        public static void CreateCoverSplash(int Top, int Left, int Height, int Width)
        {
            CoverWaitForm CoverWaitForm = new CoverWaitForm(false)
            {
                Width = Width,
                Height = Height,
                Top = Top,
                Left = Left
            };
            CoverWaitForm.ShowDialog();

            CoverWaitForm.Dispose();
            bSmallCreated = false;
            CoverWaitForm = null;
            GC.Collect();
        }

        public static void CreateCoverSplash(bool bSmall, int Top, int Left, int Height, int Width)
        {
            CoverWaitForm CoverWaitForm = new CoverWaitForm(bSmall)
            {
                Width = Width,
                Height = Height,
                Top = Top,
                Left = Left
            };
            CoverWaitForm.ShowDialog();

            CoverWaitForm.Dispose();
            bSmallCreated = false;
            CoverWaitForm = null;
            GC.Collect();
        }

        public static void CreateCoverSplash(int Top, int Left, int Height, int Width, Color BackColor)
        {
            CoverWaitForm CoverWaitForm = new CoverWaitForm(false, BackColor)
            {
                Width = Width,
                Height = Height,
                Top = Top,
                Left = Left
            };
            CoverWaitForm.ShowDialog();

            CoverWaitForm.Dispose();
            bSmallCreated = false;
            CoverWaitForm = null;
            GC.Collect();
        }
    }





    public class AnimatePanel : PictureBox
    {
        public AnimatePanel()
        {
            SetStyle(ControlStyles.UserPaint, true);

            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }
    }





    public class ANSPopupNotify : Control
    {
        public DataTable UpdatesDataTable;

        int iCurrentRow = 0;

        Font fNameFont = new Font("Segoe UI", 16.25f, FontStyle.Regular, GraphicsUnit.Pixel);
        Font fUpdatesFont = new Font("Segoe UI", 13.75f, FontStyle.Regular, GraphicsUnit.Pixel);

        SolidBrush brNameFontBrush;
        SolidBrush brUpdatesFontBrush;
        SolidBrush brTileBackBrush;

        Rectangle rTileRect;

        public System.Timers.Timer NextNotifyTimer;

        delegate void RefreshThis();

        private RefreshThis RefreshPopup;

        public ANSPopupNotify()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);

            brTileBackBrush = new SolidBrush(Color.White);
            brNameFontBrush = new SolidBrush(Color.Black);
            brUpdatesFontBrush = new SolidBrush(Color.Green);

            rTileRect = new Rectangle(0, 0, 72, 72);

            NextNotifyTimer = new System.Timers.Timer(1700);
            NextNotifyTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnNextNotifyTimerElapsed);
            RefreshPopup = new RefreshThis(this.Refresh);
        }

        private void OnNextNotifyTimerElapsed(object source, System.Timers.ElapsedEventArgs e)
        {
            if (UpdatesDataTable.Rows.Count == 1)
                return;

            if (iCurrentRow < UpdatesDataTable.Rows.Count - 1)
                iCurrentRow++;
            else
                iCurrentRow = 0;

            this.Invoke(this.RefreshPopup);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (UpdatesDataTable == null)
                return;

            if (UpdatesDataTable.Rows.Count == 0)
                return;

            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;

            DataRow Row = UpdatesDataTable.Rows[iCurrentRow];

            DrawImage(e.Graphics, Row);
            DrawNameAndUpdates(e.Graphics, Row);

            if (NextNotifyTimer.Enabled == false)
                NextNotifyTimer.Enabled = true;
        }

        private void DrawImage(Graphics G, DataRow Row)
        {
            rTileRect.X = 8;
            rTileRect.Y = 16;

            brTileBackBrush.Color = Color.FromArgb(Convert.ToInt32(Row["Color"]));

            G.FillRectangle(brTileBackBrush, rTileRect);

            if (Row["Image"] == DBNull.Value)
                return;

            byte[] b = (byte[])Row["Image"];
            MemoryStream ms = new MemoryStream(b);

            G.DrawImage(Image.FromStream(ms), 8, 16, 72, 72);

            ms.Dispose();
        }

        private void DrawNameAndUpdates(Graphics G, DataRow Row)
        {
            G.DrawString(Row["Name"].ToString(), fNameFont, brNameFontBrush, 16 + 72, 14);
            G.DrawString("+" + Row["Count"].ToString() + " " + Row["EntrySuffix"].ToString(), fUpdatesFont, brUpdatesFontBrush, 16 + 72, 40);
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            OnClickUpdate();
        }

        public event ClickEventHandler ClickUpdate;

        public delegate void ClickEventHandler(object sender, bool Open, string ModuleButtonName);

        public virtual void OnClickUpdate()
        {
            if (ClickUpdate != null)
            {
                bool Open = false;
                string ModuleButtonName = "";

                if (Convert.ToBoolean(UpdatesDataTable.Rows[iCurrentRow]["OpenOnClick"]))
                {
                    Open = true;
                    ModuleButtonName = UpdatesDataTable.Rows[iCurrentRow]["ModuleButtonName"].ToString();
                }

                ClickUpdate(this, Open, ModuleButtonName);//Raise the event
            }
        }


    }





    public class MessagesContainer : RichTextBox
    {
        Font fSenderFont = new Font("Segoe UI", 13.0f, FontStyle.Bold);
        Font fMeFont = new Font("Segoe UI", 13.0f, FontStyle.Bold);
        Font fTextFont = new Font("Segoe UI", 11.0f, FontStyle.Regular);
        Font fDateTimeFont = new Font("Segoe UI", 9.0f, FontStyle.Regular);
        Font fSpaceFont = new Font("Segoe UI", 2.0f, FontStyle.Regular);

        Color cSenderFontColor = Color.FromArgb(65, 124, 174);
        Color cMeFontColor = Color.FromArgb(255, 15, 60);
        Color cTextFontColor = Color.Black;
        Color cDateTimeColor = Color.Gray;

        DataTable dtMessagesDataTable;

        ArrayList CurrentMessages;

        public int CurrentSenderID;

        public string CurrentUserName;

        public MessagesContainer()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);

            CurrentMessages = new ArrayList();

            HideCaret(this.Handle);
        }


        private const int WM_PAINT = 15;

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_PAINT && true)
            {
                // raise the paint event
                using (Graphics graphic = base.CreateGraphics())
                    OnPaint(new PaintEventArgs(graphic,
                     base.ClientRectangle));
            }

        }

        public DataTable MessagesDataTable
        {
            get { return dtMessagesDataTable; }
            set { dtMessagesDataTable = value; this.Refresh(); }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            //e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            HideCaret(this.Handle);

            //e.Graphics.DrawImage(Image, 20, 20);
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private extern static IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

        [DllImport("user32.dll", EntryPoint = "HideCaret")]
        public static extern long HideCaret(IntPtr hwnd);


        public void BeginUpdate()
        {
            SendMessage(this.Handle, 0xb, (IntPtr)0, IntPtr.Zero);
        }

        public void EndUpdate()
        {
            SendMessage(this.Handle, 0xb, (IntPtr)1, IntPtr.Zero);
            this.Invalidate();
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            HideCaret(this.Handle);
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);

            HideCaret(this.Handle);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            HideCaret(this.Handle);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            HideCaret(this.Handle);
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);

            HideCaret(this.Handle);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            HideCaret(this.Handle);
        }


        public void ClearCurrent()
        {
            CurrentMessages.Clear();
        }

        public void AddData(int SenderID)
        {
            if (SenderID != CurrentSenderID)
            {
                CurrentMessages.Clear();

                this.Text = "";

                CurrentSenderID = SenderID;
            }

            if (dtMessagesDataTable == null)
            {
                CurrentMessages.Clear();
                return;
            }

            if (dtMessagesDataTable.Rows.Count == 0)
            {
                CurrentMessages.Clear();
                return;
            }

            if (CurrentMessages.Count > 0)
                if (Convert.ToInt32(CurrentMessages[CurrentMessages.Count - 1]) == Convert.ToInt32(dtMessagesDataTable.DefaultView[dtMessagesDataTable.DefaultView.Count - 1]["MessageID"]))
                    //if (Convert.ToInt32(CurrentMessages[dtMessagesDataTable.DefaultView.Count - 1]) == Convert.ToInt32(dtMessagesDataTable.DefaultView[dtMessagesDataTable.DefaultView.Count - 1]["MessageID"]))
                    return;

            this.BeginUpdate();

            dtMessagesDataTable.DefaultView.Sort = "MessageID ASC";

            for (int i = 0; i < dtMessagesDataTable.DefaultView.Count; i++)
            {
                if (CurrentMessages.IndexOf(dtMessagesDataTable.DefaultView[i]["MessageID"]) > -1)
                    continue;

                AddSender(dtMessagesDataTable.DefaultView[i]["SenderName"].ToString());
                AddDate(Convert.ToDateTime(dtMessagesDataTable.DefaultView[i]["SendDateTime"]).ToString("HH:mm:ss dd.MM.yyyy"));
                AddSpaceRow();
                AddText(dtMessagesDataTable.DefaultView[i]["Text"].ToString());

                CurrentMessages.Add(dtMessagesDataTable.DefaultView[i]["MessageID"]);
            }

            this.ScrollToCaret();

            this.EndUpdate();
        }

        public void AddSender(string Sender)
        {
            string tempsender = Sender;

            if (this.Text.Length > 0)
                Sender = "\n\n" + Sender + "\n";
            else
                Sender = Sender + "\n";

            this.SelectionStart = this.TextLength;
            this.SelectionLength = 0;
            this.SelectionFont = fSenderFont;

            if (tempsender == CurrentUserName)
                this.SelectionColor = cMeFontColor;
            else
                this.SelectionColor = cSenderFontColor;

            this.AppendText(Sender);
        }

        public void AddDate(string sDateTime)
        {
            sDateTime += "\n";

            this.SelectionStart = this.TextLength;
            this.SelectionLength = 0;
            this.SelectionFont = fDateTimeFont;
            this.SelectionColor = cDateTimeColor;
            this.AppendText(sDateTime);
        }

        public void AddSpaceRow()
        {
            this.SelectionStart = this.TextLength;
            this.SelectionLength = 0;
            this.SelectionFont = fSpaceFont;
            this.SelectionColor = cDateTimeColor;
            this.AppendText(" ");
        }

        public void AddText(string sText)
        {
            this.SelectionStart = this.TextLength;
            this.SelectionLength = 0;
            this.SelectionFont = fTextFont;
            this.SelectionColor = cTextFontColor;
            this.AppendText(sText);
        }
    }





    public class ClientsMessagesContainer : RichTextBox
    {
        Font fSenderFont = new Font("Segoe UI", 13.0f, FontStyle.Bold);
        Font fMeFont = new Font("Segoe UI", 13.0f, FontStyle.Bold);
        Font fTextFont = new Font("Segoe UI", 11.0f, FontStyle.Regular);
        Font fDateTimeFont = new Font("Segoe UI", 9.0f, FontStyle.Regular);
        Font fSpaceFont = new Font("Segoe UI", 2.0f, FontStyle.Regular);

        Color cSenderFontColor = Color.FromArgb(65, 124, 174);
        Color cMeFontColor = Color.FromArgb(255, 15, 60);
        Color cTextFontColor = Color.Black;
        Color cDateTimeColor = Color.Gray;

        DataTable dtMessagesDataTable;

        ArrayList CurrentMessages;

        public int CurrentSenderID;

        public string CurrentUserName;

        public ClientsMessagesContainer()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);

            CurrentMessages = new ArrayList();

            HideCaret(this.Handle);
        }


        private const int WM_PAINT = 15;

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_PAINT && true)
            {
                // raise the paint event
                using (Graphics graphic = base.CreateGraphics())
                    OnPaint(new PaintEventArgs(graphic,
                     base.ClientRectangle));
            }

        }

        public DataTable MessagesDataTable
        {
            get { return dtMessagesDataTable; }
            set { dtMessagesDataTable = value; this.Refresh(); }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            //e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            HideCaret(this.Handle);

            //e.Graphics.DrawImage(Image, 20, 20);
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private extern static IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

        [DllImport("user32.dll", EntryPoint = "HideCaret")]
        public static extern long HideCaret(IntPtr hwnd);


        public void BeginUpdate()
        {
            SendMessage(this.Handle, 0xb, (IntPtr)0, IntPtr.Zero);
        }

        public void EndUpdate()
        {
            SendMessage(this.Handle, 0xb, (IntPtr)1, IntPtr.Zero);
            this.Invalidate();
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            HideCaret(this.Handle);
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);

            HideCaret(this.Handle);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            HideCaret(this.Handle);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            HideCaret(this.Handle);
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);

            HideCaret(this.Handle);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            HideCaret(this.Handle);
        }

        public void ClearCurrent()
        {
            CurrentMessages.Clear();
        }

        public void AddData(int SenderID)
        {
            if (SenderID != CurrentSenderID)
            {
                CurrentMessages.Clear();

                this.Text = "";

                CurrentSenderID = SenderID;
            }

            if (dtMessagesDataTable == null)
            {
                CurrentMessages.Clear();
                return;
            }

            if (dtMessagesDataTable.Rows.Count == 0)
            {
                CurrentMessages.Clear();
                return;
            }

            if (CurrentMessages.Count > 0)
                if (Convert.ToInt32(CurrentMessages[CurrentMessages.Count - 1]) == Convert.ToInt32(dtMessagesDataTable.DefaultView[dtMessagesDataTable.DefaultView.Count - 1]["MessageID"]))
                    //if (Convert.ToInt32(CurrentMessages[dtMessagesDataTable.DefaultView.Count - 1]) == Convert.ToInt32(dtMessagesDataTable.DefaultView[dtMessagesDataTable.DefaultView.Count - 1]["MessageID"]))
                    return;

            this.BeginUpdate();

            dtMessagesDataTable.DefaultView.Sort = "MessageID ASC";

            for (int i = 0; i < dtMessagesDataTable.DefaultView.Count; i++)
            {
                if (CurrentMessages.IndexOf(dtMessagesDataTable.DefaultView[i]["MessageID"]) > -1)
                    continue;

                AddSender(dtMessagesDataTable.DefaultView[i]["SenderName"].ToString());
                AddDate(Convert.ToDateTime(dtMessagesDataTable.DefaultView[i]["SendDateTime"]).ToString("HH:mm:ss dd.MM.yyyy"));
                AddSpaceRow();
                AddText(dtMessagesDataTable.DefaultView[i]["Text"].ToString());

                CurrentMessages.Add(dtMessagesDataTable.DefaultView[i]["MessageID"]);
            }

            this.ScrollToCaret();

            this.EndUpdate();
        }

        public void AddDataClient(int SenderID)
        {
            if (SenderID != CurrentSenderID)
            {
                CurrentMessages.Clear();

                this.Text = "";

                CurrentSenderID = SenderID;
            }

            if (dtMessagesDataTable == null)
            {
                CurrentMessages.Clear();
                return;
            }

            if (dtMessagesDataTable.Rows.Count == 0)
            {
                CurrentMessages.Clear();
                return;
            }

            if (CurrentMessages.Count > 0)
                if (Convert.ToInt32(CurrentMessages[CurrentMessages.Count - 1]) == Convert.ToInt32(dtMessagesDataTable.DefaultView[dtMessagesDataTable.DefaultView.Count - 1]["ClientsMessageID"]))
                    return;

            this.BeginUpdate();

            dtMessagesDataTable.DefaultView.Sort = "ClientsMessageID ASC";

            for (int i = 0; i < dtMessagesDataTable.DefaultView.Count; i++)
            {
                if (CurrentMessages.IndexOf(dtMessagesDataTable.DefaultView[i]["ClientsMessageID"]) > -1)
                    continue;

                AddSender(dtMessagesDataTable.DefaultView[i]["SenderName"].ToString());
                AddDate(Convert.ToDateTime(dtMessagesDataTable.DefaultView[i]["SendDateTime"]).ToString("HH:mm:ss dd.MM.yyyy"));
                AddSpaceRow();
                AddText(dtMessagesDataTable.DefaultView[i]["MessageText"].ToString());

                CurrentMessages.Add(dtMessagesDataTable.DefaultView[i]["ClientsMessageID"]);
            }

            this.ScrollToCaret();

            this.EndUpdate();
        }

        public void AddSender(string Sender)
        {
            string tempsender = Sender;

            if (this.Text.Length > 0)
                Sender = "\n\n" + Sender + "\n";
            else
                Sender = Sender + "\n";

            this.SelectionStart = this.TextLength;
            this.SelectionLength = 0;
            this.SelectionFont = fSenderFont;

            if (tempsender == CurrentUserName)
                this.SelectionColor = cMeFontColor;
            else
                this.SelectionColor = cSenderFontColor;

            this.AppendText(Sender);
        }

        public void AddDate(string sDateTime)
        {
            sDateTime += "\n";

            this.SelectionStart = this.TextLength;
            this.SelectionLength = 0;
            this.SelectionFont = fDateTimeFont;
            this.SelectionColor = cDateTimeColor;
            this.AppendText(sDateTime);
        }

        public void AddSpaceRow()
        {
            this.SelectionStart = this.TextLength;
            this.SelectionLength = 0;
            this.SelectionFont = fSpaceFont;
            this.SelectionColor = cDateTimeColor;
            this.AppendText(" ");
        }

        public void AddText(string sText)
        {
            this.SelectionStart = this.TextLength;
            this.SelectionLength = 0;
            this.SelectionFont = fTextFont;
            this.SelectionColor = cTextFontColor;
            this.AppendText(sText);
        }
    }






    public class ZOVMessagesContainer : RichTextBox
    {
        Font fSenderFont = new Font("Segoe UI", 13.0f, FontStyle.Bold);
        Font fMeFont = new Font("Segoe UI", 13.0f, FontStyle.Bold);
        Font fTextFont = new Font("Segoe UI", 11.0f, FontStyle.Regular);
        Font fDateTimeFont = new Font("Segoe UI", 9.0f, FontStyle.Regular);
        Font fSpaceFont = new Font("Segoe UI", 2.0f, FontStyle.Regular);

        Color cSenderFontColor = Color.FromArgb(65, 124, 174);
        Color cMeFontColor = Color.FromArgb(255, 15, 60);
        Color cTextFontColor = Color.Black;
        Color cDateTimeColor = Color.Gray;

        DataTable dtMessagesDataTable;

        ArrayList CurrentMessages;

        public int CurrentSenderID;

        public string CurrentUserName;

        public ZOVMessagesContainer()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);

            CurrentMessages = new ArrayList();

            HideCaret(this.Handle);
        }


        private const int WM_PAINT = 15;

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_PAINT && true)
            {
                // raise the paint event
                using (Graphics graphic = base.CreateGraphics())
                    OnPaint(new PaintEventArgs(graphic,
                     base.ClientRectangle));
            }

        }

        public DataTable MessagesDataTable
        {
            get { return dtMessagesDataTable; }
            set { dtMessagesDataTable = value; this.Refresh(); }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            //e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            HideCaret(this.Handle);

            //e.Graphics.DrawImage(Image, 20, 20);
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private extern static IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

        [DllImport("user32.dll", EntryPoint = "HideCaret")]
        public static extern long HideCaret(IntPtr hwnd);


        public void BeginUpdate()
        {
            SendMessage(this.Handle, 0xb, (IntPtr)0, IntPtr.Zero);
        }

        public void EndUpdate()
        {
            SendMessage(this.Handle, 0xb, (IntPtr)1, IntPtr.Zero);
            this.Invalidate();
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            HideCaret(this.Handle);
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);

            HideCaret(this.Handle);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            HideCaret(this.Handle);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            HideCaret(this.Handle);
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);

            HideCaret(this.Handle);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            HideCaret(this.Handle);
        }

        public void ClearCurrent()
        {
            CurrentMessages.Clear();
        }

        public void AddData(int SenderID)
        {
            if (SenderID != CurrentSenderID)
            {
                CurrentMessages.Clear();

                this.Text = "";

                CurrentSenderID = SenderID;
            }

            if (dtMessagesDataTable == null)
            {
                CurrentMessages.Clear();
                return;
            }

            if (dtMessagesDataTable.Rows.Count == 0)
            {
                CurrentMessages.Clear();
                return;
            }

            if (CurrentMessages.Count > 0)
                if (Convert.ToInt32(CurrentMessages[CurrentMessages.Count - 1]) == Convert.ToInt32(dtMessagesDataTable.DefaultView[dtMessagesDataTable.DefaultView.Count - 1]["MessageID"]))
                    //if (Convert.ToInt32(CurrentMessages[dtMessagesDataTable.DefaultView.Count - 1]) == Convert.ToInt32(dtMessagesDataTable.DefaultView[dtMessagesDataTable.DefaultView.Count - 1]["MessageID"]))
                    return;

            this.BeginUpdate();

            dtMessagesDataTable.DefaultView.Sort = "MessageID ASC";

            for (int i = 0; i < dtMessagesDataTable.DefaultView.Count; i++)
            {
                if (CurrentMessages.IndexOf(dtMessagesDataTable.DefaultView[i]["MessageID"]) > -1)
                    continue;

                AddSender(dtMessagesDataTable.DefaultView[i]["SenderName"].ToString());
                AddDate(Convert.ToDateTime(dtMessagesDataTable.DefaultView[i]["SendDateTime"]).ToString("HH:mm:ss dd.MM.yyyy"));
                AddSpaceRow();
                AddText(dtMessagesDataTable.DefaultView[i]["Text"].ToString());

                CurrentMessages.Add(dtMessagesDataTable.DefaultView[i]["MessageID"]);
            }

            this.ScrollToCaret();

            this.EndUpdate();
        }

        public void AddDataClient(int SenderID)
        {
            if (SenderID != CurrentSenderID)
            {
                CurrentMessages.Clear();

                this.Text = "";

                CurrentSenderID = SenderID;
            }

            if (dtMessagesDataTable == null)
            {
                CurrentMessages.Clear();
                return;
            }

            if (dtMessagesDataTable.Rows.Count == 0)
            {
                CurrentMessages.Clear();
                return;
            }

            if (CurrentMessages.Count > 0)
                if (Convert.ToInt32(CurrentMessages[CurrentMessages.Count - 1]) == Convert.ToInt32(dtMessagesDataTable.DefaultView[dtMessagesDataTable.DefaultView.Count - 1]["MessageID"]))
                    return;

            this.BeginUpdate();

            dtMessagesDataTable.DefaultView.Sort = "MessageID ASC";

            for (int i = 0; i < dtMessagesDataTable.DefaultView.Count; i++)
            {
                if (CurrentMessages.IndexOf(dtMessagesDataTable.DefaultView[i]["MessageID"]) > -1)
                    continue;

                AddSender(dtMessagesDataTable.DefaultView[i]["SenderName"].ToString());
                AddDate(Convert.ToDateTime(dtMessagesDataTable.DefaultView[i]["SendDateTime"]).ToString("HH:mm:ss dd.MM.yyyy"));
                AddSpaceRow();
                AddText(dtMessagesDataTable.DefaultView[i]["MessageText"].ToString());

                CurrentMessages.Add(dtMessagesDataTable.DefaultView[i]["MessageID"]);
            }

            this.ScrollToCaret();

            this.EndUpdate();
        }

        public void AddSender(string Sender)
        {
            string tempsender = Sender;

            if (this.Text.Length > 0)
                Sender = "\n\n" + Sender + "\n";
            else
                Sender = Sender + "\n";

            this.SelectionStart = this.TextLength;
            this.SelectionLength = 0;
            this.SelectionFont = fSenderFont;

            if (tempsender == CurrentUserName)
                this.SelectionColor = cMeFontColor;
            else
                this.SelectionColor = cSenderFontColor;

            this.AppendText(Sender);
        }

        public void AddDate(string sDateTime)
        {
            sDateTime += "\n";

            this.SelectionStart = this.TextLength;
            this.SelectionLength = 0;
            this.SelectionFont = fDateTimeFont;
            this.SelectionColor = cDateTimeColor;
            this.AppendText(sDateTime);
        }

        public void AddSpaceRow()
        {
            this.SelectionStart = this.TextLength;
            this.SelectionLength = 0;
            this.SelectionFont = fSpaceFont;
            this.SelectionColor = cDateTimeColor;
            this.AppendText(" ");
        }

        public void AddText(string sText)
        {
            this.SelectionStart = this.TextLength;
            this.SelectionLength = 0;
            this.SelectionFont = fTextFont;
            this.SelectionColor = cTextFontColor;
            this.AppendText(sText);
        }
    }




    public class NotesContainer : ComponentFactory.Krypton.Toolkit.KryptonRichTextBox
    {
        public NotesContainer()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }


        private const int WM_PAINT = 15;

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_PAINT && true)
            {
                // raise the paint event
                using (Graphics graphic = base.CreateGraphics())
                    OnPaint(new PaintEventArgs(graphic,
                     base.ClientRectangle));
            }

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private extern static IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);


        public void BeginUpdate()
        {
            SendMessage(this.Handle, 0xb, (IntPtr)0, IntPtr.Zero);
        }

        public void EndUpdate()
        {
            SendMessage(this.Handle, 0xb, (IntPtr)1, IntPtr.Zero);
            this.Invalidate();
        }
    }






    public class LightPanel : ComponentFactory.Krypton.Toolkit.KryptonPanel
    {
        Color cBorderColor = Color.Black;

        Pen pBorderPen;

        Rectangle rBorderRect;

        public LightPanel()
        {
            pBorderPen = new Pen(new SolidBrush(cBorderColor));

            rBorderRect = new Rectangle(0, 0, 1, 1);
        }

        public Color BorderColor
        {
            get { return cBorderColor; }
            set { cBorderColor = value; pBorderPen.Color = cBorderColor; this.Refresh(); }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.DrawRectangle(pBorderPen, rBorderRect);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            rBorderRect.Height = this.Height - 1;
            rBorderRect.Width = this.Width - 1;
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            rBorderRect.Height = this.Height - 1;
            rBorderRect.Width = this.Width - 1;
        }

    }





    public class NotesList : Control
    {
        int iOffset = 0;

        //int iItemHeight = 40;

        bool bScrollVisible = false;

        Bitmap SimpleNoteBmp = Properties.Resources.SimpleNote;
        Bitmap FavouriteNoteBmp = Properties.Resources.FavouriteNote;

        public int iSelected = -1;
        public int iClicked = -1;

        int ItemHeight = 80;
        int ImageSize = 55;

        Rectangle rItemRect;
        Rectangle rBorderRect;

        Color cNotesNameColor = Color.Black;
        Color cSelectedNotesNameColor = Color.Black;
        Color cNotesTextSampleColor = Color.Black;
        Color cSelectedNotesTextSampleColor = Color.Black;
        Color cNotesDateColor = Color.Gray;
        Color cSelectedNotesDateColor = Color.Gray;
        Color cItemLineColor = Color.Gray;
        Color cVerticalScrollCommonShaftBackColor = Color.White;
        Color cVerticalScrollCommonThumbButtonColor = Color.Gray;
        Color cSelectedBackColor = Color.Gray;
        Color cBorderColor = Color.Gray;

        Font fNotesNameFont;
        Font fNotesTextSampleFont;
        Font fNotesDateFont;

        SolidBrush brNotesNameBrush;
        SolidBrush brSelectedNotesNameBrush;
        SolidBrush brNotesTextSampleBrush;
        SolidBrush brSelectedNotesTextSampleBrush;
        SolidBrush brNotesDateBrush;
        SolidBrush brSelectedNotesDateBrush;
        SolidBrush brVerticalScrollCommonShaftBackBrush;
        SolidBrush brVerticalScrollCommonThumbButtonBrush;
        SolidBrush brSelectedBrush;

        Pen pItemLinePen;
        Pen pBorderPen;

        Rectangle VerticalScrollShaftRect;

        public DataTable NotesDataTable;

        public int CurrentNotesID;

        public NotesList()
        {
            fNotesNameFont = new Font("Segoe UI", 20.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            fNotesTextSampleFont = new Font("Segoe UI Semilight", 16.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            fNotesDateFont = new Font("Segoe UI Semilight", 14.0f, FontStyle.Regular, GraphicsUnit.Pixel);

            brNotesNameBrush = new SolidBrush(cNotesNameColor);
            brSelectedNotesNameBrush = new SolidBrush(cSelectedNotesNameColor);
            brNotesTextSampleBrush = new SolidBrush(cNotesTextSampleColor);
            brSelectedNotesTextSampleBrush = new SolidBrush(cSelectedNotesTextSampleColor);
            brNotesDateBrush = new SolidBrush(cNotesDateColor);
            brSelectedNotesDateBrush = new SolidBrush(cSelectedNotesDateColor);
            brVerticalScrollCommonShaftBackBrush = new SolidBrush(cVerticalScrollCommonShaftBackColor);
            brVerticalScrollCommonThumbButtonBrush = new SolidBrush(cVerticalScrollCommonThumbButtonColor);
            brSelectedBrush = new SolidBrush(cSelectedBackColor);


            pItemLinePen = new Pen(new SolidBrush(cItemLineColor));
            pBorderPen = new Pen(new SolidBrush(cBorderColor));

            rItemRect = new Rectangle(0, 0, 1, ItemHeight - 1);
            rBorderRect = new Rectangle(0, 0, this.Width, this.Height);

            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            VerticalScrollShaftRect = new Rectangle(this.Width - 10, 0, 10, this.Height);
        }


        public Color NotesNameColor
        {
            get { return cNotesNameColor; }
            set { cNotesNameColor = value; brNotesNameBrush.Color = cNotesNameColor; this.Refresh(); }
        }

        public Color SelectedNotesNameColor
        {
            get { return cSelectedNotesNameColor; }
            set { cSelectedNotesNameColor = value; brSelectedNotesNameBrush.Color = SelectedNotesNameColor; this.Refresh(); }
        }

        public Color NotesTextSampleColor
        {
            get { return cNotesTextSampleColor; }
            set { cNotesTextSampleColor = value; brNotesTextSampleBrush.Color = cNotesTextSampleColor; this.Refresh(); }
        }

        public Color SelectedNotesTextSampleColor
        {
            get { return cSelectedNotesTextSampleColor; }
            set { cSelectedNotesTextSampleColor = value; brSelectedNotesTextSampleBrush.Color = cSelectedNotesTextSampleColor; this.Refresh(); }
        }

        public Color NotesDateColor
        {
            get { return cNotesDateColor; }
            set { cNotesDateColor = value; brNotesDateBrush.Color = cNotesDateColor; this.Refresh(); }
        }

        public Color SelectedNotesDateColor
        {
            get { return cSelectedNotesDateColor; }
            set { cSelectedNotesDateColor = value; brSelectedNotesDateBrush.Color = cSelectedNotesDateColor; this.Refresh(); }
        }

        public Color ItemLineColor
        {
            get { return cItemLineColor; }
            set { cItemLineColor = value; pItemLinePen.Color = cItemLineColor; this.Refresh(); }
        }

        public Color BorderColor
        {
            get { return cBorderColor; }
            set { cBorderColor = value; pBorderPen.Color = cBorderColor; this.Refresh(); }
        }

        public Color SelectedBackColor
        {
            get { return cSelectedBackColor; }
            set { cSelectedBackColor = value; brSelectedBrush.Color = cSelectedBackColor; this.Refresh(); }
        }

        public Color VerticalScrollCommonShaftBackColor
        {
            get { return cVerticalScrollCommonShaftBackColor; }
            set { cVerticalScrollCommonShaftBackColor = value; brVerticalScrollCommonShaftBackBrush.Color = cVerticalScrollCommonShaftBackColor; this.Refresh(); }
        }

        public Color VerticalScrollCommonThumbButtonColor
        {
            get { return cVerticalScrollCommonThumbButtonColor; }
            set { cVerticalScrollCommonThumbButtonColor = value; brVerticalScrollCommonThumbButtonBrush.Color = cVerticalScrollCommonThumbButtonColor; this.Refresh(); }
        }



        public Font NotesNameFont
        {
            get { return fNotesNameFont; }
            set { fNotesNameFont = value; this.Refresh(); }
        }

        public Font NotesTextSampleFont
        {
            get { return fNotesTextSampleFont; }
            set { fNotesTextSampleFont = value; this.Refresh(); }
        }

        public Font NotesDateFont
        {
            get { return fNotesDateFont; }
            set { fNotesDateFont = value; this.Refresh(); }
        }


        public DataTable UsersDataTable
        {
            get { return NotesDataTable; }
            set { NotesDataTable = value; this.Refresh(); }
        }


        public int GetCurrentNotesID()
        {
            return Convert.ToInt32(NotesDataTable.DefaultView[iSelected][0]);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.DrawRectangle(pBorderPen, rBorderRect);

            if (NotesDataTable == null)
                return;

            if (NotesDataTable.Rows.Count == 0)
                return;

            if (iSelected == -1)
                iSelected = 0;

            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            if (NotesDataTable.DefaultView.Count == 0)
                return;

            for (int i = iOffset; i < NotesDataTable.DefaultView.Count; i++)
            {
                if (this.Height <= (i) * ItemHeight - iOffset * ItemHeight)
                    break;

                string sNotesName = NotesDataTable.DefaultView[i]["NotesName"].ToString();
                string sDate = NotesDataTable.DefaultView[i]["CreationDateTime"].ToString();
                string sTextSample = NotesDataTable.DefaultView[i]["NotesText"].ToString();
                bool iPriority = Convert.ToBoolean(NotesDataTable.DefaultView[i]["Priority"]);
                int iMarginForScroll = 0;
                if (bScrollVisible)
                    iMarginForScroll = VerticalScrollShaftRect.Width;



                if (i == iClicked)
                {
                    DrawBackSelected(e.Graphics, i);

                    DrawNameSelected(sNotesName, this.Width - Convert.ToInt32(e.Graphics.MeasureString(sDate, fNotesDateFont).Width) - ImageSize - iMarginForScroll, i, e.Graphics);

                    e.Graphics.DrawString(sDate, fNotesDateFont, brSelectedNotesDateBrush, 8 + ImageSize + 5, i * (ItemHeight) + 30 - iOffset * ItemHeight);

                    DrawTextSampleSelected(sTextSample, this.Width - iMarginForScroll - ImageSize - 5, i, e.Graphics);
                }
                else
                {
                    DrawName(sNotesName, this.Width - Convert.ToInt32(e.Graphics.MeasureString(sDate, fNotesDateFont).Width) - ImageSize - iMarginForScroll, i, e.Graphics);

                    e.Graphics.DrawString(sDate, fNotesDateFont, brNotesDateBrush, 8 + ImageSize + 5, i * (ItemHeight) + 30 - iOffset * ItemHeight);

                    DrawTextSample(sTextSample, this.Width - iMarginForScroll - ImageSize - 5, i, e.Graphics);
                }

                if (bScrollVisible)
                    e.Graphics.DrawLine(pItemLinePen, 0, i * (ItemHeight) - iOffset * ItemHeight + ItemHeight,
                                        this.Width - VerticalScrollShaftRect.Width + 1, i * (ItemHeight) - iOffset * ItemHeight + ItemHeight);
                else
                    e.Graphics.DrawLine(pItemLinePen, 0, i * (ItemHeight) - iOffset * ItemHeight + ItemHeight,
                                                            this.Width, i * (ItemHeight) - iOffset * ItemHeight + ItemHeight);

                DrawPriority(e.Graphics, i, iPriority);
            }


            DrawVerticalScrollShaft(e.Graphics);
            DrawVertScrollThumb(e.Graphics);

            e.Graphics.DrawRectangle(pBorderPen, rBorderRect);
        }

        private void DrawPriority(Graphics G, int index, bool Favour)
        {
            if (Favour)
                G.DrawImage(FavouriteNoteBmp, 5, index * ItemHeight + ((ItemHeight - ImageSize) / 2) - iOffset * ItemHeight, ImageSize, ImageSize);
            else
                G.DrawImage(SimpleNoteBmp, 5, index * ItemHeight + ((ItemHeight - ImageSize) / 2) - iOffset * ItemHeight, ImageSize, ImageSize);


            // G.DrawString(shorttext, fNotesNameFont, brNotesNameBrush, 7 + SimpleNoteBmp.Width + 5, index * (ItemHeight) + 2 - iOffset * ItemHeight);
        }

        private void DrawBackSelected(Graphics G, int index)
        {
            rItemRect.X = 0;
            rItemRect.Y = index * ItemHeight - iOffset * ItemHeight;
            if (bScrollVisible)
                rItemRect.Width = this.Width - VerticalScrollShaftRect.Width;
            else
                rItemRect.Width = this.Width;
            rItemRect.Height = ItemHeight;

            G.FillRectangle(brSelectedBrush, rItemRect);
        }

        public void ScrollLast()
        {
            if (NotesDataTable.DefaultView.Count == 0)
                return;

            int iVisCount = this.Height / ItemHeight;

            if (iVisCount < NotesDataTable.DefaultView.Count - 1)
                iOffset = (NotesDataTable.DefaultView.Count * ItemHeight - iVisCount * ItemHeight) / ItemHeight;

            iSelected = NotesDataTable.DefaultView.Count - 1;
            iClicked = iSelected;

            OnItemClick();
        }

        public void ScrollPrevious(int index)
        {
            if (NotesDataTable.DefaultView.Count == 0)
                return;

            if (NotesDataTable.DefaultView.Count == 0)
            {
                iOffset = 0;
                iClicked = -1;
                iSelected = -1;
                return;
            }

            if (index > 0)
                index--;

            int iVisCount = this.Height / ItemHeight;

            if (iVisCount < index)
                iOffset = iVisCount - index;

            iSelected = index;
            iClicked = iSelected;

            OnItemClick();
        }

        public void ScrollFirst()
        {
            if (NotesDataTable == null)
                return;

            if (NotesDataTable.DefaultView.Count == 0)
                return;

            iOffset = 0;

            iSelected = 0;
            iClicked = iSelected;

            OnItemClick();
        }

        //private string OneLineToTwo(string text, Font Font, int MaxWidth)
        //{
        //    if (text.Length == 0)
        //        return "";

        //    Graphics G = this.CreateGraphics();

        //    if (G.MeasureString(text, Font).Width > MaxWidth)
        //    {
        //        int LastSpace = GetLastSpace(text);

        //        text = text.Insert(LastSpace + 1, "\n");
        //    }

        //    G.Dispose();

        //    return text;
        //}


        private void DrawName(string text, int MaxWidth, int index, Graphics G)
        {
            string shorttext = "";

            if (G.MeasureString(text, fNotesNameFont).Width > MaxWidth)
            {
                shorttext = text.Substring(0, 10);

                for (int i = 11; i < text.Length; i++)
                {
                    shorttext += text[i];

                    if (G.MeasureString(shorttext, fNotesNameFont).Width > MaxWidth)
                    {
                        shorttext = shorttext.Substring(0, shorttext.Length - 3) + "...";
                        break;
                    }
                }
            }
            else
                shorttext = text;

            G.DrawString(shorttext, fNotesNameFont, brNotesNameBrush, 7 + ImageSize + 5, index * (ItemHeight) + 2 - iOffset * ItemHeight);
        }

        private void DrawNameSelected(string text, int MaxWidth, int index, Graphics G)
        {
            string shorttext = "";

            if (G.MeasureString(text, fNotesNameFont).Width > MaxWidth)
            {
                shorttext = text.Substring(0, 10);

                for (int i = 11; i < text.Length; i++)
                {
                    shorttext += text[i];

                    if (G.MeasureString(shorttext, fNotesNameFont).Width > MaxWidth)
                    {
                        shorttext = shorttext.Substring(0, shorttext.Length - 3) + "...";
                        break;
                    }
                }
            }
            else
                shorttext = text;

            G.DrawString(shorttext, fNotesNameFont, brSelectedNotesNameBrush, 7 + ImageSize + 5, index * (ItemHeight) + 2 - iOffset * ItemHeight);
        }


        private void DrawTextSample(string text, int MaxWidth, int index, Graphics G)
        {
            string shorttext = "";



            if (G.MeasureString(text, fNotesTextSampleFont).Width > MaxWidth)
            {
                shorttext = text.Substring(0, 10);

                for (int i = 11; i < text.Length; i++)
                {
                    if (text[i].ToString() == "\n")
                        break;

                    shorttext += text[i];

                    if (G.MeasureString(shorttext, fNotesTextSampleFont).Width > MaxWidth)
                    {
                        shorttext = shorttext.Substring(0, shorttext.Length - 3) + "...";
                        break;
                    }
                }
            }
            else
            {
                if (text.IndexOf("\n") > -1)
                    shorttext = text.Substring(0, text.IndexOf("\n"));
                else
                    shorttext = text;
            }


            G.DrawString(shorttext, fNotesTextSampleFont, brNotesTextSampleBrush, 7 + ImageSize + 5, index * (ItemHeight) + 55 - iOffset * ItemHeight);
        }

        private void DrawTextSampleSelected(string text, int MaxWidth, int index, Graphics G)
        {
            string shorttext = "";

            if (G.MeasureString(text, fNotesTextSampleFont).Width > MaxWidth)
            {
                shorttext = text.Substring(0, 10);

                for (int i = 11; i < text.Length; i++)
                {
                    shorttext += text[i];

                    if (G.MeasureString(shorttext, fNotesTextSampleFont).Width > MaxWidth)
                    {
                        shorttext = shorttext.Substring(0, shorttext.Length - 3) + "...";
                        break;
                    }
                }
            }
            else
            {
                if (text.IndexOf("\n") > -1)
                    shorttext = text.Substring(0, text.IndexOf("\n"));
                else
                    shorttext = text;
            }

            G.DrawString(shorttext, fNotesTextSampleFont, brSelectedNotesTextSampleBrush, 7 + ImageSize + 5, index * (ItemHeight) + 55 - iOffset * ItemHeight);
        }


        //private int GetLastSpace(string Text)
        //{
        //    int LastSpace = 0;

        //    for (int i = Text.Length - 1; i >= 0; i--)
        //    {
        //        if (Text[i] == ' ')
        //        {
        //            LastSpace = i;
        //            break;
        //        }
        //    }

        //    if (LastSpace == 0)//no spaces was found
        //    {
        //        for (int i = Text.Length - 1; i >= 0; i--)
        //        {
        //            if (Text[i] == '.' || Text[i] == ',' || Text[i] == ';' || Text[i] == ':')
        //            {
        //                LastSpace = i;
        //                break;
        //            }
        //        }
        //    }

        //    return LastSpace;
        //}

        private void DrawVerticalScrollShaft(Graphics G)
        {
            if (this.Height >= NotesDataTable.DefaultView.Count * ItemHeight)
            {
                bScrollVisible = false;
                return;
            }

            //Shaft
            G.FillRectangle(brVerticalScrollCommonShaftBackBrush, VerticalScrollShaftRect);
            bScrollVisible = true;
        }

        private void DrawVertScrollThumb(Graphics G)
        {
            //if (this.Height >= NotesDataTable.DefaultView.Count * ItemHeight)
            //{
            //    return;
            //}

            //decimal V = this.Height;
            //decimal T = Convert.ToDecimal(NotesDataTable.DefaultView.Count * ItemHeight);

            //decimal Rtv = V / (T / 100);

            //decimal Th = Convert.ToInt32(Decimal.Round(Rtv * (V / 100), 0, MidpointRounding.AwayFromZero));

            //if (Th >= V)
            //    return;

            //decimal Ws = (V - Th) / ((T - V) / ItemHeight);

            //int posY = iOffset * Convert.ToInt32(Ws);

            //G.FillRectangle(brVerticalScrollCommonThumbButtonBrush, new Rectangle(VerticalScrollShaftRect.X + 2, posY,
            //                                                                      10 - 4, Convert.ToInt32(Th)));





            int ItemsCount = 0;
            int TopOf = 0;

            if (iOffset == 0)
                TopOf = 2;

            ItemsCount = NotesDataTable.DefaultView.Count;

            decimal V = this.Height - 4;
            decimal T = Convert.ToDecimal(ItemsCount) * Convert.ToDecimal(ItemHeight);

            decimal Rtv = V / (T / 100);

            decimal Th = Math.Truncate(Rtv * (V / 100));

            if (Th >= V)
                return;

            decimal Ws = ((V - Th) / ((T - V) / ItemHeight));

            int posY = Convert.ToInt32(Convert.ToDecimal(iOffset) * Ws) + TopOf;

            int K = 0;
            if (iOffset == ItemsCount - (this.Height / ItemHeight))
                K = this.Height - (posY + Convert.ToInt32(Th)) - 2;

            G.FillRectangle(brVerticalScrollCommonThumbButtonBrush, new Rectangle(VerticalScrollShaftRect.X + 2, posY + K,
                                                                                  10 - 4, Convert.ToInt32(Th)));
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            VerticalScrollShaftRect.X = this.Width - 10;
            VerticalScrollShaftRect.Height = this.Height;
            VerticalScrollShaftRect.Width = 10;

            rBorderRect.Height = this.Height - 1;
            rBorderRect.Width = this.Width - 1;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            VerticalScrollShaftRect.X = this.Width - 10;
            VerticalScrollShaftRect.Height = this.Height;
            VerticalScrollShaftRect.Width = 10;

            rBorderRect.Height = this.Height - 1;
            rBorderRect.Width = this.Width - 1;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            //if (!this.Focused)
            //    this.Focus();

            iSelected = SelectItem(e.X, e.Y);
        }

        private int SelectItem(int X, int Y)
        {
            if (NotesDataTable == null)
                return iSelected;

            if ((Y / ItemHeight) + iOffset > NotesDataTable.DefaultView.Count - 1)
                return iSelected;

            return (Y / ItemHeight) + iOffset;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            if (!this.Focused)
                this.Focus();

            iClicked = iSelected;

            OnItemClick();

            this.Refresh();
        }



        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (e.Delta < 0)
            {
                if (iOffset < (NotesDataTable.DefaultView.Count - (this.Height / ItemHeight)))
                {
                    iOffset++;
                    this.Refresh();
                }
            }

            if (e.Delta > 0)
                if (iOffset > 0)
                {
                    iOffset--;
                    this.Refresh();
                }
        }

        public event ItemClickEventHandler ItemClick;

        public delegate void ItemClickEventHandler(object sender, int NotesID, string NotesName);

        public virtual void OnItemClick()
        {
            if (ItemClick != null)
            {
                ItemClick(this, Convert.ToInt32(NotesDataTable.DefaultView[iSelected][0]), NotesDataTable.DefaultView[iSelected]["NotesName"].ToString());//Raise the event
            }
        }

    }





    public class NotifyLabel : Control
    {
        System.Timers.Timer AnimateTimer;
        System.Timers.Timer SVTimer;

        Color cTextColor = Color.Black;

        int iAlpha = 0;

        SolidBrush Brush;

        //bool bShow = false;
        int tAnimate = -1;

        int AnimateSpeedInMs = 1;
        int StayVisibleTimeInMs = 3000;

        delegate void LabelRefresh();
        private LabelRefresh LabelRefreshT;

        public NotifyLabel()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);

            Brush = new SolidBrush(cTextColor)
            {
                Color = Color.FromArgb(iAlpha, cTextColor)
            };
            AnimateTimer = new System.Timers.Timer(AnimateSpeedInMs);
            AnimateTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnAnimateTimedEvent);

            SVTimer = new System.Timers.Timer(StayVisibleTimeInMs);
            SVTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnSVTimedEvent);

            LabelRefreshT = new LabelRefresh(this.Refresh);
        }

        private void OnSVTimedEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            AnimateTimer.Enabled = true;
            SVTimer.Enabled = false;
        }

        private void OnAnimateTimedEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            if (tAnimate == -1)
                return;

            if (tAnimate == 1)//show
            {
                if (iAlpha < 255)
                {
                    if (iAlpha + 7 > 255)
                        Brush.Color = Color.FromArgb(iAlpha = 255, cTextColor);
                    else
                        Brush.Color = Color.FromArgb(iAlpha += 7, cTextColor);

                    this.Invoke(LabelRefreshT);
                    return;
                }
                else
                {
                    tAnimate = 0;
                    AnimateTimer.Enabled = false;
                    SVTimer.Enabled = true;
                    return;
                }
            }

            if (tAnimate == 0)//hide
            {
                if (iAlpha > 0)
                {
                    if (iAlpha - 7 < 0)
                        Brush.Color = Color.FromArgb(iAlpha = 0, cTextColor);
                    else
                        Brush.Color = Color.FromArgb(iAlpha -= 7, cTextColor);

                    this.Invoke(LabelRefreshT);

                    return;
                }
                else
                {
                    tAnimate = -1;
                    AnimateTimer.Enabled = false;
                    return;
                }
            }
        }

        public Color NotesNameColor
        {
            get { return cTextColor; }
            set { cTextColor = value; Brush.Color = Color.FromArgb(iAlpha, cTextColor); this.Refresh(); }
        }

        public int StayVisibleTime
        {
            get { return StayVisibleTimeInMs; }
            set { StayVisibleTimeInMs = value; }
        }

        public void Show(string sText)
        {
            AnimateTimer.Enabled = false;
            SVTimer.Enabled = false;
            this.Refresh();
            iAlpha = 0;
            Brush.Color = Color.FromArgb(iAlpha, cTextColor);
            Text = Text;
            tAnimate = 1;
            AnimateTimer.Enabled = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (DesignMode)
            {
                e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                Brush.Color = cTextColor;

                e.Graphics.DrawString(Text, new Font("Segoe UI", 14.0f, FontStyle.Regular), Brush, 0, 0);
            }

            //if (!bShow)
            //{
            //    return;
            //}

            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            e.Graphics.DrawString(Text, new Font("Segoe UI", 14.0f, FontStyle.Regular), Brush, 0, 0);
        }
    }





    public class InfiniumProgressBar : Control
    {
        SolidBrush Brush;
        SolidBrush FontBrush;
        int iValue = 0;

        Rectangle Rect;

        Font fValueFont = new Font("SEGOE UI", 18, FontStyle.Regular, GraphicsUnit.Pixel);

        Color C100 = Color.FromArgb(64, 189, 44);
        Color C90 = Color.FromArgb(81, 207, 60);
        Color C80 = Color.FromArgb(255, 242, 0);
        Color C70 = Color.FromArgb(255, 211, 0);
        Color C60 = Color.FromArgb(255, 173, 0);
        Color C50 = Color.FromArgb(255, 133, 0);
        Color C40 = Color.FromArgb(255, 101, 0);
        Color C30 = Color.FromArgb(255, 80, 0);
        Color C20 = Color.FromArgb(255, 52, 0);
        Color C10 = Color.FromArgb(255, 0, 0);

        public InfiniumProgressBar()
        {
            Brush = new SolidBrush(Color.FromArgb(56, 184, 238));
            FontBrush = new SolidBrush(Color.Black);

            Rect = new Rectangle(0, 0, this.Width, this.Height);
        }

        public int Value
        {
            get { return iValue; }
            set { iValue = value; this.Refresh(); }
        }

        private void SetBrush()
        {
            if (iValue == 100)
                Brush.Color = C100;
            if (iValue >= 90 && iValue <= 99)
                Brush.Color = C90;
            if (iValue >= 80 && iValue <= 89)
                Brush.Color = C80;
            if (iValue >= 70 && iValue <= 79)
                Brush.Color = C70;
            if (iValue >= 60 && iValue <= 69)
                Brush.Color = C60; ;
            if (iValue >= 50 && iValue <= 59)
                Brush.Color = C50;
            if (iValue >= 40 && iValue <= 49)
                Brush.Color = C40;
            if (iValue >= 30 && iValue <= 39)
                Brush.Color = C30;
            if (iValue >= 20 && iValue <= 29)
                Brush.Color = C20;
            if (iValue >= 0 && iValue <= 19)
                Brush.Color = C10;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            //SetBrush();

            Rect.Height = this.Height;
            Rect.Width = Convert.ToInt32(Convert.ToDecimal(this.Width) * Convert.ToDecimal(iValue) / 100);

            e.Graphics.FillRectangle(Brush, Rect);

            e.Graphics.DrawString(iValue.ToString() + "%", fValueFont, FontBrush, (this.Width - e.Graphics.MeasureString(iValue.ToString() + "%", fValueFont).Width) / 2, (this.Height - e.Graphics.MeasureString(iValue.ToString() + "%", fValueFont).Height) / 2);
        }
    }





    public class InfiniumTimeLabel : Control
    {
        Color cLineColor;

        //Font fTextFont;

        SolidBrush brFontBrush;
        SolidBrush brLineBrush;

        public InfiniumTimeLabel()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            cLineColor = Color.Black;

            brFontBrush = new SolidBrush(ForeColor);
            brLineBrush = new SolidBrush(cLineColor);
            //fTextFont = new Font("Segoe UI", 22.0f, FontStyle.Regular, GraphicsUnit.Pixel);
        }

        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
            set
            {
                base.ForeColor = value; brFontBrush.Color = value; this.Refresh();
            }
        }

        public Color LineColor
        {
            get { return cLineColor; }
            set { cLineColor = value; brLineBrush.Color = value; this.Refresh(); }
        }

        public override Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                base.Font = value; this.Refresh();
            }
        }

        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value; this.Refresh();
            }
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;

            int H = Convert.ToInt32(e.Graphics.MeasureString(this.Text, Font).Height);
            int W = Convert.ToInt32(e.Graphics.MeasureString(this.Text, Font).Width);

            e.Graphics.DrawString(this.Text, Font, brFontBrush, this.Width - W,
                                                                (this.Height - H) / 2);

            for (int i = 0; i < (this.Width - W); i += 6)
            {
                e.Graphics.FillEllipse(brLineBrush, i, (this.Height - H) / 2 + H - 9, 1, 2);
            }
        }
    }




    public class InfiniumTimeTextBox : KryptonTextBox
    {
        public int MaximumValue = 59;

        public InfiniumTimeTextBox()
        {
            //base.AutoSize = false;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            this.TextBox.MouseMove += new MouseEventHandler(TextBox_MouseMove);
            this.TextBox.MouseLeave += new EventHandler(TextBox_MouseLeave);
            this.TextBox.MouseClick += new MouseEventHandler(TextBox_MouseClick);
        }

        protected override void OnTextChanged(EventArgs e)
        {
            if (ReadOnly)
                return;

            base.OnTextChanged(e);

            bool res = Int32.TryParse(this.Text, out int val);
            if (res == true && val > -1 && val <= MaximumValue || this.Text == "00")
            {

            }
            else
            {
                this.Text = "00";
                return;
            }
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);

            if (this.Text.Length < 2)
                this.Text = "0" + this.Text;


        }

        private void TextBox_MouseMove(object sender, MouseEventArgs e)
        {
            this.OnMouseMove(e);
        }

        private void TextBox_MouseLeave(object sender, EventArgs e)
        {
            this.OnMouseLeave(e);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (ReadOnly)
                return;

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (ReadOnly)
                return;

            base.OnMouseClick(e);


        }

        private void TextBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (ReadOnly)
                return;

            this.SelectionStart = 0;
            this.SelectionLength = this.Text.Length;

            OnMouseClick(e);
        }
    }





    public class InfiniumTimePicker : Control
    {
        SolidBrush brTextFontBrush;
        SolidBrush brLabelsFontBrush;

        Font fLabelsFont;

        private bool bReadOnly = false;

        Bitmap bmpTimeUpCommon = Properties.Resources.TimeUpCommon;
        Bitmap bmpTimeDownCommon = Properties.Resources.TimeDownCommon;
        Bitmap bmpTimeUpPressed = Properties.Resources.TimeUpPressed;
        Bitmap bmpTimeDownPressed = Properties.Resources.TimeDownPressed;
        Bitmap bmpTimeUpTracking = Properties.Resources.TimeUpTracking;
        Bitmap bmpTimeDownTracking = Properties.Resources.TimeDownTracking;

        InfiniumTimeTextBox MinutesTextBox;
        InfiniumTimeTextBox HoursTextBox;

        int iHours = 0;
        int iMinutes = 0;

        bool bTracking;
        bool bHrsUpBtnTracking = false;
        bool bHrsDnBtnTracking = false;
        bool bMinUpBtnTracking = false;
        bool bMinDnBtnTracking = false;

        public InfiniumTimePicker()
        {
            SetStyle(ControlStyles.ContainerControl, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            //SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            brTextFontBrush = new SolidBrush(ForeColor);
            brLabelsFontBrush = new SolidBrush(Color.Gray);

            fLabelsFont = new System.Drawing.Font("Segoe UI", 12, FontStyle.Regular, GraphicsUnit.Pixel);

            CreateTextBoxes();
        }

        private void CreateTextBoxes()
        {
            if (MinutesTextBox != null)
                MinutesTextBox.Dispose();

            HoursTextBox = new InfiniumTimeTextBox()
            {
                Parent = this
            };
            HoursTextBox.Show();
            HoursTextBox.StateCommon.Content.Font = this.Font;
            HoursTextBox.StateCommon.Content.Color1 = this.ForeColor;
            HoursTextBox.StateCommon.Border.Color1 = Color.Transparent;
            HoursTextBox.StateCommon.Content.Padding = new System.Windows.Forms.Padding(-1, -3, -1, -1);
            HoursTextBox.Text = "00";
            HoursTextBox.TextAlign = HorizontalAlignment.Right;
            HoursTextBox.Width = Convert.ToInt32(this.CreateGraphics().MeasureString("00", Font).Width) - 3;
            HoursTextBox.Height = Convert.ToInt32(this.CreateGraphics().MeasureString(HoursTextBox.Text, Font).Height);

            HoursTextBox.Left = (this.Width - HoursTextBox.Width - 4) - HoursTextBox.Width - 10;
            HoursTextBox.Top = (this.Height - HoursTextBox.Height) / 2;
            HoursTextBox.MouseMove += new MouseEventHandler(TextBoxes_MouseMove);
            HoursTextBox.TextChanged += new EventHandler(HoursTextBox_TextChanged);
            HoursTextBox.MouseLeave += new EventHandler(TextBox_MouseLeave);


            MinutesTextBox = new InfiniumTimeTextBox()
            {
                Parent = this
            };
            MinutesTextBox.Show();
            MinutesTextBox.StateCommon.Content.Font = this.Font;
            MinutesTextBox.StateCommon.Content.Color1 = this.ForeColor;
            MinutesTextBox.StateCommon.Border.Color1 = Color.Transparent;
            MinutesTextBox.StateCommon.Content.Padding = new System.Windows.Forms.Padding(-1, -3, -1, -1);
            MinutesTextBox.Text = "00";
            MinutesTextBox.TextAlign = HorizontalAlignment.Right;
            MinutesTextBox.Width = Convert.ToInt32(this.CreateGraphics().MeasureString(MinutesTextBox.Text, Font).Width) - 3;
            MinutesTextBox.Height = Convert.ToInt32(this.CreateGraphics().MeasureString(MinutesTextBox.Text, Font).Height) - 1;

            MinutesTextBox.Left = this.Width - MinutesTextBox.Width - 4;
            MinutesTextBox.Top = (this.Height - MinutesTextBox.Height) / 2;
            MinutesTextBox.MouseMove += new MouseEventHandler(TextBoxes_MouseMove);
            MinutesTextBox.TextChanged += new EventHandler(MinutesTextBox_TextChanged);
            MinutesTextBox.MouseLeave += new EventHandler(TextBox_MouseLeave);
        }

        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
            set
            {
                base.ForeColor = value; CreateTextBoxes(); brTextFontBrush.Color = value; this.Refresh();
            }
        }

        public override Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                if (Font == value)
                    return;

                base.Font = value; CreateTextBoxes(); this.Refresh();
            }
        }

        public bool ReadOnly
        {
            get { return bReadOnly; }
            set { bReadOnly = value; SetReadOnlyTextBoxes(value); this.Refresh(); }
        }

        private void SetReadOnlyTextBoxes(bool bRO)
        {
            HoursTextBox.ReadOnly = bRO;
            MinutesTextBox.ReadOnly = bRO;
        }

        public int Hours
        {
            get { return iHours; }
            set
            {
                iHours = value;
                if (value > 9)
                    HoursTextBox.Text = value.ToString();
                else
                    HoursTextBox.Text = "0" + value.ToString();
            }
        }

        public int Minutes
        {
            get { return iMinutes; }
            set
            {
                iMinutes = value;
                if (value > 9)
                    MinutesTextBox.Text = value.ToString();
                else
                    MinutesTextBox.Text = "0" + value.ToString();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);

            e.Graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            e.Graphics.DrawString(":", Font, brTextFontBrush, HoursTextBox.Left + HoursTextBox.Width,
                                              HoursTextBox.Top + ((HoursTextBox.Height - Convert.ToInt32(e.Graphics.MeasureString(":", Font).Height)) / 2) - 3);

            if (bTracking)
            {
                if (bHrsUpBtnTracking)
                    e.Graphics.DrawImage(bmpTimeUpTracking, HoursTextBox.Left + HoursTextBox.Width - bmpTimeUpCommon.Width, HoursTextBox.Top - bmpTimeUpCommon.Height + 3, bmpTimeUpCommon.Width, bmpTimeUpCommon.Height);
                else
                    e.Graphics.DrawImage(bmpTimeUpCommon, HoursTextBox.Left + HoursTextBox.Width - bmpTimeUpCommon.Width, HoursTextBox.Top - bmpTimeUpCommon.Height + 3, bmpTimeUpCommon.Width, bmpTimeUpCommon.Height);

                if (bHrsDnBtnTracking)
                    e.Graphics.DrawImage(bmpTimeDownTracking, HoursTextBox.Left + HoursTextBox.Width - bmpTimeUpCommon.Width, HoursTextBox.Top + HoursTextBox.Height - 4, bmpTimeUpCommon.Width, bmpTimeUpCommon.Height);
                else
                    e.Graphics.DrawImage(bmpTimeDownCommon, HoursTextBox.Left + HoursTextBox.Width - bmpTimeUpCommon.Width, HoursTextBox.Top + HoursTextBox.Height - 4, bmpTimeUpCommon.Width, bmpTimeUpCommon.Height);

                if (bMinUpBtnTracking)
                    e.Graphics.DrawImage(bmpTimeUpTracking, MinutesTextBox.Left + MinutesTextBox.Width - bmpTimeUpCommon.Width, MinutesTextBox.Top - bmpTimeUpCommon.Height + 3, bmpTimeUpCommon.Width, bmpTimeUpCommon.Height);
                else
                    e.Graphics.DrawImage(bmpTimeUpCommon, MinutesTextBox.Left + MinutesTextBox.Width - bmpTimeUpCommon.Width, MinutesTextBox.Top - bmpTimeUpCommon.Height + 3, bmpTimeUpCommon.Width, bmpTimeUpCommon.Height);

                if (bMinDnBtnTracking)
                    e.Graphics.DrawImage(bmpTimeDownTracking, MinutesTextBox.Left + MinutesTextBox.Width - bmpTimeUpCommon.Width, MinutesTextBox.Top + MinutesTextBox.Height - 4, bmpTimeUpCommon.Width, bmpTimeUpCommon.Height);
                else
                    e.Graphics.DrawImage(bmpTimeDownCommon, MinutesTextBox.Left + MinutesTextBox.Width - bmpTimeUpCommon.Width, MinutesTextBox.Top + MinutesTextBox.Height - 4, bmpTimeUpCommon.Width, bmpTimeUpCommon.Height);
            }
            else
            {
                e.Graphics.DrawString("чч", fLabelsFont, brLabelsFontBrush, HoursTextBox.Left + ((HoursTextBox.Width - Convert.ToInt32(e.Graphics.MeasureString("чч", fLabelsFont).Width)) / 2 + 4),
                                            HoursTextBox.Top + HoursTextBox.Height - 7);

                e.Graphics.DrawString("мм", fLabelsFont, brLabelsFontBrush, MinutesTextBox.Left + ((MinutesTextBox.Width - Convert.ToInt32(e.Graphics.MeasureString("чч", fLabelsFont).Width)) / 2 + 2),
                                            MinutesTextBox.Top + MinutesTextBox.Height - 7);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (bReadOnly)
                return;

            base.OnMouseMove(e);

            if (!bTracking)
            {
                bTracking = true;
                this.Refresh();
            }

            if (GetTrackingButtons(e.X, e.Y) == true)
                this.Refresh();
        }

        private bool GetTrackingButtons(int X, int Y)
        {
            //bHrsUpBtnTracking = false;
            //bHrsDnBtnTracking = false;
            //bMinUpBtnTracking = false;
            //bMinDnBtnTracking = false;

            //btnHours

            bool bR = false;

            if (X >= HoursTextBox.Left + HoursTextBox.Width - bmpTimeUpCommon.Width &&
                X <= HoursTextBox.Left + HoursTextBox.Width - bmpTimeUpCommon.Width + bmpTimeUpCommon.Width)
            {
                if (Y >= HoursTextBox.Top - bmpTimeUpCommon.Height + 3 &&
                   Y <= HoursTextBox.Top - bmpTimeUpCommon.Height + 3 + bmpTimeUpCommon.Height)
                {
                    if (!bHrsUpBtnTracking)
                    {
                        bHrsUpBtnTracking = true;

                        bR = true;
                    }
                }
                else
                {
                    if (bHrsUpBtnTracking)
                    {
                        bHrsUpBtnTracking = false;

                        bR = true;
                    }
                }

                if (Y >= HoursTextBox.Top + HoursTextBox.Height - 3 &&
                   Y <= HoursTextBox.Top + HoursTextBox.Height - 3 + bmpTimeUpCommon.Height)
                {
                    if (!bHrsDnBtnTracking)
                    {
                        bHrsDnBtnTracking = true;

                        bR = true;
                    }
                }
                else
                {
                    if (bHrsDnBtnTracking)
                    {
                        bHrsDnBtnTracking = false;

                        bR = true;
                    }
                }
            }
            else
            {
                if (bHrsUpBtnTracking)
                {
                    bHrsUpBtnTracking = false;

                    bR = true;
                }
                if (bHrsDnBtnTracking)
                {
                    bHrsDnBtnTracking = false;

                    bR = true;
                }
            }

            //btnMin
            if (X >= MinutesTextBox.Left + MinutesTextBox.Width - bmpTimeUpCommon.Width &&
                X <= MinutesTextBox.Left + MinutesTextBox.Width - bmpTimeUpCommon.Width + bmpTimeUpCommon.Width)
            {
                if (Y >= MinutesTextBox.Top - bmpTimeUpCommon.Height + 3 &&
                   Y <= MinutesTextBox.Top - bmpTimeUpCommon.Height + 3 + bmpTimeUpCommon.Height)
                {
                    if (!bMinUpBtnTracking)
                    {
                        bMinUpBtnTracking = true;

                        bR = true;
                    }
                }
                else
                {
                    if (bMinUpBtnTracking)
                    {
                        bMinUpBtnTracking = false;

                        bR = true;
                    }
                }

                if (Y >= MinutesTextBox.Top + MinutesTextBox.Height - 3 &&
                   Y <= MinutesTextBox.Top + MinutesTextBox.Height - 3 + bmpTimeUpCommon.Height)
                {
                    if (!bMinDnBtnTracking)
                    {
                        bMinDnBtnTracking = true;

                        bR = true;
                    }
                }
                else
                {
                    if (bMinDnBtnTracking)
                    {
                        bMinDnBtnTracking = false;

                        bR = true;
                    }
                }
            }
            else
            {
                if (bMinUpBtnTracking)
                {
                    bMinUpBtnTracking = false;

                    bR = true;
                }
                if (bMinDnBtnTracking)
                {
                    bMinDnBtnTracking = false;

                    bR = true;
                }
            }

            return bR;
        }

        private void TextBoxes_MouseMove(object sender, MouseEventArgs e)
        {
            if (bReadOnly)
                return;

            if (!bTracking)
            {
                bTracking = true;
                this.Refresh();
            }

            if (bHrsUpBtnTracking || bHrsDnBtnTracking || bMinUpBtnTracking || bMinDnBtnTracking)
            {
                bHrsUpBtnTracking = false;
                bHrsDnBtnTracking = false;
                bMinUpBtnTracking = false;
                bMinDnBtnTracking = false;
                this.Refresh();
            }

            GetTrackingButtons(e.X, e.Y);
        }

        private void TextBox_MouseLeave(object sender, EventArgs e)
        {
            if (bReadOnly)
                return;

            if (bTracking)
            {
                bTracking = false;
                this.Refresh();
            }
        }

        private void HoursTextBox_TextChanged(object sender, EventArgs e)
        {
            if (bReadOnly)
                return;

            if (HoursTextBox.Text != "")
                iHours = Convert.ToInt32(HoursTextBox.Text);
            else
                iHours = 0;
            OnTimeChange();
        }

        private void MinutesTextBox_TextChanged(object sender, EventArgs e)
        {
            if (bReadOnly)
                return;

            if (MinutesTextBox.Text != "")
                iMinutes = Convert.ToInt32(MinutesTextBox.Text);
            else
                iMinutes = 0;
            OnTimeChange();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (bTracking)
            {
                bTracking = false;
                this.Refresh();
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            MinutesTextBox.Left = this.Width - MinutesTextBox.Width - 4;
            MinutesTextBox.Top = (this.Height - MinutesTextBox.Height) / 2;

            HoursTextBox.Left = (this.Width - HoursTextBox.Width - 4) - HoursTextBox.Width - 10;
            HoursTextBox.Top = (this.Height - HoursTextBox.Height) / 2;
        }

        protected override void OnClick(EventArgs e)
        {
            if (bReadOnly)
                return;

            base.OnClick(e);

            if (bHrsUpBtnTracking)
                if (iHours < 24)
                {
                    iHours++;
                    if (iHours > 9)
                        HoursTextBox.Text = iHours.ToString();
                    else
                        HoursTextBox.Text = "0" + iHours.ToString();
                }

            if (bHrsDnBtnTracking)
                if (iHours > 0)
                {
                    iHours--;
                    if (iHours > 9)
                        HoursTextBox.Text = iHours.ToString();
                    else
                        HoursTextBox.Text = "0" + iHours.ToString();
                }


            if (bMinUpBtnTracking)
                if (iMinutes < 60)
                {
                    iMinutes++;
                    if (iMinutes > 9)
                        MinutesTextBox.Text = iMinutes.ToString();
                    else
                        MinutesTextBox.Text = "0" + iMinutes.ToString();
                }

            if (bMinDnBtnTracking)
                if (iMinutes > 0)
                {
                    iMinutes--;
                    if (iMinutes > 9)
                        MinutesTextBox.Text = iMinutes.ToString();
                    else
                        MinutesTextBox.Text = "0" + iMinutes.ToString();
                }
        }

        protected override void OnDoubleClick(EventArgs e)
        {
            if (bReadOnly)
                return;

            base.OnDoubleClick(e);

            OnClick(e);
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);

            if (bTracking)
            {
                bTracking = false;
                this.Refresh();
            }
        }

        public event TimeChangedEventHandler TimeChanged;

        public delegate void TimeChangedEventHandler(object sender, int Hours, int Minutes);

        public virtual void OnTimeChange()
        {
            if (bReadOnly)
                return;

            if (TimeChanged != null)
            {
                TimeChanged(this, iHours, iMinutes);//Raise the event
            }
        }
    }





    public class InfiniumFunctionsContainer : Control
    {
        public int Offset = 0;

        int iMarginNameRow = 5;
        int iMarginToNextItem = 10;
        int iMarginForImageWidth = 92;

        private int iScrollWidth = 10;
        private int iMarginForPercents = 100;
        private int iMarginForTimePicker = 100;
        private int iMarginForComplete = 100;
        private int iScrollWheelOffset = 50;
        private int iTempScrollWheelOffset = 0;

        private bool bReadOnly = false;

        int TotalY = 0;

        public int iSelected = -1;
        public int iCompleteSelected = -1;
        int iIncompleteX = -1;
        int iIncompleteW = -1;
        int iCompleteX = -1;
        int iCompleteW = -1;
        public int TotalMin = 0;
        public int TotalAllocMin = 0;

        bool bVertScrollVisible = false;

        Rectangle rVerticalScrollShaftRect;
        Rectangle rVerticalScrollThumbRect;
        Rectangle rSelectedRectangle;
        Rectangle rPercentsEllipseRectangle;
        Rectangle rPercentsPieRectangle;

        Color cCaptionFontColor;
        Color cDepartmentFontColor;
        Color cExecTypeFontColor;
        Color cVerticalScrollCommonShaftBackColor = Color.White;
        Color cVerticalScrollCommonThumbButtonColor = Color.Gray;
        Color cSelectedBackColor = Color.White;
        Color cTimePickerFontColor = Color.FromArgb(70, 70, 70);

        SolidBrush brCaptionFontBrush;
        SolidBrush brDepartmentFontBrush;
        SolidBrush brExecTypeFontBrush;
        SolidBrush brVerticalScrollCommonShaftBackBrush;
        SolidBrush brVerticalScrollCommonThumbButtonBrush;
        SolidBrush brSelectedBackBrush;
        SolidBrush brTimePickerBrush;
        SolidBrush brCompleteFontBrush;
        SolidBrush brIncompleteFontBrush;
        SolidBrush brNoDataFontBrush;

        Font fPositionFont;
        Font fCaptionFont;
        Font fDepartmentFont;
        Font fExecTypeFont;
        Font fTimePickerFont;
        Font fCompleteFont;
        Font fPercentsFont;
        Font fNoDataFont;

        Pen pSplitterPen;
        Pen pPercentEllipsePen;

        InfiniumTimePicker[] InfiniumTimePickers;

        public DataTable dtFunctionsDataTable;

        Bitmap bmpExecQuery = Properties.Resources.ExecQuery;
        Bitmap bmpExecEvent = Properties.Resources.ExecEvent;
        Bitmap bmpExecWeekly = Properties.Resources.ExecWeek;
        Bitmap bmpExecDaily = Properties.Resources.ExecDaily;
        Bitmap bmpTaskComplete = Properties.Resources.TaskComplete;

        public InfiniumFunctionsContainer()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            cCaptionFontColor = Color.Black;
            cDepartmentFontColor = Color.Gray;
            cExecTypeFontColor = Color.ForestGreen;

            brCaptionFontBrush = new SolidBrush(cCaptionFontColor);
            brDepartmentFontBrush = new SolidBrush(cDepartmentFontColor);
            brExecTypeFontBrush = new SolidBrush(cExecTypeFontColor);
            brVerticalScrollCommonShaftBackBrush = new SolidBrush(cVerticalScrollCommonShaftBackColor);
            brVerticalScrollCommonThumbButtonBrush = new SolidBrush(cVerticalScrollCommonThumbButtonColor);
            brSelectedBackBrush = new SolidBrush(cSelectedBackColor);
            brTimePickerBrush = new SolidBrush(cTimePickerFontColor);
            brCompleteFontBrush = new SolidBrush(Color.DarkGray);
            brIncompleteFontBrush = new SolidBrush(Color.FromArgb(56, 184, 238));
            brNoDataFontBrush = new SolidBrush(Color.Gray);

            pSplitterPen = new Pen(new SolidBrush(Color.WhiteSmoke));
            pPercentEllipsePen = new Pen(new SolidBrush(Color.Gray), 1.7f);

            fPositionFont = new Font("Segoe UI", 18, FontStyle.Bold, GraphicsUnit.Pixel);
            fCaptionFont = new Font("Segoe UI", 18, FontStyle.Regular, GraphicsUnit.Pixel);
            fDepartmentFont = new Font("Segoe UI", 16, FontStyle.Regular, GraphicsUnit.Pixel);
            fExecTypeFont = new Font("Segoe UI", 16, FontStyle.Regular, GraphicsUnit.Pixel);
            fTimePickerFont = new System.Drawing.Font("Segoe UI", 24, FontStyle.Regular, GraphicsUnit.Pixel);
            fCompleteFont = new System.Drawing.Font("Segoe UI", 14, FontStyle.Regular, GraphicsUnit.Pixel);
            fPercentsFont = new System.Drawing.Font("Segoe UI", 11, FontStyle.Regular, GraphicsUnit.Pixel);
            fNoDataFont = new System.Drawing.Font("Segoe UI", 19, FontStyle.Regular, GraphicsUnit.Pixel);

            rVerticalScrollShaftRect = new Rectangle(this.Width - iScrollWidth, 0, iScrollWidth, this.Height);
            rVerticalScrollThumbRect = new Rectangle(this.Width - iScrollWidth, 0, iScrollWidth, this.Height);
            rSelectedRectangle = new Rectangle(0, 0, this.Width, 10);
            rPercentsEllipseRectangle = new Rectangle(0, 0, 45, 45);
            rPercentsPieRectangle = new Rectangle(0, 0, 45, 45);
        }

        public Font TimePickersFont
        {
            get { return fTimePickerFont; }
            set { fTimePickerFont = value; this.Refresh(); }
        }

        public DataTable FunctionsDataTable
        {
            get { return dtFunctionsDataTable; }
            set
            {
                dtFunctionsDataTable = value;

                if (value != null)
                {
                    if (dtFunctionsDataTable.Columns["Position"] == null)
                        dtFunctionsDataTable.Columns.Add(new DataColumn("Position", Type.GetType("System.Int32")));
                    if (dtFunctionsDataTable.Columns["InCompletePositionY1"] == null)
                        dtFunctionsDataTable.Columns.Add(new DataColumn("InCompletePositionY1", Type.GetType("System.Int32")));
                    if (dtFunctionsDataTable.Columns["InCompletePositionY2"] == null)
                        dtFunctionsDataTable.Columns.Add(new DataColumn("InCompletePositionY2", Type.GetType("System.Int32")));
                    if (dtFunctionsDataTable.Columns["CompletePositionY1"] == null)
                        dtFunctionsDataTable.Columns.Add(new DataColumn("CompletePositionY1", Type.GetType("System.Int32")));
                    if (dtFunctionsDataTable.Columns["CompletePositionY2"] == null)
                        dtFunctionsDataTable.Columns.Add(new DataColumn("CompletePositionY2", Type.GetType("System.Int32")));

                    CreateTimePickers();
                }

                this.Refresh();
            }
        }

        private int GetLastSpace(string Text)
        {
            int LastSpace = 0;

            for (int i = Text.Length - 1; i >= 0; i--)
            {
                if (Text[i] == ' ')
                {
                    LastSpace = i;
                    break;
                }
            }

            if (LastSpace == 0)//no spaces was found
            {
                for (int i = Text.Length - 1; i >= 0; i--)
                {
                    if (Text[i] == '.' || Text[i] == ',' || Text[i] == ';' || Text[i] == ':')
                    {
                        LastSpace = i;
                        break;
                    }
                }
            }

            return LastSpace;
        }

        public bool ReadOnly
        {
            get { return bReadOnly; }
            set { bReadOnly = value; SetReadOnlyPickers(value); this.Refresh(); }
        }

        private void SetReadOnlyPickers(bool bRO)
        {
            if (InfiniumTimePickers == null)
                return;

            foreach (InfiniumTimePicker Picker in InfiniumTimePickers)
            {
                Picker.ReadOnly = bRO;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            if (dtFunctionsDataTable == null)
            {
                e.Graphics.DrawString("Начните рабочий день для получения списка обязанностей и задач",
                                      fNoDataFont, brNoDataFontBrush,
                                      (this.Width - e.Graphics.MeasureString("Начните рабочий день для получения списка обязанностей и задач",
                                                                            fNoDataFont).Width) / 2,
                                      (this.Height - e.Graphics.MeasureString("Начните рабочий день для получения списка обязанностей и задач",
                                                                            fNoDataFont).Height) / 2);
                return;
            }

            if (dtFunctionsDataTable.Rows.Count == 0)
            {
                e.Graphics.DrawString("Начните рабочий день для получения списка обязанностей и задач",
                                      fNoDataFont, brNoDataFontBrush,
                                      (this.Width - e.Graphics.MeasureString("Начните рабочий день для получения списка обязанностей и задач",
                                                                            fNoDataFont).Width) / 2,
                                      (this.Height - e.Graphics.MeasureString("Начните рабочий день для получения списка обязанностей и задач",
                                                                            fNoDataFont).Height) / 2);

                return;
            }


            int CurTextPosY = 0;

            for (int i = 0; i < dtFunctionsDataTable.Rows.Count; i++)
            {
                int iNameTextY = 0;

                if (i == iSelected)
                    if (dtFunctionsDataTable.DefaultView[iSelected]["Position"] != DBNull.Value)
                        DrawBack(i, e.Graphics, ref CurTextPosY);

                DrawPriority(i, Convert.ToInt32(dtFunctionsDataTable.DefaultView[i]["FunctionExecTypeID"]), e.Graphics, ref CurTextPosY);
                //DrawPosition(i, dtFunctionsDataTable.DefaultView[i]["DepartmentName"].ToString(), e.Graphics, ref CurTextPosY);
                iNameTextY = DrawName(i, dtFunctionsDataTable.DefaultView[i]["FunctionName"].ToString(), e.Graphics, ref CurTextPosY);
                //DrawSplitter(e.Graphics, ref CurTextPosY);
                DrawDepartment(i, dtFunctionsDataTable.DefaultView[i]["DepartmentName"].ToString(), e.Graphics, ref CurTextPosY);
                DrawExecType(i, dtFunctionsDataTable.DefaultView[i]["ExecType"].ToString(), e.Graphics, ref CurTextPosY);

                DrawSplitter(e.Graphics, ref CurTextPosY);
                DrawPriorityLine(e.Graphics);
                DrawTimeLine(e.Graphics);
                DrawCompleteLine(e.Graphics);
                DrawPercentsLine(e.Graphics);

                dtFunctionsDataTable.Rows[i]["Position"] = CurTextPosY;

                DrawTimePicker(i, e.Graphics, CurTextPosY);
                DrawComplete(i, e.Graphics, Convert.ToBoolean(dtFunctionsDataTable.Rows[i]["IsComplete"]));
                DrawPercents(i, e.Graphics);
            }

            TotalY = CurTextPosY;

            DrawVerticalScrollShaft(e.Graphics);
            DrawVertScrollThumb(e.Graphics);
        }



        private void CreateTimePickers()
        {
            if (InfiniumTimePickers == null)
            {
                InfiniumTimePickers = new InfiniumTimePicker[FunctionsDataTable.Rows.Count];

                for (int i = 0; i < FunctionsDataTable.Rows.Count; i++)
                {
                    InfiniumTimePickers[i] = new InfiniumTimePicker()
                    {
                        Name = i.ToString(),
                        Parent = this,
                        Font = fTimePickerFont,
                        ForeColor = cTimePickerFontColor,
                        Hours = Convert.ToInt32(FunctionsDataTable.Rows[i]["Hours"]),
                        Minutes = Convert.ToInt32(FunctionsDataTable.Rows[i]["Minutes"])
                    };
                    InfiniumTimePickers[i].TimeChanged += new InfiniumTimePicker.TimeChangedEventHandler(TimeChange);
                }
            }
            else
            {
                for (int i = 0; i < InfiniumTimePickers.Count(); i++)
                {
                    if (InfiniumTimePickers[i] != null)
                    {
                        InfiniumTimePickers[i].Dispose();
                        InfiniumTimePickers[i] = null;
                    }
                }

                InfiniumTimePickers = new InfiniumTimePicker[FunctionsDataTable.Rows.Count];

                for (int i = 0; i < FunctionsDataTable.Rows.Count; i++)
                {
                    InfiniumTimePickers[i] = new InfiniumTimePicker()
                    {
                        Name = i.ToString(),
                        Parent = this,
                        Font = fTimePickerFont,
                        ForeColor = cTimePickerFontColor,
                        Hours = Convert.ToInt32(FunctionsDataTable.Rows[i]["Hours"]),
                        Minutes = Convert.ToInt32(FunctionsDataTable.Rows[i]["Minutes"])
                    };
                    InfiniumTimePickers[i].TimeChanged += new InfiniumTimePicker.TimeChangedEventHandler(TimeChange);
                }
            }
        }

        private void DrawComplete(int index, Graphics G, bool IsComplete)
        {
            G.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            int sCompleteHeight = Convert.ToInt32(G.MeasureString("завершено", fCompleteFont).Height);
            int sCompleteWidth = Convert.ToInt32(G.MeasureString("завершено", fCompleteFont).Width);
            int sinCompleteHeight = Convert.ToInt32(G.MeasureString("завершить", fCompleteFont).Height);
            int sinCompleteWidth = Convert.ToInt32(G.MeasureString("завершить", fCompleteFont).Width);

            if (!IsComplete)
            {
                if (index > 0)
                {
                    iIncompleteX = this.Width - iMarginForComplete - rVerticalScrollShaftRect.Width + ((iMarginForComplete - sinCompleteWidth) / 2);
                    iIncompleteW = sinCompleteWidth;
                    dtFunctionsDataTable.Rows[index]["InCompletePositionY1"] = ((Convert.ToInt32(FunctionsDataTable.Rows[index]["Position"]) -
                                                       Convert.ToInt32(FunctionsDataTable.Rows[index - 1]["Position"])) -
                                                       sinCompleteHeight) / 2 + (Convert.ToInt32(FunctionsDataTable.Rows[index - 1]["Position"])) + iMarginToNextItem - Offset;
                    dtFunctionsDataTable.Rows[index]["InCompletePositionY2"] = Convert.ToInt32(dtFunctionsDataTable.Rows[index]["InCompletePositionY1"]) + sinCompleteHeight;

                    G.DrawString("завершить", fCompleteFont, brIncompleteFontBrush,
                            iIncompleteX, Convert.ToInt32(dtFunctionsDataTable.Rows[index]["InCompletePositionY1"]));
                }
                else
                {
                    iIncompleteX = this.Width - iMarginForComplete - rVerticalScrollShaftRect.Width + ((iMarginForComplete - sinCompleteWidth) / 2);
                    iIncompleteW = sinCompleteWidth;

                    dtFunctionsDataTable.Rows[index]["InCompletePositionY1"] = (Convert.ToInt32(FunctionsDataTable.Rows[index]["Position"]) + iMarginToNextItem -
                                                        sinCompleteHeight) / 2 - Offset;
                    dtFunctionsDataTable.Rows[index]["InCompletePositionY2"] = Convert.ToInt32(dtFunctionsDataTable.Rows[index]["InCompletePositionY1"]) + sinCompleteHeight;

                    G.DrawString("завершить", fCompleteFont, brIncompleteFontBrush, this.Width - iMarginForComplete - rVerticalScrollShaftRect.Width + ((iMarginForComplete - sinCompleteWidth) / 2),
                            Convert.ToInt32(dtFunctionsDataTable.Rows[index]["InCompletePositionY1"]));
                }
            }
            else
            {
                if (index > 0)
                {
                    iCompleteX = this.Width - iMarginForComplete - rVerticalScrollShaftRect.Width + ((iMarginForComplete - sCompleteWidth) / 2);
                    iCompleteW = sCompleteWidth;

                    G.DrawImage(bmpTaskComplete, this.Width - iMarginForComplete - rVerticalScrollShaftRect.Width + ((iMarginForComplete - 32) / 2 - 2),
                                                    ((Convert.ToInt32(FunctionsDataTable.Rows[index]["Position"]) -
                                                       Convert.ToInt32(FunctionsDataTable.Rows[index - 1]["Position"])) -
                                                       32 + 5 + sCompleteHeight) / 2 + (Convert.ToInt32(FunctionsDataTable.Rows[index - 1]["Position"])) + iMarginToNextItem - Offset - sCompleteHeight - 5, 32, 32);

                    G.DrawString("завершено", fCompleteFont, brCompleteFontBrush, this.Width - iMarginForComplete - rVerticalScrollShaftRect.Width + ((iMarginForComplete - sCompleteWidth) / 2),
                          ((Convert.ToInt32(FunctionsDataTable.Rows[index]["Position"]) -
                                                       Convert.ToInt32(FunctionsDataTable.Rows[index - 1]["Position"])) -
                                                       32 + 5 + sCompleteHeight) / 2 + (Convert.ToInt32(FunctionsDataTable.Rows[index - 1]["Position"])) + iMarginToNextItem - Offset - sCompleteHeight + 32);

                    dtFunctionsDataTable.Rows[index]["CompletePositionY1"] = ((Convert.ToInt32(FunctionsDataTable.Rows[index]["Position"]) -
                                                       Convert.ToInt32(FunctionsDataTable.Rows[index - 1]["Position"])) -
                                                       32 + 5 + sCompleteHeight) / 2 + (Convert.ToInt32(FunctionsDataTable.Rows[index - 1]["Position"])) + iMarginToNextItem - Offset - sCompleteHeight + 32;
                    dtFunctionsDataTable.Rows[index]["CompletePositionY2"] = Convert.ToInt32(dtFunctionsDataTable.Rows[index]["CompletePositionY1"]) + sCompleteHeight;
                }
                else
                {
                    iCompleteX = this.Width - iMarginForComplete - rVerticalScrollShaftRect.Width + ((iMarginForComplete - sCompleteWidth) / 2);
                    iCompleteW = sCompleteWidth;

                    G.DrawImage(bmpTaskComplete, this.Width - iMarginForComplete - rVerticalScrollShaftRect.Width + ((iMarginForComplete - 32) / 2 - 2),
                                                    (Convert.ToInt32(FunctionsDataTable.Rows[index]["Position"]) -
                                                       32 + 5 + sCompleteHeight + iMarginToNextItem) / 2 - Offset - sCompleteHeight - 5, 32, 32);

                    G.DrawString("завершено", fCompleteFont, brCompleteFontBrush, this.Width - iMarginForComplete - rVerticalScrollShaftRect.Width + ((iMarginForComplete - sCompleteWidth) / 2),
                          (Convert.ToInt32(FunctionsDataTable.Rows[index]["Position"]) -
                                                       32 + 5 + sCompleteHeight + iMarginToNextItem) / 2 - Offset - sCompleteHeight + 32);

                    dtFunctionsDataTable.Rows[index]["CompletePositionY1"] = (Convert.ToInt32(FunctionsDataTable.Rows[index]["Position"]) -
                                                       32 + 5 + sCompleteHeight + iMarginToNextItem) / 2 - Offset - sCompleteHeight + 32;
                    dtFunctionsDataTable.Rows[index]["CompletePositionY2"] = Convert.ToInt32(dtFunctionsDataTable.Rows[index]["CompletePositionY1"]) + sCompleteHeight;
                }
            }
        }

        private void DrawTimePicker(int index, Graphics G, int CurTextPosY)
        {
            InfiniumTimePickers[index].Font = fTimePickerFont;
            InfiniumTimePickers[index].Width = 72;
            InfiniumTimePickers[index].Height = 52;
            InfiniumTimePickers[index].Left = this.Width - iMarginForTimePicker - rVerticalScrollShaftRect.Width - iMarginForComplete + ((iMarginForTimePicker - InfiniumTimePickers[index].Width) / 2);
            if (index == 0)
                InfiniumTimePickers[index].Top = (Convert.ToInt32(FunctionsDataTable.Rows[index]["Position"]) + iMarginToNextItem -
                                                        InfiniumTimePickers[index].Height) / 2 - Offset;
            else
            {
                InfiniumTimePickers[index].Top = ((Convert.ToInt32(FunctionsDataTable.Rows[index]["Position"]) -
                                                   Convert.ToInt32(FunctionsDataTable.Rows[index - 1]["Position"])) -
                                                   InfiniumTimePickers[index].Height) / 2 + (Convert.ToInt32(FunctionsDataTable.Rows[index - 1]["Position"])) + iMarginToNextItem - Offset;
            }
        }

        private void DrawPercents(int index, Graphics G)
        {
            if (index == 0)
            {
                TotalAllocMin = 0;
            }

            rPercentsEllipseRectangle.X = this.Width - iMarginForComplete - iMarginForPercents - iMarginForTimePicker - rVerticalScrollShaftRect.Width + ((iMarginForPercents - rPercentsEllipseRectangle.Width) / 2);

            if (index > 0)
                rPercentsEllipseRectangle.Y = ((Convert.ToInt32(FunctionsDataTable.Rows[index]["Position"]) -
                                                       Convert.ToInt32(FunctionsDataTable.Rows[index - 1]["Position"])) -
                                                       rPercentsEllipseRectangle.Height) / 2 + (Convert.ToInt32(FunctionsDataTable.Rows[index - 1]["Position"])) + iMarginToNextItem - Offset;
            else
                rPercentsEllipseRectangle.Y = (Convert.ToInt32(FunctionsDataTable.Rows[index]["Position"]) + iMarginToNextItem -
                                                        rPercentsEllipseRectangle.Height) / 2 - Offset;

            rPercentsPieRectangle.X = rPercentsEllipseRectangle.X;
            rPercentsPieRectangle.Y = rPercentsEllipseRectangle.Y;


            G.DrawEllipse(pPercentEllipsePen, rPercentsEllipseRectangle);

            if (TotalMin == 0)
                return;

            if (dtFunctionsDataTable.Rows[index]["Hours"] == DBNull.Value ||
                dtFunctionsDataTable.Rows[index]["Minutes"] == DBNull.Value)
                return;

            if (Convert.ToInt32(dtFunctionsDataTable.Rows[index]["Hours"]) == 0 &&
               Convert.ToInt32(dtFunctionsDataTable.Rows[index]["Minutes"]) == 0)
                return;

            int P = 100 * ((Convert.ToInt32(dtFunctionsDataTable.Rows[index]["Minutes"]) +
                         Convert.ToInt32(dtFunctionsDataTable.Rows[index]["Hours"]) * 60)) / TotalMin;

            TotalAllocMin += Convert.ToInt32(dtFunctionsDataTable.Rows[index]["Minutes"]) +
                         Convert.ToInt32(dtFunctionsDataTable.Rows[index]["Hours"]) * 60;

            G.FillPie(brIncompleteFontBrush, rPercentsPieRectangle, -90, P * 360 / 100);

            G.DrawString(P.ToString() + "%", fPercentsFont, brIncompleteFontBrush,
                         this.Width - iMarginForComplete - iMarginForPercents - iMarginForTimePicker -
                         rVerticalScrollShaftRect.Width + ((iMarginForPercents - Convert.ToInt32(G.MeasureString(P.ToString() + "%", fPercentsFont).Width)) / 2) + 2,
                         rPercentsEllipseRectangle.Y + rPercentsEllipseRectangle.Height + 0);
        }

        private void DrawPriority(int index, int ExecType, Graphics G, ref int CurTextPosY)
        {
            int MTN = 0;

            if (index > 0)
                MTN = iMarginToNextItem;

            Bitmap Image = null;

            if (ExecType == 1)
                Image = bmpExecDaily;
            if (ExecType == 2)
                Image = bmpExecWeekly;
            if (ExecType == 4)
                Image = bmpExecQuery;
            if (ExecType == 5)
                Image = bmpExecEvent;


            if (index > 0)
                G.DrawImage(Image, 9, CurTextPosY - Offset + iMarginToNextItem + 4, 68, 68);
            else
                G.DrawImage(Image, 9, CurTextPosY - Offset + 4, 68, 68);
        }

        private void DrawBack(int index, Graphics G, ref int CurTextPosY)
        {
            if (index > 0)
            {
                rSelectedRectangle.Y = CurTextPosY + 9 - Offset;
                rSelectedRectangle.Height = Convert.ToInt32(dtFunctionsDataTable.Rows[iSelected]["Position"]) - CurTextPosY;
            }
            else
            {
                rSelectedRectangle.Y = CurTextPosY - Offset;
                rSelectedRectangle.Height = Convert.ToInt32(dtFunctionsDataTable.Rows[iSelected]["Position"]) - CurTextPosY + 9;
            }

            rSelectedRectangle.Width = this.Width - rVerticalScrollShaftRect.Width;

            G.FillRectangle(brSelectedBackBrush, rSelectedRectangle);
        }

        private int DrawName(int index, string Text, Graphics G, ref int CurTextPosY)
        {
            int TextMaxWidth = this.Width - iMarginForImageWidth - rVerticalScrollShaftRect.Width - iMarginForTimePicker - iMarginForPercents - iMarginForComplete;

            int MTN = 0;

            if (index > 0)
                MTN = iMarginToNextItem;

            int CurrentY = 0;

            int MHR = 0;

            string CurrentRowString = "";

            for (int i = 0; i < Text.Length; i++)
            {
                if (Text[i] == '\n')
                {
                    if (CurrentY > 0)
                        MHR = iMarginNameRow;

                    G.DrawString(CurrentRowString, fCaptionFont, brCaptionFontBrush, iMarginForImageWidth, CurTextPosY + MTN +
                                (CurrentY * (G.MeasureString("String", fCaptionFont).Height - MHR)) - Offset);
                    CurrentRowString = "";
                    CurrentY++;
                    continue;
                }

                if (i == Text.Length - 1)
                {
                    if (CurrentY > 0)
                        MHR = iMarginNameRow;

                    G.DrawString(CurrentRowString += Text[i], fCaptionFont, brCaptionFontBrush, iMarginForImageWidth, CurTextPosY + MTN +
                                (CurrentY * (G.MeasureString("String", fCaptionFont).Height - MHR)) - Offset);
                    CurrentY++;
                    break;
                }

                if (G.MeasureString(CurrentRowString, fCaptionFont).Width > TextMaxWidth)
                {
                    if (CurrentY > 0)
                        MHR = iMarginNameRow;

                    int LastSpace = GetLastSpace(CurrentRowString);

                    if (LastSpace == 0)
                    {
                        G.DrawString(CurrentRowString, fCaptionFont, brCaptionFontBrush, iMarginForImageWidth, CurTextPosY + MTN +
                                    (CurrentY * (G.MeasureString("string", fCaptionFont).Height - MHR)) - Offset);
                    }
                    else
                    {
                        G.DrawString(CurrentRowString.Substring(0, LastSpace + 1), fCaptionFont, brCaptionFontBrush, iMarginForImageWidth, CurTextPosY + MTN +
                                    (CurrentY * (G.MeasureString("string", fCaptionFont).Height) - MHR) - Offset);

                        i -= (CurrentRowString.Length - LastSpace);
                    }


                    CurrentRowString = "";
                    CurrentY++;
                    continue;
                }

                CurrentRowString += Text[i];
            }

            int C = CurrentY * Convert.ToInt32((G.MeasureString("String", fCaptionFont).Height) + iMarginNameRow) + MTN;

            CurTextPosY += C - MHR * 2;

            return C - MTN - MHR;
        }

        private void DrawDepartment(int index, string DepartmentName, Graphics G, ref int CurTextPosY)
        {
            int TextMaxWidth = this.Width - iMarginForImageWidth;

            int C = Convert.ToInt32(G.MeasureString(DepartmentName, fDepartmentFont).Height);

            G.DrawString(DepartmentName, fDepartmentFont, brDepartmentFontBrush, iMarginForImageWidth, CurTextPosY - Offset - 7);

            CurTextPosY += C - 5;
        }

        private void DrawPosition(int index, string Position, Graphics G, ref int CurTextPosY)
        {
            int TextMaxWidth = this.Width - iMarginForImageWidth - rVerticalScrollShaftRect.Width - iMarginForTimePicker - iMarginForPercents - iMarginForComplete;

            int MTN = 0;

            if (index > 0)
                MTN = iMarginToNextItem;

            int CurrentY = 0;

            int MHR = 0;

            string CurrentRowString = "";

            for (int i = 0; i < Position.Length; i++)
            {
                if (Position[i] == '\n')
                {
                    if (CurrentY > 0)
                        MHR = iMarginNameRow;

                    G.DrawString(CurrentRowString, fPositionFont, brCaptionFontBrush, iMarginForImageWidth, CurTextPosY + MTN +
                                (CurrentY * (G.MeasureString("String", fPositionFont).Height - MHR)) - Offset);
                    CurrentRowString = "";
                    CurrentY++;
                    continue;
                }

                if (i == Position.Length - 1)
                {
                    if (CurrentY > 0)
                        MHR = iMarginNameRow;

                    G.DrawString(CurrentRowString += Position[i], fPositionFont, brCaptionFontBrush, iMarginForImageWidth, CurTextPosY + MTN +
                                (CurrentY * (G.MeasureString("String", fPositionFont).Height - MHR)) - Offset);
                    CurrentY++;
                    break;
                }

                if (G.MeasureString(CurrentRowString, fPositionFont).Width > TextMaxWidth)
                {
                    if (CurrentY > 0)
                        MHR = iMarginNameRow;

                    int LastSpace = GetLastSpace(CurrentRowString);

                    if (LastSpace == 0)
                    {
                        G.DrawString(CurrentRowString, fPositionFont, brCaptionFontBrush, iMarginForImageWidth, CurTextPosY + MTN +
                                    (CurrentY * (G.MeasureString("string", fPositionFont).Height - MHR)) - Offset);
                    }
                    else
                    {
                        G.DrawString(CurrentRowString.Substring(0, LastSpace + 1), fPositionFont, brCaptionFontBrush, iMarginForImageWidth, CurTextPosY + MTN +
                                    (CurrentY * (G.MeasureString("string", fPositionFont).Height) - MHR) - Offset);

                        i -= (CurrentRowString.Length - LastSpace);
                    }


                    CurrentRowString = "";
                    CurrentY++;
                    continue;
                }

                CurrentRowString += Position[i];
            }

            int C = CurrentY * Convert.ToInt32((G.MeasureString("String", fPositionFont).Height) + iMarginNameRow) + MTN;

            CurTextPosY += C - MHR * 2;
        }

        private void DrawExecType(int index, string ExecType, Graphics G, ref int CurTextPosY)
        {
            int TextMaxWidth = this.Width - iMarginForImageWidth;

            int C = Convert.ToInt32(G.MeasureString(ExecType, fExecTypeFont).Height);

            G.DrawString(ExecType, fExecTypeFont, brExecTypeFontBrush, iMarginForImageWidth, CurTextPosY - Offset - 7);

            CurTextPosY += C - 5;
        }

        private void DrawVerticalScrollShaft(Graphics G)
        {
            if (this.Height >= TotalY)
            {
                bVertScrollVisible = false;
                return;
            }

            bVertScrollVisible = true;

            //Shaft
            G.FillRectangle(brVerticalScrollCommonShaftBackBrush, rVerticalScrollShaftRect);
        }

        private void DrawVertScrollThumb(Graphics G)
        {
            decimal V = this.Height;
            decimal T = Convert.ToDecimal(TotalY - 30);

            decimal Rtv = V / (T / 100);

            decimal Th = Convert.ToInt32(Decimal.Round(Rtv * (V / 100), 0, MidpointRounding.AwayFromZero));

            if (Th >= V)
                return;

            decimal Ws = (V - Th) / ((T - V) / iScrollWheelOffset);

            int posY = Convert.ToInt32(Decimal.Round((Convert.ToDecimal(Offset) / Convert.ToDecimal(iScrollWheelOffset)) * Ws, 0, MidpointRounding.AwayFromZero)) + 2;

            if (posY + Th > V)
                posY = Convert.ToInt32(V - Th);

            rVerticalScrollThumbRect.Y = posY;
            rVerticalScrollThumbRect.Width = iScrollWidth - 4;
            rVerticalScrollThumbRect.Height = Convert.ToInt32(Th);
            rVerticalScrollThumbRect.X = rVerticalScrollShaftRect.X;

            //G.FillRectangle(brVerticalScrollCommonThumbButtonBrush, rVerticalScrollThumbRect);

            G.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;

            G.FillRectangle(brVerticalScrollCommonThumbButtonBrush, new Rectangle(rVerticalScrollThumbRect.X + 2, posY,
                                                                                  iScrollWidth - 4, Convert.ToInt32(Th)));
        }

        private void DrawSplitter(Graphics G, ref int CurTextPosY)
        {
            if (bVertScrollVisible)
                G.DrawLine(pSplitterPen, 0, CurTextPosY + 8 - Offset, this.Width - rVerticalScrollShaftRect.Width - 2, CurTextPosY + 8 - Offset);
            else
                G.DrawLine(pSplitterPen, 0, CurTextPosY + 8 - Offset, this.Width, CurTextPosY + 8 - Offset);
        }

        private void DrawPriorityLine(Graphics G)
        {
            G.DrawLine(pSplitterPen, 85, 0, 85, this.Height);
        }

        private void DrawTimeLine(Graphics G)
        {
            G.DrawLine(pSplitterPen,
                          this.Width - rVerticalScrollShaftRect.Width - iMarginForComplete - iMarginForTimePicker,
                            0,
                          this.Width - rVerticalScrollShaftRect.Width - iMarginForComplete - iMarginForTimePicker, this.Height);
        }

        private void DrawCompleteLine(Graphics G)
        {
            G.DrawLine(pSplitterPen,
                          this.Width - rVerticalScrollShaftRect.Width - iMarginForComplete,
                            0,
                          this.Width - rVerticalScrollShaftRect.Width - iMarginForComplete, this.Height);
        }

        private void DrawPercentsLine(Graphics G)
        {
            G.DrawLine(pSplitterPen,
                         this.Width - rVerticalScrollShaftRect.Width - iMarginForComplete - iMarginForTimePicker - iMarginForPercents,
                           0,
                         this.Width - rVerticalScrollShaftRect.Width - iMarginForComplete - iMarginForTimePicker - iMarginForPercents, this.Height);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (!bVertScrollVisible)
                return;

            if (Offset == 0)
                if (iTempScrollWheelOffset > 0)
                    iTempScrollWheelOffset = 0;

            if (e.Delta < 0)
            {
                if (Offset < TotalY - 30 + iScrollWheelOffset - this.Height)
                {
                    if (Offset + iScrollWheelOffset + this.Height > TotalY - 30)
                    {
                        iTempScrollWheelOffset = TotalY - 30 + iScrollWheelOffset - this.Height - Offset;
                        Offset += iTempScrollWheelOffset;
                    }
                    else
                        Offset += iScrollWheelOffset;
                    this.Refresh();
                }
            }

            if (e.Delta > 0)
                if (Offset > 0)
                {
                    if (iTempScrollWheelOffset > 0)
                    {
                        Offset -= iTempScrollWheelOffset;
                        iTempScrollWheelOffset = 0;
                    }
                    else
                        Offset -= iScrollWheelOffset;

                    this.Refresh();
                }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            //if (!this.Focused)
            //    this.Focus();

            if (FunctionsDataTable == null)
                return;

            DataRow[] sRow = FunctionsDataTable.Select("Position >= " + (e.Y + Offset - 10) + " AND Position <= " + Offset + TotalY);

            if (sRow.Count() > 0)
            {
                if (iSelected != FunctionsDataTable.Rows.IndexOf(sRow[0]))
                {
                    iSelected = FunctionsDataTable.Rows.IndexOf(sRow[0]);
                    //this.Refresh();
                }
            }
            else
            {
                if (iSelected != -1)
                {
                    iSelected = -1;
                    //this.Refresh();
                }
            }

            if (iSelected == -1)
            {
                if (this.Cursor != Cursors.Default)
                    this.Cursor = Cursors.Default;

                return;
            }

            if (Convert.ToBoolean(dtFunctionsDataTable.Rows[iSelected]["IsComplete"]) == false)
            {
                if (e.X >= iIncompleteX && e.X <= iIncompleteX + iIncompleteW)
                {
                    DataRow[] Row = dtFunctionsDataTable.Select("InCompletePositionY1 <= " + (e.Y) + " AND InCompletePositionY2 >= " + (e.Y));

                    if (Row.Count() > 0)
                    {
                        if (Convert.ToBoolean(Row[0]["IsComplete"]) == false)
                        {

                            iCompleteSelected = dtFunctionsDataTable.Rows.IndexOf(Row[0]);

                            if (this.Cursor != Cursors.Hand)
                                this.Cursor = Cursors.Hand;
                        }
                        else
                        {
                            iCompleteSelected = -1;

                            if (this.Cursor != Cursors.Default)
                                this.Cursor = Cursors.Default;
                        }
                    }
                    else
                    {
                        iCompleteSelected = -1;

                        if (this.Cursor != Cursors.Default)
                            this.Cursor = Cursors.Default;
                    }
                }
                else
                {
                    iCompleteSelected = -1;

                    if (this.Cursor != Cursors.Default)
                        this.Cursor = Cursors.Default;
                }
            }
            else
            {
                if (e.X >= iCompleteX && e.X <= iCompleteX + iCompleteW)
                {
                    DataRow[] Row = dtFunctionsDataTable.Select("CompletePositionY1 <= " + (e.Y) + " AND CompletePositionY2 >= " + (e.Y));

                    if (Row.Count() > 0)
                    {
                        if (Convert.ToBoolean(Row[0]["IsComplete"]) == true)
                        {

                            iCompleteSelected = dtFunctionsDataTable.Rows.IndexOf(Row[0]);

                            if (this.Cursor != Cursors.Hand)
                                this.Cursor = Cursors.Hand;
                        }
                        else
                        {
                            iCompleteSelected = -1;

                            if (this.Cursor != Cursors.Default)
                                this.Cursor = Cursors.Default;
                        }
                    }
                    else
                    {
                        iCompleteSelected = -1;

                        if (this.Cursor != Cursors.Default)
                            this.Cursor = Cursors.Default;
                    }
                }
                else
                {
                    iCompleteSelected = -1;

                    if (this.Cursor != Cursors.Default)
                        this.Cursor = Cursors.Default;
                }
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            rVerticalScrollShaftRect.X = this.Width - iScrollWidth;
            rVerticalScrollShaftRect.Height = this.Height;
            rVerticalScrollShaftRect.Width = iScrollWidth;

            rVerticalScrollThumbRect.X = this.Width - iScrollWidth;
            rVerticalScrollThumbRect.Height = this.Height;
            rVerticalScrollThumbRect.Width = iScrollWidth;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!this.Focused)
                this.Focus();

            if (bReadOnly)
                return;

            base.OnMouseDown(e);

            if (FunctionsDataTable != null)
            {
                DataRow[] Row = FunctionsDataTable.Select("Position >= " + (e.Y + Offset - 10) + " AND Position <= " + Offset + TotalY);

                if (Row.Count() > 0)
                {
                    iSelected = dtFunctionsDataTable.Rows.IndexOf(Row[0]);
                }
                else
                    iSelected = -1;

                OnItemClick();

                if (iCompleteSelected > -1)
                {
                    if (Convert.ToBoolean(dtFunctionsDataTable.Rows[iCompleteSelected]["IsComplete"]) == true)
                        dtFunctionsDataTable.Rows[iCompleteSelected]["IsComplete"] = false;
                    else
                        dtFunctionsDataTable.Rows[iCompleteSelected]["IsComplete"] = true;

                    iCompleteSelected = -1;

                    this.Cursor = Cursors.Default;
                }

                this.Refresh();
            }
        }

        public event ItemClickEventHandler ItemClick;

        public delegate void ItemClickEventHandler(object sender, int FunctionID);

        public virtual void OnItemClick()
        {
            if (bReadOnly)
                return;

            if (ItemClick != null)
            {
                ItemClick(this, iSelected);//Raise the event
            }
        }


        public event TimeChangedEventHandler TimeChanged;

        public delegate void TimeChangedEventHandler(object sender, int Minutes);

        public virtual void OnTimeChanged()
        {
            if (TimeChanged != null)
            {
                TimeChanged(this, TotalAllocMin);//Raise the event
            }
        }

        private void TimeChange(object sender, int Hours, int Minutes)
        {
            if (bReadOnly)
                return;

            dtFunctionsDataTable.Rows[Convert.ToInt32(((InfiniumTimePicker)sender).Name)]["Hours"] = Hours;
            dtFunctionsDataTable.Rows[Convert.ToInt32(((InfiniumTimePicker)sender).Name)]["Minutes"] = Minutes;

            this.Refresh();
            OnTimeChanged();
        }
    }





    public class InfiniumClock : Control
    {
        public int iMinutes = 0;
        public int iSeconds = 0;
        public int iHours = 0;

        Pen pMinutesPen;
        Pen pSecondsPen;
        Pen pHoursPen;

        int iMarginLines = 0;

        Image iImage;

        public InfiniumClock()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            pHoursPen = new Pen(new SolidBrush(Color.FromArgb(50, 50, 50)), 2.8f);
            pMinutesPen = new Pen(new SolidBrush(Color.FromArgb(50, 50, 50)), 2);
            pSecondsPen = new Pen(new SolidBrush(Color.Gray), 1.4f);
        }

        public Image Image
        {
            get { return iImage; }
            set { iImage = value; this.Refresh(); }
        }

        public int MarginLines
        {
            get { return iMarginLines; }
            set { iMarginLines = value; }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            int sx0 = this.Width / 2;
            int sy0 = this.Height / 2;

            int mx0 = this.Width / 2;
            int my0 = this.Height / 2;

            int hx0 = this.Width / 2;
            int hy0 = this.Height / 2;

            int sr = this.Width / 2 - 12 - iMarginLines;
            int mr = this.Width / 2 - 12 - iMarginLines;
            int hr = this.Width / 2 - 25 - iMarginLines;

            int ma = 360 / 60 * iMinutes;
            int sa = 360 / 60 * iSeconds;
            int ha = 0;
            if (iHours > 12)
                ha = 360 / 60 * ((iHours - 12) * 5) + (360 / 60 * (iMinutes * 6 / 60));
            else
                ha = 360 / 60 * (iHours * 5) + (360 / 60 * (iMinutes * 6 / 60));

            int sx1 = Convert.ToInt32(sx0 + (Math.Cos(-(90 - sa) * 3.14 / 180) * sr));
            int sy1 = Convert.ToInt32(sy0 + (Math.Sin(-(90 - sa) * 3.14 / 180) * sr));
            int sx2 = Convert.ToInt32(sx0 + (Math.Cos(-(90 - sa + 180) * 3.14 / 180) * 9));
            int sy2 = Convert.ToInt32(sy0 + (Math.Sin(-(90 - sa + 180) * 3.14 / 180) * 9));

            int mx1 = Convert.ToInt32(mx0 + (Math.Cos(-(90 - ma) * 3.14 / 180) * mr));
            int my1 = Convert.ToInt32(my0 + (Math.Sin(-(90 - ma) * 3.14 / 180) * mr));

            int hx1 = Convert.ToInt32(hx0 + (Math.Cos(-(90 - ha) * 3.14 / 180) * hr));
            int hy1 = Convert.ToInt32(hy0 + (Math.Sin(-(90 - ha) * 3.14 / 180) * hr));


            e.Graphics.DrawLine(pSecondsPen, sx0, sy0, sx1, sy1);
            e.Graphics.DrawLine(pSecondsPen, sx0, sy0, sx2, sy2);
            e.Graphics.DrawLine(pMinutesPen, mx0, my0, mx1, my1);
            e.Graphics.DrawLine(pHoursPen, hx0, hy0, hx1, hy1);
            e.Graphics.FillEllipse(pMinutesPen.Brush, hx0 - 3, hy0 - 3, 6, 6);

            GC.Collect();
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            base.OnPaintBackground(pevent);

            if (Image == null)
                return;

            pevent.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            pevent.Graphics.DrawImage(Image, 0, 0, this.Width, this.Height);
        }
    }






    public class InfiniumDayTimeClock : Control
    {
        Bitmap bmpClock = Properties.Resources.Clock44;

        public DateTime dtStartDate;
        public DateTime dtEndDate;
        public DateTime dtBreakStartDate;
        public DateTime dtBreakEndDate;

        public DateTime CurrentTime;

        public bool bStarted = false;
        public bool bBreakStart = false;
        public bool bBreakEnd = false;
        public bool bEnded = false;

        public int iBreakMinutes = 0;

        Rectangle rFillRect;

        SolidBrush brWorkdayBrush;
        SolidBrush brBreakBrush;

        public InfiniumDayTimeClock()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            brWorkdayBrush = new SolidBrush(Color.FromArgb(120, 56, 184, 238));
            brBreakBrush = new SolidBrush(Color.FromArgb(120, 213, 241, 252));
            rFillRect = new Rectangle(0, 0, 30, 30);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            e.Graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            rFillRect.Width = this.Width - 3;
            rFillRect.Height = this.Height - 3;
            rFillRect.X = 1;
            rFillRect.Y = 1;

            DateTime eStartDate = DateTime.MinValue;
            DateTime eEndDate = DateTime.MinValue;
            DateTime eBreakStartDate = DateTime.MinValue;
            DateTime eBreakEndDate = DateTime.MinValue;

            if (bStarted)
            {
                eStartDate = dtStartDate;
            }
            else
            {

            }

            if (bBreakStart == true)
            {
                eBreakStartDate = dtBreakStartDate;
            }
            else
            {

            }

            if (bBreakEnd)
            {
                eBreakEndDate = dtBreakEndDate;
            }
            else
            {
                if (bBreakStart)
                    eBreakEndDate = CurrentTime;
            }

            if (bEnded)
            {
                eEndDate = dtEndDate;
            }
            else
            {
                if (bStarted)
                    eEndDate = CurrentTime;
            }


            int sA = 360 / 12 * eStartDate.Hour - 90 + (eStartDate.Minute * 30 / 60);
            int eA = 360 / 12 * (eEndDate.Hour - eStartDate.Hour) + (eEndDate.Minute * 30 / 60) - (eStartDate.Minute * 30 / 60);
            int bsA = 360 / 12 * eBreakStartDate.Hour - 90 + (eBreakStartDate.Minute * 30 / 60);
            int beA = 360 / 12 * (eBreakEndDate.Hour - eBreakStartDate.Hour) + (eBreakEndDate.Minute * 30 / 60) - (eBreakStartDate.Minute * 30 / 60);

            if ((eEndDate - eStartDate).TotalHours > 12)
                eA = 360;
            if ((eBreakEndDate - eBreakStartDate).TotalHours > 12)
                beA = 360;

            string TotalTime = "";

            int TotalMin = Convert.ToInt32((eEndDate - eStartDate).TotalMinutes - (eBreakEndDate - eBreakStartDate).TotalMinutes);

            if ((TotalMin - Convert.ToInt32(TotalMin / 60) * 60) < 10)
                TotalTime = Convert.ToInt32(TotalMin / 60).ToString() + " : 0" + (TotalMin - Convert.ToInt32(TotalMin / 60) * 60).ToString();
            else
                TotalTime = Convert.ToInt32(TotalMin / 60).ToString() + " : " + (TotalMin - Convert.ToInt32(TotalMin / 60) * 60).ToString();

            e.Graphics.FillPie(brWorkdayBrush, rFillRect, sA, eA);

            if (bBreakStart)
                e.Graphics.FillPie(brBreakBrush, rFillRect, bsA, beA);

            e.Graphics.DrawString(TotalTime, this.Font, new SolidBrush(Color.Gray),
                                            (this.Width - e.Graphics.MeasureString(TotalTime, this.Font).Width) / 2,
                                            (this.Height - e.Graphics.MeasureString(TotalTime, this.Font).Height) / 2);

            GC.Collect();
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            base.OnPaintBackground(pevent);

            pevent.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            pevent.Graphics.DrawImage(bmpClock, 0, 0, this.Width, this.Height);
        }
    }





    public class InfiniumTipsLabel : Control
    {
        private int iMaxWidth;
        SolidBrush bFontBrush;

        public InfiniumTipsLabel()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            bFontBrush = new SolidBrush(this.ForeColor);
        }

        public int MaxWidth
        {
            get { return iMaxWidth; }
            set { iMaxWidth = value; this.Refresh(); }
        }

        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
            set
            {
                base.ForeColor = value;
                bFontBrush.Color = value;
                this.Refresh();
            }
        }

        [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            typeof(UITypeEditor))]
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;

                Graphics G = this.CreateGraphics();

                if (G.MeasureString(this.Text, this.Font).Height + 10 > this.Height)
                    this.Height = Convert.ToInt32(G.MeasureString(this.Text, this.Font).Height + 10);

                if (G.MeasureString(this.Text, this.Font).Width + 10 > this.Width)
                    this.Width = Convert.ToInt32(G.MeasureString(this.Text, this.Font).Width + 10);

                this.Refresh();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            e.Graphics.DrawString(this.Text, this.Font, bFontBrush, (this.Width - e.Graphics.MeasureString(this.Text, this.Font).Width) / 2 + 5,
                                  (this.Height - e.Graphics.MeasureString(this.Text, this.Font).Height) / 2);
        }
    }
}

