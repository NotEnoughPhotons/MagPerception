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
                    if (MagPerceptionManager.Instance?.LastGunGrips.ContainsKey(gun) != true)
                    {
                        MagPerceptionManager.Instance?.LastGunGrips.Add(gun, [__instance]);
                    }
                    else
                    {
                        var grips = MagPerceptionManager.Instance.LastGunGrips[gun];
                        grips.Add(__instance);
                        MagPerceptionManager.Instance.LastGunGrips[gun] = grips;
                    }
                    MagPerceptionManager.Instance?.OnGunAttached(gun);
                }
                else
                {
                    var mag = __instance.GetComponentInParent<Magazine>();
                    if (mag != null)
                    {
                        if (__instance != mag.grip)
                            return;

                        MagPerceptionManager.Instance?.OnMagazineAttached(mag);
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
                    if (MagPerceptionManager.Instance == null)
                        return;

                    if (MagPerceptionManager.Instance.LastGunGrips.TryGetValue(gun, out List<Grip> grips) && grips?.Count == 1)
                    {
                        MagPerceptionManager.Instance.LastGunGrips.Remove(gun);
                    }
                    else
                    {
                        if (MagPerceptionManager.Instance.LastGunGrips.ContainsKey(gun))
                        {
                            var _grips = MagPerceptionManager.Instance.LastGunGrips[gun];
                            _grips.Remove(__instance);
                            MagPerceptionManager.Instance.LastGunGrips[gun] = _grips;
                        }
                    }
                    MagPerceptionManager.Instance?.OnGunDetached(gun);
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

                        MagPerceptionManager.Instance?.OnMagazineDetached(mag);
                    }
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Gun), nameof(Gun.EjectCartridge))]
        public static class OnGunEjectRound
        {
            public static void Postfix(Gun __instance) =>
                MagPerceptionManager.Instance?.OnGunEjectRound(__instance);
        }

        [HarmonyLib.HarmonyPatch(typeof(Gun), nameof(Gun.OnMagazineInserted))]
        public static class OnMagazineInserted
        {
            public static void Postfix(Gun __instance) =>
                MagPerceptionManager.Instance?.OnMagazineInserted(__instance);
        }

        [HarmonyLib.HarmonyPatch(typeof(Gun), nameof(Gun.OnMagazineRemoved))]
        public static class OnMagazineRemoved
        {
            public static void Postfix(Gun __instance) =>
                MagPerceptionManager.Instance?.OnMagazineEjected(__instance);
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

                MagPerceptionManager.Instance?.OnGunHolstered(gun);
            }
        }
    }
}