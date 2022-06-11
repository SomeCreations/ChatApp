using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace ChatApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //Gets incoming message from Form2
        public string incMessage { get; set; }

        public string contacts = @"C:\Users\Matt\source\repos\ChatApp\ChatApp\contactList.json";

        //Reads all contacts & adds to contactBox
        private void Form1_Load(object sender, EventArgs e)
        {
            contactBox.Items.Clear();

            string jsonData = File.ReadAllText(contacts);
            var users = JsonConvert.DeserializeObject<List<Person>>(jsonData);

            foreach (var item in users)
            {
                contactBox.Items.Add(item.Firstname);
            }
        }

        //Timestamps messages
        private void button5_Click(object sender, EventArgs e)
        {
            string time = DateTime.Now.ToString("HH:mm:ss : ");
            string message = textBox1.Text;
            textBox3.AppendText(time);
            textBox3.AppendText(message);
            textBox3.AppendText(Environment.NewLine);
            textBox1.Text = "";
        }

        //Adding new contact
        private void button4_Click(object sender, EventArgs e)
        {
            var jsonData = File.ReadAllText(contacts);
            var users = JsonConvert.DeserializeObject<List<Person>>(jsonData);

            var newuser = new Person();
            newuser.Firstname = textBox2.Text;

            users.Add(newuser);

            contactBox.Items.Add(newuser.Firstname);

            jsonData = JsonConvert.SerializeObject(users, Formatting.Indented);

            File.WriteAllText(contacts, jsonData);
        }

        //Resizing tabs for efficient usage of space
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
            {
                this.Size = new Size(287, 281);
            }

            if (tabControl1.SelectedIndex == 1)
            {
                this.Size = new Size(472, 281);
            }
        }

        //JSON Strings
        public class Person
        {
            public int Id { get; set; }
            public string Firstname { get; set; }
            public string Lastname { get; set; }
            public string City { get; set; }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //---listen at the specified IP and port no.---
            IPAddress localAdd = IPAddress.Parse("192.168.1.220");
            TcpListener listener = new TcpListener(localAdd, 25565);
            Console.WriteLine("Listening...");
            listener.Start();

            //---incoming client connected---
            TcpClient client = listener.AcceptTcpClient();

            //---get the incoming data through a network stream---
            NetworkStream nwStream = client.GetStream();
            byte[] buffer = new byte[client.ReceiveBufferSize];

            //---read incoming stream---
            int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);

            //---convert the data received into a string---
            string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            Console.WriteLine("Received : " + dataReceived);

            //---write back the text to the client---
            Console.WriteLine("Sending back : " + dataReceived);
            nwStream.Write(buffer, 0, bytesRead);
            client.Close();
            listener.Stop();
            Console.ReadLine();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //---data to send to the server---
            string textToSend = textBox1.Text;

            //---create a TCPClient object at the IP and port no.---
            TcpClient client = new TcpClient("192.168.1.220", 25565);
            NetworkStream nwStream = client.GetStream();
            byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(textToSend);

            //---send the text---
            nwStream.Write(bytesToSend, 0, bytesToSend.Length);

            //---read back the text---
            byte[] bytesToRead = new byte[client.ReceiveBufferSize];
            int bytesRead = nwStream.Read(bytesToRead, 0, client.ReceiveBufferSize);
            string incMsg = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);
            textBox4.AppendText(incMsg);
            textBox4.AppendText(Environment.NewLine);
            client.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            textBox4.Text = incMessage;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Server serv = new Server();
            serv.Show();
        }
    }
}