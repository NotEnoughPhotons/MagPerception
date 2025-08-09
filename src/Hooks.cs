using System;
using System.Collections.Generic;

using Il2CppSLZ.Marrow;

namespace NEP.MagPerception
{
    public static class Hooks
    {
        [HarmonyLib.HarmonyPatch(typeof(Grip), nameof(Grip.OnAttachedToHand))]
        public static class OnGripAttached
        {
            public static Dictionary<Hand, Magazine> HoldMagazines { get; } = [];

            public static void Postfix(Hand hand, Grip __instance)
            {
                if (hand == null || __instance == null)
                    return;

                if (!hand.IsPartOfLocalPlayer())
                    return;

                var gun = __instance.GetComponentInParent<Gun>();

                if (gun != null)
                {
                    if (MagPerceptionManager.LastGunGrips.ContainsKey(gun) != true)
                    {
                        MagPerceptionManager.LastGunGrips.Add(gun, [__instance]);
                    }
                    else
                    {
                        var grips = MagPerceptionManager.LastGunGrips[gun];
                        grips.Add(__instance);
                        MagPerceptionManager.LastGunGrips[gun] = grips;
                    }
                    MagPerceptionManager.OnGunAttached(gun);
                }
                else
                {
                    var mag = __instance.GetComponentInParent<Magazine>();
                    if (mag != null)
                    {
                        if (__instance != mag.grip)
                            return;

                        MagPerceptionManager.OnMagazineAttached(mag);
                        if (!HoldMagazines.ContainsKey(hand))
                            HoldMagazines.Add(hand, mag);
                    }
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Grip), nameof(Grip.OnDetachedFromHand))]
        public static class OnGripDetached
        {
            public static void Postfix(Hand hand, Grip __instance)
            {
                if (hand == null || __instance == null)
                    return;

                if (!hand.IsPartOfLocalPlayer())
                    return;

                var gun = __instance.GetComponentInParent<Gun>();

                if (gun != null)
                {
                    if (MagPerceptionManager.LastGunGrips.TryGetValue(gun, out List<Grip> grips) && grips?.Count == 1)
                    {
                        MagPerceptionManager.LastGunGrips.Remove(gun);
                    }
                    else
                    {
                        if (MagPerceptionManager.LastGunGrips.ContainsKey(gun))
                        {
                            var _grips = MagPerceptionManager.LastGunGrips[gun];
                            _grips.Remove(__instance);
                            MagPerceptionManager.LastGunGrips[gun] = _grips;
                        }
                    }
                    MagPerceptionManager.OnGunDetached(gun);
                }
                else
                {
                    var mag = __instance.GetComponentInParent<Magazine>();
                    if (mag != null)
                    {
                        if (__instance != mag.grip)
                            return;

                        if (OnGripAttached.HoldMagazines.ContainsKey(hand))
                            OnGripAttached.HoldMagazines.Remove(hand);

                        MagPerceptionManager.OnMagazineDetached(mag);
                    }
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Gun), nameof(Gun.EjectCartridge))]
        public static class OnGunEjectRound
        {
            public static void Postfix(Gun __instance) =>
                MagPerceptionManager.OnGunEjectRound(__instance);
        }

        [HarmonyLib.HarmonyPatch(typeof(Gun), nameof(Gun.OnMagazineInserted))]
        public static class OnMagazineInserted
        {
            public static void Postfix(Gun __instance) =>
                MagPerceptionManager.OnMagazineInserted(__instance);
        }

        [HarmonyLib.HarmonyPatch(typeof(Gun), nameof(Gun.OnMagazineRemoved))]
        public static class OnMagazineRemoved
        {
            public static void Postfix(Gun __instance) =>
                MagPerceptionManager.OnMagazineEjected(__instance);
        }

        [HarmonyLib.HarmonyPatch(typeof(InventorySlotReceiver), nameof(InventorySlotReceiver.InsertInSlot))]
        public static class OnHolsteredGun
        {
            public static void Postfix(InventorySlotReceiver __instance, InteractableHost host)
            {
                if (__instance == null)
                    return;

                if (!__instance.IsPartOfLocalPlayer())
                    return;

                var gun = host?.GetComponent<Gun>();
                if (gun == null)
                    return;

                MagPerceptionManager.OnGunHolstered(gun);
            }
        }
    }
}