using Infinium.Modules.TechnologyCatalog;

using System;
using System.Data;
using System.Windows.Forms;

namespace Infinium
{
    public partial class AddOperationsTermsForm : Form
    {
        const int eHide = 2;
        const int eShow = 1;
        const int eClose = 3;
        const int eMainMenu = 4;

        bool DecimalParameter = true;
        int FormEvent = 0;
        int TechCatalogOperationsDetailID = 0;
        string TechCatalogOperationName = string.Empty;
        string Term = string.Empty;

        Form MainForm = null;
        Form TopForm = null;

        TechCatalogOperationsTerms TechCatalogOperationsTermsManager;

        public AddOperationsTermsForm(Form tMainForm, ref TechCatalogOperationsTerms tTechCatalogOperationsTermsManager, int iTechCatalogOperationsDetailID, string sTechCatalogOperationName)
        {
            MainForm = tMainForm;
            TechCatalogOperationsTermsManager = tTechCatalogOperationsTermsManager;
            TechCatalogOperationsDetailID = iTechCatalogOperationsDetailID;
            TechCatalogOperationName = sTechCatalogOperationName;
            InitializeComponent();
        }

        private void CancelOrderButton_Click(object sender, EventArgs e)
        {
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
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
                        this.Close();
                    }

                    if (FormEvent == eHide)
                    {
                        MainForm.Activate();
                        this.Hide();
                    }

                    return;
                }

                if (FormEvent == eShow)
                {
                    AnimateTimer.Enabled = false;
                    SplashForm.CloseS = true;
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
                        this.Close();
                    }

                    if (FormEvent == eHide)
                    {
                        MainForm.Activate();
                        this.Hide();
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
                }

