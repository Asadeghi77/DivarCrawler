using Newtonsoft.Json;

namespace DivarCrawler.Models
{
    public class Data
    {
        public string @enum { get; set; }
        public string enumName { get; set; }
        public string title { get; set; }
        public string image { get; set; }
        public List<WebImage> web_image { get; set; }
        public string description { get; set; }
        public bool has_chat { get; set; }
        public string red_text { get; set; }
        public string normal_text { get; set; }
        public string token { get; set; }
        public object image_overlay_tag { get; set; }
        public object image_top_left_tag { get; set; }
        public int index { get; set; }

        [JsonProperty("postapi-version")]
        public int PostapiVersion { get; set; }
        public string city { get; set; }
        public string district { get; set; }
        public string category { get; set; }
        public List<string> category_hierarchy { get; set; }
    }
    public class WebImage
    {
        public string src { get; set; }
        public string type { get; set; }
    }

    public class WidgetList
    {
        public string widget_type { get; set; }
        public Data data { get; set; }
    }
    public class Root
    {
        public List<WidgetList> widget_list { get; set; }
    }
}
