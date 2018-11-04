using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GoogleARCore;

public class SurfaceVisualizer : MonoBehaviour {

    //A surface to be instantiated
    public GameObject surface;

	void Update () {

        //If it's not currently tracking, return out of the loop
        if (Session.Status != SessionStatus.Tracking)
            return;

        //Stores the new planes that have been tracked this frame
        List<TrackedPlane> newPlanes = new List<TrackedPlane>();
        //Populates the list with the new planes tracked this frame
        Session.GetTrackables<TrackedPlane>(newPlanes, TrackableQueryFilter.New);

        //Loops through each new plane
        foreach(TrackedPlane plane in newPlanes)
        {
            //Instantiate a surface to represent the new plane
            GameObject newPlane = Instantiate(surface, Vector3.zero, Quaternion.identity);
            //Sets up this object to represent the new plane
            newPlane.GetComponent<Surface>().Initialize(plane);
        }

	}
}
