using Bas.Thoth;
using UnityEngine;
using Color = UnityEngine.Color;

namespace Bas.Debug
{
    public class DebugText : MonoBehaviour
    {
        private Entities _entities;

        private void Awake() => _entities = Singletons.Get<Entities>();

        private void OnGUI()
        {
            Color color = GUI.color;
            float fps = 1 / Time.smoothDeltaTime;
            DrawText(new Rect(10, 10, 800, 800), $"{fps:#.##} FPS", fps > 59 ? Color.green : Color.red, 80);
            DrawText(new Rect(10, 100, 800, 800), $"{_entities.Count} Entities", Color.white, 80);

            GUI.color = color;
        }

        private void DrawText(Rect rect, string text, Color color, int fontSize)
        {
            GUI.skin.label.fontSize = fontSize;
            GUI.color = color;
            GUI.Label(rect, text);
        }
    }
}