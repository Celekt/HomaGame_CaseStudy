using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/LevelDataScriptableObject", order = 1)]
public class LevelData : ScriptableObject
{
    public List<GameObject> boards;
}
