using System;
using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{

    protected static T instance =  null;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                Type t = typeof(T);

                instance = (T)FindObjectOfType(t);
                if (instance == null)
                {
                    //Debug.LogError(t + " をアタッチしているGameObjectはありません");
                }
            }

            return instance;
        }
    }

	public static void Create( string Name, Transform Parent = null )
	{
		if( instance != null )
		{
			return;
		}

		GameObject Obj = new GameObject( Name );
		Obj.transform.SetParent(Parent, false);
		instance = Obj.AddComponent<T>();
	}

	public static void Create(GameObject go)
	{
		if(instance!=null)
		{
			return;
		}
		instance = go.AddComponent<T>();
	}

    virtual protected void Awake()
    {
        // 他のGameObjectにアタッチされているか調べる.
        // アタッチされている場合は破棄する.
        if (this != Instance)
        {
            Destroy(this);
            Debug.LogError(
                typeof(T) +
                " は既に他のGameObjectにアタッチされているため、コンポーネントを破棄しました." +
                " アタッチされているGameObjectは " + Instance.gameObject.name + " です.");
            return;
        }
    }

    virtual protected void OnDestroy()
    {
        if( instance == this )
        {
            instance = null;
        }
    }


    // シーンまたぎ用
    protected void SetDontDestroy()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    protected virtual void OnCreate()
    {
        //Createの中でやる処理
    }
}
