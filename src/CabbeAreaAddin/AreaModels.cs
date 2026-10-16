using System;
using Autodesk.Revit.DB;

namespace CabbeAreaAddin
{
    public class AreaItem
    {
        public ElementId ElementId { get; set; } = ElementId.InvalidElementId;
        public long IdValue => ElementId.Value;
        public string Name { get; set; } = string.Empty;
        public string AreaScheme { get; set; } = string.Empty;
        public string LevelName { get; set; } = string.Empty;
        public double LevelElevation { get; set; }
        public double AreaSqm { get; set; }
        public string AreaType { get; set; } = "Annan";
        public bool IsPlaced { get; set; }
        public string StatusDescription => IsPlaced ? $"{AreaSqm:N1} m²" : "Outplacerad (saknar gräns)";
    }

    public class AreaSummary
    {
        public string TypeCode { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsMandatory { get; set; }
        public int PlacedCount { get; set; }
        public int UnplacedCount { get; set; }
        public double TotalSqm { get; set; }
        
        public string StatusText
        {
            get
            {
                if (IsMandatory)
                {
                    if (PlacedCount > 0)
                        return UnplacedCount > 0 ? "⚠️ Placerad (har outplacerade)" : "✓ Godkänd";
                    return "✗ SAKNAS (KRAV)";
                }

                if (PlacedCount > 0)
                    return UnplacedCount > 0 ? "⚠️ Placerad (har outplacerade)" : "✓ Förekommer";
                return "- Ej i modellen";
            }
        }

        public string StatusColor
        {
            get
            {
                if (IsMandatory && PlacedCount == 0) return "#DC2626"; // Röd
                if (PlacedCount > 0 && UnplacedCount == 0) return "#059669"; // Grön
                if (PlacedCount > 0 && UnplacedCount > 0) return "#D97706"; // Orange
                return "#64748B"; // Grå
            }
        }

        public string DisplayArea => PlacedCount > 0 ? $"{TotalSqm:N1} m²" : "-";
    }
}
