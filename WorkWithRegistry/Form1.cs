using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;
using System.Xml.Linq;

namespace WorkWithRegistry
{
    public partial class Form1 : Form
    {

        List<Inf> infor = new List<Inf>();
        public Form1()
        {
            InitializeComponent();
            show_keys_time();
        }

        private void show_keys_time()
        {
            try
            {
                RegistryKey[] rk = new RegistryKey[] { Registry.ClassesRoot,
                Registry.CurrentUser,
                Registry.LocalMachine,
                Registry.Users,
                Registry.CurrentConfig,
                Registry.PerformanceData};
                foreach (RegistryKey k in rk)
                {
                    string name = k.Name;
                    int con = k.SubKeyCount;
                    var item = new Inf();
                    item.regkey = k;
                    infor.Add(item);
                    listBox1.Items.Add(string.Format("{0} - всего элеменнтов {1}", name, con));
                }
            } catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void show_keys_reusable(int index)
        {
            RegistryKey selfkey;
            int i = 1;
            try
            {
                infor[index].b = true;
                if (infor[index].tabulation == 0)
                    selfkey = infor[index].regkey;
                else
                    selfkey = infor[index].regkey.OpenSubKey(infor[index].path);
                string[] keys = selfkey.GetSubKeyNames();
                foreach (string name in keys)
                {
                    var item = new Inf();
                    item.regkey = infor[index].regkey;
                    if (infor[index].tabulation == 0)
                        item.path = name;
                    else
                        item.path = infor[index].path + @"\" + name;
                    item.tabulation = infor[index].tabulation + 1;
                    string tab = "";
                    for (int j = 0; j < item.tabulation; j++)
                        tab += "  ";
                    listBox1.Items.Insert(index + i, tab + string.Format("{0} - всего элеменнтов {1}", name, infor[index].regkey.OpenSubKey(item.path).SubKeyCount));
                    infor.Insert(index + i, item);
                    infor[index].regkey.OpenSubKey(item.path).Close();
                    i++;
                }
                selfkey.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void hide_subkeys(int index)
        {
            int i = index + 1;
            while (infor[i].tabulation > infor[index].tabulation)
            {
                infor.RemoveAt(i);
                listBox1.Items.RemoveAt(i);
            }
            infor[index].b = false;
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = listBox1.IndexFromPoint(e.Location);
            Console.WriteLine(index);
            if (!infor[index].b)
                show_keys_reusable(index);
            else if (infor[index].b)
                hide_subkeys(index);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                int index = listBox1.SelectedIndex;
                Delete_key(index);
            }
            catch(Exception)
            {
                MessageBox.Show("Выберите ключ!");
            }
        }

        private void Delete_key(int index)
        {
            try
            {
                Delete_all_subkeys(index);
                infor.RemoveAt(index);
                listBox1.Items.RemoveAt(index);
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;
            Creat_key(index);
        }

        private void Creat_key(int index)
        {
            try
            {
                string name = read();
                var item = new Inf();
                RegistryKey key = infor[index].regkey.OpenSubKey(infor[index].path);
                key.CreateSubKey(name);
                key.Close();
                item.regkey = infor[index].regkey;
                item.path = infor[index].path + name;
                item.tabulation = infor[index].tabulation + 1;
                infor.Insert(index + 1, item);
                string tab = "";
                for (int j = 0; j < item.tabulation; j++)
                    tab += "  ";
                listBox1.Items.Insert(index + 1, tab + name);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private string read()
        {
            string text = textBox1.Text;
            if(text.Length != 0)
            {
                return text;
            }
            else
            {
                return "NoName";
            }
        }

        private void Delete_all_subkeys(int index)
        {
            try
            {
                int i = infor[index].regkey.OpenSubKey(infor[index].path).SubKeyCount;
                if (i > 0)
                    Delete_all_subkeys(index + 1);
                else
                {
                    string pathwithoutname = infor[index].path;
                    RegistryKey key = infor[index].regkey.OpenSubKey(pathwithoutname);
                    key.DeleteSubKey(listBox1.Items[index].ToString());
                    key.Close();
                    i = pathwithoutname.LastIndexOf(@"\");
                    pathwithoutname = pathwithoutname.Substring(0, i);
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
                return;
            }
        }

    }
    public class Inf{
            public string path = "";
            public RegistryKey regkey = null;
            public bool b = false;
            public int tabulation = 0;
            public override string  ToString()
                    {
 	                    return path;
                    }
        }
}
