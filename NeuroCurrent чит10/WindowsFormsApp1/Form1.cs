using System;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Drawing;

/// <_  примечание автора>
/// при несходимости:
/// поменять количество нейронов в промежуточных слоях (часто)
/// убрать нейрон смещения
/// поменять коэффицент обучения (редко)
/// </_ примечание автора>

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        NeuroWork neuroWork;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Debug.form1 = this;
            neuroWork = new NeuroWork(25, 10, 0.3f);
            //neuroWork.NormalizePhoto();
        }

        #region Click

        private void SaveButton_Click(object sender, EventArgs e)
        {
            StartNeuroWork();
        }

        void StartNeuroWork()
        {
            neuroWork.StartLearning();
        }

        private void SaveLetter_Click(object sender, EventArgs e)
        {
            SetNextPass();
        }


        private void ReadText_Click(object sender, EventArgs e)
        {
            neuroWork.StartWriteText();
        }

        private void CreateWorkSetButton_Click(object sender, EventArgs e)
        {
            neuroWork.Work();
            neuroWork.NextPreTrainingPass(' ', false);
            textBox1.Text = "";
        }

        private void DebugButton_Click(object sender, EventArgs e)
        {
            neuroWork.DebugWork();
        }

        #endregion Click

        #region ForDebugLog

        delegate void WriteLogDelegate(string text);
        WriteLogDelegate writeLogDelegate;

        delegate void ProgressDelegate(int value);
        ProgressDelegate progressDelegate;

        public void writeMassage(string text)
        {
            MessageForm.Text += "\n" + text;
        }

        public void WriteMassage(string text)
        {
            writeLogDelegate = new WriteLogDelegate(writeMassage);
            Invoke(writeLogDelegate, new object[] { text });
        }

        public void progressMaxValue(int value)
        {
            progressBar.Maximum = value;
        }

        public void ProgressMaxValue(int value)
        {
            progressDelegate = new ProgressDelegate(progressMaxValue);
            Invoke(progressDelegate, new object[] { value });
        }

        public void progressCurrentValue(int value)
        {
            if (value <= progressBar.Maximum && value >= progressBar.Minimum)
                progressBar.Value = value;
        }

        public void ProgressCurrentValue(int value)
        {
            progressDelegate = new ProgressDelegate(progressCurrentValue);
            Invoke(progressDelegate, new object[] { value });
        }

        public void progressChengeCurrentValue(int value)
        {
            progressBar.Value += value;
        }

        public void ProgressChengeCurrentValue(int value)
        {
            progressDelegate = new ProgressDelegate(progressChengeCurrentValue);
            Invoke(progressDelegate, new object[] { value });
        }

        public void clearDebugLog(int value)
        {
            MessageForm.Text = "";
        }

        public void ClearDebugLog()
        {
            progressDelegate = new ProgressDelegate(clearDebugLog);
            Invoke(progressDelegate, new object[] {44 });
        }

        #endregion ForDebugLog

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.End)
            {
                neuroWork.NextPreTrainingPass(' ', false);
                textBox1.Text = "";
                return;
            }
            else if (e.KeyCode == Keys.F5)
            {
                SetNextPass();
                return;
            }
            else if (e.KeyCode == Keys.Enter)
            {
                SetNextPass();
                return;
            }
            /**else if (e.KeyCode == Keys.NumPad9)
            {
                neuroWork.neuroNet.SaveNeuronet(@"C:\Users\Admin\Desktop\фото\text\Text.NN");
                Debug.ClearLog();
                Debug.Log("идет сохранение");
                return;
            }
            else if (e.KeyCode == Keys.NumPad8)
            {
                neuroWork.neuroNet = NeuroNet.LoadNeuronet(@"C:\Users\Admin\Desktop\фото\text\Text1.NN");
                neuroWork.ReadText();
            }
            else
            {
                Debug.ClearLog();
                Debug.Log("Value = " + e.KeyValue + " Key = " + e.KeyCode);
                for (int i = 0; i < 100; i++)
                {
                    neuroWork.Part2(e.KeyValue);
                    neuroWork.Part1();
                }
                neuroWork.NextPass();
                neuroWork.Part1();
            }*/
            //Debug.Log("e.KeyCode = " + e.KeyCode + " e.KeyValue = " + e.KeyValue);
        }

        void SetNextPass()
        {
            if (textBox1.Text.Length == 0)
            {
                Debug.Log("Поле для текста пустое");
                return;
            }
            else
            {
                neuroWork.NextPreTrainingPass(textBox1.Text[0]);
                textBox1.Text = "";
            }
        }

        private void ClearLogButton_Click(object sender, EventArgs e)
        {
            Debug.ClearLog();
        }
    }
}
