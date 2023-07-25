using Rpahel.Data;
using UnityEngine;

namespace Rpahel
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "Fighter", menuName = "ScriptableObjects/new Fighter")]
    public class FighterSO : ScriptableObject
    {
        //Name
        public new string name;
        //Icone
        public Sprite icon;
        //Game Object
        public GameObject fighterPrefab;
        //Color
        public Color color;
        //Type (court texte ex: "Grue Ninja")
        public string species;
        //Description
        public string description;
        //StartFightDialogue
        public string startDialogue;
        //EndFightDialogue
        public string endDialogue;

        //VieMax
        public int maxHealth;
        //StaminaMax
        public int maxStamina;
        //SpecialMeterMax
        public float maxSpecialMeter;

        //ComboData
        [SerializeField, HideInInspector]
        public ComboData comboData;
        //Special Attack
        public SpecialAttackSO specialAttack;

#if UNITY_EDITOR
        public string filePath;
#endif
    }
}