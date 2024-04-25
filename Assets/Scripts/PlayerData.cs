using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;

public class PlayerData : SingletonMonoBehaviour<PlayerData>
{
	public static readonly string GAME_KEY = "GOD_OF_CHESS_";
	public static readonly string PAWN_ANIM_KEY = GAME_KEY + "PAWN_ANIM_KEY";
	public static readonly string KNIGHT_ANIM_KEY = GAME_KEY + "KNIGHT_ANIM_KEY";
	public static readonly string ROOK_ANIM_KEY = GAME_KEY + "ROOK_ANIM_KEY";
	public static readonly string BISHOP_ANIM_KEY = GAME_KEY + "BISHOP_ANIM_KEY";
	public static readonly string QUEEN_ANIM_KEY = GAME_KEY + "QUEEN_ANIM_KEY";
	public static readonly string KING_ANIM_KEY = GAME_KEY + "KING_ANIM_KEY";

	public static readonly string MACE_ANIM_KEY = GAME_KEY + "MACE_ANIM_KEY";
	public static readonly string PERSIAN_ANIM_KEY = GAME_KEY + "PERSIAN_ANIM_KEY";
	public static readonly string MEDJAY_ANIM_KEY = GAME_KEY + "MEDJAY_ANIM_KEY";
	public static readonly string GREEK_ANIM_KEY = GAME_KEY + "GREEK_ANIM_KEY";
	public static readonly string SAMURAI_ANIM_KEY = GAME_KEY + "SAMURAI_ANIM_KEY";
	public static readonly string VIKING_ANIM_KEY = GAME_KEY + "VIKING_ANIM_KEY";

	public readonly string SET_PIECE_TYPE_KEY = "SET_PIECE_TYPE";
	public static bool[] unlockedSets = new bool[6];
	public static int currentSetType = 0;

	public enum eNotiFlag
    {
        None = 0,
        Registed,
    }

#pragma warning disable 0114
    private void Awake()
    {
		SetDontDestroy();
	}
#pragma warning restore 0114

	bool HasSaveKey(string key)
	{
		return PlayerPrefs.HasKey(key);
	}

	int GetSaveKeyInt(string key)
	{
		if (this.HasSaveKey(key) == true)
		{
			return PlayerPrefs.GetInt(key);
		}
		return 0;
	}

	void SetSaveKeyInt(string key, int val)
	{
		PlayerPrefs.SetInt(key, val);
		PlayerPrefs.Save();
	}

	string GetSaveKeyStr(string key)
	{
		if (this.HasSaveKey(key) == true)
		{
			return PlayerPrefs.GetString(key);
		}
		return "";
	}
	void SetSaveKeyStr(string key, string val)
	{
		PlayerPrefs.SetString(key, val);
		PlayerPrefs.Save();
	}
	/// <summary>
	/// Gets the current set type for the chess board.
	/// </summary>
	public int GetPeiceType(string key)
    {
		if(this.HasSaveKey(key) == true)
        {
			return PlayerPrefs.GetInt(key);
        }
		return 0;
    }
	/// <summary>
	/// Sets the set peice type for the chess board.
	/// </summary>
	/// <param name="key"></param>
	/// <param name="val"></param>
	public void SetPieceType(string key, int val)
    {
		PlayerPrefs.SetInt(key, val);
		PlayerPrefs.Save();
    }

	public int[] MaceAnim
	{
		get
		{
			if (this.HasSaveKey(MACE_ANIM_KEY))
			{
				return this.GetSaveKeyStr(MACE_ANIM_KEY).Split(',').Select(x => int.Parse(x)).ToArray();
			}
			return new int[]{ 0, 0, 0, 0, 0, 0 };
		}
		set
		{
			this.SetSaveKeyStr(MACE_ANIM_KEY , string.Join(",", value));
		}
	}

	public int[] PersianAnim
	{
		get
		{
			if (this.HasSaveKey(PERSIAN_ANIM_KEY))
			{
				return this.GetSaveKeyStr(PERSIAN_ANIM_KEY).Split(',').Select(x => int.Parse(x)).ToArray();
			}
			return new int[] { 0, 0, 0, 0, 0, 0 };
		}
		set
		{
			this.SetSaveKeyStr(PERSIAN_ANIM_KEY, string.Join(",", value));
		}
	}

