using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Microsoft.Win32;
using MediaColor = System.Windows.Media.Color;

namespace CabbeAreaAddin
{
    public partial class AreaCheckerWindow : Window
    {
        private readonly UIDocument _uidoc;
        private AreaAnalysisResult? _currentResult;

        public AreaCheckerWindow(UIDocument uidoc)
        {
            InitializeComponent();
            _uidoc = uidoc;
            Loaded += (s, e) => RunAnalysis();
        }

        private void RunAnalysis()
        {
            try
            {
                _currentResult = AreaCheckerLogic.Analyze(_uidoc.Document);
                UpdateUi();
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Fel vid analys", ex.Message);
            }
        }

        private void UpdateUi()
        {
            if (_currentResult == null) return;

            // Status Banner
            if (_currentResult.IsApproved)
            {
                StatusBanner.Background = new SolidColorBrush((MediaColor)ColorConverter.ConvertFromString("#ECFDF5"));
                StatusBanner.BorderBrush = new SolidColorBrush((MediaColor)ColorConverter.ConvertFromString("#A7F3D0"));
                TxtStatusIcon.Text = "✓";
                TxtStatusIcon.Foreground = new SolidColorBrush((MediaColor)ColorConverter.ConvertFromString("#059669"));
                TxtStatusTitle.Text = "STATUS: GODKÄND";
                TxtStatusTitle.Foreground = new SolidColorBrush((MediaColor)ColorConverter.ConvertFromString("#065F46"));
                TxtStatusDesc.Text = $"Modellen innehåller {_currentResult.PlacedBtaCount} st utplacerade BTA-areor (Totalt: {_currentResult.TotalBtaSqm:N1} m²).";
                TxtStatusDesc.Foreground = new SolidColorBrush((MediaColor)ColorConverter.ConvertFromString("#047857"));
            }
            else
            {
                StatusBanner.Background = new SolidColorBrush((MediaColor)ColorConverter.ConvertFromString("#FEF2F2"));
                StatusBanner.BorderBrush = new SolidColorBrush((MediaColor)ColorConverter.ConvertFromString("#FECACA"));
                TxtStatusIcon.Text = "✗";
                TxtStatusIcon.Foreground = new SolidColorBrush((MediaColor)ColorConverter.ConvertFromString("#DC2626"));
                TxtStatusTitle.Text = "STATUS: UNDERKÄND";
                TxtStatusTitle.Foreground = new SolidColorBrush((MediaColor)ColorConverter.ConvertFromString("#991B1B"));
                TxtStatusDesc.Text = "Modellen saknar utplacerade BTA-areor (Obligatoriskt krav)! Skapa och placera BTA i modellen.";
                TxtStatusDesc.Foreground = new SolidColorBrush((MediaColor)ColorConverter.ConvertFromString("#B91C1C"));
            }

            GridSummary.ItemsSource = _currentResult.Summaries;
            GridPlaced.ItemsSource = _currentResult.PlacedAreas;
            GridUnplaced.ItemsSource = _currentResult.UnplacedAreas;
            TabUnplaced.Header = $"Outplacerade Areor ({_currentResult.UnplacedAreas.Count})";
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            RunAnalysis();
        }

        private void GridPlaced_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GridPlaced.SelectedItem is AreaItem selected)
            {
                HighlightInRevit(selected.ElementId);
            }
        }

        private void BtnZoomSelected_Click(object sender, RoutedEventArgs e)
        {
            if (GridPlaced.SelectedItem is AreaItem selected)
            {
                HighlightInRevit(selected.ElementId);
            }
            else
            {
                MessageBox.Show("Markera en area i tabellen först.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void HighlightInRevit(ElementId id)
        {
            try
            {
                _uidoc.Selection.SetElementIds(new List<ElementId> { id });
                _uidoc.ShowElements(id);
            }
            catch { }
        }

        private void BtnExportCsv_Click(object sender, RoutedEventArgs e)
        {
            if (_currentResult == null) return;

            var sfd = new SaveFileDialog
            {
                FileName = $"Areakontroll_{DateTime.Now:yyyyMMdd_HHmm}.csv",
                Filter = "CSV-fil (*.csv)|*.csv",
                Title = "Exportera areor till CSV"
            };

            if (sfd.ShowDialog() == true)
            {
                try
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("Areatyp;Beskrivning;Status;Antal;Total_Area_m2");
                    foreach (var s in _currentResult.Summaries)
                    {
                        sb.AppendLine($"{s.TypeCode};{s.Description};{s.StatusText};{s.PlacedCount};{s.TotalSqm:F2}");
                    }

                    sb.AppendLine();
                    sb.AppendLine("ElementID;Typ;Namn;AreaScheme;Plan;Area_m2;Placerad");
                    foreach (var a in _currentResult.PlacedAreas)
                    {
                        sb.AppendLine($"{a.IdValue};{a.AreaType};{a.Name};{a.AreaScheme};{a.LevelName};{a.AreaSqm:F2};Ja");
                    }
                    foreach (var a in _currentResult.UnplacedAreas)
                    {
                        sb.AppendLine($"{a.IdValue};{a.AreaType};{a.Name};{a.AreaScheme};{a.LevelName};0;Nej");
                    }

                    File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                    MessageBox.Show("CSV-rapporten har sparats!", "Export klar", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Kunde inte spara filen: {ex.Message}", "Fel", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
