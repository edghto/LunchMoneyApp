using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Text;
using System.IO;

namespace LunchMoneyApp
{
    public delegate void ProcessHttpPostResponse(String response);

    public class HttpPOSTWorker
    {
        public string ContentType { get; set; }

        public HttpPOSTWorker()
        {
            ContentType = "application/x-www-form-urlencoded";
        }

        public void connect(String url, byte[] message, ProcessHttpPostResponse callback)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Method = "POST";
            webRequest.ContentType = ContentType;
            webRequest.BeginGetRequestStream(
                getRequestAsyncResult =>
                {
                    HttpWebRequest request = (HttpWebRequest)getRequestAsyncResult.AsyncState;
                    Stream postStream = request.EndGetRequestStream(getRequestAsyncResult);
                    postStream.Write(message, 0, message.Length);
                    postStream.Close();

                    request.BeginGetResponse(
                        getResponseAsyncResult =>
                        {
                            HttpWebRequest req = (HttpWebRequest)getResponseAsyncResult.AsyncState;
                            string responseStr = null;
                            try
                            {
                                HttpWebResponse res = (HttpWebResponse)request.EndGetResponse(getResponseAsyncResult);
                                Stream streamResponse = res.GetResponseStream();
                                StreamReader streamReader = new StreamReader(streamResponse);
                                responseStr = streamReader.ReadToEnd();
                                streamResponse.Close();
                                streamReader.Close();
                                res.Close();
                            }
                            catch
                            {
                            }
                            callback(responseStr);
                        }, 
                        request);
                },
                webRequest);
        }

        public void close()
        {

        }

    }
}
