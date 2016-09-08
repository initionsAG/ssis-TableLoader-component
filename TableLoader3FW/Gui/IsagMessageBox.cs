using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace ComponentFramework.Gui
{
    /// <summary>
    /// A MessageBox shwoing rtf streams
    /// </summary>
    public partial class IsagMessageBox : Form
    {
        /// <summary>
        /// constructor
        /// </summary>
        public IsagMessageBox()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sets content of RichTextBox
        /// </summary>
        /// <param name="stream">the stream containing the rtf</param>
        public void SetHelpText(string stream)
        {

            rtfHelp.Rtf = stream; 
        }

        /// <summary>
        /// React on button OK click: close window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }
    }
}
