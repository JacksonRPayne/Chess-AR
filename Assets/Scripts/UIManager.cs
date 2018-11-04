using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    //Singleton
    public static UIManager Instance;

    public void Awake()
    {
        //Singleton
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    //Stores the button that is pressed to move the board
    public Button moveBoardButton;

    //The image under the move board button
    public Image moveBoardButtonImage;
    //The primary icon on the image
    public Sprite primaryMoveBoardSprite;
    //The secondary icon on the image
    public Sprite secondaryMoveBoardSprite;

    //The slider that controls rotation of the chess board
    public Slider rotationSlider;

    //This will be true if any slider is being dragged
    public bool sliderIsBeingDragged = false;

    /// <summary>
    /// This is called whenever a slider begins being dragged
    /// </summary>
    public void StartSliderDrag()
    {
        sliderIsBeingDragged = true;
    }

    /// <summary>
    /// This is called whenever a slider stops being dragged
    /// </summary>
    public void EndSliderDrag()
    {
        sliderIsBeingDragged = false;
    }

}
