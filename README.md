# Cabbe Areakontroll - Revit C# Add-in

Ett modernt, fristående Revit-tillägg i **C# (.NET 8 & WPF)** för automatisk kvalitetsgranskning av areamodellering enligt svensk standard (SS 21054).

Stödjer **Autodesk Revit 2025** och **Autodesk Revit 2026**.

---

## 🎯 Funktioner

- **Obligatoriskt BTA-krav:** Kontrollerar direkt att modellen innehåller utplacerade BTA-areor (Bruttoarea). Saknas BTA underkänns modellen med tydlig röd banner.
- **Flera svenska areatyper:** Automatisk detektering och summering av **BTA**, **BRA**, **BOA**, **LOA**, **NTA** och **BYA** baserat på både areanamn och *Area Scheme*.
- **Outplacerade areor (*Unplaced/Unbounded*):** Identifierar och larmar för areor som skapats i tabeller men saknar fysiska gränser i modellen.
- **Interaktiv granskning:** Klicka på en area i tabellen för att **markera och zooma in direkt** på objektet i Revit.
- **Export:** Exportera hela sammanställningen till CSV med ett klick.
- **Automatisk Ribbon-integrering:** Skapar/återanvänder fliken **Cabbe** och lägger till knappen under panelen **Areor**.

---

## 🚀 Installation för användare / testare

1. Gå till [Actions](https://github.com/Cabbe86/CabbeAreaAddin/actions) och klicka på det senaste lyckade bygget.
2. Ladda ner zip-filen under **Artifacts** för din Revit-version (`CabbeAreaAddin-Revit-2025` eller `CabbeAreaAddin-Revit-2026`).
3. Extrahera filerna och kör **`Install-Addin.bat`** (eller kopiera `CabbeAreaAddin.dll` och `CabbeAreaAddin.addin` manuellt till `%APPDATA%\Autodesk\Revit\Addins\2025` resp `2026`).
4. Starta Revit – fliken **Cabbe** med panelen **Areor** visas i Ribbon!

---

## 🛠️ Utveckling & Bygg

Projektet bygger mot Revit API via `Nice3point.Revit.Api`:

```bash
# Bygg för Revit 2025
dotnet build src/CabbeAreaAddin/CabbeAreaAddin.csproj -c Release /p:RevitVersion=2025

# Bygg för Revit 2026
dotnet build src/CabbeAreaAddin/CabbeAreaAddin.csproj -c Release /p:RevitVersion=2026
```
