using System.Linq;

using MelonLoader;

using UnityEngine;

using BoneLib;
using BoneLib.BoneMenu;

using NEP.MagPerception.UI;
using System.IO;
using MelonLoader.Utils;
using MelonLoader.Preferences;
using Il2CppSLZ.Marrow;
using System.Collections.Generic;
using Il2CppSLZ.Marrow.Data;

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

        public override void OnInitializeMelon()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            const string bundlePath = "NEP.MagPerception.Resources.";
            string targetBundle = HelperMethods.IsAndroid() ? "mp_resources_quest.pack" : "mp_resources_pcvr.pack";

            Resources = HelperMethods.LoadEmbeddedAssetBundle(assembly, bundlePath + targetBundle);

            if (Resources == null)
                throw new System.Exception("Resources file is missing/invalid!");

            SetupPreferences();
            SetupBonemenu();

            Hooking.OnLevelLoaded += OnSceneWasLoaded;
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
            var nepCategory = Page.Root.CreatePage("Not Enough Photons", Color.white);
            var mainCategory = nepCategory.CreatePage("MagPerception", Color.white);
            var offsetCategory = mainCategory.CreatePage("Offset", Color.white);

            mainCategory.CreateFloat("Text Opacity", Color.white, Settings.Instance.TextOpacity, 0.05f, 0.05f, 1, (value) => Settings.Instance.TextOpacity = value);
            mainCategory.CreateFloat("Scale", Color.white, Settings.Instance.InfoScale, 0.25f, 0.25f, 1.5f, (value) => Settings.Instance.InfoScale = value);
            mainCategory.CreateEnum("Show Type", Color.white, Settings.Instance.ShowType, (showType) => Settings.Instance.ShowType = (UIShowType)showType);
            mainCategory.CreateFloat("Time Until Hidden", Color.white, Settings.Instance.TimeUntilHidden, 0.5f, 0f, 10f, (value) => Settings.Instance.TimeUntilHidden = value);
            mainCategory.CreateBool("Show With Gun", Color.white, Settings.Instance.ShowWithGun, (value) => Settings.Instance.ShowWithGun = value);

            offsetCategory.CreateFloat("X", Color.red, Settings.Instance.Offset.x, 0.025f, -1f, 1f, (value) => Settings.Instance.ChangeXYZOffset(Settings.OffsetValue.X, value));
            offsetCategory.CreateFloat("Y", Color.green, Settings.Instance.Offset.y, 0.025f, -1f, 1f, (value) => Settings.Instance.ChangeXYZOffset(Settings.OffsetValue.Y, value));
            offsetCategory.CreateFloat("Z", Color.blue, Settings.Instance.Offset.z, 0.025f, -1f, 1f, (value) => Settings.Instance.ChangeXYZOffset(Settings.OffsetValue.Z, value));
        }

        public static void OnSceneWasLoaded(LevelInfo info)
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
                    if (mag != null && magUI?.IsShown != true)
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

        int lastRounds = 0;
        int lastMaxRounds = 0;

        public override void OnUpdate()
        {
            base.OnUpdate();

            var magUI = MagPerceptionManager.Instance?.MagazineUI;

            if (magUI?.fadeOut == true && magUI.IsShown)
                magUI.FadeOut();

            if (magUI?.DisplayInfo?.Type == DisplayInfo.DisplayFor.GUN)
            {
                var gun = magUI?.DisplayInfo?.Object as Gun;

                if (gun != null && magUI != null)
                {
                    if (gun.MagazineState != null && gun.MagazineState.AmmoCount != lastRounds)
                    {
                        lastRounds = gun.MagazineState.AmmoCount;
                        magUI.DisplayGunInfo(gun);
                    }
                    else if (gun.defaultMagazine != null && gun.defaultMagazine.rounds != lastMaxRounds)
                    {
                        lastMaxRounds = gun.defaultMagazine.rounds;
                        magUI.DisplayGunInfo(gun);
                    }
                }
            }

            MagUpdate();
        }
    }
}