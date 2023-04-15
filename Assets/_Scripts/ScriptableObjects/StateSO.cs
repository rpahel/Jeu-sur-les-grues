using Data;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "State", menuName = "ScriptableObjects/new State")]
public class StateSO : ScriptableObject
{
    public STATE state;
    public Color filterColor;
    public AnimationClip stateAnimation;
    public float healthToAdd; // Amount of Health to add every every update period.
    public float healthUpdateCooldown; // Period of time between each health update
    public float staminaToAdd; // Amount of Stamina to add every update period.
    public float staminaUpdateCooldown; // Period of time between each stamina update
}
