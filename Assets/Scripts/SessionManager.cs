using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionManager : MonoBehaviour {

    public static SessionManager Instance;

    //The team whose turn it is
    public Team currentPlayingTeam;

    private ISessionType sessionType;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Saves game in a file (NOT IMPLEMENTED)
    /// </summary>
    public void SaveGame()
    {

    }

}

