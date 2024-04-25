using System.Collections.Generic;
using UnityEngine;
using System;
//using UnityEditor.Animations;

public class Rook : Chessman
{
    public override bool[,] PossibleMoves()
    {
        bool[,] r = new bool[8, 8];

        int i;

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

        if (BoardManager.Instance.iscaslting)
        {
            r[3, 0] = true;
            r[5, 0] = true;
            r[3, 7] = true;
            r[5, 7] = true;
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
        Vector2 currentTile = GetTile(pos);
        print("a1:" + currentTile + "," + selectedX + "," + selectedY);
        if (currentTile.x == selectedX - 1)
        {
            return true;
        }
        else if (currentTile.x == selectedX + 1)
        {
            return true;
        }
        else if (currentTile.y == selectedY - 1)
        {
            return true;
        }
        else if (currentTile.y == selectedY + 1)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    private Animator animator_obj;
    public List<string> parameters;
    public StatesPostionName[] states;
    private string current_state_name = "";
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
            for (int i = 0; i < vfxDetails[tempparamnumber].effects.Count; i++)// foreach (var obj in vfxDetails[tempparamnumber].effects)
            {
                WaitAfterCall((value) => { print("Enter Wait:" + value); 
                    vfxDetails[tempparamnumber].effects[value].SetActive(true); }, vfxDetails[tempparamnumber].effectsdelay[i], i);
            }
        }
        waiting_end_state = true;
        wait_for_anim_start = true;
        //current_state_name = states[paramnumber].endStateName;

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

