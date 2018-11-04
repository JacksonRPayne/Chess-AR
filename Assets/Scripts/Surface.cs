using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GoogleARCore;

public class Surface : MonoBehaviour {

    //The plane this surface represents
    TrackedPlane trackedPlane;
    //The meshrenderer attached to this object
    MeshRenderer meshRenderer;
    //The mesh for this object
    Mesh mesh;

    /// <summary>
    /// Sets the plane that this surface is representing
    /// </summary>
    /// <param name="p"></param>
    public void Initialize(TrackedPlane p)
    {
        trackedPlane = p;
    }

    void Start () {

        //Gets reference to mesh renderer
        meshRenderer = GetComponent<MeshRenderer>();
        //Initializes the mesh
        mesh = new Mesh();
        //Sets the mesh as the one being used by the mesh filter
        GetComponent<MeshFilter>().mesh = mesh;

    }
	
	void Update () {

        //If the state of the game calls for no planes to be shown
        if (!InputController.showTrackedPlanes)
        {
            //Hide this plane and return from the loop
            meshRenderer.enabled = false;
            return;
        }

        //If this surface doesn't have a plane, then return from the loop
        if (trackedPlane == null)
            return;
        //If the plane has been subsumed by another plane
        else if (trackedPlane.SubsumedBy != null)
        {
            //Get rid of this gameobject
            Destroy(gameObject);
            return;
        }
        //If the plane isn't being tracked, or the program isn't currently tracking
        else if(trackedPlane.TrackingState != TrackingState.Tracking || Session.Status != SessionStatus.Tracking)
        {
            //Hide this object and return from the loop
            meshRenderer.enabled = false;
            return;
        }

        //Set this object to be visible
        meshRenderer.enabled = true;
        //Updates the verticies of the mesh
        UpdateVerticies();

	}

    /// <summary>
    /// Updates the verticies of the mesh to represent the tracked plane
    /// </summary>
    void UpdateVerticies()
    {
        //Represents the outside verticies of the plane in a clockwise order
        List<Vector3> verticies = new List<Vector3>();
        //Populates the list with the verticies
        trackedPlane.GetBoundaryPolygon(verticies);

        //Adds the center point to the list, in order to make triangles
        verticies.Add(trackedPlane.CenterPose.position);

        //Represents the indexes of the vertex array that make up the triangles
        List<int> triangles = new List<int>();

        //Loops through all the outside verticies (-1 because we added the center vertex)
        for(int i = 0; i<verticies.Count-1; i++)
        {
            //Every triangle starts with the center point
            triangles.Add(verticies.Count - 1);
            //Then the currently iterated outside vertex
            triangles.Add(i);
            //Adds the vertex after the currently iterated one, unless
            //it's on the last vertex, then it adds the first one
            triangles.Add(i == verticies.Count - 2 ? 0 : i + 1);
        }

        //Sets the vertices of our mesh to the vertice array we created
        mesh.SetVertices(verticies);
        //Sets the triangles by passing in the ineger list of indexes in the 
        //vertex list, converted into an array
        mesh.SetTriangles(triangles.ToArray(), 0);

        //Recalculates bounds and normals to compensate for new change
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

}
