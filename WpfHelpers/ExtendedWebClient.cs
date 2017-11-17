using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace EbayWorker.Helpers
{
    public class ExtendedWebClient : WebClient
    {
        readonly CookieContainer _cookieContainer;
        readonly X509Certificate _certificate;


        public ExtendedWebClient(int connectionLimit = 10)
        {
            _cookieContainer = new CookieContainer();
            _certificate = new X509Certificate();

            ConnectionLimit = connectionLimit;
        }

        #region properties

        public string Host { get; set; }

        public string Referer { get; set; }

        public int ConnectionLimit { get; set; }

        #endregion

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);
            var webRequest = request as HttpWebRequest;
            if (webRequest != null)
            {
                webRequest.CookieContainer = _cookieContainer;
                webRequest.AllowAutoRedirect = true;
                webRequest.ServicePoint.ConnectionLimit = ConnectionLimit;

                // add certificate for HTTPS requests
                if (request.RequestUri.Scheme.Equals("https", StringComparison.CurrentCultureIgnoreCase))
                    webRequest.ClientCertificates.Add(_certificate);

                if (!string.IsNullOrEmpty(Host))
                    webRequest.Host = Host;
                if (!string.IsNullOrEmpty(Referer))
                    webRequest.Referer = Referer;
            }

            return request;
        }
    }
}
