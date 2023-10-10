using UnityEngine;

namespace Rpahel
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "Fighter", menuName = "ScriptableObjects/new Fighter")]
    public class FighterSO : ScriptableObject
    {
        // Basic Info
        private new string name;
        private Sprite icon;
        private GameObject fighterPrefab;
        private Color color;

        // Lore
        private string species; // Court texte. Ex: "Grue Ninja"
        private string description;
        private string startDialogue;
        private string endDialogue;

        // Stats
        private int maxHealth;
        private int maxStamina;
        private float maxSpecialMeter;

        // Attack
        private SpecialAttackSO specialAttack;

#if UNITY_EDITOR
        private string filePath;
#endif
    }
}