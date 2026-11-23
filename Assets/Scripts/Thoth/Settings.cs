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
        public static bool DrawGrid => Instance._drawGrid;
        public static int FPS => Instance._fps;
        public static bool BalanceEntitiesPerArea => Instance._balanceEntitiesPerArea;
        public static int EntitiesPerArea => Instance._entitiesPerArea;


        [SerializeField] private bool _drawColliders;
        [SerializeField] private bool _drawAABB;
        [SerializeField] private bool _drawGrid;
        [SerializeField] private int _fps = 60;
        [SerializeField] private bool _balanceEntitiesPerArea = true;
        [SerializeField] private int _entitiesPerArea = 10;
    }
}
