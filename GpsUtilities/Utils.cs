using System;
using System.Numerics;

namespace GpsUtilities
{
    public class Utils
    {
        
        public const int UtcCoificient = 1000000;
        
        private static bool OnSegment(GpsPointScaled p, GpsPointScaled q, GpsPointScaled r)
        {
            return q.Lat <= Math.Max(p.Lat, r.Lat) && q.Lat >= Math.Min(p.Lat, r.Lat) &&
                   q.Lon <= Math.Max(p.Lon, r.Lon) && q.Lon >= Math.Min(p.Lon, r.Lon);
        }

        // To find orientation of ordered triplet (p, q, r).
        // The function returns following values
        // 0 --> p, q and r are colinear
        // 1 --> Clockwise
        // 2 --> Counterclockwise
        private static int Orientation(GpsPointScaled p, GpsPointScaled q, GpsPointScaled r)
        {
            // See http://www.geeksforgeeks.org/orientation-3-ordered-points/
            // for details of below formula.
            var val = (q.Lon - p.Lon) * (r.Lat - q.Lat) -
                      (q.Lat - p.Lat) * (r.Lon - q.Lon);

            if (Math.Abs(val) < 0) return 0;

            return (val > 0) ? 1 : 2; // clock or counterclock wise
        }

        // The main function that returns true if line segment 'p1q1'
        // and 'p2q2' intersect.
        public static bool DoIntersect(GpsPointScaled p1, GpsPointScaled q1, GpsPointScaled p2, GpsPointScaled q2)
        {
            // Find the four orientations needed for general and
            // special cases
            var o1 = Orientation(p1, q1, p2);
            var o2 = Orientation(p1, q1, q2);
            var o3 = Orientation(p2, q2, p1);
            var o4 = Orientation(p2, q2, q1);

            // General case
            if (o1 != o2 && o3 != o4)
                return true;

            // Special Cases
            // p1, q1 and p2 are colinear and p2 lies on segment p1q1
            if (o1 == 0 && OnSegment(p1, p2, q1)) return true;

            // p1, q1 and p2 are colinear and q2 lies on segment p1q1
            if (o2 == 0 && OnSegment(p1, q2, q1)) return true;

            // p2, q2 and p1 are colinear and p1 lies on segment p2q2
            if (o3 == 0 && OnSegment(p2, p1, q2)) return true;

            // p2, q2 and q1 are colinear and q1 lies on segment p2q2
            if (o4 == 0 && OnSegment(p2, q1, q2)) return true;

            return false; // Doesn't fall in any of the above cases
        }

        public static bool DoIntersect(GpsLine p, GpsLine q)
        {
            
            return DoIntersect(p.p1.ToScaled(), p.p2.ToScaled(), q.p1.ToScaled(), q.p2.ToScaled());
        }

        public static double DistanceTo(double lat1, double lon1, double lat2, double lon2, char unit = 'K')
        {
            var rlat1 = Math.PI * lat1 / 180;
            var rlat2 = Math.PI * lat2 / 180;
            var theta = lon1 - lon2;
            var rtheta = Math.PI * theta / 180;
            var dist = Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) * Math.Cos(rlat2) * Math.Cos(rtheta);
            dist = Math.Acos(dist);
            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;

            switch (unit)
            {
               
                case 'K': //Kilometers -> default
                    return dist * 1.609344;
                case 'N': //Nautical Miles 
                    return dist * 0.8684;
                case 'M': //Miles
                    return dist;
            }

            return dist;
        }

        public static double DistanceTo(GpsPoint p1, GpsPoint p2)
        {
            return DistanceTo(p1.Lat, p1.Lon, p2.Lat, p2.Lon);
        }
        
        public static GpsPoint Crossing(GpsPoint p1, GpsPoint p2, GpsPoint p3, GpsPoint p4)
        {
            
            if (p3.Lat.Equals(p4.Lat))   // vertical
            {
                var y = p1.Lon + ((p2.Lon - p1.Lon) * (p3.Lat - p1.Lat)) / (p2.Lat - p1.Lat);
                if (y > Math.Max(p3.Lon, p4.Lon) || y < Math.Min(p3.Lon, p4.Lon) || y > Math.Max(p1.Lon, p2.Lon) || y < Math.Min(p1.Lon, p2.Lon))   // если за пределами отрезков
                    return new GpsPoint(0, 0);
                return new GpsPoint(p3.Lat, y);
            }
            var x = p1.Lat + ((p2.Lat - p1.Lat) * (p3.Lon - p1.Lon)) / (p2.Lon - p1.Lon);
            if (x > Math.Max(p3.Lat, p4.Lat) || x < Math.Min(p3.Lat, p4.Lat) || x > Math.Max(p1.Lat, p2.Lat) || x < Math.Min(p1.Lat, p2.Lat))   // если за пределами отрезков
                return new GpsPoint(0, 0);
            return new GpsPoint(x, p3.Lon);
        }

        public static GpsPoint Crossing(GpsLine p1, GpsLine p2)
        {
            return Crossing(p1.p1, p1.p2, p2.p1, p2.p2);
        }
        
        public static GpsPoint LineIntersectionPoint(GpsLine p1, GpsLine p2)
        {
            var point = LineIntersectionPoint(p1.p1.ToScaled(), p1.p2.ToScaled(), p2.p1.ToScaled(), p2.p2.ToScaled());
            
            return point.ToNormal();

        }
        
        
        public static GpsPointScaled LineIntersectionPoint(GpsPointScaled ps1, GpsPointScaled pe1, GpsPointScaled ps2, GpsPointScaled pe2)
        {
            // Get A,B,C of first line - points : ps1 to pe1
            var  A1 = pe1.Lon-ps1.Lon;
            var  B1 = ps1.Lat-pe1.Lat;
            var  C1 = A1*ps1.Lat+B1*ps1.Lon;
 
            var  A2 = pe2.Lon-ps2.Lon;
            var  B2 = ps2.Lat-pe2.Lat;
            var  C2 = A2*ps2.Lat+B2*ps2.Lon;
 
            // Get delta and check if the lines are parallel
            var delta = A1*B2 - A2*B1;
            if(delta == 0 )
                throw new System.Exception("Lines are parallel");
 
            // now return the Vector2 intersection point
            return new GpsPointScaled(
                (B2*C1 - B1*C2)/delta,
                (A1*C2 - A2*C1)/delta
            );
        }
        
        public static Vector2 LineIntersectionPoint(Vector2 ps1, Vector2 pe1, Vector2 ps2, Vector2 pe2)
        {
            // Get A,B,C of first line - points : ps1 to pe1
            var A1 = pe1.Y-ps1.Y;
            var B1 = ps1.X-pe1.X;
            var C1 = A1*ps1.X+B1*ps1.Y;
 
            //t A,B,C of second line - points : ps2 to pe2
            var A2 = pe2.Y-ps2.Y;
            var B2 = ps2.X-pe2.X;
            var C2 = A2*ps2.X+B2*ps2.Y;
 
            // Get delta and check if the lines are parallel
            var delta = A1*B2 - A2*B1;
            if(delta == 0)
                throw new System.Exception("Lines are parallel");
 
            // now return the Vector2 intersection point
            return new Vector2(
                (B2*C1 - B1*C2)/delta,
                (A1*C2 - A2*C1)/delta
            );
        }
        
    }
}