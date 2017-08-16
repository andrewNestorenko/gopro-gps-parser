using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace GpsUtilities
{
    public class Processor
    {

        public int LapCount = 0;
        public Dictionary<int, double> LapsTime = new Dictionary<int, double>();
        public Dictionary<int, List<GpsPoint>> DataPerLap = new Dictionary<int, List<GpsPoint>>();
        
        public Processor(string filename)
        {
            var listOfPoints = JsonParser.GetGpsPoints(filename);
            
            long startTime = 0;
            var csv = new List<string>();
            for (var i = 0; i < listOfPoints.Count - 1; i++)
            {
                var lapTime = 0.0;
                PutDataToAggregator(ref DataPerLap, LapCount, listOfPoints[i]);
                
                var routeLine  = new GpsLine(listOfPoints[i], listOfPoints[i + 1]); 
                if (!Utils.DoIntersect(GetStartLine(), routeLine)) continue; //skip if no intersects found
                if (LapCount == 0)
                {
                    startTime = GetTimeByCrossedLine(GetStartLine(), routeLine, startTime).Item2;
                    LapCount++;
                    continue;
                }
                //calculate current lap time
                var lapInfo = GetTimeByCrossedLine(GetStartLine(), routeLine, startTime);    
                startTime = lapInfo.Item2;    
                lapTime = lapInfo.Item1;

                LapsTime[LapCount] = lapTime;
                LapCount++;
            }
        }

        public List<GpsPoint> GetDataByLap(int lap)
        {
            return DataPerLap.ContainsKey(lap) ? DataPerLap[lap] : new List<GpsPoint>();
        }

        public double GetTimeByLap(int lap)
        {
            return LapsTime.ContainsKey(lap) ? LapsTime[lap] : 0;
        }

        public int GetFastestLapNumber()
        {
            return LapsTime.OrderBy(x => x.Value).FirstOrDefault().Key;
        }

        public double GetFastestLapTime()
        {
            return LapsTime.OrderBy(x => x.Value).FirstOrDefault().Value;
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
            return prevRecordedTime == 0 ? Tuple.Create(0.0, startNewSectorTime) : Tuple.Create(sectorTime + Math.Round(timeDiff, 2), startNewSectorTime);
            
        }

        public static void WriteToCsv(string filename, Dictionary<string, List<double>> data)
        {
            var fileName = $@"/Users/nestor/{filename}";
            var writer = File.CreateText(fileName);
            foreach (var lap in data)
            {
                writer.WriteLine(lap.Key + string.Join(",", lap.Value.Select(x => x.ToString(CultureInfo.InvariantCulture))));
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