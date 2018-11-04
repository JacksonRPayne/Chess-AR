using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//A pieces behaviour when moving on the board
public interface IPieceBehaviour {
    //Gets the spaces the piece can move when at a certain position
    List<BoardPosition> GetMovableSpaces(BoardPosition startingPosition, Team team);

    //Returns whether or not a piece can move to a space
    bool PositionValid(BoardPosition startingPosition, BoardPosition targetPosition, Team team);
}

//Used to initialize the piece behaviour
public enum PieceType
{
    Pawn,
    Rook,
    Knight,
    Bishop,
    King,
    Queen
}

#region Pawn
// The behaviour of a pawn
public class Pawn : IPieceBehaviour
{
    //Stores whether or not the pawn has moved
    public bool hasmoved = false;

    //Caches the boardstatemanager
    BoardStateManager boardStateManager = BoardStateManager.Instance;

    public List<BoardPosition> GetMovableSpaces(BoardPosition startingPosition, Team team)
    {
        List<BoardPosition> returnList = new List<BoardPosition>();

        //Loops through x
        for(int i = 0; i<8; i++)
        {
            //Loops y
            for (int j = 0; j < 8; j++)
            {
                if (PositionValid(startingPosition, new BoardPosition { x = i, y = j }, team))
                {
                    returnList.Add(new BoardPosition { x = i, y = j });
                }
            }
        }

        return returnList;
    }

    public bool PositionValid(BoardPosition startingPosition, BoardPosition targetPosition, Team team)
    {
        //Convinience
        int x = targetPosition.x;
        int y = targetPosition.y;

        //If the square isn't empty
        if (boardStateManager.currentBoardState.boardData[y, x] != "")
        {
            //If the piece is on a different team 
            if (ChessBoardManager.Instance.CheckForEnemyPiece(team, x, y))
            {
                //If the square is diagonal to the piece
                if (y == startingPosition.y + team.direction && (x == startingPosition.x + 1 || x == startingPosition.x - 1))
                    //Add this square to the list of movable squares
                    return true;
            }
        }
        //If the square is empty and it's directly in front
        else
        {
            if (y == startingPosition.y + team.direction && x == startingPosition.x)
            {
                return true;
            }
            //If it hasn't moved, the piece is 2 squares ahead, and there isn't a piece blocking it
            else if ((y == startingPosition.y + team.direction * 2 && x == startingPosition.x) && boardStateManager.currentBoardState.boardData[y - team.direction, x] == "" && !hasmoved)
            {
                //Add the square to the list of movable squares
                return true;
            }

        }

        return false;
    }
}

#endregion

#region Rook
public class Rook : IPieceBehaviour
{
    //Caches the boardstatemanager
    BoardStateManager boardStateManager = BoardStateManager.Instance;

    List<BoardPosition> returnList;

    public List<BoardPosition> GetMovableSpaces(BoardPosition startingPosition, Team team)
    {
        returnList = new List<BoardPosition>();

        //These are used to track whether the direction
        //should still be looped through, or if it's blocked
        bool up = true;
        bool down = true;
        bool left = true;
        bool right = true;

        for (int i = 1; i < 8; i++)
        {
            //Up check
            if (up)
                up = IterateDirectionCheck(startingPosition, new BoardPosition { x = startingPosition.x, y = startingPosition.y + i }, team);

            //Right check
            if (right)
                right = IterateDirectionCheck(startingPosition, new BoardPosition { x = startingPosition.x + i, y = startingPosition.y }, team);

            //Left check
            if (left)
                left = IterateDirectionCheck(startingPosition, new BoardPosition { x = startingPosition.x - i, y = startingPosition.y }, team);

            //Down check
            if (down)
                down = IterateDirectionCheck(startingPosition, new BoardPosition { x = startingPosition.x, y = startingPosition.y - i }, team);

        }

        return returnList;
    }


