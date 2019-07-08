
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]

public class SmokeTrail : MonoBehaviour
{

    [SerializeField] private int numberOfPoints = 10;
    [SerializeField] private float updateSpeed = 0.25f;
    [SerializeField] private float riseSpeed = 0.25f;
    [SerializeField] private float spread = 0.2f;

    private LineRenderer line;
    private Material lineMaterial;
    private Transform tr;
    private Vector3[] positions, directions;
    private Vector3 tempVec;

    private float timeSinceUpdate = 0.0f;
    private float lineSegment = 0.0f;
    private int currentNumberOfPoints = 2;
    private int i;
    private bool allPointsAdded = false;



    void Start()
    {
        tr = this.transform;
        line = GetComponent<LineRenderer>();
        lineMaterial = line.material;

        lineSegment = 1.0f / numberOfPoints;

        positions =  new Vector3[numberOfPoints];
        directions = new Vector3[numberOfPoints];

        line.positionCount = currentNumberOfPoints;

        for (int i = 0; i < currentNumberOfPoints; i++)
        {
            tempVec = GetSmokeVec();
            directions[i] = tempVec;
            positions[i] = tr.position;
            line.SetPosition (i, positions[i]);
        }
    }
	

    void Update()
    {
        timeSinceUpdate += Time.deltaTime; //Update time

        //If it's time to update the line
        if (timeSinceUpdate > updateSpeed)
        {
            timeSinceUpdate -= updateSpeed;

            //Add points until the target number is reached
            if (!allPointsAdded)
            {
                currentNumberOfPoints++;
                line.positionCount = currentNumberOfPoints;
                tempVec = GetSmokeVec();
                directions[0] = tempVec;
                positions[0] = tr.position;
                line.SetPosition (0, positions[0]);
            }

            if (!allPointsAdded && (currentNumberOfPoints == numberOfPoints))
            {
                allPointsAdded = true;
            }

            //Make each point in the line take the position and direction of the one before it
            for (int i = currentNumberOfPoints - 1; i > 0; i--)
            {
                tempVec = positions[i-1];
                positions[i] = tempVec;
                tempVec = directions[i-1];
                directions[i] = tempVec;
            }
            tempVec = GetSmokeVec();
            directions[0] = tempVec; //Remember and give 0th point a direction when it gets pulled up the chain in the next line of update

        }

        //Update the line
        for (int i = 1; i < currentNumberOfPoints; i++)
        {
            tempVec = positions[i];
            tempVec += directions[i] * Time.deltaTime;
            positions[i] = tempVec;

            line.SetPosition (i, positions[i]);
        }
        positions[0] = tr.position; //0th point is a special case, always follow transform directly
        line.SetPosition(0, tr.position);

        //if at max number of points, tweak offset so last segment is invisible to prevent jarring/texture jumping
        if (allPointsAdded)
        {
            float offset = lineMaterial.mainTextureOffset.x;
            offset = lineSegment * (timeSinceUpdate / updateSpeed);
        }

    }

    //Give a random upwards vector
    Vector3 GetSmokeVec ()
    {
        Vector3 smokeVec;

        smokeVec.x = Random.Range (-1.0f, 1.0f);
        smokeVec .y = Random.Range (0.0f, 1.0f);
        smokeVec.z = Random.Range (-1.0f, 1.0f);
        smokeVec.Normalize();
        smokeVec *= spread;
        smokeVec.y += riseSpeed;
        return smokeVec;
    }


}
