using System;
using System.IO;
using System.Management;
using System.Text;
using System.Xml;

namespace NullVoidCreations.WpfHelpers
{
    public class License: IDisposable
    {
        const string VALID_CHARACTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ123456789";
        const string PASSWORD = "QFByb3Blcl9QYXRvbGEhMjAxNQ==";
        const int KEY_SIZE = 23;

        string _serialKey, _activationKey, _fileName;

        #region constructor / destructor

        private License()
        {

        }

        public void Dispose()
        {
            Dispose(true);
        }

        ~License()
        {
            Dispose(false);
        }

        #endregion

        #region properties

        public string SerialKey
        {
            get { return _serialKey; }
            private set
            {
                Segment1 = value.Substring(0, 5);
                Segment2 = value.Substring(6, 5);
                Segment3 = value.Substring(12, 5);
                Segment4 = value.Substring(18, 5);
                _serialKey = value;
            }
        }

        public string ActivationKey
        {
            get { return _activationKey; }
            private set
            {
                if (string.IsNullOrEmpty(value))
                    throw new InvalidOperationException("Activation key not specified.");

                var decryptedKey = value.Decrypt(PASSWORD);
                try
                {
                    IssueDate = ExtractDate(decryptedKey, 0);
                    ExpirationDate = ExtractDate(decryptedKey, 8);
                    Email = decryptedKey.Substring(16, decryptedKey.Length - 16);
                    IsActivated = IsValid(this);

                    _activationKey = value;
                    IsActivated = true;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Invalid activation key.");
                }
            }
        }

        public string MachineKey
        {
            get;
            private set;
        }

        public string MachineName
        {
            get;
            private set;
        }

        public string Segment1
        {
            get;
            private set;
        }

        public string Segment2
        {
            get;
            private set;
        }

        public string Segment3
        {
            get;
            private set;
        }

        public string Segment4
        {
            get;
            private set;
        }

        public bool IsActivated
        {
            get;
            private set;
        }

        public string Email
        {
            get;
            private set;
        }

        public DateTime IssueDate
        {
            get;
            private set;
        }

        public DateTime ExpirationDate
        {
            get;
            private set;
        }

        #endregion

        #region private methods

        string GetMachineKey()
        {
            using (var managementClass = new ManagementClass("Win32_BIOS"))
            {
                var properties = managementClass.Properties;
                using (var managementObjects = managementClass.GetInstances())
                {
                    foreach (var managementObject in managementObjects)
                    {
                        foreach (var property in properties)
                        {
                            try
                            {
                                if (property.Name.Equals("SerialNumber"))
                                    return managementObject.Properties[property.Name].Value.ToString();
                            }
                            catch
                            {
                                // do nothing here
                            }

                        }
                    }
                }
            }

            return null;
        }

        void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!string.IsNullOrEmpty(_fileName))
                    SaveLicense(_fileName);
            }

            IsActivated = false;
            _serialKey = _activationKey = _fileName = null;
            Segment1 = Segment2 = Segment3 = Segment4 = Email = null;
            ExpirationDate = IssueDate = DateTime.MinValue;
        }

        void GenerateActivationKey(DateTime isssueDate, DateTime expirationDate, string email)
        {
            IssueDate = isssueDate;
            ExpirationDate = expirationDate;
            Email = email;
            MachineKey = GetMachineKey();
            MachineName = Environment.MachineName;

            var key = string.Format("{0:0000}{1:00}{2:00}{3:0000}{4:00}{5:00}{6}",
                IssueDate.Year,
                IssueDate.Month,
                IssueDate.Day,
                ExpirationDate.Year,
                ExpirationDate.Month,
                ExpirationDate.Day,
                Email);
            _activationKey = key.Encrypt(PASSWORD);
        }

        DateTime ExtractDate(string text, int startIndex)
        {
            var date = new DateTime(
                int.Parse(text.Substring(startIndex, 4)),
                int.Parse(text.Substring(startIndex + 4, 2)),
                int.Parse(text.Substring(startIndex + 6, 2)));

            return date;
        }

        bool IsValid(License license)
        {
            var currentDate = DateTime.Now;
            if (currentDate.Date < license.IssueDate.Date)
                return false;
            if (currentDate.Date > license.ExpirationDate.Date)
                return false;

            return true;
        }

        #endregion

        public static string ValidateSerial(string key)
        {
            if (string.IsNullOrEmpty(key))
                return "Serial key not specified.";
            if (key.Length != KEY_SIZE)
                return "Invalid serial key size.";

            return null;
        }

        public static License Generate(DateTime isssueDate, DateTime expirationDate, string email)
        {
            var randomGenerator = new Random();
            var licenseBuilder = new StringBuilder(KEY_SIZE);

            for (int index = 1; index <= KEY_SIZE - 3; index++)
            {
                var serialChar = VALID_CHARACTERS[randomGenerator.Next(VALID_CHARACTERS.Length)];
                if (index % 5 == 0 && index < KEY_SIZE - 3)
                    licenseBuilder.AppendFormat("{0}-", serialChar);
                else
                    licenseBuilder.Append(serialChar);
            }

            var license = new License();
            license.SerialKey = licenseBuilder.ToString();
            license.GenerateActivationKey(isssueDate, expirationDate, email);
            return license;
        }

        public void SaveLicense(string fileName)
        {
            if (File.Exists(fileName))
                File.Delete(fileName);

            // skip saving if serial is missing
            // this may be because license was unloaded earlier
            if (string.IsNullOrEmpty(SerialKey))
                return;

            var document = new XmlDocument();
            var root = document.CreateElement("License");

            var serialKeyNode = document.CreateElement("SerialKey");
            serialKeyNode.InnerText = SerialKey;
            root.AppendChild(serialKeyNode);

            var activationKeyNode = document.CreateElement("ActivationKey");
            activationKeyNode.InnerText = ActivationKey;
            root.AppendChild(activationKeyNode);

            var machineSerialNode = document.CreateElement("MachineKey");
            machineSerialNode.InnerText = MachineKey;
            root.AppendChild(machineSerialNode);

            var machineNameNode = document.CreateElement("MachineName");
            machineNameNode.InnerText = MachineName;
            root.AppendChild(machineNameNode);

            document.AppendChild(root);

            document.Save(fileName);
            _fileName = fileName;
        }

        public static License LoadLicense(string fileName)
        {
            if (!File.Exists(fileName))
                return null;

            var document = new XmlDocument();
            document.Load(fileName);

            var license = new License();
            license._fileName = fileName;
            license.SerialKey = document.SelectSingleNode("/License/SerialKey").InnerText;
            license.ActivationKey = document.SelectSingleNode("/License/ActivationKey").InnerText;
            license.MachineKey = document.SelectSingleNode("/License/MachineKey").InnerText;
            license.MachineName = document.SelectSingleNode("/License/MachineName").InnerText;
            return license;
        }

        public static void DeleteLicense(string serialKey, string fileName)
        {
            var license = LoadLicense(fileName);
            if (license == null)
                return;

            if (license.SerialKey.Equals(serialKey))
                File.Delete(fileName);
        }
    }
}
