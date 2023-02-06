using UnityEngine;

namespace GrujutsuData
{
    [System.Serializable]
    public enum AttackInput
    {
        RIGHT = 0,
        DOWN = 1,
        LEFT = 2,
        UP = 3
    }

    [System.Serializable]
    public enum State
    {
        NONE = 0,
        BLEEDING = 1,
        BURNING = 2,
        POISONED = 3,
        SLEEPING = 4,
        PARALYZED = 5,
    }

    [System.Serializable]
    public class ComboData
    {
        [HideInInspector]
        public int inputNb; // Starts at 1 for first input
        public string name;
        public AttackInput attackInput;
        public AnimationClip animation;
        public bool isFinalMove;
        public ComboData previousMove;
        public ComboData[] nextMoves = new ComboData[3];

        public ComboData()
        {
            inputNb = 1;
        }

        public ComboData(ComboData _previousMove)
        {
            previousMove = _previousMove;
            inputNb = _previousMove.inputNb + 1;
            name = _previousMove.name + "\'s next move";
        }
    }

    [System.Serializable]
    public struct SpecialAttackData
    {
        //name
        //Cout en stamina
        //Degats infligés
        //Degats subis(valeur négative = tu te soignes)
        //Etat infligé
        //Etat subis
    }
}
