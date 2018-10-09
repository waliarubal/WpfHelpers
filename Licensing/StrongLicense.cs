using NullVoidCreations.WpfHelpers;
using System;
using System.IO;
using System.Management;
using System.Text;

namespace NullVoidCreations.Licensing
{
    public class StrongLicense: IDisposable
    {
        const char SEPARATOR = 'ß';
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
                    throw new ArgumentNullException("Activation key not specified.");

                if (!DecodeActivationKey(value))
                    throw new InvalidOperationException("Invalid activation key.");
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

        public string BusinessName
        {
            get;
            private set;
        }

        public string ContactPerson
        {
            get;
            private set;
        }

        public string ContactNumber
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
            Segment1 = Segment2 = Segment3 = Segment4 = Email = BusinessName = ContactPerson = ContactNumber = MachineKey = MachineName = null;
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

        void GenerateActivationKey(
            DateTime isssueDate, 
            DateTime expirationDate, 
            string email, 
            string machineKey, 
            string machineName,
            string businessName,
            string contactPerson,
            string contactNumber)
        {
            IssueDate = isssueDate;
            ExpirationDate = expirationDate;
            Email = email;
            BusinessName = businessName;
            ContactPerson = contactPerson;
            ContactNumber = contactNumber;
            MachineKey = machineKey;
            MachineName = machineName;

            var keyBuilder = new StringBuilder();
            keyBuilder.AppendFormat("{0}{1}", SerialKey, SEPARATOR);
            keyBuilder.AppendFormat("{0:0000}{1}", IssueDate.Year, SEPARATOR);
            keyBuilder.AppendFormat("{0:00}{1}", IssueDate.Month, SEPARATOR);
            keyBuilder.AppendFormat("{0:00}{1}", IssueDate.Day, SEPARATOR);
            keyBuilder.AppendFormat("{0:0000}{1}", ExpirationDate.Year, SEPARATOR);
            keyBuilder.AppendFormat("{0:00}{1}", ExpirationDate.Month, SEPARATOR);
            keyBuilder.AppendFormat("{0:00}{1}", ExpirationDate.Day, SEPARATOR);
            keyBuilder.AppendFormat("{0}{1}", Email, SEPARATOR);
            keyBuilder.AppendFormat("{0}{1}", BusinessName, SEPARATOR);
            keyBuilder.AppendFormat("{0}{1}", ContactPerson, SEPARATOR);
            keyBuilder.AppendFormat("{0}{1}", ContactNumber, SEPARATOR);
            keyBuilder.AppendFormat("{0}{1}", MachineKey, SEPARATOR);
            keyBuilder.AppendFormat("{0}", MachineName);
            _activationKey = keyBuilder.ToString().Encrypt(PASSWORD);
        }

        bool DecodeActivationKey(string activationKey)
        {
            _activationKey = activationKey;
            var decryptedKey = activationKey.Decrypt(PASSWORD);

            var keyParts = decryptedKey.Split(SEPARATOR);
            if (keyParts.Length != 13)
                return false;

            var index = 0;
            try
            {
                SerialKey = keyParts[index++];
                IssueDate = new DateTime(int.Parse(keyParts[index++]), int.Parse(keyParts[index++]), int.Parse(keyParts[index++]));
                ExpirationDate = new DateTime(int.Parse(keyParts[index++]), int.Parse(keyParts[index++]), int.Parse(keyParts[index++]));
                Email = keyParts[index++];
                BusinessName = keyParts[index++];
                ContactPerson = keyParts[index++];
                ContactNumber = keyParts[index++];
                MachineKey = keyParts[index++];
                MachineName = keyParts[index++];
                IsActivated = IsValid(this);
            }
            catch (Exception)
            {
                _activationKey = null;
                return false;
            }

            return true;
        }

        void Save(string fileName)
        {
            if (File.Exists(fileName))
                File.Delete(fileName);

            // skip saving if serial is missing
            // this may be because license was unloaded earlier
            if (string.IsNullOrEmpty(SerialKey))
                return;

            File.WriteAllText(fileName, ActivationKey);
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

        static string GenerateSerial()
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

            return licenseBuilder.ToString();
        }

        #endregion

        public override string ToString()
        {
            var keyBuilder = new StringBuilder();
            keyBuilder.AppendLine("Serial Key: {0}", SerialKey);
            keyBuilder.AppendFormat("Activation Key: {0}", ActivationKey);
            return keyBuilder.ToString();
        }

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
            string businessName,
            string contactPerson,
            string contactNumber,
            string fileName = "")
        {
            return Generate(isssueDate, expirationDate, email, businessName, contactPerson, contactNumber, Environment.MachineName, GetMachineKey(), fileName);
        }

        public static StrongLicense Generate(
            DateTime isssueDate, 
            DateTime expirationDate, 
            string email, 
            string businessName,
            string contactPerson,
            string contactNumber,
            string machineName, 
            string machineKey, 
            string fileName = "")
        {
            var license = new StrongLicense();
            license.SerialKey = GenerateSerial();
            license.GenerateActivationKey(isssueDate, expirationDate, email, machineKey, machineName, businessName, contactPerson, contactNumber);

            if (!string.IsNullOrEmpty(fileName))
                license.Save(fileName);

            return license;
        }

        public static StrongLicense Load(string serialKey, string activationKey, out string errorMessage)
        {
            StrongLicense license = null;

            if (string.IsNullOrEmpty(serialKey))
                errorMessage = "Serial key not specified.";
            else
            {
                try
                {
                    license.ActivationKey = activationKey;
                    if (!serialKey.Equals(license.SerialKey, StringComparison.InvariantCultureIgnoreCase))
                    {
                        errorMessage = "Invalid serial key.";
                        license = null;
                    }
                    errorMessage = null;
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                    license = null;
                }
            }

            return license;
        }

        public static StrongLicense Load(string fileName, out string errorMessage)
        {
            errorMessage = null;
            StrongLicense license = null;

            if (!File.Exists(fileName))
                return license;

            try
            {
                license = new StrongLicense
                {
                    _fileName = fileName,
                    ActivationKey = File.ReadAllText(fileName).Decrypt(PASSWORD)
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
