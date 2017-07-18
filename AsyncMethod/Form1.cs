using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AsyncMethod
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string res;
        DateTime startTime;
        DateTime finishTime;
        private void Start()
        {
            button1.Text = "Cancel";
            button1.Enabled = false;
            progressBar1.Style = ProgressBarStyle.Marquee;
            startTime = DateTime.Now;
        }
        private void Finish()
        {
            progressBar1.Style = ProgressBarStyle.Blocks;
            finishTime = DateTime.Now;
            double timeSpent = finishTime.Subtract(startTime).TotalSeconds;
            label1.Text = timeSpent.ToString("F4") + " seconds spent";
            button1.Text = "Go";
            button1.Enabled = true;
        }
        private async void button1_Click(object sender, EventArgs e)
        {
            Start();
            try
            {
                var result = new Task<string>(Test);
                result.Start();
                await result;
                Finish();
                textBox1.Text = result.Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        long result = 0;
        string Test()
        {
            for(long i = 0; i < 10000000000; i++)
            {
                result += 1;
            }
            return result.ToString();
        }
        private void TaskMethod()
        {
            // 在這個執行緒中無法更新UI，會出現跨執行緒存取UI的錯誤
            var task = Task.Factory.StartNew(() =>
            {
                res = Test();
                textBox1.Text = res;
                return res;
            });
            var context = TaskScheduler.FromCurrentSynchronizationContext();
            // 可以在運算完成後，另外開一條執行緒更新UI
            task.ContinueWith(t =>
            {
                textBox1.Text = t.Result;
                Finish();
            },
            CancellationToken.None, TaskContinuationOptions.OnlyOnRanToCompletion, context);
        }
        
        
        private async void button2_Click(object sender, EventArgs e)
        {
            Start();
            try
            {
                var result = await Task.Factory.StartNew(() => Test());
                Finish();
                textBox1.Text = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            Start();
            try
            {
                var result = await Task.Run(() => Test());
                Finish();
                textBox1.Text = result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
