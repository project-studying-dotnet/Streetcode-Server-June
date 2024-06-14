using Newtonsoft.Json;

namespace Streetcode.DAL.Entities.Payment
{
    public class BasketOrder
    {
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("qty")]
        public int Qty { get; set; }

        [JsonProperty("sum")]
        public long Sum { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; } = string.Empty;

        [JsonProperty("unit")]
        public string Unit { get; set; } = string.Empty;

        [JsonProperty("code")]
        public string Code { get; set; } = string.Empty;

        [JsonProperty("barcode")]
        public string Barcode { get; set; } = string.Empty;

        [JsonProperty("header")]
        public string Header { get; set; } = string.Empty;

        [JsonProperty("footer")]
        public string Footer { get; set; } = string.Empty;

        [JsonProperty("tax")]
        public List<int> Tax { get; set; }

        [JsonProperty("uktzed")]
        public string Uktzed { get; set; } = string.Empty;
    }
}
