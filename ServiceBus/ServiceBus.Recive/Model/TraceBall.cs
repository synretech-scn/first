﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace ServiceBus.Recive.Model
{
    [DataContract(Name = "tb", Namespace = "http://schemas.datacontract.org/2004/07/XBowlingBalls")]
    public class TraceBall : Ball
    {
        [DataMember(Name = "ip", Order = 12)]
        public string IP { get; set; }
    }
}