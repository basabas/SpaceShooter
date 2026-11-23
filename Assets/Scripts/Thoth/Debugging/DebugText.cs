using Bas.Thoth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using Color = UnityEngine.Color;

namespace Bas.Debug
{
    public class DebugText : MonoBehaviour
    {
        private static List<Type> ExcludedTypes = new List<Type>
            {
                typeof(Thoth.Graphics),
                typeof(TransformComponent),
                typeof(Thoth.Physics)
            };

        private bool _showFrameData = true;
        private bool _showEntities;

        private Entities _entities;
        private Components _components;

        private void Awake()
        {
            _entities = Singletons.Get<Entities>();
            _components = Singletons.Get<Components>();
        }

        private void OnGUI()
        {
            if (Event.current.type == EventType.Repaint)
            {
                //Toggle when F1 is pressed
                _showFrameData ^= Keyboard.current.f1Key.wasPressedThisFrame;
                _showEntities ^= Keyboard.current.f2Key.wasPressedThisFrame;

                if (_showFrameData)
                {
                    ShowFrameData();
                }

                if (_showEntities)
                {
                    ShowEntities();
                }
            }
        }

        private void ShowFrameData()
        {
            Color color = GUI.color;
            float fps = 1 / Time.smoothDeltaTime;
            DrawText(new Rect(10, 10, 800, 800), $"{fps:#.##} FPS", fps > 59 ? Color.green : Color.red, 80);
            DrawText(new Rect(10, 100, 800, 800), $"{_entities.Count} Entities", Color.white, 80);

            GUI.color = color;
        }

        //TODO: Finish this up and make some UI for it.
        private void ShowEntities()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Entities: {_entities.Count}");
            List<Thoth.Component> components = new List<Thoth.Component>();
            for (int i = 0; i < _entities.Count; i++)
            {
                _components.GetComponents(_entities[i], components);
                var filteredComponents = components.Where(c => !ExcludedTypes.Contains(c.GetType()));
                sb.AppendLine($"Entity {i}: {_entities[i]}: {string.Join(", ", filteredComponents.Select(c => c.GetType().Name))}");
                components.Clear();
            }
            Logging.Log(sb.ToString());
        }

        private void DrawText(Rect rect, string text, Color color, int fontSize)
        {
            GUI.skin.label.fontSize = fontSize;
            GUI.color = color;
            GUI.Label(rect, text);
        }
    }
}