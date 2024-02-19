using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.SqlServer.Dts.Runtime;

namespace KingShaggySFConnectionManager
{
    public partial class KingShaggySFConnectionManagerUIForm : Form
    {
        private ConnectionManager connectionManager;
        private IServiceProvider serviceProvider;
        public KingShaggySFConnectionManagerUIForm()
        {
            InitializeComponent();
        }

        public KingShaggySFConnectionManagerUIForm(Microsoft.SqlServer.Dts.Runtime.ConnectionManager connectionManager, IServiceProvider serviceProvider)
: this()
        {
            this.connectionManager = connectionManager;
            this.serviceProvider = serviceProvider;

            SetFormValuesFromConnectionManager();
        }
        private void SetFormValuesFromConnectionManager()
        {
            string clientid = connectionManager.Properties["ClientId"].GetValue(connectionManager).ToString();
            string clientsecret = connectionManager.Properties["ClientSecret"].GetValue(connectionManager).ToString();
            string username = connectionManager.Properties["UserName"].GetValue(connectionManager).ToString();
            string password = connectionManager.Properties["Password"].GetValue(connectionManager).ToString();
            string token = connectionManager.Properties["Token"].GetValue(connectionManager).ToString();
            string url = connectionManager.Properties["Url"].GetValue(connectionManager).ToString();
            string apiversion = connectionManager.Properties["SFApiVersion"].GetValue(connectionManager).ToString();

            //string clientid = string.Empty;// connectionManager.Properties["ClientId"].GetValue(connectionManager).ToString();
            //string clientsecret = string.Empty;//connectionManager.Properties["ClientSecret"].GetValue(connectionManager).ToString();
            //string username = string.Empty;//connectionManager.Properties["UserName"].GetValue(connectionManager).ToString();
            //string password = string.Empty;//connectionManager.Properties["Password"].GetValue(connectionManager).ToString();
            //string token = string.Empty;//connectionManager.Properties["Token"].GetValue(connectionManager).ToString();
            //string url = string.Empty;//connectionManager.Properties["Url"].GetValue(connectionManager).ToString();
            //string apiversion = string.Empty;//connectionManager.Properties["SFApiVersion"].GetValue(connectionManager).ToString();

            if (!string.IsNullOrWhiteSpace(clientid))
            {
                txtClientId.Text = clientid;
            }
            if (!string.IsNullOrWhiteSpace(clientsecret))
            {
                txtClientSecret.Text = clientsecret;
            }
            if (!string.IsNullOrWhiteSpace(username))
            {
                txtUserName.Text = username;
            }
            if (!string.IsNullOrWhiteSpace(password))
            {
                txtPassword.Text = password;
            }
            if (!string.IsNullOrWhiteSpace(token))
            {
                txtToken.Text = token;
            }
            if (!string.IsNullOrWhiteSpace(url))
            {
                txtUrl.Text = url;
            }
            if (!string.IsNullOrWhiteSpace(apiversion))
            {
                txtApiVersion.Text = apiversion;
            }
        }
        private void UpdateConnectionFromControls()
        {
            connectionManager.Properties["ClientId"].SetValue(connectionManager, txtClientId.Text);
            connectionManager.Properties["ClientSecret"].SetValue(connectionManager, txtClientSecret.Text);
            connectionManager.Properties["UserName"].SetValue(connectionManager, txtUserName.Text);
            connectionManager.Properties["Password"].SetValue(connectionManager, txtPassword.Text);
            connectionManager.Properties["Token"].SetValue(connectionManager, txtToken.Text);
            connectionManager.Properties["Url"].SetValue(connectionManager, txtUrl.Text);
            connectionManager.Properties["SFApiVersion"].SetValue(connectionManager, txtApiVersion.Text);
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            UpdateConnectionFromControls();

            this.DialogResult = DialogResult.OK;

            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;

            this.Close();
        }
        private async void btnTestConnection_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtClientId.Text))
            {
                MessageBox.Show("Consumer Key(Client Id) field is empty!", "King Shaggy Connection Manager", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (string.IsNullOrEmpty(txtClientSecret.Text))
            {
                MessageBox.Show("Consumer secret(Client Secret) field is empty!", "King Shaggy Connection Manager", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (string.IsNullOrEmpty(txtUserName.Text))
            {
                MessageBox.Show("User Name field is empty!", "King Shaggy Connection Manager", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (string.IsNullOrEmpty(txtPassword.Text))
            {
                MessageBox.Show("Password field is empty!", "King Shaggy Connection Manager", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (string.IsNullOrEmpty(txtToken.Text))
            {
                MessageBox.Show("Token field is empty!", "King Shaggy Connection Manager", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (string.IsNullOrEmpty(txtUrl.Text))
            {
                MessageBox.Show("Url field is empty!", "King Shaggy Connection Manager", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            KingShaggySFConnectionManager sfconn = null;

            try
            {
                sfconn = new KingShaggySFConnectionManager();

                sfconn.ClientId = txtClientId.Text;
                sfconn.ClientSecret = txtClientSecret.Text;
                sfconn.UserName = txtUserName.Text;
                sfconn.Password = txtPassword.Text;
                sfconn.Token = txtToken.Text;
                sfconn.Url = txtUrl.Text;
                sfconn.SFApiVersion = txtApiVersion.Text;

                var connection = sfconn.CreateConnectionAsync();

                //wait for task to complete.
                while(!connection.IsCompleted)
                {
                    await System.Threading.Tasks.Task.Delay(1000);
                }

                var connResult = connection.Result;
   
                if (!string.IsNullOrEmpty(connResult.AccKSToken) && !string.IsNullOrEmpty(connResult.InstKSUrl))
                {
                    MessageBox.Show("Test connection verified", "King Shaggy Connection Manager", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Test connection failed!" + Environment.NewLine + "Check your connection credentials and verify there are valid. ", "King Shaggy Salesforce Connection Manager", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Test connection failed!" + Environment.NewLine + ex.Message, "King Shaggy Salesforce Connection Manager", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


    }
}
