using UnityEngine;


namespace SigmaReplacements
{
    namespace Suits
    {
        public class CustomSuit : CustomObject
        {
            // Suit Specific Restrictions
            float? helmetLowPressure = null;
            float? helmetHighPressure = null;
            float? jetpackMaxGravity = null;
            bool jetpackDeployed = true;
            bool helmetHidden = false;
            KerbalEVA eva = null;

            // Colors
            Color? body = null;
            Color? helmet = null;
            Color? visor = null;
            Color? flares = null;
            Color? light = null;
            Color? jetpack = null;
            Color? flag = null;
            Color? gasjets = null;
            Color? headset = null;
            Color? mug = null;
            Color? glasses = null;
            Color? backdrop = null;

            // Textures
            Texture bodyTex = null;
            Texture helmetTex = null;
            Texture visorTex = null;
            Texture flaresTex = null;
            Texture jetpackTex = null;
            Texture flagTex = null;
            Texture gasjetsTex = null;
            Texture headsetTex = null;
            Texture mugTex = null;
            Texture glassesTex = null;
            Texture backdropTex = null;

            // Normals
            Texture bodyNrm = null;
            Texture helmetNrm = null;
            Texture visorNrm = null;
            Texture jetpackNrm = null;
            Texture flagNrm = null;
            Texture headsetNrm = null;
            Texture mugNrm = null;
            Texture glassesNrm = null;
            Texture backdropNrm = null;


            void Start()
            {
                ProtoCrewMember kerbal = Apply();
                Debug.Log("CustomSuit.Start", "kerbal = " + kerbal + " (" + kerbal?.GetType() + ")");
                if (kerbal == null) return;

                eva = GetComponent<KerbalEVA>();

                // Ignore the new EVA suits
                if (eva?.gameObject?.GetChild("EVALight") != null) return;
                // Ignore the new IVA suits
                if (GetComponent<kerbalExpressionSystem>()?.gameObject?.GetChild("controlObjects01") != null || GetComponent<kerbalExpressionSystem>()?.gameObject?.GetChild("extraNodes01") != null) return;

                LoadFor(kerbal);
                ApplyTo(kerbal);

                if (HighLogic.LoadedScene == GameScenes.FLIGHT && eva != null)
                {
                    if (jetpackMaxGravity != null)
                        TimingManager.UpdateAdd(TimingManager.TimingStage.Normal, JetPack);
                    if (helmetLowPressure != null || helmetHighPressure != null)
                        TimingManager.UpdateAdd(TimingManager.TimingStage.Normal, Helmet);
                    if (Nyan.forever)
                        TimingManager.UpdateAdd(TimingManager.TimingStage.Normal, RainbowJets);
                }
            }

            void JetPack()
            {
                if
                (
                    ((eva.JetpackDeployed || eva.IsChuteState) && !jetpackDeployed) ||
                    (!eva.JetpackDeployed && !eva.IsChuteState && jetpackDeployed == FlightGlobals.ship_geeForce > jetpackMaxGravity)
                )
                {
                    jetpackDeployed = !jetpackDeployed;

                    Renderer[] jetpackRenderers = eva.gameObject.GetChild("jetpack01").GetComponentsInChildren<Renderer>(true);
                    Renderer[] chuteRenderers = eva.gameObject.GetChild("model").GetComponentsInChildren<Renderer>(true);

                    for (int i = 0; i < jetpackRenderers.Length; i++)
                    {
                        if (jetpackRenderers[i]?.name?.StartsWith("fx_gasJet") == false)
                        {
                            jetpackRenderers[i].enabled = jetpackDeployed;
                        }
                    }

                    for (int i = 0; i < chuteRenderers.Length; i++)
                    {
                        if (chuteRenderers[i]?.name?.StartsWith("fx_gasJet") == false)
                        {
                            chuteRenderers[i].enabled = jetpackDeployed;
                        }
                    }
                }
            }

