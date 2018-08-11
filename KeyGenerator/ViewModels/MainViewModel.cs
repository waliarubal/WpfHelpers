using NullVoidCreations.WpfHelpers.Base;
using System.Windows.Input;

namespace NullVoidCreations.KeyGenerator.ViewModels
{
    class MainViewModel: ViewModelBase
    {
        string _businessName, _contactPerson, _contactNumber, _email, _machineName, _biosSerial;
        int _days;
        ICommand _generate, _fillMachineInfo;

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
                    _generate = null;

                return _generate;
            }
        }

        public ICommand FillMachineInformationCommand
        {
            get
            {
                if (_fillMachineInfo == null)
                    _fillMachineInfo = null;

                return _fillMachineInfo;
            }
        }

        #endregion
    }
}
