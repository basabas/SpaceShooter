using UnityEngine;

namespace Bas.Thoth.Editor
{
    public static class Layouts
    {
        private static GUIStyle _toggleButtonStyle;
        public static GUIStyle ToggleButtonStyle
        {
            get
            {
                if (_toggleButtonStyle == null || _toggleButtonStyle.normal.background == null)
                {
                    _toggleButtonStyle = new GUIStyle()
                    {
                        normal = { textColor = Color.white, background = CreateTexture(new Color(0.1f, 0.5f, 0.1f, 1f)), },
                        hover = { textColor = Color.white, background = CreateTexture(new Color(0.1f, 0.6f, 0.1f, 1f)) },
                        active = { textColor = Color.white, background = CreateTexture(new Color(0.1f, 0.3f, 0.1f, 1f)) },
                        alignment = TextAnchor.UpperCenter,
                        imagePosition = ImagePosition.ImageLeft,
                        fixedWidth = 22,
                        fixedHeight = 22,
                        fontSize = 16
                    };
                }
                return _toggleButtonStyle;
            }
        }

        private static GUIStyle _disposeButtonStyle;
        public static GUIStyle DisposeButtonStyle
        {
            get
            {
                if (_disposeButtonStyle == null || _disposeButtonStyle.normal.background == null)
                {
                    _disposeButtonStyle = new GUIStyle()
                    {
                        normal = { textColor = Color.white, background = CreateTexture(new Color(0.7f, 0.1f, 0.1f, 1f)), },
                        hover = { textColor = Color.white, background = CreateTexture(new Color(0.8f, 0.1f, 0.1f, 1f)) },
                        active = { textColor = Color.white, background = CreateTexture(new Color(0.5f, 0.1f, 0.1f, 1f)) },
                        alignment = TextAnchor.MiddleCenter,
                        fixedWidth = 22,
                        fixedHeight = 22,
                        fontSize = 16
                    };
                }
                return _disposeButtonStyle;
            }
        }


        private static GUIStyle _centeredComponentStyle;
        public static GUIStyle CenteredComponentStyle
        {
            get
            {
                if (_centeredComponentStyle == null || _centeredComponentStyle.normal.background == null)
                {
                    var texture = CreateTexture(Color.gray4);
                    _centeredComponentStyle = new GUIStyle()
                    {
                        normal = { textColor = Color.cornflowerBlue, background = texture, },
                        hover = { textColor = Color.dodgerBlue, background = texture },
                        active = { textColor = Color.lightBlue, background = texture },
                        alignment = TextAnchor.MiddleCenter,
                        fixedHeight = 22,
                        fontSize = 14
                    };
                }
                return _centeredComponentStyle;
            }
        }
        private static Texture2D CreateTexture(Color color)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }
    }
}