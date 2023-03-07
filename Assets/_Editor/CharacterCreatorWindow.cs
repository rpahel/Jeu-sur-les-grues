using UnityEditor;
using UnityEngine;

public class CharacterCreatorWindow : EditorWindow
{
    private Vector2 _characterScrollPosition, _statsScrollPosition, _comboScrollPosition;

    //=========================================================

    [MenuItem("Tools/Character Creator")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(CharacterCreatorWindow));
    }

    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        {
            GUILayout.BeginVertical(GUILayout.MinWidth(200), GUILayout.MaxWidth(200), GUILayout.ExpandWidth(false));
            {
                _characterScrollPosition = GUILayout.BeginScrollView(_characterScrollPosition);
                {
                    GUILayout.Label("Characters", GUILayout.ExpandWidth(false));
                    for (int i = 0; i < 10; i++)
                    {
                        GUILayout.Button("Character" + i.ToString("00"));
                    }
                }
                GUILayout.EndScrollView();

                _statsScrollPosition = GUILayout.BeginScrollView(_statsScrollPosition);
                {
                    GUILayout.Label("Stats", GUILayout.ExpandWidth(false));
                    for(int i = 0; i < 10; i++)
                    {
                        GUILayout.Label("Stat" + i.ToString("00"));
                    }
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndVertical();

            _comboScrollPosition = GUILayout.BeginScrollView(_comboScrollPosition, GUILayout.Width(position.width - 200));
            {
                GUILayout.Label("Combo Tree", GUILayout.ExpandWidth(false));
                GUILayout.BeginHorizontal();
                {
                    for (int i = 0; i < 5; i++)
                    {
                        GUILayout.BeginVertical();
                        {
                            GUILayout.Label("Input " + (i+1).ToString("00"));
                            for (int j = 0; j < i + 1; j++)
                            {
                                GUILayout.FlexibleSpace();
                                GUILayout.Button("COMBO_" + i.ToString("00") + "_" + j.ToString("00"));
                                GUILayout.FlexibleSpace();
                            }
                        }
                        GUILayout.EndVertical();
                        GUILayout.FlexibleSpace();
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
        }
        GUILayout.EndHorizontal();
    }
}

// Draw a line on GUI
//Handles.BeginGUI();
//{
//    Handles.DrawLine(
//    new Vector3(0, 0),
//    new Vector3(position.width, position.height), 2);
//}
//Handles.EndGUI();

// Get position of button
//Rect lastRect;
//GUILayout.BeginHorizontal();
//{
//    if (GUILayout.Button("Button")) {/*...*/}
//    lastRect = GUILayoutUtility.GetLastRect();    //button rect    
//}
//GUILayout.EndHorizontal();
//lastRect = GUILayoutUtility.GetLastRect();    //horizontal area rect