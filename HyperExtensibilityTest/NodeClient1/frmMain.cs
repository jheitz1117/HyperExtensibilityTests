using System;
using System.Windows.Forms;
using Hyper.NodeServices.Client;
using Hyper.NodeServices.Contracts;

namespace NodeClient1
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var nodeService1 = new HyperNodeClient();

            var response = nodeService1.ProcessMessage(
                new HyperNodeMessageRequest("NodeClient1")
                {
                    CommandName = "DbUpdateCommand"
                }
            );

            MessageBox.Show(response.CommandResponseString);
        }
    }
}
