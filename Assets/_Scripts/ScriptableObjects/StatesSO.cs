using UnityEngine;

[CreateAssetMenu(fileName = "States", menuName = "ScriptableObjects/new States List")]
public class StatesSO : ScriptableObject
{
    public StateSO[] states;
}