using UnityEngine;

using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;

namespace RealisticVehicleSpeed
{
    [Serializable]
    public class Configuration
    {
        [XmlAttribute]
        public string version;

        [XmlAttribute, DefaultValue(true)]
        public bool highwaySpeed = true;

        public void Serialize(string filename)
        {
            try
            {
                using (FileStream stream = new FileStream(filename, FileMode.OpenOrCreate))
                {
                    stream.SetLength(0); // Emptying the file !!!
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(Configuration));
                    xmlSerializer.Serialize(stream, this);
                    DebugUtils.Log("Configuration saved");
                }
            }
            catch (Exception e)
            {
                DebugUtils.Warning("Couldn't save configuration at \"" + Directory.GetCurrentDirectory() + "\"");
                Debug.LogException(e);
            }
        }

        public void Deserialize(string filename)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Configuration));
            Configuration config = null;

            try
            {
                // Trying to Deserialize the configuration file
                using (FileStream stream = new FileStream(filename, FileMode.Open))
                {
                    config = xmlSerializer.Deserialize(stream) as Configuration;
                }
            }
            catch (Exception e)
            {
                // Couldn't Deserialize (XML malformed?)
                DebugUtils.Warning("Couldn't load configuration (XML malformed?)");
                Debug.LogException(e);

                config = null;
            }
        }
    }
}
