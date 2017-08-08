using System;
using System.Reflection.Metadata.Ecma335;
using GpsUtilities;
using Xunit;

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
    }
}