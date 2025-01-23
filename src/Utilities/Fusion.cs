using Il2CppSLZ.Marrow;

using UnityEngine;

namespace NEP.MagPerception
{
    public static class Fusion
    {
        private static bool Internal_IsConnected()
        {
            return LabFusion.Network.NetworkInfo.HasServer;
        }

        public static bool IsConnected
        {
            get
            {
                if (Main.HasFusion) return Internal_IsConnected();
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

        private static bool Internal_IsGunMine(this Gun gun)
        {
            if (LabFusion.Entities.GunExtender.Cache.TryGet(gun, out LabFusion.Entities.NetworkEntity gunEntity))
                return gunEntity.IsOwner;
            else
                return false;
        }

        public static bool IsGunMine(this Gun gun)
        {
            if (IsConnected) return Internal_IsGunMine(gun);
            else return true;
        }
    }
}