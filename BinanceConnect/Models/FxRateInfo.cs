using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BinanceConnect.Models
{
    public class FxRateInfoList
    {
        [JsonProperty("quotes")]
        public List<FxRateInfo> Quotes { get; set; }
    }

    public class FxRateInfo
    {
        [JsonProperty("high")]
        public decimal High { get; set; }

        [JsonProperty("open")]
        public decimal Open { get; set; }

        [JsonProperty("bid")]
        public decimal Bid { get; set; }

        [JsonProperty("currencyPairCode")]
        public string CurrencyPairCode { get; set; }

        [JsonProperty("ask")]
        public decimal Ask { get; set; }

        [JsonProperty("low")]
        public decimal Low { get; set; }

    }
}

