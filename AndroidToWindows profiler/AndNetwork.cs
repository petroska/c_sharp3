using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Xml.XPath;

namespace AndroidToWindows_profiler
{
    public class AndNetwork
    {
        public int networkId;
        public String ssid;
        public String encryption;
        public String password;

        public AndNetwork(int _networkId, String _ssid, String _encryption, String _password)
        {
            networkId = _networkId;
            ssid = _ssid;
            encryption = _encryption;
            password = _password;
        }

        public string export()
        {
            String filename = ssid.Replace(' ', '_').ToLower();
            filename = ssid.Replace('|', '_').ToLower();
            filename = Regex.Replace(filename, @"[^0-9a-zA-Z]+", "_");
            

            byte[] str_ssid = Encoding.Default.GetBytes("ssid");
            var hex_ssid = BitConverter.ToString(str_ssid);
            hex_ssid = hex_ssid.Replace("-", "");

            String auth = "";
            String encr = "";
            if (encryption.Equals("NONE"))
            {
                auth = "open";
                encr = "none";
            }
            else if (encryption.Equals("WPA-PSK"))
            {
                auth = "WPA2PSK";
                encr = "AES";
            }
            else if (encryption.Equals("WPA-EAP IEEE8021X"))
            {
                auth = "un-supported";
                encr = "un-supported";
            }
            
            XNamespace wlanNM = "http://www.microsoft.com/networking/WLAN/profile/v1";
            string CurrPath = "";
            XDocument xDoc = null;
            StringWriter sw = new StringWriter();
            XmlWriter xWrite = null;
            CurrPath = Directory.GetCurrentDirectory();
            string exportedPath = CurrPath + "\\networks\\" + filename + "_android_" + networkId + ".xml";

            if (auth.Equals("open"))
            {
                xDoc = new XDocument(
                            new XDeclaration("1.0", "utf-8", null),
                            new XElement(wlanNM + "WLANProfile",
                                new XElement(wlanNM + "name", ssid),
                                    new XElement(wlanNM + "SSIDConfig",
                                        new XElement(wlanNM + "SSID",
                                            new XElement(wlanNM + "hex", hex_ssid),
                                            new XElement(wlanNM + "name", ssid)
                                    )),
                                    new XElement(wlanNM + "connectionType", "ESS"),
                                    new XElement(wlanNM + "connectionMode", "auto"),
                                new XElement(wlanNM + "MSM",
                                    new XElement(wlanNM + "security",
                                        new XElement(wlanNM + "authEncryption",
                                            new XElement(wlanNM + "authentication", auth),
                                            new XElement(wlanNM + "encryption", encr),
                                            new XElement(wlanNM + "useOneX", "false")
                                            )
                                        ))),
                                    new XComment("This xml was created by Petros_Karakonstantis Android to Windows profiler")
                                    );


                sw = new StringWriter();
                xWrite = XmlWriter.Create(sw);
                xDoc.Save(xWrite);
                xWrite.Close();
                
                xDoc.Save(exportedPath);
            }
            else if (auth.Equals("WPA2PSK"))
            {
                xDoc = new XDocument(
                    new XDeclaration("1.0", "utf-8", null),
                    new XElement(wlanNM + "WLANProfile",
                        new XElement(wlanNM + "name", ssid),
                            new XElement(wlanNM + "SSIDConfig",
                                new XElement(wlanNM + "SSID",
                                    new XElement(wlanNM + "hex", hex_ssid),
                                    new XElement(wlanNM + "name", ssid)
                                    )),
                                    new XElement(wlanNM + "connectionType", "ESS"),
                                    new XElement(wlanNM + "connectionMode", "auto"),
                                new XElement(wlanNM + "MSM",
                                    new XElement(wlanNM + "security",
                                        new XElement(wlanNM + "authEncryption",
                                            new XElement(wlanNM + "authentication", auth),
                                            new XElement(wlanNM + "encryption", encr),
                                            new XElement(wlanNM + "useOneX", "false")
                                            ),
                                            new XElement(wlanNM + "sharedKey",
                                        new XElement(wlanNM + "keyType", "passPhrase"),
                                        new XElement(wlanNM + "protected", "false"),
                                        new XElement(wlanNM + "keyMaterial", password)
                                        )))),
                                     new XComment("This xml was created by Petros_Karakonstantis Android to Windows profiler")
                                    );

                sw = new StringWriter();
                xWrite = XmlWriter.Create(sw);
                xDoc.Save(xWrite);
                xWrite.Close();
                

                xDoc.Save(exportedPath);
            }
            else if(auth.Equals("un-supported"))
            {
                //802.1x
            }
            return exportedPath;
        }
    }
}