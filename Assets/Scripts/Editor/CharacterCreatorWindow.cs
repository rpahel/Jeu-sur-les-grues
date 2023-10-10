using Rpahel.Data;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Rpahel
{
    public class CharacterCreatorWindow : EditorWindow
    {
        [MenuItem("Tools/Character Creator")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(CharacterCreatorWindow));
        }
    }
}