using System;
using System.IO;
using System.Net;
using System.Text;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Info;

namespace ARChess
{
    public class NetworkTask
    {
        public NetworkTask()
        {
            
        }

        private string getUniqueDeviceId()
        {
            byte[] result = null;
            object uniqueId;
            if (DeviceExtendedProperties.TryGetValue("DeviceUniqueId", out uniqueId))
                result = (byte[])uniqueId;

            return Convert.ToBase64String(result);  
        }

        private string makeHttpRequest(string verb, string data)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://synchro-archess.herokuapp.com/api?identifier=" + getUniqueDeviceId());
            request.Method = verb;
            request.Headers["API_KEY"] = "traprubepreyed2ebupucramunumus4ebewruyUdraga36pacrujavuKep8afref";

            if (data != "")
            {
                request.ContentType = "application/json";
                byte[] post_bytes = Encoding.UTF8.GetBytes(data);
                Stream writer = request.GetRequestStream();
                writer.Write(post_bytes, 0, post_bytes.Length);
                writer.Close();
            }
  
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            String returninfo = reader.ReadToEnd();
            reader.Close();
            response.Close();

            return returninfo;
        }

        public GameResponse getGameState()
        {
            string response = makeHttpRequest("GET", "");

            GameResponse state = null;

            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(GameResponse));
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(response));
            state = (GameResponse)js.ReadObject(stream);

            return state;
        }

        public GameResponse createGame()
        {
            string response = makeHttpRequest("POST", "{}");

            GameResponse state = null;

            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(GameResponse));
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(response));
            state = (GameResponse)js.ReadObject(stream);

            return state;
        }

        public bool sendGameState(CurrentGameState state)
        {
            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(CurrentGameState));
            MemoryStream stream = new MemoryStream();
            js.WriteObject(stream, state);
            stream.Position = 0;
            StreamReader reader = new StreamReader(stream);
            string json = reader.ReadToEnd();

            makeHttpRequest("PUT", json);

            return true;
        }

        public bool resignGame()
        {
            makeHttpRequest("DELETE", "");

            return true;
        }
    }
}
