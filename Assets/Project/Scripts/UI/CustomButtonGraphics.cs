using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BigProject.UI
{

#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.UI;

    [CustomEditor(typeof(CustomButtonGraphics), true)]
    public class CustomLevelButtonEditor : ButtonEditor
    {
        private SerializedProperty normalButtonColor;
        private SerializedProperty highlightedButtonColor;
        private SerializedProperty pressedButtonColor;
        private SerializedProperty selectedButtonColor;
        private SerializedProperty disabledButtonColor;
        private SerializedProperty edgesOfButton;
        private SerializedProperty backgroundImage;

        protected override void OnEnable()
        {
            base.OnEnable();
            normalButtonColor = serializedObject.FindProperty("_normalButtonColor");
            highlightedButtonColor = serializedObject.FindProperty("_highlightedButtonColor");
            pressedButtonColor = serializedObject.FindProperty("_pressedButtonColor");
            selectedButtonColor = serializedObject.FindProperty("_selectedButtonColor");
            disabledButtonColor = serializedObject.FindProperty("_disabledButtonColor");

            edgesOfButton = serializedObject.FindProperty("_edgesOfButton");
            backgroundImage = serializedObject.FindProperty("_backgroundImage");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.PropertyField(normalButtonColor);
            EditorGUILayout.PropertyField(highlightedButtonColor);
            EditorGUILayout.PropertyField(pressedButtonColor);
            EditorGUILayout.PropertyField(selectedButtonColor);
            EditorGUILayout.PropertyField(disabledButtonColor);

            EditorGUILayout.PropertyField(edgesOfButton);
            EditorGUILayout.PropertyField(backgroundImage);
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif


    public class CustomButtonGraphics : Button
    {
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private List<Image> _edgesOfButton;

        [Header("Настройки цвета фона кнопки")]
        [SerializeField] private Color _normalButtonColor;
        [SerializeField] private Color _highlightedButtonColor;
        [SerializeField] private Color _pressedButtonColor;
        [SerializeField] private Color _selectedButtonColor;
        [SerializeField] private Color _disabledButtonColor;

        private float _transitionDuration;
        protected override void Awake()
        {
            base.Awake();
            _transitionDuration = 0.1f;
        }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);

            switch (state)
            {
                case SelectionState.Normal:
                    StartCoroutine(AnimateButton(_normalButtonColor, false));
                    break;
                case SelectionState.Highlighted:
                    StartCoroutine(AnimateButton(_highlightedButtonColor, true));
                    break;
                case SelectionState.Pressed:
                    StartCoroutine(AnimateButton(_pressedButtonColor, true));
                    break;
                case SelectionState.Selected:
                    StartCoroutine(AnimateButton(_selectedButtonColor, true));
                    break;
                case SelectionState.Disabled:
                    StartCoroutine(AnimateButton(_disabledButtonColor, false));
                    break;
            }
        }

        private IEnumerator AnimateButton(Color targetBackgroundColor, bool shouldShowEdges)
        {
            float timer = 0;
            Color startBackgroundColor = _backgroundImage.color;
            Color startEdgeColor = new Color(0, 0, 0, 0);

            if (_edgesOfButton.Count > 0)
                startEdgeColor = _edgesOfButton[0].color;

            Color targetEdgeColor = shouldShowEdges ? new Color(1, 1, 1, 1) : new Color(1, 1, 1, 0);

            while (timer < _transitionDuration)
            {
                timer += Time.deltaTime;
                _backgroundImage.color = Color.Lerp(startBackgroundColor, targetBackgroundColor, timer / _transitionDuration);

                foreach (var edge in _edgesOfButton)
                    edge.color = Color.Lerp(startEdgeColor, targetEdgeColor, timer / _transitionDuration);

                yield return null;
            }

            _backgroundImage.color = targetBackgroundColor;

            foreach (var edge in _edgesOfButton)
                edge.color = targetEdgeColor;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            foreach (var edge in _edgesOfButton)
                edge.color = new Color(1, 1, 1, 0);

            _backgroundImage.color = new Color(1, 1, 1, 0);
        }
    }
}