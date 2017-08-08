namespace GpsUtilities
{
    public class GpsPoint
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
        public long Utc;
        public double Speed;

        public GpsPoint(double lat, double lon, long utc, double spd )
        {
            Lat = lat;
            Lon = lon;
            Utc = utc;
            Speed = spd;
        }

        public GpsPoint(double lat, double lon)
        {
            Lat = lat;
            Lon = lon;
        }

        public double DistanceTo(GpsPoint to)
        {
            return Utils.DistanceTo(this, to);
        }
    }
}

