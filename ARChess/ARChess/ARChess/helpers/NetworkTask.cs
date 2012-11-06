using System;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ARChess.helpers
{
    public class NetworkTask
    {
        HttpWebRequest mRequest;
        String mData;

        public NetworkTask()
        {
            String uriString = "";
            mRequest = WebRequest.CreateHttp(uriString);
            //mRequest.;
        }

        public void sendString(String data)
        {
            mData = data;
            byte[] byteArray = Encoding.UTF8.GetBytes(data);

            mRequest.Method = "POST";
            mRequest.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback), mRequest);
        }

        private void GetRequestStreamCallback(IAsyncResult asyncResult)
        {

        }
    }
}
