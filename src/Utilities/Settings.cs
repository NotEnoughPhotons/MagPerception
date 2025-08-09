using NEP.MagPerception.UI;

using UnityEngine;

using Tomlet.Attributes;
using System.Drawing;
using System.Collections.Generic;
using System;

namespace NEP.MagPerception
{
    public class Settings
    {
        [TomlNonSerialized]
        public static Settings Instance { get; internal set; }

        [TomlProperty("InfoScale")]
        private float _infoScale { get; set; } = 0.75f;

        [TomlNonSerialized]
        public float InfoScale
        {
            get
            {
                return _infoScale;
            }
            set
            {
                _infoScale = value;
                Main.PrefsCategory.SaveToFile(false);
            }
        }

        [TomlProperty("Offset")]
        private Vector3 _offset { get; set; } = new(0.075f, 0f, 0f);

        [TomlNonSerialized]
        public Vector3 Offset
        {
            get
            {
                return _offset;
            }
            set
            {
                _offset = value;
                Main.PrefsCategory.SaveToFile(false);
            }
        }

        [TomlProperty("TimeUntilHidden")]
        private float _timeUntilHidden { get; set; } = 3f;

        [TomlNonSerialized]
        public float TimeUntilHidden
        {
            get
            {
                return _timeUntilHidden;
            }
            set
            {
                _timeUntilHidden = value;
                Main.PrefsCategory.SaveToFile(false);
            }
        }

        [TomlProperty("ShowType")]
        private UIShowType _showType { get; set; } = UIShowType.FadeShow;

        [TomlNonSerialized]
        public UIShowType ShowType
        {
            get
            {
                return _showType;
            }
            set
            {
                _showType = value;
                Main.PrefsCategory.SaveToFile(false);

                if (MagPerceptionManager.LastGuns?.Count > 0 || MagPerceptionManager.LastMags?.Count > 0)
                {
                    List<MagazineUI> UIs = [];
                    MagPerceptionManager.LastGuns.ForEach(x =>
                    {
                        if (x != null && MagPerceptionManager.MagazineUIs.ContainsKey(x))
                            UIs.Add(MagPerceptionManager.MagazineUIs[x]);
                    });
                    MagPerceptionManager.LastMags.ForEach(x =>
                    {
                        if (x != null && MagPerceptionManager.MagazineUIs.ContainsKey(x))
                            UIs.Add(MagPerceptionManager.MagazineUIs[x]);
                    });
                    UIs.ForEach(x => x.OnMagEvent());
                }
            }
        }

        [TomlProperty("ShowWithGun")]
        private bool _showWithGun { get; set; } = true;

        [TomlNonSerialized]
        public bool ShowWithGun
        {
            get
            {
                return _showWithGun;
            }
            set
            {
                _showWithGun = value;
                Main.PrefsCategory.SaveToFile(false);
            }
        }

        [TomlProperty("TextOpacity")]
        private float _textOpacity { get; set; } = 0.5f;

        [TomlNonSerialized]
        public float TextOpacity
        {
            get
            {
                return _textOpacity;
            }
            set
            {
                _textOpacity = value;
                Main.PrefsCategory.SaveToFile(false);
            }
        }

        [TomlNonSerialized]
        public Color32 TextColor
        {
            get
            {
                var drawingColor = ColorTranslator.FromHtml(TextColorHEX);
                return new Color32(drawingColor.R, drawingColor.G, drawingColor.B, drawingColor.A);
            }
            set
            {
                TextColorHEX = $"#{value.r:X2}{value.g:X2}{value.b:X2}";
            }
        }

        [TomlProperty("TextColor")]
        private string _textColorHEX { get; set; } = "#FFFFFF";

        [TomlNonSerialized]
        public string TextColorHEX
        {
            get => _textColorHEX;
            set
            {
                _textColorHEX = value;
                OnColorChanged?.Invoke();
                Main.PrefsCategory.SaveToFile(false);
            }
        }

        public Action OnColorChanged;

        public void ChangeHSV(HSVValue hsvValue, float value)
        {
            UnityEngine.Color.RGBToHSV(TextColor, out float H, out float S, out float V);
            switch (hsvValue)
            {
                case HSVValue.H:
                    H = value; break;

                case HSVValue.V:
                    V = value; break;

                case HSVValue.S:
                    S = value; break;
            }
            TextColor = UnityEngine.Color.HSVToRGB(H, S, V);
        }

        public void ChangeXYZOffset(OffsetValue xyz, float value)
        {
            var offset = Offset;
            switch (xyz)
            {
                case OffsetValue.X:
                    offset.x = value; break;
                case OffsetValue.Y:
                    offset.y = value; break;
                case OffsetValue.Z:
                    offset.z = value; break;
            }
            Offset = offset;
        }

        public enum OffsetValue
        {
            X,
            Y,
            Z,
        }

        public enum HSVValue
        {
            H,
            S,
            V
        }
    }
}