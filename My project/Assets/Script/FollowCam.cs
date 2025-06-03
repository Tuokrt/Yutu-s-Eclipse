using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform yutu; 
    public float smoothTime = 0.3f;
    public Vector3 offset = new Vector3(2, 0,0);
    public float followSpeed = 5f;
    public float yPosition = 0f;
    public float xPosition;
    public float zPosition = -10f;
    public float xOffset = 2f;

    // Start is called before the first frame update
    void Start()
    {
        xPosition = yutu.position.x;
        transform.position = new Vector3(xPosition + xOffset, yPosition,zPosition);
    }

   
    void LateUpdate()
    {
        xPosition = yutu.position.x;
        Vector3 targetvector3 = new Vector3 (xPosition+xOffset, yPosition,zPosition);
        transform.position = Vector3.Lerp(transform.position, targetvector3, followSpeed * Time.deltaTime); ;
    }
}
