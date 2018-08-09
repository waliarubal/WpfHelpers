using NullVoidCreations.Licensing;
using System;
using System.IO;

namespace SerialGenerator
{
    class Program
    {
        static void Main(string[] args)
        {

            if (args.Length != 7)
                return;

            var email = args[0];
            var machineName = args[1];
            var biosSerial = args[2];
            var days = args[3];
            var businessName = args[4];
            var contactPerson = args[5];
            var contactNumber = args[6];

            var fileName = Path.Combine(Environment.CurrentDirectory, string.Format("{0}_License.aes", email));

            var license = StrongLicense.Generate(
                DateTime.Now, 
                DateTime.Now.AddDays(double.Parse(days)), 
                email, 
                businessName, 
                contactPerson, 
                contactNumber, 
                machineName, 
                biosSerial, 
                fileName);
        }
    }
}
