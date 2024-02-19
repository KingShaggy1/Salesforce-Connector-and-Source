using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.SqlServer.Dts.Pipeline.Design;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Runtime.Design;

namespace KingShaggySalesforceSource
{
    class KingShaggySalesforceSourceUI : IDtsComponentUI
    {
        private IServiceProvider serviceProvider;
        private IDTSComponentMetaData100 metaData;
        private IDtsConnectionService connectionService;

        public void Delete(IWin32Window parentWindow)
        {
        }
        public bool Edit(IWin32Window parentWindow, Variables variables, Connections connections)
        {
            ShowForm(parentWindow);

            return true;
        }

        public void Help(IWin32Window parentWindow)
        {
        }

        private DialogResult ShowForm(IWin32Window window)
        {
            KingShaggySalesforceSourceUIForm form = new KingShaggySalesforceSourceUIForm(metaData, serviceProvider);

            return form.ShowDialog(window);
        }
        public void Initialize(IDTSComponentMetaData100 dtsComponentMetadata, IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.metaData = dtsComponentMetadata;

            this.connectionService = (IDtsConnectionService)serviceProvider.GetService(typeof(IDtsConnectionService));
        }

        public void New(IWin32Window parentWindow)
        {
            //Commented below out because it shows form on drag and drop source component.
            //ShowForm(parentWindow);
        }
    }
}
