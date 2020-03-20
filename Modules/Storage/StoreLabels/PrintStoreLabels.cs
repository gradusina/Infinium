using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Printing;

namespace Infinium.Modules.Storage.StoreLabels
{
    public struct Info
    {
        public string BarcodeNumber;
    }

    public class PrintManagerStoreLabels
    {
        Barcode Barcode;
        public PrintDocument PD;

        public int PaperHeight = 80;
        public int PaperWidth = 40;

        public int CurrentLabelNumber = 0;

        public int PrintedCount = 0;

        public bool Printed = false;

        public ArrayList LabelInfo;

        public PrintManagerStoreLabels()
        {
            Barcode = new Barcode();

            InitializePrinter();

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

        public void ClearLabelInfo()
        {
            CurrentLabelNumber = 0;
            PrintedCount = 0;
            LabelInfo.Clear();
            GC.Collect();
        }

        public void AddLabelInfo(ref Info LabelInfoItem)
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

            ev.Graphics.DrawImage(Barcode.GetBarcode(Barcode.BarcodeLength.Short, 30, ((Info)LabelInfo[CurrentLabelNumber]).BarcodeNumber), 0, 10);

            Barcode.DrawBarcodeText(Barcode.BarcodeLength.Short, ev.Graphics, ((Info)LabelInfo[CurrentLabelNumber]).BarcodeNumber, 4, 41);

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
            PD.DefaultPageSettings.Landscape = false;
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

        public string GetBarcodeNumber(int BarcodeType, int StoreLabelNumber)
        {
            string Type = string.Empty;
            if (BarcodeType.ToString().Length == 1)
                Type = "00" + BarcodeType.ToString();
            if (BarcodeType.ToString().Length == 2)
                Type = "0" + BarcodeType.ToString();
            if (BarcodeType.ToString().Length == 3)
                Type = BarcodeType.ToString();

            string Number = string.Empty;
            if (StoreLabelNumber.ToString().Length == 1)
                Number = "00000000" + StoreLabelNumber.ToString();
            if (StoreLabelNumber.ToString().Length == 2)
                Number = "0000000" + StoreLabelNumber.ToString();
            if (StoreLabelNumber.ToString().Length == 3)
                Number = "000000" + StoreLabelNumber.ToString();
            if (StoreLabelNumber.ToString().Length == 4)
                Number = "00000" + StoreLabelNumber.ToString();
            if (StoreLabelNumber.ToString().Length == 5)
                Number = "0000" + StoreLabelNumber.ToString();
            if (StoreLabelNumber.ToString().Length == 6)
                Number = "000" + StoreLabelNumber.ToString();
            if (StoreLabelNumber.ToString().Length == 7)
                Number = "00" + StoreLabelNumber.ToString();
            if (StoreLabelNumber.ToString().Length == 8)
                Number = "0" + StoreLabelNumber.ToString();

            System.Text.StringBuilder BarcodeNumber = new System.Text.StringBuilder(Type);
            BarcodeNumber.Append(Number);

            return BarcodeNumber.ToString();
        }

    }


    public class FFFF
    {
        int imageType = 1;

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
        Font NotesFont;
        Font HeaderFont;
        Font FrontOrderFont;
        Font DecorOrderFont;
        Font DispatchFont;

        Pen Pen;

        public FFFF()
        {
            Barcode = new Barcode();

            InitializePrinter();
            InitializeFonts();
        }

        public int ImageType
        {
            set { imageType = value; }
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
            ClientFont = new Font("Arial", 40.0f, FontStyle.Bold);
            DocFont = new Font("Arial", 18.0f, FontStyle.Regular);
            InfoFont = new Font("Arial", 5.0f, FontStyle.Regular);
            NotesFont = new Font("Arial", 16.0f, FontStyle.Bold);
            HeaderFont = new Font("Arial", 9.0f, FontStyle.Regular);
            FrontOrderFont = new Font("Arial", 8.0f, FontStyle.Regular);
            DecorOrderFont = new Font("Arial", 10.0f, FontStyle.Regular);
            DispatchFont = new Font("Arial", 8.0f, FontStyle.Bold);
            Pen = new Pen(FontBrush)
            {
                Width = 1
            };
        }

        public void ClearLabelInfo()
        {
            CurrentLabelNumber = 0;
            PrintedCount = 0;
            GC.Collect();
        }

