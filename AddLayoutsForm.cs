using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoCadAddLayouts
{
    public partial class AddLayoutsForm : Form
    {
        public AddLayoutsForm()
        {
            InitializeComponent();
        }

        private void AddLayoutsForm_Load(object sender, EventArgs e)
        {
            PopulateListbox(txtPath.Text.Trim());
        }

        private void PopulateListbox(string dwgPath)
        {
            if (!string.IsNullOrEmpty(dwgPath))
            {
                using (var fbd = new FolderBrowserDialog())
                {
                    fbd.SelectedPath = dwgPath;
                    string[] files = Directory.GetFiles(fbd.SelectedPath, "*.dwg");
                    lstDwgs.DataSource = files;
                    lblInfo.Text = $"Total number of drawings: {lstDwgs.Items.Count}";
                }
            }
        }
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    PopulateListbox(fbd.SelectedPath);
                    txtPath.Text = fbd.SelectedPath;
                }
            }
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            AddLayoutsUtil addLayoutsUtil = new AddLayoutsUtil();
            if (lstDwgs.Items.Count > 0)
            {
                addLayoutsUtil.AddLayouts(lstDwgs.Items.Cast<string>());
                Close();
            }
            else
            {
                lblInfo.Text = $"No drawings selected.";
            }
        }
    }
}
