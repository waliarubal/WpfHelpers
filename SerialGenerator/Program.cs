using NullVoidCreations.WpfHelpers;
using System;
using System.IO;

namespace SerialGenerator
{
    class Program
    {
        static void Main(string[] args)
        {

            if (args.Length != 4)
                return;

            var email = args[0];
            var machineName = args[1];
            var biosSerial = args[2];
            var days = args[3];

            var fileName = Path.Combine(Environment.CurrentDirectory, string.Format("{0}_License.aes", email));

            var license = StrongLicense.Generate(DateTime.Now, DateTime.Now.AddDays(double.Parse(days)), email, machineName, biosSerial, fileName);
        }
    }
}
