using UnityEngine;
using UnityEngine.InputSystem;
using Transform = Bas.Thoth.Models.Transform;

namespace Bas.Thoth.Topdown
{
    public class CameraComponent : Component
    {
        public static Vector3 CameraSize => new Vector3(_camera.orthographicSize, _camera.orthographicSize);

        private UnityEngine.Transform _cameraTransform;
        private static Camera _camera;

        private float _zValue;

        private float zoom = 1;

        private TransformComponent _transformComponent;

        public override void Init()
        {
            _transformComponent = Singletons.Get<TransformComponent>();
            _camera = Object.FindAnyObjectByType<Camera>();
            _cameraTransform = _camera.transform;
            _zValue = _cameraTransform.position.z;
        }

        public override void UpdateEntity(int id, float deltaTime)
        {
            Transform transform = _transformComponent[id];
            _cameraTransform.position = new Vector3(transform.x, transform.y, _zValue);

            zoom = Mathf.Clamp(zoom - Mouse.current.scroll.value.y / 100f, 0.2f, 1);

            float height = World.CameraMaxWidth / _camera.aspect;
            _camera.orthographicSize = height / 2 * zoom;
        }
    }
}
