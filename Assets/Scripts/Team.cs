using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Team {

    //Whether the team is in check
    //[HideInInspector]
    public bool isInCheck;
    //Name of the team (ie: black or white)
    public string teamName;
    //The king of this team
    public Piece king;
    //Which side of the board it's on (white is 1 black is -1) 
    public int direction;
    //Indicates whether or not it's this teams turn;
    public bool currentlyPlaying;

    //Stores all of the pieces in the team
    [HideInInspector]
    public List<Piece> pieces;

    public Team opposingTeam;

    //Example: d or l for dark or light
    public string boardStateIdentifierPrefix;

    /// <summary>
    /// Updates the isInCheck variable to be accurate to the current boardstate
    /// </summary>
    public void UpdateCheckStatus()
    {
        foreach (Piece piece in opposingTeam.pieces)
        {
            if(piece.pieceBehaviour.PositionValid(piece.boardPosition, king.boardPosition, opposingTeam))
            {
                isInCheck = true;
                return;
            }
        }

        isInCheck = false;
    }

    /// <summary>
    /// Deletes all of the pieces in this team
    /// </summary>
    public void DeleteAllPieces()
    {
        if(pieces.Count > 0)
        {
            //Iteration is backwards so list indexes don't change when deleting items
            for (int i = pieces.Count-1; i >=0; i--)
            {
                //Remove the piece from the board
                if (pieces[i] != null)
                {
                    pieces[i].RemovePiece();
                    //Sets the square's current piece to none
                    ChessBoardManager.Instance.SquareDictionary[pieces[i].boardPosition].GetComponent<Square>().currentPiece = null;
                }
                //Remove the piece from the list
                pieces.RemoveAt(i);
            }
        }
    }

}

public enum TeamIdentifier
{
    White,
    Black
}