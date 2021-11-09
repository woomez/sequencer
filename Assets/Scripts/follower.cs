using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;

public class follower : MonoBehaviour
{
    private SplineFollower car;
    public double pos;
    public Vector3 vec;
    public Quaternion rot;
    // Start is called before the first frame update
    void Start()
    {
        car = GetComponent<SplineFollower>();
    }

    // Update is called once per frame

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            pos = car.result.percent;
            vec = car.result.position;
            rot = car.result.rotation;
            Debug.Log(vec);
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            pos = car.result.percent;
            Debug.Log("key pressed");
        }
    }
}
