using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDocilable
{

    //List of functions needed for a docile enemy

    IEnumerator Searching();

    void FieldOfView();

    IEnumerator LookAround();

    IEnumerator StopAtWaypoint();

    IEnumerator LostSightDelay();

    IEnumerator Delay();

    void OnDrawGizmos();

}

public interface IAlertable
{

    //List of functions needed for a alert enemy

    //void Alert();

    void Attack();

    void FieldOfView();

    IEnumerator LookAround();

    void OnDrawGizmos();

}
