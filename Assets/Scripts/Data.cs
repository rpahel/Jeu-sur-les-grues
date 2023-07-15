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

    public class ComboData
    {
        [HideInInspector]
        public int inputNb; // Starts at 1 for first input
        public string name;
        public ATTACKINPUT attackInput;
        public AnimationClip animation;
        public bool isFinalMove;
        public ComboData previousMove;
        public ComboData[] nextMoves; // UP, RIGHT, DOWN
        public Vector2 PositionOnGui { get; set; }

        public ComboData(bool isFirst)
        {
            if (!isFirst)
                return;

            inputNb = 1;
            attackInput = ATTACKINPUT.ATTACK;
            name = "A";
            nextMoves = new ComboData[3];

            for(int i = 0; i<3; i++)
            {
                CreateNextMove((ATTACKINPUT)i);
            }
        }

        private ComboData(ComboData _previousMove, ATTACKINPUT input)
        {
            previousMove = _previousMove;
            inputNb = _previousMove.inputNb + 1;
            name = _previousMove.name + input.ToString()[0];
            nextMoves = new ComboData[3];
        }

        //===================================================

        public void CreateNextMove(ATTACKINPUT input)
        {
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
            int deepestDepth = inputNb;
            foreach (ComboData comboData in nextMoves)
            {
                if(comboData == null)
                    continue;

                int newDepth = comboData.GetComboDepth();
                if(newDepth > deepestDepth)
                    deepestDepth = newDepth;
            }
            return deepestDepth;
        }

        public int GetNbOfCombosAtDepth(int depth)
        {
            if(depth == inputNb)
                return 1;

            int ret = 0;
            for(int i = 0; i < 3; i++)
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

            List<ComboData> ret = new();
            for (int i = 0; i < 3; i++)
            {
                if (nextMoves[i] == null)
                    continue;

                ret.AddRange(nextMoves[i].GetAllCombosAtDepth(depth));
            }

            return ret.ToArray();
        }
    }
}