using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

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

    [System.Serializable]
    public class ComboData
    {
        [HideInInspector]
        public int inputNb; // Starts at 1 for first input
        public string name;
        public ACTIONINPUT actionInput;
        public int damage;
        public STATE inflictedState;
        public AnimationClip animation;
        //public bool isFinalMove; // Forcement final si y a pas d'enfants
        [SerializeField, HideInInspector]
        private ComboData[] nextMoves = null; // UP, RIGHT, DOWN
        public Vector2 PositionOnGui { get; set; }
        public Vector2 ParentPositionOnGui { get; set; }

        public ComboData()
        {
            nextMoves = null; // S'assurer que nextMoves est bien vide pour empecher Unity de faire des tableaux a l'infini
        }

        public ComboData(bool isFirst)
        {
            if (!isFirst)
                return;

            inputNb = 1;
            actionInput = ACTIONINPUT.ATTACK;
            name = "A";
            nextMoves = new ComboData[3];

            for (int i = 0; i < 3; i++)
            {
                CreateNextMove((ACTIONINPUT)i);
            }
        }

        private ComboData(ComboData _previousMove, ACTIONINPUT input)
        {
            inputNb = _previousMove.inputNb + 1;
            name = _previousMove.name + input.ToString()[0];
            actionInput = input;
        }

        //===================================================

        public void CreateNextMove(ACTIONINPUT input)
        {
            if(nextMoves == null || nextMoves.Length == 0)
                nextMoves = new ComboData[3];

            if (nextMoves[(int)input] != null && nextMoves[(int)input].name != null && nextMoves[(int)input].name != "") // Le slot est deja pris
                return;

            nextMoves[(int)input] = new(this, input);
        }

        public void DeleteMove()
        {
            if (nextMoves != null && nextMoves.Length != 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (nextMoves[i] == null)
                        continue;

                    nextMoves[i].DeleteMove();
                    nextMoves[i] = null;
                }
            }

            inputNb = 0;
            actionInput = (ACTIONINPUT)0;
            name = null;
            nextMoves = null;
        }

        public int GetComboDepth()
        {
            if(nextMoves == null || nextMoves.Length == 0)
                return inputNb;

            int deepestDepth = inputNb;
            foreach (ComboData comboData in nextMoves)
            {
                if (comboData == null)
                    continue;

                int newDepth = comboData.GetComboDepth();
                if (newDepth > deepestDepth)
                    deepestDepth = newDepth;
            }
            return deepestDepth;
        }

        public int GetNbOfCombosAtDepth(int depth)
        {
            if (depth == inputNb)
                return 1;

            if (nextMoves == null || nextMoves.Length == 0)
                return 0;

            int ret = 0;
            for (int i = 0; i < 3; i++)
            {
                if (nextMoves[i] == null || nextMoves[i].name == "")
                    continue;

                ret += nextMoves[i].GetNbOfCombosAtDepth(depth);
            }

            return ret;
        }

        public ComboData[] GetAllCombosAtDepth(int depth)
        {
            if (depth == inputNb)
                return new ComboData[1] { this };

            if (nextMoves == null || nextMoves.Length == 0)
                return null;

            List<ComboData> ret = new();
            for (int i = 0; i < 3; i++)
            {
                if (nextMoves[i] == null || nextMoves[i].name == "")
                    continue;

                ComboData[] combos = nextMoves[i].GetAllCombosAtDepth(depth);
                if (combos != null)
                    ret.AddRange(combos);
            }

            return ret.ToArray();
        }

        public ComboData GetNextMoveByInput(int input)
        {
            if (nextMoves == null || nextMoves.Length <= input) return null;
            return nextMoves[input];
        }

        public ComboData[] GetNextMoves() {  return nextMoves; }

        public ACTIONINPUT[] AvailableNextMoves()
        {
            if(nextMoves == null || nextMoves.Length == 0)
                return new ACTIONINPUT[3] { ACTIONINPUT.UP, ACTIONINPUT.ATTACK, ACTIONINPUT.DOWN };

            ACTIONINPUT[] nextAvailableMoves = new ACTIONINPUT[3];

            bool isFull = true;
            for(int i = 0;i < 3; i++)
            {
                if (nextMoves[i] != null && nextMoves[i].name != null && nextMoves[i].name != "")
                {
                    nextAvailableMoves[i] = ACTIONINPUT.DODGE;
                }
                else
                {
                    isFull = false;
                    nextAvailableMoves[i] = (ACTIONINPUT)i;
                }
            }

            if (isFull)
                return null;

            return nextAvailableMoves;
        }

        public void SetPositionOnGuiOnChildren(Vector2 pos)
        {
            if (nextMoves == null || nextMoves.Length == 0) return;

            foreach (var move in nextMoves)
            {
                if (move != null)
                    move.ParentPositionOnGui = pos;
            }
        }
    }
}