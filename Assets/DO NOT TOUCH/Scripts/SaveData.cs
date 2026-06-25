using System;
using System.Collections.Generic;

[Serializable]
public class LevelSaveData
{
    public string sceneName;
    public bool completed;
    public float bestTime;
}

[Serializable]
public class SaveData
{
    public List<LevelSaveData> levels = new();
}