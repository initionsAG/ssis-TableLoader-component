using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Microsoft.SqlServer.Dts.Pipeline.Design;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Runtime.Design;

namespace TableLoader
{
    class TableLoaderUI : IDtsComponentUI
    {
        IDTSComponentMetaData100 _metadata;
        IServiceProvider _serviceProvider;
    
        public bool Edit(IWin32Window parentWindow, Variables variables, Connections connections)
        {
           frmTableLoaderUI frm =new frmTableLoaderUI(connections, _metadata, _serviceProvider, variables);

            return frm.ShowDialog(parentWindow) == DialogResult.OK;
        }
       
        public void Initialize(IDTSComponentMetaData100 dtsComponentMetadata, IServiceProvider serviceProvider)
        {
            _metadata = dtsComponentMetadata;
            _serviceProvider = serviceProvider;
        }

        public void New(IWin32Window parentWindow) {}
        public void Help(IWin32Window parentWindow) {}
        public void Delete(IWin32Window parentWindow) {}

    }
}