            void Helmet()
            {
                if (FlightGlobals.currentMainBody?.atmosphereContainsOxygen != true) return;

                double pressure = FlightGlobals.getStaticPressure();

                if (helmetHidden != !(pressure < helmetLowPressure || pressure > helmetHighPressure || helmetLowPressure == helmetHighPressure))
                {
                    helmetHidden = !helmetHidden;

                    Renderer[] renderers = eva.gameObject.GetChild("helmet01").GetComponentsInChildren<Renderer>(true);

                    for (int i = 0; i < renderers.Length; i++)
                    {
                        if (renderers[i]?.name == "helmet" || renderers[i]?.name == "visor" || renderers[i]?.name == "flare1" || renderers[i]?.name == "flare2")
                            renderers[i].enabled = !helmetHidden;
                    }
                }
            }

            void OnDestroy()
            {
                if (jetpackMaxGravity != null)
                    TimingManager.UpdateRemove(TimingManager.TimingStage.Normal, JetPack);
                if (helmetLowPressure != null || helmetHighPressure != null)
                    TimingManager.UpdateRemove(TimingManager.TimingStage.Normal, Helmet);
                if (Nyan.forever)
                    TimingManager.UpdateRemove(TimingManager.TimingStage.Normal, RainbowJets);
            }

            internal override void LoadFor(ProtoCrewMember kerbal)
            {
                Debug.Log("CustomSuit.LoadFor", "kerbal = " + kerbal);

                if (kerbal == null) return;

                Info.hash = "";
                int? useChance = null;
                string collection = "";

                for (int i = 0; i < SuitInfo.DataBase?.Count; i++)
                {
                    SuitInfo info = (SuitInfo)SuitInfo.DataBase[i].GetFor(kerbal);

                    if (info != null)
                    {
                        Type type = Type.IVA;
                        if (eva != null) type = Type.EVA;
                        else if (kerbal.GetType() == typeof(CrewMember) && ((CrewMember)kerbal).activity == 0) type = Type.EVA;

                        if (info.type != null && info.type != type) continue;
                        Debug.Log("CustomSuit.LoadFor", "Matched suit type = " + info.type + " to current activity = " + type);

                        bool useSuit = true;
                        if (eva != null)
                        {
                            double pressure = FlightGlobals.getStaticPressure();
                            useSuit = !(pressure < info.suitMinPressure) && !(pressure > info.suitMaxPressure);
                            if (useSuit) Debug.Log("CustomSuit.LoadFor", "Matched suitMinPressure = " + info.suitMinPressure + ", suitMaxPressure = " + info.suitMaxPressure + " to current atmospheric pressure = " + pressure);
                        }


                        if (string.IsNullOrEmpty(collection) || collection == info.collection)
                        {
                            if (info.useChance != 1)
                                useChance = kerbal.Hash(info.useGameSeed) % 100;

                            if (info.useChance == 1 || useChance < info.useChance * 100)
                            {
                                Debug.Log("CustomSuit.LoadFor", "Matched suit useChance = " + info.useChance + " to generated chance = " + useChance + " %");
                                Debug.Log("CustomSuit.LoadFor", "Matched suit collection = " + info.collection + " to current collection = " + collection);
                                // Collection
                                collection = info.collection;

                                // Suit Specific Requirements
                                if (useSuit)
                                {
                                    helmetLowPressure = helmetLowPressure ?? info.helmetLowPressure;
                                    helmetHighPressure = helmetHighPressure ?? info.helmetHighPressure;
                                }
                                jetpackMaxGravity = jetpackMaxGravity ?? info.jetpackMaxGravity;

                                // Colors
                                if (useSuit)
                                {
                                    body = body ?? info.body.Pick(kerbal, info.useGameSeed);
                                    helmet = helmet ?? info.helmet.Pick(kerbal, info.useGameSeed);
                                    visor = visor ?? info.visor.Pick(kerbal, info.useGameSeed);
                                    flares = flares ?? info.flares.Pick(kerbal, info.useGameSeed);
                                    light = light ?? info.light.Pick(kerbal, info.useGameSeed);
                                }
                                jetpack = jetpack ?? info.jetpack.Pick(kerbal, info.useGameSeed);
                                flag = flag ?? info.flag.Pick(kerbal, info.useGameSeed);
                                gasjets = gasjets ?? info.gasjets.Pick(kerbal, info.useGameSeed);
                                headset = headset ?? info.headset.Pick(kerbal, info.useGameSeed);
                                mug = mug ?? info.mug.Pick(kerbal, info.useGameSeed);
                                glasses = glasses ?? info.glasses.Pick(kerbal, info.useGameSeed);
                                backdrop = backdrop ?? info.backdrop.Pick(kerbal, info.useGameSeed);

                                // Textures
                                if (useSuit)
                                {
                                    bodyTex = bodyTex ?? info.bodyTex.Pick(kerbal, info.useGameSeed);
                                    helmetTex = helmetTex ?? info.helmetTex.Pick(kerbal, info.useGameSeed);
                                    visorTex = visorTex ?? info.visorTex.Pick(kerbal, info.useGameSeed);
                                    flaresTex = flaresTex ?? info.flaresTex.Pick(kerbal, info.useGameSeed);
                                }
                                jetpackTex = jetpackTex ?? info.jetpackTex.Pick(kerbal, info.useGameSeed);
                                flagTex = flagTex ?? info.flagTex.Pick(kerbal, info.useGameSeed);
                                gasjetsTex = gasjetsTex ?? info.gasjetsTex.Pick(kerbal, info.useGameSeed);
                                headsetTex = headsetTex ?? info.headsetTex.Pick(kerbal, info.useGameSeed);
                                mugTex = mugTex ?? info.mugTex.Pick(kerbal, info.useGameSeed);
                                glassesTex = glassesTex ?? info.glassesTex.Pick(kerbal, info.useGameSeed);
                                backdropTex = backdropTex ?? info.backdropTex.Pick(kerbal, info.useGameSeed);

                                // Normals
                                if (useSuit)
                                {
                                    bodyNrm = bodyNrm ?? info.bodyNrm.At(bodyTex, info.bodyTex, kerbal, info.useGameSeed);
                                    helmetNrm = helmetNrm ?? info.helmetNrm.At(helmetTex, info.helmetTex, kerbal, info.useGameSeed);
                                    visorNrm = visorNrm ?? info.visorNrm.At(visorTex, info.visorTex, kerbal, info.useGameSeed);
                                }
                                jetpackNrm = jetpackNrm ?? info.jetpackNrm.At(jetpackTex, info.jetpackTex, kerbal, info.useGameSeed);
                                flagNrm = flagNrm ?? info.flagNrm.At(flagTex, info.flagTex, kerbal, info.useGameSeed);
                                headsetNrm = headsetNrm ?? info.headsetNrm.At(headsetTex, info.headsetTex, kerbal, info.useGameSeed);
                                mugNrm = mugNrm ?? info.mugNrm.At(mugTex, info.mugTex, kerbal, info.useGameSeed);
                                glassesNrm = glassesNrm ?? info.glassesNrm.At(glassesTex, info.glassesTex, kerbal, info.useGameSeed);
                                backdropNrm = backdropNrm ?? info.backdropNrm.At(backdropTex, info.backdropTex, kerbal, info.useGameSeed);
                            }
                        }
                    }
                }
            }