    /// <summary>
    ///Checks if the board position is valid for the bishop movement, returns true if it should keep looping and false otherwise
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private bool IterateDirectionCheck(BoardPosition startingPosition, BoardPosition targetPosition, Team team)
    {
        bool runAgain = true;

        //Convinience
        int x = targetPosition.x;
        int y = targetPosition.y;

        //Checks to see if it's in the bounds of the array
        if ((y <= 7) && (x <= 7) && (y >= 0) && (x >= 0))
        {
            //If the square is empty
            if (boardStateManager.currentBoardState.boardData[y, x] == "")
            {
                //Add the square to the return
                returnList.Add(new BoardPosition { x = x, y = y });
            }
            //If the square isn't empty
            else
            {
                //If it's an enemy return the square, then return
                if (ChessBoardManager.Instance.CheckForEnemyPiece(team, x, y))
                {
                    returnList.Add(new BoardPosition { x = x, y = y });
                    runAgain = false;
                }
                //If it's a friendly, just return
                else
                {
                    runAgain = false;
                }
            }
        }

        return runAgain;
    }

    public bool PositionValid(BoardPosition startingPosition, BoardPosition targetPosition, Team team)
    {
        foreach (BoardPosition b in GetMovableSpaces(startingPosition, team))
        {
            if (b.Equals(targetPosition))
                return true;
        }

        return false;
    }
}

#endregion

#region Bishop
public class Bishop : IPieceBehaviour
{
    //Caches the boardstatemanager
    BoardStateManager boardStateManager = BoardStateManager.Instance;

    List<BoardPosition> returnList;

    public List<BoardPosition> GetMovableSpaces(BoardPosition startingPosition, Team team)
    {
        returnList = new List<BoardPosition>();

        //These are used to track whether the direction
        //should still be looped through, or if it's blocked
        bool rightUp = true;
        bool rightDown = true;
        bool leftUp = true;
        bool leftDown = true;

        for (int i = 1; i < 8; i++)
        {
            //Right up check
            if(rightUp)
                rightUp = IterateDirectionCheck(startingPosition, new BoardPosition { x = startingPosition.x + i, y = startingPosition.y + i }, team);

            //Left down check
            if(leftDown)
                leftDown = IterateDirectionCheck(startingPosition, new BoardPosition { x = startingPosition.x - i, y = startingPosition.y - i }, team);

            //Left up check
            if(leftUp)
                leftUp = IterateDirectionCheck(startingPosition, new BoardPosition { x = startingPosition.x - i, y = startingPosition.y + i }, team);

            //Right down check
            if(rightDown)
                rightDown = IterateDirectionCheck(startingPosition, new BoardPosition { x = startingPosition.x + i, y = startingPosition.y - i }, team);

        }

        return returnList;
    }

    /// <summary>
    ///Checks if the board position is valid for the bishop movement, returns true if it should keep looping and false otherwise
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private bool IterateDirectionCheck(BoardPosition startingPosition, BoardPosition targetPosition, Team team)
    {
        bool runAgain = true;

        //Convinience
        int x = targetPosition.x;
        int y = targetPosition.y;

        //Checks to see if it's in the bounds of the array
        if ((y <= 7) && (x <= 7) && (y >= 0) && (x >= 0))
        {
            //If the square is empty
            if (boardStateManager.currentBoardState.boardData[y, x] == "")
            {
                //Add the square to the return
                returnList.Add(new BoardPosition { x = x, y = y });
            }
            //If the square isn't empty
            else
            {
                //If it's an enemy return the square, then return
                if(ChessBoardManager.Instance.CheckForEnemyPiece(team, x, y))
                {
                    returnList.Add(new BoardPosition { x = x, y = y });
                    runAgain = false;
                }
                //If it's a friendly, just return
                else
                {
                    runAgain = false;
                }
            }
        }

        return runAgain;
    }

    public bool PositionValid(BoardPosition startingPosition, BoardPosition targetPosition, Team team)
    {
        foreach (BoardPosition b in GetMovableSpaces(startingPosition, team))
        {
            if (b.Equals(targetPosition))
                return true;
        }

        return false;
    }

}
#endregion

#region Knight
class Knight : IPieceBehaviour
{
    //Caches the boardstatemanager
    BoardStateManager boardStateManager = BoardStateManager.Instance;

