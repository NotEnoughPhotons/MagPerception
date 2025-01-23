using UnityEngine;

using NEP.MagPerception.UI;

using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Data;
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

        public Gun lastGun;

        public Magazine lastMag;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            GameObject magUI = GameObject.Instantiate(Main.Resources.LoadAsset("MagazineLayer").Cast<GameObject>(), transform);

            magUI.transform.SetParent(transform);
            MagazineUI = magUI.AddComponent<MagazineUI>();

            MagazineUI.ammoCounterText = magUI.transform.Find("AmmoCounter").GetComponent<TextMeshProUGUI>();
            MagazineUI.ammoInventoryText = magUI.transform.Find("AmmoInventory").GetComponent<TextMeshProUGUI>();
            MagazineUI.ammoTypeText = magUI.transform.Find("AmmoType").GetComponent<TextMeshProUGUI>();
            MagazineUI.animator = magUI.GetComponent<Animator>();

            magUI.SetActive(false);
        }

        /// <summary>
        /// Called when a player grabs a magazine.
        /// </summary>
        public void OnMagazineAttached(Magazine magazine)
        {
            lastMag = magazine;
            MagazineUI.Show();
            MagazineUI.OnMagEvent();
            MagazineUI.UpdateParent(lastMag.insertPointTransform);
            MagazineUI.DisplayMagInfo(magazine.magazineState);
        }

        /// <summary>
        /// Called when the player inserts a magazine into their gun.
        /// </summary>
        public void OnMagazineInserted(Gun gun)
        {
            lastGun = gun;

            if (!Settings.Instance.ShowWithGun)
                return;

            MagazineUI.Show();
            MagazineUI.OnMagEvent();
            MagazineUI.UpdateParent(lastGun.firePointTransform);
            MagazineUI.DisplayGunInfo(lastGun);
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
            MagazineUI.UpdateParent(lastGun.firePointTransform);
            MagazineUI.DisplayGunInfo(lastGun);
        }

        /// <summary>
        /// Called when a player grabs a gun.
        /// </summary>
        public void OnGunAttached(Gun gun)
        {
            if (!Settings.Instance.ShowWithGun)
                return;

            lastGun = gun;
            MagazineUI.OnMagEvent();
            MagazineUI.UpdateParent(gun.firePointTransform);
            MagazineUI.Show();
        }

        /// <summary>
        /// Called when a player lets go of a gun.
        /// </summary>
        public void OnGunDetached(Gun gun)
        {
            if (!Settings.Instance.ShowWithGun)
                return;

            lastGun = null;
            MagazineUI.OnMagEvent();

            if (lastMag != null)
            {
                MagazineUI.UpdateParent(gun.firePointTransform);
                MagazineUI.DisplayMagInfo(lastMag.magazineState);
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
            MagazineUI.DisplayGunInfo(lastGun);
        }
    }
}