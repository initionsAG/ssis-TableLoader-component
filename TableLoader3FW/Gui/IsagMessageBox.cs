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
    public partial class IsagMessageBox : Form
    {
        public IsagMessageBox()
        {
            InitializeComponent();
        }

        public void SetHelpText(string stream)
        {

            rtfHelp.Rtf = stream; 
           // rtfHelp.LoadFile(stream, RichTextBoxStreamType.RichText);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }
    }
}
