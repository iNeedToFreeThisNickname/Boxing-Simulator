using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "New Level")]
public class Level : ScriptableObject
{
    
    [Range(0.1f, 0.5f)] public float opponentDamage;
    [Range(0.1f, 0.7f)] public float opponentStandUpChance;
}
