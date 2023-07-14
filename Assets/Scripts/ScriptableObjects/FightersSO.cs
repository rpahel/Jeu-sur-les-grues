using UnityEngine;

namespace Rpahel
{
    [CreateAssetMenu(fileName = "Fighters List", menuName = "ScriptableObjects/new Fighters List")]
    public class FightersSO : ScriptableObject
    {
        public FighterSO[] fighters;
    }
}