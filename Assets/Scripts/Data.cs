using Rpahel.Interfaces;
using UnityEngine;

namespace Rpahel.Data
{
    [System.Serializable]
    public enum ACTIONINPUT
    {
        UP = 0,
        ATTACK = 1,
        DOWN = 2,
        DODGE = 3
    }

    [System.Serializable]
    public enum STATE
    {
        DEFAULT = 0,
        BLEEDING = 1,
        BURNING = 2,
        POISONED = 3,
        SLEEPING = 4,
        PARALYZED = 5,
    }

    public class AttackData : ScriptableObject, IGUIRenderable
    {
        [HideInInspector]
        private int inputNb; // Starts at 1 for first input
        private new string name;
        private int damage;
        private ACTIONINPUT actionInput;
        private STATE inflictedState;
        private AnimationClip animation;
        private AttackData[] childAttacks = null; // UP, RIGHT, DOWN

#if UNITY_EDITOR
        public Vector2 PositionOnGui { get; set; }
        public Vector2 ParentPositionOnGui { get; set; }
#endif


#if UNITY_EDITOR
        public void Initialise(int inputNb, string name, ACTIONINPUT actionInput)
        {
            this.inputNb = inputNb;
            this.name = name;
            this.actionInput = actionInput;
        }

        public void CreateChildAttack(ACTIONINPUT actionInput)
        {
            childAttacks ??= new AttackData[3];

            if (childAttacks[(int)actionInput] != null)
            {
                Debug.LogWarning("You are trying to create an attack in an already occupied slot.");
                return;
            }

            AttackData attackData = ScriptableObject.CreateInstance<AttackData>();
            attackData.Initialise(inputNb + 1, name + actionInput.ToString()[0], actionInput);
        }

        public void OnGUI() { }
#endif
    }
}