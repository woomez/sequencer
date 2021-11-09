using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;

public class trigger_sound : MonoBehaviour
{
    //public Material ShinyBlue;
    private SplineComputer _spline;
    ChuckSubInstance chuckk;
    public GameObject testcar;
    public GameObject pickup1;
    public GameObject pickup2;
    private GameObject car;
    private follower carpos;
    public double curr_pos;
    // selector prefab
    public GameObject the_selectorPrefab;
    int inst = 0;
    int mode;

    SplineSample sample;

    //--------- GRAPHICS -----------
    // number of chickens (MUST match NUM_CHICKENS in ChucK code)
    int NUM_CHICKENS = 32;
    // the global chicken array
    GameObject[] m_chickens;
    // the chicken sequencer playhead
    public GameObject m_playhead;
    // the selector thingy over the selected chicken
    GameObject m_selector;

    // reference to animator component
    private Animator m_animPlayhead;
    // time step for run animation
    private float m_animPlayheadTimeRun;
    // which chicken is selected for editing
    private int m_selectedChicken = 0;

    // --------- AUDIO -------------
    // float sync
    private ChuckFloatSyncer m_ckPlayheadPos;
    // int sync
    private ChuckIntSyncer m_ckCurrentChicken;
    bool isStatic = true;
    // --------- SHARED ------------
    // sequence: this information is duplicated across Chuck and Unity
    // and is communicated to chuck using ChucK events
    private float[] m_seqRate;
    private float[] m_seqGain;
    private int[] m_seqInst;
    // previous discrete chicken number
    private int m_previousChicken = -1;

    void Start()
    {
        Application.targetFrameRate = 50;
        chuckk = GetComponent<ChuckSubInstance>();
        GameObject car = GameObject.Find("Red_Car");
        carpos = car.GetComponent<follower>();
        _spline = GetComponent<SplineComputer>();
        //car = GetComponent<SplineFollower>();
        InitGraphics();
        InitAudio();
        InitSequence();
    }

    void InitAudio()
    {
        // run the sequencer
        GetComponent<ChuckSubInstance>().RunFile("chickencer.ck", true);
        Debug.Log("in2");
        // add the float sync
        m_ckPlayheadPos = gameObject.AddComponent<ChuckFloatSyncer>();
        m_ckPlayheadPos.SyncFloat(GetComponent<ChuckSubInstance>(), "playheadPos");
        // add the int sync
        m_ckCurrentChicken = gameObject.AddComponent<ChuckIntSyncer>();
        m_ckCurrentChicken.SyncInt(GetComponent<ChuckSubInstance>(), "currentChicken");
    }

    void InitGraphics()
    {
        // instantiate chicken array
        m_chickens = new GameObject[NUM_CHICKENS];

        // make chickens
        for (int i = 0; i < NUM_CHICKENS; i++)
        {
            sample = _spline.Evaluate(0.03030303 * i);
            m_chickens[i] = Instantiate(pickup1, sample.position, sample.rotation);
            m_chickens[i].transform.localScale = new Vector3(0, 0, 0);
        }

        // instantiate the playhead
        m_playhead = Instantiate(testcar, new Vector3(0, 0, 0), Quaternion.Euler(0, 90, 0));
        // move the playhead
        m_playhead.transform.position = _spline.Evaluate(0).position;
        m_playhead.transform.rotation = _spline.Evaluate(0).rotation;
        // scale the playhead
        m_playhead.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

        // the first chicken
        GameObject c = m_chickens[0];
        //instantiate the selector
        m_selector = Instantiate(the_selectorPrefab, _spline.Evaluate(0).position, _spline.Evaluate(0).rotation);
        m_selector.transform.localScale = new Vector3(2.5f, 1f, 2.5f);
        // position the selector
        PositionSelector();
    }

    void InitSequence()
    {
        // sequence, the rate for each element
        m_seqRate = new float[NUM_CHICKENS];
        // sequence, the volume for each element
        m_seqGain = new float[NUM_CHICKENS];
        m_seqInst = new int[NUM_CHICKENS];
        // initialize
        for (int i = 0; i < NUM_CHICKENS; i++)
        {
            m_seqRate[i] = 1.0f;
            m_seqGain[i] = 0.0f;
            //m_seqInst[i] = 0;
        }
    }


    // Update is called once per frame
    void Update()
    {
        curr_pos = m_ckPlayheadPos.GetCurrentValue();
        int currentChicken = m_ckCurrentChicken.GetCurrentValue();
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (isStatic == true)
            {
                mode = currentChicken;
                isStatic = false;
            }
            else
            { mode = m_selectedChicken;
                isStatic = true;
            }

        }

        if (isStatic == false)
        { mode = currentChicken; }
        if (isStatic == true)
        { mode = m_selectedChicken; }

        //m_playhead.transform.position = _spline.Evaluate(0.03030303 * currentChicken).position;
        //m_playhead.transform.rotation = _spline.Evaluate(0.03030303 * currentChicken).rotation;
        m_playhead.transform.position = _spline.Evaluate(0.03030303 * currentChicken).position;
        m_playhead.transform.rotation = _spline.Evaluate(0.03030303 * currentChicken).rotation;
        m_selector.transform.position = _spline.Evaluate(m_selectedChicken * 0.03030303).position;

        // get input (for chicken editing)
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            m_selectedChicken--;
            if (m_selectedChicken < 0) m_selectedChicken = NUM_CHICKENS - 1;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            m_selectedChicken++;
            if (m_selectedChicken >= NUM_CHICKENS) m_selectedChicken = 0;
        }
        else if (Input.GetKeyDown(KeyCode.H))
        {
            AdjustGain(mode, 0);
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            AdjustGain(mode, 1);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            AdjustGain(mode, 2);
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            AdjustGain(mode, 2);
        }
    }

    void PositionSelector()
    {
        // move the playhead
        m_selector.transform.position = _spline.Evaluate(m_selectedChicken * 0.03030303).position;
        
    }


    void AdjustGain(int which, int inst)
    {
        // ajdust
        if (m_seqGain[which] == 0)
            m_seqGain[which] = 0.7f;
        else
        {
            m_seqGain[which] = 0;
        }

        m_seqInst[which] = inst;

        // scale according to rate and gain
        m_chickens[which].transform.localScale = new Vector3(m_seqRate[which]*10, m_seqGain[which]*10, m_seqGain[which]*10);

        // set which and rate and fire the event
        GetComponent<ChuckSubInstance>().SetInt("editWhich", which);
        GetComponent<ChuckSubInstance>().SetFloat("editRate", m_seqRate[which]);
        GetComponent<ChuckSubInstance>().SetFloat("editGain", m_seqGain[which]);
        GetComponent<ChuckSubInstance>().SetInt("inst", m_seqInst[which]);
        GetComponent<ChuckSubInstance>().BroadcastEvent("editHappened");
    }

}
