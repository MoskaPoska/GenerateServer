using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web;

namespace GenerateServer
{
    public partial class Form1 : Form
    {
        Thread thread;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (thread == null)
            {
                thread = new Thread(StartServer);
                thread.IsBackground = true;
                thread.Start();
                Text = "Server is working";
            }
        }
        private async void StartServer()
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.IP);
            byte[] buff = new byte[1024];
            EndPoint endPoint = new IPEndPoint(IPAddress.Any, 11000);
            IPAddress address = Dns.GetHostAddresses(Dns.GetHostName())[2];
            socket.Bind(new IPEndPoint(address, int.Parse(textBox2.Text)));
            try
            {
                do
                {
                    await socket.ReceiveFromAsync(new ArraySegment<byte>(buff), SocketFlags.None, endPoint).ContinueWith(async x =>
                    {
                        SocketReceiveFromResult result = x.Result;
                        int countQuotes = int.Parse(Encoding.Unicode.GetString(buff, 0, result.ReceivedBytes)); ;
                        StringBuilder builder = new StringBuilder();
                        builder.AppendLine($"{result.ReceivedBytes} получил от {result.RemoteEndPoint}");
                        builder.AppendLine($"Подключился {DateTime.Now}");
                        builder.AppendLine($"Цитата {countQuotes}");
                        textBox1.BeginInvoke(new Action<string>(AddText), builder.ToString());

                        string response = GenerateQuotes(countQuotes);
                        buff = Encoding.Default.GetBytes(response);
                        await socket.SendToAsync(new ArraySegment<byte>(buff), SocketFlags.None, result.RemoteEndPoint);
                    });
                } while (true);
                Text = "Server is working";
            }
            catch (SocketException ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
        }

        private string GenerateQuotes(int countQuotes)
        {
            Random random = new Random();
            string[] quotes = { "Цитата1", "Цитата2", "Цитата3" };
            string qote = "";
            for (int i = 0; i < countQuotes; i++)
            {
                int index = random.Next(0, quotes.Length);
                qote += quotes[index] + Environment.NewLine;
            }
            return qote;
        }
        private void AddText(string str)
        {
            StringBuilder builder = new StringBuilder(textBox1.Text);
            builder.AppendLine(str);
            textBox1.Text = builder.ToString();
        }
    }
}