using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;
using GpsUtilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Tester
{
    class Program
    {
                    
        private static readonly Tuple<GpsPoint, GpsPoint> StartLine = GetStartLine();

        private static readonly Dictionary<int, double> RealData = new Dictionary<int, double>
        {
            {1, 39.82},
            {2, 40.29},
            {3, 40.02},
            {4, 40.31},
            {5, 40.25},
            {6, 39.90},
            {7, 39.91},
            {8, 40.32},
            {9, 40.15},
            {10, 39.80},
            {13, 39.65},
            {21, 39.88},
        };
        
        // Driver program to test above functions
        public static void Main(string[] args)
        {
            var files = Directory.GetFiles(@"D:\GoPro\Gonsalez11.01");
            Array.Sort(files);
            foreach (var t in files)
            {
                Process(t);
            }
            
        }

        public static void Process(string filename)
        {
            var gpsData = JsonParser.ParseByFileName(filename);
            int.TryParse(Path.GetFileNameWithoutExtension(filename).Substring(3), out var carNumber);
            var listOfPoints = gpsData.data.Select(x => new GpsPoint(x.lat, x.lon, x.utc, x.spd)).ToList();
            var counter = 0;
            long startTime = 0;
            var laps = new List<double>();
            for (var i = 0; i < listOfPoints.Count - 1; i++)
            {
                if (!Utils.DoIntersect(StartLine.Item1, StartLine.Item2, listOfPoints[i], listOfPoints[i + 1])) continue;

                // find crossing point
                var cros = Utils.Crossing(StartLine.Item1, StartLine.Item2, listOfPoints[i], listOfPoints[i + 1]);
                if (counter > 0) 
                {
                    var finishPoint = cros.DistanceTo(listOfPoints[i]) > cros.DistanceTo(listOfPoints[i+1]) ? listOfPoints[i+1] : listOfPoints[i];
                    
                    var distanceToFinish = cros.DistanceTo(finishPoint);
                    var timeDiff = distanceToFinish * 1000 / finishPoint.Speed;
                    var lapTime = CalculateLapTime(startTime, finishPoint.Utc);// +  timeDiff;
                    laps.Add(lapTime);
                } 
                
                counter++;
                startTime = listOfPoints[i].Utc;
            }
            
            Console.WriteLine($"Car:{carNumber}. Laps:{counter}. Best:{laps.Min()}. Diff:{RealData[carNumber]-laps.Min()}");
        }

        public static double CalculateLapTime(double prev, double curr)
        {
            return (curr - prev) / 1000000;
        }

        public static Tuple<GpsPoint, GpsPoint> GetStartLine()
        {
            return Tuple.Create(new GpsPoint(50.3732507, 30.4650736), new GpsPoint(50.373232, 30.464971));
        }
    }
}
