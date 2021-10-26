using PdfiumViewer;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace jpview
{
    public partial class FormMain : Form
    {
        private readonly string icao;
        private readonly WebClient web = new WebClient();

        //private List<string> mirrors = new List<string>();

        public FormMain(string[] args)
        {
            InitializeComponent();
            web.DownloadFileCompleted += new AsyncCompletedEventHandler(OnFileDownloadCompleted);

            if (args.Length < 1)
            {
                Console.WriteLine("Enter ICAO as first argument");
                Exit();
            } else if (args.Length == 2 && args[0].Equals("-f")) {
                SetPdf(args[1]);
            } else {

                icao = args[0].ToUpper();

                if (!ValidateIcao(icao))
                {
                    Console.WriteLine("Unknown ICAO");
                    Exit();
                }

                new Thread(() =>
                {
                // C:\Users\%USERNAME%
                // TODO: store in directory
                string fileName = string.Format("{0}.pdf", icao);
                    if (!File.Exists(fileName) || new FileInfo(fileName).Length == 0) {
                        web.DownloadFileAsync(new Uri(string.Format("https://www.virtualairlines.eu/charts/{0}.pdf", icao)), fileName, fileName);
                    }
                    else
                    {
                        SetPdfInvoke(fileName);
                    }
                }).Start();
            }
        }

        private void Exit()
        {
            Environment.Exit(0);
        }

        private void OnFileDownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            SetPdfInvoke(e.UserState.ToString());
        }

        private void SetPdfInvoke(string fileName)
        {
            var pdf = PdfDocument.Load(fileName);
            pdfViewer.Invoke(new Action(() => pdfViewer.Document = pdf));
        }

        private void SetPdf(string fileName)
        {
            var pdf = PdfDocument.Load(fileName);
            pdfViewer.Document = pdf;
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

        private void PdfViewer_LinkClick(object sender, LinkClickEventArgs e)
        {
           
        }
    }
}
