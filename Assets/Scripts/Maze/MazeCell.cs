// code adapted from
//  https://catlikecoding.com/unity/tutorials/prototypes/maze-2/

using UnityEngine;

public class MazeCell : MonoBehaviour
{
    System.Collections.Generic.Stack<MazeCell> cellPool;

    public MazeCell GetInstance ()
	{
        MazeCell instance;

		if (cellPool == null)
		{
			cellPool = new();
		}
		if (cellPool.TryPop(out instance))
		{
			instance.gameObject.SetActive(true);
		}
		else
		{
			instance = Instantiate(this);
			instance.cellPool = cellPool;
		}
		return instance;
	}

	public void Recycle ()
	{
		cellPool.Push(this);
		gameObject.SetActive(false);
	}
}
