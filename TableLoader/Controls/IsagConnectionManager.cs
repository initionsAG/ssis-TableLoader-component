using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Microsoft.SqlServer.Dts.Runtime.Design;
using System.Collections;
using Microsoft.SqlServer.Dts.Runtime;
using System.Data.SqlClient;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.Design;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;

namespace TableLoader
{
    public partial class IsagConnectionManager : IsagUltraComboEditor
    {

        public IServiceProvider ServiceProvider { get; set; }
        public Connections ComponentConnections { get; set; }
        public IDTSComponentMetaData100 ComponentMetaData { get; set; }
        public string ConnectionManagerName { get; set; }
        private List<ConnectionManager> _connectionManagerList = new List<ConnectionManager>();

        public SqlConnection SelectedConnection
        {
            get
            {
                if (HasConnection())
                    return (SqlConnection)(((ConnectionManager)this.SelectedItem.ListObject).AcquireConnection(null));
                else return null;
            }
        }

        public IsagConnectionManager(IDTSComponentMetaData100 componentMetaData, IServiceProvider serviceProvider, Connections connections, string connectionManagerName)
        {
            InitializeComponent();

            this.Dock = DockStyle.Fill;
            this.ButtonsRight.Add(new EditorButton() {Text = "New", ButtonStyle = Infragistics.Win.UIElementButtonStyle.Office2007RibbonButton});
            this.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;

            ComponentMetaData = componentMetaData;
            ServiceProvider = serviceProvider;
            ComponentConnections = connections;
            ConnectionManagerName = connectionManagerName;

            this.EditorButtonClick += new EditorButtonEventHandler(IsagConnectionManager_EditorButtonClick);
        }

        protected override void InitLayout()
        {
            base.InitLayout();

            InitializeConnectionManager();
        }

        private void InitializeConnectionManager()
        {
            foreach (ConnectionManager connMgr in ComponentConnections)
            {
                if (connMgr.CreationName.StartsWith("ADO.NET")) _connectionManagerList.Add(connMgr);
            }

            this.DataSource = _connectionManagerList;
            this.DisplayMember = "Name";
            this.ValueMember = "Name";
            this.DataBind();

            try
            {
                IDTSRuntimeConnection100 connection = ComponentMetaData.RuntimeConnectionCollection[ConnectionManagerName];
                if (connection != null && ComponentConnections.Contains(connection.ConnectionManagerID))
                {
                    this.Text = ComponentConnections[connection.ConnectionManagerID].Name;
                }
            }
            catch (Exception)
            {
                
                //Es wurde noch kein ConnectionManager ausgewählt
            }

        }


        private void IsagConnectionManager_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            IDtsConnectionService connectionService =
                    (IDtsConnectionService)ServiceProvider.GetService(typeof(IDtsConnectionService));
            ArrayList newConnections = connectionService.CreateConnection("ADO.NET");


            foreach (ConnectionManager connMgr in newConnections)
            {
                if (connMgr.CreationName.StartsWith("ADO.NET"))
                {
                    _connectionManagerList.Add(connMgr);
                    this.DataBind();
                    this.Text = connMgr.Name;
                }
            }
        }

        public bool HasConnection()
        { 
            return this.SelectedItem.ListObject != null;
        }
    }
}
