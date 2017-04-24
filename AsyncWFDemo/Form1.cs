using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AsyncWFDemo
{
    public partial class Form1 : Form
    {
        private readonly string sampleFilePath = AppDomain.CurrentDomain.BaseDirectory
                                                 + @"..\..\..\AsynchronousPrograming\SampleData\SampleCsvRecords.csv";
        /// <summary>
        /// Contains event handlers for Form1
        /// </summary>
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Synchronous event handler
        /// </summary>
        private void Button1_Click(object sender, EventArgs e)
        {
            using (var reader = new StreamReader(sampleFilePath))
            {
                var counter = 0;
                for (var i = 0; i < 1000000; i++)
                {
                    var content = reader.ReadToEnd();
                    counter += content.Length;
                }
                MessageBox.Show($"{counter} characters have been red.", "Work completed.");
            }
        }

        /// <summary>
        /// Asynchronous event handler
        /// </summary>
        private async void Button2_Click(object sender, EventArgs e)
        {
            using (var reader = new StreamReader(sampleFilePath))
            {
                var counter = 0;
                for (var i = 0; i < 1000; i++)
                {
                    var content = await reader.ReadToEndAsync();
                    counter += content.Length;
                }
                MessageBox.Show($"{counter} characters have been red.", "Work completed.");
            }
        }

        /// <summary>
        /// Deadlock button event handler - example of bad usage of async
        /// 
        /// I    What causes the deadlock? Can we observe similar behavoiur in Console app ? Why not ??
        /// 
        /// II.  How can we fix it ?
        /// 
        /// III. Can we fix it only by changing GetFileContentAsync() ?
        ///      (so that event handler Button3_Click remains the same)
        /// </summary>
        private void Button3_Click(object sender, EventArgs e)
        {
            // Following code will cause a deadlock
            var jsonTask = GetFileContentAsync();
            textBox1.Text = jsonTask.Result;
        }

        /// <summary>
        /// Helper method for Button3_Click event handler
        /// </summary>
        private async Task<string> GetFileContentAsync()
        {
            using (var reader = new StreamReader(sampleFilePath))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}
