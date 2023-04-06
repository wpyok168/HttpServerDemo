using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Remoting.Contexts;

namespace HttpServerDemo
{
    internal class Program
    {
        static HttpListener sSocket = null;
        static void Main(string[] args)
        {
            sSocket = new HttpListener();
            sSocket.Prefixes.Add("http://127.0.0.1:8899/");
            sSocket.Start();
            sSocket.BeginGetContext(new AsyncCallback(GetContextCallBack), sSocket);
            Console.Read();
        }
        static void GetContextCallBack(IAsyncResult ar)
        {
            try
            {
                sSocket = ar.AsyncState as HttpListener;
                HttpListenerContext context = sSocket.EndGetContext(ar);
                sSocket.BeginGetContext(new AsyncCallback(GetContextCallBack), sSocket);

                Console.WriteLine(context.Request.Url.PathAndQuery);

                //其它处理code
                
                //请求
                
                HttpListenerRequest request = context.Request;
                RequestProcessing(request);

                //响应
                HttpListenerResponse response = context.Response;
                ResponseProcessing(response);
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }

        //请求处理
        private static string RequestProcessing(HttpListenerRequest request, HttpListenerResponse response)
        {
            string postData = new StreamReader(request.InputStream).ReadToEnd();
            Console.WriteLine("收到请求：" + postData);

            //响应处理
           
            ResponseProcessing(response);
            return string.Empty;
        }
        //请求处理
        private static string RequestProcessing(HttpListenerRequest request)
        {
            string postData = new StreamReader(request.InputStream).ReadToEnd();
            Console.WriteLine("收到请求：" + postData);
            return string.Empty;
        }

        //相应处理
        private static string ResponseProcessing(HttpListenerResponse response)
        {
            string responseBody = "<html><body><h1>test server</h1><form method=post action=/form><input type=text name=foo value=foovalue><input type=submit name=bar value=barvalue></form>";
            response.ContentLength64 = System.Text.Encoding.UTF8.GetByteCount(responseBody);
            response.ContentType = "text/html; Charset=UTF-8";
            //输出响应内容
            Stream output = response.OutputStream;
            using (StreamWriter sw = new StreamWriter(output))
            {
                sw.Write(responseBody);
            }
            Console.WriteLine("响应结束");
            return string.Empty;
        }
    }
}
