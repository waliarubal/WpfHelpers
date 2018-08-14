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
        string _businessName, _contactPerson, _contactNumber, _email, _machineName, _biosSerial;
        int _days;
        ICommand _generate, _fillMachineInfo, _reset;

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

        #endregion

        void Reset()
        {
            BusinessName = "NullVoid Creations";
            ContactPerson = "Rubal Walia";
            ContactNumber = "+91 99288 93416";
            Email = "walia.rubal@gmail.com";
            Days = 30;
        }

        void Generate()
        {
            foreach(var property in GetType().GetProperties())
            {
                var data = property.GetValue(this, null);
                if (data == null || string.IsNullOrEmpty(data.ToString()))
                {
                    MessageBox.Show(string.Format("Please enter value for {0}.", property.Name.SplitCamelCase()), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            var licenseFile = Path.Combine(Application.Current.GetStartupDirectory(), string.Format("{0}_license.aes", Email));
            var license = StrongLicense.Generate(DateTime.Now, DateTime.Now.AddDays(Days), Email, BusinessName, ContactPerson, ContactNumber, licenseFile);
            if (license == null)
                MessageBox.Show("Failed to generate license.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            else
                MessageBox.Show("License generated successfully.");
        }

        void FillMachineInfo()
        {
            MachineName = Environment.MachineName;
            BiosSerial = StrongLicense.GetMachineKey();
        }
    }
}
