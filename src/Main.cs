using System.Linq;

using MelonLoader;

using UnityEngine;

using NEP.MagPerception.UI;
using System.IO;
using MelonLoader.Utils;
using MelonLoader.Preferences;
using Il2CppSLZ.Marrow;
using System.Collections.Generic;
using System;
using Object = UnityEngine.Object;

namespace NEP.MagPerception
{
    public static class BuildInfo
    {
        public const string Name = "MagPerception"; // Name of the Mod.  (MUST BE SET)
        public const string Author = "NotEnoughPhotons"; // Author of the Mod.  (Set as null if none)
        public const string Company = "NotEnoughPhotons"; // Company that made the Mod.  (Set as null if none)
        public const string Version = "1.2.2"; // Version of the Mod.  (MUST BE SET)
        public const string DownloadLink = "https://thunderstore.io/c/bonelab/p/NotEnoughPhotons/MagPerception/"; // Download Link for the Mod.  (Set as null if none)
    }

    public class Main : MelonMod
    {
        internal static AssetBundle Resources { get; private set; }

        internal static MelonPreferences_ReflectiveCategory PrefsCategory { get; private set; }

        public static bool IsAssemblyLoaded(string name) => AppDomain.CurrentDomain.GetAssemblies().Any(x => x.GetName() != null && string.Equals(x.GetName().Name, name, StringComparison.OrdinalIgnoreCase));

        public override void OnInitializeMelon()
        {
            if (!IsAssemblyLoaded("BoneLib"))
            {
                LoggerInstance.Error("BoneLib is required for this mod to work");
            }
            else
            {
                OnInitialize();
            }
        }

        private static void OnInitialize()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            const string bundlePath = "NEP.MagPerception.Resources.";
            string targetBundle = BoneLib.HelperMethods.IsAndroid() ? "mp_resources_quest.pack" : "mp_resources_pcvr.pack";

            Resources = BoneLib.HelperMethods.LoadEmbeddedAssetBundle(assembly, bundlePath + targetBundle);

            if (Resources == null)
                throw new System.Exception("Resources file is missing/invalid!");

            SetupPreferences();
            SetupBonemenu();

            BoneLib.Hooking.OnLevelLoaded += (_) => OnSceneWasLoaded();
        }

        private static void SetupPreferences()
        {
            PrefsCategory = MelonPreferences.CreateCategory<Settings>("MagPerception");
            var authorDir = Directory.CreateDirectory(Path.Combine(MelonEnvironment.UserDataDirectory, "Not Enough Photons"));
            var modDir = Directory.CreateDirectory(Path.Combine(authorDir.FullName, "MagPerception"));
            PrefsCategory.SetFilePath(Path.Combine(modDir.FullName, "MagPerception.cfg"));
            PrefsCategory.SaveToFile(false);
            Settings.Instance = PrefsCategory.GetValue<Settings>();
        }

        private static void SetupBonemenu()
        {
            var nepCategory = BoneLib.BoneMenu.Page.Root.CreatePage("Not Enough Photons", Color.white);
            var mainCategory = nepCategory.CreatePage("MagPerception", Color.white);
            var offsetCategory = mainCategory.CreatePage("Offset", Color.white);
            var textColorCategory = mainCategory.CreatePage("Text Color", Color.red);

            mainCategory.CreateFloat("Text Opacity", Color.white, Settings.Instance.TextOpacity, 0.05f, 0.05f, 1, (value) => Settings.Instance.TextOpacity = value);
            mainCategory.CreateFloat("Scale", Color.white, Settings.Instance.InfoScale, 0.25f, 0.25f, 1.5f, (value) => Settings.Instance.InfoScale = value);
            mainCategory.CreateEnum("Show Type", Color.white, Settings.Instance.ShowType, (showType) => Settings.Instance.ShowType = (UIShowType)showType);
            mainCategory.CreateFloat("Time Until Hidden", Color.white, Settings.Instance.TimeUntilHidden, 0.5f, 0f, 10f, (value) => Settings.Instance.TimeUntilHidden = value);
            mainCategory.CreateBool("Show With Gun", Color.white, Settings.Instance.ShowWithGun, (value) => Settings.Instance.ShowWithGun = value);

            offsetCategory.CreateFloat("X", Color.red, Settings.Instance.Offset.x, 0.025f, -1f, 1f, (value) => Settings.Instance.ChangeXYZOffset(Settings.OffsetValue.X, value));
            offsetCategory.CreateFloat("Y", Color.green, Settings.Instance.Offset.y, 0.025f, -1f, 1f, (value) => Settings.Instance.ChangeXYZOffset(Settings.OffsetValue.Y, value));
            offsetCategory.CreateFloat("Z", Color.blue, Settings.Instance.Offset.z, 0.025f, -1f, 1f, (value) => Settings.Instance.ChangeXYZOffset(Settings.OffsetValue.Z, value));

            Color.RGBToHSV(Settings.Instance.TextColor, out float H, out float S, out float V);
            textColorCategory.CreateFloat("Hue", Color.red, H, 0.05f, 0, 1, (val) => Settings.Instance.ChangeHSV(Settings.HSVValue.H, val));
            textColorCategory.CreateFloat("Saturation", Color.green, S, 0.05f, 0, 1, (val) => Settings.Instance.ChangeHSV(Settings.HSVValue.S, val));
            textColorCategory.CreateFloat("Value", Color.blue, V, 0.05f, 0, 1, (val) => Settings.Instance.ChangeHSV(Settings.HSVValue.V, val));
        }

        public static void OnSceneWasLoaded()
        {
            new GameObject("Mag Perception Manager").AddComponent<MagPerceptionManager>();
        }

        internal static Object GetObjectFromResources(string name)
        {
            Object[] objects = Resources.LoadAllAssets();

            return objects.FirstOrDefault((asset) => asset.name == name);
        }

        void MagUpdate()
        {
            var mag = Hooks.OnMagAttached.CurrentMagazine;
            var hand = Hooks.OnMagAttached.HoldingHand;

            var magUI = MagPerceptionManager.Instance?.MagazineUI;

            if (hand != null)
            {
                if (IsPressed(hand))
                {
                    if (mag != null && magUI != null && magUI?.IsShown != true)
                        MagPerceptionManager.Instance?.OnMagazineAttached(mag);
                }
            }
        }

        readonly List<Hand> _holding = [];

        private bool IsPressed(Hand hand)
        {
            if (hand.Controller == null)
                return false;

            if (!hand.Controller.GetMenuButtonDown())
            {
                _holding.Remove(hand);
            }
            else
            {
                if (!_holding.Contains(hand))
                {
                    _holding.Add(hand);
                    return true;
                }
            }
            return false;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            var magUI = MagPerceptionManager.Instance?.MagazineUI;

            if (magUI?.fadeOut == true && magUI.IsShown)
                magUI.FadeOut();

            magUI?.UpdateInfo(magUI?.DisplayInfo);

            MagUpdate();
        }
    }
}