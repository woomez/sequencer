using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;

public class positioner : MonoBehaviour
{
    public double pos;
    private SplinePositioner testcar;
    // Start is called before the first frame update
    void Start()
    {
        testcar = GetComponent<SplinePositioner>();
        GameObject tester = GameObject.Find("Spline");
        trigger_sound trig = tester.GetComponent<trigger_sound>();
        pos = trig.curr_pos;
    }

    // Update is called once per frame
    void Update()
    {
        testcar.SetPercent(pos);
    }
}
