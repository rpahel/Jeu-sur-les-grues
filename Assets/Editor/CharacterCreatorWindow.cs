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
        private const string fightersSOpath = "Assets/Resources/ScriptableObjects/FIGHTERS";
        private static string[] subFolders;
        private static string[] fighterNames;
        private string newFighterName;
        private static FightersSO fightersList;

        private ComboData selectedCombo;
        private int[] combosNbPerInput; // Number of combos for each input
        private ComboData[][] combosPerInput; // Combos for each input
        private int comboDepth;
        private ACTIONINPUT[] availableNextMoves;
        private bool hasClickedDelete;

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
            Save(selectedFighter);
        }

        public void OnLostFocus()
        {
            Save(selectedFighter);
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
            GUILayout.BeginHorizontal();
            {
                GUILayout.BeginVertical(GUILayout.MinWidth(250), GUILayout.MaxWidth(250), GUILayout.ExpandWidth(false));
                {
                    CharactersWindow();

                    if (selectedCombo == null)
                        StatsWindow();
                    else
                        ComboDataWindow();
                }
                GUILayout.EndVertical();

                ComboTreeWindow();
            }
            GUILayout.EndHorizontal();
        }


        private static string GetFighterNameFromFolder(string folderPath)
        {
            // Retourne le string APRES le premier underscore
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

        private void Save<T>(T objectToSave) where T : ScriptableObject
        {
            if (objectToSave != null)
            {
                EditorUtility.SetDirty(objectToSave);
                AssetDatabase.SaveAssetIfDirty(objectToSave);
            }
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

        private void LoadFighterComboData()
        {
            fighterComboData = selectedFighter.comboData;
            comboDepth = fighterComboData.GetComboDepth();
            combosNbPerInput = new int[comboDepth];
            combosPerInput = new ComboData[comboDepth][];
            for (int i = 1; i <= comboDepth; i++)
            {
                combosNbPerInput[i - 1] = fighterComboData.GetNbOfCombosAtDepth(i);

                //Debug.Log("Combos at depth " + i + ": " + combosNbPerInput[i - 1]);

                if (i == 1)
                    combosPerInput[i - 1] = new ComboData[1] { fighterComboData };
                else
                    combosPerInput[i - 1] = fighterComboData.GetAllCombosAtDepth(i);
            }
        }

        private void CharacterSelectButtons()
        {
            foreach (string folder in subFolders)
            {
                string fighterName = GetFighterNameFromFolder(folder);
                if (GUILayout.Button(fighterName))
                {
                    Save(selectedFighter);
                    selectedCombo = null;
                    selectedFighter = Resources.Load<FighterSO>("ScriptableObjects/FIGHTERS" + "/FIGHTER_" + fighterName + "/FIGHTER_" + fighterName);
                    LoadFighterComboData();
                }
            }
        }

        private void CreateNewCharacter()
        {
            // Creation d'un nouveau FIGHTER S.O. et sa SPEATK
            selectedFighter = ScriptableObject.CreateInstance<FighterSO>();
            SpecialAttackSO speAtk = ScriptableObject.CreateInstance<SpecialAttackSO>();
            selectedFighter.specialAttack = speAtk;
            selectedFighter.name = newFighterName;
            newFighterName = newFighterName.ToUpper().Replace(" ", "").Replace("_", "");

            // Creation d'un nouveau COMBODATA pour le nouveau FIGHTER
            selectedFighter.comboData = new ComboData(true);
            LoadFighterComboData();

            // Creation d'un dossier pour le nouveau FIGHTER
            string path = AssetDatabase.CreateFolder(fightersSOpath, "FIGHTER_" + newFighterName);
            path = AssetDatabase.GUIDToAssetPath(path);
            AssetDatabase.CreateAsset(selectedFighter, path + "/FIGHTER_" + newFighterName + ".asset");
            AssetDatabase.CreateAsset(speAtk, path + "/SPEATK_" + newFighterName + ".asset");
            selectedFighter.filePath = path;

            // Update Directories
            subFolders = Directory.GetDirectories(fightersSOpath, "FIGHTER_*", SearchOption.TopDirectoryOnly);
            fighterNames = UpdateFighterNames();

            // Clear name field
            newFighterName = "";

            // Ajouter a la liste des FIGHTERS
            fightersList.fighters.Add(selectedFighter);

            Save(selectedFighter);

            Save(fightersList);
        }
        
        private void CreateCharacterButton()
        {
            newFighterName = EditorGUILayout.TextField(newFighterName);
            if (GUILayout.Button("Create Fighter"))
            {
                if (newFighterName is null or "" or " ")
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
                        Save(selectedFighter);
                        selectedCombo = null;
                        CreateNewCharacter();
                    }
                }
            }
        }

        private void CharactersWindow()
        {
            _characterScrollPosition = GUILayout.BeginScrollView(_characterScrollPosition);
            {
                GUILayout.Label("Characters", GUILayout.ExpandWidth(false));
                CharacterSelectButtons();
                CreateCharacterButton();
            }
            GUILayout.EndScrollView();
        }

        private void StatsWindow()
        {
            // Styles
            GUIStyle textAreaStyle = new GUIStyle(EditorStyles.textArea);
            textAreaStyle.wordWrap = true;
            GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
            labelStyle.wordWrap = true;

            // Stats section
            _statsScrollPosition = GUILayout.BeginScrollView(_statsScrollPosition);
            {
                GUILayout.Label("Stats" + (selectedFighter == null ? "" : (" - " + selectedFighter.name)), labelStyle);
                if (selectedFighter != null)
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

                    selectedFighter.maxHealth = EditorGUILayout.IntField("Max Health: ", selectedFighter.maxHealth);
                    selectedFighter.maxStamina = EditorGUILayout.IntField("Max Stamina: ", selectedFighter.maxStamina);
                    selectedFighter.maxSpecialMeter = EditorGUILayout.FloatField("Max Special Meter: ", selectedFighter.maxSpecialMeter);

                    selectedFighter.specialAttack = (SpecialAttackSO)EditorGUILayout.ObjectField("Special Attack: ", selectedFighter.specialAttack, typeof(SpecialAttackSO), false);
                    GUILayout.Label("Path: ", GUILayout.ExpandWidth(false));
                    GUILayout.Label(selectedFighter.filePath, labelStyle);
                }
            }
            GUILayout.EndScrollView();
        }

        private void ShowAvailableNextMovesButtons()
        {
            for(int i = 0; i < 3; i++)
            {
                if (availableNextMoves == null) break;

                if (availableNextMoves[i] != ACTIONINPUT.DODGE && GUILayout.Button(((ACTIONINPUT)i).ToString()))
                {
                    selectedCombo.CreateNextMove((ACTIONINPUT)i);
                    LoadFighterComboData();
                    Save(selectedFighter);
                    availableNextMoves = selectedCombo.AvailableNextMoves();
                }
            }
        }

        private void ComboDataWindow()
        {
            // Styles
            GUIStyle textAreaStyle = new GUIStyle(EditorStyles.textArea);
            textAreaStyle.wordWrap = true;
            GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
            labelStyle.wordWrap = true;

            // Stats section
            _statsScrollPosition = GUILayout.BeginScrollView(_statsScrollPosition);
            {
                GUILayout.Label(selectedFighter.name + ": Combo " + selectedCombo.name, labelStyle);

                selectedCombo.name = EditorGUILayout.TextField("Name: ", selectedCombo.name);
                GUILayout.Label("Attack Input: " + selectedCombo.actionInput.ToString());
                selectedCombo.damage = EditorGUILayout.IntField("Damage: ", selectedCombo.damage);
                selectedCombo.inflictedState = (STATE)EditorGUILayout.EnumPopup("Inflicted State: ", selectedCombo.inflictedState);
                selectedCombo.animation = (AnimationClip)EditorGUILayout.ObjectField("Animation Clip: ", selectedCombo.animation, typeof(AnimationClip), false);
                
                if (availableNextMoves != null)
                {
                    EditorGUILayout.Space(20);
                    GUILayout.Label("Create next move.");
                    ShowAvailableNextMovesButtons();
                }

                EditorGUILayout.Space(20);

                if (selectedCombo.inputNb > 2)
                {
                    if (!hasClickedDelete)
                    {
                        if(GUILayout.Button("Delete Combo"))
                            hasClickedDelete = true;
                    }
                    else
                    {
                        GUILayout.Label("Are you sure ?");

                        if (GUILayout.Button("NO, DON'T DELETE"))
                            hasClickedDelete = false;

                        if (GUILayout.Button("YES, DELETE THE COMBO MOVE"))
                        {
                            hasClickedDelete = false;
                            selectedCombo.DeleteMove();
                            selectedCombo = null;
                            LoadFighterComboData();
                            Save(selectedFighter);
                        }
                    }
                }
            }
            GUILayout.EndScrollView();
        }

        private void ComboTreeWindow()
        {
            _comboScrollPosition = GUILayout.BeginScrollView(_comboScrollPosition, GUILayout.Width(position.width - 250));
            {
                GUILayout.Label("Combo Tree", GUILayout.ExpandWidth(false));
                GUILayout.BeginHorizontal();
                {
                    for (int i = 0; i < comboDepth; i++)
                    {
                        GUILayout.BeginVertical();
                        {
                            GUILayout.Label("Input " + (i + 1).ToString("00"));
                            for (int j = 0; j < combosNbPerInput[i]; j++)
                            {
                                GUILayout.FlexibleSpace();
                                ComboData currentCombo = combosPerInput[i][j];

                                if (GUILayout.Button(currentCombo.name))
                                {
                                    selectedCombo = currentCombo;
                                    availableNextMoves = selectedCombo.AvailableNextMoves();
                                }
                                                                
                                Rect lastRect = GUILayoutUtility.GetLastRect();
                                currentCombo.PositionOnGui = lastRect.center - 0.5f * lastRect.width * Vector2.right;
                                currentCombo.SetPositionOnGuiOnChildren(lastRect.center + 0.5f * lastRect.width * Vector2.right);

                                if(currentCombo.ParentPositionOnGui != Vector2.zero)
                                {
                                    Handles.DrawLine(
                                        currentCombo.ParentPositionOnGui,
                                        currentCombo.PositionOnGui,
                                        0);
                                }

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
    }
}