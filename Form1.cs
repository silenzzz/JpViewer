using PdfiumViewer;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace jpview
{
    public partial class FormMain : Form
    {
        private string icao;
        private WebClient web = new WebClient();

        public FormMain(string[] args)
        {
            InitializeComponent();
            web.DownloadFileCompleted += new AsyncCompletedEventHandler(OnFileDownloadCompleted);

            if (args.Length < 1)
            {
                Console.WriteLine("Enter ICAO as first argument");
                Exit();
            }

            icao = args[0].ToUpper();

            if (!ValidateIcao(icao))
            {
                Console.WriteLine("Unknown ICAO");
                Exit();
            }

            new Thread(() =>
            {
                string fileName = icao + ".pdf";
                if (!File.Exists(fileName)) {
                    web.DownloadFileAsync(new Uri(string.Format("https://vau.aero/navdb/chart/{0}.pdf", icao)), fileName, fileName);
                }
                else
                {
                    SetPdf(fileName);
                }
            }).Start();
        }

        private void Exit()
        {
            Environment.Exit(0);
        }

        private void OnFileDownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            SetPdf(e.UserState.ToString());
        }

        private void SetPdf(string fileName)
        {
            var pdf = PdfDocument.Load(fileName);
            pdfViewer.Invoke(new Action(() => pdfViewer.Document = pdf));
        }

        private bool ValidateIcao(string icao)
        {
            if (icao.Length != 4)
            {
                return false;
            }
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://airportsbase.org/search.php?code=" + icao);
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                return response.StatusCode == HttpStatusCode.OK;
            } catch (WebException)
            {
                return false;
            }
        }

        private void ButtonAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show(string.Format("Created by DeMmAge\nVersion - {0}\nUsed resources:\n" +
                "https://vau.aero/\n" +
                "http://airportsbase.org/", typeof(Program).Assembly.GetName().Version.ToString()), "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ButtonTop_Click(object sender, EventArgs e)
        {
            if (TopMost)
            {
                TopMost = false;
                buttonTop.Text = "Top";
            } else
            {
                TopMost = true;
                buttonTop.Text = "UnTop";
            }
        }
    }
}
