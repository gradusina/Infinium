using System;
using System.Collections;
using System.Windows.Forms;

namespace Infinium
{
    public partial class LanguageForm : Form
    {
        public static string SelectedDate = "";
        Form TopForm = null;
        public static ArrayList ProfLangs;

        public LanguageForm()
        {
            InitializeComponent();

            ProfLangs = new ArrayList();
        }

        private void OKDateButton_Click(object sender, EventArgs e)
        {
            ProfLangs.Clear();

            for (int i = 0; i < ProfListBox.ItemCount; i++)
            {
                ProfLangs.Add(ProfListBox.Items[i]);
            }

            this.Close();
        }

        private void CancelDateButton_Click(object sender, EventArgs e)
        {
            //ProfLangs.Clear();

            this.Close();
        }

        private void toolTipController1_BeforeShow(object sender, DevExpress.Utils.ToolTipControllerShowEventArgs e)
        {
            if (e.SelectedControl.Name == "A1CheckBox")
            {
                e.ToolTip = "Понимаю и могу употребить в речи знакомые фразы и выражения,\r\n" +
                    "необходимые для выполнения конкретных задач. Могу представиться/представить других,\r\n" +
                    "задавать/отвечать на вопросы о месте жительства, знакомых, имуществе. Могу участвовать\r\n" +
                    "в несложном разговоре, если собеседник говорит медленно и отчетливо и готов оказать помощь.";
            }

            if (e.SelectedControl.Name == "A2CheckBox")
            {
                e.ToolTip = "Понимаю отдельные предложения и часто встречающиеся выражения,\r\n" +
                            "связанные с основными сферами жизни (например, основные сведения \r\n" +
                            "о себе и членах своей семьи, покупках, устройстве на работу и т.п.).\r\n" +
                            "Могу выполнить задачи, связанные с простым обменом информации на\r\n" +
                            "знакомые или бытовые темы. В простых выражениях могу рассказать о себе,\r\n" +
                            "своих родных и близких, описать основные аспекты повседневной жизни.";
            }

            if (e.SelectedControl.Name == "B1CheckBox")
            {
                e.ToolTip = "Понимаю основные идеи четких сообщений, сделанных на литературном языке\r\n" +
                            "на разные темы, типично возникающие на работе, учебе, досуге и т.д.\r\n" +
                            "Умею общаться в большинстве ситуаций, которые могут возникнуть во время\r\n" +
                            "пребывания в стране изучаемого языка. Могу составить связное сообщение\r\n" +
                            "на известные или особо интересующие меня темы. Могу описать впечатления\r\n," +
                            "события, надежды, стремления, изложить и обосновать свое мнение и планы на будущее.";
            }

            if (e.SelectedControl.Name == "B2CheckBox")
            {
                e.ToolTip = "Понимаю общее содержание сложных текстов на абстрактные и конкретные темы,\r\n" +
                            "в том числе узкоспециальные тексты. Говорю достаточно быстро и спонтанно, \r\n" +
                            "чтобы постоянно общаться с носителями языка без особых затруднений для любой из сторон.\r\n" +
                            "Я умею делать четкие, подробные сообщения на различные темы и изложить свой взгляд \r\n" +
                            "на основную проблему, показать преимущество и недостатки разных мнений.";
            }

            if (e.SelectedControl.Name == "C1CheckBox")
            {
                e.ToolTip = "Понимаю объемные сложные тексты на различную тематику, распознаю скрытое значение.\r\n" +
                            "Говорю спонтанно в быстром темпе, не испытывая затруднений с подбором слов и выражений.\r\n" +
                            "Гибко и эффективно использую язык для общения в научной и профессиональной деятельности.\r\n" +
                            "Могу создать точное , детальное, хорошо выстроенное сообщение на сложные темы,\r\n" +
                            "демонстрируя владение моделями организации текста, средствами связи и объединением его элементов.";
            }

            if (e.SelectedControl.Name == "C2CheckBox")
            {
                e.ToolTip = "Понимаю практически любое устное или письменное сообщение, могу составить связный текст,\r\n" +
                            "опираясь на несколько устных и письменных источников. Говорю спонтанно с высоким темпом \r\n" +
                            "и высокой степенью точности, подчеркивая оттенки значений даже в самых сложных случаях.";
            }
        }

        string GetLangWithoutLevel(string Lang)
        {
            return Lang.Substring(0, Lang.IndexOf("(") - 1);
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            if (LangListBox.SelectedIndex == -1)
                return;

            string Level = "";

            if (A1CheckBox.Checked)
                Level = "(A1)";
            if (A2CheckBox.Checked)
                Level = "(A2)";
            if (B1CheckBox.Checked)
                Level = "(B1)";
            if (B2CheckBox.Checked)
                Level = "(B2)";
            if (C1CheckBox.Checked)
                Level = "(C1)";
            if (C2CheckBox.Checked)
                Level = "(C2)";

            ProfListBox.Items.Add(LangListBox.SelectedItem.ToString() + " " + Level);

            LangListBox.Items.RemoveAt(LangListBox.SelectedIndex);
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (ProfListBox.SelectedIndex == -1)
                return;

            LangListBox.Items.Add(GetLangWithoutLevel(ProfListBox.SelectedItem.ToString()));

            ProfListBox.Items.RemoveAt(ProfListBox.SelectedIndex);

            LangListBox.SortOrder = SortOrder.Ascending;
        }

        private void LanguageForm_Shown(object sender, EventArgs e)
        {
            if (ProfLangs.Count > 0)
            {
                for (int i = 0; i < ProfLangs.Count; i++)
                {
                    LangListBox.Items.RemoveAt(LangListBox.FindString(ProfLangs[i].ToString().Substring(0, ProfLangs[i].ToString().IndexOf("(") - 1)));
                    ProfListBox.Items.Add(ProfLangs[i]);
                }
            }
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
    }
}
