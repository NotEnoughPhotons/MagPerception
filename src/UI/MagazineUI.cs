using UnityEngine;

using Il2CppSLZ.Marrow;
using Il2CppTMPro;

namespace NEP.MagPerception.UI
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class MagazineUI : MonoBehaviour
    {
        public MagazineUI(System.IntPtr ptr) : base(ptr)
        {
        }

        public TextMeshProUGUI AmmoCounterText { get; internal set; }
        public TextMeshProUGUI AmmoInventoryText { get; internal set; }
        public TextMeshProUGUI AmmoTypeText { get; internal set; }

        public Animator Animator { get; internal set; }

        private float timeSinceLastEvent = 0.0f;
        private bool fadeOut = false;

        private float fadeOutTime = 0.0f;
        private const float fadeOutDuration = 0.25f;

        private Quaternion lastRotation;

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
                // undefined
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

        public void DisplayGunInfo(Gun gun)
        {
            if (gun == null)
            {
                // Chances are you are charging the gun with your hand off the main grip.
                // Will fix in the future.
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

            bool toppedOff = gun.chamberedCartridge != null && magazineState.AmmoCount == magazineState.magazineData.rounds;

            int ammoCount = magazineState.AmmoCount;
            int maxAmmo = magazineState.magazineData.rounds;
            string ammoType = magazineState.magazineData.platform;

            var ammoInventory = AmmoInventory.Instance.GetCartridgeCount(magazineState.cartridgeData);

            counterText = toppedOff ? $"{ammoCount}+1/{maxAmmo}" : $"{ammoCount}/{maxAmmo}";

            AmmoCounterText.text = counterText;
            AmmoInventoryText.text = "RESERVE: " + ammoInventory.ToString();
            AmmoTypeText.text = ammoType;
        }

        public void DisplayMagInfo(MagazineState magazineState)
        {
            if (magazineState == null)
                return;

            int ammoCount = magazineState.AmmoCount;
            int maxAmmo = magazineState.magazineData.rounds;
            string ammoType = magazineState.magazineData.platform;

            var ammoInventory = AmmoInventory.Instance.GetCartridgeCount(magazineState.cartridgeData);

            AmmoCounterText.text = $"{ammoCount}/{maxAmmo}";
            AmmoInventoryText.text = "RESERVE: " + ammoInventory.ToString();
            AmmoTypeText.text = ammoType;
        }

        public void UpdateParent(Transform attachment)
        {
            transform.parent = attachment;
        }

        private void FadeIn()
        {
            timeSinceLastEvent = 0.0f;
            if (fadeOut)
            {
                gameObject.SetActive(true);
                Animator?.Play("mag_enter_01");
                fadeOut = false;
            }
        }

        private void FadeOut()
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
                    gameObject.SetActive(false);
                }
            }
        }
    }
}