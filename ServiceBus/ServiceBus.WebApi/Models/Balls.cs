using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace ServiceBus.WebApi.Models
{
    [CollectionDataContract(Name = "x", Namespace = "http://schemas.datacontract.org/2004/07/XBowlingBalls")]
    public class Balls : List<Ball>
    {
        public Balls() { }
        public Balls(List<Ball> Balls) : base(Balls) { }
    }
}