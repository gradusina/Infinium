namespace Infinium
{
    partial class ComplexSawingForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AnimateTimer = new System.Windows.Forms.Timer(this.components);
            this.OkSawingButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.StandardButtonsPalette = new ComponentFactory.Krypton.Toolkit.KryptonPalette(this.components);
            this.CancelSawingButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.panel56 = new System.Windows.Forms.Panel();
            this.dgvComplexSawing = new Infinium.PercentageDataGrid();
            this.kryptonBorderEdge123 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.kryptonBorderEdge124 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.kryptonBorderEdge125 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.kryptonBorderEdge126 = new ComponentFactory.Krypton.Toolkit.KryptonBorderEdge();
            this.MainPanel = new System.Windows.Forms.Panel();
            this.panel56.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvComplexSawing)).BeginInit();
            this.MainPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // AnimateTimer
            // 
            this.AnimateTimer.Interval = 1;
            this.AnimateTimer.Tick += new System.EventHandler(this.AnimateTimer_Tick);
            // 
            // OkSawingButton
            // 
            this.OkSawingButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OkSawingButton.Location = new System.Drawing.Point(455, 321);
            this.OkSawingButton.Name = "OkSawingButton";
            this.OkSawingButton.Palette = this.StandardButtonsPalette;
            this.OkSawingButton.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.OkSawingButton.Size = new System.Drawing.Size(130, 40);
            this.OkSawingButton.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.OkSawingButton.StateCommon.Content.Image.ImageH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.OkSawingButton.StateCommon.Content.Image.ImageV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Near;
            this.OkSawingButton.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.OkSawingButton.StateCommon.Content.ShortText.TextV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.OkSawingButton.StateTracking.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(164)))), ((int)(((byte)(61)))));
            this.OkSawingButton.StateTracking.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.OkSawingButton.TabIndex = 3;
            this.OkSawingButton.TabStop = false;
            this.OkSawingButton.Values.Text = "ОК";
            this.OkSawingButton.Click += new System.EventHandler(this.OkSawingButton_Click);
            // 
            // StandardButtonsPalette
            // 
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.OverrideDefault.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(177)))), ((int)(((byte)(229)))));
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.OverrideDefault.Back.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(167)))), ((int)(((byte)(214)))));
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.OverrideDefault.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.OverrideDefault.Border.Color1 = System.Drawing.Color.Transparent;
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.OverrideDefault.Border.Color2 = System.Drawing.Color.Gray;
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.OverrideDefault.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.OverrideDefault.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.OverrideDefault.Border.Rounding = 0;
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(177)))), ((int)(((byte)(229)))));
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Back.Color2 = System.Drawing.Color.White;
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Border.Color1 = System.Drawing.Color.Transparent;
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Border.Color2 = System.Drawing.Color.Black;
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Border.Rounding = 0;
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Content.ShortText.Color1 = System.Drawing.Color.White;
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Content.ShortText.Color2 = System.Drawing.Color.White;
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Content.ShortText.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Content.ShortText.MultiLine = ComponentFactory.Krypton.Toolkit.InheritBool.True;
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Content.ShortText.MultiLineH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.StateCommon.Content.ShortText.TextV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Far;
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.StateTracking.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(164)))), ((int)(((byte)(61)))));
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.StateTracking.Back.Color2 = System.Drawing.Color.White;
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.StateTracking.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.StateTracking.Border.Color1 = System.Drawing.Color.Transparent;
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.StateTracking.Border.Color2 = System.Drawing.Color.Black;
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.StateTracking.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.StateTracking.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.StandardButtonsPalette.ButtonStyles.ButtonCommon.StateTracking.Border.Rounding = 0;
            // 
            // CancelSawingButton
            // 
            this.CancelSawingButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelSawingButton.Location = new System.Drawing.Point(619, 321);
            this.CancelSawingButton.Name = "CancelSawingButton";
            this.CancelSawingButton.Palette = this.StandardButtonsPalette;
            this.CancelSawingButton.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Custom;
            this.CancelSawingButton.Size = new System.Drawing.Size(130, 40);
            this.CancelSawingButton.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.CancelSawingButton.StateCommon.Content.Image.ImageH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.CancelSawingButton.StateCommon.Content.Image.ImageV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Near;
            this.CancelSawingButton.StateCommon.Content.ShortText.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.CancelSawingButton.StateCommon.Content.ShortText.TextV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
            this.CancelSawingButton.StateTracking.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(164)))), ((int)(((byte)(61)))));
            this.CancelSawingButton.StateTracking.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.CancelSawingButton.TabIndex = 99;
            this.CancelSawingButton.TabStop = false;
            this.CancelSawingButton.Values.Text = "Отмена";
            this.CancelSawingButton.Click += new System.EventHandler(this.CancelSawingButton_Click);
            // 
            // panel56
            // 
            this.panel56.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel56.Controls.Add(this.dgvComplexSawing);
            this.panel56.Controls.Add(this.kryptonBorderEdge123);
            this.panel56.Controls.Add(this.kryptonBorderEdge124);
            this.panel56.Controls.Add(this.kryptonBorderEdge125);
            this.panel56.Controls.Add(this.kryptonBorderEdge126);
            this.panel56.Location = new System.Drawing.Point(0, 0);
            this.panel56.Name = "panel56";
            this.panel56.Size = new System.Drawing.Size(765, 311);
            this.panel56.TabIndex = 502;
            // 
            // dgvComplexSawing
            // 
            this.dgvComplexSawing.AllowUserToDeleteRows = false;
            this.dgvComplexSawing.AllowUserToResizeColumns = false;
            this.dgvComplexSawing.AllowUserToResizeRows = false;
            this.dgvComplexSawing.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvComplexSawing.BackText = "Нет данных";
            this.dgvComplexSawing.ColumnHeadersHeight = 40;
            this.dgvComplexSawing.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvComplexSawing.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvComplexSawing.HideOuterBorders = true;
            this.dgvComplexSawing.Location = new System.Drawing.Point(1, 1);
            this.dgvComplexSawing.MultiSelect = false;
            this.dgvComplexSawing.Name = "dgvComplexSawing";
            this.dgvComplexSawing.PercentLineWidth = 0;
            this.dgvComplexSawing.RowHeadersVisible = false;
            this.dgvComplexSawing.RowTemplate.Height = 30;
            this.dgvComplexSawing.SelectedColorStyle = Infinium.PercentageDataGrid.ColorStyle.Blue;
            this.dgvComplexSawing.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvComplexSawing.Size = new System.Drawing.Size(763, 309);
            this.dgvComplexSawing.StandardStyle = false;
            this.dgvComplexSawing.StateCommon.Background.Color1 = System.Drawing.Color.White;
            this.dgvComplexSawing.StateCommon.Background.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.dgvComplexSawing.StateCommon.BackStyle = ComponentFactory.Krypton.Toolkit.PaletteBackStyle.GridBackgroundList;
            this.dgvComplexSawing.StateCommon.DataCell.Back.Color1 = System.Drawing.Color.White;
            this.dgvComplexSawing.StateCommon.DataCell.Border.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(210)))));
            this.dgvComplexSawing.StateCommon.DataCell.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.dgvComplexSawing.StateCommon.DataCell.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.dgvComplexSawing.StateCommon.DataCell.Content.Color1 = System.Drawing.Color.Black;
            this.dgvComplexSawing.StateCommon.DataCell.Content.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.dgvComplexSawing.StateCommon.HeaderColumn.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.dgvComplexSawing.StateCommon.HeaderColumn.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.dgvComplexSawing.StateCommon.HeaderColumn.Border.Color1 = System.Drawing.Color.Black;
            this.dgvComplexSawing.StateCommon.HeaderColumn.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.dgvComplexSawing.StateCommon.HeaderColumn.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)(((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.dgvComplexSawing.StateCommon.HeaderColumn.Content.Color1 = System.Drawing.Color.White;
            this.dgvComplexSawing.StateCommon.HeaderColumn.Content.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.dgvComplexSawing.StateDisabled.DataCell.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(210)))));
            this.dgvComplexSawing.StateDisabled.DataCell.Border.Color1 = System.Drawing.Color.DimGray;
            this.dgvComplexSawing.StateDisabled.DataCell.Border.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.dgvComplexSawing.StateDisabled.DataCell.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.dgvComplexSawing.StateDisabled.DataCell.Content.Color1 = System.Drawing.Color.Gray;
            this.dgvComplexSawing.StateSelected.DataCell.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(177)))), ((int)(((byte)(229)))));
            this.dgvComplexSawing.StateSelected.DataCell.Back.ColorStyle = ComponentFactory.Krypton.Toolkit.PaletteColorStyle.Solid;
            this.dgvComplexSawing.StateSelected.DataCell.Content.Color1 = System.Drawing.Color.White;
            this.dgvComplexSawing.TabIndex = 73;
            this.dgvComplexSawing.UseCustomBackColor = true;
            // 
            // kryptonBorderEdge123
            // 
            this.kryptonBorderEdge123.Dock = System.Windows.Forms.DockStyle.Top;
            this.kryptonBorderEdge123.Location = new System.Drawing.Point(1, 0);
            this.kryptonBorderEdge123.Name = "kryptonBorderEdge123";
            this.kryptonBorderEdge123.Size = new System.Drawing.Size(763, 1);
            this.kryptonBorderEdge123.StateCommon.Color1 = System.Drawing.Color.Black;
            this.kryptonBorderEdge123.Text = "kryptonBorderEdge123";
            // 
            // kryptonBorderEdge124
            // 
            this.kryptonBorderEdge124.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.kryptonBorderEdge124.Location = new System.Drawing.Point(1, 310);
            this.kryptonBorderEdge124.Name = "kryptonBorderEdge124";
            this.kryptonBorderEdge124.Size = new System.Drawing.Size(763, 1);
            this.kryptonBorderEdge124.StateCommon.Color1 = System.Drawing.Color.Black;
            this.kryptonBorderEdge124.Text = "kryptonBorderEdge124";
            // 
            // kryptonBorderEdge125
            // 
            this.kryptonBorderEdge125.Dock = System.Windows.Forms.DockStyle.Right;
            this.kryptonBorderEdge125.Location = new System.Drawing.Point(764, 0);
            this.kryptonBorderEdge125.Name = "kryptonBorderEdge125";
            this.kryptonBorderEdge125.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.kryptonBorderEdge125.Size = new System.Drawing.Size(1, 311);
            this.kryptonBorderEdge125.StateCommon.Color1 = System.Drawing.Color.Black;
            this.kryptonBorderEdge125.Text = "kryptonBorderEdge125";
            // 
            // kryptonBorderEdge126
            // 
            this.kryptonBorderEdge126.Dock = System.Windows.Forms.DockStyle.Left;
            this.kryptonBorderEdge126.Location = new System.Drawing.Point(0, 0);
            this.kryptonBorderEdge126.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.kryptonBorderEdge126.Name = "kryptonBorderEdge126";
            this.kryptonBorderEdge126.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.kryptonBorderEdge126.Size = new System.Drawing.Size(1, 311);
            this.kryptonBorderEdge126.StateCommon.Color1 = System.Drawing.Color.Black;
            this.kryptonBorderEdge126.Text = "kryptonBorderEdge126";
            // 
            // MainPanel
            // 
            this.MainPanel.BackColor = System.Drawing.Color.White;
            this.MainPanel.Controls.Add(this.panel56);
            this.MainPanel.Controls.Add(this.OkSawingButton);
            this.MainPanel.Controls.Add(this.CancelSawingButton);
            this.MainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainPanel.Location = new System.Drawing.Point(0, 0);
            this.MainPanel.Name = "MainPanel";
            this.MainPanel.Size = new System.Drawing.Size(765, 372);
            this.MainPanel.TabIndex = 87;
            // 
            // ComplexSawingForm
            // 
            this.AccessibleName = "false";
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(765, 372);
            this.Controls.Add(this.MainPanel);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ComplexSawingForm";
            this.Opacity = 0D;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Выбор";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ComplexSawingForm_FormClosing);
            this.Shown += new System.EventHandler(this.ComplexSawingForm_Shown);
            this.panel56.ResumeLayout(false);
            this.panel56.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvComplexSawing)).EndInit();
            this.MainPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer AnimateTimer;
        private ComponentFactory.Krypton.Toolkit.KryptonButton OkSawingButton;
        private ComponentFactory.Krypton.Toolkit.KryptonButton CancelSawingButton;
        private ComponentFactory.Krypton.Toolkit.KryptonPalette StandardButtonsPalette;
        private System.Windows.Forms.Panel panel56;
        private PercentageDataGrid dgvComplexSawing;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge123;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge124;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge125;
        private ComponentFactory.Krypton.Toolkit.KryptonBorderEdge kryptonBorderEdge126;
        private System.Windows.Forms.Panel MainPanel;
    }
}