using Il2CppSLZ.Marrow;
using LabFusion.Entities;
using LabFusion.Player;
using UnityEngine;

namespace NEP.MagPerception
{
    /// <summary>
    /// Class that holds all of the methods to bring compatibility with Fusion. Public methods have checks to prevent using methods from the assembly when it isn't present
    /// </summary>
    public static class Fusion
    {
        public static bool HasFusion => Main.FindMelon("LabFusion", "Lakatrazz") != null;
        
        public static bool IsConnected
        {
            get
            {
                if (HasFusion) return Internal_IsConnected();
                else return false;
            }
        }
        
        private static bool Internal_IsConnected()
        {
            return LabFusion.Network.NetworkInfo.HasServer;
        }

        private static bool Internal_IsPartOfLocalPlayer(this Component comp)
        {
            NetworkPlayer localPlayer = LocalPlayer.GetNetworkPlayer();

            if (localPlayer == null)
                throw new System.NullReferenceException("Player is not connected to a Fusion lobby!");
            
            RigManager localRig = localPlayer.RigRefs.RigManager;
            Transform root = comp.transform.root;

            if (!RigManager.Cache.TryGet(root.gameObject, out RigManager rootRig))
                throw new System.NullReferenceException("Fusion rig reference could not be found!");
            
            if (localRig.Pointer == rootRig.Pointer) return true;
            
            return false;
        }

        public static bool IsPartOfLocalPlayer(this Component comp)
        {
            if (IsConnected) return Internal_IsPartOfLocalPlayer(comp);
            else return true;
        }
    }
}