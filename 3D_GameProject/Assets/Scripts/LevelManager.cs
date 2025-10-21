using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public int levelCount = 0;
    public List<int> levelSequence = new List<int>()
    {
        3, 4, 2, 8, 1
    };

    private void Awake()
    {
        if (instance == null)
            instance = this;
            DontDestroyOnLoad(instance);
    }



}
