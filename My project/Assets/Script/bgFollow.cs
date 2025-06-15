using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bgFollow : MonoBehaviour
{
    public Transform ecamera;
    public Vector3 offset = new Vector3 (0, 0, 10);

    // Start is called before the first frame update
    void Start()
    {
        transform.position = ecamera.position + offset;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = ecamera.position+ offset;
    }
}
