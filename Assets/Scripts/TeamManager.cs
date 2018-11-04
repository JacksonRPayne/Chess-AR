using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour {

    public Team WhiteTeam;
    public Team BlackTeam;

    public static TeamManager Instance;

    //Stores the team whose turn it currently is
    public Team currentPlayingTeam;

    ISessionType sessionType;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        //Sets opposing teams
        WhiteTeam.opposingTeam = BlackTeam;
        BlackTeam.opposingTeam = WhiteTeam;
    }

    private void Start()
    {
        sessionType = new LocalSessionType();
        SetTurn(WhiteTeam);
    }

    public void ChangeTurn()
    {
        //If it's white's turn
        if (currentPlayingTeam == WhiteTeam)
        {
            //Set it to black's turn
            SetTurn(BlackTeam);
        }
        //If it's black's turn
        else if (currentPlayingTeam == BlackTeam)
        {
            //Set it to white's turn
            SetTurn(WhiteTeam);
        }
        //If it's neither
        else
        {
            //Throw an error
            Debug.LogError("No viable team is currently playing");
        }

        //Do other things depending on the session type
        sessionType.ChangeTurn();

    }

    /// <summary>
    /// Sets the turn to a specific team
    /// </summary>
    /// <param name="team"></param>
    private void SetTurn(Team team)
    {

        currentPlayingTeam = team;

        team.opposingTeam.currentlyPlaying = false;
        team.currentlyPlaying = true;
    }

}

interface ISessionType
{
    /// <summary>
    /// Responds to the change of team (ex: rotates board, updates UI etc.)
    /// </summary>
    void ChangeTurn();
}

/// <summary>
/// Represents a local game, meaning 2 players are using the same board
/// </summary>
public class LocalSessionType : ISessionType
{
    public void ChangeTurn()
    {
        ChessBoardManager.Instance.RotateBoard();
    }
}