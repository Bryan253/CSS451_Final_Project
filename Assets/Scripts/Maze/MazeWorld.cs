// code adapted from
//  https://catlikecoding.com/unity/tutorials/prototypes/maze-2/

using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

using static Unity.Mathematics.math;
using Random = UnityEngine.Random;

public class MazeWorld : MonoBehaviour
{
	[SerializeField]
	MazeCreate visualization;

    [SerializeField, Tooltip("Use zero for random seed.")]
	int seed;

	[SerializeField]
	int2 mazeSize = int2(10, 10);

	Maze maze;

	void Awake ()
	{
		maze = new Maze(mazeSize);
        
        new MazeGenerate
		{
			maze = maze,
			seed = seed != 0 ? seed : Random.Range(1, int.MaxValue)
		}.Schedule().Complete();

		visualization.Visualize(maze);
	}

    void OnDestroy ()
	{
		maze.Dispose();
	}
}