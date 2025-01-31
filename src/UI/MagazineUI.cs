using UnityEngine;

using Il2CppSLZ.Marrow;
using Il2CppTMPro;
using Il2CppSLZ.Marrow.Data;

namespace NEP.MagPerception.UI
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class MagazineUI(System.IntPtr ptr) : MonoBehaviour(ptr)
    {
        public TextMeshProUGUI AmmoCounterText { get; internal set; }
        public TextMeshProUGUI AmmoInventoryText { get; internal set; }
        public TextMeshProUGUI AmmoTypeText { get; internal set; }

        public Animator Animator { get; internal set; }

        private float timeSinceLastEvent = 0.0f;
        internal bool fadeOut = false;

        private float fadeOutTime = 0.0f;
        private const float fadeOutDuration = 0.25f;

        private Quaternion lastRotation;

        public DisplayInfo DisplayInfo { get; private set; } = null;

        public bool IsShown => gameObject.active;

        private void Awake()
        {
            Animator = GetComponent<Animator>();
        }

        private void FixedUpdate()
        {
            transform.localScale = new Vector3(-1f, 1f, 1f) * Settings.Instance.InfoScale;
            transform.LookAt(BoneLib.Player.Head);

            transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero + Settings.Instance.Offset, 8f * Time.fixedDeltaTime);
            transform.rotation = Quaternion.Slerp(lastRotation, transform.rotation, 8f * Time.fixedDeltaTime);

            if (AmmoCounterText != null)
            {
                AmmoCounterText.faceColor = Settings.Instance.TextColor;
                AmmoCounterText.alpha = Settings.Instance.TextOpacity;
            }

            if (AmmoTypeText != null)
            {
                AmmoTypeText.faceColor = Settings.Instance.TextColor;
                AmmoTypeText.alpha = Settings.Instance.TextOpacity;
            }

            if (AmmoInventoryText != null)
            {
                AmmoInventoryText.faceColor = Settings.Instance.TextColor;
                AmmoInventoryText.alpha = Settings.Instance.TextOpacity;
            }

            UIShowType showType = Settings.Instance.ShowType;

            if (showType == UIShowType.Always)
            {
                Show();
                return;
            }
            else if (showType == UIShowType.FadeShow)
            {
                timeSinceLastEvent += Time.deltaTime;

                if (timeSinceLastEvent > Settings.Instance.TimeUntilHidden)
                {
                    timeSinceLastEvent = 0.0f;
                    FadeOut();
                }
            }
            else if (showType == UIShowType.Hide)
            {
                Hide();
            }
        }

        public void Show()
            => gameObject.SetActive(true);

        public void Hide()
            => gameObject.SetActive(false);

        public void OnMagEvent()
        {
            UIShowType showType = Settings.Instance.ShowType;

            if (showType == UIShowType.Always)
            {
                if (!IsShown)
                    FadeIn();
            }
            else if (showType == UIShowType.FadeShow)
            {
                FadeIn();
            }
            else if (showType == UIShowType.Hide)
            {
                Hide();
            }
        }

        public CartridgeData CartridgeData { get; private set; } = null;

        public void DisplayGunInfo(Gun gun)
        {
            DisplayInfo = new DisplayInfo(DisplayInfo.DisplayFor.GUN, gun);
            if (gun == null)
            {
                // Chances are you are charging the gun with your hand off the main grip.
                // Will fix in the future.

                // Future person here, this is fixed
                return;
            }

            var magazineState = gun.MagazineState;

            string counterText;
            if (magazineState == null)
            {
                counterText = gun.chamberedCartridge != null ? "+1/0" : "0/0";
                AmmoCounterText.text = counterText;
                AmmoInventoryText.text = "RESERVE: None";
                AmmoTypeText.text = "Unknown";
                return;
            }

            bool inChamber = gun.chamberedCartridge != null;
            bool toppedOff = inChamber && magazineState.AmmoCount == magazineState.magazineData.rounds;

            int ammoCount = magazineState.AmmoCount;
            int maxAmmo = magazineState.magazineData.rounds;
            string ammoType = magazineState.magazineData.platform;

            if (inChamber && !toppedOff && gun.cartridgeState != Gun.CartridgeStates.SPENT)
                ammoCount++;

            CartridgeData = magazineState.cartridgeData;

            var ammoInventory = AmmoInventory.Instance.GetCartridgeCount(magazineState.cartridgeData);

            counterText = toppedOff ? $"{ammoCount}+1/{maxAmmo}" : $"{ammoCount}/{maxAmmo}";

            AmmoCounterText.text = counterText;
            AmmoInventoryText.text = "RESERVE: " + ammoInventory.ToString();
            AmmoTypeText.text = ammoType;
        }

        public void DisplayMagInfo(MagazineState magazineState)
        {
            DisplayInfo = new DisplayInfo(DisplayInfo.DisplayFor.MAG, magazineState);
            if (magazineState == null)
                return;

            int ammoCount = magazineState.AmmoCount;
            int maxAmmo = magazineState.magazineData.rounds;
            string ammoType = magazineState.magazineData.platform;

            CartridgeData = magazineState.cartridgeData;

            var ammoInventory = AmmoInventory.Instance.GetCartridgeCount(magazineState.cartridgeData);

            AmmoCounterText.text = $"{ammoCount}/{maxAmmo}";
            AmmoInventoryText.text = "RESERVE: " + ammoInventory.ToString();
            AmmoTypeText.text = ammoType;
        }

        public void UpdateInfo(DisplayInfo info)
        {
            if (info == null) return;

            if (info.Type == DisplayInfo.DisplayFor.GUN)
            {
                if (info.Object is Gun gun)
                    DisplayGunInfo(gun);
            }
            else if (info.Type == DisplayInfo.DisplayFor.MAG)
            {
                if (info.Object is MagazineState mag)
                    DisplayMagInfo(mag);
            }
        }

        public void UpdateParent(Transform attachment)
        {
            transform.parent = attachment;
        }

        public void FadeIn()
        {
            timeSinceLastEvent = 0.0f;
            Show();
            Animator?.Play("mag_enter_01");
            fadeOut = false;
        }

        public void FadeOut()
        {
            if (!fadeOut)
            {
                Animator?.SetTrigger("exit");
                fadeOut = true;
            }
            else
            {
                fadeOutTime += Time.deltaTime;

                if (fadeOutTime > fadeOutDuration)
                {
                    fadeOutTime = 0.0f;
                    Hide();
                }
            }
        }
    }

    public class DisplayInfo(DisplayInfo.DisplayFor Type, object Object)
    {
        public DisplayFor Type { get; } = Type;

        // Gun for GUN Type, MagazineState for MAG Type
        public object Object { get; } = Object;

        public enum DisplayFor
        {
            GUN,
            MAG
        }
    }
}