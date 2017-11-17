using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
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
    }
}
