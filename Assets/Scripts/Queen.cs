using System.Collections.Generic;
using UnityEngine;
using System;
//using UnityEditor.Animations;


public class Queen : Chessman
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

        // Right
        i = CurrentX;
        while (true)
        {
            i++;
            if (i >= 8) break;

            if (Move(i, CurrentY, ref r)) break;
        }

        // Left
        i = CurrentX;
        while (true)
        {
            i--;
            if (i < 0) break;

            if (Move(i, CurrentY, ref r)) break;
        }

        // Up
        i = CurrentY;
        while (true)
        {
            i++;
            if (i >= 8) break;

            if (Move(CurrentX, i, ref r)) break;
        }

        // Down
        i = CurrentY;
        while (true)
        {
            i--;
            if (i < 0) break;

            if (Move(CurrentX, i, ref r)) break;

        }


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
        else return true;
    }

    private Animator animator_obj;
    public List<string> parameters;
    public StatesPostionName[] states;
    //private string current_state_name = "";
    private bool waiting_end_state = false;
    private bool wait_for_anim_start = false;
    // Use this for initialization
    void Start()
    {
        animator_obj = GetComponent<Animator>();

        foreach (var cont in animator_obj.parameters)
        {
            parameters.Add(cont.name);
        }
    }
    bool test;
    float t, timeToReachTarget;
    Vector3 startPosition, target;
    public float[] arrowTime;
    public GameObject startposObj;

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
                    if (tempparamnumber < states.Length - 1 && normalizedTime >= arrowTime[tempparamnumber])
                    {
                        if (vfxDetails[tempparamnumber].attackType == AttackType.Projectile || vfxDetails[tempparamnumber].attackType == AttackType.GroundProjectile)   // For projectiles
                        {
                            if (!test)
                            {

                                test = true;
                                for (int i = 0; i < vfxDetails[tempparamnumber].effects.Count; i++)
                                {
                                    WaitAfterCall((value) =>
                                    {
                                        vfxDetails[tempparamnumber].effects[value].transform.position = vfxDetails[tempparamnumber].attackType == 
                                        AttackType.GroundProjectile ? transform.position : startposObj.transform.position;
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
                        else if(vfxDetails[tempparamnumber].attackType == AttackType.InkoveAtEnemyFeet)   //For Standing Attack directly at enemy position like earthquake
                        {
                            if (!test)
                            {

                                test = true;
                                for (int i = 0; i < vfxDetails[tempparamnumber].effects.Count; i++)
                                {
                                    WaitAfterCall((value) =>
                                    {
                                        vfxDetails[tempparamnumber].effects[value].transform.position = BoardManager.Instance.Chessmans[BoardManager.Instance.selectionX, BoardManager.Instance.selectionY].transform.position;
                                        vfxDetails[tempparamnumber].effects[value].SetActive(true);
                                        print("Enter Wait:" + value);

                                    }, vfxDetails[tempparamnumber].effectsdelay[i], i);
                                }
                            }

                        }
                        else    // For Standing Attacks from player to enemy like a laser
                        {
                            if (!test)
                            {

                                test = true;
                                for (int i = 0; i < vfxDetails[tempparamnumber].effects.Count; i++)
                                {
                                    WaitAfterCall((value) =>
                                    {
                                        vfxDetails[tempparamnumber].effects[value].transform.position = startposObj.transform.position;
                                        vfxDetails[tempparamnumber].effects[value].SetActive(true);
                                        print("Enter Wait:" + value);

                                    }, vfxDetails[tempparamnumber].effectsdelay[i], i);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                print("end");
                check_end_state();
            }
        }
    }

    private Action _normal_callback = null;
    private int tempparamnumber;
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
            SetDestination(BoardManager.Instance.Chessmans[BoardManager.Instance.selectionX,
                BoardManager.Instance.selectionY].transform.position + new Vector3(0, 0.65f, 0), 1.05f);
        }
        waiting_end_state = true;
        wait_for_anim_start = true;
        //current_state_name = states[paramnumber].endStateName;

    }

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


