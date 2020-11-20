using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Versioning;
using UnityEngine;

public class Mats_Singleton : MonoBehaviour
{
    public static Mats_Singleton instance;
    void Awake()
    {
        if (instance != null && instance != this)
            gameObject.SetActive(false);
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    public Material matFarEnemy;
    public Material matNearEnemy;

}
