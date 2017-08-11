using System;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices.ComTypes;
using GpsUtilities;
using Xunit;
using Xunit.Sdk;

namespace UnitTests
{
    public class UnitTest1
    {
        [Fact]
        public void Test_DistanceToPoint()
        {
            var distnace = new GpsPoint(50.3732507, 30.4650736).DistanceTo(new GpsPoint(50.373232, 30.464971));
            Assert.Equal(Math.Round(distnace, 6), 0.007567);
        }

        [Fact]
        public void Test_DistanceToCrossPoint()
        {
            var crossPoint = new GpsPoint(50.3732471, 30.4650542);
            
            var distance = new GpsPoint(50.3732507, 30.4650736).DistanceTo(new GpsPoint(50.373232, 30.464971));
            
            Assert.True(
                Math.Abs(new GpsPoint(50.3732507, 30.4650736).DistanceTo(crossPoint) + new GpsPoint(50.373232, 30.464971).DistanceTo(crossPoint) - distance) < 0.00001
            );
        }


        [Fact]
//        [InlineData(]
        public void Test_CrossPoint()//GpsLine p1, GpsLine p2, GpsPoint cross)
        {
            var line1 = new GpsLine(new GpsPoint(50.426117, 30.503304), new GpsPoint(50.426455, 30.504652));
            var line2 = new GpsLine(new GpsPoint(50.426458, 30.503837), new GpsPoint(50.426051, 30.504221));
//            var crossPoint = new GpsPoint();
            var crossing = Utils.LineIntersectionPoint(line1, line2);
            Assert.False(crossing.Equals(new GpsPoint(0, 0)));
            Assert.True(crossing.Equals(new GpsPoint(50.42629, 30.50400)));
        }

    }

}