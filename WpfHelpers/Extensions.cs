using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace NullVoidCreations.WpfHelpers
{
    public static class Extensions
    {
        public static string SplitCamelCase(this string stringToSplit)
        {
            return Regex.Replace(
                Regex.Replace(
                    stringToSplit,
                    @"(\P{Ll})(\P{Ll}\p{Ll})",
                    "$1 $2"
                ),
                @"(\p{Ll})(\P{Ll})",
                "$1 $2"
            );
        }

        public static string GetSHA512(this FileInfo fileInfo)
        {
            if (fileInfo.Exists)
            {
                var fileSystemHelper = new FileSystemHelper();
                return fileSystemHelper.GetSHA512(fileInfo.FullName);
            }

            return default(string);
        }

        public static string GetMD5(this FileInfo fileInfo)
        {
            if (fileInfo.Exists)
            {
                var fileSystemHelper = new FileSystemHelper();
                return fileSystemHelper.GetMD5(fileInfo.FullName);
            }

            return default(string);
        }

        public static bool Run(this FileInfo fileInfo, string arguments, bool runAsAdministrator, bool hideUi = false)
        {
            if (fileInfo.Exists)
            {
                var fileSystemHelper = new FileSystemHelper();
                return fileSystemHelper.RunProgram(fileInfo.FullName, arguments, runAsAdministrator, hideUi);
            }

            return false;
        }

        public static bool Run(this FileInfo fileInfo, string arguments, string userName, string password)
        {
            if (fileInfo.Exists)
            {
                var pInvoke = new PlatformInvoke();
                return pInvoke.RunProgram(fileInfo.FullName, arguments, userName, password);
            }

            return false;
        }

        public static string GetStartupDirectory(this Application application)
        {
            return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        }

        public static StringBuilder AppendLineFormatted(this StringBuilder instance, string format, params object[] parts)
        {
            return instance.AppendLine(string.Format(format, parts));
        }

        public static DateTime GetInternetTime(this DateTime instance, Uri url = null)
        {
            if (url == null)
                url = new Uri("Https://www.google.com/");

            try
            {
                var myHttpWebRequest = WebRequest.Create(url);
                var response = myHttpWebRequest.GetResponse();
                var date = response.Headers["date"];
                return DateTime.ParseExact(date,
                                           "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                                           CultureInfo.InvariantCulture.DateTimeFormat,
                                           DateTimeStyles.AssumeUniversal);
            }
            catch
            {
                return DateTime.Now;
            }

        }

        public static string Encrypt(this string plainText, string password)
        {
            var stringCipher = new StringCipher();
            return stringCipher.Encrypt(plainText, password);
        }

        public static string Decrypt(this string cipherText, string password)
        {
            var stringCipher = new StringCipher();
            return stringCipher.Decrypt(cipherText, password);
        }

        public static StringBuilder AppendLine(this StringBuilder builder, string format, params object[] parts)
        {
            return builder.AppendLine(string.Format(format, parts));
        }

        public static SecureString ToSecureString(this string stringToConvert)
        {
            var secureString = new SecureString();
            if (string.IsNullOrEmpty(stringToConvert))
                return secureString;

            foreach (char c in stringToConvert)
                secureString.AppendChar(c);

            return secureString;
        }
    }
}
