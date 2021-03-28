using ComponentFactory.Krypton.Toolkit;

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Infinium



{
    //news
    public class InfiniumScrollContainer : Control
    {
        SolidBrush brBackBrush;
        Color cBackColor = Color.White;

        public InfiniumScrollContainer()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);

            brBackBrush = new SolidBrush(cBackColor);
        }

        public Color BackgroundColor
        {
            get { return cBackColor; }
            set { cBackColor = value; brBackBrush.Color = value; this.Refresh(); }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(brBackBrush, ClientRectangle);

            base.OnPaint(e);
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            //base.OnPaintBackground(pevent);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var parms = base.CreateParams;
                parms.Style &= ~0x02000000;  // Turn off WS_CLIPCHILDREN
                return parms;
            }
        }

        protected override void OnClick(EventArgs e)
        {
            this.Focus();

            base.OnClick(e);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);

        }
    }


    public class InfiniumVerticalScrollBar : Control
    {
        public int iTotalControlHeight = 0;//height of scrollable container (area)
        public int iScrollWheelOffset = 30;
        int iOffset = 0;

        Rectangle rVerticalScrollShaftRect;

        bool bThumbTracking = false;
        bool bThumbClicked = false;
        int iThumbYClicked = -1;


        public int ThumbSize = 0;//height of thumb
        public decimal iThumbPosition = 0;//current position of thumb on scrollshaft
        public decimal ThumbOneStepWidth = 0;//value offset scrollable area on one step of a thumb
        public int ThumbStepsCount = 0;//total count of thumb steps (in pixels) on scrollshaft
        public decimal ThumbStepWidthOnScrollWheel = 0;//thumb offset on scrollshaft (Y) on every scrollwheeloffset


        Color cVerticalScrollCommonShaftBackColor = Color.LightBlue;
        Color cVerticalScrollCommonThumbButtonColor = Color.DarkGray;
        Color cVerticalScrollTrackingShaftBackColor = Color.Blue;
        Color cVerticalScrollTrackingThumbButtonColor = Color.Gray;

        SolidBrush brVerticalScrollCommonShaftBackBrush;
        SolidBrush brVerticalScrollCommonThumbButtonBrush;
        SolidBrush brVerticalScrollTrackingShaftBackBrush;
        SolidBrush brVerticalScrollTrackingThumbButtonBrush;

        public InfiniumVerticalScrollBar()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            brVerticalScrollCommonShaftBackBrush = new SolidBrush(cVerticalScrollCommonShaftBackColor);
            brVerticalScrollCommonThumbButtonBrush = new SolidBrush(cVerticalScrollCommonThumbButtonColor);
            brVerticalScrollTrackingShaftBackBrush = new SolidBrush(cVerticalScrollTrackingShaftBackColor);
            brVerticalScrollTrackingThumbButtonBrush = new SolidBrush(cVerticalScrollTrackingThumbButtonColor);

            rVerticalScrollShaftRect = new Rectangle(0, 0, 0, 0);

            Visible = true;
        }

        public int ScrollWheelOffset
        {
            get { return iScrollWheelOffset; }
            set { iScrollWheelOffset = value; this.Refresh(); }
        }

        public int TotalControlHeight
        {
            get { return iTotalControlHeight; }
            set
            {
                iTotalControlHeight = value; InitializeThumb();
                this.Refresh();
            }
        }


        public int Offset
        {
            get { return iOffset; }
            set
            {
                iOffset = value;

                if (iScrollWheelOffset > 0)
                {
                    iThumbPosition = Convert.ToInt32(Decimal.Round(Offset * ThumbStepWidthOnScrollWheel / iScrollWheelOffset,
                                        0, MidpointRounding.AwayFromZero));
                    //if (iThumbPosition + ThumbSize > this.Height)
                    //    iThumbPosition = this.Height - Convert.ToInt32(ThumbSize);
                }

                this.Refresh();
            }
        }


        public Color VerticalScrollCommonShaftBackColor
        {
            get { return cVerticalScrollCommonShaftBackColor; }
            set { cVerticalScrollCommonShaftBackColor = value; brVerticalScrollCommonShaftBackBrush.Color = value; this.Refresh(); }
        }

        public Color VerticalScrollCommonThumbButtonColor
        {
            get { return cVerticalScrollCommonThumbButtonColor; }
            set { cVerticalScrollCommonThumbButtonColor = value; brVerticalScrollCommonThumbButtonBrush.Color = value; this.Refresh(); }
        }

        public Color VerticalScrollTrackingShaftBackColor
        {
            get { return cVerticalScrollTrackingShaftBackColor; }
            set { cVerticalScrollTrackingShaftBackColor = value; brVerticalScrollTrackingShaftBackBrush.Color = value; this.Refresh(); }
        }

        public Color VerticalScrollTrackingThumbButtonColor
        {
            get { return cVerticalScrollTrackingThumbButtonColor; }
            set { cVerticalScrollTrackingThumbButtonColor = value; brVerticalScrollTrackingThumbButtonBrush.Color = value; this.Refresh(); }
        }



        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            DrawVerticalScrollShaft(e.Graphics);

            if (TotalControlHeight == 0 || this.Height == 0 || ScrollWheelOffset == 0 || this.Width == 0)
                return;


            DrawVerticalScrollThumb(e.Graphics);
        }


        private void DrawVerticalScrollShaft(Graphics G)
        {
            //Shaft
            if (bThumbTracking)
                G.FillRectangle(brVerticalScrollTrackingShaftBackBrush, rVerticalScrollShaftRect);
            else
                G.FillRectangle(brVerticalScrollCommonShaftBackBrush, rVerticalScrollShaftRect);
        }

        public void InitializeThumb()
        {
            if (TotalControlHeight == 0 || ScrollWheelOffset == 0)
                return;

            if (TotalControlHeight == this.Height)
                return;

            decimal V = this.Height;
            decimal T = Convert.ToDecimal(TotalControlHeight);

            decimal Rtv = V / (T / 100);

            ThumbSize = Convert.ToInt32(Decimal.Round(Rtv * (V / 100), 0, MidpointRounding.AwayFromZero));

            if (ThumbSize >= V)
                return;

            ThumbStepWidthOnScrollWheel = (V - ThumbSize) / ((T - V) / ScrollWheelOffset);//ширина шага ползунка при одном смещении

            int posY = Convert.ToInt32(Decimal.Round((Convert.ToDecimal(iOffset) / Convert.ToDecimal(ScrollWheelOffset)) *
                                                        ThumbStepWidthOnScrollWheel, 0, MidpointRounding.AwayFromZero));

            if (posY + ThumbSize > V)
                posY = Convert.ToInt32(V - ThumbSize);

            ThumbStepsCount = this.Height - ThumbSize;

            ThumbOneStepWidth = (Convert.ToDecimal(TotalControlHeight) - this.Height) / ThumbStepsCount;

            iThumbPosition = posY;
        }

        private void DrawVerticalScrollThumb(Graphics G)
        {
            if (this.Height == TotalControlHeight)
                return;

            if (!bThumbTracking)
                G.FillRectangle(brVerticalScrollCommonThumbButtonBrush, new Rectangle(rVerticalScrollShaftRect.X + 2,
                                        Convert.ToInt32(Decimal.Round(iThumbPosition, 0, MidpointRounding.AwayFromZero)),
                                                                                  this.Width - 4, Convert.ToInt32(ThumbSize)));
            else
                G.FillRectangle(brVerticalScrollTrackingThumbButtonBrush, new Rectangle(rVerticalScrollShaftRect.X + 2,
                                        Convert.ToInt32(Decimal.Round(iThumbPosition, 0, MidpointRounding.AwayFromZero)),
                                                                                  this.Width - 4, Convert.ToInt32(ThumbSize)));
        }

        public void SetPosition(int Pos)
        {
            if (Pos * ThumbOneStepWidth > TotalControlHeight - this.Height)
            {
                iOffset = TotalControlHeight - this.Height;
                iThumbPosition = this.Height - Convert.ToInt32(ThumbSize);

                this.Refresh();

                OnScrollPositionChanged(Offset);

                return;
            }
            else
                if (Pos * ThumbOneStepWidth < 0)
            {
                iThumbPosition = 0;
                iOffset = 0;
                this.Refresh();

                OnScrollPositionChanged(Offset);

                return;
            }
            else
            {
                iOffset = Convert.ToInt32(ThumbOneStepWidth * Pos);
            }


            iThumbPosition = Pos;

            this.Refresh();

            OnScrollPositionChanged(Offset);
        }


        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            rVerticalScrollShaftRect.Height = this.Height;
            rVerticalScrollShaftRect.Width = this.Width;

            this.Refresh();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            //if (e.Y < iThumbYClicked)
            //{
            //    if (bThumbClicked)
            //    {
            //        SetPosition(0);
            //        return;
            //    }
            //}

            //if (e.Y > this.Height - iThumbYClicked)
            //{
            //    if (bThumbClicked)
            //    {
            //        SetPosition(ThumbStepsCount);
            //        return;
            //    }
            //}

            if (bThumbClicked)
            {
                SetPosition(e.Y - iThumbYClicked);
                return;
            }

            if (e.Y > iThumbPosition && e.Y < iThumbPosition + ThumbSize)
            {
                if (this.Cursor != Cursors.Hand)
                {
                    bThumbTracking = true;
                    this.Cursor = Cursors.Hand;
                    this.Refresh();
                }

                iThumbYClicked = e.Y - Convert.ToInt32(Decimal.Round(iThumbPosition, 0, MidpointRounding.AwayFromZero));
            }
            else
            {
                if (this.Cursor != Cursors.Default)
                {
                    bThumbTracking = false;
                    this.Cursor = Cursors.Default;
                    this.Refresh();
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (bThumbTracking)
                bThumbClicked = true;
            else
                bThumbClicked = false;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            iThumbYClicked = -1;
            bThumbClicked = false;
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);

            bThumbClicked = false;
            bThumbTracking = false;
            this.Refresh();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (!bThumbClicked)
            {
                bThumbClicked = false;
                bThumbTracking = false;
                Cursor = Cursors.Default;
                this.Refresh();
            }
        }


        public event ScrollEventHandler ScrollPositionChanged;

        public delegate void ScrollEventHandler(object sender, int tOffset);


        public virtual void OnScrollPositionChanged(int tOffset)
        {
            ScrollPositionChanged?.Invoke(this, tOffset);//Raise the event
        }
    }



    public class InfiniumHorizontalScrollBar : Control
    {
        public int iTotalControlWidth = 0;//height of scrollable container (area)
        public int iScrollWheelOffset = 30;
        int iOffset = 0;

        Rectangle rHorizontalScrollShaftRect;

        bool bThumbTracking = false;
        bool bThumbClicked = false;
        int iThumbYClicked = -1;


        public int ThumbSize = 0;//height of thumb
        public decimal iThumbPosition = 0;//current position of thumb on scrollshaft
        public decimal ThumbOneStepWidth = 0;//value offset scrollable area on one step of a thumb
        public int ThumbStepsCount = 0;//total count of thumb steps (in pixels) on scrollshaft
        public decimal ThumbStepWidthOnScrollWheel = 0;//thumb offset on scrollshaft (Y) on every scrollwheeloffset


        Color cHorizontalScrollCommonShaftBackColor = Color.LightBlue;
        Color cHorizontalScrollCommonThumbButtonColor = Color.DarkGray;
        Color cHorizontalScrollTrackingShaftBackColor = Color.Blue;
        Color cHorizontalScrollTrackingThumbButtonColor = Color.Gray;

        SolidBrush brHorizontalScrollCommonShaftBackBrush;
        SolidBrush brHorizontalScrollCommonThumbButtonBrush;
        SolidBrush brHorizontalScrollTrackingShaftBackBrush;
        SolidBrush brHorizontalScrollTrackingThumbButtonBrush;

        public InfiniumHorizontalScrollBar()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            brHorizontalScrollCommonShaftBackBrush = new SolidBrush(cHorizontalScrollCommonShaftBackColor);
            brHorizontalScrollCommonThumbButtonBrush = new SolidBrush(cHorizontalScrollCommonThumbButtonColor);
            brHorizontalScrollTrackingShaftBackBrush = new SolidBrush(cHorizontalScrollTrackingShaftBackColor);
            brHorizontalScrollTrackingThumbButtonBrush = new SolidBrush(cHorizontalScrollTrackingThumbButtonColor);

            rHorizontalScrollShaftRect = new Rectangle(0, 0, 0, 0);

            Visible = true;
        }

        public int ScrollWheelOffset
        {
            get { return iScrollWheelOffset; }
            set { iScrollWheelOffset = value; this.Refresh(); }
        }

        public int TotalControlWidth
        {
            get { return iTotalControlWidth; }
            set
            {
                iTotalControlWidth = value; InitializeThumb(); SetVisibility();
                this.Refresh();
            }
        }


        public int Offset
        {
            get { return iOffset; }
            set
            {
                iOffset = value;

                if (iScrollWheelOffset > 0)
                {
                    iThumbPosition = Convert.ToInt32(Decimal.Round(Offset * ThumbStepWidthOnScrollWheel / iScrollWheelOffset,
                                        0, MidpointRounding.AwayFromZero));
                    //if (iThumbPosition + ThumbSize > this.Height)
                    //    iThumbPosition = this.Height - Convert.ToInt32(ThumbSize);
                }

                this.Refresh();
            }
        }


        public Color HorizontalScrollCommonShaftBackColor
        {
            get { return cHorizontalScrollCommonShaftBackColor; }
            set { cHorizontalScrollCommonShaftBackColor = value; brHorizontalScrollCommonShaftBackBrush.Color = value; this.Refresh(); }
        }

        public Color HorizontalScrollCommonThumbButtonColor
        {
            get { return cHorizontalScrollCommonThumbButtonColor; }
            set { cHorizontalScrollCommonThumbButtonColor = value; brHorizontalScrollCommonThumbButtonBrush.Color = value; this.Refresh(); }
        }

        public Color HorizontalScrollTrackingShaftBackColor
        {
            get { return cHorizontalScrollTrackingShaftBackColor; }
            set { cHorizontalScrollTrackingShaftBackColor = value; brHorizontalScrollTrackingShaftBackBrush.Color = value; this.Refresh(); }
        }

        public Color HorizontalScrollTrackingThumbButtonColor
        {
            get { return cHorizontalScrollTrackingThumbButtonColor; }
            set { cHorizontalScrollTrackingThumbButtonColor = value; brHorizontalScrollTrackingThumbButtonBrush.Color = value; this.Refresh(); }
        }



        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (TotalControlWidth == 0 || this.Height == 0 || ScrollWheelOffset == 0 || this.Width == 0)
                return;

            if (TotalControlWidth > this.Width)
            {
                DrawHorizontalScrollShaft(e.Graphics);
                DrawHorizontalScrollThumb(e.Graphics);
            }
        }


        private void DrawHorizontalScrollShaft(Graphics G)
        {
            //Shaft
            if (bThumbTracking)
                G.FillRectangle(brHorizontalScrollTrackingShaftBackBrush, rHorizontalScrollShaftRect);
            else
                G.FillRectangle(brHorizontalScrollCommonShaftBackBrush, rHorizontalScrollShaftRect);
        }

        public void InitializeThumb()
        {
            if (TotalControlWidth == 0 || ScrollWheelOffset == 0)
                return;


            decimal V = this.Width;
            decimal T = Convert.ToDecimal(TotalControlWidth);

            decimal Rtv = V / (T / 100);

            ThumbSize = Convert.ToInt32(Decimal.Round(Rtv * (V / 100), 0, MidpointRounding.AwayFromZero));

            if (ThumbSize >= V)
                return;

            ThumbStepWidthOnScrollWheel = (V - ThumbSize) / ((T - V) / ScrollWheelOffset);//ширина шага ползунка при одном смещении

            int posY = Convert.ToInt32(Decimal.Round((Convert.ToDecimal(iOffset) / Convert.ToDecimal(ScrollWheelOffset)) *
                                                        ThumbStepWidthOnScrollWheel, 0, MidpointRounding.AwayFromZero));

            if (posY + ThumbSize > V)
                posY = Convert.ToInt32(V - ThumbSize);

            ThumbStepsCount = this.Width - ThumbSize;

            ThumbOneStepWidth = (Convert.ToDecimal(TotalControlWidth) - this.Width) / ThumbStepsCount;

            iThumbPosition = posY;
        }

        private void DrawHorizontalScrollThumb(Graphics G)
        {
            if (!bThumbTracking)
                G.FillRectangle(brHorizontalScrollCommonThumbButtonBrush, new Rectangle(Convert.ToInt32(Decimal.Round(iThumbPosition, 0, MidpointRounding.AwayFromZero)),
                                                                                        rHorizontalScrollShaftRect.Y + 2,
                                                                                        Convert.ToInt32(ThumbSize), this.Height - 4));
            else
                G.FillRectangle(brHorizontalScrollTrackingThumbButtonBrush, new Rectangle(Convert.ToInt32(Decimal.Round(iThumbPosition, 0, MidpointRounding.AwayFromZero)),
                                                                                        rHorizontalScrollShaftRect.Y + 2,
                                                                                        Convert.ToInt32(ThumbSize), this.Height - 4));
        }

        public void SetPosition(int Pos)
        {
            if (Pos * ThumbOneStepWidth > TotalControlWidth - this.Width)
            {
                iOffset = TotalControlWidth - this.Width;
                iThumbPosition = this.Width - Convert.ToInt32(ThumbSize);

                this.Refresh();

                OnScrollPositionChanged(Offset);

                return;
            }
            else
                if (Pos * ThumbOneStepWidth < 0)
            {
                iThumbPosition = 0;
                iOffset = 0;
                this.Refresh();

                OnScrollPositionChanged(Offset);

                return;
            }
            else
            {
                iOffset = Convert.ToInt32(ThumbOneStepWidth * Pos);
            }


            iThumbPosition = Pos;

            this.Refresh();

            OnScrollPositionChanged(Offset);
        }


        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            InitializeThumb();

            rHorizontalScrollShaftRect.Height = this.Height;
            rHorizontalScrollShaftRect.Width = this.Width;

            this.Refresh();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            //if (e.Y < iThumbYClicked)
            //{
            //    if (bThumbClicked)
            //    {
            //        SetPosition(0);
            //        return;
            //    }
            //}

            //if (e.Y > this.Height - iThumbYClicked)
            //{
            //    if (bThumbClicked)
            //    {
            //        SetPosition(ThumbStepsCount);
            //        return;
            //    }
            //}

            if (bThumbClicked)
            {
                SetPosition(e.X - iThumbYClicked);
                return;
            }

            if (e.X > iThumbPosition && e.X < iThumbPosition + ThumbSize)
            {
                if (this.Cursor != Cursors.Hand)
                {
                    bThumbTracking = true;
                    this.Cursor = Cursors.Hand;
                    this.Refresh();
                }

                iThumbYClicked = e.X - Convert.ToInt32(Decimal.Round(iThumbPosition, 0, MidpointRounding.AwayFromZero));
            }
            else
            {
                if (this.Cursor != Cursors.Default)
                {
                    bThumbTracking = false;
                    this.Cursor = Cursors.Default;
                    this.Refresh();
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (bThumbTracking)
                bThumbClicked = true;
            else
                bThumbClicked = false;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            iThumbYClicked = -1;
            bThumbClicked = false;
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);

            bThumbClicked = false;
            bThumbTracking = false;
            this.Refresh();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (!bThumbClicked)
            {
                bThumbClicked = false;
                bThumbTracking = false;
                Cursor = Cursors.Default;
                this.Refresh();
            }
        }

        public void SetVisibility()
        {
            if (this.Width >= TotalControlWidth)
                this.Visible = false;
            else
                this.Visible = true;
        }


        public event ScrollEventHandler ScrollPositionChanged;

        public delegate void ScrollEventHandler(object sender, int tOffset);


        public virtual void OnScrollPositionChanged(int tOffset)
        {
            ScrollPositionChanged?.Invoke(this, tOffset);//Raise the event
        }
    }



    public struct UserImagesStruct
    {
        public int UserID;
        public int UserTypeID;
        public Bitmap Photo;
    }


    public class InfiniumAttachItem : Control
    {
        string sExtension = "";
        int iFileSize = 0;
        string sKBFileSize = "";
        string sFileName = "";
        int iNewsAttachID = -1;


        SolidBrush brFileNameBrush;
        SolidBrush brFileSizeBrush;

        Font fFileNameFont;
        Font fFileSizeFont;

        Color cFileNameColor;
        Color cFileSizeColor;

        int iFNW = 0;
        int iFNH = 0;
        int iFSW = 0;
        int iFSH = 0;

        public InfiniumAttachItem()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            cFileNameColor = Color.ForestGreen;
            cFileSizeColor = Color.DarkGray;

            brFileNameBrush = new SolidBrush(cFileNameColor);
            brFileSizeBrush = new SolidBrush(cFileSizeColor);

            fFileNameFont = new Font("Segoe UI", 16.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            fFileSizeFont = new Font("Segoe UI", 15.0f, FontStyle.Regular, GraphicsUnit.Pixel);

            this.Cursor = Cursors.Hand;

            this.BackColor = Color.Transparent;

            //this.Height = GetInitialHeight();
            //this.Width = 150;
        }

        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        var parms = base.CreateParams;
        //        parms.Style &= ~0x02000000;  // Turn off WS_CLIPCHILDREN
        //        return parms;
        //    }
        //}

        private void SetInitialHeight()
        {
            using (Graphics G = this.CreateGraphics())
            {
                this.Height = Convert.ToInt32(G.MeasureString(sFileName, fFileNameFont).Height) + 1;
            }
        }

        private void SetInitialWidth()
        {
            using (Graphics G = this.CreateGraphics())
            {
                this.Width = Convert.ToInt32(G.MeasureString(sFileName, fFileNameFont).Width)
                         + Convert.ToInt32(G.MeasureString(" (" + sKBFileSize + ")", fFileSizeFont).Width) + 2;
            }
        }


        public Font FileNameFont
        {
            get { return fFileNameFont; }
            set { fFileNameFont = value; this.Refresh(); }
        }

        public Font FileSizeFont
        {
            get { return fFileSizeFont; }
            set { fFileSizeFont = value; this.Refresh(); }
        }

        public string Extension
        {
            get { return sExtension; }
            set { sExtension = value; }
        }

        public int NewsAttachID
        {
            get { return iNewsAttachID; }
            set { iNewsAttachID = value; }
        }

        public int FileSize
        {
            get { return iFileSize; }
            set { iFileSize = value; }
        }

        public string KBFileSize
        {
            get { return sKBFileSize; }
            set { sKBFileSize = value; }
        }

        public string FileName
        {
            get { return sFileName; }
            set { sFileName = value; SetInitialHeight(); SetInitialWidth(); }
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

            if (this.Text == "")
                return;

            if (sFileName == "")
                return;

            if (iFileSize == 0)
                return;

            iFNW = Convert.ToInt32(e.Graphics.MeasureString(sFileName, fFileNameFont).Width);
            iFNH = Convert.ToInt32(e.Graphics.MeasureString(sFileName, fFileNameFont).Height);
            iFSW = Convert.ToInt32(e.Graphics.MeasureString(" (" + sKBFileSize + ")", fFileSizeFont).Width);
            iFSH = Convert.ToInt32(e.Graphics.MeasureString(" (" + sKBFileSize + ")", fFileSizeFont).Height);

            e.Graphics.DrawString(sFileName, fFileNameFont, brFileNameBrush, 0, 0);
            e.Graphics.DrawString("(" + sKBFileSize + ")", fFileSizeFont, brFileSizeBrush, iFNW - 5, 2);
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            OnFileLabelClick(NewsAttachID);
        }


        public event FileLabelClickedEventHandler FileLabelClicked;

        public delegate void FileLabelClickedEventHandler(object sender, int NewsAttachID);

        public virtual void OnFileLabelClick(int NewsAttachID)
        {
            FileLabelClicked?.Invoke(this, NewsAttachID);//Raise the event
        }
    }


    public class InfiniumNewsAttachmentsList : Control
    {
        public InfiniumAttachItem[] FileLabel;

        Label AllFilesLabel;

        int iMarginToNextLabel = 5;
        int iMarginForCaptionLabel = 25;

        SolidBrush brFontBrush;

        DataRow[] dRows;

        int MaxCount = 3;

        bool bAllFilesShowed = false;

        public int StandardHeight = 0;

        Bitmap ImageFileBMP = Properties.Resources.ImageFile;
        Bitmap PDFFileBMP = Properties.Resources.PDFFile;
        Bitmap ExcelFileBMP = Properties.Resources.ExcelFile;
        Bitmap WordFileBMP = Properties.Resources.WordFile;
        Bitmap ArchiveFileBMP = Properties.Resources.ArchiveFile;
        Bitmap OtherFileBMP = Properties.Resources.OtherFile;


        public InfiniumNewsAttachmentsList(DataRow[] Rows)
        {
            dRows = Rows;

            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            //SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            this.Font = new System.Drawing.Font("Segoe UI", 16.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            this.ForeColor = Color.Gray;

            brFontBrush = new SolidBrush(this.ForeColor);

            AllFilesLabel = new Label()
            {
                Font = new Font("Segoe UI", 15.0f, FontStyle.Regular, GraphicsUnit.Pixel),
                ForeColor = Color.FromArgb(56, 184, 238),
                Left = 2,
                Top = 0,
                Cursor = Cursors.Hand
            };
            AllFilesLabel.Click += OnAllFilesLabelClick;
            AllFilesLabel.AutoSize = true;
            AllFilesLabel.Visible = false;
            AllFilesLabel.Parent = this;

            this.BackColor = Color.Transparent;

            Create(dRows);
        }


        public void Create(DataRow[] Rows)
        {
            if (FileLabel == null)
            {
                FileLabel = new InfiniumAttachItem[Rows.Count()];

                if (Rows.Count() == 0)
                    return;

                this.Height += iMarginForCaptionLabel;

                for (int i = 0; i < Rows.Count(); i++)
                {
                    FileLabel[i] = new InfiniumAttachItem()
                    {
                        Visible = true,
                        Parent = this
                    };
                    FileLabel[i].FileLabelClicked += OnLabelClick;
                    FileLabel[i].Text = GetFileNameWithoutExt(Rows[i]["FileName"].ToString());
                    FileLabel[i].Extension = GetExtension(Rows[i]["FileName"].ToString());
                    FileLabel[i].FileSize = Convert.ToInt32(Rows[i]["FileSize"]);
                    FileLabel[i].KBFileSize = GetFileSizeInKB(FileLabel[i].FileSize);
                    FileLabel[i].FileName = Rows[i]["FileName"].ToString();
                    FileLabel[i].NewsAttachID = Convert.ToInt32(Rows[i]["NewsAttachID"]);
                    FileLabel[i].Left = 2 + 28;//28 - imageWidth;


                    int iLabelHeight = GetLabelHeight(FileLabel[i]);


                    if (i < MaxCount || bAllFilesShowed == true)
                    {
                        FileLabel[i].Top = i * (iLabelHeight + iMarginToNextLabel) + iMarginForCaptionLabel;
                        //FileLabel[i].Visible = true;
                        this.Height += iLabelHeight + iMarginToNextLabel;
                    }
                    else
                    {
                        FileLabel[i].Visible = false;

                        if (AllFilesLabel.Visible == false)
                        {
                            this.Height += AllFilesLabel.Height + 2;
                            AllFilesLabel.Text = "Все файлы(" + Rows.Count().ToString() + ")";
                            AllFilesLabel.Top = this.Height - AllFilesLabel.Height;
                            AllFilesLabel.Visible = true;
                        }
                    }
                }

                //this.Refresh();
            }
        }

        public void ShowAllFiles()
        {
            StandardHeight = this.Height;

            this.Height = 1;

            AllFilesLabel.Visible = false;

            this.Height += iMarginForCaptionLabel;

            for (int i = 0; i < FileLabel.Count(); i++)
            {
                int iLabelHeight = GetLabelHeight(FileLabel[i]);

                FileLabel[i].Top = i * (iLabelHeight + iMarginToNextLabel) + iMarginForCaptionLabel;
                FileLabel[i].Visible = true;
                this.Height += iLabelHeight + iMarginToNextLabel;
            }
        }


        public int GetLabelHeight(InfiniumAttachItem Label)
        {
            using (Graphics G = Label.CreateGraphics())
            {
                return Convert.ToInt32(G.MeasureString(Label.Text, Label.FileNameFont).Height);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (FileLabel == null)
                return;

            for (int i = 0; i < FileLabel.Count(); i++)
            {
                FileLabel[i].Dispose();
            }
        }

        private string GetFileSizeInKB(int FileSize)
        {
            if (FileSize < 1000)//байты
                return FileSize.ToString() + " Б";
            else
            {
                decimal Ks = Decimal.Round(Convert.ToDecimal(FileSize) / 1024, 0, MidpointRounding.AwayFromZero);

                return String.Format("{0:### ### ### ### ### ###}", Convert.ToInt32(Ks)).TrimStart() + " КБ";
            }
        }

        private string GetFileNameWithoutExt(string FileName)
        {
            return System.IO.Path.GetFileNameWithoutExtension(FileName);
        }

        private string GetExtension(string FileName)
        {
            return System.IO.Path.GetExtension(FileName);
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

            e.Graphics.DrawString("Файлы:", this.Font, brFontBrush, 0, 0);

            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            int Count = 0;

            if (FileLabel.Count() > MaxCount && !bAllFilesShowed)
                Count = MaxCount;
            else
                Count = FileLabel.Count();


            for (int i = 0; i < Count; i++)
            {

                if (FileLabel[i].Extension.Equals(".pdf", StringComparison.InvariantCultureIgnoreCase))
                {
                    PDFFileBMP.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);
                    e.Graphics.DrawImage(PDFFileBMP, 4, FileLabel[i].Top - 2, 24, 24);
                }
                else
                    if (FileLabel[i].Extension.Equals(".doc", StringComparison.InvariantCultureIgnoreCase) ||
                        FileLabel[i].Extension.Equals(".docx", StringComparison.InvariantCultureIgnoreCase))
                {
                    WordFileBMP.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);
                    e.Graphics.DrawImage(WordFileBMP, 4, FileLabel[i].Top - 2, 24, 24);
                }
                else
                        if (FileLabel[i].Extension.Equals(".xls", StringComparison.InvariantCultureIgnoreCase) ||
                            FileLabel[i].Extension.Equals(".xlsx", StringComparison.InvariantCultureIgnoreCase))
                {
                    ExcelFileBMP.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);
                    e.Graphics.DrawImage(ExcelFileBMP, 4, FileLabel[i].Top - 2, 24, 24);
                }
                else
                            if (FileLabel[i].Extension.Equals(".zip", StringComparison.InvariantCultureIgnoreCase) ||
                                FileLabel[i].Extension.Equals(".rar", StringComparison.InvariantCultureIgnoreCase))
                {
                    ArchiveFileBMP.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);
                    e.Graphics.DrawImage(ArchiveFileBMP, 4, FileLabel[i].Top - 2, 24, 24);
                }
                else
                                if (FileLabel[i].Extension.Equals(".jpg", StringComparison.InvariantCultureIgnoreCase) ||
                                    FileLabel[i].Extension.Equals(".jpeg", StringComparison.InvariantCultureIgnoreCase) ||
                                    FileLabel[i].Extension.Equals(".png", StringComparison.InvariantCultureIgnoreCase) ||
                                    FileLabel[i].Extension.Equals(".bmp", StringComparison.InvariantCultureIgnoreCase) ||
                                    FileLabel[i].Extension.Equals(".tiff", StringComparison.InvariantCultureIgnoreCase) ||
                                    FileLabel[i].Extension.Equals(".tif", StringComparison.InvariantCultureIgnoreCase) ||
                                    FileLabel[i].Extension.Equals(".gif", StringComparison.InvariantCultureIgnoreCase) ||
                                    FileLabel[i].Extension.Equals(".tga", StringComparison.InvariantCultureIgnoreCase) ||
                                    FileLabel[i].Extension.Equals(".psd", StringComparison.InvariantCultureIgnoreCase) ||
                                    FileLabel[i].Extension.Equals(".wmf", StringComparison.InvariantCultureIgnoreCase) ||
                                    FileLabel[i].Extension.Equals(".emf", StringComparison.InvariantCultureIgnoreCase))
                {
                    ImageFileBMP.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);
                    e.Graphics.DrawImage(ImageFileBMP, 4, FileLabel[i].Top - 2, 24, 24);
                }
                else
                {
                    OtherFileBMP.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);
                    e.Graphics.DrawImage(OtherFileBMP, 4, FileLabel[i].Top - 2, 24, 24);
                }
            }

            //e.Graphics.DrawRectangle(new Pen(Color.Black, 1), new Rectangle(1, 1, this.Width - 2, this.Height - 2));
        }

        public void OnLabelClick(object sender, int NewsAttachID)
        {
            OnFileLabelClick(NewsAttachID);
        }

        public void OnAllFilesLabelClick(object sender, EventArgs e)
        {
            bAllFilesShowed = true;
            ShowAllFiles();

            OnAllFilesClick();
        }

        public event FileLabelClickedEventHandler FileLabelClicked;
        public event EventHandler AllFilesClicked;

        public delegate void FileLabelClickedEventHandler(object sender, int NewsAttachID);

        public virtual void OnFileLabelClick(int NewsAttachID)
        {
            FileLabelClicked?.Invoke(this, NewsAttachID);//Raise the event
        }

        public virtual void OnAllFilesClick()
        {
            AllFilesClicked?.Invoke(this, new EventArgs());//Raise the event
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            Parent.Focus();
        }
    }


    public class InfiniumNewsContainer : Control
    {
        public int Offset = 0;

        int iTempScrollWheelOffset = 0;

        int iMarginToNextItem = 21;

        public DataTable ClientsManagersDT;
        public DataTable ManagersDT;

        public int NewsCount = 0;

        public bool bNeedMoreNews = false;

        public InfiniumVerticalScrollBar sbVerticalScrollBar = new InfiniumVerticalScrollBar();

        public DataTable NewsDT;
        public DataTable DepartmentDT;
        public DataTable UsersDT;
        public DataTable CommentsDT;
        public DataTable AttachsDT;
        public DataTable NewsLikesDT;
        public DataTable CommentsLikesDT;

        public InfiniumNewsItem[] NewsItems;

        public InfiniumScrollContainer ScrollContainer;

        private UserImagesStruct[] UserImages;

        int iCurrentUserID;

        public InfiniumNewsContainer()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);

            sbVerticalScrollBar.Parent = this;
            sbVerticalScrollBar.Height = this.Height;
            sbVerticalScrollBar.Left = this.Width - sbVerticalScrollBar.Width;
            sbVerticalScrollBar.Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            sbVerticalScrollBar.ScrollPositionChanged += OnScrollPositionChanged;

            ScrollContainer = new InfiniumScrollContainer()
            {
                Parent = this,
                Top = 0,
                Left = 0,
                Width = this.Width - sbVerticalScrollBar.Width,
                Height = this.Height,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            ScrollContainer.SizeChanged += OnScrollContainerSizeChanged;

            this.BackColor = Color.White;
        }


        [DllImport("user32.dll", SetLastError = true)]
        static extern UInt32 GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, UInt32 dwNewLong);



        public void SetNoClip()
        {
            UInt32 f = 0x02000000;

            SetWindowLong(ScrollContainer.Handle, -16, ((GetWindowLong(ScrollContainer.Handle, -16) & ~f)));
        }

        public void SetClipStandard()
        {
            SetWindowLong(ScrollContainer.Handle, -16, ((GetWindowLong(this.Handle, -16))));
        }


        public int CurrentUserID
        {
            get { return iCurrentUserID; }
            set { iCurrentUserID = value; }
        }

        public DataTable UsersDataTable
        {
            get { return UsersDT; }
            set
            {
                UsersDT = value;

                if (value == null)
                    return;

                int index = 0;

                UserImages = new UserImagesStruct[UsersDT.Rows.Count];

                foreach (DataRow Row in UsersDT.Rows)
                {
                    if (Row["Photo"] != DBNull.Value)
                    {
                        byte[] b = (byte[])Row["Photo"];

                        using (MemoryStream ms = new MemoryStream(b))
                        {
                            Bitmap Bmp = new Bitmap(ms);

                            UserImages[index].UserID = Convert.ToInt32(Row["UserID"]);
                            UserImages[index].UserTypeID = Convert.ToInt32(Row["SenderTypeID"]);
                            UserImages[index].Photo = new Bitmap(Bmp);

                            Bmp.Dispose();
                        }
                    }

                    index++;
                }

                GC.Collect();
            }
        }

        public DataTable NewsDataTable
        {
            get { return NewsDT; }
            set
            {
                NewsDT = value;

                //if (value == null)
                //    return;

                //if (NewsDT.Columns["Page"] == null)
                //    NewsDT.Columns.Add(new DataColumn("Page", System.Type.GetType("System.Int32")));

                //int iPage = 0;
                //int iN = 0;

                //for (int i = 0; i < NewsDT.Rows.Count; i++)
                //{
                //    NewsDT.Rows[i]["Page"] = iPage;

                //    iN++;

                //    if (iN >= iMaxNewsItemsOnPage)
                //    {
                //        iN = 0;
                //        iPage++;
                //    }
                //}

                //if (PageSelector != null)
                //{
                //    NewsDT.DefaultView.RowFilter = "Page = " + PageSelector.SelectedPage;
                //    NewsDT.DefaultView.Sort = "NewsID ASC";
                //}
            }
        }

        public void CreateNews()
        {
            NewsCount = NewsDT.Rows.Count;

            if (NewsItems != null)
            {
                for (int i = 0; i < NewsItems.Count(); i++)
                {
                    if (NewsItems[i] != null)
                        NewsItems[i].Dispose();
                }
            }

            NewsItems = new InfiniumNewsItem[NewsDT.DefaultView.Count];

            if (NewsDT.DefaultView.Count == 0)
                return;

            int CurTextPosY = 0;

            ScrollContainer.Height = 0;
            ScrollContainer.Width = this.Width - sbVerticalScrollBar.Width;

            for (int i = 0; i < NewsDT.DefaultView.Count; i++)
            {
                NewsItems[i] = new InfiniumNewsItem(CurrentUserID, ref UserImages, UsersDataTable)
                {
                    Parent = ScrollContainer,
                    Width = ScrollContainer.Width,
                    NewsID = Convert.ToInt32(NewsDT.Rows[i]["NewsID"]),
                    SenderID = Convert.ToInt32(NewsDT.Rows[i]["SenderID"]),
                    ManagersDT = ManagersDT,
                    ClientsManagersDT = ClientsManagersDT
                };
                if (NewsDT.Columns["SenderTypeID"] != null)
                {
                    if (NewsDT.Rows[i]["SenderTypeID"].ToString() == "0")
                    {
                        int index = Array.FindIndex(UserImages, UsIm => UsIm.UserID == NewsItems[i].SenderID);

                        if (index > -1)
                        {
                            NewsItems[i].SenderImage = UserImages[index].Photo;
                            NewsItems[i].SenderName = UsersDT.Select("UserID = " + NewsDT.Rows[i]["SenderID"])[0]["Name"].ToString();
                        }
                    }

                    if (NewsDT.Rows[i]["SenderTypeID"].ToString() == "1")
                    {
                        NewsItems[i].SenderName = ManagersDT.Select("ManagerID = " + NewsDT.Rows[i]["SenderID"])[0]["Name"].ToString();
                    }

                    if (NewsDT.Rows[i]["SenderTypeID"].ToString() == "2")
                    {
                        NewsItems[i].SenderName = ManagersDT.Select("ClientID = " + NewsDT.Rows[i]["SenderID"])[0]["ClientName"].ToString();
                    }
                    if (NewsDT.Rows[i]["SenderTypeID"].ToString() == "3")
                    {
                        NewsItems[i].SenderName = ClientsManagersDT.Select("ManagerID = " + NewsDT.Rows[i]["SenderID"])[0]["Name"].ToString();
                    }
                }
                else
                {
                    int index = Array.FindIndex(UserImages, UsIm => UsIm.UserID == NewsItems[i].SenderID);

                    if (index > -1)
                    {
                        NewsItems[i].SenderImage = UserImages[index].Photo;
                        NewsItems[i].SenderName = UsersDT.Select("UserID = " + NewsDT.Rows[i]["SenderID"])[0]["Name"].ToString();
                    }
                }

                NewsItems[i].CommentsLikesRows = CommentsLikesDT.Select("NewsID = " + NewsDT.DefaultView[i]["NewsID"]);
                NewsItems[i].NewsLikesRows = NewsLikesDT.Select("NewsID = " + NewsDT.DefaultView[i]["NewsID"]);
                NewsItems[i].AttachmentsRows = AttachsDT.Select("NewsID = " + NewsDT.DefaultView[i]["NewsID"]);
                NewsItems[i].CommentsRows = CommentsDT.Select("NewsID = " + NewsDT.DefaultView[i]["NewsID"]);
                NewsItems[i].Date = NewsDT.Rows[i]["DateTime"].ToString();
                NewsItems[i].HeaderText = NewsDT.Rows[i]["HeaderText"].ToString();
                NewsItems[i].NewsText = NewsDT.Rows[i]["BodyText"].ToString();
                NewsItems[i].Top = CurTextPosY;
                if (NewsDT.Rows[i]["New"] != DBNull.Value)
                    NewsItems[i].IsNew = true;
                if (NewsDT.Rows[i]["NewComments"] != DBNull.Value)
                    NewsItems[i].NewCommentsCount = Convert.ToInt32(NewsDT.Rows[i]["NewComments"]);
                NewsItems[i].Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                NewsItems[i].AllCommentsLabelClicked += OnAllCommentsClick;
                NewsItems[i].CommentsLabelClicked += OnCommentsClick;
                NewsItems[i].CommentsCancelButtonClicked += OnCommentsCancelButtonClick;
                NewsItems[i].AttachsAllFilesClicked += OnAttachsAllFilesClick;
                NewsItems[i].SendButtonClicked += OnCommentsSendButtonClick;
                NewsItems[i].CommentsRemoveClicked += OnCommentsRemoveButtonClick;
                NewsItems[i].CommentsEditClicked += OnCommentsEditButtonClick;
                NewsItems[i].CommentsCommentClicked += OnCommentsCommentClick;
                NewsItems[i].CommentsTextBoxSizeChanged += OnCommentsTextBoxSizeChanged;
                NewsItems[i].RemoveLabelClicked += OnRemoveClicked;
                NewsItems[i].EditLabelClicked += OnEditClicked;
                NewsItems[i].AttachClicked += OnAttachClicked;
                NewsItems[i].CommentsLikeClicked += OnCommentsLikeClick;
                NewsItems[i].LikeClicked += OnLikeClick;
                NewsItems[i].QuoteLabelClicked += OnNewsQuoteLabelClick;
                NewsItems[i].CommentsQuoteLabelClicked += OnCommentsQuoteLabelClick;

                CurTextPosY += NewsItems[i].Height + iMarginToNextItem;

                if (ScrollContainer.Height < CurTextPosY)
                    if (i != NewsDT.DefaultView.Count - 1)
                        ScrollContainer.Height += NewsItems[i].Height + iMarginToNextItem;
                    else
                        ScrollContainer.Height += NewsItems[i].Height;
            }

            if (ScrollContainer.Height < this.Height)
                ScrollContainer.Height = this.Height;

            VerticalScrollBar.Visible = true;
            VerticalScrollBar.TotalControlHeight = ScrollContainer.Height;
            VerticalScrollBar.Refresh();
        }


        public void ScrollToNews(int index)
        {
            Offset = 0;

            Offset = NewsItems[index].Top - this.Height - iMarginToNextItem;

            Offset += NewsItems[index].iMarginForImageHeight + iMarginToNextItem + 36;
            ScrollContainer.Top = -Offset;
            VerticalScrollBar.Offset = Offset;
            VerticalScrollBar.Refresh();
        }

        public int FindRow(int NewsID)
        {
            for (int i = 0; i < NewsDT.DefaultView.Count; i++)
            {
                if (Convert.ToInt32(NewsDT.DefaultView[i]["NewsID"]) == NewsID)
                    return i;
            }

            return -1;
        }

        public void SetNewsPositions()
        {
            if (NewsItems == null)
                return;

            int CurTextPosY = 0;

            ScrollContainer.Height = 0;

            for (int i = 0; i < NewsItems.Count(); i++)
            {
                NewsItems[i].Top = CurTextPosY;

                CurTextPosY += NewsItems[i].Height + iMarginToNextItem;

                if (ScrollContainer.Height < CurTextPosY)
                    if (i != NewsDT.DefaultView.Count - 1)
                        ScrollContainer.Height += NewsItems[i].Height + iMarginToNextItem;
                    else
                        ScrollContainer.Height += NewsItems[i].Height;

                NewsItems[i].Refresh();
            }

            if (ScrollContainer.Height < this.Height)
                ScrollContainer.Height = this.Height;

            if (Offset + this.Height > ScrollContainer.Height)
            {
                Offset = ScrollContainer.Height - this.Height;
                ScrollContainer.Top = -Offset;
                VerticalScrollBar.Visible = true;
                VerticalScrollBar.Offset = Offset;
                VerticalScrollBar.Refresh();
            }

            this.Refresh();
        }

        //public void SetNewsPositions(InfiniumNewsItem NewsItem)
        //{
        //    int Height = 0;

        //    int index = -1;

        //    for (int i = 0; i < NewsItems.Count(); i++)
        //    {
        //        if (i != NewsDT.DefaultView.Count - 1)
        //            Height += NewsItems[i].Height + iMarginToNextItem;
        //        else
        //            Height += NewsItems[i].Height;

        //        if (NewsItems[i] == NewsItem)
        //            index = i;
        //    }

        //    if (Height > ScrollContainer.Height)//news item getting bigger
        //    {
        //        for (int i = index + 1; i < NewsItems.Count(); i++)
        //        {
        //            NewsItems[i].Top += Height - ScrollContainer.Height;
        //        }

        //        ScrollContainer.Height += Height - ScrollContainer.Height;
        //    }
        //    else//news item getting smaller
        //    {

        //    }
        //}

        public void ScrollToTop()
        {
            Offset = 0;
            ScrollContainer.Top = -Offset;
            VerticalScrollBar.Offset = Offset;
            VerticalScrollBar.Refresh();
        }

        public void ShowAllComments(int NewsID)
        {
            int i = FindRow(NewsID);

            NewsItems[i].AllCommentsVisible = true;
            this.Focus();
        }

        public void ReloadNewsItem(int NewsID, bool bAllComments)
        {
            int i = FindRow(NewsID);

            int CurTextPosY = 0;

            if (i != -1)//there is no NewsID in this default view (deleted by somebody)
                CurTextPosY = NewsItems[i].Top;

            if (NewsItems[i] != null)
                NewsItems[i].Dispose();

            NewsItems[i] = new InfiniumNewsItem(CurrentUserID, ref UserImages, UsersDataTable)
            {
                Parent = ScrollContainer,
                Width = this.Width,
                AllCommentsVisible = bAllComments,
                ManagersDT = ManagersDT,
                ClientsManagersDT = ClientsManagersDT,
                NewsID = Convert.ToInt32(NewsDT.DefaultView[i]["NewsID"]),
                SenderID = Convert.ToInt32(NewsDT.DefaultView[i]["SenderID"])
            };
            if (NewsDT.Columns["SenderTypeID"] != null)
            {
                if (NewsDT.Rows[i]["SenderTypeID"].ToString() == "0")
                {
                    int index = Array.FindIndex(UserImages, UsIm => UsIm.UserID == NewsItems[i].SenderID);

                    if (index > -1)
                        NewsItems[i].SenderImage = UserImages[index].Photo;
                    NewsItems[i].SenderName = UsersDT.Select("UserID = " + NewsDT.Rows[i]["SenderID"])[0]["Name"].ToString();
                }

                if (NewsDT.Rows[i]["SenderTypeID"].ToString() == "1")
                {
                    NewsItems[i].SenderName = ManagersDT.Select("ManagerID = " + NewsDT.Rows[i]["SenderID"])[0]["Name"].ToString();
                }

                if (NewsDT.Rows[i]["SenderTypeID"].ToString() == "2")
                {
                    NewsItems[i].SenderName = ManagersDT.Select("ClientID = " + NewsDT.Rows[i]["SenderID"])[0]["ClientName"].ToString();
                }
                if (NewsDT.Rows[i]["SenderTypeID"].ToString() == "3")
                {
                    NewsItems[i].SenderName = ClientsManagersDT.Select("ManagerID = " + NewsDT.Rows[i]["SenderID"])[0]["Name"].ToString();
                }
            }
            else
            {
                int index = Array.FindIndex(UserImages, UsIm => UsIm.UserID == NewsItems[i].SenderID);

                if (index > -1)
                    NewsItems[i].SenderImage = UserImages[index].Photo;
                NewsItems[i].SenderName = UsersDT.Select("UserID = " + NewsDT.Rows[i]["SenderID"])[0]["Name"].ToString();
            }
            NewsItems[i].CommentsLikesRows = CommentsLikesDT.Select("NewsID = " + NewsDT.DefaultView[i]["NewsID"]);
            NewsItems[i].NewsLikesRows = NewsLikesDT.Select("NewsID = " + NewsDT.DefaultView[i]["NewsID"]);
            NewsItems[i].AttachmentsRows = AttachsDT.Select("NewsID = " + NewsDT.DefaultView[i]["NewsID"]);
            NewsItems[i].CommentsRows = CommentsDT.Select("NewsID = " + NewsDT.DefaultView[i]["NewsID"]);
            NewsItems[i].Date = NewsDT.DefaultView[i]["DateTime"].ToString();
            NewsItems[i].NewsText = NewsDT.DefaultView[i]["BodyText"].ToString();
            NewsItems[i].HeaderText = NewsDT.DefaultView[i]["HeaderText"].ToString();
            NewsItems[i].Top = CurTextPosY;
            NewsItems[i].AllCommentsLabelClicked += OnAllCommentsClick;
            NewsItems[i].CommentsLabelClicked += OnCommentsClick;
            NewsItems[i].CommentsCancelButtonClicked += OnCommentsCancelButtonClick;
            NewsItems[i].AttachsAllFilesClicked += OnAttachsAllFilesClick;
            NewsItems[i].SendButtonClicked += OnCommentsSendButtonClick;
            NewsItems[i].CommentsRemoveClicked += OnCommentsRemoveButtonClick;
            NewsItems[i].CommentsEditClicked += OnCommentsEditButtonClick;
            NewsItems[i].CommentsCommentClicked += OnCommentsCommentClick;
            NewsItems[i].CommentsTextBoxSizeChanged += OnCommentsTextBoxSizeChanged;
            NewsItems[i].RemoveLabelClicked += OnRemoveClicked;
            NewsItems[i].EditLabelClicked += OnEditClicked;
            NewsItems[i].AttachClicked += OnAttachClicked;
            NewsItems[i].CommentsLikeClicked += OnCommentsLikeClick;
            NewsItems[i].LikeClicked += OnLikeClick;
            NewsItems[i].QuoteLabelClicked += OnNewsQuoteLabelClick;
            NewsItems[i].CommentsQuoteLabelClicked += OnCommentsQuoteLabelClick;

            SetNewsPositions();
        }

        public void ReloadLikes(int NewsID)
        {
            int i = FindRow(NewsID);

            NewsItems[i].NewsLikesRows = NewsLikesDT.Select("NewsID = " + NewsDT.DefaultView[i]["NewsID"]);
            NewsItems[i].CommentsLikesRows = CommentsLikesDT.Select("NewsID = " + NewsDT.DefaultView[i]["NewsID"]);
            NewsItems[i].ReloadLikes();
            NewsItems[i].Refresh();
        }

        public void ScrollToCommentsTextBox(InfiniumNewsItem NewsItem)
        {
            if (Offset + this.Height < NewsItem.Top + NewsItem.Height)
            {
                Offset = NewsItem.Top - this.Height + NewsItem.Height;
                ScrollContainer.Top = -Offset;
                VerticalScrollBar.Offset = Offset;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public InfiniumVerticalScrollBar VerticalScrollBar
        {
            get { return sbVerticalScrollBar; }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }

        //public void RefreshThis()
        //{
        //    OnRefreshed(this, null);
        //}

        private void OnScrollContainerSizeChanged(object sender, EventArgs e)
        {
            sbVerticalScrollBar.TotalControlHeight = ScrollContainer.Height;
            sbVerticalScrollBar.InitializeThumb();
            sbVerticalScrollBar.Refresh();
        }

        private void OnAttachsAllFilesClick(object sender)
        {
            SetNewsPositions();
            this.Refresh();
        }

        private void OnAllCommentsClick(object sender)
        {
            SetNewsPositions();
            this.Refresh();
            this.Focus();
        }

        private void OnCommentsCancelButtonClick(object sender)
        {
            SetNewsPositions();
            this.Refresh();
            this.Focus();
            SetNoClip();
        }

        private void OnAttachLabelClicked(object sender, int NewsAttachID)
        {
            OnAttachClicked(sender, NewsAttachID);
        }

        private void OnLikeClick(object sender)
        {
            OnLikeClicked(sender, ((InfiniumNewsItem)sender).NewsID);
        }

        private void OnCommentsLikeClick(object sender, int NewsID, int NewsCommentID)
        {
            OnCommentLikeClicked(sender, NewsID, NewsCommentID);
        }

        private void OnCommentsClick(object sender)
        {
            SetClipStandard();

            for (int i = 0; i < NewsItems.Count(); i++)
            {
                if (sender != NewsItems[i])
                    NewsItems[i].CloseCommentsTextBox();
            }

            SetNewsPositions();
            ScrollToCommentsTextBox((InfiniumNewsItem)sender);
            SetNoClip();
            this.Refresh();
        }

        private void OnEditClicked(object sender)
        {
            OnEditNewsClicked(sender, ((InfiniumNewsItem)sender).NewsID);
        }

        private void OnRemoveClicked(object sender)
        {
            OnRemoveNewsClicked(sender, ((InfiniumNewsItem)sender).NewsID);
        }

        private void OnCommentsQuoteLabelClick(object sender)
        {
            OnCommentsQuoteLabelClicked(sender);
        }

        private void OnNewsQuoteLabelClick(object sender)
        {
            OnNewsQuoteLabelClicked(sender);
        }

        private void OnCommentsTextBoxSizeChanged(object sender, EventArgs e)
        {
            SetNewsPositions();
            ScrollToCommentsTextBox((InfiniumNewsItem)sender);
        }

        private void OnCommentsCommentClick(object sender)
        {
            SetClipStandard();

            int index = FindRow(((InfiniumNewsCommentItem)sender).NewsID);

            for (int i = 0; i < NewsItems.Count(); i++)
            {
                if (index != i)
                    NewsItems[i].CloseCommentsTextBox();
            }

            SetNoClip();
            SetNewsPositions();
            ScrollToCommentsTextBox(NewsItems[index]);
            this.Refresh();
        }

        private void OnScrollPositionChanged(object sender, int tOffset)
        {
            Offset = tOffset;
            ScrollContainer.Top = -Offset;

            //for more news
            if (Offset > ScrollContainer.Height - this.Height - 350 && NewsCount < 80)
            {
                if (!bNeedMoreNews)
                {
                    bNeedMoreNews = true;
                    OnNeedMoreNews(this, null);
                }
            }
            else
            {
                if (bNeedMoreNews)
                {
                    bNeedMoreNews = false;
                    OnNoNeedMoreNews(this, null);
                }
            }
        }

        private void OnCommentsSendButtonClick(object sender, string Text, bool bEdit, int iNewsCommentID)
        {
            SetNoClip();
            OnSendButtonClicked(sender, Text, ((InfiniumNewsItem)sender).NewsID, CurrentUserID, bEdit, iNewsCommentID);//Security.CurrentUserID
            this.Refresh();
        }

        private void OnCommentsRemoveButtonClick(object sender, int NewsID, int NewsCommentID)
        {
            OnRemoveCommentClicked(sender, NewsID, NewsCommentID);
        }

        private void OnCommentsEditButtonClick(object sender, int NewsID, int NewsCommentID)
        {
            SetNewsPositions();
            OnEditCommentClicked(sender, NewsID, NewsCommentID);
            int i = FindRow(NewsID);
            ScrollToCommentsTextBox(NewsItems[i]);
        }


        public event SendButtonEventHandler CommentSendButtonClicked;
        public event CommentEventHandler RemoveCommentClicked;
        public event CommentEventHandler EditCommentClicked;
        public event CommentEventHandler CommentLikeClicked;
        public event LabelClickEventHandler RemoveNewsClicked;
        public event LabelClickEventHandler EditNewsClicked;
        public event LabelClickEventHandler LikeClicked;
        public event QuoteLabelClikedEventHandler NewsQuoteLabelClicked;
        public event QuoteLabelClikedEventHandler CommentsQuoteLabelClicked;
        public event EventHandler Refreshed;
        public event EventHandler NeedMoreNews;
        public event EventHandler NoNeedMoreNews;
        public event AttachClickedEventHandler AttachClicked;

        public delegate void SendButtonEventHandler(object sender, string Text, int NewsID, int SenderID, bool bEdit, int NewsCommentID);
        public delegate void CommentEventHandler(object sender, int NewsID, int NewsCommentID);
        public delegate void LabelClickEventHandler(object sender, int NewsID);
        public delegate void AttachClickedEventHandler(object sender, int NewsAttachID);
        public delegate void QuoteLabelClikedEventHandler(object sender);


        public virtual void OnNeedMoreNews(object sender, EventArgs e)
        {
            NeedMoreNews?.Invoke(this, e);//Raise the event
        }

        public virtual void OnNoNeedMoreNews(object sender, EventArgs e)
        {
            NoNeedMoreNews?.Invoke(this, e);//Raise the event
        }


        public virtual void OnNewsQuoteLabelClicked(object sender)
        {
            NewsQuoteLabelClicked?.Invoke(this);//Raise the event
        }

        public virtual void OnCommentsQuoteLabelClicked(object sender)
        {
            CommentsQuoteLabelClicked?.Invoke(this);//Raise the event
        }


        public virtual void OnLikeClicked(object sender, int NewsID)
        {
            LikeClicked?.Invoke(this, NewsID);//Raise the event
        }

        public virtual void OnCommentLikeClicked(object sender, int NewsID, int NewsCommentID)
        {
            CommentLikeClicked?.Invoke(this, NewsID, NewsCommentID);//Raise the event
        }


        public virtual void OnAttachClicked(object sender, int NewsAttachID)
        {
            AttachClicked?.Invoke(this, NewsAttachID);//Raise the event
        }

        public virtual void OnRefreshed(object sender, EventArgs e)
        {
            Refreshed?.Invoke(this, e);//Raise the event
        }

        public virtual void OnEditNewsClicked(object sender, int NewsID)
        {
            EditNewsClicked?.Invoke(this, NewsID);//Raise the event
        }

        public virtual void OnRemoveNewsClicked(object sender, int NewsID)
        {
            RemoveNewsClicked?.Invoke(this, NewsID);//Raise the event
        }

        public virtual void OnSendButtonClicked(object sender, string Text, int NewsID, int SenderID, bool bEdit, int NewsCommentID)
        {
            CommentSendButtonClicked?.Invoke(this, Text, NewsID, SenderID, bEdit, NewsCommentID);//Raise the event
        }

        public virtual void OnRemoveCommentClicked(object sender, int NewsID, int NewsCommentID)
        {
            RemoveCommentClicked?.Invoke(this, NewsID, NewsCommentID);//Raise the event
        }

        public virtual void OnEditCommentClicked(object sender, int NewsID, int NewsCommentID)
        {
            EditCommentClicked?.Invoke(this, NewsID, NewsCommentID);//Raise the event
        }



        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            ScrollContainer.Focus();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            if (VerticalScrollBar != null)
            {
                VerticalScrollBar.Width = 14;
                VerticalScrollBar.Left = this.Width - VerticalScrollBar.Width;
                VerticalScrollBar.TotalControlHeight = ScrollContainer.Height;
                VerticalScrollBar.Height = this.Height;
                VerticalScrollBar.Refresh();
            }

            SetNewsPositions();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (!VerticalScrollBar.Visible)
                return;

            if (Offset == 0)
                if (iTempScrollWheelOffset > 0)
                    iTempScrollWheelOffset = 0;

            if (Offset > ScrollContainer.Height - this.Height - 350 && NewsCount < 80)
            {
                if (!bNeedMoreNews)
                {
                    bNeedMoreNews = true;
                    OnNeedMoreNews(this, null);
                }
            }
            else
            {
                if (bNeedMoreNews)
                {
                    bNeedMoreNews = false;
                    OnNoNeedMoreNews(this, null);
                }
            }

            if (e.Delta < 0)
            {
                if (Offset < ScrollContainer.Height - this.Height)
                {
                    if (Offset + VerticalScrollBar.ScrollWheelOffset + this.Height > ScrollContainer.Height)//last
                    {
                        iTempScrollWheelOffset = ScrollContainer.Height - this.Height - Offset;
                        Offset += iTempScrollWheelOffset;
                    }
                    else
                    {
                        Offset += VerticalScrollBar.ScrollWheelOffset;
                    }

                    ScrollContainer.Top = -Offset;
                    VerticalScrollBar.Offset = Offset;
                    VerticalScrollBar.Refresh();
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
                        Offset -= VerticalScrollBar.ScrollWheelOffset;

                    if (Offset < 0)
                        Offset = 0;

                    OnNoNeedMoreNews(this, null);

                    ScrollContainer.Top = -Offset;
                    VerticalScrollBar.Offset = Offset;
                    VerticalScrollBar.Refresh();
                }
        }
    }


    public class InfiniumNewsItem : Control
    {
        Bitmap imSenderImage;

        public DataTable ClientsManagersDT;
        public DataTable ManagersDT;

        Color cHeaderFontColor = Color.FromArgb(18, 164, 217);
        Color cSenderFontColor = Color.Gray;
        Color cTextFontColor = Color.FromArgb(30, 30, 30);
        Color cDarkSplitterColor = Color.LightGray;
        Color cLightSplitterColor = Color.FromArgb(240, 240, 240);
        Color cNewBackColor = Color.FromArgb(255, 250, 228);

        Font fHeaderFont;
        Font fSenderFont;
        Font fTextFont;

        Pen pImageBorderPen;
        Pen pDarkSplitterPen;
        Pen pLightSplitterPen;

        Rectangle ImageRect;

        UserImagesStruct[] UsersImages;

        SolidBrush brHeaderFontBrush;
        SolidBrush brSenderFontBrush;
        SolidBrush brTextFontBrush;
        SolidBrush brBackBrush;

        int iNewsID = -1;
        string sSenderName = "";
        int iSenderID = -1;
        string sDate = "";
        string sHeaderText = "";
        string sNewsText = "";

        int iLikesCount = 0;

        int iNewCommentsCount = 0;

        bool bNew = false;

        bool bAllCommentsVisible = false;
        int iCommentsHeight = 0;
        int iMaxCommentsInNews = 2;

        int iPrevCommentsTextBoxHeight = 0;

        public int iMarginForImageWidth = 140;
        public int iMarginForImageHeight = 130;
        int iMarginForText = 8;
        int iCurTextPosY = 0;
        int iMarginToNextComment = 8;
        int iMarginTextRows = 5;

        int iFirstWidth = 0;
        int iFirstHeight = 0;

        InfiniumNewsCommentItem[] CommentsItems;

        DataTable UsersDT = null;
        public DataRow[] CommentsRows = null;
        public DataRow[] AttachmentsRows = null;
        public DataRow[] NewsLikesRows = null;
        public DataRow[] CommentsLikesRows = null;

        InfiniumNewsControlButtons ControlButtons;
        public InfiniumNewsCommentsRichTextBox CommentsTextBox;
        InfiniumNewsAttachmentsList AttachmentsList;

        int CurrentUserID = -1;

        //CONSTRUCTOR
        public InfiniumNewsItem(int iCurrentUserID, ref UserImagesStruct[] tUsersImages, DataTable tUsersDT)
        {
            CurrentUserID = iCurrentUserID;

            //SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            UsersImages = tUsersImages;
            UsersDT = tUsersDT;

            fHeaderFont = new System.Drawing.Font("Segoe UI Light", 18.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            fSenderFont = new System.Drawing.Font("Segoe UI", 16.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            fTextFont = new System.Drawing.Font("Segoe UI", 17.0f, FontStyle.Regular, GraphicsUnit.Pixel);

            brHeaderFontBrush = new SolidBrush(cHeaderFontColor);
            brSenderFontBrush = new SolidBrush(cSenderFontColor);
            brTextFontBrush = new SolidBrush(cTextFontColor);
            brBackBrush = new SolidBrush(Color.White);

            pImageBorderPen = new Pen(new SolidBrush(Color.LightGray));
            pDarkSplitterPen = new Pen(new SolidBrush(cDarkSplitterColor));
            pLightSplitterPen = new Pen(new SolidBrush(cLightSplitterColor));

            ImageRect = new Rectangle(0, 0, 0, 0);

            this.BackColor = Color.White;
        }


        public Bitmap SenderImage
        {
            get { return imSenderImage; }
            set { imSenderImage = value; }
        }


        public Color HeaderFontColor
        {
            get { return cHeaderFontColor; }
            set { cHeaderFontColor = value; brHeaderFontBrush.Color = value; }
        }

        public Color SenderFontColor
        {
            get { return cSenderFontColor; }
            set { cSenderFontColor = value; brSenderFontBrush.Color = value; }
        }

        public Color TextFontColor
        {
            get { return cTextFontColor; }
            set { cTextFontColor = value; brTextFontBrush.Color = value; }
        }

        public Color SplitterDarkColor
        {
            get { return cDarkSplitterColor; }
            set { cDarkSplitterColor = value; pDarkSplitterPen.Color = value; }
        }

        public Color SplitterLightColor
        {
            get { return cLightSplitterColor; }
            set { cLightSplitterColor = value; pLightSplitterPen.Color = value; }
        }

        public Font HeaderFont
        {
            get { return fHeaderFont; }
            set { fHeaderFont = value; }
        }

        public Font TextFont
        {
            get { return fTextFont; }
            set { fTextFont = value; }
        }

        public Font SenderFont
        {
            get { return fSenderFont; }
            set { fSenderFont = value; }
        }

        public bool AllCommentsVisible
        {
            get { return bAllCommentsVisible; }
            set { bAllCommentsVisible = value; this.Refresh(); }
        }

        public int LikesCount
        {
            get { return iLikesCount; }
            set { iLikesCount = value; }
        }

        public int NewsID
        {
            get { return iNewsID; }
            set { iNewsID = value; }
        }

        public int NewCommentsCount
        {
            get { return iNewCommentsCount; }
            set
            {
                iNewCommentsCount = value;

                if (value > 0)
                    brBackBrush.Color = cNewBackColor;
                else
                    brBackBrush.Color = Color.White;
            }
        }

        public bool IsNew
        {
            get { return bNew; }
            set
            {
                bNew = value;

                if (value == true)
                    brBackBrush.Color = cNewBackColor;
                else
                    brBackBrush.Color = Color.White;
            }
        }

        public string HeaderText
        {
            get { return sHeaderText; }
            set { sHeaderText = value; }
        }

        public string SenderName
        {
            get { return sSenderName; }
            set { sSenderName = value; }
        }

        public string NewsText
        {
            get { return sNewsText; }
            set
            {
                sNewsText = value;
                this.Height += GetInitialHeight();
                iFirstWidth = this.Width;
                iFirstHeight = this.Height;
                Initialize();
            }
        }

        public string Date
        {
            get { return sDate; }
            set { sDate = value; }
        }

        public int SenderID
        {
            get { return iSenderID; }
            set { iSenderID = value; }
        }


        private void Initialize()
        {
            if (AttachmentsList != null)
                AttachmentsList.Dispose();

            if (AttachmentsRows.Count() > 0)
            {
                AttachmentsList = new InfiniumNewsAttachmentsList(AttachmentsRows)
                {
                    Top = this.Height + 10,
                    Parent = this,
                    Width = this.Width - iMarginForImageWidth - iMarginForText - 2 - 2,
                    Left = iMarginForImageWidth + iMarginForText + 2,
                    Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
                };
                AttachmentsList.AllFilesClicked += OnAttachsAllFilesLabelClick;
                AttachmentsList.FileLabelClicked += OnAttachLabelClicked;

                this.Height += AttachmentsList.Height;

                this.Height += 10;
            }

            if (ControlButtons != null)
                ControlButtons.Dispose();

            //if (sNewsText.Contains("распространяется нашим"))
            //    this.Height = this.Height;

            ControlButtons = new InfiniumNewsControlButtons()
            {
                Parent = this,
                Left = iMarginForImageWidth + iMarginForText + 2,
                Top = this.Height + 6,
                CurrentUserID = CurrentUserID,
                LikesRows = NewsLikesRows
            };
            ControlButtons.CommentsLabelClicked += OnCommentsClick;
            ControlButtons.RemoveLabelClicked += OnRemoveClick;
            ControlButtons.EditLabelClicked += OnEditClick;
            ControlButtons.LikeClicked += OnLikeClick;
            ControlButtons.CommentsLabelVisible = true;
            ControlButtons.QuoteLabelVisible = true;
            ControlButtons.QuoteLabelClicked += OnQuoteLabelClick;
            ControlButtons.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            if (SenderID == CurrentUserID)
            {
                ControlButtons.EditLabelVisible = true;
                ControlButtons.RemoveLabelVisible = true;
            }

            this.Height += 36;//for splitter

            if (CommentsTextBox != null)
                CommentsTextBox.Dispose();

            CommentsTextBox = new InfiniumNewsCommentsRichTextBox()
            {
                Visible = false,
                Parent = this,
                Width = this.Width - iMarginForImageWidth - iMarginForText - 20,
                Left = iMarginForImageWidth + iMarginForText,
                Height = 150,
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            CommentsTextBox.CancelButtonClicked += OnCommentsCancelButtonClick;
            CommentsTextBox.SendButtonClicked += OnCommentsSendButtonClick;
            CommentsTextBox.RichTextBoxSizeChanged += OnRichTextBoxSizeChanged;
            iPrevCommentsTextBoxHeight = CommentsTextBox.Height;

            CreateComments();

        }

        public void CreateComments()
        {
            if (CommentsRows.Count() == 0)
            {
                return;
            }

            int Count = 0;

            if (!bAllCommentsVisible)
            {
                if (CommentsRows.Count() > iMaxCommentsInNews)
                    Count = iMaxCommentsInNews;
                else
                    Count = CommentsRows.Count();
            }
            else
                Count = CommentsRows.Count();


            if (CommentsItems != null)//clear
            {
                for (int i = 0; i < CommentsItems.Count(); i++)
                {
                    CommentsItems[i].Dispose();
                }

                CommentsItems = null;

                GC.Collect();
            }

            CommentsItems = new InfiniumNewsCommentItem[Count];

            int iTempHeight = this.Height;

            this.Height += 6;//margin between newstext and first comment            

            int iCurCommentsPosY = this.Height;

            for (int i = 0; i < Count; i++)
            {
                using (DataTable DT = new DataTable())
                {
                    DT.Columns.Add(new DataColumn("NewsCommentsLikeID", System.Type.GetType("System.Int32")));
                    DT.Columns.Add(new DataColumn("NewsID", System.Type.GetType("System.Int32")));
                    DT.Columns.Add(new DataColumn("NewsCommentID", System.Type.GetType("System.Int32")));
                    DT.Columns.Add(new DataColumn("UserID", System.Type.GetType("System.Int32")));
                    DT.Columns.Add(new DataColumn("UserTypeID", System.Type.GetType("System.Int32")));

                    CommentsLikesRows.CopyToDataTable(DT, LoadOption.OverwriteChanges);

                    CommentsItems[i] = new InfiniumNewsCommentItem(CurrentUserID)
                    {
                        Parent = this,
                        Width = this.Width - iMarginForImageWidth - 14,
                        NewsID = Convert.ToInt32(CommentsRows[i]["NewsID"]),
                        NewsCommentID = Convert.ToInt32(CommentsRows[i]["NewsCommentID"]),
                        SenderID = Convert.ToInt32(CommentsRows[i]["UserID"])
                    };
                    if (CommentsRows[i].Table.Columns["UserTypeID"] != null)
                    {
                        if (CommentsRows[i]["UserTypeID"].ToString() == "0")
                        {
                            int index = Array.FindIndex(UsersImages, UI => UI.UserID == CommentsItems[i].SenderID);
                            if (index > -1)
                                CommentsItems[i].SenderImage = UsersImages[index].Photo;
                            CommentsItems[i].SenderName = UsersDT.Select("UserID = " + CommentsRows[i]["UserID"])[0]["Name"].ToString();
                        }

                        if (CommentsRows[i]["UserTypeID"].ToString() == "1")
                        {
                            CommentsItems[i].SenderName = ManagersDT.Select("ManagerID = " + CommentsRows[i]["UserID"])[0]["Name"].ToString();
                        }

                        if (CommentsRows[i]["UserTypeID"].ToString() == "2")
                        {
                            CommentsItems[i].SenderName = ManagersDT.Select("ClientID = " + CommentsRows[i]["UserID"])[0]["ClientName"].ToString();
                        }
                        if (CommentsRows[i]["UserTypeID"].ToString() == "3")
                        {
                            CommentsItems[i].SenderName = ClientsManagersDT.Select("ManagerID = " + CommentsRows[i]["UserID"])[0]["Name"].ToString();
                        }
                    }
                    else
                    {
                        int index = Array.FindIndex(UsersImages, UI => UI.UserID == CommentsItems[i].SenderID);
                        if (index > -1)
                            CommentsItems[i].SenderImage = UsersImages[index].Photo;
                        CommentsItems[i].SenderName = UsersDT.Select("UserID = " + CommentsRows[i]["UserID"])[0]["Name"].ToString();
                    }

                    CommentsItems[i].CommentsLikesRows = DT.Select("NewsCommentID = " + CommentsRows[i]["NewsCommentID"]);
                    CommentsItems[i].Date = CommentsRows[i]["DateTime"].ToString();
                    CommentsItems[i].CommentText = CommentsRows[i]["NewsComment"].ToString();
                    CommentsItems[i].Top = iCurCommentsPosY;
                    CommentsItems[i].Left = iMarginForImageWidth + iMarginForText;
                    CommentsItems[i].Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                    CommentsItems[i].AllCommentsLabelClicked += OnAllCommentsClick;
                    CommentsItems[i].RemoveLabelClicked += OnCommentsRemoveButtonClick;
                    CommentsItems[i].EditLabelClicked += OnCommentsEditButtonClick;
                    CommentsItems[i].CommentLabelClicked += OnCommentsCommentButtonClick;
                    CommentsItems[i].LikeClicked += OnCommentsLikeClick;
                    CommentsItems[i].QuoteLabelClicked += OnCommentsQuoteLabelClick;
                }

                iCurCommentsPosY += CommentsItems[i].Height + iMarginToNextComment;

                if (this.Height < iCurCommentsPosY)
                    if (i != Count - 1)
                        this.Height += CommentsItems[i].Height + iMarginToNextComment;
                    else
                        this.Height += CommentsItems[i].Height;

                if (i == Count - 1 && bAllCommentsVisible == false && Count != CommentsRows.Count())
                {
                    CommentsItems[i].iAllCommentsCount = CommentsRows.Count() - iMaxCommentsInNews;
                    CommentsItems[i].AllCommentsVisible = true;
                }

                if (i == Count - 1)
                    CommentsItems[i].CommentsVisible = true;

                CommentsItems[i].Refresh();
            }

            this.Height += 26;//for splitter

            iCommentsHeight = this.Height - iTempHeight;
        }

        public void ReloadLikes()
        {
            ControlButtons.LikesRows = NewsLikesRows;


            if (CommentsRows.Count() == 0)
            {
                return;
            }

            int Count = 0;

            if (!bAllCommentsVisible)
            {
                if (CommentsRows.Count() > iMaxCommentsInNews)
                    Count = iMaxCommentsInNews;
                else
                    Count = CommentsRows.Count();
            }
            else
                Count = CommentsRows.Count();

            for (int i = 0; i < Count; i++)
            {
                using (DataTable DT = new DataTable())
                {
                    DT.Columns.Add(new DataColumn("NewsCommentsLikeID", System.Type.GetType("System.Int32")));
                    DT.Columns.Add(new DataColumn("NewsID", System.Type.GetType("System.Int32")));
                    DT.Columns.Add(new DataColumn("NewsCommentID", System.Type.GetType("System.Int32")));
                    DT.Columns.Add(new DataColumn("UserID", System.Type.GetType("System.Int32")));
                    DT.Columns.Add(new DataColumn("UserTypeID", System.Type.GetType("System.Int32")));

                    CommentsLikesRows.CopyToDataTable(DT, LoadOption.OverwriteChanges);

                    CommentsItems[i].CommentsLikesRows = DT.Select("NewsCommentID = " + CommentsRows[i]["NewsCommentID"]);
                    CommentsItems[i].ReloadLikes();
                }
            }
        }

        private void SetPositions(int YOffset)
        {
            if (ControlButtons != null)
                ControlButtons.Top += YOffset;

            if (CommentsTextBox != null)
                CommentsTextBox.Top += YOffset;

            if (CommentsItems != null)
            {
                for (int i = 0; i < CommentsItems.Count(); i++)
                {
                    if (CommentsItems[i] == null)
                        return;

                    CommentsItems[i].Top += YOffset;
                }
            }
        }

        private void OpenCommentsTextBox(string CommentsText, int iNewsCommentID)
        {
            this.Height += CommentsTextBox.Height + 5;
            CommentsTextBox.Clear();
            CommentsTextBox.Top = this.Height - CommentsTextBox.Height - 5;
            CommentsTextBox.Visible = true;

            if (CommentsText.Length > 0)
            {
                CommentsTextBox.CommentsText = CommentsText;
                CommentsTextBox.Edit = true;
                CommentsTextBox.NewsCommentID = iNewsCommentID;
            }
            else
                CommentsTextBox.Edit = false;

            this.Refresh();
            CommentsTextBox.Refresh();
        }

        public void CloseCommentsTextBox()
        {
            if (CommentsTextBox.Visible)
            {
                this.Height -= CommentsTextBox.Height + 5;
                CommentsTextBox.Visible = false;
            }
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            iCurTextPosY = 0;

            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            e.Graphics.FillRectangle(brBackBrush, ClientRectangle);

            DrawImage(e.Graphics);
            DrawHeader(e.Graphics, ref iCurTextPosY);
            DrawSenderAndDate(e.Graphics, ref iCurTextPosY);
            DrawNewsText(e.Graphics, ref iCurTextPosY);

            if (iCurTextPosY < iMarginForImageHeight + 9)
                DrawTextSplitter(e.Graphics, iMarginForImageHeight + 13);
            else
                DrawTextSplitter(e.Graphics, iCurTextPosY);

            if (AttachmentsList != null)
                DrawAttachsSplitter(e.Graphics, AttachmentsList.Top + AttachmentsList.Height + 1);

            DrawMainSplitter(e.Graphics, this.Height - 1);


        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            //base.OnPaintBackground(pevent);
        }

        private void DrawImage(Graphics G)
        {
            if (SenderImage == null)
            {
                Bitmap NoImageBitmap = new Bitmap(155, 136);
                Graphics Gr = Graphics.FromImage(NoImageBitmap);
                Font F = new Font("Segoe UI", 14.0f, FontStyle.Regular, GraphicsUnit.Pixel);
                Gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                Gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                Gr.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
                Gr.DrawString("Нет фото", F, new SolidBrush(Color.Gray), (155 - Gr.MeasureString("Нет фото", F).Width) / 2, (136 - Gr.MeasureString("Нет фото", F).Height) / 2 - 2);

                NoImageBitmap.SetResolution(G.DpiX, G.DpiY);

                G.DrawImage(NoImageBitmap, 3, 8, iMarginForImageWidth, iMarginForImageHeight);
            }
            else
            {
                SenderImage.SetResolution(G.DpiX, G.DpiY);

                G.DrawImage(SenderImage, 3, 8, iMarginForImageWidth, iMarginForImageHeight);
            }

            ImageRect.X = 3;
            ImageRect.Y = 8;
            ImageRect.Width = iMarginForImageWidth;
            ImageRect.Height = iMarginForImageHeight;

            G.DrawRectangle(pImageBorderPen, ImageRect);
        }

        private int DrawSenderAndDate(Graphics G, ref int CurTextPosY)
        {
            if (SenderName.Length == 0)
                return 0;

            int C = Convert.ToInt32(G.MeasureString(SenderName, fSenderFont).Height);

            G.DrawString(SenderName + ", " + Date, fSenderFont, brSenderFontBrush, iMarginForImageWidth + iMarginForText, CurTextPosY);

            CurTextPosY += C;

            return C;
        }

        private int DrawHeader(Graphics G, ref int CurTextPosY)
        {
            if (sHeaderText.Length == 0)
            {
                iCurTextPosY += 3;
                return 0;
            }

            int TextMaxWidth = this.Width - iMarginForImageWidth;

            int CurrentY = 0;

            string CurrentRowString = "";

            for (int i = 0; i < sHeaderText.Length; i++)
            {
                if (sHeaderText[i] == '\n')
                {
                    G.DrawString(CurrentRowString, fHeaderFont, brHeaderFontBrush,
                                 iMarginForImageWidth + iMarginForText, CurTextPosY +
                                 (CurrentY * (G.MeasureString("String", fHeaderFont).Height + iMarginTextRows)) + 4);
                    CurrentRowString = "";
                    CurrentY++;
                    continue;
                }

                if (i == sHeaderText.Length - 1)
                {
                    G.DrawString(CurrentRowString += sHeaderText[i], fHeaderFont, brHeaderFontBrush,
                                 iMarginForImageWidth + iMarginForText, CurTextPosY +
                                 (CurrentY * (G.MeasureString("String", fHeaderFont).Height + iMarginTextRows)) + 4);
                    CurrentY++;
                }
                else
                {

                    if (G.MeasureString(CurrentRowString, fHeaderFont).Width > TextMaxWidth)
                    {
                        int LastSpace = GetLastSpace(CurrentRowString);

                        if (LastSpace == 0)
                        {
                            G.DrawString(CurrentRowString, fHeaderFont, brHeaderFontBrush,
                                         iMarginForImageWidth + iMarginForText, CurTextPosY +
                                        (CurrentY * (G.MeasureString("String", fHeaderFont).Height + iMarginTextRows)) + 4);
                        }
                        else
                        {
                            G.DrawString(CurrentRowString.Substring(0, LastSpace + 1), fHeaderFont, brHeaderFontBrush,
                                         iMarginForImageWidth + iMarginForText, CurTextPosY +
                                         (CurrentY * (G.MeasureString("String", fHeaderFont).Height + iMarginTextRows)) + 4);

                            //if (CurrentRowString[CurrentRowString.Length - (CurrentRowString.Length - LastSpace)] == ' ')
                            //    i -= (CurrentRowString.Length - LastSpace - 1);
                            //else
                            i -= (CurrentRowString.Length - LastSpace);
                        }


                        CurrentRowString = "";
                        CurrentY++;
                        continue;
                    }
                }

                CurrentRowString += sHeaderText[i];
            }

            int C = CurrentY * Convert.ToInt32((G.MeasureString("String", fHeaderFont).Height) + iMarginTextRows);

            CurTextPosY += C;

            return C;
        }

        private int DrawNewsText(Graphics G, ref int CurTextPosY)
        {
            if (sNewsText.Length == 0)
                return 0;

            int TextMaxWidth = this.Width - iMarginForImageWidth;

            int CurrentY = 0;

            string CurrentRowString = "";

            for (int i = 0; i < sNewsText.Length; i++)
            {
                if (sNewsText[i] == '\n')
                {
                    G.DrawString(CurrentRowString, fTextFont, brTextFontBrush,
                                 iMarginForImageWidth + iMarginForText, CurTextPosY +
                                 (CurrentY * (G.MeasureString("String", fTextFont).Height + iMarginTextRows)) + 4);
                    CurrentRowString = "";
                    CurrentY++;
                    continue;
                }

                if (i == sNewsText.Length - 1)
                {
                    G.DrawString(CurrentRowString += sNewsText[i], fTextFont, brTextFontBrush,
                                 iMarginForImageWidth + iMarginForText, CurTextPosY +
                                 (CurrentY * (G.MeasureString("String", fTextFont).Height + iMarginTextRows)) + 4);
                    CurrentY++;
                }
                else
                {
                    if (G.MeasureString(CurrentRowString, fTextFont).Width > TextMaxWidth)
                    {
                        int LastSpace = GetLastSpace(CurrentRowString);

                        if (LastSpace == 0)
                        {
                            G.DrawString(CurrentRowString, fTextFont, brTextFontBrush,
                                         iMarginForImageWidth + iMarginForText, CurTextPosY +
                                        (CurrentY * (G.MeasureString("String", fTextFont).Height + iMarginTextRows)) + 4);
                        }
                        else
                        {
                            G.DrawString(CurrentRowString.Substring(0, LastSpace + 1), fTextFont, brTextFontBrush,
                                         iMarginForImageWidth + iMarginForText, CurTextPosY +
                                         (CurrentY * (G.MeasureString("String", fTextFont).Height + iMarginTextRows)) + 4);

                            //if (CurrentRowString[CurrentRowString.Length - (CurrentRowString.Length - LastSpace)] == ' ')
                            //    i -= (CurrentRowString.Length - LastSpace - 1);
                            //else
                            i -= (CurrentRowString.Length - LastSpace);
                        }


                        CurrentRowString = "";
                        CurrentY++;

                        //CurrentRowString += sNewsText[i];
                        continue;
                    }
                }

                CurrentRowString += sNewsText[i];
            }

            int C = CurrentY * Convert.ToInt32((G.MeasureString("String", fTextFont).Height) + iMarginTextRows);

            CurTextPosY += C;

            return C;
        }

        private void DrawMainSplitter(Graphics G, int Y)
        {
            G.DrawLine(pDarkSplitterPen, 0, Y, this.Width, Y);
        }

        private void DrawTextSplitter(Graphics G, int Y)
        {
            G.DrawLine(pLightSplitterPen, iMarginForImageWidth + iMarginForText + 2, Y, this.Width, Y);
        }

        private void DrawAttachsSplitter(Graphics G, int Y)
        {
            G.DrawLine(pLightSplitterPen, iMarginForImageWidth + iMarginForText + 2, Y, this.Width, Y);
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

        private int GetInitialHeaderHeight()
        {
            using (Graphics G = this.CreateGraphics())
            {
                int TextMaxWidth = this.Width - iMarginForImageWidth;

                int CurrentY = 0;

                string CurrentRowString = "";

                for (int i = 0; i < sHeaderText.Length; i++)
                {
                    if (sHeaderText[i] == '\n')
                    {
                        CurrentRowString = "";
                        CurrentY++;
                        continue;
                    }

                    if (i == sHeaderText.Length - 1)
                    {
                        CurrentY++;
                    }
                    else
                    {

                        if (G.MeasureString(CurrentRowString, fHeaderFont).Width > TextMaxWidth)
                        {
                            int LastSpace = GetLastSpace(CurrentRowString);

                            if (LastSpace == 0)
                            {

                            }
                            else
                            {

                                i -= (CurrentRowString.Length - LastSpace - 1);
                            }


                            CurrentRowString = "";
                            CurrentY++;
                            continue;
                        }
                    }

                    CurrentRowString += sHeaderText[i];
                }

                int C = CurrentY * Convert.ToInt32((G.MeasureString("String", fHeaderFont).Height) + iMarginTextRows);

                return C;
            }
        }


        private int GetInitialHeight()
        {
            using (Graphics G = this.CreateGraphics())
            {
                int iCaptionHeight = GetInitialHeaderHeight();
                int iSenderHeight = Convert.ToInt32(G.MeasureString(SenderName + sDate, fSenderFont).Height);

                int TextMaxWidth = this.Width - iMarginForImageWidth;

                int CurrentY = 0;

                string CurrentRowString = "";

                for (int i = 0; i < sNewsText.Length; i++)
                {
                    if (sNewsText[i] == '\n')
                    {
                        CurrentRowString = "";
                        CurrentY++;
                        continue;
                    }

                    if (i == sNewsText.Length - 1)
                    {
                        CurrentY++;

                    }
                    else
                    {

                        if (G.MeasureString(CurrentRowString, fTextFont).Width > TextMaxWidth)
                        {
                            int LastSpace = GetLastSpace(CurrentRowString);

                            if (LastSpace == 0)
                            {

                            }
                            else
                            {
                                i -= (CurrentRowString.Length - LastSpace);
                            }

                            CurrentRowString = "";
                            CurrentY++;
                            continue;
                        }
                    }

                    CurrentRowString += sNewsText[i];
                }

                int C = CurrentY * Convert.ToInt32((G.MeasureString("String", fTextFont).Height) + iMarginTextRows) + iSenderHeight + iCaptionHeight;

                if (C < iMarginForImageHeight + 9)
                    C = iMarginForImageHeight + 9;

                return C;
            }
        }

        private void OnAttachLabelClicked(object sender, int NewsAttachID)
        {
            OnAttachClick(NewsAttachID);
        }

        private void OnEditClick(object sender)
        {
            OnEditLabelClick();
        }

        private void OnQuoteLabelClick(object sender)
        {
            Clipboard.SetText(NewsText, TextDataFormat.UnicodeText);
            OnQuoteLabelClicked();
        }

        private void OnCommentsQuoteLabelClick(object sender)
        {
            OnCommentsQuoteLabelClicked();
        }

        private void OnLikeClick(object sender)
        {
            OnLikeClicked();
        }

        private void OnCommentsLikeClick(object sender)
        {
            OnCommentsLikeClicked(sender, ((InfiniumNewsCommentItem)sender).NewsID, ((InfiniumNewsCommentItem)sender).NewsCommentID);
        }

        private void OnCommentsClick(object sender)
        {
            if (CommentsTextBox.Visible)
                return;

            if (CommentsRows.Count() > iMaxCommentsInNews && bAllCommentsVisible == false)
            {
                OnAllCommentsClick(ControlButtons);
            }

            OpenCommentsTextBox("", -1);
            OnCommentsLabelClick();
        }

        private void OnRemoveClick(object sender)
        {
            OnRemoveLabelClicked(this);
        }

        private void OnAllCommentsClick(object sender)
        {
            //CloseCommentsTextBox();
            //this.Height = 0;
            //this.Height += GetInitialHeight();
            this.Height -= iCommentsHeight;
            bAllCommentsVisible = true;
            //Initialize();
            CreateComments();
            OnAllCommentsLabelClick();
        }

        private void OnCommentsCancelButtonClick(object sender, EventArgs e)
        {
            CloseCommentsTextBox();
            OnCommentsCancelButtonClick();
        }

        private void OnAttachsAllFilesLabelClick(object sender, EventArgs e)
        {
            this.Height -= AttachmentsList.StandardHeight;
            this.Height += AttachmentsList.Height;
            SetPositions(AttachmentsList.Height - AttachmentsList.StandardHeight);
            OnAttachsAllFilesClick();
        }

        private void OnCommentsSendButtonClick(object sender, string Text, bool bEdit)
        {
            OnSendButtonClicked(sender, Text, bEdit, ((InfiniumNewsCommentsRichTextBox)sender).NewsCommentID);
        }


        private void OnCommentsCommentButtonClick(object sender)
        {
            if (CommentsTextBox.Visible)
                return;

            if (CommentsRows.Count() > iMaxCommentsInNews && bAllCommentsVisible == false)
            {
                OnAllCommentsClick(ControlButtons);
            }

            OpenCommentsTextBox("", -1);
            OnCommentsCommentClicked(sender);
        }

        private void OnCommentsRemoveButtonClick(object sender)
        {
            OnCommentsRemoveClicked(sender, ((InfiniumNewsCommentItem)sender).NewsID, ((InfiniumNewsCommentItem)sender).NewsCommentID);
        }

        private void OnCommentsEditButtonClick(object sender)
        {
            if (CommentsTextBox.Visible)
                return;

            OpenCommentsTextBox(((InfiniumNewsCommentItem)sender).CommentText, ((InfiniumNewsCommentItem)sender).NewsCommentID);
            OnCommentsEditClicked(sender, ((InfiniumNewsCommentItem)sender).NewsID, ((InfiniumNewsCommentItem)sender).NewsCommentID);
        }

        private void OnRichTextBoxSizeChanged(object sender, EventArgs e)
        {
            if (CommentsTextBox.Height != iPrevCommentsTextBoxHeight)
            {
                if (CommentsTextBox.Height < iPrevCommentsTextBoxHeight)
                    this.Height -= iPrevCommentsTextBoxHeight - CommentsTextBox.Height;
                else
                    this.Height += CommentsTextBox.Height - iPrevCommentsTextBoxHeight;

                iPrevCommentsTextBoxHeight = CommentsTextBox.Height;
            }

            OnCommentsTextBoxSizeChanged(e);
        }


        public event LabelClickedEventHandler EditLabelClicked;
        public event LabelClickedEventHandler AllCommentsLabelClicked;
        public event LabelClickedEventHandler CommentsLabelClicked;
        public event LabelClickedEventHandler RemoveLabelClicked;
        public event LabelClickedEventHandler CommentsCancelButtonClicked;
        public event LabelClickedEventHandler AttachsAllFilesClicked;
        public event LabelClickedEventHandler LikeClicked;
        public event LabelClickedEventHandler QuoteLabelClicked;
        public event LabelClickedEventHandler CommentsQuoteLabelClicked;
        public event CommentsEventHandler CommentsLikeClicked;
        public event SendButtonEventHandler SendButtonClicked;
        public event CommentsEventHandler CommentsRemoveClicked;
        public event CommentsEventHandler CommentsEditClicked;
        public event LabelClickedEventHandler CommentsCommentClicked;
        public event EventHandler CommentsTextBoxSizeChanged;
        public event AttachClickedEventHandler AttachClicked;

        public delegate void LabelClickedEventHandler(object sender);
        public delegate void SendButtonEventHandler(object sender, string Text, bool bEdit, int NewsCommentID);
        public delegate void CommentsEventHandler(object sender, int NewsID, int NewsCommentID);
        public delegate void AttachClickedEventHandler(object sender, int NewsAttachID);


        public virtual void OnAttachClick(int NewsAttachID)
        {
            AttachClicked?.Invoke(this, NewsAttachID);//Raise the event
        }


        public virtual void OnQuoteLabelClicked()
        {
            QuoteLabelClicked?.Invoke(this);//Raise the event
        }

        public virtual void OnCommentsQuoteLabelClicked()
        {
            CommentsQuoteLabelClicked?.Invoke(this);//Raise the event
        }


        public virtual void OnLikeClicked()
        {
            LikeClicked?.Invoke(this);//Raise the event
        }

        public virtual void OnCommentsLikeClicked(object sender, int NewsID, int NewsCommentID)
        {
            CommentsLikeClicked?.Invoke(sender, NewsID, NewsCommentID);//Raise the event
        }

        public virtual void OnEditLabelClick()
        {
            EditLabelClicked?.Invoke(this);//Raise the event
        }

        public virtual void OnCommentsTextBoxSizeChanged(EventArgs e)
        {
            CommentsTextBoxSizeChanged?.Invoke(this, e);//Raise the event
        }

        public virtual void OnAttachsAllFilesClick()
        {
            AttachsAllFilesClicked?.Invoke(this);//Raise the event
        }

        public virtual void OnAllCommentsLabelClick()
        {
            AllCommentsLabelClicked?.Invoke(this);//Raise the event
        }

        public virtual void OnCommentsLabelClick()
        {
            CommentsLabelClicked?.Invoke(this);//Raise the event
        }

        public virtual void OnCommentsCancelButtonClick()
        {
            CommentsCancelButtonClicked?.Invoke(this);//Raise the event
        }


        public virtual void OnRemoveLabelClicked(object sender)
        {
            RemoveLabelClicked?.Invoke(sender);//Raise the event
        }

        public virtual void OnCommentsCommentClicked(object sender)
        {
            CommentsCommentClicked?.Invoke(sender);//Raise the event
        }

        public virtual void OnCommentsRemoveClicked(object sender, int NewsID, int NewsCommentID)
        {
            CommentsRemoveClicked?.Invoke(sender, NewsID, NewsCommentID);//Raise the event
        }

        public virtual void OnCommentsEditClicked(object sender, int NewsID, int NewsCommentID)
        {
            CommentsEditClicked?.Invoke(sender, NewsID, NewsCommentID);//Raise the event
        }


        public virtual void OnSendButtonClicked(object sender, string Text, bool bEdit, int NewsCommentID)
        {
            SendButtonClicked?.Invoke(this, Text, bEdit, NewsCommentID);//Raise the event
        }


        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            Parent.Focus();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            //if(iFirstHeight > 0 && iFirstWidth > 0)
            //    if (this.Height != iFirstHeight || this.Width != iFirstWidth)
            //    {
            //        SetPositions(this.Height - iFirstHeight);

            //        iFirstWidth = this.Width;
            //        iFirstHeight = this.Height;
            //    }
        }
    }


    public class InfiniumNewsCommentItem : Control
    {
        Bitmap imSenderImage;

        Color cSenderFontColor = Color.FromArgb(18, 164, 217);
        Color cDateFontColor = Color.Gray;
        Color cTextFontColor = Color.FromArgb(30, 30, 30);
        Color cSplitterColor = Color.FromArgb(240, 240, 240);

        Font fSenderFont;
        Font fDateFont;
        Font fTextFont;

        int iNewsID = -1;
        int iNewsCommentID = -1;
        int iSenderID = -1;
        string sSenderName = "";
        string sDate = "";
        string sCommentText = "";

        public bool bAllCommentsVisible = false;
        public bool bCommentsVisible = false;
        public int iAllCommentsCount = -1;

        int iLikesCount = 0;

        int iMarginForImageWidth = 90;
        int iMarginForImageHeight = 84;
        int iCurTextPosY = 0;
        int iMarginTextRows = 5;
        int iMarginText = 5;

        Pen pImageBorderPen;
        Pen pSplitterPen;

        Rectangle ImageRect;

        SolidBrush brSenderFontBrush;
        SolidBrush brDateFontBrush;
        SolidBrush brTextFontBrush;

        InfiniumNewsControlButtons ControlButtons;

        public DataRow[] CommentsLikesRows = null;

        int CurrentUserID = -1;

        public InfiniumNewsCommentItem(int iCurrentUserID)
        {
            CurrentUserID = iCurrentUserID;

            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            fSenderFont = new System.Drawing.Font("Segoe UI", 16.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            fDateFont = new System.Drawing.Font("Segoe UI", 15.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            fTextFont = new System.Drawing.Font("Segoe UI", 15.0f, FontStyle.Regular, GraphicsUnit.Pixel);

            brSenderFontBrush = new SolidBrush(cSenderFontColor);
            brDateFontBrush = new SolidBrush(cDateFontColor);
            brTextFontBrush = new SolidBrush(cTextFontColor);

            pImageBorderPen = new Pen(new SolidBrush(Color.LightGray));
            pSplitterPen = new Pen(new SolidBrush(cSplitterColor));

            ImageRect = new Rectangle(0, 0, 0, 0);

            this.BackColor = Color.Transparent;
        }


        public Bitmap SenderImage
        {
            get { return imSenderImage; }
            set { imSenderImage = value; }
        }


        public Color SenderFontColor
        {
            get { return cSenderFontColor; }
            set { cSenderFontColor = value; brSenderFontBrush.Color = value; }
        }

        public Color DateFontColor
        {
            get { return cDateFontColor; }
            set { cDateFontColor = value; brDateFontBrush.Color = value; }
        }

        public Color TextFontColor
        {
            get { return cTextFontColor; }
            set { cTextFontColor = value; brTextFontBrush.Color = value; }
        }

        public Color SplitterColor
        {
            get { return cSplitterColor; }
            set { cSplitterColor = value; pSplitterPen.Color = value; }
        }


        public Font TextFont
        {
            get { return fTextFont; }
            set { fTextFont = value; }
        }

        public Font SenderFont
        {
            get { return fSenderFont; }
            set { fSenderFont = value; }
        }

        public Font DateFont
        {
            get { return fDateFont; }
            set { fDateFont = value; }
        }


        public int LikesCount
        {
            get { return iLikesCount; }
            set { iLikesCount = value; }
        }


        public int NewsID
        {
            get { return iNewsID; }
            set { iNewsID = value; }
        }

        public int NewsCommentID
        {
            get { return iNewsCommentID; }
            set { iNewsCommentID = value; }
        }

        public int SenderID
        {
            get { return iSenderID; }
            set { iSenderID = value; }
        }

        public string SenderName
        {
            get { return sSenderName; }
            set { sSenderName = value; }
        }

        public string Date
        {
            get { return sDate; }
            set { sDate = value; }
        }

        public string CommentText
        {
            get { return sCommentText; }

            set
            {
                sCommentText = value;
                this.Height = GetInitialHeight();

                ControlButtons = new InfiniumNewsControlButtons()
                {
                    Parent = this,
                    Top = this.Height + 5,
                    Font = new Font("Segoe UI", 14, FontStyle.Regular, GraphicsUnit.Pixel),
                    Left = iMarginForImageWidth + iMarginText,
                    CurrentUserID = CurrentUserID,
                    LikesRows = CommentsLikesRows
                };
                ControlButtons.RemoveLabelClicked += OnCommentsRemoveLabelClick;
                ControlButtons.EditLabelClicked += OnCommentsEditLabelClick;
                ControlButtons.CommentsLabelClicked += OnCommentLabelClick;
                ControlButtons.LikeClicked += OnLikeClicked;
                ControlButtons.QuoteLabelClicked += OnQuoteLabelClick;

                ControlButtons.QuoteLabelVisible = true;

                if (bAllCommentsVisible)
                    ControlButtons.AllCommentsLabelVisible = true;

                if (SenderID == CurrentUserID)
                {
                    ControlButtons.EditLabelVisible = true;
                    ControlButtons.RemoveLabelVisible = true;
                }

                ControlButtons.AllCommentsLabelClicked += OnCommentsAllCommentsLabelClick;

                this.Height += 28;//for splitter;
            }
        }

        public bool AllCommentsVisible
        {
            get { return bAllCommentsVisible; }
            set { bAllCommentsVisible = value; ControlButtons.AllCommentsLabelVisible = value; ControlButtons.AllCommentsCount = iAllCommentsCount; this.Refresh(); }
        }

        public bool CommentsVisible
        {
            get { return bCommentsVisible; }
            set { bCommentsVisible = value; ControlButtons.CommentsLabelVisible = value; this.Refresh(); }
        }


        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        var parms = base.CreateParams;
        //        parms.Style &= ~0x02000000;  // Turn off WS_CLIPCHILDREN
        //        return parms;
        //    }
        //}


        protected override void OnPaint(PaintEventArgs e)
        {
            iCurTextPosY = 0;

            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            DrawImage(e.Graphics);
            int iS = DrawSender(e.Graphics, ref iCurTextPosY);
            int iD = DrawDate(e.Graphics, ref iCurTextPosY);
            DrawCommentsText(e.Graphics, ref iCurTextPosY, iS + iD);

            if (this.Height < iCurTextPosY)
                this.Height = iCurTextPosY;

            DrawSplitter(e.Graphics, this.Height - 24);
        }

        private void DrawImage(Graphics G)
        {
            if (SenderImage == null)
            {
                Bitmap NoImageBitmap = new Bitmap(155, 136);
                Graphics Gr = Graphics.FromImage(NoImageBitmap);
                Font F = new Font("Segoe UI", 14.0f, FontStyle.Regular, GraphicsUnit.Pixel);
                Gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                Gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                Gr.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
                Gr.DrawString("Нет фото", F, new SolidBrush(Color.Gray), (155 - Gr.MeasureString("Нет фото", F).Width) / 2, (136 - Gr.MeasureString("Нет фото", F).Height) / 2 - 2);

                NoImageBitmap.SetResolution(G.DpiX, G.DpiY);

                G.DrawImage(NoImageBitmap, 3, 8, iMarginForImageWidth, iMarginForImageHeight);
            }
            else
            {
                SenderImage.SetResolution(G.DpiX, G.DpiY);

                G.DrawImage(SenderImage, 3, 8, iMarginForImageWidth, iMarginForImageHeight);
            }

            ImageRect.X = 3;
            ImageRect.Y = 8;
            ImageRect.Width = iMarginForImageWidth;
            ImageRect.Height = iMarginForImageHeight;

            G.DrawRectangle(pImageBorderPen, ImageRect);
        }

        private int DrawSender(Graphics G, ref int CurTextPosY)
        {
            if (SenderName.Length == 0)
                return 0;

            int C = Convert.ToInt32(G.MeasureString(SenderName, fSenderFont).Height);

            G.DrawString(SenderName, fSenderFont, brSenderFontBrush, iMarginForImageWidth + iMarginText, CurTextPosY);

            CurTextPosY += C;

            return C;
        }

        private int DrawDate(Graphics G, ref int CurTextPosY)
        {
            if (Date.Length == 0)
                return 0;

            int TextMaxWidth = this.Width - iMarginForImageWidth;

            int C = Convert.ToInt32(G.MeasureString(Date, fDateFont).Height);

            G.DrawString(Date, fDateFont, brDateFontBrush, iMarginForImageWidth + iMarginText, CurTextPosY);

            CurTextPosY += C;

            return C;
        }

        private int DrawCommentsText(Graphics G, ref int CurTextPosY, int iHeaderAndSender)
        {
            if (sCommentText.Length == 0)
                return 0;

            int TextMaxWidth = this.Width - iMarginForImageWidth;

            int CurrentY = 0;

            string CurrentRowString = "";

            for (int i = 0; i < CommentText.Length; i++)
            {
                if (CommentText[i] == '\n')
                {
                    G.DrawString(CurrentRowString, fTextFont, brTextFontBrush,
                                 iMarginForImageWidth + iMarginText, CurTextPosY +
                                 (CurrentY * (G.MeasureString("String", fTextFont).Height + iMarginTextRows)) + 4);
                    CurrentRowString = "";
                    CurrentY++;
                    continue;
                }

                if (i == CommentText.Length - 1)
                {
                    G.DrawString(CurrentRowString += CommentText[i], fTextFont, brTextFontBrush,
                                 iMarginForImageWidth + iMarginText, CurTextPosY +
                                 (CurrentY * (G.MeasureString("String", fTextFont).Height + iMarginTextRows)) + 4);
                    CurrentY++;
                }
                else
                {

                    if (G.MeasureString(CurrentRowString, fTextFont).Width > TextMaxWidth)
                    {
                        int LastSpace = GetLastSpace(CurrentRowString);

                        if (LastSpace == 0)
                        {
                            G.DrawString(CurrentRowString, fTextFont, brTextFontBrush,
                                         iMarginForImageWidth + iMarginText, CurTextPosY +
                                        (CurrentY * (G.MeasureString("String", fTextFont).Height + iMarginTextRows)) + 4);
                        }
                        else
                        {
                            G.DrawString(CurrentRowString.Substring(0, LastSpace + 1), fTextFont, brTextFontBrush,
                                         iMarginForImageWidth + iMarginText, CurTextPosY +
                                         (CurrentY * (G.MeasureString("String", fTextFont).Height + iMarginTextRows)) + 4);

                            i -= (CurrentRowString.Length - LastSpace);
                        }


                        CurrentRowString = "";
                        CurrentY++;
                        continue;
                    }
                }

                CurrentRowString += CommentText[i];
            }

            int C = CurrentY * Convert.ToInt32((G.MeasureString("String", fTextFont).Height) + iMarginTextRows);

            if (C + iHeaderAndSender < iMarginForImageHeight)//if text and header smaller than photo
                CurTextPosY += iMarginForImageHeight - iHeaderAndSender + 2;
            else
                CurTextPosY += C;

            return C;
        }

        private void DrawSplitter(Graphics G, int Y)
        {
            G.DrawLine(pSplitterPen, iMarginForImageWidth + iMarginText + 2, Y, this.Width, Y);
        }


        public void ReloadLikes()
        {
            ControlButtons.LikesRows = CommentsLikesRows;
        }

        private int GetInitialHeight()
        {
            using (Graphics G = this.CreateGraphics())
            {
                int iSenderHeight = Convert.ToInt32(G.MeasureString("Text", fSenderFont).Height);
                int iDateHeight = Convert.ToInt32(G.MeasureString("Text", fDateFont).Height);

                int TextMaxWidth = this.Width - iMarginForImageWidth;

                int CurrentY = 0;

                string CurrentRowString = "";

                for (int i = 0; i < CommentText.Length; i++)
                {
                    if (CommentText[i] == '\n')
                    {
                        CurrentRowString = "";
                        CurrentY++;
                        continue;
                    }

                    if (i == CommentText.Length - 1)
                    {
                        CurrentY++;

                    }
                    else
                    {

                        if (G.MeasureString(CurrentRowString, fTextFont).Width > TextMaxWidth)
                        {
                            int LastSpace = GetLastSpace(CurrentRowString);

                            if (LastSpace == 0)
                            {

                            }
                            else
                            {
                                i -= (CurrentRowString.Length - LastSpace - 1);
                            }

                            CurrentRowString = "";
                            CurrentY++;
                            continue;
                        }
                    }

                    CurrentRowString += CommentText[i];
                }

                int C = CurrentY * Convert.ToInt32((G.MeasureString("String", fTextFont).Height) + iMarginTextRows)
                                        + iDateHeight + iSenderHeight;

                if (C < iMarginForImageHeight + 4)
                    C = iMarginForImageHeight + 4;

                return C;
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

        private void OnLikeClicked(object sender)
        {
            OnLikeClick();
        }

        private void OnQuoteLabelClick(object sender)
        {
            Clipboard.SetText(CommentText, TextDataFormat.UnicodeText);
            OnQuoteLabelClick();
        }


        private void OnCommentLabelClick(object sender)
        {
            OnCommentLabelClick();
        }

        private void OnCommentsAllCommentsLabelClick(object sender)
        {
            OnAllCommentsLabelClick();
        }

        private void OnCommentsRemoveLabelClick(object sender)
        {
            OnRemoveLabelClick();
        }

        private void OnCommentsEditLabelClick(object sender)
        {
            OnEditLabelClick();
        }


        public event LabelClickedEventHandler AllCommentsLabelClicked;
        public event LabelClickedEventHandler RemoveLabelClicked;
        public event LabelClickedEventHandler EditLabelClicked;
        public event LabelClickedEventHandler CommentLabelClicked;
        public event LabelClickedEventHandler QuoteLabelClicked;
        public event LabelClickedEventHandler LikeClicked;

        public delegate void LabelClickedEventHandler(object sender);



        public virtual void OnQuoteLabelClick()
        {
            QuoteLabelClicked?.Invoke(this);//Raise the event
        }


        public virtual void OnLikeClick()
        {
            LikeClicked?.Invoke(this);//Raise the event
        }

        public virtual void OnCommentLabelClick()
        {
            CommentLabelClicked?.Invoke(this);//Raise the event
        }

        public virtual void OnAllCommentsLabelClick()
        {
            AllCommentsLabelClicked?.Invoke(this);//Raise the event
        }

        public virtual void OnRemoveLabelClick()
        {
            RemoveLabelClicked?.Invoke(this);//Raise the event
        }

        public virtual void OnEditLabelClick()
        {
            EditLabelClicked?.Invoke(this);//Raise the event
        }


        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            Parent.Focus();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            if (this.Height < iMarginForImageHeight)
                this.Height = iMarginForImageHeight;

            if (this.Width < iMarginForImageWidth)
                this.Width = iMarginForImageWidth;
        }
    }


    public class InfiniumNewsControlButtons : Control
    {
        bool bAllCommentsLabelVisible = false;
        bool bCommentsLabelVisible = false;
        bool bQuoteLabelVisible = false;
        bool bEditLabelVisible = false;
        bool bRemoveLabelVisible = false;

        bool bAllCommentsLabelTrack;
        bool bCommentsLabelTrack;
        bool bQuoteLabelTrack;
        bool bEditLabelTrack;
        bool bRemoveLabelTrack;
        bool bLikeTrack;

        int iAllCommentsLabelWidth = 0;
        int iCommentsLabelWidth = 0;
        int iQuoteLabelWidth = 0;
        int iEditLabelWidth = 0;
        int iRemoveLabelWidth = 0;

        int iAllCommentsLabelLeft = 0;
        int iCommentsLabelLeft = 0;
        int iQuoteLabelLeft = 0;
        int iEditLabelLeft = 0;
        int iRemoveLabelLeft = 0;

        int iMarginToNextLabel = 20;
        int iNewsID = -1;
        int iNewsCommentID = -1;

        int iLikeWidth = 0;
        int iLikeLeft = 0;
        //int iLikeHeight = 15;
        int iLikeMargin = 3;
        int iLikesCount = 0;

        int iCurrentUserID;

        int iAllCommentsCount = -1;

        bool bTipUp = false;

        bool bILike = false;

        SolidBrush brFontBrush;

        Bitmap LikeInactiveBMP = Properties.Resources.LikeInActive;
        Bitmap LikeLikedBMP = Properties.Resources.LikeLiked;

        Form LikesList;

        public DataRow[] rLikesRows;

        public InfiniumNewsControlButtons()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            //SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            brFontBrush = new SolidBrush(this.ForeColor);

            this.Font = new Font("Segoe UI", 15.0f, FontStyle.Regular, GraphicsUnit.Pixel);

            this.ForeColor = Color.FromArgb(74, 197, 252);
            this.BackColor = Color.Transparent;

            this.Height = 1;
            this.Width = 1;

            SetInitialWidth();
            SetInitialHeight();

            CreateLikesListForm();
        }

        private void SetInitialWidth()
        {
            using (Graphics G = this.CreateGraphics())
            {
                int W = 0;

                if (bAllCommentsLabelVisible)
                    W += Convert.ToInt32(G.MeasureString("Все комментарии (999)", this.Font).Width) + iMarginToNextLabel;
                if (bCommentsLabelVisible)
                    W += Convert.ToInt32(G.MeasureString("Комментировать", this.Font).Width) + iMarginToNextLabel;
                if (bQuoteLabelVisible)
                    W += Convert.ToInt32(G.MeasureString("Цитировать", this.Font).Width) + iMarginToNextLabel;
                if (bEditLabelVisible)
                    W += Convert.ToInt32(G.MeasureString("Редактировать", this.Font).Width) + iMarginToNextLabel;
                if (bRemoveLabelVisible)
                    W += Convert.ToInt32(G.MeasureString("Удалить", this.Font).Width) + iMarginToNextLabel;
                W += Convert.ToInt32(G.MeasureString("999 больше не нравится", this.Font).Width) + iMarginToNextLabel + this.Height - 4;//likes

                this.Width = W;
            }
        }

        private void SetInitialHeight()
        {
            using (Graphics G = this.CreateGraphics())
            {
                this.Height = Convert.ToInt32(G.MeasureString("Комментировать", this.Font).Height) + 2;
            }
        }

        private void CreateLikesListForm()
        {
            LikesList = new Form()
            {
                FormBorderStyle = FormBorderStyle.None,
                BackColor = Color.LightGray,
                Width = 50,
                Height = 20,
                StartPosition = FormStartPosition.Manual,
                Text = "sdfsd",
                Visible = false
            };
        }

        public int AllCommentsCount
        {
            get { return iAllCommentsCount; }
            set { iAllCommentsCount = value; this.Refresh(); }
        }

        public int LikesCount
        {
            get { return iLikesCount; }
            set { iLikesCount = value; this.Refresh(); }
        }

        public DataRow[] LikesRows
        {
            get { return rLikesRows; }
            set
            {
                rLikesRows = value;

                if (value != null)
                    LikesCount = LikesRows.Count();
                else
                    LikesCount = 0;

                bILike = AmILike();
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
                base.Font = value;

                SetInitialWidth();
                SetInitialHeight();
                this.Refresh();
            }
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

                brFontBrush.Color = value;
                //SetInitialWidth();
                //SetInitialHeight();
                this.Refresh();
            }
        }


        public bool AllCommentsLabelVisible
        {
            get { return bAllCommentsLabelVisible; }
            set { bAllCommentsLabelVisible = value; SetInitialWidth(); }
        }

        public bool CommentsLabelVisible
        {
            get { return bCommentsLabelVisible; }
            set { bCommentsLabelVisible = value; SetInitialWidth(); }
        }

        public bool QuoteLabelVisible
        {
            get { return bQuoteLabelVisible; }
            set { bQuoteLabelVisible = value; SetInitialWidth(); }
        }

        public bool EditLabelVisible
        {
            get { return bEditLabelVisible; }
            set { bEditLabelVisible = value; SetInitialWidth(); }
        }

        public bool RemoveLabelVisible
        {
            get { return bRemoveLabelVisible; }
            set { bRemoveLabelVisible = value; SetInitialWidth(); }
        }


        public bool AmILike()
        {
            if (LikesRows == null)
                return false;

            foreach (DataRow Row in LikesRows)
            {
                if (Convert.ToInt32(Row["UserID"]) == CurrentUserID)
                {
                    return true;
                }
            }

            return false;
        }

        public int CurrentUserID
        {
            get { return iCurrentUserID; }
            set { iCurrentUserID = value; }
        }

        public int NewsID
        {
            get { return iNewsID; }
            set { iNewsID = value; }
        }

        public int NewsCommentID
        {
            get { return iNewsCommentID; }
            set { iNewsCommentID = value; }
        }


        public void ShowTip(int X, int Y)
        {
            LikesList.Left = X;
            LikesList.Top = Y;
            LikesList.Show();
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            int iCurPos = 0;

            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

            iAllCommentsLabelLeft = 0;
            iCommentsLabelLeft = 0;
            iQuoteLabelLeft = 0;
            iEditLabelLeft = 0;
            iRemoveLabelLeft = 0;

            if (iAllCommentsCount == -1)
                iAllCommentsLabelWidth = Convert.ToInt32(e.Graphics.MeasureString("Все комментарии", this.Font).Width);
            else
                iAllCommentsLabelWidth = Convert.ToInt32(e.Graphics.MeasureString("Все комментарии (" + iAllCommentsCount.ToString() + ")", this.Font).Width);
            iCommentsLabelWidth = Convert.ToInt32(e.Graphics.MeasureString("Комментировать", this.Font).Width);
            iQuoteLabelWidth = Convert.ToInt32(e.Graphics.MeasureString("Цитировать", this.Font).Width);
            iEditLabelWidth = Convert.ToInt32(e.Graphics.MeasureString("Редактировать", this.Font).Width);
            iRemoveLabelWidth = Convert.ToInt32(e.Graphics.MeasureString("Удалить", this.Font).Width);

            if (AllCommentsLabelVisible)
            {
                if (iAllCommentsCount == -1)
                    e.Graphics.DrawString("Все комментарии", this.Font, brFontBrush, 0, 0);
                else
                    e.Graphics.DrawString("Все комментарии (" + iAllCommentsCount.ToString() + ")", this.Font, brFontBrush, 0, 0);

                iCurPos += iAllCommentsLabelWidth + iMarginToNextLabel;
                iAllCommentsLabelLeft = 0;
            }

            if (CommentsLabelVisible)
            {
                e.Graphics.DrawString("Комментировать", this.Font, brFontBrush, iCurPos, 0);
                iCommentsLabelLeft = iCurPos;
                iCurPos += iCommentsLabelWidth + iMarginToNextLabel;
            }

            if (QuoteLabelVisible)
            {
                e.Graphics.DrawString("Цитировать", this.Font, brFontBrush, iCurPos, 0);
                iQuoteLabelLeft = iCurPos;
                iCurPos += iQuoteLabelWidth + iMarginToNextLabel;
            }

            if (EditLabelVisible)
            {
                e.Graphics.DrawString("Редактировать", this.Font, brFontBrush, iCurPos, 0);
                iEditLabelLeft = iCurPos;
                iCurPos += iEditLabelWidth + iMarginToNextLabel;
            }

            if (RemoveLabelVisible)
            {
                e.Graphics.DrawString("Удалить", this.Font, brFontBrush, iCurPos, 0);
                iRemoveLabelLeft = iCurPos;
                iCurPos += iRemoveLabelWidth + iMarginToNextLabel;
            }

            //like
            if (iLikesCount == 0)
            {
                e.Graphics.DrawImage(LikeInactiveBMP, iCurPos, (this.Height - (this.Height - 9)) / 2,
                                        (this.Height - 9) * LikeInactiveBMP.VerticalResolution / LikeInactiveBMP.HorizontalResolution,
                                        this.Height - 9);
            }
            else
            {
                e.Graphics.DrawImage(LikeLikedBMP, iCurPos, (this.Height - (this.Height - 9)) / 2,
                                        (this.Height - 9) * LikeLikedBMP.VerticalResolution / LikeLikedBMP.HorizontalResolution, this.Height - 9);
                e.Graphics.DrawString(iLikesCount.ToString(), this.Font, brFontBrush,
                                      iCurPos + (this.Height - 9) * LikeLikedBMP.VerticalResolution / LikeLikedBMP.HorizontalResolution + iLikeMargin, 0);
            }

            iLikeLeft = iCurPos;
            iLikeWidth = Convert.ToInt32((this.Height - 9) * LikeInactiveBMP.VerticalResolution / LikeInactiveBMP.HorizontalResolution);

            iCurPos += iLikeWidth + iMarginToNextLabel;

            if (bTipUp)
            {
                if (bILike)
                    e.Graphics.DrawString("Больше не нравится", this.Font, brFontBrush, iCurPos, 0);
                else
                    e.Graphics.DrawString("Мне нравится", this.Font, brFontBrush, iCurPos, 0);
            }


        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (bAllCommentsLabelVisible && e.X > iAllCommentsLabelLeft && e.X < iAllCommentsLabelLeft + iAllCommentsLabelWidth - 5)
                bAllCommentsLabelTrack = true;
            else
                bAllCommentsLabelTrack = false;


            if (bCommentsLabelVisible && e.X > iCommentsLabelLeft && e.X < iCommentsLabelLeft + iCommentsLabelWidth - 5)
                bCommentsLabelTrack = true;
            else
                bCommentsLabelTrack = false;


            if (bQuoteLabelVisible && e.X > iQuoteLabelLeft && e.X < iQuoteLabelLeft + iQuoteLabelWidth - 5)
                bQuoteLabelTrack = true;
            else
                bQuoteLabelTrack = false;


            if (bEditLabelVisible && e.X > iEditLabelLeft && e.X < iEditLabelLeft + iEditLabelWidth - 5)
                bEditLabelTrack = true;
            else
                bEditLabelTrack = false;


            if (bRemoveLabelVisible && e.X > iRemoveLabelLeft && e.X < iRemoveLabelLeft + iRemoveLabelWidth - 5)
                bRemoveLabelTrack = true;
            else
                bRemoveLabelTrack = false;

            if (e.X > iLikeLeft && e.X < iLikeLeft + iLikeWidth)
            {
                bLikeTrack = true;

                if (bTipUp == false)
                {
                    bTipUp = true;
                    OnLikesListPopup();
                    this.Refresh();
                }
            }
            else
            {
                bLikeTrack = false;

                if (bTipUp)
                {
                    bTipUp = false;
                    this.Refresh();
                }

            }

            if (bAllCommentsLabelTrack || bCommentsLabelTrack || bQuoteLabelTrack || bEditLabelTrack || bRemoveLabelTrack || bLikeTrack)
            {
                if (this.Cursor != Cursors.Hand)
                    this.Cursor = Cursors.Hand;
            }
            else
            {
                if (this.Cursor != Cursors.Default)
                    this.Cursor = Cursors.Default;
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            this.Cursor = Cursors.Default;
            if (bTipUp)
            {
                bTipUp = false;
                this.Refresh();
            }

        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            if (bAllCommentsLabelTrack)
                OnAllCommentsLabelClick();

            if (bCommentsLabelTrack)
                OnCommentsLabelClick();

            if (bQuoteLabelTrack)
                OnQuoteLabelClick();

            if (bEditLabelTrack)
                OnEditLabelClick();

            if (bRemoveLabelTrack)
                OnRemoveLabelClick();

            if (bLikeTrack)
                OnLikeClick();
        }

        public event LabelClickedEventHandler AllCommentsLabelClicked;
        public event LabelClickedEventHandler CommentsLabelClicked;
        public event LabelClickedEventHandler QuoteLabelClicked;
        public event LabelClickedEventHandler EditLabelClicked;
        public event LabelClickedEventHandler RemoveLabelClicked;
        public event LabelClickedEventHandler LikeClicked;
        public event LikesListPopupEventHandler LikesListPopup;

        public delegate void LabelClickedEventHandler(object sender);
        public delegate void LikesListPopupEventHandler(object sender);



        public virtual void OnLikesListPopup()
        {
            LikesListPopup?.Invoke(this);//Raise the event
        }


        public virtual void OnAllCommentsLabelClick()
        {
            AllCommentsLabelClicked?.Invoke(this);//Raise the event
        }

        public virtual void OnCommentsLabelClick()
        {
            CommentsLabelClicked?.Invoke(this);//Raise the event
        }

        public virtual void OnQuoteLabelClick()
        {
            QuoteLabelClicked?.Invoke(this);//Raise the event
        }

        public virtual void OnEditLabelClick()
        {
            EditLabelClicked?.Invoke(this);//Raise the event
        }

        public virtual void OnRemoveLabelClick()
        {
            RemoveLabelClicked?.Invoke(this);//Raise the event
        }

        public virtual void OnLikeClick()
        {
            LikeClicked?.Invoke(this);//Raise the event
        }
    }


    public class InfiniumNewsCommentsRichTextBox : Control
    {
        ComponentFactory.Krypton.Toolkit.KryptonRichTextBox RichTextBox;

        ComponentFactory.Krypton.Toolkit.KryptonButton SendButton;
        ComponentFactory.Krypton.Toolkit.KryptonButton CancelButton;

        Label InfoLabel;

        bool bEdit = false;
        int iStartHeight = 150;
        int iNewsCommentID = -1;
        int iLineHeight = -1;

        bool bCtrlEnter = false;

        public InfiniumNewsCommentsRichTextBox()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            Create();

            this.BackColor = Color.Transparent;
        }

        private void Create()
        {
            RichTextBox = new ComponentFactory.Krypton.Toolkit.KryptonRichTextBox()
            {
                Parent = this
            };
            RichTextBox.StateCommon.Content.Font = new System.Drawing.Font("Segoe UI", 15.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            RichTextBox.TextChanged += OnRichTextBoxTextChanged;
            RichTextBox.KeyDown += OnRichTextBoxKeyDown;

            SendButton = new ComponentFactory.Krypton.Toolkit.KryptonButton()
            {
                Parent = this,
                Text = "Отправить"
            };
            SendButton.StateCommon.Back.Color1 = Color.FromArgb(56, 184, 238);
            SendButton.StateCommon.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            SendButton.StateCommon.Content.ShortText.Color1 = Color.White;
            SendButton.StateCommon.Content.ShortText.Font = new Font("Segoe UI", 15.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            SendButton.StateCommon.Border.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.False;
            SendButton.StateCommon.Border.Rounding = 0;
            SendButton.Click += OnSendButtonClicked;

            SendButton.OverrideDefault.Back.Color1 = Color.FromArgb(56, 184, 238);
            SendButton.OverrideDefault.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;

            SendButton.StateTracking.Back.Color1 = Color.FromArgb(67, 191, 246);


            CancelButton = new ComponentFactory.Krypton.Toolkit.KryptonButton()
            {
                Parent = this,
                Text = "Отмена"
            };
            CancelButton.StateCommon.Back.Color1 = Color.DarkGray;
            CancelButton.StateCommon.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            CancelButton.StateCommon.Content.ShortText.Color1 = Color.White;
            CancelButton.StateCommon.Content.ShortText.Font = new Font("Segoe UI", 15.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            CancelButton.StateCommon.Border.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.False;
            CancelButton.StateCommon.Border.Rounding = 0;
            CancelButton.Click += OnCancelButtonClicked;

            CancelButton.OverrideDefault.Back.Color1 = Color.DarkGray;
            CancelButton.OverrideDefault.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;

            CancelButton.StateTracking.Back.Color1 = Color.FromArgb(179, 179, 179);

            InfoLabel = new Label()
            {
                Parent = this,
                ForeColor = Color.FromArgb(110, 110, 110),
                Font = CancelButton.Font,
                Text = "CTRL+Enter - отправить сообщение",

                AutoSize = true
            };
        }

        public string CommentsText
        {
            get { return RichTextBox.Text; }
            set { RichTextBox.Text = value; }
        }

        public bool Edit
        {
            get { return bEdit; }
            set { bEdit = value; }
        }

        public int NewsCommentID
        {
            get { return iNewsCommentID; }
            set { iNewsCommentID = value; }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            RichTextBox.Width = this.Width;
            RichTextBox.Height = this.Height - 40;
            RichTextBox.Left = 0;
            RichTextBox.Top = 0;
            SetLineHeight();

            SendButton.Width = 106;
            SendButton.Height = 32;
            SendButton.Left = 1;
            SendButton.Top = RichTextBox.Top + RichTextBox.Height + 5;

            CancelButton.Width = 106;
            CancelButton.Height = 32;
            CancelButton.Left = 1 + SendButton.Width + 6;
            CancelButton.Top = RichTextBox.Top + RichTextBox.Height + 5;

            InfoLabel.Top = CancelButton.Top + (CancelButton.Height - InfoLabel.Height) / 2;
            InfoLabel.Left = CancelButton.Width + CancelButton.Left + 5;
        }

        private void SetLineHeight()
        {
            using (Graphics G = RichTextBox.CreateGraphics())
            {
                iLineHeight = Convert.ToInt32(G.MeasureString("Text", RichTextBox.StateCommon.Content.Font).Height);
            }
        }

        private void OnRichTextBoxTextChanged(object sender, EventArgs e)
        {
            if (bCtrlEnter)
                return;

            using (Graphics G = RichTextBox.CreateGraphics())
            {
                int TH = RichTextBox.GetLineFromCharIndex(RichTextBox.Text.Length + 1) * iLineHeight + iLineHeight;

                bool bChanged = false;

                if (TH + 40 > RichTextBox.Height)
                {
                    this.Height = TH + 40 + 40;//40 for buttons
                    bChanged = true;
                }

                if (TH + 40 < RichTextBox.Height && this.Height > iStartHeight)
                {
                    this.Height = TH + 40 + 40;//40 for buttons
                    bChanged = true;
                }

                if (this.Height < iStartHeight)
                {
                    this.Height = iStartHeight;
                    bChanged = true;
                }

                if (bChanged)
                    OnRichTextBoxSizeChanged(this, e);
            }
        }

        private void OnRichTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                OnSendButtonClicked(SendButton, null);
            }
        }

        public event SendButtonEventHandler SendButtonClicked;
        public event EventHandler CancelButtonClicked;
        public event EventHandler RichTextBoxSizeChanged;

        public delegate void SendButtonEventHandler(object sender, string Text, bool bIsEdit);


        public void Clear()
        {
            RichTextBox.Clear();
        }

        public virtual void OnRichTextBoxSizeChanged(object sender, EventArgs e)
        {
            RichTextBoxSizeChanged?.Invoke(this, e);//Raise the event
        }

        public virtual void OnSendButtonClicked(object sender, EventArgs e)
        {
            SendButtonClicked?.Invoke(this, RichTextBox.Text, bEdit);//Raise the event
        }

        public virtual void OnCancelButtonClicked(object sender, EventArgs e)
        {
            CancelButtonClicked?.Invoke(this, null);//Raise the event
        }


        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            RichTextBox.Focus();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (this.Visible)
                this.Focus();
        }
    }





    //projects
    public class InfiniumProjectsVerticalScrollBar : Control
    {
        public int iTotalControlHeight = 0;//height of scrollable container (area)
        public int iScrollWheelOffset = 30;
        int iOffset = 0;

        Rectangle rVerticalScrollShaftRect;

        bool bThumbTracking = false;
        bool bThumbClicked = false;
        int iThumbYClicked = -1;

        public int ThumbSize = 0;//height of thumb
        public decimal iThumbPosition = 0;//current position of thumb on scrollshaft
        public decimal ThumbOneStepWidth = 0;//value offset scrollable area on one step of a thumb
        public int ThumbStepsCount = 0;//total count of thumb steps (in pixels) on scrollshaft
        public decimal ThumbStepWidthOnScrollWheel = 0;//thumb offset on scrollshaft (Y) on every scrollwheeloffset

        Color cVerticalScrollCommonShaftBackColor = Color.LightBlue;
        Color cVerticalScrollCommonThumbButtonColor = Color.DarkGray;
        Color cVerticalScrollTrackingShaftBackColor = Color.Blue;
        Color cVerticalScrollTrackingThumbButtonColor = Color.Gray;

        SolidBrush brVerticalScrollCommonShaftBackBrush;
        SolidBrush brVerticalScrollCommonThumbButtonBrush;
        SolidBrush brVerticalScrollTrackingShaftBackBrush;
        SolidBrush brVerticalScrollTrackingThumbButtonBrush;

        public InfiniumProjectsVerticalScrollBar()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);

            brVerticalScrollCommonShaftBackBrush = new SolidBrush(cVerticalScrollCommonShaftBackColor);
            brVerticalScrollCommonThumbButtonBrush = new SolidBrush(cVerticalScrollCommonThumbButtonColor);
            brVerticalScrollTrackingShaftBackBrush = new SolidBrush(cVerticalScrollTrackingShaftBackColor);
            brVerticalScrollTrackingThumbButtonBrush = new SolidBrush(cVerticalScrollTrackingThumbButtonColor);

            rVerticalScrollShaftRect = new Rectangle(0, 0, 0, 0);
        }

        public int ScrollWheelOffset
        {
            get { return iScrollWheelOffset; }
            set { iScrollWheelOffset = value; this.Refresh(); }
        }

        public int TotalControlHeight
        {
            get { return iTotalControlHeight; }
            set
            {
                iTotalControlHeight = value; InitializeThumb(); SetVisibility();
                this.Refresh();
            }
        }

        public int Offset
        {
            get { return iOffset; }
            set
            {
                iOffset = value;

                if (iScrollWheelOffset > 0)
                {
                    iThumbPosition = Convert.ToInt32(Decimal.Round(Offset * ThumbStepWidthOnScrollWheel / iScrollWheelOffset,
                                        0, MidpointRounding.AwayFromZero));
                    //if (iThumbPosition + ThumbSize > this.Height)
                    //    iThumbPosition = this.Height - Convert.ToInt32(ThumbSize);
                }

                this.Refresh();
            }
        }


        public Color VerticalScrollCommonShaftBackColor
        {
            get { return cVerticalScrollCommonShaftBackColor; }
            set { cVerticalScrollCommonShaftBackColor = value; brVerticalScrollCommonShaftBackBrush.Color = value; this.Refresh(); }
        }

        public Color VerticalScrollCommonThumbButtonColor
        {
            get { return cVerticalScrollCommonThumbButtonColor; }
            set { cVerticalScrollCommonThumbButtonColor = value; brVerticalScrollCommonThumbButtonBrush.Color = value; this.Refresh(); }
        }

        public Color VerticalScrollTrackingShaftBackColor
        {
            get { return cVerticalScrollTrackingShaftBackColor; }
            set { cVerticalScrollTrackingShaftBackColor = value; brVerticalScrollTrackingShaftBackBrush.Color = value; this.Refresh(); }
        }

        public Color VerticalScrollTrackingThumbButtonColor
        {
            get { return cVerticalScrollTrackingThumbButtonColor; }
            set { cVerticalScrollTrackingThumbButtonColor = value; brVerticalScrollTrackingThumbButtonBrush.Color = value; this.Refresh(); }
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (TotalControlHeight == 0 || this.Height == 0 || ScrollWheelOffset == 0 || this.Width == 0)
                return;

            if (TotalControlHeight > this.Height)
            {
                DrawVerticalScrollShaft(e.Graphics);
                DrawVerticalScrollThumb(e.Graphics);
            }
        }


        private void DrawVerticalScrollShaft(Graphics G)
        {
            //Shaft
            if (bThumbTracking)
                G.FillRectangle(brVerticalScrollTrackingShaftBackBrush, rVerticalScrollShaftRect);
            else
                G.FillRectangle(brVerticalScrollCommonShaftBackBrush, rVerticalScrollShaftRect);
        }

        public void InitializeThumb()
        {
            if (TotalControlHeight == 0 || ScrollWheelOffset == 0)
                return;

            decimal V = this.Height;
            decimal T = Convert.ToDecimal(TotalControlHeight);

            decimal Rtv = V / (T / 100);

            ThumbSize = Convert.ToInt32(Decimal.Round(Rtv * (V / 100), 0, MidpointRounding.AwayFromZero));

            if (ThumbSize >= V)
                return;

            ThumbStepWidthOnScrollWheel = (V - ThumbSize) / ((T - V) / ScrollWheelOffset);//ширина шага ползунка при одном смещении

            int posY = Convert.ToInt32(Decimal.Round((Convert.ToDecimal(iOffset) / Convert.ToDecimal(ScrollWheelOffset)) *
                                                        ThumbStepWidthOnScrollWheel, 0, MidpointRounding.AwayFromZero));

            if (posY + ThumbSize > V)
                posY = Convert.ToInt32(V - ThumbSize);

            ThumbStepsCount = this.Height - ThumbSize;

            ThumbOneStepWidth = (Convert.ToDecimal(TotalControlHeight) - this.Height) / ThumbStepsCount;

            iThumbPosition = posY;
        }

        private void DrawVerticalScrollThumb(Graphics G)
        {
            if (!bThumbTracking)
                G.FillRectangle(brVerticalScrollCommonThumbButtonBrush, new Rectangle(rVerticalScrollShaftRect.X + 2,
                                        Convert.ToInt32(Decimal.Round(iThumbPosition, 0, MidpointRounding.AwayFromZero)),
                                                                                  this.Width - 4, Convert.ToInt32(ThumbSize)));
            else
                G.FillRectangle(brVerticalScrollTrackingThumbButtonBrush, new Rectangle(rVerticalScrollShaftRect.X + 2,
                                        Convert.ToInt32(Decimal.Round(iThumbPosition, 0, MidpointRounding.AwayFromZero)),
                                                                                  this.Width - 4, Convert.ToInt32(ThumbSize)));
        }

        public void SetPosition(int Pos)
        {
            if (Pos * ThumbOneStepWidth > TotalControlHeight - this.Height)
            {
                iOffset = TotalControlHeight - this.Height;
                iThumbPosition = this.Height - Convert.ToInt32(ThumbSize);

                this.Refresh();

                OnScrollPositionChanged(Offset);

                return;
            }
            else
                if (Pos * ThumbOneStepWidth < 0)
            {
                iThumbPosition = 0;
                iOffset = 0;
                this.Refresh();

                OnScrollPositionChanged(Offset);

                return;
            }
            else
            {
                iOffset = Convert.ToInt32(ThumbOneStepWidth * Pos);
            }


            iThumbPosition = Pos;

            this.Refresh();

            OnScrollPositionChanged(Offset);
        }

        public void SetVisibility()
        {
            if (this.Height >= TotalControlHeight)
                this.Visible = false;
            else
                this.Visible = true;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            rVerticalScrollShaftRect.Height = this.Height;
            rVerticalScrollShaftRect.Width = this.Width;

            InitializeThumb();

            this.Refresh();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            //if (e.Y < iThumbYClicked)
            //{
            //    if (bThumbClicked)
            //    {
            //        SetPosition(0);
            //        return;
            //    }
            //}

            //if (e.Y > this.Height - iThumbYClicked)
            //{
            //    if (bThumbClicked)
            //    {
            //        SetPosition(ThumbStepsCount);
            //        return;
            //    }
            //}

            if (bThumbClicked)
            {
                SetPosition(e.Y - iThumbYClicked);
                return;
            }

            if (e.Y > iThumbPosition && e.Y < iThumbPosition + ThumbSize)
            {
                if (this.Cursor != Cursors.Hand)
                {
                    bThumbTracking = true;
                    this.Cursor = Cursors.Hand;
                    this.Refresh();
                }

                iThumbYClicked = e.Y - Convert.ToInt32(Decimal.Round(iThumbPosition, 0, MidpointRounding.AwayFromZero));
            }
            else
            {
                if (this.Cursor != Cursors.Default)
                {
                    bThumbTracking = false;
                    this.Cursor = Cursors.Default;
                    this.Refresh();
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (bThumbTracking)
                bThumbClicked = true;
            else
                bThumbClicked = false;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            iThumbYClicked = -1;
            bThumbClicked = false;
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);

            bThumbClicked = false;
            bThumbTracking = false;
            this.Refresh();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (!bThumbClicked)
            {
                bThumbClicked = false;
                bThumbTracking = false;
                Cursor = Cursors.Default;
                this.Refresh();
            }
        }


        public event ScrollEventHandler ScrollPositionChanged;

        public delegate void ScrollEventHandler(object sender, int tOffset);


        public virtual void OnScrollPositionChanged(int tOffset)
        {
            ScrollPositionChanged?.Invoke(this, tOffset);//Raise the event
        }
    }




    public class InfiniumProjectsFilterGroupItem : Control
    {
        string sCaption = "";
        int iNewCount = 0;

        int iMarginToCount = 5;
        int iMarginCaptionLeft = 28;
        int iMarginIconLeft = 0;

        int iStatus = -1;

        Font fCaptionFont;
        Font fNewCountFont;

        Color cCaptionColor = Color.FromArgb(60, 60, 60);
        Color cCaptionTrackingColor = Color.FromArgb(0, 161, 214);
        Color cCaptionSelectedColor = Color.FromArgb(56, 184, 238);

        Color cNewCountColor = Color.FromArgb(60, 60, 60);
        Color cNewCountTrackingColor = Color.FromArgb(0, 161, 214);
        Color cNewCountSelectedColor = Color.FromArgb(56, 184, 238);

        SolidBrush brCaptionBrush;
        SolidBrush brNewCountBrush;

        bool bTracking = false;

        bool bSelected = false;

        Bitmap IconGroupMySelected = Properties.Resources.ProjectGroupMy;
        Bitmap IconGroupMyGray = Properties.Resources.ProjectGroupMyGray;
        Bitmap IconGroupAllSelected = Properties.Resources.ProjectGroupAll;
        Bitmap IconGroupAllGray = Properties.Resources.ProjectGroupAllGray;
        Bitmap IconGroupUsersSelected = Properties.Resources.ProjectGroupUsers;
        Bitmap IconGroupUsersGray = Properties.Resources.ProjectGroupUsersGray;
        Bitmap IconGroupDepsSelected = Properties.Resources.ProjectGroupDeps;
        Bitmap IconGroupDepsGray = Properties.Resources.ProjectGroupDepsGray;

        public InfiniumProjectsFilterGroupItem()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            fCaptionFont = new Font("Segoe UI", 18.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            fNewCountFont = new Font("Segoe UI", 18.0f, FontStyle.Regular, GraphicsUnit.Pixel);

            brCaptionBrush = new SolidBrush(cCaptionColor);
            brNewCountBrush = new SolidBrush(cNewCountColor);

            this.BackColor = Color.Transparent;
            this.Cursor = Cursors.Hand;

            SetInitialSize();
        }


        public string Caption
        {
            get { return sCaption; }
            set { sCaption = value; SetInitialSize(); }
        }

        public int Status
        {
            get { return iStatus; }
            set { iStatus = value; this.Refresh(); }
        }

        public int NewCount
        {
            get { return iNewCount; }
            set { iNewCount = value; SetInitialSize(); }
        }


        public Font CaptionFont
        {
            get { return fCaptionFont; }
            set { fCaptionFont = value; this.Refresh(); SetInitialSize(); }
        }

        public Font NewCountFont
        {
            get { return fNewCountFont; }
            set { fNewCountFont = value; this.Refresh(); SetInitialSize(); }
        }


        public Color CaptionColor
        {
            get { return cCaptionColor; }
            set { cCaptionColor = value; this.Refresh(); }
        }

        public Color CaptionTrackingColor
        {
            get { return cCaptionTrackingColor; }
            set { cCaptionTrackingColor = value; this.Refresh(); }
        }

        public Color CaptionSelectedColor
        {
            get { return cCaptionSelectedColor; }
            set { cCaptionSelectedColor = value; this.Refresh(); }
        }


        public Color NewCountColor
        {
            get { return cNewCountColor; }
            set { cNewCountColor = value; this.Refresh(); }
        }

        public Color NewCountTrackingColor
        {
            get { return cNewCountTrackingColor; }
            set { cNewCountTrackingColor = value; this.Refresh(); }
        }

        public Color NewCountSelectedColor
        {
            get { return cNewCountSelectedColor; }
            set { cNewCountSelectedColor = value; this.Refresh(); }
        }


        public bool Selected
        {
            get { return bSelected; }
            set
            {
                bSelected = value;

                if (value)
                {
                    brCaptionBrush.Color = cCaptionSelectedColor;
                    brNewCountBrush.Color = cNewCountSelectedColor;
                }
                else
                {
                    brCaptionBrush.Color = cCaptionColor;
                    brNewCountBrush.Color = cNewCountColor;
                }

                OnSelectedChanged(this, null);

                this.Refresh();
            }
        }


        public void SetInitialSize()
        {
            if (sCaption.Length == 0)
            {
                this.Width = 50;
                this.Height = 25;
            }

            using (Graphics G = this.CreateGraphics())
            {
                if (iNewCount > 0)
                    this.Width = Convert.ToInt32(G.MeasureString(sCaption, fCaptionFont).Width) + Convert.ToInt32(G.MeasureString(iNewCount.ToString(), fNewCountFont).Width + iMarginToCount + iMarginCaptionLeft);
                else
                    this.Width = Convert.ToInt32(G.MeasureString(sCaption, fCaptionFont).Width + iMarginCaptionLeft);

                this.Height = Convert.ToInt32(G.MeasureString(sCaption, fCaptionFont).Height);
            }
        }



        public event EventHandler SelectedChanged;


        public void OnSelectedChanged(object sender, EventArgs e)
        {
            SelectedChanged?.Invoke(sender, e);//Raise the event
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);

            if (sCaption.Length == 0)
                return;

            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            if (bSelected)
            {
                if (Status == 0)//start
                {
                    IconGroupMySelected.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);

                    e.Graphics.DrawImage(IconGroupMySelected, iMarginIconLeft, (this.Height - IconGroupMySelected.Height) / 2 + 2);
                }

                if (Status == 1)//pause
                {
                    IconGroupUsersSelected.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);

                    e.Graphics.DrawImage(IconGroupUsersSelected, iMarginIconLeft, (this.Height - IconGroupUsersSelected.Height) / 2 + 2);
                }

                if (Status == 2)//cancel
                {
                    IconGroupDepsSelected.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);

                    e.Graphics.DrawImage(IconGroupDepsSelected, iMarginIconLeft, (this.Height - IconGroupDepsSelected.Height) / 2 + 2);
                }

                if (Status == 3)//stop
                {
                    IconGroupAllSelected.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);

                    e.Graphics.DrawImage(IconGroupAllSelected, iMarginIconLeft, (this.Height - IconGroupAllSelected.Height) / 2 + 2);
                }
            }
            else
            {
                if (Status == 0)//start
                {
                    IconGroupMyGray.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);

                    e.Graphics.DrawImage(IconGroupMyGray, iMarginIconLeft, (this.Height - IconGroupMyGray.Height) / 2 + 2);
                }

                if (Status == 1)//pause
                {
                    IconGroupUsersGray.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);

                    e.Graphics.DrawImage(IconGroupUsersGray, iMarginIconLeft, (this.Height - IconGroupUsersGray.Height) / 2 + 2);
                }

                if (Status == 2)//cancel
                {
                    IconGroupDepsGray.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);

                    e.Graphics.DrawImage(IconGroupDepsGray, iMarginIconLeft, (this.Height - IconGroupDepsGray.Height) / 2 + 2);
                }

                if (Status == 3)//stop
                {
                    IconGroupAllGray.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);

                    e.Graphics.DrawImage(IconGroupAllGray, iMarginIconLeft, (this.Height - IconGroupAllGray.Height) / 2 + 2);
                }
            }

            e.Graphics.DrawString(sCaption, fCaptionFont, brCaptionBrush, iMarginCaptionLeft, 0);

            if (iNewCount > 0)
                e.Graphics.DrawString(iNewCount.ToString(), fNewCountFont, brNewCountBrush,
                                        e.Graphics.MeasureString(sCaption, fCaptionFont).Width + iMarginToCount + iMarginCaptionLeft,
                                        (this.Height - e.Graphics.MeasureString(iNewCount.ToString(), fNewCountFont).Height) / 2);

        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            Parent.Focus();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!bTracking && !bSelected)
            {
                bTracking = true;
                brCaptionBrush.Color = cCaptionTrackingColor;
                brNewCountBrush.Color = cNewCountTrackingColor;
                this.Refresh();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (bTracking && !bSelected)
            {
                bTracking = false;
                brCaptionBrush.Color = cCaptionColor;
                brNewCountBrush.Color = cNewCountColor;
                this.Refresh();
            }
        }
    }




    public class InfiniumProjectsFilterStateItem : Control
    {
        string sCaption = "";
        int iNewCount = 0;

        int iMarginToCount = 5;
        int iMarginCaptionLeft = 28;
        int iMarginIconLeft = 4;

        Font fCaptionFont;
        Font fNewCountFont;

        Color cCaptionColor = Color.FromArgb(60, 60, 60);
        Color cCaptionTrackingColor = Color.FromArgb(0, 161, 214);
        Color cCaptionSelectedColor = Color.FromArgb(56, 184, 238);

        Color cNewCountColor = Color.FromArgb(60, 60, 60);
        Color cNewCountTrackingColor = Color.FromArgb(0, 161, 214);
        Color cNewCountSelectedColor = Color.FromArgb(56, 184, 238);

        SolidBrush brCaptionBrush;
        SolidBrush brNewCountBrush;

        bool bTracking = false;
        int iStatus = -1;
        bool bSelected = false;

        Bitmap IconStartSelected = Properties.Resources.ProjectStart;
        Bitmap IconStartGray = Properties.Resources.ProjectStartGray;
        Bitmap IconStopSelected = Properties.Resources.ProjectStop;
        Bitmap IconStopGray = Properties.Resources.ProjectStopGray;
        Bitmap IconPauseSelected = Properties.Resources.ProjectPause;
        Bitmap IconPauseGray = Properties.Resources.ProjectPauseGray;
        Bitmap IconCancelSelected = Properties.Resources.ProjectCanceled;
        Bitmap IconCancelGray = Properties.Resources.ProjectCanceledGray;
        Bitmap IconAllSelected = Properties.Resources.ProjectsAll;
        Bitmap IconAllGray = Properties.Resources.ProjectsAllGray;

        public InfiniumProjectsFilterStateItem()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            fCaptionFont = new Font("Segoe UI", 18.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            fNewCountFont = new Font("Segoe UI", 18.0f, FontStyle.Regular, GraphicsUnit.Pixel);

            brCaptionBrush = new SolidBrush(cCaptionColor);
            brNewCountBrush = new SolidBrush(cNewCountColor);

            this.BackColor = Color.Transparent;
            this.Cursor = Cursors.Hand;

            SetInitialSize();
        }


        public string Caption
        {
            get { return sCaption; }
            set { sCaption = value; SetInitialSize(); }
        }


        public int NewCount
        {
            get { return iNewCount; }
            set { iNewCount = value; SetInitialSize(); }
        }


        public int Status
        {
            get { return iStatus; }
            set { iStatus = value; this.Refresh(); }
        }


        public Font CaptionFont
        {
            get { return fCaptionFont; }
            set { fCaptionFont = value; this.Refresh(); SetInitialSize(); }
        }

        public Font NewCountFont
        {
            get { return fNewCountFont; }
            set { fNewCountFont = value; this.Refresh(); SetInitialSize(); }
        }


        public Color CaptionColor
        {
            get { return cCaptionColor; }
            set { cCaptionColor = value; this.Refresh(); }
        }

        public Color CaptionTrackingColor
        {
            get { return cCaptionTrackingColor; }
            set { cCaptionTrackingColor = value; this.Refresh(); }
        }

        public Color CaptionSelectedColor
        {
            get { return cCaptionSelectedColor; }
            set { cCaptionSelectedColor = value; this.Refresh(); }
        }


        public Color NewCountColor
        {
            get { return cNewCountColor; }
            set { cNewCountColor = value; this.Refresh(); }
        }

        public Color NewCountTrackingColor
        {
            get { return cNewCountTrackingColor; }
            set { cNewCountTrackingColor = value; this.Refresh(); }
        }

        public Color NewCountSelectedColor
        {
            get { return cNewCountSelectedColor; }
            set { cNewCountSelectedColor = value; this.Refresh(); }
        }


        public bool Selected
        {
            get { return bSelected; }
            set
            {
                bSelected = value;

                if (value)
                {
                    brCaptionBrush.Color = cCaptionSelectedColor;
                    brNewCountBrush.Color = cNewCountSelectedColor;
                }
                else
                {
                    brCaptionBrush.Color = cCaptionColor;
                    brNewCountBrush.Color = cNewCountColor;
                }

                this.Refresh();
            }
        }


        public void SetInitialSize()
        {
            if (sCaption.Length == 0)
            {
                this.Width = 50;
                this.Height = 25;
            }

            using (Graphics G = this.CreateGraphics())
            {
                if (iNewCount > 0)
                    this.Width = Convert.ToInt32(G.MeasureString(sCaption, fCaptionFont).Width) + Convert.ToInt32(G.MeasureString(iNewCount.ToString(), fNewCountFont).Width + iMarginToCount + iMarginCaptionLeft + iMarginIconLeft);
                else
                    this.Width = Convert.ToInt32(G.MeasureString(sCaption, fCaptionFont).Width + iMarginCaptionLeft + iMarginIconLeft);

                this.Height = Convert.ToInt32(G.MeasureString(sCaption, fCaptionFont).Height);
            }
        }



        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);

            if (sCaption.Length == 0)
                return;

            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;


            //SelectedGreenBMP.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);

            if (bSelected)
            {
                //e.Graphics.DrawImage(SelectedGreenBMP, 0, (this.Height - (this.Height - 8)) / 2, SelectedGreenBMP.Width * (this.Height - 8) / SelectedGreenBMP.Height, this.Height - 8);

                if (Status == 0)//start
                {
                    IconStartSelected.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);

                    e.Graphics.DrawImage(IconStartSelected, iMarginIconLeft, (this.Height - IconStartSelected.Height) / 2 + 2);
                }

                if (Status == 1)//pause
                {
                    IconPauseSelected.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);

                    e.Graphics.DrawImage(IconPauseSelected, iMarginIconLeft, (this.Height - IconPauseSelected.Height) / 2 + 2);
                }

                if (Status == 2)//cancel
                {
                    IconCancelSelected.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);

                    e.Graphics.DrawImage(IconCancelSelected, iMarginIconLeft, (this.Height - IconCancelSelected.Height) / 2 + 2);
                }

                if (Status == 3)//stop
                {
                    IconStopSelected.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);

                    e.Graphics.DrawImage(IconStopSelected, iMarginIconLeft, (this.Height - IconStopSelected.Height) / 2 + 2);
                }

                if (Status == 4)//all
                {
                    IconAllSelected.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);

                    e.Graphics.DrawImage(IconAllSelected, iMarginIconLeft, (this.Height - IconAllSelected.Height) / 2 + 2);
                }
            }
            else
            {
                if (Status == 0)//start
                {
                    IconStartGray.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);

                    e.Graphics.DrawImage(IconStartGray, iMarginIconLeft, (this.Height - IconStartGray.Height) / 2 + 2);
                }

                if (Status == 1)//pause
                {
                    IconPauseGray.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);

                    e.Graphics.DrawImage(IconPauseGray, iMarginIconLeft, (this.Height - IconPauseGray.Height) / 2 + 2);
                }

                if (Status == 2)//cancel
                {
                    IconCancelGray.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);

                    e.Graphics.DrawImage(IconCancelGray, iMarginIconLeft, (this.Height - IconCancelGray.Height) / 2 + 2);
                }

                if (Status == 3)//stop
                {
                    IconStopGray.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);

                    e.Graphics.DrawImage(IconStopGray, iMarginIconLeft, (this.Height - IconStopGray.Height) / 2 + 2);
                }

                if (Status == 4)//all
                {
                    IconAllGray.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);

                    e.Graphics.DrawImage(IconAllGray, iMarginIconLeft, (this.Height - IconAllGray.Height) / 2 + 2);
                }
            }


            e.Graphics.DrawString(sCaption, fCaptionFont, brCaptionBrush, iMarginCaptionLeft, 0);

            if (iNewCount > 0)
                e.Graphics.DrawString(iNewCount.ToString(), fNewCountFont, brNewCountBrush,
                                        e.Graphics.MeasureString(sCaption, fCaptionFont).Width + iMarginToCount + iMarginCaptionLeft,
                                        (this.Height - e.Graphics.MeasureString(iNewCount.ToString(), fNewCountFont).Height) / 2);

        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            Parent.Focus();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!bTracking && !bSelected)
            {
                bTracking = true;
                brCaptionBrush.Color = cCaptionTrackingColor;
                brNewCountBrush.Color = cNewCountTrackingColor;
                this.Refresh();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (bTracking && !bSelected)
            {
                bTracking = false;
                brCaptionBrush.Color = cCaptionColor;
                brNewCountBrush.Color = cNewCountColor;
                this.Refresh();
            }
        }
    }




    public class InfiniumProjectsFilterGroups : Control
    {
        public int Offset = 0;

        int iTempScrollWheelOffset = 0;

        int iMarginToNextItem = 7;

        int iSelected = 0;

        int iUsersHeight = 0;
        int iDepartmentsHeight = 0;

        bool bUsersVisible = false;
        bool bDepartmentsVisible = false;

        Font fCaptionFont;
        Font fNewCountFont;

        Font fSubItemsCaptionFont;
        Font fSubItemsNewCountFont;

        Color cCaptionColor = Color.FromArgb(60, 60, 60);
        Color cCaptionTrackingColor = Color.FromArgb(0, 161, 214);
        Color cCaptionSelectedColor = Color.FromArgb(56, 184, 238);

        Color cNewCountColor = Color.FromArgb(60, 60, 60);
        Color cNewCountTrackingColor = Color.FromArgb(0, 161, 214);
        Color cNewCountSelectedColor = Color.FromArgb(56, 184, 238);

        Color cSubItemsCaptionColor = Color.FromArgb(60, 60, 60);
        Color cSubItemsCaptionTrackingColor = Color.FromArgb(0, 161, 214);
        Color cSubItemsCaptionSelectedColor = Color.FromArgb(56, 184, 238);

        Color cSubItemsNewCountColor = Color.FromArgb(60, 60, 60);
        Color cSubItemsNewCountTrackingColor = Color.FromArgb(0, 161, 214);
        Color cSubItemsNewCountSelectedColor = Color.FromArgb(56, 184, 238);

        public InfiniumScrollContainer ScrollContainer;
        public InfiniumProjectsVerticalScrollBar VerticalScroll;

        public InfiniumProjectsFilterGroupItem[] GroupItems;
        public InfiniumProjectsFilterGroupItem[] UsersItems;
        public InfiniumProjectsFilterGroupItem[] DepartmentsItems;

        DataTable UsersDT;
        DataTable DepartmentsDT;

        int iSelectedUser = -1;
        int iSelectedDepartment = -1;

        public InfiniumProjectsFilterGroups()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);

            VerticalScroll = new InfiniumProjectsVerticalScrollBar()
            {
                Parent = this,
                Height = this.Height,
                Width = 12
            };
            VerticalScroll.Left = this.Width - VerticalScroll.Width;
            VerticalScroll.ScrollWheelOffset = 30;
            VerticalScroll.TotalControlHeight = this.Height;
            VerticalScroll.Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            VerticalScroll.ScrollPositionChanged += OnScrollPositionChanged;

            ScrollContainer = new InfiniumScrollContainer()
            {
                Parent = this,
                Top = 0,
                Left = 0,
                Width = this.Width - VerticalScroll.Width,
                Height = this.Height,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            //ScrollContainer.Paint += OnScrollContainerPaint;
            //ScrollContainer.SizeChanged += OnScrollContainerSizeChanged;

            fCaptionFont = new Font("Segoe UI", 18.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            fNewCountFont = new Font("Segoe UI", 18.0f, FontStyle.Regular, GraphicsUnit.Pixel);

            fSubItemsCaptionFont = new Font("Segoe UI", 18.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            fSubItemsNewCountFont = new Font("Segoe UI", 18.0f, FontStyle.Regular, GraphicsUnit.Pixel);

            Initialize();
        }



        public Font CaptionFont
        {
            get { return fCaptionFont; }
            set
            {
                fCaptionFont = value;

                foreach (InfiniumProjectsFilterGroupItem Item in GroupItems)
                {
                    Item.CaptionFont = value;
                }
            }
        }

        public Font NewCountFont
        {
            get { return fNewCountFont; }
            set
            {
                fNewCountFont = value;

                foreach (InfiniumProjectsFilterGroupItem Item in GroupItems)
                {
                    Item.NewCountFont = value;
                }
            }
        }

        public Font SubItemsCaptionFont
        {
            get { return fSubItemsCaptionFont; }
            set
            {
                fSubItemsCaptionFont = value;

                if (UsersItems != null)
                    foreach (InfiniumProjectsFilterGroupItem Item in UsersItems)
                    {
                        Item.CaptionFont = value;
                    }

                if (DepartmentsItems != null)
                    foreach (InfiniumProjectsFilterGroupItem Item in DepartmentsItems)
                    {
                        Item.CaptionFont = value;
                    }
            }
        }

        public Font SubItemsNewCountFont
        {
            get { return fSubItemsNewCountFont; }
            set
            {
                fSubItemsNewCountFont = value;

                if (UsersItems != null)
                    foreach (InfiniumProjectsFilterGroupItem Item in UsersItems)
                    {
                        Item.NewCountFont = value;
                    }

                if (DepartmentsItems != null)
                    foreach (InfiniumProjectsFilterGroupItem Item in DepartmentsItems)
                    {
                        Item.NewCountFont = value;
                    }
            }
        }


        public Color CaptionColor
        {
            get { return cCaptionColor; }
            set
            {
                cCaptionColor = value;

                foreach (InfiniumProjectsFilterGroupItem Item in GroupItems)
                {
                    Item.CaptionColor = value;
                }
            }
        }

        public Color CaptionTrackingColor
        {
            get { return cCaptionTrackingColor; }
            set
            {
                cCaptionTrackingColor = value;

                foreach (InfiniumProjectsFilterGroupItem Item in GroupItems)
                {
                    Item.CaptionTrackingColor = value;
                }
            }
        }

        public Color CaptionSelectedColor
        {
            get { return cCaptionSelectedColor; }
            set
            {
                cCaptionSelectedColor = value;

                foreach (InfiniumProjectsFilterGroupItem Item in GroupItems)
                {
                    Item.CaptionSelectedColor = value;
                }
            }
        }


        public Color SubItemsCaptionColor
        {
            get { return cSubItemsCaptionColor; }
            set
            {
                cSubItemsCaptionColor = value;

                if (UsersItems != null)
                    foreach (InfiniumProjectsFilterGroupItem Item in UsersItems)
                    {
                        Item.CaptionColor = value;
                    }

                if (DepartmentsItems != null)
                    foreach (InfiniumProjectsFilterGroupItem Item in DepartmentsItems)
                    {
                        Item.CaptionColor = value;
                    }
            }
        }

        public Color SubItemsCaptionTrackingColor
        {
            get { return cSubItemsCaptionTrackingColor; }
            set
            {
                cSubItemsCaptionTrackingColor = value;

                if (UsersItems != null)
                    foreach (InfiniumProjectsFilterGroupItem Item in UsersItems)
                    {
                        Item.CaptionTrackingColor = value;
                    }

                if (DepartmentsItems != null)
                    foreach (InfiniumProjectsFilterGroupItem Item in DepartmentsItems)
                    {
                        Item.CaptionTrackingColor = value;
                    }
            }
        }

        public Color SubItemsCaptionSelectedColor
        {
            get { return cSubItemsCaptionSelectedColor; }
            set
            {
                cSubItemsCaptionSelectedColor = value;

                if (UsersItems != null)
                    foreach (InfiniumProjectsFilterGroupItem Item in UsersItems)
                    {
                        Item.CaptionSelectedColor = value;
                    }

                if (DepartmentsItems != null)
                    foreach (InfiniumProjectsFilterGroupItem Item in DepartmentsItems)
                    {
                        Item.CaptionSelectedColor = value;
                    }
            }
        }


        public Color NewCountColor
        {
            get { return cNewCountColor; }
            set
            {
                cNewCountColor = value;

                foreach (InfiniumProjectsFilterGroupItem Item in GroupItems)
                {
                    Item.NewCountColor = value;
                }
            }
        }

        public Color NewCountTrackingColor
        {
            get { return cNewCountTrackingColor; }
            set
            {
                cNewCountTrackingColor = value;

                foreach (InfiniumProjectsFilterGroupItem Item in GroupItems)
                {
                    Item.NewCountTrackingColor = value;
                }
            }
        }

        public Color NewCountSelectedColor
        {
            get { return cNewCountSelectedColor; }
            set
            {
                cNewCountSelectedColor = value;

                foreach (InfiniumProjectsFilterGroupItem Item in GroupItems)
                {
                    Item.NewCountSelectedColor = value;
                }
            }
        }


        public Color SubItemsNewCountColor
        {
            get { return cSubItemsNewCountColor; }
            set
            {
                cSubItemsNewCountColor = value;

                if (UsersItems != null)
                    foreach (InfiniumProjectsFilterGroupItem Item in UsersItems)
                    {
                        Item.NewCountColor = value;
                    }

                if (DepartmentsItems != null)
                    foreach (InfiniumProjectsFilterGroupItem Item in DepartmentsItems)
                    {
                        Item.NewCountColor = value;
                    }
            }
        }

        public Color SubItemsNewCountTrackingColor
        {
            get { return cSubItemsNewCountTrackingColor; }
            set
            {
                cSubItemsNewCountTrackingColor = value;

                if (UsersItems != null)
                    foreach (InfiniumProjectsFilterGroupItem Item in UsersItems)
                    {
                        Item.NewCountTrackingColor = value;
                    }

                if (DepartmentsItems != null)
                    foreach (InfiniumProjectsFilterGroupItem Item in DepartmentsItems)
                    {
                        Item.NewCountTrackingColor = value;
                    }
            }
        }

        public Color SubItemsNewCountSelectedColor
        {
            get { return cSubItemsNewCountSelectedColor; }
            set
            {
                cSubItemsNewCountSelectedColor = value;

                if (UsersItems != null)
                    foreach (InfiniumProjectsFilterGroupItem Item in UsersItems)
                    {
                        Item.NewCountSelectedColor = value;
                    }

                if (DepartmentsItems != null)
                    foreach (InfiniumProjectsFilterGroupItem Item in DepartmentsItems)
                    {
                        Item.NewCountSelectedColor = value;
                    }
            }
        }


        public int Selected
        {
            get { return iSelected; }
            set
            {
                if (iSelected == value)
                    return;

                iSelected = value;

                if (GroupItems != null)
                    GroupItems[iSelected].Selected = true;

                OnSelectedChanged(this, null);
            }
        }

        public int SelectedUser
        {
            get { return iSelectedUser; }
            set
            {
                if (iSelectedUser != value)
                {
                    iSelectedUser = value;
                    OnSelectedChanged(this, null);
                }
            }
        }

        public int SelectedDepartment
        {
            get { return iSelectedDepartment; }
            set
            {
                if (iSelectedDepartment != value)
                {
                    iSelectedDepartment = value;
                    OnSelectedChanged(this, null);
                }
            }
        }


        public void DeselectAll()
        {
            foreach (InfiniumProjectsFilterGroupItem GroupItem in GroupItems)
            {
                if (GroupItem.Selected)
                    GroupItem.Selected = false;
            }

            foreach (InfiniumProjectsFilterGroupItem UserItem in UsersItems)
            {
                if (UserItem.Selected)
                    UserItem.Selected = false;
            }

            foreach (InfiniumProjectsFilterGroupItem DepartmentItem in DepartmentsItems)
            {
                if (DepartmentItem.Selected)
                    DepartmentItem.Selected = false;
            }

            iSelected = -1;
            iSelectedDepartment = -1;
            iSelectedUser = -1;
        }

        public void Initialize()
        {
            GroupItems = new InfiniumProjectsFilterGroupItem[4];

            int iCurPosY = 0;

            GroupItems[0] = new InfiniumProjectsFilterGroupItem()
            {
                Parent = ScrollContainer,
                Width = ScrollContainer.Width,
                Caption = "Мои проекты",
                Status = 0,
                Top = iCurPosY,
                Left = 0,
                Selected = true
            };
            GroupItems[0].Click += OnGroupItemClick;

            iCurPosY += GroupItems[0].Height;

            GroupItems[1] = new InfiniumProjectsFilterGroupItem()
            {
                Parent = ScrollContainer,
                Width = ScrollContainer.Width,
                Caption = "Сотрудники",
                Status = 1,
                Top = iCurPosY + iMarginToNextItem,
                Left = 0
            };
            GroupItems[1].Click += OnGroupItemClick;

            iCurPosY += GroupItems[1].Height + iMarginToNextItem;

            GroupItems[2] = new InfiniumProjectsFilterGroupItem()
            {
                Parent = ScrollContainer,
                Width = ScrollContainer.Width,
                Caption = "Отделы",
                Status = 2,
                Top = iCurPosY + iMarginToNextItem,
                Left = 0
            };
            GroupItems[2].Click += OnGroupItemClick;

            iCurPosY += GroupItems[2].Height + iMarginToNextItem;

            GroupItems[3] = new InfiniumProjectsFilterGroupItem()
            {
                Parent = ScrollContainer,
                Width = ScrollContainer.Width,
                Caption = "Все",
                Status = 3,
                Top = iCurPosY + iMarginToNextItem,
                Left = 0
            };
            GroupItems[3].Click += OnGroupItemClick;

            iCurPosY += GroupItems[3].Height + iMarginToNextItem;

            this.Height = iCurPosY;
            ScrollContainer.Height = this.Height;
            VerticalScroll.TotalControlHeight = this.Height;
        }

        public void CreateUsersItems()
        {
            int iCurPosY = 0;

            if (UsersItems != null)
            {
                for (int i = 0; i < UsersItems.Count(); i++)
                {
                    UsersItems[i].Dispose();
                }

                UsersItems = new InfiniumProjectsFilterGroupItem[0];
            }

            if (UsersDT == null)
                return;

            UsersItems = new InfiniumProjectsFilterGroupItem[UsersDT.Rows.Count];

            for (int i = 0; i < UsersDT.Rows.Count; i++)
            {
                UsersItems[i] = new InfiniumProjectsFilterGroupItem()
                {
                    Parent = ScrollContainer,
                    Width = ScrollContainer.Width,
                    Visible = false,
                    Caption = UsersDT.Rows[i]["Name"].ToString(),
                    Top = iCurPosY + iMarginToNextItem,
                    CaptionFont = new System.Drawing.Font("Segoe UI", 16.0f, FontStyle.Regular, GraphicsUnit.Pixel),
                    NewCountFont = new System.Drawing.Font("Segoe UI", 16.0f, FontStyle.Regular, GraphicsUnit.Pixel),
                    Left = 10
                };
                UsersItems[i].Click += OnUsersItemClick;

                iCurPosY += iMarginToNextItem + UsersItems[i].Height;
            }
        }

        public void CreateDepartmentsItems()
        {
            int iCurPosY = 0;

            if (DepartmentsItems != null)
            {
                for (int i = 0; i < DepartmentsItems.Count(); i++)
                {
                    DepartmentsItems[i].Dispose();
                }

                DepartmentsItems = new InfiniumProjectsFilterGroupItem[0];
            }

            if (DepartmentsDT == null)
                return;

            DepartmentsItems = new InfiniumProjectsFilterGroupItem[DepartmentsDT.Rows.Count];

            for (int i = 0; i < DepartmentsDT.Rows.Count; i++)
            {
                DepartmentsItems[i] = new InfiniumProjectsFilterGroupItem()
                {
                    Parent = ScrollContainer,
                    Width = ScrollContainer.Width,
                    Visible = false,
                    Caption = DepartmentsDT.Rows[i]["DepartmentName"].ToString(),
                    Top = iCurPosY + iMarginToNextItem,
                    CaptionFont = new System.Drawing.Font("Segoe UI", 16.0f, FontStyle.Regular, GraphicsUnit.Pixel),
                    NewCountFont = new System.Drawing.Font("Segoe UI", 16.0f, FontStyle.Regular, GraphicsUnit.Pixel),
                    Left = 10
                };
                DepartmentsItems[i].Click += OnDepartmentItemClick;

                iCurPosY += iMarginToNextItem + DepartmentsItems[i].Height;
            }
        }

        public DataTable UsersDataTable
        {
            get { return UsersDT; }
            set
            {
                //bool bVis = false;

                if (bUsersVisible)
                {
                    //{
                    //    if (UsersItems.Count() == 0)
                    //        bUsersVisible = false;
                    //    else
                    HideUsers();
                    //}
                    // bVis = true;
                }

                UsersDT = value;
                CreateUsersItems();

                //if (bVis && UsersDT.Rows.Count > 0)
                //    ShowUsers();
            }
        }

        public DataTable DepartmentsDataTable
        {
            get { return DepartmentsDT; }
            set
            {
                //bool bVis = false;

                if (bDepartmentsVisible)
                {
                    //if (DepartmentsItems.Count() == 0)
                    //    bDepartmentsVisible = false;
                    //else
                    HideDepartments();

                    //bVis = true;
                }

                DepartmentsDT = value;
                CreateDepartmentsItems();

                //if (bVis && DepartmentsDT.Rows.Count > 0)
                //    ShowDepartments();
            }
        }


        private void OnSelectedChange(object sender, EventArgs e)
        {
            OnSelectedChanged(sender, e);
        }

        private void OnScrollPositionChanged(object sender, int tOffset)
        {
            Offset = tOffset;
            ScrollContainer.Top = -Offset;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            VerticalScroll.Height = this.Height;
            VerticalScroll.Refresh();
        }


        private void OnGroupItemClick(object sender, EventArgs e)
        {
            if (((InfiniumProjectsFilterGroupItem)sender).Selected == false)
            {
                for (int i = 0; i < GroupItems.Count(); i++)
                {
                    if (sender != GroupItems[i])
                    {
                        GroupItems[i].Selected = false;
                    }
                    else
                    {
                        GroupItems[i].Selected = true;
                        if (i != 1 && i != 2)
                            this.Selected = i;
                        else
                            iSelected = i;
                    }
                }

                OnItemClicked(sender, e);
            }

            //users
            if (sender == GroupItems[1])
            {
                if (!bUsersVisible)
                {
                    if (bDepartmentsVisible)
                        HideDepartments();

                    ShowUsers();
                }
                else
                    if (bUsersVisible)
                {
                    if (UsersItems.Count() == 0)
                        this.Height = this.Height;
                    HideUsers();
                }
            }
            else//departments           
            {
                if (sender == GroupItems[2])
                {
                    if (!bDepartmentsVisible)
                    {
                        if (bUsersVisible)
                        {
                            if (UsersItems.Count() == 0)
                                bUsersVisible = false;
                            else
                                HideUsers();
                        }

                        ShowDepartments();
                    }
                    else
                        if (bDepartmentsVisible)
                        HideDepartments();
                }
                else//all
                {
                    if (bDepartmentsVisible)
                        HideDepartments();

                    if (bUsersVisible)
                    {
                        if (UsersItems.Count() == 0)
                            bUsersVisible = false;
                        else
                            HideUsers();
                    }
                }
            }

        }


        public void Expand()
        {
            if (UsersItems.Count() > 0 && !bUsersVisible && Selected == 1)
                ShowUsers();

            if (DepartmentsItems.Count() > 0 && !bDepartmentsVisible && Selected == 2)
                ShowDepartments();
        }

        private void ShowUsers()
        {
            bUsersVisible = true;

            //if (UsersDataTable != null)
            //{
            //    if (UsersDataTable.Rows.Count == 0)
            //    {
            //        if(bDepartmentsVisible)
            //            HideDepartments();

            //        return;
            //    }
            //}
            //else
            //{
            //    if (bDepartmentsVisible)
            //        HideDepartments();

            //    return;
            //}

            int w = GroupItems[1].Top + GroupItems[1].Height;
            iUsersHeight = 0;

            foreach (InfiniumProjectsFilterGroupItem Item in UsersItems)
            {
                Item.Top += w;
                Item.Visible = true;
                iUsersHeight += Item.Height + iMarginToNextItem;
            }

            GroupItems[2].Top += iUsersHeight;
            GroupItems[3].Top += iUsersHeight;

            //if (UsersItems.Count() > 0)
            //{
            //    UsersItems[0].Selected = true;
            //    SelectedUser = Convert.ToInt32(UsersDT.Rows[0]["UserID"]);
            //}

            ScrollContainer.Height = GroupItems[3].Top + GroupItems[3].Height;
            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
        }

        private void HideUsers()
        {
            bUsersVisible = false;
            SelectedUser = -1;

            if (UsersItems == null)
                return;

            int w = GroupItems[1].Top + GroupItems[1].Height;

            if (UsersItems.Count() > 0)
                iUsersHeight = 0;

            foreach (InfiniumProjectsFilterGroupItem Item in UsersItems)
            {
                Item.Top -= w;
                Item.Visible = false;
                Item.Selected = false;
                iUsersHeight += Item.Height + iMarginToNextItem;
            }

            GroupItems[2].Top -= iUsersHeight;
            GroupItems[3].Top -= iUsersHeight;

            ScrollContainer.Height = GroupItems[3].Top + GroupItems[3].Height;
            VerticalScroll.TotalControlHeight = ScrollContainer.Height;

            foreach (InfiniumProjectsFilterGroupItem Item in UsersItems)
            {
                if (Item.Selected)
                    Item.Selected = false;
            }
        }


        private void ShowDepartments()
        {
            bDepartmentsVisible = true;

            //if (DepartmentsDataTable != null)
            //{
            //    if (DepartmentsDataTable.Rows.Count == 0)
            //    {
            //        if(bUsersVisible)
            //        {
            //            if (UsersItems.Count() == 0)
            //                this.Height = this.Height;
            //            HideUsers();
            //        }
            //        return;
            //    }
            //}
            //else
            //{
            //    if (bUsersVisible)
            //    {
            //        if (UsersItems.Count() == 0)
            //            this.Height = this.Height;
            //        HideUsers();
            //    }
            //    return;
            //}

            //if (bUsersVisible)
            //{
            //    if (UsersItems.Count() == 0)
            //        this.Height = this.Height;
            //    HideUsers();
            //}

            int w = 0;

            if (bUsersVisible)
                w = GroupItems[2].Top + GroupItems[2].Height + iUsersHeight;
            else
                w = GroupItems[2].Top + GroupItems[2].Height;

            iDepartmentsHeight = 0;


            foreach (InfiniumProjectsFilterGroupItem Item in DepartmentsItems)
            {
                Item.Top += w;
                Item.Visible = true;
                iDepartmentsHeight += Item.Height + iMarginToNextItem;
            }

            GroupItems[3].Top += iDepartmentsHeight;

            //if (DepartmentsItems.Count() > 0)
            //{
            //    DepartmentsItems[0].Selected = true;
            //    SelectedDepartment = Convert.ToInt32(DepartmentsDT.Rows[0]["DepartmentID"]);
            //}

            ScrollContainer.Height = GroupItems[3].Top + GroupItems[3].Height;
            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
        }

        private void HideDepartments()
        {
            bDepartmentsVisible = false;
            SelectedDepartment = -1;

            if (DepartmentsItems == null)
                return;

            int w = 0;

            if (bUsersVisible)
                w = GroupItems[2].Top + GroupItems[2].Height + iUsersHeight;
            else
                w = GroupItems[2].Top + GroupItems[2].Height;

            if (DepartmentsItems.Count() > 0)
                iDepartmentsHeight = 0;

            foreach (InfiniumProjectsFilterGroupItem Item in DepartmentsItems)
            {
                Item.Top -= w;
                Item.Visible = false;
                Item.Selected = false;
                iDepartmentsHeight += Item.Height + iMarginToNextItem;
            }

            GroupItems[3].Top -= iDepartmentsHeight;

            ScrollContainer.Height = GroupItems[3].Top + GroupItems[3].Height;
            VerticalScroll.TotalControlHeight = ScrollContainer.Height;

            foreach (InfiniumProjectsFilterGroupItem Item in DepartmentsItems)
            {
                if (Item.Selected)
                    Item.Selected = false;
            }
        }



        private void OnUsersItemClick(object sender, EventArgs e)
        {
            if (UsersItems != null)
                for (int i = 0; i < UsersItems.Count(); i++)
                {
                    if (UsersItems[i] != sender)
                        UsersItems[i].Selected = false;
                    else
                    {
                        UsersItems[i].Selected = true;
                        this.SelectedUser = Convert.ToInt32(UsersDataTable.Rows[i]["UserID"]);
                    }
                }

            //((InfiniumProjectsFilterGroupItem)sender).Selected = true;

            OnUserItemClicked(sender, e);
        }

        private void OnDepartmentItemClick(object sender, EventArgs e)
        {
            if (DepartmentsItems != null)
                for (int i = 0; i < DepartmentsItems.Count(); i++)
                {
                    if (DepartmentsItems[i] != sender)
                        DepartmentsItems[i].Selected = false;
                    else
                    {
                        DepartmentsItems[i].Selected = true;
                        this.SelectedDepartment = Convert.ToInt32(DepartmentsDataTable.Rows[i]["DepartmentID"]);
                    }
                }

            OnDepartmentItemClicked(sender, e);
        }

        public event EventHandler ItemClicked;
        public event EventHandler UserItemClicked;
        public event EventHandler DepartmentItemClicked;
        public event EventHandler SelectedChanged;

        public virtual void OnSelectedChanged(object sender, EventArgs e)
        {
            SelectedChanged?.Invoke(sender, e);//Raise the event
        }


        public virtual void OnItemClicked(object sender, EventArgs e)
        {
            ItemClicked?.Invoke(sender, e);//Raise the event
        }

        public virtual void OnUserItemClicked(object sender, EventArgs e)
        {
            UserItemClicked?.Invoke(sender, e);//Raise the event
        }

        public virtual void OnDepartmentItemClicked(object sender, EventArgs e)
        {
            DepartmentItemClicked?.Invoke(sender, e);//Raise the event
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            ScrollContainer.Focus();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (!VerticalScroll.Visible)
                return;

            if (Offset == 0)
                if (iTempScrollWheelOffset > 0)
                    iTempScrollWheelOffset = 0;

            if (e.Delta < 0)
            {
                if (Offset < ScrollContainer.Height - this.Height)
                {
                    if (Offset + VerticalScroll.ScrollWheelOffset + this.Height > ScrollContainer.Height)//last
                    {
                        iTempScrollWheelOffset = ScrollContainer.Height - this.Height - Offset;
                        Offset += iTempScrollWheelOffset;
                    }
                    else
                    {
                        Offset += VerticalScroll.ScrollWheelOffset;
                    }

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
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
                        Offset -= VerticalScroll.ScrollWheelOffset;

                    if (Offset < 0)
                        Offset = 0;

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
                }
        }
    }




    public class InfiniumProjectsMembersItem : Control
    {
        string sCaption = "";

        Font fCaptionFont;

        Color cCaptionColor = Color.FromArgb(60, 60, 60);

        SolidBrush brCaptionBrush;

        public InfiniumProjectsMembersItem()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            fCaptionFont = new Font("Segoe UI", 18.0f, FontStyle.Regular, GraphicsUnit.Pixel);

            brCaptionBrush = new SolidBrush(cCaptionColor);

            this.BackColor = Color.Transparent;

            SetInitialSize();
        }


        public string Caption
        {
            get { return sCaption; }
            set { sCaption = value; SetInitialSize(); }
        }

        public Font CaptionFont
        {
            get { return fCaptionFont; }
            set { fCaptionFont = value; this.Refresh(); SetInitialSize(); }
        }

        public Color CaptionColor
        {
            get { return cCaptionColor; }
            set { cCaptionColor = value; brCaptionBrush.Color = cCaptionColor; this.Refresh(); }
        }

        public void SetInitialSize()
        {
            if (sCaption.Length == 0)
            {
                this.Width = 50;
                this.Height = 25;
            }

            using (Graphics G = this.CreateGraphics())
            {
                this.Width = Convert.ToInt32(G.MeasureString(sCaption, fCaptionFont).Width);

                this.Height = Convert.ToInt32(G.MeasureString(sCaption, fCaptionFont).Height);
            }
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);

            if (sCaption.Length == 0)
                return;

            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;


            e.Graphics.DrawString(sCaption, fCaptionFont, brCaptionBrush, 0, 0);
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            Parent.Focus();
        }
    }




    public class InfiniumProjectsMembersList : Control
    {
        public int Offset = 0;

        int iTempScrollWheelOffset = 0;

        int iMarginToNextItem = 7;

        Font fCaptionFont;

        Font fSubItemsCaptionFont;

        Color cCaptionColor = Color.FromArgb(60, 60, 60);
        Color cSubItemsCaptionColor = Color.FromArgb(60, 60, 60);

        public InfiniumScrollContainer ScrollContainer;
        public InfiniumProjectsVerticalScrollBar VerticalScroll;

        DataTable UsersDT;
        DataTable DepartmentsDT;

        InfiniumProjectsMembersItem UsersCaption;
        InfiniumProjectsMembersItem DepartmentsCaption;
        InfiniumProjectsMembersItem[] UsersMembersItem;
        InfiniumProjectsMembersItem[] DepartmentsMembersItem;


        public InfiniumProjectsMembersList()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);

            VerticalScroll = new InfiniumProjectsVerticalScrollBar()
            {
                Parent = this,
                Height = this.Height,
                Width = 12
            };
            VerticalScroll.Left = this.Width - VerticalScroll.Width;
            VerticalScroll.ScrollWheelOffset = 30;
            VerticalScroll.TotalControlHeight = this.Height;
            VerticalScroll.Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            VerticalScroll.ScrollPositionChanged += OnScrollPositionChanged;

            ScrollContainer = new InfiniumScrollContainer()
            {
                Parent = this,
                Top = 0,
                Left = 0,
                Width = this.Width - VerticalScroll.Width,
                Height = this.Height,
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            ScrollContainer.Paint += OnScrollContainerPaint;

            fCaptionFont = new Font("Segoe UI", 18.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            fSubItemsCaptionFont = new Font("Segoe UI", 18.0f, FontStyle.Regular, GraphicsUnit.Pixel);
        }


        public Font CaptionFont
        {
            get { return fCaptionFont; }
            set
            {
                fCaptionFont = value;
            }
        }

        public Font SubItemsCaptionFont
        {
            get { return fSubItemsCaptionFont; }
            set
            {
                fSubItemsCaptionFont = value;
            }
        }


        public Color CaptionColor
        {
            get { return cCaptionColor; }
            set
            {
                cCaptionColor = value;
            }
        }

        public Color SubItemsCaptionColor
        {
            get { return cSubItemsCaptionColor; }
            set
            {
                cSubItemsCaptionColor = value;
            }
        }


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public InfiniumProjectsVerticalScrollBar VerticalScrollBar
        {
            get { return VerticalScroll; }
            set { VerticalScroll = value; VerticalScroll.Left = this.Width - VerticalScroll.Width; }
        }


        public void CreateItems()
        {
            ScrollContainer.Height = this.Height;
            VerticalScroll.TotalControlHeight = this.Height;
            Offset = 0;
            ScrollContainer.Top = -Offset;
            VerticalScroll.Offset = Offset;
            VerticalScroll.Refresh();

            //clear
            if (UsersMembersItem != null)
                if (UsersMembersItem.Count() > 0)
                    foreach (InfiniumProjectsMembersItem Item in UsersMembersItem)
                    {
                        Item.Dispose();
                    }

            UsersMembersItem = new InfiniumProjectsMembersItem[0];

            if (DepartmentsMembersItem != null)
                if (DepartmentsMembersItem.Count() > 0)
                    foreach (InfiniumProjectsMembersItem Item in DepartmentsMembersItem)
                    {
                        Item.Dispose();
                    }

            DepartmentsMembersItem = new InfiniumProjectsMembersItem[0];

            if (UsersCaption != null)
                UsersCaption.Dispose();

            if (DepartmentsCaption != null)
                DepartmentsCaption.Dispose();
            ////////////////////////////////////////////////////////////////////////////////


            if (UsersDataTable == null || DepartmentsDataTable == null)
                return;


            int iCurPosY = 0;


            if (UsersDataTable.Rows.Count > 0)
            {
                UsersCaption = new InfiniumProjectsMembersItem()
                {
                    Parent = ScrollContainer,
                    Caption = "Сотрудники",
                    Top = iCurPosY,
                    Left = 0,
                    CaptionFont = fCaptionFont,
                    CaptionColor = cCaptionColor
                };
                iCurPosY += UsersCaption.Height + iMarginToNextItem;

                UsersMembersItem = new InfiniumProjectsMembersItem[UsersDataTable.Rows.Count];

                for (int i = 0; i < UsersDataTable.Rows.Count; i++)
                {
                    UsersMembersItem[i] = new InfiniumProjectsMembersItem()
                    {
                        Parent = ScrollContainer,
                        Caption = UsersDataTable.Rows[i]["Name"].ToString(),
                        Top = iCurPosY,
                        Left = 10,
                        CaptionFont = fSubItemsCaptionFont,
                        CaptionColor = cSubItemsCaptionColor
                    };
                    iCurPosY += UsersMembersItem[i].Height + iMarginToNextItem;
                }
            }


            if (DepartmentsDataTable.Rows.Count > 0)
            {
                DepartmentsCaption = new InfiniumProjectsMembersItem()
                {
                    Parent = ScrollContainer,
                    Caption = "Отделы",
                    Top = iCurPosY,
                    Left = 0,
                    CaptionFont = fCaptionFont,
                    CaptionColor = cCaptionColor
                };
                iCurPosY += DepartmentsCaption.Height + iMarginToNextItem;

                DepartmentsMembersItem = new InfiniumProjectsMembersItem[DepartmentsDataTable.Rows.Count];

                for (int i = 0; i < DepartmentsDataTable.Rows.Count; i++)
                {
                    DepartmentsMembersItem[i] = new InfiniumProjectsMembersItem()
                    {
                        Parent = ScrollContainer,
                        Caption = DepartmentsDataTable.Rows[i]["DepartmentName"].ToString(),
                        Top = iCurPosY,
                        Left = 10,
                        CaptionFont = fSubItemsCaptionFont,
                        CaptionColor = cSubItemsCaptionColor
                    };
                    iCurPosY += DepartmentsMembersItem[i].Height + iMarginToNextItem;
                }
            }

            ScrollContainer.Height = iCurPosY;
            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
        }


        private void OnScrollContainerPaint(object sender, PaintEventArgs e)
        {
            if (DepartmentsDataTable == null || UsersDataTable == null)
                return;
        }

        public DataTable UsersDataTable
        {
            get { return UsersDT; }
            set
            {
                UsersDT = value;
            }
        }

        public DataTable DepartmentsDataTable
        {
            get { return DepartmentsDT; }
            set
            {
                DepartmentsDT = value;
            }
        }

        private void OnScrollPositionChanged(object sender, int tOffset)
        {
            Offset = tOffset;
            ScrollContainer.Top = -Offset;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            VerticalScroll.Height = this.Height;
            VerticalScroll.Refresh();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (!VerticalScroll.Visible)
                return;

            if (Offset == 0)
                if (iTempScrollWheelOffset > 0)
                    iTempScrollWheelOffset = 0;

            if (e.Delta < 0)
            {
                if (Offset < ScrollContainer.Height - this.Height)
                {
                    if (Offset + VerticalScroll.ScrollWheelOffset + this.Height > ScrollContainer.Height)//last
                    {
                        iTempScrollWheelOffset = ScrollContainer.Height - this.Height - Offset;
                        Offset += iTempScrollWheelOffset;
                    }
                    else
                    {
                        Offset += VerticalScroll.ScrollWheelOffset;
                    }

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
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
                        Offset -= VerticalScroll.ScrollWheelOffset;

                    if (Offset < 0)
                        Offset = 0;

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
                }
        }
    }




    public class InfiniumProjectsFilterStates : Control
    {
        public int Offset = 0;

        int iTempScrollWheelOffset = 0;

        int iMarginToNextItem = 7;

        int iSelected = 0;

        Font fCaptionFont;
        Font fNewCountFont;

        Color cCaptionColor = Color.FromArgb(60, 60, 60);
        Color cCaptionTrackingColor = Color.FromArgb(0, 161, 214);
        Color cCaptionSelectedColor = Color.FromArgb(56, 184, 238);

        Color cNewCountColor = Color.FromArgb(60, 60, 60);
        Color cNewCountTrackingColor = Color.FromArgb(0, 161, 214);
        Color cNewCountSelectedColor = Color.FromArgb(56, 184, 238);

        public InfiniumScrollContainer ScrollContainer;
        public InfiniumProjectsVerticalScrollBar VerticalScroll;

        public InfiniumProjectsFilterStateItem[] GroupItems;

        public InfiniumProjectsFilterStates()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);

            VerticalScroll = new InfiniumProjectsVerticalScrollBar()
            {
                Parent = this,
                Height = this.Height,
                Width = 12
            };
            VerticalScroll.Left = this.Width - VerticalScroll.Width;
            VerticalScroll.ScrollWheelOffset = 30;
            VerticalScroll.TotalControlHeight = this.Height;
            VerticalScroll.Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            VerticalScroll.ScrollPositionChanged += OnScrollPositionChanged;

            ScrollContainer = new InfiniumScrollContainer()
            {
                Parent = this,
                Top = 0,
                Left = 0,
                Width = this.Width - VerticalScroll.Width,
                Height = this.Height,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            //ScrollContainer.Paint += OnScrollContainerPaint;
            //ScrollContainer.SizeChanged += OnScrollContainerSizeChanged;

            fCaptionFont = new Font("Segoe UI", 18.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            fNewCountFont = new Font("Segoe UI", 18.0f, FontStyle.Regular, GraphicsUnit.Pixel);

            Initialize();
        }



        public Font CaptionFont
        {
            get { return fCaptionFont; }
            set
            {
                fCaptionFont = value;

                if (GroupItems != null)
                    foreach (InfiniumProjectsFilterStateItem Item in GroupItems)
                    {
                        Item.CaptionFont = value;
                    }
            }
        }

        public Font NewCountFont
        {
            get { return fNewCountFont; }
            set
            {
                fNewCountFont = value;

                if (GroupItems != null)
                    foreach (InfiniumProjectsFilterStateItem Item in GroupItems)
                    {
                        Item.NewCountFont = value;
                    }
            }
        }


        public Color CaptionColor
        {
            get { return cCaptionColor; }
            set
            {
                cCaptionColor = value;

                if (GroupItems != null)
                    foreach (InfiniumProjectsFilterStateItem Item in GroupItems)
                    {
                        Item.CaptionColor = value;
                    }
            }
        }

        public Color CaptionTrackingColor
        {
            get { return cCaptionTrackingColor; }
            set
            {
                cCaptionTrackingColor = value;

                if (GroupItems != null)
                    foreach (InfiniumProjectsFilterStateItem Item in GroupItems)
                    {
                        Item.CaptionTrackingColor = value;
                    }
            }
        }

        public Color CaptionSelectedColor
        {
            get { return cCaptionSelectedColor; }
            set
            {
                cCaptionSelectedColor = value;

                if (GroupItems != null)
                    foreach (InfiniumProjectsFilterStateItem Item in GroupItems)
                    {
                        Item.CaptionSelectedColor = value;
                    }
            }
        }


        public Color NewCountColor
        {
            get { return cNewCountColor; }
            set
            {
                cNewCountColor = value;

                if (GroupItems != null)
                    foreach (InfiniumProjectsFilterStateItem Item in GroupItems)
                    {
                        Item.NewCountColor = value;
                    }
            }
        }

        public Color NewCountTrackingColor
        {
            get { return cNewCountTrackingColor; }
            set
            {
                cNewCountTrackingColor = value;

                if (GroupItems != null)
                    foreach (InfiniumProjectsFilterStateItem Item in GroupItems)
                    {
                        Item.NewCountTrackingColor = value;
                    }
            }
        }

        public Color NewCountSelectedColor
        {
            get { return cNewCountSelectedColor; }
            set
            {
                cNewCountSelectedColor = value;

                if (GroupItems != null)
                    foreach (InfiniumProjectsFilterStateItem Item in GroupItems)
                    {
                        Item.NewCountSelectedColor = value;
                    }
            }
        }


        public int Selected
        {
            get { return iSelected; }
            set
            {
                iSelected = value;

                if (GroupItems != null)
                    GroupItems[iSelected].Selected = true;
            }
        }


        public void Initialize()
        {
            GroupItems = new InfiniumProjectsFilterStateItem[5];

            int iCurPosY = 0;

            GroupItems[0] = new InfiniumProjectsFilterStateItem()
            {
                Parent = ScrollContainer,
                Width = ScrollContainer.Width,
                Caption = "Выполняются",
                Top = iCurPosY,
                Status = 0,
                Left = 0,
                Selected = true
            };
            GroupItems[0].Click += OnGroupItemClick;

            iCurPosY += GroupItems[0].Height;

            GroupItems[1] = new InfiniumProjectsFilterStateItem()
            {
                Parent = ScrollContainer,
                Width = ScrollContainer.Width,
                Caption = "Приостановлены",
                Top = iCurPosY + iMarginToNextItem,
                Status = 1,
                Left = 0
            };
            GroupItems[1].Click += OnGroupItemClick;

            iCurPosY += GroupItems[1].Height + iMarginToNextItem;

            GroupItems[2] = new InfiniumProjectsFilterStateItem()
            {
                Parent = ScrollContainer,
                Width = ScrollContainer.Width,
                Caption = "Отменены",
                Top = iCurPosY + iMarginToNextItem,
                Status = 2,
                Left = 0
            };
            GroupItems[2].Click += OnGroupItemClick;

            iCurPosY += GroupItems[2].Height + iMarginToNextItem;

            GroupItems[3] = new InfiniumProjectsFilterStateItem()
            {
                Parent = ScrollContainer,
                Width = ScrollContainer.Width,
                Caption = "Завершены",
                Top = iCurPosY + iMarginToNextItem,
                Status = 3,
                Left = 0
            };
            GroupItems[3].Click += OnGroupItemClick;

            iCurPosY += GroupItems[3].Height + iMarginToNextItem;

            GroupItems[4] = new InfiniumProjectsFilterStateItem()
            {
                Parent = ScrollContainer,
                Width = ScrollContainer.Width,
                Caption = "Все",
                Top = iCurPosY + iMarginToNextItem,
                Status = 4,
                Left = 0
            };
            GroupItems[4].Click += OnGroupItemClick;

            iCurPosY += GroupItems[4].Height + iMarginToNextItem;

            this.Height = iCurPosY;
            ScrollContainer.Height = this.Height;
            VerticalScroll.TotalControlHeight = this.Height;
        }

        public void DeselectAll()
        {
            foreach (InfiniumProjectsFilterStateItem GroupItem in GroupItems)
            {
                GroupItem.Selected = false;
            }

            iSelected = -1;
        }

        private void OnScrollPositionChanged(object sender, int tOffset)
        {
            Offset = tOffset;
            ScrollContainer.Top = -Offset;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            VerticalScroll.Height = this.Height;
            VerticalScroll.Refresh();
        }


        private void OnGroupItemClick(object sender, EventArgs e)
        {
            if (((InfiniumProjectsFilterStateItem)sender).Selected == false)
            {
                for (int i = 0; i < GroupItems.Count(); i++)
                {
                    if (sender != GroupItems[i])
                        GroupItems[i].Selected = false;
                    else
                    {
                        GroupItems[i].Selected = true;
                        this.Selected = i;
                    }
                }

                OnItemClicked(sender, e);

            }

        }



        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            ScrollContainer.Focus();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (!VerticalScroll.Visible)
                return;

            if (Offset == 0)
                if (iTempScrollWheelOffset > 0)
                    iTempScrollWheelOffset = 0;

            if (e.Delta < 0)
            {
                if (Offset < ScrollContainer.Height - this.Height)
                {
                    if (Offset + VerticalScroll.ScrollWheelOffset + this.Height > ScrollContainer.Height)//last
                    {
                        iTempScrollWheelOffset = ScrollContainer.Height - this.Height - Offset;
                        Offset += iTempScrollWheelOffset;
                    }
                    else
                    {
                        Offset += VerticalScroll.ScrollWheelOffset;
                    }

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
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
                        Offset -= VerticalScroll.ScrollWheelOffset;

                    if (Offset < 0)
                        Offset = 0;

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
                }
        }


        public event EventHandler ItemClicked;

        public virtual void OnItemClicked(object sender, EventArgs e)
        {
            ItemClicked?.Invoke(sender, e);//Raise the event
        }

    }




    public enum InfiniumProjectStates
    {
        Start = 0,
        Pause = 1,
        Cancel = 2,
        Stop = 3
    }




    public class InfiniumProjectsListItem : Control
    {
        Color cCaptionColor = Color.FromArgb(60, 60, 60);
        Color cStatusColor = Color.FromArgb(150, 150, 150);

        Color cBackColor = Color.White;
        Color cBackSelectedColor = Color.FromArgb(245, 252, 255);

        Color cBorderColor = Color.FromArgb(240, 240, 240);

        Pen pBorderPen;

        Font fCaptionFont = new Font("Segoe UI", 17.0f, FontStyle.Regular, GraphicsUnit.Pixel);
        Font fStatusFont = new Font("Segoe UI", 14.0f, FontStyle.Regular, GraphicsUnit.Pixel);

        SolidBrush brCaptionBrush;
        SolidBrush brStatusBrush;

        bool bSelected = false;

        public string sAuthor = "";
        string sCaption = "Project Caption";

        public bool bPropos = false;

        InfiniumProjectStates eStatus;

        Bitmap IconStart = Properties.Resources.ProjectItemStart;
        Bitmap IconStop = Properties.Resources.ProjectItemStop;
        Bitmap IconCancel = Properties.Resources.ProjectItemCanceled;
        Bitmap IconPause = Properties.Resources.ProjectItemPause;

        public InfiniumProjectsListItem()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            this.BackColor = Color.White;
            this.Cursor = Cursors.Hand;

            brCaptionBrush = new SolidBrush(cCaptionColor);
            brStatusBrush = new SolidBrush(cStatusColor);

            pBorderPen = new Pen(new SolidBrush(cBorderColor));

            this.Height = 50;
            this.Width = 100;
        }


        public Color BorderColor
        {
            get { return cBorderColor; }
            set { cBorderColor = value; pBorderPen.Color = value; this.Refresh(); }
        }

        public Color CaptionColor
        {
            get { return cCaptionColor; }
            set { cCaptionColor = value; this.Refresh(); }
        }

        public Color StatusColor
        {
            get { return cStatusColor; }
            set { cStatusColor = value; this.Refresh(); }
        }

        public Color BackSelectedColor
        {
            get { return cBackSelectedColor; }
            set { cBackSelectedColor = value; this.Refresh(); }
        }

        public Font CaptionFont
        {
            get { return fCaptionFont; }
            set { fCaptionFont = value; this.Refresh(); }
        }

        public Font StatusFont
        {
            get { return fStatusFont; }
            set { fStatusFont = value; this.Refresh(); }
        }

        public bool Selected
        {
            get { return bSelected; }
            set
            {
                bSelected = value;

                if (value)
                    this.BackColor = cBackSelectedColor;
                else
                    this.BackColor = cBackColor;

                this.Refresh();
            }
        }

        public string Caption
        {
            get { return sCaption; }
            set { sCaption = value; this.Refresh(); }
        }

        public InfiniumProjectStates Status
        {
            get { return eStatus; }
            set { eStatus = value; this.Refresh(); }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

            string text = "";

            if (e.Graphics.MeasureString(sCaption, fCaptionFont).Width > this.Width)
            {
                for (int i = sCaption.Length; i >= 0; i--)
                {
                    if (e.Graphics.MeasureString(sCaption.Substring(0, i), fCaptionFont).Width <= this.Width)
                    {
                        text = sCaption.Substring(0, i - 1) + "...";
                        break;
                    }
                }
            }
            else
                text = sCaption;

            e.Graphics.DrawString(text, fCaptionFont, brCaptionBrush, 0, 0);

            int iCaptHeight = Convert.ToInt32(e.Graphics.MeasureString(text, fCaptionFont).Height);

            if (eStatus == InfiniumProjectStates.Start)
            {
                e.Graphics.DrawImage(IconStart, 4, iCaptHeight + 3, 16 * IconStart.Width / IconStart.Height, 16);
                e.Graphics.DrawString("Выполняется", fStatusFont, brStatusBrush, 16 + 7, iCaptHeight + 1);
            }

            if (eStatus == InfiniumProjectStates.Pause)
            {
                e.Graphics.DrawImage(IconPause, 4, iCaptHeight + 3, 16 * IconPause.Width / IconPause.Height, 16);
                e.Graphics.DrawString("Приостановлен", fStatusFont, brStatusBrush, 16 + 7, iCaptHeight + 1);
            }

            if (eStatus == InfiniumProjectStates.Stop)
            {
                e.Graphics.DrawImage(IconStop, 4, iCaptHeight + 3, 16 * IconStop.Width / IconStop.Height, 16);
                e.Graphics.DrawString("Завершен", fStatusFont, brStatusBrush, 16 + 7, iCaptHeight + 1);
            }

            if (eStatus == InfiniumProjectStates.Cancel)
            {
                e.Graphics.DrawImage(IconCancel, 4, iCaptHeight + 3, 16 * IconCancel.Width / IconCancel.Height, 16);
                e.Graphics.DrawString("Отменен", fStatusFont, brStatusBrush, 16 + 7, iCaptHeight + 1);
            }


            e.Graphics.DrawLine(pBorderPen, 0, this.Height - 1, this.Width, this.Height - 1);
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            Parent.Focus();
        }
    }




    public class InfiniumProjectsList : Control
    {
        public int Offset = 0;

        int iTempScrollWheelOffset = 0;

        InfiniumScrollContainer ScrollContainer;
        public InfiniumProjectsVerticalScrollBar VerticalScroll;

        public InfiniumProjectsListItem[] ProjectItems;
        public DataTable UsersDataTable;

        DataTable ProjectsDT;

        int iSelected = -1;

        int iProjectID = -1;

        public bool bPropos = false;

        public InfiniumProjectsList()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);

            VerticalScroll = new InfiniumProjectsVerticalScrollBar()
            {
                Parent = this,
                Height = this.Height
            };
            VerticalScroll.Left = this.Width - VerticalScroll.Width;
            VerticalScroll.TotalControlHeight = this.Height;
            VerticalScroll.ScrollPositionChanged += OnScrollPositionChanged;

            ScrollContainer = new InfiniumScrollContainer()
            {
                Parent = this,
                Top = 0,
                Left = 0,
                Width = this.Width,
                Height = this.Height,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            ScrollContainer.SizeChanged += OnScrollContainerSizeChanged;
        }

        public void InitializeItems()
        {
            if (ProjectItems != null)
            {
                for (int i = 0; i < ProjectItems.Count(); i++)
                {
                    if (ProjectItems[i] != null)
                    {
                        ProjectItems[i].Dispose();
                        ProjectItems[i] = null;
                    }
                }

                ProjectItems = new InfiniumProjectsListItem[0];
            }

            Offset = 0;
            ScrollContainer.Top = -Offset;
            VerticalScroll.Offset = Offset;
            VerticalScroll.Refresh();

            if (ProjectsDT == null)
                return;

            if (ProjectsDT.DefaultView.Count > 0)
            {
                ProjectItems = new InfiniumProjectsListItem[ProjectsDT.Rows.Count];

                for (int i = 0; i < ProjectsDT.DefaultView.Count; i++)
                {
                    ProjectItems[i] = new InfiniumProjectsListItem()
                    {
                        Parent = ScrollContainer,
                        Height = 60,
                        Top = i * 60,
                        Status = (InfiniumProjectStates)Convert.ToInt32(ProjectsDT.DefaultView[i]["ProjectStatusID"]),
                        Caption = ProjectsDT.DefaultView[i]["ProjectName"].ToString(),
                        Width = ScrollContainer.Width,
                        Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
                    };
                    ProjectItems[i].Click += OnProjectItemClick;
                }
            }

            if (ProjectItems == null || ProjectsDataTable.Rows.Count == 0)
                return;

            ProjectItems[0].Selected = true;
            Selected = 0;
            OnSelectedChanged(this, null);

            if (ProjectItems.Count() * 60 > this.Height)
                ScrollContainer.Height = ProjectItems.Count() * 60;
            else
                ScrollContainer.Height = this.Height;

            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
            VerticalScroll.Refresh();
            //this.Refresh();
        }



        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public InfiniumProjectsVerticalScrollBar VerticalScrollBar
        {
            get { return VerticalScroll; }
            set { VerticalScroll = value; VerticalScroll.Left = this.Width - VerticalScroll.Width; }
        }


        public DataTable ProjectsDataTable
        {
            get { return ProjectsDT; }
            set
            {
                ProjectsDT = value;
                Selected = -1;
            }
        }

        public int Selected
        {
            get { return iSelected; }
            set
            {
                iSelected = value;

                if (iSelected == -1)
                {
                    iProjectID = -1;
                    return;
                }

                if (ProjectsDT == null)
                {
                    iProjectID = -1;
                    return;
                }

                if (ProjectsDT.Rows.Count == 0)
                {
                    iProjectID = -1;
                    return;
                }

                iProjectID = Convert.ToInt32(ProjectsDT.Rows[iSelected]["ProjectID"]);

                for (int i = 0; i < ProjectItems.Count(); i++)
                {
                    if (i != iSelected)
                        ProjectItems[i].Selected = false;
                    else
                        ProjectItems[i].Selected = true;
                }

                OnSelectedChanged(this, null);
            }
        }

        public int ProjectID
        {
            get
            {
                if (iSelected > -1)
                    return Convert.ToInt32(ProjectsDT.Rows[iSelected]["ProjectID"]);
                else
                    return -1;
            }
        }

        public void Filter()
        {
            InitializeItems();

            if (ProjectItems == null)
            {
                Selected = -1;
                this.Visible = false;

                OnSelectedChanged(this, null);

                return;
            }

            if (ProjectItems.Count() == 0)
                Selected = -1;

            this.Visible = ProjectItems.Count() > 0;

            OnSelectedChanged(this, null);
        }

        public void SetStatus(int ProjectID, int StatusID)
        {
            ProjectsDT.Select("ProjectID = " + ProjectID)[0]["ProjectStatusID"] = StatusID;

            int i = ProjectsDT.Rows.IndexOf(ProjectsDT.Select("ProjectID = " + ProjectID)[0]);

            ProjectItems[i].Status = (InfiniumProjectStates)StatusID;
        }

        private void OnScrollContainerSizeChanged(object sender, EventArgs e)
        {
            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
        }

        private void OnScrollPositionChanged(object sender, int tOffset)
        {
            Offset = tOffset;
            ScrollContainer.Top = -Offset;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (!VerticalScroll.Visible)
                return;

            if (Offset == 0)
                if (iTempScrollWheelOffset > 0)
                    iTempScrollWheelOffset = 0;

            if (e.Delta < 0)
            {
                if (Offset < ScrollContainer.Height - this.Height)
                {
                    if (Offset + VerticalScroll.ScrollWheelOffset + this.Height > ScrollContainer.Height)//last
                    {
                        iTempScrollWheelOffset = ScrollContainer.Height - this.Height - Offset;
                        Offset += iTempScrollWheelOffset;
                    }
                    else
                    {
                        Offset += VerticalScroll.ScrollWheelOffset;
                    }

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
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
                        Offset -= VerticalScroll.ScrollWheelOffset;

                    if (Offset < 0)
                        Offset = 0;

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
                }
        }

        private void OnProjectItemClick(object sender, EventArgs e)
        {
            for (int i = 0; i < ProjectItems.Count(); i++)
            {
                if (sender != ProjectItems[i] && ProjectItems[i] != null)
                    ProjectItems[i].Selected = false;
                else
                {
                    ProjectItems[i].Selected = true;
                    this.Selected = i;
                }
            }

            //((InfiniumProjectsListItem)sender).Selected = true;

            //this.Selected = 
        }

        private void OnVerticalScrollVisibleChanged(object sender, EventArgs e)
        {
            if (VerticalScroll.Visible)
                ScrollContainer.Width = this.Width - VerticalScroll.Width;
            else
                ScrollContainer.Width = this.Width;
        }



        public event EventHandler SelectedChanged;

        public virtual void OnSelectedChanged(object sender, EventArgs e)
        {
            SelectedChanged?.Invoke(sender, e);//Raise the event
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            ScrollContainer.Focus();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            VerticalScroll.Left = this.Width - VerticalScroll.Width;
        }
    }




    public class InfiniumProjectsDescriptionItem : Control
    {
        string sDescriptionText = "";

        int iMarginTextRows = 5;

        SolidBrush brTextFontBrush;

        Color cTextFontColor = Color.FromArgb(30, 30, 30);

        Font fTextFont;

        public InfiniumProjectsDescriptionItem()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);

            brTextFontBrush = new SolidBrush(cTextFontColor);

            fTextFont = new System.Drawing.Font("Segoe UI", 17.0f, FontStyle.Regular, GraphicsUnit.Pixel);

            this.BackColor = Color.White;
        }



        public Color TextFontColor
        {
            get { return cTextFontColor; }
            set { cTextFontColor = value; brTextFontBrush.Color = value; }
        }

        public string DescriptionText
        {
            get { return sDescriptionText; }
            set
            {
                sDescriptionText = value;
                SetInitialSize();
                this.Refresh();
            }
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            DrawText(e.Graphics);
        }


        private int DrawText(Graphics G)
        {
            if (sDescriptionText.Length == 0)
                return 0;

            int TextMaxWidth = this.Width;

            int CurrentY = 0;

            string CurrentRowString = "";

            for (int i = 0; i < sDescriptionText.Length; i++)
            {
                if (sDescriptionText[i] == '\n')
                {
                    G.DrawString(CurrentRowString, fTextFont, brTextFontBrush,
                                 0, (CurrentY * (G.MeasureString("String", fTextFont).Height + iMarginTextRows)) + 4);
                    CurrentRowString = "";
                    CurrentY++;
                    continue;
                }

                if (i == sDescriptionText.Length - 1)
                {
                    G.DrawString(CurrentRowString += sDescriptionText[i], fTextFont, brTextFontBrush,
                                 0, (CurrentY * (G.MeasureString("String", fTextFont).Height + iMarginTextRows)) + 4);
                    CurrentY++;
                }
                else
                {
                    if (G.MeasureString(CurrentRowString, fTextFont).Width > TextMaxWidth)
                    {
                        int LastSpace = GetLastSpace(CurrentRowString);

                        if (LastSpace == 0)
                        {
                            G.DrawString(CurrentRowString, fTextFont, brTextFontBrush,
                                         0, (CurrentY * (G.MeasureString("String", fTextFont).Height + iMarginTextRows)) + 4);
                        }
                        else
                        {
                            G.DrawString(CurrentRowString.Substring(0, LastSpace + 1), fTextFont, brTextFontBrush,
                                         0, (CurrentY * (G.MeasureString("String", fTextFont).Height + iMarginTextRows)) + 4);

                            i -= (CurrentRowString.Length - LastSpace);
                        }


                        CurrentRowString = "";
                        CurrentY++;

                        continue;
                    }
                }

                CurrentRowString += sDescriptionText[i];
            }

            int C = CurrentY * Convert.ToInt32((G.MeasureString("String", fTextFont).Height) + iMarginTextRows);

            return C;
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

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            Parent.Focus();
        }

        private void SetInitialSize()
        {
            if (sDescriptionText.Length == 0)
            {
                this.Height = 0;
                return;
            }

            using (Graphics G = this.CreateGraphics())
            {
                int TextMaxWidth = this.Width;

                int CurrentY = 0;

                string CurrentRowString = "";

                for (int i = 0; i < sDescriptionText.Length; i++)
                {
                    if (sDescriptionText[i] == '\n')
                    {
                        CurrentRowString = "";
                        CurrentY++;
                        continue;
                    }

                    if (i == sDescriptionText.Length - 1)
                    {
                        CurrentY++;
                    }
                    else
                    {
                        if (G.MeasureString(CurrentRowString, fTextFont).Width > TextMaxWidth)
                        {
                            int LastSpace = GetLastSpace(CurrentRowString);

                            if (LastSpace == 0)
                            {

                            }
                            else
                            {
                                i -= (CurrentRowString.Length - LastSpace);
                            }


                            CurrentRowString = "";
                            CurrentY++;

                            continue;
                        }
                    }

                    CurrentRowString += sDescriptionText[i];
                }

                int C = CurrentY * Convert.ToInt32((G.MeasureString("String", fTextFont).Height) + iMarginTextRows);

                this.Height = C;
            }
        }

    }




    public class InfiniumProjectsDescriptionBox : Control
    {
        int Offset = 0;

        int iTempScrollWheelOffset = 0;

        InfiniumProjectsVerticalScrollBar VerticalScroll;
        InfiniumScrollContainer ScrollContainer;

        public InfiniumProjectsDescriptionItem tDescriptionItem;

        public InfiniumProjectsDescriptionBox()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);

            VerticalScroll = new InfiniumProjectsVerticalScrollBar()
            {
                Parent = this,
                Height = this.Height,
                Width = 12
            };
            VerticalScroll.Left = this.Width - VerticalScroll.Width;
            VerticalScroll.Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            VerticalScroll.ScrollPositionChanged += OnScrollPositionChanged;

            ScrollContainer = new InfiniumScrollContainer()
            {
                Parent = this,
                Top = 0,
                Left = 0,
                Width = this.Width - VerticalScroll.Width,
                Height = this.Height,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            ScrollContainer.SizeChanged += OnScrollContainerSizeChanged;

            tDescriptionItem = new InfiniumProjectsDescriptionItem()
            {
                Parent = ScrollContainer,
                Width = ScrollContainer.Width,
                Height = ScrollContainer.Height
            };
            tDescriptionItem.SizeChanged += OnItemSizeChanged;
            tDescriptionItem.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
        }


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public InfiniumProjectsDescriptionItem DescriptionItem
        {
            get { return tDescriptionItem; }
            set { tDescriptionItem = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public InfiniumProjectsVerticalScrollBar VerticalScrollBar
        {
            get { return VerticalScroll; }
            set { VerticalScroll = value; VerticalScroll.Left = this.Width - VerticalScroll.Width; }
        }


        private void OnItemSizeChanged(object sender, EventArgs e)
        {
            ScrollContainer.Height = DescriptionItem.Height;
            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
        }

        private void OnScrollContainerSizeChanged(object sender, EventArgs e)
        {
            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
            VerticalScroll.Left = this.Width - VerticalScroll.Width;

            Offset = 0;
            ScrollContainer.Top = -Offset;
            VerticalScroll.Offset = Offset;
            VerticalScroll.Refresh();
        }

        private void OnScrollPositionChanged(object sender, int tOffset)
        {
            Offset = tOffset;
            ScrollContainer.Top = -Offset;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (!VerticalScroll.Visible)
                return;

            if (Offset == 0)
                if (iTempScrollWheelOffset > 0)
                    iTempScrollWheelOffset = 0;

            if (e.Delta < 0)
            {
                if (Offset < ScrollContainer.Height - this.Height)
                {
                    if (Offset + VerticalScroll.ScrollWheelOffset + this.Height > ScrollContainer.Height)//last
                    {
                        iTempScrollWheelOffset = ScrollContainer.Height - this.Height - Offset;
                        Offset += iTempScrollWheelOffset;
                    }
                    else
                    {
                        Offset += VerticalScroll.ScrollWheelOffset;
                    }

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
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
                        Offset -= VerticalScroll.ScrollWheelOffset;

                    if (Offset < 0)
                        Offset = 0;

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
                }
        }

        private void OnVerticalScrollVisibleChanged(object sender, EventArgs e)
        {
            if (VerticalScroll.Visible)
                ScrollContainer.Width = this.Width - VerticalScroll.Width;
            else
                ScrollContainer.Width = this.Width;
        }

    }




    public class InfiniumProjectNewsContainer : Control
    {
        public int Offset = 0;

        int iTempScrollWheelOffset = 0;

        int iMarginToNextItem = 21;

        public int NewsCount = 0;

        public bool bNeedMoreNews = false;

        public InfiniumVerticalScrollBar sbVerticalScrollBar = new InfiniumVerticalScrollBar();

        public DataTable NewsDT;
        public DataTable DepartmentDT;
        public DataTable UsersDT;
        public DataTable CommentsDT;
        public DataTable AttachsDT;
        public DataTable NewsLikesDT;
        public DataTable CommentsLikesDT;

        public InfiniumProjectNewsItem[] NewsItems;

        public InfiniumScrollContainer ScrollContainer;

        private UserImagesStruct[] UserImages;

        int iCurrentUserID;

        public InfiniumProjectNewsContainer()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);

            sbVerticalScrollBar.Parent = this;
            sbVerticalScrollBar.Height = this.Height;
            sbVerticalScrollBar.Left = this.Width - sbVerticalScrollBar.Width;
            sbVerticalScrollBar.Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            sbVerticalScrollBar.ScrollPositionChanged += OnScrollPositionChanged;

            ScrollContainer = new InfiniumScrollContainer()
            {
                Parent = this,
                Top = 0,
                Left = 0,
                Width = this.Width - sbVerticalScrollBar.Width,
                Height = this.Height,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            ScrollContainer.Paint += OnScrollContainerPaint;
            ScrollContainer.SizeChanged += OnScrollContainerSizeChanged;

            this.BackColor = Color.White;
        }


        [DllImport("user32.dll", SetLastError = true)]
        static extern UInt32 GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, UInt32 dwNewLong);


        public void SetNoClip()
        {
            UInt32 f = 0x02000000;

            SetWindowLong(ScrollContainer.Handle, -16, ((GetWindowLong(ScrollContainer.Handle, -16) & ~f)));
        }

        public void SetClipStandard()
        {
            SetWindowLong(ScrollContainer.Handle, -16, ((GetWindowLong(this.Handle, -16))));
        }


        public int CurrentUserID
        {
            get { return iCurrentUserID; }
            set { iCurrentUserID = value; }
        }

        public DataTable UsersDataTable
        {
            get { return UsersDT; }
            set
            {
                UsersDT = value;

                if (value == null)
                    return;

                int index = 0;

                UserImages = new UserImagesStruct[UsersDT.Rows.Count];

                foreach (DataRow Row in UsersDT.Rows)
                {
                    if (Row["Photo"] != DBNull.Value)
                    {
                        byte[] b = (byte[])Row["Photo"];

                        using (MemoryStream ms = new MemoryStream(b))
                        {
                            Bitmap Bmp = new Bitmap(ms);

                            UserImages[index].UserID = Convert.ToInt32(Row["UserID"]);
                            UserImages[index].Photo = new Bitmap(Bmp);

                            Bmp.Dispose();
                        }
                    }

                    index++;
                }

                GC.Collect();
            }
        }

        public DataTable NewsDataTable
        {
            get { return NewsDT; }
            set
            {
                NewsDT = value;

                //if (value == null)
                //    return;

                //if (NewsDT.Columns["Page"] == null)
                //    NewsDT.Columns.Add(new DataColumn("Page", System.Type.GetType("System.Int32")));

                //int iPage = 0;
                //int iN = 0;

                //for (int i = 0; i < NewsDT.Rows.Count; i++)
                //{
                //    NewsDT.Rows[i]["Page"] = iPage;

                //    iN++;

                //    if (iN >= iMaxNewsItemsOnPage)
                //    {
                //        iN = 0;
                //        iPage++;
                //    }
                //}

                //if (PageSelector != null)
                //{
                //    NewsDT.DefaultView.RowFilter = "Page = " + PageSelector.SelectedPage;
                //    NewsDT.DefaultView.Sort = "NewsID ASC";
                //}
            }
        }

        public void CreateNews()
        {
            NewsCount = NewsDT.Rows.Count;

            if (NewsItems != null)
            {
                for (int i = 0; i < NewsItems.Count(); i++)
                {
                    if (NewsItems[i] != null)
                        NewsItems[i].Dispose();
                }
            }

            NewsItems = new InfiniumProjectNewsItem[NewsDT.DefaultView.Count];

            if (NewsDT.DefaultView.Count == 0)
                return;

            int CurTextPosY = 0;

            ScrollContainer.Height = 0;
            ScrollContainer.Width = this.Width - sbVerticalScrollBar.Width;

            for (int i = 0; i < NewsDT.DefaultView.Count; i++)
            {
                NewsItems[i] = new InfiniumProjectNewsItem(CurrentUserID, ref UserImages, UsersDataTable)
                {
                    Parent = ScrollContainer,
                    Width = ScrollContainer.Width,
                    NewsID = Convert.ToInt32(NewsDT.Rows[i]["NewsID"]),
                    SenderID = Convert.ToInt32(NewsDT.Rows[i]["SenderID"])
                };
                NewsItems[i].SenderImage = UserImages[Array.FindIndex(UserImages, UsIm => UsIm.UserID == NewsItems[i].SenderID)].Photo;
                NewsItems[i].SenderName = UsersDT.Select("UserID = " + NewsDT.Rows[i]["SenderID"])[0]["Name"].ToString();
                NewsItems[i].CommentsLikesRows = CommentsLikesDT.Select("NewsID = " + NewsDT.DefaultView[i]["NewsID"]);
                NewsItems[i].NewsLikesRows = NewsLikesDT.Select("NewsID = " + NewsDT.DefaultView[i]["NewsID"]);
                NewsItems[i].AttachmentsRows = AttachsDT.Select("NewsID = " + NewsDT.DefaultView[i]["NewsID"]);
                NewsItems[i].CommentsRows = CommentsDT.Select("NewsID = " + NewsDT.DefaultView[i]["NewsID"]);
                NewsItems[i].Date = NewsDT.Rows[i]["DateTime"].ToString();
                NewsItems[i].NewsText = NewsDT.Rows[i]["BodyText"].ToString();
                NewsItems[i].Top = CurTextPosY;
                if (NewsDT.Rows[i]["New"] != DBNull.Value)
                    NewsItems[i].IsNew = true;
                if (NewsDT.Rows[i]["NewComments"] != DBNull.Value)
                {
                    NewsItems[i].NewCommentsCount = Convert.ToInt32(NewsDT.Rows[i]["NewComments"]);
                    NewsItems[i].ShowAllComments();
                }
                NewsItems[i].Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                NewsItems[i].AllCommentsLabelClicked += OnAllCommentsClick;
                NewsItems[i].CommentsLabelClicked += OnCommentsClick;
                NewsItems[i].CommentsCancelButtonClicked += OnCommentsCancelButtonClick;
                NewsItems[i].AttachsAllFilesClicked += OnAttachsAllFilesClick;
                NewsItems[i].SendButtonClicked += OnCommentsSendButtonClick;
                NewsItems[i].CommentsRemoveClicked += OnCommentsRemoveButtonClick;
                NewsItems[i].CommentsEditClicked += OnCommentsEditButtonClick;
                NewsItems[i].CommentsCommentClicked += OnCommentsCommentClick;
                NewsItems[i].CommentsTextBoxSizeChanged += OnCommentsTextBoxSizeChanged;
                NewsItems[i].RemoveLabelClicked += OnRemoveClicked;
                NewsItems[i].EditLabelClicked += OnEditClicked;
                NewsItems[i].AttachClicked += OnAttachClicked;
                NewsItems[i].CommentsLikeClicked += OnCommentsLikeClick;
                NewsItems[i].LikeClicked += OnLikeClick;
                NewsItems[i].QuoteLabelClicked += OnNewsQuoteLabelClick;
                NewsItems[i].CommentsQuoteLabelClicked += OnCommentsQuoteLabelClick;

                CurTextPosY += NewsItems[i].Height + iMarginToNextItem;

                if (ScrollContainer.Height < CurTextPosY)
                    if (i != NewsDT.DefaultView.Count - 1)
                        ScrollContainer.Height += NewsItems[i].Height + iMarginToNextItem;
                    else
                        ScrollContainer.Height += NewsItems[i].Height;
            }

            if (ScrollContainer.Height < this.Height)
                ScrollContainer.Height = this.Height;

            VerticalScrollBar.Visible = true;
            VerticalScrollBar.TotalControlHeight = ScrollContainer.Height;
            VerticalScrollBar.Refresh();
        }


        public void Clear()
        {
            if (NewsItems != null)
            {
                for (int i = 0; i < NewsItems.Count(); i++)
                {
                    if (NewsItems[i] != null)
                        NewsItems[i].Dispose();
                }

                NewsItems = new InfiniumProjectNewsItem[0];
            }

            Offset = 0;
            ScrollContainer.Top = -Offset;
            VerticalScrollBar.Offset = Offset;
            VerticalScrollBar.Refresh();

            ScrollContainer.Height = this.Height;
            VerticalScrollBar.TotalControlHeight = ScrollContainer.Height;

            this.Refresh();
        }

        public void ScrollToNews(int index)
        {
            Offset = 0;

            Offset = NewsItems[index].Top - this.Height - iMarginToNextItem;

            Offset += NewsItems[index].iMarginForImageHeight + iMarginToNextItem + 36;
            ScrollContainer.Top = -Offset;
            VerticalScrollBar.Offset = Offset;
            VerticalScrollBar.Refresh();
        }

        public int FindRow(int NewsID)
        {
            for (int i = 0; i < NewsDT.DefaultView.Count; i++)
            {
                if (Convert.ToInt32(NewsDT.DefaultView[i]["NewsID"]) == NewsID)
                    return i;
            }

            return -1;
        }

        public void SetNewsPositions()
        {
            if (NewsItems == null)
                return;

            int CurTextPosY = 0;

            ScrollContainer.Height = 0;

            for (int i = 0; i < NewsItems.Count(); i++)
            {
                NewsItems[i].Top = CurTextPosY;

                CurTextPosY += NewsItems[i].Height + iMarginToNextItem;

                if (ScrollContainer.Height < CurTextPosY)
                    if (i != NewsDT.DefaultView.Count - 1)
                        ScrollContainer.Height += NewsItems[i].Height + iMarginToNextItem;
                    else
                        ScrollContainer.Height += NewsItems[i].Height;

                NewsItems[i].Refresh();
            }

            if (ScrollContainer.Height < this.Height)
                ScrollContainer.Height = this.Height;

            if (Offset + this.Height > ScrollContainer.Height)
            {
                Offset = ScrollContainer.Height - this.Height;
                ScrollContainer.Top = -Offset;
                VerticalScrollBar.Visible = true;
                VerticalScrollBar.Offset = Offset;
                VerticalScrollBar.Refresh();
            }
        }

        //public void SetNewsPositions(InfiniumNewsItem NewsItem)
        //{
        //    int Height = 0;

        //    int index = -1;

        //    for (int i = 0; i < NewsItems.Count(); i++)
        //    {
        //        if (i != NewsDT.DefaultView.Count - 1)
        //            Height += NewsItems[i].Height + iMarginToNextItem;
        //        else
        //            Height += NewsItems[i].Height;

        //        if (NewsItems[i] == NewsItem)
        //            index = i;
        //    }

        //    if (Height > ScrollContainer.Height)//news item getting bigger
        //    {
        //        for (int i = index + 1; i < NewsItems.Count(); i++)
        //        {
        //            NewsItems[i].Top += Height - ScrollContainer.Height;
        //        }

        //        ScrollContainer.Height += Height - ScrollContainer.Height;
        //    }
        //    else//news item getting smaller
        //    {

        //    }
        //}

        public void ScrollToTop()
        {
            Offset = 0;
            ScrollContainer.Top = -Offset;
            VerticalScrollBar.Offset = Offset;
            VerticalScrollBar.Refresh();
        }

        public void ShowAllComments(int NewsID)
        {
            int i = FindRow(NewsID);

            NewsItems[i].AllCommentsVisible = true;
            this.Focus();
        }

        public void ReloadNewsItem(int NewsID, bool bAllComments)
        {
            int i = FindRow(NewsID);

            if (i == -1)//there is no NewsID in this default view (deleted by somebody)
                i = -1;

            int CurTextPosY = NewsItems[i].Top;

            if (NewsItems[i] != null)
                NewsItems[i].Dispose();

            NewsItems[i] = new InfiniumProjectNewsItem(CurrentUserID, ref UserImages, UsersDataTable)
            {
                Parent = ScrollContainer,
                Width = this.Width,
                AllCommentsVisible = bAllComments,
                NewsID = Convert.ToInt32(NewsDT.DefaultView[i]["NewsID"]),
                SenderID = Convert.ToInt32(NewsDT.DefaultView[i]["SenderID"])
            };
            NewsItems[i].SenderImage = UserImages[Array.FindIndex(UserImages, UsIm => UsIm.UserID == NewsItems[i].SenderID)].Photo;
            NewsItems[i].SenderName = UsersDT.Select("UserID = " + NewsDT.DefaultView[i]["SenderID"])[0]["Name"].ToString();
            NewsItems[i].CommentsLikesRows = CommentsLikesDT.Select("NewsID = " + NewsDT.DefaultView[i]["NewsID"]);
            NewsItems[i].NewsLikesRows = NewsLikesDT.Select("NewsID = " + NewsDT.DefaultView[i]["NewsID"]);
            NewsItems[i].AttachmentsRows = AttachsDT.Select("NewsID = " + NewsDT.DefaultView[i]["NewsID"]);
            NewsItems[i].CommentsRows = CommentsDT.Select("NewsID = " + NewsDT.DefaultView[i]["NewsID"]);
            NewsItems[i].Date = NewsDT.DefaultView[i]["DateTime"].ToString();
            NewsItems[i].NewsText = NewsDT.DefaultView[i]["BodyText"].ToString();
            NewsItems[i].Top = CurTextPosY;
            NewsItems[i].AllCommentsLabelClicked += OnAllCommentsClick;
            NewsItems[i].CommentsLabelClicked += OnCommentsClick;
            NewsItems[i].CommentsCancelButtonClicked += OnCommentsCancelButtonClick;
            NewsItems[i].AttachsAllFilesClicked += OnAttachsAllFilesClick;
            NewsItems[i].SendButtonClicked += OnCommentsSendButtonClick;
            NewsItems[i].CommentsRemoveClicked += OnCommentsRemoveButtonClick;
            NewsItems[i].CommentsEditClicked += OnCommentsEditButtonClick;
            NewsItems[i].CommentsCommentClicked += OnCommentsCommentClick;
            NewsItems[i].CommentsTextBoxSizeChanged += OnCommentsTextBoxSizeChanged;
            NewsItems[i].RemoveLabelClicked += OnRemoveClicked;
            NewsItems[i].EditLabelClicked += OnEditClicked;
            NewsItems[i].AttachClicked += OnAttachClicked;
            NewsItems[i].CommentsLikeClicked += OnCommentsLikeClick;
            NewsItems[i].LikeClicked += OnLikeClick;
            NewsItems[i].QuoteLabelClicked += OnNewsQuoteLabelClick;
            NewsItems[i].CommentsQuoteLabelClicked += OnCommentsQuoteLabelClick;

            SetNewsPositions();
        }

        public void ReloadLikes(int NewsID)
        {
            int i = FindRow(NewsID);

            NewsItems[i].NewsLikesRows = NewsLikesDT.Select("NewsID = " + NewsDT.DefaultView[i]["NewsID"]);
            NewsItems[i].CommentsLikesRows = CommentsLikesDT.Select("NewsID = " + NewsDT.DefaultView[i]["NewsID"]);
            NewsItems[i].ReloadLikes();
            NewsItems[i].Refresh();
        }

        public void ScrollToCommentsTextBox(InfiniumProjectNewsItem NewsItem)
        {
            if (Offset + this.Height < NewsItem.Top + NewsItem.Height)
            {
                Offset = NewsItem.Top - this.Height + NewsItem.Height;
                ScrollContainer.Top = -Offset;
                VerticalScrollBar.Offset = Offset;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public InfiniumVerticalScrollBar VerticalScrollBar
        {
            get { return sbVerticalScrollBar; }
        }

        private void OnScrollContainerPaint(object sender, PaintEventArgs e)
        {
            //if (InfiniumCommentsItems == null)
            //    return;


        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //TotalY = ScrollContainer.Height;
            //VerticalScrollBar.TotalControlHeight = ScrollContainer.Height;
            //VerticalScrollBar.Refresh();


        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }

        //public void RefreshThis()
        //{
        //    OnRefreshed(this, null);
        //}

        private void OnScrollContainerSizeChanged(object sender, EventArgs e)
        {
            sbVerticalScrollBar.TotalControlHeight = ScrollContainer.Height;
            sbVerticalScrollBar.InitializeThumb();
            sbVerticalScrollBar.Refresh();
        }

        private void OnAttachsAllFilesClick(object sender)
        {
            SetNewsPositions();
            this.Refresh();
        }

        private void OnAllCommentsClick(object sender)
        {
            SetNewsPositions();
            this.Refresh();
            this.Focus();
        }

        private void OnCommentsCancelButtonClick(object sender)
        {
            SetNewsPositions();
            this.Refresh();
            this.Focus();
            SetNoClip();
        }

        private void OnAttachLabelClicked(object sender, int NewsAttachID)
        {
            OnAttachClicked(sender, NewsAttachID);
        }

        private void OnLikeClick(object sender)
        {
            OnLikeClicked(sender, ((InfiniumProjectNewsItem)sender).NewsID);
        }

        private void OnCommentsLikeClick(object sender, int NewsID, int NewsCommentID)
        {
            OnCommentLikeClicked(sender, NewsID, NewsCommentID);
        }

        private void OnCommentsClick(object sender)
        {
            SetClipStandard();

            for (int i = 0; i < NewsItems.Count(); i++)
            {
                if (sender != NewsItems[i])
                    NewsItems[i].CloseCommentsTextBox();
            }

            SetNewsPositions();
            ScrollToCommentsTextBox((InfiniumProjectNewsItem)sender);
            SetNoClip();
            this.Refresh();
        }

        private void OnEditClicked(object sender)
        {
            OnEditNewsClicked(sender, ((InfiniumProjectNewsItem)sender).NewsID);
        }

        private void OnRemoveClicked(object sender)
        {
            OnRemoveNewsClicked(sender, ((InfiniumProjectNewsItem)sender).NewsID);
        }

        private void OnCommentsQuoteLabelClick(object sender)
        {
            OnCommentsQuoteLabelClicked(sender);
        }

        private void OnNewsQuoteLabelClick(object sender)
        {
            OnNewsQuoteLabelClicked(sender);
        }

        private void OnCommentsTextBoxSizeChanged(object sender, EventArgs e)
        {
            SetNewsPositions();
            ScrollToCommentsTextBox((InfiniumProjectNewsItem)sender);
        }

        private void OnCommentsCommentClick(object sender)
        {
            SetClipStandard();

            int index = FindRow(((InfiniumNewsCommentItem)sender).NewsID);

            for (int i = 0; i < NewsItems.Count(); i++)
            {
                if (index != i)
                    NewsItems[i].CloseCommentsTextBox();
            }

            SetNewsPositions();
            ScrollToCommentsTextBox(NewsItems[index]);
            SetNoClip();
            this.Refresh();
        }

        private void OnScrollPositionChanged(object sender, int tOffset)
        {
            Offset = tOffset;
            ScrollContainer.Top = -Offset;

            //for more news
            if (Offset > ScrollContainer.Height - this.Height - 200 && NewsCount < 80)
            {
                if (!bNeedMoreNews)
                {
                    bNeedMoreNews = true;
                    OnNeedMoreNews(this, null);
                }
            }
            else
            {
                if (bNeedMoreNews)
                {
                    bNeedMoreNews = false;
                    OnNoNeedMoreNews(this, null);
                }
            }
        }

        private void OnCommentsSendButtonClick(object sender, string Text, bool bEdit, int iNewsCommentID, bool bNoNotify)
        {
            SetNoClip();
            OnSendButtonClicked(sender, Text, ((InfiniumProjectNewsItem)sender).NewsID, CurrentUserID, bEdit, iNewsCommentID, bNoNotify);//Security.CurrentUserID
        }

        private void OnCommentsRemoveButtonClick(object sender, int NewsID, int NewsCommentID)
        {
            OnRemoveCommentClicked(sender, NewsID, NewsCommentID);
        }

        private void OnCommentsEditButtonClick(object sender, int NewsID, int NewsCommentID)
        {
            SetNewsPositions();
            OnEditCommentClicked(sender, NewsID, NewsCommentID);
            int i = FindRow(NewsID);
            ScrollToCommentsTextBox(NewsItems[i]);
        }


        public event SendButtonEventHandler CommentSendButtonClicked;
        public event CommentEventHandler RemoveCommentClicked;
        public event CommentEventHandler EditCommentClicked;
        public event CommentEventHandler CommentLikeClicked;
        public event LabelClickEventHandler RemoveNewsClicked;
        public event LabelClickEventHandler EditNewsClicked;
        public event LabelClickEventHandler LikeClicked;
        public event QuoteLabelClikedEventHandler NewsQuoteLabelClicked;
        public event QuoteLabelClikedEventHandler CommentsQuoteLabelClicked;
        public event EventHandler Refreshed;
        public event EventHandler NeedMoreNews;
        public event EventHandler NoNeedMoreNews;
        public event AttachClickedEventHandler AttachClicked;

        public delegate void SendButtonEventHandler(object sender, string Text, int NewsID, int SenderID, bool bEdit, int NewsCommentID, bool bNoNotify);
        public delegate void CommentEventHandler(object sender, int NewsID, int NewsCommentID);
        public delegate void LabelClickEventHandler(object sender, int NewsID);
        public delegate void AttachClickedEventHandler(object sender, int NewsAttachID);
        public delegate void QuoteLabelClikedEventHandler(object sender);


        public virtual void OnNeedMoreNews(object sender, EventArgs e)
        {
            NeedMoreNews?.Invoke(this, e);//Raise the event
        }

        public virtual void OnNoNeedMoreNews(object sender, EventArgs e)
        {
            NoNeedMoreNews?.Invoke(this, e);//Raise the event
        }


        public virtual void OnNewsQuoteLabelClicked(object sender)
        {
            NewsQuoteLabelClicked?.Invoke(this);//Raise the event
        }

        public virtual void OnCommentsQuoteLabelClicked(object sender)
        {
            CommentsQuoteLabelClicked?.Invoke(this);//Raise the event
        }


        public virtual void OnLikeClicked(object sender, int NewsID)
        {
            LikeClicked?.Invoke(this, NewsID);//Raise the event
        }

        public virtual void OnCommentLikeClicked(object sender, int NewsID, int NewsCommentID)
        {
            CommentLikeClicked?.Invoke(this, NewsID, NewsCommentID);//Raise the event
        }


        public virtual void OnAttachClicked(object sender, int NewsAttachID)
        {
            AttachClicked?.Invoke(this, NewsAttachID);//Raise the event
        }

        public virtual void OnRefreshed(object sender, EventArgs e)
        {
            Refreshed?.Invoke(this, e);//Raise the event
        }

        public virtual void OnEditNewsClicked(object sender, int NewsID)
        {
            EditNewsClicked?.Invoke(this, NewsID);//Raise the event
        }

        public virtual void OnRemoveNewsClicked(object sender, int NewsID)
        {
            RemoveNewsClicked?.Invoke(this, NewsID);//Raise the event
        }

        public virtual void OnSendButtonClicked(object sender, string Text, int NewsID, int SenderID, bool bEdit, int NewsCommentID, bool bNoNotify)
        {
            CommentSendButtonClicked?.Invoke(this, Text, NewsID, SenderID, bEdit, NewsCommentID, bNoNotify);//Raise the event
        }

        public virtual void OnRemoveCommentClicked(object sender, int NewsID, int NewsCommentID)
        {
            RemoveCommentClicked?.Invoke(this, NewsID, NewsCommentID);//Raise the event
        }

        public virtual void OnEditCommentClicked(object sender, int NewsID, int NewsCommentID)
        {
            EditCommentClicked?.Invoke(this, NewsID, NewsCommentID);//Raise the event
        }



        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            ScrollContainer.Focus();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            if (VerticalScrollBar != null)
            {
                //VerticalScrollBar.Width = 14;
                VerticalScrollBar.Left = this.Width - VerticalScrollBar.Width;
                VerticalScrollBar.TotalControlHeight = ScrollContainer.Height;
                VerticalScrollBar.Height = this.Height;
                VerticalScrollBar.Refresh();
            }

            SetNewsPositions();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (!VerticalScrollBar.Visible)
                return;

            if (Offset == 0)
                if (iTempScrollWheelOffset > 0)
                    iTempScrollWheelOffset = 0;

            if (Offset > ScrollContainer.Height - this.Height - 200 && NewsCount < 80)
            {
                if (!bNeedMoreNews)
                {
                    bNeedMoreNews = true;
                    OnNeedMoreNews(this, null);
                }
            }
            else
            {
                if (bNeedMoreNews)
                {
                    bNeedMoreNews = false;
                    OnNoNeedMoreNews(this, null);
                }
            }

            if (e.Delta < 0)
            {
                if (Offset < ScrollContainer.Height - this.Height)
                {
                    if (Offset + VerticalScrollBar.ScrollWheelOffset + this.Height > ScrollContainer.Height)//last
                    {
                        iTempScrollWheelOffset = ScrollContainer.Height - this.Height - Offset;
                        Offset += iTempScrollWheelOffset;
                    }
                    else
                    {
                        Offset += VerticalScrollBar.ScrollWheelOffset;
                    }

                    ScrollContainer.Top = -Offset;
                    VerticalScrollBar.Offset = Offset;
                    VerticalScrollBar.Refresh();
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
                        Offset -= VerticalScrollBar.ScrollWheelOffset;

                    if (Offset < 0)
                        Offset = 0;

                    //OnNoNeedMoreNews(this, null);

                    ScrollContainer.Top = -Offset;
                    VerticalScrollBar.Offset = Offset;
                    VerticalScrollBar.Refresh();
                }

            if (Offset > ScrollContainer.Height - this.Height - 200 && NewsCount < 80)
            {
                if (!bNeedMoreNews)
                {
                    bNeedMoreNews = true;
                    OnNeedMoreNews(this, null);
                }
            }
            else
            {
                if (bNeedMoreNews)
                {
                    bNeedMoreNews = false;
                    OnNoNeedMoreNews(this, null);
                }
            }
        }
    }




    public class InfiniumProjectNewsItem : Control
    {
        Bitmap imSenderImage;

        Color cHeaderFontColor = Color.FromArgb(18, 164, 217);
        Color cSenderFontColor = Color.FromArgb(18, 164, 217);
        Color cTextFontColor = Color.FromArgb(30, 30, 30);
        Color cDarkSplitterColor = Color.FromArgb(236, 236, 236);
        Color cLightSplitterColor = Color.FromArgb(249, 249, 249);
        Color cNewBackColor = Color.FromArgb(255, 250, 228);

        Font fHeaderFont;
        Font fSenderFont;
        Font fTextFont;

        Pen pImageBorderPen;
        Pen pDarkSplitterPen;
        Pen pLightSplitterPen;

        Rectangle ImageRect;

        UserImagesStruct[] UsersImages;

        SolidBrush brHeaderFontBrush;
        SolidBrush brSenderFontBrush;
        SolidBrush brTextFontBrush;
        SolidBrush brBackBrush;

        int iNewsID = -1;
        string sSenderName = "";
        int iSenderID = -1;
        string sDate = "";
        string sHeaderText = "";
        string sNewsText = "";

        int iLikesCount = 0;

        int iNewCommentsCount = 0;

        bool bNew = false;

        bool bAllCommentsVisible = false;
        int iCommentsHeight = 0;
        int iMaxCommentsInNews = 2;

        int iPrevCommentsTextBoxHeight = 0;

        public int iMarginForImageWidth = 100;
        public int iMarginForImageHeight = 92;
        int iMarginForText = 8;
        int iCurTextPosY = 0;
        int iMarginToNextComment = 8;
        int iMarginTextRows = 5;

        int iFirstWidth = 0;
        int iFirstHeight = 0;

        InfiniumNewsCommentItem[] CommentsItems;

        DataTable UsersDT = null;
        public DataRow[] CommentsRows = null;
        public DataRow[] AttachmentsRows = null;
        public DataRow[] NewsLikesRows = null;
        public DataRow[] CommentsLikesRows = null;

        InfiniumNewsControlButtons ControlButtons;
        public InfiniumProjectNewsCommentsRichTextBox CommentsTextBox;
        InfiniumNewsAttachmentsList AttachmentsList;

        int CurrentUserID = -1;

        //CONSTRUCTOR
        public InfiniumProjectNewsItem(int iCurrentUserID, ref UserImagesStruct[] tUsersImages, DataTable tUsersDT)
        {
            CurrentUserID = iCurrentUserID;

            //SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            UsersImages = tUsersImages;
            UsersDT = tUsersDT;

            fHeaderFont = new System.Drawing.Font("Segoe UI Light", 18.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            fSenderFont = new System.Drawing.Font("Segoe UI", 16.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            fTextFont = new System.Drawing.Font("Segoe UI", 17.0f, FontStyle.Regular, GraphicsUnit.Pixel);

            brHeaderFontBrush = new SolidBrush(cHeaderFontColor);
            brSenderFontBrush = new SolidBrush(cSenderFontColor);
            brTextFontBrush = new SolidBrush(cTextFontColor);
            brBackBrush = new SolidBrush(Color.White);

            pImageBorderPen = new Pen(new SolidBrush(Color.LightGray));
            pDarkSplitterPen = new Pen(new SolidBrush(cDarkSplitterColor));
            pLightSplitterPen = new Pen(new SolidBrush(cLightSplitterColor));

            ImageRect = new Rectangle(0, 0, 0, 0);

            this.BackColor = Color.White;
        }


        public Bitmap SenderImage
        {
            get { return imSenderImage; }
            set { imSenderImage = value; }
        }


        public Color HeaderFontColor
        {
            get { return cHeaderFontColor; }
            set { cHeaderFontColor = value; brHeaderFontBrush.Color = value; }
        }

        public Color SenderFontColor
        {
            get { return cSenderFontColor; }
            set { cSenderFontColor = value; brSenderFontBrush.Color = value; }
        }

        public Color TextFontColor
        {
            get { return cTextFontColor; }
            set { cTextFontColor = value; brTextFontBrush.Color = value; }
        }

        public Color SplitterDarkColor
        {
            get { return cDarkSplitterColor; }
            set { cDarkSplitterColor = value; pDarkSplitterPen.Color = value; }
        }

        public Color SplitterLightColor
        {
            get { return cLightSplitterColor; }
            set { cLightSplitterColor = value; pLightSplitterPen.Color = value; }
        }

        public Font HeaderFont
        {
            get { return fHeaderFont; }
            set { fHeaderFont = value; }
        }

        public Font TextFont
        {
            get { return fTextFont; }
            set { fTextFont = value; }
        }

        public Font SenderFont
        {
            get { return fSenderFont; }
            set { fSenderFont = value; }
        }

        public bool AllCommentsVisible
        {
            get { return bAllCommentsVisible; }
            set { bAllCommentsVisible = value; this.Refresh(); }
        }

        public int LikesCount
        {
            get { return iLikesCount; }
            set { iLikesCount = value; }
        }

        public int NewsID
        {
            get { return iNewsID; }
            set { iNewsID = value; }
        }

        public int NewCommentsCount
        {
            get { return iNewCommentsCount; }
            set
            {
                iNewCommentsCount = value;

                if (value > 0)
                    brBackBrush.Color = cNewBackColor;
                else
                    brBackBrush.Color = Color.White;
            }
        }

        public bool IsNew
        {
            get { return bNew; }
            set
            {
                bNew = value;

                if (value == true)
                    brBackBrush.Color = cNewBackColor;
                else
                    brBackBrush.Color = Color.White;
            }
        }

        public string HeaderText
        {
            get { return sHeaderText; }
            set { sHeaderText = value; }
        }

        public string SenderName
        {
            get { return sSenderName; }
            set { sSenderName = value; }
        }

        public string NewsText
        {
            get { return sNewsText; }
            set
            {
                sNewsText = value;
                this.Height += GetInitialHeight();
                iFirstWidth = this.Width;
                iFirstHeight = this.Height;
                Initialize();
            }
        }

        public string Date
        {
            get { return sDate; }
            set { sDate = value; }
        }

        public int SenderID
        {
            get { return iSenderID; }
            set { iSenderID = value; }
        }


        private void Initialize()
        {
            if (AttachmentsList != null)
                AttachmentsList.Dispose();

            if (AttachmentsRows.Count() > 0)
            {
                AttachmentsList = new InfiniumNewsAttachmentsList(AttachmentsRows)
                {
                    Top = this.Height + 10,
                    Parent = this,
                    Width = this.Width - iMarginForImageWidth - iMarginForText - 2 - 2,
                    Left = iMarginForImageWidth + iMarginForText + 2,
                    Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
                };
                AttachmentsList.AllFilesClicked += OnAttachsAllFilesLabelClick;
                AttachmentsList.FileLabelClicked += OnAttachLabelClicked;

                this.Height += AttachmentsList.Height;

                this.Height += 10;
            }

            if (ControlButtons != null)
                ControlButtons.Dispose();

            //if (sNewsText.Contains("распространяется нашим"))
            //    this.Height = this.Height;

            ControlButtons = new InfiniumNewsControlButtons()
            {
                Parent = this,
                Left = iMarginForImageWidth + iMarginForText + 2,
                Top = this.Height + 6,
                CurrentUserID = CurrentUserID,
                LikesRows = NewsLikesRows
            };
            ControlButtons.CommentsLabelClicked += OnCommentsClick;
            ControlButtons.RemoveLabelClicked += OnRemoveClick;
            ControlButtons.EditLabelClicked += OnEditClick;
            ControlButtons.LikeClicked += OnLikeClick;
            ControlButtons.CommentsLabelVisible = true;
            ControlButtons.QuoteLabelVisible = true;
            ControlButtons.QuoteLabelClicked += OnQuoteLabelClick;
            ControlButtons.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            if (SenderID == CurrentUserID)
            {
                ControlButtons.EditLabelVisible = true;
                ControlButtons.RemoveLabelVisible = true;
            }

            this.Height += 36;//for splitter

            if (CommentsTextBox != null)
                CommentsTextBox.Dispose();

            CommentsTextBox = new InfiniumProjectNewsCommentsRichTextBox()
            {
                Visible = false,
                Parent = this,
                Width = this.Width - iMarginForImageWidth - iMarginForText - 20,
                Left = iMarginForImageWidth + iMarginForText,
                Height = 150,
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            CommentsTextBox.CancelButtonClicked += OnCommentsCancelButtonClick;
            CommentsTextBox.SendButtonClicked += OnCommentsSendButtonClick;
            CommentsTextBox.RichTextBoxSizeChanged += OnRichTextBoxSizeChanged;
            iPrevCommentsTextBoxHeight = CommentsTextBox.Height;

            CreateComments();
        }

        public void CreateComments()
        {
            if (CommentsRows.Count() == 0)
            {
                return;
            }

            int Count = 0;

            if (!bAllCommentsVisible)
            {
                if (CommentsRows.Count() > iMaxCommentsInNews)
                    Count = iMaxCommentsInNews;
                else
                    Count = CommentsRows.Count();
            }
            else
                Count = CommentsRows.Count();


            if (CommentsItems != null)//clear
            {
                for (int i = 0; i < CommentsItems.Count(); i++)
                {
                    CommentsItems[i].Dispose();
                }

                CommentsItems = null;

                GC.Collect();
            }

            CommentsItems = new InfiniumNewsCommentItem[Count];

            int iTempHeight = this.Height;

            this.Height += 6;//margin between newstext and first comment            

            int iCurCommentsPosY = this.Height;

            for (int i = 0; i < Count; i++)
            {
                using (DataTable DT = new DataTable())
                {
                    DT.Columns.Add(new DataColumn("NewsCommentsLikeID", System.Type.GetType("System.Int32")));
                    DT.Columns.Add(new DataColumn("NewsID", System.Type.GetType("System.Int32")));
                    DT.Columns.Add(new DataColumn("NewsCommentID", System.Type.GetType("System.Int32")));
                    DT.Columns.Add(new DataColumn("UserID", System.Type.GetType("System.Int32")));
                    DT.Columns.Add(new DataColumn("ProjectID", System.Type.GetType("System.Int32")));

                    CommentsLikesRows.CopyToDataTable(DT, LoadOption.OverwriteChanges);

                    CommentsItems[i] = new InfiniumNewsCommentItem(CurrentUserID)
                    {
                        Parent = this,
                        Width = this.Width - iMarginForImageWidth - 14,
                        NewsID = Convert.ToInt32(CommentsRows[i]["NewsID"]),
                        NewsCommentID = Convert.ToInt32(CommentsRows[i]["NewsCommentID"]),
                        SenderID = Convert.ToInt32(CommentsRows[i]["UserID"])
                    };
                    CommentsItems[i].SenderImage = UsersImages[Array.FindIndex(UsersImages, UI => UI.UserID == CommentsItems[i].SenderID)].Photo;
                    CommentsItems[i].SenderName = UsersDT.Select("UserID = " + CommentsRows[i]["UserID"])[0]["Name"].ToString();
                    CommentsItems[i].CommentsLikesRows = DT.Select("NewsCommentID = " + CommentsRows[i]["NewsCommentID"]);
                    CommentsItems[i].Date = CommentsRows[i]["DateTime"].ToString();
                    CommentsItems[i].CommentText = CommentsRows[i]["NewsComment"].ToString();
                    CommentsItems[i].Top = iCurCommentsPosY;
                    CommentsItems[i].Left = iMarginForImageWidth + iMarginForText;
                    CommentsItems[i].Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                    CommentsItems[i].AllCommentsLabelClicked += OnAllCommentsClick;
                    CommentsItems[i].RemoveLabelClicked += OnCommentsRemoveButtonClick;
                    CommentsItems[i].EditLabelClicked += OnCommentsEditButtonClick;
                    CommentsItems[i].CommentLabelClicked += OnCommentsCommentButtonClick;
                    CommentsItems[i].LikeClicked += OnCommentsLikeClick;
                    CommentsItems[i].QuoteLabelClicked += OnCommentsQuoteLabelClick;
                }

                iCurCommentsPosY += CommentsItems[i].Height + iMarginToNextComment;

                if (this.Height < iCurCommentsPosY)
                    if (i != Count - 1)
                        this.Height += CommentsItems[i].Height + iMarginToNextComment;
                    else
                        this.Height += CommentsItems[i].Height;

                if (i == Count - 1 && bAllCommentsVisible == false && Count != CommentsRows.Count())
                {
                    CommentsItems[i].iAllCommentsCount = CommentsRows.Count() - iMaxCommentsInNews;
                    CommentsItems[i].AllCommentsVisible = true;
                }

                if (i == Count - 1)
                    CommentsItems[i].CommentsVisible = true;

                CommentsItems[i].Refresh();
            }

            this.Height += 26;//for splitter

            iCommentsHeight = this.Height - iTempHeight;
        }

        public void ShowAllComments()
        {
            this.Height -= iCommentsHeight;
            bAllCommentsVisible = true;
            CreateComments();
            OnAllCommentsLabelClick();
        }

        public void ReloadLikes()
        {
            ControlButtons.LikesRows = NewsLikesRows;


            if (CommentsRows.Count() == 0)
            {
                return;
            }

            int Count = 0;

            if (!bAllCommentsVisible)
            {
                if (CommentsRows.Count() > iMaxCommentsInNews)
                    Count = iMaxCommentsInNews;
                else
                    Count = CommentsRows.Count();
            }
            else
                Count = CommentsRows.Count();

            for (int i = 0; i < Count; i++)
            {
                using (DataTable DT = new DataTable())
                {
                    DT.Columns.Add(new DataColumn("NewsCommentsLikeID", System.Type.GetType("System.Int32")));
                    DT.Columns.Add(new DataColumn("NewsID", System.Type.GetType("System.Int32")));
                    DT.Columns.Add(new DataColumn("NewsCommentID", System.Type.GetType("System.Int32")));
                    DT.Columns.Add(new DataColumn("UserID", System.Type.GetType("System.Int32")));
                    DT.Columns.Add(new DataColumn("ProjectID", System.Type.GetType("System.Int32")));

                    CommentsLikesRows.CopyToDataTable(DT, LoadOption.OverwriteChanges);

                    CommentsItems[i].CommentsLikesRows = DT.Select("NewsCommentID = " + CommentsRows[i]["NewsCommentID"]);
                    CommentsItems[i].ReloadLikes();
                }
            }
        }

        private void SetPositions(int YOffset)
        {
            if (ControlButtons != null)
                ControlButtons.Top += YOffset;

            if (CommentsTextBox != null)
                CommentsTextBox.Top += YOffset;

            if (CommentsItems != null)
            {
                for (int i = 0; i < CommentsItems.Count(); i++)
                {
                    if (CommentsItems[i] == null)
                        return;

                    CommentsItems[i].Top += YOffset;
                }
            }
        }

        private void OpenCommentsTextBox(string CommentsText, int iNewsCommentID)
        {
            this.Height += CommentsTextBox.Height + 5;
            CommentsTextBox.Clear();
            CommentsTextBox.Top = this.Height - CommentsTextBox.Height - 5;
            CommentsTextBox.Visible = true;

            if (CommentsText.Length > 0)
            {
                CommentsTextBox.CommentsText = CommentsText;
                CommentsTextBox.Edit = true;
                CommentsTextBox.NewsCommentID = iNewsCommentID;
            }
            else
                CommentsTextBox.Edit = false;

            this.Refresh();
            CommentsTextBox.Refresh();
        }

        public void CloseCommentsTextBox()
        {
            if (CommentsTextBox.Visible)
            {
                this.Height -= CommentsTextBox.Height + 5;
                CommentsTextBox.Visible = false;
            }
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            iCurTextPosY = 0;

            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            e.Graphics.FillRectangle(brBackBrush, ClientRectangle);

            DrawImage(e.Graphics);
            DrawHeader(e.Graphics, ref iCurTextPosY);
            iCurTextPosY += 3;
            DrawSenderAndDate(e.Graphics, ref iCurTextPosY);
            DrawNewsText(e.Graphics, ref iCurTextPosY);

            if (iCurTextPosY < iMarginForImageHeight + 9)
                DrawTextSplitter(e.Graphics, iMarginForImageHeight + 13);
            else
                DrawTextSplitter(e.Graphics, iCurTextPosY);

            if (AttachmentsList != null)
                DrawAttachsSplitter(e.Graphics, AttachmentsList.Top + AttachmentsList.Height + 1);

            DrawMainSplitter(e.Graphics, this.Height - 1);


        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            //base.OnPaintBackground(pevent);
        }

        private void DrawImage(Graphics G)
        {
            if (SenderImage == null)
                return;

            SenderImage.SetResolution(G.DpiX, G.DpiY);

            G.DrawImage(SenderImage, 3, 8, iMarginForImageWidth, iMarginForImageHeight);

            ImageRect.X = 3;
            ImageRect.Y = 8;
            ImageRect.Width = iMarginForImageWidth;
            ImageRect.Height = iMarginForImageHeight;

            G.DrawRectangle(pImageBorderPen, ImageRect);
        }

        private int DrawSenderAndDate(Graphics G, ref int CurTextPosY)
        {
            if (SenderName.Length == 0)
                return 0;

            int C = Convert.ToInt32(G.MeasureString(SenderName, fSenderFont).Height);

            G.DrawString(SenderName + ", " + Date, fSenderFont, brSenderFontBrush, iMarginForImageWidth + iMarginForText, CurTextPosY);

            CurTextPosY += C;

            return C;
        }

        private int DrawHeader(Graphics G, ref int CurTextPosY)
        {
            if (sHeaderText.Length == 0)
                return 0;

            int TextMaxWidth = this.Width - iMarginForImageWidth;

            int CurrentY = 0;

            string CurrentRowString = "";

            for (int i = 0; i < sHeaderText.Length; i++)
            {
                if (sHeaderText[i] == '\n')
                {
                    G.DrawString(CurrentRowString, fHeaderFont, brHeaderFontBrush,
                                 iMarginForImageWidth + iMarginForText, CurTextPosY +
                                 (CurrentY * (G.MeasureString("String", fHeaderFont).Height + iMarginTextRows)) + 4);
                    CurrentRowString = "";
                    CurrentY++;
                    continue;
                }

                if (i == sHeaderText.Length - 1)
                {
                    G.DrawString(CurrentRowString += sHeaderText[i], fHeaderFont, brHeaderFontBrush,
                                 iMarginForImageWidth + iMarginForText, CurTextPosY +
                                 (CurrentY * (G.MeasureString("String", fHeaderFont).Height + iMarginTextRows)) + 4);
                    CurrentY++;
                }
                else
                {

                    if (G.MeasureString(CurrentRowString, fHeaderFont).Width > TextMaxWidth)
                    {
                        int LastSpace = GetLastSpace(CurrentRowString);

                        if (LastSpace == 0)
                        {
                            G.DrawString(CurrentRowString, fHeaderFont, brHeaderFontBrush,
                                         iMarginForImageWidth + iMarginForText, CurTextPosY +
                                        (CurrentY * (G.MeasureString("String", fHeaderFont).Height + iMarginTextRows)) + 4);
                        }
                        else
                        {
                            G.DrawString(CurrentRowString.Substring(0, LastSpace + 1), fHeaderFont, brHeaderFontBrush,
                                         iMarginForImageWidth + iMarginForText, CurTextPosY +
                                         (CurrentY * (G.MeasureString("String", fHeaderFont).Height + iMarginTextRows)) + 4);

                            //if (CurrentRowString[CurrentRowString.Length - (CurrentRowString.Length - LastSpace)] == ' ')
                            //    i -= (CurrentRowString.Length - LastSpace - 1);
                            //else
                            i -= (CurrentRowString.Length - LastSpace);
                        }


                        CurrentRowString = "";
                        CurrentY++;
                        continue;
                    }
                }

                CurrentRowString += sHeaderText[i];
            }

            int C = CurrentY * Convert.ToInt32((G.MeasureString("String", fHeaderFont).Height) + iMarginTextRows);

            CurTextPosY += C;

            return C;
        }

        private int DrawNewsText(Graphics G, ref int CurTextPosY)
        {
            if (sNewsText.Length == 0)
                return 0;

            int TextMaxWidth = this.Width - iMarginForImageWidth;

            int CurrentY = 0;

            string CurrentRowString = "";

            for (int i = 0; i < sNewsText.Length; i++)
            {
                if (sNewsText[i] == '\n')
                {
                    G.DrawString(CurrentRowString, fTextFont, brTextFontBrush,
                                 iMarginForImageWidth + iMarginForText, CurTextPosY +
                                 (CurrentY * (G.MeasureString("String", fTextFont).Height + iMarginTextRows)) + 4);
                    CurrentRowString = "";
                    CurrentY++;
                    continue;
                }

                if (i == sNewsText.Length - 1)
                {
                    G.DrawString(CurrentRowString += sNewsText[i], fTextFont, brTextFontBrush,
                                 iMarginForImageWidth + iMarginForText, CurTextPosY +
                                 (CurrentY * (G.MeasureString("String", fTextFont).Height + iMarginTextRows)) + 4);
                    CurrentY++;
                }
                else
                {
                    if (G.MeasureString(CurrentRowString, fTextFont).Width > TextMaxWidth)
                    {
                        int LastSpace = GetLastSpace(CurrentRowString);

                        if (LastSpace == 0)
                        {
                            G.DrawString(CurrentRowString, fTextFont, brTextFontBrush,
                                         iMarginForImageWidth + iMarginForText, CurTextPosY +
                                        (CurrentY * (G.MeasureString("String", fTextFont).Height + iMarginTextRows)) + 4);
                        }
                        else
                        {
                            G.DrawString(CurrentRowString.Substring(0, LastSpace + 1), fTextFont, brTextFontBrush,
                                         iMarginForImageWidth + iMarginForText, CurTextPosY +
                                         (CurrentY * (G.MeasureString("String", fTextFont).Height + iMarginTextRows)) + 4);

                            //if (CurrentRowString[CurrentRowString.Length - (CurrentRowString.Length - LastSpace)] == ' ')
                            //    i -= (CurrentRowString.Length - LastSpace - 1);
                            //else
                            i -= (CurrentRowString.Length - LastSpace);
                        }


                        CurrentRowString = "";
                        CurrentY++;

                        //CurrentRowString += sNewsText[i];
                        continue;
                    }
                }

                CurrentRowString += sNewsText[i];
            }

            int C = CurrentY * Convert.ToInt32((G.MeasureString("String", fTextFont).Height) + iMarginTextRows);

            CurTextPosY += C;

            return C;
        }

        private void DrawMainSplitter(Graphics G, int Y)
        {
            G.DrawLine(pDarkSplitterPen, 0, Y, this.Width, Y);
        }

        private void DrawTextSplitter(Graphics G, int Y)
        {
            G.DrawLine(pLightSplitterPen, iMarginForImageWidth + iMarginForText + 2, Y, this.Width, Y);
        }

        private void DrawAttachsSplitter(Graphics G, int Y)
        {
            G.DrawLine(pLightSplitterPen, iMarginForImageWidth + iMarginForText + 2, Y, this.Width, Y);
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

        private int GetInitialHeaderHeight()
        {
            using (Graphics G = this.CreateGraphics())
            {
                int TextMaxWidth = this.Width - iMarginForImageWidth;

                int CurrentY = 0;

                string CurrentRowString = "";

                for (int i = 0; i < sHeaderText.Length; i++)
                {
                    if (sHeaderText[i] == '\n')
                    {
                        CurrentRowString = "";
                        CurrentY++;
                        continue;
                    }

                    if (i == sHeaderText.Length - 1)
                    {
                        CurrentY++;
                    }
                    else
                    {

                        if (G.MeasureString(CurrentRowString, fHeaderFont).Width > TextMaxWidth)
                        {
                            int LastSpace = GetLastSpace(CurrentRowString);

                            if (LastSpace == 0)
                            {

                            }
                            else
                            {

                                i -= (CurrentRowString.Length - LastSpace - 1);
                            }


                            CurrentRowString = "";
                            CurrentY++;
                            continue;
                        }
                    }

                    CurrentRowString += sHeaderText[i];
                }

                int C = CurrentY * Convert.ToInt32((G.MeasureString("String", fHeaderFont).Height) + iMarginTextRows);

                return C;
            }
        }


        private int GetInitialHeight()
        {
            using (Graphics G = this.CreateGraphics())
            {
                int iCaptionHeight = GetInitialHeaderHeight();
                int iSenderHeight = Convert.ToInt32(G.MeasureString(SenderName + sDate, fSenderFont).Height);

                int TextMaxWidth = this.Width - iMarginForImageWidth;

                int CurrentY = 0;

                string CurrentRowString = "";

                for (int i = 0; i < sNewsText.Length; i++)
                {
                    if (sNewsText[i] == '\n')
                    {
                        CurrentRowString = "";
                        CurrentY++;
                        continue;
                    }

                    if (i == sNewsText.Length - 1)
                    {
                        CurrentY++;

                    }
                    else
                    {

                        if (G.MeasureString(CurrentRowString, fTextFont).Width > TextMaxWidth)
                        {
                            int LastSpace = GetLastSpace(CurrentRowString);

                            if (LastSpace == 0)
                            {

                            }
                            else
                            {
                                i -= (CurrentRowString.Length - LastSpace);
                            }

                            CurrentRowString = "";
                            CurrentY++;
                            continue;
                        }
                    }

                    CurrentRowString += sNewsText[i];
                }

                int C = CurrentY * Convert.ToInt32((G.MeasureString("String", fTextFont).Height) + iMarginTextRows) + iSenderHeight + iCaptionHeight;

                if (C < iMarginForImageHeight + 9)
                    C = iMarginForImageHeight + 9;

                return C;
            }
        }

        private void OnAttachLabelClicked(object sender, int NewsAttachID)
        {
            OnAttachClick(NewsAttachID);
        }

        private void OnEditClick(object sender)
        {
            OnEditLabelClick();
        }

        private void OnQuoteLabelClick(object sender)
        {
            Clipboard.SetText(NewsText, TextDataFormat.UnicodeText);
            OnQuoteLabelClicked();
        }

        private void OnCommentsQuoteLabelClick(object sender)
        {
            OnCommentsQuoteLabelClicked();
        }

        private void OnLikeClick(object sender)
        {
            OnLikeClicked();
        }

        private void OnCommentsLikeClick(object sender)
        {
            OnCommentsLikeClicked(sender, ((InfiniumNewsCommentItem)sender).NewsID, ((InfiniumNewsCommentItem)sender).NewsCommentID);
        }

        private void OnCommentsClick(object sender)
        {
            if (CommentsTextBox.Visible)
                return;

            if (CommentsRows.Count() > iMaxCommentsInNews && bAllCommentsVisible == false)
            {
                OnAllCommentsClick(ControlButtons);
            }

            OpenCommentsTextBox("", -1);
            OnCommentsLabelClick();
        }

        private void OnRemoveClick(object sender)
        {
            OnRemoveLabelClicked(this);
        }

        private void OnAllCommentsClick(object sender)
        {
            //CloseCommentsTextBox();
            //this.Height = 0;
            //this.Height += GetInitialHeight();
            this.Height -= iCommentsHeight;
            bAllCommentsVisible = true;
            //Initialize();
            CreateComments();
            OnAllCommentsLabelClick();
        }

        private void OnCommentsCancelButtonClick(object sender, EventArgs e)
        {
            CloseCommentsTextBox();
            OnCommentsCancelButtonClick();
        }

        private void OnAttachsAllFilesLabelClick(object sender, EventArgs e)
        {
            this.Height -= AttachmentsList.StandardHeight;
            this.Height += AttachmentsList.Height;
            SetPositions(AttachmentsList.Height - AttachmentsList.StandardHeight);
            OnAttachsAllFilesClick();
        }

        private void OnCommentsSendButtonClick(object sender, string Text, bool bEdit, bool bNoNotify)
        {
            OnSendButtonClicked(sender, Text, bEdit, ((InfiniumProjectNewsCommentsRichTextBox)sender).NewsCommentID, bNoNotify);
        }


        private void OnCommentsCommentButtonClick(object sender)
        {
            if (CommentsTextBox.Visible)
                return;

            if (CommentsRows.Count() > iMaxCommentsInNews && bAllCommentsVisible == false)
            {
                OnAllCommentsClick(ControlButtons);
            }

            OpenCommentsTextBox("", -1);
            OnCommentsCommentClicked(sender);
        }

        private void OnCommentsRemoveButtonClick(object sender)
        {
            OnCommentsRemoveClicked(sender, ((InfiniumNewsCommentItem)sender).NewsID, ((InfiniumNewsCommentItem)sender).NewsCommentID);
        }

        private void OnCommentsEditButtonClick(object sender)
        {
            if (CommentsTextBox.Visible)
                return;

            OpenCommentsTextBox(((InfiniumNewsCommentItem)sender).CommentText, ((InfiniumNewsCommentItem)sender).NewsCommentID);
            OnCommentsEditClicked(sender, ((InfiniumNewsCommentItem)sender).NewsID, ((InfiniumNewsCommentItem)sender).NewsCommentID);
        }

        private void OnRichTextBoxSizeChanged(object sender, EventArgs e)
        {
            if (CommentsTextBox.Height != iPrevCommentsTextBoxHeight)
            {
                if (CommentsTextBox.Height < iPrevCommentsTextBoxHeight)
                    this.Height -= iPrevCommentsTextBoxHeight - CommentsTextBox.Height;
                else
                    this.Height += CommentsTextBox.Height - iPrevCommentsTextBoxHeight;

                iPrevCommentsTextBoxHeight = CommentsTextBox.Height;
            }

            OnCommentsTextBoxSizeChanged(e);
        }


        public event LabelClickedEventHandler EditLabelClicked;
        public event LabelClickedEventHandler AllCommentsLabelClicked;
        public event LabelClickedEventHandler CommentsLabelClicked;
        public event LabelClickedEventHandler RemoveLabelClicked;
        public event LabelClickedEventHandler CommentsCancelButtonClicked;
        public event LabelClickedEventHandler AttachsAllFilesClicked;
        public event LabelClickedEventHandler LikeClicked;
        public event LabelClickedEventHandler QuoteLabelClicked;
        public event LabelClickedEventHandler CommentsQuoteLabelClicked;
        public event CommentsEventHandler CommentsLikeClicked;
        public event SendButtonEventHandler SendButtonClicked;
        public event CommentsEventHandler CommentsRemoveClicked;
        public event CommentsEventHandler CommentsEditClicked;
        public event LabelClickedEventHandler CommentsCommentClicked;
        public event EventHandler CommentsTextBoxSizeChanged;
        public event AttachClickedEventHandler AttachClicked;

        public delegate void LabelClickedEventHandler(object sender);
        public delegate void SendButtonEventHandler(object sender, string Text, bool bEdit, int NewsCommentID, bool bNoNotify);
        public delegate void CommentsEventHandler(object sender, int NewsID, int NewsCommentID);
        public delegate void AttachClickedEventHandler(object sender, int NewsAttachID);


        public virtual void OnAttachClick(int NewsAttachID)
        {
            AttachClicked?.Invoke(this, NewsAttachID);//Raise the event
        }


        public virtual void OnQuoteLabelClicked()
        {
            QuoteLabelClicked?.Invoke(this);//Raise the event
        }

        public virtual void OnCommentsQuoteLabelClicked()
        {
            CommentsQuoteLabelClicked?.Invoke(this);//Raise the event
        }


        public virtual void OnLikeClicked()
        {
            LikeClicked?.Invoke(this);//Raise the event
        }

        public virtual void OnCommentsLikeClicked(object sender, int NewsID, int NewsCommentID)
        {
            CommentsLikeClicked?.Invoke(sender, NewsID, NewsCommentID);//Raise the event
        }

        public virtual void OnEditLabelClick()
        {
            EditLabelClicked?.Invoke(this);//Raise the event
        }

        public virtual void OnCommentsTextBoxSizeChanged(EventArgs e)
        {
            CommentsTextBoxSizeChanged?.Invoke(this, e);//Raise the event
        }

        public virtual void OnAttachsAllFilesClick()
        {
            AttachsAllFilesClicked?.Invoke(this);//Raise the event
        }

        public virtual void OnAllCommentsLabelClick()
        {
            AllCommentsLabelClicked?.Invoke(this);//Raise the event
        }

        public virtual void OnCommentsLabelClick()
        {
            CommentsLabelClicked?.Invoke(this);//Raise the event
        }

        public virtual void OnCommentsCancelButtonClick()
        {
            CommentsCancelButtonClicked?.Invoke(this);//Raise the event
        }


        public virtual void OnRemoveLabelClicked(object sender)
        {
            RemoveLabelClicked?.Invoke(sender);//Raise the event
        }

        public virtual void OnCommentsCommentClicked(object sender)
        {
            CommentsCommentClicked?.Invoke(sender);//Raise the event
        }

        public virtual void OnCommentsRemoveClicked(object sender, int NewsID, int NewsCommentID)
        {
            CommentsRemoveClicked?.Invoke(sender, NewsID, NewsCommentID);//Raise the event
        }

        public virtual void OnCommentsEditClicked(object sender, int NewsID, int NewsCommentID)
        {
            CommentsEditClicked?.Invoke(sender, NewsID, NewsCommentID);//Raise the event
        }


        public virtual void OnSendButtonClicked(object sender, string Text, bool bEdit, int NewsCommentID, bool bNoNotify)
        {
            SendButtonClicked?.Invoke(this, Text, bEdit, NewsCommentID, bNoNotify);//Raise the event
        }


        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            Parent.Focus();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            //if(iFirstHeight > 0 && iFirstWidth > 0)
            //    if (this.Height != iFirstHeight || this.Width != iFirstWidth)
            //    {
            //        SetPositions(this.Height - iFirstHeight);

            //        iFirstWidth = this.Width;
            //        iFirstHeight = this.Height;
            //    }
        }
    }





    public class InfiniumProjectNewsCommentsRichTextBox : Control
    {
        ComponentFactory.Krypton.Toolkit.KryptonRichTextBox RichTextBox;

        ComponentFactory.Krypton.Toolkit.KryptonButton SendButton;
        ComponentFactory.Krypton.Toolkit.KryptonButton CancelButton;

        ComponentFactory.Krypton.Toolkit.KryptonCheckBox NoNotifyCheckBox;

        Label InfoLabel;

        bool bEdit = false;
        int iStartHeight = 150;
        int iNewsCommentID = -1;
        int iLineHeight = -1;

        bool bCtrlEnter = false;

        public InfiniumProjectNewsCommentsRichTextBox()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            Create();

            this.BackColor = Color.Transparent;
        }

        private void Create()
        {
            RichTextBox = new ComponentFactory.Krypton.Toolkit.KryptonRichTextBox()
            {
                Parent = this
            };
            RichTextBox.StateCommon.Content.Font = new System.Drawing.Font("Segoe UI", 15.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            RichTextBox.TextChanged += OnRichTextBoxTextChanged;
            RichTextBox.KeyDown += OnRichTextBoxKeyDown;

            SendButton = new ComponentFactory.Krypton.Toolkit.KryptonButton()
            {
                Parent = this,
                Text = "Отправить"
            };
            SendButton.StateCommon.Back.Color1 = Color.FromArgb(56, 184, 238);
            SendButton.StateCommon.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            SendButton.StateCommon.Content.ShortText.Color1 = Color.White;
            SendButton.StateCommon.Content.ShortText.Font = new Font("Segoe UI", 15.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            SendButton.StateCommon.Border.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.False;
            SendButton.StateCommon.Border.Rounding = 0;
            SendButton.Click += OnSendButtonClicked;
            SendButton.OverrideDefault.Back.Color1 = Color.FromArgb(56, 184, 238);
            SendButton.OverrideDefault.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            SendButton.StateTracking.Back.Color1 = Color.FromArgb(67, 191, 246);

            CancelButton = new ComponentFactory.Krypton.Toolkit.KryptonButton()
            {
                Parent = this,
                Text = "Отмена"
            };
            CancelButton.StateCommon.Back.Color1 = Color.DarkGray;
            CancelButton.StateCommon.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            CancelButton.StateCommon.Content.ShortText.Color1 = Color.White;
            CancelButton.StateCommon.Content.ShortText.Font = new Font("Segoe UI", 15.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            CancelButton.StateCommon.Border.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.False;
            CancelButton.StateCommon.Border.Rounding = 0;
            CancelButton.Click += OnCancelButtonClicked;
            CancelButton.OverrideDefault.Back.Color1 = Color.DarkGray;
            CancelButton.OverrideDefault.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            CancelButton.StateTracking.Back.Color1 = Color.FromArgb(179, 179, 179);

            InfoLabel = new Label()
            {
                Parent = this,
                ForeColor = Color.FromArgb(110, 110, 110),
                Font = CancelButton.Font,
                Text = "CTRL+Enter - отправить сообщение",
                AutoSize = true
            };
            NoNotifyCheckBox = new KryptonCheckBox()
            {
                Parent = this
            };
            NoNotifyCheckBox.StateCommon.ShortText.Color1 = Color.Black;
            NoNotifyCheckBox.Text = "Без уведомлений";
            NoNotifyCheckBox.Checked = false;
        }

        public string CommentsText
        {
            get { return RichTextBox.Text; }
            set { RichTextBox.Text = value; }
        }

        public bool Edit
        {
            get { return bEdit; }
            set { bEdit = value; }
        }

        public int NewsCommentID
        {
            get { return iNewsCommentID; }
            set { iNewsCommentID = value; }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            RichTextBox.Width = this.Width;
            RichTextBox.Height = this.Height - 40;
            RichTextBox.Left = 0;
            RichTextBox.Top = 0;
            SetLineHeight();

            SendButton.Width = 106;
            SendButton.Height = 32;
            SendButton.Left = 1;
            SendButton.Top = RichTextBox.Top + RichTextBox.Height + 5;

            CancelButton.Width = 106;
            CancelButton.Height = 32;
            CancelButton.Left = 1 + SendButton.Width + 6;
            CancelButton.Top = RichTextBox.Top + RichTextBox.Height + 5;

            InfoLabel.Top = CancelButton.Top + (CancelButton.Height - InfoLabel.Height) / 2;
            InfoLabel.Left = CancelButton.Width + CancelButton.Left + 5;

            NoNotifyCheckBox.Top = CancelButton.Top + (CancelButton.Height - NoNotifyCheckBox.Height) / 2;
            NoNotifyCheckBox.Left = InfoLabel.Width + InfoLabel.Left + 5;
        }

        private void SetLineHeight()
        {
            using (Graphics G = RichTextBox.CreateGraphics())
            {
                iLineHeight = Convert.ToInt32(G.MeasureString("Text", RichTextBox.StateCommon.Content.Font).Height);
            }
        }

        private void OnRichTextBoxTextChanged(object sender, EventArgs e)
        {
            if (bCtrlEnter)
                return;

            using (Graphics G = RichTextBox.CreateGraphics())
            {
                int TH = RichTextBox.GetLineFromCharIndex(RichTextBox.Text.Length + 1) * iLineHeight + iLineHeight;

                bool bChanged = false;

                if (TH + 40 > RichTextBox.Height)
                {
                    this.Height = TH + 40 + 40;//40 for buttons
                    bChanged = true;
                }

                if (TH + 40 < RichTextBox.Height && this.Height > iStartHeight)
                {
                    this.Height = TH + 40 + 40;//40 for buttons
                    bChanged = true;
                }

                if (this.Height < iStartHeight)
                {
                    this.Height = iStartHeight;
                    bChanged = true;
                }

                if (bChanged)
                    OnRichTextBoxSizeChanged(this, e);
            }
        }

        private void OnRichTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                OnSendButtonClicked(SendButton, null);
            }
        }

        public event SendButtonEventHandler SendButtonClicked;
        public event EventHandler CancelButtonClicked;
        public event EventHandler RichTextBoxSizeChanged;

        public delegate void SendButtonEventHandler(object sender, string Text, bool bIsEdit, bool bNoNotify);


        public void Clear()
        {
            RichTextBox.Clear();
        }

        public virtual void OnRichTextBoxSizeChanged(object sender, EventArgs e)
        {
            RichTextBoxSizeChanged?.Invoke(this, e);//Raise the event
        }

        public virtual void OnSendButtonClicked(object sender, EventArgs e)
        {
            SendButtonClicked?.Invoke(this, RichTextBox.Text, bEdit, NoNotifyCheckBox.Checked);//Raise the event
        }

        public virtual void OnCancelButtonClicked(object sender, EventArgs e)
        {
            CancelButtonClicked?.Invoke(this, null);//Raise the event
        }


        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            RichTextBox.Focus();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (this.Visible)
                this.Focus();
        }
    }








    //documents

    public class InfiniumFileListItem : Control
    {
        Color cCaptionColor = Color.FromArgb(60, 60, 60);
        Color cStatusColor = Color.FromArgb(150, 150, 150);
        Color cSelectedCaptionColor = Color.FromArgb(56, 184, 238);

        Color cBackColor = Color.White;
        Color cBackSelectedColor = Color.FromArgb(253, 253, 253);

        Color cBorderColor = Color.FromArgb(240, 240, 240);

        Color cFileSizeColor = Color.FromArgb(120, 120, 120);

        Pen pBorderPen;

        public int iMarginForFileSizePercents = 43;
        public int iMarginForAuthorPercents = 58;
        public int iMarginForLastModifiedPercents = 85;

        Font fCaptionFont = new Font("Segoe UI", 18.0f, FontStyle.Regular, GraphicsUnit.Pixel);
        Font fStatusFont = new Font("Segoe UI", 14.0f, FontStyle.Regular, GraphicsUnit.Pixel);
        Font fFileSizeFont = new Font("Segoe UI", 12.0f, FontStyle.Regular, GraphicsUnit.Pixel);

        SolidBrush brCaptionBrush;
        SolidBrush brSelectedCaptionBrush;
        SolidBrush brStatusBrush;
        SolidBrush brFileSizeBrush;

        bool bSelected = false;
        bool bCheckVisible = false;
        bool bChecked = false;

        int iFolderID = -1;
        int iFileID = -1;
        int iFileSize = 0;

        int iCheckHeight = 0;
        int iCheckWidth = 0;
        int iCheckWidthWithMargin = 0;

        string sCaption = "Folder name";

        int iItemHeight = 50;

        string sExtension = "";
        string sAuthor = "";
        string sLastModified = "";

        Bitmap IconFolder = Properties.Resources.Folder;
        Bitmap ArchiveFileBMP = Properties.Resources.ArchiveFile;
        Bitmap ExcelFileBMP = Properties.Resources.ExcelFileIcon;
        Bitmap WordFileBMP = Properties.Resources.WordFileIcon;
        Bitmap ImageFileBMP = Properties.Resources.ImageFileIcon;
        Bitmap PDFFileBMP = Properties.Resources.PDFFileIcon;
        Bitmap OtherFileBMP = Properties.Resources.OtherFile;

        Bitmap RootUpBMP = Properties.Resources.FolderUp;

        Bitmap FileCheckedBMP = Properties.Resources.FileChecked;
        Bitmap FileUncheckedBMP = Properties.Resources.FileUnchecked;


        public InfiniumFileListItem()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            this.BackColor = Color.White;
            this.Cursor = Cursors.Hand;

            brCaptionBrush = new SolidBrush(cCaptionColor);
            brSelectedCaptionBrush = new SolidBrush(cSelectedCaptionColor);
            brStatusBrush = new SolidBrush(cStatusColor);
            brFileSizeBrush = new SolidBrush(cFileSizeColor);

            pBorderPen = new Pen(new SolidBrush(cBorderColor));

            this.Height = iItemHeight;
            this.Width = 50;
        }


        public Color BorderColor
        {
            get { return cBorderColor; }
            set { cBorderColor = value; pBorderPen.Color = value; this.Refresh(); }
        }

        public Color CaptionColor
        {
            get { return cCaptionColor; }
            set { cCaptionColor = value; brCaptionBrush.Color = cCaptionColor; this.Refresh(); }
        }

        public Color SelectedCaptionColor
        {
            get { return cSelectedCaptionColor; }
            set { cSelectedCaptionColor = value; brSelectedCaptionBrush.Color = cSelectedCaptionColor; this.Refresh(); }
        }


        public Color StatusColor
        {
            get { return cStatusColor; }
            set { cStatusColor = value; this.Refresh(); }
        }

        public Color BackSelectedColor
        {
            get { return cBackSelectedColor; }
            set { cBackSelectedColor = value; this.Refresh(); }
        }

        public Font CaptionFont
        {
            get { return fCaptionFont; }
            set { fCaptionFont = value; this.Refresh(); }
        }

        public Font StatusFont
        {
            get { return fStatusFont; }
            set { fStatusFont = value; this.Refresh(); }
        }

        public bool Selected
        {
            get { return bSelected; }
            set
            {
                bSelected = value;

                if (value)
                    this.BackColor = cBackSelectedColor;
                else
                    this.BackColor = cBackColor;

                this.Refresh();
            }
        }


        public bool CheckVisible
        {
            get { return bCheckVisible; }
            set
            {
                bCheckVisible = value;

                if (sExtension == "root")
                    bCheckVisible = false;

                if (value == false)
                    bChecked = false;

                this.Refresh();
            }
        }

        public bool Checked
        {
            get { return bChecked; }
            set
            {
                bChecked = value;

                this.Refresh();
            }
        }


        public string Caption
        {
            get { return sCaption; }
            set { sCaption = value; this.Refresh(); }
        }

        public string Extension
        {
            get { return sExtension; }
            set { sExtension = value; this.Refresh(); }
        }


        public string Author
        {
            get { return sAuthor; }
            set { sAuthor = value; this.Refresh(); }
        }


        public string LastModified
        {
            get { return sLastModified; }
            set { sLastModified = value; this.Refresh(); }
        }


        public int ItemHeight
        {
            get { return iItemHeight; }
            set { iItemHeight = value; this.Height = value; this.Refresh(); }
        }

        public int FolderID
        {
            get { return iFolderID; }
            set { iFolderID = value; }
        }

        public int FileID
        {
            get { return iFileID; }
            set { iFileID = value; }
        }

        public int FileSize
        {
            get { return iFileSize; }
            set { iFileSize = value; }
        }

        public int RightMarginForSize
        {
            get { return iMarginForFileSizePercents; }
            set { iMarginForFileSizePercents = value; }
        }

        public event ItemRightClickedEventHandler ItemRightClicked;

        public delegate void ItemRightClickedEventHandler(object sender, int FolderID, int FileID);


        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == System.Windows.Forms.MouseButtons.Right)
                OnItemRightClicked(this, FolderID, FileID);

            if (bCheckVisible)
                if (e.X >= 5 && e.X <= 5 + iCheckWidth)
                {
                    bChecked = !bChecked;
                    this.Refresh();
                }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

            string text = "";

            if (e.Graphics.MeasureString(sCaption, fCaptionFont).Width > (this.Width / 100 * iMarginForFileSizePercents) - 60)
            {
                for (int i = sCaption.Length; i >= 0; i--)
                {
                    if (e.Graphics.MeasureString(sCaption.Substring(0, i), fCaptionFont).Width <= (this.Width / 100 * iMarginForFileSizePercents) - 60)
                    {
                        text = sCaption.Substring(0, i - 1) + "...";
                        break;
                    }
                }
            }
            else
                text = sCaption;



            if (bCheckVisible)
            {
                iCheckHeight = 28;
                iCheckWidth = FileCheckedBMP.Width * iCheckHeight / FileCheckedBMP.Height;
                iCheckWidthWithMargin = iCheckWidth + 15;

                if (bChecked)
                {
                    FileCheckedBMP.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);
                    e.Graphics.DrawImage(FileCheckedBMP, 5, (this.Height - iCheckHeight) / 2 - 2,
                                         iCheckWidth, iCheckHeight);
                }
                else
                {
                    FileUncheckedBMP.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);
                    e.Graphics.DrawImage(FileUncheckedBMP, 5, (this.Height - iCheckHeight) / 2 - 2,
                                         iCheckWidth, iCheckHeight);
                }
            }
            else
            {
                iCheckWidthWithMargin = 0;
            }


            if (sExtension == "root")//root folder up
            {
                RootUpBMP.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);

                e.Graphics.DrawImage(RootUpBMP, 5, (this.Height - IconFolder.Height) / 2 - 2);
            }
            else
                if (sExtension == "folder")//folder
            {
                IconFolder.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);

                e.Graphics.DrawImage(IconFolder, 5 + iCheckWidthWithMargin, (this.Height - IconFolder.Height) / 2 - 2);
            }
            else//file
            {
                if (Extension.Equals("pdf", StringComparison.InvariantCultureIgnoreCase))
                {
                    PDFFileBMP.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);
                    e.Graphics.DrawImage(PDFFileBMP, 5 + iCheckWidthWithMargin, (this.Height - 25) / 2,
                                                         PDFFileBMP.Width * 25 / PDFFileBMP.Height, 25);
                }
                else
                    if (Extension.Equals("doc", StringComparison.InvariantCultureIgnoreCase) ||
                        Extension.Equals("docx", StringComparison.InvariantCultureIgnoreCase))
                {
                    WordFileBMP.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);
                    e.Graphics.DrawImage(WordFileBMP, 5 + iCheckWidthWithMargin, (this.Height - 25) / 2,
                                                          WordFileBMP.Width * 25 / WordFileBMP.Height, 25);
                }
                else
                        if (Extension.Equals("xls", StringComparison.InvariantCultureIgnoreCase) ||
                            Extension.Equals("xlsx", StringComparison.InvariantCultureIgnoreCase))
                {
                    ExcelFileBMP.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);
                    e.Graphics.DrawImage(ExcelFileBMP, 5 + iCheckWidthWithMargin, (this.Height - 25) / 2,
                                                           ExcelFileBMP.Width * 25 / ExcelFileBMP.Height, 25);
                }
                else
                            if (Extension.Equals("zip", StringComparison.InvariantCultureIgnoreCase) ||
                                Extension.Equals("rar", StringComparison.InvariantCultureIgnoreCase))
                {
                    ArchiveFileBMP.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);
                    e.Graphics.DrawImage(ArchiveFileBMP, 5 + iCheckWidthWithMargin, (this.Height - 25) / 2,
                                             ArchiveFileBMP.Width * 25 / ArchiveFileBMP.Height, 25);
                }
                else
                                if (Extension.Equals("jpg", StringComparison.InvariantCultureIgnoreCase) ||
                                    Extension.Equals("jpeg", StringComparison.InvariantCultureIgnoreCase) ||
                                    Extension.Equals("png", StringComparison.InvariantCultureIgnoreCase) ||
                                    Extension.Equals("bmp", StringComparison.InvariantCultureIgnoreCase) ||
                                    Extension.Equals("tiff", StringComparison.InvariantCultureIgnoreCase) ||
                                    Extension.Equals("tif", StringComparison.InvariantCultureIgnoreCase) ||
                                    Extension.Equals("gif", StringComparison.InvariantCultureIgnoreCase) ||
                                    Extension.Equals("tga", StringComparison.InvariantCultureIgnoreCase) ||
                                    Extension.Equals("psd", StringComparison.InvariantCultureIgnoreCase) ||
                                    Extension.Equals("wmf", StringComparison.InvariantCultureIgnoreCase) ||
                                    Extension.Equals("emf", StringComparison.InvariantCultureIgnoreCase))
                {
                    ImageFileBMP.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);
                    e.Graphics.DrawImage(ImageFileBMP, 5 + iCheckWidthWithMargin, (this.Height - 25) / 2,
                                         ImageFileBMP.Width * 25 / ImageFileBMP.Height, 25);
                }
                else
                {
                    OtherFileBMP.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);
                    e.Graphics.DrawImage(OtherFileBMP, 5 + iCheckWidthWithMargin, (this.Height - 25) / 2,
                                         OtherFileBMP.Width * 25 / OtherFileBMP.Height, 25);
                }
            }


            if (bSelected)
                e.Graphics.DrawString(text, fCaptionFont, brSelectedCaptionBrush, 5 + iCheckWidthWithMargin + IconFolder.Width + 3,
                                                      (this.Height - e.Graphics.MeasureString(sCaption, fCaptionFont).Height) / 2 - 1);
            else
                e.Graphics.DrawString(text, fCaptionFont, brCaptionBrush, 5 + iCheckWidthWithMargin + IconFolder.Width + 3,
                                      (this.Height - e.Graphics.MeasureString(sCaption, fCaptionFont).Height) / 2 - 1);

            if (iFileSize > 0)
                e.Graphics.DrawString(GetFileSizeInKB(iFileSize), fFileSizeFont, brFileSizeBrush, this.Width / 100 * iMarginForFileSizePercents,
                                      (this.Height - e.Graphics.MeasureString(iFileSize.ToString(), fFileSizeFont).Height) / 2);

            e.Graphics.DrawString(Author, fFileSizeFont, brFileSizeBrush, this.Width / 100 * iMarginForAuthorPercents,
                                      (this.Height - e.Graphics.MeasureString(Author, fFileSizeFont).Height) / 2);

            e.Graphics.DrawString(LastModified, fFileSizeFont, brFileSizeBrush, this.Width / 100 * iMarginForLastModifiedPercents,
                                      (this.Height - e.Graphics.MeasureString(LastModified, fFileSizeFont).Height) / 2);

            e.Graphics.DrawLine(pBorderPen, 0, this.Height, this.Width, this.Height);
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            Parent.Focus();
        }

        public virtual void OnItemRightClicked(object sender, int FolderID, int FileID)
        {
            ItemRightClicked?.Invoke(sender, FolderID, FileID);//Raise the event
        }

        private string GetFileSizeInKB(int FileSize)
        {
            if (FileSize < 1000)//байты
                return FileSize.ToString() + " Б";
            else
            {
                decimal Ks = Decimal.Round(Convert.ToDecimal(FileSize) / 1024, 0, MidpointRounding.AwayFromZero);

                return String.Format("{0:### ### ### ### ### ###}", Convert.ToInt32(Ks)).TrimStart() + " КБ";
            }
        }
    }





    public class InfiniumFileList : Control
    {
        public int Offset = 0;

        int iTempScrollWheelOffset = 0;

        InfiniumScrollContainer ScrollContainer;
        public InfiniumProjectsVerticalScrollBar VerticalScroll;

        public InfiniumFileListItem[] FileItems;

        DataTable ItemsDT;

        int iSelected = -1;
        int iEntered = -1;

        bool bCheckVisible = false;

        public InfiniumFileList()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);

            VerticalScroll = new InfiniumProjectsVerticalScrollBar()
            {
                Parent = this,
                Height = this.Height
            };
            VerticalScroll.Left = this.Width - VerticalScroll.Width;
            VerticalScroll.TotalControlHeight = this.Height;
            VerticalScroll.ScrollPositionChanged += OnScrollPositionChanged;

            ScrollContainer = new InfiniumScrollContainer()
            {
                Parent = this,
                Top = 0,
                Left = 0,
                Width = this.Width,
                Height = this.Height,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            ScrollContainer.SizeChanged += OnScrollContainerSizeChanged;
        }

        public void InitializeItems()
        {
            if (FileItems != null)
            {
                for (int i = 0; i < FileItems.Count(); i++)
                {
                    if (FileItems[i] != null)
                    {
                        FileItems[i].Dispose();
                        FileItems[i] = null;
                    }
                }

                FileItems = new InfiniumFileListItem[0];
            }

            Offset = 0;
            ScrollContainer.Top = -Offset;
            VerticalScroll.Offset = Offset;
            VerticalScroll.Refresh();

            if (ItemsDT == null)
                return;

            //ItemsDataTable.DefaultView.Sort = "ItemName ASC";

            if (ItemsDT.DefaultView.Count > 0)
            {
                FileItems = new InfiniumFileListItem[ItemsDT.DefaultView.Count];

                for (int i = 0; i < ItemsDT.DefaultView.Count; i++)
                {
                    FileItems[i] = new InfiniumFileListItem()
                    {
                        Parent = ScrollContainer
                    };
                    FileItems[i].Top = i * FileItems[i].ItemHeight;
                    FileItems[i].Caption = ItemsDT.DefaultView[i]["ItemName"].ToString();
                    FileItems[i].Extension = ItemsDT.DefaultView[i]["Extension"].ToString();
                    FileItems[i].FolderID = Convert.ToInt32(ItemsDT.DefaultView[i]["FolderID"]);
                    FileItems[i].FileID = Convert.ToInt32(ItemsDT.DefaultView[i]["FileID"]);
                    FileItems[i].FileSize = Convert.ToInt32(ItemsDT.DefaultView[i]["FileSize"]);
                    FileItems[i].Author = ItemsDataTable.DefaultView[i]["Author"].ToString();
                    FileItems[i].LastModified = ItemsDataTable.DefaultView[i]["LastModified"].ToString();
                    FileItems[i].Width = ScrollContainer.Width;
                    FileItems[i].Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                    FileItems[i].Click += OnItemClick;
                    FileItems[i].DoubleClick += OnItemDoubleClick;
                    FileItems[i].ItemRightClicked += OnItemRightClick;
                }
            }

            SetScrollHeight();

        }

        private void SetScrollHeight()
        {
            if (FileItems == null || ItemsDataTable.DefaultView.Count == 0)
                return;

            if (FileItems.Count() * FileItems[0].Height > this.Height)
                ScrollContainer.Height = FileItems.Count() * FileItems[0].Height;
            else
                ScrollContainer.Height = this.Height;

            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
            VerticalScroll.Refresh();
        }


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public InfiniumProjectsVerticalScrollBar VerticalScrollBar
        {
            get { return VerticalScroll; }
            set { VerticalScroll = value; VerticalScroll.Left = this.Width - VerticalScroll.Width; }
        }


        public DataTable ItemsDataTable
        {
            get
            {
                if (bCheckVisible)
                {
                    if (FileItems.Count() > 0)
                        for (int i = 0; i < FileItems.Count(); i++)
                        {
                            if (FileItems[i] == null)
                                continue;

                            if (FileItems[i].Checked)
                                ItemsDT.Rows[i]["Checked"] = true;
                            else
                                ItemsDT.Rows[i]["Checked"] = false;
                        }
                }

                return ItemsDT;
            }
            set
            {
                ItemsDT = value;

                if (ItemsDT == null)
                    Selected = -1;
                else
                    if (ItemsDT.Rows.Count == 0)
                {
                    Selected = -1;
                }

                InitializeItems();
            }
        }

        public int Selected
        {
            get { return iSelected; }
            set
            {
                iSelected = value;

                if (iSelected == -1)
                {
                    return;
                }

                if (ItemsDT == null)
                {
                    return;
                }

                if (ItemsDT.Rows.Count == 0)
                {
                    return;
                }

                for (int i = 0; i < FileItems.Count(); i++)
                {
                    if (i != iSelected)
                        FileItems[i].Selected = false;
                    else
                        FileItems[i].Selected = true;
                }

                if (iSelected > -1)
                    OnSelectedChanged(this, ((InfiniumFileListItem)FileItems[iSelected]).FolderID, ((InfiniumFileListItem)FileItems[iSelected]).FileID);
                else
                    OnSelectedChanged(this, -1, -1);
            }
        }

        public int Entered
        {
            get { return iEntered; }
            set
            {
                iEntered = value;
            }
        }

        public bool CheckVisible
        {
            get { return bCheckVisible; }
            set
            {


                if (FileItems == null)
                {
                    bCheckVisible = false;
                    return;
                }

                if (FileItems.Count() == 0)
                {
                    bCheckVisible = false;
                    return;
                }

                if (value == true)
                    if (bCheckVisible)
                        return;
                    else
                    {
                        foreach (InfiniumFileListItem Item in FileItems)
                        {
                            Item.CheckVisible = true;
                        }

                        bCheckVisible = true;
                    }

                if (value == false)
                    if (!bCheckVisible)
                        return;
                    else
                    {
                        foreach (InfiniumFileListItem Item in FileItems)
                        {
                            Item.CheckVisible = false;
                        }

                        bCheckVisible = false;
                    }
            }
        }

        public void EnterInFolder(int FolderID)
        {
            if (FileItems != null)
            {
                if (FileItems.Count() > 0)
                    FileItems[0].Selected = true;
                else
                    return;
            }
            else
                return;

            Selected = 0;

            if (iSelected > -1)
                OnSelectedChanged(this, ((InfiniumFileListItem)FileItems[iSelected]).FolderID, ((InfiniumFileListItem)FileItems[iSelected]).FileID);
            else
                OnSelectedChanged(this, FolderID, -1);

            OnFolderEntered(this, FolderID, -1);
        }

        public int GetIndex(int FolderID)
        {
            for (int i = 0; i < FileItems.Count(); i++)
            {
                if (FileItems[i].FolderID == FolderID)
                    return i;
            }

            return 0;
        }

        public void RootOutFolder(int FolderID)
        {
            if (Entered > -1)
                FileItems[GetIndex(iEntered)].Selected = true;
            else
                FileItems[0].Selected = true;

            if (iSelected > -1)
                OnSelectedChanged(this, ((InfiniumFileListItem)FileItems[iSelected]).FolderID, ((InfiniumFileListItem)FileItems[iSelected]).FileID);
            else
                OnSelectedChanged(this, FolderID, -1);

            OnFolderEntered(this, FolderID, -1);
        }

        private void OnItemDoubleClick(object sender, EventArgs e)
        {
            int FolderID = ((InfiniumFileListItem)sender).FolderID;
            int FileID = ((InfiniumFileListItem)sender).FileID;

            if (ItemsDT.Select("FolderID = " + FolderID)[0]["Extension"].ToString() == "root")
                OnRootDoubleClicked(sender, FolderID, FileID);
            else
                OnItemDoubleClicked(sender, FolderID, FileID);
        }

        private void OnScrollContainerSizeChanged(object sender, EventArgs e)
        {
            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
        }

        private void OnScrollPositionChanged(object sender, int tOffset)
        {
            Offset = tOffset;
            ScrollContainer.Top = -Offset;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (!VerticalScroll.Visible)
                return;

            if (Offset == 0)
                if (iTempScrollWheelOffset > 0)
                    iTempScrollWheelOffset = 0;

            if (e.Delta < 0)
            {
                if (Offset < ScrollContainer.Height - this.Height)
                {
                    if (Offset + VerticalScroll.ScrollWheelOffset + this.Height > ScrollContainer.Height)//last
                    {
                        iTempScrollWheelOffset = ScrollContainer.Height - this.Height - Offset;
                        Offset += iTempScrollWheelOffset;
                    }
                    else
                    {
                        Offset += VerticalScroll.ScrollWheelOffset;
                    }

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
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
                        Offset -= VerticalScroll.ScrollWheelOffset;

                    if (Offset < 0)
                        Offset = 0;

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
                }
        }

        private void OnItemClick(object sender, EventArgs e)
        {
            for (int i = 0; i < FileItems.Count(); i++)
            {
                if (sender != FileItems[i] && FileItems[i] != null)
                    FileItems[i].Selected = false;
                else
                {
                    FileItems[i].Selected = true;
                    Selected = i;
                }
            }
        }

        private void OnItemRightClick(object sender, int FolderID, int FileID)
        {
            for (int i = 0; i < FileItems.Count(); i++)
            {
                if (sender != FileItems[i] && FileItems[i] != null)
                    FileItems[i].Selected = false;
                else
                {
                    FileItems[i].Selected = true;
                    Selected = i;
                }
            }

            OnItemRightClicked(sender, FolderID, FileID);
        }

        private void OnVerticalScrollVisibleChanged(object sender, EventArgs e)
        {
            if (VerticalScroll.Visible)
                ScrollContainer.Width = this.Width - VerticalScroll.Width;
            else
                ScrollContainer.Width = this.Width;
        }


        public delegate void ItemDoubleClickedEventHandler(object sender, int FolderID, int FileID);
        public delegate void ItemRightClickedEventHandler(object sender, int FolderID, int FileID);

        public event ItemRightClickedEventHandler SelectedChanged;
        public event ItemRightClickedEventHandler ItemRightClicked;
        public event ItemDoubleClickedEventHandler ItemDoubleClick;
        public event ItemDoubleClickedEventHandler RootDoubleClick;
        public event ItemDoubleClickedEventHandler FolderEntered;

        public virtual void OnFolderEntered(object sender, int FolderID, int FileID)
        {
            FolderEntered?.Invoke(sender, FolderID, FileID);//Raise the event
        }


        public virtual void OnSelectedChanged(object sender, int FolderID, int FileID)
        {
            SelectedChanged?.Invoke(sender, FolderID, FileID);//Raise the event
        }

        public virtual void OnItemDoubleClicked(object sender, int FolderID, int FileID)
        {
            ItemDoubleClick?.Invoke(sender, FolderID, FileID);//Raise the event
        }


        public virtual void OnRootDoubleClicked(object sender, int FolderID, int FileID)
        {
            RootDoubleClick?.Invoke(this, FolderID, FileID);//Raise the event
        }


        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            ScrollContainer.Focus();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            SetScrollHeight();

            VerticalScroll.Left = this.Width - VerticalScroll.Width;
        }


        public virtual void OnItemRightClicked(object sender, int FolderID, int FileID)
        {
            ItemRightClicked?.Invoke(sender, FolderID, FileID);//Raise the event
        }
    }





    public class InfiniumFilesAttributeItem : Control
    {
        Color cCaptionColor = Color.FromArgb(60, 60, 60);
        Color cSelectedCaptionColor = Color.FromArgb(56, 184, 238);

        Color cBackColor = Color.White;
        Color cBackSelectedColor = Color.White;

        Color cBorderColor = Color.FromArgb(240, 240, 240);

        Pen pBorderPen;

        Font fCaptionFont = new Font("Segoe UI", 18.0f, FontStyle.Regular, GraphicsUnit.Pixel);
        Font fFileSizeFont = new Font("Segoe UI", 14.0f, FontStyle.Regular, GraphicsUnit.Pixel);

        SolidBrush brCaptionBrush;
        SolidBrush brSelectedCaptionBrush;

        bool bSelected = false;
        bool bChecked = false;

        int iCheckHeight = 0;
        int iCheckWidth = 0;
        int iCheckWidthWithMargin = 0;

        string sCaption = "Item";

        int iItemHeight = 50;

        Bitmap FileCheckedBMP = Properties.Resources.FileChecked;
        Bitmap FileUncheckedBMP = Properties.Resources.FileUnchecked;


        public InfiniumFilesAttributeItem()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            this.BackColor = Color.White;
            this.Cursor = Cursors.Hand;

            brCaptionBrush = new SolidBrush(cCaptionColor);
            brSelectedCaptionBrush = new SolidBrush(cSelectedCaptionColor);

            pBorderPen = new Pen(new SolidBrush(cBorderColor));

            this.Height = iItemHeight;
            this.Width = 50;
        }


        public Color BorderColor
        {
            get { return cBorderColor; }
            set { cBorderColor = value; pBorderPen.Color = value; this.Refresh(); }
        }

        public Color CaptionColor
        {
            get { return cCaptionColor; }
            set { cCaptionColor = value; brCaptionBrush.Color = cCaptionColor; this.Refresh(); }
        }

        public Color SelectedCaptionColor
        {
            get { return cSelectedCaptionColor; }
            set { cSelectedCaptionColor = value; brSelectedCaptionBrush.Color = cSelectedCaptionColor; this.Refresh(); }
        }

        public Color BackSelectedColor
        {
            get { return cBackSelectedColor; }
            set { cBackSelectedColor = value; this.Refresh(); }
        }

        public Font CaptionFont
        {
            get { return fCaptionFont; }
            set { fCaptionFont = value; this.Refresh(); }
        }

        public bool Selected
        {
            get { return bSelected; }
            set
            {
                bSelected = value;

                if (value)
                    this.BackColor = cBackSelectedColor;
                else
                    this.BackColor = cBackColor;

                this.Refresh();
            }
        }

        public bool Checked
        {
            get { return bChecked; }
            set
            {
                bChecked = value;

                this.Refresh();
            }
        }


        public string Caption
        {
            get { return sCaption; }
            set { sCaption = value; this.Refresh(); }
        }

        public int ItemHeight
        {
            get { return iItemHeight; }
            set { iItemHeight = value; this.Height = value; this.Refresh(); }
        }


        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.X >= 5 && e.X <= 5 + iCheckWidth)
            {
                bChecked = !bChecked;
                this.Refresh();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;


            iCheckHeight = 28;
            iCheckWidth = FileCheckedBMP.Width * iCheckHeight / FileCheckedBMP.Height;
            iCheckWidthWithMargin = iCheckWidth + 15;

            if (bChecked)
            {
                FileCheckedBMP.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);
                e.Graphics.DrawImage(FileCheckedBMP, 5, (this.Height - iCheckHeight) / 2 - 2,
                                        iCheckWidth, iCheckHeight);
            }
            else
            {
                FileUncheckedBMP.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);
                e.Graphics.DrawImage(FileUncheckedBMP, 5, (this.Height - iCheckHeight) / 2 - 2,
                                        iCheckWidth, iCheckHeight);
            }


            string text = "";

            if (e.Graphics.MeasureString(sCaption, fCaptionFont).Width > this.Width - iCheckWidth)
            {
                for (int i = sCaption.Length; i >= 0; i--)
                {
                    if (e.Graphics.MeasureString(sCaption.Substring(0, i), fCaptionFont).Width <= this.Width - iCheckWidth)
                    {
                        text = sCaption.Substring(0, i - 1) + "...";
                        break;
                    }
                }
            }
            else
                text = sCaption;

            if (bSelected)
                e.Graphics.DrawString(text, fCaptionFont, brSelectedCaptionBrush, 5 + iCheckWidthWithMargin,
                                                      (this.Height - e.Graphics.MeasureString(sCaption, fCaptionFont).Height) / 2 - 1);
            else
                e.Graphics.DrawString(text, fCaptionFont, brCaptionBrush, 5 + iCheckWidthWithMargin,
                                      (this.Height - e.Graphics.MeasureString(sCaption, fCaptionFont).Height) / 2 - 1);


            e.Graphics.DrawLine(pBorderPen, 0, this.Height, this.Width, this.Height);
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            Parent.Focus();
        }
    }





    public class InfiniumFilesAttributesList : Control
    {
        public int Offset = 0;

        int iTempScrollWheelOffset = 0;

        InfiniumScrollContainer ScrollContainer;
        public InfiniumProjectsVerticalScrollBar VerticalScroll;

        public InfiniumFilesAttributeItem[] Items;

        DataTable ItemsDT;

        int iSelected = -1;

        public InfiniumFilesAttributesList()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);

            VerticalScroll = new InfiniumProjectsVerticalScrollBar()
            {
                Parent = this,
                Height = this.Height
            };
            VerticalScroll.Left = this.Width - VerticalScroll.Width;
            VerticalScroll.TotalControlHeight = this.Height;
            VerticalScroll.ScrollPositionChanged += OnScrollPositionChanged;

            ScrollContainer = new InfiniumScrollContainer()
            {
                Parent = this,
                Top = 0,
                Left = 0,
                Width = this.Width,
                Height = this.Height,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            ScrollContainer.SizeChanged += OnScrollContainerSizeChanged;
        }

        public void InitializeItems()
        {
            if (Items != null)
            {
                for (int i = 0; i < Items.Count(); i++)
                {
                    if (Items[i] != null)
                    {
                        Items[i].Dispose();
                        Items[i] = null;
                    }
                }

                Items = new InfiniumFilesAttributeItem[0];
            }

            Offset = 0;
            ScrollContainer.Top = -Offset;
            VerticalScroll.Offset = Offset;
            VerticalScroll.Refresh();

            if (ItemsDT == null)
                return;

            if (ItemsDT.DefaultView.Count > 0)
            {
                Items = new InfiniumFilesAttributeItem[ItemsDT.DefaultView.Count];

                for (int i = 0; i < ItemsDT.DefaultView.Count; i++)
                {
                    Items[i] = new InfiniumFilesAttributeItem()
                    {
                        Parent = ScrollContainer
                    };
                    Items[i].Top = i * Items[i].ItemHeight;
                    Items[i].Caption = ItemsDT.DefaultView[i]["AttributeName"].ToString();
                    Items[i].Width = ScrollContainer.Width;
                    Items[i].Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                    Items[i].Click += OnItemClick;
                }
            }

            SetScrollHeight();

        }

        private void SetScrollHeight()
        {
            if (Items == null || ItemsDataTable.DefaultView.Count == 0)
            {
                ScrollContainer.Height = this.Height;
                VerticalScroll.TotalControlHeight = ScrollContainer.Height;
                VerticalScroll.Refresh();

                return;
            }

            if (Items.Count() * Items[0].Height > this.Height)
                ScrollContainer.Height = Items.Count() * Items[0].Height;
            else
                ScrollContainer.Height = this.Height;

            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
            VerticalScroll.Refresh();
        }


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public InfiniumProjectsVerticalScrollBar VerticalScrollBar
        {
            get { return VerticalScroll; }
            set { VerticalScroll = value; VerticalScroll.Left = this.Width - VerticalScroll.Width; }
        }


        public DataTable ItemsDataTable
        {
            get
            {
                //if (ItemsDT == null)
                //    return ItemsDT;

                //if (Items.Count() > 0)
                //    for (int i = 0; i < Items.Count(); i++)
                //    {
                //        if (Items[i] == null)
                //            continue;

                //        if (Items[i].Checked)
                //            ItemsDT.Rows[i]["Checked"] = true;
                //        else
                //            ItemsDT.Rows[i]["Checked"] = false;
                //    }

                return ItemsDT;
            }
            set
            {
                ItemsDT = value;
            }
        }

        public int Selected
        {
            get { return iSelected; }
            set
            {
                iSelected = value;

                if (iSelected == -1)
                {
                    return;
                }

                if (ItemsDT == null)
                {
                    return;
                }

                if (ItemsDT.Rows.Count == 0)
                {
                    return;
                }

                for (int i = 0; i < Items.Count(); i++)
                {
                    if (i != iSelected)
                        Items[i].Selected = false;
                    else
                        Items[i].Selected = true;
                }

                OnSelectedChanged(this, null);
            }
        }

        private void OnScrollContainerSizeChanged(object sender, EventArgs e)
        {
            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
        }

        private void OnScrollPositionChanged(object sender, int tOffset)
        {
            Offset = tOffset;
            ScrollContainer.Top = -Offset;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (!VerticalScroll.Visible)
                return;

            if (Offset == 0)
                if (iTempScrollWheelOffset > 0)
                    iTempScrollWheelOffset = 0;

            if (e.Delta < 0)
            {
                if (Offset < ScrollContainer.Height - this.Height)
                {
                    if (Offset + VerticalScroll.ScrollWheelOffset + this.Height > ScrollContainer.Height)//last
                    {
                        iTempScrollWheelOffset = ScrollContainer.Height - this.Height - Offset;
                        Offset += iTempScrollWheelOffset;
                    }
                    else
                    {
                        Offset += VerticalScroll.ScrollWheelOffset;
                    }

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
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
                        Offset -= VerticalScroll.ScrollWheelOffset;

                    if (Offset < 0)
                        Offset = 0;

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
                }
        }

        private void OnItemClick(object sender, EventArgs e)
        {
            for (int i = 0; i < Items.Count(); i++)
            {
                if (sender != Items[i] && Items[i] != null)
                    Items[i].Selected = false;
                else
                {
                    Items[i].Selected = true;

                    Selected = i;
                }
            }

            OnItemClicked(sender, ((InfiniumFilesAttributeItem)sender).Caption);
        }

        private void OnVerticalScrollVisibleChanged(object sender, EventArgs e)
        {
            if (VerticalScroll.Visible)
                ScrollContainer.Width = this.Width - VerticalScroll.Width;
            else
                ScrollContainer.Width = this.Width;
        }



        public event EventHandler SelectedChanged;
        public event ItemClickedEventHandler ItemClicked;

        public delegate void ItemClickedEventHandler(object sender, string AttributeName);

        public virtual void OnSelectedChanged(object sender, EventArgs e)
        {
            SelectedChanged?.Invoke(sender, e);//Raise the event
        }


        public virtual void OnItemClicked(object sender, string AttributeName)
        {
            ItemClicked(sender, AttributeName);
        }


        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            ScrollContainer.Focus();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            SetScrollHeight();

            VerticalScroll.Left = this.Width - VerticalScroll.Width;
        }
    }





    public class InfiniumFilesUsersItem : Control
    {
        Color cCaptionColor = Color.FromArgb(60, 60, 60);
        Color cSelectedCaptionColor = Color.FromArgb(56, 184, 238);

        Color cBackColor = Color.White;
        Color cBackSelectedColor = Color.White;

        Color cBorderColor = Color.FromArgb(240, 240, 240);

        Pen pBorderPen;

        Font fCaptionFont = new Font("Segoe UI", 18.0f, FontStyle.Regular, GraphicsUnit.Pixel);
        Font fFileSizeFont = new Font("Segoe UI", 14.0f, FontStyle.Regular, GraphicsUnit.Pixel);

        SolidBrush brCaptionBrush;
        SolidBrush brSelectedCaptionBrush;

        bool bSelected = false;

        int iCheckHeight = 0;
        int iCheckWidth = 0;
        int iCheckWidthWithMargin = 0;

        string sCaption = "Item";

        int iItemHeight = 50;

        int iUserID;

        Bitmap RemoveItemBMP = Properties.Resources.RemoveItem;


        public InfiniumFilesUsersItem()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            this.BackColor = Color.White;
            this.Cursor = Cursors.Hand;

            brCaptionBrush = new SolidBrush(cCaptionColor);
            brSelectedCaptionBrush = new SolidBrush(cSelectedCaptionColor);

            pBorderPen = new Pen(new SolidBrush(cBorderColor));

            this.Height = iItemHeight;
            this.Width = 50;
        }


        public Color BorderColor
        {
            get { return cBorderColor; }
            set { cBorderColor = value; pBorderPen.Color = value; this.Refresh(); }
        }

        public Color CaptionColor
        {
            get { return cCaptionColor; }
            set { cCaptionColor = value; brCaptionBrush.Color = cCaptionColor; this.Refresh(); }
        }

        public Color SelectedCaptionColor
        {
            get { return cSelectedCaptionColor; }
            set { cSelectedCaptionColor = value; brSelectedCaptionBrush.Color = cSelectedCaptionColor; this.Refresh(); }
        }

        public Color BackSelectedColor
        {
            get { return cBackSelectedColor; }
            set { cBackSelectedColor = value; this.Refresh(); }
        }

        public Font CaptionFont
        {
            get { return fCaptionFont; }
            set { fCaptionFont = value; this.Refresh(); }
        }

        public bool Selected
        {
            get { return bSelected; }
            set
            {
                bSelected = value;

                if (value)
                    this.BackColor = cBackSelectedColor;
                else
                    this.BackColor = cBackColor;

                this.Refresh();
            }
        }


        public string Caption
        {
            get { return sCaption; }
            set { sCaption = value; this.Refresh(); }
        }

        public int ItemHeight
        {
            get { return iItemHeight; }
            set { iItemHeight = value; this.Height = value; this.Refresh(); }
        }


        public int UserID
        {
            get { return iUserID; }
            set { iUserID = value; this.Refresh(); }
        }


        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.X >= 5 && e.X <= 5 + iCheckWidth)
            {
                OnItemRemoveClicked(this, UserID);
                this.Refresh();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;


            iCheckHeight = 28;
            iCheckWidth = RemoveItemBMP.Width * iCheckHeight / RemoveItemBMP.Height;
            iCheckWidthWithMargin = iCheckWidth + 15;

            RemoveItemBMP.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);
            e.Graphics.DrawImage(RemoveItemBMP, 5, (this.Height - iCheckHeight) / 2 - 2,
                                    iCheckWidth, iCheckHeight);


            string text = "";

            if (e.Graphics.MeasureString(sCaption, fCaptionFont).Width > this.Width - iCheckWidth)
            {
                for (int i = sCaption.Length; i >= 0; i--)
                {
                    if (e.Graphics.MeasureString(sCaption.Substring(0, i), fCaptionFont).Width <= this.Width - iCheckWidth)
                    {
                        text = sCaption.Substring(0, i - 1) + "...";
                        break;
                    }
                }
            }
            else
                text = sCaption;

            if (bSelected)
                e.Graphics.DrawString(text, fCaptionFont, brSelectedCaptionBrush, 5 + iCheckWidthWithMargin,
                                                      (this.Height - e.Graphics.MeasureString(sCaption, fCaptionFont).Height) / 2 - 1);
            else
                e.Graphics.DrawString(text, fCaptionFont, brCaptionBrush, 5 + iCheckWidthWithMargin,
                                      (this.Height - e.Graphics.MeasureString(sCaption, fCaptionFont).Height) / 2 - 1);


            e.Graphics.DrawLine(pBorderPen, 0, this.Height, this.Width, this.Height);
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            Parent.Focus();
        }


        public event ItemRemoveClickedEventHandler ItemRemoveClicked;

        public delegate void ItemRemoveClickedEventHandler(object sender, int UserID);

        public virtual void OnItemRemoveClicked(object sender, int UserID)
        {
            ItemRemoveClicked?.Invoke(sender, UserID);
        }
    }





    public class InfiniumFilesUsersList : Control
    {
        public int Offset = 0;

        int iTempScrollWheelOffset = 0;

        InfiniumScrollContainer ScrollContainer;
        public InfiniumProjectsVerticalScrollBar VerticalScroll;

        public InfiniumFilesUsersItem[] Items;

        public DataTable ItemsDT;
        public DataTable UsersDataTable;

        int iSelected = -1;

        public InfiniumFilesUsersList()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);

            VerticalScroll = new InfiniumProjectsVerticalScrollBar()
            {
                Parent = this,
                Height = this.Height
            };
            VerticalScroll.Left = this.Width - VerticalScroll.Width;
            VerticalScroll.TotalControlHeight = this.Height;
            VerticalScroll.ScrollPositionChanged += OnScrollPositionChanged;

            ScrollContainer = new InfiniumScrollContainer()
            {
                Parent = this,
                Top = 0,
                Left = 0,
                Width = this.Width,
                Height = this.Height,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            ScrollContainer.SizeChanged += OnScrollContainerSizeChanged;
        }

        public void InitializeItems()
        {
            if (Items != null)
            {
                for (int i = 0; i < Items.Count(); i++)
                {
                    if (Items[i] != null)
                    {
                        Items[i].Dispose();
                        Items[i] = null;
                    }
                }

                Items = new InfiniumFilesUsersItem[0];
            }

            Offset = 0;
            ScrollContainer.Top = -Offset;
            VerticalScroll.Offset = Offset;
            VerticalScroll.Refresh();

            if (ItemsDT == null)
                return;

            if (ItemsDT.DefaultView.Count > 0)
            {
                Items = new InfiniumFilesUsersItem[ItemsDT.DefaultView.Count];

                for (int i = 0; i < ItemsDT.DefaultView.Count; i++)
                {
                    Items[i] = new InfiniumFilesUsersItem()
                    {
                        Parent = ScrollContainer
                    };
                    Items[i].Top = i * Items[i].ItemHeight;
                    Items[i].Caption = UsersDataTable.Select("UserID = " + ItemsDT.DefaultView[i]["UserID"])[0]["Name"].ToString();
                    Items[i].UserID = Convert.ToInt32(ItemsDT.DefaultView[i]["UserID"]);
                    Items[i].Width = ScrollContainer.Width;
                    Items[i].Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                    Items[i].Click += OnItemClick;
                    Items[i].ItemRemoveClicked += OnItemRemoveClicked;
                }
            }

            SetScrollHeight();

        }

        private void SetScrollHeight()
        {
            if (Items == null || ItemsDataTable.DefaultView.Count == 0)
            {
                ScrollContainer.Height = this.Height;
                VerticalScroll.TotalControlHeight = ScrollContainer.Height;
                VerticalScroll.Refresh();

                return;
            }

            if (Items.Count() * Items[0].Height > this.Height)
                ScrollContainer.Height = Items.Count() * Items[0].Height;
            else
                ScrollContainer.Height = this.Height;

            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
            VerticalScroll.Refresh();
        }


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public InfiniumProjectsVerticalScrollBar VerticalScrollBar
        {
            get { return VerticalScroll; }
            set { VerticalScroll = value; VerticalScroll.Left = this.Width - VerticalScroll.Width; }
        }


        public DataTable ItemsDataTable
        {
            get
            {
                //if (ItemsDT == null)
                //    return ItemsDT;

                //if (Items.Count() > 0)
                //    for (int i = 0; i < Items.Count(); i++)
                //    {
                //        if (Items[i] == null)
                //            continue;

                //        if (Items[i].Checked)
                //            ItemsDT.Rows[i]["Checked"] = true;
                //        else
                //            ItemsDT.Rows[i]["Checked"] = false;
                //    }

                return ItemsDT;
            }
            set
            {
                ItemsDT = value;
            }
        }

        public int Selected
        {
            get { return iSelected; }
            set
            {
                iSelected = value;

                if (iSelected == -1)
                {
                    return;
                }

                if (ItemsDT == null)
                {
                    return;
                }

                if (ItemsDT.Rows.Count == 0)
                {
                    return;
                }

                for (int i = 0; i < Items.Count(); i++)
                {
                    if (i != iSelected)
                        Items[i].Selected = false;
                    else
                        Items[i].Selected = true;
                }

                OnSelectedChanged(this, null);
            }
        }

        private void OnScrollContainerSizeChanged(object sender, EventArgs e)
        {
            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
        }

        private void OnScrollPositionChanged(object sender, int tOffset)
        {
            Offset = tOffset;
            ScrollContainer.Top = -Offset;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (!VerticalScroll.Visible)
                return;

            if (Offset == 0)
                if (iTempScrollWheelOffset > 0)
                    iTempScrollWheelOffset = 0;

            if (e.Delta < 0)
            {
                if (Offset < ScrollContainer.Height - this.Height)
                {
                    if (Offset + VerticalScroll.ScrollWheelOffset + this.Height > ScrollContainer.Height)//last
                    {
                        iTempScrollWheelOffset = ScrollContainer.Height - this.Height - Offset;
                        Offset += iTempScrollWheelOffset;
                    }
                    else
                    {
                        Offset += VerticalScroll.ScrollWheelOffset;
                    }

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
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
                        Offset -= VerticalScroll.ScrollWheelOffset;

                    if (Offset < 0)
                        Offset = 0;

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
                }
        }

        private void OnItemClick(object sender, EventArgs e)
        {
            for (int i = 0; i < Items.Count(); i++)
            {
                if (sender != Items[i] && Items[i] != null)
                    Items[i].Selected = false;
                else
                {
                    Items[i].Selected = true;

                    Selected = i;
                }
            }
        }

        private void OnVerticalScrollVisibleChanged(object sender, EventArgs e)
        {
            if (VerticalScroll.Visible)
                ScrollContainer.Width = this.Width - VerticalScroll.Width;
            else
                ScrollContainer.Width = this.Width;
        }

        private void OnItemRemoveClick(object sender, int UserID)
        {
            OnItemRemoveClicked(sender, UserID);
        }

        public event EventHandler SelectedChanged;
        public event ItemRemoveClickedEventHandler ItemRemoveClicked;

        public delegate void ItemRemoveClickedEventHandler(object sender, int UserID);


        public virtual void OnSelectedChanged(object sender, EventArgs e)
        {
            SelectedChanged?.Invoke(sender, e);//Raise the event
        }

        public virtual void OnItemRemoveClicked(object sender, int UserID)
        {
            ItemRemoveClicked?.Invoke(sender, UserID);
        }


        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            ScrollContainer.Focus();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            SetScrollHeight();

            VerticalScroll.Left = this.Width - VerticalScroll.Width;
        }
    }





    public class InfiniumFilesAttributesView : Control
    {
        public int Offset = 0;

        int iTempScrollWheelOffset = 0;

        public InfiniumScrollContainer ScrollContainer;
        public InfiniumProjectsVerticalScrollBar VerticalScroll;

        DataTable AttributesDT;
        public DataTable SignsDT;
        public DataTable ReadListDT;
        public DataTable UsersDataTable;

        int iMarginToNextAttribute = 22;
        int iMarginToAttributeData = 2;
        int iMarginToAttributeName = 7;
        int iMarginToNextAttributeData = 11;

        public int CurrentUserID = -1;

        int iSignButtonLeft = -1;
        int iSignButtonTop = -1;
        int iSignButtonWidth = -1;
        int iSignButtonHeight = -1;

        int iReadButtonLeft = -1;
        int iReadButtonTop = -1;
        int iReadButtonWidth = -1;
        int iReadButtonHeight = -1;

        public int FileID = -1;

        bool bSignButtonVisible = false;
        bool bReadButtonVisible = false;
        bool bSignButtonTrack = false;
        bool bReadButtonTrack = false;

        public bool bFirstSign = false;

        Color cAttributeGroupFontColor = Color.FromArgb(110, 110, 110);
        Color cAttributeItemFontColor = Color.FromArgb(130, 130, 130);
        Color cAttributeValueNotSetColor = Color.FromArgb(180, 180, 180);
        Color cAttributeValueSetColor = Color.FromArgb(120, 120, 120);
        Color cAttributeButtonColor = Color.FromArgb(56, 184, 238);

        Font fAttributeGroupFont = new Font("Segoe UI Semibold", 16.0f, FontStyle.Regular, GraphicsUnit.Pixel);
        Font fAttributeItemFont = new Font("Segoe UI", 15.0f, FontStyle.Regular, GraphicsUnit.Pixel);

        SolidBrush brAttributeGroupFontBrush;
        SolidBrush brAttributeItemFontBrush;
        SolidBrush brAttributeValueNotSetBrush;
        SolidBrush brAttributeValueSetBrush;
        SolidBrush brAttributeButtonBrush;

        public InfiniumFilesAttributesView()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);

            brAttributeGroupFontBrush = new SolidBrush(cAttributeGroupFontColor);
            brAttributeItemFontBrush = new SolidBrush(cAttributeItemFontColor);
            brAttributeValueNotSetBrush = new SolidBrush(cAttributeValueNotSetColor);
            brAttributeValueSetBrush = new SolidBrush(cAttributeValueSetColor);
            brAttributeButtonBrush = new SolidBrush(cAttributeButtonColor);

            VerticalScroll = new InfiniumProjectsVerticalScrollBar()
            {
                Parent = this,
                Height = this.Height
            };
            VerticalScroll.Left = this.Width - VerticalScroll.Width;
            VerticalScroll.TotalControlHeight = this.Height;
            VerticalScroll.ScrollPositionChanged += OnScrollPositionChanged;

            ScrollContainer = new InfiniumScrollContainer()
            {
                Parent = this,
                Top = 0,
                Left = 0,
                Width = this.Width,
                Height = this.Height,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            ScrollContainer.SizeChanged += OnScrollContainerSizeChanged;
            ScrollContainer.Paint += OnPaintScrollContainer;
            ScrollContainer.MouseMove += OnScrollContainerMouseMove;
            ScrollContainer.MouseLeave += OnScrollContainerMouseLeave;
            ScrollContainer.Click += OnScrollContainerClick;
        }

        public void InitializeItems()
        {
            //Offset = 0;
            //ScrollContainer.Top = -Offset;
            //VerticalScroll.Offset = Offset;
            //VerticalScroll.Refresh();

            //if (ItemsDT == null)
            //    return;

            //if (ItemsDT.DefaultView.Count > 0)
            //{

            //}

            //SetScrollHeight();

            ScrollContainer.Refresh();
        }

        private void SetScrollHeight()
        {
            Offset = 0;
            ScrollContainer.Top = -Offset;
            VerticalScroll.Offset = Offset;
            VerticalScroll.Refresh();

            if (AttributesDataTable == null)
                return;

            if (AttributesDataTable.DefaultView.Count == 0)
            {
                ScrollContainer.Height = this.Height;
                VerticalScroll.TotalControlHeight = ScrollContainer.Height;
                VerticalScroll.Refresh();

                return;
            }

            //if (Items.Count() * Items[0].Height > this.Height)
            //    ScrollContainer.Height = Items.Count() * Items[0].Height;
            //else
            //    ScrollContainer.Height = this.Height;

            //VerticalScroll.TotalControlHeight = ScrollContainer.Height;
            //VerticalScroll.Refresh();
        }


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public InfiniumProjectsVerticalScrollBar VerticalScrollBar
        {
            get { return VerticalScroll; }
            set { VerticalScroll = value; VerticalScroll.Left = this.Width - VerticalScroll.Width; }
        }


        public DataTable AttributesDataTable
        {
            get
            {
                return AttributesDT;
            }
            set
            {
                AttributesDT = value;
                InitializeItems();
            }
        }


        private void OnPaintScrollContainer(object sender, PaintEventArgs e)
        {
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            if (AttributesDataTable == null)
                return;

            if (AttributesDataTable.Rows.Count == 0)
            {
                e.Graphics.DrawString("нет аттрибутов", fAttributeItemFont, brAttributeItemFontBrush,
                                                  (this.Width - e.Graphics.MeasureString("нет аттрибутов", fAttributeItemFont).Width) / 2,
                                                  (this.Height - e.Graphics.MeasureString("нет аттрибутов", fAttributeItemFont).Height) / 2);
                return;
            }

            if (SignsDT == null)
                return;

            int iCurTextPosY = 0;

            bSignButtonVisible = false;
            bReadButtonVisible = false;

            for (int i = 0; i < AttributesDataTable.Rows.Count; i++)
            {
                int iMA = 0;

                if (i > 0)
                    iMA = iMarginToNextAttribute;
                else
                    iMA = 0;

                string sCapt = "";

                if (AttributesDataTable.Rows[i]["AttributeName"].ToString() == "Подписи")
                {
                    if (bFirstSign)
                        sCapt = AttributesDataTable.Rows[i]["AttributeName"].ToString().ToUpper() + " (первая подпись)";
                    else
                        sCapt = AttributesDataTable.Rows[i]["AttributeName"].ToString().ToUpper();
                }
                else
                    sCapt = AttributesDataTable.Rows[i]["AttributeName"].ToString().ToUpper();


                e.Graphics.DrawString(sCapt, fAttributeGroupFont, brAttributeGroupFontBrush,
                                                  10, iCurTextPosY + iMA);

                iCurTextPosY += Convert.ToInt32(e.Graphics.MeasureString(sCapt,
                                                fAttributeGroupFont).Height + iMA);


                if (AttributesDataTable.Rows[i]["AttributeName"].ToString() == "Подписи")
                {
                    for (int j = 0; j < SignsDT.Rows.Count; j++)
                    {
                        int iM = 0;

                        if (j > 0)
                            iM = iMarginToNextAttributeData;
                        else
                            iM = iMarginToAttributeName;

                        e.Graphics.DrawString(UsersDataTable.Select("UserID = " + SignsDT.Rows[j]["UserID"])[0]["Name"].ToString(), fAttributeItemFont, brAttributeItemFontBrush,
                                              10, iCurTextPosY + iM);
                        iCurTextPosY += Convert.ToInt32(e.Graphics.MeasureString(UsersDataTable.Select("UserID = " + SignsDT.Rows[j]["UserID"])[0]["Name"].ToString(),
                                                        fAttributeItemFont).Height) + iM;



                        if (Convert.ToBoolean(SignsDT.Rows[j]["IsSigned"]) == true)
                        {
                            e.Graphics.DrawString(Convert.ToDateTime(SignsDT.Rows[j]["DateTime"]).ToString("dd.MM.yyyy HH:mm:ss"), fAttributeItemFont, brAttributeValueSetBrush,
                                              10, iCurTextPosY + iMarginToAttributeData);
                            iCurTextPosY += Convert.ToInt32(e.Graphics.MeasureString(SignsDT.Rows[j]["DateTime"].ToString(),
                                                            fAttributeItemFont).Height) + iMarginToAttributeData;
                        }
                        else
                        {
                            if (Convert.ToInt32(SignsDT.Rows[j]["UserID"]) == CurrentUserID)
                            {
                                iSignButtonLeft = 10;
                                iSignButtonTop = iCurTextPosY + iMarginToAttributeData;
                                iSignButtonWidth = Convert.ToInt32(e.Graphics.MeasureString("подписать", fAttributeItemFont).Width);
                                iSignButtonHeight = Convert.ToInt32(e.Graphics.MeasureString("подписать", fAttributeItemFont).Height);

                                bSignButtonVisible = true;

                                e.Graphics.DrawString("подписать", fAttributeItemFont, brAttributeButtonBrush, 10, iCurTextPosY + iMarginToAttributeData);
                                iCurTextPosY += Convert.ToInt32(e.Graphics.MeasureString("подписать", fAttributeItemFont).Height) + iMarginToAttributeData;
                            }
                            else
                            {
                                e.Graphics.DrawString("нет", fAttributeItemFont, brAttributeValueNotSetBrush, 10, iCurTextPosY + iMarginToAttributeData);
                                iCurTextPosY += Convert.ToInt32(e.Graphics.MeasureString("нет", fAttributeItemFont).Height) + iMarginToAttributeData;
                            }

                        }

                    }
                }


                if (AttributesDataTable.Rows[i]["AttributeName"].ToString() == "Ознакомлен")
                {
                    for (int j = 0; j < ReadListDT.Rows.Count; j++)
                    {
                        int iM = 0;

                        if (j > 0)
                            iM = iMarginToNextAttributeData;
                        else
                            iM = iMarginToAttributeName;

                        e.Graphics.DrawString(UsersDataTable.Select("UserID = " + ReadListDT.Rows[j]["UserID"])[0]["Name"].ToString(), fAttributeItemFont, brAttributeItemFontBrush,
                                              10, iCurTextPosY + iM);
                        iCurTextPosY += Convert.ToInt32(e.Graphics.MeasureString(UsersDataTable.Select("UserID = " + ReadListDT.Rows[j]["UserID"])[0]["Name"].ToString(),
                                                        fAttributeItemFont).Height) + iM;



                        if (Convert.ToBoolean(ReadListDT.Rows[j]["IsSigned"]) == true)
                        {
                            e.Graphics.DrawString(Convert.ToDateTime(ReadListDT.Rows[j]["DateTime"]).ToString("dd.MM.yyyy HH:mm:ss"), fAttributeItemFont, brAttributeValueSetBrush,
                                              10, iCurTextPosY + iMarginToAttributeData);
                            iCurTextPosY += Convert.ToInt32(e.Graphics.MeasureString(ReadListDT.Rows[j]["DateTime"].ToString(),
                                                            fAttributeItemFont).Height) + iMarginToAttributeData;
                        }
                        else
                        {
                            if (Convert.ToInt32(ReadListDT.Rows[j]["UserID"]) == CurrentUserID)
                            {
                                iReadButtonLeft = 10;
                                iReadButtonTop = iCurTextPosY + iMarginToAttributeData;
                                iReadButtonWidth = Convert.ToInt32(e.Graphics.MeasureString("ознакомлен", fAttributeItemFont).Width);
                                iReadButtonHeight = Convert.ToInt32(e.Graphics.MeasureString("ознакомлен", fAttributeItemFont).Height);

                                bReadButtonVisible = true;

                                e.Graphics.DrawString("ознакомлен", fAttributeItemFont, brAttributeButtonBrush, 10, iCurTextPosY + iMarginToAttributeData);
                                iCurTextPosY += Convert.ToInt32(e.Graphics.MeasureString("ознакомлен", fAttributeItemFont).Height) + iMarginToAttributeData;
                            }
                            else
                            {
                                e.Graphics.DrawString("нет", fAttributeItemFont, brAttributeValueNotSetBrush, 10, iCurTextPosY + iMarginToAttributeData);
                                iCurTextPosY += Convert.ToInt32(e.Graphics.MeasureString("нет", fAttributeItemFont).Height) + iMarginToAttributeData;
                            }

                        }

                    }
                }



                if (AttributesDataTable.Rows[i]["AttributeName"].ToString() == "Бумага")
                {
                    if (Convert.ToBoolean(AttributesDataTable.Rows[i]["Value"]) == false)
                        e.Graphics.DrawString("нет", fAttributeItemFont, brAttributeValueNotSetBrush,
                                            10, iCurTextPosY + iMarginToAttributeName);
                    else
                        e.Graphics.DrawString("да", fAttributeItemFont, brAttributeValueSetBrush,
                                            10, iCurTextPosY + iMarginToAttributeName);

                    iCurTextPosY += Convert.ToInt32(e.Graphics.MeasureString("данет",
                                                    fAttributeItemFont).Height) + iMarginToAttributeName;
                }
            }

            if (ScrollContainer.Height < iCurTextPosY)
                ScrollContainer.Height = iCurTextPosY;
            else
                ScrollContainer.Height = this.Height;

            if (VerticalScroll.TotalControlHeight != ScrollContainer.Height)
            {
                VerticalScroll.TotalControlHeight = ScrollContainer.Height;
                VerticalScroll.Refresh();
            }
        }


        private void OnScrollContainerSizeChanged(object sender, EventArgs e)
        {
            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
        }

        private void OnScrollPositionChanged(object sender, int tOffset)
        {
            Offset = tOffset;
            ScrollContainer.Top = -Offset;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (!VerticalScroll.Visible)
                return;

            if (Offset == 0)
                if (iTempScrollWheelOffset > 0)
                    iTempScrollWheelOffset = 0;

            if (e.Delta < 0)
            {
                if (Offset < ScrollContainer.Height - this.Height)
                {
                    if (Offset + VerticalScroll.ScrollWheelOffset + this.Height > ScrollContainer.Height)//last
                    {
                        iTempScrollWheelOffset = ScrollContainer.Height - this.Height - Offset;
                        Offset += iTempScrollWheelOffset;
                    }
                    else
                    {
                        Offset += VerticalScroll.ScrollWheelOffset;
                    }

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
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
                        Offset -= VerticalScroll.ScrollWheelOffset;

                    if (Offset < 0)
                        Offset = 0;

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
                }
        }


        private void OnVerticalScrollVisibleChanged(object sender, EventArgs e)
        {
            if (VerticalScroll.Visible)
                ScrollContainer.Width = this.Width - VerticalScroll.Width;
            else
                ScrollContainer.Width = this.Width;
        }

        private void OnScrollContainerMouseMove(object sender, MouseEventArgs e)
        {
            if (bSignButtonVisible)
            {
                if (e.X >= iSignButtonLeft && e.X <= iSignButtonLeft + iSignButtonWidth && e.Y >= iSignButtonTop && e.Y <= iSignButtonTop + iSignButtonHeight)
                {
                    if (!bSignButtonTrack)
                    {
                        bSignButtonTrack = true;
                        this.Cursor = Cursors.Hand;
                    }
                }
                else
                {
                    if (bSignButtonTrack)
                    {
                        bSignButtonTrack = false;
                        this.Cursor = Cursors.Default;
                    }
                }
            }
            else
            {
                if (bSignButtonTrack)
                {
                    bSignButtonTrack = false;
                    this.Cursor = Cursors.Default;
                }
            }

            if (bReadButtonVisible)
            {
                if (e.X >= iReadButtonLeft && e.X <= iReadButtonLeft + iReadButtonWidth && e.Y >= iReadButtonTop && e.Y <= iReadButtonTop + iReadButtonHeight)
                {
                    if (!bReadButtonTrack)
                    {
                        bReadButtonTrack = true;
                        this.Cursor = Cursors.Hand;
                    }
                }
                else
                {
                    if (bReadButtonTrack)
                    {
                        bReadButtonTrack = false;
                        this.Cursor = Cursors.Default;
                    }
                }
            }
            else
                if (bReadButtonTrack)
            {
                bReadButtonTrack = false;
                this.Cursor = Cursors.Default;
            }
        }

        private void OnScrollContainerMouseLeave(object sender, EventArgs e)
        {
            base.OnMouseLeave(e);

            if (bSignButtonTrack)
            {
                bSignButtonTrack = false;
                this.Cursor = Cursors.Default;
            }

            if (bReadButtonTrack)
            {
                bReadButtonTrack = false;
                this.Cursor = Cursors.Default;
            }
        }

        private void OnScrollContainerClick(object sender, EventArgs e)
        {
            if (bSignButtonVisible && bSignButtonTrack)
                OnSignButtonClicked(sender, CurrentUserID, FileID);

            if (bReadButtonVisible && bReadButtonTrack)
                OnReadButtonClicked(sender, CurrentUserID, FileID);
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            ScrollContainer.Focus();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            SetScrollHeight();

            VerticalScroll.Left = this.Width - VerticalScroll.Width;
        }


        public delegate void OnSignButtonClickedEventHandler(object sender, int UserID, int FileID);

        public event OnSignButtonClickedEventHandler SignButtonClicked;
        public event OnSignButtonClickedEventHandler ReadButtonClicked;

        public virtual void OnSignButtonClicked(object sender, int UserID, int FileID)
        {
            SignButtonClicked?.Invoke(sender, UserID, FileID);
        }

        public virtual void OnReadButtonClicked(object sender, int UserID, int FileID)
        {
            ReadButtonClicked?.Invoke(sender, UserID, FileID);
        }
    }






    public class InfiniumFilesPermissionsUsersList : Control
    {
        public int Offset = 0;

        int iTempScrollWheelOffset = 0;

        public InfiniumScrollContainer ScrollContainer;
        public InfiniumProjectsVerticalScrollBar VerticalScroll;

        public DataTable DepartmentsDataTable;
        public DataTable UsersDataTable;

        public DataTable UsersItemsDT;
        public DataTable DepsItemsDT;

        int iMarginToNextUser = 2;
        int iMarginToUser = 2;
        int iLeftMargin = 20;

        public bool bAdminOnly = false;
        public bool bAllUsers = false;

        public int CurrentUserID = -1;

        Color cAttributeCaptionFontColor = Color.FromArgb(140, 140, 140);
        Color cAttributeGroupFontColor = Color.FromArgb(56, 184, 238);
        Color cAttributeItemFontColor = Color.FromArgb(140, 140, 140);

        Font fAttributeCaptionFont = new Font("Segoe UI Semibold", 16.0f, FontStyle.Regular, GraphicsUnit.Pixel);
        Font fAttributeGroupFont = new Font("Segoe UI Semibold", 16.0f, FontStyle.Regular, GraphicsUnit.Pixel);
        Font fAttributeItemFont = new Font("Segoe UI", 14.0f, FontStyle.Regular, GraphicsUnit.Pixel);

        SolidBrush brAttributeCaptionFontBrush;
        SolidBrush brAttributeGroupFontBrush;
        SolidBrush brAttributeItemFontBrush;

        public InfiniumFilesPermissionsUsersList()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);

            brAttributeCaptionFontBrush = new SolidBrush(cAttributeCaptionFontColor);
            brAttributeGroupFontBrush = new SolidBrush(cAttributeGroupFontColor);
            brAttributeItemFontBrush = new SolidBrush(cAttributeItemFontColor);

            VerticalScroll = new InfiniumProjectsVerticalScrollBar()
            {
                Parent = this,
                Height = this.Height
            };
            VerticalScroll.Left = this.Width - VerticalScroll.Width;
            VerticalScroll.TotalControlHeight = this.Height;
            VerticalScroll.ScrollPositionChanged += OnScrollPositionChanged;

            ScrollContainer = new InfiniumScrollContainer()
            {
                Parent = this,
                Top = 0,
                Left = 0,
                Width = this.Width,
                Height = this.Height,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            ScrollContainer.SizeChanged += OnScrollContainerSizeChanged;
            ScrollContainer.Paint += OnPaintScrollContainer;
        }

        public void InitializeItems()
        {
            ScrollContainer.Refresh();
        }


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public InfiniumProjectsVerticalScrollBar VerticalScrollBar
        {
            get { return VerticalScroll; }
            set { VerticalScroll = value; VerticalScroll.Left = this.Width - VerticalScroll.Width; }
        }


        private void OnPaintScrollContainer(object sender, PaintEventArgs e)
        {
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            int iCurTextPosY = 0;

            if (DepsItemsDT == null)
                return;

            if (UsersItemsDT == null)
                return;


            e.Graphics.DrawString("Права доступа", fAttributeCaptionFont, brAttributeCaptionFontBrush, 0, iCurTextPosY);
            iCurTextPosY += Convert.ToInt32(e.Graphics.MeasureString("Права доступа", fAttributeCaptionFont).Height) + iMarginToUser + 6;


            if (bAllUsers)
            {
                e.Graphics.DrawString("Открытый доступ", fAttributeGroupFont, brAttributeGroupFontBrush, iLeftMargin, iCurTextPosY);
                iCurTextPosY += Convert.ToInt32(e.Graphics.MeasureString("Открытый доступ", fAttributeGroupFont).Height) + iMarginToUser;
            }

            if (bAdminOnly)
            {
                e.Graphics.DrawString("Только администратор", fAttributeGroupFont, brAttributeGroupFontBrush, iLeftMargin, iCurTextPosY);
                iCurTextPosY += Convert.ToInt32(e.Graphics.MeasureString("Только администратор", fAttributeGroupFont).Height) + iMarginToUser;
            }

            if (DepsItemsDT.Rows.Count > 0)
            {
                e.Graphics.DrawString("Отделы", fAttributeGroupFont, brAttributeGroupFontBrush, iLeftMargin, iCurTextPosY);
                iCurTextPosY += Convert.ToInt32(e.Graphics.MeasureString("Отделы", fAttributeGroupFont).Height) + iMarginToUser;
            }

            for (int i = 0; i < DepsItemsDT.Rows.Count; i++)
            {
                string sDep = DepartmentsDataTable.Select("DepartmentID = " +
                                      DepsItemsDT.Rows[i]["DepartmentID"])[0]["DepartmentName"].ToString();
                e.Graphics.DrawString(sDep, fAttributeItemFont, brAttributeItemFontBrush, iLeftMargin + 2, iCurTextPosY);
                iCurTextPosY += Convert.ToInt32(e.Graphics.MeasureString(sDep, fAttributeItemFont).Height) + iMarginToNextUser;
            }

            if (UsersItemsDT.Rows.Count > 0)
            {
                e.Graphics.DrawString("Сотрудники", fAttributeGroupFont, brAttributeGroupFontBrush, iLeftMargin, iCurTextPosY);
                iCurTextPosY += Convert.ToInt32(e.Graphics.MeasureString("Сотрудники", fAttributeGroupFont).Height) + iMarginToUser;
            }

            for (int i = 0; i < UsersItemsDT.Rows.Count; i++)
            {
                string sUs = UsersDataTable.Select("UserID = " +
                                          UsersItemsDT.Rows[i]["UserID"])[0]["Name"].ToString();
                e.Graphics.DrawString(sUs, fAttributeItemFont, brAttributeItemFontBrush, iLeftMargin + 2, iCurTextPosY);
                iCurTextPosY += Convert.ToInt32(e.Graphics.MeasureString(sUs, fAttributeItemFont).Height) + iMarginToNextUser;
            }


            if (ScrollContainer.Height < iCurTextPosY)
                ScrollContainer.Height = iCurTextPosY;
            else
                ScrollContainer.Height = this.Height;

            if (VerticalScroll.TotalControlHeight != ScrollContainer.Height)
            {
                VerticalScroll.TotalControlHeight = ScrollContainer.Height;
                VerticalScroll.Refresh();
            }
        }


        private void OnScrollContainerSizeChanged(object sender, EventArgs e)
        {
            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
        }

        private void OnScrollPositionChanged(object sender, int tOffset)
        {
            Offset = tOffset;
            ScrollContainer.Top = -Offset;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (!VerticalScroll.Visible)
                return;

            if (Offset == 0)
                if (iTempScrollWheelOffset > 0)
                    iTempScrollWheelOffset = 0;

            if (e.Delta < 0)
            {
                if (Offset < ScrollContainer.Height - this.Height)
                {
                    if (Offset + VerticalScroll.ScrollWheelOffset + this.Height > ScrollContainer.Height)//last
                    {
                        iTempScrollWheelOffset = ScrollContainer.Height - this.Height - Offset;
                        Offset += iTempScrollWheelOffset;
                    }
                    else
                    {
                        Offset += VerticalScroll.ScrollWheelOffset;
                    }

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
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
                        Offset -= VerticalScroll.ScrollWheelOffset;

                    if (Offset < 0)
                        Offset = 0;

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
                }
        }


        private void OnVerticalScrollVisibleChanged(object sender, EventArgs e)
        {
            if (VerticalScroll.Visible)
                ScrollContainer.Width = this.Width - VerticalScroll.Width;
            else
                ScrollContainer.Width = this.Width;
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            ScrollContainer.Focus();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            VerticalScroll.Left = this.Width - VerticalScroll.Width;
        }

    }






    public class InfiniumLightButton : Control
    {
        Bitmap bImage;
        Bitmap bDisImage;
        string sCaption = "";

        bool bTracking = false;
        bool bDisabled = false;

        SolidBrush brFontBrush;

        Color cBorderColor = Color.FromArgb(0, 205, 252);
        Color cBackColor = Color.Transparent;
        Color cTrackBackColor = Color.White;

        Pen pBorderPen;

        Rectangle rBorderRect;

        ToolTip ToolTip;

        string sToolTipText = "";

        int iTextYMargin = 0;

        public InfiniumLightButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            this.BackColor = Color.Transparent;
            cBackColor = this.BackColor;

            ToolTip = new System.Windows.Forms.ToolTip();
            ToolTip.SetToolTip(this, sToolTipText);

            brFontBrush = new SolidBrush(this.ForeColor);

            pBorderPen = new Pen(new SolidBrush(cBorderColor));

            rBorderRect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
        }


        public Bitmap Image
        {
            get { return bImage; }
            set { bImage = value; this.Refresh(); }
        }

        public Bitmap DisabledImage
        {
            get { return bDisImage; }
            set { bDisImage = value; this.Refresh(); }
        }

        public bool Disabled
        {
            get { return bDisabled; }
            set { bDisabled = value; if (value) Cursor = Cursors.Default; else Cursor = Cursors.Hand; }
        }

        public string Caption
        {
            get { return sCaption; }
            set { sCaption = value; this.Refresh(); }
        }

        public string ToolTipText
        {
            get { return sToolTipText; }
            set { sToolTipText = value; ToolTip.SetToolTip(this, value); }
        }

        public Color BorderColor
        {
            get { return cBorderColor; }
            set { cBorderColor = value; pBorderPen.Color = value; this.Refresh(); }
        }

        public Color BackTrackColor
        {
            get { return cTrackBackColor; }
            set { cTrackBackColor = value; }
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
                brFontBrush.Color = value;
                this.Refresh();
            }
        }

        public int TextYMargin
        {
            get { return iTextYMargin; }
            set { iTextYMargin = value; this.Refresh(); }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (Image == null)
                return;

            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            Image.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);

            if (bDisImage != null)
                bDisImage.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);

            if (sCaption.Length > 0)
            {
                if (bDisabled && bDisImage != null)
                    e.Graphics.DrawImage(bDisImage, 4, (this.Height - Image.Height) / 2);
                else
                    e.Graphics.DrawImage(Image, 4, (this.Height - Image.Height) / 2);
            }
            else
            {
                if (bDisabled && bDisImage != null)
                    e.Graphics.DrawImage(bDisImage, (this.Width - Image.Width) / 2, (this.Height - Image.Height) / 2);
                else
                    e.Graphics.DrawImage(Image, (this.Width - Image.Width) / 2, (this.Height - Image.Height) / 2);
            }

            if (sCaption.Length > 0)
                e.Graphics.DrawString(sCaption, this.Font, brFontBrush, 7 + Image.Width + 2, (this.Height - e.Graphics.MeasureString(sCaption, this.Font).Height) / 2 + iTextYMargin);

            if (bTracking)
                e.Graphics.DrawRectangle(pBorderPen, rBorderRect);

            //if (this.Width != 7 + Image.Width + 2 + e.Graphics.MeasureString(sCaption, this.Font).Width + 5)
            //    this.Width = 7 + Image.Width + 2 + Convert.ToInt32(e.Graphics.MeasureString(sCaption, this.Font).Width) + 5;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!bTracking)
            {
                bTracking = true;
                this.BackColor = cTrackBackColor;
                this.Refresh();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (bTracking)
            {
                bTracking = false;
                this.BackColor = cBackColor;
                this.Refresh();
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            rBorderRect.Width = this.Width - 1;
            rBorderRect.Height = this.Height - 1;
        }
    }









    //pictures

    public class InfiniumPicturesAlbumItem : Control
    {
        Color cBorderColor = Color.FromArgb(90, 90, 90);

        Color cHeaderFontColor = Color.White;
        Color cAuthorFontColor = Color.FromArgb(56, 184, 238);
        Color cDateFontColor = Color.FromArgb(120, 120, 120);
        Color cLikesAndCommentsFontColor = Color.FromArgb(120, 120, 120);
        Color cBottomPanelBackColor = Color.FromArgb(225, 225, 225);
        Color cHeaderBackColor = Color.FromArgb(56, 184, 238);
        Color cSamplesBack = Color.FromArgb(250, 250, 250);

        Color cShadowColor0 = Color.FromArgb(175, 175, 175);
        Color cShadowColor1 = Color.FromArgb(185, 185, 185);
        Color cShadowColor2 = Color.FromArgb(200, 200, 200);
        Color cShadowColor3 = Color.FromArgb(215, 215, 215);
        Color cShadowColor4 = Color.FromArgb(230, 230, 230);
        Color cShadowColor5 = Color.FromArgb(250, 250, 250);

        Font fHeaderFont = new Font("Segoe UI", 16.0f, FontStyle.Regular, GraphicsUnit.Pixel);
        Font fAuthorFont = new Font("Segoe UI", 13.0f, FontStyle.Regular, GraphicsUnit.Pixel);
        Font fDateFont = new Font("Segoe UI", 13.0f, FontStyle.Regular, GraphicsUnit.Pixel);
        Font fLikesAndCommentsFont = new Font("Segoe UI", 15.0f, FontStyle.Regular, GraphicsUnit.Pixel);

        SolidBrush brHeaderFontBrush;
        SolidBrush brAuthorFontBrush;
        SolidBrush brDateFontBrush;
        SolidBrush brLikesAndCommentsFontBrush;
        SolidBrush brBottomPanelBrush;
        SolidBrush brHeaderBackBrush;
        SolidBrush brSamplesBack;

        Pen pBorderPen;
        Pen pShadowPen0;
        Pen pShadowPen1;
        Pen pShadowPen2;
        Pen pShadowPen3;
        Pen pShadowPen4;
        Pen pShadowPen5;

        Bitmap imAuthorPhoto;

        public Bitmap BigSample;
        public Bitmap SmallSample1;
        public Bitmap SmallSample2;
        public Bitmap SmallSample3;

        public Bitmap ActiveUser1;
        public Bitmap ActiveUser2;
        public Bitmap ActiveUser3;
        public Bitmap ActiveUser4;

        public Bitmap LikeActiveBMP = Properties.Resources.LikeAlbumActive;
        public Bitmap LikeInactiveBMP = Properties.Resources.LikeAlbumInactive;

        int iAuthorPictureHeight = 48;
        int iAuthorPictureWidth = 52;

        int iBigSampleHeight = 204;
        int iBigSampleWidth = 306;

        int iSmallSampleHeight = 64;
        int iSmallSampleWidth = 97;

        int iUsersHeight = 30;
        int iUsersWidth = 32;

        int iMarginLeft = 22;

        int iLikesCount = 0;

        int iInitialLeft = 0;
        int iInitialTop = 0;

        bool bTrack = false;

        int iAlbumID = -1;

        Rectangle rBorderRect;
        Rectangle rBottomPanelRect;
        Rectangle rHeaderRect;
        Rectangle rBigSampleRect;
        Rectangle rSmallSampleRect;

        string sHeader = "";
        int iAuthorID = -1;
        string sAuthorName = "";
        string sDateTime = "";

        public InfiniumPicturesAlbumItem()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);

            brAuthorFontBrush = new SolidBrush(cAuthorFontColor);
            brDateFontBrush = new SolidBrush(cDateFontColor);
            brHeaderFontBrush = new SolidBrush(cHeaderFontColor);
            brLikesAndCommentsFontBrush = new SolidBrush(cLikesAndCommentsFontColor);
            brBottomPanelBrush = new SolidBrush(cBottomPanelBackColor);
            brHeaderBackBrush = new SolidBrush(cHeaderBackColor);
            brSamplesBack = new SolidBrush(cSamplesBack);

            pBorderPen = new Pen(new SolidBrush(cBorderColor));
            pShadowPen0 = new Pen(new SolidBrush(cShadowColor0));
            pShadowPen1 = new Pen(new SolidBrush(cShadowColor1));
            pShadowPen2 = new Pen(new SolidBrush(cShadowColor2));
            pShadowPen3 = new Pen(new SolidBrush(cShadowColor3));
            pShadowPen4 = new Pen(new SolidBrush(cShadowColor4));
            pShadowPen5 = new Pen(new SolidBrush(cShadowColor5));

            rBorderRect = new Rectangle(0, 0, 0, 0);
            rBottomPanelRect = new Rectangle(0, 0, 0, 0);
            rHeaderRect = new Rectangle(0, 0, 0, 0);
            rBigSampleRect = new Rectangle(0, 0, iBigSampleWidth, iBigSampleHeight);
            rSmallSampleRect = new Rectangle(0, 0, iSmallSampleWidth, iSmallSampleHeight);

            this.Cursor = Cursors.Hand;

            this.Width = 453;
            this.Height = 370;
        }


        public string Header
        {
            get { return sHeader; }
            set { sHeader = value; }
        }

        public string DateTime
        {
            get { return sDateTime; }
            set { sDateTime = value; }
        }

        public string AuthorName
        {
            get { return sAuthorName; }
            set { sAuthorName = value; }
        }

        public int AuthorID
        {
            get { return iAuthorID; }
            set { iAuthorID = value; }
        }

        public int LikesCount
        {
            get { return iLikesCount; }
            set { iLikesCount = value; }
        }


        public int AlbumID
        {
            get { return iAlbumID; }
            set { iAlbumID = value; }
        }


        public Bitmap AuthorPhoto
        {
            get { return imAuthorPhoto; }
            set { imAuthorPhoto = value; }
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            e.Graphics.FillRectangle(brHeaderBackBrush, rHeaderRect);

            if (e.Graphics.MeasureString(sHeader, fHeaderFont).Width > (this.Width - iMarginLeft - iMarginLeft))
            {
                for (int i = sHeader.Length; i >= 0; i--)
                {
                    if (e.Graphics.MeasureString(sHeader.Substring(0, i), fHeaderFont).Width <= (this.Width - iMarginLeft - iMarginLeft))
                    {
                        sHeader = sHeader.Substring(0, i - 1) + "...";
                        break;
                    }
                }
            }

            if (sHeader.Length > 0)
                e.Graphics.DrawString(sHeader, fHeaderFont, brHeaderFontBrush, iMarginLeft,
                                      (rHeaderRect.Height - e.Graphics.MeasureString(sHeader, fHeaderFont).Height) / 2 + 2);

            e.Graphics.DrawRectangle(pBorderPen, rBorderRect);

            //big sample
            rBigSampleRect.X = iMarginLeft;
            rBigSampleRect.Y = rHeaderRect.Height + 20;

            e.Graphics.FillRectangle(brSamplesBack, rBigSampleRect);

            e.Graphics.DrawImage(BigSample, iMarginLeft + ((iBigSampleWidth - BigSample.Width) / 2), rHeaderRect.Height + 20 +
                                            ((iBigSampleHeight - BigSample.Height) / 2));

            //small sample1
            rSmallSampleRect.X = iMarginLeft + rBigSampleRect.Width + 6;
            rSmallSampleRect.Y = rHeaderRect.Height + 20;

            e.Graphics.FillRectangle(brSamplesBack, rSmallSampleRect);

            e.Graphics.DrawImage(SmallSample1, iMarginLeft + rBigSampleRect.Width + 6 + ((iSmallSampleWidth - SmallSample1.Width) / 2), rHeaderRect.Height + 20 +
                                            ((iSmallSampleHeight - SmallSample1.Height) / 2));



            //small sample2
            rSmallSampleRect.X = iMarginLeft + rBigSampleRect.Width + 6;
            rSmallSampleRect.Y = rHeaderRect.Height + 20 + rSmallSampleRect.Height + 6;

            e.Graphics.FillRectangle(brSamplesBack, rSmallSampleRect);

            e.Graphics.DrawImage(SmallSample2, iMarginLeft + rBigSampleRect.Width + 6 + ((iSmallSampleWidth - SmallSample2.Width) / 2),
                                 rHeaderRect.Height + 20 + rSmallSampleRect.Height + 6 + ((iSmallSampleHeight - SmallSample2.Height) / 2));

            //small sample3
            rSmallSampleRect.X = iMarginLeft + rBigSampleRect.Width + 6;
            rSmallSampleRect.Y = rHeaderRect.Height + 20 + (rSmallSampleRect.Height + 6) * 2;

            e.Graphics.FillRectangle(brSamplesBack, rSmallSampleRect);

            e.Graphics.DrawImage(SmallSample3, iMarginLeft + rBigSampleRect.Width + 6 + ((iSmallSampleWidth - SmallSample3.Width) / 2),
                                 rHeaderRect.Height + 20 + (rSmallSampleRect.Height + 6) * 2 + ((iSmallSampleHeight - SmallSample3.Height) / 2));

            //author and date
            if (imAuthorPhoto != null)
            {
                imAuthorPhoto.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);

                e.Graphics.DrawImage(imAuthorPhoto, iMarginLeft, rHeaderRect.Height + 20 + iBigSampleHeight + 6, iAuthorPictureWidth, iAuthorPictureHeight);
            }


            if (sAuthorName.Length > 0)
                e.Graphics.DrawString(sAuthorName, fAuthorFont, brAuthorFontBrush, iMarginLeft + iAuthorPictureWidth + 3,
                                      rHeaderRect.Height + 20 + iBigSampleHeight + 6 - 2);

            if (sDateTime.Length > 0)
                e.Graphics.DrawString(sDateTime, fDateFont, brDateFontBrush, iMarginLeft + iAuthorPictureWidth + 3,
                                      rHeaderRect.Height + 20 + iBigSampleHeight + 6 - 2 +
                                      e.Graphics.MeasureString(sDateTime, fDateFont).Height - 2);

            e.Graphics.FillRectangle(brBottomPanelBrush, rBottomPanelRect);


            //likes and comments
            if (iLikesCount > 0)
            {
                LikeActiveBMP.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);

                e.Graphics.DrawImage(LikeActiveBMP, iMarginLeft, rBottomPanelRect.Y + (rBottomPanelRect.Height - LikeActiveBMP.Height) / 2 + 1);
            }
            else
            {
                LikeInactiveBMP.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);

                e.Graphics.DrawImage(LikeInactiveBMP, iMarginLeft, rBottomPanelRect.Y + (rBottomPanelRect.Height - LikeInactiveBMP.Height) / 2 + 1);
            }

            if (iLikesCount > 0)
                e.Graphics.DrawString(iLikesCount.ToString(), fLikesAndCommentsFont, brLikesAndCommentsFontBrush,
                                      iMarginLeft + LikeActiveBMP.Width + 4,
                                      rBottomPanelRect.Y + (rBottomPanelRect.Height - LikeInactiveBMP.Height) / 2 - 1);



            //ActiveUsers
            if (ActiveUser4 != null)
            {
                ActiveUser4.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);

                e.Graphics.DrawImage(ActiveUser4, this.Width - iMarginLeft - iUsersWidth + 4,
                                     rBottomPanelRect.Y + (rBottomPanelRect.Height - iUsersHeight) / 2, iUsersWidth, iUsersHeight);
            }

            if (ActiveUser3 != null)
            {
                ActiveUser3.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);

                int iLeft = 0;

                if (ActiveUser4 != null)
                    iLeft += iUsersWidth + 4;

                if (ActiveUser3 != null)
                    iLeft += iUsersWidth + 4;

                e.Graphics.DrawImage(ActiveUser3, this.Width - iMarginLeft - iLeft + 4,
                                     rBottomPanelRect.Y + (rBottomPanelRect.Height - iUsersHeight) / 2, iUsersWidth, iUsersHeight);
            }

            if (ActiveUser2 != null)
            {
                ActiveUser2.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);

                int iLeft = 0;

                if (ActiveUser4 != null)
                    iLeft += iUsersWidth + 4;

                if (ActiveUser3 != null)
                    iLeft += iUsersWidth + 4;

                if (ActiveUser2 != null)
                    iLeft += iUsersWidth + 4;

                e.Graphics.DrawImage(ActiveUser2, this.Width - iMarginLeft - iLeft + 4,
                                     rBottomPanelRect.Y + (rBottomPanelRect.Height - iUsersHeight) / 2, iUsersWidth, iUsersHeight);
            }

            if (ActiveUser1 != null)
            {
                ActiveUser1.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);

                int iLeft = 0;

                if (ActiveUser4 != null)
                    iLeft += iUsersWidth + 4;

                if (ActiveUser3 != null)
                    iLeft += iUsersWidth + 4;

                if (ActiveUser2 != null)
                    iLeft += iUsersWidth + 4;

                if (ActiveUser1 != null)
                    iLeft += iUsersWidth + 4;

                e.Graphics.DrawImage(ActiveUser1, this.Width - iMarginLeft - iLeft + 4,
                                    rBottomPanelRect.Y + (rBottomPanelRect.Height - iUsersHeight) / 2, iUsersWidth, iUsersHeight);
            }

            //if(!bTrack)
            DrawShadow(e.Graphics, bTrack);
        }


        private void DrawShadow(Graphics G, bool bUp)
        {


            int iM = 1;

            if (!bUp)
                iM = 0;

            //right
            if (bUp)
                G.DrawLine(pShadowPen0, this.Width - 5, 9 + iM, this.Width - 5, this.Height - 5);
            G.DrawLine(pShadowPen1, this.Width - 5 + iM, 9 + iM, this.Width - 5 + iM, this.Height - 5 + iM);
            G.DrawLine(pShadowPen2, this.Width - 4 + iM, 9 + iM, this.Width - 4 + iM, this.Height - 4 + iM);
            G.DrawLine(pShadowPen3, this.Width - 3 + iM, 9 + iM, this.Width - 3 + iM, this.Height - 3 + iM);
            G.DrawLine(pShadowPen4, this.Width - 2 + iM, 9 + iM, this.Width - 2 + iM, this.Height - 2 + iM);
            G.DrawLine(pShadowPen5, this.Width - 1 + iM, 9 + iM, this.Width - 1 + iM, this.Height - 1 + iM);

            //bottom
            if (bUp)
                G.DrawLine(pShadowPen0, 9 + iM, this.Height - 5, this.Width - 5 + iM, this.Height - 5);
            G.DrawLine(pShadowPen1, 9 + iM, this.Height - 5 + iM, this.Width - 5 + iM, this.Height - 5 + iM);
            G.DrawLine(pShadowPen2, 9 + iM, this.Height - 4 + iM, this.Width - 4 + iM, this.Height - 4 + iM);
            G.DrawLine(pShadowPen3, 9 + iM, this.Height - 3 + iM, this.Width - 3 + iM, this.Height - 3 + iM);
            G.DrawLine(pShadowPen4, 9 + iM, this.Height - 2 + iM, this.Width - 2 + iM, this.Height - 2 + iM);
            G.DrawLine(pShadowPen5, 9 + iM, this.Height - 1 + iM, this.Width - 1 + iM, this.Height - 1 + iM);
        }


        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            rBorderRect.Width = this.Width - 6;
            rBorderRect.Height = this.Height - 6;
            rBorderRect.Y = 0;
            rBorderRect.X = 0;

            rBottomPanelRect.Width = this.Width - 7;
            rBottomPanelRect.Height = 40;
            rBottomPanelRect.Y = this.Height - 6 - 40;
            rBottomPanelRect.X = 1;

            rHeaderRect.Width = this.Width - 7;
            rHeaderRect.Height = 40;
            rHeaderRect.Y = 1;
            rHeaderRect.X = 1;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!bTrack)
            {
                bTrack = true;

                if (iInitialLeft == 0)
                {
                    iInitialLeft = this.Left;
                    iInitialTop = this.Top;
                }

                this.Left = iInitialLeft - 1;
                this.Top = iInitialTop - 1;
                this.Refresh();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (bTrack)
            {
                bTrack = false;
                this.Left = iInitialLeft;
                this.Top = iInitialTop;
                this.Refresh();
            }
        }
    }





    public class InfiniumPicturesAlbums : Control
    {
        public int Offset = 0;

        int iTempScrollWheelOffset = 0;

        int iMarginToNextItem = 40;
        int iItemWidth = 453;
        int iItemHeight = 370;

        int iRowsCount = 0;

        public InfiniumScrollContainer ScrollContainer;
        public InfiniumProjectsVerticalScrollBar VerticalScroll;

        public DataTable ItemsDT;

        public InfiniumPicturesAlbumItem[] AlbumItems;

        public InfiniumPicturesAlbums()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);

            VerticalScroll = new InfiniumProjectsVerticalScrollBar()
            {
                Parent = this,
                Height = this.Height
            };
            VerticalScroll.Left = this.Width - VerticalScroll.Width;
            VerticalScroll.TotalControlHeight = this.Height;
            VerticalScroll.ScrollPositionChanged += OnScrollPositionChanged;

            ScrollContainer = new InfiniumScrollContainer()
            {
                Parent = this,
                Top = 0,
                Left = 0,
                Width = this.Width,
                Height = this.Height,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            ScrollContainer.SizeChanged += OnScrollContainerSizeChanged;
        }



        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public InfiniumProjectsVerticalScrollBar VerticalScrollBar
        {
            get { return VerticalScroll; }
            set { VerticalScroll = value; VerticalScroll.Left = this.Width - VerticalScroll.Width; }
        }

        public DataTable ItemsDataTable
        {
            get { return ItemsDT; }
            set
            {
                ItemsDT = value;
            }
        }


        public void InitializeItems()
        {
            if (AlbumItems != null)
            {
                for (int i = 0; i < AlbumItems.Count(); i++)
                {
                    if (AlbumItems[i] != null)
                    {
                        AlbumItems[i].Dispose();
                        AlbumItems[i] = null;
                    }
                }

                AlbumItems = new InfiniumPicturesAlbumItem[0];
            }

            Offset = 0;
            ScrollContainer.Top = -Offset;
            VerticalScroll.Offset = Offset;
            VerticalScroll.Refresh();

            if (ItemsDT == null)
                return;

            int iMaxCount = GetMaxCountInRow();

            int R = (this.Width - ((iMaxCount * iItemWidth) + ((iMaxCount - 1) * iMarginToNextItem))) / 2;

            if (ItemsDT.DefaultView.Count > 0)
            {
                AlbumItems = new InfiniumPicturesAlbumItem[ItemsDT.DefaultView.Count];

                int iCurRow = 0;
                int iCurItem = 0;

                for (int i = 0; i < ItemsDT.DefaultView.Count; i++)
                {
                    AlbumItems[i] = new InfiniumPicturesAlbumItem()
                    {
                        Parent = ScrollContainer
                    };
                    AlbumItems[i].Top = iCurRow * (AlbumItems[i].Height + iMarginToNextItem) + iMarginToNextItem;
                    AlbumItems[i].Left = iCurItem * (AlbumItems[i].Width + iMarginToNextItem) + R;
                    AlbumItems[i].Header = ItemsDT.DefaultView[i]["AlbumName"].ToString();
                    AlbumItems[i].AuthorName = ItemsDT.DefaultView[i]["AuthorName"].ToString();
                    AlbumItems[i].AuthorID = Convert.ToInt32(ItemsDT.DefaultView[i]["AuthorID"]);
                    AlbumItems[i].AlbumID = Convert.ToInt32(ItemsDT.DefaultView[i]["AlbumID"]);
                    AlbumItems[i].DateTime = ItemsDT.DefaultView[i]["DateTime"].ToString();
                    AlbumItems[i].LikesCount = Convert.ToInt32(ItemsDT.DefaultView[i]["LikesCount"]);
                    AlbumItems[i].Anchor = AnchorStyles.Left | AnchorStyles.Top;
                    AlbumItems[i].Click += OnItemClick;

                    byte[] b = (byte[])ItemsDT.DefaultView[i]["AuthorPhoto"];

                    using (MemoryStream ms = new MemoryStream(b))
                    {
                        AlbumItems[i].AuthorPhoto = (Bitmap)Image.FromStream(ms);
                    }


                    b = (byte[])ItemsDT.DefaultView[i]["BigSample"];

                    using (MemoryStream ms = new MemoryStream(b))
                    {
                        AlbumItems[i].BigSample = (Bitmap)Image.FromStream(ms);
                    }

                    b = (byte[])ItemsDT.DefaultView[i]["SmallSample1"];

                    using (MemoryStream ms = new MemoryStream(b))
                    {
                        AlbumItems[i].SmallSample1 = (Bitmap)Image.FromStream(ms);
                    }

                    b = (byte[])ItemsDT.DefaultView[i]["SmallSample2"];

                    using (MemoryStream ms = new MemoryStream(b))
                    {
                        AlbumItems[i].SmallSample2 = (Bitmap)Image.FromStream(ms);
                    }


                    b = (byte[])ItemsDT.DefaultView[i]["SmallSample3"];

                    using (MemoryStream ms = new MemoryStream(b))
                    {
                        AlbumItems[i].SmallSample3 = (Bitmap)Image.FromStream(ms);
                    }


                    if (ItemsDT.DefaultView[i]["ActiveUser1"] != DBNull.Value)
                    {
                        b = (byte[])ItemsDT.DefaultView[i]["ActiveUser1"];

                        using (MemoryStream ms = new MemoryStream(b))
                        {
                            AlbumItems[i].ActiveUser1 = (Bitmap)Image.FromStream(ms);
                        }
                    }


                    if (ItemsDT.DefaultView[i]["ActiveUser2"] != DBNull.Value)
                    {
                        b = (byte[])ItemsDT.DefaultView[i]["ActiveUser2"];

                        using (MemoryStream ms = new MemoryStream(b))
                        {
                            AlbumItems[i].ActiveUser2 = (Bitmap)Image.FromStream(ms);
                        }
                    }


                    if (ItemsDT.DefaultView[i]["ActiveUser3"] != DBNull.Value)
                    {
                        b = (byte[])ItemsDT.DefaultView[i]["ActiveUser3"];

                        using (MemoryStream ms = new MemoryStream(b))
                        {
                            AlbumItems[i].ActiveUser3 = (Bitmap)Image.FromStream(ms);
                        }
                    }


                    if (ItemsDT.DefaultView[i]["ActiveUser4"] != DBNull.Value)
                    {
                        b = (byte[])ItemsDT.DefaultView[i]["ActiveUser4"];

                        using (MemoryStream ms = new MemoryStream(b))
                        {
                            AlbumItems[i].ActiveUser4 = (Bitmap)Image.FromStream(ms);
                        }
                    }

                    if (iCurItem < iMaxCount - 1)
                        iCurItem++;
                    else
                    {
                        iCurRow++;
                        iCurItem = 0;
                    }
                }

                iRowsCount = iCurRow + 1;
            }

            GC.Collect();

            SetScrollHeight();
        }

        private int GetMaxCountInRow()
        {
            return (this.Width + iMarginToNextItem) / (iItemWidth + iMarginToNextItem);
        }

        private void SetScrollHeight()
        {
            if (AlbumItems == null || ItemsDataTable.DefaultView.Count == 0)
                return;

            if (iRowsCount * (iItemHeight + iMarginToNextItem) > this.Height)
                ScrollContainer.Height = iRowsCount * (iItemHeight + iMarginToNextItem);
            else
                ScrollContainer.Height = this.Height;

            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
            VerticalScroll.Refresh();
        }

        private void OnScrollContainerSizeChanged(object sender, EventArgs e)
        {
            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
        }

        private void OnScrollPositionChanged(object sender, int tOffset)
        {
            Offset = tOffset;
            ScrollContainer.Top = -Offset;
        }


        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (!VerticalScroll.Visible)
                return;

            if (Offset == 0)
                if (iTempScrollWheelOffset > 0)
                    iTempScrollWheelOffset = 0;

            if (e.Delta < 0)
            {
                if (Offset < ScrollContainer.Height - this.Height)
                {
                    if (Offset + VerticalScroll.ScrollWheelOffset + this.Height > ScrollContainer.Height)//last
                    {
                        iTempScrollWheelOffset = ScrollContainer.Height - this.Height - Offset;
                        Offset += iTempScrollWheelOffset;
                    }
                    else
                    {
                        Offset += VerticalScroll.ScrollWheelOffset;
                    }

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
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
                        Offset -= VerticalScroll.ScrollWheelOffset;

                    if (Offset < 0)
                        Offset = 0;

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
                }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            SetScrollHeight();

            VerticalScroll.Left = this.Width - VerticalScroll.Width;
        }


        private void OnItemClick(object sender, EventArgs e)
        {
            OnItemClicked(sender, ((InfiniumPicturesAlbumItem)sender).AlbumID);
        }

        public delegate void ItemClickedEventHandler(object sender, int AlbumID);


        public event ItemClickedEventHandler ItemClicked;


        public virtual void OnItemClicked(object sender, int AlbumID)
        {
            ItemClicked?.Invoke(sender, AlbumID);//Raise the event
        }

    }





    public class InfiniumPictureItem : Control
    {
        Color cLikesAndCommentsFontColor = Color.FromArgb(120, 120, 120);

        Font fLikesAndCommentsFont = new Font("Segoe UI", 15.0f, FontStyle.Regular, GraphicsUnit.Pixel);

        SolidBrush brLikesAndCommentsFontBrush;

        public Bitmap Image;

        public Bitmap LikeActiveBMP = Properties.Resources.LikePicActive;
        public Bitmap LikeInactiveBMP = Properties.Resources.LikePicIncative;

        int iLikesCount = 0;
        int iCommentsCount = 0;

        int iMarginForLikes = 30;

        int iPictureID = -1;
        int iAlbumID = -1;

        int iWidth = 364;
        int iHeight = 242 + 30;//242 + 30 for likes

        int iLikePicLeft = 0;
        int iLikePicTop = 0;
        bool bLikeTrack = false;
        bool bPicTrack = false;

        public InfiniumPictureItem()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            brLikesAndCommentsFontBrush = new SolidBrush(cLikesAndCommentsFontColor);

            this.Width = iWidth;
            this.Height = iHeight;
        }

        public int LikesCount
        {
            get { return iLikesCount; }
            set { iLikesCount = value; }
        }

        public int CommentsCount
        {
            get { return iCommentsCount; }
            set { iCommentsCount = value; }
        }

        public int PictureID
        {
            get { return iPictureID; }
            set { iPictureID = value; }
        }

        public int AlbumID
        {
            get { return iAlbumID; }
            set { iAlbumID = value; }
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            //e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            if (Image != null)
            {
                //int iNewHeight = iHeight;
                //int iNewWidth = Image.Width * iNewHeight / Image.Height;

                e.Graphics.DrawImage(Image, 0, 0);
            }

            if (iLikesCount > 0)
            {
                LikeActiveBMP.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);

                float W = e.Graphics.MeasureString(iLikesCount.ToString(), fLikesAndCommentsFont).Width;

                iLikePicTop = this.Height - iMarginForLikes + (iMarginForLikes - LikeActiveBMP.Height) / 2;

                e.Graphics.DrawImage(LikeActiveBMP, this.Width - (int)W - LikeActiveBMP.Width, iLikePicTop);

                iLikePicLeft = this.Width - (int)W - LikeActiveBMP.Width;

                e.Graphics.DrawString(iLikesCount.ToString(), fLikesAndCommentsFont, brLikesAndCommentsFontBrush, this.Width - W,
                                     this.Height - iMarginForLikes + (iMarginForLikes - e.Graphics.MeasureString(iLikesCount.ToString(), fLikesAndCommentsFont).Height) / 2);
            }
            else
            {
                LikeInactiveBMP.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);

                iLikePicTop = this.Height - iMarginForLikes + (iMarginForLikes - LikeActiveBMP.Height) / 2;

                e.Graphics.DrawImage(LikeInactiveBMP, this.Width - LikeActiveBMP.Width, iLikePicTop);

                iLikePicLeft = this.Width - LikeActiveBMP.Width;

            }

            if (this.Width > Image.Width)
                this.Width = Image.Width;
        }


        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.X >= iLikePicLeft && e.X <= iLikePicLeft + LikeActiveBMP.Width &&
                e.Y >= iLikePicTop && e.Y <= iLikePicTop + LikeActiveBMP.Height)
            {
                if (!bLikeTrack)
                {
                    this.Cursor = Cursors.Hand;
                    bLikeTrack = true;
                }
            }
            else
            {
                if (bLikeTrack)
                {
                    this.Cursor = Cursors.Default;
                    bLikeTrack = false;
                }
            }


            if (e.X >= 0 && e.X <= this.Width &&
                e.Y >= 0 && e.Y <= Image.Height)
            {
                if (!bPicTrack)
                {
                    this.Cursor = Cursors.Hand;
                    bPicTrack = true;
                }
            }
            else
            {
                if (bPicTrack)
                {
                    this.Cursor = Cursors.Default;
                    bPicTrack = false;
                }
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (bLikeTrack)
            {
                this.Cursor = Cursors.Default;
                bLikeTrack = false;
            }

            if (bPicTrack)
            {
                this.Cursor = Cursors.Default;
                bPicTrack = false;
            }

        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (bLikeTrack)
            {
                OnLikeClicked(this, iPictureID);
            }

            if (bPicTrack)
            {
                OnItemClicked(this, AlbumID, PictureID);
            }
        }

        public event LikeClickedEventHandler LikeClicked;
        public event ItemClickedEventHandler ItemClicked;

        public delegate void LikeClickedEventHandler(object sender, int PictureID);
        public delegate void ItemClickedEventHandler(object sender, int AlbumID, int PictureID);

        public virtual void OnLikeClicked(object sender, int PictureID)
        {
            LikeClicked?.Invoke(sender, PictureID);//Raise the event
        }

        public virtual void OnItemClicked(object sender, int AlbumID, int PictureID)
        {
            ItemClicked?.Invoke(sender, AlbumID, PictureID);//Raise the event
        }
    }





    public class InfiniumPicturesContainer : Control
    {
        public int Offset = 0;

        int iTempScrollWheelOffset = 0;

        int iMarginToNextItem = 40;
        int iItemHeight = 272;

        int iRowsCount = 0;

        public InfiniumScrollContainer ScrollContainer;
        public InfiniumProjectsVerticalScrollBar VerticalScroll;

        public DataTable ItemsDT;

        public InfiniumPictureItem[] PicItems;

        public InfiniumPicturesContainer()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);

            VerticalScroll = new InfiniumProjectsVerticalScrollBar()
            {
                Parent = this,
                Height = this.Height
            };
            VerticalScroll.Left = this.Width - VerticalScroll.Width;
            VerticalScroll.TotalControlHeight = this.Height;
            VerticalScroll.ScrollPositionChanged += OnScrollPositionChanged;

            ScrollContainer = new InfiniumScrollContainer()
            {
                Parent = this,
                Top = 0,
                Left = 0,
                Width = this.Width,
                Height = this.Height,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            ScrollContainer.SizeChanged += OnScrollContainerSizeChanged;
        }



        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public InfiniumProjectsVerticalScrollBar VerticalScrollBar
        {
            get { return VerticalScroll; }
            set { VerticalScroll = value; VerticalScroll.Left = this.Width - VerticalScroll.Width; }
        }

        public DataTable ItemsDataTable
        {
            get { return ItemsDT; }
            set
            {
                ItemsDT = value;
                InitializeItems();
            }
        }


        public void InitializeItems()
        {
            if (PicItems != null)
            {
                for (int i = 0; i < PicItems.Count(); i++)
                {
                    if (PicItems[i] != null)
                    {
                        PicItems[i].Dispose();
                        PicItems[i] = null;
                    }
                }

                PicItems = new InfiniumPictureItem[0];
            }

            Offset = 0;
            ScrollContainer.Top = -Offset;
            VerticalScroll.Offset = Offset;
            VerticalScroll.Refresh();

            if (ItemsDT == null)
                return;

            int iCurWidth = iMarginToNextItem;

            if (ItemsDT.DefaultView.Count > 0)
            {
                PicItems = new InfiniumPictureItem[ItemsDT.DefaultView.Count];

                int iCurRow = 0;

                for (int i = 0; i < ItemsDT.DefaultView.Count; i++)
                {
                    PicItems[i] = new InfiniumPictureItem()
                    {
                        Parent = ScrollContainer,
                        LikesCount = Convert.ToInt32(ItemsDT.DefaultView[i]["LikesCount"]),
                        AlbumID = Convert.ToInt32(ItemsDT.DefaultView[i]["AlbumID"]),
                        PictureID = Convert.ToInt32(ItemsDT.DefaultView[i]["PictureID"]),
                        Anchor = AnchorStyles.Left | AnchorStyles.Top
                    };
                    PicItems[i].ItemClicked += OnItemClick;
                    PicItems[i].LikeClicked += OnLikeClick;


                    byte[] b = (byte[])ItemsDT.DefaultView[i]["Image"];

                    using (MemoryStream ms = new MemoryStream(b))
                    {
                        PicItems[i].Image = (Bitmap)Image.FromStream(ms);
                    }


                    if (iCurWidth + PicItems[i].Image.Width + iMarginToNextItem >= this.Width)
                    {
                        iCurWidth = iMarginToNextItem;
                        iCurRow++;
                    }

                    PicItems[i].Top = iCurRow * (PicItems[i].Height + iMarginToNextItem) + iMarginToNextItem;
                    PicItems[i].Left = iCurWidth;
                    iCurWidth += PicItems[i].Image.Width + iMarginToNextItem;
                }

                iRowsCount = iCurRow + 1;
            }

            GC.Collect();

            SetScrollHeight();
        }

        public void RefreshItem(int PictureID, int iLikesCount)
        {
            int index = ItemsDataTable.Rows.IndexOf(ItemsDataTable.Select("PictureID = " + PictureID)[0]);

            PicItems[index].LikesCount = iLikesCount;
            PicItems[index].Refresh();
        }

        private void SetScrollHeight()
        {
            if (PicItems == null || ItemsDataTable.DefaultView.Count == 0)
                return;

            if (iRowsCount * (iItemHeight + iMarginToNextItem) > this.Height)
                ScrollContainer.Height = iRowsCount * (iItemHeight + iMarginToNextItem);
            else
                ScrollContainer.Height = this.Height;

            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
            VerticalScroll.Refresh();
        }

        private void OnScrollContainerSizeChanged(object sender, EventArgs e)
        {
            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
        }

        private void OnScrollPositionChanged(object sender, int tOffset)
        {
            Offset = tOffset;
            ScrollContainer.Top = -Offset;
        }


        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (!VerticalScroll.Visible)
                return;

            if (Offset == 0)
                if (iTempScrollWheelOffset > 0)
                    iTempScrollWheelOffset = 0;

            if (e.Delta < 0)
            {
                if (Offset < ScrollContainer.Height - this.Height)
                {
                    if (Offset + VerticalScroll.ScrollWheelOffset + this.Height > ScrollContainer.Height)//last
                    {
                        iTempScrollWheelOffset = ScrollContainer.Height - this.Height - Offset;
                        Offset += iTempScrollWheelOffset;
                    }
                    else
                    {
                        Offset += VerticalScroll.ScrollWheelOffset;
                    }

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
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
                        Offset -= VerticalScroll.ScrollWheelOffset;

                    if (Offset < 0)
                        Offset = 0;

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
                }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            SetScrollHeight();

            VerticalScroll.Left = this.Width - VerticalScroll.Width;
        }


        private void OnItemClick(object sender, int AlbumID, int PictureID)
        {
            OnItemClicked(sender, AlbumID, PictureID);
        }

        private void OnLikeClick(object sender, int PictureID)
        {
            LikeClicked(sender, PictureID);
        }


        public delegate void ItemClickedEventHandler(object sender, int AlbumID, int PictureID);
        public delegate void LikeClickedEventHandler(object sender, int PictureID);

        public event ItemClickedEventHandler ItemClicked;
        public event LikeClickedEventHandler LikeClicked;

        public virtual void OnItemClicked(object sender, int AlbumID, int PictureID)
        {
            ItemClicked?.Invoke(sender, AlbumID, PictureID);//Raise the event
        }

        public virtual void OnLikeClicked(object sender, int PictureID)
        {
            LikeClicked?.Invoke(sender, PictureID);//Raise the event
        }

    }



    //lightstart

    [DefaultEvent("Click")]
    public class InfiniumTile : Control
    {
        bool bTracking = false;

        Color cCaptionColor = Color.FromArgb(56, 184, 238);
        Color cDescriptionColor = Color.FromArgb(114, 114, 114);

        Font fCaptionFont;
        Font fDescriptionFont;

        SolidBrush brCaptionFontBrush;
        SolidBrush brDescriptionFontBrush;

        Pen pBorderPen;

        Rectangle rBorderRect;

        Bitmap iImage;

        int iMarginTextLeft = 25;
        int iImageWidth = 82;

        string sCaption = "";
        string sDescription = "";
        string sFormName = "";

        public InfiniumTile()
        {
            this.Height = 130;
            this.Width = 300;

            fCaptionFont = new Font("Segoe UI", 18, FontStyle.Regular, GraphicsUnit.Pixel);
            fDescriptionFont = new System.Drawing.Font("Segoe UI", 13, FontStyle.Regular, GraphicsUnit.Pixel);

            brCaptionFontBrush = new SolidBrush(cCaptionColor);
            brDescriptionFontBrush = new SolidBrush(cDescriptionColor);

            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            pBorderPen = new Pen(new SolidBrush(Color.FromArgb(230, 230, 230)));
            rBorderRect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);

            this.BackColor = Color.Transparent;
        }

        public Bitmap Image
        {
            get { return iImage; }
            set { iImage = value; this.Refresh(); }
        }


        [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            typeof(UITypeEditor))]
        public string Caption
        {
            get { return sCaption; }
            set { sCaption = value; this.Refresh(); }
        }

        [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            typeof(UITypeEditor))]
        public string Description
        {
            get { return sDescription; }
            set { sDescription = value; this.Refresh(); }
        }

        [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            typeof(UITypeEditor))]
        public string FormName
        {
            get { return sFormName; }
            set { sFormName = value; this.Refresh(); }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            if (iImage != null)
            {
                iImage.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);
                e.Graphics.DrawImage(Image, 8, 8, iImageWidth, iImageWidth);
            }

            string sCapt = "";

            if (sCaption.Length > 0)
            {
                sCapt = GetFormattedString(e.Graphics, fCaptionFont, sCaption, this.Width - iImageWidth - iMarginTextLeft);

                e.Graphics.DrawString(sCapt, fCaptionFont, brCaptionFontBrush, iImageWidth + iMarginTextLeft, 8);
            }

            if (sDescription.Length > 0)
            {
                string sDesc = GetFormattedString(e.Graphics, fDescriptionFont, sDescription, this.Width - iImageWidth - iMarginTextLeft);


                e.Graphics.DrawString(sDesc, fDescriptionFont, brDescriptionFontBrush, iImageWidth + iMarginTextLeft + 1,
                                      e.Graphics.MeasureString(sCapt, fCaptionFont).Height + 8);
            }

            //if (bTracking)
            //{
            //    RectangleF Rect = new RectangleF(0, 0, this.Width - 40, this.Height - 15);
            //    using (GraphicsPath GraphPath = GetRoundPath(Rect, 50))
            //    {
            //        this.Region = new Region(GraphPath);
            //        e.Graphics.DrawPath(pBorderPen, GraphPath);
            //    }
            //}
            if (bTracking)
                e.Graphics.DrawRectangle(pBorderPen, rBorderRect);
        }

        GraphicsPath GetRoundPath(RectangleF Rect, int radius)
        {
            float r2 = radius / 2f;
            GraphicsPath GraphPath = new GraphicsPath();
            GraphPath.AddArc(Rect.X, Rect.Y, radius, radius, 180, 90);
            GraphPath.AddLine(Rect.X + r2, Rect.Y, Rect.Width - r2, Rect.Y);
            GraphPath.AddArc(Rect.X + Rect.Width - radius, Rect.Y, radius, radius, 270, 90);
            GraphPath.AddLine(Rect.Width, Rect.Y + r2, Rect.Width, Rect.Height - r2);
            GraphPath.AddArc(Rect.X + Rect.Width - radius,
                             Rect.Y + Rect.Height - radius, radius, radius, 0, 90);
            GraphPath.AddLine(Rect.Width - r2, Rect.Height, Rect.X + r2, Rect.Height);
            GraphPath.AddArc(Rect.X, Rect.Y + Rect.Height - radius, radius, radius, 90, 90);
            GraphPath.AddLine(Rect.X, Rect.Height - r2, Rect.X, Rect.Y + r2);
            GraphPath.CloseFigure();
            return GraphPath;
        }

        private string GetFormattedString(Graphics G, Font CurFont, string sText, int MaxWidth)
        {
            if (sText.Length == 0)
                return "";

            string sRes = "";

            for (int i = 0; i < sText.Length; i++)
            {
                sRes += sText[i];

                if (G.MeasureString(sRes, CurFont).Width > MaxWidth)
                {
                    int LastSpace = GetLastSpace(sRes);

                    if (LastSpace > 0)
                    {
                        sRes = sRes.Insert(LastSpace + 1, "\n");
                    }
                }
            }

            return sRes;
        }

        public int GetLastSpace(string Text)
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

        private void OnContextClick(object sender, EventArgs e)
        {
            MessageBox.Show("sdfsdf");
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                OnRightMouseClicked(this, FormName);
            }
            else
            {
                OnItemClicked(this, FormName);
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

            bTracking = false;
            this.Refresh();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            rBorderRect.Width = this.Width - 1;
            rBorderRect.Height = this.Height - 1;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            this.Height = 130;
            this.Width = 300;

            rBorderRect.Width = this.Width - 1;
            rBorderRect.Height = this.Height - 1;
        }



        public delegate void ItemClickedEventHandler(object sender, string FormName);

        public event ItemClickedEventHandler ItemClicked;
        public event ItemClickedEventHandler RightMouseClicked;

        public virtual void OnItemClicked(object sender, string FormName)
        {
            ItemClicked?.Invoke(sender, FormName);//Raise the event
        }

        public virtual void OnRightMouseClicked(object sender, string FormName)
        {
            RightMouseClicked?.Invoke(sender, FormName);//Raise the event
        }
    }





    public class InfiniumTilesContainer : Control
    {
        public int Offset = 0;

        int iTempScrollWheelOffset = 0;

        int iMarginToNextItem = 40;
        int iItemWidth = 300;
        int iItemHeight = 130;

        int iRowsCount = 0;

        public int MenuItemID = -1;

        public InfiniumScrollContainer ScrollContainer;
        public InfiniumProjectsVerticalScrollBar VerticalScroll;

        public DataTable ItemsDT;

        public InfiniumTile[] InfiniumTiles;

        public InfiniumTilesContainer()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            VerticalScroll = new InfiniumProjectsVerticalScrollBar()
            {
                Parent = this,
                Height = this.Height
            };
            VerticalScroll.Left = this.Width - VerticalScroll.Width;
            VerticalScroll.TotalControlHeight = this.Height;
            VerticalScroll.ScrollPositionChanged += OnScrollPositionChanged;

            ScrollContainer = new InfiniumScrollContainer()
            {
                Parent = this,
                Top = 0,
                Left = 0,
                Width = this.Width,
                Height = this.Height,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            ScrollContainer.SizeChanged += OnScrollContainerSizeChanged;
            ScrollContainer.MouseMove += ScrollContainerMouseMove;

            this.BackColor = Color.Transparent;
        }



        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public InfiniumProjectsVerticalScrollBar VerticalScrollBar
        {
            get { return VerticalScroll; }
            set { VerticalScroll = value; VerticalScroll.Left = this.Width - VerticalScroll.Width; }
        }

        public DataTable ItemsDataTable
        {
            get { return ItemsDT; }
            set
            {
                ItemsDT = value;
            }
        }

        public void InitializeItems()
        {
            if (InfiniumTiles != null)
            {
                for (int i = 0; i < InfiniumTiles.Count(); i++)
                {
                    if (InfiniumTiles[i] != null)
                    {
                        InfiniumTiles[i].Dispose();
                        InfiniumTiles[i] = null;
                    }
                }

                InfiniumTiles = new InfiniumTile[0];
            }

            Offset = 0;
            ScrollContainer.Top = -Offset;
            VerticalScroll.Offset = Offset;
            VerticalScroll.Refresh();

            if (ItemsDT == null)
                return;

            ItemsDT.DefaultView.RowFilter = "MenuItemID = " + MenuItemID;

            int iMaxCount = GetMaxCountInRow();

            int R = (this.Width - ((iMaxCount * iItemWidth) + ((iMaxCount - 1) * iMarginToNextItem))) / 2;

            if (ItemsDT.DefaultView.Count > 0)
            {
                InfiniumTiles = new InfiniumTile[ItemsDT.DefaultView.Count];

                int iCurRow = 0;
                int iCurItem = 0;

                for (int i = 0; i < ItemsDT.DefaultView.Count; i++)
                {
                    InfiniumTiles[i] = new InfiniumTile()
                    {
                        Parent = ScrollContainer,
                        Caption = ItemsDT.DefaultView[i]["ModuleName"].ToString(),
                        Description = ItemsDT.DefaultView[i]["Description"].ToString(),
                        FormName = ItemsDT.DefaultView[i]["FormName"].ToString()
                    };
                    if (ItemsDT.DefaultView[i]["Picture"] != DBNull.Value)
                    {

                        byte[] b = (byte[])ItemsDT.DefaultView[i]["Picture"];

                        using (MemoryStream ms = new MemoryStream(b))
                        {
                            InfiniumTiles[i].Image = (Bitmap)Image.FromStream(ms);
                        }
                    }

                    InfiniumTiles[i].Parent = ScrollContainer;
                    if (iCurRow == 0)
                        InfiniumTiles[i].Top = 10;
                    else
                        InfiniumTiles[i].Top = iCurRow * (InfiniumTiles[i].Height + iMarginToNextItem) + 10;
                    InfiniumTiles[i].Left = iCurItem * (InfiniumTiles[i].Width + iMarginToNextItem) + R;
                    InfiniumTiles[i].Anchor = AnchorStyles.Left | AnchorStyles.Top;
                    InfiniumTiles[i].ItemClicked += OnItemClick;
                    InfiniumTiles[i].RightMouseClicked += OnRightMouseClicked;

                    if (iCurItem < iMaxCount - 1)
                        iCurItem++;
                    else
                    {
                        if (i != ItemsDT.DefaultView.Count - 1)
                            iCurRow++;
                        iCurItem = 0;
                    }
                }

                iRowsCount = iCurRow + 1;
            }

            GC.Collect();

            SetScrollHeight();
        }

        private int GetMaxCountInRow()
        {
            return (this.Width - 10 + iMarginToNextItem) / (iItemWidth + iMarginToNextItem);
        }

        private void SetScrollHeight()
        {
            if (InfiniumTiles == null || ItemsDataTable.DefaultView.Count == 0)
                return;

            if (iRowsCount * (iItemHeight + iMarginToNextItem) - iMarginToNextItem + 10 > this.Height)
                ScrollContainer.Height = iRowsCount * (iItemHeight + iMarginToNextItem) - iMarginToNextItem + 10;
            else
                ScrollContainer.Height = this.Height;

            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
            VerticalScroll.Refresh();
        }

        private void OnScrollContainerSizeChanged(object sender, EventArgs e)
        {
            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
        }

        private void OnScrollPositionChanged(object sender, int tOffset)
        {
            Offset = tOffset;
            ScrollContainer.Top = -Offset;
        }


        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (!VerticalScroll.Visible)
                return;

            if (Offset == 0)
                if (iTempScrollWheelOffset > 0)
                    iTempScrollWheelOffset = 0;

            if (e.Delta < 0)
            {
                if (Offset < ScrollContainer.Height - this.Height)
                {
                    if (Offset + VerticalScroll.ScrollWheelOffset + this.Height > ScrollContainer.Height)//last
                    {
                        iTempScrollWheelOffset = ScrollContainer.Height - this.Height - Offset;
                        Offset += iTempScrollWheelOffset;
                    }
                    else
                    {
                        Offset += VerticalScroll.ScrollWheelOffset;
                    }

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
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
                        Offset -= VerticalScroll.ScrollWheelOffset;

                    if (Offset < 0)
                        Offset = 0;

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
                }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            SetScrollHeight();

            VerticalScroll.Left = this.Width - VerticalScroll.Width;
        }

        private void ScrollContainerMouseMove(object sender, MouseEventArgs e)
        {
            if (ScrollContainer.Focused == false)
                ScrollContainer.Focus();
        }

        private void OnItemClick(object sender, string FormName)
        {
            OnItemClicked(sender, FormName);
        }

        public delegate void ItemClickedEventHandler(object sender, string FormName);

        public event ItemClickedEventHandler ItemClicked;
        public event ItemClickedEventHandler RightMouseClicked;


        public virtual void OnItemClicked(object sender, string FormName)
        {
            ItemClicked?.Invoke(sender, FormName);//Raise the event
        }

        public virtual void OnRightMouseClicked(object sender, string FormName)
        {
            RightMouseClicked?.Invoke(sender, FormName);//Raise the event
        }
    }





    public class InfiniumNotifyItem : Control
    {
        Color cCaptionColor = Color.FromArgb(56, 184, 238);
        Color cCountColor = Color.FromArgb(255, 255, 255);
        Color cTrackColor = Color.FromArgb(245, 245, 245);
        Color cCountBackColor = Color.FromArgb(89, 197, 243);

        Font fCaptionFont = new Font("Segoe UI", 17.0f, FontStyle.Regular, GraphicsUnit.Pixel);
        Font fCountFont = new Font("Segoe UI Semibold", 12.0f, FontStyle.Regular, GraphicsUnit.Pixel);

        SolidBrush brCaptionBrush;
        SolidBrush brCountFontBrush;
        SolidBrush brCountBackBrush;

        Pen pBorderPen;

        string sCaption = "Item";

        public string FormName = "";

        int iItemHeight = 50;
        bool bTrack = false;
        public Bitmap Image;

        public int Count;

        public InfiniumNotifyItem()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            this.BackColor = Color.Transparent;
            this.Cursor = Cursors.Hand;

            brCaptionBrush = new SolidBrush(cCaptionColor);
            brCountFontBrush = new SolidBrush(cCountColor);
            brCountBackBrush = new SolidBrush(cCountBackColor);

            pBorderPen = new Pen(new SolidBrush(cTrackColor));

            this.Height = iItemHeight;
            this.Width = 50;
        }


        public string Caption
        {
            get { return sCaption; }
            set { sCaption = value; this.Refresh(); }
        }

        public int ItemHeight
        {
            get { return iItemHeight; }
            set { iItemHeight = value; this.Height = value; this.Refresh(); }
        }

        private void DrawCount(Graphics G)
        {
            G.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            G.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
            G.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

            G.FillEllipse(brCountBackBrush, this.Width - 5 - 26 + 1 - 12, (this.Height - 26) / 2, 24, 24);
            G.DrawString(Count.ToString(), fCountFont, brCountFontBrush, this.Width - 5 - 26 - 12 + (26 - G.MeasureString(Count.ToString(), fCountFont).Width) / 2 + 1,
                        (this.Height - 26) / 2 + (26 - G.MeasureString(Count.ToString(), fCountFont).Height) / 2);

            G.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            G.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            G.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;

            if (Image != null)
            {
                Image.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);
                e.Graphics.DrawImage(Image, 0, 5, 40, 40);
            }

            string text = "";

            if (e.Graphics.MeasureString(sCaption, fCaptionFont).Width > this.Width - 50 - 18 - 45)
            {
                for (int i = sCaption.Length; i >= 0; i--)
                {
                    if (e.Graphics.MeasureString(sCaption.Substring(0, i), fCaptionFont).Width <= this.Width - 50 - 18 - 45)
                    {
                        text = sCaption.Substring(0, i - 1) + "...";
                        break;
                    }
                }
            }
            else
                text = sCaption;

            e.Graphics.DrawString(text, fCaptionFont, brCaptionBrush, 50 + 18,
                                 (this.Height - e.Graphics.MeasureString(sCaption, fCaptionFont).Height) / 2);

            DrawCount(e.Graphics);


            if (bTrack)
            {
                e.Graphics.DrawLine(pBorderPen, 0, 1, this.Width - 1, 1);
                e.Graphics.DrawLine(pBorderPen, 0, this.Height - 1, this.Width - 1, this.Height - 1);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!bTrack)
            {
                bTrack = true;
                this.Refresh();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (bTrack)
            {
                bTrack = false;
                this.Refresh();
            }
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            if (Parent != null)
                Parent.Focus();
        }
    }





    public class InfiniumNotifyList : Control
    {
        public int Offset = 0;

        int iTempScrollWheelOffset = 0;

        InfiniumScrollContainer ScrollContainer;
        public InfiniumProjectsVerticalScrollBar VerticalScroll;

        public InfiniumNotifyItem[] Items;

        public DataTable ItemsDT;
        public DataTable ModulesDataTable;

        int iMarginToNextItem = 10;

        public InfiniumNotifyList()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            VerticalScroll = new InfiniumProjectsVerticalScrollBar()
            {
                Parent = this,
                Height = this.Height
            };
            VerticalScroll.Left = this.Width - VerticalScroll.Width;
            VerticalScroll.TotalControlHeight = this.Height;
            VerticalScroll.ScrollPositionChanged += OnScrollPositionChanged;

            ScrollContainer = new InfiniumScrollContainer()
            {
                Parent = this,
                Top = 0,
                Left = 0,
                Width = this.Width,
                Height = this.Height,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            ScrollContainer.SizeChanged += OnScrollContainerSizeChanged;

            this.BackColor = Color.Transparent;
        }

        public void InitializeItems()
        {
            if (Items != null)
            {
                for (int i = 0; i < Items.Count(); i++)
                {
                    if (Items[i] != null)
                    {
                        Items[i].Dispose();
                        Items[i] = null;
                    }
                }

                Items = new InfiniumNotifyItem[0];
            }

            Offset = 0;
            ScrollContainer.Top = -Offset;
            VerticalScroll.Offset = Offset;
            VerticalScroll.Refresh();

            if (ItemsDT == null)
                return;

            if (ItemsDT.DefaultView.Count > 0)
            {
                Items = new InfiniumNotifyItem[ItemsDT.DefaultView.Count];

                for (int i = 0; i < ItemsDT.DefaultView.Count; i++)
                {
                    Items[i] = new InfiniumNotifyItem();

                    Items[i].Top = i * (Items[i].ItemHeight + iMarginToNextItem);
                    Items[i].Caption = ItemsDT.DefaultView[i]["ModuleName"].ToString();
                    Items[i].Count = Convert.ToInt32(ItemsDT.DefaultView[i]["Count"]);
                    Items[i].FormName = ItemsDT.DefaultView[i]["FormName"].ToString();


                    if (ModulesDataTable.Select("ModuleID = " + ItemsDT.DefaultView[i]["ModuleID"])[0]["Picture"] != DBNull.Value)
                    {
                        byte[] b = (byte[])ModulesDataTable.Select("ModuleID = " + ItemsDT.DefaultView[i]["ModuleID"])[0]["Picture"];

                        using (MemoryStream ms = new MemoryStream(b))
                        {
                            Items[i].Image = (Bitmap)Image.FromStream(ms);
                        }
                    }

                    Items[i].Parent = ScrollContainer;
                    Items[i].Width = ScrollContainer.Width;
                    Items[i].Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                    Items[i].Click += OnItemClick;
                }
            }

            SetScrollHeight();
        }

        private void SetScrollHeight()
        {
            if (Items == null || ItemsDataTable.DefaultView.Count == 0)
            {
                ScrollContainer.Height = this.Height;
                VerticalScroll.TotalControlHeight = ScrollContainer.Height;
                VerticalScroll.Refresh();

                return;
            }

            if (Items.Count() * (Items[0].Height + iMarginToNextItem) > this.Height)
                ScrollContainer.Height = Items.Count() * (Items[0].Height + iMarginToNextItem);
            else
                ScrollContainer.Height = this.Height;

            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
            VerticalScroll.Refresh();
        }


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public InfiniumProjectsVerticalScrollBar VerticalScrollBar
        {
            get { return VerticalScroll; }
            set { VerticalScroll = value; VerticalScroll.Left = this.Width - VerticalScroll.Width; }
        }


        public DataTable ItemsDataTable
        {
            get
            {
                //if (ItemsDT == null)
                //    return ItemsDT;

                //if (Items.Count() > 0)
                //    for (int i = 0; i < Items.Count(); i++)
                //    {
                //        if (Items[i] == null)
                //            continue;

                //        if (Items[i].Checked)
                //            ItemsDT.Rows[i]["Checked"] = true;
                //        else
                //            ItemsDT.Rows[i]["Checked"] = false;
                //    }

                return ItemsDT;
            }
            set
            {
                ItemsDT = value;
            }
        }

        private void OnScrollContainerSizeChanged(object sender, EventArgs e)
        {
            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
        }

        private void OnScrollPositionChanged(object sender, int tOffset)
        {
            Offset = tOffset;
            ScrollContainer.Top = -Offset;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (!VerticalScroll.Visible)
                return;

            if (Offset == 0)
                if (iTempScrollWheelOffset > 0)
                    iTempScrollWheelOffset = 0;

            if (e.Delta < 0)
            {
                if (Offset < ScrollContainer.Height - this.Height)
                {
                    if (Offset + VerticalScroll.ScrollWheelOffset + this.Height > ScrollContainer.Height)//last
                    {
                        iTempScrollWheelOffset = ScrollContainer.Height - this.Height - Offset;
                        Offset += iTempScrollWheelOffset;
                    }
                    else
                    {
                        Offset += VerticalScroll.ScrollWheelOffset;
                    }

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
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
                        Offset -= VerticalScroll.ScrollWheelOffset;

                    if (Offset < 0)
                        Offset = 0;

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
                }
        }

        private void OnItemClick(object sender, EventArgs e)
        {
            OnItemClicked(sender, ((InfiniumNotifyItem)sender).FormName);
        }

        private void OnVerticalScrollVisibleChanged(object sender, EventArgs e)
        {
            if (VerticalScroll.Visible)
                ScrollContainer.Width = this.Width - VerticalScroll.Width;
            else
                ScrollContainer.Width = this.Width;
        }



        public event ItemClickedEventHandler ItemClicked;

        public delegate void ItemClickedEventHandler(object sender, string FormName);

        public virtual void OnItemClicked(object sender, string FormName)
        {
            ItemClicked?.Invoke(sender, FormName);
        }


        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            ScrollContainer.Focus();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            SetScrollHeight();

            VerticalScroll.Left = this.Width - VerticalScroll.Width;
        }
    }





    public class InfiniumStartMenuItem : Control
    {
        Color cCaptionColor = Color.DimGray;
        Color cSelectedFontColor = Color.FromArgb(56, 184, 238);
        Color cTrackColor = Color.FromArgb(89, 197, 243);
        Color cTrackBorderColor = Color.FromArgb(240, 240, 240);

        Font fCaptionFont = new Font("Segoe UI", 18.0f, FontStyle.Regular, GraphicsUnit.Pixel);

        SolidBrush brCaptionBrush;
        SolidBrush brSelectedFontBrush;
        SolidBrush brTrackFontBrush;

        string sCaption = "Item";

        int iItemHeight = 40;
        bool bTrack = false;

        Pen pBorderPen;

        bool bSelected = false;

        public InfiniumStartMenuItem()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            this.BackColor = Color.Transparent;
            this.Cursor = Cursors.Hand;

            brCaptionBrush = new SolidBrush(cCaptionColor);
            brSelectedFontBrush = new SolidBrush(cSelectedFontColor);
            brTrackFontBrush = new SolidBrush(cTrackColor);

            pBorderPen = new Pen(new SolidBrush(cTrackBorderColor));

            this.Height = iItemHeight;
            this.Width = 50;
        }

        public bool Selected
        {
            get { return bSelected; }
            set { bSelected = value; this.Refresh(); }
        }

        public string Caption
        {
            get { return sCaption; }
            set { sCaption = value; this.Refresh(); }
        }

        public int ItemHeight
        {
            get { return iItemHeight; }
            set { iItemHeight = value; this.Height = value; this.Refresh(); }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            string text = "";

            if (e.Graphics.MeasureString(sCaption, fCaptionFont).Width > this.Width - 10)
            {
                for (int i = sCaption.Length; i >= 0; i--)
                {
                    if (e.Graphics.MeasureString(sCaption.Substring(0, i), fCaptionFont).Width <= this.Width - 10)
                    {
                        text = sCaption.Substring(0, i - 1) + "...";
                        break;
                    }
                }
            }
            else
                text = sCaption;

            //if(bTrack)
            //    e.Graphics.DrawString(sCaption, fCaptionFont, brTrackFontBrush, 0,
            //                                     (this.Height - e.Graphics.MeasureString(sCaption, fCaptionFont).Height) / 2);
            //else
            if (bSelected)
                e.Graphics.DrawString(sCaption, fCaptionFont, brSelectedFontBrush, 0,
                                 (this.Height - e.Graphics.MeasureString(sCaption, fCaptionFont).Height) / 2);
            else
                e.Graphics.DrawString(sCaption, fCaptionFont, brCaptionBrush, 0,
                                 (this.Height - e.Graphics.MeasureString(sCaption, fCaptionFont).Height) / 2);


            if (bTrack)
            {
                e.Graphics.DrawLine(pBorderPen, 0, 1, this.Width - 1, 1);
                e.Graphics.DrawLine(pBorderPen, 0, this.Height - 1, this.Width - 1, this.Height - 1);

            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!bTrack)
            {
                bTrack = true;
                this.Refresh();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (bTrack)
            {
                bTrack = false;
                this.Refresh();
            }
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            Parent.Focus();
        }
    }





    public class InfiniumStartMenu : Control
    {
        public int Offset = 0;

        int iTempScrollWheelOffset = 0;

        InfiniumScrollContainer ScrollContainer;
        public InfiniumProjectsVerticalScrollBar VerticalScroll;

        public InfiniumStartMenuItem[] Items;

        public DataTable ItemsDT;
        public DataTable ModulesDataTable;

        int iSelected = -1;

        int iMarginToNextItem = 10;

        public InfiniumStartMenu()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            VerticalScroll = new InfiniumProjectsVerticalScrollBar()
            {
                Parent = this,
                Height = this.Height
            };
            VerticalScroll.Left = this.Width - VerticalScroll.Width;
            VerticalScroll.TotalControlHeight = this.Height;
            VerticalScroll.ScrollPositionChanged += OnScrollPositionChanged;

            ScrollContainer = new InfiniumScrollContainer()
            {
                Parent = this,
                Top = 0,
                Left = 0,
                Width = this.Width,
                Height = this.Height,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            ScrollContainer.SizeChanged += OnScrollContainerSizeChanged;

            this.BackColor = Color.Transparent;
        }

        public void InitializeItems()
        {
            if (Items != null)
            {
                for (int i = 0; i < Items.Count(); i++)
                {
                    if (Items[i] != null)
                    {
                        Items[i].Dispose();
                        Items[i] = null;
                    }
                }

                Items = new InfiniumStartMenuItem[0];
            }

            Offset = 0;
            ScrollContainer.Top = -Offset;
            VerticalScroll.Offset = Offset;
            VerticalScroll.Refresh();

            if (ItemsDT == null)
                return;

            if (ItemsDT.DefaultView.Count > 0)
            {
                Items = new InfiniumStartMenuItem[ItemsDT.DefaultView.Count];

                for (int i = 0; i < ItemsDT.DefaultView.Count; i++)
                {
                    Items[i] = new InfiniumStartMenuItem();

                    Items[i].Top = i * (Items[i].ItemHeight + iMarginToNextItem);
                    Items[i].Caption = ItemsDT.DefaultView[i]["MenuItemName"].ToString();

                    Items[i].Parent = ScrollContainer;
                    Items[i].Width = ScrollContainer.Width;
                    Items[i].Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                    Items[i].Click += OnItemClick;
                }
            }

            SetScrollHeight();
        }

        private void SetScrollHeight()
        {
            if (Items == null || ItemsDataTable.DefaultView.Count == 0)
            {
                ScrollContainer.Height = this.Height;
                VerticalScroll.TotalControlHeight = ScrollContainer.Height;
                VerticalScroll.Refresh();

                return;
            }

            if (Items.Count() * (Items[0].Height + iMarginToNextItem) > this.Height)
                ScrollContainer.Height = Items.Count() * (Items[0].Height + iMarginToNextItem);
            else
                ScrollContainer.Height = this.Height;

            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
            VerticalScroll.Refresh();
        }


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public InfiniumProjectsVerticalScrollBar VerticalScrollBar
        {
            get { return VerticalScroll; }
            set { VerticalScroll = value; VerticalScroll.Left = this.Width - VerticalScroll.Width; }
        }


        public DataTable ItemsDataTable
        {
            get
            {
                //if (ItemsDT == null)
                //    return ItemsDT;

                //if (Items.Count() > 0)
                //    for (int i = 0; i < Items.Count(); i++)
                //    {
                //        if (Items[i] == null)
                //            continue;

                //        if (Items[i].Checked)
                //            ItemsDT.Rows[i]["Checked"] = true;
                //        else
                //            ItemsDT.Rows[i]["Checked"] = false;
                //    }

                return ItemsDT;
            }
            set
            {
                ItemsDT = value;
            }
        }

        public int Selected
        {
            get { return iSelected; }
            set
            {
                iSelected = value;
                if (Items != null)
                    if (Items.Count() > 0)
                    {
                        Items[iSelected].Selected = true;
                        OnItemClick(Items[iSelected], null);
                    }
            }
        }

        private void OnScrollContainerSizeChanged(object sender, EventArgs e)
        {
            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
        }

        private void OnScrollPositionChanged(object sender, int tOffset)
        {
            Offset = tOffset;
            ScrollContainer.Top = -Offset;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (!VerticalScroll.Visible)
                return;

            if (Offset == 0)
                if (iTempScrollWheelOffset > 0)
                    iTempScrollWheelOffset = 0;

            if (e.Delta < 0)
            {
                if (Offset < ScrollContainer.Height - this.Height)
                {
                    if (Offset + VerticalScroll.ScrollWheelOffset + this.Height > ScrollContainer.Height)//last
                    {
                        iTempScrollWheelOffset = ScrollContainer.Height - this.Height - Offset;
                        Offset += iTempScrollWheelOffset;
                    }
                    else
                    {
                        Offset += VerticalScroll.ScrollWheelOffset;
                    }

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
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
                        Offset -= VerticalScroll.ScrollWheelOffset;

                    if (Offset < 0)
                        Offset = 0;

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
                }
        }

        private void OnItemClick(object sender, EventArgs e)
        {
            foreach (InfiniumStartMenuItem Item in Items)
            {
                if (sender == Item)
                {
                    iSelected = ItemsDataTable.Rows.IndexOf(ItemsDataTable.Select("MenuItemName = '" + Item.Caption + "'")[0]);
                    Item.Selected = true;
                    OnItemClicked(sender, ((InfiniumStartMenuItem)sender).Caption,
                        Convert.ToInt32(ItemsDataTable.Select("MenuItemName = '" + ((InfiniumStartMenuItem)sender).Caption + "'")[0]["MenuItemID"]));
                }
                else
                {
                    if (Item.Selected)
                        Item.Selected = false;
                }
            }

        }


        public event ItemClickedEventHandler ItemClicked;

        public delegate void ItemClickedEventHandler(object sender, string MenuItemName, int MenuItemID);


        public virtual void OnItemClicked(object sender, string MenuItemName, int MenuItemID)
        {
            ItemClicked?.Invoke(sender, MenuItemName, MenuItemID);
        }


        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            ScrollContainer.Focus();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            SetScrollHeight();

            VerticalScroll.Left = this.Width - VerticalScroll.Width;
        }
    }





    public class InfiniumMinimizeItem : Control
    {
        Color cCaptionColor = Color.FromArgb(100, 100, 100);
        Color cBorderColor = Color.FromArgb(240, 240, 240);

        Font fCaptionFont = new Font("Segoe UI", 13.0f, FontStyle.Regular, GraphicsUnit.Pixel);

        SolidBrush brCaptionBrush;

        Pen pBorderPen;

        string sCaption = "Item";

        public int iItemHeight = 80;
        public int iItemWidth = 110;
        bool bTrack = false;
        public Bitmap Image;

        Bitmap CloseBMP = Properties.Resources.Closerun;

        public InfiniumMinimizeItem()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            this.Width = iItemWidth;
            this.Height = iItemHeight;

            this.BackColor = Color.Transparent;
            this.Cursor = Cursors.Hand;

            brCaptionBrush = new SolidBrush(cCaptionColor);

            pBorderPen = new Pen(new SolidBrush(cBorderColor));
        }


        public string Caption
        {
            get { return sCaption; }
            set { sCaption = value; this.Refresh(); }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;

            if (Image != null)
            {
                Image.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);
                e.Graphics.DrawImage(Image, (this.Width - 40) / 2, 5, 40, 40);
            }

            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            string text = "";

            if (e.Graphics.MeasureString(sCaption, fCaptionFont).Width > this.Width - 10)
            {
                for (int i = sCaption.Length; i >= 0; i--)
                {
                    if (e.Graphics.MeasureString(sCaption.Substring(0, i), fCaptionFont).Width <= this.Width - 10)
                    {
                        text = sCaption.Substring(0, i - 1) + "...";
                        break;
                    }
                }
            }
            else
                text = sCaption;

            e.Graphics.DrawString(text, fCaptionFont, brCaptionBrush, (this.Width - e.Graphics.MeasureString(text, fCaptionFont).Width) / 2,
                                  40 + 8);

            if (bTrack)
            {
                e.Graphics.DrawLine(pBorderPen, 0, 1, this.Width - 1, 1);
                e.Graphics.DrawLine(pBorderPen, 0, this.Height - 1, this.Width - 1, this.Height - 1);

                e.Graphics.DrawImage(CloseBMP, this.Width - 14, 4, 10, 10);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (Parent != null)
                if (!Parent.Focused)
                    Parent.Focus();

            if (!bTrack)
            {
                bTrack = true;
                this.Refresh();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (bTrack)
            {
                bTrack = false;
                this.Refresh();
            }
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            if (Parent != null)
                Parent.Focus();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.X > this.Width - 14 && e.X < this.Width - 14 + 10 && e.Y > 4 && e.Y < 10 + 4)
                OnCloseClicked(this, Caption);
            else
                OnItemClicked(this, Caption);
        }

        public event ItemClickedEventHandler CloseClicked;
        public event ItemClickedEventHandler ItemClicked;

        public delegate void ItemClickedEventHandler(object sender, string ModuleName);

        public virtual void OnCloseClicked(object sender, string ModuleName)
        {
            CloseClicked?.Invoke(sender, ModuleName);//Raise the event
        }

        public virtual void OnItemClicked(object sender, string ModuleName)
        {
            ItemClicked?.Invoke(sender, ModuleName);//Raise the event
        }
    }





    public class InfiniumMinimizeList : Control
    {
        public int Offset = 0;

        int iTempScrollWheelOffset = 0;

        public InfiniumScrollContainer ScrollContainer;
        public InfiniumHorizontalScrollBar HorizontalScroll;

        public InfiniumMinimizeItem[] Items;

        public DataTable ModulesDataTable;

        ArrayList ItemsArray;

        int iMarginToNextItem = 10;

        public InfiniumMinimizeList()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            HorizontalScroll = new InfiniumHorizontalScrollBar()
            {
                Parent = this,
                Width = this.Width
            };
            HorizontalScroll.Top = this.Top - HorizontalScroll.Top;
            HorizontalScroll.TotalControlWidth = this.Width;
            HorizontalScroll.ScrollPositionChanged += OnScrollPositionChanged;

            ScrollContainer = new InfiniumScrollContainer()
            {
                Parent = this,
                Top = 0,
                Left = 0,
                Width = this.Width,
                Height = this.Height,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            ScrollContainer.SizeChanged += OnScrollContainerSizeChanged;

            this.BackColor = Color.Transparent;

            ItemsArray = new ArrayList();
        }

        public void InitializeItems()
        {
            if (Items != null)
            {
                for (int i = 0; i < Items.Count(); i++)
                {
                    if (Items[i] != null)
                    {
                        Items[i].Dispose();
                        Items[i] = null;
                    }
                }

                Items = new InfiniumMinimizeItem[0];
            }

            Offset = 0;
            ScrollContainer.Left = -Offset;
            HorizontalScroll.Offset = Offset;
            HorizontalScroll.Refresh();

            if (ItemsArray == null)
                return;

            if (ItemsArray.Count > 0)
            {
                Items = new InfiniumMinimizeItem[ItemsArray.Count];

                for (int i = 0; i < ItemsArray.Count; i++)
                {
                    Items[i] = new InfiniumMinimizeItem();
                    Items[i].Left = i * (Items[i].iItemWidth + iMarginToNextItem);
                    Items[i].Caption = ModulesDataTable.Select("FormName = '" + ((Form)ItemsArray[i]).Name + "'")[0]["ModuleName"].ToString();

                    byte[] b = (byte[])ModulesDataTable.Select("FormName = '" + ((Form)ItemsArray[i]).Name + "'")[0]["Picture"];

                    using (MemoryStream ms = new MemoryStream(b))
                    {
                        Items[i].Image = (Bitmap)Image.FromStream(ms);
                    }

                    Items[i].Parent = ScrollContainer;
                    Items[i].Anchor = AnchorStyles.Left | AnchorStyles.Top;
                    Items[i].ItemClicked += OnItemClick;
                    Items[i].CloseClicked += OnCloseClick;
                }
            }

            SetScrollWidth();
        }

        private void SetScrollWidth()
        {
            if (Items == null || ItemsArray.Count == 0)
            {
                ScrollContainer.Width = this.Width;
                HorizontalScroll.TotalControlWidth = ScrollContainer.Width;
                HorizontalScroll.Refresh();

                return;
            }

            if (Items.Count() * (Items[0].Width + iMarginToNextItem) > this.Width)
                ScrollContainer.Width = Items.Count() * (Items[0].Width + iMarginToNextItem);
            else
                ScrollContainer.Width = this.Width;

            HorizontalScroll.TotalControlWidth = ScrollContainer.Width;
            HorizontalScroll.Refresh();
        }

        public void AddModule(ref Form Form)
        {
            for (int i = 0; i < ItemsArray.Count; i++)
            {
                if (((Form)ItemsArray[i]).Name == Form.Name)
                {
                    return;
                }
            }

            ItemsArray.Add(Form);
            InitializeItems();
        }

        public void RemoveModule(string FormName)
        {
            for (int i = 0; i < ItemsArray.Count; i++)
            {
                if (((Form)ItemsArray[i]).Name == FormName)
                {
                    ItemsArray.RemoveAt(i);
                    break;
                }

            }

            InitializeItems();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public InfiniumHorizontalScrollBar HorizontalScrollBar
        {
            get { return HorizontalScroll; }
            set { HorizontalScroll = value; HorizontalScroll.Top = this.Height - HorizontalScroll.Height; }
        }


        private void OnScrollContainerSizeChanged(object sender, EventArgs e)
        {
            HorizontalScroll.TotalControlWidth = ScrollContainer.Width;
        }

        private void OnScrollPositionChanged(object sender, int tOffset)
        {
            Offset = tOffset;
            ScrollContainer.Left = -Offset;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (!HorizontalScroll.Visible)
                return;

            if (Offset == 0)
                if (iTempScrollWheelOffset > 0)
                    iTempScrollWheelOffset = 0;

            if (e.Delta < 0)
            {
                if (Offset < ScrollContainer.Width - this.Width)
                {
                    if (Offset + HorizontalScroll.ScrollWheelOffset + this.Width > ScrollContainer.Width)//last
                    {
                        iTempScrollWheelOffset = ScrollContainer.Width - this.Width - Offset;
                        Offset += iTempScrollWheelOffset;
                    }
                    else
                    {
                        Offset += HorizontalScroll.ScrollWheelOffset;
                    }

                    ScrollContainer.Left = -Offset;
                    HorizontalScroll.Offset = Offset;
                    HorizontalScroll.Refresh();
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
                        Offset -= HorizontalScroll.ScrollWheelOffset;

                    if (Offset < 0)
                        Offset = 0;

                    ScrollContainer.Left = -Offset;
                    HorizontalScroll.Offset = Offset;
                    HorizontalScroll.Refresh();
                }
        }

        private void OnItemClick(object sender, string ModuleName)
        {
            OnItemClicked(sender, ModulesDataTable.Select("ModuleName = '" + ModuleName + "'")[0]["FormName"].ToString());
        }

        private void OnCloseClick(object sender, string ModuleName)
        {
            OnCloseClicked(sender, ModulesDataTable.Select("ModuleName = '" + ((InfiniumMinimizeItem)sender).Caption + "'")[0]["FormName"].ToString());
        }

        private void OnVerticalScrollVisibleChanged(object sender, EventArgs e)
        {
            if (HorizontalScroll.Visible)
                ScrollContainer.Width = this.Width - HorizontalScroll.Width;
            else
                ScrollContainer.Width = this.Width;
        }

        public Form GetForm(string FormName)
        {
            foreach (Form F in ItemsArray)
            {
                if (F.Name == FormName)
                    return F;
            }

            return null;
        }

        public event ItemClickedEventHandler ItemClicked;
        public event ItemClickedEventHandler CloseClicked;

        public delegate void ItemClickedEventHandler(object sender, string FormName);

        public virtual void OnCloseClicked(object sender, string FormName)
        {
            CloseClicked?.Invoke(sender, FormName);//Raise the event
        }

        public virtual void OnItemClicked(object sender, string FormName)
        {
            ItemClicked?.Invoke(sender, FormName);
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            ScrollContainer.Focus();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            SetScrollWidth();

            HorizontalScroll.Width = this.Width;
            HorizontalScroll.Top = this.Height - HorizontalScroll.Height;
        }
    }





    public class InfiniumFilesMenuItem : Control
    {
        Color cCaptionColor = Color.DimGray;
        Color cSelectedFontColor = Color.FromArgb(56, 184, 238);
        Color cTrackColor = Color.FromArgb(89, 197, 243);
        Color cTrackBorderColor = Color.FromArgb(240, 240, 240);

        Font fCaptionFont = new Font("Segoe UI", 18.0f, FontStyle.Regular, GraphicsUnit.Pixel);

        SolidBrush brCaptionBrush;
        SolidBrush brSelectedFontBrush;
        SolidBrush brTrackFontBrush;

        string sCaption = "Item";
        public int iCount = 0;
        public int FolderID = 0;

        int iItemHeight = 35;
        bool bTrack = false;

        Pen pBorderPen;

        bool bSelected = false;

        public InfiniumFilesMenuItem()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            this.BackColor = Color.Transparent;
            this.Cursor = Cursors.Hand;

            brCaptionBrush = new SolidBrush(cCaptionColor);
            brSelectedFontBrush = new SolidBrush(cSelectedFontColor);
            brTrackFontBrush = new SolidBrush(cTrackColor);

            pBorderPen = new Pen(new SolidBrush(cTrackBorderColor));

            this.Height = iItemHeight;
            this.Width = 50;
        }

        public bool Selected
        {
            get { return bSelected; }
            set { bSelected = value; this.Refresh(); }
        }

        public string Caption
        {
            get { return sCaption; }
            set { sCaption = value; this.Refresh(); }
        }

        public int ItemHeight
        {
            get { return iItemHeight; }
            set { iItemHeight = value; this.Height = value; this.Refresh(); }
        }

        public int Count
        {
            get { return iCount; }
            set { iCount = value; iCount = value; this.Refresh(); }
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            string text = "";

            if (e.Graphics.MeasureString(sCaption, fCaptionFont).Width > this.Width - 10)
            {
                for (int i = sCaption.Length; i >= 0; i--)
                {
                    if (e.Graphics.MeasureString(sCaption.Substring(0, i), fCaptionFont).Width <= this.Width - 10)
                    {
                        text = sCaption.Substring(0, i - 1) + "...";
                        break;
                    }
                }
            }
            else
                text = sCaption;

            //if(bTrack)
            //    e.Graphics.DrawString(sCaption, fCaptionFont, brTrackFontBrush, 0,
            //                                     (this.Height - e.Graphics.MeasureString(sCaption, fCaptionFont).Height) / 2);
            //else

            if (Count > 0)
                text += " (" + Count.ToString() + ")";

            if (bSelected)
                e.Graphics.DrawString(text, fCaptionFont, brSelectedFontBrush, 0,
                                 (this.Height - e.Graphics.MeasureString(text, fCaptionFont).Height) / 2);
            else
                e.Graphics.DrawString(text, fCaptionFont, brCaptionBrush, 0,
                                 (this.Height - e.Graphics.MeasureString(text, fCaptionFont).Height) / 2);


            if (bTrack)
            {
                e.Graphics.DrawLine(pBorderPen, 0, 1, this.Width - 1, 1);
                e.Graphics.DrawLine(pBorderPen, 0, this.Height - 1, this.Width - 1, this.Height - 1);

            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!bTrack)
            {
                bTrack = true;
                this.Refresh();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (bTrack)
            {
                bTrack = false;
                this.Refresh();
            }
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            Parent.Focus();
        }
    }





    public class InfiniumFilesMenu : Control
    {
        public int Offset = 0;

        int iTempScrollWheelOffset = 0;

        InfiniumScrollContainer ScrollContainer;
        public InfiniumProjectsVerticalScrollBar VerticalScroll;

        public InfiniumFilesMenuItem[] Items;

        public DataTable ItemsDT;

        int iSelected = -1;

        int iMarginToNextItem = 10;

        public InfiniumFilesMenu()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            VerticalScroll = new InfiniumProjectsVerticalScrollBar()
            {
                Parent = this,
                Height = this.Height
            };
            VerticalScroll.Left = this.Width - VerticalScroll.Width;
            VerticalScroll.TotalControlHeight = this.Height;
            VerticalScroll.ScrollPositionChanged += OnScrollPositionChanged;

            ScrollContainer = new InfiniumScrollContainer()
            {
                Parent = this,
                Top = 0,
                Left = 0,
                Width = this.Width,
                Height = this.Height,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            ScrollContainer.SizeChanged += OnScrollContainerSizeChanged;

            this.BackColor = Color.Transparent;
        }

        public void InitializeItems()
        {
            if (Items != null)
            {
                for (int i = 0; i < Items.Count(); i++)
                {
                    if (Items[i] != null)
                    {
                        Items[i].Dispose();
                        Items[i] = null;
                    }
                }

                Items = new InfiniumFilesMenuItem[0];
            }

            Offset = 0;
            ScrollContainer.Top = -Offset;
            VerticalScroll.Offset = Offset;
            VerticalScroll.Refresh();

            if (ItemsDT == null)
                return;



            if (ItemsDT.DefaultView.Count > 0)
            {
                Items = new InfiniumFilesMenuItem[ItemsDT.DefaultView.Count];

                for (int i = 0; i < ItemsDT.DefaultView.Count; i++)
                {
                    Items[i] = new InfiniumFilesMenuItem();

                    Items[i].Top = i * (Items[i].ItemHeight + iMarginToNextItem);
                    Items[i].Caption = ItemsDT.DefaultView[i]["Category"].ToString();
                    Items[i].FolderID = Convert.ToInt32(ItemsDT.DefaultView[i]["FolderID"]);
                    Items[i].Parent = ScrollContainer;
                    Items[i].Width = ScrollContainer.Width;
                    Items[i].Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                    Items[i].Click += OnItemClick;
                }
            }

            SetScrollHeight();
        }

        private void SetScrollHeight()
        {
            if (Items == null || ItemsDataTable.DefaultView.Count == 0)
            {
                ScrollContainer.Height = this.Height;
                VerticalScroll.TotalControlHeight = ScrollContainer.Height;
                VerticalScroll.Refresh();

                return;
            }

            if (Items.Count() * (Items[0].Height + iMarginToNextItem) > this.Height)
                ScrollContainer.Height = Items.Count() * (Items[0].Height + iMarginToNextItem);
            else
                ScrollContainer.Height = this.Height;

            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
            VerticalScroll.Refresh();
        }


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public InfiniumProjectsVerticalScrollBar VerticalScrollBar
        {
            get { return VerticalScroll; }
            set { VerticalScroll = value; VerticalScroll.Left = this.Width - VerticalScroll.Width; }
        }


        public DataTable ItemsDataTable
        {
            get
            {
                //if (ItemsDT == null)
                //    return ItemsDT;

                //if (Items.Count() > 0)
                //    for (int i = 0; i < Items.Count(); i++)
                //    {
                //        if (Items[i] == null)
                //            continue;

                //        if (Items[i].Checked)
                //            ItemsDT.Rows[i]["Checked"] = true;
                //        else
                //            ItemsDT.Rows[i]["Checked"] = false;
                //    }

                return ItemsDT;
            }
            set
            {
                ItemsDT = value;
            }
        }

        public int Selected
        {
            get { return iSelected; }
            set
            {
                iSelected = value;
                if (Items != null)
                    if (Items.Count() > 0)
                    {
                        Items[iSelected].Selected = true;
                        OnItemClick(Items[iSelected], null);
                    }
            }
        }

        public InfiniumFilesMenuItem GetItemByFolderID(int FolderID)
        {
            if (Items == null)
                return null;

            foreach (InfiniumFilesMenuItem Item in Items)
            {
                if (Item.FolderID == FolderID)
                    return Item;
            }

            return null;
        }

        private void OnScrollContainerSizeChanged(object sender, EventArgs e)
        {
            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
        }

        private void OnScrollPositionChanged(object sender, int tOffset)
        {
            Offset = tOffset;
            ScrollContainer.Top = -Offset;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (!VerticalScroll.Visible)
                return;

            if (Offset == 0)
                if (iTempScrollWheelOffset > 0)
                    iTempScrollWheelOffset = 0;

            if (e.Delta < 0)
            {
                if (Offset < ScrollContainer.Height - this.Height)
                {
                    if (Offset + VerticalScroll.ScrollWheelOffset + this.Height > ScrollContainer.Height)//last
                    {
                        iTempScrollWheelOffset = ScrollContainer.Height - this.Height - Offset;
                        Offset += iTempScrollWheelOffset;
                    }
                    else
                    {
                        Offset += VerticalScroll.ScrollWheelOffset;
                    }

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
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
                        Offset -= VerticalScroll.ScrollWheelOffset;

                    if (Offset < 0)
                        Offset = 0;

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
                }
        }

        private void OnItemClick(object sender, EventArgs e)
        {
            foreach (InfiniumFilesMenuItem Item in Items)
            {
                if (sender == Item)
                {
                    iSelected = ItemsDataTable.Rows.IndexOf(ItemsDataTable.Select("FolderID = '" + Item.FolderID + "'")[0]);
                    Item.Selected = true;
                    OnItemClicked(sender, ((InfiniumFilesMenuItem)sender).FolderID, ((InfiniumFilesMenuItem)sender).Caption);
                }
                else
                {
                    if (Item.Selected)
                        Item.Selected = false;
                }
            }

        }


        public event ItemClickedEventHandler ItemClicked;

        public delegate void ItemClickedEventHandler(object sender, int FolderID, string Caption);


        public virtual void OnItemClicked(object sender, int FolderID, string Caption)
        {
            ItemClicked?.Invoke(sender, FolderID, Caption);
        }


        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            ScrollContainer.Focus();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            SetScrollHeight();

            VerticalScroll.Left = this.Width - VerticalScroll.Width;
        }
    }




    public class InfiniumNewsClientsMenuItem : Control
    {
        Color cCaptionColor = Color.DimGray;
        Color cSelectedFontColor = Color.FromArgb(56, 184, 238);
        Color cTrackColor = Color.FromArgb(89, 197, 243);
        Color cTrackBorderColor = Color.FromArgb(240, 240, 240);

        Font fCaptionFont = new Font("Segoe UI", 18.0f, FontStyle.Regular, GraphicsUnit.Pixel);

        SolidBrush brCaptionBrush;
        SolidBrush brSelectedFontBrush;
        SolidBrush brTrackFontBrush;

        string sCaption = "Item";

        int iItemHeight = 35;
        bool bTrack = false;

        public int ClientID = -1;

        Pen pBorderPen;

        bool bSelected = false;

        public InfiniumNewsClientsMenuItem()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            this.BackColor = Color.Transparent;
            this.Cursor = Cursors.Hand;

            brCaptionBrush = new SolidBrush(cCaptionColor);
            brSelectedFontBrush = new SolidBrush(cSelectedFontColor);
            brTrackFontBrush = new SolidBrush(cTrackColor);

            pBorderPen = new Pen(new SolidBrush(cTrackBorderColor));

            this.Height = iItemHeight;
            this.Width = 50;
        }

        public bool Selected
        {
            get { return bSelected; }
            set { bSelected = value; this.Refresh(); }
        }

        public string Caption
        {
            get { return sCaption; }
            set { sCaption = value; this.Refresh(); }
        }

        public int ItemHeight
        {
            get { return iItemHeight; }
            set { iItemHeight = value; this.Height = value; this.Refresh(); }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            string text = "";

            if (e.Graphics.MeasureString(sCaption, fCaptionFont).Width > this.Width - 10)
            {
                for (int i = sCaption.Length; i >= 0; i--)
                {
                    if (e.Graphics.MeasureString(sCaption.Substring(0, i), fCaptionFont).Width <= this.Width - 10)
                    {
                        text = sCaption.Substring(0, i - 1) + "...";
                        break;
                    }
                }
            }
            else
                text = sCaption;

            //if(bTrack)
            //    e.Graphics.DrawString(sCaption, fCaptionFont, brTrackFontBrush, 0,
            //                                     (this.Height - e.Graphics.MeasureString(sCaption, fCaptionFont).Height) / 2);
            //else
            if (bSelected)
                e.Graphics.DrawString(sCaption, fCaptionFont, brSelectedFontBrush, 0,
                                 (this.Height - e.Graphics.MeasureString(sCaption, fCaptionFont).Height) / 2);
            else
                e.Graphics.DrawString(sCaption, fCaptionFont, brCaptionBrush, 0,
                                 (this.Height - e.Graphics.MeasureString(sCaption, fCaptionFont).Height) / 2);


            if (bTrack)
            {
                e.Graphics.DrawLine(pBorderPen, 0, 1, this.Width - 1, 1);
                e.Graphics.DrawLine(pBorderPen, 0, this.Height - 1, this.Width - 1, this.Height - 1);

            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!bTrack)
            {
                bTrack = true;
                this.Refresh();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (bTrack)
            {
                bTrack = false;
                this.Refresh();
            }
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            OnItemClicked(this, Caption, ClientID);

            Parent.Focus();
        }


        public event ItemClickedEventHandler ItemClicked;

        public delegate void ItemClickedEventHandler(object sender, string ClientName, int ClientID);


        public virtual void OnItemClicked(object sender, string ClientName, int ClientID)
        {
            ItemClicked?.Invoke(sender, ClientName, ClientID);
        }
    }




    public class InfiniumNewsClientsMenu : Control
    {
        public int Offset = 0;

        int iTempScrollWheelOffset = 0;

        InfiniumScrollContainer ScrollContainer;
        public InfiniumProjectsVerticalScrollBar VerticalScroll;

        public InfiniumNewsClientsMenuItem[] Items;

        public DataTable ItemsDT;
        public DataTable ModulesDataTable;

        int iSelected = -1;

        int iMarginToNextItem = 10;

        public InfiniumNewsClientsMenu()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            VerticalScroll = new InfiniumProjectsVerticalScrollBar()
            {
                Parent = this,
                Height = this.Height
            };
            VerticalScroll.Left = this.Width - VerticalScroll.Width;
            VerticalScroll.TotalControlHeight = this.Height;
            VerticalScroll.ScrollPositionChanged += OnScrollPositionChanged;

            ScrollContainer = new InfiniumScrollContainer()
            {
                Parent = this,
                Top = 0,
                Left = 0,
                Width = this.Width,
                Height = this.Height,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            ScrollContainer.SizeChanged += OnScrollContainerSizeChanged;

            this.BackColor = Color.Transparent;
        }

        public void InitializeItems()
        {
            if (Items != null)
            {
                for (int i = 0; i < Items.Count(); i++)
                {
                    if (Items[i] != null)
                    {
                        Items[i].Dispose();
                        Items[i] = null;
                    }
                }

                Items = new InfiniumNewsClientsMenuItem[0];
            }

            Offset = 0;
            ScrollContainer.Top = -Offset;
            VerticalScroll.Offset = Offset;
            VerticalScroll.Refresh();

            if (ItemsDT == null)
                return;

            if (ItemsDT.DefaultView.Count > 0)
            {
                Items = new InfiniumNewsClientsMenuItem[ItemsDT.DefaultView.Count];

                for (int i = 0; i < ItemsDT.DefaultView.Count; i++)
                {
                    Items[i] = new InfiniumNewsClientsMenuItem();

                    Items[i].Top = i * (Items[i].ItemHeight + iMarginToNextItem);
                    Items[i].Caption = ItemsDT.DefaultView[i]["ClientName"].ToString();
                    Items[i].ClientID = Convert.ToInt32(ItemsDT.DefaultView[i]["ClientID"]);
                    Items[i].Parent = ScrollContainer;
                    Items[i].Width = ScrollContainer.Width;
                    Items[i].Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                    Items[i].ItemClicked += OnItemClicked;
                }
            }

            SetScrollHeight();
        }

        private void SetScrollHeight()
        {
            if (Items == null || ClientsDataTable.DefaultView.Count == 0)
            {
                ScrollContainer.Height = this.Height;
                VerticalScroll.TotalControlHeight = ScrollContainer.Height;
                VerticalScroll.Refresh();

                return;
            }

            if (Items.Count() * (Items[0].Height + iMarginToNextItem) > this.Height)
                ScrollContainer.Height = Items.Count() * (Items[0].Height + iMarginToNextItem);
            else
                ScrollContainer.Height = this.Height;

            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
            VerticalScroll.Refresh();
        }


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public InfiniumProjectsVerticalScrollBar VerticalScrollBar
        {
            get { return VerticalScroll; }
            set { VerticalScroll = value; VerticalScroll.Left = this.Width - VerticalScroll.Width; }
        }


        public DataTable ClientsDataTable
        {
            get
            {
                //if (ItemsDT == null)
                //    return ItemsDT;

                //if (Items.Count() > 0)
                //    for (int i = 0; i < Items.Count(); i++)
                //    {
                //        if (Items[i] == null)
                //            continue;

                //        if (Items[i].Checked)
                //            ItemsDT.Rows[i]["Checked"] = true;
                //        else
                //            ItemsDT.Rows[i]["Checked"] = false;
                //    }

                return ItemsDT;
            }
            set
            {
                ItemsDT = value;
            }
        }

        public int Selected
        {
            get { return iSelected; }
            set
            {
                if (value == -1)
                {
                    if (iSelected > -1)
                    {
                        Items[iSelected].Selected = false;
                        iSelected = value;
                        return;
                    }
                    else
                        return;
                }

                iSelected = value;
                if (Items != null)
                    if (Items.Count() > 0)
                    {
                        Items[iSelected].Selected = true;
                        OnItemClicked(Items[iSelected], Items[iSelected].Caption, Items[iSelected].ClientID);
                    }
            }
        }

        private void OnScrollContainerSizeChanged(object sender, EventArgs e)
        {
            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
        }

        private void OnScrollPositionChanged(object sender, int tOffset)
        {
            Offset = tOffset;
            ScrollContainer.Top = -Offset;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (!VerticalScroll.Visible)
                return;

            if (Offset == 0)
                if (iTempScrollWheelOffset > 0)
                    iTempScrollWheelOffset = 0;

            if (e.Delta < 0)
            {
                if (Offset < ScrollContainer.Height - this.Height)
                {
                    if (Offset + VerticalScroll.ScrollWheelOffset + this.Height > ScrollContainer.Height)//last
                    {
                        iTempScrollWheelOffset = ScrollContainer.Height - this.Height - Offset;
                        Offset += iTempScrollWheelOffset;
                    }
                    else
                    {
                        Offset += VerticalScroll.ScrollWheelOffset;
                    }

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
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
                        Offset -= VerticalScroll.ScrollWheelOffset;

                    if (Offset < 0)
                        Offset = 0;

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
                }
        }


        public event ItemClickedEventHandler ItemClicked;

        public delegate void ItemClickedEventHandler(object sender, string ClientName, int ClientID);


        public virtual void OnItemClicked(object sender, string ClientName, int ClientID)
        {
            ItemClicked?.Invoke(sender, ClientName, ClientID);

            if (Selected > -1)
                Items[iSelected].Selected = false;

            iSelected = ClientsDataTable.Rows.IndexOf(ClientsDataTable.Select("ClientID = " + ClientID)[0]);
            ((InfiniumNewsClientsMenuItem)sender).Selected = true;
        }


        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            ScrollContainer.Focus();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            SetScrollHeight();

            VerticalScroll.Left = this.Width - VerticalScroll.Width;
        }
    }



    public class InfiniumNewsClientsManagersMenu : Control
    {
        public int Offset = 0;

        int iTempScrollWheelOffset = 0;

        InfiniumScrollContainer ScrollContainer;
        public InfiniumProjectsVerticalScrollBar VerticalScroll;

        public InfiniumNewsClientsMenuItem[] Items;

        public DataTable ItemsDT;
        public DataTable ModulesDataTable;

        int iSelected = -1;

        int iMarginToNextItem = 10;

        public InfiniumNewsClientsManagersMenu()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            VerticalScroll = new InfiniumProjectsVerticalScrollBar()
            {
                Parent = this,
                Height = this.Height
            };
            VerticalScroll.Left = this.Width - VerticalScroll.Width;
            VerticalScroll.TotalControlHeight = this.Height;
            VerticalScroll.ScrollPositionChanged += OnScrollPositionChanged;

            ScrollContainer = new InfiniumScrollContainer()
            {
                Parent = this,
                Top = 0,
                Left = 0,
                Width = this.Width,
                Height = this.Height,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            ScrollContainer.SizeChanged += OnScrollContainerSizeChanged;

            this.BackColor = Color.Transparent;
        }

        public void InitializeItems()
        {
            if (Items != null)
            {
                for (int i = 0; i < Items.Count(); i++)
                {
                    if (Items[i] != null)
                    {
                        Items[i].Dispose();
                        Items[i] = null;
                    }
                }

                Items = new InfiniumNewsClientsMenuItem[0];
            }

            Offset = 0;
            ScrollContainer.Top = -Offset;
            VerticalScroll.Offset = Offset;
            VerticalScroll.Refresh();

            if (ItemsDT == null)
                return;

            if (ItemsDT.DefaultView.Count > 0)
            {
                Items = new InfiniumNewsClientsMenuItem[ItemsDT.DefaultView.Count];

                for (int i = 0; i < ItemsDT.DefaultView.Count; i++)
                {
                    Items[i] = new InfiniumNewsClientsMenuItem();

                    Items[i].Top = i * (Items[i].ItemHeight + iMarginToNextItem);
                    Items[i].Caption = ItemsDT.DefaultView[i]["Name"].ToString();
                    Items[i].ClientID = Convert.ToInt32(ItemsDT.DefaultView[i]["ManagerID"]);
                    Items[i].Parent = ScrollContainer;
                    Items[i].Width = ScrollContainer.Width;
                    Items[i].Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                    Items[i].ItemClicked += OnItemClicked;
                }
            }

            SetScrollHeight();
        }

        private void SetScrollHeight()
        {
            if (Items == null || ClientsManagersDataTable.DefaultView.Count == 0)
            {
                ScrollContainer.Height = this.Height;
                VerticalScroll.TotalControlHeight = ScrollContainer.Height;
                VerticalScroll.Refresh();

                return;
            }

            if (Items.Count() * (Items[0].Height + iMarginToNextItem) > this.Height)
                ScrollContainer.Height = Items.Count() * (Items[0].Height + iMarginToNextItem);
            else
                ScrollContainer.Height = this.Height;

            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
            VerticalScroll.Refresh();
        }


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public InfiniumProjectsVerticalScrollBar VerticalScrollBar
        {
            get { return VerticalScroll; }
            set { VerticalScroll = value; VerticalScroll.Left = this.Width - VerticalScroll.Width; }
        }


        public DataTable ClientsManagersDataTable
        {
            get
            {
                //if (ItemsDT == null)
                //    return ItemsDT;

                //if (Items.Count() > 0)
                //    for (int i = 0; i < Items.Count(); i++)
                //    {
                //        if (Items[i] == null)
                //            continue;

                //        if (Items[i].Checked)
                //            ItemsDT.Rows[i]["Checked"] = true;
                //        else
                //            ItemsDT.Rows[i]["Checked"] = false;
                //    }

                return ItemsDT;
            }
            set
            {
                ItemsDT = value;
            }
        }

        public int Selected
        {
            get { return iSelected; }
            set
            {
                if (value == -1)
                {
                    if (iSelected > -1)
                    {
                        Items[iSelected].Selected = false;
                        iSelected = value;
                        return;
                    }
                    else
                        return;
                }

                iSelected = value;
                if (Items != null)
                    if (Items.Count() > 0)
                    {
                        Items[iSelected].Selected = true;
                        OnItemClicked(Items[iSelected], Items[iSelected].Caption, Items[iSelected].ClientID);
                    }
            }
        }

        private void OnScrollContainerSizeChanged(object sender, EventArgs e)
        {
            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
        }

        private void OnScrollPositionChanged(object sender, int tOffset)
        {
            Offset = tOffset;
            ScrollContainer.Top = -Offset;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (!VerticalScroll.Visible)
                return;

            if (Offset == 0)
                if (iTempScrollWheelOffset > 0)
                    iTempScrollWheelOffset = 0;

            if (e.Delta < 0)
            {
                if (Offset < ScrollContainer.Height - this.Height)
                {
                    if (Offset + VerticalScroll.ScrollWheelOffset + this.Height > ScrollContainer.Height)//last
                    {
                        iTempScrollWheelOffset = ScrollContainer.Height - this.Height - Offset;
                        Offset += iTempScrollWheelOffset;
                    }
                    else
                    {
                        Offset += VerticalScroll.ScrollWheelOffset;
                    }

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
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
                        Offset -= VerticalScroll.ScrollWheelOffset;

                    if (Offset < 0)
                        Offset = 0;

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
                }
        }


        public event ItemClickedEventHandler ItemClicked;

        public delegate void ItemClickedEventHandler(object sender, string Name, int ManagerID);


        public virtual void OnItemClicked(object sender, string Name, int ManagerID)
        {
            ItemClicked?.Invoke(sender, Name, ManagerID);

            if (Selected > -1)
                Items[iSelected].Selected = false;

            iSelected = ClientsManagersDataTable.Rows.IndexOf(ClientsManagersDataTable.Select("ManagerID = " + ManagerID)[0]);
            ((InfiniumNewsClientsMenuItem)sender).Selected = true;
        }


        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            ScrollContainer.Focus();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            SetScrollHeight();

            VerticalScroll.Left = this.Width - VerticalScroll.Width;
        }
    }







    public class InfiniumMessagesSelectedUserItem : Control
    {
        Color cCaptionColor = Color.FromArgb(70, 70, 70);
        Color cSelectedFontColor = Color.FromArgb(56, 184, 238);
        Color cTrackColor = Color.FromArgb(120, 120, 120);
        Color cOnlineColor = Color.FromArgb(56, 184, 238);
        Color cCountBackColor = Color.FromArgb(255, 203, 22);
        Color cCountFontColor = Color.White;

        Font fCaptionFont = new Font("Segoe UI", 17.0f, FontStyle.Regular, GraphicsUnit.Pixel);
        Font fOnlineFont = new Font("Segoe UI", 12.0f, FontStyle.Regular, GraphicsUnit.Pixel);
        Font fCountFont = new Font("Segoe UI", 12.0f, FontStyle.Bold, GraphicsUnit.Pixel);

        SolidBrush brCaptionBrush;
        SolidBrush brSelectedFontBrush;
        SolidBrush brTrackFontBrush;
        SolidBrush brOnlineColorBrush;
        SolidBrush brCountFontBrush;

        public Bitmap Image = null;
        public Bitmap CloseBMP = Properties.Resources.CloseUser;

        string sCaption = "Item";
        public int UserID = -1;


        public int Count = 0;

        public bool Online = false;

        int iItemHeight = 58;
        bool bTrack = false;

        int iImageHeight = 40;
        int iImageWidth = 43;

        Pen pBorderPen;
        Pen pCountPen;

        bool bSelected = false;

        Rectangle rCountRect;

        public InfiniumMessagesSelectedUserItem()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            this.BackColor = Color.Transparent;
            this.Cursor = Cursors.Hand;

            brCaptionBrush = new SolidBrush(cCaptionColor);
            brSelectedFontBrush = new SolidBrush(cSelectedFontColor);
            brTrackFontBrush = new SolidBrush(cTrackColor);
            brOnlineColorBrush = new SolidBrush(cOnlineColor);
            brCountFontBrush = new SolidBrush(cCountFontColor);

            pBorderPen = new Pen(new SolidBrush(cTrackColor));
            pCountPen = new Pen(new SolidBrush(cCountBackColor));

            this.Height = iItemHeight;
            this.Width = 50;

            Rectangle rCountRect = new Rectangle(0, 0, 0, 0);
        }

        public bool Selected
        {
            get { return bSelected; }
            set { bSelected = value; this.Refresh(); }
        }

        public string Caption
        {
            get { return sCaption; }
            set { sCaption = value; this.Refresh(); }
        }

        public int ItemHeight
        {
            get { return iItemHeight; }
            set { iItemHeight = value; this.Height = value; this.Refresh(); }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            string text = "";

            int iY = Convert.ToInt32((this.Height - iImageHeight - e.Graphics.MeasureString("Online", fOnlineFont).Height) / 2);

            int iN = 0;

            if (bTrack)
                iN += 20;

            if (Count > 0)
            {
                if (bTrack)
                    iN += 25 + 14;
                else
                    iN += 25 + 20 + 14;
            }

            if (bSelected)
            {
                text = TrimText(e.Graphics, sCaption, this.Width - iImageWidth - 12 - iN);

                e.Graphics.DrawString(text, fCaptionFont, brSelectedFontBrush, 0 + iImageWidth + 4, iY +
                                        (iImageHeight - e.Graphics.MeasureString(text, fCaptionFont).Height) / 2);
            }
            else
                if (bTrack)
            {
                text = TrimText(e.Graphics, sCaption, this.Width - iImageWidth - 12 - iN);

                e.Graphics.DrawString(text, fCaptionFont, brTrackFontBrush, 0 + iImageWidth + 4,
                                    (iImageHeight - e.Graphics.MeasureString(text, fCaptionFont).Height) / 2);

            }
            else
            {
                text = TrimText(e.Graphics, sCaption, this.Width - iImageWidth - 12 - iN);

                e.Graphics.DrawString(text, fCaptionFont, brCaptionBrush, 0 + iImageWidth + 4,
                                    (iImageHeight - e.Graphics.MeasureString(text, fCaptionFont).Height) / 2);
            }

            Image.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);

            if (Online)
                e.Graphics.DrawString("Online", fOnlineFont, brOnlineColorBrush, 2, iY + iImageHeight);

            e.Graphics.DrawImage(Image, 0, iY, iImageWidth, iImageHeight);

            if (bTrack)
            {
                CloseBMP.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);

                e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;

                e.Graphics.DrawImage(CloseBMP, this.Width - 12 - 20, 0, 15, 15);
            }

            rCountRect.X = this.Width - 12 - 20 - 4 - 25;
            rCountRect.Y = (iImageHeight - 20) / 2;
            rCountRect.Width = 25;
            rCountRect.Height = 20;

            if (Count > 0)
            {
                DrawRoundedRectangle(e.Graphics, rCountRect, 4, pCountPen, cCountBackColor);
                e.Graphics.DrawString(Count.ToString(), fCountFont, brCountFontBrush, this.Width - 12 - 20 - 4 - 25 + Convert.ToInt32((25 - e.Graphics.MeasureString(Count.ToString(), fCountFont).Width) / 2) + 1, (iImageHeight - 20) / 2 + 2);
            }
        }

        private string TrimText(Graphics g, string sText, int MaxWidth)
        {
            string sRes = "";

            if (g.MeasureString(sText, fCaptionFont).Width > MaxWidth)//this.Width - iImageWidth - 12 - 20 - 4 - 25 - 10
            {
                for (int i = sCaption.Length; i >= 0; i--)
                {
                    if (g.MeasureString(sText.Substring(0, i), fCaptionFont).Width <= MaxWidth)
                    {
                        sRes = sCaption.Substring(0, i - 1) + "...";
                        break;
                    }
                }
            }
            else
                sRes = sText;

            return sRes;
        }

        private void DrawRoundedRectangle(Graphics gfx, Rectangle Bounds, int CornerRadius, Pen DrawPen, Color FillColor)
        {
            gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            int strokeOffset = Convert.ToInt32(Math.Ceiling(DrawPen.Width));
            Bounds = Rectangle.Inflate(Bounds, -strokeOffset, -strokeOffset);

            DrawPen.EndCap = DrawPen.StartCap = System.Drawing.Drawing2D.LineCap.Round;

            System.Drawing.Drawing2D.GraphicsPath gfxPath = new System.Drawing.Drawing2D.GraphicsPath();
            gfxPath.AddArc(Bounds.X, Bounds.Y, CornerRadius, CornerRadius, 180, 90);
            gfxPath.AddArc(Bounds.X + Bounds.Width - CornerRadius, Bounds.Y, CornerRadius, CornerRadius, 270, 90);
            gfxPath.AddArc(Bounds.X + Bounds.Width - CornerRadius, Bounds.Y + Bounds.Height - CornerRadius, CornerRadius, CornerRadius, 0, 90);
            gfxPath.AddArc(Bounds.X, Bounds.Y + Bounds.Height - CornerRadius, CornerRadius, CornerRadius, 90, 90);
            gfxPath.CloseAllFigures();

            gfx.FillPath(new SolidBrush(FillColor), gfxPath);
            gfx.DrawPath(DrawPen, gfxPath);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!bTrack)
            {
                bTrack = true;
                this.Refresh();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.X >= this.Width - 12 - 20 && e.X <= this.Width - 12 - 20 + 15 && e.Y >= 0 && e.Y <= 15)
            {
                OnCloseClicked(this, UserID);
            }
            else
                OnItemClicked(this, Caption, UserID);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (bTrack)
            {
                bTrack = false;
                this.Refresh();
            }
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            Parent.Focus();
        }


        public event CloseClickedEventHandler CloseClicked;
        public event ItemClickedEventHandler ItemClicked;

        public delegate void CloseClickedEventHandler(object sender, int UserID);
        public delegate void ItemClickedEventHandler(object sender, string Name, int UserID);

        public virtual void OnCloseClicked(object sender, int UserID)
        {
            CloseClicked?.Invoke(sender, UserID);
        }

        public virtual void OnItemClicked(object sender, string Name, int UserID)
        {
            ItemClicked?.Invoke(sender, Name, UserID);
        }
    }


    public class InfiniumMessagesSelectedUsersList : Control
    {
        public int Offset = 0;

        int iTempScrollWheelOffset = 0;

        InfiniumScrollContainer ScrollContainer;
        public InfiniumProjectsVerticalScrollBar VerticalScroll;

        public InfiniumMessagesSelectedUserItem[] Items;

        public DataTable ItemsDT;

        bool bCloseVisible = false;

        public int iSelected = -1;

        int iMarginToNextItem = 3;

        public InfiniumMessagesSelectedUsersList()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            VerticalScroll = new InfiniumProjectsVerticalScrollBar()
            {
                Parent = this,
                Height = this.Height
            };
            VerticalScroll.Left = this.Width - VerticalScroll.Width;
            VerticalScroll.TotalControlHeight = this.Height;
            VerticalScroll.ScrollPositionChanged += OnScrollPositionChanged;

            ScrollContainer = new InfiniumScrollContainer()
            {
                Parent = this,
                Top = 0,
                Left = 0,
                Width = this.Width,
                Height = this.Height,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            ScrollContainer.SizeChanged += OnScrollContainerSizeChanged;

            this.BackColor = Color.Transparent;
        }

        public void InitializeItems()
        {
            if (Items != null)
            {
                for (int i = 0; i < Items.Count(); i++)
                {
                    if (Items[i] != null)
                    {
                        Items[i].Dispose();
                        Items[i] = null;
                    }
                }

                Items = new InfiniumMessagesSelectedUserItem[0];
            }

            Selected = -1;

            Offset = 0;
            ScrollContainer.Top = -Offset;
            VerticalScroll.Offset = Offset;
            VerticalScroll.Refresh();

            if (ItemsDT == null)
                return;

            if (ItemsDT.DefaultView.Count > 0)
            {
                Items = new InfiniumMessagesSelectedUserItem[ItemsDT.DefaultView.Count];

                for (int i = 0; i < ItemsDT.DefaultView.Count; i++)
                {
                    Items[i] = new InfiniumMessagesSelectedUserItem();

                    Items[i].Top = i * (Items[i].ItemHeight + iMarginToNextItem);
                    Items[i].Caption = ItemsDT.DefaultView[i]["Name"].ToString();
                    Items[i].UserID = Convert.ToInt32(ItemsDT.DefaultView[i]["UserID"]);
                    Items[i].Online = Convert.ToBoolean(ItemsDT.DefaultView[i]["Online"]);
                    Items[i].Count = Convert.ToInt32(ItemsDT.DefaultView[i]["Count"]);

                    if (ItemsDT.DefaultView[i]["Photo"] != DBNull.Value)
                    {
                        byte[] b = (byte[])ItemsDT.DefaultView[i]["Photo"];

                        using (MemoryStream ms = new MemoryStream(b))
                        {
                            Items[i].Image = (Bitmap)Image.FromStream(ms);
                        }
                    }
                    else
                        Items[i].Image = Properties.Resources.NoImage;

                    Items[i].Parent = ScrollContainer;
                    Items[i].Width = ScrollContainer.Width;
                    Items[i].Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                    Items[i].ItemClicked += ItemClick;
                    Items[i].CloseClicked += OnCloseClicked;
                }
            }

            SetScrollHeight();
        }

        //public void AddUser()
        //{
        //    foreach (DataRow Row in ItemsDataTable.Rows)
        //    {
        //        for (int i = 0; i < Items.Count(); i++)
        //        {
        //            if (Items[i].UserID == Convert.ToInt32(Row["UserID"]))
        //            {
        //                break;
        //            }

        //            if (i == Items.Count() - 1)//new user
        //            { 

        //            }
        //        } 
        //    }          
        //}

        private void SetScrollHeight()
        {
            if (Items == null || ItemsDataTable.DefaultView.Count == 0)
            {
                ScrollContainer.Height = this.Height;
                VerticalScroll.TotalControlHeight = ScrollContainer.Height;
                VerticalScroll.Refresh();

                return;
            }

            if (Items.Count() * (Items[0].Height + iMarginToNextItem) > this.Height)
                ScrollContainer.Height = Items.Count() * (Items[0].Height + iMarginToNextItem);
            else
                ScrollContainer.Height = this.Height;

            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
            VerticalScroll.Refresh();
        }

        public void ScrollDown()
        {
            Offset = ScrollContainer.Height - this.Height;
            ScrollContainer.Top = -Offset;
            VerticalScroll.Offset = Offset;
            VerticalScroll.Refresh();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public InfiniumProjectsVerticalScrollBar VerticalScrollBar
        {
            get { return VerticalScroll; }
            set { VerticalScroll = value; VerticalScroll.Left = this.Width - VerticalScroll.Width; }
        }


        public DataTable ItemsDataTable
        {
            get
            {
                //if (ItemsDT == null)
                //    return ItemsDT;

                //if (Items.Count() > 0)
                //    for (int i = 0; i < Items.Count(); i++)
                //    {
                //        if (Items[i] == null)
                //            continue;

                //        if (Items[i].Checked)
                //            ItemsDT.Rows[i]["Checked"] = true;
                //        else
                //            ItemsDT.Rows[i]["Checked"] = false;
                //    }

                return ItemsDT;
            }
            set
            {
                ItemsDT = value;
            }
        }

        public bool CloseVisible
        {
            get { return bCloseVisible; }
            set { bCloseVisible = value; }
        }

        public int Selected
        {
            get { return iSelected; }
            set
            {
                if (value > -1)
                {
                    iSelected = value;
                    if (Items != null)
                        if (Items.Count() > 0)
                        {
                            Items[iSelected].Selected = true;
                            OnItemClicked(Items[value], Items[value].Name, Items[value].UserID);
                        }
                }
                else
                {
                    if (iSelected > -1 && Items.Count() > 0)
                    {
                        Items[iSelected].Selected = false;
                        Items[iSelected].Refresh();
                    }
                    iSelected = value;
                }
            }
        }

        public void SelectOnly(int i)
        {
            if (i > -1)
            {
                iSelected = i;
                if (Items != null)
                    if (Items.Count() > 0)
                    {
                        Items[iSelected].Selected = true;
                    }
            }
            else
            {
                if (iSelected > -1 && Items.Count() > 0)
                {
                    Items[iSelected].Selected = false;
                    Items[iSelected].Refresh();
                }
                iSelected = i;
            }
        }

        private void OnScrollContainerSizeChanged(object sender, EventArgs e)
        {
            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
        }

        private void OnScrollPositionChanged(object sender, int tOffset)
        {
            Offset = tOffset;
            ScrollContainer.Top = -Offset;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (!VerticalScroll.Visible)
                return;

            if (Offset == 0)
                if (iTempScrollWheelOffset > 0)
                    iTempScrollWheelOffset = 0;

            if (e.Delta < 0)
            {
                if (Offset < ScrollContainer.Height - this.Height)
                {
                    if (Offset + VerticalScroll.ScrollWheelOffset + this.Height > ScrollContainer.Height)//last
                    {
                        iTempScrollWheelOffset = ScrollContainer.Height - this.Height - Offset;
                        Offset += iTempScrollWheelOffset;
                    }
                    else
                    {
                        Offset += VerticalScroll.ScrollWheelOffset;
                    }

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
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
                        Offset -= VerticalScroll.ScrollWheelOffset;

                    if (Offset < 0)
                        Offset = 0;

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
                }
        }

        private void ItemClick(object sender, string Name, int UserID)
        {
            for (int i = 0; i < Items.Count(); i++)
            {
                if (Items[i] == sender)
                {
                    iSelected = i;
                    Items[i].Selected = true;
                }
                else
                    Items[i].Selected = false;
            }

            OnItemClicked(sender, Name, UserID);
        }

        public event ItemClickedEventHandler ItemClicked;
        public event CloseClickedEventHandler CloseClicked;

        public delegate void ItemClickedEventHandler(object sender, string Name, int UserID);
        public delegate void CloseClickedEventHandler(object sender, int UserID);


        public virtual void OnCloseClicked(object sender, int UserID)
        {
            CloseClicked?.Invoke(sender, UserID);
        }

        public virtual void OnItemClicked(object sender, string Name, int UserID)
        {
            ItemClicked?.Invoke(sender, Name, UserID);
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            ScrollContainer.Focus();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            SetScrollHeight();

            VerticalScroll.Left = this.Width - VerticalScroll.Width;
        }
    }




    public class InfiniumMessagesUserItem : Control
    {
        Color cCaptionColor = Color.FromArgb(70, 70, 70);
        Color cSelectedFontColor = Color.FromArgb(56, 184, 238);
        Color cTrackColor = Color.FromArgb(120, 120, 120);
        Color cOnlineColor = Color.FromArgb(56, 184, 238);

        Font fCaptionFont = new Font("Segoe UI", 17.0f, FontStyle.Regular, GraphicsUnit.Pixel);
        Font fOnlineFont = new Font("Segoe UI", 12.0f, FontStyle.Regular, GraphicsUnit.Pixel);

        SolidBrush brCaptionBrush;
        SolidBrush brSelectedFontBrush;
        SolidBrush brTrackFontBrush;
        SolidBrush brOnlineColorBrush;

        public Bitmap Image = null;

        string sCaption = "Item";
        public int UserID = -1;

        public bool Online = false;

        int iItemHeight = 58;
        bool bTrack = false;

        int iImageHeight = 40;
        int iImageWidth = 43;

        Pen pBorderPen;

        bool bSelected = false;

        public InfiniumMessagesUserItem()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            this.BackColor = Color.Transparent;
            this.Cursor = Cursors.Hand;

            brCaptionBrush = new SolidBrush(cCaptionColor);
            brSelectedFontBrush = new SolidBrush(cSelectedFontColor);
            brTrackFontBrush = new SolidBrush(cTrackColor);
            brOnlineColorBrush = new SolidBrush(cOnlineColor);

            pBorderPen = new Pen(new SolidBrush(cTrackColor));

            this.Height = iItemHeight;
            this.Width = 50;
        }

        public bool Selected
        {
            get { return bSelected; }
            set { bSelected = value; this.Refresh(); }
        }

        public string Caption
        {
            get { return sCaption; }
            set { sCaption = value; this.Refresh(); }
        }

        public int ItemHeight
        {
            get { return iItemHeight; }
            set { iItemHeight = value; this.Height = value; this.Refresh(); }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            string text = "";

            int iY = Convert.ToInt32((this.Height - iImageHeight - e.Graphics.MeasureString("Online", fOnlineFont).Height) / 2);

            if (e.Graphics.MeasureString(sCaption, fCaptionFont).Width > this.Width - 10)
            {
                for (int i = sCaption.Length; i >= 0; i--)
                {
                    if (e.Graphics.MeasureString(sCaption.Substring(0, i), fCaptionFont).Width <= this.Width - 10)
                    {
                        text = sCaption.Substring(0, i - 1) + "...";
                        break;
                    }
                }
            }
            else
                text = sCaption;

            if (bSelected)
                e.Graphics.DrawString(sCaption, fCaptionFont, brSelectedFontBrush, 0 + iImageWidth + 4, iY +
                                        (iImageHeight - e.Graphics.MeasureString(sCaption, fCaptionFont).Height) / 2);
            else
                if (bTrack)
            {
                e.Graphics.DrawString(sCaption, fCaptionFont, brTrackFontBrush, 0 + iImageWidth + 4,
                                    (iImageHeight - e.Graphics.MeasureString(sCaption, fCaptionFont).Height) / 2);


            }
            else
                e.Graphics.DrawString(sCaption, fCaptionFont, brCaptionBrush, 0 + iImageWidth + 4,
                                    (iImageHeight - e.Graphics.MeasureString(sCaption, fCaptionFont).Height) / 2);


            Image.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);

            if (Online)
                e.Graphics.DrawString("Online", fOnlineFont, brOnlineColorBrush, 2, iY + iImageHeight);

            e.Graphics.DrawImage(Image, 0, iY, iImageWidth, iImageHeight);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!bTrack)
            {
                bTrack = true;
                this.Refresh();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            OnItemClicked(this, Caption, UserID);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (bTrack)
            {
                bTrack = false;
                this.Refresh();
            }
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            Parent.Focus();
        }


        public event ItemClickedEventHandler ItemClicked;

        //public delegate void CloseClickedEventHandler(object sender, int UserID);
        public delegate void ItemClickedEventHandler(object sender, string Name, int UserID);

        //public virtual void OnCloseClicked(object sender, int UserID)
        //{
        //    if (CloseClicked != null)
        //        CloseClicked(sender, UserID);
        //}

        public virtual void OnItemClicked(object sender, string Name, int UserID)
        {
            ItemClicked?.Invoke(sender, Name, UserID);
        }
    }


    public class InfiniumMessagesUsersList : Control
    {
        public int Offset = 0;

        int iTempScrollWheelOffset = 0;

        InfiniumScrollContainer ScrollContainer;
        public InfiniumProjectsVerticalScrollBar VerticalScroll;

        public InfiniumMessagesUserItem[] Items;

        public DataTable ItemsDT;

        bool bCloseVisible = false;

        public int iSelected = -1;

        int iMarginToNextItem = 3;

        public InfiniumMessagesUsersList()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            VerticalScroll = new InfiniumProjectsVerticalScrollBar()
            {
                Parent = this,
                Height = this.Height
            };
            VerticalScroll.Left = this.Width - VerticalScroll.Width;
            VerticalScroll.TotalControlHeight = this.Height;
            VerticalScroll.ScrollPositionChanged += OnScrollPositionChanged;

            ScrollContainer = new InfiniumScrollContainer()
            {
                Parent = this,
                Top = 0,
                Left = 0,
                Width = this.Width,
                Height = this.Height,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            ScrollContainer.SizeChanged += OnScrollContainerSizeChanged;

            this.BackColor = Color.Transparent;
        }

        public void InitializeItems()
        {
            if (Items != null)
            {
                for (int i = 0; i < Items.Count(); i++)
                {
                    if (Items[i] != null)
                    {
                        Items[i].Dispose();
                        Items[i] = null;
                    }
                }

                Items = new InfiniumMessagesUserItem[0];
            }

            Selected = -1;

            Offset = 0;
            ScrollContainer.Top = -Offset;
            VerticalScroll.Offset = Offset;
            VerticalScroll.Refresh();

            if (ItemsDT == null)
                return;

            if (ItemsDT.DefaultView.Count > 0)
            {
                Items = new InfiniumMessagesUserItem[ItemsDT.DefaultView.Count];

                for (int i = 0; i < ItemsDT.DefaultView.Count; i++)
                {
                    Items[i] = new InfiniumMessagesUserItem();

                    Items[i].Top = i * (Items[i].ItemHeight + iMarginToNextItem);
                    Items[i].Caption = ItemsDT.DefaultView[i]["Name"].ToString();
                    Items[i].UserID = Convert.ToInt32(ItemsDT.DefaultView[i]["UserID"]);
                    Items[i].Online = Convert.ToBoolean(ItemsDT.DefaultView[i]["Online"]);

                    if (ItemsDT.DefaultView[i]["Photo"] != DBNull.Value)
                    {
                        byte[] b = (byte[])ItemsDT.DefaultView[i]["Photo"];

                        using (MemoryStream ms = new MemoryStream(b))
                        {
                            Items[i].Image = (Bitmap)Image.FromStream(ms);
                        }
                    }
                    else
                        Items[i].Image = Properties.Resources.NoImage;

                    Items[i].Parent = ScrollContainer;
                    Items[i].Width = ScrollContainer.Width;
                    Items[i].Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                    Items[i].ItemClicked += ItemClick;
                }
            }

            SetScrollHeight();
        }

        private void SetScrollHeight()
        {
            if (Items == null || ItemsDataTable.DefaultView.Count == 0)
            {
                ScrollContainer.Height = this.Height;
                VerticalScroll.TotalControlHeight = ScrollContainer.Height;
                VerticalScroll.Refresh();

                return;
            }

            if (Items.Count() * (Items[0].Height + iMarginToNextItem) > this.Height)
                ScrollContainer.Height = Items.Count() * (Items[0].Height + iMarginToNextItem);
            else
                ScrollContainer.Height = this.Height;

            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
            VerticalScroll.Refresh();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public InfiniumProjectsVerticalScrollBar VerticalScrollBar
        {
            get { return VerticalScroll; }
            set { VerticalScroll = value; VerticalScroll.Left = this.Width - VerticalScroll.Width; }
        }


        public DataTable ItemsDataTable
        {
            get
            {
                //if (ItemsDT == null)
                //    return ItemsDT;

                //if (Items.Count() > 0)
                //    for (int i = 0; i < Items.Count(); i++)
                //    {
                //        if (Items[i] == null)
                //            continue;

                //        if (Items[i].Checked)
                //            ItemsDT.Rows[i]["Checked"] = true;
                //        else
                //            ItemsDT.Rows[i]["Checked"] = false;
                //    }

                return ItemsDT;
            }
            set
            {
                ItemsDT = value;
            }
        }

        public bool CloseVisible
        {
            get { return bCloseVisible; }
            set { bCloseVisible = value; }
        }

        public int Selected
        {
            get { return iSelected; }
            set
            {
                if (value > -1)
                {
                    iSelected = value;
                    if (Items != null)
                        if (Items.Count() > 0)
                        {
                            Items[iSelected].Selected = true;
                            OnItemClicked(Items[value], Items[value].Name, Items[value].UserID);
                        }
                }
                else
                {
                    if (iSelected > -1)
                    {
                        Items[iSelected].Selected = false;
                        Items[iSelected].Refresh();
                    }
                    iSelected = value;
                }
            }
        }

        private void OnScrollContainerSizeChanged(object sender, EventArgs e)
        {
            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
        }

        private void OnScrollPositionChanged(object sender, int tOffset)
        {
            Offset = tOffset;
            ScrollContainer.Top = -Offset;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (!VerticalScroll.Visible)
                return;

            if (Offset == 0)
                if (iTempScrollWheelOffset > 0)
                    iTempScrollWheelOffset = 0;

            if (e.Delta < 0)
            {
                if (Offset < ScrollContainer.Height - this.Height)
                {
                    if (Offset + VerticalScroll.ScrollWheelOffset + this.Height > ScrollContainer.Height)//last
                    {
                        iTempScrollWheelOffset = ScrollContainer.Height - this.Height - Offset;
                        Offset += iTempScrollWheelOffset;
                    }
                    else
                    {
                        Offset += VerticalScroll.ScrollWheelOffset;
                    }

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
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
                        Offset -= VerticalScroll.ScrollWheelOffset;

                    if (Offset < 0)
                        Offset = 0;

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
                }
        }

        private void ItemClick(object sender, string Name, int UserID)
        {
            for (int i = 0; i < Items.Count(); i++)
            {
                if (Items[i] == sender)
                {
                    iSelected = i;
                    Items[i].Selected = true;
                }
                else
                    Items[i].Selected = false;
            }

            OnItemClicked(sender, Name, UserID);
        }

        public event ItemClickedEventHandler ItemClicked;
        public event CloseClickedEventHandler CloseClicked;

        public delegate void ItemClickedEventHandler(object sender, string Name, int UserID);
        public delegate void CloseClickedEventHandler(object sender, int UserID);


        public virtual void OnCloseClicked(object sender, int UserID)
        {
            CloseClicked?.Invoke(sender, UserID);
        }

        public virtual void OnItemClicked(object sender, string Name, int UserID)
        {
            ItemClicked?.Invoke(sender, Name, UserID);
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            ScrollContainer.Focus();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            VerticalScroll.Left = this.Width - VerticalScroll.Width;
            VerticalScroll.Height = this.Height;

            SetScrollHeight();
        }
    }





    public class InfiniumMessageItem : Control
    {
        int iCurTextPosY = 0;
        int iMarginTextRows = 5;

        Color cMeTextColor = Color.FromArgb(0, 0, 0);
        Color cMeColor = Color.FromArgb(0, 120, 202);
        Color cSenderTextColor = Color.FromArgb(80, 80, 80);
        Color cDateColor = Color.FromArgb(140, 140, 140);

        Font fTextFont = new Font("Segoe UI", 17.0f, FontStyle.Regular, GraphicsUnit.Pixel);
        Font fSenderFont = new Font("Segoe UI", 15.0f, FontStyle.Regular, GraphicsUnit.Pixel);
        Font fDateFont = new Font("Segoe UI", 15.0f, FontStyle.Regular, GraphicsUnit.Pixel);

        SolidBrush brMeTextBrush;
        SolidBrush brSenderTextFontBrush;
        SolidBrush brDateBrush;
        SolidBrush brMeBrush;

        public string DateTime = "";
        string sMessage = "";
        public string Sender = "";

        public bool Me = false;

        int iMarginForDate = 140;
        int iMarginForSender = 120;
        int iMarginForMessage = 20;

        public InfiniumMessageItem()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            this.BackColor = Color.Transparent;

            brMeTextBrush = new SolidBrush(cMeTextColor);
            brSenderTextFontBrush = new SolidBrush(cSenderTextColor);
            brDateBrush = new SolidBrush(cDateColor);
            brMeBrush = new SolidBrush(cMeColor);

            this.Height = 3;
            this.Width = 50;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            if (Me)
            {
                e.Graphics.DrawString(Sender, fSenderFont, brMeBrush, iMarginForSender - e.Graphics.MeasureString(Sender, fSenderFont).Width, 0);
                DrawText(brMeTextBrush, e.Graphics, ref iCurTextPosY);
                e.Graphics.DrawString(DateTime, fDateFont, brMeTextBrush, this.Width - iMarginForDate - 12, 0);
            }
            else
            {
                e.Graphics.DrawString(Sender, fSenderFont, brSenderTextFontBrush, iMarginForSender - e.Graphics.MeasureString(Sender, fSenderFont).Width, 0);
                DrawText(brSenderTextFontBrush, e.Graphics, ref iCurTextPosY);
                e.Graphics.DrawString(DateTime, fDateFont, brSenderTextFontBrush, this.Width - iMarginForDate - 12, 0);
            }


        }

        public string Message
        {
            get { return sMessage; }
            set { sMessage = value; this.Height = GetInitialHeight(); this.Refresh(); }
        }

        private int GetInitialHeight()
        {
            using (Graphics G = this.CreateGraphics())
            {
                if (sMessage.Length == 0)
                    return 0;

                int TextMaxWidth = this.Width - iMarginForDate - iMarginForSender - iMarginForMessage - iMarginForMessage - 12;

                int CurrentY = 0;

                string CurrentRowString = "";

                for (int i = 0; i < sMessage.Length; i++)
                {
                    if (sMessage[i] == '\n')
                    {
                        CurrentRowString = "";
                        CurrentY++;
                        continue;
                    }

                    if (i == sMessage.Length - 1)
                    {
                        CurrentY++;
                    }
                    else
                    {
                        if (G.MeasureString(CurrentRowString, fTextFont).Width > TextMaxWidth)
                        {
                            int LastSpace = GetLastSpace(CurrentRowString);

                            if (LastSpace == 0)
                            {

                            }
                            else
                            {
                                i -= (CurrentRowString.Length - LastSpace);
                            }


                            CurrentRowString = "";
                            CurrentY++;

                            continue;
                        }
                    }

                    CurrentRowString += sMessage[i];
                }

                int C = CurrentY * Convert.ToInt32((G.MeasureString("String", fTextFont).Height) + iMarginTextRows);

                return C;
            }
        }


        private int DrawText(SolidBrush Brush, Graphics G, ref int CurTextPosY)
        {
            if (sMessage.Length == 0)
                return 0;

            int TextMaxWidth = this.Width - iMarginForDate - iMarginForSender - iMarginForMessage - iMarginForMessage - 12;

            int CurrentY = 0;

            string CurrentRowString = "";

            for (int i = 0; i < sMessage.Length; i++)
            {
                if (sMessage[i] == '\n')
                {
                    G.DrawString(CurrentRowString, fTextFont, Brush,
                                 iMarginForSender + iMarginForMessage, CurTextPosY +
                                 (CurrentY * (G.MeasureString("String", fTextFont).Height + iMarginTextRows)) - 1);
                    CurrentRowString = "";
                    CurrentY++;
                    continue;
                }

                if (i == sMessage.Length - 1)
                {
                    G.DrawString(CurrentRowString += sMessage[i], fTextFont, Brush,
                                 iMarginForSender + iMarginForMessage, CurTextPosY +
                                 (CurrentY * (G.MeasureString("String", fTextFont).Height + iMarginTextRows)) - 1);
                    CurrentY++;
                }
                else
                {
                    if (G.MeasureString(CurrentRowString, fTextFont).Width > TextMaxWidth)
                    {
                        int LastSpace = GetLastSpace(CurrentRowString);

                        if (LastSpace == 0)
                        {
                            G.DrawString(CurrentRowString, fTextFont, Brush,
                                         iMarginForSender + iMarginForMessage, CurTextPosY +
                                        (CurrentY * (G.MeasureString("String", fTextFont).Height + iMarginTextRows)) - 1);
                        }
                        else
                        {
                            G.DrawString(CurrentRowString.Substring(0, LastSpace + 1), fTextFont, Brush,
                                         iMarginForSender + iMarginForMessage, CurTextPosY +
                                         (CurrentY * (G.MeasureString("String", fTextFont).Height + iMarginTextRows)) - 1);

                            i -= (CurrentRowString.Length - LastSpace);
                        }


                        CurrentRowString = "";
                        CurrentY++;

                        continue;
                    }
                }

                CurrentRowString += sMessage[i];
            }

            int C = CurrentY * Convert.ToInt32((G.MeasureString("String", fTextFont).Height) + iMarginTextRows);

            return C;
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


        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            Parent.Focus();
        }
    }


    public class InfiniumMessagesContainer : Control
    {
        public int Offset = 0;

        int iTempScrollWheelOffset = 0;

        InfiniumScrollContainer ScrollContainer;
        public InfiniumProjectsVerticalScrollBar VerticalScroll;

        public InfiniumMessageItem[] Items;

        public DataTable ItemsDT;
        public DataTable UsersDataTable;

        int iMarginToNextItem = 14;

        public int CurrentUserID = -1;

        public InfiniumMessagesContainer()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            VerticalScroll = new InfiniumProjectsVerticalScrollBar()
            {
                Parent = this,
                Height = this.Height
            };
            VerticalScroll.Left = this.Width - VerticalScroll.Width;
            VerticalScroll.TotalControlHeight = this.Height;
            VerticalScroll.ScrollPositionChanged += OnScrollPositionChanged;

            ScrollContainer = new InfiniumScrollContainer()
            {
                Parent = this,
                Top = 0,
                Left = 0,
                Width = this.Width,
                Height = this.Height,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            ScrollContainer.SizeChanged += OnScrollContainerSizeChanged;

            this.BackColor = Color.Transparent;
        }

        public void InitializeItems()
        {
            if (Items != null)
            {
                for (int i = 0; i < Items.Count(); i++)
                {
                    if (Items[i] != null)
                    {
                        Items[i].Dispose();
                        Items[i] = null;
                    }
                }

                Items = new InfiniumMessageItem[0];
            }

            Offset = 0;
            ScrollContainer.Top = -Offset;
            VerticalScroll.Offset = Offset;
            VerticalScroll.Refresh();

            if (ItemsDT == null)
                return;

            int iPosY = 0;

            if (ItemsDT.DefaultView.Count > 0)
            {
                Items = new InfiniumMessageItem[ItemsDT.DefaultView.Count];

                for (int i = 0; i < ItemsDT.DefaultView.Count; i++)
                {
                    Items[i] = new InfiniumMessageItem()
                    {
                        Top = iPosY,
                        Parent = ScrollContainer,
                        Width = ScrollContainer.Width,
                        Message = ItemsDT.DefaultView[i]["Text"].ToString(),
                        DateTime = Convert.ToDateTime(ItemsDT.DefaultView[i]["SendDateTime"]).ToString("dd.MM.yyyy HH:mm:ss"),
                        Sender = GetName(UsersDataTable.Select("UserID = " + ItemsDT.DefaultView[i]["SenderUserID"])[0]["Name"].ToString())
                    };
                    if (Convert.ToInt32(ItemsDT.DefaultView[i]["SenderUserID"]) == CurrentUserID)
                        Items[i].Me = true;
                    else
                        Items[i].Me = false;

                    Items[i].Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                    Items[i].Refresh();

                    iPosY += Items[i].Height + iMarginToNextItem;
                }
            }

            SetScrollHeight(iPosY);

            ScrollDown();
        }

        public void ScrollDown()
        {
            //if (Offset < ScrollContainer.Height - this.Height)

            Offset = ScrollContainer.Height - this.Height;
            ScrollContainer.Top = -Offset;
            VerticalScroll.Offset = Offset;
            VerticalScroll.Refresh();
        }

        private void SetScrollHeight(int iPosY)
        {
            ScrollContainer.Height = iPosY;

            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
            VerticalScroll.Refresh();
        }


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public InfiniumProjectsVerticalScrollBar VerticalScrollBar
        {
            get { return VerticalScroll; }
            set { VerticalScroll = value; VerticalScroll.Left = this.Width - VerticalScroll.Width; }
        }


        public DataTable ItemsDataTable
        {
            get
            {
                //if (ItemsDT == null)
                //    return ItemsDT;

                //if (Items.Count() > 0)
                //    for (int i = 0; i < Items.Count(); i++)
                //    {
                //        if (Items[i] == null)
                //            continue;

                //        if (Items[i].Checked)
                //            ItemsDT.Rows[i]["Checked"] = true;
                //        else
                //            ItemsDT.Rows[i]["Checked"] = false;
                //    }

                return ItemsDT;
            }
            set
            {
                ItemsDT = value;
            }
        }

        public string GetName(string UserName)
        {
            int c = 0;

            string Name = "";

            for (int i = 0; i < UserName.Length; i++)
            {
                if (UserName[i] != ' ')
                {
                    if (c == 0)
                        continue;
                    else
                        Name += UserName[i];
                }
                else
                {
                    if (c == 0)
                        c++;
                    else
                        break;
                }
            }

            return Name;
        }

        private void OnScrollContainerSizeChanged(object sender, EventArgs e)
        {
            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
        }

        private void OnScrollPositionChanged(object sender, int tOffset)
        {
            Offset = tOffset;
            ScrollContainer.Top = -Offset;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (!VerticalScroll.Visible)
                return;

            if (Offset == 0)
                if (iTempScrollWheelOffset > 0)
                    iTempScrollWheelOffset = 0;

            if (e.Delta < 0)
            {
                if (Offset < ScrollContainer.Height - this.Height)
                {
                    if (Offset + VerticalScroll.ScrollWheelOffset + this.Height > ScrollContainer.Height)//last
                    {
                        iTempScrollWheelOffset = ScrollContainer.Height - this.Height - Offset;
                        Offset += iTempScrollWheelOffset;
                    }
                    else
                    {
                        Offset += VerticalScroll.ScrollWheelOffset;
                    }

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
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
                        Offset -= VerticalScroll.ScrollWheelOffset;

                    if (Offset < 0)
                        Offset = 0;

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
                }
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            ScrollContainer.Focus();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            VerticalScroll.Left = this.Width - VerticalScroll.Width;
        }
    }







    public class InfiniumDocumentsSelectUserItem : Control
    {
        Color cCaptionColor = Color.FromArgb(60, 60, 60);
        Color cSelectedCaptionColor = Color.FromArgb(56, 184, 238);

        Color cBackColor = Color.White;
        Color cBackSelectedColor = Color.White;

        Color cBorderColor = Color.FromArgb(240, 240, 240);

        Pen pBorderPen;

        Font fCaptionFont = new Font("Segoe UI", 18.0f, FontStyle.Regular, GraphicsUnit.Pixel);
        Font fFileSizeFont = new Font("Segoe UI", 14.0f, FontStyle.Regular, GraphicsUnit.Pixel);

        SolidBrush brCaptionBrush;
        SolidBrush brSelectedCaptionBrush;

        bool bChecked = false;

        int iCheckHeight = 0;
        int iCheckWidth = 0;
        int iCheckWidthWithMargin = 0;

        string sCaption = "Item";

        public int UserID = -1;

        int iItemHeight = 50;

        Bitmap FileCheckedBMP = Properties.Resources.FileChecked;
        Bitmap FileUncheckedBMP = Properties.Resources.FileUnchecked;


        public InfiniumDocumentsSelectUserItem()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            this.BackColor = Color.White;
            this.Cursor = Cursors.Hand;

            brCaptionBrush = new SolidBrush(cCaptionColor);
            brSelectedCaptionBrush = new SolidBrush(cSelectedCaptionColor);

            pBorderPen = new Pen(new SolidBrush(cBorderColor));

            this.Height = iItemHeight;
            this.Width = 50;
        }


        public Color BorderColor
        {
            get { return cBorderColor; }
            set { cBorderColor = value; pBorderPen.Color = value; this.Refresh(); }
        }

        public Color CaptionColor
        {
            get { return cCaptionColor; }
            set { cCaptionColor = value; brCaptionBrush.Color = cCaptionColor; this.Refresh(); }
        }

        public Color SelectedCaptionColor
        {
            get { return cSelectedCaptionColor; }
            set { cSelectedCaptionColor = value; brSelectedCaptionBrush.Color = cSelectedCaptionColor; this.Refresh(); }
        }

        public Color BackSelectedColor
        {
            get { return cBackSelectedColor; }
            set { cBackSelectedColor = value; this.Refresh(); }
        }

        public Font CaptionFont
        {
            get { return fCaptionFont; }
            set { fCaptionFont = value; this.Refresh(); }
        }

        public bool Checked
        {
            get { return bChecked; }
            set
            {
                bChecked = value;

                this.Refresh();
            }
        }


        public string Caption
        {
            get { return sCaption; }
            set { sCaption = value; this.Refresh(); }
        }

        public int ItemHeight
        {
            get { return iItemHeight; }
            set { iItemHeight = value; this.Height = value; this.Refresh(); }
        }


        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.X >= 5 && e.X <= 5 + iCheckWidth)
            {
                bChecked = !bChecked;
                this.Refresh();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;


            iCheckHeight = 28;
            iCheckWidth = FileCheckedBMP.Width * iCheckHeight / FileCheckedBMP.Height;
            iCheckWidthWithMargin = iCheckWidth + 15;

            if (bChecked)
            {
                FileCheckedBMP.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);
                e.Graphics.DrawImage(FileCheckedBMP, 5, (this.Height - iCheckHeight) / 2 - 2,
                                        iCheckWidth, iCheckHeight);
            }
            else
            {
                FileUncheckedBMP.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);
                e.Graphics.DrawImage(FileUncheckedBMP, 5, (this.Height - iCheckHeight) / 2 - 2,
                                        iCheckWidth, iCheckHeight);
            }


            string text = "";

            if (e.Graphics.MeasureString(sCaption, fCaptionFont).Width > this.Width - iCheckWidth)
            {
                for (int i = sCaption.Length; i >= 0; i--)
                {
                    if (e.Graphics.MeasureString(sCaption.Substring(0, i), fCaptionFont).Width <= this.Width - iCheckWidth)
                    {
                        text = sCaption.Substring(0, i - 1) + "...";
                        break;
                    }
                }
            }
            else
                text = sCaption;


            e.Graphics.DrawString(text, fCaptionFont, brCaptionBrush, 5 + iCheckWidthWithMargin,
                                      (this.Height - e.Graphics.MeasureString(sCaption, fCaptionFont).Height) / 2 - 1);


            e.Graphics.DrawLine(pBorderPen, 0, this.Height, this.Width, this.Height);
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            Parent.Focus();
        }
    }


    public class InfiniumDocumentsSelectUsersList : Control
    {
        public int Offset = 0;

        int iTempScrollWheelOffset = 0;

        InfiniumScrollContainer ScrollContainer;
        public InfiniumProjectsVerticalScrollBar VerticalScroll;

        public InfiniumDocumentsSelectUserItem[] Items;

        DataTable ItemsDT;

        public InfiniumDocumentsSelectUsersList()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);

            VerticalScroll = new InfiniumProjectsVerticalScrollBar()
            {
                Parent = this,
                Height = this.Height
            };
            VerticalScroll.Left = this.Width - VerticalScroll.Width;
            VerticalScroll.TotalControlHeight = this.Height;
            VerticalScroll.ScrollPositionChanged += OnScrollPositionChanged;

            ScrollContainer = new InfiniumScrollContainer()
            {
                Parent = this,
                Top = 0,
                Left = 0,
                Width = this.Width,
                Height = this.Height,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            ScrollContainer.SizeChanged += OnScrollContainerSizeChanged;
        }

        public void InitializeItems()
        {
            if (Items != null)
            {
                for (int i = 0; i < Items.Count(); i++)
                {
                    if (Items[i] != null)
                    {
                        Items[i].Dispose();
                        Items[i] = null;
                    }
                }

                Items = new InfiniumDocumentsSelectUserItem[0];
            }

            Offset = 0;
            ScrollContainer.Top = -Offset;
            VerticalScroll.Offset = Offset;
            VerticalScroll.Refresh();

            if (ItemsDT == null)
                return;

            if (ItemsDT.DefaultView.Count > 0)
            {
                Items = new InfiniumDocumentsSelectUserItem[ItemsDT.DefaultView.Count];

                for (int i = 0; i < ItemsDT.DefaultView.Count; i++)
                {
                    Items[i] = new InfiniumDocumentsSelectUserItem()
                    {
                        Parent = ScrollContainer
                    };
                    Items[i].Top = i * Items[i].ItemHeight;
                    Items[i].Caption = ItemsDT.DefaultView[i]["Name"].ToString();
                    Items[i].UserID = Convert.ToInt32(ItemsDT.DefaultView[i]["UserID"]);
                    Items[i].Width = ScrollContainer.Width;
                    Items[i].Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                }
            }

            SetScrollHeight();

        }

        private void SetScrollHeight()
        {
            if (Items == null || ItemsDataTable.DefaultView.Count == 0)
            {
                ScrollContainer.Height = this.Height;
                VerticalScroll.TotalControlHeight = ScrollContainer.Height;
                VerticalScroll.Refresh();

                return;
            }

            if (Items.Count() * Items[0].Height > this.Height)
                ScrollContainer.Height = Items.Count() * Items[0].Height;
            else
                ScrollContainer.Height = this.Height;

            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
            VerticalScroll.Refresh();
        }


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public InfiniumProjectsVerticalScrollBar VerticalScrollBar
        {
            get { return VerticalScroll; }
            set { VerticalScroll = value; VerticalScroll.Left = this.Width - VerticalScroll.Width; }
        }


        public DataTable ItemsDataTable
        {
            get
            {
                //if (ItemsDT == null)
                //    return ItemsDT;

                //if (Items.Count() > 0)
                //    for (int i = 0; i < Items.Count(); i++)
                //    {
                //        if (Items[i] == null)
                //            continue;

                //        if (Items[i].Checked)
                //            ItemsDT.Rows[i]["Checked"] = true;
                //        else
                //            ItemsDT.Rows[i]["Checked"] = false;
                //    }

                return ItemsDT;
            }
            set
            {
                ItemsDT = value;
            }
        }

        public void SelectItem(int UserID)
        {
            Items[ItemsDataTable.Rows.IndexOf(ItemsDataTable.Select("UserID = " + UserID)[0])].Checked = true;
        }

        public DataTable GetSelectedDataTable()
        {
            using (DataTable DT = new DataTable())
            {
                DT.Columns.Add(new DataColumn("UserID", Type.GetType("System.Int32")));

                foreach (InfiniumDocumentsSelectUserItem Item in Items)
                {
                    if (Item.Checked)
                    {
                        DataRow NewRow = DT.NewRow();
                        NewRow["UserID"] = Item.UserID;
                        DT.Rows.Add(NewRow);
                    }
                }

                return DT;
            }
        }

        private void OnScrollContainerSizeChanged(object sender, EventArgs e)
        {
            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
        }

        private void OnScrollPositionChanged(object sender, int tOffset)
        {
            Offset = tOffset;
            ScrollContainer.Top = -Offset;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (!VerticalScroll.Visible)
                return;

            if (Offset == 0)
                if (iTempScrollWheelOffset > 0)
                    iTempScrollWheelOffset = 0;

            if (e.Delta < 0)
            {
                if (Offset < ScrollContainer.Height - this.Height)
                {
                    if (Offset + VerticalScroll.ScrollWheelOffset + this.Height > ScrollContainer.Height)//last
                    {
                        iTempScrollWheelOffset = ScrollContainer.Height - this.Height - Offset;
                        Offset += iTempScrollWheelOffset;
                    }
                    else
                    {
                        Offset += VerticalScroll.ScrollWheelOffset;
                    }

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
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
                        Offset -= VerticalScroll.ScrollWheelOffset;

                    if (Offset < 0)
                        Offset = 0;

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
                }
        }

        private void OnVerticalScrollVisibleChanged(object sender, EventArgs e)
        {
            if (VerticalScroll.Visible)
                ScrollContainer.Width = this.Width - VerticalScroll.Width;
            else
                ScrollContainer.Width = this.Width;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            SetScrollHeight();

            VerticalScroll.Left = this.Width - VerticalScroll.Width;
        }
    }





    public class InfiniumDocumentsMenuItem : Control
    {
        Font fCaptionFont = new Font("Segoe UI", 17.0f, FontStyle.Regular, GraphicsUnit.Pixel);
        Font fCountFont = new Font("Segoe UI", 15.0f, FontStyle.Regular, GraphicsUnit.Pixel);

        SolidBrush brCaptionBrush = new SolidBrush(Color.FromArgb(60, 60, 60));
        SolidBrush brSelectedFontBrush = new SolidBrush(Color.FromArgb(60, 60, 60));
        SolidBrush brTrackFontBrush = new SolidBrush(Color.FromArgb(60, 60, 60));
        SolidBrush brSelectedBackBrush = new SolidBrush(Color.FromArgb(245, 245, 245));
        SolidBrush brCountBrush = new SolidBrush(Color.FromArgb(160, 160, 160));
        SolidBrush brSelectedCountBrush = new SolidBrush(Color.FromArgb(160, 160, 160));

        Bitmap iImage;

        string sCaption = "Item";

        int iItemHeight = 65;
        bool bTrack = false;

        int iCount = 0;

        bool bSelected = false;
        public int index = -1;

        public InfiniumDocumentsMenuItem()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            this.BackColor = Color.Transparent;
            this.Cursor = Cursors.Hand;

            this.Height = iItemHeight;
            this.Width = 50;
        }

        public bool Selected
        {
            get { return bSelected; }
            set
            {
                bSelected = value;
                this.Refresh();
            }
        }

        public string Caption
        {
            get { return sCaption; }
            set { sCaption = value; this.Refresh(); }
        }

        public int ItemHeight
        {
            get { return iItemHeight; }
            set { iItemHeight = value; this.Height = value; this.Refresh(); }
        }

        public int Count
        {
            get { return iCount; }
            set { iCount = value; this.Refresh(); }
        }

        public Bitmap Image
        {
            get { return iImage; }
            set { iImage = value; this.Refresh(); }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            if (bTrack || bSelected)
            {
                e.Graphics.FillRectangle(brSelectedBackBrush, this.ClientRectangle);
            }

            if (Image != null)
            {
                Image.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);
                e.Graphics.DrawImage(iImage, 15, (this.Height - Image.Height) / 2);
            }

            if (bSelected)
                e.Graphics.DrawString(sCaption, fCaptionFont, brSelectedFontBrush, Image.Width + 15 + 12,
                                     (this.Height - e.Graphics.MeasureString(sCaption, fCaptionFont).Height) / 2);
            else
                e.Graphics.DrawString(sCaption, fCaptionFont, brCaptionBrush, Image.Width + 15 + 12,
                                     (this.Height - e.Graphics.MeasureString(sCaption, fCaptionFont).Height) / 2);

            if (Count > 0)
            {
                e.Graphics.DrawString(Count.ToString(), fCountFont, brCountBrush, this.Width - e.Graphics.MeasureString(Count.ToString(), fCountFont).Width - 10,
                                     (this.Height - e.Graphics.MeasureString(Count.ToString(), fCountFont).Height) / 2 + 1);
            }

        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!bTrack)
            {
                bTrack = true;
                this.Refresh();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (bTrack)
            {
                bTrack = false;
                this.Refresh();
            }
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            Selected = true;

            OnItemClicked(this, sCaption, index, bSelected);

            Parent.Focus();
        }


        public event ClickEventHandler ItemClicked;

        public delegate void ClickEventHandler(object sender, string Name, int index, bool bSelected);

        public virtual void OnItemClicked(object sender, string Name, int index, bool bSelected)
        {
            ItemClicked?.Invoke(sender, Name, index, bSelected);
        }
    }


    public class InfiniumDocumentsMenu : Control
    {
        public int Offset = 0;

        int iTempScrollWheelOffset = 0;

        InfiniumScrollContainer ScrollContainer;
        public InfiniumProjectsVerticalScrollBar VerticalScroll;

        public InfiniumDocumentsMenuItem[] Items;

        public DataTable ItemsDT;

        public string SelectedName = "";

        int iSelected = 0;

        int iMarginToNextItem = 0;

        public InfiniumDocumentsMenu()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            VerticalScroll = new InfiniumProjectsVerticalScrollBar()
            {
                Parent = this,
                Height = this.Height
            };
            VerticalScroll.Left = this.Width - VerticalScroll.Width;
            VerticalScroll.TotalControlHeight = this.Height;
            VerticalScroll.ScrollPositionChanged += OnScrollPositionChanged;

            ScrollContainer = new InfiniumScrollContainer()
            {
                Parent = this,
                Top = 0,
                Left = 0,
                Width = this.Width,
                Height = this.Height,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            ScrollContainer.SizeChanged += OnScrollContainerSizeChanged;
            ScrollContainer.BackgroundColor = Color.White;
        }

        public void InitializeItems()
        {
            if (Items != null)
            {
                for (int i = 0; i < Items.Count(); i++)
                {
                    if (Items[i] != null)
                    {
                        Items[i].Dispose();
                        Items[i] = null;
                    }
                }

                Items = new InfiniumDocumentsMenuItem[0];
            }

            Offset = 0;
            ScrollContainer.Top = -Offset;
            VerticalScroll.Offset = Offset;
            VerticalScroll.Refresh();

            if (ItemsDT == null)
                return;

            if (ItemsDT.DefaultView.Count > 0)
            {
                Items = new InfiniumDocumentsMenuItem[ItemsDT.DefaultView.Count];

                for (int i = 0; i < ItemsDT.DefaultView.Count; i++)
                {
                    Items[i] = new InfiniumDocumentsMenuItem();

                    Items[i].Top = i * (Items[i].ItemHeight + iMarginToNextItem);
                    Items[i].Caption = ItemsDT.DefaultView[i]["Name"].ToString();
                    Items[i].index = i;

                    if (ItemsDT.DefaultView[i]["Image"] != DBNull.Value)
                    {
                        byte[] b = (byte[])ItemsDT.DefaultView[i]["Image"];

                        using (MemoryStream ms = new MemoryStream(b))
                        {
                            Items[i].Image = (Bitmap)Image.FromStream(ms);
                        }
                    }

                    Items[i].Parent = ScrollContainer;
                    Items[i].Width = ScrollContainer.Width;
                    Items[i].Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                    Items[i].ItemClicked += OnItemClicked;
                }
            }

            SetScrollHeight();
        }

        private void SetScrollHeight()
        {
            if (Items == null || ItemsDataTable.DefaultView.Count == 0)
            {
                ScrollContainer.Height = this.Height;
                VerticalScroll.TotalControlHeight = ScrollContainer.Height;
                VerticalScroll.Refresh();

                return;
            }

            if (Items.Count() * (Items[0].Height + iMarginToNextItem) > this.Height)
                ScrollContainer.Height = Items.Count() * (Items[0].Height + iMarginToNextItem);
            else
                ScrollContainer.Height = this.Height;

            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
            VerticalScroll.Refresh();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public InfiniumProjectsVerticalScrollBar VerticalScrollBar
        {
            get { return VerticalScroll; }
            set { VerticalScroll = value; VerticalScroll.Left = this.Width - VerticalScroll.Width; }
        }

        public int Selected
        {
            get { return iSelected; }
            set
            {
                iSelected = value;

                if (Items == null)
                    return;

                Items[value].Selected = true;
                SelectedName = Items[value].Caption;

                foreach (InfiniumDocumentsMenuItem Item in Items)
                {
                    if (Item.index != value)
                        Item.Selected = false;
                }

                OnSelectedChanged(this, SelectedName, value);
            }
        }

        public DataTable ItemsDataTable
        {
            get
            {
                //if (ItemsDT == null)
                //    return ItemsDT;

                //if (Items.Count() > 0)
                //    for (int i = 0; i < Items.Count(); i++)
                //    {
                //        if (Items[i] == null)
                //            continue;

                //        if (Items[i].Checked)
                //            ItemsDT.Rows[i]["Checked"] = true;
                //        else
                //            ItemsDT.Rows[i]["Checked"] = false;
                //    }

                return ItemsDT;
            }
            set
            {
                ItemsDT = value;
            }
        }

        private void OnScrollContainerSizeChanged(object sender, EventArgs e)
        {
            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
        }

        private void OnScrollPositionChanged(object sender, int tOffset)
        {
            Offset = tOffset;
            ScrollContainer.Top = -Offset;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (!VerticalScroll.Visible)
                return;

            if (Offset == 0)
                if (iTempScrollWheelOffset > 0)
                    iTempScrollWheelOffset = 0;

            if (e.Delta < 0)
            {
                if (Offset < ScrollContainer.Height - this.Height)
                {
                    if (Offset + VerticalScroll.ScrollWheelOffset + this.Height > ScrollContainer.Height)//last
                    {
                        iTempScrollWheelOffset = ScrollContainer.Height - this.Height - Offset;
                        Offset += iTempScrollWheelOffset;
                    }
                    else
                    {
                        Offset += VerticalScroll.ScrollWheelOffset;
                    }

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
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
                        Offset -= VerticalScroll.ScrollWheelOffset;

                    if (Offset < 0)
                        Offset = 0;

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
                }
        }

        private void OnItemClicked(object sender, string Name, int index, bool bSelected)
        {
            if (index == iSelected)
                return;

            iSelected = index;
            SelectedName = Name;

            foreach (InfiniumDocumentsMenuItem Item in Items)
            {
                if (Item.index != index)
                    Item.Selected = false;
            }

            OnSelectedChanged(this, Name, index);
        }



        public event SelectedChangedEventHandler SelectedChanged;

        public delegate void SelectedChangedEventHandler(object sender, string Name, int index);


        public virtual void OnSelectedChanged(object sender, string Name, int index)
        {
            iSelected = index;

            SelectedChanged?.Invoke(sender, Name, index);
        }


        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            ScrollContainer.Focus();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            SetScrollHeight();

            VerticalScroll.Left = this.Width - VerticalScroll.Width;
        }
    }





    public class InfiniumDocumentsUpdatesScrollContainer : Control
    {
        SolidBrush brBackBrush;
        Color cBackColor = Color.FromArgb(220, 220, 220);

        public InfiniumDocumentsUpdatesScrollContainer()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            brBackBrush = new SolidBrush(cBackColor);

            this.BackColor = cBackColor;
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            base.OnPaintBackground(pevent);
        }

        protected override void OnClick(EventArgs e)
        {
            this.Focus();

            base.OnClick(e);
        }
    }




    public class InfiniumDocumentItem : Control
    {
        Font fUserNameFont = new Font("Segoe UI Semibold", 16.0f, FontStyle.Regular, GraphicsUnit.Pixel);
        Font fDateFont = new Font("Segoe UI", 13.0f, FontStyle.Regular, GraphicsUnit.Pixel);
        Font fDescriptionFont = new Font("Segoe UI", 14.0f, FontStyle.Regular, GraphicsUnit.Pixel);
        Font fParametersFont = new Font("Segoe UI", 12.0f, FontStyle.Regular, GraphicsUnit.Pixel);
        Font fLabelsFont = new Font("Segoe UI Semibold", 13.0f, FontStyle.Regular, GraphicsUnit.Pixel);

        SolidBrush brUserNameBrush = new SolidBrush(Color.FromArgb(70, 70, 70));
        SolidBrush brDateBrush = new SolidBrush(Color.FromArgb(163, 163, 163));
        SolidBrush brDescriptionBrush = new SolidBrush(Color.FromArgb(80, 80, 80));
        SolidBrush brParametersBrush = new SolidBrush(Color.FromArgb(80, 80, 80));
        SolidBrush brNoBrush = new SolidBrush(Color.FromArgb(163, 163, 163));
        SolidBrush brLabelsBrush = new SolidBrush(Color.FromArgb(55, 82, 128));

        Pen pBorderPen = new Pen(new SolidBrush(Color.FromArgb(200, 200, 200)));

        Rectangle rBorderRect = new Rectangle(0, 0, 0, 0);

        int iImageWidth = 73;
        int iImageHeight = 68;

        int iBorderMargin = 15;
        int iButtonsMargin = 180;

        public static int iInitialHeight = 165;

        public DataTable UsersDataTable;

        public Bitmap Image = null;

        Bitmap CommentsCountBMP = Properties.Resources.DocCommentsCount;
        Bitmap RecipientsCountBMP = Properties.Resources.DocUserCount;
        Bitmap FilesCountBMP = Properties.Resources.DocFileCount;
        Bitmap DeleteBMP = Properties.Resources.DocDelete;
        Bitmap EditBMP = Properties.Resources.DocEdit;

        int iEditLeft = 0;
        int iDeleteLeft = 0;
        int iBMPLeft = 0;
        int iFilesBMPTop = 0;
        int iRecipientsBMPTop = 0;
        int iCommentsBMPTop = 0;

        bool bEditTrack = false;
        bool bDeleteTrack = false;
        bool bComTrack = false;
        bool bFileTrack = false;
        bool bRecipTrack = false;
        bool bImageTrack = false;
        bool bUserNameTrack = false;

        public string sUserName = "";
        public string sDate = "";
        public string sDocType = "";
        public string sCorrespondent = "";
        public string sDescription = "";
        public string sFactory = "";
        public string sIncomeNumber = "";
        public string sRegNumber = "";

        public int UserID = -1;
        public int iItemIndex = -1;
        public int iDocumentID = -1;
        public int iDocumentCategoryID = -1;

        public int iCommentsCount = 0;
        public int iFilesCount = 0;
        public int iRecipientsCount = 0;

        string sToolTipText = "";

        public bool bCanEdit = false;

        ToolTip ToolTip;



        public InfiniumDocumentItem()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            this.BackColor = Color.White;

            this.Height = iInitialHeight;
            this.Width = 1;

            ToolTip = new System.Windows.Forms.ToolTip();
            ToolTip.SetToolTip(this, sToolTipText);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            rBorderRect = this.ClientRectangle;
            rBorderRect.Width = this.ClientRectangle.Width - 1;
            rBorderRect.Height = this.ClientRectangle.Height - 1;

            e.Graphics.DrawRectangle(pBorderPen, rBorderRect);

            Image.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);

            e.Graphics.DrawImage(Image, iBorderMargin, iBorderMargin, iImageWidth, iImageHeight);

            e.Graphics.DrawString(sUserName, fUserNameFont, brUserNameBrush, iBorderMargin + 5 + iImageWidth, iBorderMargin - 2);

            e.Graphics.DrawString(sDate, fDateFont, brDateBrush, iBorderMargin + 5 + iImageWidth + e.Graphics.MeasureString(sUserName, fUserNameFont).Width, iBorderMargin + 1);

            if (sDescription.Length > 0)
                e.Graphics.DrawString(sDescription, fDescriptionFont, brDescriptionBrush, iBorderMargin + 5 + iImageWidth + 1, iBorderMargin + 25);
            else
                e.Graphics.DrawString("нет описания", fDescriptionFont, brNoBrush, iBorderMargin + 5 + iImageWidth + 1, iBorderMargin + 25);

            e.Graphics.DrawString("Тип документа", fLabelsFont, brLabelsBrush, iBorderMargin + 5 + iImageWidth, iBorderMargin + 55);


            string stype = sDocType;

            if (e.Graphics.MeasureString(sDocType, fParametersFont).Width > 145)
            {
                for (int i = sDocType.Length; i >= 0; i--)
                {
                    if (e.Graphics.MeasureString(sDocType.Substring(0, i), fParametersFont).Width <= 145)
                    {
                        stype = sDocType.Substring(0, i - 1) + "...";
                        break;
                    }
                }
            }

            e.Graphics.DrawString(stype, fParametersFont, brParametersBrush, iBorderMargin + 5 + iImageWidth, iBorderMargin + 75);


            e.Graphics.DrawString("Регистр. номер", fLabelsFont, brLabelsBrush, iBorderMargin + 5 + iImageWidth + 150, iBorderMargin + 55);


            if (sRegNumber.Length > 0)
            {
                string sregnumber = sRegNumber;

                if (e.Graphics.MeasureString(sRegNumber, fParametersFont).Width > 145)
                {
                    for (int i = sRegNumber.Length; i >= 0; i--)
                    {
                        if (e.Graphics.MeasureString(sRegNumber.Substring(0, i), fParametersFont).Width <= 145)
                        {
                            sregnumber = sRegNumber.Substring(0, i - 1) + "...";
                            break;
                        }
                    }
                }

                e.Graphics.DrawString(sregnumber, fParametersFont, brParametersBrush, iBorderMargin + 5 + iImageWidth + 150, iBorderMargin + 75);
            }
            else
                e.Graphics.DrawString("-", fParametersFont, brParametersBrush, iBorderMargin + 5 + iImageWidth + 150, iBorderMargin + 75);



            e.Graphics.DrawString("Корреспондент", fLabelsFont, brLabelsBrush, iBorderMargin + 5 + iImageWidth + 300, iBorderMargin + 55);


            if (sCorrespondent.Length > 0)
            {
                string scorr = sCorrespondent;

                if (e.Graphics.MeasureString(sCorrespondent, fParametersFont).Width > this.Width - iBorderMargin - 5 - iImageWidth - 300 - iButtonsMargin)
                {
                    for (int i = sCorrespondent.Length; i >= 0; i--)
                    {
                        if (e.Graphics.MeasureString(sCorrespondent.Substring(0, i), fParametersFont).Width <= this.Width - iBorderMargin - 5 - iImageWidth - 300 - iButtonsMargin)
                        {
                            scorr = sCorrespondent.Substring(0, i - 1) + "...";
                            break;
                        }
                    }
                }

                e.Graphics.DrawString(scorr, fParametersFont, brParametersBrush, iBorderMargin + 5 + iImageWidth + 300, iBorderMargin + 75);
            }
            else
                e.Graphics.DrawString("-", fParametersFont, brParametersBrush, iBorderMargin + 5 + iImageWidth + 300, iBorderMargin + 75);


            e.Graphics.DrawString("Участок", fLabelsFont, brLabelsBrush, iBorderMargin + 5 + iImageWidth, iBorderMargin + 100);
            e.Graphics.DrawString(sFactory, fParametersFont, brParametersBrush, iBorderMargin + 5 + iImageWidth, iBorderMargin + 120);



            e.Graphics.DrawString("Входящий номер", fLabelsFont, brLabelsBrush, iBorderMargin + 5 + iImageWidth + 150, iBorderMargin + 100);


            if (sIncomeNumber.Length > 0)
            {
                string sinc = sIncomeNumber;

                if (e.Graphics.MeasureString(sIncomeNumber, fParametersFont).Width > 145)
                {
                    for (int i = sIncomeNumber.Length; i >= 0; i--)
                    {
                        if (e.Graphics.MeasureString(sIncomeNumber.Substring(0, i), fParametersFont).Width <= 145)
                        {
                            sinc = sIncomeNumber.Substring(0, i - 1) + "...";
                            break;
                        }
                    }
                }

                e.Graphics.DrawString(sinc, fParametersFont, brParametersBrush, iBorderMargin + 5 + iImageWidth + 150, iBorderMargin + 120);
            }
            else
                e.Graphics.DrawString("-", fParametersFont, brParametersBrush, iBorderMargin + 5 + iImageWidth + 150, iBorderMargin + 120);


            CommentsCountBMP.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);
            FilesCountBMP.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);
            RecipientsCountBMP.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);

            iBMPLeft = this.Width - iButtonsMargin;
            iCommentsBMPTop = iBorderMargin + 102;
            iRecipientsBMPTop = iBorderMargin + 2;
            iFilesBMPTop = iBorderMargin + 52;

            e.Graphics.DrawImage(RecipientsCountBMP, this.Width - iButtonsMargin, iBorderMargin + 2);

            e.Graphics.DrawString(GetLabelText(iRecipientsCount, "получател", "ь", "я", "ей"), fDescriptionFont, brDescriptionBrush,
                                    this.Width - iButtonsMargin + RecipientsCountBMP.Width + 3, iBorderMargin + 8);

            e.Graphics.DrawImage(FilesCountBMP, this.Width - iButtonsMargin, iBorderMargin + 52);

            e.Graphics.DrawString(GetLabelText(iFilesCount, "файл", "", "а", "ов"), fDescriptionFont, brDescriptionBrush,
                                    this.Width - iButtonsMargin + RecipientsCountBMP.Width + 3, iBorderMargin + 58);

            e.Graphics.DrawImage(CommentsCountBMP, this.Width - iButtonsMargin, iBorderMargin + 102);

            e.Graphics.DrawString(GetLabelText(iCommentsCount, "комментар", "ий", "ия", "иев"), fDescriptionFont, brDescriptionBrush,
                                    this.Width - iButtonsMargin + RecipientsCountBMP.Width + 3, iBorderMargin + 108);

            if (bCanEdit)
            {
                iEditLeft = iImageWidth + 5 + iBorderMargin + Convert.ToInt32(e.Graphics.MeasureString(sUserName, fUserNameFont).Width) +
                            Convert.ToInt32(e.Graphics.MeasureString(sDate, fDateFont).Width) + 15;

                iDeleteLeft = iEditLeft + 20 + 10;

                EditBMP.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);
                DeleteBMP.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);

                e.Graphics.DrawImage(EditBMP, iEditLeft, iBorderMargin + 1, 20, 20);
                e.Graphics.DrawImage(DeleteBMP, iDeleteLeft, iBorderMargin + 1, 20, 20);

            }
        }

        private string GetLabelText(int i, string sMain, string s1suf, string s2suf, string s3suf)
        {
            char[] s = new char[2];

            s[0] = i.ToString()[i.ToString().Length - 1];

            if (i > 9)
                s[1] = i.ToString()[i.ToString().Length - 2];
            else
                s[1] = '-';

            if (s[0] == '1' && s[1] != '1')
                return i.ToString() + " " + sMain + s1suf;
            else
                if ((s[0] == '2' && s[1] != '1') || (s[0] == '3' && s[1] != '1') || (s[0] == '4' && s[1] != '1'))
                return i.ToString() + " " + sMain + s2suf;
            else
                return i.ToString() + " " + sMain + s3suf;
        }

        public string ToolTipText
        {
            get { return sToolTipText; }
            set { sToolTipText = value; ToolTip.SetToolTip(this, value); }
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            Parent.Focus();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            bFileTrack = false;
            bRecipTrack = false;
            bComTrack = false;
            bEditTrack = false;
            bDeleteTrack = false;
            bImageTrack = false;

            if (bCanEdit)
            {
                if (e.Y >= iBorderMargin && e.Y <= iBorderMargin + 20 && e.X >= iEditLeft && e.X <= iEditLeft + 20)
                {
                    if (ToolTipText != "Редактировать")
                        ToolTipText = "Редактировать";

                    bEditTrack = true;
                }

                if (e.Y >= iBorderMargin && e.Y <= iBorderMargin + 20 && e.X >= iDeleteLeft && e.X <= iDeleteLeft + 20)
                {
                    if (ToolTipText != "Удалить")
                        ToolTipText = "Удалить";

                    bDeleteTrack = true;
                }
            }





            if (e.Y >= iCommentsBMPTop && e.Y <= iCommentsBMPTop + CommentsCountBMP.Height && e.X >= iBMPLeft && e.X <= iBMPLeft + CommentsCountBMP.Width)
            {
                if (ToolTipText != "Открыть документ")
                    ToolTipText = "Открыть документ";

                bComTrack = true;
            }

            if (e.Y >= iRecipientsBMPTop && e.Y <= iRecipientsBMPTop + RecipientsCountBMP.Height && e.X >= iBMPLeft && e.X <= iBMPLeft + RecipientsCountBMP.Width)
            {
                if (ToolTipText != "Открыть документ")
                    ToolTipText = "Открыть документ";

                bRecipTrack = true;
            }

            if (e.Y >= iFilesBMPTop && e.Y <= iFilesBMPTop + FilesCountBMP.Height && e.X >= iBMPLeft && e.X <= iBMPLeft + FilesCountBMP.Width)
            {
                if (ToolTipText != "Открыть документ")
                    ToolTipText = "Открыть документ";

                bFileTrack = true;
            }

            if (e.Y >= iBorderMargin && e.Y <= iBorderMargin + iImageHeight && e.X >= iBorderMargin && e.X <= iBorderMargin + iImageWidth)
            {
                if (ToolTipText != "Открыть документ")
                    ToolTipText = "Открыть документ";

                bImageTrack = true;
            }

            if (!bFileTrack && !bRecipTrack && !bComTrack && !bEditTrack && !bDeleteTrack && !bImageTrack)
            {
                this.Cursor = Cursors.Default;
                ToolTipText = "";
            }
            else
                this.Cursor = Cursors.Hand;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (bCanEdit)
            {
                if (bEditTrack)
                    OnEditClicked(this, iDocumentID, iDocumentCategoryID);

                if (bDeleteTrack)
                    OnDeleteClicked(this, iDocumentID, iDocumentCategoryID);
            }

            if (bUserNameTrack || bImageTrack || bFileTrack || bComTrack || bRecipTrack)
                OnItemClicked(this, iDocumentID, iDocumentCategoryID);
        }

        public event EditClickedEventHandler EditClicked;
        public event EditClickedEventHandler DeleteClicked;
        public event EditClickedEventHandler ItemClicked;

        public delegate void EditClickedEventHandler(object sender, int DocumentID, int DocumentCategoryID);

        public virtual void OnEditClicked(object sender, int DocumentID, int DocumentCategoryID)
        {
            EditClicked?.Invoke(sender, DocumentID, DocumentCategoryID);
        }

        public virtual void OnDeleteClicked(object sender, int DocumentID, int DocumentCategoryID)
        {
            DeleteClicked?.Invoke(sender, DocumentID, DocumentCategoryID);
        }

        public virtual void OnItemClicked(object sender, int DocumentID, int DocumentCategoryID)
        {
            ItemClicked?.Invoke(sender, DocumentID, DocumentCategoryID);
        }

    }


    public class InfiniumDocumentsList : Control
    {
        Font fCaptionFont = new Font("Segoe UI", 22.0f, FontStyle.Regular, GraphicsUnit.Pixel);

        SolidBrush brCaptionBrush = new SolidBrush(Color.FromArgb(110, 110, 110));

        public int Offset = 0;

        public int iCurPosY = 0;

        int iTempScrollWheelOffset = 0;
        int iMarginToNextItem = 20;

        InfiniumDocumentsUpdatesScrollContainer ScrollContainer;
        public InfiniumProjectsVerticalScrollBar VerticalScroll;

        public InfiniumDocumentItem[] Items;

        public DataTable ItemsDataTable = null;
        public DataTable UsersDataTable = null;
        public DataTable DocumentsTypesDataTable = null;
        public DataTable FactoryDataTable = null;
        public DataTable DocumentsCategoriesDataTable = null;
        public DataTable CorrespondentsDataTable = null;

        public InfiniumDocumentsList()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);

            VerticalScroll = new InfiniumProjectsVerticalScrollBar()
            {
                Parent = this,
                Width = 12
            };
            VerticalScroll.Left = this.Width - VerticalScroll.Width;
            VerticalScroll.VerticalScrollCommonShaftBackColor = Color.FromArgb(220, 220, 220);
            VerticalScroll.VerticalScrollCommonThumbButtonColor = Color.Silver;
            VerticalScroll.VerticalScrollTrackingShaftBackColor = Color.FromArgb(242, 242, 242);
            VerticalScroll.VerticalScrollTrackingThumbButtonColor = Color.FromArgb(140, 140, 140);
            VerticalScroll.Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            VerticalScroll.ScrollPositionChanged += OnScrollPositionChanged;
            VerticalScroll.ScrollWheelOffset = 60;
            VerticalScroll.Visible = true;

            ScrollContainer = new InfiniumDocumentsUpdatesScrollContainer()
            {
                Parent = this,
                Top = 0,
                Left = 0,
                Width = this.Width,
                Height = this.Height,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            ScrollContainer.SizeChanged += OnScrollContainerSizeChanged;
            ScrollContainer.Paint += OnScrollContainerPaint;
        }

        public void InitializeItems()
        {
            if (Items != null)
            {
                for (int i = 0; i < Items.Count(); i++)
                {
                    if (Items[i] != null)
                    {
                        Items[i].Dispose();
                        Items[i] = null;
                    }
                }

                Items = new InfiniumDocumentItem[0];
            }

            iCurPosY = 0;
            Offset = 0;
            ScrollContainer.Top = -Offset;
            VerticalScroll.Offset = Offset;
            VerticalScroll.Refresh();

            if (ItemsDataTable == null)
                return;

            if (ItemsDataTable.DefaultView.Count > 0)
            {
                Items = new InfiniumDocumentItem[ItemsDataTable.DefaultView.Count];

                for (int i = 0; i < ItemsDataTable.DefaultView.Count; i++)
                {
                    iCurPosY += iMarginToNextItem;
                    Items[i] = new InfiniumDocumentItem()
                    {
                        Parent = ScrollContainer,
                        Left = iMarginToNextItem,
                        Width = ScrollContainer.Width - iMarginToNextItem - iMarginToNextItem,

                        Top = iCurPosY,
                        iDocumentID = Convert.ToInt32(ItemsDataTable.DefaultView[i]["DocumentID"]),
                        iDocumentCategoryID = Convert.ToInt32(ItemsDataTable.DefaultView[i]["DocumentCategoryID"]),
                        sUserName = UsersDataTable.Select("UserID = " + ItemsDataTable.DefaultView[i]["UserID"])[0]["ShortName"].ToString(),
                        UserID = Convert.ToInt32(ItemsDataTable.DefaultView[i]["UserID"]),
                        sDocType = DocumentsTypesDataTable.Select("DocumentTypeID = " + ItemsDataTable.DefaultView[i]["DocumentTypeID"])[0]["DocumentType"].ToString(),
                        sDate = Convert.ToDateTime(ItemsDataTable.DefaultView[i]["DateTime"]).ToString("dd.MM.yyyy HH:mm"),
                        sDescription = ItemsDataTable.DefaultView[i]["Description"].ToString(),
                        iRecipientsCount = Convert.ToInt32(ItemsDataTable.DefaultView[i]["RecipientsCount"]),
                        iFilesCount = Convert.ToInt32(ItemsDataTable.DefaultView[i]["FilesCount"]),
                        iCommentsCount = Convert.ToInt32(ItemsDataTable.DefaultView[i]["CommentsCount"])
                    };
                    if (Security.CurrentUserID == Items[i].UserID)
                        Items[i].bCanEdit = true;

                    if (ItemsDataTable.Columns["CorrespondentID"] != null)
                        if (Convert.ToInt32(ItemsDataTable.DefaultView[i]["CorrespondentID"]) != -1)
                            Items[i].sCorrespondent = CorrespondentsDataTable.Select("CorrespondentID = " + ItemsDataTable.DefaultView[i]["CorrespondentID"])[0]["CorrespondentName"].ToString();

                    Items[i].sRegNumber = ItemsDataTable.DefaultView[i]["RegNumber"].ToString();
                    Items[i].sFactory = FactoryDataTable.Select("FactoryID = " + ItemsDataTable.DefaultView[i]["FactoryID"])[0]["Factory"].ToString();
                    if (Items[i].iDocumentCategoryID == 1)//income
                        Items[i].sIncomeNumber = ItemsDataTable.DefaultView[i]["IncomeNumber"].ToString();

                    Items[i].iItemIndex = i;


                    if (UsersDataTable.Select("UserID = " + Items[i].UserID)[0]["Photo"] != DBNull.Value)
                    {
                        byte[] b = (byte[])UsersDataTable.Select("UserID = " + Items[i].UserID)[0]["Photo"];

                        using (MemoryStream ms = new MemoryStream(b))
                        {
                            Items[i].Image = (Bitmap)Image.FromStream(ms);
                        }
                    }
                    else
                        Items[i].Image = Properties.Resources.NoImage;


                    Items[i].Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                    Items[i].EditClicked += OnEditClicked;
                    Items[i].DeleteClicked += OnDeleteClicked;
                    Items[i].ItemClicked += OnItemClicked;

                    iCurPosY += Items[i].Height;
                }
            }

            iCurPosY += iMarginToNextItem;

            if (iCurPosY > this.Height)
                ScrollContainer.Height = iCurPosY;
            else
                ScrollContainer.Height = this.Height;

            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
            VerticalScroll.Refresh();

            ScrollContainer.Refresh();
        }



        public void SetHeight(int ItemIndex, int iHeight)
        {
            for (int i = ItemIndex + 1; i < Items.Count(); i++)
                Items[i].Top += iHeight;


            int iH = 0;

            iH += iMarginToNextItem;

            foreach (InfiniumDocumentItem Item in Items)
            {
                iH += Item.Height;
                iH += iMarginToNextItem;
            }

            if (this.Height >= iH)
            {
                ScrollContainer.Height = this.Height;
                Offset = 0;
                iTempScrollWheelOffset = 0;
                ScrollContainer.Top = 0;
            }
            else
                ScrollContainer.Height = iH;

            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
            VerticalScroll.Refresh();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            if (iCurPosY > this.Height)
                ScrollContainer.Height = iCurPosY;
            else
                ScrollContainer.Height = this.Height;

            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
            VerticalScroll.Refresh();

            ScrollContainer.Refresh();
        }

        private void OnScrollContainerPaint(object sender, PaintEventArgs e)
        {
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            if (Items == null || (Items != null && Items.Count() == 0))
                e.Graphics.DrawString("Нет документов", fCaptionFont, brCaptionBrush,
                                        (this.Width - e.Graphics.MeasureString("Нет обновлений", fCaptionFont).Width) / 2,
                                        (this.Height - e.Graphics.MeasureString("Нет обновлений", fCaptionFont).Height) / 2);
        }

        private void OnScrollContainerSizeChanged(object sender, EventArgs e)
        {
            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
        }

        private void OnScrollPositionChanged(object sender, int tOffset)
        {
            Offset = tOffset;
            ScrollContainer.Top = -Offset;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (!VerticalScroll.Visible)
                return;

            if (Offset == 0)
                if (iTempScrollWheelOffset > 0)
                    iTempScrollWheelOffset = 0;

            if (e.Delta < 0)
            {
                if (Offset < ScrollContainer.Height - this.Height)
                {
                    if (Offset + VerticalScroll.ScrollWheelOffset + this.Height > ScrollContainer.Height)//last
                    {
                        iTempScrollWheelOffset = ScrollContainer.Height - this.Height - Offset;
                        Offset += iTempScrollWheelOffset;
                    }
                    else
                    {
                        Offset += VerticalScroll.ScrollWheelOffset;
                    }

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
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
                        Offset -= VerticalScroll.ScrollWheelOffset;

                    if (Offset < 0)
                        Offset = 0;

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
                }
        }


        public event EditClickedEventHandler EditClicked;
        public event EditClickedEventHandler DeleteClicked;
        public event EditClickedEventHandler ItemClicked;

        public delegate void EditClickedEventHandler(object sender, int DocumentID, int DocumentCategoryID);

        public virtual void OnEditClicked(object sender, int DocumentID, int DocumentCategoryID)
        {
            EditClicked?.Invoke(sender, DocumentID, DocumentCategoryID);
        }

        public virtual void OnDeleteClicked(object sender, int DocumentID, int DocumentCategoryID)
        {
            DeleteClicked?.Invoke(sender, DocumentID, DocumentCategoryID);
        }

        public virtual void OnItemClicked(object sender, int DocumentID, int DocumentCategoryID)
        {
            ItemClicked?.Invoke(sender, DocumentID, DocumentCategoryID);
        }

    }




    public class InfiniumDocumentsUpdatesItem : Control
    {
        Color cBackColor = Color.White;
        //Color cBorderColor = Color.FromArgb(192, 192, 192);

        Font fUserNameFont = new Font("Segoe UI Semibold", 17.0f, FontStyle.Regular, GraphicsUnit.Pixel);
        Font fDateFont = new Font("Segoe UI", 13.0f, FontStyle.Regular, GraphicsUnit.Pixel);
        Font fTextFont = new Font("Segoe UI", 16.0f, FontStyle.Regular, GraphicsUnit.Pixel);
        Font fParametersFont = new Font("Segoe UI", 13.0f, FontStyle.Regular, GraphicsUnit.Pixel);
        Font fCategoryFont = new Font("Segoe UI", 16.0f, FontStyle.Regular, GraphicsUnit.Pixel);
        Font fCorrespondentFont = new Font("Segoe UI", 16.0f, FontStyle.Regular, GraphicsUnit.Pixel);
        Font fLabelsFont = new Font("Segoe UI Semibold", 14.0f, FontStyle.Regular, GraphicsUnit.Pixel);

        SolidBrush brUserNameBrush = new SolidBrush(Color.FromArgb(70, 70, 70));
        SolidBrush brDateBrush = new SolidBrush(Color.FromArgb(163, 163, 163));
        SolidBrush brTextBrush = new SolidBrush(Color.FromArgb(80, 80, 80));
        SolidBrush brParametersBrush = new SolidBrush(Color.FromArgb(80, 80, 80));
        SolidBrush brCategoryBrush = new SolidBrush(Color.FromArgb(56, 184, 238));
        SolidBrush brCorrespondentBrush = new SolidBrush(Color.FromArgb(56, 184, 238));
        SolidBrush brNoBrush = new SolidBrush(Color.FromArgb(163, 163, 163));
        SolidBrush brLabelsBrush = new SolidBrush(Color.FromArgb(80, 80, 80));

        Pen pBorderPen = new Pen(new SolidBrush(Color.FromArgb(200, 200, 200)));

        Rectangle rBorderRect = new Rectangle(0, 0, 0, 0);

        int iImageWidth = 73;
        int iImageHeight = 68;

        int iBorderMargin = 15;

        public static int iInitialHeight = 410;



        ToolTip ToolTip;

        string sToolTipText = "";

        public DataTable CommentsDataTable;
        public DataTable UsersDataTable;
        public DataTable RecipientsDataTable;
        public DataTable FilesDataTable;
        public DataTable CommentsFilesDataTable;
        public DataTable CurrentFilesDataTable;
        public DataTable ConfirmsDataTable = null;
        public DataTable ConfirmsRecipientsDataTable = null;

        public bool bInRecipients = false;

        public Bitmap Image = null;

        public string sUserName = "";
        public string sDate = "";
        public string sDocType = "";
        public string sCategory = "";
        public string sCorrespondent = "";
        public string sDescription = "";
        public string sFactory = "";
        public string sIncomeNumber = "";
        public string sRegNumber = "";

        public int UserID = -1;
        public int iItemIndex = -1;
        public int iDocumentID = -1;
        public int iDocumentCategoryID = -1;

        public InfiniumDocumentsUpdatesControlPanel ControlPanel;
        public InfiniumDocumentsUpdatesUsersList UsersList;
        public InfiniumDocumentsUpdatesFilesList FileList;

        public InfiniumDocumentsUpdatesItem()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            this.BackColor = Color.White;

            this.Height = iInitialHeight;
            this.Width = 1;

            ToolTip = new System.Windows.Forms.ToolTip();
            ToolTip.SetToolTip(this, sToolTipText);

            CurrentFilesDataTable = new DataTable();
            CurrentFilesDataTable.Columns.Add(new DataColumn("FileName", Type.GetType("System.String")));
            CurrentFilesDataTable.Columns.Add(new DataColumn("FilePath", Type.GetType("System.String")));
            CurrentFilesDataTable.Columns.Add(new DataColumn("FileSizeText", Type.GetType("System.String")));
            CurrentFilesDataTable.Columns.Add(new DataColumn("FileSize", Type.GetType("System.Int64")));
            CurrentFilesDataTable.Columns.Add(new DataColumn("IsNew", Type.GetType("System.Boolean")));
        }

        public void Initialize()
        {
            ControlPanel = new InfiniumDocumentsUpdatesControlPanel()
            {
                Width = this.Width,
                Parent = this,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            ControlPanel.ControlPanelSizeChanged += OnControlPanelSizeChanged;
            ControlPanel.CommentsDataTable = CommentsDataTable;
            ControlPanel.UsersDataTable = UsersDataTable;
            ControlPanel.FilesDataTable = CommentsFilesDataTable;
            ControlPanel.ConfirmsDataTable = ConfirmsDataTable;
            ControlPanel.ConfirmsRecipientsDataTable = ConfirmsRecipientsDataTable;
            ControlPanel.bShowButtons = RecipientsDataTable.Select("UserID = " + Security.CurrentUserID).Count() > 0;
            ControlPanel.CreateComments();
            ControlPanel.CreateTextBox();
            this.Height += ControlPanel.Height;
            ControlPanel.Top = this.Height - ControlPanel.Height;
            ControlPanel.CommentsTextBoxOpened += OnCommentsTextBoxOpened;
            ControlPanel.FileLabelClicked += OnCommentsTextBoxFileLabelClick;
            ControlPanel.SendButtonClicked += OnSendButtonClicked;
            ControlPanel.EditCommentClicked += OnEditCommentClicked;
            ControlPanel.DeleteCommentClicked += OnDeleteCommentClicked;
            ControlPanel.FileClicked += OnCommentFileClicked;
            ControlPanel.ItemConfirmClicked += OnItemConfirmClicked;
            ControlPanel.ItemCancelClicked += OnItemCancelClicked;
            ControlPanel.ItemEditClicked += OnItemEditClicked;
            ControlPanel.ConfirmEditClicked += OnConfirmEditClicked;
            ControlPanel.ConfirmDeleteClicked += OnConfirmDeleteClicked;
            ControlPanel.AddConfirmClicked += OnAddConfirmClicked;
            ControlPanel.AddUserClicked += OnAddUserClicked;

            UsersList = new InfiniumDocumentsUpdatesUsersList()
            {
                Width = this.Width - iImageWidth - 7 - iBorderMargin,
                Left = iImageWidth + 7 + iBorderMargin + 4,
                Parent = this,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                ItemsDataTable = RecipientsDataTable,
                UsersDataTable = UsersDataTable,
                Top = iBorderMargin + 195
            };
            UsersList.InitializeItems();

            FileList = new InfiniumDocumentsUpdatesFilesList()
            {
                Width = this.Width - iImageWidth - 7 - iBorderMargin,
                Left = iImageWidth + 7 + iBorderMargin,
                Parent = this,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                ItemsDataTable = FilesDataTable,
                Top = iBorderMargin + 195 + UsersList.Height + 30
            };
            FileList.InitializeItems();
            FileList.ItemClicked += OnFileClick;
        }

        public string ToolTipText
        {
            get { return sToolTipText; }
            set { sToolTipText = value; ToolTip.SetToolTip(this, value); }
        }

        public void ChangeItemSize(int Height)
        {
            this.Height += Height;

            OnItemSizeChanged(this, iItemIndex, Height);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            rBorderRect = this.ClientRectangle;
            rBorderRect.Width = this.ClientRectangle.Width - 1;
            rBorderRect.Height = this.ClientRectangle.Height - 1;

            e.Graphics.DrawRectangle(pBorderPen, rBorderRect);

            //if (bInRecipients)
            //{
            //    AddConfirmBMP.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);

            //    e.Graphics.DrawImage(AddConfirmBMP, this.Width - AddConfirmBMP.Width - iBorderMargin, iBorderMargin);
            //}

            //iAddConfirmLeft = this.Width - AddConfirmBMP.Width - iBorderMargin;

            Image.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);

            e.Graphics.DrawImage(Image, iBorderMargin, iBorderMargin, iImageWidth, iImageHeight);

            e.Graphics.DrawString(sUserName, fUserNameFont, brUserNameBrush, 7 + iImageWidth + iBorderMargin, iBorderMargin - 4);

            e.Graphics.DrawString(sDate, fDateFont, brDateBrush, 7 + iImageWidth + iBorderMargin + e.Graphics.MeasureString(sUserName, fUserNameFont).Width, iBorderMargin);

            e.Graphics.DrawString("добавил(а) новый", fTextFont, brTextBrush, 7 + iImageWidth + iBorderMargin + 1, iBorderMargin + 21);

            e.Graphics.DrawString(sCategory, fCategoryFont, brCategoryBrush, 7 + iImageWidth + iBorderMargin + 138, iBorderMargin + 21);

            if (sCorrespondent.Length > 0)
            {
                if (iDocumentCategoryID == 2)
                {
                    e.Graphics.DrawString("к", fTextFont, brTextBrush, 7 + iImageWidth + iBorderMargin + 135 + e.Graphics.MeasureString(sCategory, fCategoryFont).Width, iBorderMargin + 21);
                    e.Graphics.DrawString(sCorrespondent, fCorrespondentFont, brCorrespondentBrush, 7 + iImageWidth + iBorderMargin + 135 + e.Graphics.MeasureString(sCategory, fCategoryFont).Width + 20, iBorderMargin + 21);
                }
                else
                {
                    e.Graphics.DrawString("от", fTextFont, brTextBrush, 7 + iImageWidth + iBorderMargin + 135 + e.Graphics.MeasureString(sCategory, fCategoryFont).Width, iBorderMargin + 21);
                    e.Graphics.DrawString(sCorrespondent, fCorrespondentFont, brCorrespondentBrush, 7 + iImageWidth + iBorderMargin + 135 + e.Graphics.MeasureString(sCategory, fCategoryFont).Width + 25, iBorderMargin + 21);
                }
            }


            e.Graphics.DrawString("Тип документа", fLabelsFont, brLabelsBrush, 7 + iImageWidth + iBorderMargin + 1, iBorderMargin + 65);
            e.Graphics.DrawString(sDocType, fParametersFont, brParametersBrush, 7 + iImageWidth + iBorderMargin + 2, iBorderMargin + 85);

            e.Graphics.DrawString("Описание", fLabelsFont, brLabelsBrush, 7 + iImageWidth + iBorderMargin + 200 + 1, iBorderMargin + 65);

            if (sDescription.Length > 0)
                e.Graphics.DrawString(sDescription, fParametersFont, brParametersBrush, iImageWidth + iBorderMargin + 7 + 200 + 2, iBorderMargin + 85);
            else
                e.Graphics.DrawString("нет описания", fParametersFont, brNoBrush, iImageWidth + iBorderMargin + 7 + 200 + 2, iBorderMargin + 85);

            e.Graphics.DrawString("Участок", fLabelsFont, brLabelsBrush, 7 + iImageWidth + iBorderMargin + 1, iBorderMargin + 115);
            e.Graphics.DrawString(sFactory, fParametersFont, brParametersBrush, 7 + iImageWidth + iBorderMargin + 2, iBorderMargin + 135);

            e.Graphics.DrawString("Регистрационный номер", fLabelsFont, brLabelsBrush, 7 + iImageWidth + iBorderMargin + 200 + 1, iBorderMargin + 115);

            if (sRegNumber.Length > 0)
                e.Graphics.DrawString(sRegNumber, fParametersFont, brParametersBrush, 7 + iImageWidth + iBorderMargin + 200 + 2, iBorderMargin + 135);
            else
                e.Graphics.DrawString("нет", fParametersFont, brNoBrush, 7 + iImageWidth + iBorderMargin + 200 + 2, iBorderMargin + 135);

            if (sIncomeNumber.Length > 0)
            {
                e.Graphics.DrawString("Входящий номер", fLabelsFont, brLabelsBrush, 7 + iImageWidth + iBorderMargin + 500 + 1, iBorderMargin + 115);
                e.Graphics.DrawString(sIncomeNumber, fParametersFont, brParametersBrush, 7 + iImageWidth + iBorderMargin + 500 + 2, iBorderMargin + 135);
            }

            e.Graphics.DrawString("получатели:", fLabelsFont, brLabelsBrush, 7 + iImageWidth + iBorderMargin + 1, iBorderMargin + 170);

            e.Graphics.DrawString("прикрепленные файлы:", fLabelsFont, brLabelsBrush, 7 + iImageWidth + iBorderMargin + 1, iBorderMargin + 270);
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            Parent.Focus();
        }

        private void OnCommentsTextBoxFileLabelClick(object sender, EventArgs e)
        {
            OnCommentsTextBoxFileLabelClicked(this, e);
        }

        private void OnControlPanelSizeChanged(object sender, int Height)
        {
            ChangeItemSize(Height);
        }

        public void CloseCommentsTextBox()
        {
            ControlPanel.CloseCommentsTextBox();
            CurrentFilesDataTable.Clear();
        }



        private void OnFileClick(object sender, int DocumentFileID)
        {
            OnFileClicked(sender, DocumentFileID);
        }

        public event ItemSizeChangedEventHandler ItemSizeChanged;
        public event EventHandler CommentsTextBoxOpened;
        public event EventHandler CommentsTextBoxFileLabelClicked;
        public event SendButtonEventHandler SendButtonClicked;
        public event CommentEditEventHandler EditCommentClicked;
        public event CommentEditEventHandler DeleteCommentClicked;
        public event FileClickEventHandler FileClicked;
        public event CommentFileClickEventHandler CommentFileClicked;
        public event AddConfirmClickEventHandler AddConfirmClicked;
        public event AddConfirmClickEventHandler AddUserClicked;
        public event CheckConfirmClickedEventHandler ItemConfirmClicked;
        public event CheckConfirmClickedEventHandler ItemCancelClicked;
        public event CheckConfirmClickedEventHandler ItemEditClicked;
        public event EditConfirmClickedEventHandler ConfirmEditClicked;
        public event EditConfirmClickedEventHandler ConfirmDeleteClicked;

        public delegate void ItemSizeChangedEventHandler(object sender, int ItemIndex, int Height);
        public delegate void SendButtonEventHandler(object sender, string Text, bool bIsEdit);
        public delegate void CommentEditEventHandler(object sender, int DocumentCommentID);
        public delegate void FileClickEventHandler(object sender, int DocumentFileID);
        public delegate void CommentFileClickEventHandler(object sender, int DocumentCommentFileID);
        public delegate void AddConfirmClickEventHandler(object sender, int DocumentID, int DocumentCategoryID);
        public delegate void EditConfirmClickedEventHandler(object sender, int DocumentConfirmationID);
        public delegate void CheckConfirmClickedEventHandler(object sender, int DocumentConfirmationRecipientID);

        public virtual void OnItemSizeChanged(object sender, int ItemIndex, int Height)
        {
            ItemSizeChanged?.Invoke(sender, ItemIndex, Height);
        }

        public virtual void OnCommentsTextBoxOpened(object sender, EventArgs e)
        {
            CurrentFilesDataTable.Clear();

            ControlPanel.CommentsTextBox.FilesLabel.Text = "Прикрепить файлы";

            CommentsTextBoxOpened?.Invoke(this, e);
        }

        public virtual void OnCommentsTextBoxFileLabelClicked(object sender, EventArgs e)
        {
            CommentsTextBoxFileLabelClicked?.Invoke(this, e);
        }

        public virtual void OnSendButtonClicked(object sender, string Text, bool bEdit)
        {
            SendButtonClicked?.Invoke(this, Text, bEdit);//Raise the event
        }

        public virtual void OnEditCommentClicked(object sender, int iDocumentCommentID)
        {
            EditCommentClicked?.Invoke(this, iDocumentCommentID);//Raise the event
        }

        public virtual void OnDeleteCommentClicked(object sender, int iDocumentCommentID)
        {
            DeleteCommentClicked?.Invoke(this, iDocumentCommentID);//Raise the event
        }

        public virtual void OnFileClicked(object sender, int DocumentFileID)
        {
            FileClicked?.Invoke(this, DocumentFileID);//Raise the event
        }

        public virtual void OnCommentFileClicked(object sender, int DocumentCommentFileID)
        {
            CommentFileClicked?.Invoke(this, DocumentCommentFileID);//Raise the event
        }

        public virtual void OnAddConfirmClicked(object sender, EventArgs e)
        {
            AddConfirmClicked?.Invoke(this, iDocumentID, iDocumentCategoryID);//Raise the event
        }

        public virtual void OnAddUserClicked(object sender, EventArgs e)
        {
            AddUserClicked?.Invoke(this, iDocumentID, iDocumentCategoryID);//Raise the event
        }

        public virtual void OnItemConfirmClicked(object sender, int DocumentConfirmationRecipientID)
        {
            ItemConfirmClicked?.Invoke(sender, DocumentConfirmationRecipientID);
        }

        public virtual void OnItemCancelClicked(object sender, int DocumentConfirmationRecipientID)
        {
            ItemCancelClicked?.Invoke(sender, DocumentConfirmationRecipientID);
        }

        public virtual void OnItemEditClicked(object sender, int DocumentConfirmationRecipientID)
        {
            ItemEditClicked?.Invoke(sender, DocumentConfirmationRecipientID);
        }

        public virtual void OnConfirmEditClicked(object sender, int DocumentConfirmationID)
        {
            ConfirmEditClicked?.Invoke(sender, DocumentConfirmationID);
        }

        public virtual void OnConfirmDeleteClicked(object sender, int DocumentConfirmationID)
        {
            ConfirmDeleteClicked?.Invoke(sender, DocumentConfirmationID);
        }


    }


    public class InfiniumDocumentsUpdatesList : Control
    {
        Font fCaptionFont = new Font("Segoe UI", 22.0f, FontStyle.Regular, GraphicsUnit.Pixel);

        SolidBrush brCaptionBrush = new SolidBrush(Color.FromArgb(110, 110, 110));

        public int Offset = 0;

        int iTempScrollWheelOffset = 0;
        int iMarginToNextItem = 20;

        public int iCurrentCommentsEditIndex = -1;

        public int iCurPosY = 0;

        public InfiniumDocumentsUpdatesScrollContainer ScrollContainer;
        public InfiniumProjectsVerticalScrollBar VerticalScroll;

        public InfiniumDocumentsUpdatesItem[] Items;

        public DataTable ItemsDataTable = null;
        public DataTable CommentsDataTable = null;
        public DataTable UsersDataTable = null;
        public DataTable DocumentsTypesDataTable = null;
        public DataTable RecipientsDataTable = null;
        public DataTable FilesDataTable = null;
        public DataTable FactoryDataTable = null;
        public DataTable CommentsFilesDataTable = null;
        public DataTable CorrespondentsDataTable = null;
        public DataTable DocumentsCategoriesDataTable = null;
        public DataTable ConfirmsDataTable = null;
        public DataTable ConfirmsRecipientsDataTable = null;

        public int DocumentCommentID = -1;

        public InfiniumDocumentsUpdatesList()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);

            VerticalScroll = new InfiniumProjectsVerticalScrollBar()
            {
                Parent = this,
                Height = this.Height
            };
            VerticalScroll.Left = this.Width - VerticalScroll.Width;
            VerticalScroll.TotalControlHeight = this.Height;
            VerticalScroll.Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            VerticalScroll.Width = 12;
            VerticalScroll.ScrollPositionChanged += OnScrollPositionChanged;

            ScrollContainer = new InfiniumDocumentsUpdatesScrollContainer()
            {
                Parent = this,
                Top = 0,
                Left = 0,
                Width = this.Width,
                Height = this.Height,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            ScrollContainer.SizeChanged += OnScrollContainerSizeChanged;
            ScrollContainer.Paint += OnScrollContainerPaint;
        }

        public void InitializeItems()
        {
            if (Items != null)
            {
                for (int i = 0; i < Items.Count(); i++)
                {
                    if (Items[i] != null)
                    {
                        Items[i].Dispose();
                        Items[i] = null;
                    }
                }

                Items = new InfiniumDocumentsUpdatesItem[0];
            }

            iCurPosY = 0;
            Offset = 0;
            ScrollContainer.Top = -Offset;
            VerticalScroll.Offset = Offset;
            VerticalScroll.Refresh();

            if (ItemsDataTable == null)
                return;

            if (ItemsDataTable.DefaultView.Count > 0)
            {
                Items = new InfiniumDocumentsUpdatesItem[ItemsDataTable.DefaultView.Count];

                for (int i = 0; i < ItemsDataTable.DefaultView.Count; i++)
                {
                    iCurPosY += iMarginToNextItem;
                    Items[i] = new InfiniumDocumentsUpdatesItem()
                    {
                        Parent = ScrollContainer,
                        Left = iMarginToNextItem,
                        Width = ScrollContainer.Width - iMarginToNextItem - iMarginToNextItem,

                        Top = iCurPosY,
                        iDocumentID = Convert.ToInt32(ItemsDataTable.DefaultView[i]["DocumentID"]),
                        iDocumentCategoryID = Convert.ToInt32(ItemsDataTable.DefaultView[i]["DocumentCategoryID"]),
                        sUserName = UsersDataTable.Select("UserID = " + ItemsDataTable.DefaultView[i]["UserID"])[0]["Name"].ToString(),
                        UserID = Convert.ToInt32(ItemsDataTable.DefaultView[i]["UserID"]),
                        sDocType = DocumentsTypesDataTable.Select("DocumentTypeID = " + ItemsDataTable.DefaultView[i]["DocumentTypeID"])[0]["DocumentType"].ToString(),
                        sCategory = DocumentsCategoriesDataTable.Select("DocumentCategoryID = " + ItemsDataTable.DefaultView[i]["DocumentCategoryID"])[0]["DocumentCategory"].ToString()
                    };
                    if (ItemsDataTable.DefaultView[i]["CorrespondentID"] != DBNull.Value)
                        if (Convert.ToInt32(ItemsDataTable.DefaultView[i]["CorrespondentID"]) != -1)
                            Items[i].sCorrespondent = CorrespondentsDataTable.Select("CorrespondentID = " + ItemsDataTable.DefaultView[i]["CorrespondentID"])[0]["CorrespondentName"].ToString();
                    Items[i].sDate = Convert.ToDateTime(ItemsDataTable.DefaultView[i]["DateTime"]).ToString("dd.MM.yyyy HH:mm");
                    Items[i].sDescription = ItemsDataTable.DefaultView[i]["Description"].ToString();
                    Items[i].sRegNumber = ItemsDataTable.DefaultView[i]["RegNumber"].ToString();
                    Items[i].sFactory = FactoryDataTable.Select("FactoryID = " + ItemsDataTable.DefaultView[i]["FactoryID"])[0]["Factory"].ToString();
                    if (Items[i].iDocumentCategoryID == 1)//income
                        Items[i].sIncomeNumber = ItemsDataTable.DefaultView[i]["IncomeNumber"].ToString();

                    if (RecipientsDataTable.Select("UserID = " + Items[i].UserID + " AND DocumentCategoryID = " + Items[i].iDocumentCategoryID + " AND DocumentID = " + Items[i].iDocumentID).Count() > 0)
                        Items[i].bInRecipients = true;

                    Items[i].iItemIndex = i;


                    if (UsersDataTable.Select("UserID = " + Items[i].UserID)[0]["Photo"] != DBNull.Value)
                    {
                        byte[] b = (byte[])UsersDataTable.Select("UserID = " + Items[i].UserID)[0]["Photo"];

                        using (MemoryStream ms = new MemoryStream(b))
                        {
                            Items[i].Image = (Bitmap)Image.FromStream(ms);
                        }
                    }
                    else
                        Items[i].Image = Properties.Resources.NoImage;

                    Items[i].UsersDataTable = UsersDataTable;
                    Items[i].RecipientsDataTable = RecipientsDataTable;

                    //comments and comments files
                    Items[i].CommentsDataTable = new DataTable();
                    Items[i].CommentsDataTable = CommentsDataTable.Clone();
                    Items[i].CommentsFilesDataTable = new DataTable();
                    Items[i].CommentsFilesDataTable = CommentsFilesDataTable.Clone();

                    foreach (DataRow cRow in CommentsDataTable.Select("DocumentCategoryID = " + Items[i].iDocumentCategoryID + " AND DocumentID = " + ItemsDataTable.DefaultView[i]["DocumentID"]))
                    {
                        Items[i].CommentsDataTable.Rows.Add(cRow.ItemArray);

                        foreach (DataRow rRow in CommentsFilesDataTable.Select("DocumentCommentID = " + cRow["DocumentCommentID"]))
                        {
                            Items[i].CommentsFilesDataTable.Rows.Add(rRow.ItemArray);
                        }
                    }


                    //recipients
                    Items[i].RecipientsDataTable = new DataTable();
                    Items[i].RecipientsDataTable = RecipientsDataTable.Clone();

                    foreach (DataRow rRow in RecipientsDataTable.Select("DocumentID = " + ItemsDataTable.DefaultView[i]["DocumentID"]))
                    {
                        Items[i].RecipientsDataTable.Rows.Add(rRow.ItemArray);
                    }


                    //files
                    Items[i].FilesDataTable = new DataTable();
                    Items[i].FilesDataTable = FilesDataTable.Clone();

                    foreach (DataRow rRow in FilesDataTable.Select("DocumentID = " + ItemsDataTable.DefaultView[i]["DocumentID"]))
                    {
                        Items[i].FilesDataTable.Rows.Add(rRow.ItemArray);
                    }

                    //confirms
                    Items[i].ConfirmsDataTable = new DataTable();
                    Items[i].ConfirmsDataTable = ConfirmsDataTable.Clone();

                    foreach (DataRow cRow in ConfirmsDataTable.Select("DocumentID = " + ItemsDataTable.DefaultView[i]["DocumentID"]))
                    {
                        Items[i].ConfirmsDataTable.Rows.Add(cRow.ItemArray);
                    }

                    Items[i].ConfirmsRecipientsDataTable = new DataTable();
                    Items[i].ConfirmsRecipientsDataTable = ConfirmsRecipientsDataTable.Clone();

                    if (Items[i].ConfirmsDataTable.Rows.Count > 0)
                        foreach (DataRow cRow in ConfirmsRecipientsDataTable.Select("DocumentConfirmationID = " + Items[i].ConfirmsDataTable.Rows[0]["DocumentConfirmationID"]))
                        {
                            Items[i].ConfirmsRecipientsDataTable.Rows.Add(cRow.ItemArray);
                        }


                    Items[i].Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                    Items[i].ItemSizeChanged += OnItemSizeChanged;
                    Items[i].CommentsTextBoxOpened += OnCommentsTextBoxOpened;
                    Items[i].CommentsTextBoxFileLabelClicked += OnCommentsTextBoxFileLabelClick;
                    Items[i].SendButtonClicked += OnCommentsSendButtonClick;
                    Items[i].EditCommentClicked += OnEditCommentClick;
                    Items[i].DeleteCommentClicked += OnDeleteCommentClick;
                    Items[i].FileClicked += OnFileClicked;
                    Items[i].CommentFileClicked += OnCommentFileClicked;
                    Items[i].AddConfirmClicked += OnAddConfirmClicked;
                    Items[i].AddUserClicked += OnAddUserClicked;
                    Items[i].ItemConfirmClicked += OnItemConfirmClicked;
                    Items[i].ItemCancelClicked += OnItemCancelClicked;
                    Items[i].ItemEditClicked += OnItemEditClicked;
                    Items[i].ConfirmEditClicked += OnConfirmEditClicked;
                    Items[i].ConfirmDeleteClicked += OnConfirmDeleteClicked;

                    Items[i].Initialize();

                    iCurPosY += Items[i].Height;
                }
            }

            iCurPosY += iMarginToNextItem;

            if (iCurPosY > this.Height)
                ScrollContainer.Height = iCurPosY;
            else
                ScrollContainer.Height = this.Height;

            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
            VerticalScroll.Refresh();

            ScrollContainer.Refresh();
        }


        public void SetHeight(int ItemIndex, int iHeight)
        {
            for (int i = ItemIndex + 1; i < Items.Count(); i++)
                Items[i].Top += iHeight;


            int iH = 0;

            iH += iMarginToNextItem;

            foreach (InfiniumDocumentsUpdatesItem Item in Items)
            {
                iH += Item.Height;
                iH += iMarginToNextItem;
            }

            if (this.Height >= iH)
            {
                ScrollContainer.Height = this.Height;
                Offset = 0;
                iTempScrollWheelOffset = 0;
                ScrollContainer.Top = 0;
            }
            else
                ScrollContainer.Height = iH;

            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
            VerticalScroll.Refresh();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public InfiniumProjectsVerticalScrollBar VerticalScrollBar
        {
            get { return VerticalScroll; }
            set { VerticalScroll = value; VerticalScroll.Left = this.Width - VerticalScroll.Width; }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            if (iCurPosY > this.Height)
                ScrollContainer.Height = iCurPosY;
            else
                ScrollContainer.Height = this.Height;

            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
            VerticalScroll.Refresh();

            ScrollContainer.Refresh();
        }

        private void OnScrollContainerPaint(object sender, PaintEventArgs e)
        {
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            if (Items == null || (Items != null && Items.Count() == 0))
                e.Graphics.DrawString("Нет обновлений", fCaptionFont, brCaptionBrush,
                                        (this.Width - e.Graphics.MeasureString("Нет обновлений", fCaptionFont).Width) / 2,
                                        (this.Height - e.Graphics.MeasureString("Нет обновлений", fCaptionFont).Height) / 2);
        }

        private void OnScrollContainerSizeChanged(object sender, EventArgs e)
        {
            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
        }

        private void OnScrollPositionChanged(object sender, int tOffset)
        {
            Offset = tOffset;
            ScrollContainer.Top = -Offset;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (!VerticalScroll.Visible)
                return;

            if (Offset == 0)
                if (iTempScrollWheelOffset > 0)
                    iTempScrollWheelOffset = 0;

            if (e.Delta < 0)
            {
                if (Offset < ScrollContainer.Height - this.Height)
                {
                    if (Offset + VerticalScroll.ScrollWheelOffset + this.Height > ScrollContainer.Height)//last
                    {
                        iTempScrollWheelOffset = ScrollContainer.Height - this.Height - Offset;
                        Offset += iTempScrollWheelOffset;
                    }
                    else
                    {
                        Offset += VerticalScroll.ScrollWheelOffset;
                    }

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
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
                        Offset -= VerticalScroll.ScrollWheelOffset;

                    if (Offset < 0)
                        Offset = 0;

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
                }
        }

        private void OnItemSizeChanged(object sender, int ItemIndex, int Height)
        {
            SetHeight(ItemIndex, Height);
        }

        private void OnCommentsTextBoxOpened(object sender, EventArgs e)
        {
            if (iCurrentCommentsEditIndex != -1 && iCurrentCommentsEditIndex != ((InfiniumDocumentsUpdatesItem)sender).iItemIndex)
                Items[iCurrentCommentsEditIndex].CloseCommentsTextBox();

            iCurrentCommentsEditIndex = ((InfiniumDocumentsUpdatesItem)sender).iItemIndex;

            if (!(((InfiniumDocumentsUpdatesItem)sender).ControlPanel.CommentsTextBox.bEdit))
                DocumentCommentID = -1;
        }

        private void OnCommentsTextBoxFileLabelClick(object sender, EventArgs e)
        {
            OnCommentsTextBoxFileLabelClicked(sender, e);
        }

        private void OnCommentsSendButtonClick(object sender, string Text, bool bIsNew)
        {
            OnCommentsSendButtonClicked(sender, ((InfiniumDocumentsUpdatesItem)sender).iDocumentID, DocumentCommentID,
                ((InfiniumDocumentsUpdatesItem)sender).iDocumentCategoryID, Text, bIsNew,
                ((InfiniumDocumentsUpdatesItem)sender).CurrentFilesDataTable);
        }

        private void OnEditCommentClick(object sender, int iDocumentCommentID)
        {
            DocumentCommentID = iDocumentCommentID;

            InfiniumDocumentsUpdatesItem Item = (InfiniumDocumentsUpdatesItem)sender;

            int iComPos = Item.Top - Offset + Item.ControlPanel.Top + Item.ControlPanel.CommentsTextBox.Top + Item.ControlPanel.CommentsTextBox.RichTextBox.Top;

            if (iComPos + Item.ControlPanel.CommentsTextBox.Height > this.Height)
                Offset += (iComPos + Item.ControlPanel.CommentsTextBox.Height) - this.Height;

            ScrollContainer.Top = -Offset;
            VerticalScrollBar.Offset = Offset;
            VerticalScrollBar.Refresh();

            OnEditCommentClicked(sender, iDocumentCommentID);
        }

        private void OnDeleteCommentClick(object sender, int iDocumentCommentID)
        {
            OnDeleteCommentClicked(sender, iDocumentCommentID);
        }

        public event EventHandler CommentsTextBoxFileLabelClicked;
        public event CommentsSendButtonClickedEventHandler CommentsSendButtonClicked;
        public event CommentEditEventHandler EditCommentClicked;
        public event CommentEditEventHandler DeleteCommentClicked;
        public event FileClickEventHandler FileClicked;
        public event CommentFileClickEventHandler CommentFileClicked;
        public event AddConfirmClickEventHandler AddConfirmClicked;
        public event AddConfirmClickEventHandler AddUserClicked;
        public event CheckConfirmClickedEventHandler ConfirmItemConfirmClicked;
        public event CheckConfirmClickedEventHandler ConfirmItemCancelClicked;
        public event CheckConfirmClickedEventHandler ConfirmItemEditClicked;
        public event EditConfirmClickedEventHandler ConfirmEditClicked;
        public event EditConfirmClickedEventHandler ConfirmDeleteClicked;

        public delegate void CommentsSendButtonClickedEventHandler(object sender, int DocumentID, int DocumentCommentID, int DocumentCategoryID, string Text, bool bIsNew, DataTable FilesDataTable);
        public delegate void CommentEditEventHandler(object sender, int DocumentCommentID);
        public delegate void FileClickEventHandler(object sender, int DocumentFileID);
        public delegate void CommentFileClickEventHandler(object sender, int DocumentCommentFileID);
        public delegate void AddConfirmClickEventHandler(object sender, int DocumentID, int DocumentCategoryID);
        public delegate void EditConfirmClickedEventHandler(object sender, int DocumentConfirmationID);
        public delegate void CheckConfirmClickedEventHandler(object sender, int DocumentConfirmationRecipientID);

        public virtual void OnCommentsTextBoxFileLabelClicked(object sender, EventArgs e)
        {
            CommentsTextBoxFileLabelClicked?.Invoke(sender, e);
        }

        public virtual void OnCommentsSendButtonClicked(object sender, int DocumentID, int DocumentCommentID, int DocumentCategoryID, string Text, bool bIsNew, DataTable FilesDataTable)
        {
            CommentsSendButtonClicked?.Invoke(sender, DocumentID, DocumentCommentID, DocumentCategoryID, Text, bIsNew, FilesDataTable);
        }

        public virtual void OnEditCommentClicked(object sender, int iDocumentCommentID)
        {
            EditCommentClicked?.Invoke(sender, iDocumentCommentID);
        }

        public virtual void OnDeleteCommentClicked(object sender, int iDocumentCommentID)
        {
            DeleteCommentClicked?.Invoke(sender, iDocumentCommentID);
        }

        public virtual void OnFileClicked(object sender, int DocumentFileID)
        {
            FileClicked?.Invoke(sender, DocumentFileID);
        }

        public virtual void OnCommentFileClicked(object sender, int DocumentCommentFileID)
        {
            CommentFileClicked?.Invoke(this, DocumentCommentFileID);//Raise the event
        }

        public virtual void OnAddConfirmClicked(object sender, int iDocumentID, int iDocumentCategoryID)
        {
            if (ConfirmsDataTable.Select("DocumentID = " + iDocumentID + " AND DocumentCategoryID = " + iDocumentCategoryID).Count() > 0)
            {
                if (AddConfirmClicked != null)
                    OnConfirmEditClicked(this, Convert.ToInt32(ConfirmsDataTable.Select("DocumentID = " + iDocumentID + " AND DocumentCategoryID = " + iDocumentCategoryID)[0]["DocumentConfirmationID"]));
            }
            else
            {
                AddConfirmClicked?.Invoke(this, iDocumentID, iDocumentCategoryID);//Raise the event
            }
        }

        public virtual void OnAddUserClicked(object sender, int iDocumentID, int iDocumentCategoryID)
        {
            AddUserClicked?.Invoke(this, iDocumentID, iDocumentCategoryID);//Raise the event
        }

        public virtual void OnItemConfirmClicked(object sender, int DocumentConfirmationRecipientID)
        {
            ConfirmItemConfirmClicked?.Invoke(sender, DocumentConfirmationRecipientID);
        }

        public virtual void OnItemCancelClicked(object sender, int DocumentConfirmationRecipientID)
        {
            ConfirmItemCancelClicked?.Invoke(sender, DocumentConfirmationRecipientID);
        }

        public virtual void OnItemEditClicked(object sender, int DocumentConfirmationRecipientID)
        {
            ConfirmItemEditClicked?.Invoke(sender, DocumentConfirmationRecipientID);
        }

        public virtual void OnConfirmEditClicked(object sender, int DocumentConfirmationID)
        {
            ConfirmEditClicked?.Invoke(sender, DocumentConfirmationID);
        }

        public virtual void OnConfirmDeleteClicked(object sender, int DocumentConfirmationID)
        {
            ConfirmDeleteClicked?.Invoke(sender, DocumentConfirmationID);
        }

    }




    public class InfiniumDocumentsUpdatesUserItem : Control
    {
        Font fUserNameFont = new Font("Segoe UI", 11.0f, FontStyle.Regular, GraphicsUnit.Pixel);

        SolidBrush brCaptionBrush = new SolidBrush(Color.FromArgb(162, 162, 162));

        Pen pBorderPen = new Pen(new SolidBrush(Color.FromArgb(252, 252, 252)));

        Rectangle rImageRect = new Rectangle(0, 0, 0, 0);

        public string sUserName = "";

        public int UserID = -1;

        public int iItemHeight = 61;
        public int iItemWidth = 46;

        public int iImageWidth = 44;
        public int iImageHeight = 41;

        bool bTrack = false;
        public Bitmap Image;

        public InfiniumDocumentsUpdatesUserItem()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            this.Width = iItemWidth;
            this.Height = iItemHeight;

            this.BackColor = Color.Transparent;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            rImageRect.Width = iImageWidth + 1;
            rImageRect.Height = iImageHeight + 1;

            e.Graphics.DrawRectangle(pBorderPen, rImageRect);

            Image.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);
            e.Graphics.DrawImage(Image, 1, 1, iImageWidth, iImageHeight);

            string text = "";

            int w = Convert.ToInt32(this.Width);

            string Name = Path.GetFileNameWithoutExtension(sUserName);

            if (e.Graphics.MeasureString(Name, fUserNameFont).Width > w)
            {
                for (int i = Name.Length; i >= 0; i--)
                {
                    if (e.Graphics.MeasureString(Name.Substring(0, i), fUserNameFont).Width <= w)
                    {
                        text = Name.Substring(0, i - 1) + "...";
                        break;
                    }
                }
            }
            else
                text = Name;

            e.Graphics.DrawString(text, fUserNameFont, brCaptionBrush, (this.Width - e.Graphics.MeasureString(text, fUserNameFont).Width) / 2, iImageHeight + 2);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (Parent != null)
                if (!Parent.Focused)
                    Parent.Focus();

            if (!bTrack)
            {
                bTrack = true;
                this.Refresh();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (bTrack)
            {
                bTrack = false;
                this.Refresh();
            }
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            if (Parent != null)
                Parent.Focus();
        }

    }


    public class InfiniumDocumentsUpdatesUsersList : Control
    {
        public int Offset = 0;

        int iTempScrollWheelOffset = 0;

        public InfiniumDocumentsUpdatesScrollContainer ScrollContainer;
        public InfiniumHorizontalScrollBar HorizontalScroll;

        public InfiniumDocumentsUpdatesUserItem[] Items;

        public DataTable ItemsDataTable;
        public DataTable UsersDataTable;

        int iMarginToNextItem = 10;

        public int DocType = -1;
        public int DocID = -1;

        public InfiniumDocumentsUpdatesUsersList()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            HorizontalScroll = new InfiniumHorizontalScrollBar()
            {
                Parent = this,
                Width = this.Width
            };
            HorizontalScroll.Top = this.Top - HorizontalScroll.Top;
            HorizontalScroll.TotalControlWidth = this.Width;
            HorizontalScroll.ScrollPositionChanged += OnScrollPositionChanged;
            HorizontalScroll.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            HorizontalScroll.Height = 9;
            HorizontalScroll.HorizontalScrollCommonShaftBackColor = Color.FromArgb(250, 250, 250);
            HorizontalScroll.HorizontalScrollCommonThumbButtonColor = Color.FromArgb(192, 192, 192);
            HorizontalScroll.HorizontalScrollTrackingShaftBackColor = Color.FromArgb(250, 250, 250);
            HorizontalScroll.HorizontalScrollTrackingThumbButtonColor = Color.FromArgb(180, 180, 180);

            ScrollContainer = new InfiniumDocumentsUpdatesScrollContainer()
            {
                Parent = this,
                Top = 0,
                Left = 0,
                Width = this.Width,
                Height = this.Height,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            ScrollContainer.SizeChanged += OnScrollContainerSizeChanged;
            ScrollContainer.BackColor = Color.Transparent;

            this.Height = 75;
            this.BackColor = Color.Transparent;
        }

        public void InitializeItems()
        {
            if (Items != null)
            {
                for (int i = 0; i < Items.Count(); i++)
                {
                    if (Items[i] != null)
                    {
                        Items[i].Dispose();
                        Items[i] = null;
                    }
                }

                Items = new InfiniumDocumentsUpdatesUserItem[0];
            }

            Offset = 0;
            ScrollContainer.Left = -Offset;
            HorizontalScroll.Offset = Offset;
            HorizontalScroll.Refresh();

            if (ItemsDataTable == null)
                return;

            if (ItemsDataTable.Rows.Count > 0)
            {
                Items = new InfiniumDocumentsUpdatesUserItem[ItemsDataTable.Rows.Count];

                for (int i = 0; i < ItemsDataTable.Rows.Count; i++)
                {
                    Items[i] = new InfiniumDocumentsUpdatesUserItem();
                    Items[i].Left = i * (Items[i].Width + iMarginToNextItem);
                    Items[i].sUserName = GetName(UsersDataTable.Select("UserID = " + ItemsDataTable.Rows[i]["UserID"])[0]["Name"].ToString());
                    Items[i].UserID = Convert.ToInt32(ItemsDataTable.Rows[i]["UserID"]);

                    if (UsersDataTable.Select("UserID = " + Items[i].UserID)[0]["Photo"] != DBNull.Value)
                    {
                        byte[] b = (byte[])UsersDataTable.Select("UserID = " + Items[i].UserID)[0]["Photo"];

                        using (MemoryStream ms = new MemoryStream(b))
                        {
                            Items[i].Image = (Bitmap)Image.FromStream(ms);
                        }
                    }
                    else
                        Items[i].Image = Properties.Resources.NoImage;

                    Items[i].Parent = ScrollContainer;
                    Items[i].Anchor = AnchorStyles.Left | AnchorStyles.Top;
                }
            }

            SetScrollWidth();
        }


        public string GetName(string UserName)
        {
            int c = 0;

            string Name = "";

            for (int i = 0; i < UserName.Length; i++)
            {
                if (UserName[i] != ' ')
                {
                    if (c == 0)
                        continue;
                    else
                        Name += UserName[i];
                }
                else
                {
                    if (c == 0)
                        c++;
                    else
                        break;
                }
            }

            return Name;
        }


        private void SetScrollWidth()
        {
            if (Items == null)
            {
                ScrollContainer.Width = this.Width;
                HorizontalScroll.TotalControlWidth = ScrollContainer.Width;
                HorizontalScroll.Refresh();

                return;
            }

            if (Items.Count() == 0)
            {
                ScrollContainer.Width = this.Width;
                HorizontalScroll.TotalControlWidth = ScrollContainer.Width;
                HorizontalScroll.Refresh();

                return;
            }

            if (Items.Count() * (Items[0].Width + iMarginToNextItem) > this.Width)
                ScrollContainer.Width = Items.Count() * (Items[0].Width + iMarginToNextItem);
            else
                ScrollContainer.Width = this.Width;

            HorizontalScroll.TotalControlWidth = ScrollContainer.Width;
            HorizontalScroll.Refresh();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public InfiniumHorizontalScrollBar HorizontalScrollBar
        {
            get { return HorizontalScroll; }
            set { HorizontalScroll = value; HorizontalScroll.Top = this.Height - HorizontalScroll.Height; }
        }


        private void OnScrollContainerSizeChanged(object sender, EventArgs e)
        {
            HorizontalScroll.TotalControlWidth = ScrollContainer.Width;
        }

        private void OnScrollPositionChanged(object sender, int tOffset)
        {
            Offset = tOffset;
            ScrollContainer.Left = -Offset;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (!HorizontalScroll.Visible)
                return;

            if (Offset == 0)
                if (iTempScrollWheelOffset > 0)
                    iTempScrollWheelOffset = 0;

            if (e.Delta < 0)
            {
                if (Offset < ScrollContainer.Width - this.Width)
                {
                    if (Offset + HorizontalScroll.ScrollWheelOffset + this.Width > ScrollContainer.Width)//last
                    {
                        iTempScrollWheelOffset = ScrollContainer.Width - this.Width - Offset;
                        Offset += iTempScrollWheelOffset;
                    }
                    else
                    {
                        Offset += HorizontalScroll.ScrollWheelOffset;
                    }

                    ScrollContainer.Left = -Offset;
                    HorizontalScroll.Offset = Offset;
                    HorizontalScroll.Refresh();
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
                        Offset -= HorizontalScroll.ScrollWheelOffset;

                    if (Offset < 0)
                        Offset = 0;

                    ScrollContainer.Left = -Offset;
                    HorizontalScroll.Offset = Offset;
                    HorizontalScroll.Refresh();
                }
        }

        private void OnVerticalScrollVisibleChanged(object sender, EventArgs e)
        {
            if (HorizontalScroll.Visible)
                ScrollContainer.Width = this.Width - HorizontalScroll.Width;
            else
                ScrollContainer.Width = this.Width;
        }


        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            ScrollContainer.Focus();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            SetScrollWidth();

            HorizontalScroll.Width = this.Width;
            HorizontalScroll.Top = this.Height - HorizontalScroll.Height;
        }
    }




    public class InfiniumDocumentsUpdatesFileItem : Control
    {
        Color cCaptionColor = Color.FromArgb(100, 100, 100);
        Color cBorderColor = Color.FromArgb(240, 240, 240);

        Font fCaptionFont = new Font("Segoe UI", 12.0f, FontStyle.Regular, GraphicsUnit.Pixel);

        SolidBrush brCaptionBrush;

        Pen pBorderPen;

        string sCaption = "Item";

        public int iItemHeight = 80;
        public int iItemWidth = 90;
        bool bTrack = false;
        public Bitmap Image;

        public int iDocumentFileID = -1;

        public InfiniumDocumentsUpdatesFileItem()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            this.Width = iItemWidth;
            this.Height = iItemHeight;

            this.BackColor = Color.Transparent;
            this.Cursor = Cursors.Hand;

            brCaptionBrush = new SolidBrush(cCaptionColor);

            pBorderPen = new Pen(new SolidBrush(cBorderColor));
        }


        public string Caption
        {
            get { return sCaption; }
            set { sCaption = value; this.Refresh(); }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            //e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            if (Image != null)
            {
                Image.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);
                e.Graphics.DrawImage(Image, (this.Width - 40) / 2, 5, 40, 40);
            }

            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            string text = "";

            int w = Convert.ToInt32(this.Width - 10 - e.Graphics.MeasureString(Path.GetExtension(sCaption), fCaptionFont).Width);

            string Name = Path.GetFileNameWithoutExtension(sCaption);

            if (e.Graphics.MeasureString(Name, fCaptionFont).Width > w)
            {
                for (int i = Name.Length; i >= 0; i--)
                {
                    if (e.Graphics.MeasureString(Name.Substring(0, i), fCaptionFont).Width <= w)
                    {
                        text = Name.Substring(0, i - 1) + "...";
                        break;
                    }
                }
            }
            else
                text = Name;

            e.Graphics.DrawString(text + Path.GetExtension(sCaption), fCaptionFont, brCaptionBrush, (this.Width - e.Graphics.MeasureString(text + Path.GetExtension(sCaption), fCaptionFont).Width) / 2,
                                  40 + 8);

            if (bTrack)
            {
                e.Graphics.DrawLine(pBorderPen, 0, 1, this.Width - 1, 1);
                e.Graphics.DrawLine(pBorderPen, 0, this.Height - 1, this.Width - 1, this.Height - 1);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (Parent != null)
                if (!Parent.Focused)
                    Parent.Focus();

            if (!bTrack)
            {
                bTrack = true;
                this.Refresh();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (bTrack)
            {
                bTrack = false;
                this.Refresh();
            }
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            if (Parent != null)
                Parent.Focus();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            OnItemClicked(this, iDocumentFileID);
        }


        public event ItemClickedEventHandler ItemClicked;

        public delegate void ItemClickedEventHandler(object sender, int DocumentFileID);

        public virtual void OnItemClicked(object sender, int DocumentFileID)
        {
            ItemClicked?.Invoke(sender, DocumentFileID);//Raise the event
        }
    }


    public class InfiniumDocumentsUpdatesFilesList : Control
    {
        public int Offset = 0;

        int iTempScrollWheelOffset = 0;

        public InfiniumDocumentsUpdatesScrollContainer ScrollContainer;
        public InfiniumHorizontalScrollBar HorizontalScroll;

        public InfiniumDocumentsUpdatesFileItem[] Items;

        public DataTable ItemsDataTable;

        int iMarginToNextItem = 5;

        public int DocType = -1;
        public int DocID = -1;

        Bitmap ImageFileBMP = Properties.Resources.ImageFileBig;
        Bitmap PDFFileBMP = Properties.Resources.PDFFileBig;
        Bitmap ExcelFileBMP = Properties.Resources.ExcelFileBig;
        Bitmap WordFileBMP = Properties.Resources.WordFileBig;
        Bitmap ArchiveFileBMP = Properties.Resources.ArchiveFileBig;
        Bitmap OtherFileBMP = Properties.Resources.OtherFileBig;

        public InfiniumDocumentsUpdatesFilesList()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            HorizontalScroll = new InfiniumHorizontalScrollBar()
            {
                Parent = this,
                Width = this.Width
            };
            HorizontalScroll.Top = this.Top - HorizontalScroll.Top;
            HorizontalScroll.TotalControlWidth = this.Width;
            HorizontalScroll.ScrollPositionChanged += OnScrollPositionChanged;
            HorizontalScroll.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            HorizontalScroll.Height = 9;
            HorizontalScroll.HorizontalScrollCommonShaftBackColor = Color.FromArgb(250, 250, 250);
            HorizontalScroll.HorizontalScrollCommonThumbButtonColor = Color.FromArgb(192, 192, 192);
            HorizontalScroll.HorizontalScrollTrackingShaftBackColor = Color.FromArgb(250, 250, 250);
            HorizontalScroll.HorizontalScrollTrackingThumbButtonColor = Color.FromArgb(180, 180, 180);

            ScrollContainer = new InfiniumDocumentsUpdatesScrollContainer()
            {
                Parent = this,
                Top = 0,
                Left = 0,
                Width = this.Width,
                Height = this.Height,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            ScrollContainer.SizeChanged += OnScrollContainerSizeChanged;
            ScrollContainer.BackColor = Color.Transparent;

            this.Height = 80;
            this.BackColor = Color.Transparent;
        }

        public void InitializeItems()
        {
            if (Items != null)
            {
                for (int i = 0; i < Items.Count(); i++)
                {
                    if (Items[i] != null)
                    {
                        Items[i].Dispose();
                        Items[i] = null;
                    }
                }

                Items = new InfiniumDocumentsUpdatesFileItem[0];
            }

            Offset = 0;
            ScrollContainer.Left = -Offset;
            HorizontalScroll.Offset = Offset;
            HorizontalScroll.Refresh();

            if (ItemsDataTable == null)
                return;

            if (ItemsDataTable.Rows.Count > 0)
            {
                Items = new InfiniumDocumentsUpdatesFileItem[ItemsDataTable.Rows.Count];

                for (int i = 0; i < ItemsDataTable.Rows.Count; i++)
                {
                    Items[i] = new InfiniumDocumentsUpdatesFileItem();
                    Items[i].Left = i * (Items[i].iItemWidth + iMarginToNextItem);
                    Items[i].Caption = ItemsDataTable.Rows[i]["FileName"].ToString();
                    Items[i].iDocumentFileID = Convert.ToInt32(ItemsDataTable.Rows[i][0]);

                    string Ext = Path.GetExtension(ItemsDataTable.Rows[i]["FileName"].ToString());

                    if (Ext.Equals(".pdf", StringComparison.InvariantCultureIgnoreCase))
                    {
                        Items[i].Image = PDFFileBMP;
                    }
                    else
                        if (Ext.Equals(".doc", StringComparison.InvariantCultureIgnoreCase) ||
                            Ext.Equals(".docx", StringComparison.InvariantCultureIgnoreCase))
                    {
                        Items[i].Image = WordFileBMP;
                    }
                    else
                            if (Ext.Equals(".xls", StringComparison.InvariantCultureIgnoreCase) ||
                                Ext.Equals(".xlsx", StringComparison.InvariantCultureIgnoreCase))
                    {
                        Items[i].Image = ExcelFileBMP;
                    }
                    else
                                if (Ext.Equals(".zip", StringComparison.InvariantCultureIgnoreCase) ||
                                    Ext.Equals(".rar", StringComparison.InvariantCultureIgnoreCase))
                    {
                        Items[i].Image = ArchiveFileBMP;
                    }
                    else
                                    if (Ext.Equals(".jpg", StringComparison.InvariantCultureIgnoreCase) ||
                                        Ext.Equals(".jpeg", StringComparison.InvariantCultureIgnoreCase) ||
                                        Ext.Equals(".png", StringComparison.InvariantCultureIgnoreCase) ||
                                        Ext.Equals(".bmp", StringComparison.InvariantCultureIgnoreCase) ||
                                        Ext.Equals(".tiff", StringComparison.InvariantCultureIgnoreCase) ||
                                        Ext.Equals(".tif", StringComparison.InvariantCultureIgnoreCase) ||
                                        Ext.Equals(".gif", StringComparison.InvariantCultureIgnoreCase) ||
                                        Ext.Equals(".tga", StringComparison.InvariantCultureIgnoreCase) ||
                                        Ext.Equals(".psd", StringComparison.InvariantCultureIgnoreCase) ||
                                        Ext.Equals(".wmf", StringComparison.InvariantCultureIgnoreCase) ||
                                        Ext.Equals(".emf", StringComparison.InvariantCultureIgnoreCase))
                    {
                        Items[i].Image = ImageFileBMP;
                    }
                    else
                    {
                        Items[i].Image = OtherFileBMP;
                    }


                    Items[i].Parent = ScrollContainer;
                    Items[i].Anchor = AnchorStyles.Left | AnchorStyles.Top;
                    Items[i].ItemClicked += OnItemClick;
                }
            }

            SetScrollWidth();
        }

        private void SetScrollWidth()
        {
            if (Items == null)
            {
                ScrollContainer.Width = this.Width;
                HorizontalScroll.TotalControlWidth = ScrollContainer.Width;
                HorizontalScroll.Refresh();

                return;
            }

            if (Items.Count() == 0)
            {
                ScrollContainer.Width = this.Width;
                HorizontalScroll.TotalControlWidth = ScrollContainer.Width;
                HorizontalScroll.Refresh();

                return;
            }

            if (Items.Count() * (Items[0].Width + iMarginToNextItem) > this.Width)
                ScrollContainer.Width = Items.Count() * (Items[0].Width + iMarginToNextItem);
            else
                ScrollContainer.Width = this.Width;

            HorizontalScroll.TotalControlWidth = ScrollContainer.Width;
            HorizontalScroll.Refresh();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public InfiniumHorizontalScrollBar HorizontalScrollBar
        {
            get { return HorizontalScroll; }
            set { HorizontalScroll = value; HorizontalScroll.Top = this.Height - HorizontalScroll.Height; }
        }


        private void OnScrollContainerSizeChanged(object sender, EventArgs e)
        {
            HorizontalScroll.TotalControlWidth = ScrollContainer.Width;
        }

        private void OnScrollPositionChanged(object sender, int tOffset)
        {
            Offset = tOffset;
            ScrollContainer.Left = -Offset;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (!HorizontalScroll.Visible)
                return;

            if (Offset == 0)
                if (iTempScrollWheelOffset > 0)
                    iTempScrollWheelOffset = 0;

            if (e.Delta < 0)
            {
                if (Offset < ScrollContainer.Width - this.Width)
                {
                    if (Offset + HorizontalScroll.ScrollWheelOffset + this.Width > ScrollContainer.Width)//last
                    {
                        iTempScrollWheelOffset = ScrollContainer.Width - this.Width - Offset;
                        Offset += iTempScrollWheelOffset;
                    }
                    else
                    {
                        Offset += HorizontalScroll.ScrollWheelOffset;
                    }

                    ScrollContainer.Left = -Offset;
                    HorizontalScroll.Offset = Offset;
                    HorizontalScroll.Refresh();
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
                        Offset -= HorizontalScroll.ScrollWheelOffset;

                    if (Offset < 0)
                        Offset = 0;

                    ScrollContainer.Left = -Offset;
                    HorizontalScroll.Offset = Offset;
                    HorizontalScroll.Refresh();
                }
        }

        private void OnItemClick(object sender, int DocumentFileID)
        {
            OnItemClicked(sender, DocumentFileID);
        }

        private void OnVerticalScrollVisibleChanged(object sender, EventArgs e)
        {
            if (HorizontalScroll.Visible)
                ScrollContainer.Width = this.Width - HorizontalScroll.Width;
            else
                ScrollContainer.Width = this.Width;
        }


        public event ItemClickedEventHandler ItemClicked;

        public delegate void ItemClickedEventHandler(object sender, int DocumentFileID);

        public virtual void OnItemClicked(object sender, int DocumentFileID)
        {
            ItemClicked?.Invoke(sender, DocumentFileID);
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            ScrollContainer.Focus();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            SetScrollWidth();

            HorizontalScroll.Width = this.Width;
            HorizontalScroll.Top = this.Height - HorizontalScroll.Height;
        }
    }




    public class InfiniumDocumentsUpdatesControlPanel : Control
    {
        Color cBackColor = Color.FromArgb(249, 249, 249);



        Pen pBorderPen = new Pen(new SolidBrush(Color.FromArgb(210, 210, 210)));

        Rectangle rBorderRect = new Rectangle(0, 0, 0, 0);



        public static int iInitialHeight = 1;



        ToolTip ToolTip;

        string sToolTipText = "";

        public InfiniumDocumentsUpdatesCommentsTextBox CommentsTextBox;
        public InfiniumDocumentsCommentsList CommentsList;
        public InfiniumDocumentsConfirmList ConfirmItem;

        public DataTable CommentsDataTable;
        public DataTable UsersDataTable;
        public DataTable FilesDataTable;
        public DataTable ConfirmsDataTable = null;
        public DataTable ConfirmsRecipientsDataTable = null;

        public bool bShowButtons = false;

        public InfiniumDocumentsUpdatesControlPanel()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            this.BackColor = cBackColor;

            ToolTip = new System.Windows.Forms.ToolTip();
            ToolTip.SetToolTip(this, sToolTipText);

            this.Height = iInitialHeight;
            this.Width = 1;
        }


        public void CreateTextBox()
        {
            this.Height += InfiniumDocumentsUpdatesCommentsTextBox.iInitialContainerHeight - iInitialHeight;

            CommentsTextBox = new InfiniumDocumentsUpdatesCommentsTextBox()
            {
                Parent = this,
                Width = this.Width - 80
            };
            CommentsTextBox.Top = this.Height - CommentsTextBox.Height - 1;
            CommentsTextBox.Left = 40;
            CommentsTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            CommentsTextBox.bShowButtons = bShowButtons;
            CommentsTextBox.Initialize();
            CommentsTextBox.RichTextBoxSizeChanged += OnRichTextBoxSizeChanged;
            CommentsTextBox.CancelButtonClicked += OnCancelButtonClicked;
            CommentsTextBox.CommentsTextBoxOpened += OnCommentsTextBoxOpened;
            CommentsTextBox.SendButtonClicked += OnSendButtonClicked;
            CommentsTextBox.FileLabelClicked += OnFileLabelClick;
            CommentsTextBox.AddConfirmClicked += OnAddConfirmClicked;
            CommentsTextBox.AddUserClicked += OnAddUserClicked;
        }

        public void CreateComments()
        {
            if (ConfirmsDataTable.Rows.Count > 0)
            {
                ConfirmItem = new InfiniumDocumentsConfirmList()
                {
                    Parent = this,
                    Width = this.Width - 80,
                    Top = 20,
                    Left = 40,
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                    ConfirmsDataTable = ConfirmsDataTable,
                    ConfirmsRecipientsDataTable = ConfirmsRecipientsDataTable,
                    UsersDataTable = UsersDataTable
                };
                if (CommentsDataTable.Rows.Count > 0)
                    ConfirmItem.bIsComments = true;
                ConfirmItem.InitializeItems();
                ConfirmItem.ItemConfirmClicked += OnItemConfirmClicked;
                ConfirmItem.ItemCancelClicked += OnItemCancelClicked;
                ConfirmItem.ItemEditClicked += OnItemEditClicked;
                ConfirmItem.EditClicked += OnConfirmEditClicked;
                ConfirmItem.DeleteClicked += OnConfirmDeleteClicked;
                this.Height += ConfirmItem.Height + 20;
            }

            CommentsList = new InfiniumDocumentsCommentsList()
            {
                Parent = this,
                Width = this.Width - 80
            };
            if (ConfirmItem != null)
                CommentsList.Top = ConfirmItem.Height + ConfirmItem.Top;
            else
                CommentsList.Top = 1;

            CommentsList.Left = 40;
            CommentsList.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            CommentsList.ItemsDataTable = CommentsDataTable;
            CommentsList.UsersDataTable = UsersDataTable;
            CommentsList.FilesDataTable = FilesDataTable;
            CommentsList.CommentsContainerSizeChanged += OnCommentsContainerSizeChanged;
            CommentsList.EditCommentClicked += OnEditCommentClick;
            CommentsList.DeleteCommentClicked += OnDeleteCommentClick;
            CommentsList.FileClicked += OnFileClicked;

            CommentsList.InitializeItems();

            if (CommentsDataTable.Rows.Count > 0)
                this.Height += CommentsList.Height + 15;//margin before commentstextbox panel
            else
                this.Height += CommentsList.Height + 1;
        }

        public string ToolTipText
        {
            get { return sToolTipText; }
            set { sToolTipText = value; ToolTip.SetToolTip(this, value); }
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);


            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            rBorderRect = this.ClientRectangle;
            rBorderRect.Width = this.ClientRectangle.Width - 1;
            rBorderRect.Height = this.ClientRectangle.Height - 1;

            e.Graphics.DrawRectangle(pBorderPen, rBorderRect);
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            Parent.Focus();
        }


        private void OnFileLabelClick(object sender, EventArgs e)
        {
            OnFileLabelClicked(this, e);
        }

        public void CloseCommentsTextBox()
        {
            CommentsTextBox.CloseCommentsTextBox();
        }

        private void OnCloseCommentsTextBox(object sender, EventArgs e)
        {

        }

        private void OnCancelButtonClicked(object sender, int iHeight)
        {
            this.Height += iHeight;
            OnControlPanelSizeChanged(this, iHeight);
        }

        private void OnRichTextBoxSizeChanged(object sender, int iHeight)
        {
            this.Height += iHeight;
            OnControlPanelSizeChanged(this, iHeight);
        }

        private void OnCommentsContainerSizeChanged(object sender, int iHeight)
        {
            this.Height += iHeight;
            CommentsTextBox.Top += iHeight;
            OnControlPanelSizeChanged(this, iHeight);
        }

        private void OnEditCommentClick(object sender, int iDocumentCommentID, string Text)
        {
            if (CommentsTextBox.bEdit)
                if (iDocumentCommentID == CommentsTextBox.iDocumentCommentID)
                    return;

            if (CommentsTextBox.bOpened)
            {
                CommentsTextBox.RichTextBox.Text = Text;
                CommentsTextBox.iDocumentCommentID = iDocumentCommentID;
            }
            else
            {
                CommentsTextBox.OpenCommentsTextBox(true);
                CommentsTextBox.RichTextBox.Text = Text;
                CommentsTextBox.iDocumentCommentID = iDocumentCommentID;
            }

            OnEditCommentClicked(this, iDocumentCommentID);
        }

        private void OnDeleteCommentClick(object sender, int iDocumentCommentID, string Text)
        {
            OnDeleteCommentClicked(this, iDocumentCommentID);
        }


        public event SizeChangedEventHandler ControlPanelSizeChanged;
        public event EventHandler CommentsTextBoxOpened;
        public event EventHandler FileLabelClicked;
        public event SendButtonEventHandler SendButtonClicked;
        public event CommentEditEventHandler EditCommentClicked;
        public event CommentEditEventHandler DeleteCommentClicked;
        public event FileClickEventHandler FileClicked;
        public event CheckConfirmClickedEventHandler ItemConfirmClicked;
        public event CheckConfirmClickedEventHandler ItemCancelClicked;
        public event CheckConfirmClickedEventHandler ItemEditClicked;
        public event EditConfirmClickedEventHandler ConfirmEditClicked;
        public event EditConfirmClickedEventHandler ConfirmDeleteClicked;
        public event EventHandler AddConfirmClicked;
        public event EventHandler AddUserClicked;

        public delegate void SizeChangedEventHandler(object sender, int Height);
        public delegate void SendButtonEventHandler(object sender, string Text, bool bIsEdit);
        public delegate void CommentEditEventHandler(object sender, int DocumentCommentID);
        public delegate void FileClickEventHandler(object sender, int DocumentCommentFileID);
        public delegate void EditConfirmClickedEventHandler(object sender, int DocumentConfirmationID);
        public delegate void CheckConfirmClickedEventHandler(object sender, int DocumentConfirmationRecipientID);

        public virtual void OnControlPanelSizeChanged(object sender, int Height)
        {
            ControlPanelSizeChanged?.Invoke(sender, Height);
        }

        public virtual void OnCommentsTextBoxOpened(object sender, EventArgs e)
        {
            CommentsTextBoxOpened?.Invoke(this, e);
        }

        public virtual void OnFileLabelClicked(object sender, EventArgs e)
        {
            FileLabelClicked?.Invoke(this, e);
        }

        public virtual void OnSendButtonClicked(object sender, string Text, bool bEdit)
        {
            SendButtonClicked?.Invoke(this, Text, bEdit);//Raise the event
        }

        public virtual void OnEditCommentClicked(object sender, int iDocumentCommentID)
        {
            EditCommentClicked?.Invoke(this, iDocumentCommentID);//Raise the event
        }

        public virtual void OnDeleteCommentClicked(object sender, int iDocumentCommentID)
        {
            DeleteCommentClicked?.Invoke(this, iDocumentCommentID);//Raise the event
        }

        public virtual void OnFileClicked(object sender, int DocumentCommentFileID)
        {
            FileClicked?.Invoke(sender, DocumentCommentFileID);
        }

        public virtual void OnAddConfirmClicked(object sender, EventArgs e)
        {
            AddConfirmClicked?.Invoke(this, e);//Raise the event
        }

        public virtual void OnAddUserClicked(object sender, EventArgs e)
        {
            AddUserClicked?.Invoke(this, e);//Raise the event
        }


        public virtual void OnItemConfirmClicked(object sender, int DocumentConfirmationRecipientID)
        {
            ItemConfirmClicked?.Invoke(sender, DocumentConfirmationRecipientID);
        }

        public virtual void OnItemCancelClicked(object sender, int DocumentConfirmationRecipientID)
        {
            ItemCancelClicked?.Invoke(sender, DocumentConfirmationRecipientID);
        }

        public virtual void OnItemEditClicked(object sender, int DocumentConfirmationRecipientID)
        {
            ItemEditClicked?.Invoke(sender, DocumentConfirmationRecipientID);
        }

        public virtual void OnConfirmEditClicked(object sender, int DocumentConfirmationID)
        {
            ConfirmEditClicked?.Invoke(sender, DocumentConfirmationID);
        }

        public virtual void OnConfirmDeleteClicked(object sender, int DocumentConfirmationID)
        {
            ConfirmDeleteClicked?.Invoke(sender, DocumentConfirmationID);
        }

    }



    public class InfiniumDocumentsCommentsItem : Control
    {
        int iMarginTextRows = 5;

        Color cBackColor = Color.White;
        //Color cBorderColor = Color.FromArgb(192, 192, 192);

        Font fUserNameFont = new Font("Segoe UI Semibold", 16.0f, FontStyle.Regular, GraphicsUnit.Pixel);
        Font fDateFont = new Font("Segoe UI", 12.0f, FontStyle.Regular, GraphicsUnit.Pixel);
        Font fTextFont = new Font("Segoe UI", 16.0f, FontStyle.Regular, GraphicsUnit.Pixel);

        SolidBrush brUserNameBrush = new SolidBrush(Color.FromArgb(60, 60, 60));
        SolidBrush brDateBrush = new SolidBrush(Color.FromArgb(155, 155, 155));
        SolidBrush brTextBrush = new SolidBrush(Color.FromArgb(60, 60, 60));

        Pen pBorderPen = new Pen(new SolidBrush(Color.FromArgb(200, 200, 200)));

        Rectangle rBorderRect = new Rectangle(0, 0, 0, 0);

        int iImageWidth = 58;
        int iImageHeight = 54;

        int iEditLeft = 0;
        int iDeleteLeft = 0;

        Bitmap DeleteBMP = Properties.Resources.DocDelete;
        Bitmap EditBMP = Properties.Resources.DocEdit;



        ToolTip ToolTip;

        string sToolTipText = "";
        public int iDocumentCommentID = -1;
        public Bitmap Image = null;

        public bool bCanEdit = false;

        public string sUserName = "";
        public string sDate = "";
        public string sText = "";

        public int UserID = -1;
        public int iItemIndex = -1;

        public DataTable FilesDataTable = null;
        public InfiniumDocumentsUpdatesFilesList FileList = null;

        public InfiniumDocumentsCommentsItem()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            this.BackColor = Color.Transparent;

            ToolTip = new System.Windows.Forms.ToolTip();
            ToolTip.SetToolTip(this, sToolTipText);

            this.Height = 1;
            this.Width = 1;
        }


        private int GetInitialHeight()
        {
            using (Graphics G = this.CreateGraphics())
            {
                if (sText.Length == 0)
                    return iImageHeight + 5;

                int iSenderHeight = Convert.ToInt32(G.MeasureString("String", fUserNameFont).Height);

                int TextMaxWidth = this.Width - 5 - iImageWidth;

                int CurrentY = 0;

                string CurrentRowString = "";

                for (int i = 0; i < sText.Length; i++)
                {
                    if (sText[i] == '\n')
                    {
                        CurrentRowString = "";
                        CurrentY++;
                        continue;
                    }

                    if (i == sText.Length - 1)
                    {
                        CurrentY++;
                    }
                    else
                    {
                        if (G.MeasureString(CurrentRowString, fTextFont).Width > TextMaxWidth)
                        {
                            int LastSpace = GetLastSpace(CurrentRowString);

                            if (LastSpace == 0)
                            {

                            }
                            else
                            {
                                i -= (CurrentRowString.Length - LastSpace);
                            }


                            CurrentRowString = "";
                            CurrentY++;

                            continue;
                        }
                    }

                    CurrentRowString += sText[i];
                }

                int C = CurrentY * Convert.ToInt32((G.MeasureString("String", fTextFont).Height) + iMarginTextRows) + iSenderHeight;

                if (C < iImageHeight + 5)
                    return iImageHeight + 5;

                return C;
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

        public string ToolTipText
        {
            get { return sToolTipText; }
            set { sToolTipText = value; ToolTip.SetToolTip(this, value); }
        }

        public string CommentText
        {
            get { return sText; }
            set
            {
                sText = value;
                this.Height = GetInitialHeight();

                if (FilesDataTable.Rows.Count > 0)
                {
                    FileList = new InfiniumDocumentsUpdatesFilesList()
                    {
                        Width = this.Width - iImageWidth,
                        Left = iImageWidth,
                        Parent = this,
                        Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                        ItemsDataTable = FilesDataTable
                    };
                    FileList.Top = this.Height - FileList.Height - 2;
                    FileList.Anchor = AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;
                    FileList.InitializeItems();
                    FileList.ItemClicked += OnFileClicked;

                    this.Height += FileList.Height + 2;
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            //e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            //e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

            //rBorderRect = this.ClientRectangle;
            //rBorderRect.Width = this.ClientRectangle.Width - 1;
            //rBorderRect.Height = this.ClientRectangle.Height - 1;

            //e.Graphics.DrawRectangle(pBorderPen, rBorderRect);

            Image.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);

            e.Graphics.DrawImage(Image, 0, 0, iImageWidth, iImageHeight);

            e.Graphics.DrawString(sUserName, fUserNameFont, brUserNameBrush, 5 + iImageWidth, -3);

            e.Graphics.DrawString(sDate, fDateFont, brDateBrush, 5 + iImageWidth + e.Graphics.MeasureString(sUserName, fUserNameFont).Width, 1);

            if (bCanEdit)
            {
                iEditLeft = iImageWidth + 7 + Convert.ToInt32(e.Graphics.MeasureString(sUserName, fUserNameFont).Width) +
                            Convert.ToInt32(e.Graphics.MeasureString(sDate, fDateFont).Width) + 15;

                iDeleteLeft = iEditLeft + 20 + 10;

                EditBMP.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);
                DeleteBMP.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);

                e.Graphics.DrawImage(EditBMP, iEditLeft, -1, 20, 20);
                e.Graphics.DrawImage(DeleteBMP, iDeleteLeft, -1, 20, 20);

            }

            DrawText(brTextBrush, e.Graphics, Convert.ToInt32(e.Graphics.MeasureString(sUserName, fUserNameFont).Height), 5 + iImageWidth);
        }

        private void DrawText(SolidBrush Brush, Graphics G, int PosY, int PosX)
        {
            if (sText.Length == 0)
                return;

            int TextMaxWidth = this.Width - 5 - iImageWidth;

            int CurrentY = 0;

            string CurrentRowString = "";

            for (int i = 0; i < sText.Length; i++)
            {
                if (sText[i] == '\n')
                {
                    G.DrawString(CurrentRowString, fTextFont, Brush,
                                 PosX, PosY +
                                 (CurrentY * (G.MeasureString("String", fTextFont).Height + iMarginTextRows)) - 1);
                    CurrentRowString = "";
                    CurrentY++;
                    continue;
                }

                if (i == sText.Length - 1)
                {
                    G.DrawString(CurrentRowString += sText[i], fTextFont, Brush,
                                 PosX, PosY +
                                 (CurrentY * (G.MeasureString("String", fTextFont).Height + iMarginTextRows)) - 1);
                    CurrentY++;
                }
                else
                {
                    if (G.MeasureString(CurrentRowString, fTextFont).Width > TextMaxWidth)
                    {
                        int LastSpace = GetLastSpace(CurrentRowString);

                        if (LastSpace == 0)
                        {
                            G.DrawString(CurrentRowString, fTextFont, Brush,
                                         PosX, PosY +
                                        (CurrentY * (G.MeasureString("String", fTextFont).Height + iMarginTextRows)) - 1);
                        }
                        else
                        {
                            G.DrawString(CurrentRowString.Substring(0, LastSpace + 1), fTextFont, Brush,
                                         PosX, PosY +
                                         (CurrentY * (G.MeasureString("String", fTextFont).Height + iMarginTextRows)) - 1);

                            i -= (CurrentRowString.Length - LastSpace);
                        }


                        CurrentRowString = "";
                        CurrentY++;

                        continue;
                    }
                }

                CurrentRowString += sText[i];
            }
        }


        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (bCanEdit)
            {
                if (e.Y >= -1 && e.Y <= 19 && e.X >= iEditLeft && e.X <= iEditLeft + 20)
                    OnEditClicked(this);

                if (e.Y >= -1 && e.Y <= 19 && e.X >= iDeleteLeft && e.X <= iDeleteLeft + 20)
                    OnDeleteClicked(this);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (bCanEdit)
            {
                if (e.Y >= -1 && e.Y <= 19 && e.X >= iEditLeft && e.X <= iEditLeft + 20)
                {
                    if (this.Cursor != Cursors.Hand)
                        this.Cursor = Cursors.Hand;

                    if (ToolTipText != "Редактировать")
                        ToolTipText = "Редактировать";
                }
                else if (e.Y >= -1 && e.Y <= 19 && e.X >= iDeleteLeft && e.X <= iDeleteLeft + 20)
                {
                    if (this.Cursor != Cursors.Hand)
                        this.Cursor = Cursors.Hand;

                    if (ToolTipText != "Удалить")
                        ToolTipText = "Удалить";
                }
                else
                {
                    this.Cursor = Cursors.Default;
                    ToolTipText = "";
                }
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (this.Cursor != Cursors.Default)
            {
                this.Cursor = Cursors.Default;
                ToolTipText = "";
            }
        }


        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            Parent.Focus();
        }


        public event EditClickedEventHandler EditClicked;
        public event EditClickedEventHandler DeleteClicked;
        public event FileClickEventHandler FileClicked;

        public delegate void EditClickedEventHandler(object sender, int DocumentCommentID, string Text);
        public delegate void FileClickEventHandler(object sender, int DocumentCommentFileID);

        public virtual void OnEditClicked(object sender)
        {
            EditClicked?.Invoke(sender, iDocumentCommentID, sText);
        }

        public virtual void OnDeleteClicked(object sender)
        {
            DeleteClicked?.Invoke(sender, iDocumentCommentID, sText);
        }

        public virtual void OnFileClicked(object sender, int DocumentCommentFileID)
        {
            FileClicked?.Invoke(sender, DocumentCommentFileID);
        }

    }


    public class InfiniumDocumentsCommentsList : Control
    {
        public int Offset = 0;

        int iTempScrollWheelOffset = 0;
        int iMarginToNextItem = 20;
        int iMarginForLabel = 55;
        int iMarginForComments = 20;

        //int iMaxHeight = 500;
        int iMaxCount = 1;

        int iAllCommentsHeight = 0;

        public bool bExpanded = false;

        public static int iInitialHeight = 54;

        InfiniumDocumentsUpdatesScrollContainer ScrollContainer;
        public InfiniumProjectsVerticalScrollBar VerticalScroll;

        public InfiniumDocumentsCommentsItem[] Items;
        public Label AllCommentsLabel;

        public DataTable ItemsDataTable;
        public DataTable UsersDataTable = null;
        public DataTable FilesDataTable = null;


        public InfiniumDocumentsCommentsList()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            VerticalScroll = new InfiniumProjectsVerticalScrollBar()
            {
                Parent = this,
                Height = this.Height
            };
            VerticalScroll.Left = this.Width - VerticalScroll.Width;
            VerticalScroll.TotalControlHeight = this.Height;
            VerticalScroll.Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            VerticalScroll.Width = 12;
            VerticalScroll.ScrollPositionChanged += OnScrollPositionChanged;

            ScrollContainer = new InfiniumDocumentsUpdatesScrollContainer()
            {
                Parent = this,
                Top = iMarginForLabel,
                Left = 0,
                Width = this.Width,
                Height = this.Height - iMarginForLabel,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            ScrollContainer.SizeChanged += OnScrollContainerSizeChanged;
            ScrollContainer.BackColor = Color.Transparent;

            AllCommentsLabel = new Label()
            {
                Parent = this,
                Top = 15,
                Left = 0,
                Font = new Font("Segoe UI", 15.0f, FontStyle.Regular, GraphicsUnit.Pixel),
                ForeColor = Color.FromArgb(56, 184, 238),
                AutoSize = true,
                Cursor = Cursors.Hand
            };
            AllCommentsLabel.Click += OnLabelClick;

            this.Height = iInitialHeight;
        }

        public void InitializeItems()
        {
            if (Items != null)
            {
                for (int i = 0; i < Items.Count(); i++)
                {
                    if (Items[i] != null)
                    {
                        Items[i].Dispose();
                        Items[i] = null;
                    }
                }

                Items = new InfiniumDocumentsCommentsItem[0];
            }

            int iCurPosY = 0;
            Offset = 0;
            ScrollContainer.Top = -Offset + iMarginForLabel;
            VerticalScroll.Offset = Offset;
            VerticalScroll.Refresh();

            if (ItemsDataTable == null)
                return;

            if (ItemsDataTable.DefaultView.Count > 0)
            {
                Items = new InfiniumDocumentsCommentsItem[ItemsDataTable.DefaultView.Count];

                for (int i = 0; i < ItemsDataTable.DefaultView.Count; i++)
                {
                    Items[i] = new InfiniumDocumentsCommentsItem()
                    {
                        Parent = ScrollContainer,
                        Left = 0,
                        Width = ScrollContainer.Width,

                        Top = iCurPosY,
                        sUserName = UsersDataTable.Select("UserID = " + ItemsDataTable.DefaultView[i]["UserID"])[0]["Name"].ToString(),
                        UserID = Convert.ToInt32(ItemsDataTable.DefaultView[i]["UserID"]),
                        iDocumentCommentID = Convert.ToInt32(ItemsDataTable.DefaultView[i]["DocumentCommentID"]),
                        //comments files
                        FilesDataTable = new DataTable()
                    };
                    Items[i].FilesDataTable = FilesDataTable.Clone();

                    foreach (DataRow rRow in FilesDataTable.Select("DocumentCommentID = " + ItemsDataTable.DefaultView[i]["DocumentCommentID"]))
                    {
                        Items[i].FilesDataTable.Rows.Add(rRow.ItemArray);
                    }
                    Items[i].sDate = Convert.ToDateTime(ItemsDataTable.DefaultView[i]["DateTime"]).ToString("dd.MM.yyyy HH:mm");
                    Items[i].CommentText = ItemsDataTable.DefaultView[i]["Text"].ToString();//initialize here

                    if (Convert.ToInt32(ItemsDataTable.DefaultView[i]["UserID"]) == Security.CurrentUserID)
                        Items[i].bCanEdit = true;

                    Items[i].iItemIndex = i;


                    if (UsersDataTable.Select("UserID = " + Items[i].UserID)[0]["Photo"] != DBNull.Value)
                    {
                        byte[] b = (byte[])UsersDataTable.Select("UserID = " + Items[i].UserID)[0]["Photo"];

                        using (MemoryStream ms = new MemoryStream(b))
                        {
                            Items[i].Image = (Bitmap)Image.FromStream(ms);
                        }
                    }
                    else
                        Items[i].Image = Properties.Resources.NoImage;

                    Items[i].Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                    Items[i].EditClicked += OnEditCommentClicked;
                    Items[i].DeleteClicked += OnDeleteCommentClicked;
                    Items[i].FileClicked += OnFileClicked;

                    iCurPosY += Items[i].Height;

                    iAllCommentsHeight = iCurPosY;

                    iCurPosY += iMarginToNextItem;
                }
            }

            if (Items == null)
            {
                this.Height = 0;
                ScrollContainer.Height = 0;
                VerticalScroll.TotalControlHeight = 0;
                return;
            }

            if (Items.Count() == 0)
            {
                this.Height = 0;
                ScrollContainer.Height = 0;
                VerticalScroll.TotalControlHeight = 0;
                return;
            }

            iCurPosY = 0;

            for (int c = 0; c < iMaxCount; c++)
            {
                iCurPosY += Items[c].Height;
            }

            if (Items.Count() == iMaxCount)
            {
                AllCommentsLabel.Visible = false;
                this.Height = iCurPosY + iMarginForComments;
                ScrollContainer.Top = iMarginForComments;
            }
            else
            {
                AllCommentsLabel.Visible = true;

                AllCommentsLabel.Text = GetLabelText(Items.Count());
                this.Height = iCurPosY + iMarginForLabel;
                iAllCommentsHeight += iMarginForLabel;
                ScrollContainer.Top = iMarginForLabel;
            }

            ScrollContainer.Height = iCurPosY;
            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public InfiniumProjectsVerticalScrollBar VerticalScrollBar
        {
            get { return VerticalScroll; }
            set { VerticalScroll = value; VerticalScroll.Left = this.Width - VerticalScroll.Width; }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }

        private string GetLabelText(int i)
        {
            char[] s = new char[2];

            s[0] = i.ToString()[i.ToString().Length - 1];

            if (i > 9)
                s[1] = i.ToString()[i.ToString().Length - 2];
            else
                s[1] = '-';

            if (s[0] == '1' && s[1] != '1')
                return i.ToString() + " комментарий";
            else
                if ((s[0] == '2' && s[1] != '1') || (s[0] == '3' && s[1] != '1') || (s[0] == '4' && s[1] != '1'))
                return i.ToString() + " комментария";
            else
                return i.ToString() + " комментариев";
        }

        private void OnLabelClick(object sender, EventArgs e)
        {
            if (!bExpanded)
            {
                bExpanded = true;
                AllCommentsLabel.Text = "Скрыть комментарии";

                int size = this.Height;

                this.Height = iAllCommentsHeight;
                ScrollContainer.Height = this.Height;
                VerticalScroll.TotalControlHeight = ScrollContainer.Height;

                OnCommentsContainerSizeChanged(this, iAllCommentsHeight - size);
            }
            else
            {
                bExpanded = false;
                AllCommentsLabel.Text = GetLabelText(Items.Count());
                int size = this.Height;
                int iCurPosY = 0;

                for (int c = 0; c < iMaxCount; c++)
                {
                    iCurPosY += Items[c].Height;
                }

                iCurPosY += iMarginForLabel;

                this.Height = iCurPosY;
                ScrollContainer.Height = this.Height;
                VerticalScroll.TotalControlHeight = ScrollContainer.Height;

                OnCommentsContainerSizeChanged(this, this.Height - size);
            }
        }

        private void OnScrollContainerSizeChanged(object sender, EventArgs e)
        {
            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
        }

        private void OnScrollPositionChanged(object sender, int tOffset)
        {
            Offset = tOffset;
            ScrollContainer.Top = -Offset;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (!VerticalScroll.Visible)
                return;

            if (Offset == 0)
                if (iTempScrollWheelOffset > 0)
                    iTempScrollWheelOffset = 0;

            if (e.Delta < 0)
            {
                if (Offset < ScrollContainer.Height - this.Height)
                {
                    if (Offset + VerticalScroll.ScrollWheelOffset + this.Height > ScrollContainer.Height)//last
                    {
                        iTempScrollWheelOffset = ScrollContainer.Height - this.Height - Offset;
                        Offset += iTempScrollWheelOffset;
                    }
                    else
                    {
                        Offset += VerticalScroll.ScrollWheelOffset;
                    }

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
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
                        Offset -= VerticalScroll.ScrollWheelOffset;

                    if (Offset < 0)
                        Offset = 0;

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
                }
        }

        private void OnVerticalScrollVisibleChanged(object sender, EventArgs e)
        {
            if (VerticalScroll.Visible)
                ScrollContainer.Width = this.Width - VerticalScroll.Width;
            else
                ScrollContainer.Width = this.Width;
        }


        public event SizeChangedEventHandler CommentsContainerSizeChanged;
        public event CommentEditEventHandler EditCommentClicked;
        public event CommentEditEventHandler DeleteCommentClicked;
        public event FileClickEventHandler FileClicked;

        public delegate void SizeChangedEventHandler(object sender, int Height);
        public delegate void CommentEditEventHandler(object sender, int DocumentCommentID, string Text);
        public delegate void FileClickEventHandler(object sender, int DocumentCommentFileID);

        public virtual void OnCommentsContainerSizeChanged(object sender, int Height)
        {
            CommentsContainerSizeChanged?.Invoke(sender, Height);
        }

        public virtual void OnEditCommentClicked(object sender, int iDocumentCommentID, string Text)
        {
            EditCommentClicked?.Invoke(this, iDocumentCommentID, ((InfiniumDocumentsCommentsItem)sender).sText);
        }

        public virtual void OnDeleteCommentClicked(object sender, int iDocumentCommentID, string Text)
        {
            DeleteCommentClicked?.Invoke(this, iDocumentCommentID, "");
        }

        public virtual void OnFileClicked(object sender, int DocumentCommentFileID)
        {
            FileClicked?.Invoke(sender, DocumentCommentFileID);
        }
    }





    public class InfiniumDocumentsConfirmItem : Control
    {
        Color cBackColor = Color.White;

        Font fUserNameFont = new Font("Segoe UI", 14.0f, FontStyle.Regular, GraphicsUnit.Pixel);
        Font fDateFont = new Font("Segoe UI", 11.0f, FontStyle.Regular, GraphicsUnit.Pixel);
        Font fTextFont = new Font("Segoe UI", 13.0f, FontStyle.Regular, GraphicsUnit.Pixel);

        SolidBrush brUserNameBrush = new SolidBrush(Color.FromArgb(60, 60, 60));
        SolidBrush brDateBrush = new SolidBrush(Color.FromArgb(155, 155, 155));
        SolidBrush brTextBrush = new SolidBrush(Color.FromArgb(60, 60, 60));
        SolidBrush brConfirmBrush = new SolidBrush(Color.FromArgb(56, 184, 238));
        SolidBrush brCancelBrush = new SolidBrush(Color.FromArgb(255, 0, 0));
        SolidBrush brEditBrush = new SolidBrush(Color.FromArgb(169, 169, 169));

        Pen pBorderPen = new Pen(new SolidBrush(Color.FromArgb(200, 200, 200)));

        Rectangle rBorderRect = new Rectangle(0, 0, 0, 0);

        int iConfirmLeft = 0;
        int iConfirmWidth = 0;
        int iCancelLeft = 0;
        int iCancelWidth = 0;

        Bitmap CheckOKBMP = Properties.Resources.CheckDocOK;
        Bitmap CanceledBMP = Properties.Resources.CheckCanceled;

        public int iDocumentConfirmationRecipientID = -1;
        public Bitmap Image = null;

        public bool bCanEdit = false;

        public int iStatus = -1;

        public string sUserName = "";
        public string sDate = "";

        public int UserID = -1;
        public int iItemIndex = -1;

        public InfiniumDocumentsConfirmItem()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            this.BackColor = Color.Transparent;

            this.Height = 20;
            this.Width = 1;
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;



            if (iStatus == 1)
            {
                CheckOKBMP.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);
                e.Graphics.DrawImage(CheckOKBMP, 0, 0);
            }

            if (iStatus == 2)
            {
                CanceledBMP.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);
                e.Graphics.DrawImage(CanceledBMP, 0, 0);
            }

            iConfirmLeft = -1;
            iCancelLeft = -1;
            iConfirmWidth = -1;
            iCancelWidth = -1;

            e.Graphics.DrawString(sUserName, fUserNameFont, brUserNameBrush, CheckOKBMP.Width + 3, (this.Height - e.Graphics.MeasureString(sUserName, fUserNameFont).Height) / 2);

            e.Graphics.DrawString(sDate, fDateFont, brDateBrush, e.Graphics.MeasureString(sUserName, fUserNameFont).Width + CanceledBMP.Width, 3);

            if (bCanEdit)
            {
                if (iStatus == 0)
                {
                    iConfirmLeft = CheckOKBMP.Width + 3 + (int)e.Graphics.MeasureString(sUserName, fUserNameFont).Width + (int)e.Graphics.MeasureString(sDate, fDateFont).Width;
                    e.Graphics.DrawString("подтвердить", fTextFont, brConfirmBrush, iConfirmLeft, (this.Height - e.Graphics.MeasureString(sUserName, fUserNameFont).Height) / 2 + 1);
                    iConfirmWidth = (int)e.Graphics.MeasureString("подтвердить", fTextFont).Width;

                    iCancelLeft = CanceledBMP.Width + 3 + (int)e.Graphics.MeasureString(sUserName, fUserNameFont).Width + (int)e.Graphics.MeasureString(sDate, fDateFont).Width + iConfirmWidth + 2;
                    e.Graphics.DrawString("отклонить", fTextFont, brCancelBrush, iCancelLeft, (this.Height - e.Graphics.MeasureString(sUserName, fUserNameFont).Height) / 2 + 1);
                    iCancelWidth = (int)e.Graphics.MeasureString("отклонить", fTextFont).Width;
                }
                else
                {
                    iConfirmLeft = CheckOKBMP.Width + 3 + (int)e.Graphics.MeasureString(sUserName, fUserNameFont).Width + (int)e.Graphics.MeasureString(sDate, fDateFont).Width;
                    e.Graphics.DrawString("отменить", fTextFont, brConfirmBrush, iConfirmLeft, (this.Height - e.Graphics.MeasureString(sUserName, fUserNameFont).Height) / 2 + 1);
                    iConfirmWidth = (int)e.Graphics.MeasureString("отменить", fTextFont).Width;
                }
            }

            //e.Graphics.DrawString("запросил(а) подтверждение", fTextFont, brTextBrush, 5 + iImageWidth, 23);



            //if (bCanEdit)
            //{
            //    iEditLeft = iImageWidth + 7 + Convert.ToInt32(e.Graphics.MeasureString(sUserName, fUserNameFont).Width) +
            //                Convert.ToInt32(e.Graphics.MeasureString(sDate, fDateFont).Width) + 15;

            //    iDeleteLeft = iEditLeft + 20 + 10;

            //    EditBMP.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);
            //    DeleteBMP.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);

            //    e.Graphics.DrawImage(EditBMP, iEditLeft, -1, 20, 20);
            //    e.Graphics.DrawImage(DeleteBMP, iDeleteLeft, -1, 20, 20);

            //}
        }


        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (bCanEdit)
            {
                if (e.Y >= 3 && e.Y <= this.Height - 3 && e.X >= iConfirmLeft && e.X <= iConfirmLeft + iConfirmWidth)
                {
                    if (iStatus == 0)
                    {
                        OnConfirmClicked(this);
                        iStatus = 1;
                        this.Refresh();
                    }
                    else
                    {
                        OnEditClicked(this);
                        iStatus = 0;
                        this.Refresh();
                    }
                }
                else if (e.Y >= 3 && e.Y <= this.Height - 3 && e.X >= iCancelLeft && e.X <= iCancelLeft + iCancelWidth)
                {
                    OnCancelClicked(this);
                    iStatus = 2;
                    this.Refresh();
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (bCanEdit)
            {
                if (e.Y >= 3 && e.Y <= this.Height - 3 && e.X >= iConfirmLeft && e.X <= iConfirmLeft + iConfirmWidth)
                {
                    if (this.Cursor != Cursors.Hand)
                        this.Cursor = Cursors.Hand;
                }
                else if (e.Y >= 3 && e.Y <= this.Height - 3 && e.X >= iCancelLeft && e.X <= iCancelLeft + iCancelWidth)
                {
                    if (this.Cursor != Cursors.Hand)
                        this.Cursor = Cursors.Hand;
                }
                else
                {
                    this.Cursor = Cursors.Default;
                }
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (this.Cursor != Cursors.Default)
            {
                this.Cursor = Cursors.Default;
            }
        }


        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            Parent.Focus();
        }


        public event EditClickedEventHandler ConfirmClicked;
        public event EditClickedEventHandler CancelClicked;
        public event EditClickedEventHandler EditClicked;

        public delegate void EditClickedEventHandler(object sender, int iDocumentConfirmationRecipientID);

        public virtual void OnConfirmClicked(object sender)
        {
            ConfirmClicked?.Invoke(sender, iDocumentConfirmationRecipientID);
        }

        public virtual void OnCancelClicked(object sender)
        {
            CancelClicked?.Invoke(sender, iDocumentConfirmationRecipientID);
        }

        public virtual void OnEditClicked(object sender)
        {
            EditClicked?.Invoke(sender, iDocumentConfirmationRecipientID);
        }
    }


    public class InfiniumDocumentsConfirmList : Control
    {
        int iMarginToNextItem = 10;

        Font fUserNameFont = new Font("Segoe UI Semibold", 16.0f, FontStyle.Regular, GraphicsUnit.Pixel);
        Font fDateFont = new Font("Segoe UI", 12.0f, FontStyle.Regular, GraphicsUnit.Pixel);
        Font fTextFont = new Font("Segoe UI", 16.0f, FontStyle.Regular, GraphicsUnit.Pixel);

        SolidBrush brUserNameBrush = new SolidBrush(Color.FromArgb(60, 60, 60));
        SolidBrush brDateBrush = new SolidBrush(Color.FromArgb(155, 155, 155));
        SolidBrush brTextBrush = new SolidBrush(Color.FromArgb(60, 60, 60));


        Pen pBorderPen = new Pen(new SolidBrush(Color.FromArgb(240, 240, 240)));

        Rectangle rBorderRect = new Rectangle(0, 0, 0, 0);

        int iImageWidth = 58;
        int iImageHeight = 54;

        int iEditLeft = 0;
        int iDeleteLeft = 0;

        Bitmap DeleteBMP = Properties.Resources.DocDelete;
        Bitmap EditBMP = Properties.Resources.DocEdit;

        ToolTip ToolTip;

        string sToolTipText = "";
        public Bitmap Image = null;

        public bool bIsComments = false;
        public bool bCanEdit = false;

        public string sUserName = "";
        public string sDate = "";
        public string sText = "";

        public int UserID = -1;
        public int iDocumentConfirmationID = -1;

        public DataTable ConfirmsDataTable = null;
        public DataTable ConfirmsRecipientsDataTable = null;
        public DataTable UsersDataTable = null;

        InfiniumDocumentsConfirmItem[] Items;

        public InfiniumDocumentsConfirmList()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            this.BackColor = Color.Transparent;

            ToolTip = new System.Windows.Forms.ToolTip();
            ToolTip.SetToolTip(this, sToolTipText);

            this.Height = iImageHeight + 15;
            this.Width = 1;
        }

        public string ToolTipText
        {
            get { return sToolTipText; }
            set { sToolTipText = value; ToolTip.SetToolTip(this, value); }
        }


        public void InitializeItems()
        {
            if (UsersDataTable.Select("UserID = " + ConfirmsDataTable.Rows[0]["UserID"])[0]["Photo"] != DBNull.Value)
            {
                byte[] b = (byte[])UsersDataTable.Select("UserID = " + ConfirmsDataTable.Rows[0]["UserID"])[0]["Photo"];

                using (MemoryStream ms = new MemoryStream(b))
                {
                    Image = (Bitmap)System.Drawing.Image.FromStream(ms);
                }
            }
            else
                Image = Properties.Resources.NoImage;

            sUserName = UsersDataTable.Select("UserID = " + ConfirmsDataTable.Rows[0]["UserID"])[0]["Name"].ToString();

            sDate = Convert.ToDateTime(ConfirmsDataTable.Rows[0]["DateTime"]).ToString("dd.MM.yyyy HH:mm");

            if (Convert.ToInt32(ConfirmsDataTable.Rows[0]["UserID"]) == Security.CurrentUserID)
                bCanEdit = true;

            iDocumentConfirmationID = Convert.ToInt32(ConfirmsDataTable.Rows[0]["DocumentConfirmationID"]);

            if (ConfirmsRecipientsDataTable.Rows.Count == 0)
                return;

            Items = new InfiniumDocumentsConfirmItem[ConfirmsRecipientsDataTable.Rows.Count];

            for (int i = 0; i < ConfirmsRecipientsDataTable.Rows.Count; i++)
            {
                Items[i] = new InfiniumDocumentsConfirmItem()
                {
                    Parent = this,
                    Left = iImageWidth + 7
                };
                Items[i].Top = i * (Items[i].Height + iMarginToNextItem) + iImageHeight + 10;
                Items[i].Width = this.Width;
                Items[i].Anchor = AnchorStyles.Top | AnchorStyles.Left;

                Items[i].sUserName = UsersDataTable.Select("UserID = " + ConfirmsRecipientsDataTable.Rows[i]["UserID"])[0]["Name"].ToString();
                Items[i].iStatus = Convert.ToInt32(ConfirmsRecipientsDataTable.Rows[i]["Status"]);
                if (ConfirmsRecipientsDataTable.Rows[i]["DateTime"] != DBNull.Value)
                    Items[i].sDate = Convert.ToDateTime(ConfirmsRecipientsDataTable.Rows[i]["DateTime"]).ToString("dd.MM.yyyy HH:mm");
                Items[i].UserID = Convert.ToInt32(ConfirmsRecipientsDataTable.Rows[i]["UserID"]);
                Items[i].iDocumentConfirmationRecipientID = Convert.ToInt32(ConfirmsRecipientsDataTable.Rows[i]["DocumentConfirmationRecipientID"]);
                Items[i].bCanEdit = Convert.ToInt32(ConfirmsRecipientsDataTable.Rows[i]["UserID"]) == Security.CurrentUserID;

                Items[i].ConfirmClicked += OnItemConfirmClicked;
                Items[i].CancelClicked += OnItemCancelClicked;
                Items[i].EditClicked += OnItemEditClicked;
            }

            this.Height += Items.Count() * (Items[0].Height + iMarginToNextItem);


            this.Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (ConfirmsDataTable == null)
                return;

            if (ConfirmsDataTable.Rows.Count == 0)
                return;

            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;


            Image.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);

            e.Graphics.DrawImage(Image, 0, 0, iImageWidth, iImageHeight);

            e.Graphics.DrawString(sUserName, fUserNameFont, brUserNameBrush, 5 + iImageWidth, -3);

            e.Graphics.DrawString(sDate, fDateFont, brDateBrush, 5 + iImageWidth + e.Graphics.MeasureString(sUserName, fUserNameFont).Width, 1);

            e.Graphics.DrawString("запросил(а) подтверждение", fTextFont, brTextBrush, 5 + iImageWidth, 23);



            if (bCanEdit)
            {
                iEditLeft = iImageWidth + 7 + Convert.ToInt32(e.Graphics.MeasureString(sUserName, fUserNameFont).Width) +
                            Convert.ToInt32(e.Graphics.MeasureString(sDate, fDateFont).Width) + 15;

                iDeleteLeft = iEditLeft + 20 + 10;

                EditBMP.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);
                DeleteBMP.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);

                e.Graphics.DrawImage(EditBMP, iEditLeft, -1, 20, 20);
                e.Graphics.DrawImage(DeleteBMP, iDeleteLeft, -1, 20, 20);
            }

            if (bIsComments)
                e.Graphics.DrawLine(pBorderPen, 0, this.Height - 1, this.Width, this.Height - 1);
        }


        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (bCanEdit)
            {
                if (e.Y >= -1 && e.Y <= 19 && e.X >= iEditLeft && e.X <= iEditLeft + 20)
                    OnEditClicked(this, iDocumentConfirmationID);

                if (e.Y >= -1 && e.Y <= 19 && e.X >= iDeleteLeft && e.X <= iDeleteLeft + 20)
                    OnDeleteClicked(this, iDocumentConfirmationID);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (bCanEdit)
            {
                if (e.Y >= -1 && e.Y <= 19 && e.X >= iEditLeft && e.X <= iEditLeft + 20)
                {
                    this.Cursor = Cursors.Hand;

                    if (ToolTipText != "Редактировать")
                        ToolTipText = "Редактировать";
                }
                else if (e.Y >= -1 && e.Y <= 19 && e.X >= iDeleteLeft && e.X <= iDeleteLeft + 20)
                {
                    this.Cursor = Cursors.Hand;

                    if (ToolTipText != "Удалить")
                        ToolTipText = "Удалить";
                }
                else
                {
                    this.Cursor = Cursors.Default;
                    ToolTipText = "";
                }
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (this.Cursor != Cursors.Default)
            {
                this.Cursor = Cursors.Default;
                ToolTipText = "";
            }
        }


        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            Parent.Focus();
        }


        public event CheckClickedEventHandler ItemConfirmClicked;
        public event CheckClickedEventHandler ItemCancelClicked;
        public event CheckClickedEventHandler ItemEditClicked;
        public event EditClickedEventHandler EditClicked;
        public event EditClickedEventHandler DeleteClicked;

        public delegate void EditClickedEventHandler(object sender, int DocumentConfirmationID);
        public delegate void CheckClickedEventHandler(object sender, int DocumentConfirmationRecipientID);

        public virtual void OnItemConfirmClicked(object sender, int DocumentConfirmationRecipientID)
        {
            ItemConfirmClicked?.Invoke(sender, DocumentConfirmationRecipientID);
        }

        public virtual void OnItemCancelClicked(object sender, int DocumentConfirmationRecipientID)
        {
            ItemCancelClicked?.Invoke(sender, DocumentConfirmationRecipientID);
        }

        public virtual void OnItemEditClicked(object sender, int DocumentConfirmationRecipientID)
        {
            ItemEditClicked?.Invoke(sender, DocumentConfirmationRecipientID);
        }

        public virtual void OnEditClicked(object sender, int DocumentConfirmationID)
        {
            EditClicked?.Invoke(sender, DocumentConfirmationID);
        }

        public virtual void OnDeleteClicked(object sender, int DocumentConfirmationID)
        {
            DeleteClicked?.Invoke(sender, DocumentConfirmationID);
        }

    }







    public class InfiniumDocumentsUpdatesCommentsTextBox : Control
    {
        public ComponentFactory.Krypton.Toolkit.KryptonRichTextBox RichTextBox;

        ComponentFactory.Krypton.Toolkit.KryptonButton SendButton;
        ComponentFactory.Krypton.Toolkit.KryptonButton CancelButton;

        public Label FilesLabel;

        int iAddConfirmLeft = 0;
        int iAddButtonsTop = 0;

        int iAddUserLeft = 0;

        Font fCommentsTextBoxFont = new Font("Segoe UI", 14.0f, FontStyle.Regular, GraphicsUnit.Pixel);

        Bitmap AddConfirmBMP = Properties.Resources.DocCheckList;
        Bitmap AddUserBMP = Properties.Resources.DocAddUser;

        Rectangle rTextBoxRect = new Rectangle(0, 0, 350, 30);

        SolidBrush brCommentsTextBoxBackBrush = new SolidBrush(Color.White);
        SolidBrush brCommentsTextFontBrush = new SolidBrush(Color.FromArgb(178, 178, 178));

        Pen pBorderPen = new Pen(new SolidBrush(Color.FromArgb(210, 210, 210)));

        public bool bEdit = false;
        public static int iExpandedTextBoxHeight = 100;
        public static int iExpandedContainerHeight = 175;
        public static int iInitialContainerHeight = 70;

        bool bCtrlEnter = false;

        public bool bOpened = false;

        public ProgressBar ProgressBar;


        ToolTip ToolTip;

        string sToolTipText = "";

        public int iDocumentCommentID = -1;

        bool bTextTrack = false;
        bool bConfirmTrack = false;
        bool bUserTrack = false;

        public bool bShowButtons = false;

        public string ToolTipText
        {
            get { return sToolTipText; }
            set { sToolTipText = value; ToolTip.SetToolTip(this, value); }
        }


        public InfiniumDocumentsUpdatesCommentsTextBox()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            this.Height = iInitialContainerHeight;
            this.BackColor = Color.Transparent;


            ToolTip = new System.Windows.Forms.ToolTip();
            ToolTip.SetToolTip(this, sToolTipText);

        }

        public void Initialize()
        {
            RichTextBox = new ComponentFactory.Krypton.Toolkit.KryptonRichTextBox()
            {
                Parent = this,
                Left = 0,
                Height = iExpandedTextBoxHeight,
                Width = this.Width
            };
            RichTextBox.StateCommon.Content.Font = new System.Drawing.Font("Segoe UI", 15.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            RichTextBox.StateCommon.Border.Color1 = pBorderPen.Color;
            RichTextBox.TextChanged += OnRichTextBoxTextChanged;
            RichTextBox.KeyDown += OnRichTextBoxKeyDown;
            RichTextBox.Visible = false;

            SendButton = new ComponentFactory.Krypton.Toolkit.KryptonButton()
            {
                Parent = this,
                Left = RichTextBox.Left + 1,
                Width = 106,
                Height = 32,
                Text = "Отправить"
            };
            SendButton.StateCommon.Back.Color1 = Color.FromArgb(56, 184, 238);
            SendButton.StateCommon.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            SendButton.StateCommon.Content.ShortText.Color1 = Color.White;
            SendButton.StateCommon.Content.ShortText.Font = new Font("Segoe UI", 15.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            SendButton.StateCommon.Border.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.False;
            SendButton.StateCommon.Border.Rounding = 0;
            SendButton.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            SendButton.Click += OnSendButtonClicked;
            SendButton.OverrideDefault.Back.Color1 = Color.FromArgb(56, 184, 238);
            SendButton.OverrideDefault.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            SendButton.StateTracking.Back.Color1 = Color.FromArgb(67, 191, 246);
            SendButton.Visible = false;


            CancelButton = new ComponentFactory.Krypton.Toolkit.KryptonButton()
            {
                Parent = this,
                Text = "Отмена",
                Width = 106,
                Height = 32,
                Left = 1 + SendButton.Width + 6
            };
            CancelButton.StateCommon.Back.Color1 = Color.DarkGray;
            CancelButton.StateCommon.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            CancelButton.StateCommon.Content.ShortText.Color1 = Color.White;
            CancelButton.StateCommon.Content.ShortText.Font = new Font("Segoe UI", 15.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            CancelButton.StateCommon.Border.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.False;
            CancelButton.StateCommon.Border.Rounding = 0;
            CancelButton.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            CancelButton.Click += OnCancelButtonClick;
            CancelButton.OverrideDefault.Back.Color1 = Color.DarkGray;
            CancelButton.OverrideDefault.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            CancelButton.StateTracking.Back.Color1 = Color.FromArgb(179, 179, 179);
            CancelButton.Visible = false;

            FilesLabel = new Label()
            {
                Parent = this,
                ForeColor = Color.FromArgb(56, 184, 238),
                Font = CancelButton.Font,
                Text = "Прикрепить файлы",
                Left = CancelButton.Width + CancelButton.Left + 5,
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                Cursor = Cursors.Hand,
                AutoSize = true,
                Visible = false
            };
            FilesLabel.Click += OnFileLabelClick;
        }

        public void CloseCommentsTextBox()
        {
            bEdit = false;
            bOpened = false;
            OnCancelButtonClick(this, null);
            OnCommentsTextBoxClosed(this);
        }

        public void OpenCommentsTextBox(bool Edit)
        {
            if (bEdit)
                return;

            bEdit = Edit;
            bOpened = true;

            RichTextBox.Visible = true;
            RichTextBox.Top = rTextBoxRect.Y;

            SendButton.Visible = true;
            SendButton.Top = RichTextBox.Top + RichTextBox.Height + 5;

            CancelButton.Visible = true;
            CancelButton.Top = SendButton.Top;

            FilesLabel.Visible = true;
            FilesLabel.Top = CancelButton.Top;

            this.Height = iExpandedContainerHeight;

            CancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            SendButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            FilesLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;

            RichTextBox.Focus();

            OnRichTextBoxSizeChanged(this, iExpandedContainerHeight - iInitialContainerHeight);
            OnCommentsTextBoxOpened(this);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            e.Graphics.DrawLine(pBorderPen, 0, 0, this.Width, 0);

            rTextBoxRect.Y = (this.Height - rTextBoxRect.Height) / 2;
            e.Graphics.DrawRectangle(pBorderPen, rTextBoxRect);
            e.Graphics.FillRectangle(brCommentsTextBoxBackBrush, rTextBoxRect.Left + 1, rTextBoxRect.Top + 1, rTextBoxRect.Width - 1, rTextBoxRect.Height - 1);
            e.Graphics.DrawString("Прокомментировать", fCommentsTextBoxFont, brCommentsTextFontBrush, 6, rTextBoxRect.Top + 6);

            if (bShowButtons)
            {
                AddConfirmBMP.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);

                iAddButtonsTop = 18;
                iAddConfirmLeft = 365;

                e.Graphics.DrawImage(AddConfirmBMP, iAddConfirmLeft, 18);


                AddUserBMP.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);

                iAddUserLeft = iAddConfirmLeft + AddConfirmBMP.Width + 5;

                e.Graphics.DrawImage(AddUserBMP, iAddUserLeft, 18);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            bTextTrack = false;
            bConfirmTrack = false;
            bUserTrack = false;

            if (e.X >= rTextBoxRect.Left && e.X <= rTextBoxRect.Left + rTextBoxRect.Width &&
               e.Y >= rTextBoxRect.Top && e.Y <= rTextBoxRect.Top + rTextBoxRect.Height)
            {
                bTextTrack = true;
            }

            if (e.X >= iAddConfirmLeft && e.X <= iAddConfirmLeft + AddConfirmBMP.Width && e.Y >= iAddButtonsTop && e.Y <= iAddButtonsTop + AddConfirmBMP.Height)
            {
                if (ToolTipText != "Запрос подтверждения")
                {
                    ToolTipText = "Запрос подтверждения";
                }

                bConfirmTrack = true;
            }

            if (e.X >= iAddUserLeft && e.X <= iAddUserLeft + AddUserBMP.Width && e.Y >= iAddButtonsTop && e.Y <= iAddButtonsTop + AddUserBMP.Height)
            {
                if (ToolTipText != "Добавить получателей")
                {
                    ToolTipText = "Добавить получателей";
                }

                bUserTrack = true;
            }

            if (bTextTrack)
                this.Cursor = Cursors.IBeam;

            if (bConfirmTrack)
                this.Cursor = Cursors.Hand;

            if (bUserTrack)
                this.Cursor = Cursors.Hand;

            if (!bTextTrack && !bConfirmTrack && !bUserTrack)
            {
                if (ToolTipText != "")
                {
                    ToolTipText = "";
                }

                if (this.Cursor != Cursors.Default)
                    this.Cursor = Cursors.Default;
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (this.Cursor == Cursors.IBeam)
                this.Cursor = Cursors.Default;

            if (this.Cursor == Cursors.Hand)
            {
                this.Cursor = Cursors.Default;
            }

            if (ToolTipText != "")
            {
                ToolTipText = "";
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (bTextTrack)
            {
                OpenCommentsTextBox(false);
            }

            if (bConfirmTrack)
            {
                OnAddConfirmClicked();
            }

            if (bUserTrack)
                OnAddUserClicked();
        }

        private void OnFileLabelClick(object sender, EventArgs e)
        {
            OnFileLabelClicked(this);
        }

        private void OnCancelButtonClick(object sender, EventArgs e)
        {
            bEdit = false;
            bOpened = false;

            if (!RichTextBox.Visible)
                return;

            RichTextBox.Visible = false;
            SendButton.Visible = false;
            CancelButton.Visible = false;
            FilesLabel.Visible = false;

            RichTextBox.Clear();
            RichTextBox.Height = iExpandedTextBoxHeight;

            int iH = this.Height;

            this.Height = iInitialContainerHeight;

            CancelButton.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            SendButton.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            FilesLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            OnCancelButtonClicked(this, iInitialContainerHeight - iH);
            OnCommentsTextBoxClosed(this);
        }

        private void OnRichTextBoxTextChanged(object sender, EventArgs e)
        {
            if (bCtrlEnter)
                return;

            int iHeight = 0;

            int iPos = RichTextBox.GetPositionFromCharIndex(RichTextBox.Text.Length).Y;

            if (iPos + 45 != RichTextBox.Height && iPos + 45 >= iExpandedTextBoxHeight)
            {
                iHeight = (RichTextBox.GetPositionFromCharIndex(RichTextBox.Text.Length).Y + 45) - RichTextBox.Height;
                RichTextBox.Height += iHeight;
                this.Height += iHeight;
                OnRichTextBoxSizeChanged(this, iHeight);
            }
        }

        private void OnRichTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                OnSendButtonClicked(SendButton, null);
            }
        }

        public event SendButtonEventHandler SendButtonClicked;
        public event CancelEventHandler CancelButtonClicked;
        public event SizeChangedEventHandler RichTextBoxSizeChanged;
        public event EventHandler CommentsTextBoxOpened;
        public event EventHandler FileLabelClicked;
        public event EventHandler CommentsTextBoxClosed;
        public event EventHandler AddConfirmClicked;
        public event EventHandler AddUserClicked;

        public delegate void SendButtonEventHandler(object sender, string Text, bool bIsEdit);
        public delegate void SizeChangedEventHandler(object sender, int Height);
        public delegate void CancelEventHandler(object sender, int Height);
        public delegate void CommentEditEventHandler(object sender, int DocumentCommentID);

        public void Clear()
        {
            RichTextBox.Clear();
        }

        public virtual void OnRichTextBoxSizeChanged(object sender, int Height)
        {
            RichTextBoxSizeChanged?.Invoke(this, Height);//Raise the event
        }

        public virtual void OnSendButtonClicked(object sender, EventArgs e)
        {
            SendButtonClicked?.Invoke(this, RichTextBox.Text, bEdit);//Raise the event
        }

        public virtual void OnCancelButtonClicked(object sender, int iHeight)
        {
            CancelButtonClicked?.Invoke(this, iHeight);//Raise the event
        }

        public virtual void OnCommentsTextBoxOpened(object sender)
        {
            CommentsTextBoxOpened?.Invoke(this, null);//Raise the event
        }

        public virtual void OnFileLabelClicked(object sender)
        {
            FileLabelClicked?.Invoke(this, null);//Raise the event
        }

        public virtual void OnCommentsTextBoxClosed(object sender)
        {
            CommentsTextBoxClosed?.Invoke(this, null);//Raise the event
        }

        public virtual void OnAddConfirmClicked()
        {
            AddConfirmClicked?.Invoke(this, null);//Raise the event
        }

        public virtual void OnAddUserClicked()
        {
            AddUserClicked?.Invoke(this, null);//Raise the event
        }



        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            RichTextBox.Focus();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (this.Visible)
                this.Focus();
        }
    }



    public class InfiniumDocumentsCheckButton : Control
    {
        Font fCaptionFont = new Font("Segoe UI", 16.0f, FontStyle.Regular, GraphicsUnit.Pixel);
        Font fCheckedCaptionFont = new Font("Segoe UI Semibold", 16.0f, FontStyle.Regular, GraphicsUnit.Pixel);

        SolidBrush brCaptionBrush = new SolidBrush(Color.FromArgb(70, 70, 70));
        SolidBrush brCheckedCaptionBrush = new SolidBrush(Color.FromArgb(60, 60, 60));

        Pen pBorderPen = new Pen(new SolidBrush(Color.FromArgb(200, 200, 200)));
        Pen pLinePen = new Pen(new SolidBrush(Color.FromArgb(230, 230, 230)), 2);
        Pen pCheckedLinePen = new Pen(new SolidBrush(Color.FromArgb(83, 206, 255)), 2);

        Rectangle rBorderRect = new Rectangle(0, 0, 0, 0);

        ToolTip ToolTip;

        string sToolTipText = "";

        public bool bCheck = false;

        public string sCaption = "";


        public InfiniumDocumentsCheckButton()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            ToolTip = new System.Windows.Forms.ToolTip();
            ToolTip.SetToolTip(this, sToolTipText);

            this.Cursor = Cursors.Hand;

            this.Height = 40;
            this.Width = 90;
        }

        public string ToolTipText
        {
            get { return sToolTipText; }
            set { sToolTipText = value; ToolTip.SetToolTip(this, value); }
        }

        public string Caption
        {
            get { return sCaption; }
            set { sCaption = value; this.Refresh(); }
        }

        public bool Checked
        {
            get { return bCheck; }
            set { bCheck = value; this.Refresh(); }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            rBorderRect = this.ClientRectangle;
            rBorderRect.Width = this.ClientRectangle.Width - 1;
            rBorderRect.Height = this.ClientRectangle.Height - 1;

            e.Graphics.DrawRectangle(pBorderPen, rBorderRect);

            if (bCheck)
            {
                e.Graphics.DrawString(sCaption, fCheckedCaptionFont, brCaptionBrush, (this.Width - e.Graphics.MeasureString(sCaption, fCaptionFont).Width) / 2,
                                                                         (this.Height - e.Graphics.MeasureString(sCaption, fCaptionFont).Height) / 2);
                e.Graphics.DrawLine(pCheckedLinePen, 15, this.Height - 6, this.Width - 15, this.Height - 6);
            }
            else
            {
                e.Graphics.DrawString(sCaption, fCaptionFont, brCaptionBrush, (this.Width - e.Graphics.MeasureString(sCaption, fCaptionFont).Width) / 2,
                                                                         (this.Height - e.Graphics.MeasureString(sCaption, fCaptionFont).Height) / 2);
                e.Graphics.DrawLine(pLinePen, 15, this.Height - 6, this.Width - 15, this.Height - 6);
            }

        }

        protected override void OnClick(EventArgs e)
        {
            if (!Checked)
                Checked = true;

            base.OnClick(e);
        }
    }







    public class InfiniumContractorsMenuItem : Control
    {
        Font fCaptionFont = new Font("Segoe UI Semibold", 15.0f, FontStyle.Regular, GraphicsUnit.Pixel);

        SolidBrush brCaptionBrush = new SolidBrush(Color.FromArgb(60, 60, 60));
        SolidBrush brSelectedFontBrush = new SolidBrush(Color.FromArgb(60, 60, 60));
        SolidBrush brTrackFontBrush = new SolidBrush(Color.FromArgb(60, 60, 60));
        SolidBrush brSelectedBackBrush = new SolidBrush(Color.FromArgb(245, 245, 245));

        SolidBrush brItemColorBrush = new SolidBrush(Color.White);

        Rectangle rColorRect = new Rectangle(10, 0, 7, 7);

        string sCaption = "Item";

        int iItemHeight = 37;
        bool bTrack = false;

        bool bSelected = false;
        public int index = -1;

        ToolTip ToolTip;

        string sToolTipText = "";

        public InfiniumContractorsMenuItem()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            this.BackColor = Color.Transparent;
            this.Cursor = Cursors.Hand;

            ToolTip = new System.Windows.Forms.ToolTip();
            ToolTip.SetToolTip(this, sToolTipText);

            this.Height = iItemHeight;
            this.Width = 50;
        }


        public string ToolTipText
        {
            get { return sToolTipText; }
            set { sToolTipText = value; ToolTip.SetToolTip(this, value); }
        }


        public bool Selected
        {
            get { return bSelected; }
            set
            {
                bSelected = value;
                this.Refresh();
            }
        }

        public string Caption
        {
            get { return sCaption; }
            set
            {
                sCaption = value;

                this.Refresh();
            }
        }

        public int ItemHeight
        {
            get { return iItemHeight; }
            set { iItemHeight = value; this.Height = value; this.Refresh(); }
        }

        public Color ItemColor
        {
            get { return brItemColorBrush.Color; }
            set { brItemColorBrush.Color = value; this.Refresh(); }
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            if (bTrack || bSelected)
            {
                e.Graphics.FillRectangle(brSelectedBackBrush, this.ClientRectangle);
            }

            rColorRect.Y = (this.Height - rColorRect.Height) / 2;

            e.Graphics.FillRectangle(brItemColorBrush, rColorRect);

            string sText = "";

            if (e.Graphics.MeasureString(sCaption, fCaptionFont).Width > this.Width - rColorRect.Width - rColorRect.X - 10 - 8)
            {
                for (int i = sCaption.Length; i >= 0; i--)
                {
                    if (e.Graphics.MeasureString(sCaption.Substring(0, i), fCaptionFont).Width <= this.Width - rColorRect.Width - rColorRect.X - 10 - 8)
                    {
                        sText = sCaption.Substring(0, i - 1) + "...";
                        break;
                    }
                }
            }
            else
            {
                sText = sCaption;
            }

            if (bSelected)
                e.Graphics.DrawString(sText, fCaptionFont, brSelectedFontBrush, rColorRect.Width + rColorRect.X + 10,
                                     (this.Height - e.Graphics.MeasureString(sText, fCaptionFont).Height) / 2);
            else
                e.Graphics.DrawString(sText, fCaptionFont, brCaptionBrush, rColorRect.Width + rColorRect.X + 10,
                                     (this.Height - e.Graphics.MeasureString(sText, fCaptionFont).Height) / 2);

        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!bTrack)
            {
                bTrack = true;
                this.Refresh();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (bTrack)
            {
                bTrack = false;
                this.Refresh();
            }
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            Selected = true;

            OnItemClicked(this, sCaption, index, bSelected);

            Parent.Focus();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            using (Graphics G = this.CreateGraphics())
            {
                if (G.MeasureString(sCaption, fCaptionFont).Width > this.Width - rColorRect.Width - rColorRect.X - 10 - 8)
                    ToolTipText = sCaption;
                else
                    ToolTipText = "";
            }
        }


        public event ClickEventHandler ItemClicked;

        public delegate void ClickEventHandler(object sender, string Name, int index, bool bSelected);

        public virtual void OnItemClicked(object sender, string Name, int index, bool bSelected)
        {
            ItemClicked?.Invoke(sender, Name, index, bSelected);
        }
    }


    public class InfiniumContractorsMenu : Control
    {
        public int Offset = 0;

        int iTempScrollWheelOffset = 0;

        Color[] ItemColors;

        InfiniumScrollContainer ScrollContainer;
        public InfiniumProjectsVerticalScrollBar VerticalScroll;

        public InfiniumContractorsMenuItem[] Items;

        public DataTable ItemsDT;

        public string SelectedName = "";

        int iSelected = -1;

        int iMarginToNextItem = 0;

        public InfiniumContractorsMenu()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            VerticalScroll = new InfiniumProjectsVerticalScrollBar()
            {
                Parent = this,
                Height = this.Height,
                Width = 12,
                TotalControlHeight = this.Height,
                Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
            };
            VerticalScroll.Left = this.Width - VerticalScroll.Width;
            VerticalScroll.TotalControlHeight = this.Height;
            VerticalScroll.VerticalScrollCommonShaftBackColor = Color.FromArgb(240, 240, 240);
            VerticalScroll.VerticalScrollCommonThumbButtonColor = Color.Silver;
            VerticalScroll.VerticalScrollTrackingShaftBackColor = Color.FromArgb(230, 230, 230);
            VerticalScroll.VerticalScrollTrackingThumbButtonColor = Color.FromArgb(140, 140, 140);
            VerticalScroll.ScrollPositionChanged += OnScrollPositionChanged;

            ScrollContainer = new InfiniumScrollContainer()
            {
                Parent = this,
                Top = 0,
                Left = 0,
                Width = this.Width,
                Height = this.Height,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            ScrollContainer.SizeChanged += OnScrollContainerSizeChanged;
            ScrollContainer.BackgroundColor = Color.White;

            ItemColors = new Color[5];

            ItemColors[0] = Color.FromArgb(104, 193, 95);
            ItemColors[1] = Color.FromArgb(120, 125, 195);
            ItemColors[2] = Color.FromArgb(201, 118, 125);
            ItemColors[3] = Color.FromArgb(243, 189, 59);
            ItemColors[4] = Color.FromArgb(104, 185, 224);
        }

        public void InitializeItems()
        {
            if (Items != null)
            {
                for (int i = 0; i < Items.Count(); i++)
                {
                    if (Items[i] != null)
                    {
                        Items[i].Dispose();
                        Items[i] = null;
                    }
                }

                Items = new InfiniumContractorsMenuItem[0];
            }

            Offset = 0;
            ScrollContainer.Top = -Offset;
            VerticalScroll.Offset = Offset;
            VerticalScroll.Refresh();

            if (ItemsDT == null)
                return;

            if (ItemsDT.DefaultView.Count > 0)
            {
                Items = new InfiniumContractorsMenuItem[ItemsDT.DefaultView.Count];

                int iC = 0;

                for (int i = 0; i < ItemsDT.DefaultView.Count; i++)
                {
                    Items[i] = new InfiniumContractorsMenuItem();

                    Items[i].Top = i * (Items[i].ItemHeight + iMarginToNextItem);
                    Items[i].Caption = ItemsDT.DefaultView[i]["Name"].ToString();
                    Items[i].index = i;
                    Items[i].ItemColor = ItemColors[iC];

                    if (iC == ItemColors.Count() - 1)
                        iC = 0;
                    else
                        iC++;


                    Items[i].Parent = ScrollContainer;
                    Items[i].Width = ScrollContainer.Width;
                    Items[i].Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                    Items[i].ItemClicked += OnItemClicked;
                }

                Selected = 0;
            }

            SetScrollHeight();
        }

        private void SetScrollHeight()
        {
            if (Items == null || ItemsDataTable.DefaultView.Count == 0)
            {
                ScrollContainer.Height = this.Height;
                VerticalScroll.TotalControlHeight = ScrollContainer.Height;
                VerticalScroll.Refresh();

                return;
            }

            if (Items.Count() * (Items[0].Height + iMarginToNextItem) > this.Height)
                ScrollContainer.Height = Items.Count() * (Items[0].Height + iMarginToNextItem);
            else
                ScrollContainer.Height = this.Height;

            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
            VerticalScroll.Refresh();
        }

        public int Selected
        {
            get { return iSelected; }
            set
            {
                iSelected = value;

                if (Items == null)
                    return;

                Items[value].Selected = true;
                SelectedName = Items[value].Caption;

                foreach (InfiniumContractorsMenuItem Item in Items)
                {
                    if (Item.index != value)
                        Item.Selected = false;
                }

                OnSelectedChanged(this, SelectedName, value);
            }
        }

        public Color SelectedColor
        {
            get
            {
                if (Selected > -1)
                    return Items[Selected].ItemColor;
                else
                    return Color.White;
            }
        }

        public DataTable ItemsDataTable
        {
            get
            {
                //if (ItemsDT == null)
                //    return ItemsDT;

                //if (Items.Count() > 0)
                //    for (int i = 0; i < Items.Count(); i++)
                //    {
                //        if (Items[i] == null)
                //            continue;

                //        if (Items[i].Checked)
                //            ItemsDT.Rows[i]["Checked"] = true;
                //        else
                //            ItemsDT.Rows[i]["Checked"] = false;
                //    }

                return ItemsDT;
            }
            set
            {
                ItemsDT = value;
            }
        }

        private void OnScrollContainerSizeChanged(object sender, EventArgs e)
        {
            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
        }

        private void OnScrollPositionChanged(object sender, int tOffset)
        {
            Offset = tOffset;
            ScrollContainer.Top = -Offset;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (!VerticalScroll.Visible)
                return;

            if (Offset == 0)
                if (iTempScrollWheelOffset > 0)
                    iTempScrollWheelOffset = 0;

            if (e.Delta < 0)
            {
                if (Offset < ScrollContainer.Height - this.Height)
                {
                    if (Offset + VerticalScroll.ScrollWheelOffset + this.Height > ScrollContainer.Height)//last
                    {
                        iTempScrollWheelOffset = ScrollContainer.Height - this.Height - Offset;
                        Offset += iTempScrollWheelOffset;
                    }
                    else
                    {
                        Offset += VerticalScroll.ScrollWheelOffset;
                    }

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
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
                        Offset -= VerticalScroll.ScrollWheelOffset;

                    if (Offset < 0)
                        Offset = 0;

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
                }
        }

        private void OnItemClicked(object sender, string Name, int index, bool bSelected)
        {
            if (index == iSelected)
                return;

            iSelected = index;
            SelectedName = Name;

            foreach (InfiniumContractorsMenuItem Item in Items)
            {
                if (Item.index != index)
                    Item.Selected = false;
            }

            OnSelectedChanged(this, Name, index);
        }



        public event SelectedChangedEventHandler SelectedChanged;

        public delegate void SelectedChangedEventHandler(object sender, string Name, int ContractorCategoryID);


        public virtual void OnSelectedChanged(object sender, string Name, int index)
        {
            iSelected = index;

            SelectedChanged?.Invoke(sender, Name, Convert.ToInt32(ItemsDataTable.DefaultView[index]["ContractorCategoryID"]));
        }


        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            ScrollContainer.Focus();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            SetScrollHeight();

            VerticalScroll.Left = this.Width - VerticalScroll.Width;
        }
    }




    public class InfiniumContractorsSubMenuItem : Control
    {
        Font fCaptionFont = new Font("Segoe UI Semibold", 15.0f, FontStyle.Regular, GraphicsUnit.Pixel);

        SolidBrush brCaptionBrush = new SolidBrush(Color.FromArgb(60, 60, 60));
        SolidBrush brSelectedFontBrush = new SolidBrush(Color.FromArgb(60, 60, 60));
        SolidBrush brTrackFontBrush = new SolidBrush(Color.FromArgb(60, 60, 60));
        SolidBrush brSelectedBackBrush = new SolidBrush(Color.FromArgb(245, 245, 245));

        SolidBrush brItemColorBrush = new SolidBrush(Color.White);

        Rectangle rColorRect = new Rectangle(10, 0, 7, 7);

        string sCaption = "Item";

        int iItemHeight = 37;
        bool bTrack = false;

        bool bSelected = false;
        public int index = -1;

        ToolTip ToolTip;

        string sToolTipText = "";

        public InfiniumContractorsSubMenuItem()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            this.BackColor = Color.Transparent;
            this.Cursor = Cursors.Hand;

            ToolTip = new System.Windows.Forms.ToolTip();
            ToolTip.SetToolTip(this, sToolTipText);

            this.Height = iItemHeight;
            this.Width = 50;
        }


        public string ToolTipText
        {
            get { return sToolTipText; }
            set { sToolTipText = value; ToolTip.SetToolTip(this, value); }
        }


        public bool Selected
        {
            get { return bSelected; }
            set
            {
                bSelected = value;
                this.Refresh();
            }
        }

        public string Caption
        {
            get { return sCaption; }
            set
            {
                sCaption = value;

                this.Refresh();
            }
        }

        public int ItemHeight
        {
            get { return iItemHeight; }
            set { iItemHeight = value; this.Height = value; this.Refresh(); }
        }

        public Color ItemColor
        {
            get { return brItemColorBrush.Color; }
            set { brItemColorBrush.Color = value; this.Refresh(); }
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            if (bTrack || bSelected)
            {
                e.Graphics.FillRectangle(brSelectedBackBrush, this.ClientRectangle);
            }

            rColorRect.Y = (this.Height - rColorRect.Height) / 2;

            e.Graphics.FillRectangle(brItemColorBrush, rColorRect);

            string sText = "";

            if (e.Graphics.MeasureString(sCaption, fCaptionFont).Width > this.Width - rColorRect.Width - rColorRect.X - 10 - 8)
            {
                for (int i = sCaption.Length; i >= 0; i--)
                {
                    if (e.Graphics.MeasureString(sCaption.Substring(0, i), fCaptionFont).Width <= this.Width - rColorRect.Width - rColorRect.X - 10 - 8)
                    {
                        sText = sCaption.Substring(0, i - 1) + "...";
                        break;
                    }
                }
            }
            else
            {
                sText = sCaption;
            }

            if (bSelected)
                e.Graphics.DrawString(sText, fCaptionFont, brSelectedFontBrush, rColorRect.Width + rColorRect.X + 10,
                                     (this.Height - e.Graphics.MeasureString(sText, fCaptionFont).Height) / 2);
            else
                e.Graphics.DrawString(sText, fCaptionFont, brCaptionBrush, rColorRect.Width + rColorRect.X + 10,
                                     (this.Height - e.Graphics.MeasureString(sText, fCaptionFont).Height) / 2);

        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!bTrack)
            {
                bTrack = true;
                this.Refresh();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (bTrack)
            {
                bTrack = false;
                this.Refresh();
            }
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            Selected = true;

            OnItemClicked(this, sCaption, index, bSelected);

            Parent.Focus();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            using (Graphics G = this.CreateGraphics())
            {
                if (G.MeasureString(sCaption, fCaptionFont).Width > this.Width - rColorRect.Width - rColorRect.X - 10 - 8)
                    ToolTipText = sCaption;
                else
                    ToolTipText = "";
            }
        }


        public event ClickEventHandler ItemClicked;

        public delegate void ClickEventHandler(object sender, string Name, int index, bool bSelected);

        public virtual void OnItemClicked(object sender, string Name, int index, bool bSelected)
        {
            ItemClicked?.Invoke(sender, Name, index, bSelected);
        }
    }


    public class InfiniumContractorsSubMenu : Control
    {
        public int Offset = 0;

        int iTempScrollWheelOffset = 0;

        public Color ItemsColor = Color.White;

        InfiniumScrollContainer ScrollContainer;
        public InfiniumProjectsVerticalScrollBar VerticalScroll;

        public InfiniumContractorsSubMenuItem[] Items;

        public DataTable ItemsDT;

        public string SelectedName = "";

        int iSelected = -1;

        int iMarginToNextItem = 0;

        public InfiniumContractorsSubMenu()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            VerticalScroll = new InfiniumProjectsVerticalScrollBar()
            {
                Parent = this,
                Height = this.Height,
                Width = 12,
                TotalControlHeight = this.Height,
                Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom
            };
            VerticalScroll.Left = this.Width - VerticalScroll.Width;
            VerticalScroll.TotalControlHeight = this.Height;
            VerticalScroll.VerticalScrollCommonShaftBackColor = Color.FromArgb(240, 240, 240);
            VerticalScroll.VerticalScrollCommonThumbButtonColor = Color.Silver;
            VerticalScroll.VerticalScrollTrackingShaftBackColor = Color.FromArgb(230, 230, 230);
            VerticalScroll.VerticalScrollTrackingThumbButtonColor = Color.FromArgb(140, 140, 140);
            VerticalScroll.ScrollPositionChanged += OnScrollPositionChanged;

            ScrollContainer = new InfiniumScrollContainer()
            {
                Parent = this,
                Top = 0,
                Left = 0,
                Width = this.Width,
                Height = this.Height,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            ScrollContainer.SizeChanged += OnScrollContainerSizeChanged;
            ScrollContainer.BackgroundColor = Color.White;
        }

        public void InitializeItems()
        {
            if (Items != null)
            {
                for (int i = 0; i < Items.Count(); i++)
                {
                    if (Items[i] != null)
                    {
                        Items[i].Dispose();
                        Items[i] = null;
                    }
                }

                Items = new InfiniumContractorsSubMenuItem[0];
            }

            Offset = 0;
            ScrollContainer.Top = -Offset;
            VerticalScroll.Offset = Offset;
            VerticalScroll.Refresh();

            if (ItemsDT == null)
                return;

            if (ItemsDT.DefaultView.Count > 0)
            {
                Items = new InfiniumContractorsSubMenuItem[ItemsDT.DefaultView.Count];

                for (int i = 0; i < ItemsDT.DefaultView.Count; i++)
                {
                    Items[i] = new InfiniumContractorsSubMenuItem();

                    Items[i].Top = i * (Items[i].ItemHeight + iMarginToNextItem);
                    Items[i].Caption = ItemsDT.DefaultView[i]["Name"].ToString();
                    Items[i].index = i;
                    Items[i].ItemColor = ItemsColor;


                    Items[i].Parent = ScrollContainer;
                    Items[i].Width = ScrollContainer.Width;
                    Items[i].Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                    Items[i].ItemClicked += OnItemClicked;
                }

                Selected = 0;
            }

            SetScrollHeight();
        }

        private void SetScrollHeight()
        {
            if (Items == null || ItemsDataTable.DefaultView.Count == 0)
            {
                ScrollContainer.Height = this.Height;
                VerticalScroll.TotalControlHeight = ScrollContainer.Height;
                VerticalScroll.Refresh();

                return;
            }

            if (Items.Count() * (Items[0].Height + iMarginToNextItem) > this.Height)
                ScrollContainer.Height = Items.Count() * (Items[0].Height + iMarginToNextItem);
            else
                ScrollContainer.Height = this.Height;

            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
            VerticalScroll.Refresh();
        }

        public int Selected
        {
            get { return iSelected; }
            set
            {
                iSelected = value;

                if (Items == null)
                    return;

                Items[value].Selected = true;
                SelectedName = Items[value].Caption;

                foreach (InfiniumContractorsSubMenuItem Item in Items)
                {
                    if (Item.index != value)
                        Item.Selected = false;
                }

                OnSelectedChanged(this, SelectedName, value);
            }
        }

        public int SelectedContractorSubCategoryID
        {
            get
            {
                if (Selected > -1)
                {
                    if (ItemsDataTable.DefaultView.Count == 0)
                        return -1;
                    else
                        return Convert.ToInt32(ItemsDataTable.DefaultView[Selected]["ContractorSubCategoryID"]);
                }
                else
                    return -1;
            }
        }

        public DataTable ItemsDataTable
        {
            get
            {
                //if (ItemsDT == null)
                //    return ItemsDT;

                //if (Items.Count() > 0)
                //    for (int i = 0; i < Items.Count(); i++)
                //    {
                //        if (Items[i] == null)
                //            continue;

                //        if (Items[i].Checked)
                //            ItemsDT.Rows[i]["Checked"] = true;
                //        else
                //            ItemsDT.Rows[i]["Checked"] = false;
                //    }

                return ItemsDT;
            }
            set
            {
                ItemsDT = value;
            }
        }

        private void OnScrollContainerSizeChanged(object sender, EventArgs e)
        {
            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
        }

        private void OnScrollPositionChanged(object sender, int tOffset)
        {
            Offset = tOffset;
            ScrollContainer.Top = -Offset;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (!VerticalScroll.Visible)
                return;

            if (Offset == 0)
                if (iTempScrollWheelOffset > 0)
                    iTempScrollWheelOffset = 0;

            if (e.Delta < 0)
            {
                if (Offset < ScrollContainer.Height - this.Height)
                {
                    if (Offset + VerticalScroll.ScrollWheelOffset + this.Height > ScrollContainer.Height)//last
                    {
                        iTempScrollWheelOffset = ScrollContainer.Height - this.Height - Offset;
                        Offset += iTempScrollWheelOffset;
                    }
                    else
                    {
                        Offset += VerticalScroll.ScrollWheelOffset;
                    }

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
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
                        Offset -= VerticalScroll.ScrollWheelOffset;

                    if (Offset < 0)
                        Offset = 0;

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
                }
        }

        private void OnItemClicked(object sender, string Name, int index, bool bSelected)
        {
            if (index == iSelected)
                return;

            iSelected = index;
            SelectedName = Name;

            foreach (InfiniumContractorsSubMenuItem Item in Items)
            {
                if (Item.index != index)
                    Item.Selected = false;
            }

            OnSelectedChanged(this, Name, index);
        }



        public event SelectedChangedEventHandler SelectedChanged;

        public delegate void SelectedChangedEventHandler(object sender, string Name, int index);


        public virtual void OnSelectedChanged(object sender, string Name, int index)
        {
            iSelected = index;

            SelectedChanged?.Invoke(sender, Name, index);
        }


        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            ScrollContainer.Focus();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            SetScrollHeight();

            VerticalScroll.Left = this.Width - VerticalScroll.Width;
        }
    }




    public class InfiniumContractorsTextBox : TextBox
    {
        ToolTip ToolTip;

        string sToolTipText = "";

        public InfiniumContractorsTextBox(Color cBackColor)
        {
            ToolTip = new System.Windows.Forms.ToolTip();
            ToolTip.SetToolTip(this, sToolTipText);

            this.Font = new Font("Segoe UI", 14.0f, FontStyle.Regular, GraphicsUnit.Pixel);
            this.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ReadOnly = true;
            this.BackColor = cBackColor;
            this.ForeColor = Color.FromArgb(70, 70, 70);
        }

        public string ToolTipText
        {
            get { return sToolTipText; }
            set { sToolTipText = value; ToolTip.SetToolTip(this, value); }
        }

        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;

                using (Graphics G = this.CreateGraphics())
                {
                    this.Width = Convert.ToInt32(G.MeasureString(value, Font).Width);
                }
            }
        }
    }




    public class InfiniumContractorItem : Control
    {
        Color cBackColor = Color.White;

        Font fNameFont = new Font("Segoe UI Semibold", 20.0f, FontStyle.Regular, GraphicsUnit.Pixel);
        Font fTextFont = new Font("Segoe UI", 15.0f, FontStyle.Regular, GraphicsUnit.Pixel);
        Font fLabelsFont = new Font("Segoe UI Semibold", 15.0f, FontStyle.Regular, GraphicsUnit.Pixel);
        Font fLinkFont = new Font("Segoe UI", 15.0f, FontStyle.Regular, GraphicsUnit.Pixel);

        SolidBrush brNameBrush = new SolidBrush(Color.FromArgb(70, 70, 70));
        SolidBrush brTextBrush = new SolidBrush(Color.FromArgb(80, 80, 80));
        SolidBrush brNoBrush = new SolidBrush(Color.FromArgb(163, 163, 163));
        SolidBrush brLabelsBrush = new SolidBrush(Color.FromArgb(70, 70, 70));
        SolidBrush brLinkBrush = new SolidBrush(Color.FromArgb(56, 174, 238));
        public SolidBrush brItemColorBrush = new SolidBrush(Color.White);

        Pen pBorderPen = new Pen(new SolidBrush(Color.FromArgb(200, 200, 200)));

        Rectangle rBorderRect = new Rectangle(0, 0, 0, 0);
        Rectangle rColorRect = new Rectangle(15, 15, 15, 15);

        int iBorderMargin = 17;
        int iTextMargin = 23;

        int iEditLeft = 0;
        int iDeleteLeft = 0;
        int iEditTop = 0;

        Bitmap DeleteBMP = Properties.Resources.DocDelete;
        Bitmap EditBMP = Properties.Resources.DocEdit;

        public int iInitialHeight = 175;
        int iReadLeft = 0;
        int iReadWidth = 0;
        int iReadTop = 0;
        int iReadHeight = 0;

        ToolTip ToolTip;

        string sToolTipText = "";

        public DataTable ContactsDataTable;

        public int iItemIndex = -1;

        public InfiniumContractsContactsPanel ContactsPanel;

        public string sName;
        public string sEmail;
        public string sWebsite;
        public string sAddress;
        public string sFacebook;
        public string sSkype;
        public string sCountry;
        public string sUNN;
        public string sDescription;

        InfiniumContractorsTextBox NameTextBox;
        InfiniumContractorsTextBox EmailTextBox;
        InfiniumContractorsTextBox WebsiteTextBox;
        InfiniumContractorsTextBox AddressTextBox;
        InfiniumContractorsTextBox FacebookTextBox;
        InfiniumContractorsTextBox SkypeTextBox;
        InfiniumContractorsTextBox CountryTextBox;
        InfiniumContractorsTextBox UNNTextBox;

        public int iContractorID = -1;
        public bool bCanEdit = false;

        bool bEditTrack = false;
        bool bDeleteTrack = false;
        bool bReadTrack = false;

        public InfiniumContractorItem()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            this.BackColor = Color.White;

            this.Height = iInitialHeight;
            this.Width = 1;

            ToolTip = new System.Windows.Forms.ToolTip();
            ToolTip.SetToolTip(this, sToolTipText);
        }

        public void Initialize()
        {
            using (Graphics G = this.CreateGraphics())
            {
                NameTextBox = new InfiniumContractorsTextBox(Color.White)
                {
                    Font = fNameFont,
                    Parent = this,
                    Text = sName,
                    Left = iBorderMargin + 27,
                    Top = iBorderMargin - 10
                };
                if (NameTextBox.Width > this.Width - iBorderMargin - iTextMargin - iBorderMargin - iTextMargin - EditBMP.Width - EditBMP.Width - 10)
                    NameTextBox.Width = this.Width - iBorderMargin - iTextMargin - iBorderMargin - iTextMargin - EditBMP.Width - EditBMP.Width - 10;
                NameTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

                CountryTextBox = new InfiniumContractorsTextBox(Color.White)
                {
                    Font = fTextFont,
                    Parent = this,
                    Left = iBorderMargin + iTextMargin + (int)G.MeasureString("Страна, город:", fLabelsFont).Width + 5,
                    Top = iBorderMargin + 30
                };
                CountryTextBox.Text = GetCaption(sCountry, G, 500 - CountryTextBox.Left - 10);
                if (CountryTextBox.Text != sCountry)
                    CountryTextBox.ToolTipText = sCountry;

                AddressTextBox = new InfiniumContractorsTextBox(Color.White)
                {
                    Font = fTextFont,
                    Parent = this,
                    Left = iBorderMargin + iTextMargin + (int)G.MeasureString("Адрес:", fLabelsFont).Width + 5,
                    Top = iBorderMargin + 60
                };
                AddressTextBox.Text = GetCaption(sAddress, G, 500 - AddressTextBox.Left - 10);
                if (AddressTextBox.Text != sAddress)
                    AddressTextBox.ToolTipText = sAddress;

                UNNTextBox = new InfiniumContractorsTextBox(Color.White)
                {
                    Font = fTextFont,
                    Parent = this,
                    Left = iBorderMargin + iTextMargin + (int)G.MeasureString("УНН:", fLabelsFont).Width + 5,
                    Top = iBorderMargin + 90
                };
                UNNTextBox.Text = GetCaption(sUNN, G, 500 - UNNTextBox.Left - 10);
                if (UNNTextBox.Text != sUNN)
                    UNNTextBox.ToolTipText = sUNN;




                EmailTextBox = new InfiniumContractorsTextBox(Color.White)
                {
                    Font = fTextFont,
                    Parent = this,
                    Left = 500 + iBorderMargin + (int)G.MeasureString("E-mail:", fLabelsFont).Width + 5,
                    Top = iBorderMargin + 30
                };
                EmailTextBox.Text = GetCaption(sEmail, G, this.Width - EmailTextBox.Width - 10);
                if (EmailTextBox.Text != sEmail)
                    EmailTextBox.ToolTipText = sEmail;

                WebsiteTextBox = new InfiniumContractorsTextBox(Color.White)
                {
                    Font = fTextFont,
                    Parent = this,
                    Left = 500 + iBorderMargin + (int)G.MeasureString("Веб-сайт:", fLabelsFont).Width + 5,
                    Top = iBorderMargin + 60
                };
                WebsiteTextBox.Text = GetCaption(sWebsite, G, this.Width - WebsiteTextBox.Width - 10);
                if (WebsiteTextBox.Text != sWebsite)
                    WebsiteTextBox.ToolTipText = sWebsite;

                SkypeTextBox = new InfiniumContractorsTextBox(Color.White)
                {
                    Font = fTextFont,
                    Parent = this,
                    Left = 500 + iBorderMargin + (int)G.MeasureString("Skype:", fLabelsFont).Width + 5,
                    Top = iBorderMargin + 90
                };
                SkypeTextBox.Text = GetCaption(sSkype, G, this.Width - SkypeTextBox.Width - 10);
                if (SkypeTextBox.Text != sSkype)
                    SkypeTextBox.ToolTipText = sSkype;

                FacebookTextBox = new InfiniumContractorsTextBox(Color.White)
                {
                    Font = fTextFont,
                    Parent = this,
                    Left = 500 + iBorderMargin + (int)G.MeasureString("Facebook:", fLabelsFont).Width + 5,
                    Top = iBorderMargin + 120
                };
                FacebookTextBox.Text = GetCaption(sFacebook, G, this.Width - FacebookTextBox.Width - 10);
                if (FacebookTextBox.Text != sFacebook)
                    FacebookTextBox.ToolTipText = sFacebook;
            }

            ContactsPanel = new InfiniumContractsContactsPanel()
            {
                Width = this.Width,
                Parent = this,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                ContactsDataTable = ContactsDataTable
            };
            ContactsPanel.Initialize();
            ContactsPanel.ControlPanelSizeChanged += OnControlPanelSizeChanged;
            this.Height += ContactsPanel.Height;
            ContactsPanel.Top = this.Height - ContactsPanel.Height;
        }

        public string GetCaption(string Caption, Graphics G, int iW)
        {
            string sText = "";

            if (G.MeasureString(Caption, fTextFont).Width > iW)
            {
                for (int i = Caption.Length; i >= 0; i--)
                {
                    if (G.MeasureString(Caption.Substring(0, i), fTextFont).Width <= iW)
                    {
                        sText = Caption.Substring(0, i - 1) + "...";
                        break;
                    }
                }
            }
            else
            {
                sText = Caption;
            }

            return sText;
        }

        public string ToolTipText
        {
            get { return sToolTipText; }
            set { sToolTipText = value; ToolTip.SetToolTip(this, value); }
        }

        public void ChangeItemSize(int Height)
        {
            this.Height += Height;

            OnItemSizeChanged(this, iItemIndex, Height);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            rBorderRect = this.ClientRectangle;
            rBorderRect.Width = this.ClientRectangle.Width - 1;
            rBorderRect.Height = this.ClientRectangle.Height - 1;

            e.Graphics.DrawRectangle(pBorderPen, rBorderRect);
            e.Graphics.FillRectangle(brItemColorBrush, rColorRect);

            if (bCanEdit)
            {
                iEditLeft = this.Width - 28 - 28 - 10 - iBorderMargin;
                iEditTop = iBorderMargin;
                iDeleteLeft = this.Width - 28 - iBorderMargin;

                EditBMP.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);
                DeleteBMP.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);

                e.Graphics.DrawImage(EditBMP, iEditLeft, iEditTop, 28, 28);
                e.Graphics.DrawImage(DeleteBMP, iDeleteLeft, iEditTop, 28, 28);
            }

            e.Graphics.DrawString("Страна, город:", fLabelsFont, brLabelsBrush, iBorderMargin + iTextMargin, iBorderMargin + 30);

            e.Graphics.DrawString("Адрес:", fLabelsFont, brLabelsBrush, iBorderMargin + iTextMargin, iBorderMargin + 60);

            e.Graphics.DrawString("УНН:", fLabelsFont, brLabelsBrush, iBorderMargin + iTextMargin, iBorderMargin + 90);

            e.Graphics.DrawString("Описание:", fLabelsFont, brLabelsBrush, iBorderMargin + iTextMargin, iBorderMargin + 120);

            if (sDescription.Length > 0)
            {
                iReadLeft = iBorderMargin + iTextMargin + (int)e.Graphics.MeasureString("Описание:", fLabelsFont).Width;
                iReadWidth = (int)e.Graphics.MeasureString("читать...", fLabelsFont).Width;
                iReadTop = iBorderMargin + 120;
                iReadHeight = (int)e.Graphics.MeasureString("читать...", fLabelsFont).Height;

                e.Graphics.DrawString("читать...", fLinkFont, brLinkBrush, iReadLeft, iBorderMargin + 120);
            }



            e.Graphics.DrawString("E-mail:", fLabelsFont, brLabelsBrush, iBorderMargin + 500, iBorderMargin + 30);

            e.Graphics.DrawString("Веб-сайт:", fLabelsFont, brLabelsBrush, iBorderMargin + 500, iBorderMargin + 60);

            e.Graphics.DrawString("Skype:", fLabelsFont, brLabelsBrush, iBorderMargin + 500, iBorderMargin + 90);

            e.Graphics.DrawString("Facebook:", fLabelsFont, brLabelsBrush, iBorderMargin + 500, iBorderMargin + 120);
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            Parent.Focus();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            bReadTrack = false;
            bEditTrack = false;
            bDeleteTrack = false;

            if (e.X >= iReadLeft && e.X <= iReadLeft + iReadWidth && e.Y >= iReadTop && e.Y <= iReadTop + iReadHeight)
            {
                bReadTrack = true;
            }

            if (bCanEdit)
            {
                if (e.Y >= iEditTop && e.Y <= iEditTop + EditBMP.Height && e.X >= iEditLeft && e.X <= iEditLeft + EditBMP.Width)
                {
                    bEditTrack = true;

                    if (ToolTipText != "Редактировать")
                        ToolTipText = "Редактировать";
                }

                if (e.Y >= iEditTop && e.Y <= iEditTop + EditBMP.Height && e.X >= iDeleteLeft && e.X <= iDeleteLeft + EditBMP.Width)
                {
                    bDeleteTrack = true;

                    if (ToolTipText != "Удалить")
                        ToolTipText = "Удалить";
                }
            }

            if (!bReadTrack && !bEditTrack && !bDeleteTrack)
            {
                if (ToolTipText != "")
                    ToolTipText = "";

                if (this.Cursor != Cursors.Default)
                    this.Cursor = Cursors.Default;
            }

            if (bReadTrack || bEditTrack || bDeleteTrack)
            {
                if (this.Cursor != Cursors.Hand)
                    this.Cursor = Cursors.Hand;
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (this.Cursor != Cursors.Default)
            {
                this.Cursor = Cursors.Default;
                ToolTipText = "";
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (bReadTrack)
                OnReadDescriptionClicked();

            if (bEditTrack)
                OnEditClicked(this, iContractorID);

            if (bDeleteTrack)
                OnDeleteClicked(this, iContractorID);
        }

        private void OnControlPanelSizeChanged(object sender, int Height)
        {
            ChangeItemSize(Height);
        }

        public event ItemSizeChangedEventHandler ItemSizeChanged;

        public event EditEventHandler EditClicked;
        public event EditEventHandler DeleteClicked;
        public event ReadDescriptionEventHandler ReadDescriptionClicked;

        public delegate void ItemSizeChangedEventHandler(object sender, int ItemIndex, int Height);
        public delegate void EditEventHandler(object sender, int iContractorID);
        public delegate void ReadDescriptionEventHandler(object sender, string sText);

        public virtual void OnItemSizeChanged(object sender, int ItemIndex, int Height)
        {
            ItemSizeChanged?.Invoke(sender, ItemIndex, Height);
        }


        public virtual void OnEditClicked(object sender, int iContractorID)
        {
            EditClicked?.Invoke(this, iContractorID);//Raise the event
        }

        public virtual void OnDeleteClicked(object sender, int iContractorID)
        {
            DeleteClicked?.Invoke(this, iContractorID);//Raise the event
        }

        public virtual void OnReadDescriptionClicked()
        {
            ReadDescriptionClicked?.Invoke(this, sDescription);//Raise the event
        }

    }


    public class InfiniumContractorsList : Control
    {
        Font fCaptionFont = new Font("Segoe UI", 22.0f, FontStyle.Regular, GraphicsUnit.Pixel);

        SolidBrush brCaptionBrush = new SolidBrush(Color.FromArgb(110, 110, 110));

        public int Offset = 0;

        public Color ItemColor = Color.White;

        int iTempScrollWheelOffset = 0;
        int iMarginToNextItem = 20;

        public int iCurrentCommentsEditIndex = -1;

        public int iCurPosY = 0;

        public InfiniumDocumentsUpdatesScrollContainer ScrollContainer;
        public InfiniumProjectsVerticalScrollBar VerticalScroll;

        public InfiniumContractorItem[] Items;

        public DataTable ItemsDataTable = null;
        public DataTable CountriesDataTable = null;
        public DataTable CitiesDataTable = null;
        public DataTable ContactsDataTable = null;

        public int DocumentCommentID = -1;
        public bool bCanEdit = false;

        public InfiniumContractorsList()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);

            VerticalScroll = new InfiniumProjectsVerticalScrollBar()
            {
                Parent = this,
                Height = this.Height
            };
            VerticalScroll.Left = this.Width - VerticalScroll.Width;
            VerticalScroll.TotalControlHeight = this.Height;
            VerticalScroll.ScrollWheelOffset = 50;
            VerticalScroll.Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            VerticalScroll.VerticalScrollCommonShaftBackColor = Color.FromArgb(240, 240, 240);
            VerticalScroll.VerticalScrollCommonThumbButtonColor = Color.Silver;
            VerticalScroll.VerticalScrollTrackingShaftBackColor = Color.FromArgb(230, 230, 230);
            VerticalScroll.VerticalScrollTrackingThumbButtonColor = Color.FromArgb(140, 140, 140);
            VerticalScroll.Width = 12;
            VerticalScroll.ScrollPositionChanged += OnScrollPositionChanged;

            ScrollContainer = new InfiniumDocumentsUpdatesScrollContainer()
            {
                Parent = this,
                Top = 0,
                Left = 0,
                Width = this.Width,
                Height = this.Height,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            ScrollContainer.SizeChanged += OnScrollContainerSizeChanged;
            ScrollContainer.Paint += OnScrollContainerPaint;
        }

        public void InitializeItems()
        {
            if (Items != null)
            {
                for (int i = 0; i < Items.Count(); i++)
                {
                    if (Items[i] != null)
                    {
                        Items[i].Dispose();
                        Items[i] = null;
                    }
                }

                Items = new InfiniumContractorItem[0];
            }

            iCurPosY = 0;
            Offset = 0;
            ScrollContainer.Top = -Offset;
            VerticalScroll.Offset = Offset;
            VerticalScroll.Refresh();

            if (ItemsDataTable == null)
                return;

            if (ItemsDataTable.DefaultView.Count > 0)
            {
                Items = new InfiniumContractorItem[ItemsDataTable.DefaultView.Count];

                for (int i = 0; i < ItemsDataTable.DefaultView.Count; i++)
                {
                    iCurPosY += iMarginToNextItem;
                    Items[i] = new InfiniumContractorItem()
                    {
                        Parent = ScrollContainer,
                        Left = iMarginToNextItem,
                        Width = ScrollContainer.Width - iMarginToNextItem - iMarginToNextItem,

                        bCanEdit = bCanEdit,
                        Top = iCurPosY
                    };
                    Items[i].brItemColorBrush.Color = ItemColor;
                    Items[i].sName = ItemsDataTable.DefaultView[i]["Name"].ToString();
                    Items[i].sEmail = ItemsDataTable.DefaultView[i]["Email"].ToString();
                    Items[i].sWebsite = ItemsDataTable.DefaultView[i]["Website"].ToString();
                    Items[i].sAddress = ItemsDataTable.DefaultView[i]["Address"].ToString();
                    Items[i].sFacebook = ItemsDataTable.DefaultView[i]["Facebook"].ToString();
                    Items[i].sSkype = ItemsDataTable.DefaultView[i]["Skype"].ToString();
                    Items[i].sUNN = ItemsDataTable.DefaultView[i]["UNN"].ToString();
                    Items[i].sDescription = ItemsDataTable.DefaultView[i]["Description"].ToString();

                    Items[i].sCountry = CountriesDataTable.Select("CountryID = " + ItemsDataTable.DefaultView[i]["CountryID"])[0]["Name"].ToString() +
                                        ", " + CitiesDataTable.Select("CityID = " + ItemsDataTable.DefaultView[i]["CityID"])[0]["Name"].ToString();

                    Items[i].iItemIndex = i;
                    Items[i].iContractorID = Convert.ToInt32(ItemsDataTable.DefaultView[i]["ContractorID"]);

                    //contacts
                    Items[i].ContactsDataTable = new DataTable();
                    Items[i].ContactsDataTable = ContactsDataTable.Clone();

                    foreach (DataRow cRow in ContactsDataTable.Select("ContractorID = " + Items[i].iContractorID))
                    {
                        Items[i].ContactsDataTable.Rows.Add(cRow.ItemArray);
                    }

                    Items[i].Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

                    Items[i].ItemSizeChanged += OnItemSizeChanged;
                    Items[i].EditClicked += OnEditClicked;
                    Items[i].DeleteClicked += OnDeleteClicked;
                    Items[i].ReadDescriptionClicked += OnReadDescriptionClicked;


                    Items[i].Initialize();

                    iCurPosY += Items[i].Height;
                }
            }

            iCurPosY += iMarginToNextItem;

            if (iCurPosY > this.Height)
                ScrollContainer.Height = iCurPosY;
            else
                ScrollContainer.Height = this.Height;

            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
            VerticalScroll.Refresh();

            ScrollContainer.Refresh();
        }


        public void SetHeight(int ItemIndex, int iHeight)
        {
            for (int i = ItemIndex + 1; i < Items.Count(); i++)
                Items[i].Top += iHeight;


            int iH = 0;

            iH += iMarginToNextItem;

            foreach (InfiniumContractorItem Item in Items)
            {
                iH += Item.Height;
                iH += iMarginToNextItem;
            }

            if (this.Height >= iH)
            {
                ScrollContainer.Height = this.Height;
                Offset = 0;
                iTempScrollWheelOffset = 0;
                ScrollContainer.Top = 0;
            }
            else
                ScrollContainer.Height = iH;

            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
            VerticalScroll.Refresh();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            VerticalScroll.Left = this.Width - VerticalScroll.Width;

            if (iCurPosY > this.Height)
                ScrollContainer.Height = iCurPosY;
            else
                ScrollContainer.Height = this.Height;

            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
            VerticalScroll.Refresh();

            ScrollContainer.Refresh();
        }

        private void OnScrollContainerPaint(object sender, PaintEventArgs e)
        {
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            if (Items == null || (Items != null && Items.Count() == 0))
                e.Graphics.DrawString("Нет контрагентов", fCaptionFont, brCaptionBrush,
                                        (this.Width - e.Graphics.MeasureString("Нет обновлений", fCaptionFont).Width) / 2,
                                        (this.Height - e.Graphics.MeasureString("Нет обновлений", fCaptionFont).Height) / 2);
        }

        private void OnScrollContainerSizeChanged(object sender, EventArgs e)
        {
            VerticalScroll.TotalControlHeight = ScrollContainer.Height;
        }

        private void OnScrollPositionChanged(object sender, int tOffset)
        {
            Offset = tOffset;
            ScrollContainer.Top = -Offset;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (!VerticalScroll.Visible)
                return;

            if (Offset == 0)
                if (iTempScrollWheelOffset > 0)
                    iTempScrollWheelOffset = 0;

            if (e.Delta < 0)
            {
                if (Offset < ScrollContainer.Height - this.Height)
                {
                    if (Offset + VerticalScroll.ScrollWheelOffset + this.Height > ScrollContainer.Height)//last
                    {
                        iTempScrollWheelOffset = ScrollContainer.Height - this.Height - Offset;
                        Offset += iTempScrollWheelOffset;
                    }
                    else
                    {
                        Offset += VerticalScroll.ScrollWheelOffset;
                    }

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
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
                        Offset -= VerticalScroll.ScrollWheelOffset;

                    if (Offset < 0)
                        Offset = 0;

                    ScrollContainer.Top = -Offset;
                    VerticalScroll.Offset = Offset;
                    VerticalScroll.Refresh();
                }
        }

        private void OnItemSizeChanged(object sender, int ItemIndex, int Height)
        {
            SetHeight(ItemIndex, Height);
        }


        public event EditEventHandler EditClicked;
        public event EditEventHandler DeleteClicked;
        public event ReadDescriptionEventHandler ReadDescription;

        public delegate void EditEventHandler(object sender, int iContractorID);
        public delegate void ReadDescriptionEventHandler(object sender, string sText);

        public virtual void OnEditClicked(object sender, int iContractorID)
        {
            EditClicked?.Invoke(sender, iContractorID);
        }

        public virtual void OnDeleteClicked(object sender, int iContractorID)
        {
            DeleteClicked?.Invoke(sender, iContractorID);
        }

        public virtual void OnReadDescriptionClicked(object sender, string sText)
        {
            ReadDescription?.Invoke(sender, sText);
        }
    }




    public class InfiniumContractsContactsPanel : Control
    {
        Color cBackColor = Color.FromArgb(249, 249, 249);

        Font fLinkFont = new Font("Segoe UI", 14.0f, FontStyle.Regular, GraphicsUnit.Pixel);
        Font fTextFont = new Font("Segoe UI", 15.0f, FontStyle.Regular, GraphicsUnit.Pixel);
        Font fLabelsFont = new Font("Segoe UI Semibold", 15.0f, FontStyle.Regular, GraphicsUnit.Pixel);

        SolidBrush brNoLinkBrush = new SolidBrush(Color.FromArgb(200, 200, 200));
        SolidBrush brLinkBrush = new SolidBrush(Color.FromArgb(56, 184, 238));
        SolidBrush brColorBrush = new SolidBrush(Color.FromArgb(56, 184, 238));
        SolidBrush brTextBrush = new SolidBrush(Color.FromArgb(80, 80, 80));
        SolidBrush brLabelsBrush = new SolidBrush(Color.FromArgb(70, 70, 70));

        Pen pBorderPen = new Pen(new SolidBrush(Color.FromArgb(210, 210, 210)));

        Rectangle rBorderRect = new Rectangle(0, 0, 0, 0);
        Rectangle rColorRect = new Rectangle(15, 15, 12, 12);

        Bitmap ContactDnBMP = Properties.Resources.ContractorContactsDown;
        Bitmap ContactUpBMP = Properties.Resources.ContractorContactsUp;

        public int iInitialHeight = 35;
        //public int iExpandedHeight = 35;

        int iBorderMargin = 17;
        int iTextMargin = 23;
        int iMarginToNextItem = 15;
        int iItemHeight = 90;

        int iLinkLeft = 0;
        int iLinkWidth = 0;
        int iLinkTop = 0;
        int iLinkHeight = 0;

        public DataTable ContactsDataTable;
        public bool bExpanded = false;

        ToolTip ToolTip;

        string sToolTipText = "";
        string sLink = "Показать контакты";

        public InfiniumContractsContactsPanel()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            this.BackColor = cBackColor;

            ToolTip = new System.Windows.Forms.ToolTip();
            ToolTip.SetToolTip(this, sToolTipText);

            this.Height = iInitialHeight;
            this.Width = 1;
        }

        public string ToolTipText
        {
            get { return sToolTipText; }
            set { sToolTipText = value; ToolTip.SetToolTip(this, value); }
        }

        public void Initialize()
        {
            if (ContactsDataTable == null)
                return;

            if (ContactsDataTable.Rows.Count == 0)
                return;

            using (Graphics G = this.CreateGraphics())
            {
                for (int i = 0; i < ContactsDataTable.Rows.Count; i++)
                {
                    CreateTextBox(G, ContactsDataTable.Rows[i]["Name"].ToString(), (int)G.MeasureString("Имя:", fLabelsFont).Width + 5 + iBorderMargin + iTextMargin,
                                    i * (iItemHeight + iMarginToNextItem) + iBorderMargin + 25,
                                    iBorderMargin + 350 - ((int)G.MeasureString("Имя:", fLabelsFont).Width + 5 + iBorderMargin + iTextMargin) - 10, false);

                    CreateTextBox(G, ContactsDataTable.Rows[i]["Position"].ToString(), (int)G.MeasureString("Должность:", fLabelsFont).Width + 5 + iBorderMargin + iTextMargin,
                                    i * (iItemHeight + iMarginToNextItem) + iBorderMargin + 55,
                                    iBorderMargin + 350 - ((int)G.MeasureString("Должность:", fLabelsFont).Width + 5 + iBorderMargin + iTextMargin) - 10, false);

                    CreateTextBox(G, ContactsDataTable.Rows[i]["Email"].ToString(), (int)G.MeasureString("E-mail:", fLabelsFont).Width + 5 + iBorderMargin + iTextMargin,
                                    i * (iItemHeight + iMarginToNextItem) + iBorderMargin + 85,
                                    iBorderMargin + 350 - ((int)G.MeasureString("E-mail:", fLabelsFont).Width + 5 + iBorderMargin + iTextMargin) - 10, false);



                    CreateTextBox(G, ContactsDataTable.Rows[i]["Phone1"].ToString(), (int)G.MeasureString("Телефон1:", fLabelsFont).Width + 5 + iBorderMargin + 350,
                                    i * (iItemHeight + iMarginToNextItem) + iBorderMargin + 25,
                                    iBorderMargin + 650 - ((int)G.MeasureString("Телефон1:", fLabelsFont).Width + 5 + iBorderMargin + 350) - 10, false);

                    CreateTextBox(G, ContactsDataTable.Rows[i]["Phone2"].ToString(), (int)G.MeasureString("Телефон2:", fLabelsFont).Width + 5 + iBorderMargin + 350,
                                    i * (iItemHeight + iMarginToNextItem) + iBorderMargin + 55,
                                    iBorderMargin + 650 - ((int)G.MeasureString("Телефон2:", fLabelsFont).Width + 5 + iBorderMargin + 350) - 10, false);

                    CreateTextBox(G, ContactsDataTable.Rows[i]["Phone3"].ToString(), (int)G.MeasureString("Телефон3:", fLabelsFont).Width + 5 + iBorderMargin + 350,
                                    i * (iItemHeight + iMarginToNextItem) + iBorderMargin + 85,
                                    iBorderMargin + 650 - ((int)G.MeasureString("Телефон3:", fLabelsFont).Width + 5 + iBorderMargin + 350) - 10, false);



                    CreateTextBox(G, ContactsDataTable.Rows[i]["Website"].ToString(), (int)G.MeasureString("Веб-сайт:", fLabelsFont).Width + 5 + iBorderMargin + 650,
                                   i * (iItemHeight + iMarginToNextItem) + iBorderMargin + 25,
                                   this.Width - iBorderMargin - iTextMargin - iBorderMargin - iTextMargin - ((int)G.MeasureString("Веб-сайт:", fLabelsFont).Width + 5 + iBorderMargin + 650), true);

                    CreateTextBox(G, ContactsDataTable.Rows[i]["Skype"].ToString(), (int)G.MeasureString("Skype:", fLabelsFont).Width + 5 + iBorderMargin + 650,
                                    i * (iItemHeight + iMarginToNextItem) + iBorderMargin + 55,
                                    this.Width - iBorderMargin - iTextMargin - iBorderMargin - iTextMargin - ((int)G.MeasureString("Skype:", fLabelsFont).Width + 5 + iBorderMargin + 650), true);

                    CreateTextBox(G, ContactsDataTable.Rows[i]["Description"].ToString(), (int)G.MeasureString("Описание:", fLabelsFont).Width + 5 + iBorderMargin + 650,
                                    i * (iItemHeight + iMarginToNextItem) + iBorderMargin + 85,
                                    this.Width - iBorderMargin - iTextMargin - iBorderMargin - iTextMargin - ((int)G.MeasureString("Описание:", fLabelsFont).Width + 5 + iBorderMargin + 650), true);
                }
            }
        }

        public void CreateTextBox(Graphics G, string sText, int iLeft, int iTop, int MaxWidth, bool bAnchorExpand)
        {
            InfiniumContractorsTextBox TextBox = new InfiniumContractorsTextBox(cBackColor)
            {
                Font = fTextFont,

                Parent = this,
                Left = iLeft,
                Top = iTop
            };
            if (bAnchorExpand)
            {
                TextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                TextBox.Text = sText;
                TextBox.Width = MaxWidth;

                string s = GetCaption(sText, G, MaxWidth);

                if (s != sText)
                    TextBox.ToolTipText = sText;
            }
            else
                TextBox.Text = GetCaption(sText, G, MaxWidth);

            if (TextBox.Text != sText)
                TextBox.ToolTipText = sText;


        }

        public string GetCaption(string Caption, Graphics G, int iW)
        {
            string sText = "";

            if (G.MeasureString(Caption, fTextFont).Width > iW)
            {
                for (int i = Caption.Length; i >= 0; i--)
                {
                    if (G.MeasureString(Caption.Substring(0, i), fTextFont).Width <= iW)
                    {
                        sText = Caption.Substring(0, i - 1) + "...";
                        break;
                    }
                }
            }
            else
            {
                sText = Caption;
            }

            return sText;
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);


            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            rBorderRect = this.ClientRectangle;
            rBorderRect.Width = this.ClientRectangle.Width - 1;
            rBorderRect.Height = this.ClientRectangle.Height - 1;

            e.Graphics.DrawRectangle(pBorderPen, rBorderRect);


            if (ContactsDataTable != null)
            {
                if (ContactsDataTable.Rows.Count > 0)
                {
                    iLinkLeft = iBorderMargin + iTextMargin;
                    iLinkTop = 7;
                    iLinkWidth = (int)e.Graphics.MeasureString(sLink, fLinkFont).Width;
                    iLinkHeight = (int)e.Graphics.MeasureString(sLink, fLinkFont).Height;

                    e.Graphics.DrawString(sLink, fLinkFont, brLinkBrush, iLinkLeft, 7);

                    if (bExpanded)
                    {
                        ContactUpBMP.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);
                        e.Graphics.DrawImage(ContactUpBMP, iLinkLeft + iLinkWidth + 3, iLinkTop + 5);
                        iLinkWidth += ContactUpBMP.Width + 5;
                    }
                    else
                    {
                        ContactDnBMP.SetResolution(e.Graphics.DpiX, e.Graphics.DpiY);
                        e.Graphics.DrawImage(ContactDnBMP, iLinkLeft + iLinkWidth + 3, iLinkTop + 5);
                        iLinkWidth += ContactDnBMP.Width + 3;
                    }
                }
                else
                    e.Graphics.DrawString("Нет контактов", fLinkFont, brNoLinkBrush, iBorderMargin + iTextMargin, 7);
            }
            else
                e.Graphics.DrawString(sLink, fLinkFont, brNoLinkBrush, iBorderMargin + iTextMargin, 7);

            if (bExpanded)
            {
                for (int i = 0; i < ContactsDataTable.Rows.Count; i++)
                {
                    rColorRect.Y = i * (iItemHeight + iMarginToNextItem) + iBorderMargin + 29;

                    e.Graphics.FillRectangle(brColorBrush, rColorRect);

                    e.Graphics.DrawString("Имя:", fLabelsFont, brLabelsBrush, iBorderMargin + iTextMargin,
                                                    i * (iItemHeight + iMarginToNextItem) + iBorderMargin + 25);

                    e.Graphics.DrawString("Должность:", fLabelsFont, brLabelsBrush, iBorderMargin + iTextMargin,
                                                    i * (iItemHeight + iMarginToNextItem) + iBorderMargin + 55);

                    e.Graphics.DrawString("E-mail:", fLabelsFont, brLabelsBrush, iBorderMargin + iTextMargin,
                                                    i * (iItemHeight + iMarginToNextItem) + iBorderMargin + 85);



                    e.Graphics.DrawString("Телефон 1:", fLabelsFont, brLabelsBrush, iBorderMargin + 350,
                                                    i * (iItemHeight + iMarginToNextItem) + iBorderMargin + 25);

                    e.Graphics.DrawString("Телефон 2:", fLabelsFont, brLabelsBrush, iBorderMargin + 350,
                                                    i * (iItemHeight + iMarginToNextItem) + iBorderMargin + 55);

                    e.Graphics.DrawString("Телефон 3:", fLabelsFont, brLabelsBrush, iBorderMargin + 350,
                                                    i * (iItemHeight + iMarginToNextItem) + iBorderMargin + 85);



                    e.Graphics.DrawString("Веб-сайт:", fLabelsFont, brLabelsBrush, iBorderMargin + 650,
                                                    i * (iItemHeight + iMarginToNextItem) + iBorderMargin + 25);

                    e.Graphics.DrawString("Skype:", fLabelsFont, brLabelsBrush, iBorderMargin + 650,
                                                    i * (iItemHeight + iMarginToNextItem) + iBorderMargin + 55);

                    e.Graphics.DrawString("Описание:", fLabelsFont, brLabelsBrush, iBorderMargin + 650,
                                                    i * (iItemHeight + iMarginToNextItem) + iBorderMargin + 85);
                }
            }
        }


        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (iLinkWidth > 0)
            {
                if (e.X >= iLinkLeft && e.X <= iLinkLeft + iLinkWidth && e.Y >= iLinkTop && e.Y <= iLinkTop + iLinkHeight)
                {
                    if (this.Cursor != Cursors.Hand)
                        this.Cursor = Cursors.Hand;
                }
                else
                {
                    if (this.Cursor == Cursors.Hand)
                        this.Cursor = Cursors.Default;
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (iLinkWidth > 0)
            {
                if (e.X >= iLinkLeft && e.X <= iLinkLeft + iLinkWidth && e.Y >= iLinkTop && e.Y <= iLinkTop + iLinkHeight)
                {
                    if (!bExpanded)
                    {
                        this.Height += ContactsDataTable.Rows.Count * (iItemHeight + iMarginToNextItem) + iBorderMargin;
                        sLink = "Скрыть контакты";
                        bExpanded = true;
                        this.Refresh();
                        OnControlPanelSizeChanged(this, this.Height - iInitialHeight);
                    }
                    else
                    {
                        sLink = "Показать контакты";
                        bExpanded = false;
                        this.Refresh();
                        OnControlPanelSizeChanged(this, iInitialHeight - this.Height);
                        this.Height = iInitialHeight;
                    }
                }
            }
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            Parent.Focus();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);


        }

        private void OnEditCommentClick(object sender, int iDocumentCommentID, string Text)
        {
            OnEditCommentClicked(this, iDocumentCommentID);
        }

        private void OnDeleteCommentClick(object sender, int iDocumentCommentID, string Text)
        {
            OnDeleteCommentClicked(this, iDocumentCommentID);
        }



        public event CommentEditEventHandler EditCommentClicked;
        public event CommentEditEventHandler DeleteCommentClicked;
        public event SizeChangedEventHandler ControlPanelSizeChanged;

        public delegate void SizeChangedEventHandler(object sender, int Height);
        public delegate void CommentEditEventHandler(object sender, int DocumentCommentID);

        public virtual void OnControlPanelSizeChanged(object sender, int Height)
        {
            ControlPanelSizeChanged?.Invoke(sender, Height);
        }

        public virtual void OnEditCommentClicked(object sender, int iDocumentCommentID)
        {
            EditCommentClicked?.Invoke(this, iDocumentCommentID);//Raise the event
        }

        public virtual void OnDeleteCommentClicked(object sender, int iDocumentCommentID)
        {
            DeleteCommentClicked?.Invoke(this, iDocumentCommentID);//Raise the event
        }

    }
}
