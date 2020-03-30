﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using AnnoDesigner.model;
using NLog;

namespace AnnoDesigner.Localization
{
    public class TreeLocalization : ILocalizationHelper
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static Dictionary<string, Dictionary<string, string>> Translations;

        private readonly ICommons _commons;

        static TreeLocalization()
        {
            //This dictionary initialisation can be find on :
            //https://docs.google.com/spreadsheets/d/1CjECty43mkkm1waO4yhQl1rzZ-ZltrBgj00aq-WJX4w/edit?usp=sharing 
            //Steps to format:
            //Run CreateDictionary Script
            //Copy Output
            //Replace the escaped characters (\t\r\n) with the actual characters from within an editor of your choice
            Translations = new Dictionary<string, Dictionary<string, string>>()
            {
                {
                    "eng", new Dictionary<string, string>() {
                        { "RoadTile" , "Road tile" },
                        { "BorderlessRoadTile" , "Borderless road tile" },
                        { "Military" , "Military" },
                        { "Camps" , "Camps" },
                        { "Production" , "Production" },
                        { "Ornament" , "Ornaments" },
                        { "ManorialPalace" , "Manorial Palace" },
                        { "PlayerBuildings" , "Player Buildings" },
                        { "Harbour" , "Harbour" },
                        { "Harbor" , "Harbour" },
                        { "Residence" , "Residence" },
                        { "Animalfarm" , "Animalfarm" },
                        { "Factory" , "Factory" },
                        { "Farm" , "Farm" },
                        { "FarmFields" , "Farm Fields" },
                        { "Plantation" , "Plantation" },
                        { "Resource" , "Resource" },
                        { "Public" , "Public Buildings" },
                        { "PublicBuildings" , "Public Buildings" },
                        { "Demand" , "Demand" },
                        { "Marine" , "Marine" },
                        { "Special" , "Special" },
                        { "Trade" , "Trade" },
                        { "(1)Ecos" , "(1) Ecos" },
                        { "(2)Tycoons" , "(2) Tycoons" },
                        { "(3)Techs" , "(3) Techs" },
                        { "Others" , "Others" },
                        { "Energy" , "Energy" },
                        { "BlackSmokers(DeepSea)" , "Black Smokers (Deep Sea)" },
                        { "BlackSmokers(Normal)" , "Black Smokers (Normal)" },
                        { "(1)Earth" , "(1) Earth" },
                        { "(2)Arctic" , "(2) Arctic" },
                        { "(3)Moon" , "(3) Moon" },
                        { "(4)Tundra" , "(4) Tundra" },
                        { "(5)Orbit" , "(5) Orbit" },
                        { "Facilities" , "Facilities" },
                        { "Ornaments" , "Ornaments" },
                        { "GlobalModules" , "Global Modules" },
                        { "Laboratories" , "Laboratories" },
                        { "Modules" , "Modules" },
                        { "Agriculture" , "Agriculture" },
                        { "Biotech" , "Biotech" },
                        { "Chemical" , "Chemical" },
                        { "Electronics" , "Electronics" },
                        { "Food" , "Food" },
                        { "Heavy" , "Heavy" },
                        { "Mining" , "Mining" },
                        { "TundraModules" , "Tundra Modules" },
                        { "IndustryModules" , "Industry Modules" },
                        { "CentralTechUnitReactor" , "Central Tech Unit Reactor" },
                        { "(01)Farmers" , "(01) Farmers" },
                        { "(02)Workers" , "(02) Workers" },
                        { "(03)Artisans" , "(03) Artisans" },
                        { "(04)Engineers" , "(04) Engineers" },
                        { "(05)Investors" , "(05) Investors" },
                        { "(06)OldWorldFields" , "(06) Old World Fields" },
                        { "(07)Jornaleros" , "(07) Jornaleros" },
                        { "(08)Obreros" , "(08) Obreros" },
                        { "(09)NewWorldFields" , "(09) New World Fields" },
                        { "(10)Explorers" , "(10) Explorers" },
                        { "(11)Technicians" , "(11) Technicians" },
                        { "(12)ArcticFarmFields" , "(12) Arctic Farm Fields" },
                        { "AllWorlds" , "All Worlds" },
                        { "Attractiveness" , "Attractiveness" },
                        { "Electricity" , "Electricity" },
                        { "FarmBuildings" , "Farm Buildings" },
                        { "ProductionBuildings" , "Production Buildings" },
                        { "Depots" , "Depots" },
                        { "Logistics" , "Logistics" },
                        { "Shipyards" , "Shipyards" },
                        { "SpecialBuildings" , "Special Buildings" },
                        { "MiningBuildings" , "Mining Buildings" },
                        { "OrnamentalBuilding" , "Ornaments" },
                        { "-RoadPresets" , "- Road Presets" },
                        { "Roads(x3)" , "Roads (x3)" },
                        { "Roads(x4)" , "Roads (x4)" },
                        { "Roads(x5)" , "Roads (x5)" },
                        { "OtherRoads" , "Other Roads" }
                    }
                    },
                {
                    "ger", new Dictionary<string, string>() {
                        { "RoadTile" , "Straßenkachel" },
                        { "BorderlessRoadTile" , "Straßenkachel (randlos)" },
                        { "Military" , "Militär" },
                        { "Camps" , "Lager" },
                        { "Production" , "Produktion" },
                        { "Ornament" , "Ornamente" },
                        { "ManorialPalace" , "Herrschaftspalast" },
                        { "PlayerBuildings" , "Spielergebäude" },
                        { "Harbour" , "Hafen" },
                        { "Harbor" , "Hafen" },
                        { "Residence" , "Wohnsitz" },
                        { "Animalfarm" , "Tierfarm" },
                        { "Factory" , "Fabrik" },
                        { "Farm" , "Bauernhof" },
                        { "FarmFields" , "Farm-Felder" },
                        { "Plantation" , "Pflanzung" },
                        { "Resource" , "Ressource" },
                        { "Public" , "Öffentliche Gebäude" },
                        { "PublicBuildings" , "Öffentliche Gebäude" },
                        { "Demand" , "Nachfrage" },
                        { "Marine" , "Marine" },
                        { "Special" , "Spezial" },
                        { "Trade" , "Handel" },
                        { "(1)Ecos" , "(1) Ecos" },
                        { "(2)Tycoons" , "(2) Tycoons" },
                        { "(3)Techs" , "(3) Techniker" },
                        { "Others" , "Andere" },
                        { "Energy" , "Energie" },
                        { "BlackSmokers(DeepSea)" , "Schwarze Raucher (Tiefsee)" },
                        { "BlackSmokers(Normal)" , "Schwarze Raucher (Normal)" },
                        { "(1)Earth" , "(1) Erde" },
                        { "(2)Arctic" , "(2) Arktis" },
                        { "(3)Moon" , "(3) Moon" },
                        { "(4)Tundra" , "(4) Tundra" },
                        { "(5)Orbit" , "(5) Umlaufbahn" },
                        { "Facilities" , "Einrichtungen" },
                        { "Ornaments" , "Ornamente" },
                        { "GlobalModules" , "Globale Module" },
                        { "Laboratories" , "Laboratorien" },
                        { "Modules" , "Module" },
                        { "Agriculture" , "Landwirtschaft" },
                        { "Biotech" , "Biotechnologie" },
                        { "Chemical" , "Chemikalie" },
                        { "Electronics" , "Elektronik" },
                        { "Food" , "Lebensmittel" },
                        { "Heavy" , "Schwer" },
                        { "Mining" , "Bergbau" },
                        { "TundraModules" , "Tundra-Module" },
                        { "IndustryModules" , "Industriemodule" },
                        { "CentralTechUnitReactor" , "Zentraler Tech-Unit-Reaktor" },
                        { "(01)Farmers" , "(01) Bauern" },
                        { "(02)Workers" , "(02) Arbeiter" },
                        { "(03)Artisans" , "(03) Handwerker" },
                        { "(04)Engineers" , "(04) Ingenieure" },
                        { "(05)Investors" , "(05) Investoren" },
                        { "(06)OldWorldFields" , "(06) Felder der Alten Welt" },
                        { "(07)Jornaleros" , "(07) Jornaleros" },
                        { "(08)Obreros" , "(08) Obreros" },
                        { "(09)NewWorldFields" , "(09) Felder der Neuen Welt" },
                        { "(10)Explorers" , "(10) Entdecker" },
                        { "(11)Technicians" , "(11) Techniker" },
                        { "(12)ArcticFarmFields" , "(12) Felder der Arktis" },
                        { "AllWorlds" , "Alle Welten" },
                        { "Attractiveness" , "Attraktivität" },
                        { "Electricity" , "Elektrizität" },
                        { "FarmBuildings" , "Bauernhofgebäude" },
                        { "ProductionBuildings" , "Produktionsgebäude" },
                        { "Depots" , "Depots" },
                        { "Logistics" , "Logistik" },
                        { "Shipyards" , "Schiffswerften" },
                        { "SpecialBuildings" , "Spezielle Gebäude" },
                        { "MiningBuildings" , "Bergbau Gebäude" },
                        { "OrnamentalBuilding" , "Ornamente" },
                        { "-RoadPresets" , "- Straßenvoreinstellungen" },
                        { "Roads(x3)" , "Straßen (x3)" },
                        { "Roads(x4)" , "Straßen (x4)" },
                        { "Roads(x5)" , "Straßen (x5)" },
                        { "OtherRoads" , "Andere Straßen" }
                    }
                    },
                 {
                    "fra", new Dictionary<string, string>() {
                        { "RoadTile" , "Route" },
                        { "BorderlessRoadTile" , "Route sans bordure" },
                        { "Military" , "Militaire" },
                        { "Camps" , "Camps" },
                        { "Production" , "Production" },
                        { "Ornament" , "Ornements" },
                        { "ManorialPalace" , "Palais" },
                        { "PlayerBuildings" , "Bâtiments du joueur" },
                        { "Harbour" , "Port" },
                        { "Harbor" , "Port" },
                        { "Residence" , "Résidence" },
                        { "Animalfarm" , "Ferme animalière" },
                        { "Factory" , "Usinek" },
                        { "Farm" , "Ferme animalière" },
                        { "FarmFields" , "Champs agricole" },
                        { "Plantation" , "Plantation" },
                        { "Resource" , "Ressource" },
                        { "Public" , "Bâtiments publics" },
                        { "PublicBuildings" , "Bâtiments publics" },
                        { "Demand" , "Demande" },
                        { "Marine" , "Marine" },
                        { "Special" , "Spécial" },
                        { "Trade" , "Échange" },
                        { "(1)Ecos" , "(1) Écos" },
                        { "(2)Tycoons" , "(2) Entrepreneurs" },
                        { "(3)Techs" , "(3) Techniciens" },
                        { "Others" , "Autres" },
                        { "Energy" , "Énergie" },
                        { "BlackSmokers(DeepSea)" , "Fumeurs noirs (Grands fonds marins)" },
                        { "BlackSmokers(Normal)" , "Fumeurs noirs (Normal)" },
                        { "(1)Earth" , "(1) Terre" },
                        { "(2)Arctic" , "(2) Arctique" },
                        { "(3)Moon" , "(3) Lune" },
                        { "(4)Tundra" , "(4) Toundra" },
                        { "(5)Orbit" , "(5) Orbite" },
                        { "Facilities" , "Installations" },
                        { "Ornaments" , "Ornements" },
                        { "GlobalModules" , "Modules global" },
                        { "Laboratories" , "Laboratoires" },
                        { "Modules" , "Modules" },
                        { "Agriculture" , "Agriculture" },
                        { "Biotech" , "Biotechnologie" },
                        { "Chemical" , "Chimique" },
                        { "Electronics" , "Électronique" },
                        { "Food" , "Alimentation" },
                        { "Heavy" , "Lourd" },
                        { "Mining" , "Exploitation minière" },
                        { "TundraModules" , "Modules de la Toundra" },
                        { "IndustryModules" , "Modules industriel" },
                        { "CentralTechUnitReactor" , "Réacteur de l'unité de technologie" },
                        { "(01)Farmers" , "(01) Fermiers" },
                        { "(02)Workers" , "(02) Ouvriers" },
                        { "(03)Artisans" , "(03) Artisans" },
                        { "(04)Engineers" , "(04) Ingénieurs" },
                        { "(05)Investors" , "(05) Investisseurs" },
                        { "(06)OldWorldFields" , "(06) L'ancien monde" },
                        { "(07)Jornaleros" , "(07) Jornaleros" },
                        { "(08)Obreros" , "(08) Obreros" },
                        { "(09)NewWorldFields" , "(09) Nouveau monde" },
                        { "(10)Explorers" , "(10) Explorateurs" },
                        { "(11)Technicians" , "(11) Techniciens" },
                        { "(12)ArcticFarmFields" , "(12) Champs agricoles de l'Arctique" },
                        { "AllWorlds" , "Tous les mondes" },
                        { "Attractiveness" , "Attractivité" },
                        { "Electricity" , "Électricité" },
                        { "FarmBuildings" , "Bâtiments agricoles" },
                        { "ProductionBuildings" , "Bâtiments de production" },
                        { "Depots" , "Dépôts" },
                        { "Logistics" , "Logistique" },
                        { "Shipyards" , "Chantiers navals" },
                        { "SpecialBuildings" , "Bâtiments spéciaux" },
                        { "MiningBuildings" , "Bâtiments miniers" },
                        { "OrnamentalBuilding" , "Ornements" },
                        { "-RoadPresets" , "- Schémas Routiers" },
                        { "Roads(x3)" , "Routes (x3)" },
                        { "Roads(x4)" , "Routes (x4)" },
                        { "Roads(x5)" , "Routes (x5)" },
                        { "OtherRoads" , "Autres Routes" }
                    }
                    },
                {
                    "pol", new Dictionary<string, string>() {
                        { "RoadTile" , "Płytka drogowa" },
                        { "BorderlessRoadTile" , "Płytka drogowa bez granic" },
                        { "Military" , "Wojsko" },
                        { "Camps" , "Obozy" },
                        { "Production" , "Produkcja" },
                        { "Ornament" , "Ornamenty" },
                        { "ManorialPalace" , "Pałac Dworski" },
                        { "PlayerBuildings" , "Budynki graczy" },
                        { "Harbour" , "Port" },
                        { "Harbor" , "Port" },
                        { "Residence" , "Rezydencja" },
                        { "Animalfarm" , "Hodowla" },
                        { "Factory" , "Fabryka" },
                        { "Farm" , "Farma" },
                        { "FarmFields" , "Pola uprawne" },
                        { "Plantation" , "Plantacja" },
                        { "Resource" , "Zasoby" },
                        { "Public" , "Budynki publiczne" },
                        { "PublicBuildings" , "Budynki publiczne" },
                        { "Demand" , "Popyt" },
                        { "Marine" , "Żegluga morska" },
                        { "Special" , "Specjalne" },
                        { "Trade" , "Handel" },
                        { "(1)Ecos" , "(1) Ekosi" },
                        { "(2)Tycoons" , "(2) Fabryci" },
                        { "(3)Techs" , "(3) Technosi" },
                        { "Others" , "Inne" },
                        { "Energy" , "Energia" },
                        { "BlackSmokers(DeepSea)" , "Kominy hydrotermalne (Błękitna Głębia)" },
                        { "BlackSmokers(Normal)" , "Kominy hydrotermalne (normalnie)" },
                        { "(1)Earth" , "(1) Ziemia" },
                        { "(2)Arctic" , "(2) Arktyka" },
                        { "(3)Moon" , "(3) Księżyc" },
                        { "(4)Tundra" , "(4) Tundra" },
                        { "(5)Orbit" , "(5) Orbita" },
                        { "Facilities" , "Udogodnienia" },
                        { "Ornaments" , "Ornamenty" },
                        { "GlobalModules" , "Moduły globalne" },
                        { "Laboratories" , "Laboratoria" },
                        { "Modules" , "Moduły" },
                        { "Agriculture" , "Rolnictwo" },
                        { "Biotech" , "Biotechnologia" },
                        { "Chemical" , "Chemikalia" },
                        { "Electronics" , "Elektronika" },
                        { "Food" , "Żywność" },
                        { "Heavy" , "Ciężki" },
                        { "Mining" , "Górnictwo" },
                        { "TundraModules" , "Moduły Tundra" },
                        { "IndustryModules" , "Moduły branżowe" },
                        { "CentralTechUnitReactor" , "Reaktor centralnej jednostki technicznej" },
                        { "(01)Farmers" , "(01) Farmerzy" },
                        { "(02)Workers" , "(02) Robotnicy" },
                        { "(03)Artisans" , "(03) Rzemieślnicy" },
                        { "(04)Engineers" , "(04) Inżynierowie" },
                        { "(05)Investors" , "(05) Inwestorzy" },
                        { "(06)OldWorldFields" , "(06) Pola Starego Świata" },
                        { "(07)Jornaleros" , "(07) Jornaleros" },
                        { "(08)Obreros" , "(08) Obreros" },
                        { "(09)NewWorldFields" , "(09) Pola Nowego Świata" },
                        { "(10)Explorers" , "(10) odkrywcy" },
                        { "(11)Technicians" , "(11) Technicy" },
                        { "(12)ArcticFarmFields" , "(12) Pola uprawne arktycznej" },
                        { "AllWorlds" , "Wszystkie światy" },
                        { "Attractiveness" , "Atrakcyjność" },
                        { "Electricity" , "Energia elektryczna" },
                        { "FarmBuildings" , "Budynki rolnicze" },
                        { "ProductionBuildings" , "Budynki produkcyjne" },
                        { "Depots" , "Magazyny" },
                        { "Logistics" , "Logistyka" },
                        { "Shipyards" , "Stocznie" },
                        { "SpecialBuildings" , "Budynki specjalne" },
                        { "MiningBuildings" , "Budynki górnicze" },
                        { "OrnamentalBuilding" , "Ornamenty" },
                        { "-RoadPresets" , "- Wstępne nastawy drogowe" },
                        { "Roads(x3)" , "Drogi (x3)" },
                        { "Roads(x4)" , "Drogi (x4)" },
                        { "Roads(x5)" , "Drogi (x5)" },
                        { "OtherRoads" , "Inne Drogi" }
                    }
                    },
                {
                    "rus", new Dictionary<string, string>() {
                        { "RoadTile" , "Плитка для дорог" },
                        { "BorderlessRoadTile" , "Плитка для безграничных дорог" },
                        { "Military" , "Военные" },
                        { "Camps" , "Лагеря" },
                        { "Production" , "Производство" },
                        { "Ornament" , "Орнаменты" },
                        { "ManorialPalace" , "Поместный дворец" },
                        { "PlayerBuildings" , "Здания для игроков" },
                        { "Harbour" , "гавань" },
                        { "Harbor" , "гавань" },
                        { "Residence" , "Резиденция" },
                        { "Animalfarm" , "Анимальфарм" },
                        { "Factory" , "Завод" },
                        { "Farm" , "Ферма" },
                        { "FarmFields" , "Фермерские поля" },
                        { "Plantation" , "Плантация" },
                        { "Resource" , "Ресурс" },
                        { "Public" , "Общественные здания" },
                        { "PublicBuildings" , "Общественные здания" },
                        { "Demand" , "Спрос" },
                        { "Marine" , "Морпех" },
                        { "Special" , "Специально" },
                        { "Trade" , "Торговля" },
                        { "(1)Ecos" , "(1) Экос" },
                        { "(2)Tycoons" , "(2) Талисманы" },
                        { "(3)Techs" , "(3) Техники" },
                        { "Others" , "Другие" },
                        { "Energy" , "Энергетика" },
                        { "BlackSmokers(DeepSea)" , "Чернокожие курильщики (Глубоководье)" },
                        { "BlackSmokers(Normal)" , "Чернокожие курильщики (нормальные)" },
                        { "(1)Earth" , "(1) Земля" },
                        { "(2)Arctic" , "(2) Арктика" },
                        { "(3)Moon" , "(3) Луна" },
                        { "(4)Tundra" , "(4) Тундра" },
                        { "(5)Orbit" , "(5) Орбита" },
                        { "Facilities" , "Объекты" },
                        { "Ornaments" , "Орнаменты" },
                        { "GlobalModules" , "Глобальные модули" },
                        { "Laboratories" , "Лаборатории" },
                        { "Modules" , "Модули" },
                        { "Agriculture" , "Сельское хозяйство" },
                        { "Biotech" , "Биотехника" },
                        { "Chemical" , "Химикаты" },
                        { "Electronics" , "Электроника" },
                        { "Food" , "Продукты" },
                        { "Heavy" , "Тяжелый" },
                        { "Mining" , "Горное дело" },
                        { "TundraModules" , "Модули тундры" },
                        { "IndustryModules" , "Модули промышленности" },
                        { "CentralTechUnitReactor" , "Реактор центрального технологического блока" },
                        { "(01)Farmers" , "(01) Фермеры" },
                        { "(02)Workers" , "(02) Рабочие" },
                        { "(03)Artisans" , "(03) Ремесленники" },
                        { "(04)Engineers" , "(04) Инженеры" },
                        { "(05)Investors" , "(05) Инвесторы" },
                        { "(06)OldWorldFields" , "(06) Поля Старого Света" },
                        { "(07)Jornaleros" , "(07) Хорналерос" },
                        { "(08)Obreros" , "(08) Обрерос" },
                        { "(09)NewWorldFields" , "(09) Новые Поля Мира" },
                        { "(10)Explorers" , "(10) odkrywcy" },
                        { "(11)Technicians" , "(11) Technicy" },
                        { "(12)ArcticFarmFields" , "(12) Pola uprawne arktycznej" },
                        { "AllWorlds" , "Все миры" },
                        { "Attractiveness" , "Привлекательность" },
                        { "Electricity" , "Электричество" },
                        { "FarmBuildings" , "Фермерские здания" },
                        { "ProductionBuildings" , "Производственные здания" },
                        { "Depots" , "Аптеки" },
                        { "Logistics" , "Логистика" },
                        { "Shipyards" , "Судоремонтные заводы" },
                        { "SpecialBuildings" , "Специальные здания" },
                        { "MiningBuildings" , "Горнодобывающие здания" },
                        { "OrnamentalBuilding" , "Орнаменты" },
                        { "-RoadPresets" , "- Дорожные предустановки" },
                        { "Roads(x3)" , "Дороги (x3)" },
                        { "Roads(x4)" , "Дороги (x4)" },
                        { "Roads(x5)" , "Дороги (x5)" },
                        { "OtherRoads" , "Другая дорога" }
                    }
                    },
            };
        }

