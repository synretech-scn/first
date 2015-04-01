using Newtonsoft.Json;
using ServiceBus.WebApi.Infrastructure;
using ServiceBus.WebApi.Models;
using ServiceBus.WebApi.Queue;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace ServiceBus.WebApi.Controllers
{
    public class HomeController : ApiController
    {
        // GET api/home
        public IEnumerable<string> Get()
        {
            var config = ServiceBusQueue.GetInstance(0);

            return new string[] { config.ToString() };
            //return new string[] { config.ServiceNamespace, config.ServiceIssuer, config.QueueName, config.ServiceSecret };
        }

        // Post api/home/5
        public void Post(int venueId)
        {
            using (var stream = System.Web.HttpContext.Current.Request.InputStream)
            {
                stream.Position = 0;

                using (StreamReader reader = new StreamReader(stream))
                {
                    using (JsonTextReader jsonReader = new JsonTextReader(reader))
                    {
                        // deserialize
                        JsonSerializer ser = new JsonSerializer();
                        List<BallEx> instance = ser.Deserialize<List<BallEx>>(jsonReader);

                        Balls legacyBalls = new Balls();

                        foreach (BallEx ball in instance)
                        {
                            legacyBalls.Add(
                                new Ball
                                {
                                    Bowler = ball.bowler,
                                    First = ball.first.ToString(),
                                    Foul = ball.foul.ToString(),
                                    Home = ball.home.ToString(),
                                    Id = ball.id,
                                    Lane = ball.lane.ToString(),
                                    Pins = ball.pins.ToString(),
                                    Position = ball.position.ToString(),
                                    Score = ball.score.ToString(),
                                    Square = ball.square.ToString(),
                                    Strikes = ball.strikes.ToString(),
                                    VenueId = venueId.ToString()
                                }
                             );
                        }

                        // post it to the queue
                        //int sbInstanceId = venueId % ServiceBusQueue.TOTAL_NUMBER_QUEUES;
                        //ServiceBusQueue.GetInstance(sbInstanceId).PostBalls(legacyBalls, venueId.ToString());
                        string ip = HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null ? HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString() : HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
                        int logInstanceId = venueId % ServiceBusLoggingQueue.TOTAL_NUMBER_LOGGING_QUEUES;
                        ServiceBusLoggingQueue.GetInstance(logInstanceId).PostBalls(legacyBalls, venueId.ToString(), ip);
                    }
                }
            }
        }
    }
}
