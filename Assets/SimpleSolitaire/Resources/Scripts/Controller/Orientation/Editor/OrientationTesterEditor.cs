using UnityEditor;
using UnityEngine;

namespace SimpleSolitaire.Controller.Editor
{
    [CustomEditor(typeof(OrientationTester))]
    public class OrientationTesterEditor : UnityEditor.Editor
    {
        public static OrientationTester Target;

        private void OnEnable()
        {
            if (Target == null) Target = target as OrientationTester;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            DrawCustomInspector();
        }

        private void DrawCustomInspector()
        {
            EditorGUILayout.Space();

            if (Application.isPlaying)
            {
                if (GUILayout.Button("Set orientation"))
                {
                    Target.SetOrientation();
                }
            }
            else
            {
                if (GUILayout.Button("Set orientation (Required save)"))
                {
                    Target.SetOrientationInEditor();
                }
                
                if (GUILayout.Button("Save current layout to container"))
                {
                    Target.SaveValuesToContainer();
                    EditorUtility.SetDirty(Target.Logic.OrientationContainer);

                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
        }
    }
}