using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Infinium
{
    public partial class CatalogFinishedProductsForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        bool NeedSplash = false;

        int FormEvent = 0;

        Form TopForm = null;
        LightStartForm LightStartForm;

        FinishedImagesCatalog FinishedImagesCatalogManager;

        public CatalogFinishedProductsForm(LightStartForm tLightStartForm)
        {
            InitializeComponent();
            LightStartForm = tLightStartForm;

            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            Initialize();
            while (!SplashForm.bCreated) ;
        }

        private void CatalogFinishedProductsForm_Shown(object sender, EventArgs e)
        {
            while (!SplashForm.bCreated) ;
            FormEvent = eShow;
            AnimateTimer.Enabled = true;
            NeedSplash = true;
        }

        private void AnimateTimer_Tick(object sender, EventArgs e)
        {
            if (!DatabaseConfigsManager.Animation)
            {
                this.Opacity = 1;

                if (FormEvent == eClose || FormEvent == eHide)
                {
                    AnimateTimer.Enabled = false;

                    if (FormEvent == eClose)
                    {
                        LightStartForm.CloseForm(this);
                        this.Close();
                    }

                    if (FormEvent == eHide)
                    {
                        LightStartForm.HideForm(this);
                    }


                    return;
                }

                if (FormEvent == eShow)
                {
                    AnimateTimer.Enabled = false;
                    SplashForm.CloseS = true;
                    NeedSplash = true;
                    return;
                }

            }

            if (FormEvent == eClose || FormEvent == eHide)
            {
                if (Convert.ToDecimal(this.Opacity) != Convert.ToDecimal(0.00))
                    this.Opacity = Convert.ToDouble(Convert.ToDecimal(this.Opacity) - Convert.ToDecimal(0.05));
                else
                {
                    AnimateTimer.Enabled = false;

                    if (FormEvent == eClose)
                    {
                        LightStartForm.CloseForm(this);
                        this.Close();
                    }

                    if (FormEvent == eHide)
                    {
                        LightStartForm.HideForm(this);
                    }

                }

                return;
            }


            if (FormEvent == eShow || FormEvent == eShow)
            {
                if (this.Opacity != 1)
                    this.Opacity += 0.05;
                else
                {
                    AnimateTimer.Enabled = false;
                    SplashForm.CloseS = true;
                    NeedSplash = true;
                }

                return;
            }
        }

        private void NavigateMenuCloseButton_Click(object sender, EventArgs e)
        {
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void Initialize()
        {
            FinishedImagesCatalogManager = new FinishedImagesCatalog();
            FinishedImagesCatalogManager.UpdateImages();

            dgvImages.DataSource = FinishedImagesCatalogManager.ClientsCatalogImagesBS;
            dgvImagesSetting(ref dgvImages);
        }

        private void dgvImagesSetting(ref PercentageDataGrid grid)
        {
            grid.AutoGenerateColumns = false;

            foreach (DataGridViewColumn Column in grid.Columns)
            {
                Column.ReadOnly = false;
                Column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            grid.Columns["ImageID"].ReadOnly = true;
            if (grid.Columns.Contains("FileSize"))
                grid.Columns["FileSize"].Visible = false;
            if (grid.Columns.Contains("ProductType"))
                grid.Columns["ProductType"].Visible = false;
            if (grid.Columns.Contains("ConfigID"))
                grid.Columns["ConfigID"].Visible = false;
            if (grid.Columns.Contains("Description"))
                grid.Columns["Description"].Visible = false;
            if (grid.Columns.Contains("Sizes"))
                grid.Columns["Sizes"].Visible = false;
            if (grid.Columns.Contains("Material"))
                grid.Columns["Material"].Visible = false;
            if (grid.Columns.Contains("ToSite"))
                grid.Columns["ToSite"].Visible = false;
            if (grid.Columns.Contains("CatSlider"))
                grid.Columns["CatSlider"].Visible = false;
            if (grid.Columns.Contains("MainSlider"))
                grid.Columns["MainSlider"].Visible = false;
            if (grid.Columns.Contains("Color"))
                grid.Columns["Color"].Visible = false;
            if (grid.Columns.Contains("Name"))
                grid.Columns["Name"].Visible = false;
            if (grid.Columns.Contains("Category"))
                grid.Columns["Category"].Visible = false;

            grid.Columns["FileName"].HeaderText = "Имя файла";

            //int DisplayIndex = 0;
            //grid.Columns["TechStoreNameColumn"].DisplayIndex = DisplayIndex++;
            //grid.Columns["ColorColumn"].DisplayIndex = DisplayIndex++;
            //grid.Columns["PatinaColumn"].DisplayIndex = DisplayIndex++;
            //grid.Columns["Count"].DisplayIndex = DisplayIndex++;
            //grid.Columns["MeasuresColumn"].DisplayIndex = DisplayIndex++;

        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 6 && m.WParam.ToInt32() == 1)
            {
                if (TopForm != null)
                    TopForm.Activate();
            }
        }

        private void openFileDialog4_FileOk(object sender, CancelEventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            NeedSplash = false;
            int ImageID = 0;
            if (dgvImages.SelectedRows.Count > 0 && dgvImages.SelectedRows[0].Cells["ImageID"].Value != DBNull.Value)
                ImageID = Convert.ToInt32(dgvImages.SelectedRows[0].Cells["ImageID"].Value);

            var fileInfo = new System.IO.FileInfo(openFileDialog4.FileName);

            string sFileName = System.IO.Path.GetFileNameWithoutExtension(openFileDialog4.FileName);
            string sExtension = fileInfo.Extension;
            string sPath = openFileDialog4.FileName;
            FinishedImagesCatalogManager.AttachImage(ImageID, sFileName, sExtension, sPath, fileInfo.Length);
            FinishedImagesCatalogManager.UpdateImages();
            FinishedImagesCatalogManager.MoveToImage(ImageID);

            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void btnSaveImages_Click(object sender, EventArgs e)
        {
            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;

            NeedSplash = false;
            int ImageID = 0;
            if (dgvImages.SelectedRows.Count > 0 && dgvImages.SelectedRows[0].Cells["ImageID"].Value != DBNull.Value)
                ImageID = Convert.ToInt32(dgvImages.SelectedRows[0].Cells["ImageID"].Value);
            if (ImageID != 0)
            {
                FinishedImagesCatalogManager.EditImageRowBeforeSaving(ImageID, cbToSite.Checked, cbCatSlider.Checked, cbMainSlider.Checked,
                    kryptonRichTextBox6.Text, kryptonRichTextBox5.Text, kryptonRichTextBox4.Text,
                    kryptonRichTextBox1.Text, kryptonRichTextBox2.Text, kryptonRichTextBox3.Text);
            }
            FinishedImagesCatalogManager.SaveImages();
            FinishedImagesCatalogManager.UpdateImages();
            FinishedImagesCatalogManager.MoveToImage(ImageID);

            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void btnDeleteImage_Click(object sender, EventArgs e)
        {
            int ImageID = 0;
            if (dgvImages.SelectedRows.Count > 0 && dgvImages.SelectedRows[0].Cells["ImageID"].Value != DBNull.Value)
                ImageID = Convert.ToInt32(dgvImages.SelectedRows[0].Cells["ImageID"].Value);

            if (ImageID == 0)
            {
                return;
            }
            bool OKCancel = Infinium.LightMessageBox.Show(ref TopForm, true,
                      "Вы уверены, что хотите удалить?",
                      "Удаление");

            if (!OKCancel)
                return;

            Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
            T.Start();

            while (!SplashWindow.bSmallCreated) ;
            NeedSplash = false;
            FinishedImagesCatalogManager.DeleteImageRow(ImageID);
            FinishedImagesCatalogManager.SaveImages();
            FinishedImagesCatalogManager.UpdateImages();

            NeedSplash = true;
            while (SplashWindow.bSmallCreated)
                SmallWaitForm.CloseS = true;
        }

        private void btnAddImage_Click(object sender, EventArgs e)
        {
            FinishedImagesCatalogManager.NewImageRow();
            FinishedImagesCatalogManager.SaveImages();
            FinishedImagesCatalogManager.UpdateImages();
        }

        private void dgvImages_SelectionChanged(object sender, EventArgs e)
        {
            int ImageID = 0;
            if (dgvImages.SelectedRows.Count > 0 && dgvImages.SelectedRows[0].Cells["ImageID"].Value != DBNull.Value)
                ImageID = Convert.ToInt32(dgvImages.SelectedRows[0].Cells["ImageID"].Value);
            kryptonRichTextBox1.Text = string.Empty;
            kryptonRichTextBox2.Text = string.Empty;
            kryptonRichTextBox3.Text = string.Empty;
            cbToSite.Checked = false;
            if (NeedSplash)
            {
                Thread T = new Thread(delegate () { SplashWindow.CreateSmallSplash(ref TopForm, "Загрузка данных с сервера.\r\nПодождите..."); });
                T.Start();

                while (!SplashWindow.bSmallCreated) ;

                pcbxImage.Image = null;
                Image FinishedProductImage = FinishedImagesCatalogManager.GetImage(ImageID);
                if (FinishedProductImage != null)
                {
                    pcbxImage.Image = FinishedProductImage;
                    if (pcbxImage.Image == null)
                        pcbxImage.Cursor = Cursors.Default;
                    else
                        pcbxImage.Cursor = Cursors.Hand;
                }

                if (ImageID != 0)
                {
                    kryptonRichTextBox6.Text = dgvImages.SelectedRows[0].Cells["Category"].Value.ToString();
                    kryptonRichTextBox5.Text = dgvImages.SelectedRows[0].Cells["Name"].Value.ToString();
                    kryptonRichTextBox4.Text = dgvImages.SelectedRows[0].Cells["Color"].Value.ToString();
                    kryptonRichTextBox1.Text = dgvImages.SelectedRows[0].Cells["Description"].Value.ToString();
                    kryptonRichTextBox2.Text = dgvImages.SelectedRows[0].Cells["Sizes"].Value.ToString();
                    kryptonRichTextBox3.Text = dgvImages.SelectedRows[0].Cells["Material"].Value.ToString();
                    cbToSite.Checked = Convert.ToBoolean(dgvImages.SelectedRows[0].Cells["ToSite"].Value);
                    cbCatSlider.Checked = Convert.ToBoolean(dgvImages.SelectedRows[0].Cells["CatSlider"].Value);
                    cbMainSlider.Checked = Convert.ToBoolean(dgvImages.SelectedRows[0].Cells["MainSlider"].Value);
                }
                while (SplashWindow.bSmallCreated)
                    SmallWaitForm.CloseS = true;
            }
            else
            {
                pcbxImage.Image = null;
                Image FinishedProductImage = FinishedImagesCatalogManager.GetImage(ImageID);
                if (FinishedProductImage != null)
                {
                    pcbxImage.Image = FinishedProductImage;
                    if (pcbxImage.Image == null)
                        pcbxImage.Cursor = Cursors.Default;
                    else
                        pcbxImage.Cursor = Cursors.Hand;
                }
                if (ImageID != 0)
                {
                    kryptonRichTextBox6.Text = dgvImages.SelectedRows[0].Cells["Category"].Value.ToString();
                    kryptonRichTextBox5.Text = dgvImages.SelectedRows[0].Cells["Name"].Value.ToString();
                    kryptonRichTextBox4.Text = dgvImages.SelectedRows[0].Cells["Color"].Value.ToString();
                    kryptonRichTextBox1.Text = dgvImages.SelectedRows[0].Cells["Description"].Value.ToString();
                    kryptonRichTextBox2.Text = dgvImages.SelectedRows[0].Cells["Sizes"].Value.ToString();
                    kryptonRichTextBox3.Text = dgvImages.SelectedRows[0].Cells["Material"].Value.ToString();
                    cbToSite.Checked = Convert.ToBoolean(dgvImages.SelectedRows[0].Cells["ToSite"].Value);
                    cbCatSlider.Checked = Convert.ToBoolean(dgvImages.SelectedRows[0].Cells["CatSlider"].Value);
                    cbMainSlider.Checked = Convert.ToBoolean(dgvImages.SelectedRows[0].Cells["MainSlider"].Value);
                }
            }
        }

        private void pcbxImage_Click(object sender, EventArgs e)
        {
            if (pcbxImage.Image == null)
                return;

            PhantomForm PhantomForm = new PhantomForm();
            PhantomForm.Show();

            ZoomImageForm ZoomImageForm = new ZoomImageForm(pcbxImage.Image, ref TopForm);

            TopForm = ZoomImageForm;

            ZoomImageForm.ShowDialog();

            PhantomForm.Close();
            PhantomForm.Dispose();

            TopForm = null;

            ZoomImageForm.Dispose();
        }

        private void btnAddFoto_Click(object sender, EventArgs e)
        {
            int ImageID = 0;
            if (dgvImages.SelectedRows.Count > 0 && dgvImages.SelectedRows[0].Cells["ImageID"].Value != DBNull.Value)
                ImageID = Convert.ToInt32(dgvImages.SelectedRows[0].Cells["ImageID"].Value);

            if (ImageID == 0)
            {
                MessageBox.Show("Фото не может быть прикреплено");
                return;
            }
            openFileDialog4.ShowDialog();
        }
    }
}
