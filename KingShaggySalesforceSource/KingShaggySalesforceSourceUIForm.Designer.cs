
namespace KingShaggySalesforceSource
{
    partial class KingShaggySalesforceSourceUIForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KingShaggySalesforceSourceUIForm));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbAccessMode = new System.Windows.Forms.ComboBox();
            this.lblAccessMode = new System.Windows.Forms.Label();
            this.cbConnectionList = new System.Windows.Forms.ComboBox();
            this.lblConnection = new System.Windows.Forms.Label();
            this.btnNewConnectionManager = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblSelectTable = new System.Windows.Forms.Label();
            this.pnlQuerySelection = new System.Windows.Forms.Panel();
            this.txtBoxQuery = new System.Windows.Forms.TextBox();
            this.pnlTableSelection = new System.Windows.Forms.Panel();
            this.cbTablesList = new System.Windows.Forms.ComboBox();
            this.btnObjectPreview = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.pnlQuerySelection.SuspendLayout();
            this.pnlTableSelection.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(86, 7);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Enabled = false;
            this.btnOK.Location = new System.Drawing.Point(5, 7);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 327);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(416, 38);
            this.panel1.TabIndex = 7;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnCancel);
            this.panel2.Controls.Add(this.btnOK);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(246, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(170, 38);
            this.panel2.TabIndex = 8;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.groupBox1);
            this.panel3.Location = new System.Drawing.Point(12, 12);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(402, 84);
            this.panel3.TabIndex = 9;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbAccessMode);
            this.groupBox1.Controls.Add(this.lblAccessMode);
            this.groupBox1.Controls.Add(this.cbConnectionList);
            this.groupBox1.Controls.Add(this.lblConnection);
            this.groupBox1.Controls.Add(this.btnNewConnectionManager);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(402, 84);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Connection Settings";
            // 
            // cbAccessMode
            // 
            this.cbAccessMode.AllowDrop = true;
            this.cbAccessMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAccessMode.Enabled = false;
            this.cbAccessMode.FormattingEnabled = true;
            this.cbAccessMode.Items.AddRange(new object[] {
            "Table",
            "Query"});
            this.cbAccessMode.Location = new System.Drawing.Point(120, 60);
            this.cbAccessMode.Name = "cbAccessMode";
            this.cbAccessMode.Size = new System.Drawing.Size(129, 21);
            this.cbAccessMode.TabIndex = 4;
            this.cbAccessMode.SelectedIndexChanged += new System.EventHandler(this.cbAccessMode_SelectedIndexChanged);
            // 
            // lblAccessMode
            // 
            this.lblAccessMode.AutoSize = true;
            this.lblAccessMode.Location = new System.Drawing.Point(11, 65);
            this.lblAccessMode.Name = "lblAccessMode";
            this.lblAccessMode.Size = new System.Drawing.Size(109, 20);
            this.lblAccessMode.TabIndex = 3;
            this.lblAccessMode.Text = "Access Mode:";
            // 
            // cbConnectionList
            // 
            this.cbConnectionList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbConnectionList.FormattingEnabled = true;
            this.cbConnectionList.Location = new System.Drawing.Point(120, 33);
            this.cbConnectionList.Name = "cbConnectionList";
            this.cbConnectionList.Size = new System.Drawing.Size(188, 21);
            this.cbConnectionList.TabIndex = 1;
            this.cbConnectionList.SelectedIndexChanged += new System.EventHandler(this.cbConnectionList_SelectedIndexChanged);
            // 
            // lblConnection
            // 
            this.lblConnection.AutoSize = true;
            this.lblConnection.Location = new System.Drawing.Point(8, 36);
            this.lblConnection.Name = "lblConnection";
            this.lblConnection.Size = new System.Drawing.Size(143, 20);
            this.lblConnection.TabIndex = 0;
            this.lblConnection.Text = "Select Connection:";
            // 
            // btnNewConnectionManager
            // 
            this.btnNewConnectionManager.Location = new System.Drawing.Point(314, 33);
            this.btnNewConnectionManager.Name = "btnNewConnectionManager";
            this.btnNewConnectionManager.Size = new System.Drawing.Size(75, 23);
            this.btnNewConnectionManager.TabIndex = 2;
            this.btnNewConnectionManager.Text = "New...";
            this.btnNewConnectionManager.UseVisualStyleBackColor = true;
            this.btnNewConnectionManager.Click += new System.EventHandler(this.btnNewConnectionManager_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lblSelectTable);
            this.groupBox2.Controls.Add(this.pnlQuerySelection);
            this.groupBox2.Controls.Add(this.pnlTableSelection);
            this.groupBox2.Controls.Add(this.btnObjectPreview);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(401, 216);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Configuration";
            // 
            // lblSelectTable
            // 
            this.lblSelectTable.AutoSize = true;
            this.lblSelectTable.Location = new System.Drawing.Point(10, 16);
            this.lblSelectTable.Name = "lblSelectTable";
            this.lblSelectTable.Size = new System.Drawing.Size(97, 20);
            this.lblSelectTable.TabIndex = 8;
            this.lblSelectTable.Text = "Select Table";
            this.lblSelectTable.Visible = false;
            // 
            // pnlQuerySelection
            // 
            this.pnlQuerySelection.Controls.Add(this.txtBoxQuery);
            this.pnlQuerySelection.Location = new System.Drawing.Point(10, 59);
            this.pnlQuerySelection.Name = "pnlQuerySelection";
            this.pnlQuerySelection.Size = new System.Drawing.Size(385, 122);
            this.pnlQuerySelection.TabIndex = 11;
            this.pnlQuerySelection.Visible = false;
            // 
            // txtBoxQuery
            // 
            this.txtBoxQuery.Location = new System.Drawing.Point(0, 0);
            this.txtBoxQuery.Multiline = true;
            this.txtBoxQuery.Name = "txtBoxQuery";
            this.txtBoxQuery.Size = new System.Drawing.Size(375, 91);
            this.txtBoxQuery.TabIndex = 0;
            this.txtBoxQuery.TextChanged += new System.EventHandler(this.txtBoxQuery_TextChanged);
            // 
            // pnlTableSelection
            // 
            this.pnlTableSelection.Controls.Add(this.cbTablesList);
            this.pnlTableSelection.Location = new System.Drawing.Point(10, 32);
            this.pnlTableSelection.Name = "pnlTableSelection";
            this.pnlTableSelection.Size = new System.Drawing.Size(378, 22);
            this.pnlTableSelection.TabIndex = 10;
            this.pnlTableSelection.Visible = false;
            // 
            // cbTablesList
            // 
            this.cbTablesList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTablesList.FormattingEnabled = true;
            this.cbTablesList.Location = new System.Drawing.Point(0, 0);
            this.cbTablesList.Name = "cbTablesList";
            this.cbTablesList.Size = new System.Drawing.Size(238, 21);
            this.cbTablesList.TabIndex = 7;
            this.cbTablesList.SelectedIndexChanged += new System.EventHandler(this.cbTablesList_SelectedIndexChanged);
            // 
            // btnObjectPreview
            // 
            this.btnObjectPreview.Enabled = false;
            this.btnObjectPreview.Location = new System.Drawing.Point(10, 187);
            this.btnObjectPreview.Name = "btnObjectPreview";
            this.btnObjectPreview.Size = new System.Drawing.Size(75, 23);
            this.btnObjectPreview.TabIndex = 9;
            this.btnObjectPreview.Text = "Preview";
            this.btnObjectPreview.UseVisualStyleBackColor = true;
            this.btnObjectPreview.Click += new System.EventHandler(this.btnObjectPreview_Click);
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.groupBox2);
            this.panel4.Location = new System.Drawing.Point(13, 105);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(401, 216);
            this.panel4.TabIndex = 10;
            // 
            // KingShaggySalesforceSourceUIForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(416, 365);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "KingShaggySalesforceSourceUIForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "King Shaggy Source Editor";
            this.Load += new System.EventHandler(this.KingShaggySalesforceSourceUIForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.pnlQuerySelection.ResumeLayout(false);
            this.pnlQuerySelection.PerformLayout();
            this.pnlTableSelection.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cbConnectionList;
        private System.Windows.Forms.Label lblConnection;
        private System.Windows.Forms.Button btnNewConnectionManager;
        private System.Windows.Forms.Label lblAccessMode;
        private System.Windows.Forms.ComboBox cbAccessMode;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Panel pnlTableSelection;
        private System.Windows.Forms.ComboBox cbTablesList;
        private System.Windows.Forms.Label lblSelectTable;
        private System.Windows.Forms.Button btnObjectPreview;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel pnlQuerySelection;
        private System.Windows.Forms.TextBox txtBoxQuery;

        #endregion
    }
}