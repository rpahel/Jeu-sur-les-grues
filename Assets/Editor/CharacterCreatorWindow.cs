using Rpahel;
using Rpahel.Data;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Rpahel
{
    public class CharacterCreatorWindow : EditorWindow
    {
        private Vector2 _characterScrollPosition, _statsScrollPosition, _comboScrollPosition;
        private FighterSO selectedFighter;
        private ComboData fighterComboData;
        private ComboData selectedCombo;
        private int[] combosNbPerInput; // Number of combos for each input
        private ComboData[][] combosPerInput; // Combos for each input
        private int comboDepth;
        private const string fightersSOpath = "Assets/Resources/ScriptableObjects/FIGHTERS";
        private static string[] subFolders;
        private static string[] fighterNames;
        private string newFighterName;
        private static FightersSO fightersList;

        //=========================================================

        [MenuItem("Tools/Character Creator")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(CharacterCreatorWindow));

            // Dossiers
            subFolders = Directory.GetDirectories(fightersSOpath, "FIGHTER_*", SearchOption.TopDirectoryOnly);
            fighterNames = UpdateFighterNames();

            // Liste des fighters
            fightersList = (FightersSO)Resources.Load("ScriptableObjects/FIGHTERS/FIGHTERS");
            for (int i = fightersList.fighters.Count - 1; i >= 0; i--)
                if (fightersList.fighters[i] == null)
                    fightersList.fighters.RemoveAt(i);
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

        public void OnFocus()
        {
            // Dossiers
            subFolders = Directory.GetDirectories(fightersSOpath, "FIGHTER_*", SearchOption.TopDirectoryOnly);
            fighterNames = UpdateFighterNames();

            // Liste des fighters
            fightersList = (FightersSO)Resources.Load("ScriptableObjects/FIGHTERS/FIGHTERS");
            for (int i = fightersList.fighters.Count - 1; i >= 0; i--)
                if (fightersList.fighters[i] == null)
                    fightersList.fighters.RemoveAt(i);
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
                GUILayout.BeginVertical(GUILayout.MinWidth(250), GUILayout.MaxWidth(250), GUILayout.ExpandWidth(false));
                {
                    #region Characters section
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
                                
                                fighterComboData = selectedFighter.comboData;
                                comboDepth = fighterComboData.GetComboDepth();
                                combosNbPerInput = new int[comboDepth];
                                combosPerInput = new ComboData[comboDepth][];
                                for (int i = 1; i <= comboDepth; i++)
                                {
                                    combosNbPerInput[i - 1] = fighterComboData.GetNbOfCombosAtDepth(i);
                                    if (i == 1)
                                        combosPerInput[i - 1] = new ComboData[1] { fighterComboData };
                                    else
                                        combosPerInput[i - 1] = fighterComboData.GetAllCombosAtDepth(i);
                                }
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
                                    
                                    selectedFighter.comboData = new ComboData(true);
                                    fighterComboData = selectedFighter.comboData;
                                    comboDepth = selectedFighter.comboData.GetComboDepth();
                                    combosNbPerInput = new int[comboDepth];
                                    combosPerInput = new ComboData[comboDepth][];
                                    for (int i = 1; i <= comboDepth; i++)
                                    {
                                        combosNbPerInput[i - 1] = fighterComboData.GetNbOfCombosAtDepth(i);
                                        if(i == 1)
                                            combosPerInput[i - 1] = new ComboData[1] { fighterComboData };
                                        else
                                            combosPerInput[i - 1] = fighterComboData.GetAllCombosAtDepth(i);
                                    }

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

                                    fightersList.fighters.Add(selectedFighter);

                                    EditorUtility.SetDirty(selectedFighter);
                                    AssetDatabase.SaveAssetIfDirty(selectedFighter);

                                    EditorUtility.SetDirty(fightersList);
                                    AssetDatabase.SaveAssetIfDirty(fightersList);

                                }
                            }
                        }
                    }
                    GUILayout.EndScrollView();
                    #endregion

                    #region Stats section
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

                            GUILayout.Label("Type: ", GUILayout.ExpandWidth(false));
                            selectedFighter.species = EditorGUILayout.TextField(selectedFighter.species);
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
                    #endregion
                }
                GUILayout.EndVertical();

                _comboScrollPosition = GUILayout.BeginScrollView(_comboScrollPosition, GUILayout.Width(position.width - 250));
                {
                    GUILayout.Label("Combo Tree", GUILayout.ExpandWidth(false));
                    GUILayout.BeginHorizontal();
                    {
                        for (int i = 0; i < comboDepth; i++)
                        {
                            GUILayout.BeginVertical();
                            {
                                GUILayout.Label("Input " + (i+1).ToString("00"));
                                for (int j = 0; j < combosNbPerInput[i]; j++)
                                {
                                    GUILayout.FlexibleSpace();

                                    if (GUILayout.Button(combosPerInput[i][j].name))
                                        selectedCombo = combosPerInput[i][j];

                                    // TODO : Connect Button to previous button

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