using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace NullVoidCreations.WpfHelpers
{
    public class SettingsManager
    {
        readonly Dictionary<string, string> _settings;
        string _fileName, _password;

        #region constructor/destructor

        public SettingsManager()
        {
            _settings = new Dictionary<string, string>();
        }

        ~SettingsManager()
        {
            _settings.Clear();
        }

        #endregion

        public void Load(string fileName, string password)
        {
            if (!File.Exists(fileName))
                return;

            _fileName = fileName;
            _password = password;
            var decryptedSettings = File.ReadAllText(fileName).Decrypt(password);

            var document = new XmlDocument();
            document.LoadXml(decryptedSettings);

            _settings.Clear();
            foreach(XmlNode node in document.SelectNodes("//Setting"))
            {
                var key = node.Attributes["Key"].Value;
                var value = node.Attributes["Value"].Value;

                _settings.Add(key, value);
            }
        }

        public void Save()
        {
            if (string.IsNullOrEmpty(_fileName))
                return;

            Save(_fileName, _password);
        }

        public void Save(string fileName, string password)
        {
            _fileName = fileName;
            _password = password;

            var document = new XmlDocument();
            var rootNode = document.CreateElement("Settings");

            XmlElement tempNode;
            XmlAttribute tempAttribute;
            foreach(var key in _settings.Keys)
            {
                tempNode = document.CreateElement("Setting");

                tempAttribute = document.CreateAttribute("Key");
                tempAttribute.Value = key;
                tempNode.Attributes.Append(tempAttribute);

                tempAttribute = document.CreateAttribute("Value");
                tempAttribute.Value = _settings[key].ToString();
                tempNode.Attributes.Append(tempAttribute);

                rootNode.AppendChild(tempNode);
            }

            document.AppendChild(rootNode);

            using (var stringWriter = new StringWriter())
            using (var xmlWriter = XmlWriter.Create(stringWriter))
            {
                document.WriteTo(xmlWriter);
                xmlWriter.Flush();

                var encryptedSettings = stringWriter.GetStringBuilder().ToString().Encrypt(password);
                
                var fileInfo = new FileInfo(fileName);
                if (fileInfo.Exists)
                    fileInfo.Delete();
                else if (!fileInfo.Directory.Exists)
                    Directory.CreateDirectory(fileInfo.DirectoryName);

                File.WriteAllText(fileName, encryptedSettings);
            }
        }

        public T GetValue<T>(string key, T defaultValue = default(T))
        {
            if (_settings.ContainsKey(key))
                return (T) Convert.ChangeType(_settings[key], typeof(T));
            else
                return defaultValue;
        }

        public void SetValue<T>(string key, T value)
        {
            string stringValue;
            if (value == null)
                stringValue = string.Empty;
            else
                stringValue = value.ToString();

            if (_settings.ContainsKey(key))
                _settings[key] = stringValue;
            else
                _settings.Add(key, stringValue);
        }
    }
}
