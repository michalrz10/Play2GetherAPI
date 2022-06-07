using System.Collections.Generic;

namespace Play2GetherAPI.ControllerModels
{
    public class Point
    {
        public Point(float lon, float lat)
        {
            type = "Point";
            coordinates = new List<float>() { lon, lat };
        }
        public string type { get; set; }
        public List<float> coordinates { get; set; }
    }
}
