using UnityEngine;

namespace Bas.Thoth
{
    [CreateAssetMenu(fileName = "Settings", menuName = "Thoth/Settings", order = 1)]
    public class Settings : ScriptableObject
    {
        private static Settings Instance => _instance == null ? (_instance = Resources.Load<Settings>(Application.isEditor ? "EditorSettings" : "Settings")) : _instance;
        private static Settings _instance;
        public static bool DrawColliders => Instance._drawColliders;
        public static bool DrawAABB => Instance._drawAABB;
        public static int FPS => Instance._fps;

        [SerializeField] private bool _drawColliders;
        [SerializeField] private bool _drawAABB;
        [SerializeField] private int _fps;
    }
}
