using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private Transform  cam;
    [SerializeField] private float relativeMove = .3f;
    [SerializeField] private bool lockY = false;

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector2(cam.position.x * relativeMove, lockY ? transform.position.y : cam.position.y * relativeMove);
    }
}
