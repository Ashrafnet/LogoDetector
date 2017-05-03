using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LogoDetector
{
    [Serializable]
    public class ClarifiConfig:ICloneable
    {
        public string Default_Client_Id { get; set; } = "8ns8IsM7DbE7F9UWn3v6EibebJ0QCxAuKQI_6PQg";
        public string Default_Client_Secret { get; set; } = "YygxUA3MmWG7VfNI1pZ5QGbyTZs4h_anzmog_3nE";
        public string Default_Model_Path { get; set; } = "logo_detector_120";

        public int Batch_Size { get; set; } = 50;
        public string Default_Concept_ID { get; set; } = "logo_70";

        private string _defualt_model_path_issam = "Cropped Logo Detector";
        private string _client_Id_issam = "Wads5t98mMepkTuQFLbQvdYBPH0xIBz_7KqdhMIp";
        private string _client_Secret_issam = "Igqe-qov2dbcS8EQDHoI8DxRR0PiEN3y3Tj4Vu2m";

        private static string configfile
        {
            get
            {
                var assymplpath = Assembly.GetExecutingAssembly().Location;
                return Path.Combine( Path.GetDirectoryName(assymplpath),"logodetector.config");
                 
            }
        }
        public void SaveCurrentConfig()
        {
            WriteToXmlFile<ClarifiConfig>(configfile, this);
        }
        public static ClarifiConfig CurrentConfig()
        {
            ClarifiConfig configs=null ;
            if (File.Exists(configfile))
                configs = ReadFromXmlFile<ClarifiConfig>(configfile);
            else
                configs = null;

            if (configs == null)
                return new ClarifiConfig();

            return configs;

        }
        /// <summary>
        /// Writes the given object instance to an XML file.
        /// <para>Only Public properties and variables will be written to the file. These can be any type though, even other classes.</para>
        /// <para>If there are public properties/variables that you do not want written to the file, decorate them with the [XmlIgnore] attribute.</para>
        /// <para>Object type must have a parameterless constructor.</para>
        /// </summary>
        /// <typeparam name="T">The type of object being written to the file.</typeparam>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the file.</param>
        /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
        private static void WriteToXmlFile<T>(string filePath, T objectToWrite, bool append = false) where T : new()
        {
            TextWriter writer = null;
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                writer = new StreamWriter(filePath, append);
                serializer.Serialize(writer, objectToWrite);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        /// <summary>
        /// Reads an object instance from an XML file.
        /// <para>Object type must have a parameterless constructor.</para>
        /// </summary>
        /// <typeparam name="T">The type of object to read from the file.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the XML file.</returns>
        private static T ReadFromXmlFile<T>(string filePath) where T : new()
        {
            TextReader reader = null;
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                reader = new StreamReader(filePath);
                return (T)serializer.Deserialize(reader);
            }
            catch
            {
                return default(T );
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
