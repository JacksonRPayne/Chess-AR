using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleARCore;
using UnityEngine.EventSystems;


//Use only for debugging
//using Input = GoogleARCore.InstantPreviewInput; 

public class InputController : MonoBehaviour {

    //The camera on this object
    private Camera cam;
    //The selected piece
    private GameObject selectedObject;

    //Represents the type of input to be processed
    private IInputType inputType;
    //Used to set the inputType in the editor
    public InputType inputTypeSetter;

    private IHighlightable hoveredObject;

    //Represents the phase of the app the user is in, 
    //to adjust the role of input
    public InputPhase inputPhase;

    //The prefab that represents the chess board
    public GameObject chessBoardPrefab;

    //The chessboard in the scene
    private Transform chessBoard;

    //Stores the speed at which the chessboard is resized by the user
    public float scaleSpeed = 0.005f;

    //Used for debugging values
    public Text debugText;

    //Stores the maximum size of the chess board
    public float maxBoardSize = 2f;

    //Stores the minimum size of the chess board
    public float minimumBoardSize = 0.25f;

    //Decides whether or not the tracked planes are visible
    public static bool showTrackedPlanes = true;


	void Start () {
        //Gets the camera component
        cam = GetComponent<Camera>();

        //Sets the input type based on the enum, to it's corresponding class
        switch (inputTypeSetter)
        {
            case InputType.Computer:
                inputType = new ComputerInputType();
                break;
            case InputType.Mobile:
                inputType = new MobileInputType();
                break;
        }

        debugText.text = "debug text";
	}

    void Update()
    {
        //Goes through each touch
        foreach (Touch t in Input.touches)
        {
            //If any are over UI, then return
            if (EventSystem.current.IsPointerOverGameObject(t.fingerId))
                return;
        }

        //Returns out if a slider is being dragged
        if (UIManager.Instance.sliderIsBeingDragged)
            return;

        if (inputPhase == InputPhase.Placing)
        {
            //If the player is selecting
            if (inputType.IsHovering()) {

                //Stores the data from the raycast
                TrackableHit trackableHit;

                //Shoots a ray that can only hit within the bounds of a tracked plane, and if it hits:
                if (Frame.Raycast(inputType.GetCursorPosition().x, inputType.GetCursorPosition().y, TrackableHitFlags.PlaneWithinBounds, out trackableHit))
                {
                    //Create an anchor at the hitPoint
                    Anchor anchor = Session.CreateAnchor(trackableHit.Pose);
                    //Instantiates the chessboard at the hitpoint, as a prefab of the anchor
                    chessBoard = Instantiate(chessBoardPrefab, trackableHit.Pose.position, Quaternion.identity, anchor.transform).transform;

                    //Face chessboard to player
                    chessBoard.LookAt(transform);
                    chessBoard.rotation = Quaternion.Euler(new Vector3(0, chessBoard.rotation.y, 0));

                    //Changes the input phase after the board is placed
                    inputPhase = InputPhase.Playing;

                    //Hides the tracked planes
                    showTrackedPlanes = false;

                    //Sets the move board button to interactable
                    UIManager.Instance.moveBoardButton.interactable = true;
                }
            }

            return;
        }

        else if (inputPhase == InputPhase.Playing)
        {
            //Cast a ray from its mouse position
            Ray ray = cam.ScreenPointToRay(inputType.GetCursorPosition());
            //Temp variable
            RaycastHit hit;
            //If the ray hits
            if (Physics.Raycast(ray, out hit))
            {
                //If it hits a square
                if (hit.collider.tag == "Square")
                {
                    Square selectedSquare = hit.transform.GetComponent<Square>();

                    if (selectedObject != null && inputType.IsSelecting())
                    {
                        //Get the piece script
                        Piece selectedPiece = selectedObject.GetComponent<Piece>();

                        //If the piece is on the team whose turn it is, and it can move to the spot
                        if (selectedPiece.team == TeamManager.Instance.currentPlayingTeam && selectedPiece.Move(selectedSquare.boardPosition))
                        {
                            //Resets the selected piece
                            selectedPiece.UnHighlight();
                            selectedObject = null;
                            hoveredObject = null;
                            return;
                        }

                    }

                    //If the square has a piece
                    if (selectedSquare.currentPiece != null)
                    {
                        Piece selectedPiece = selectedSquare.currentPiece;
                        //If the player is selecting
                        if (inputType.IsSelecting())
                        {
                            //If the piece belongs to the team whose turn it is
                            if (selectedPiece.team == TeamManager.Instance.currentPlayingTeam)
                            {
                                //If the selected piece is a new piece
                                if (selectedPiece.gameObject != selectedObject && selectedObject != null)
                                {
                                    //Unhighlights the old object
                                    selectedObject.GetComponent<Piece>().UnHighlight();
                                }

                                //Set as selected object
                                selectedObject = selectedPiece.gameObject;
                                //Highlight the squares it can move to
                                selectedPiece.HighlightAllPossibleMoves();
                                //Selects the piece graphically
                                selectedPiece.Select();
                            }
                        }
                        //If the player is hovering and the piece isn't already selected
                        else if (inputType.IsHovering() && !selectedPiece.selected)
                        {
                            if (selectedPiece != (Object)hoveredObject)
                            {
                                if (hoveredObject != null && !hoveredObject.selected)
                                    hoveredObject.UnHighlight();
                            }

                            hoveredObject = selectedPiece;
                            hoveredObject.Highlight(Piece.hoverMagnitude);

                        }
                    }
                    else
                    {
                        if (inputType.IsHovering() && !selectedSquare.selected)
                        {
                            if (selectedSquare != (Object)hoveredObject)
                            {
                                if (hoveredObject != null && !hoveredObject.selected)
                                    hoveredObject.UnHighlight();
                            }

                            hoveredObject = selectedSquare;
                            hoveredObject.Highlight(Square.hoverHighlightMagnitude);

                        }


                    }
                }
            }
        }


        else if(inputPhase == InputPhase.Moving)
        {

            //If there are 2 fingers on the screen
            if (Input.touchCount >1)
            {
                //Gets both of the touches
                Touch touch1 = Input.GetTouch(0);
                Touch touch2 = Input.GetTouch(1);

                //Gets the positions of the touches last frame
                Vector2 previousPos1 = touch1.position - touch1.deltaPosition;
                Vector2 previousPos2 = touch2.position - touch2.deltaPosition;

                //Gets the distance between both fingers last frame
                float previousDistance = (previousPos1 - previousPos2).magnitude;
                //Gets the distance between both fingers this frame
                float currentDistance = (touch1.position - touch2.position).magnitude;

                //Gets the change in distance between the 2 fingers in this frame
                float distanceMagnitude = (currentDistance - previousDistance);
                //Multiplies it with scaleSpeed
                float changeAmount = distanceMagnitude * scaleSpeed;

                //What the new scale of the board will be
                Vector3 newScale = chessBoard.localScale + new Vector3(changeAmount, changeAmount, changeAmount);


                //If the new scale is bigger than the max scale, set the scale to the max scale
                if (newScale.x > maxBoardSize)
                    chessBoard.transform.localScale = new Vector3(maxBoardSize, maxBoardSize, maxBoardSize);

                //If the new scale is smaller than the min scale, set the scale to the min scale
                else if (newScale.x < minimumBoardSize)
                    chessBoard.transform.localScale = new Vector3(minimumBoardSize, minimumBoardSize, minimumBoardSize);

                //If the new scale is within the set bounds, set it to the new scale
                else
                    chessBoard.transform.localScale = newScale;


            }

            //If the player is selecting
            else if (inputType.IsHovering())
            {

                //Stores the data from the raycast
                TrackableHit trackableHit;

                //Shoots a ray that can only hit within the bounds of a tracked plane, and if it hits:
                if (Frame.Raycast(inputType.GetCursorPosition().x, inputType.GetCursorPosition().y, TrackableHitFlags.PlaneWithinBounds, out trackableHit))
                {
                    //Moves the chessboard to the selected point
                    chessBoard.position = trackableHit.Pose.position;

                }
            }
        }

        debugText.text = UIManager.Instance.sliderIsBeingDragged.ToString();

    }

