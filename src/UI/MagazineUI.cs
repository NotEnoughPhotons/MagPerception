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

        public TextMeshProUGUI ammoCounterText;
        public TextMeshProUGUI ammoInventoryText;
        public TextMeshProUGUI ammoTypeText;

        public Animator animator;

        public bool IsBeingInteracted { get; set; } = false;

        private float timeSinceLastEvent = 0.0f;
        private bool fadeOut = false;

        private float fadeOutTime = 0.0f;
        private const float fadeOutDuration = 0.25f;

        private Vector3 targetPosition;
        private Quaternion lastRotation;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void FixedUpdate()
        {
            transform.localScale = new Vector3(-1f, 1f, 1f) * Settings.Instance.InfoScale;
            transform.LookAt(BoneLib.Player.Head);

            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition + Settings.Instance.Offset, 8f * Time.fixedDeltaTime);
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
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

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

            string counterText = "";
            var magazineState = gun.MagazineState;

            if (magazineState == null)
            {
                counterText = gun.chamberedCartridge != null ? "+1/0" : "0/0";
                ammoCounterText.text = counterText;
                ammoInventoryText.text = "RESERVE: None";
                ammoTypeText.text = "Unknown";
                return;
            }

            bool toppedOff = gun.chamberedCartridge != null && magazineState.AmmoCount == magazineState.magazineData.rounds;

            int ammoCount = magazineState.AmmoCount;
            int maxAmmo = magazineState.magazineData.rounds;
            string ammoType = magazineState.magazineData.platform;

            var ammoInventory = AmmoInventory.Instance.GetCartridgeCount(magazineState.cartridgeData);

            if (toppedOff)
            {
                counterText = $"{ammoCount}+1/{maxAmmo}";
            }
            else
            {
                counterText = $"{ammoCount}/{maxAmmo}";
            }

            ammoCounterText.text = counterText;
            ammoInventoryText.text = "RESERVE: " + ammoInventory.ToString();
            ammoTypeText.text = ammoType;
        }

        public void DisplayMagInfo(MagazineState magazineState)
        {
            if (magazineState == null)
            {
                return;
            }

            int ammoCount = magazineState.AmmoCount;
            int maxAmmo = magazineState.magazineData.rounds;
            string ammoType = magazineState.magazineData.platform;

            var ammoInventory = AmmoInventory.Instance.GetCartridgeCount(magazineState.cartridgeData);

            ammoCounterText.text = $"{ammoCount}/{maxAmmo}";
            ammoInventoryText.text = "RESERVE: " + ammoInventory.ToString();
            ammoTypeText.text = ammoType;
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
                animator?.Play("mag_enter_01");
                fadeOut = false;
            }
        }

        private void FadeOut()
        {
            if (!fadeOut)
            {
                animator?.SetTrigger("exit");
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