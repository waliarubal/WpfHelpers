using NullVoidCreations.Licensing;
using NullVoidCreations.WpfHelpers;
using NullVoidCreations.WpfHelpers.Base;
using NullVoidCreations.WpfHelpers.Commands;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace NullVoidCreations.KeyGenerator.ViewModels
{
    class MainViewModel: ViewModelBase
    {
        string _businessName, _contactPerson, _contactNumber, _email, _machineName, _biosSerial, _serialKey, _activationKey;
        int _days;
        ICommand _generate, _fillMachineInfo, _reset, _validate;

        #region properties

        public string BusinessName
        {
            get { return _businessName; }
            set { Set(nameof(BusinessName), ref _businessName, value); }
        }

        public string ContactPerson
        {
            get { return _contactPerson; }
            set { Set(nameof(ContactPerson), ref _contactPerson, value); }
        }

        public string ContactNumber
        {
            get { return _contactNumber; }
            set { Set(nameof(ContactNumber), ref _contactNumber, value); }
        }

        public string Email
        {
            get { return _email; }
            set { Set(nameof(Email), ref _email, value); }
        }

        public string MachineName
        {
            get { return _machineName; }
            set { Set(nameof(MachineName), ref _machineName, value); }
        }

        public string BiosSerial
        {
            get { return _biosSerial; }
            set { Set(nameof(BiosSerial), ref _biosSerial, value); }
        }

        public int Days
        {
            get { return _days; }
            set { Set(nameof(Days), ref _days, value); }
        }

        public string SerialKey
        {
            get { return _serialKey; }
            private set { Set(nameof(SerialKey), ref _serialKey, value); }
        }

        public string ActivationKey
        {
            get { return _activationKey; }
            private set { Set(nameof(ActivationKey), ref _activationKey, value); }
        }

        #endregion

        #region commands

        public ICommand GenerateCommand
        {
            get
            {
                if (_generate == null)
                    _generate = new RelayCommand(Generate) { IsSynchronous = true };

                return _generate;
            }
        }

        public ICommand FillMachineInformationCommand
        {
            get
            {
                if (_fillMachineInfo == null)
                    _fillMachineInfo = new RelayCommand(FillMachineInfo);

                return _fillMachineInfo;
            }
        }

        public ICommand ResetCommand
        {
            get
            {
                if (_reset == null)
                    _reset = new RelayCommand(Reset);

                return _reset;
            }
        }

        public ICommand ValidateCommand
        {
            get
            {
                if (_validate == null)
                    _validate = new RelayCommand(Validate);

                return _validate;
            }
        }

        #endregion

        void Reset()
        {
            BusinessName = "NullVoid Creations";
            ContactPerson = "Rubal Walia";
            ContactNumber = "+91 99288 93416";
            Email = "walia.rubal@gmail.com";
            Days = 30;
            SerialKey = ActivationKey = string.Empty;
        }

        void Validate()
        {
            if (string.IsNullOrEmpty(SerialKey) || string.IsNullOrEmpty(ActivationKey))
            {
                MessageBox.Show("License not generated.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var license = StrongLicense.Load(SerialKey, ActivationKey, out string errorMessage);
            if (string.IsNullOrEmpty(errorMessage))
                MessageBox.Show("License is valid.");
            else
                MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        void Generate()
        {
            foreach(var property in GetType().GetProperties())
            {
                if (property.GetSetMethod() == null)
                    continue;

                var data = property.GetValue(this, null);
                if (data == null || string.IsNullOrEmpty(data.ToString()))
                {
                    MessageBox.Show(string.Format("Please enter value for {0}.", property.Name.SplitCamelCase()), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            var licenseFile = Path.Combine(Application.Current.GetStartupDirectory(), string.Format("{0}_license.aes", Email));
            var license = StrongLicense.Generate(DateTime.Now, DateTime.Now.AddDays(Days), Email, BusinessName, ContactPerson, ContactNumber, licenseFile);
            SerialKey = license.SerialKey;
            ActivationKey = license.ActivationKey;
            Clipboard.SetText(license.ToString());
            if (license == null)
                MessageBox.Show("Failed to generate license.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            else
                MessageBox.Show("License generated successfully and copied to clipboard.");
        }

        void FillMachineInfo()
        {
            MachineName = Environment.MachineName;
            BiosSerial = StrongLicense.GetMachineKey();
        }
    }
}
