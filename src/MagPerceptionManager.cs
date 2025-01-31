using UnityEngine;

using NEP.MagPerception.UI;

using Il2CppSLZ.Marrow;
using Il2CppTMPro;
using Il2CppSLZ.Bonelab;
using System.Collections.Generic;

namespace NEP.MagPerception
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class MagPerceptionManager(System.IntPtr ptr) : MonoBehaviour(ptr)
    {
        public static MagPerceptionManager Instance { get; private set; }

        public MagazineUI MagazineUI { get; private set; }

        public Gun LastGun { get; internal set; }

        public Magazine LastMag { get; internal set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            GameObject magUI = GameObject.Instantiate(Main.Resources.LoadAsset("MagazineLayer").Cast<GameObject>(), transform);

            magUI.transform.SetParent(transform);
            MagazineUI = magUI.AddComponent<MagazineUI>();

            MagazineUI.AmmoCounterText = magUI.transform.Find("AmmoCounter").GetComponent<TextMeshProUGUI>();
            MagazineUI.AmmoInventoryText = magUI.transform.Find("AmmoInventory").GetComponent<TextMeshProUGUI>();
            MagazineUI.AmmoTypeText = magUI.transform.Find("AmmoType").GetComponent<TextMeshProUGUI>();
            MagazineUI.Animator = magUI.GetComponent<Animator>();

            magUI.SetActive(false);
        }

        internal readonly List<Grip> LastGunGrips = [];

        /// <summary>
        /// Called when a player grabs a magazine.
        /// </summary>
        public void OnMagazineAttached(Magazine magazine)
        {
            LastMag = magazine;
            MagazineUI.OnMagEvent();
            MagazineUI.UpdateParent(LastMag.insertPointTransform);
            MagazineUI.DisplayMagInfo(magazine.magazineState);
        }

        /// <summary>
        /// Called when a magazine previously grabbed is dropped
        /// </summary>
        public void OnMagazineDetached(Magazine magazine)
        {
            if (magazine != LastMag)
                return;

            LastMag = null;

            if (LastGun != null)
            {
                MagazineUI.OnMagEvent();
                MagazineUI.UpdateParent(LastGun.firePointTransform);
                MagazineUI.DisplayGunInfo(LastGun);
            }
            else
            {
                MagazineUI.FadeOut();
            }
        }

        /// <summary>
        /// Called when the player inserts a magazine into their gun.
        /// </summary>
        public void OnMagazineInserted(Gun gun)
        {
            if (!Settings.Instance.ShowWithGun)
                return;

            if (LastGun != gun)
                return;

            MagazineUI.OnMagEvent();
            MagazineUI.UpdateParent(LastGun.firePointTransform);
            MagazineUI.DisplayGunInfo(LastGun);
        }

        /// <summary>
        /// Called when the player ejects the magazine from their gun.
        /// </summary>
        public void OnMagazineEjected(Gun gun)
        {
            if (!Settings.Instance.ShowWithGun)
                return;

            if (LastGun != gun)
                return;

            MagazineUI.OnMagEvent();
            MagazineUI.UpdateParent(LastGun.firePointTransform);
            MagazineUI.DisplayGunInfo(LastGun);
        }

        /// <summary>
        /// Called when a player grabs a gun.
        /// </summary>
        public void OnGunAttached(Gun gun)
        {
            if (!Settings.Instance.ShowWithGun)
                return;

            if (gun.GetComponent<SpawnGun>() != null)
                return;

            LastGun = gun;
            MagazineUI.OnMagEvent();
            MagazineUI.UpdateParent(gun.firePointTransform);
            MagazineUI.DisplayGunInfo(gun);
        }

        /// <summary>
        /// Called when a player lets go of a gun.
        /// </summary>
        public void OnGunDetached(Gun gun)
        {
            if (!Settings.Instance.ShowWithGun)
                return;

            if (LastGunGrips.Count > 0)
                return;

            if (LastGun != gun)
                return;

            LastGun = null;

            if (LastMag != null)
            {
                MagazineUI.OnMagEvent();
                MagazineUI.UpdateParent(LastMag.insertPointTransform);
                MagazineUI.DisplayMagInfo(LastMag.magazineState);
            }
            else
            {
                MagazineUI.FadeOut();
            }
        }

        /// <summary>
        /// Called when a round (spent or unspent) is ejected from the chamber.
        /// </summary>
        public void OnGunEjectRound(Gun gun)
        {
            if (!Settings.Instance.ShowWithGun)
                return;

            if (LastGun != gun)
                return;

            MagazineUI.OnMagEvent();
            MagazineUI.UpdateParent(LastGun.firePointTransform);
            MagazineUI.DisplayGunInfo(LastGun);
        }

        /// <summary>
        /// Called when a gun gets holstered
        /// </summary>
        public void OnGunHolstered(Gun gun)
        {
            if (!Settings.Instance.ShowWithGun)
                return;

            if (LastGun != gun)
                return;

            LastGun = null;
            LastGunGrips.Clear();

            if (LastMag != null)
            {
                MagazineUI.OnMagEvent();
                MagazineUI.UpdateParent(LastMag.insertPointTransform);
                MagazineUI.DisplayMagInfo(LastMag.magazineState);
            }
            else
            {
                MagazineUI.FadeOut();
            }
        }
    }
}