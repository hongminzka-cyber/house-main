using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float walkSpeed = 2f;
    public float standingHeight = 0.4f;

    [HideInInspector] public Cube current;
    [HideInInspector] public LinkedList<Cube> path;
}