    public List<BoardPosition> GetMovableSpaces(BoardPosition startingPosition, Team team)
    {
        List<BoardPosition> returnList = new List<BoardPosition>();
        //Loops x
        for (int x = 0; x < 8; x++)
        {
            //Loops y
            for (int y = 0; y < 8; y++)
            {
                //Checks if the correct, then adds the square as a movable square
                if (PositionValid(startingPosition, new BoardPosition { x = x, y = y }, team)) {
                    //The square isn't empty
                    if (boardStateManager.currentBoardState.boardData[y, x] != "")
                    {
                        //If it's an enemy piece
                        if(ChessBoardManager.Instance.CheckForEnemyPiece(team, x, y))
                            //Add the square to the return
                            returnList.Add(new BoardPosition { x = x, y = y });
                    }
                    //If the square is empty
                    else
                    {
                        //Add the square to the return
                        returnList.Add(new BoardPosition { x = x, y = y });
                    }
                }
            }
        }

        return returnList;
    }


    /// <summary>
    /// Checks whether the movement is an L shape or a J shape, returns false if neither
    /// </summary>
    /// <param name="startingPosition"></param>
    /// <param name="targetPosition"></param>
    /// <returns></returns>
    public bool PositionValid(BoardPosition startingPosition, BoardPosition targetPosition, Team team)
    {
        //For convinience (and to copy old code)
        int x = startingPosition.x;
        int y = startingPosition.y;

        int targetX = targetPosition.x;
        int targetY = targetPosition.y;

        //Determines logic for both knight movement shapes
        bool isJShape = (targetX == x + 1 || targetX == x - 1) && (targetY == y + 2 || targetY == y - 2);
        bool isLShape = (targetX == x + 2 || targetX == x - 2) && (targetY == y + 1 || targetY == y - 1);

        //Returns true if either of them are true;
        return (isJShape || isLShape);
    }
}
#endregion

#region King
class King : IPieceBehaviour
{
    //Caches the boardstatemanager
    BoardStateManager boardStateManager = BoardStateManager.Instance;

    public List<BoardPosition> GetMovableSpaces(BoardPosition startingPosition, Team team)
    {
        List<BoardPosition> returnList = new List<BoardPosition>();
        //Loops x
        for (int x = 0; x < 8; x++)
        {
            //Loops y
            for (int y = 0; y < 8; y++)
            {
                //Checks if the correct, then adds the square as a movable square
                if (PositionValid(startingPosition, new BoardPosition { x = x, y = y }, team))
                {
                    //The square isn't empty
                    if (boardStateManager.currentBoardState.boardData[y, x] != "")
                    {
                        //If it's an enemy piece
                        if (ChessBoardManager.Instance.CheckForEnemyPiece(team, x, y))
                            //Add the square to the return
                            returnList.Add(new BoardPosition { x = x, y = y });
                    }
                    //If the square is empty
                    else
                    {
                        //Add the square to the return
                        returnList.Add(new BoardPosition { x = x, y = y });
                    }
                }
            }
        }

        return returnList;
    }

    public bool PositionValid(BoardPosition startingPosition, BoardPosition targetPosition, Team team)
    {
        //Caches the xs and ys of the current position and the target position
        int x = startingPosition.x;
        int y = startingPosition.y;

        int targetX = targetPosition.x;
        int targetY = targetPosition.y;

        //Finds the amount changed on the x and y
        int xDistance = targetX - x;
        int yDistance = targetY - y;

        return (Mathf.Abs(xDistance) < 2 && Mathf.Abs(yDistance) < 2);
    }
}
#endregion

#region Queen
public class Queen : IPieceBehaviour
{
    //References the behaviour of rooks and bishops
    private Rook rookBehaviour = new Rook();
    private Bishop bishopBehavior = new Bishop();

    public List<BoardPosition> GetMovableSpaces(BoardPosition startingPosition, Team team)
    {
        List<BoardPosition> returnList = new List<BoardPosition>();

        //Creates a returnlist with all spaces a rook or a bishop could move to
        returnList.AddRange(rookBehaviour.GetMovableSpaces(startingPosition, team));
        returnList.AddRange(bishopBehavior.GetMovableSpaces(startingPosition, team));

        return returnList;
    }

    public bool PositionValid(BoardPosition startingPosition, BoardPosition targetPosition, Team team)
    {
        //Returns whether or not a bishop or a rook could move to that space
        return (rookBehaviour.PositionValid(startingPosition, targetPosition, team) || bishopBehavior.PositionValid(startingPosition, targetPosition, team));
    }
}
#endregion