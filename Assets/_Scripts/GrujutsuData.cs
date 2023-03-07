using UnityEngine;

namespace GrujutsuData
{
    [System.Serializable]
    public enum ATTACKINPUT
    {
        RIGHT = 0,
        DOWN = 1,
        LEFT = 2,
        UP = 3
    }

    [System.Serializable]
    public enum STATE
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
        public ATTACKINPUT attackInput;
        public AnimationClip animation;
        public bool isFinalMove;
        public ComboData previousMove;
        public ComboData[] nextMoves;
        public Vector2 PositionOnGui { get; set; }

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

        //===================================================

        public ComboData CreateNextMove()
        {
            ComboData newCombo = new(this);

            if(nextMoves != null && nextMoves.Length > 0)
            {
                ComboData[] newNextMoves = new ComboData[nextMoves.Length + 1];
                for(int i = 0; i < nextMoves.Length; i++)
                    newNextMoves[i] = nextMoves[i];

                newNextMoves[nextMoves.Length] = newCombo;
                nextMoves = newNextMoves;
            }
            else
                nextMoves = new ComboData[1] { newCombo };

            return newCombo;
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

    [System.Serializable]
    public struct StateData
    {
        public STATE state;
        public Color filterColor;
        public AnimationClip stateAnimation;
        public float healthToAdd; // Amount of Health to add every loseCooldown period.
        public float healthUpdateCooldown; // Period of time between each health update
        public float staminaToAdd; // Amount of Stamina to add every loseCooldown period.
        public float staminaUpdateCooldown; // Period of time between each stamina update
    }
}