        public TreeLocalization(ICommons commonsToUse)
        {
            _commons = commonsToUse;
        }

        public string GetLocalization(string valueToTranslate)
        {
            return GetLocalization(valueToTranslate, null);
        }

        public string GetLocalization(string valueToTranslate, string languageCode = null)
        {
            if (string.IsNullOrWhiteSpace(languageCode))
            {
                languageCode = Localization.GetLanguageCodeFromName(_commons.SelectedLanguage);
            }

            if (!Localization.LanguageCodeMap.ContainsValue(languageCode))
            {
                languageCode = "eng";
            }

            try
            {
                if (Translations[languageCode].TryGetValue(valueToTranslate.Replace(" ", string.Empty), out string foundLocalization))
                {
                    return foundLocalization;
                }
                else
                {
                    logger.Trace($"try to set localization to english for: : \"{valueToTranslate}\"");
                    if (Translations["eng"].TryGetValue(valueToTranslate.Replace(" ", string.Empty), out string engLocalization))
                    {
                        return engLocalization;
                    }
                    else
                    {
                        logger.Trace($"found no localization (\"eng\") and ({languageCode}) for : \"{valueToTranslate}\"");
                        return valueToTranslate;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"error getting localization ({languageCode}) for: \"{valueToTranslate}\"");
                return valueToTranslate;
            }
        }
    }
}