                return;
            }
        }

        private void Initialize()
        {
            TechCatalogOperationsTermsManager.TechCatalogOperationsDetailID = TechCatalogOperationsDetailID;
            TechCatalogOperationsTermsManager.RefreshTerms();

            cmbMathSymbol.DataSource = TechCatalogOperationsTermsManager.MathSymbolsList;
            cmbMathSymbol.ValueMember = "MathSymbol";
            cmbMathSymbol.DisplayMember = "MathSymbol";

            cmbLogicOperations.DataSource = TechCatalogOperationsTermsManager.LogicOperationsList;
            cmbLogicOperations.ValueMember = "LogicOperation";
            cmbLogicOperations.DisplayMember = "LogicOperation";

            cmbParameters.DataSource = TechCatalogOperationsTermsManager.ParametersList;
            cmbParameters.ValueMember = "Parameter";
            cmbParameters.DisplayMember = "Name";

            dgvOperationsTermsSetting();
        }

        private void dgvOperationsTermsSetting()
        {
            dgvOperationsTerms.DataSource = TechCatalogOperationsTermsManager.TermsList;
            dgvOperationsTerms.Columns.Add(TechCatalogOperationsTermsManager.ParameterColumn);
            dgvOperationsTerms.Columns.Add(TechCatalogOperationsTermsManager.LogicOperationsColumn);
            dgvOperationsTerms.Columns["TechCatalogOperationsDetailID"].Visible = false;
            dgvOperationsTerms.Columns["TechCatalogOperationsTermID"].Visible = false;
            dgvOperationsTerms.Columns["Term"].Visible = false;
            dgvOperationsTerms.Columns["Parameter"].Visible = false;
            dgvOperationsTerms.Columns["LogicOperation"].Visible = false;

            foreach (DataGridViewColumn Column in dgvOperationsTerms.Columns)
            {
                Column.ReadOnly = false;
            }
            dgvOperationsTerms.Columns["MathSymbol"].HeaderText = "Знак";
            dgvOperationsTerms.Columns["TermDisplayName"].HeaderText = "Условие";

            dgvOperationsTerms.Columns["ParameterColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvOperationsTerms.Columns["ParameterColumn"].MinimumWidth = 100;
            dgvOperationsTerms.Columns["MathSymbol"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvOperationsTerms.Columns["MathSymbol"].Width = 100;
            dgvOperationsTerms.Columns["LogicOperationsColumn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvOperationsTerms.Columns["LogicOperationsColumn"].Width = 100;
            dgvOperationsTerms.Columns["TermDisplayName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvOperationsTerms.Columns["TermDisplayName"].MinimumWidth = 50;

            dgvOperationsTerms.AutoGenerateColumns = false;
            int DisplayIndex = 0;
            dgvOperationsTerms.Columns["ParameterColumn"].DisplayIndex = DisplayIndex++;
            dgvOperationsTerms.Columns["MathSymbol"].DisplayIndex = DisplayIndex++;
            dgvOperationsTerms.Columns["TermDisplayName"].DisplayIndex = DisplayIndex++;
            dgvOperationsTerms.Columns["LogicOperationsColumn"].DisplayIndex = DisplayIndex++;
        }

        private void NavigateMenuCloseButton_Click(object sender, EventArgs e)
        {
            FormEvent = eClose;
            AnimateTimer.Enabled = true;
        }

        private void cmbParameters_SelectedIndexChanged(object sender, EventArgs e)
        {
            Term = ((DataRowView)cmbParameters.SelectedItem).Row["Parameter"].ToString();
            switch (Term)
            {
                case "CoverID":
                    DecimalParameter = false;
                    tbTerm.Visible = false;
                    cmbTerm.Visible = true;
                    cmbTerm.DataSource = TechCatalogOperationsTermsManager.CoversList;
                    cmbTerm.ValueMember = "CoverID";
                    cmbTerm.DisplayMember = "CoverName";
                    break;
                case "InsetTypeID":
                    DecimalParameter = false;
                    tbTerm.Visible = false;
                    cmbTerm.Visible = true;
                    cmbTerm.DataSource = TechCatalogOperationsTermsManager.InsetTypesList;
                    cmbTerm.ValueMember = "InsetTypeID";
                    cmbTerm.DisplayMember = "InsetTypeName";
                    break;
                case "InsetColorID":
                    DecimalParameter = false;
                    tbTerm.Visible = false;
                    cmbTerm.Visible = true;
                    cmbTerm.DataSource = TechCatalogOperationsTermsManager.InsetColorsList;
                    cmbTerm.ValueMember = "InsetColorID";
                    cmbTerm.DisplayMember = "ColorName";
                    break;
                case "ColorID":
                    DecimalParameter = false;
                    tbTerm.Visible = false;
                    cmbTerm.Visible = true;
                    cmbTerm.DataSource = TechCatalogOperationsTermsManager.ColorsList;
                    cmbTerm.ValueMember = "ColorID";
                    cmbTerm.DisplayMember = "TechStoreName";
                    break;
                case "PatinaID":
                    DecimalParameter = false;
                    tbTerm.Visible = false;
                    cmbTerm.Visible = true;
                    cmbTerm.DataSource = TechCatalogOperationsTermsManager.PatinaList;
                    cmbTerm.ValueMember = "PatinaID";
                    cmbTerm.DisplayMember = "PatinaName";
                    break;
                case "Diameter":
                    DecimalParameter = true;
                    tbTerm.Visible = true;
                    cmbTerm.Visible = false;
                    break;
                case "Thickness":
                    DecimalParameter = true;
                    tbTerm.Visible = true;
                    cmbTerm.Visible = false;
                    break;
                case "Length":
                    DecimalParameter = true;
                    tbTerm.Visible = true;
                    cmbTerm.Visible = false;
                    break;
                case "Height":
                    DecimalParameter = true;
                    tbTerm.Visible = true;
                    cmbTerm.Visible = false;
                    break;
                case "Width":
                    DecimalParameter = true;
                    tbTerm.Visible = true;
                    cmbTerm.Visible = false;
                    break;
                case "Admission":
                    DecimalParameter = true;
                    tbTerm.Visible = true;
                    cmbTerm.Visible = false;
                    break;
                case "InsetHeightAdmission":
                    DecimalParameter = true;
                    tbTerm.Visible = true;
                    cmbTerm.Visible = false;
                    break;
                case "InsetWidthAdmission":
                    DecimalParameter = true;
                    tbTerm.Visible = true;
                    cmbTerm.Visible = false;
                    break;
                case "Capacity":
                    DecimalParameter = true;
                    tbTerm.Visible = true;
                    cmbTerm.Visible = false;
                    break;
                case "Weight":
                    DecimalParameter = true;
                    tbTerm.Visible = true;
                    cmbTerm.Visible = false;
                    break;
            }
        }

        private void btnSaveTerms_Click(object sender, EventArgs e)
        {
            TechCatalogOperationsTermsManager.SaveTerms();
            TechCatalogOperationsTermsManager.SaveChangesToTechCatalogOperations();
            TechCatalogOperationsTermsManager.ClearTerms();
            TechCatalogOperationsTermsManager.UpdateTerms();
            TechCatalogOperationsTermsManager.FillParameterNames();
        }

        private void btnAddTerm_Click(object sender, EventArgs e)
        {
            bool TryParseOk = false;
            decimal dec = 0;
            if (DecimalParameter)
            {
                TryParseOk = decimal.TryParse(tbTerm.Text, out dec);
            }
            else
            {
                TryParseOk = decimal.TryParse(((DataRowView)cmbTerm.SelectedItem).Row[Term].ToString(), out dec);
            }

            if (TryParseOk)
                TechCatalogOperationsTermsManager.AddTerm(
                    ((DataRowView)cmbParameters.SelectedItem).Row["Parameter"].ToString(),
                    ((DataRowView)cmbMathSymbol.SelectedItem).Row["MathSymbol"].ToString(),
                    dec,
                    ((DataRowView)cmbLogicOperations.SelectedItem).Row["LogicOperation"].ToString());
            TechCatalogOperationsTermsManager.FillParameterNames();
            tbTerm.Clear();
        }

        private void btnRemoveTerm_Click(object sender, EventArgs e)
        {
            bool OkCancel = Infinium.LightMessageBox.Show(ref TopForm, true, "Вы подтверждаете удаление?", "Внимание");
            if (!OkCancel)
                return;
            TechCatalogOperationsTermsManager.RemoveTerm();
        }

        private void AddOperationsTermsForm_Load(object sender, EventArgs e)
        {
            label2.Text = TechCatalogOperationName;
            Initialize();
        }
    }
}
