using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using GpsUtilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace WebUI.Controllers
{
    public class MapController : Controller
    {
        
        const string Folder = @"/Users/nestor/Dropbox/Projects/GpsUtilities/Source/Gonsalez_14_08_17/";
        
        // GET
        public IActionResult Index()
        {
            var files = Directory.GetFiles(Folder, "*.json");
            ViewBag.files = files.Select(Path.GetFileName).ToList();
            return View();

        }

        public IActionResult Render()
        {
            StringValues file = @"nestor_8.json";
            Request.Query.TryGetValue("filename", out file);
            var processor = new Processor(string.Concat(Folder, "/", file));
            ViewBag.Processor = processor;
            ViewBag.FasterLapTime = processor.GetFastestLapTime();
            
            
            ViewBag.Points = processor.GetDataByLap(processor.GetFastestLapNumber());
            return View();
        }


        public IActionResult Compare()
        {
            StringValues values = new StringValues(new string[] { 
                "nestor_7.json",
                "pasha_7.json",
                "naum_7.json",
                "labinsky_7.json",
                "hloponin_7.json",
            });

            var data = new Dictionary<string, List<double>>();
            foreach (var file in values)
            {
                var processor = new Processor(string.Concat(Folder, "/", file));
                data[file] = processor.GetDataByLap(processor.GetFastestLapNumber()).Select(x => x.Speed).ToList();
            }
            Processor.WriteToCsv("multi_comparison.json", data);

            return View();
        }
    }
}