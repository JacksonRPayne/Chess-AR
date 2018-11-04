using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessBoardManager : MonoBehaviour {

    //Prefab of a square
    public GameObject square;

    //Dictionary to get the square at a certain board position
    public Dictionary<BoardPosition, GameObject> SquareDictionary = new Dictionary<BoardPosition, GameObject>();

    #region Singleton
    //Singleton
    public static ChessBoardManager Instance;

    private void Awake()
    {
        //Set this as the instance
        if (Instance == null)
            Instance = this;
        //If there is another, destroy this one
        else
            Destroy(gameObject);
    }

    #endregion

    void Start () {
        //Starts by setting up chess board
        InitializeChessBoard();
	}


    /// <summary>
    /// Sets up the board as well as the pieces
    /// </summary>
    private void InitializeChessBoard()
    {
        //Spawns the board
        SpawnChessBoard();

        //Spawns pieces
        LoadBoardState(BoardStateManager.Instance.DefaultBoardState);
    }

    /// <summary>
    /// Sets up the board by using a boardstate
    /// </summary>
    /// <param name="boardState"></param>
    public void LoadBoardState(BoardState boardState)
    {
        
        //Deletes all of the old pieces from the board
        TeamManager.Instance.WhiteTeam.DeleteAllPieces();
        TeamManager.Instance.BlackTeam.DeleteAllPieces();

        //Loops through y
        for(int i= 0; i<8; i++)
        {
            //Loops through x
            for(int j = 0; j<8; j++)
            {
                //Defines the value of the boardstate with the current index to be
                //used later down NOTE: x and y are flipped due to the way the 2d list is layed out
                string currentIteration = boardState.boardData[i, j];

                //If the string isn't empty at the current index
                if (currentIteration != "") {

                    GameObject piece;

                    //If it's a pawn thats moved
                    if (currentIteration == "lpm" || currentIteration == "dpm")
                    {
                        //Sets the piece to a pawn of the right team
                        piece = BoardStateManager.Instance.PieceDictionary[currentIteration.Remove(2)];
                    }
                    //If it's anything else
                    else
                    {
                        //Gets piece from the string of the current index of the boardstate 2d array
                        piece = BoardStateManager.Instance.PieceDictionary[currentIteration];
                    }

                    //Gets the square from the square dictionary
                    GameObject square = SquareDictionary[new BoardPosition() { x = j, y = i }];
                    //Instantiates the piece at the square's position, parented to the board
                    GameObject newPiece = Instantiate(piece, square.transform.position, piece.transform.rotation, transform);

                    //Sets the piece's board position to the correct one
                    Piece newPieceData = newPiece.GetComponent<Piece>();

                    //If it's a pawn that's moved
                    if (currentIteration == "lpm" || currentIteration == "dpm")
                    {
                        //Sets up the new piece to have moved and have the right identifier
                        Pawn pawnBehaviour = new Pawn();
                        pawnBehaviour.hasmoved = true;
                        newPieceData.pieceBehaviour = pawnBehaviour;

                        newPieceData.boardStateIdentifier = currentIteration;
                    }

                    //Sets the correct board position for the new piece
                    newPieceData.boardPosition = new BoardPosition { x = j, y = i };

                }
            }
        }

        //Changes the current boardstate to the boardstate loaded
        BoardStateManager.Instance.currentBoardState = boardState;
    }

    /// <summary>
    /// Saves the current board state
    /// </summary>
    public void SaveBoardState(out BoardState boardState)
    {
        //What will be returned
        BoardState currentBoardState = new BoardState();

        //Loops through y
        for(int i = 0; i<8; i++)
        {
            //Loops through x
            for(int j =0; j<8; j++)
            {
                //NOTE: x and y are flipped because of the way the 2d list is layed out
                Square currentIterationSquare = SquareDictionary[new BoardPosition { x = j, y = i }].GetComponent<Square>();
                Piece currentIterationPiece = currentIterationSquare.GetComponent<Square>().currentPiece;

                if (currentIterationPiece != null)
                    currentBoardState.boardData[i, j] = currentIterationPiece.boardStateIdentifier;
                else
                    currentBoardState.boardData[i, j] = "";
            }
        }

        boardState = currentBoardState;
    }

    /// <summary>
    /// Instantiates the squares of the chess board
    /// </summary>
    private void SpawnChessBoard()
    {
        //Loops through columns
        for (int i = 0; i < 8; i++)
        {
            //Loops through rows
            for (int j = 0; j < 8; j++)
            {
                //Spawns a square that is spaced correctly from the last square, and at the current y, 
                //and is spaced so the board is in the center of the parent
                Vector3 squarePosition = new Vector3((i * square.transform.lossyScale.x) - (square.transform.lossyScale.x*3.5f), transform.position.y, (j * square.transform.lossyScale.z) -(square.transform.lossyScale.z*3.5f));
                GameObject spawnedSquare = Instantiate(square, squarePosition + new Vector3(transform.position.x, 0, transform.position.z), Quaternion.identity, transform);

                //Get's the mesh renderer on the graphics and the square component on the main object
                MeshRenderer squareRenderer = spawnedSquare.GetComponentInChildren<MeshRenderer>();
                Square squareData = spawnedSquare.GetComponent<Square>();

                //Sets board position of the square 
                squareData.boardPosition = new BoardPosition { x = i, y = j };
                //Updates the dictionary to add this square
                SquareDictionary.Add(new BoardPosition { x = i, y = j }, spawnedSquare);

                //This will be true every other time to account for the chessboard pattern
                if ((i + j) % 2 == 1)
                {
                    //Set the square to a light square
                    squareRenderer.material = squareData.lightMaterial;
                }
                else
                {
                    //Set the square to a dark square
                    squareRenderer.material = squareData.darkMaterial;
                }

            }
        }
    }


    /// <summary>
    /// Returns true if the piece on that square is an enemy one
    /// </summary>
    /// <param name="team"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool CheckForEnemyPiece(Team team, int x, int y)
    {
        GameObject square = SquareDictionary[new BoardPosition { x = x, y = y }];

        if (square != null)
            return square.GetComponent<Square>().currentPiece.team.teamName != team.teamName; 
        else
            return false;
    }

    /// <summary>
    /// Rotates the board 180 degrees
    /// </summary>
    public void RotateBoard()
    {
        StartCoroutine(RotateBoardAnimation());
    }

    public IEnumerator RotateBoardAnimation()
    {
        int n = 20;

        for (int i = 0; i < n; i++)
        {
            transform.Rotate(new Vector3(0, 180 / n, 0));
            yield return null;
        }

    }

}
