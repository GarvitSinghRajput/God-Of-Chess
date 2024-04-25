using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using ChessRules;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BoardManager : MonoBehaviour
{
    public Button btnQueen;
    public Button btnRook;
    public Button btnBishop;
    public Button btnKnight;
    public GameObject panelPickChess;
    public List<Material> chessMatIcons;
    public GameObject textGameEnd;
    public GameObject checkPanel;
    public GameObject greenindicatorprefab;
    public Transform greenindicatorparent;

    public static BoardManager Instance { set; get; }
    public bool[,] allowedMoves { set; get; }
    public Chessman[,] Chessmans { get; set; }
    public Chessman selectedChessman;
    public GameObject textNotation;

    private const float TILE_SIZE = 1.0f;
    private const float TILE_OFFSET = 0.5f;

    public int selectionX = -1;
    public int selectionY = -1;

    public List<GameObject> chessmanPrefabs;
    public List<GameObject> allchessmanPrefabs;
    public List<GameObject> activeChessman;

    public Chessman.SetType whiteSetType;
    public Chessman.SetType blackSetType;

    private Dictionary<string, GameObject> chessPrefabs;
    private Dictionary<string, GameObject> figures;
    private Dictionary<string, GameObject> greenindicators;
    public Dictionary<Chessman, bool[,]> moveListBlack { set; get; }
    public Dictionary<Chessman, bool[,]> moveListWhite { set; get; }

    public Dictionary<Chessman, bool[,]> moveList = new Dictionary<Chessman, bool[,]>();
    public int[] EnPassantMove { set; get; }

    private Quaternion orientation;

    public bool isWhiteTurn = true;

    private bool isActiveAnimation = false;
    private bool isPickFigure = false;
    private float animationTime;
    private bool isMove;
    private bool canSelectNext = true;
    private Chessman lastChessman = null;

    [HideInInspector]
    public int moveCount = 0;
    public Chess ChessEngine { set; get; }

    private Vector2 vFromPos, vToPos;

    public int[] tempWhiteArr;
    public int[] tempBlackArr;
    [HideInInspector]
    public King whiteKing = null;
    [HideInInspector]
    public King blackKing = null;
    [HideInInspector]
    public bool isGettingMoves = false;
    [HideInInspector]
    public bool isBlackKingChecked;
    [HideInInspector]
    public bool isWhiteKingChecked;
    [HideInInspector]
    public bool isSettingAttackerAndKing = false;
    [HideInInspector]
    public bool iscaslting = false;

    //private void Awake()
    //{
    //    tempWhiteArr[0] = PlayerData.Instance.PawnAnim;
    //    tempWhiteArr[1] = PlayerData.Instance.RookAnim;
    //    tempWhiteArr[2] = PlayerData.Instance.BishopAnim;
    //    tempWhiteArr[3] = PlayerData.Instance.KnightAnim;
    //    tempWhiteArr[4] = PlayerData.Instance.QueenAnim;
    //    tempWhiteArr[5] = PlayerData.Instance.KingAnim;
    //}

    private void Start()
    {
        if (!PlayerData.Instance)
            PlayerData.Create("Data");

        CheckSetPieceType(PlayerData.Instance.GetPeiceType(PlayerData.Instance.SET_PIECE_TYPE_KEY));

        SetPieceAnims(whiteSetType, tempWhiteArr);
        SetPieceAnims(blackSetType, tempBlackArr);
        Instance = this;
        ChessEngine = new Chess();
        SpawnAllChessmans();
        ShowFigure();
    }

    private void CheckSetPieceType(int i)
    {
        if (i == 0)
        {
            whiteSetType = Chessman.SetType.Macedonian;
            tempWhiteArr = PlayerData.Instance.MaceAnim;
        }
        else if (i == 1)
        {
            whiteSetType = Chessman.SetType.Persian;
            tempWhiteArr = PlayerData.Instance.PersianAnim;
        }
        else if (i == 2)
        {
            whiteSetType = Chessman.SetType.Samurai;
            tempWhiteArr = PlayerData.Instance.SamuraiAnim;
        }
        else if (i == 3)
        {
            whiteSetType = Chessman.SetType.Viking;
            tempWhiteArr = PlayerData.Instance.VikingAnim;
        }
        else if (i == 4)
        {
            whiteSetType = Chessman.SetType.Medjay;
            tempWhiteArr = PlayerData.Instance.MedjayAnim;
        }
        else
        {
            whiteSetType = Chessman.SetType.Greek;
            tempWhiteArr = PlayerData.Instance.GreekAnim;
        }
    }

    private void Update()
    {
        if (!isPickFigure)
        {
            if (!isActiveAnimation)
            {
                if (!isMove)
                {
                    UpdateSelection();
                    //DrawChessBoard();
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (selectionX >= 0 && selectionY >= 0)
                        {
                            if (selectedChessman == null)
                            {
                                // Select the chessman
                                print("Chessman is Null" + selectionX + "," + selectionY);
                                SelectChessman(selectionX, selectionY);
                            }
                            else
                            {
                                // Move the chessman
                                print("Move the Chessman" + selectionX + "," + selectionY);
                                MoveChessman(selectionX, selectionY);
                            }
                        }
                    }
                }
            }
            else
            {
                ChessAnimation();
            }
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            SceneManager.LoadSceneAsync(0);
            Screen.orientation = ScreenOrientation.Portrait;
        }
    }

    private void SelectChessman(int x, int y)
    {
        if (Chessmans[x, y] == null)
        {
            //print("X,Y  Null ");
            return;
        }

        if (Chessmans[x, y].isWhite != isWhiteTurn)
        {
            //print("X,Y  is Not White ");

            return;
        }
        if (!canSelectNext)
            return;

        bool hasAtleastOneMove = false;
        allowedMoves = Chessmans[x, y].PossibleMoves();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (allowedMoves[i, j])
                {
                    hasAtleastOneMove = true;
                }
            }
        }

        if (!hasAtleastOneMove)
        {
            print("Doen't Have Atleast One Move");
            return;
        }

        selectedChessman = Chessmans[x, y];
        selectedChessman.GetComponent<Outline>().enabled = true;
        selectedChessman.GetComponent<Animator>().enabled = true;
        BoardHighlights.Instance.HighLightAllowedMoves(allowedMoves);
        print("Chessman Selected ");
    }

    public void MoveChessman(int x, int y, Chessman sc = null, bool enPassant = false)
    {
        //Code for EnPassant move this method will be called from Pawn.cs from Update fxn if using Enpassant.
        if (selectedChessman.curPieceType == Chessman.PieceType.Pawn && !enPassant)
        {

            if (selectedChessman.GetComponent<Pawn>().attack_EnPassant_left)
            {
                if (selectionX == selectedChessman.CurrentX - 1 && selectionY == selectedChessman.CurrentY + 1)
                    return;

                if (selectionX == selectedChessman.CurrentX - 1 && selectionY == selectedChessman.CurrentY - 1)
                    return;
            }
            if (selectedChessman.GetComponent<Pawn>().attack_EnPassant_right)
            {
                if (selectionX == selectedChessman.CurrentX + 1 && selectionY == selectedChessman.CurrentY + 1)
                    return;

                if (selectionX == selectedChessman.CurrentX + 1 && selectionY == selectedChessman.CurrentY - 1)
                    return;
            }
        }

        if (sc != null)
        {
            selectedChessman = sc;
        }
        if (allowedMoves[x, y])
        {

            print("Move is Allowed");

            animationTime = 1f;

            iskill = 0;
            BoardHighlights.Instance.HideHighlights();

            int tempteleportNum = (int)selectedChessman.curSetType;
            BoardHighlights.Instance.teleporteffect[tempteleportNum].SetActive(false);
            BoardHighlights.Instance.teleporteffect[tempteleportNum].transform.position = new Vector3(selectedChessman.CurrentX + 0.5f, BoardHighlights.Instance.teleporteffect[tempteleportNum].transform.position.y, selectedChessman.CurrentY + 0.5f);
            BoardHighlights.Instance.teleporteffect[tempteleportNum].SetActive(true);
            isMove = true;
            WaitAfterCall((val) => {
                isActiveAnimation = true;
            });

            ///////////Code for King Castling move
            lastChessman = selectedChessman;
            int xPos = selectionX;
            int yPos = selectionY;
            if (lastChessman.curPieceType == Chessman.PieceType.King && !lastChessman.hasMoved)
            {
                WaitAfterCall((val) =>
                {
                    CastlingDone(xPos, yPos, lastChessman);
                }, 2f);
            }

            ////////////Code for EnPassant move
            if (selectedChessman.curPieceType == Chessman.PieceType.Pawn)
            {
                Pawn p = selectedChessman.GetComponent<Pawn>();
                if (selectedChessman.isWhite)
                {
                    if (selectedChessman.CurrentY + 2 == selectionY)
                        p.die_EnPassant = true;
                    p.count = moveCount;
                }
                else if (!selectedChessman.isWhite)
                {
                    if (selectedChessman.CurrentY - 2 == selectionY)
                        p.die_EnPassant = true;
                    p.count = moveCount;
                }
                else
                    p.die_EnPassant = false;
            }
            canSelectNext = false;
            selectedChessman.hasMoved = true;
            moveCount++;
        }
        else
        {
            print("Move Not Allowed");
            BoardHighlights.Instance.HideHighlights();
            selectedChessman.GetComponent<Outline>().enabled = false;
            selectedChessman.GetComponent<Animator>().enabled = false;
            selectedChessman = null;
        }
    }

    /// <summary>
    /// Get list of all chessman on board and their moves.
    /// </summary>
    public void GetChessMoves()
    {
        isGettingMoves = true;
        if (moveList != null)
            moveList.Clear();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Chessman c = Chessmans[i, j];
                if (c != null)
                {
                    if (c.curPieceType != Chessman.PieceType.Pawn && c.curPieceType != Chessman.PieceType.King)
                    {
                        if (!moveList.ContainsKey(c))
                            moveList.Add(c, c.PossibleMoves());
                    }
                    else if (c.curPieceType == Chessman.PieceType.Pawn && !moveList.ContainsKey(c))
                    {
                        if (!moveList.ContainsKey(c))
                            moveList.Add(c, new bool[8, 8]);
                    }
                    else
                    {
                        if (!moveList.ContainsKey(c))
                            moveList.Add(c, new bool[8, 8]);
                    }
                }
            }
        }
        GetPawnMoves();
        GetKingMoves();
    }

    public void GetPawnMoves()
    {
        foreach (KeyValuePair<Chessman, bool[,]> kvp in moveList)
        {

            if (kvp.Key.curPieceType == Chessman.PieceType.Pawn)
            {
                if (kvp.Key.CurrentX - 1 < 0 || kvp.Key.CurrentX + 1 > 7 || kvp.Key.CurrentY - 1 < 0 || kvp.Key.CurrentY + 1 > 7)
                    continue;
                if (!kvp.Key.isWhite)
                {
                    kvp.Value[kvp.Key.CurrentX + 1, kvp.Key.CurrentY - 1] = true;
                    kvp.Value[kvp.Key.CurrentX - 1, kvp.Key.CurrentY - 1] = true;
                }
                else
                {
                    kvp.Value[kvp.Key.CurrentX + 1, kvp.Key.CurrentY + 1] = true;
                    kvp.Value[kvp.Key.CurrentX - 1, kvp.Key.CurrentY + 1] = true;
                }
            }
        }
    }

    private void GetKingMoves()
    {
        foreach (KeyValuePair<Chessman, bool[,]> kvp in moveList)
        {
            if (kvp.Key.curPieceType == Chessman.PieceType.King)
            {
                if (kvp.Key.CurrentX - 1 < 0 || kvp.Key.CurrentX + 1 > 7 || kvp.Key.CurrentY - 1 < 0 || kvp.Key.CurrentY + 1 > 7)
                    continue;

                kvp.Value[kvp.Key.CurrentX + 1, kvp.Key.CurrentY] = true;
                kvp.Value[kvp.Key.CurrentX - 1, kvp.Key.CurrentY] = true;
                kvp.Value[kvp.Key.CurrentX, kvp.Key.CurrentY + 1] = true;
                kvp.Value[kvp.Key.CurrentX, kvp.Key.CurrentY - 1] = true;
                kvp.Value[kvp.Key.CurrentX + 1, kvp.Key.CurrentY - 1] = true;
                kvp.Value[kvp.Key.CurrentX - 1, kvp.Key.CurrentY - 1] = true;
                kvp.Value[kvp.Key.CurrentX + 1, kvp.Key.CurrentY + 1] = true;
                kvp.Value[kvp.Key.CurrentX - 1, kvp.Key.CurrentY + 1] = true;

            }
        }
        isGettingMoves = false;
    }

    IEnumerator EffectShow(Action<string> can_get_callback, float time, string value)
    {
        yield return new WaitForSeconds(time);
        if (can_get_callback != null)
        {
            can_get_callback(value);
        }
    }

    public void WaitAfterCall(Action<string> can_get_callback = null, float time = 0.2f, string value = "")
    {
        StartCoroutine(EffectShow(can_get_callback, time, value));
    }

    private void doMove(int x, int y)
    {
        //print(selectedChessman.CurrentX+","+selectedChessman.CurrentY+","+x+","+y);
        vFromPos = new Vector2(selectedChessman.CurrentX, selectedChessman.CurrentY);
        vToPos = new Vector2(x, y);

        greenindicators[selectedChessman.CurrentX.ToString() + selectedChessman.CurrentY.ToString()].transform.position = GetTileVector(selectionX, selectionY, 0.0009f);
        greenindicators[selectionX.ToString() + selectionY.ToString()] = greenindicators[selectedChessman.CurrentX.ToString() + selectedChessman.CurrentY.ToString()];
        greenindicators[selectedChessman.CurrentX.ToString() + selectedChessman.CurrentY.ToString()] = null;
        WaitAfterCall((val) => {
            Chessmans[x, y].gameObject.SetActive(true);
            greenindicators[x.ToString() + y.ToString()].SetActive(true);
            isMove = false;
        });
        selectedChessman.GetComponentInChildren<Animator>().SetBool("run", false);
        selectedChessman.GetComponentInChildren<Animator>().SetBool("isAttack", false);
        Chessmans[selectedChessman.CurrentX, selectedChessman.CurrentY] = null;
        selectedChessman.SetPosition(x, y);
        Chessmans[x, y] = selectedChessman;

        if (selectedChessman.GetType() == typeof(Pawn))
        {
            if (y == 7)
            {
                isPickFigure = true;
                panelPickChess.SetActive(true);
                selectedChessman = Chessmans[x, y];
                btnQueen.GetComponent<Image>().material = chessMatIcons[0];
                btnRook.GetComponent<Image>().material = chessMatIcons[1];
                btnBishop.GetComponent<Image>().material = chessMatIcons[2];
                btnKnight.GetComponent<Image>().material = chessMatIcons[3];
                panelPickChess.GetComponent<Image>().color = new Color(0.05f, 0.05f, 0.05f, 0.9f);
                return;
            }

            else if (y == 0)
            {
                isPickFigure = true;
                panelPickChess.SetActive(true);
                selectedChessman = Chessmans[x, y];
                btnQueen.GetComponent<Image>().material = chessMatIcons[4];
                btnRook.GetComponent<Image>().material = chessMatIcons[5];
                btnBishop.GetComponent<Image>().material = chessMatIcons[6];
                btnKnight.GetComponent<Image>().material = chessMatIcons[7];
                panelPickChess.GetComponent<Image>().color = new Color(1, 1, 1, 0.9f);
                return;
            }
        }
        StartChessMove();
    }

    public void PickChessFigure(int btnId)
    {
        //print("PickChessFigure:" + btnId);
        string pawnMorph = "";
        if (isWhiteTurn)
        {
            switch (btnId)
            {
                case 1: pawnMorph = "Q"; break;
                case 2: pawnMorph = "R"; break;
                case 3: pawnMorph = "B"; break;
                case 4: pawnMorph = "N"; break;
            }
        }
        else
        {
            switch (btnId)
            {
                case 1: pawnMorph = "q"; break;
                case 2: pawnMorph = "r"; break;
                case 3: pawnMorph = "b"; break;
                case 4: pawnMorph = "n"; break;
            }
        }
        isPickFigure = false;
        panelPickChess.SetActive(false);
        StartChessMove(pawnMorph);
        if (selectedChessman != null)
            lastChessman = selectedChessman;
        WaitAfterCall((value) =>
        {
            GetChessMoves();
            SetAttackerAndKing(lastChessman.isWhite);
            KingCheckPanel();
            canSelectNext = true;
        });
    }

    private void StartChessMove(string pawnMorph = "")
    {
        if (isWhiteTurn)
        {
            //SwitchAnimators(isWhiteTurn);
            //Camera.main.transform.position = new Vector3(4.2f, 8, 11);
            //Camera.main.transform.rotation = Quaternion.Euler(50, 180, 0);
        }
        else
        {
            //SwitchAnimators(isWhiteTurn);
            //Camera.main.transform.position = new Vector3(4.2f, 8, -3.66f);
            //Camera.main.transform.rotation = Quaternion.Euler(50, 0, 0);
        }
        isWhiteTurn = !isWhiteTurn;
        DropObject(vFromPos, vToPos, pawnMorph);
        BoardHighlights.Instance.HideHighlights();
        selectedChessman.GetComponent<Outline>().enabled = false;
        //selectedChessman.GetComponent<Animator>().enabled = false;
        selectedChessman = null;

        if (ChessEngine.YieldValidMoves().Count() == 0)
        {
            if (ChessEngine.IsCheck)
            {
                //print(ChessEngine.YieldValidMoves().Count());
                textGameEnd.SetActive(true);
                if (isWhiteTurn)
                {
                    textGameEnd.GetComponentInChildren<Text>().text = "Black Wins";
                    textGameEnd.GetComponentInChildren<Text>().fontSize = 42;
                    Camera.main.transform.position = new Vector3(4.2f, 8, 11);
                    Camera.main.transform.rotation = Quaternion.Euler(50, 180, 0);
                }
                else
                {
                    textGameEnd.GetComponentInChildren<Text>().text = "White Wins";
                    textGameEnd.GetComponentInChildren<Text>().fontSize = 42;
                    Camera.main.transform.position = new Vector3(4.2f, 8, -3.66f);
                    Camera.main.transform.rotation = Quaternion.Euler(50, 0, 0);
                }
                //isWhiteTurn = !isWhiteTurn;
                ChessEngine = new Chess();
                //ShowFiguretemp();
            }
            else
            {
                textGameEnd.SetActive(true);
                textGameEnd.GetComponentInChildren<Text>().text = "Stalemate";
                textGameEnd.GetComponentInChildren<Text>().fontSize = 42;
                Camera.main.transform.position = new Vector3(4.2f, 8, 11);
                Camera.main.transform.rotation = Quaternion.Euler(50, 180, 0);
                isWhiteTurn = !isWhiteTurn;
                ChessEngine = new Chess();
                ShowFiguretemp();
            }
        }
    }

    public void BtnOk()
    {
        textGameEnd.SetActive(false);
    }

    private int iskill;

    private void ChessAnimation()
    {
        if (animationTime < 0)
        {
            isActiveAnimation = false;
            //doMove(selectionX, selectionY);
            selectedChessman.GetComponentInChildren<Animator>().SetBool("run", false);
            int tempteleportNum = (int)selectedChessman.curSetType;
            //print("check:"+selectedChessman.curPieceType + "," + iskill);

            if (selectedChessman.curPieceType != Chessman.PieceType.Rook)
            {

                // This teleport is called at the end of animation frame

                BoardHighlights.Instance.teleporteffect[tempteleportNum].SetActive(false);
                BoardHighlights.Instance.teleporteffect[tempteleportNum].transform.position = new Vector3(selectionX + 0.5f, BoardHighlights.Instance.teleporteffect[tempteleportNum].transform.position.y, selectionY + 0.5f);
                BoardHighlights.Instance.teleporteffect[tempteleportNum].SetActive(true);
            }
            lastChessman = selectedChessman;
            //////
            // After move predictions to check if King is being attacked or not
            //////
            WaitAfterCall((value) =>
            {
                GetChessMoves();
                SetAttackerAndKing(lastChessman.isWhite);
                KingCheckPanel();
                canSelectNext = true;
            });

            doMove(selectionX, selectionY);

        }
        else
        {
            if (selectedChessman.AllowMoves(selectionX, selectionY, selectedChessman.transform.position) && iskill < 2)
            {
                selectedChessman.gameObject.SetActive(true);
                //print("Kill_A");
                if (iskill < 1)
                {
                    iskill = 1;
                    if (selectedChessman.curPieceType == Chessman.PieceType.Rook) ChangeLayersRecursively(selectedChessman.transform, 9);
                    selectedChessman.transform.LookAt(Chessmans[selectionX, selectionY].transform);
                    //print("Kill:" + selectedChessman.CurrentX + "," + selectedChessman.CurrentY + "," + selectionX + "," + selectionY);
                    string figure = ChessEngine.GetFigureAt(selectionX, selectionY).ToString();
                    //print("check:" + selectedChessman.curPieceType + "," + iskill);
                    var temptime = 0f;
                    if (selectedChessman.curPieceType == Chessman.PieceType.Rook)
                    {
                        WaitAfterCall((val) =>
                        {
                            ChangeLayersRecursively(selectedChessman.transform, 0);
                        }, 0.3f);
                        int tempteleportNum = (int)selectedChessman.curSetType;
                        var curtile = selectedChessman.GetTile(selectedChessman.transform.position);
                        lastChessman = selectedChessman;
                        //////
                        // After move predictions to check if King is being attacked or not
                        //////
                        WaitAfterCall((value) =>
                        {
                            GetChessMoves();
                            SetAttackerAndKing(lastChessman.isWhite);
                            KingCheckPanel();
                            canSelectNext = true;
                        });

                        BoardHighlights.Instance.teleporteffect[tempteleportNum].SetActive(false);
                        BoardHighlights.Instance.teleporteffect[tempteleportNum].transform.position = new Vector3(curtile.x + 0.5f, BoardHighlights.Instance.teleporteffect[tempteleportNum].transform.position.y, curtile.y + 0.5f);
                        BoardHighlights.Instance.teleporteffect[tempteleportNum].SetActive(true);
                        temptime = 0.5f;
                    }
                    WaitAfterCall((val) =>
                    {
                        if (figure != ".")
                        {
                            print("Kill_C");
                            var tempselectchessman = Chessmans[selectionX, selectionY];
                            tempselectchessman.GetComponent<Animator>().enabled = true;
                            tempselectchessman.transform.LookAt(selectedChessman.transform);
                            //tempselectchessman.level_complete(0, () => {
                            print("Kill_D");
                            selectedChessman.level_complete(0, () => {
                                print("Kill_E");
                                tempselectchessman.level_complete(1, () =>
                                {
                                    print("Kill_F");
                                    //iskill = 2;
                                    if (tempselectchessman.gameObject)
                                        Destroy(tempselectchessman.gameObject);
                                    if (greenindicators[tempselectchessman.CurrentX.ToString() + tempselectchessman.CurrentY.ToString()].gameObject)
                                        Destroy(greenindicators[tempselectchessman.CurrentX.ToString() + tempselectchessman.CurrentY.ToString()].gameObject);
                                    Vector2 curtile = selectedChessman.GetTile(selectedChessman.transform.position);
                                    if (selectedChessman.curPieceType != Chessman.PieceType.Rook)
                                    {
                                        int tempteleportNum = (int)selectedChessman.curSetType;
                                        lastChessman = selectedChessman;
                                        //////
                                        // After move predictions to check if King is being attacked or not
                                        //////
                                        WaitAfterCall((value) =>
                                        {
                                            GetChessMoves();
                                            SetAttackerAndKing(lastChessman.isWhite);
                                            KingCheckPanel();
                                            canSelectNext = true;
                                        });

                                        BoardHighlights.Instance.teleporteffect[tempteleportNum].SetActive(false);
                                        BoardHighlights.Instance.teleporteffect[tempteleportNum].transform.position = new Vector3(curtile.x + 0.5f, BoardHighlights.Instance.teleporteffect[tempteleportNum].transform.position.y, curtile.y + 0.5f);
                                        BoardHighlights.Instance.teleporteffect[tempteleportNum].SetActive(true);
                                    }
                                    else
                                        selectedChessman.GetComponentInChildren<Animator>().SetBool("run", true);
                                    WaitAfterCall((val) =>
                                    {
                                        iskill = 2;
                                    }, 0.3f);
                                });
                            });
                            //});
                        }
                    }, temptime);
                }
            }
            else
            {
                //iskill = 0;
                //print(selectedChessman.curPieceType+","+iskill);
                if (selectedChessman.curPieceType == Chessman.PieceType.Rook && iskill == 1)
                    selectedChessman.gameObject.SetActive(true);
                else
                    selectedChessman.gameObject.SetActive(false);

                greenindicators[selectedChessman.CurrentX.ToString() + selectedChessman.CurrentY.ToString()].SetActive(false);
                animationTime -= Time.deltaTime;
                Vector3 startPosition = selectedChessman.transform.position;
                Vector3 endPosition = GetTileVector(selectionX, selectionY);
                float speed = 1.8f;
                selectedChessman.transform.position = Vector3.Lerp(startPosition, endPosition, speed * Time.deltaTime);
            }
            //var tiletemp = selectedChessman.GetTile(selectedChessman.transform.position);
            //print(selectedChessman.name + "," + (int)tiletemp.x+","+ (int)tiletemp.y+","+ selectedChessman.transform.position);

        }
    }

    void ChangeLayersRecursively(Transform trans, int layernumber)
    {
        foreach (Transform child in trans)
        {
            child.gameObject.layer = layernumber;
            ChangeLayersRecursively(child, layernumber);
        }
    }

    private void UpdateSelection()
    {
        if (!Camera.main)
        { return; }

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("ChessPlane")))
        {
            selectionX = (int)hit.point.x;
            selectionY = (int)hit.point.z;
        }
        else
        {
            selectionX = -1;
            selectionY = -1;
        }
    }

    private void SpawnAllChessmans()
    {
        chessPrefabs = new Dictionary<string, GameObject>();
        figures = new Dictionary<string, GameObject>();
        greenindicators = new Dictionary<string, GameObject>();
        activeChessman = new List<GameObject>();
        Chessmans = new Chessman[8, 8];
        EnPassantMove = new int[2] { -1, -1 };
        string[] settype = { "P", "R", "B", "N", "Q", "K" };

        for (int i = 0; i < settype.Length; i++)
        {

            int prefabNo = (int)whiteSetType * 6 + i;
            allchessmanPrefabs[prefabNo].GetComponent<Chessman>().isWhite = true;
            allchessmanPrefabs[prefabNo].GetComponent<Chessman>().paramnumber = GetPieceAnims(whiteSetType)[i];
            chessPrefabs[settype[i]] = allchessmanPrefabs[prefabNo];
        }

        for (int i = 0; i < settype.Length; i++)
        {
            int prefabNo = (int)blackSetType * 6 + i;
            allchessmanPrefabs[prefabNo].GetComponent<Chessman>().isWhite = false;
            allchessmanPrefabs[prefabNo].GetComponent<Chessman>().paramnumber = GetPieceAnims(blackSetType)[i];
            chessPrefabs[settype[i].ToLower()] = allchessmanPrefabs[prefabNo];
        }

        Transform textnotPa = GameObject.Find("TextNotation").transform;
        Transform emptyPa = GameObject.Find("Empty").transform;

        //Create Natation
        for (int i = 0; i < 8; i++)
        {
            Vector3 start = new Vector3(9.33f, 0, 1 * (i - 1.67f));
            GameObject go = Instantiate(textNotation, start, Quaternion.Euler(90, 0, 0), textnotPa);
            go.GetComponent<TMPro.TextMeshPro>().text = (i + 1).ToString();
            go.GetComponent<TMPro.TextMeshPro>().fontSize = 5;
            //go.GetComponent<TMPro.TextMeshPro>().color = Color.black;

            Vector3 start2 = new Vector3(1 * (i + 10.40f), 0, -2.75f);
            GameObject go2 = Instantiate(textNotation, start2, Quaternion.Euler(90, 0, 0), textnotPa);
            go2.GetComponent<TMPro.TextMeshPro>().text = ((char)('a' + i)).ToString();
            go2.GetComponent<TMPro.TextMeshPro>().fontSize = 4;
            //go2.GetComponent<TMPro.TextMeshPro>().color = Color.black;

            Vector3 start3 = new Vector3(-1.35f, 0, 1 * (-i + 9.76f));
            GameObject go3 = Instantiate(textNotation, start3, Quaternion.Euler(90, 180, 0), textnotPa);
            go3.GetComponent<TMPro.TextMeshPro>().text = (i + 1).ToString();
            go3.GetComponent<TMPro.TextMeshPro>().fontSize = 5;
            //go3.GetComponent<TMPro.TextMeshPro>().color = Color.black;

            Vector3 start4 = new Vector3(1 * (-i - 2.39f), 0, 10.75f);
            GameObject go4 = Instantiate(textNotation, start4, Quaternion.Euler(90, 180, 0), textnotPa);
            go4.GetComponent<TMPro.TextMeshPro>().text = ((char)('a' + i)).ToString();
            go4.GetComponent<TMPro.TextMeshPro>().fontSize = 4;
            //go4.GetComponent<TMPro.TextMeshPro>().color = Color.black;
        }
        //Create Figure
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                string key = "" + x + y;
                figures[key] = new GameObject("Empty");
                figures[key].transform.parent = emptyPa;
            }
        }
    }

    private int[] GetPieceAnims(Chessman.SetType settype)
    {

        if (settype == Chessman.SetType.Macedonian)
        {
            return PlayerData.Instance.MaceAnim;
        }
        else if (settype == Chessman.SetType.Persian)
        {
            return PlayerData.Instance.PersianAnim;
        }
        else if (settype == Chessman.SetType.Samurai)
        {
            return PlayerData.Instance.SamuraiAnim;
        }
        else if (settype == Chessman.SetType.Viking)
        {
            return PlayerData.Instance.VikingAnim;
        }
        else if (settype == Chessman.SetType.Medjay)
        {
            return PlayerData.Instance.MedjayAnim;
        }
        else
        {
            return PlayerData.Instance.GreekAnim;
        }
    }

    private void SetPieceAnims(Chessman.SetType settype, int[] val)
    {

        if (settype == Chessman.SetType.Macedonian)
        {
            PlayerData.Instance.MaceAnim = val;
        }
        else if (settype == Chessman.SetType.Persian)
        {
            PlayerData.Instance.PersianAnim = val;
        }
        else if (settype == Chessman.SetType.Samurai)
        {
            PlayerData.Instance.SamuraiAnim = val;
        }
        else if (settype == Chessman.SetType.Viking)
        {
            PlayerData.Instance.VikingAnim = val;
        }
        else if (settype == Chessman.SetType.Medjay)
        {
            PlayerData.Instance.MedjayAnim = val;
        }
        else
        {
            PlayerData.Instance.GreekAnim = val;
        }
    }

    private Vector3 GetTileVector(int x, int y, float z = 0)
    {
        Vector3 origin = Vector3.zero;
        origin.x += (TILE_SIZE * x) + TILE_OFFSET;
        origin.z += (TILE_SIZE * y) + TILE_OFFSET;
        origin.y = z;
        return origin;
    }

    private void DrawChessBoard()
    {
        Vector3 wightLine = Vector3.right * 8;
        Vector3 hightLine = Vector3.forward * 8;

        for (int i = 0; i <= 8; i++)
        {
            Vector3 start = Vector3.forward * i;
            Debug.DrawLine(start, start + wightLine);
            start = Vector3.right * i;
            Debug.DrawLine(start, start + hightLine);
            for (int j = 0; j <= 8; j++) { } // ???
        }

        //Draw the selection
        if (selectionX >= 0 && selectionY >= 0)
        {
            Debug.DrawLine(
                Vector3.forward * selectionY + Vector3.right * selectionX,
                Vector3.forward * (selectionY + 1) + Vector3.right * (selectionX + 1));
            Debug.DrawLine(
                Vector3.forward * (selectionY + 1) + Vector3.right * selectionX,
                Vector3.forward * selectionY + Vector3.right * (selectionX + 1));
        }
    }

    public void ShowFigure()
    {
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                string key = "" + x + y;
                string figure = ChessEngine.GetFigureAt(x, y).ToString();
                if (figures[key] != null && figures[key].name == figure + " " + x + " " + y)
                    continue;
                if (figure != ".")
                {
                    //if (figures[key] != null)
                    //    StartCoroutine(DestroyChesman(figures[key]));
                    //print("key:"+ key+","+ figures[key]);
                    Destroy(figures[key]);

                    if (figure.All(char.IsUpper))
                    {
                        orientation = Quaternion.Euler(0, 0, 0);
                    }
                    else
                    {
                        orientation = Quaternion.Euler(0, 180, 0);
                    }
                    if (figure == "N")
                    {
                        figures[key] = Instantiate(chessPrefabs[figure], GetTileVector(x, y, 0.0f), orientation);
                    }
                    else
                    {
                        //chessPrefabs[figure].name = chessPrefabs[figure].name + x +y;
                        figures[key] = Instantiate(chessPrefabs[figure], GetTileVector(x, y), orientation);
                    }
                    greenindicators[key] = Instantiate(greenindicatorprefab, GetTileVector(x, y, 0.0009f), greenindicatorprefab.transform.rotation, greenindicatorparent);
                    if (chessPrefabs[figure].GetComponent<Chessman>().isWhite)
                        greenindicators[key].GetComponent<SpriteRenderer>().color = new Color(0, 1, 0, 0.75f);
                    else
                        greenindicators[key].GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 0.75f);

                    figures[key].name = figure + " " + x + " " + y;
                    figures[key].AddComponent<Outline>();
                    figures[key].GetComponent<Outline>().OutlineWidth = 8f;
                    figures[key].GetComponent<Outline>().enabled = false;
                    figures[key].transform.SetParent(transform);
                    Chessmans[x, y] = figures[key].GetComponent<Chessman>();
                    Chessmans[x, y].SetPosition(x, y);
                    if (activeChessman.Contains(figures[key]))
                        activeChessman.Add(figures[key]);
                    figures[key].GetComponent<Animator>().enabled = false;
                }
            }
        }
        WaitAfterCall((value) =>
        {
            whiteKing = GameObject.FindGameObjectWithTag(GlobalVariables.TAG_ISWHITEKING).GetComponent<King>();
            blackKing = GameObject.FindGameObjectWithTag(GlobalVariables.TAG_ISBLACKKING).GetComponent<King>();
            GetChessMoves();
        }, 0.5f);
    }

    public void ShowFiguretemp()
    {
        //Code for KING Castling condition.
        King k = null;
        if (selectedChessman.curPieceType == Chessman.PieceType.King)
        {
            k = selectedChessman.GetComponent<King>();
            if (k != null && k.canCastle && (selectionX == 6 || selectionX == 2))
            {
                if (k.isWhite)
                {
                    orientation = Quaternion.Euler(0, 0, 0);
                }
                else
                {
                    orientation = Quaternion.Euler(0, 180, 0);
                }
                figures["" + selectionX + selectionY] = figures["" + vFromPos.x + vFromPos.y];
                figures["" + selectionX + selectionY].transform.position = GetTileVector(selectionX, selectionY);
                figures["" + selectionX + selectionY].transform.rotation = orientation;
                //figures[key].GetComponent<Animator>().enabled = false;
                WaitAfterCall((val) => { figures[val].GetComponent<Animator>().enabled = false; }, 0.2f, "" + selectionX + selectionY);

                figures["" + selectionX + selectionY].name = (k.isWhite ? "K" : "k") + " " + selectionX + " " + selectionY;
                //figures[key].AddComponent<Outline>();
                figures["" + selectionX + selectionY].GetComponent<Outline>().OutlineWidth = 8f;
                figures["" + selectionX + selectionY].GetComponent<Outline>().enabled = false;
                figures["" + selectionX + selectionY].transform.SetParent(transform);
                Chessmans[selectionX, selectionY] = figures["" + selectionX + selectionY].GetComponent<Chessman>();
                Chessmans[selectionX, selectionY].SetPosition(selectionX, selectionY);
                if (activeChessman.Contains(figures["" + selectionX + selectionY]))
                    activeChessman.Add(figures["" + selectionX + selectionY]);
                return;
            }
        }

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                string key = "" + x + y;
                string figure = ChessEngine.GetFigureAt(x, y).ToString();

                if (figures[key] != null && figures[key].name == figure + " " + x + " " + y)
                    continue;

                if (figure != ".")
                {

                    if (figure.All(char.IsUpper))
                    {
                        orientation = Quaternion.Euler(0, 0, 0);
                    }
                    else
                    {
                        orientation = Quaternion.Euler(0, 180, 0);
                    }
                    figures[key] = figures["" + vFromPos.x + vFromPos.y];
                    if (figure == "N")
                    {
                        //figures[key] = Instantiate(chessPrefabs[figure], GetTileVector(x, y, 0.0f), orientation);
                        figures[key].transform.position = GetTileVector(x, y, 0.0f);
                    }
                    else
                    {
                        //figures[key] = Instantiate(chessPrefabs[figure], GetTileVector(x, y), orientation);
                        figures[key].transform.position = GetTileVector(x, y);
                    }
                    figures[key].transform.rotation = orientation;
                    //figures[key].GetComponent<Animator>().enabled = false;
                    if (isWhiteTurn)
                    {

                    }
                    WaitAfterCall((val) => { figures[val].GetComponent<Animator>().enabled = false; }, 0.2f, key);

                    figures[key].name = figure + " " + x + " " + y;
                    //figures[key].AddComponent<Outline>();
                    figures[key].GetComponent<Outline>().OutlineWidth = 8f;
                    figures[key].GetComponent<Outline>().enabled = false;
                    figures[key].transform.SetParent(transform);
                    Chessmans[x, y] = figures[key].GetComponent<Chessman>();
                    Chessmans[x, y].SetPosition(x, y);
                    if (activeChessman.Contains(figures[key]))
                        activeChessman.Add(figures[key]);
                }
            }
        }
        WaitAfterCall((value) =>
        {
            GetChessMoves();
        }, 0.5f);
    }

    IEnumerator DestroyChesman(GameObject chessmanObj)
    {

        if (chessmanObj.GetComponent<Animator>() != null)
        {
            //print("Into Animation");
            chessmanObj.GetComponent<Animator>().SetBool("die", true);
        }
        yield return new WaitForSeconds(2);
        if (chessmanObj)
            Destroy(chessmanObj);
    }

    public void DropObject(Vector2 from, Vector2 to, string pawnMorph = "")
    {
        string e2 = VectorToSquare(from);
        string e4 = VectorToSquare(to);
        string figure = ChessEngine.GetFigureAt(e2).ToString();
        string move = figure + e2 + e4 + pawnMorph;
        //print(move);
        ChessEngine = ChessEngine.Move(move);
        if (string.IsNullOrEmpty(pawnMorph))
        {
            //print("ShowFiguretemp");
            ShowFiguretemp();
        }
        else
        {
            //print("ShowFigure");
            Destroy(selectedChessman.gameObject);
            ShowFigure();
        }
    }

    public string VectorToSquare(Vector2 vector)
    {
        int x = Convert.ToInt32(vector.x);
        int y = Convert.ToInt32(vector.y);
        if (x >= 0 && x <= 7 && y >= 0 && y <= 7)
        {
            return ((char)('a' + x)).ToString() + (y + 1).ToString();
        }
        return "";
    }

    private void KingCheckPanel()
    {
        if (isBlackKingChecked)
        {
            checkPanel.SetActive(true);
            blackKing.isChecked = true;
            whiteKing.isChecked = false;
        }
        else if (isWhiteKingChecked)
        {
            checkPanel.SetActive(true);
            whiteKing.isChecked = true;
            blackKing.isChecked = false;
        }
        else
        {
            checkPanel.SetActive(false);
            blackKing.isChecked = false;
            whiteKing.isChecked = false;
        }
    }

    /// <summary>
    /// This method checks and sets the attacker and the king if it is being attacked by attacker.
    /// </summary>
    /// <param name="isWhite"></param>
    public void SetAttackerAndKing(bool isWhite)
    {
        isSettingAttackerAndKing = true;
        foreach (KeyValuePair<Chessman, bool[,]> kvp in moveList)
        {
            if (isWhite)
            {
                if (kvp.Key.IsMovePossible(blackKing.CurrentX, blackKing.CurrentY))
                {
                    isBlackKingChecked = true;
                    kvp.Key.attackingKing = true;
                    ReturnCheckedKing = blackKing;
                    ReturnAttacker = kvp.Key;
                }
                else
                {
                    isWhiteKingChecked = false;
                    kvp.Key.attackingKing = false;
                }
            }
            else
            {
                if (kvp.Key.IsMovePossible(whiteKing.CurrentX, whiteKing.CurrentY))
                {
                    isWhiteKingChecked = true;
                    kvp.Key.attackingKing = true;
                    ReturnCheckedKing = whiteKing;
                    ReturnAttacker = kvp.Key;
                }
                else
                {
                    isBlackKingChecked = false;
                    kvp.Key.attackingKing = false;
                }
            }
        }
        isSettingAttackerAndKing = false;
    }

    /// <summary>
    /// Moves pieces for castling(King and Rook)
    /// </summary>
    /// <param name="x">selectionX</param>
    /// <param name="y">selectionY</param> 
    /// <param name="sc">King reference</param> 
    private void CastlingDone(int x, int y, Chessman sc)
    {
        if (selectedChessman == null)
            selectedChessman = sc;
        selectedChessman.hasMoved = true;
        iscaslting = true;
        if (x == 6)
        {
            Chessmans[x, y] = selectedChessman;
            Chessmans[x - 1, y] = null;
            Chessman c = Chessmans[x + 1, y];
            if (c.curPieceType == Chessman.PieceType.Rook && !c.hasMoved)
            {
                selectionX = x - 1;
                selectionY = y;
                selectedChessman = c;
                allowedMoves = c.PossibleMoves();
                if (c.isWhite)
                    isWhiteTurn = true;
                else
                    isWhiteTurn = false;

                MoveChessman(x - 1, y, c);
            }
        }
        else if (x == 2)
        {
            Chessmans[x, y] = selectedChessman;
            Chessmans[x - 1, y] = null;
            Chessman c = Chessmans[x - 2, y];
            if (c.curPieceType == Chessman.PieceType.Rook && !c.hasMoved)
            {
                selectionX = x + 1;
                selectionY = y;
                selectedChessman = c;
                allowedMoves = c.PossibleMoves();
                if (c.isWhite)
                    isWhiteTurn = true;
                else
                    isWhiteTurn = false;

                MoveChessman(x + 1, y, c);
            }
        }
        sc.GetComponent<King>().canCastle = false;
        WaitAfterCall((value) =>
        {
            iscaslting = false;
        }, 2f);
    }

    /// <summary>
    /// Code for Attacking and Moving Enpassant move.
    /// </summary>
    /// <param name="c"></param>
    public void AttackEnPassant(Chessman c)
    {
        Chessman c1, c2;
        Pawn p1 = null, p2 = null;
        if (c.CurrentX - 1 < 0)
            c1 = null;
        else
            c1 = Chessmans[c.CurrentX - 1, c.CurrentY];
        if (c.CurrentX + 1 > 7)
            c2 = null;
        else
            c2 = Chessmans[c.CurrentX + 1, c.CurrentY];

        if (c1 != null)
            p1 = c1.GetComponent<Pawn>();
        if (c2 != null)
            p2 = c2.GetComponent<Pawn>();
        if (c1 != null && c1.curPieceType == Chessman.PieceType.Pawn && p1.die_EnPassant && p1.count + 1 == moveCount)
        {
            //For UPPER LEFT
            if (selectionX == c.CurrentX - 1 && selectionY == c.CurrentY + 1)
            {
                if (Input.GetMouseButton(0))
                {
                    selectionX = c.CurrentX - 1;
                    selectionY = c.CurrentY;
                    selectedChessman = c;
                    allowedMoves[c.CurrentX - 1, c.CurrentY] = true;
                    MoveChessman(c.CurrentX - 1, c.CurrentY, c, true);
                    WaitAfterCall(value =>
                    {
                        selectionX = c.CurrentX - 1;
                        selectionY = c.CurrentY + 1;
                        selectedChessman = c;
                        allowedMoves[c.CurrentX - 1, c.CurrentY + 1] = true;
                        MoveChessman(c.CurrentX - 1, c.CurrentY + 1, c, true);
                    }, 3f);
                }
            }
            //For LOWER LEFT
            if (selectionX == c.CurrentX - 1 && selectionY == c.CurrentY - 1)
            {
                if (Input.GetMouseButton(0))
                {
                    selectionX = c.CurrentX - 1;
                    selectionY = c.CurrentY;
                    selectedChessman = c;
                    allowedMoves[c.CurrentX - 1, c.CurrentY] = true;
                    MoveChessman(c.CurrentX - 1, c.CurrentY, c, true);
                    WaitAfterCall(value =>
                    {
                        selectionX = c.CurrentX - 1;
                        selectionY = c.CurrentY - 1;
                        selectedChessman = c;
                        allowedMoves[c.CurrentX - 1, c.CurrentY - 1] = true;
                        MoveChessman(c.CurrentX - 1, c.CurrentY - 1, c, true);
                    }, 3f);
                }
            }
        }
        if (c2 != null && c2.curPieceType == Chessman.PieceType.Pawn && p2.die_EnPassant && p2.count + 1 == moveCount)
        {
            //For UPPER RIGHT
            if (selectionX == c.CurrentX + 1 && selectionY == c.CurrentY + 1)
            {
                if (Input.GetMouseButton(0))
                {
                    selectionX = c.CurrentX + 1;
                    selectionY = c.CurrentY;
                    selectedChessman = c;
                    allowedMoves[c.CurrentX + 1, c.CurrentY] = true;
                    MoveChessman(c.CurrentX + 1, c.CurrentY, c, true);
                    WaitAfterCall(value =>
                    {
                        selectionX = c.CurrentX + 1;
                        selectionY = c.CurrentY + 1;
                        selectedChessman = c;
                        allowedMoves[c.CurrentX + 1, c.CurrentY + 1] = true;
                        MoveChessman(c.CurrentX + 1, c.CurrentY + 1, c, true);
                    }, 3f);
                }
            }
            //For LOWER RIGHT
            if (selectionX == c.CurrentX + 1 && selectionY == c.CurrentY - 1)
            {
                if (Input.GetMouseButton(0))
                {
                    selectionX = c.CurrentX + 1;
                    selectionY = c.CurrentY;
                    selectedChessman = c;
                    allowedMoves[c.CurrentX + 1, c.CurrentY] = true;
                    MoveChessman(c.CurrentX + 1, c.CurrentY, c, true);
                    WaitAfterCall(value =>
                    {
                        selectionX = c.CurrentX + 1;
                        selectionY = c.CurrentY - 1;
                        selectedChessman = c;
                        allowedMoves[c.CurrentX + 1, c.CurrentY - 1] = true;
                        MoveChessman(c.CurrentX + 1, c.CurrentY - 1, c, true);
                    }, 3f);
                }
            }
        }
    }

    //private void SwitchAnimators(bool isWhite)
    //{
    //    foreach(KeyValuePair<Chessman,bool[,]> kvp in moveList)
    //    {
    //        if(kvp.Key.isWhite != isWhite)
    //        {
    //            if (kvp.Value.Length > 0)
    //                kvp.Key.GetComponent<Animator>().enabled = true;
    //            else
    //                kvp.Key.GetComponent<Animator>().enabled = false;
    //        }
    //        else
    //            kvp.Key.GetComponent<Animator>().enabled = false;
    //    }
    //} 

    public Chessman ReturnAttacker
    {
        get;
        set;
    }

    public Chessman ReturnCheckedKing
    {
        get;
        set;
    }
}
[Serializable]
public class StatesPostionName
{
    public string startStateName;
    public string endStateName;
}

[Serializable]
public class VFXDetails
{
    public List<GameObject> effects;
    public List<float> effectsdelay;

    [Header("Valid For Queen Only")]
    public AttackType attackType;
}

public enum AttackType
{
    Projectile = 0,
    GroundProjectile = 1,
    InkoveAtEnemyFeet = 2,
    InvokeAtQueenPos = 3
};