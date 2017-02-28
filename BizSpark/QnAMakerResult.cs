using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BizSpark
{
    public class QnAMakerResult
    {
        ///
 
        /// The top answer found in the QnA Service.
        /// </summary>
 
 
        [JsonProperty(PropertyName = "answer")]
        public string Answer { get; set; }
 
        ///
 
        /// The score in range [0, 100] corresponding to the top answer found in the QnA    Service.
        /// </summary>
 
 
        [JsonProperty(PropertyName = "score")]
        public double Score { get; set; }
    }
}