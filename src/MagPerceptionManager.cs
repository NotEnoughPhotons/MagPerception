using UnityEngine;

using NEP.MagPerception.UI;

using Il2CppSLZ.Marrow;
using Il2CppTMPro;
using Il2CppSLZ.Bonelab;
using System.Collections.Generic;
using MelonLoader;

namespace NEP.MagPerception
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class MagPerceptionManager(System.IntPtr ptr) : MonoBehaviour(ptr)
    {
        public static MagPerceptionManager Instance { get; private set; }

        internal Dictionary<object, MagazineUI> MagazineUIs { get; } = [];

        public List<Gun> LastGuns { get; } = [];

        public List<Magazine> LastMags { get; } = [];

        internal readonly Dictionary<Gun, List<Grip>> LastGunGrips = [];

        private void Awake()
        {
            Instance = this;
        }

        private MagazineUI AddMagazineUI(object gunOrMag, Vector3 startPosition, bool addOffset = true)
        {
            if (gunOrMag.GetType() != typeof(Magazine) && gunOrMag.GetType() != typeof(Gun))
                return null;

            if (MagazineUIs.ContainsKey(gunOrMag))
                return GetMagazineUI(gunOrMag);

            GameObject magUI = GameObject.Instantiate(Main.Resources.LoadAsset("MagazineLayer").Cast<GameObject>(), transform);

            magUI.transform.SetParent(transform);
            magUI.transform.localPosition = !addOffset ? startPosition : (startPosition + Settings.Instance?.Offset ?? Vector3.zero);
            MagazineUI MagazineUI = magUI.AddComponent<MagazineUI>();

            MagazineUI.AmmoCounterText = magUI.transform.Find("AmmoCounter").GetComponent<TextMeshProUGUI>();
            MagazineUI.AmmoInventoryText = magUI.transform.Find("AmmoInventory").GetComponent<TextMeshProUGUI>();
            MagazineUI.AmmoTypeText = magUI.transform.Find("AmmoType").GetComponent<TextMeshProUGUI>();
            MagazineUI.Animator = magUI.GetComponent<Animator>();

            magUI.SetActive(false);

            MagazineUIs.Add(gunOrMag, MagazineUI);

            return MagazineUI;
        }

        private MagazineUI GetMagazineUI(object gunOrMag)
        {
            if (gunOrMag is not Magazine && gunOrMag is not Gun)
                return null;

            if (!MagazineUIs.ContainsKey(gunOrMag))
                return null;

            return MagazineUIs[gunOrMag];
        }

        /// <summary>
        /// Called when a player grabs a magazine.
        /// </summary>
        internal void OnMagazineAttached(Magazine magazine)
        {
            if (magazine == null)
                return;

            LastMags.Add(magazine);

            var magazineUI = AddMagazineUI(magazine, magazine.insertPointTransform.position);
            if (magazineUI == null)
                return;

            magazineUI.OnMagEvent();
            magazineUI.UpdateParent(magazine.insertPointTransform);
            magazineUI.DisplayMagInfo(magazine);
        }

        /// <summary>
        /// Called when a magazine previously grabbed is dropped
        /// </summary>
        internal void OnMagazineDetached(Magazine magazine)
        {
            if (!LastMags.Contains(magazine))
                return;

            var magUI = GetMagazineUI(magazine);

            if (magUI == null)
                MelonLogger.Msg("Mag UI is null");

            LastMags.Remove(magazine);

            magUI?.FadeOut();
        }

        /// <summary>
        /// Called when the player inserts a magazine into their gun.
        /// </summary>
        internal void OnMagazineInserted(Gun gun)
        {
            if (!Settings.Instance.ShowWithGun)
                return;

            if (!LastGuns.Contains(gun))
                return;

            var magazineUI = AddMagazineUI(gun, gun.firePointTransform.position);
            if (magazineUI == null)
                return;

            magazineUI.OnMagEvent();
            magazineUI.UpdateParent(gun.firePointTransform);
            magazineUI.DisplayGunInfo(gun);
        }

        /// <summary>
        /// Called when the player ejects the magazine from their gun.
        /// </summary>
        internal void OnMagazineEjected(Gun gun)
        {
            if (!Settings.Instance.ShowWithGun)
                return;

            if (!LastGuns.Contains(gun))
                return;

            var magazineUI = AddMagazineUI(gun, gun.firePointTransform.position);
            if (magazineUI == null)
                return;

            magazineUI.OnMagEvent();
            magazineUI.UpdateParent(gun.firePointTransform);
            magazineUI.DisplayGunInfo(gun);
        }

        /// <summary>
        /// Called when a player grabs a gun.
        /// </summary>
        internal void OnGunAttached(Gun gun)
        {
            if (!Settings.Instance.ShowWithGun)
                return;

            if (gun == null)
                return;

            if (gun.GetComponent<SpawnGun>() != null)
                return;

            LastGuns.Add(gun);

            var magazineUI = AddMagazineUI(gun, gun.firePointTransform.position);
            if (magazineUI == null)
                return;

            magazineUI.OnMagEvent();
            magazineUI.UpdateParent(gun.firePointTransform);
            magazineUI.DisplayGunInfo(gun);
        }

        /// <summary>
        /// Called when a player lets go of a gun.
        /// </summary>
        internal void OnGunDetached(Gun gun)
        {
            if (!Settings.Instance.ShowWithGun)
                return;

            if (LastGunGrips.TryGetValue(gun, out List<Grip> grips) && grips.Count > 0)
                return;

            if (!LastGuns.Contains(gun))
                return;

            LastGuns.Remove(gun);

            var magUI = GetMagazineUI(gun);

            magUI?.FadeOut();
        }

        /// <summary>
        /// Called when a round (spent or unspent) is ejected from the chamber.
        /// </summary>
        internal void OnGunEjectRound(Gun gun)
        {
            if (!Settings.Instance.ShowWithGun)
                return;

            if (!LastGuns.Contains(gun))
                return;

            var magazineUI = AddMagazineUI(gun, gun.firePointTransform.position);
            if (magazineUI == null)
                return;

            magazineUI.OnMagEvent();
            magazineUI.UpdateParent(gun.firePointTransform);
            magazineUI.DisplayGunInfo(gun);
        }

        /// <summary>
        /// Called when a gun gets holstered
        /// </summary>
        internal void OnGunHolstered(Gun gun)
        {
            if (!Settings.Instance.ShowWithGun)
                return;

            if (!LastGuns.Contains(gun))
                return;

            if (LastGunGrips.ContainsKey(gun))
                LastGunGrips[gun] = [];
            OnGunDetached(gun);
        }
    }
}