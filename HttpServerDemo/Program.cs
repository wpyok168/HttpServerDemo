using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Remoting.Contexts;
using GetCIDbyBatchActivation;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

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
                string a = System.Web.HttpUtility.HtmlEncode(context.Request.Url.Query);

                //其它处理code

                //请求
                //HttpListenerRequest request = context.Request;
                RequestProcessing(context.Request, context.Response);


                //响应
                //HttpListenerResponse response = context.Response;
                //ResponseProcessing(response);
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
        }

        //post 请求处理
        private static string RequestProcessing(HttpListenerRequest request, HttpListenerResponse response)
        {
            if (request.HttpMethod=="GET")
            {
                if (request.Url.ToString().Contains("getcid/?iids="))
                {
                    string cid = string.Empty;
                    string str = System.Web.HttpUtility.UrlDecode(request.Url.PathAndQuery);
                    string iid = Regex.Replace(str, "-","").Replace(" ", "").Replace("getcid/?iids=","");
                    iid = Regex.Match(iid, "[\\d]+").Value;
                    if (Regex.IsMatch(iid, "[\\d]{63}"))
                    {
                        iid= Regex.Match(iid, "[\\d]{63}").Value;
                    }
                    else if (Regex.IsMatch(iid, "[\\d]{54}"))
                    {
                        iid = Regex.Match(iid, "[\\d]{54}").Value;
                    }
                    //string iid = "690494024111248275641345743539870015978502549282339542426203524";
                    cid=XmlRequest.MSXmlRequest(1, iid, "00000-04249-038-820384-03-2052-9200.0000-0902023");
                    string outstr = $"安装ID：{iid}</br>确认ID：{cid}";
                    IIDResponseProcessing(response, outstr);
                }
            }
            else if (request.HttpMethod == "POST")
            {
                //post 请求处理
                //string postData = new StreamReader(request.InputStream).ReadToEnd();
               // postData = System.Web.HttpUtility.UrlDecode(postData); //中文需要解码
               // Console.WriteLine("收到请求：" + postData);
            }
            
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
            string responseBody = "<html><body><h1>test server</h1><form method=post action=/form><input type=text name=foo value='好人一个'><input type=submit name=bar value=barvalue></form>";
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

        private static string IIDResponseProcessing(HttpListenerResponse response,string cid)
        {
            string responseBody = cid;
            response.ContentLength64 = System.Text.Encoding.UTF8.GetByteCount(responseBody);
            response.ContentType = "text/html; Charset=UTF-8";
            //输出响应内容
            Stream output = response.OutputStream;
            using (StreamWriter sw = new StreamWriter(output))
            {
                sw.Write(responseBody);
            }
            Console.WriteLine("响应数据"+ cid);
            Console.WriteLine("响应结束");
            return string.Empty;
        }
    }
}
