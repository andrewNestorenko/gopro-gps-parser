using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace GpsUtilities
{
    public class JsonParser
    {
        public static GpsData ParseByFileName(string file)
        {
            if (File.Exists(file))
            {
                return JsonConvert.DeserializeObject<GpsData>(
                    File.ReadAllText(file)
                );    
            }
            throw new FileNotFoundException();
        }
        
        public static List<GpsPoint> GetGpsPoints(string filename)
        {
            var gpsData = ParseByFileName(filename);
            return gpsData.data.Select(x => new GpsPoint(x.lat, x.lon, x.utc, x.spd)).ToList();    
        }
        
    }

    public class JsonLine
    {
        public double lat;
        public double lon;
        public double alt;
        public double spd;
        public double spd_3d;
        public long utc;
        public float gps_accuracy;
        public int gps_fix;
        public double temp;
    }

    public class GpsData
    {
        public List<JsonLine> data;
    }
}