        public void PrintPage(object sender, PrintPageEventArgs ev)
        {
            if (imageType == 1)
            {
                ev.Graphics.Clear(Color.White);

                ev.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

                int AdmissionX = -10;
                int AdmissionY = 25;

                ev.Graphics.DrawLine(Pen, AdmissionX + 115, AdmissionY + 56, AdmissionX + 115, AdmissionY + 275);
                ev.Graphics.DrawLine(Pen, AdmissionX + 395, AdmissionY + 56, AdmissionX + 395, AdmissionY + 275);
                ev.Graphics.DrawLine(Pen, AdmissionX + 115, AdmissionY + 56, AdmissionX + 395, AdmissionY + 56);
                ev.Graphics.DrawLine(Pen, AdmissionX + 115, AdmissionY + 275, AdmissionX + 395, AdmissionY + 275);

                AdmissionX = -10;
                AdmissionY = 25;

                ev.Graphics.DrawString("Дверь", ClientFont, FontBrush, AdmissionX + 165, AdmissionY + 106);
                ev.Graphics.DrawString("№ 21", ClientFont, FontBrush, AdmissionX + 178, AdmissionY + 166);
            }
            if (imageType == 2)
            {
                ev.Graphics.Clear(Color.White);

                ev.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

                int Admission = 35;
                ev.Graphics.DrawString("Запасные ключи от дверей:", NotesFont, FontBrush, 10, Admission + 6);
                ev.Graphics.DrawString("С ООО \"ЗОВ-ПРОФИЛЬ\"", NotesFont, FontBrush, 10, Admission + 46);
                ev.Graphics.DrawString("С ООО \"ТермоПрофильСистемы\"", NotesFont, FontBrush, 10, Admission + 86);
                ev.Graphics.DrawString("Вскрытие ящика №1 производится", NotesFont, FontBrush, 10, Admission + 146);
                ev.Graphics.DrawString("только при чрезвычайных", NotesFont, FontBrush, 10, Admission + 186);
                ev.Graphics.DrawString("обстоятельствах или в присутствии", NotesFont, FontBrush, 10, Admission + 226);
                ev.Graphics.DrawString("руководства С ООО \"ЗОВ-ПРОФИЛЬ\",", NotesFont, FontBrush, 10, Admission + 266);
                ev.Graphics.DrawString("С ООО \"ТермоПрофильСистемы\"", NotesFont, FontBrush, 10, Admission + 306);
            }
        }

        public void Print()
        {
            PD.DefaultPageSettings.Landscape = false;
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

        public string GetBarcodeNumber(int BarcodeType, int StoreLabelNumber)
        {
            string Type = string.Empty;
            if (BarcodeType.ToString().Length == 1)
                Type = "00" + BarcodeType.ToString();
            if (BarcodeType.ToString().Length == 2)
                Type = "0" + BarcodeType.ToString();
            if (BarcodeType.ToString().Length == 3)
                Type = BarcodeType.ToString();

            string Number = string.Empty;
            if (StoreLabelNumber.ToString().Length == 1)
                Number = "00000000" + StoreLabelNumber.ToString();
            if (StoreLabelNumber.ToString().Length == 2)
                Number = "0000000" + StoreLabelNumber.ToString();
            if (StoreLabelNumber.ToString().Length == 3)
                Number = "000000" + StoreLabelNumber.ToString();
            if (StoreLabelNumber.ToString().Length == 4)
                Number = "00000" + StoreLabelNumber.ToString();
            if (StoreLabelNumber.ToString().Length == 5)
                Number = "0000" + StoreLabelNumber.ToString();
            if (StoreLabelNumber.ToString().Length == 6)
                Number = "000" + StoreLabelNumber.ToString();
            if (StoreLabelNumber.ToString().Length == 7)
                Number = "00" + StoreLabelNumber.ToString();
            if (StoreLabelNumber.ToString().Length == 8)
                Number = "0" + StoreLabelNumber.ToString();

            System.Text.StringBuilder BarcodeNumber = new System.Text.StringBuilder(Type);
            BarcodeNumber.Append(Number);

            return BarcodeNumber.ToString();
        }

    }

    public class Barcode
    {
        BarcodeLib.Barcode Barcod;

        SolidBrush FontBrush;

        public enum BarcodeLength { Short, Medium, Long };

        public Barcode()
        {
            Barcod = new BarcodeLib.Barcode();

            FontBrush = new System.Drawing.SolidBrush(Color.Black);
        }

        public void DrawBarcodeText(BarcodeLength BarcodeLength, Graphics Graphics, string Text, int X, int Y)
        {
            int CharOffset = 0;
            int CharWidth = 0;
            float FontSize = 0;

            if (BarcodeLength == Barcode.BarcodeLength.Short)
            {
                CharWidth = 8;
                CharOffset = 7;
                FontSize = 8.0f;
            }
            if (BarcodeLength == Barcode.BarcodeLength.Medium)
            {
                CharWidth = 18;
                CharOffset = 5;
                FontSize = 12.0f;
            }
            if (BarcodeLength == Barcode.BarcodeLength.Long)
            {
                CharWidth = 26;
                CharOffset = 5;
                FontSize = 14.0f;
            }

            Font F = new Font("Arial", FontSize, FontStyle.Bold);

            for (int i = 0; i < Text.Length; i++)
            {
                Graphics.DrawString(Text[i].ToString(), F, FontBrush, i * CharWidth + CharOffset + X, Y + 2);
            }

            F.Dispose();
        }

        public Image GetBarcode(BarcodeLength BarcodeLength, int BarcodeHeight, string Text)
        {
            //set length and height
            if (BarcodeLength == Barcode.BarcodeLength.Short)
            {
                Barcod.Width = 101 + 12;
            }
            if (BarcodeLength == Barcode.BarcodeLength.Medium)
            {
                Barcod.Width = 202 + 12;
            }
            if (BarcodeLength == Barcode.BarcodeLength.Long)
            {
                Barcod.Width = 303 + 12;
            }

            Barcod.Height = BarcodeHeight;


            //create area
            Bitmap B = new Bitmap(Barcod.Width, BarcodeHeight);
            Graphics G = Graphics.FromImage(B);
            G.Clear(Color.White);


            //create barcode
            Image Bar = Barcod.Encode(BarcodeLib.TYPE.CODE128C, Text);
            G.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            G.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            G.DrawImage(Bar, 0, 2);


            //create text
            G.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;


            Bar.Dispose();
            G.Dispose();

            GC.Collect();

            return B;
        }
    }
}
