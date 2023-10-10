using Rpahel.Data;
using UnityEngine;

namespace Rpahel
{
    [CreateAssetMenu(fileName = "Special Attack", menuName = "ScriptableObjects/new Special Attack")]
    public class SpecialAttackSO : ScriptableObject
    {
        //name
        public new string name;
        //Cout en stamina
        public float staminaCost;
        //Degats infliges
        public float inflictedDmg;
        //Degats subis(valeur negative = tu te soignes)
        public float selfDmg;
        //Etat inflige
        public STATE inflictedState;
        //Etat subis
        public STATE selfState;
        //Animation
        public AnimationClip animation;
    }
}
