using UnityEngine;

using NEP.MagPerception.UI;

using Il2CppSLZ.Marrow;
using Il2CppTMPro;

namespace NEP.MagPerception
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class MagPerceptionManager : MonoBehaviour
    {
        public MagPerceptionManager(System.IntPtr ptr) : base(ptr)
        {
        }

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

        /// <summary>
        /// Called when a player grabs a magazine.
        /// </summary>
        public void OnMagazineAttached(Magazine magazine)
        {
            LastMag = magazine;
            MagazineUI.Show();
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
            MagazineUI.Hide();
            MagazineUI.OnMagEvent();

            if (LastGun != null)
            {
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
            LastGun = gun;

            if (!Settings.Instance.ShowWithGun)
                return;

            MagazineUI.Show();
            MagazineUI.OnMagEvent();
            MagazineUI.UpdateParent(LastGun.firePointTransform);
            MagazineUI.DisplayGunInfo(LastGun);
        }

        /// <summary>
        /// Called when the player ejects the magazine from their gun.
        /// </summary>
        public void OnMagazineEjected()
        {
            if (!Settings.Instance.ShowWithGun)
                return;

            MagazineUI.Show();
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

            LastGun = gun;
            MagazineUI.OnMagEvent();
            MagazineUI.UpdateParent(gun.firePointTransform);
            MagazineUI.Show();
        }

        /// <summary>
        /// Called when a player lets go of a gun.
        /// </summary>
        public void OnGunDetached()
        {
            if (!Settings.Instance.ShowWithGun)
                return;

            LastGun = null;
            MagazineUI.OnMagEvent();

            if (LastMag != null)
            {
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
        public void OnGunEjectRound()
        {
            if (!Settings.Instance.ShowWithGun)
                return;

            MagazineUI.OnMagEvent();
            MagazineUI.DisplayGunInfo(LastGun);
        }
    }
}