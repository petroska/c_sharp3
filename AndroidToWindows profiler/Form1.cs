using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Xml.XPath;
using System.Diagnostics;

namespace AndroidToWindows_profiler
{



    public partial class Form1 : Form
    {
        private List<string> FolderXml;

        public string ExportCommand(string _path, string _interface, string _profilename)
        {    

            string command = "/c " + "netsh wlan export profile \"" + _profilename + "\" folder = \"" + _path + "\" key=clear interface=\"" + _interface + "\"";
            System.Diagnostics.ProcessStartInfo procStartInfo =
            new System.Diagnostics.ProcessStartInfo("cmd", command);

            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.UseShellExecute = false;
            procStartInfo.CreateNoWindow = true;

            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo = procStartInfo;
            proc.Start();

            string result = proc.StandardOutput.ReadToEnd();

            proc.WaitForExit();
            //return result;
          return command + result;
        }

        public string ImportProfile(string _interface, string _filename, string _user)
        {
            string command = "netsh wlan add profile filename = \"" + _filename + "\" Interface = \"" + _interface + "\" user = " + _user;
            System.Diagnostics.ProcessStartInfo procStartInfo =
            new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command);
        
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.UseShellExecute = false;
            procStartInfo.CreateNoWindow = true;
        
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo = procStartInfo;
            proc.Start();
            string result = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();
            return command + result;
          //return result + command;
        }

        public List<string> getInstalledProfiles(string _interface)
        //public string getInstalledProfiles(string _interface)
        {
            string command = "netsh wlan show profiles interface=\"" + _interface + "\"";
            System.Diagnostics.ProcessStartInfo procStartInfo =
            new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command);
            List<string> InstalledProfiles = new List<string> { };
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.UseShellExecute = false;
            procStartInfo.CreateNoWindow = true;

            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo = procStartInfo;
            proc.Start();
            string result = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();

            //string[] lines = result.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            //string[] nets = "";
            string[] lines = Regex.Split(result, ".*\\bUser Profile\\b\\s*:\\s");
            for (int i = 1; i < lines.Length; i++)
            {

                InstalledProfiles.Add(lines[i].Trim('\n','\r'));
                textBox4.AppendText("Found " + lines[i] + " network installed.");
                textBox4.AppendText(Environment.NewLine);
            }
       

