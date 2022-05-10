using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SecurityPlusManager
{
    /*
     * Alright, so this is a program that lets me read the .spl log files
     * It's really just a glorified decryption text box, but I had fun making it
     *
     * Most of the code in this file is quite self-explanatory if you know C#,
     * and if you don't, then I'm sorry but you need to learn that first
     */
    public partial class Form1 : Form
    {
        protected Crypto decryptor = new Crypto();
        
        public Form1(string pathToFile)
        {
            InitializeComponent();
            if (pathToFile != "noFile")
            {
                LoadFile(pathToFile);
            }

            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(Form1_DragEnter);
            this.DragDrop += new DragEventHandler(Form1_DragDrop);

            txtMain.AllowDrop = true;
            txtMain.DragEnter += new DragEventHandler(Form1_DragEnter);
            txtMain.DragDrop += new DragEventHandler(Form1_DragDrop);
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            LoadFile(files[0]);
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            this.Menu = new MainMenu();
            MenuItem item = new MenuItem("File");
            this.Menu.MenuItems.Add(item);
            item.MenuItems.Add("Open", new EventHandler(OpenClicked));
            item.MenuItems.Add("Exit", new EventHandler(ExitClicked));
        }

        private void LoadFile(string path)
        {
            foreach (string line in File.ReadLines(path))
            {
                txtMain.AppendText(decryptor.Decrypt(line));
                txtMain.AppendText("\n");
            }
        }
        
        private void OpenClicked(object sender, EventArgs e)
        {
            txtMain.Multiline = true;
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.InitialDirectory = "C:\\";
            dialog.Title = "Select a .spl file";
            dialog.DefaultExt = "spl";
            dialog.Filter = "spl file (*.spl)|*.spl";
            dialog.CheckFileExists = true;
            dialog.CheckPathExists = true;
            
            DialogResult dialogResult = dialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                string path = dialog.FileName;
                foreach (string line in File.ReadLines(path))
                {
                    if (decryptor.Decrypt(line) != null)
                    {
                        txtMain.AppendText(decryptor.Decrypt(line));
                        txtMain.AppendText("\n");
                    }
                }
            }
        }
        
        private void ExitClicked(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}