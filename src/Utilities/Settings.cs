using NEP.MagPerception.UI;

using UnityEngine;

using Tomlet.Attributes;
using System.Drawing;

namespace NEP.MagPerception
{
    public class Settings
    {
        [TomlNonSerialized]
        public static Settings Instance { get; internal set; }

        [TomlNonSerialized]
        private float _infoScale = 0.75f;

        [TomlProperty("InfoScale")]
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

        [TomlNonSerialized]
        private Vector3 _offset = new(0f, 0.1f, 0f);

        [TomlProperty("Offset")]
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

        [TomlNonSerialized]
        private float _timeUntilHidden = 3f;

        [TomlProperty("TimeUntilHidden")]
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

        [TomlNonSerialized]
        private UIShowType _showType = UIShowType.FadeShow;

        [TomlProperty("ShowType")]
        public UIShowType ShowType
        {
            get
            {
                return _showType;
            }
            set
            {
                _showType = value;
                if (MagPerceptionManager.Instance?.LastGun != null || MagPerceptionManager.Instance?.LastMag != null)
                    MagPerceptionManager.Instance?.MagazineUI?.OnMagEvent();
                Main.PrefsCategory.SaveToFile(false);
            }
        }

        [TomlNonSerialized]
        private bool _showWithGun = true;

        [TomlProperty("ShowWithGun")]
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

        [TomlNonSerialized]
        private float _textOpacity = 1;

        [TomlProperty("TextOpacity")]
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

        [TomlNonSerialized]
        private string _textColorHEX = "#FFFFFF";

        [TomlProperty("TextColor")]
        public string TextColorHEX
        {
            get => _textColorHEX;
            set
            {
                _textColorHEX = value;
                Main.PrefsCategory.SaveToFile(false);
            }
        }

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