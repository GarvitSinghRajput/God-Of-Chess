using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChessRules;
using System;

public abstract class Chessman : MonoBehaviour
{
    public int CurrentX { set; get; }
    public int CurrentY { set; get; }
    private const float TILE_SIZE = 1.0f;
    private const float TILE_OFFSET = 0.5f;
    public bool isWhite;
    public int paramnumber;
    [HideInInspector]
    public bool hasMoved = false;
    public bool canMove;
    public bool isCheckingPrediction = false;
    //[HideInInspector]
    public bool attackingKing = false;

    Dictionary<Chessman, bool[,]> moveList = new Dictionary<Chessman, bool[,]>();
    /// <summary>
    /// Sets position(currentX, currentY) of a chessman.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void SetPosition(int x, int y)
    {
        CurrentX = x;
        CurrentY = y;
    }
    /// <summary>
    /// Returns all the Possible moves of a chessman.
    /// </summary>
    /// <returns></returns>
    public virtual bool[,] PossibleMoves()
    {
        bool[,] r = new bool[8, 8];
        Chess chess = BoardManager.Instance.ChessEngine;

        string figure = chess.GetFigureAt(CurrentX, CurrentY).ToString();
        foreach (string move in chess.YieldValidMoves()) // Pe2e4
        {
            if (figure == move[0].ToString() &&
                CurrentX == (move[1] - 'a') &&
                CurrentY == (move[2] - '1'))
            {
                int x = move[3] - 'a';
                int y = move[4] - '1';
                r[x, y] = true;
            }
        }

        return r;
    }
    /// <summary>
    /// Returns whether the chess piece can move on X, Y.
    /// </summary>
    /// <param name="xPos"></param>
    /// <param name="yPos"></param>
    /// <returns></returns>
    public bool IsMovePossible(int xPos, int yPos)
    {
        bool[,] r = new bool[8, 8];
        r = PossibleMoves();
        if (r[xPos, yPos])
            return true;
        else return false;
    }
    /// <summary>
    /// Checks wheather the block[x,y] is safe to play for KING.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool canMoveAt(int x, int y)
    {
        moveList = BoardManager.Instance.moveList;
        foreach(KeyValuePair<Chessman,bool[,]> kvp in moveList)
        {
            if (isWhite != kvp.Key.isWhite && kvp.Value[x, y])
                return false;
        }
        return true;
    }
    /// <summary>
    /// Returns possible Moves if the King is checked.
    /// </summary>
    /// <param name="currentMoves"></param>
    /// <returns></returns>
    public bool[,] PossibleMovesOnCheck(bool[,] currentMoves)
    {
        bool[,] r = new bool[8, 8];
        Chessman attacker = BoardManager.Instance.ReturnAttacker;
        Chessman king = BoardManager.Instance.ReturnCheckedKing;
        
        int attackerX = attacker.CurrentX;
        int attackerY = attacker.CurrentY;
        int kingX = king.CurrentX;
        int kingY = king.CurrentY;
        moveList = BoardManager.Instance.moveList;
        if (currentMoves[attackerX, attackerY])
            r[attackerX, attackerY] = true;

        while (attackerX != kingX || attackerY != kingY)
        {
            if (currentMoves[attackerX, attackerY])
                r[attackerX, attackerY] = true;
            if (attackerX != kingX || attackerY != kingY)
            {
                if (attackerX < kingX) attackerX++;
                else if (attackerX > kingX) attackerX--;

                if (attackerY < kingY) attackerY++;
                else if (attackerY > kingY) attackerY--;
            }
        }
        return r;
        
    }