            internal override void ApplyTo(ProtoCrewMember kerbal)
            {
                Debug.Log("CustomHead.ApplyTo", "kerbal = " + kerbal);

                if (kerbal == null) return;

                if (Nyan.nyan)
                {
                    if (HighLogic.LoadedScene == GameScenes.MAINMENU || Nyan.forever)
                    {
                        NyanSuit.ApplyTo(kerbal, this);
                        return;
                    }
                }

                Renderer[] renderers = GetComponentsInChildren<Renderer>(true);

                for (int i = 0; i < renderers?.Length; i++)
                {
                    string name = renderers[i]?.name;
                    Material material = renderers[i]?.material;
                    if (material == null) continue;

                    if (name == "body01" || name == "mesh_female_kerbalAstronaut01_body01" || name == "coat01" || name == "pants01" || name == "mesh_bowTie01")
                    {
                        material.SetColor(body);
                        material.SetTexture(bodyTex);
                        material.SetNormal(bodyNrm);
                    }

                    else

                    if (name == "helmet" || name == "mesh_female_kerbalAstronaut01_helmet" || name == "mesh_backpack" || name == "mesh_hazm_helmet" || name == "mesh_helmet_support" || name == "helmetConstr01")
                    {
                        material.SetColor(helmet);
                        material.SetTexture(helmetTex);
                        material.SetNormal(helmetNrm);
                    }

                    else

                    if (name == "visor" || name == "mesh_female_kerbalAstronaut01_visor" || name == "mesh_hazm_visor")
                    {
                        material.SetColor(visor);
                        material.SetTexture(visorTex);
                        material.SetNormal(visorNrm);
                    }

                    else

                    if (name == "flare1" || name == "flare2")
                    {
                        if (flares != null)
                        {
                            material.shader = Shader.Find("Particles/Alpha Blended");
                            material.SetTintColor(flares);
                        }

                        material.SetTexture(flaresTex);

                        if (light != null)
                        {
                            Light lights = renderers[i].GetComponentInParent<Light>();
                            if (lights != null) lights.color = (Color)light;
                        }
                    }

                    else

                    if (material.mainTexture?.name == "EVAjetpack")
                    {
                        material.SetColor(jetpack);
                        material.SetTexture(jetpackTex);
                        material.SetNormal(jetpackNrm);
                    }

                    else

                    if (name == "kbEVA_flagDecals")
                    {
                        material.SetColor(flag);
                        material.SetTexture(flagTex);
                        material.SetNormal(flagNrm);
                    }

                    else

                    if (material.mainTexture?.name == "fairydust")
                    {
                        material.SetTintColor(gasjets);
                        material.SetTexture(gasjetsTex);
                    }

                    else

                    if (material.mainTexture?.name == "kbGeneKerman_headset")
                    {
                        material.SetColor(headset);
                        material.SetTexture(headsetTex);
                        material.SetNormal(headsetNrm);
                    }

                    else

                    if (name == "gene_mug_base" || name == "gene_mug_handle")
                    {
                        material.SetColor(mug);
                        material.SetTexture(mugTex);
                        material.SetNormal(mugNrm);
                    }

                    else

                    if (material.mainTexture?.name == "wernerVonKerman_glasses")
                    {
                        material.SetColor(glasses);
                        material.SetTexture(glassesTex);
                        material.SetNormal(glassesNrm);
                    }

                    else

                    if (name == "backdrop")
                    {
                        material.SetColor(backdrop);
                        material.SetTexture(backdropTex);
                        material.SetNormal(backdropNrm);
                    }
                }
            }


            // Nyan
            int index = 0;
            float wait = 0;
            Color[] rainbow = new[] { new Color(1, 0, 0, 0.5f), new Color(1, 0.6f, 0, 0.5f), new Color(1, 1, 0, 0.5f), new Color(0.2f, 1, 0, 0.5f), new Color(0, 0.6f, 1, 0.5f), new Color(0.4f, 0.2f, 1, 0.5f) };

            void RainbowJets()
            {
                if (wait > 0.1)
                {
                    Renderer[] renderers = eva.gameObject.GetChild("jetpack01").GetComponentsInChildren<Renderer>(true);

                    for (int i = 0; i < renderers.Length; i++)
                    {
                        if (renderers[i]?.name?.StartsWith("fx_gasJet") == true)
                        {
                            renderers[i].material.SetTintColor(rainbow[index]);
                        }
                    }

                    index = (index + 1) % 6;
                    wait = 0;
                }
                else
                {
                    wait += Time.deltaTime;
                }
            }
        }

        internal enum Type
        {
            EVA,
            IVA
        }
    }
}
