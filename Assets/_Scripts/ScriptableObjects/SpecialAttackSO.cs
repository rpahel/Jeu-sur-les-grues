using Data;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Special Attack", menuName = "ScriptableObjects/new Special Attack")]
public class SpecialAttackSO : ScriptableObject
{
    //name
    public new string name;
    //Cout en stamina
    public float staminaCost;
    //Degats infligés
    public float inflictedDmg;
    //Degats subis(valeur négative = tu te soignes)
    public float selfDmg;
    //Etat infligé
    public STATE inflictedState;
    //Etat subis
    public STATE selfState;
    //Animation
    public AnimationClip animation;
}
