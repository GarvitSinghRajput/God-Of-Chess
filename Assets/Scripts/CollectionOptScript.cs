using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CollectionOptScript : MonoBehaviour
{
    public GameObject setsUi;
    public GameObject[] setTypeBtns;

    public ToggleGroup pawntoggleG;
    public ToggleGroup rooktoggleG;
    public ToggleGroup bishoptoggleG;
    public ToggleGroup knighttoggleG;
    public ToggleGroup queentoggleG;
    public ToggleGroup kingtoggleG;
    private int itemtype;
    public int selectedSetType;


    private void Start()
    {
        itemtype = PlayerData.Instance.GetPeiceType(PlayerData.Instance.SET_PIECE_TYPE_KEY);
        SelectSetType();
    }

    private void OnEnable()
    {

        foreach (Toggle tog in pawntoggleG.GetComponentsInChildren<Toggle>())
        {
            if (tog.name == PlayerData.Instance.PawnAnim.ToString())
                tog.isOn = true;
            else
                tog.isOn = false;
        }
        foreach (Toggle tog in rooktoggleG.GetComponentsInChildren<Toggle>())
        {
            if (tog.name == PlayerData.Instance.RookAnim.ToString())
                tog.isOn = true;
            else
                tog.isOn = false;
        }
        foreach (Toggle tog in bishoptoggleG.GetComponentsInChildren<Toggle>())
        {
            if (tog.name == PlayerData.Instance.BishopAnim.ToString())
                tog.isOn = true;
            else
                tog.isOn = false;
        }
        foreach (Toggle tog in knighttoggleG.GetComponentsInChildren<Toggle>())
        {
            if (tog.name == PlayerData.Instance.KnightAnim.ToString())
                tog.isOn = true;
            else
                tog.isOn = false;
        }
        foreach (Toggle tog in queentoggleG.GetComponentsInChildren<Toggle>())
        {
            if (tog.name == PlayerData.Instance.QueenAnim.ToString())
                tog.isOn = true;
            else
                tog.isOn = false;
        }
        foreach (Toggle tog in kingtoggleG.GetComponentsInChildren<Toggle>())
        {
            if (tog.name == PlayerData.Instance.KingAnim.ToString())
                tog.isOn = true;
            else
                tog.isOn = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDisable()
    {
        setsUi.SetActive(false);
        //print("pawn:" + pawntoggleG.ActiveToggles().FirstOrDefault().name);
        //print("rook:" + rooktoggleG.ActiveToggles().FirstOrDefault().name);
        //print("bishop:" + bishoptoggleG.ActiveToggles().FirstOrDefault().name);
        //print("knight:" + knighttoggleG.ActiveToggles().FirstOrDefault().name);
        //print("queen:" + queentoggleG.ActiveToggles().FirstOrDefault().name);
        //print("kingtoggleG:" + pawntoggleG.ActiveToggles().FirstOrDefault().name);
    }

    public void BackClik()
    {
        setsUi.SetActive(false);
    }

    public void SetsClick(int typeNo)
    {
        setsUi.SetActive(true);
        itemtype = typeNo;
        if (typeNo == 0)
        {            
             toggleUpdate(PlayerData.Instance.MaceAnim);
        }
        else if (typeNo == 1)
        {
            toggleUpdate(PlayerData.Instance.PersianAnim);
        }
        else if (typeNo == 2)
        {
            toggleUpdate(PlayerData.Instance.SamuraiAnim);
        }
        else if (typeNo == 3)
        {
            toggleUpdate(PlayerData.Instance.VikingAnim);
        }
        else if (typeNo == 4)
        {
            toggleUpdate(PlayerData.Instance.MedjayAnim);
        }
        else
        {
            toggleUpdate(PlayerData.Instance.GreekAnim);
        }           

    }

    void toggleUpdate(int[] arr)
    {
        foreach (Toggle tog in pawntoggleG.GetComponentsInChildren<Toggle>())
        {
            if (tog.name == arr[0].ToString())
                tog.isOn = true;
            else
                tog.isOn = false;
        }
        foreach (Toggle tog in rooktoggleG.GetComponentsInChildren<Toggle>())
        {
            if (tog.name == arr[1].ToString())
                tog.isOn = true;
            else
                tog.isOn = false;
        }
        foreach (Toggle tog in bishoptoggleG.GetComponentsInChildren<Toggle>())
        {
            if (tog.name == arr[2].ToString())
                tog.isOn = true;
            else
                tog.isOn = false;
        }
        foreach (Toggle tog in knighttoggleG.GetComponentsInChildren<Toggle>())
        {
            if (tog.name == arr[3].ToString())
                tog.isOn = true;
            else
                tog.isOn = false;
        }
        foreach (Toggle tog in queentoggleG.GetComponentsInChildren<Toggle>())
        {
            if (tog.name == arr[4].ToString())
                tog.isOn = true;
            else
                tog.isOn = false;
        }
        foreach (Toggle tog in kingtoggleG.GetComponentsInChildren<Toggle>())
        {
            if (tog.name == arr[5].ToString())
                tog.isOn = true;
            else
                tog.isOn = false;
        }
    }

    public void AnimOtionClick(bool isactive)
    {
        if (!pawntoggleG.IsActive())
            return;
        PlayerData.Instance.PawnAnim = int.Parse(pawntoggleG.ActiveToggles().FirstOrDefault().name);
        PlayerData.Instance.RookAnim = int.Parse(rooktoggleG.ActiveToggles().FirstOrDefault().name);
        PlayerData.Instance.BishopAnim = int.Parse(bishoptoggleG.ActiveToggles().FirstOrDefault().name);
        PlayerData.Instance.KnightAnim = int.Parse(knighttoggleG.ActiveToggles().FirstOrDefault().name);
        PlayerData.Instance.QueenAnim = int.Parse(queentoggleG.ActiveToggles().FirstOrDefault().name);
        PlayerData.Instance.KingAnim = int.Parse(kingtoggleG.ActiveToggles().FirstOrDefault().name);
        //print("pawn:" + pawntoggleG.ActiveToggles().FirstOrDefault().name);
        //print("rook:" + rooktoggleG.ActiveToggles().FirstOrDefault().name);
        //print("bishop:" + bishoptoggleG.ActiveToggles().FirstOrDefault().name);
        //print("knight:" + knighttoggleG.ActiveToggles().FirstOrDefault().name);
        //print("queen:" + queentoggleG.ActiveToggles().FirstOrDefault().name);
        //print("kingtoggleG:" + kingtoggleG.ActiveToggles().FirstOrDefault().name);

        if (itemtype == 0)
        {
            PlayerData.Instance.MaceAnim = getallvalues();
        }
        else if (itemtype == 1)
        {
            PlayerData.Instance.PersianAnim = getallvalues();
        }
        else if (itemtype == 2)
        {
            PlayerData.Instance.SamuraiAnim = getallvalues();
        }
        else if (itemtype == 3)
        {
            PlayerData.Instance.VikingAnim = getallvalues();
        }
        else if (itemtype == 4)
        {
            PlayerData.Instance.MedjayAnim = getallvalues();
        }
        else
        {
            PlayerData.Instance.GreekAnim = getallvalues();
        }
    }

    int[] getallvalues()
    {
       return new int[] { int.Parse(pawntoggleG.ActiveToggles().FirstOrDefault().name),
                int.Parse(rooktoggleG.ActiveToggles().FirstOrDefault().name),
                int.Parse(bishoptoggleG.ActiveToggles().FirstOrDefault().name),
                int.Parse(knighttoggleG.ActiveToggles().FirstOrDefault().name),
                int.Parse(queentoggleG.ActiveToggles().FirstOrDefault().name),
                int.Parse(kingtoggleG.ActiveToggles().FirstOrDefault().name)
            };
    }

    public void SelectSetType()
    {
        selectedSetType = itemtype;

        foreach(GameObject gm in setTypeBtns)
        {
            gm.GetComponent<Image>().color = new Color(1, 1, 1, 0.67f);
        }

        setTypeBtns[itemtype].GetComponent<Image>().color = new Color(1f, 0.88f, 0.45f, 0.67f);
        PlayerData.Instance.SetPieceType(PlayerData.Instance.SET_PIECE_TYPE_KEY, itemtype);
        setsUi.SetActive(false);
    }
}
