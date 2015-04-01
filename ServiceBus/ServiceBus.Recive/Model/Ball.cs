using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Recive.Model
{
    [DataContract(Name = "a", Namespace = "http://schemas.datacontract.org/2004/07/XBowlingBalls")]
    public class Ball
    {
        [DataMember(Name = "l", Order = 6)]
        public string Lane { get; set; }

        [DataMember(Name = "b", Order = 0)]
        public string Bowler { get; set; }

        [DataMember(Name = "h", Order = 3)]
        public string Home { get; set; }

        [DataMember(Name = "p", Order = 7)]
        public string Position { get; set; }

        [DataMember(Name = "s", Order = 8)]
        public string Square { get; set; }

        [DataMember(Name = "f", Order = 2)]
        public string First { get; set; }

        [DataMember(Name = "u", Order = 9)]
        public string Foul { get; set; }

        [DataMember(Name = "i", Order = 4)]
        public string Pins { get; set; }

        [DataMember(Name = "k", Order = 5)]
        public string Strikes { get; set; }

        [DataMember(Name = "c", Order = 1)]
        public string Score { get; set; }

        [DataMember(Name = "o", Order = 10)]
        public string Id { get; set; }

        [DataMember(Name = "r", Order = 11)]
        public string VenueId { get; set; }
    }
}
