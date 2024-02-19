using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace KingShaggySalesforceSource
{
    public partial class KingShaggyTablePreview : Form
    {
        private DataTable dgvPreviewObject { get; set; }
        public KingShaggyTablePreview(DataTable dataTable)
        {
            InitializeComponent();
            dgvPreviewObject = dataTable;
        }
        private void KingShaggyTablePreview_Load(object sender, EventArgs e)
        {
            if (dgvPreviewObject.Rows.Count != 0)
            {
                previewDataGrid.DataSource = dgvPreviewObject;
            }
        }
    }
}
