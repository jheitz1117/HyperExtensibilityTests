using System;
using System.Windows.Forms;
using Hyper.NodeServices.Contracts;
using NodeExtensions.DbCommands.Contracts;

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

            var requestObject = new ExecuteStoredProcedureRequest();

            var response = nodeService1.ProcessMessage(
                new HyperNodeMessageRequest("NodeClient1")
                {
                    CommandName = "UpdateDatabase"
                }
            );

            MessageBox.Show(response.CommandResponseString);
        }
    }
}
