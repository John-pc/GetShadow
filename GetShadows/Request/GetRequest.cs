using System;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text;
using HtmlAgilityPack;
using ZXing;
using ZXing.QrCode;

namespace GetShadows.Request
{
    public class GetRequest
    {
        public string ResponseStr { get; set; }

        public void GetContent(string urlAddress)
        {
            var request = (HttpWebRequest) WebRequest.Create(urlAddress);
            var response = (HttpWebResponse) request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                if (response.CharacterSet == null)
                    readStream = new StreamReader(receiveStream);
                else
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));

                ResponseStr = readStream.ReadToEnd();

                response.Close();
                readStream.Close();
            }
            else
                Console.WriteLine("Get Content Error!");
        }

        public void SerachInfo(string url)
        {
            var web = new HtmlWeb();
            var doc = web.Load(url);
            var freeNode = doc.GetElementbyId("free");
            if (freeNode == null) return;
            var frees = freeNode.SelectNodes("//div[@class='col-lg-4 text-center']");
            foreach (var free in frees)
                DealWithServer(free);
        }

        private void DealWithServer(HtmlNode free)
        {
            var serverNode = free.SelectNodes("h4");

            var server = new Server
            {
                IP = GetValue(serverNode[0].InnerText),
                Port = GetValue(serverNode[1].InnerText),
                Password = GetValue(serverNode[2].InnerText),
                Method = GetValue(serverNode[3].InnerText)
            };
            var url = GetQRCode(server);
            GenerateQrcode(url, server.IP);
        }

        private string GetValue(string orgin)
        {
            try
            {
                var split = orgin.Split(':');
                return split[1];
            }
            catch
            {
                return "";
            }
        }

        private string GetQRCode(Server server)
        {
            var url = server.Method + (object) ":" + server.Password + "@" + server.IP + ":" +
                      server.Port;
            return "ss://" + Convert.ToBase64String(Encoding.UTF8.GetBytes(url));
        }

        private void GenerateQrcode(string url, string name)
        {
            var options = new QrCodeEncodingOptions
            {
                DisableECI = true,
                CharacterSet = "UTF-8",
                Width = 400,
                Height = 400
            };

            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = options
            };
            var bitmap = writer.Write(url);
            if (!string.IsNullOrEmpty(name))
            {
                var path = Path.Combine(Environment.CurrentDirectory, name)+".png";
                bitmap.Save(path, ImageFormat.Png);
            }
        }
    }

    public class Server
    {
        public string IP { get; set; }
        public string Port { get; set; }
        public string Method { get; set; }
        public string Password { get; set; }
    }
}