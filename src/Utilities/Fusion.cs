using Il2CppSLZ.Marrow;

using UnityEngine;

namespace NEP.MagPerception
{
    /// <summary>
    /// Class that holds all of the methods to bring compatibility with Fusion. Public methods have checks to prevent using methods from the assembly when it isn't present
    /// </summary>
    public static class Fusion
    {
        public static bool HasFusion => Main.FindMelon("LabFusion", "Lakatrazz") != null;

        private static bool Internal_IsConnected()
        {
            return LabFusion.Network.NetworkInfo.HasServer;
        }

        public static bool IsConnected
        {
            get
            {
                if (HasFusion) return Internal_IsConnected();
                else return false;
            }
        }

        private static bool Internal_IsPartOfLocalPlayer(this Component comp)
        {
            return LabFusion.Extensions.ComponentExtensions.IsPartOfSelf(comp) && LabFusion.Extensions.ComponentExtensions.IsPartOfPlayer(comp);
        }

        public static bool IsPartOfLocalPlayer(this Component comp)
        {
            if (IsConnected) return Internal_IsPartOfLocalPlayer(comp);
            else return true;
        }
    }
}