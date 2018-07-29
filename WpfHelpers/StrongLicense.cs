using System;
using System.IO;
using System.Management;
using System.Text;
using System.Xml;

namespace NullVoidCreations.WpfHelpers
{
    public class StrongLicense: IDisposable
    {
        const string VALID_CHARACTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ123456789";
        const string PASSWORD = "QFByb3Blcl9QYXRvbGEhMjAxNQ==";
        const int KEY_SIZE = 23;

        string _serialKey, _activationKey, _fileName;

        #region constructor / destructor

        private StrongLicense()
        {

        }

        public void Dispose()
        {
            Dispose(true);
        }

        ~StrongLicense()
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
                var errorMessage = ValidateSerial(value);
                if (!string.IsNullOrEmpty(errorMessage))
                    throw new Exception(errorMessage);

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
                catch (Exception)
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

        void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!string.IsNullOrEmpty(_fileName))
                    Save(_fileName);
            }

            IsActivated = false;
            _serialKey = _activationKey = _fileName = null;
            Segment1 = Segment2 = Segment3 = Segment4 = Email = null;
            ExpirationDate = IssueDate = DateTime.MinValue;
        }

        string ValidateSerial(string key)
        {
            if (string.IsNullOrEmpty(key))
                return "Serial key not specified.";
            if (key.Length != KEY_SIZE)
                return "Invalid serial key size.";

            return null;
        }

        void GenerateActivationKey(DateTime isssueDate, DateTime expirationDate, string email, string machineKey, string machineName)
        {
            IssueDate = isssueDate;
            ExpirationDate = expirationDate;
            Email = email;
            MachineKey = machineKey;
            MachineName = machineName;

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

        void Save(string fileName)
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

        void Delete(string serialKey, string fileName)
        {
            var license = Load(fileName, out string errorMessage);
            if (license == null)
                return;

            if (license.SerialKey.Equals(serialKey))
                File.Delete(fileName);
        }

        DateTime ExtractDate(string text, int startIndex)
        {
            var date = new DateTime(
                int.Parse(text.Substring(startIndex, 4)),
                int.Parse(text.Substring(startIndex + 4, 2)),
                int.Parse(text.Substring(startIndex + 6, 2)));

            return date;
        }

        bool IsValid(StrongLicense license)
        {
            var currentDate = DateTime.Now;
            if (currentDate.Date < license.IssueDate.Date)
                return false;
            if (currentDate.Date > license.ExpirationDate.Date)
                return false;
            if (string.IsNullOrEmpty(license.MachineKey))
                return false;
            if (!license.MachineKey.Equals(GetMachineKey()))
                return false;

            return true;
        }

        #endregion

        public static string GetMachineKey()
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

        public static StrongLicense Generate(
            DateTime isssueDate,
            DateTime expirationDate,
            string email,
            string fileName)
        {
            return Generate(isssueDate, expirationDate, email, Environment.MachineName, GetMachineKey(), fileName);
        }

        public static StrongLicense Generate(
            DateTime isssueDate, 
            DateTime expirationDate, 
            string email, 
            string machineName, 
            string machineKey, 
            string fileName)
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

            var license = new StrongLicense();
            license.SerialKey = licenseBuilder.ToString();
            license.GenerateActivationKey(isssueDate, expirationDate, email, machineKey, machineName);
            return license;
        }

        public static StrongLicense Load(string fileName, out string errorMessage)
        {
            errorMessage = null;
            StrongLicense license = null;

            if (!File.Exists(fileName))
                return license;

            var document = new XmlDocument();
            document.Load(fileName);

            try
            {
                license = new StrongLicense
                {
                    _fileName = fileName,
                    SerialKey = document.SelectSingleNode("/License/SerialKey").InnerText,
                    ActivationKey = document.SelectSingleNode("/License/ActivationKey").InnerText,
                    MachineKey = document.SelectSingleNode("/License/MachineKey").InnerText,
                    MachineName = document.SelectSingleNode("/License/MachineName").InnerText
                };
            }
            catch(Exception ex)
            {
                errorMessage = ex.Message;
                license = null;
            }

            return license;
        }
    }
}
