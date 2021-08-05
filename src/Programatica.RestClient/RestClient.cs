using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Programatica.RestClient
{
    /// <summary>
    /// RestClient
    /// </summary>
    public class RestClient
    {
        public string EndPoint { get; set; }
        public HttpVerbEnum Method { get; set; }
        public string ContentType { get; set; }
        public string PostData { get; set; }

        public List<RequestHeader> RequestHeaders { get; set; }

        public NetworkCredential Credentials { get; set; }

        public RestClient(string endpoint,
                          HttpVerbEnum method,
                          string contentType,
                          string postData,
                          List<RequestHeader> requestHeaders,
                          NetworkCredential credentials)
        {
            EndPoint = endpoint;
            Method = method;
            ContentType = contentType;
            PostData = postData;
            RequestHeaders = requestHeaders;
            Credentials = credentials;
        }

        /// <summary>
        /// MakeRequest
        /// </summary>
        /// <returns></returns>
        public string MakeRequest()
        {
            return MakeRequest("");
        }

        /// <summary>
        /// MakeRequest
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public string MakeRequest(string parameters)
        {
            var request = (HttpWebRequest)WebRequest.Create(EndPoint + parameters);

            request.Method = Method.ToString();
            request.ContentLength = 0;
            request.ContentType = ContentType;
            request.Credentials = Credentials;

            foreach (var header in RequestHeaders)
            {
                request.Headers[header.Name] = header.Value;
            }

            if (!string.IsNullOrEmpty(PostData) && (Method == HttpVerbEnum.POST || Method == HttpVerbEnum.PUT))
            {
                var encoding = new UTF8Encoding();
                var bytes = Encoding.GetEncoding("iso-8859-1").GetBytes(PostData);
                request.ContentLength = bytes.Length;

                using (var writeStream = request.GetRequestStream())
                {
                    writeStream.Write(bytes, 0, bytes.Length);
                }
            }

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                var responseValue = string.Empty;

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var message = String.Format("Request failed. Received HTTP {0}", response.StatusCode);
                    throw new ApplicationException(message);
                }

                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                    {
                        using (var reader = new StreamReader(responseStream))
                        {
                            responseValue = reader.ReadToEnd();
                        }
                    }
                }

                return responseValue;
            }

        }
    }
}
