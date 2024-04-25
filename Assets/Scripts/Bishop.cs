using System.Collections.Generic;
using UnityEngine;
using System;
//using UnityEditor.Animations;


public class Bishop : Chessman
{
    public override bool[,] PossibleMoves()
    {

        bool[,] r = new bool[8, 8];

        int i, j;

        // Top left
        i = CurrentX;
        j = CurrentY;
        while (true)
        {
            i--;
            j++;
            if (i < 0 || j >= 8) break;

            if (Move(i, j, ref r)) break;
        }

        // Top right
        i = CurrentX;
        j = CurrentY;
        while (true)
        {
            i++;
            j++;
            if (i >= 8 || j >= 8) break;

            if (Move(i, j, ref r)) break;
        }

        // Down left
        i = CurrentX;
        j = CurrentY;
        while (true)
        {
            i--;
            j--;
            if (i < 0 || j < 0) break;

            if (Move(i, j, ref r)) break;
        }

        // Down right
        i = CurrentX;
        j = CurrentY;
        while (true)
        {
            i++;
            j--;
            if (i >= 8 || j < 0) break;

            if (Move(i, j, ref r)) break;
        }

        //if (!this.isCheckingPrediction)
        //{
            if (isWhite)
            {
                if (BoardManager.Instance.isWhiteKingChecked)
                    return PossibleMovesOnCheck(r);
            }
            else
            {
                if (BoardManager.Instance.isBlackKingChecked)
                    return PossibleMovesOnCheck(r);
            }
        //}

        if (!this.isCheckingPrediction && !BoardManager.Instance.isGettingMoves && !BoardManager.Instance.isSettingAttackerAndKing)
        {
            this.isCheckingPrediction = true;
            r = AreNextMovesValid(r);
        }
        return r;
    }


    public override bool AllowMoves(int selectedX = -1, int selectedY = -1, Vector3 pos = default)
    {
        Chessman c = BoardManager.Instance.Chessmans[selectedX, selectedY];
        if (c == null) return false;
        else
        {
            return true;
        }
    }

    public GameObject bowObj;
    public GameObject ArrowObj;
    private Animator animator_obj;
    public List<string> parameters;
    public StatesPostionName[] states;
    private string current_state_name = "";
    public bool waiting_end_state = false;
    public bool wait_for_anim_start = false;

    public float[] arrowTime;
    private Vector3 arrowOriPos;
    private Transform arrowOriParent;

    // Use this for initialization
    void Start()
    {
        animator_obj = GetComponent<Animator>();
        foreach (var cont in animator_obj.parameters)
        {
            parameters.Add(cont.name);
        }
        if (ArrowObj)
        {
            arrowOriPos = ArrowObj.transform.localPosition;
            arrowOriParent = ArrowObj.transform.parent;
        }
    }

    bool test;


    // Update is called once per frame
    void Update()
    {
        if (waiting_end_state)
        {
            if (wait_for_anim_start)
            {
                if (isPlaying(animator_obj, states[tempparamnumber].endStateName)/*animator_obj.GetCurrentAnimatorStateInfo(0).IsName(states[tempparamnumber].endStateName)*/)
                {
                    wait_for_anim_start = false;
                }
                if (animator_obj.GetCurrentAnimatorStateInfo(0).IsName(states[tempparamnumber].endStateName))
                {
                    float normalizedTime = animator_obj.GetCurrentAnimatorStateInfo(0).normalizedTime;
                    if (ArrowObj && tempparamnumber < states.Length - 1 && normalizedTime >= arrowTime[tempparamnumber])
                    {
                        if (!test)
                        {
                            test = true;
                            for (int i = 0; i < vfxDetails[tempparamnumber].effects.Count; i++)
                            {
                                WaitAfterCall((value) =>
                                {
                                    vfxDetails[tempparamnumber].effects[value].transform.position = bowObj.transform.GetChild(1).position;
                                    vfxDetails[tempparamnumber].effects[value].SetActive(true);
                                    print("Enter Wait:" + value);
                                }, vfxDetails[tempparamnumber].effectsdelay[i], i);
                            }
                        }
                        t += Time.deltaTime / timeToReachTarget;
                        for (int i = 0; i < vfxDetails[tempparamnumber].effects.Count; i++)
                        {
                            if (vfxDetails[tempparamnumber].effects[i].activeSelf)
                            {
                                print(vfxDetails[tempparamnumber].effects[i].name);
                                vfxDetails[tempparamnumber].effects[i].transform.position = Vector3.MoveTowards(vfxDetails[tempparamnumber].effects[i].transform.position, target, t);
                            }
                        }
                    }
                }
            }
            else
            {
                print("end");
                check_end_state();
                ArrowObj.transform.GetChild(0).gameObject.SetActive(true);
            }
        }
    }

    private Action _normal_callback = null;
    public int tempparamnumber;
    public VFXDetails[] vfxDetails;

    public override void level_complete(int parnum, Action can_get_callback = null)
    {
        if (can_get_callback != null)
            this._normal_callback = can_get_callback;
        if (parnum == 1)
            tempparamnumber = states.Length - 1;
        else
            tempparamnumber = paramnumber;
        //paramnumber = parnum;
        print("level_complete:" + parameters[tempparamnumber]);
        animator_obj.SetBool(parameters[tempparamnumber], true);
        if (tempparamnumber != states.Length - 1)
        {
            SetDestination(BoardManager.Instance.Chessmans[BoardManager.Instance.selectionX, BoardManager.Instance.selectionY].transform.position + new Vector3(0, 0.65f, 0), 1.05f);
            ArrowObj.transform.GetChild(0).gameObject.SetActive(false);
        }
        waiting_end_state = true;
        wait_for_anim_start = true;
        //current_state_name = states[paramnumber].endStateName;

    }
    float t, timeToReachTarget;
    Vector3 startPosition, target;
    public void SetDestination(Vector3 destination, float time)
    {
        test = false;
        t = 0;
        //vfxDetails[tempparamnumber].effects[0].transform.position= startPosition = bowObj.transform.GetChild(1).position;
        timeToReachTarget = time;
        target = destination;
    }
    public void check_end_state()
    {
        print(animator_obj.GetCurrentAnimatorStateInfo(0).IsName(states[tempparamnumber].endStateName));
        if (isPlaying(animator_obj, states[tempparamnumber].endStateName)/*animator_obj.GetCurrentAnimatorStateInfo(0).IsName(states[tempparamnumber].endStateName)*/)
        {
            waiting_end_state = false;
            //if (current_state_name == states[paramnumber])
            {
                animator_obj.SetBool(parameters[tempparamnumber], false);
                if (tempparamnumber != states.Length - 1)
                {
                    foreach (var obj in vfxDetails[tempparamnumber].effects)
                    {
                        obj.SetActive(false);
                    }
                }
                print("animation has been ended");
                //transform.position = GetTileCenter(CurrentX, CurrentY);
                if (this._normal_callback != null)
                {
                    this._normal_callback();
                    this._normal_callback = null;
                }
            }
        }
    }
}



