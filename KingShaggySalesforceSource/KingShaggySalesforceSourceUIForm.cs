using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Runtime.Design;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.DataTransformationServices.Controls;

namespace KingShaggySalesforceSource
{
    public partial class KingShaggySalesforceSourceUIForm : Form
    {
        private IDTSComponentMetaData100 metaData;
        private IServiceProvider serviceProvider;
        private IDtsConnectionService connectionService;
        private CManagedComponentWrapper designTimeInstance;

        //string baseUrl = string.Empty;
        string subUrl = "/services/data/";
        //string baseUrl = "/services/data/v59.0/";
        string connectionTypeName = "KS Salesforce Connection";

        KingShaggySalesforceSource objRet;
        private class ConnectionManagerItem
        {
            public string ID;
            public string Name { get; set; }
            public KingShaggySFConnectionManager.KingShaggySFConnectionManager ConnManager { get; set; }
            public override string ToString()
            {
                return Name;
            }
        }
        public string SFQuery { get; set; }
        public KingShaggySalesforceSourceUIForm()
        {
            InitializeComponent();
        }
        public KingShaggySalesforceSourceUIForm(IDTSComponentMetaData100 metaData, IServiceProvider serviceProvider) : this()
        {
            this.metaData = metaData;
            this.serviceProvider = serviceProvider;
            this.connectionService = (IDtsConnectionService)serviceProvider.GetService(typeof(IDtsConnectionService));
            this.designTimeInstance = metaData.Instantiate();
        }
        private void KingShaggySalesforceSourceUIForm_Load(object sender, EventArgs e)
        {
            var connections = connectionService.GetConnections();

            string connectionManagerId = string.Empty;

            var currentConnectionManager = this.metaData.RuntimeConnectionCollection[0];
            if (currentConnectionManager != null)
            {
                connectionManagerId = currentConnectionManager.ConnectionManagerID;
            }

            int ksCMCheck = 0;

            for (int i = 0; i < connections.Count; i++)
            {
                var conn = connections[i].InnerObject as KingShaggySFConnectionManager.KingShaggySFConnectionManager;

                if (conn != null)
                {                    
                    var item = new ConnectionManagerItem()
                    {
                        Name = connections[i].Name,
                        ConnManager = conn,
                        ID = connections[i].ID
                    };
                    cbConnectionList.Items.Add(item);

                    if (connections[i].ID.Equals(connectionManagerId))
                    {
                        cbConnectionList.SelectedIndex = ksCMCheck;
                    }

                    ksCMCheck++;
                }
                
            }
            //Set access mode, table selection, and txtbox query selection here?
            this.cbAccessMode.SelectedItem = this.metaData.CustomPropertyCollection["AccessMode"].Value;
        }
        private async void btnOK_Click(object sender, EventArgs e)
        {
            if (cbTablesListSelected || txtBoxQueryHasValue)
            {
                string objQuery = string.Empty;

                var connections = connectionService.GetConnections();
                this.metaData.RuntimeConnectionCollection[0].ConnectionManager.AcquireConnection(connections);
                var accessTU = this.metaData.RuntimeConnectionCollection[0].ConnectionManager.AcquireConnection(connections) as KingShaggySFConnectionManager.KingShaggySFConnectionManager.AuthConn;

                var fullBaseUrl = accessTU.InstKSUrl + subUrl + accessTU.ApiKSVersion + "/";

                if (cbTablesListSelected)
                {
                    objRet = new KingShaggySalesforceSource();

                    var objectName = cbTablesList.SelectedValue.ToString();

                    //var objFields = await objRet.GetObjectFields(objectName, client);
                    var objFields = await objRet.GetObjectFields(fullBaseUrl, accessTU.AccKSToken, objectName);

                    if (!string.IsNullOrEmpty(objFields.ToString()))
                    {
                        objQuery = "SELECT " + objFields + " FROM " + objectName;
                    }
                    this.metaData.CustomPropertyCollection["SelectedObject"].Value = cbTablesList.SelectedValue.ToString();
                    //Clear out txtBoxQuery and property
                    this.txtBoxQuery.Text = string.Empty;
                    this.metaData.CustomPropertyCollection["CustomQuery"].Value = string.Empty;
                }
                else if (txtBoxQueryHasValue)
                {
                    objQuery = this.txtBoxQuery.Text.Trim();
                    this.metaData.CustomPropertyCollection["CustomQuery"].Value = objQuery;

                    this.metaData.CustomPropertyCollection["SelectedObject"].Value = string.Empty;
                }

                if (!string.IsNullOrWhiteSpace(objQuery))
                {
                    designTimeInstance.AcquireConnections(null);
                    designTimeInstance.SetComponentProperty("SOAQ", objQuery);

                    designTimeInstance.ReinitializeMetaData();
                    designTimeInstance.ReleaseConnections();                   
                }

                //Set other Components here for UI retrival later.
                this.metaData.CustomPropertyCollection["AccessMode"].Value = cbAccessMode.SelectedItem.ToString();

                if (cbConnectionList.SelectedItem != null)
                {
                    var item = cbConnectionList.SelectedItem.ToString();

                    SetRuntimeConnection(item);
                }
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
        private void btnNewConnectionManager_Click(object sender, EventArgs e)
        {
            //Connection Manager Type KS Salesforce
            System.Collections.ArrayList newConns = connectionService.CreateConnection(connectionTypeName);

            foreach (ConnectionManager cm in newConns)
            {
                var item = new ConnectionManagerItem()
                {
                    Name = cm.Name,
                    ConnManager = cm.InnerObject as KingShaggySFConnectionManager.KingShaggySFConnectionManager,
                    ID = cm.ID
                };

                cbConnectionList.Items.Insert(0, item);
                cbConnectionList.SelectedItem = item;

                SetRuntimeConnection(item);
            }
        }

        private void SetRuntimeConnection(ConnectionManagerItem item)
        {
            this.metaData.RuntimeConnectionCollection[0].ConnectionManagerID = item.ID;
            this.metaData.RuntimeConnectionCollection[0].Name = item.Name;

            this.metaData.RuntimeConnectionCollection[0].Description = "Connection manager for Salesforce";
        }
        private void SetRuntimeConnection(object selectedItem)
        {
            var connections = connectionService.GetConnections();

            if (connections.Contains(selectedItem))
            {
                this.metaData.RuntimeConnectionCollection[0].ConnectionManager = DtsConvert.ToConnectionManager90(connections[selectedItem]);

                this.metaData.RuntimeConnectionCollection[0].ConnectionManagerID = connections[selectedItem].ID;
                this.metaData.RuntimeConnectionCollection[0].Name = connections[selectedItem].Name;

                //check setting connections[selectedItem].Description and use without hardcoding.
                this.metaData.RuntimeConnectionCollection[0].Description = connections[selectedItem].Description;
                this.metaData.RuntimeConnectionCollection[0].Description = "Connection manager for Salesforce";

            }
        }
        private void cbConnectionList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbConnectionList.SelectedIndex >= 0)
            {
                //Reset cbAccessMode
                cbAccessMode.SelectedIndex = -1;

                cbConnectionListSelected = true;                
                this.cbAccessMode.Enabled = cbConnectionListSelected;

                SetRuntimeConnection(cbConnectionList.SelectedItem.ToString());
            }
            else
            {
                this.cbAccessMode.Enabled = false;
            }
        }

