using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.SqlServer.Dts.Design;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Runtime.Design;

namespace KingShaggySFConnectionManager
{
    class KingShaggySFConnectionManagerUI : IDtsConnectionManagerUI
    {
        private IServiceProvider serviceProvider;
        private ConnectionManager connectionManager;

        public void Delete(IWin32Window parentWindow)
        {
            //throw new NotImplementedException();
        }

        public bool Edit(IWin32Window parentWindow, Connections connections, ConnectionManagerUIArgs connectionUIArg)
        {
            return EditConnection(parentWindow);
        }

        public void Initialize(ConnectionManager connectionManager, IServiceProvider serviceProvider)
        {
            this.connectionManager = connectionManager;
            this.serviceProvider = serviceProvider;
        }
        public bool New(IWin32Window parentWindow, Connections connections, ConnectionManagerUIArgs connectionUIArg)
        {
            //Try to implement below.
            //[Obsolete("Use Microsoft.SqlServer.IntegrationServices.Designer.Model.IClipboardService instead")]
            IDtsClipboardService clipboardService;

            clipboardService = (IDtsClipboardService)serviceProvider.GetService(typeof(IDtsClipboardService));
            if (clipboardService != null)
            {
                if (clipboardService.IsPasteActive)
                {
                    return true;
                }
            }

            return EditConnection(parentWindow);
        }

        private bool EditConnection(IWin32Window parentWindow)
        {
            KingShaggySFConnectionManagerUIForm frm = new KingShaggySFConnectionManagerUIForm(connectionManager, serviceProvider);

            var result = frm.ShowDialog();

            if (result == DialogResult.OK)
                return true;

            return false;
        }
    }
}
