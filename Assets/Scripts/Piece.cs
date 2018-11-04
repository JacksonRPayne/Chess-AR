using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour, IHighlightable {

    //Board position of the piece
    public BoardPosition boardPosition;
    //String that represents the piece (ex: light pawn is lp)
    public string boardStateIdentifier;

    //The type of piece the piece is
    public IPieceBehaviour pieceBehaviour;
    //This is used to set the pieceBehaviour in the editor
    public PieceType pieceType;

    //The team this piece belongs to
    public Team team;
    //Used to set team through the inspector
    public TeamIdentifier teamIdentifier;

    //This stores all of the renderers in the children of this object
    private Renderer[] renderers;

    //The magnitude of a hightlight
    public static float hoverMagnitude = 0.15f;

    //Represents whether or not the piece is currently selected by the player
    public bool selected { get; set; }

    //Represents the amount the piece darkens when selected
    private static float selectionHighlightMagnitude = 0.25f;

    //The original colors of the graphics objects
    private Color[] origninalColors;

    //The width of the outline on the piece when selected
    public float selectedPieceOutlineWidth = 1.1f;

    public void Awake()
    {
        //Initializes this variable to prevent null errors
        selected = false;

        //Sets the team to the team the teamidentifier indicates
        if (teamIdentifier == TeamIdentifier.White)
            team = TeamManager.Instance.WhiteTeam;
        else
            team = TeamManager.Instance.BlackTeam;

        if (pieceBehaviour == null)
        {
            //Sets the piecebehaviour based on the piecetype
            switch (pieceType)
            {
                case PieceType.Pawn:
                    pieceBehaviour = new Pawn();
                    break;
                case PieceType.Rook:
                    pieceBehaviour = new Rook();
                    break;
                case PieceType.Bishop:
                    pieceBehaviour = new Bishop();
                    break;
                case PieceType.Knight:
                    pieceBehaviour = new Knight();
                    break;
                case PieceType.King:
                    pieceBehaviour = new King();
                    team.king = this;
                    break;
                case PieceType.Queen:
                    pieceBehaviour = new Queen();
                    break;
            }
        }
    }

    private void Start()
    {
        //Sets the square's piece to this one
        ChessBoardManager.Instance.SquareDictionary[boardPosition].GetComponent<Square>().currentPiece = this;

        //Adds itself to the pieces list in its team
        team.pieces.Add(this);

        //Gets the renderers of the children (graphics objects)
        renderers = GetComponentsInChildren<Renderer>();

        //Sets it to be the amount of renderers in the children
        origninalColors = new Color[renderers.Length];

        //Sets up the original colors to be corresponding to the renderers
        for (int i = 0; i < renderers.Length; i++)
        {
            origninalColors[i] = renderers[i].material.color;
        }
    }

    /// <summary>
    /// Tries to move the piece to a new position
    /// </summary>
    /// <param name="newPosition"></param>
    public bool Move(BoardPosition newPosition)
    {

        //Check if the piece can move at all
        List<BoardPosition> movableSpaces = pieceBehaviour.GetMovableSpaces(boardPosition, team);
        if (movableSpaces.Count > 0)
        {

            //Check if that position is a viable position (using IPieceBehaviour)
            foreach (BoardPosition position in pieceBehaviour.GetMovableSpaces(boardPosition, team))
            {
                if (newPosition.Equals(position))
                {

                    //Stores the current board state
                    BoardState currentState = new BoardState();
                    currentState.boardData = (string[,])BoardStateManager.Instance.currentBoardState.boardData.Clone();

                    //Moves the piece to the new position
                    ExcecuteMove(newPosition);

                    //If the move puts the team in check
                    if (team.isInCheck)
                    {
                        //Revert back to the old state
                        ChessBoardManager.Instance.LoadBoardState(currentState);
                    }
                    else
                    {
                        //Switches the turns
                        TeamManager.Instance.ChangeTurn();
                        //Resets board highlights
                        Square.ResetBoardHighlights();
                        return true;
                    }

                }

            }
        }

        return false;

    }

    /// <summary>
    /// Does the physical movement of the piece 
    /// </summary>
    /// <param name="newPosition"></param>
    private void PerformMovement(BoardPosition newPosition)
    {
        //Move the piece
        transform.position = ChessBoardManager.Instance.SquareDictionary[newPosition].transform.position;

    }

    /// <summary>
    /// Moves the piece, as well as sets the new boardposition and updates the current boardState
    /// </summary>
    /// <param name="newPosition"></param>
    public void ExcecuteMove(BoardPosition newPosition)
    {
        //Caches the suqre that is being moved to 
        Square newSquare = ChessBoardManager.Instance.SquareDictionary[newPosition].GetComponent<Square>();

        //If there is a piece in the square that this piece is moving to
        if (newSquare.currentPiece != null)
        {
            //Capture it
            newSquare.currentPiece.GetCaptured();
        }

        //Sets the square it's on to empty
        ChessBoardManager.Instance.SquareDictionary[boardPosition].GetComponent<Square>().currentPiece = null;
        //Update it's board position
        boardPosition = newPosition;
        //Move the piece phisically
        PerformMovement(newPosition);
        //Set's the piece of the square it's moving to to this piece
        newSquare.currentPiece = this;

        //If it's a pawn
        if (pieceType == PieceType.Pawn)
        {
            //Cast the behaviour as a pawn
            Pawn pawn = (Pawn)pieceBehaviour;
            //If it hasn't moved, set that it has moved
            if (!pawn.hasmoved)
            {
                pawn.hasmoved = true;
                boardStateIdentifier = team.boardStateIdentifierPrefix + "pm";
            }
        }

        //Save the new BoardState
        ChessBoardManager.Instance.SaveBoardState(out BoardStateManager.Instance.currentBoardState);

        //Update the check statuses
        TeamManager.Instance.WhiteTeam.UpdateCheckStatus();
        TeamManager.Instance.BlackTeam.UpdateCheckStatus();

    }

    /// <summary>
    /// Highlights the squares in which this piece can move
    /// </summary>
    public void HighlightAllPossibleMoves()
    {
        //Resets all highlights
        Square.ResetBoardHighlights();

        //Loops through all possible squares that are movable
        foreach (BoardPosition b in pieceBehaviour.GetMovableSpaces(boardPosition, team))
        {
            //Highlights the square
            ChessBoardManager.Instance.SquareDictionary[b].GetComponent<Square>().Select();
        }
    }

    /// <summary>
    /// Highlights the piece depending on the magnitude entered
    /// </summary>
    /// <param name="magnitude"></param>
    public void Highlight(float magnitude)
    {

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = origninalColors[i] - new Color(magnitude, magnitude, magnitude);
        }
    }

    public void UnHighlight()
    {
        //Sets the colors back to the original ones
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = origninalColors[i];
            renderers[i].material.SetFloat("_OutlineWidth", 1f);

        }

        //If it was selected, make it not selected anymore
        selected = false;

    }

    public void Select()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.SetFloat("_OutlineWidth", selectedPieceOutlineWidth);
        }

        //Sets it as selected
        selected = true;
    }

    /// <summary>
    /// Captures the piece this method is called from
    /// </summary>
    public void GetCaptured()
    {
        //TEMP
        team.pieces.Remove(this);
        Destroy(gameObject);
    }

    /// <summary>
    /// Removes the piece without treating it as a capture
    /// </summary>
    public void RemovePiece()
    {
        Destroy(gameObject);
    }

}
