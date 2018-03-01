using System.Collections.Generic;
using UnityEngine;
using Type = ProtoCrewMember.KerbalType;
using Roster = ProtoCrewMember.RosterStatus;
using Gender = ProtoCrewMember.Gender;


namespace SigmaReplacements
{
    public class UIKerbalsMenu : UIKerbalMenu
    {
        // Stock Main Menu Kerbals
        internal static CrewMember[] munKerbals = new CrewMember[] { new CrewMember(Type.Crew, Roster.Assigned, "Bob Kerman", Gender.Male, "Scientist", true, false, 0.3f, 0.1f, 0) };
        internal static CrewMember[] orbitKerbals = new CrewMember[]
        {
            new CrewMember(Type.Crew, Roster.Assigned, "Bill Kerman", Gender.Male, "Engineer", true, false, 0.5f, 0.8f, 0),
            new CrewMember(Type.Crew, Roster.Assigned, "Bob Kerman", Gender.Male, "Scientist", true, false, 0.3f, 0.1f, 0),
            new CrewMember(Type.Crew, Roster.Assigned, "Jebediah Kerman", Gender.Male, "Pilot", true, true, 0.5f, 0.5f, 0),
            new CrewMember(Type.Crew, Roster.Assigned, "Valentina Kerman", Gender.Female, "Pilot", true, true, 0.55f, 0.4f, 0)
        };

        // Dictionaries
        internal static Dictionary<int, CrewMember> munSceneKerbals = new Dictionary<int, CrewMember>();
        internal static Dictionary<int, CrewMember> orbitSceneKerbals = new Dictionary<int, CrewMember>();
    }

    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    internal class UIKerbalMenuLoader : MonoBehaviour
    {
        static bool loaded = false;

        void Awake()
        {
            if (!loaded)
            {
                loaded = true;
                Debug.Log("UIKerbalLoader", "Awake");

                // Mun Scene
                ConfigNode[] MunSceneKerbal = UserSettings.ConfigNode.GetNodes("MunSceneKerbal");

                for (int i = 0; i < MunSceneKerbal?.Length; i++)
                {
                    if (int.TryParse(MunSceneKerbal[i]?.GetValue("index"), out int index))
                    {
                        if (!UIKerbalsMenu.munSceneKerbals.ContainsKey(index))
                        {
                            if (index == 0)
                            {
                                UIKerbalsMenu.munSceneKerbals.Add(0, UIKerbalsMenu.munKerbals[0]);
                            }
                            else
                            {
                                UIKerbalsMenu.munSceneKerbals.Add(index, new CrewMember(Type.Crew, Roster.Available, "MunKerbal" + index, ProtoCrewMember.Gender.Male, "", false, false, 0.5f, 0.5f, 0));
                            }
                        }

                        UIKerbalsMenu.munSceneKerbals[index].Load(MunSceneKerbal[i]);
                    }
                }

                // Orbit Scene
                ConfigNode[] OrbitSceneKerbal = UserSettings.ConfigNode.GetNodes("OrbitSceneKerbal");

                for (int i = 0; i < OrbitSceneKerbal?.Length; i++)
                {
                    if (int.TryParse(OrbitSceneKerbal[i]?.GetValue("index"), out int index))
                    {
                        if (!UIKerbalsMenu.orbitSceneKerbals.ContainsKey(index))
                        {
                            if (index < 4)
                            {
                                UIKerbalsMenu.orbitSceneKerbals.Add(0, UIKerbalsMenu.orbitKerbals[index]);
                            }
                            else
                            {
                                UIKerbalsMenu.orbitSceneKerbals.Add(index, new CrewMember(Type.Crew, Roster.Available, "OrbitKerbal" + index, ProtoCrewMember.Gender.Male, "", false, false, 0.5f, 0.5f, 0));
                            }
                        }

                        UIKerbalsMenu.orbitSceneKerbals[index].Load(OrbitSceneKerbal[i]);
                    }
                }

                // Apply the Components
                GameObject munScene = GameObject.Find("MunScene")?.GetChild("Kerbals");

                for (int i = 0; i < munScene?.transform?.childCount; i++)
                {
                    UIKerbalsMenu component = munScene.transform.GetChild(i).gameObject.AddOrGetComponent<UIKerbalsMenu>();

                    if (UIKerbalsMenu.munSceneKerbals.ContainsKey(i))
                        component.crewMember = UIKerbalsMenu.munSceneKerbals[i];
                }


                GameObject orbitScene = GameObject.Find("OrbitScene")?.GetChild("Kerbals");

                for (int i = 0; i < orbitScene?.transform?.childCount; i++)
                {
                    UIKerbalsMenu component = orbitScene.transform.GetChild(i).gameObject.AddOrGetComponent<UIKerbalsMenu>();

                    if (UIKerbalsMenu.orbitSceneKerbals.ContainsKey(i))
                        component.crewMember = UIKerbalsMenu.orbitSceneKerbals[i];
                }
            }
        }
    }
}
