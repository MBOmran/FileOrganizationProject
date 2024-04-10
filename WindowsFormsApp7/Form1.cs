using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp7
{
    public partial class Form1 : Form
    {
        ImageList imagelist1;
        string sortColumn = null;
        bool isAscending;
        public void ShowDrives()
        {
            treeView1.BeginUpdate();
            string[] Drives = Directory.GetLogicalDrives();
            foreach (string drive in Drives)
            {
                TreeNode node = new TreeNode(drive);
                treeView1.Nodes.Add(node);
                AddDirectories(node);
            }
            treeView1.EndUpdate();
        }
        public void ShowFileNames(string sortColumn)
        {
            DirectoryInfo di = new DirectoryInfo(treeView1.SelectedNode.FullPath);
            FileInfo[] filearray = { };
            ListViewItem item;
            imagelist1 = new ImageList();
            listView1.Items.Clear();
            listView1.SmallImageList = imagelist1;
            if (di.Exists)
            {
                filearray = di.GetFiles();
            }
            switch (sortColumn)
            {
                case "Name":
                    filearray = filearray.OrderBy(f => f.Name).ToArray();
                    break;
                case "Size":
                    filearray = filearray.OrderBy(f => f.Length).ToArray();
                    break;
                case "LastWriteTime":
                    filearray = filearray.OrderBy(f => f.LastWriteTime).ToArray();
                    break;
                default:
                    break;
            }
            listView1.BeginUpdate();
            foreach (FileInfo filei in filearray)
            {
                Icon icon = SystemIcons.WinLogo;
                item = new ListViewItem(filei.Name);
                listView1.Items.Add(item);
                if (!imagelist1.Images.ContainsKey(filei.Extension))
                {
                    icon = System.Drawing.Icon.ExtractAssociatedIcon(filei.FullName);
                    imagelist1.Images.Add(filei.Extension, icon);
                }
                item.SubItems.Add((filei.Length / 1048576).ToString() + " Megabytes");
                item.ImageKey = filei.Extension;
                item.SubItems.Add(filei.LastWriteTime.ToString());
                item.SubItems.Add(GetAttrs(filei));
                
            }                
            listView1.EndUpdate();

        }
        public void AddDirectories(TreeNode node)
        {
            string path = node.FullPath;
            DirectoryInfo di = new DirectoryInfo(path);
            DirectoryInfo[] diarray = { };
            try
            {
                if (di.Exists)
                {
                    diarray = di.GetDirectories();
                }
            }

            catch
            {
                return;
            }
            foreach (DirectoryInfo d in diarray)
            {
                TreeNode nodedir = new TreeNode(d.Name);
                node.Nodes.Add(nodedir);
            }
        }
        public string GetAttrs(FileInfo filei)
        {
            string attribute = "";
            if ((filei.Attributes & FileAttributes.Archive) != 0)
            {
                attribute += "A";
            }
            else if ((filei.Attributes & FileAttributes.Hidden) != 0)
            {
                attribute += "H";

            }
            else if ((filei.Attributes & FileAttributes.ReadOnly) != 0)
            {
                attribute += "R";

            }
            else if ((filei.Attributes & FileAttributes.System) != 0)
            {
                attribute += "S";

            }
            return attribute;
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ShowDrives();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ShowFileNames(null);
        }

        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            treeView1.BeginUpdate();
            foreach (TreeNode node in e.Node.Nodes)
            {
                AddDirectories(node);
            }
            treeView1.EndUpdate();
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            string diskfile = treeView1.SelectedNode.FullPath;
            if (!diskfile.EndsWith("\\"))
            {
                diskfile += "\\";
            }
            diskfile += listView1.FocusedItem.Text;
            if (File.Exists(diskfile))
            {
                Process.Start(diskfile);
            }
        }

        public class FileNameComparer : IComparer<FileInfo>
        {
            public int Compare(FileInfo x, FileInfo y)
            {
                return x.Name.CompareTo(y.Name);
            }
        }

        public class FileSizeComparer : IComparer<FileInfo>
        {
            public int Compare(FileInfo x, FileInfo y)
            {
                return x.Length.CompareTo(y.Length);
            }
        }

        public class LastModifiedComparer : IComparer<FileInfo>
        {
            public int Compare(FileInfo x, FileInfo y)
            {
                return x.LastWriteTime.CompareTo(y.LastWriteTime);
            }
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {

            // Determine the clicked column based on the index (adjust based on actual column order)
            switch (e.Column)
            {
                case 0:
                    sortColumn = "Name";
                    break;
                case 1:
                    sortColumn = "Size";
                    break;
                case 2:
                    sortColumn = "LastWriteTime";
                    break;
            }
            listView1.Items.Clear();
            ShowFileNames(sortColumn);
        }
    }
}
  
