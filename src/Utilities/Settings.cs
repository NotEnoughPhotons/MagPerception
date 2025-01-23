using NEP.MagPerception.UI;

using UnityEngine;

using Tomlet.Attributes;

namespace NEP.MagPerception
{
    public class Settings
    {
        [TomlNonSerialized]
        public static Settings Instance;

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
        private Vector3 _offset = new Vector3(0.075f, 0f, 0f);

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
        private float _timeUntilHidden = 1.5f;

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
                Main.PrefsCategory.SaveToFile(false);
            }
        }

        [TomlNonSerialized]
        private bool _showWithGun = false;

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
    }
}