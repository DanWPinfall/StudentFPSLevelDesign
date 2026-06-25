using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
    public string sceneName;
    public string displayName;
    public string creatorName;

    public GameObject levelPrefab;
    public Vector3 previewScale = Vector3.one * 0.1f;
}