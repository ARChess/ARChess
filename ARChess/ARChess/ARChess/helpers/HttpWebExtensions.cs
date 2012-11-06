using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace System.Net
{
    public static class HttpWebRequestExtensions
    {
        private const int DefaultRequestTimeout = 5000;

        public static HttpWebResponse GetResponse(this HttpWebRequest request)
        {
            var dataReady = new AutoResetEvent(false);
            HttpWebResponse response = null;
            var callback = new AsyncCallback(delegate(IAsyncResult asynchronousResult)
            {
                response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
                dataReady.Set();
            });

            request.BeginGetResponse(callback, request);

            if (dataReady.WaitOne(DefaultRequestTimeout))
            {
                return response;
            }

            return null;
        }

        public static Stream GetRequestStream(this HttpWebRequest request)
        {
            var dataReady = new AutoResetEvent(false);
            Stream stream = null;
            var callback = new AsyncCallback(delegate(IAsyncResult asynchronousResult)
            {
                stream = (Stream)request.EndGetRequestStream(asynchronousResult);
                dataReady.Set();
            });

            request.BeginGetRequestStream(callback, request);
            if (!dataReady.WaitOne(DefaultRequestTimeout))
            {
                return null;
            }

            return stream;
        }
    }
}
