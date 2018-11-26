using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Temple.Models
{
    public class PreferenciaAprendizaje
    {
        [JsonProperty("idCat")]
        public int idCat { get; set; }
        [JsonProperty("idSub")]
        public int idSub { get; set; }
        [JsonProperty("desCat")]
        public string desCat { get; set; }        
        [JsonProperty("desSub")]
        public string desSub { get; set; }
    }
}