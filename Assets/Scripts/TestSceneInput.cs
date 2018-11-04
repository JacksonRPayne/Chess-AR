using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSceneInput : MonoBehaviour {

    public GameObject board;
	
	// Update is called once per frame
	void Update () {

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            Physics.Raycast(ray, out hit);

            Instantiate(board, hit.point, Quaternion.identity);
        }

	}
}
