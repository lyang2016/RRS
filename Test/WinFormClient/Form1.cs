using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WinFormClient
{
    public partial class Form1 : Form
    {
        private RRSService.PreAuthManagerClient client;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var result = client.HelloWorld();
            MessageBox.Show(result, "Hello World WCF");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                client = new RRSService.PreAuthManagerClient();
                client.ClientCredentials.UserName.UserName = "amhc\\yangl";
                client.ClientCredentials.UserName.Password = "Lwei$319";
                client.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            var result = client.CheckMemberFromWeb(txtIVRCode.Text, txtSubscriberId.Text, txtMemberSeq.Text,
                                                   txtAuthTypeId.Text,
                                                   DateTime.Parse(txtDateOfBirth.Text), dtpStartDate.Value);
            var msg = "IsValid = " + result.IsValid + Environment.NewLine +
                      "ErrorMessage = " + result.ErrorMessage;
            MessageBox.Show(msg, "Check Member from Web");
        }


    }
}