    //Used for prediction for a chess piece
    int previousX, previousY;
    Chessman previousChessman;
    /// <summary>
    /// This method checks if the possible moves(current moves) are valid or not.
    /// </summary>
    /// <param name="currentMoves"></param>
    /// <returns></returns>
    public bool[,] AreNextMovesValid(bool[,] currentMoves)
    {
        previousX = CurrentX;
        previousY = CurrentY;        
        bool[,] r = new bool[8, 8];
        r = currentMoves;
        Chessman king = this.isWhite ? BoardManager.Instance.whiteKing : BoardManager.Instance.blackKing;
        for(int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (currentMoves[x, y])
                {
                    previousChessman = BoardManager.Instance.Chessmans[x, y];
                    BoardManager.Instance.Chessmans[x, y] = null;
                    BoardManager.Instance.Chessmans[CurrentX, CurrentY] = null;
                    SetPosition(x, y);
                    BoardManager.Instance.Chessmans[x, y] = this;
                    BoardManager.Instance.GetChessMoves();
                    PredictNextMove(king, ref r);
                }
            }
        }
        //BoardManager.Instance.GetChessMoves();
        isCheckingPrediction = false;
        return r;
    }

    private void PredictNextMove(Chessman k, ref bool[,] r)
    {
        moveList = BoardManager.Instance.moveList;
        //BoardManager.Instance.SetAttackerAndKing(isWhite);
        foreach(KeyValuePair<Chessman, bool[,]> kvp in moveList)
        {
            if (kvp.Key.isWhite != this.isWhite)
            {
                if (kvp.Value[k.CurrentX, k.CurrentY])
                {
                    r[CurrentX, CurrentY] = false;
                    BoardManager.Instance.Chessmans[CurrentX, CurrentY] = previousChessman;
                    SetPosition(previousX, previousY);
                    BoardManager.Instance.Chessmans[previousX, previousY] = this;
                    return;
                }
                else
                {
                    r[CurrentX, CurrentY] = true;
                }
            }
        }
        BoardManager.Instance.Chessmans[CurrentX, CurrentY] = previousChessman;
        SetPosition(previousX, previousY);
        BoardManager.Instance.Chessmans[previousX, previousY] = this;
    }

    public virtual bool AllowMoves(int selectedX = -1 , int selectedY = -1,Vector3 pos = default)
    {
        return false;
    }

    public virtual void level_complete(int param,System.Action can_get_callback = null)
    {
       
    }

    public bool Move(int x, int y, ref bool[,] r)
    {
        if (x >= 0 && x < 8 && y >= 0 && y < 8)
        {
            Chessman c = BoardManager.Instance.Chessmans[x, y];
            if (c == null)
                r[x, y] = true;
            else
            {
                if (isWhite != c.isWhite)
                    r[x, y] = true;
                return true;
            }
        }
        return false;
    }

    public Vector3 GetTileCenter(int x, int y)
    {
        Vector3 origin = Vector3.zero;
        origin.x += (TILE_SIZE * x) + TILE_OFFSET;
        origin.z += (TILE_SIZE * y) + TILE_OFFSET;

        return origin;
    }

    public Vector2 GetTile(Vector3 pos)
    {
        Vector2 origin = Vector3.zero;
        origin.x += (int)((pos.x/ TILE_SIZE) - TILE_OFFSET);
        origin.y += (int)((pos.z/ TILE_SIZE) - TILE_OFFSET);

        return origin;
    }

    public PieceType curPieceType;
    public SetType curSetType;

    public enum PieceType
    {
        Pawn =0,
        Rook,
        Bishop,
        Knight,
        Queen,
        King
    }

    public enum SetType
    {
        Macedonian = 0,
        Persian = 1,
        Samurai = 2,
        Viking = 3,
        Medjay = 4,
        Greek = 5
    }

    public bool isPlaying(Animator anim, string stateName)
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName(stateName) &&
                anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            return true;
        else
            return false;
    }

    IEnumerator EffectShow(Action<int> can_get_callback, float time, int value = 0)
    {
        yield return new WaitForSeconds(time);
        if (can_get_callback != null)
        {
            can_get_callback(value);
        }
    }

    public void WaitAfterCall(Action<int> can_get_callback = null, float time = 0.2f,int value = 0)
    {
        StartCoroutine(EffectShow(can_get_callback, time,value));
    }
}


