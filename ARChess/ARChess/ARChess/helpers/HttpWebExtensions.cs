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
        public static WebResponse GetResponse(this WebRequest request)
        {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            IAsyncResult asyncResult = request.BeginGetResponse(r => autoResetEvent.Set(), null);
            // Wait until the call is finished
            autoResetEvent.WaitOne();
            return request.EndGetResponse(asyncResult);
        }

        public static Stream GetRequestStream(this WebRequest request)
        {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            IAsyncResult asyncResult = request.BeginGetRequestStream(r => autoResetEvent.Set(), null);
            // Wait until the call is finished
            autoResetEvent.WaitOne();
            return request.EndGetRequestStream(asyncResult);
        }
    }
}
