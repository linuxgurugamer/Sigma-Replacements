using System;
using System.Collections.Generic;
using UnityEngine;


namespace SigmaReplacements
{
    namespace SkyBox
    {
        public class CustomSkyBox
        {
            // Settings
            bool? rotate = null;
            bool? mirror = null;

            // Textures
            Texture[] SkyBox = null;

            internal CustomSkyBox(Mode gameMode, int seed)
            {
                Debug.Log("CustomSkyBox.LoadFor", "Mode = " + gameMode);

                int? useChance = null;
                string collection = "";
                List<Texture[]> SkyBoxList = new List<Texture[]>();

                for (int i = 0; i < SkyBoxInfo.DataBase?.Count; i++)
                {
                    SkyBoxInfo info = ((SkyBoxInfo)SkyBoxInfo.DataBase[i]).GetFor(gameMode);

                    if (info != null)
                    {
                        if (string.IsNullOrEmpty(collection) || collection == info.collection)
                        {
                            if (info.useChance != 1)
                            {
                                useChance = Math.Abs(seed % 100);
                                seed = seed.ToString().GetHashCode();
                            }

                            if (info.useChance == 1 || useChance < info.useChance * 100)
                            {
                                Debug.Log("CustomSkyBox.LoadFor", "Matched suit useChance = " + info.useChance + " to generated chance = " + useChance + " %");
                                Debug.Log("CustomSkyBox.LoadFor", "Matched suit collection = " + info.collection + " to current collection = " + collection);
                                // Collection
                                collection = info.collection;

                                // Settings
                                rotate = rotate ?? info.rotate;
                                mirror = mirror ?? info.mirror == true ? (Math.Abs(seed % 2) == 1) : info.mirror;

                                // Textures
                                if (info.SkyBox?.Count > 0)
                                    SkyBoxList.AddRange(info.SkyBox);
                            }
                        }
                    }
                }

                SkyBox = SkyBoxList.Pick();
            }

            internal void ApplyTo(GameObject skybox)
            {
                Debug.Log("CustomSkyBox.ApplyTo", "Applying to skybox = " + skybox);
                if (skybox == null) return;

                // Set Textures
                Renderer[] renderers = skybox?.GetComponentsInChildren<Renderer>();

                for (int i = 0; i < renderers?.Length; i++)
                {
                    string name = renderers[i]?.name;
                    Material material = renderers[i]?.material;

                    if (material == null) continue;

                    // Select Texture
                    if (SkyBox != null)
                    {
                        if (name == "XP")
                            material.SetTexture(mirror == true ? SkyBox[1] : SkyBox[0]);
                        else if (name == "XN")
                            material.SetTexture(mirror == true ? SkyBox[0] : SkyBox[1]);
                        else if (name == "YP") // SQUAD inverts YP with YN
                            material.SetTexture(SkyBox[3]);
                        else if (name == "YN") // SQUAD inverts YN with YP
                            material.SetTexture(SkyBox[2]);
                        else if (name == "ZP")
                            material.SetTexture(SkyBox[4]);
                        else if (name == "ZN")
                            material.SetTexture(SkyBox[5]);
                    }
                    else if (mirror == true)
                    {
                        if (DefaultSkyBox.XP != null && DefaultSkyBox.XN != null)
                        {
                            if (name == "XP")
                                material.SetTexture(DefaultSkyBox.XN);
                            if (name == "XN")
                                material.SetTexture(DefaultSkyBox.XP);
                        }
                    }

                    // Flip Texture
                    if (mirror == true)
                    {
                        material.SetTextureScale("_MainTex", new Vector2(-1, 1));
                    }
                }


                // Rotate CubeMap
                if (rotate == true)
                {
                    string hash = "" + HighLogic.CurrentGame?.Seed;

                    int x = hash.GetHashCode();
                    hash = x.ToString();
                    int y = hash.GetHashCode();
                    hash = y.ToString();
                    int z = hash.GetHashCode();
                    hash = z.ToString();

                    GalaxyCubeControl cube = skybox.GetComponent<GalaxyCubeControl>();
                    if (cube != null)
                        cube.initRot = Quaternion.Euler(x % 360, y % 360, z % 360);
                    else
                        skybox.transform.rotation = Quaternion.Euler(x % 360, y % 360, z % 360);
                }
            }
        }
    }
}