	public int[] MedjayAnim
	{
		get
		{
			if (this.HasSaveKey(MEDJAY_ANIM_KEY))
			{
				return this.GetSaveKeyStr(MEDJAY_ANIM_KEY).Split(',').Select(x => int.Parse(x)).ToArray();
			}
			return new int[] { 0, 0, 0, 0, 0, 0 };
		}
		set
		{
			this.SetSaveKeyStr(MEDJAY_ANIM_KEY, string.Join(",", value));
		}
	}

	public int[] GreekAnim
	{
		get
		{
			if (this.HasSaveKey(GREEK_ANIM_KEY))
			{
				return this.GetSaveKeyStr(GREEK_ANIM_KEY).Split(',').Select(x => int.Parse(x)).ToArray();
			}
			return new int[] { 0, 0, 0, 0, 0, 0 };
		}
		set
		{
			this.SetSaveKeyStr(GREEK_ANIM_KEY, string.Join(",", value));
		}
	}

	public int[] SamuraiAnim
	{
		get
		{
			if (this.HasSaveKey(SAMURAI_ANIM_KEY))
			{
				return this.GetSaveKeyStr(SAMURAI_ANIM_KEY).Split(',').Select(x => int.Parse(x)).ToArray();
			}
			return new int[] { 0, 0, 0, 0, 0, 0 };
		}
		set
		{
			this.SetSaveKeyStr(SAMURAI_ANIM_KEY, string.Join(",", value));
		}
	}

	public int[] VikingAnim
	{
		get
		{
			if (this.HasSaveKey(VIKING_ANIM_KEY))
			{
				return this.GetSaveKeyStr(VIKING_ANIM_KEY).Split(',').Select(x => int.Parse(x)).ToArray();
			}
			return new int[] { 0, 0, 0, 0, 0, 0 };
		}
		set
		{
			this.SetSaveKeyStr(VIKING_ANIM_KEY, string.Join(",", value));
		}
	}

	public int PawnAnim
	{
		get
		{
			if (this.HasSaveKey(PAWN_ANIM_KEY))
			{
				return this.GetSaveKeyInt(PAWN_ANIM_KEY);
			}
			return 0;
		}
		set
		{
			this.SetSaveKeyInt(PAWN_ANIM_KEY, value);			
		}
	}

	public int KnightAnim
	{
		get
		{
			if (this.HasSaveKey(KNIGHT_ANIM_KEY))
			{
				return this.GetSaveKeyInt(KNIGHT_ANIM_KEY);
			}
			return 0;
		}
		set
		{
			this.SetSaveKeyInt(KNIGHT_ANIM_KEY, value);
		}
	}
	public int RookAnim
	{
		get
		{
			if (this.HasSaveKey(ROOK_ANIM_KEY))
			{
				return this.GetSaveKeyInt(ROOK_ANIM_KEY);
			}
			return 0;
		}
		set
		{
			this.SetSaveKeyInt(ROOK_ANIM_KEY, value);
		}
	}
	public int BishopAnim
	{
		get
		{
			if (this.HasSaveKey(BISHOP_ANIM_KEY))
			{
				return this.GetSaveKeyInt(BISHOP_ANIM_KEY);
			}
			return 0;
		}
		set
		{
			this.SetSaveKeyInt(BISHOP_ANIM_KEY, value);
		}
	}
	public int QueenAnim
	{
		get
		{
			if (this.HasSaveKey(QUEEN_ANIM_KEY))
			{
				return this.GetSaveKeyInt(QUEEN_ANIM_KEY);
			}
			return 0;
		}
		set
		{
			this.SetSaveKeyInt(QUEEN_ANIM_KEY, value);
		}
	}
	public int KingAnim
	{
		get
		{
			if (this.HasSaveKey(KING_ANIM_KEY))
			{
				return this.GetSaveKeyInt(KING_ANIM_KEY);
			}
			return 0;
		}
		set
		{
			this.SetSaveKeyInt(KING_ANIM_KEY, value);
		}
	}
}
