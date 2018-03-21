using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMelee
{
    
    //List of functions needed for a melee enemy

    void Attacking(); // Only attacking is needed here as it is what contains 'charge' functionality

}

public interface IRanged
{

    //List of functions needed for a ranged enemy

    void Attacking();

    void Shoot();

    //void FindCover

    //void Advance

    //void Flank

    //void Reposition

}

public interface IBoss
{

    //List of functions needed for a boss enemy

    void FieldOfView();

    IEnumerator LookAround();

    void OnDrawGizmos();

    void Shoot(); //Shoot hailstorm of bullets

    void ThrowExplosive();

    //WIP
}
