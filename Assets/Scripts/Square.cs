using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour, IHighlightable {

    //Stores the materials for the light square and the dark square
    public Material lightMaterial;
    public Material darkMaterial;

    //Stores the position on the board this square is at
    public BoardPosition boardPosition;

    //The current piece on the square
    public Piece currentPiece;

    //List of all the renderers in the children (if there is to be multiple objects making up the graphics)
    private Renderer[] renderers;

    //The original colors of the graphics objects
    private Color[] origninalColors;

    //Stores all squares with a highlight applied
    private static List<Square> highlightedSquares;

    //These store the magnitudes that the color of the
    //material will change when hovered and selected
    public static float selectHighlightMagnitude = 0.25f;
    public static float hoverHighlightMagnitude = 0.15f;

    //Represents whether or not the square is selected
    public bool selected { get; set; }

    //The width of the outline when selected
    public float selectedSquareOutlineWidth = 1.1f;

    private void Start()
    {
        //Init
        selected = false;

        //Gets the renderers of the children (graphics objects)
        renderers = GetComponentsInChildren<Renderer>();

        //Sets it to be the amount of renderers in the children
        origninalColors = new Color[renderers.Length];

        //Sets up the original colors to be corresponding to the renderers
        for (int i = 0; i < renderers.Length; i++)
        {
            origninalColors[i] = renderers[i].material.color;
        }

        highlightedSquares = new List<Square>();

    }

    /// <summary>
    /// Highlights the square
    /// </summary>
    public void Highlight(float magnitude)
    {

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = origninalColors[i] - new Color(magnitude, magnitude, magnitude);
        }

        highlightedSquares.Add(this);

    }

    /// <summary>
    /// Removes all highlight from the square
    /// </summary>
    public void UnHighlight()
    {
        //Sets the colors back to the original ones
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = origninalColors[i];
        }

        highlightedSquares.Remove(this);
        selected = false;

        renderers[0].material.SetFloat("_OutlineWidth", 1.0f);
        renderers[0].material.renderQueue = 3000;
    }

    public void Select()
    {
        renderers[0].material.SetFloat("_OutlineWidth", selectedSquareOutlineWidth);
        renderers[0].material.renderQueue = 5000;
        highlightedSquares.Add(this);
        selected = true;
    }

    /// <summary>
    /// Unhighlights every square on the board
    /// </summary>
    public static void ResetBoardHighlights()
    {
        //Needed for unhighlighting because if you iterated
        //through the highlightedSquares list, unhighlighting
        //each one, that would remove it and therefore change
        //the list during the iteration
        List<Square> temp = new List<Square>();

        //Creates temp - a replica of highlightedSquares
        foreach (Square s in highlightedSquares)
        {
            temp.Add(s);
        }

        //Unhighlights the squares in temp
        foreach (Square s in temp)
        {
            s.UnHighlight();
        }

    }

}
