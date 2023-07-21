using System.Collections.Generic;
using UnityEngine;

namespace Rpahel.Data
{
    [System.Serializable]
    public enum ATTACKINPUT
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
        public ATTACKINPUT attackInput;
        public AnimationClip animation;
        public bool isFinalMove;
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
            attackInput = ATTACKINPUT.ATTACK;
            name = "A";
            nextMoves = new ComboData[3];

            for (int i = 0; i < 3; i++)
            {
                CreateNextMove((ATTACKINPUT)i);
            }
        }

        private ComboData(ComboData _previousMove, ATTACKINPUT input)
        {
            inputNb = _previousMove.inputNb + 1;
            name = _previousMove.name + input.ToString()[0];
            attackInput = input;
        }

        //===================================================

        public void CreateNextMove(ATTACKINPUT input)
        {
            nextMoves ??= new ComboData[3];

            if (nextMoves[(int)input] != null) // Le slot est deja pris
                return;

            nextMoves[(int)input] = new(this, input);
        }

        // TODO : DELETE MOVE AND ITS CHILDREN USING RECURSION
        public void DeleteMove()
        {

        }

        public int GetComboDepth()
        {
            if(nextMoves == null)
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

            if (nextMoves == null)
                return 0;

            int ret = 0;
            for (int i = 0; i < 3; i++)
            {
                if (nextMoves[i] == null)
                    continue;

                ret += nextMoves[i].GetNbOfCombosAtDepth(depth);
            }

            return ret;
        }

        public ComboData[] GetAllCombosAtDepth(int depth)
        {
            if (depth == inputNb)
                return new ComboData[1] { this };

            if (nextMoves == null)
                return null;

            List<ComboData> ret = new();
            for (int i = 0; i < 3; i++)
            {
                if (nextMoves[i] == null)
                    continue;

                ret.AddRange(nextMoves[i].GetAllCombosAtDepth(depth));
            }

            return ret.ToArray();
        }

        public ComboData GetNextMoveByInput(int input)
        {
            if (nextMoves == null || nextMoves.Length <= input) return null;
            return nextMoves[input];
        }

        public ComboData[] GetNextMoves() {  return nextMoves; }

        public void SetPositionOnGuiOnChildren(Vector2 pos)
        {
            if (nextMoves == null) return;

            foreach (var move in nextMoves)
            {
                if (move != null)
                    move.ParentPositionOnGui = pos;
            }
        }
    }
}