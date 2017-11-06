using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Gender = ProtoCrewMember.Gender;
using Type = ProtoCrewMember.KerbalType;


namespace SigmaReplacements
{
    internal class Info
    {
        // Identifiers
        internal string name = null;

        // Requirements
        internal bool useGameSeed = false;
        internal float useChance = 1;
        internal Type? rosterStatus = null;
        internal Gender? gender = null;
        internal string[] trait = null;
        internal bool? veteran = null;
        internal bool? isBadass = null;
        int minLevel = 0;
        int maxLevel = 5;
        float minCourage = 0;
        float maxCourage = 1;
        float minStupidity = 0;
        float maxStupidity = 1;
        // For MainMenuKerbals
        internal float? courage = null;
        internal float? stupidity = null;
        internal int? experienceLevel = null;

        // Collection
        internal string collection = "";

        // Get
        internal Info GetFor(ProtoCrewMember kerbal)
        {
            if (name == null || name == kerbal.name)
            {
                Debug.Log("Info.GetFor", "Matched name = " + name + " to kerbal name = " + kerbal.name);
                if (rosterStatus == null || rosterStatus == kerbal.type)
                {
                    Debug.Log("Info.GetFor", "Matched rosterStatus = " + rosterStatus + " to kerbal rosterStatus = " + kerbal.type);
                    if (gender == null || gender == kerbal.gender)
                    {
                        Debug.Log("Info.GetFor", "Matched gender = " + gender + " to kerbal gender = " + kerbal.gender);
                        if (trait == null || trait.Contains(kerbal.trait))
                        {
                            Debug.Log("Info.GetFor", "Matched trait = " + trait + " to kerbal trait = " + kerbal.trait);
                            if (veteran == null || veteran == kerbal.veteran)
                            {
                                Debug.Log("Info.GetFor", "Matched veteran = " + veteran + " to kerbal veteran = " + kerbal.veteran);
                                if (isBadass == null || isBadass == kerbal.isBadass)
                                {
                                    Debug.Log("Info.GetFor", "Matched isBadass = " + isBadass + " to kerbal isBadass = " + kerbal.isBadass);
                                    if (minLevel <= kerbal.experienceLevel && maxLevel >= kerbal.experienceLevel)
                                    {
                                        Debug.Log("Info.GetFor", "Matched minLevel = " + minLevel + ", maxLevel = " + maxLevel + " to kerbal level = " + kerbal.experienceLevel);
                                        if (minCourage <= kerbal.courage && maxCourage >= kerbal.courage)
                                        {
                                            Debug.Log("Info.GetFor", "Matched minCourage = " + minCourage + ", maxCourage = " + maxCourage + " to kerbal courage = " + kerbal.courage);
                                            if (minStupidity <= kerbal.stupidity && maxStupidity >= kerbal.stupidity)
                                            {
                                                Debug.Log("Info.GetFor", "Matched minStupidity = " + minStupidity + ", maxStupidity = " + maxStupidity + " to kerbal stupidity = " + kerbal.stupidity);
                                                Debug.Log("Info.GetFor", "Return this Info");
                                                return this;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            Debug.Log("Info.GetFor", "Return null");
            return null;
        }


        // New Info
        internal Info(ConfigNode requirements, ConfigNode info)
        {
            Parse(requirements, info);
        }

        // Parse from ConfigNode
        internal void Parse(ConfigNode requirements, ConfigNode info)
        {
            Debug.Log("Info", "new Info from:");
            Debug.Log("Info", "Requirements node = " + requirements);
            Debug.Log("Info", "Info node = " + info);

            // Parse Info Requirements
            useGameSeed = Parse(requirements.GetValue("useGameSeed"), useGameSeed);
            useChance = Parse(requirements.GetValue("useChance"), useChance);
            name = requirements.GetValue("name");
            rosterStatus = Parse(requirements.GetValue("rosterStatus"), rosterStatus);
            gender = Parse(requirements.GetValue("gender"), gender);
            trait = requirements.HasValue("trait") ? requirements.GetValues("trait") : null;
            veteran = Parse(requirements.GetValue("veteran"), veteran);
            isBadass = Parse(requirements.GetValue("isBadass"), isBadass);
            minLevel = Parse(requirements.GetValue("minLevel"), minLevel);
            maxLevel = Parse(requirements.GetValue("maxLevel"), maxLevel);
            minCourage = Parse(requirements.GetValue("minCourage"), minCourage);
            maxCourage = Parse(requirements.GetValue("maxCourage"), maxCourage);
            minStupidity = Parse(requirements.GetValue("minStupidity"), minStupidity);
            maxStupidity = Parse(requirements.GetValue("maxStupidity"), maxStupidity);
            // For MainMenuKerbals
            experienceLevel = Parse(requirements.GetValue("level"), experienceLevel);
            courage = Parse(requirements.GetValue("courage"), courage);
            stupidity = Parse(requirements.GetValue("stupidity"), stupidity);


            // Parse Collection
            collection = info.GetValue("collection");
        }


        // Parsers
        internal float Parse(string s, float defaultValue) { return float.TryParse(s, out float f) ? f : defaultValue; }
        internal float? Parse(string s, float? defaultValue) { return float.TryParse(s, out float f) ? f : defaultValue; }
        internal bool Parse(string s, bool defaultValue) { return bool.TryParse(s, out bool b) ? b : defaultValue; }
        internal bool? Parse(string s, bool? defaultValue) { return bool.TryParse(s, out bool b) ? b : defaultValue; }
        internal int Parse(string s, int defaultValue) { return int.TryParse(s, out int b) ? b : defaultValue; }
        internal int? Parse(string s, int? defaultValue) { return int.TryParse(s, out int b) ? b : defaultValue; }

        internal Type? Parse(string s, Type? defaultValue)
        {
            try { return (Type)Enum.Parse(typeof(Type), s); }
            catch { return defaultValue; }
        }

        internal Gender? Parse(string s, Gender? defaultValue)
        {
            try { return (Gender)Enum.Parse(typeof(Gender), s); }
            catch { return defaultValue; }
        }

        internal Texture Parse(string s, Texture defaultValue)
        {
            return Resources.FindObjectsOfTypeAll<Texture>().FirstOrDefault(t => t.name == s) ?? defaultValue;
        }

        internal List<Texture> Parse(string[] s, List<Texture> defaultValue)
        {
            for (int i = 0; i < s.Length; i++)
            {
                Texture tex = null;
                tex = Parse(s[i], tex);
                if (tex != null && !defaultValue.Contains(tex))
                    defaultValue.Add(tex);
            }
            return defaultValue;
        }

        internal Color? Parse(string s, Color? defaultValue)
        {
            try { return ConfigNode.ParseColor(s); }
            catch { return defaultValue; }
        }

        internal List<Color> Parse(string[] s, List<Color> defaultValue)
        {
            for (int i = 0; i < s.Length; i++)
            {
                Color? col = null;
                col = Parse(s[i], col);
                if (!defaultValue.Contains((Color)col))
                    defaultValue.Add((Color)col);
            }

            return defaultValue;
        }
    }
}