    /// <summary>
    /// Switches between the moving and playing input phase
    /// </summary>
    public void ToggleInputPhase()
    {
        //If the game is in the placing phase
        if (inputPhase == InputPhase.Placing)
            //Don't do anything
            return;

        //If the game is in the playing phase
        if(inputPhase == InputPhase.Playing)
        {
            //Changes the phase
            inputPhase = InputPhase.Moving;
            //Changes the icon on the button
            UIManager.Instance.moveBoardButtonImage.sprite = UIManager.Instance.secondaryMoveBoardSprite;
            //Enables the rotationslider
            UIManager.Instance.rotationSlider.gameObject.SetActive(true);
            //Hides the tracked planes
            showTrackedPlanes = true;
        }
        //If the game is in the moving phase
        else
        {
            //Changes the phase
            inputPhase = InputPhase.Playing;
            //Changes the icon on the button
            UIManager.Instance.moveBoardButtonImage.sprite = UIManager.Instance.primaryMoveBoardSprite;
            //Disables the rotationslider
            UIManager.Instance.rotationSlider.gameObject.SetActive(false);
            //Shows the tracked planes
            showTrackedPlanes = false;
        }
    }


    /// <summary>
    /// Updates the rotation of the chess board based on the value of the slider
    /// </summary>
    public void UpdateChessBoardRotation()
    {
        //Gets the new y rotation based off a fraction of 360
        float newYRot = 360 * (UIManager.Instance.rotationSlider.value-0.5f);

        if (chessBoard != null)
        {
            //Sets the y rotation of the chessboard to the new y rotation
            chessBoard.rotation = Quaternion.Euler(new Vector3(0, -newYRot, 0));
        }
    }


}

interface IInputType
{
    bool IsHovering();
    bool IsSelecting();
    Vector3 GetCursorPosition();
}

class ComputerInputType : IInputType
{
    /// <summary>
    /// Will return the mouse position when using a computer
    /// </summary>
    /// <returns></returns>
    public Vector3 GetCursorPosition()
    {
        return Input.mousePosition;
    }

    /// <summary>
    /// On computer, the mouse is always hovering
    /// </summary>
    /// <returns></returns>
    public bool IsHovering()
    {
        return true;
    }

    /// <summary>
    /// The user is selecting something when the left mouse button is presssed
    /// </summary>
    /// <returns></returns>
    public bool IsSelecting()
    {
        //Returns the state of the left mouse button
        return Input.GetMouseButton(0);
    }


}

class MobileInputType : IInputType
{
    public Vector3 GetCursorPosition()
    {
        if (Input.touchCount > 0)
            return Input.touches[0].position;
        else
            return Vector3.zero;
    }

    public bool IsHovering()
    {
        return (Input.touchCount == 1 && Input.touches[0].phase != TouchPhase.Ended);
    }

    public bool IsSelecting()
    {
        return (Input.touchCount == 1 && Input.touches[0].phase == TouchPhase.Ended);
    }
}

interface IHighlightable
{
    void Highlight(float magnitude);
    void UnHighlight();
    bool selected { get; set; }
    void Select();
}

public enum InputType
{
    Computer,
    Mobile
}

public enum InputPhase
{
    Placing,
    Playing,
    Moving
}