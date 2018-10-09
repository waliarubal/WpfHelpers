using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NullVoidCreations.Licensing;

namespace Tests
{
    [TestClass]
    public class StrongLicenseTest
    {

        StrongLicense GetLicense(int validForDays, string email, string businessName, string contactPerson, string contactNumber)
        {
            var issueDate = DateTime.Now;
            return StrongLicense.Generate(issueDate, issueDate.AddDays(validForDays), email, businessName, contactPerson, contactNumber);
        }

        [TestMethod]
        public void GetMachineKey()
        {
            Assert.IsNotNull(StrongLicense.GetMachineKey(), "Failed to get machine key.");
        }

        [DataTestMethod]
        [DataRow(30, "walia.rubal@gmail.com", "NullVoid Creations", "Rubal Walia", "+91 99288 93416")]
        public void GenerateLicense(int validForDays, string email, string businessName, string contactPerson, string contactNumber)
        {
            var license = GetLicense(validForDays, email, businessName, contactPerson, contactNumber);
            Assert.IsNotNull(license, "Failed to generate license.");
            Assert.IsNotNull(license.SerialKey, "Serial key not generated.");
            Assert.AreEqual(email, license.Email);
            Assert.AreEqual(businessName, license.BusinessName);
            Assert.AreEqual(contactPerson, license.ContactPerson);
            Assert.AreEqual(contactNumber, license.ContactNumber);
        }

        [DataTestMethod]
        [DataRow(30, "walia.rubal@gmail.com", "NullVoid Creations", "Rubal Walia", "+91 99288 93416")]
        public void LoadLicense(int validForDays, string email, string businessName, string contactPerson, string contactNumber)
        {
            var license = GetLicense(validForDays, email, businessName, contactPerson, contactNumber);
            Assert.IsNotNull(license, "Failed to generate license.");

            var loadedLicnse = StrongLicense.Load(license.SerialKey, license.ActivationKey, out string errorMessage);
            Assert.IsNull(errorMessage, errorMessage);
            Assert.AreEqual(license.SerialKey, loadedLicnse.SerialKey, "Serial keys don't match.");
            Assert.AreEqual(license.ActivationKey, loadedLicnse.ActivationKey, "Activation keys don't match.");
            Assert.IsTrue(license.IsActivated, "License is not active.");
        }
    }
}
