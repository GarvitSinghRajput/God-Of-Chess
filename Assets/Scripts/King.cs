using System.Collections.Generic;
using UnityEngine;
using System;
//using UnityEditor.Animations;


public class King : Chessman
{
    Dictionary<Chessman, bool[,]> moveList = new Dictionary<Chessman, bool[,]>();
    public bool isChecked = false;
    public bool[,] movesPossible;
    public bool canCastle = true;
    int castleSide = 0; // 1 = Right side ; 2 = Left side ; 3 = Both sides
    public override bool[,] PossibleMoves()
    {
        bool[,] r = new bool[8, 8];
        //BoardManager.Instance.GetChessMoves();
        moveList = bManager.moveList;

        NewMove(CurrentX + 1, CurrentY, ref r); // up
        NewMove(CurrentX - 1, CurrentY, ref r); // down
        NewMove(CurrentX, CurrentY - 1, ref r); // left
        NewMove(CurrentX, CurrentY + 1, ref r); // right
        NewMove(CurrentX + 1, CurrentY - 1, ref r); // up left
        NewMove(CurrentX - 1, CurrentY - 1, ref r); // down left
        NewMove(CurrentX + 1, CurrentY + 1, ref r); // up right
        NewMove(CurrentX - 1, CurrentY + 1, ref r); // down right

        if (!hasMoved) {
            if (CheckCastling())
                CastlingMove(ref r);
        }    
        return r;
    }

    public bool NewMove(int x, int y, ref bool[,] r)
    {
        bool r_new = CheckKingMoves(x,y);
        //bool r_new = IsMovePossible(x,y);
        //Debug.LogWarning(r_new + " at : " + x +" , " + y);
        if (x >= 0 && x < 8 && y >= 0 && y < 8)
        {
            Chessman c = bManager.Chessmans[x, y];

            if (c == null && r_new)
                r[x, y] = true;
            else if (c != null && r_new)
            {
                if (isWhite != c.isWhite)
                    r[x, y] = true;
                return true;
            }
            else
            {
                r[x, y] = false;
                return true;
            }
        }
        return false;
    }

    public bool CheckKingMoves(int x, int y)
    {
        if (x < 0 || x > 7 || y < 0 || y > 7 || moveList == null)
            return false;
        foreach (KeyValuePair<Chessman,bool[,]> kvp in moveList)
        {
            if (isWhite != kvp.Key.isWhite)
            {
                if (kvp.Value[x, y] == true)
                    return false;
            }
        }
        return true;
    }

    private bool CheckCastling()
    {
        if (this.isChecked) return false;
        Chessman c1 = bManager.Chessmans[CurrentX + 2, CurrentY];
        Chessman c2 = bManager.Chessmans[CurrentX + 1, CurrentY];
        Chessman c3 = bManager.Chessmans[CurrentX - 1, CurrentY];
        Chessman c4 = bManager.Chessmans[CurrentX - 2, CurrentY];
        Chessman c5 = bManager.Chessmans[CurrentX - 3, CurrentY];
        if (c1 == null && c2 == null && canMoveAt(CurrentX + 2, CurrentY) && canMoveAt(CurrentX + 1, CurrentY) &&
            c3 == null && c4 == null && c5 == null && canMoveAt(CurrentX - 1, CurrentY) &&
                canMoveAt(CurrentX - 2, CurrentY) && canMoveAt(CurrentX - 3, CurrentY))
        {
            if (bManager.Chessmans[0, CurrentY] != null && bManager.Chessmans[7, CurrentY] != null &&
                !bManager.Chessmans[0, CurrentY].hasMoved && !bManager.Chessmans[7, CurrentY].hasMoved)
            {
                castleSide = 3;
                return true;
            }else if (bManager.Chessmans[7, CurrentY] != null && !bManager.Chessmans[7, CurrentY].hasMoved)
            {
                castleSide = 1;
                return true;
            }
            else if (bManager.Chessmans[0, CurrentY] != null && !bManager.Chessmans[0, CurrentY].hasMoved)
            {
                castleSide = 2;
                return true;
            }
        }
        else if (c1 == null && c2 == null && canMoveAt(CurrentX + 2, CurrentY) && canMoveAt(CurrentX + 1, CurrentY))
        {
            if (bManager.Chessmans[7, CurrentY] != null && !bManager.Chessmans[7, CurrentY].hasMoved)
            {
                castleSide = 1;
                return true;
            }
        }
        else if (c3 == null && c4 == null && c5 == null && canMoveAt(CurrentX - 1, CurrentY) &&
            canMoveAt(CurrentX - 2, CurrentY) && canMoveAt(CurrentX - 3, CurrentY))
        {
            if (bManager.Chessmans[0, CurrentY] != null && !bManager.Chessmans[0, CurrentY].hasMoved)
            {
                castleSide = 2;
                return true;
            }
        }
        castleSide = 0;
        return false;
    }

    private void CastlingMove(ref bool[,] r)
    {
        if (isChecked)
            return;
        if (castleSide == 3)                        //Can moves both sides.
        {                          
                NewMove(CurrentX - 1, CurrentY, ref r);
                NewMove(CurrentX - 2, CurrentY, ref r);

                NewMove(CurrentX + 1, CurrentY, ref r);
                NewMove(CurrentX + 2, CurrentY, ref r);
        }
        else if (castleSide == 2)                   //Can only move left side.
        {
                NewMove(CurrentX - 1, CurrentY, ref r);
                NewMove(CurrentX - 2, CurrentY, ref r);
        }
        else if (castleSide == 1)                   //Can move only right side.
        {
                NewMove(CurrentX + 1, CurrentY, ref r);
                NewMove(CurrentX + 2, CurrentY, ref r);
        }
    }

    public override bool AllowMoves(int selectedX = -1, int selectedY = -1, Vector3 pos = default)
    {
        Chessman c = bManager.Chessmans[selectedX, selectedY];
        if (c == null) return false;
        Vector2 currentTile = GetTile(pos);
        //print("a1:"+ currentTile+","+ selectedX+","+ selectedY);
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
        else if (currentTile.x == selectedX - 1 && currentTile.y == selectedY + 1)
        {
            return true;
        }
        else if (currentTile.x == selectedX + 1 && currentTile.y == selectedY + 1)
        {
            return true;
        }
        else if (currentTile.x == selectedX - 1 && currentTile.y == selectedY - 1)
        {
            return true;
        }
        else if (currentTile.x == selectedX + 1 && currentTile.y == selectedY - 1)
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
    public VFXDetails[] vfxDetails;
    private string current_state_name = "";
    private bool waiting_end_state = false;
    private bool wait_for_anim_start = false;
    private Action _normal_callback = null;
    private int tempparamnumber;
    private BoardManager bManager;


    // Use this for initialization
    void Start()
    {
        if (isWhite)
            gameObject.tag = GlobalVariables.TAG_ISWHITEKING;
        else
            gameObject.tag = GlobalVariables.TAG_ISBLACKKING;
        animator_obj = GetComponent<Animator>();

        foreach (var cont in animator_obj.parameters)
        {
            parameters.Add(cont.name);
        }
        bManager = BoardManager.Instance;
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

    public override void level_complete(int parnum, Action can_get_callback = null)
    {
        if (can_get_callback != null)
            this._normal_callback = can_get_callback;
        if (parnum == 1)
            tempparamnumber = states.Length - 1;
        else
            tempparamnumber = paramnumber;
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


