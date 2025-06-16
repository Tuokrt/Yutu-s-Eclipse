using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bgFollow : MonoBehaviour
{
    public Transform eyutu;
    public Vector3 offset = new Vector3 (6, 0, 10);

    // Start is called before the first frame update
    void Start()
    {
        transform.position = eyutu.position + offset;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = eyutu.position+ offset;
    }
}
