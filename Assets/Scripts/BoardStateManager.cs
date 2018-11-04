using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoardStateManager : MonoBehaviour {

    //Singleton
    public static BoardStateManager Instance;

    //Prefabs for every piece in the game
    public GameObject lightPawn;
    public GameObject darkPawn;
    public GameObject lightKnight;
    public GameObject darkKnight;
    public GameObject lightBishop;
    public GameObject darkBishop;
    public GameObject lightRook;
    public GameObject darkRook;
    public GameObject lightQueen;
    public GameObject darkQueen;
    public GameObject lightKing;
    public GameObject darkKing;

    //Dictionary of strings that correspond with pieces
    public Dictionary<string, GameObject> PieceDictionary;

    //The default board state (standard chess board)
    public BoardState DefaultBoardState = new BoardState() { boardData = new string[,] 
    {
        {"lr", "lkn","lb","lki","lq","lb","lkn","lr"},
        {"lp","lp","lp","lp","lp","lp","lp","lp"},
        {"","","","","","","","" },
        {"","","","","","","","" },
        {"","","","","","","","" },
        {"","","","","","","","" },
        {"dp","dp","dp","dp","dp","dp","dp","dp" },
        {"dr", "dkn","db","dki","dq","db","dkn","dr"}
    }
    };

    /// <summary>
    /// Holds a BoardState representation of the chess board
    /// </summary>
    public BoardState currentBoardState;

    void Awake()
    {
        //Set this as the instance
        if (Instance == null)
            Instance = this;
        //If there is another, destroy this one
        else
            Destroy(gameObject);

        //CHANGE THIS WHEN IMPLEMENTING SAVED GAMES
        currentBoardState = DefaultBoardState;

        //Defines the strings associated with each piece
        PieceDictionary = new Dictionary<string, GameObject>
        {
            {"lp", lightPawn }, {"dp", darkPawn},
            {"lkn", lightKnight}, {"dkn", darkKnight},
            {"lb", lightBishop}, {"db", darkBishop},
            {"lr", lightRook }, {"dr", darkRook },
            {"lq", lightQueen}, {"dq", darkQueen},
            {"lki", lightKing}, {"dki", darkKing}
        };

    }

}
