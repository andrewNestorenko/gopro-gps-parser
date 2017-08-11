namespace GpsUtilities
{
    public class GpsLine
    {

        public readonly GpsPoint p1;
        public readonly GpsPoint p2;
        
        public GpsLine(GpsPoint p1, GpsPoint p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var o = (GpsLine) obj;
            return this.p1 == o.p1 && this.p2 == o.p2;
        }
    }
}