using System;
using UnityEditor;
using UnityEngine;

namespace Extensions
{
    public static class Extensions
    {
        #region GameObjects
        public static void DestroyChildren(this Transform father)
        {
            foreach (Transform child in father) {
                GameObject.Destroy(child.gameObject);
            }
        }

        public static void DestroyParticleObjectOnActionStop(this ParticleSystem ps)
        {
            ParticleSystem.MainModule main = ps.main;
            main.stopAction = ParticleSystemStopAction.Destroy;
        }
        #endregion

        #region Trigonometry
        public static float DirectionAngle(this Vector3 self)
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3 targetPos = Camera.main.WorldToScreenPoint(self);

            mousePos.x -= targetPos.x;
            mousePos.y -= targetPos.y;

            return Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
        }

        public static float DirectionAngle(this Vector3 self, Vector3 target)
        {
            Vector3 dir = target - self;

            return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        }

        public static Vector3 DirectionAngle(this Vector3 target, bool returnVector3)
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3 targetPos = Camera.main.WorldToScreenPoint(target);

            mousePos.x -= targetPos.x;
            mousePos.y -= targetPos.y;

            return mousePos;
        }

        public static Vector3 DirectionAngle(this Vector3 self, Vector3 target, bool returnVector2) => target - self;

        public static Vector3 GetAngleDirection(this float angle)
        {
            return new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Tan(angle * Mathf.Deg2Rad));
        }

        public static Vector3 AngleAxisToVector3(float angle, float rotationDistance) => Quaternion.AngleAxis(angle, Vector3.forward) * (Vector3.right * rotationDistance);
        #endregion
    }
}

namespace Utils
{
    // my version of:
    // https://gist.github.com/aarthificial/f2dbb58e4dbafd0a93713a380b9612af
    [Serializable]
    public struct Optional<T>
    {
        [SerializeField] private bool _enabled;
        [SerializeField] private T _value;

        public bool Enabled => _enabled;
        public T Value => _value;

        public Optional(T initialValue)
        {
            _enabled = true;
            _value = initialValue;
        }
    }

    public struct OptionalNonSerializable<T>
    {
        private bool _enabled;
        private T _value;

        public bool Enabled => _enabled;
        public T Value => _value;

        public OptionalNonSerializable(T initialValue)
        {
            _enabled = true;
            _value = initialValue;
        }
    }

#if UNITY_EDITOR
    namespace Editor
    {
        [CustomPropertyDrawer(typeof(Optional<>))]
        public class OptionalPropertyDrawer : UnityEditor.PropertyDrawer
        {
            public override float GetPropertyHeight(UnityEditor.SerializedProperty property, GUIContent label)
            {
                var valueProperty = property.FindPropertyRelative("_value");

                return EditorGUI.GetPropertyHeight(valueProperty);
            }

            public override void OnGUI(
                Rect position,
                UnityEditor.SerializedProperty property,
                GUIContent label
            )
            {
                var valueProperty = property.FindPropertyRelative("_value");
                var enabledProperty = property.FindPropertyRelative("_enabled");

                EditorGUI.BeginProperty(position, label, property);

                position.width -= 24;

                EditorGUI.BeginDisabledGroup(!enabledProperty.boolValue);
                EditorGUI.PropertyField(position, valueProperty, label, true);
                EditorGUI.EndDisabledGroup();

                int indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;

                position.x += position.width + 24;
                position.width = position.height = EditorGUI.GetPropertyHeight(enabledProperty);
                position.x -= position.width;

                EditorGUI.PropertyField(position, enabledProperty, GUIContent.none);

                EditorGUI.indentLevel = indent;

                EditorGUI.EndProperty();
            }
        }
    }
#endif
}