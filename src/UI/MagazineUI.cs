using UnityEngine;

using Il2CppSLZ.Marrow;
using Il2CppTMPro;
using System;
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

        public bool IsShown => gameObject != null && gameObject.active;

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

            if (DisplayInfo != null)
                UpdateInfo(DisplayInfo);

            if (showType == UIShowType.FadeShow)
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
                if (!IsShown || fadeOut)
                    FadeIn();
            }
            else if (showType == UIShowType.FadeShow)
            {
                if (!IsShown || fadeOut)
                    FadeIn();
            }
            else if (showType == UIShowType.Hide)
            {
                Hide();
            }
        }

        public static string GetReserve(CartridgeData data)
        {
            if (data == null || AmmoInventory.Instance == null || AmmoInventory.Instance.GetGroupByCartridge(data) == null)
                return "0";

            if (Main.FindMelon("InfiniteAmmo", "SoulWithMae") != null)
                return "INFINITE";
            else
                return AmmoInventory.Instance.GetCartridgeCount(data).ToString();
        }

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
                if (gun.defaultMagazine != null && gun.defaultCartridge != null)
                {
                    counterText = gun.chamberedCartridge != null ? $"+1/{gun.defaultMagazine.rounds}" : $"0/{gun.defaultMagazine.rounds}";
                    AmmoCounterText.text = counterText;

                    AmmoInventoryText.text = $"RESERVE: {GetReserve(gun.defaultCartridge)}";
                    AmmoTypeText.text = !string.IsNullOrWhiteSpace(gun.defaultMagazine.platform) ? gun.defaultMagazine.platform : "Unknown";
                }
                else
                {
                    counterText = gun.chamberedCartridge != null ? "+1/0" : "0/0";
                    AmmoCounterText.text = counterText;
                    AmmoInventoryText.text = "RESERVE: None";
                    AmmoTypeText.text = "Unknown";
                }
                return;
            }

            bool inChamber = gun.chamberedCartridge != null;
            bool toppedOff = inChamber && magazineState.AmmoCount == magazineState.magazineData.rounds;

            int ammoCount = magazineState.AmmoCount;
            int maxAmmo = magazineState.magazineData.rounds;
            string ammoType = magazineState.magazineData.platform;

            if (inChamber && !toppedOff && gun.cartridgeState != Gun.CartridgeStates.SPENT)
                ammoCount++;

            var ammoInventory = GetReserve(magazineState.cartridgeData);

            counterText = toppedOff ? $"{ammoCount}+1/{maxAmmo}" : $"{ammoCount}/{maxAmmo}";

            AmmoCounterText.text = counterText;
            AmmoInventoryText.text = "RESERVE: " + ammoInventory;
            AmmoTypeText.text = ammoType;
        }

        public void DisplayMagInfo(Magazine magazine)
        {
            DisplayInfo = new DisplayInfo(DisplayInfo.DisplayFor.MAG, magazine);
            var magazineState = magazine?.magazineState;
            if (magazineState == null)
            {
                AmmoCounterText.text = "0/0";
                AmmoInventoryText.text = "RESERVE: None";
                AmmoTypeText.text = "Unknown";
                return;
            }

            int ammoCount = magazineState.AmmoCount;
            int maxAmmo = magazineState.magazineData.rounds;
            string ammoType = magazineState.magazineData.platform;

            var ammoInventory = GetReserve(magazineState.cartridgeData);

            AmmoCounterText.text = $"{ammoCount}/{maxAmmo}";
            AmmoInventoryText.text = "RESERVE: " + ammoInventory;
            AmmoTypeText.text = ammoType;
        }

        public void UpdateInfo(DisplayInfo info)
        {
            if (info == null || info.Object == null)
                return;

            if (info.Type == DisplayInfo.DisplayFor.GUN)
            {
                if (info.Object is Gun gun)
                    DisplayGunInfo(gun);
            }
            else if (info.Type == DisplayInfo.DisplayFor.MAG)
            {
                if (info.Object is Magazine mag)
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
            fadeOut = false;
            Show();
            Animator?.Play("mag_enter_01");
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

    public class DisplayInfo
    {
        public DisplayFor Type { get; }

        // Gun for GUN Type, Magazine for MAG Type
        public object Object { get; }

        public DisplayInfo(DisplayFor type, object @object)
        {
            Type = type;
            if (@object is not Magazine && @object is not Gun)
                throw new Exception("Object must be of type Gun or Magazine!");
            Object = @object;
        }

        public enum DisplayFor
        {
            GUN,
            MAG
        }
    }
}