            //return result;
            return InstalledProfiles;
            //return result + command;
        }

        public void checkForNetorksDir()
        {

            if (Directory.Exists("networks"))
            {
                textBox1.Text = Directory.GetCurrentDirectory() + "\\networks";
                FolderXml = findXMLs(Directory.GetCurrentDirectory() + "\\networks");
                button2.Enabled = true;

                textBox2.Text = Directory.GetCurrentDirectory() + "\\networks";
                if (radioButton2.Checked | radioButton1.Checked)
                    button4.Enabled = true;
            }
        }

        public List<string> findXMLs(string _path)
        {
            string[] files = Directory.GetFiles(_path);
            List<string> profileFiles = new List<string> { };
            string tmp = "";
            int netCounter = 0;
            foreach(var item in files)
            {
                tmp = item.ToString();
                tmp = tmp.Substring(  tmp.Length-4  ,   4  );
                if (tmp.Equals(".xml"))
                {
                    netCounter++;
                    profileFiles.Add(item);
                    //textBox4.AppendText(item.ToString());
                    //textBox4.AppendText(Environment.NewLine);
                }
            }
            return profileFiles;
        }

        public Form1()
        {
            InitializeComponent();
            
            comboBox1.Sorted = true;
            comboBox2.Sorted = true;
            comboBox3.Sorted = true;
            checkForNetorksDir();
            
            int wlanCounter = 0;
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in adapters)
            {
                IPInterfaceProperties properties = adapter.GetIPProperties();
                //string tmp = "";
                
                if(adapter.NetworkInterfaceType.ToString().Contains("Wireless"))
                //if (adapter.Name.Contains("Wireless Network"))
                {
                    comboBox1.Items.Add(adapter.Name);
                    comboBox2.Items.Add(adapter.Name);
                    comboBox3.Items.Add(adapter.Name);
                    wlanCounter++;
                }
                else
                {
                    //comboBox1.SelectedIndex = 0;
                    //comboBox1.Items.Add("Unsuported adapter");
                }
            }
            if (wlanCounter == 0)
            {
                MessageBox.Show("No Wireless adapters found, do you even Wifi bro?");
                button1.Enabled = false;
                button2.Enabled = false;
                button3.Enabled = false;
                button4.Enabled = false;
                radioButton1.Enabled = false;
                radioButton2.Enabled = false;
            }
            else    //EDO OI ENTOLES APO THN STIGMH POU YPARXOYN WIRELESS INTERFACES
            {
                comboBox1.SelectedIndex = 0;
                comboBox2.SelectedIndex = 0;
                comboBox3.SelectedIndex = 0;
                checkForNetorksDir();
                
            }

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        public void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
                
                //string[] files = Directory.GetFiles(folderBrowserDialog1.SelectedPath);
                List<string> profiles = new List<string> { };
                profiles = findXMLs(folderBrowserDialog1.SelectedPath);
                
                //FolderXml = profileFiles;
                toolStripStatusLabel1.Text = "Folder selected, found " + profiles.Count + " profiles.";
                //importProfile(profiles, comboBox1.SelectedItem.ToString());
                
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string result3 = "";
            string ImportOK = " is added on ";

            for (int i = 0; i < FolderXml.Count; i++)
            {
                if(radioButton8.Checked == true)
                {
                    result3 = result3 + ImportProfile(comboBox1.SelectedItem.ToString(), FolderXml[i], "all");
                    textBox4.AppendText(result3);
                }
                else if(radioButton7.Checked == true)
                {
                    result3 = result3 + ImportProfile(comboBox1.SelectedItem.ToString(), FolderXml[i], "current");
                    textBox4.AppendText(result3);
                }
                
            }
            //textBox4.AppendText(item.ToString());
            //textBox4.AppendText(Environment.NewLine);
            int ImpPofiles = new Regex(Regex.Escape(ImportOK)).Matches(result3).Count;
            toolStripStatusLabel1.Text = "Imported successfully " + ImpPofiles + " profiles.";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult result2 = folderBrowserDialog2.ShowDialog();
            if (result2 == DialogResult.OK)
            {
                // export logic
                //string[] files2 = Directory.GetFiles(folderBrowserDialog2.SelectedPath);
                textBox2.Text = folderBrowserDialog2.SelectedPath;
                if (radioButton2.Checked)
                    button4.Enabled = true;
                toolStripStatusLabel1.Text = "folder selected";
                //MessageBox.Show("Files found: " + files2.Length.ToString(), "Message");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string filename = "";

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.FileName = "wpa_supplicant";
            ofd.Filter = "Config Files |*.conf";
            DialogResult dr = ofd.ShowDialog();

            if (dr == DialogResult.OK)
            {
                filename = ofd.FileName;
                textBox3.Text = filename;
                toolStripStatusLabel1.Text = "file selected";
                if (radioButton3.Checked | radioButton4.Checked)
                    button6.Enabled = true;
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (!textBox3.Text.Equals(""))
                button6.Enabled = true;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (!textBox3.Text.Equals(""))
                button6.Enabled = true;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //CONVERT AND EXPORT CODE HERE
            //..............

            if (!Directory.Exists("networks"))
                Directory.CreateDirectory("networks");
            else
            {
                textBox4.AppendText("Directory networks exists!");
                textBox4.AppendText(Environment.NewLine);
            }

            int counter = 0;
            int netId = 0;
            int unsupported = 0;
            string line;
            string tmp_ssid = "";
            string tmp_encr = "";
            string tmp_pass = "";
            List<AndNetwork> AndNetworks = new List<AndNetwork>();
            List<String> ConvertedPath = new List<string>();

            // Read the file and display it line by line.
            //System.IO.StreamReader file = new System.IO.StreamReader("..\\..\\wpa_supplicant.conf");
            System.IO.StreamReader file = new System.IO.StreamReader(textBox3.Text);
            while ((line = file.ReadLine()) != null)
            {
                if (line.StartsWith("network={"))
                {
                    while (!line.StartsWith("}"))
                    {
                        if (line.StartsWith("\tssid="))
                        {
                            tmp_ssid = line.Substring(7);
                            tmp_ssid = tmp_ssid.TrimEnd('"');
                        }
                        else if (line.StartsWith("\tpsk"))
                        {
                            tmp_pass = line.Substring(6);
                            tmp_pass = tmp_pass.TrimEnd('"');
                        }
                        else if (line.StartsWith("\tkey_mgmt"))
                            tmp_encr = line.Substring(10);

                        line = file.ReadLine();
                    }

                    counter++;

                    if (tmp_encr.Equals("NONE"))
                    {
                        netId++;
                        AndNetworks.Add(new AndNetwork(netId, tmp_ssid, tmp_encr, "NONE"));
                    }
                    else if (tmp_encr.Equals("WPA-PSK"))
                    {
                        netId++;
                        AndNetworks.Add(new AndNetwork(netId, tmp_ssid, tmp_encr, tmp_pass));
                    }
                    else
                        unsupported++;

                    tmp_ssid = "";
                    tmp_encr = "";
                    tmp_pass = "";
                }
                textBox4.AppendText("network #" + counter + " parsed.");
                textBox4.AppendText(Environment.NewLine);
                toolStripStatusLabel1.Text = "Parsed " + counter + " networks from file.";
                
            }

            textBox4.AppendText("Unsupported netowrks found " + unsupported);
            textBox4.AppendText(Environment.NewLine);
            textBox4.AppendText("Total netowrks found " + counter);
            textBox4.AppendText(Environment.NewLine);
            //textBox4.AppendText("Total netowrks added to the list " + AndNetworks.Count);
            //textBox4.AppendText(Environment.NewLine);
            file.Close();
            string result2 = "";
            if (radioButton3.Checked == true)
                for (int i = 0; i < AndNetworks.Count; i++)
                    ConvertedPath.Add(AndNetworks[i].export());
            else if (radioButton4.Enabled == true)
            {
                for (int i = 0; i < AndNetworks.Count; i++)
                    ConvertedPath.Add(AndNetworks[i].export());
                for (int i = 0; i < AndNetworks.Count; i++)
                {
                    if(radioButton5.Checked == true)
                        result2 = ImportProfile(comboBox3.SelectedItem.ToString(), ConvertedPath[i], "all");
                    else if(radioButton6.Checked == true)
                        result2 = ImportProfile(comboBox3.SelectedItem.ToString(), ConvertedPath[i], "current");
                    textBox4.AppendText("Convert and Imported: " + result2);
                    textBox4.AppendText(Environment.NewLine);
                }
            }
        }

        private void toolStripProgressBar1_Click(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (!textBox2.Text.Equals(""))
                button6.Enabled = true;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (!textBox2.Text.Equals(""))
                button4.Enabled = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string executedOK = " successfully.";
            string curPath = textBox2.Text;
            string Winterface = comboBox2.SelectedItem.ToString();
            
            List<string> InstalledProfiles = getInstalledProfiles(comboBox2.SelectedItem.ToString());
            string cmdresult = "";
            for(int i=0; i < InstalledProfiles.Count(); i++)
                cmdresult = cmdresult + ExportCommand(curPath, Winterface, InstalledProfiles[i]);
           
            int expPofiles = new Regex(Regex.Escape(executedOK)).Matches(cmdresult).Count;
            toolStripStatusLabel1.Text = "Exported successfully " + expPofiles + " profiles.";
            textBox4.AppendText("CMD success: " + cmdresult);
            textBox4.AppendText(Environment.NewLine);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Form1.ActiveForm.Width = 645;
            button7.Visible = false;
            button8.Visible = true;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Form1.ActiveForm.Width = 362;
            button8.Visible = false;
            button7.Visible = true;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://petros.karakonstantis.gr/");
        }
    }
}
