﻿using System.Resources;
using UnityEngine;


namespace SigmaReplacements
{
    namespace MenuButtons
    {
        internal class Nyan
        {
            internal static bool nyan = false;
            internal static bool forever = false;
            static Texture MenuButton;

            private static ResourceManager resourceMan;

            private Nyan()
            {
            }

            private static ResourceManager ResourceManager
            {
                get
                {
                    if (ReferenceEquals(resourceMan, null))
                    {
                        ResourceManager temp = new ResourceManager("SigmaReplacements.MenuButtons.Nyan", typeof(Nyan).Assembly);
                        resourceMan = temp;
                    }
                    return resourceMan;
                }
            }

            internal static Texture nyanMenuButton
            {
                get
                {
                    if (MenuButton == null)
                    {
                        byte[] bytes = (byte[])ResourceManager.GetObject("nyanMenuButton");
                        MenuButton = bytes.ToDDS();
                    }

                    return MenuButton;
                }
            }
        }
    }
}
