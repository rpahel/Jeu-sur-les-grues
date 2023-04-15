using Data;
using UnityEngine;

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
    //Type(court texte ex: "Grue Ninja")
    public string species;
    //Description
    public string description;
    //StartFightDialogue
    public string startDialogue;
    //EndFightDialogue
    public string endDialogue;

    //VieMax
    public float maxHealth;
    //StaminaMax
    public float maxStamina;
    //SpecialMeterMax
    public float maxSpecialMeter;

    //ComboData
    public ComboData comboData;
    //Special Attack
    public SpecialAttackSO specialAttack;
}
