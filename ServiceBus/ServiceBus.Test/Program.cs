using ServiceBus.WebApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceBus.Test
{
    class Program
    {
        public static readonly Random r = new Random();
        public void DoWork()
        {
            for (int i = 0; i < 1; ++i)
            {
                int venueId = r.Next(110000, 119999);
                List<BallEx> balls = new List<BallEx>();
                for (int j = 0; j < 10; ++j)
                    balls.Add(new BallEx { bowler = "test" + r.Next(100, 999).ToString(), first = true, foul = false, home = r.Next(1, 10), id = r.Next(211000, 311100).ToString(), lane = r.Next(1, 10), pins = 0x200, position = 1, score = r.Next(1, 10).ToString(), square = r.Next(1, 10), strikes = r.Next(1, 10), venueId = venueId });

                HttpWebRequest request = WebRequest.CreateHttp("http://venueapilog.cloudapp.net/venue/" + venueId.ToString() + "/balls");
                //HttpWebRequest request = WebRequest.CreateHttp("http://127.0.0.1:1494/venue/" + venueId.ToString() + "/balls");
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                request.Method = "POST";

                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(List<BallEx>));
                using (MemoryStream ms = new MemoryStream())
                {
                    ser.WriteObject(ms, balls);
                    string data = Encoding.UTF8.GetString(ms.ToArray());
                    request.ContentLength = data.Length;
                    using (Stream stream = request.GetRequestStream())
                    {
                        ms.WriteTo(stream);
                    }
                }
                Console.WriteLine("Begin Request: {0}", venueId);
                try
                {
                    WebResponse response = request.GetResponse();
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    Console.WriteLine("{0}:" + ((System.Net.HttpWebResponse)(response)).StatusCode.ToString(), venueId);
                }
                catch (Exception e)
                {

                }
            }
        }

        static void Main(string[] args)
        {
            System.Threading.ThreadPool.SetMaxThreads(1000, 1000);
            Console.WriteLine("How many requests?");
            int threads = Convert.ToInt32(Console.ReadLine());
            Parallel.For(0, threads, t =>
            {
                //ThreadStart myThreadDelegate = new ThreadStart(Program.DoWork);
                Program p = new Program();
                Thread myThread = new Thread(p.DoWork);
                myThread.Start();

            });
            Console.ReadKey();
        }
    }
}
