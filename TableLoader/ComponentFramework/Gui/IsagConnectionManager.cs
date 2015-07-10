using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using System.Data.SqlClient;
using Microsoft.SqlServer.Dts.Runtime.Design;
using System.Collections;

namespace ComponentFramework.Gui
{
    /// <summary>
    /// A user control for choosing an ADO:NET connection manager. 
    /// It is also possible to create and select a new one.
    /// 
    /// </summary>
    public partial class IsagConnectionManager : UserControl
    {
        public event EventHandler ConnectionManagerChanged;
        public IServiceProvider ServiceProvider { get; set; }
        public Connections ComponentConnections { get; set; }
        public IDTSComponentMetaData100 ComponentMetaData { get; set; }
        public string ConnectionManagerName { get; set; }
        private BindingList<ConnectionManager> _connectionManagerList = new BindingList<ConnectionManager>();
        public string ConnectionManager { get { return ((ConnectionManager) cmbConMgr.SelectedItem).Name; } }
        public SqlConnection SelectedConnection
        {
            get
            {
                if (HasConnection())
                    return (SqlConnection)(((ConnectionManager)cmbConMgr.SelectedItem).AcquireConnection(null));
                else return null;
            }
        }
        /// <summary>
        /// the constructor
        /// </summary>
        /// <param name="componentMetaData">the components metadata</param>
        /// <param name="serviceProvider">"service provider of the component</param>
        /// <param name="connections">all connections of the component</param>
        /// <param name="connectionManagerName">the name of the components connection manager</param>
        public void Initialize(IDTSComponentMetaData100 componentMetaData, IServiceProvider serviceProvider, Connections connections, string connectionManagerName)
        {
            //InitializeComponent();         

            ComponentMetaData = componentMetaData;
            ServiceProvider = serviceProvider;
            ComponentConnections = connections;
            ConnectionManagerName = connectionManagerName;

            InitializeConnectionManager();
        }

        private void InitializeConnectionManager()
        {
            foreach (ConnectionManager connMgr in ComponentConnections)
            {
                if (connMgr.CreationName.StartsWith("ADO.NET")) _connectionManagerList.Add(connMgr);
            }

            cmbConMgr.DisplayMember = "Name";
            cmbConMgr.DataSource = _connectionManagerList;

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

        public bool HasConnection()
        {
            return cmbConMgr.SelectedItem != null;
        }


        public IsagConnectionManager()
        {
            InitializeComponent();
        }


        private void btnNewConMgr_Click(object sender, EventArgs e)
        {
            IDtsConnectionService connectionService =
                    (IDtsConnectionService)ServiceProvider.GetService(typeof(IDtsConnectionService));
            ArrayList newConnections = connectionService.CreateConnection("ADO.NET");


            foreach (ConnectionManager connMgr in newConnections)
            {
                if (connMgr.CreationName.StartsWith("ADO.NET"))
                {
                    _connectionManagerList.Add(connMgr);
                    cmbConMgr.SelectedItem = connMgr;
                }
            }
        }

        private void cmbConMgr_SelectedValueChanged(object sender, EventArgs e)
        {
            if (this.ConnectionManagerChanged != null) this.ConnectionManagerChanged(sender, e);
        }
    }
}
