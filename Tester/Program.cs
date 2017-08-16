using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using GpsUtilities;

namespace Tester
{
    class Program
    {
        private static readonly GpsLine StartLine = GetStartLine();

        private static readonly List<Tuple<GpsPoint, GpsPoint>> Sectors = new List<Tuple<GpsPoint, GpsPoint>>
        {
//            Tuple.Create(new GpsPoint(50.3732507, 30.4650736), new GpsPoint(50.373232, 30.464971)), // start
            Tuple.Create(new GpsPoint(50.372872, 30.464020), new GpsPoint(50.372805, 30.463984)), // before forest
            Tuple.Create(new GpsPoint(50.373169, 30.464741), new GpsPoint(50.373173, 30.464838)), // after bridge
            Tuple.Create(new GpsPoint(50.373091, 30.464332), new GpsPoint(50.373045, 30.464425)), // before start straight
        };

        private static readonly Dictionary<int, double> RealData = new Dictionary<int, double>
        {
            {1, 44.07},
            {2, 43.87},
            {3, 43.91},
            {4, 43.59},
            {5, 44.30},
            {6, 44.08},
            {7, 43.87},
            {8, 43.66},
            {9, 43.69},
            {10, 43.69},
            {11, 43.83},
            {12, 43.79}
        };
        
        // Driver program to test above functions
        public static void Main(string[] args)
        {
            var dir = @"/Users/nestor/Dropbox/Projects/GpsUtilities/Source/Gonsalez_14_08_17";
            var files = Directory.GetFiles(dir, "*.json", SearchOption.AllDirectories);
            
//            Array.Sort(files);
            foreach (var t in files)
            {
                Process(t);
//                break;
            }
//            Process(@"/Users/nestor/Dropbox/pikulin_7.json");
            Console.WriteLine("Process finished");
        }

        public static void Process(string filename)
        {
            var listOfPoints = JsonParser.GetGpsPoints(filename);
            
            var lapCounter = 0;
            long startTime = 0;
            var lapsTime = new List<double>();
            var lapDataSpeedAggregator = new Dictionary<int, List<double>>();
            var csv = new List<string>();
            double lapTime;
//            var sectorsTime = new Dictionary<int, Dictionary<int, long>>();
//            var currentSector = 1;
            for (var i = 0; i < listOfPoints.Count - 1; i++)
            {
                PutDataToAggregator(ref lapDataSpeedAggregator, lapCounter, listOfPoints[i].Speed);
                
                var routeLine  = new GpsLine(listOfPoints[i], listOfPoints[i + 1]); 
                if (!Utils.DoIntersect(StartLine, routeLine)) continue; //skip if no intersects found
                if (lapCounter == 0)
                {
                    startTime = GetTimeByCrossedLine(StartLine, routeLine, startTime).Item2;
                    lapCounter++;
                    continue;
                }
                else
                {
                    //calculate current lap time
                    var lapInfo = GetTimeByCrossedLine(StartLine, routeLine, startTime);    
                    startTime = lapInfo.Item2;    
                    lapTime = lapInfo.Item1;
                }
                    
                lapsTime.Add(lapTime);
                lapCounter++;
            }

//            var counter = 1;
            var info = $"{Path.GetFileName(filename)}. {lapsTime.Count} laps found";
            if (lapsTime.Count > 0)
            {
                info += $" .Best: {lapsTime.Min()}";
            }
            Console.WriteLine(info);


//            var sum = new List<double>();
//            foreach (var time in lapsTime)
//            {
//                var diff = time - RealData[counter];
//                sum.Add(Math.Abs(diff));
//                Console.WriteLine($"Lap {counter}. Time {time}. Diff: {diff}");
//                counter++;
//            }
//            Console.WriteLine($"Total difference {sum.Sum()}. Max diff: {sum.Max()}");
        }

        public static void PutDataToAggregator<T>(ref Dictionary<int, List<T>> aggregator, int lap, T data)
        {
            
            if (!aggregator.ContainsKey(lap)) aggregator[lap] = new List<T>();
            aggregator[lap].Add(data);
        }

        public static Tuple<double, long> GetTimeByCrossedLine(GpsLine sectorLine, GpsLine routeLine, long prevRecordedTime)
        {
            var cros = Utils.LineIntersectionPoint(sectorLine, routeLine);
            var finishPoint = cros.DistanceTo(routeLine.p1) <= cros.DistanceTo(routeLine.p2)
                ? routeLine.p1
                : routeLine.p2;
                
            var distanceToFinish = cros.DistanceTo(finishPoint);
            var timeDiff = distanceToFinish * 1000 / finishPoint.Speed;
            timeDiff *= finishPoint.Equals(routeLine.p1) ? 1 : -1;


            var sectorTime = CalculateLapTime(prevRecordedTime, finishPoint.Utc);
            var startNewSectorTime = finishPoint.Utc + (long) (timeDiff * Utils.UtcCoificient);
            if (prevRecordedTime == 0)
            {
                return Tuple.Create(0.0, startNewSectorTime);
            }
            else
            {
                return Tuple.Create(sectorTime + Math.Round(timeDiff, 2), startNewSectorTime);    
            }
            
        }
        

        public static void WriteToCsv(int filename, Dictionary<int, List<double>> data)
        {
            var fileName = $@"D:\{filename}.csv";
            var writer = File.CreateText(fileName);
            foreach (KeyValuePair<int, List<double>> lap in data)
            {
                writer.WriteLine(string.Join(",", lap.Value.Select(x => x.ToString(CultureInfo.InvariantCulture))));
            }
            writer.Dispose();
        }

        // returns time in seconds based on UTC timestamp
        public static double CalculateLapTime(double prev, double curr)
        {
            return Math.Round((curr - prev) / Utils.UtcCoificient, 2);
        }

        public static GpsLine GetStartLine()
        {
//            return new GpsLine(new GpsPoint(50.373232, 30.464971), new GpsPoint(50.3732507, 30.4650736)); //original line
            return new GpsLine(new GpsPoint(50.373221, 30.464931), new GpsPoint(50.3732507, 30.4650736)); //longer line
        }
    }
}