        private void cbAccessMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbAccessMode.SelectedIndex >= 0)
            {
                cbAccessModeSelected = true;
                //Disable first before...
                btnObjectPreview.Enabled = false;
                btnOK.Enabled = false;
            }

            object accessMode = cbAccessMode.SelectedItem;

            var connections = connectionService.GetConnections();
            this.metaData.RuntimeConnectionCollection[0].ConnectionManager.AcquireConnection(connections);
            var accessTU = this.metaData.RuntimeConnectionCollection[0].ConnectionManager.AcquireConnection(connections) as KingShaggySFConnectionManager.KingShaggySFConnectionManager.AuthConn;

            if (Convert.ToString(accessMode) == "Table")
            {
                this.txtBoxQuery.Clear();

                this.lblSelectTable.Text = "Select Table";
                this.lblSelectTable.Visible = true;
                this.pnlQuerySelection.Visible = false;
                this.pnlTableSelection.Visible = true;

                string baseUrl = accessTU.InstKSUrl + subUrl + accessTU.ApiKSVersion + "/";

                LoadObjectsTablesList(baseUrl, accessTU.AccKSToken);

            }
            else if (Convert.ToString(accessMode) == "Query")
            {
                this.lblSelectTable.Text = "Enter Query";
                this.lblSelectTable.Visible = true;
                this.pnlTableSelection.Visible = false;
                this.pnlQuerySelection.Visible = true;

                var txtBoxQuery = this.metaData.CustomPropertyCollection["CustomQuery"].Value;
                if (txtBoxQuery != null)
                {
                    this.txtBoxQuery.Text = (txtBoxQuery.ToString());
                }
            }
        }

        private async void LoadObjectsTablesList(string baseUrl, string accessToken)
        {
            DataTable dt = null;// = new DataTable();

            using (var client = new HttpClient())
            {
                // Set up the request headers
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                // Make the request to get the object metadata
                //var response = client.PostAsync(tokenEndpoint, request).ContinueWith(task => task.GetAwaiter()).Result.GetResult();

                var response = client.GetAsync($"{baseUrl}sobjects").ContinueWith(task => task.GetAwaiter()).Result.GetResult();
                //var response = await client.GetAsync($"{baseUrl}sobjects");

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = response.Content.ReadAsStringAsync().ContinueWith(task => task.GetAwaiter()).Result.GetResult();

                    var objectNames = JsonConvert.DeserializeObject<dynamic>(responseBody);

                    JArray sobjectsArray = objectNames.sobjects;

                    string[] nameFields = { "name", "label" };

                    dt = ConvertJArrayToDataTable(sobjectsArray, nameFields);

                    //Set ds to null and clear
                    cbTablesList.DataSource = null;
                    cbTablesList.Items.Clear();

                    DataRow row = dt.NewRow();
                    //row["attributes"] = -1;
                    //row["Label"] = "--- Select Object/Table ---";
                    row["name"] = "--- Select Object/Table ---";
                    row["Label"] = "SelectTable";
                    dt.Rows.InsertAt(row, 0);

                    cbTablesList.DataSource = dt;
                    cbTablesList.DisplayMember = "name";
                    cbTablesList.ValueMember = "name";

                    //if choosen already set default.
                    var objValue = this.metaData.CustomPropertyCollection["SelectedObject"].Value;
                    if (objValue != null && !objValue.Equals(""))
                    {
                        cbTablesList.SelectedValue = objValue;
                    }
                    else
                    {
                        if (cbTablesList.DataSource != null)
                        {
                            cbTablesList.SelectedIndex = 0;
                        }
                        //cbTablesList.SelectedIndex = -1;
                    }
                }
                else
                {
                    // Handle error
                    Console.WriteLine($"Failed to get object names: {response.ReasonPhrase}");
                    MessageBox.Show("Unable to retrieve Objects list from Salesforce. " + response.ReasonPhrase, "King Shaggy Salesforce Source UI", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private async void btnObjectPreview_Click(object sender, EventArgs e)
        {
            var connections = connectionService.GetConnections();
            this.metaData.RuntimeConnectionCollection[0].ConnectionManager.AcquireConnection(connections);
            var accessTU = this.metaData.RuntimeConnectionCollection[0].ConnectionManager.AcquireConnection(connections) as KingShaggySFConnectionManager.KingShaggySFConnectionManager.AuthConn;

            var cbAccessModeText = cbAccessMode.SelectedItem.ToString();
            var fullBaseUrl = accessTU.InstKSUrl + subUrl + accessTU.ApiKSVersion + "/";

            if (cbConnectionListSelected & (cbAccessModeText == "Table"))
            {
                objRet = new KingShaggySalesforceSource();

                var objectName = cbTablesList.SelectedValue.ToString();                

                //var objFields = objRet.GetObjectFields(fullBaseUrl, accessTU.AccKSToken, objectName).ContinueWith(task => task.GetAwaiter()).Result.GetResult();
                var objFields = await objRet.GetObjectFields(fullBaseUrl, accessTU.AccKSToken, objectName);

                if (!string.IsNullOrEmpty(objFields.ToString()))
                {
                    var objQuery = "SELECT " + objFields + " FROM " + objectName;

                    objQuery += " LIMIT 27";
                    //objQuery = objQuery + " LIMIT 27";

                    PopulatePreviewDataGrid(accessTU.AccKSToken, fullBaseUrl, objQuery);
                }
            }
            else if (cbConnectionListSelected & (cbAccessModeText == "Query"))
            {
                var enteredQuery = this.txtBoxQuery.Text.Trim();

                string modifiedQuery = enteredQuery.Contains("LIMIT") ? enteredQuery.Substring(0, enteredQuery.IndexOf("LIMIT")) + "LIMIT 27" : enteredQuery + " LIMIT 27";

                if (!string.IsNullOrEmpty(enteredQuery))
                {
                    PopulatePreviewDataGrid(accessTU.AccKSToken, fullBaseUrl, modifiedQuery);
                }
            }
        }
        private async void PopulatePreviewDataGrid(string accToken, string apiEndpoint, string query)
        {
            objRet = new KingShaggySalesforceSource();
            KingShaggyTablePreview dgPreview;

            var dt = await System.Threading.Tasks.Task.Run(() => objRet.GetRecordsAsync(accToken, apiEndpoint, query));
            //(var iDataTable, string nextRecordsUrl) = await System.Threading.Tasks.Task.Run(() => objRet.GetRecordsAsync(client, query));

            if (dt.Rows.Count == 0)
            {
                //Empty records do something.
                dt.Columns.Add("No Records Message", typeof(string));
                dt.Rows.Add("No Records return!");
            }

            dgPreview = new KingShaggyTablePreview(dt);
            dgPreview.ShowDialog();
        }
        private void cbTablesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbTablesList.SelectedIndex > 0)
            {
                //Enable Preview button
                btnObjectPreviewEnabled = true;
                btnObjectPreview.Enabled = true;

                cbTablesListSelected = true;

                btnOK.Enabled = true;
            }
            else
            {
                btnObjectPreviewEnabled = false;
                btnObjectPreview.Enabled = false;

                cbTablesListSelected = false;
            }
        }
        private void txtBoxQuery_TextChanged(object sender, EventArgs e)
        {
            this.btnObjectPreview.Enabled = !string.IsNullOrWhiteSpace(this.txtBoxQuery.Text);
            btnObjectPreviewEnabled = !string.IsNullOrWhiteSpace(this.txtBoxQuery.Text);
            txtBoxQueryHasValue = !string.IsNullOrWhiteSpace(this.txtBoxQuery.Text);

            cbTablesListSelected = !txtBoxQueryHasValue;

            btnOK.Enabled = !string.IsNullOrWhiteSpace(this.txtBoxQuery.Text);
        }

        static DataTable ConvertJArrayToDataTable(JArray jArray, string[] nameFields)
        {
            DataTable dataTable = new DataTable();

            foreach (string field in nameFields)
            {
                dataTable.Columns.Add(field, typeof(string)); //Adjust for type
            }

            foreach (JObject jObject in jArray)
            {
                DataRow row = dataTable.NewRow();

                foreach (string field in nameFields)
                {
                    JToken value;
                    if (jObject.TryGetValue(field, out value))
                    {
                        row[field] = value.ToString(); //Adjust the conversion.
                    }
                    else
                    {
                        row[field] = DBNull.Value; //If the field is not present set to DBNull
                    }
                }
                dataTable.Rows.Add(row);
            }
            return dataTable;
        }

        //Use below to enable Ok button
        public bool cbConnectionListSelected { get; set; }
        public bool cbAccessModeSelected { get; set; }
        public bool cbTablesListSelected { get; set; }
        public bool txtBoxQueryHasValue { get; set; }
        public bool btnObjectPreviewEnabled { get; set; }

    }
}
