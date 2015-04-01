using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiceBus.WebApi.Models
{
    public class BallEx
    {
        /// <summary>
        /// The Lane identifier in the Venue.
        /// </summary>
        public int lane { get; set; }

        /// <summary>
        /// Name of the Bowler URL Encoded (UTF-8) in the Scoring System.
        /// </summary>
        public string bowler { get; set; }

        /// <summary>
        /// Home Lane of the Bowler
        /// </summary>
        public int home { get; set; }

        /// <summary>
        /// Position the Bowler is bowling on the lane.
        /// </summary>
        public int position { get; set; }

        /// <summary>
        /// The Square number this Ball represents.
        /// </summary>
        public int square { get; set; }

        /// <summary>
        /// True if it is the first throw of the Frame.
        /// </summary>
        public bool first { get; set; }

        /// <summary>
        /// Represents if this Ball is a Foul.
        /// </summary>
        public bool foul { get; set; }

        /// <summary>
        /// Pin Bit Mask, with each bit that is turned on representing a Standing Pin (a pin that was
        /// NOT knocked down during the throw). 
        ///         Pin1 = 0x1
        ///         Pin2 = 0x2
        ///         Pin3 = 0x4
        ///         Pin4 = 0x8
        ///         Pin5 = 0x10
        ///         Pin6 = 0x20
        ///         Pin7 = 0x40
        ///         Pin8 = 0x80
        ///         Pin9 = 0x100
        ///         Pin10 = 0x200
        /// </summary>
        public int pins { get; set; }

        /// <summary>
        /// Consecutive Strikes that have been thrown.  This field is optional.
        /// </summary>
        public int strikes { get; set; }

        /// <summary>
        /// The Score displayed in the scoring system (can be numeric or alpha).
        /// </summary>
        public string score { get; set; }

        /// <summary>
        /// Used for tournaments, in the format of
        /// Scoring System Bowler Id~Team Name~Player Email~Division
        /// None of the following characters are allowed: <>/&\"\'
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// Id of the Venue.
        /// </summary>
        public int venueId { get; set; }
    }
}