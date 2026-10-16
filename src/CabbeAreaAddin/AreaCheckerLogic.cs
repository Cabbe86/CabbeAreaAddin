using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace CabbeAreaAddin
{
    public class AreaAnalysisResult
    {
        public bool IsApproved { get; set; }
        public int PlacedBtaCount { get; set; }
        public double TotalBtaSqm { get; set; }
        public List<AreaSummary> Summaries { get; set; } = new();
        public List<AreaItem> PlacedAreas { get; set; } = new();
        public List<AreaItem> UnplacedAreas { get; set; } = new();
    }

    public static class AreaCheckerLogic
    {
        private const double SqftToSqm = 0.09290304;

        private static readonly Dictionary<string, (string Desc, bool Mandatory)> TargetTypes = new()
        {
            { "BTA", ("Bruttoarea (Obligatorisk)", true) },
            { "BRA", ("Bruksarea", false) },
            { "BOA", ("Boarea", false) },
            { "LOA", ("Lokalarea", false) },
            { "NTA", ("Nettoarea", false) },
            { "BYA", ("Byggnadsarea", false) }
        };

        public static AreaAnalysisResult Analyze(Document doc)
        {
            var result = new AreaAnalysisResult();

            var areas = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Areas)
                .WhereElementIsNotElementType()
                .Cast<Area>()
                .ToList();

            var placedList = new List<AreaItem>();
            var unplacedList = new List<AreaItem>();

            foreach (var area in areas)
            {
                var nameParam = area.get_Parameter(BuiltInParameter.ROOM_NAME);
                string name = nameParam?.AsString()?.Trim() ?? string.Empty;
                string scheme = area.AreaScheme?.Name?.Trim() ?? string.Empty;
                string levelName = area.Level?.Name ?? "Inget plan";
                double levelElevation = area.Level?.Elevation ?? -9999;

                string searchStr = $"{name} {scheme}".ToUpperInvariant();
                string matchedType = "Annan";

                foreach (var code in TargetTypes.Keys)
                {
                    if (searchStr.Contains(code))
                    {
                        matchedType = code;
                        break;
                    }
                }

                bool isPlaced = area.Location != null && area.Area > 0;
                double sqm = Math.Round(area.Area * SqftToSqm, 2);

                var item = new AreaItem
                {
                    ElementId = area.Id,
                    Name = string.IsNullOrWhiteSpace(name) ? "(Namnlös)" : name,
                    AreaScheme = string.IsNullOrWhiteSpace(scheme) ? "-" : scheme,
                    LevelName = levelName,
                    LevelElevation = levelElevation,
                    AreaSqm = sqm,
                    AreaType = matchedType,
                    IsPlaced = isPlaced
                };

                if (isPlaced)
                    placedList.Add(item);
                else
                    unplacedList.Add(item);
            }

            result.PlacedAreas = placedList
                .OrderBy(a => a.LevelElevation)
                .ThenBy(a => a.AreaType)
                .ThenBy(a => a.Name)
                .ToList();

            result.UnplacedAreas = unplacedList
                .OrderBy(a => a.AreaType)
                .ThenBy(a => a.Name)
                .ToList();

            // Bygg sammanställning per typ
            foreach (var kvp in TargetTypes)
            {
                string code = kvp.Key;
                var info = kvp.Value;

                var placedOfType = result.PlacedAreas.Where(a => a.AreaType == code).ToList();
                var unplacedOfType = result.UnplacedAreas.Where(a => a.AreaType == code).ToList();

                double totalSqm = placedOfType.Sum(a => a.AreaSqm);

                result.Summaries.Add(new AreaSummary
                {
                    TypeCode = code,
                    Description = info.Desc,
                    IsMandatory = info.Mandatory,
                    PlacedCount = placedOfType.Count,
                    UnplacedCount = unplacedOfType.Count,
                    TotalSqm = totalSqm
                });

                if (code == "BTA")
                {
                    result.PlacedBtaCount = placedOfType.Count;
                    result.TotalBtaSqm = totalSqm;
                }
            }

            result.IsApproved = result.PlacedBtaCount > 0;
            return result;
        }
    }
}
