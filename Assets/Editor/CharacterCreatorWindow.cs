using Rpahel;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Rpahel
{
    // TODO : Add newly created Scriptable Object Fighter to the list of fighters
    public class CharacterCreatorWindow : EditorWindow
    {
        private Vector2 _characterScrollPosition, _statsScrollPosition, _comboScrollPosition;
        private FighterSO selectedFighter;
        private const string fightersSOpath = "Assets/Resources/ScriptableObjects/FIGHTERS";
        private static string[] subFolders;
        private static string[] fighterNames;
        private string newFighterName;

        //=========================================================

        [MenuItem("Tools/Character Creator")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(CharacterCreatorWindow));
            subFolders = Directory.GetDirectories(fightersSOpath, "FIGHTER_*", SearchOption.TopDirectoryOnly);
            fighterNames = UpdateFighterNames();
        }

        public void OnDestroy()
        {
            if (selectedFighter)
            {
                EditorUtility.SetDirty(selectedFighter);
                AssetDatabase.SaveAssetIfDirty(selectedFighter);
            }
        }

        public void OnLostFocus()
        {
            if (selectedFighter)
            {
                EditorUtility.SetDirty(selectedFighter);
                AssetDatabase.SaveAssetIfDirty(selectedFighter);
            }
        }

        private void OnGUI()
        {
            // Styles
            GUIStyle textAreaStyle = new GUIStyle(EditorStyles.textArea);
            textAreaStyle.wordWrap = true;
            GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
            labelStyle.wordWrap = true;
            //labelStyle.stretchWidth = false;

            // Layout
            GUILayout.BeginHorizontal();
            {
                GUILayout.BeginVertical(GUILayout.MinWidth(300), GUILayout.MaxWidth(300), GUILayout.ExpandWidth(false));
                {
                    // Characters section
                    _characterScrollPosition = GUILayout.BeginScrollView(_characterScrollPosition);
                    {
                        GUILayout.Label("Characters", GUILayout.ExpandWidth(false));
                        foreach (string folder in subFolders)
                        {
                            string fighterName = GetFighterNameFromFolder(folder);
                            if (GUILayout.Button(fighterName))
                            {
                                if (selectedFighter)
                                {
                                    EditorUtility.SetDirty(selectedFighter);
                                    AssetDatabase.SaveAssetIfDirty(selectedFighter);
                                }

                                selectedFighter = Resources.Load<FighterSO>("ScriptableObjects/FIGHTERS" + "/FIGHTER_" + fighterName + "/FIGHTER_" + fighterName);
                            }
                        }

                        newFighterName = EditorGUILayout.TextField(newFighterName);
                        if (GUILayout.Button("Create Fighter"))
                        {
                            if(newFighterName is null or "" or " ")
                            {
                                Debug.LogError("Please input a fighter name.");
                            }
                            else
                            {
                                if (IsNameUsed(newFighterName))
                                {
                                    Debug.LogError(newFighterName + " already exists.");
                                }
                                else
                                {
                                    // Creation d'un nouveau FIGHTER S.O. et sa SPEATK
                                    selectedFighter = ScriptableObject.CreateInstance<FighterSO>();
                                    SpecialAttackSO speAtk = ScriptableObject.CreateInstance<SpecialAttackSO>();

                                    selectedFighter.specialAttack = speAtk;
                                    selectedFighter.name = newFighterName;
                                    newFighterName = newFighterName.ToUpper().Replace(" ", "").Replace("_", "");

                                    string path = AssetDatabase.CreateFolder(fightersSOpath, "FIGHTER_" + newFighterName);
                                    path = AssetDatabase.GUIDToAssetPath(path);
                                    selectedFighter.filePath = path;

                                    AssetDatabase.CreateAsset(selectedFighter, path + "/FIGHTER_" + newFighterName + ".asset");
                                    AssetDatabase.CreateAsset(speAtk, path + "/SPEATK_" + newFighterName + ".asset");

                                    // Update Directories
                                    subFolders = Directory.GetDirectories(fightersSOpath, "FIGHTER_*", SearchOption.TopDirectoryOnly);
                                    fighterNames = UpdateFighterNames();

                                    // Clear name field
                                    newFighterName = "";

                                    foreach (var fighterName in fighterNames)
                                    {
                                        Debug.Log(fighterName);
                                    }
                                }
                            }
                        }
                    }
                    GUILayout.EndScrollView();

                    // Stats section
                    _statsScrollPosition = GUILayout.BeginScrollView(_statsScrollPosition);
                    {
                        GUILayout.Label("Stats" + (selectedFighter == null ? "" : (" - " + selectedFighter.name)), labelStyle);
                        if(selectedFighter != null)
                        {
                            selectedFighter.name = EditorGUILayout.TextField("Name: ", selectedFighter.name);
                            selectedFighter.icon = (Sprite)EditorGUILayout.ObjectField("Icon: ", selectedFighter.icon, typeof(Sprite), false);
                            selectedFighter.fighterPrefab = (GameObject)EditorGUILayout.ObjectField("Prefab: ", selectedFighter.fighterPrefab, typeof(GameObject), false);
                            selectedFighter.color = EditorGUILayout.ColorField("Color: ", selectedFighter.color);
                            selectedFighter.species = EditorGUILayout.TextField("Type: ", selectedFighter.species);

                            GUILayout.Label("Description: ", GUILayout.ExpandWidth(false));
                            selectedFighter.description = EditorGUILayout.TextArea(selectedFighter.description, textAreaStyle);
                            GUILayout.Label("Start Dialogue: ", GUILayout.ExpandWidth(false));
                            selectedFighter.startDialogue = EditorGUILayout.TextArea(selectedFighter.startDialogue, textAreaStyle);
                            GUILayout.Label("End Dialogue: ", GUILayout.ExpandWidth(false));
                            selectedFighter.endDialogue = EditorGUILayout.TextArea(selectedFighter.endDialogue, textAreaStyle);

                            selectedFighter.maxHealth = EditorGUILayout.FloatField("Max Health: ", selectedFighter.maxHealth);
                            selectedFighter.maxStamina = EditorGUILayout.FloatField("Max Stamina: ", selectedFighter.maxStamina);
                            selectedFighter.maxSpecialMeter = EditorGUILayout.FloatField("Max Special Meter: ", selectedFighter.maxSpecialMeter);

                            selectedFighter.specialAttack = (SpecialAttackSO)EditorGUILayout.ObjectField("Special Attack: ", selectedFighter.specialAttack, typeof(SpecialAttackSO), false);
                            GUILayout.Label("Path: ", GUILayout.ExpandWidth(false));
                            GUILayout.Label(selectedFighter.filePath, labelStyle);
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
                                GUILayout.Label("Input " + (i + 1).ToString("00"));
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

        private static string GetFighterNameFromFolder(string folderPath)
        {
            return folderPath.Split('_')[1];
        }

        private static string[] UpdateFighterNames()
        {
            string[] fighterNames = new string[subFolders.Length];
            for(int i = 0; i < fighterNames.Length; i++)
            {
                fighterNames[i] = GetFighterNameFromFolder(subFolders[i]);
            }

            return fighterNames;
        }

        private bool IsNameUsed(string name)
        {
            name = name.ToUpper().Replace(" ", "").Replace("_", "");
            foreach (var fighterName in fighterNames)
            {
                if (name == fighterName)
                    return true;
            }

            return false;
        }
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