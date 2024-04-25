using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;
//using UnityEditor.Animations;

public class Pawn : Chessman
{
    public int count = 0;
    public bool die_EnPassant = false;    //Can die by EnPassant
    public bool attack_EnPassant_right = false; //Can attack using EnPassant
    public bool attack_EnPassant_left = false; //Can attack using EnPassant
    public override bool[,] PossibleMoves()
    {
        bool[,] r = new bool[8, 8];

        Chessman c, c2;

        int[] e = BoardManager.Instance.EnPassantMove;

        if (isWhite)
        {
            ////// White team move //////

            //Check for EnPassant Move
            if (CurrentY == 4)
                CheckEnpassant();
            else
            {
                attack_EnPassant_left = false;
                attack_EnPassant_right = false;
            }

            // Diagonal left
            if (CurrentX != 0 && CurrentY != 7)
            {
                if(attack_EnPassant_left)
                    r[CurrentX - 1, CurrentY + 1] = true;

                c = BoardManager.Instance.Chessmans[CurrentX - 1, CurrentY + 1];
                if (c != null && !c.isWhite)
                    r[CurrentX - 1, CurrentY + 1] = true;
            }

            // Diagonal right
            if (CurrentX != 7 && CurrentY != 7)
            {
                if (attack_EnPassant_right)
                    r[CurrentX + 1, CurrentY + 1] = true;

                c = BoardManager.Instance.Chessmans[CurrentX + 1, CurrentY + 1];
                if (c != null && !c.isWhite)
                    r[CurrentX + 1, CurrentY + 1] = true;
            }

            // Middle
            if (CurrentY != 7)
            {
                c = BoardManager.Instance.Chessmans[CurrentX, CurrentY + 1];
                if (c == null)
                    r[CurrentX, CurrentY + 1] = true;
            }

            // Middle on first move
            if (CurrentY == 1)
            {
                c = BoardManager.Instance.Chessmans[CurrentX, CurrentY + 1];
                c2 = BoardManager.Instance.Chessmans[CurrentX, CurrentY + 2];
                if (c == null && c2 == null)
                    r[CurrentX, CurrentY + 2] = true;
            }
            if (BoardManager.Instance.isWhiteKingChecked)
                return PossibleMovesOnCheck(r);
        }
        else
        {
            ////// Black team move //////

            //Check for EnPassant Move
            if (CurrentY == 3)
                CheckEnpassant();
            else
            {
                attack_EnPassant_left = false;
                attack_EnPassant_right = false;
            }
            // Diagonal left
            if (CurrentX != 0 && CurrentY != 0)
            {
                if (attack_EnPassant_left)
                    r[CurrentX - 1, CurrentY - 1] = true;

                c = BoardManager.Instance.Chessmans[CurrentX - 1, CurrentY - 1];
                if (c != null && c.isWhite)
                    r[CurrentX - 1, CurrentY - 1] = true;
            }

            // Diagonal right
            if (CurrentX != 7 && CurrentY != 0)
            {
                if (attack_EnPassant_right)
                    r[CurrentX + 1, CurrentY - 1] = true;

                c = BoardManager.Instance.Chessmans[CurrentX + 1, CurrentY - 1];
                if (c != null && c.isWhite)
                    r[CurrentX + 1, CurrentY - 1] = true;
            }

            // Middle
            if (CurrentY != 0)
            {
                c = BoardManager.Instance.Chessmans[CurrentX, CurrentY - 1];
                if (c == null)
                    r[CurrentX, CurrentY - 1] = true;
            }

            // Middle on first move
            if (CurrentY == 6)
            {
                c = BoardManager.Instance.Chessmans[CurrentX, CurrentY - 1];
                c2 = BoardManager.Instance.Chessmans[CurrentX, CurrentY - 2];
                if (c == null && c2 == null)
                    r[CurrentX, CurrentY - 2] = true;
            }
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
    /// <summary>
    /// Checks if a Pawn can move using EnPassant move.
    /// </summary>
    private void CheckEnpassant()
    {
        if (CurrentX - 1 >= 0)
        {
            Chessman c = BoardManager.Instance.Chessmans[CurrentX - 1, CurrentY];
            if (c != null && c.curPieceType == PieceType.Pawn && c.GetComponent<Pawn>().die_EnPassant && c.GetComponent<Pawn>().count + 1 == BoardManager.Instance.moveCount)
                attack_EnPassant_left = true;
            else attack_EnPassant_left = false;
        }

        if (CurrentX + 1 <= 7)
        {
            Chessman c = BoardManager.Instance.Chessmans[CurrentX + 1, CurrentY];
            if (c != null && c.curPieceType == PieceType.Pawn && c.GetComponent<Pawn>().die_EnPassant && c.GetComponent<Pawn>().count + 1 == BoardManager.Instance.moveCount)
                attack_EnPassant_right = true;
            else attack_EnPassant_right = false;
        }
    }

    public override bool AllowMoves(int selectedX = -1, int selectedY = -1, Vector3 pos = default)
    {
        Chessman c = BoardManager.Instance.Chessmans[selectedX, selectedY];
        if (c == null) return false;
        Vector2 currentTile = GetTile(pos);
        print("a1:" + currentTile + "," + selectedX + "," + selectedY);

        if (currentTile.x == selectedX - 1 && currentTile.y == selectedY + 1)
        {
            print("a");
            return true;
        }
        else if (currentTile.x == selectedX + 1 && currentTile.y == selectedY + 1)
        {
            print("b");
            return true;
        }
        else if (currentTile.x == selectedX - 1 && currentTile.y == selectedY - 1)
        {
            print("c");
            return true;
        }
        else if (currentTile.x == selectedX + 1 && currentTile.y == selectedY - 1)
        {
            print("d");
            return true;
        }
        else if(currentTile.x == selectedX + 1 && currentTile.y == selectedY)
        {
            print("EnPass_Left");
            return true;
        }
        else if(currentTile.x == selectedX - 1 && currentTile.y == selectedY)
        {
            print("EnPass_Right");
            return true;
        }
        else
        {
            print("e");
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
                if (isPlaying(animator_obj, states[tempparamnumber].endStateName))
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
        if (attack_EnPassant_left || attack_EnPassant_right)
            BoardManager.Instance.AttackEnPassant(this);
    }

    public override void level_complete(int parnum,Action can_get_callback = null)
    {
        if(can_get_callback != null)
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
            for(int i =0;i < vfxDetails[tempparamnumber].effects.Count;i++)// foreach (var obj in vfxDetails[tempparamnumber].effects)
            {
                WaitAfterCall((value) => { print("Enter Wait:"+value); 
                    vfxDetails[tempparamnumber].effects[value].SetActive(true); }, vfxDetails[tempparamnumber].effectsdelay[i],i);
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
