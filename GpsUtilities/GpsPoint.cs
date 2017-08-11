using System;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace GpsUtilities
{
    public class GpsPoint
    {
        public double Lat { get; }
        public double Lon { get; }
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

        public override string ToString()
        {
            return string.Join(",", new string[2] {Lat.ToString(), Lon.ToString()});
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            GpsPoint p = obj as GpsPoint;
            return p.Lat == this.Lat && p.Lon == this.Lon;
        }

        public GpsPointScaled ToScaled()
        {
            return new GpsPointScaled(this);
        }
    }
    
    public class GpsPointScaled
    {
        public readonly long Lat;
        public readonly long Lon;
        public readonly long Utc;
        public readonly double Speed;
        public const int X = 10000000;

        public GpsPointScaled(GpsPoint p)
        {
            this.Lat = (long)(p.Lat*X);
            this.Lon = (long)(p.Lon*X);
            this.Utc = p.Utc;
            this.Speed = p.Speed;
        }

        public GpsPointScaled(long Lat, long Lon)
        {
            this.Lat = Lat;
            this.Lon = Lon;
        }

        public GpsPoint ToNormal()
        {
            return new GpsPoint((double)Lat/X, (double)Lon/X);
        }
    }
}

