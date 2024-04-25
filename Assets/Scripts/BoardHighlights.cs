using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoardHighlights : MonoBehaviour
{
    public static BoardHighlights Instance { set; get; }

    public GameObject highlightPrefab;

    private List<GameObject> highlights;

    public Texture2D greentex;

    public Texture2D redtex;

    public Texture2D yellowtex;

    public GameObject[] teleporteffect;

    private void Start()
    {
        Instance = this;
        highlights = new List<GameObject>();
    }

    private GameObject GetHighLightObject()
    {
        GameObject go = highlights.Find(g => !g.activeSelf);

        if (go == null)
        {
            go = Instantiate(highlightPrefab);
            highlights.Add(go);
        }

        return go;
    }

    public void HighLightAllowedMoves(bool[,] moves)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (moves[i, j])
                {
                    //print("ij:"+i+","+j);
                    GameObject go = GetHighLightObject();
                    go.SetActive(true);
                    go.transform.position = new Vector3(i + 0.5f, 0.0001f, j + 0.5f);
                    Chessman c = BoardManager.Instance.Chessmans[i, j];
#pragma warning disable 0618
                    if (c!=null && BoardManager.Instance.selectedChessman.isWhite != c.isWhite)
                    {
                        go.transform.GetChild(0).GetComponentInChildren<ParticleSystem>().startColor = Color.yellow;
                        go.transform.GetComponentInChildren<ParticleSystemRenderer>().material.mainTexture = yellowtex;
                    }
                    else if(BoardManager.Instance.selectedChessman.isWhite)
                    {
                        go.transform.GetChild(0).GetComponentInChildren<ParticleSystem>().startColor = Color.green;
                        go.transform.GetComponentInChildren<ParticleSystemRenderer>().material.mainTexture = greentex;
                    }
                    else
                    {
                        go.transform.GetChild(0).GetComponentInChildren<ParticleSystem>().startColor = Color.red;
                        go.transform.GetComponentInChildren<ParticleSystemRenderer>().material.mainTexture = redtex;
                    }
#pragma warning restore 0618
                }
            }
        }
    }

    public void HideHighlights()
    {
        foreach (GameObject go in highlights)
            go.SetActive(false);
    }

  
}
