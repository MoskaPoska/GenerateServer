using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    public partial class Form1 : Form
    {
        private UdpClient udpClient;
        private IPEndPoint endPoint;
        public Form1()
        {
            InitializeComponent();
            udpClient = new UdpClient();
            endPoint = new IPEndPoint(IPAddress.Parse("192.168.56.1"), 11000);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string countQuotesText = textBox4.Text;
            if(int.TryParse(countQuotesText, out int countQuotes))
            {
                byte[] requestByte = Encoding.Unicode.GetBytes(countQuotesText);
                udpClient.Send(requestByte, requestByte.Length, endPoint);

                byte[] responesByte = udpClient.Receive(ref endPoint);
                string quotes=Encoding.Default.GetString(responesByte);
                textBox1.Text = quotes;
            }
            else
            {
                MessageBox.Show("Неправильное число");
            }
        }
    }
}