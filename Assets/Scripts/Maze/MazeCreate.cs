// code adapted from
//  https://catlikecoding.com/unity/tutorials/prototypes/maze-2/

using UnityEngine;

[CreateAssetMenu]
public class MazeCreate : ScriptableObject
{
    [SerializeField]
    MazeCell deadEnd, passage, corner, tPassage, xPassage;

    static Quaternion[] rotations =
	{
		Quaternion.identity,
		Quaternion.Euler(0f, 90f, 0f),
		Quaternion.Euler(0f, 180f, 0f),
		Quaternion.Euler(0f, 270f, 0f)
	};

    public void Visualize (Maze maze)
	{
		for (int i = 0; i < maze.Length; i++)
		{
			(MazeCell, int) prefabWithRotation = GetPrefab(maze[i]);
			MazeCell instance = prefabWithRotation.Item1.GetInstance();
			instance.transform.SetPositionAndRotation(
				maze.IndexToWorldPosition(i), rotations[prefabWithRotation.Item2]
			);
		}
	}

	(MazeCell, int) GetPrefab (MazeFlags flags) => flags switch
	{
		MazeFlags.PassageN => (deadEnd, 0),
		MazeFlags.PassageE => (deadEnd, 1),
		MazeFlags.PassageS => (deadEnd, 2),
		MazeFlags.PassageW => (deadEnd, 3),

		MazeFlags.PassageN | MazeFlags.PassageS => (passage, 0),
		MazeFlags.PassageE | MazeFlags.PassageW => (passage, 1),

		MazeFlags.PassageN | MazeFlags.PassageE => (corner, 0),
		MazeFlags.PassageE | MazeFlags.PassageS => (corner, 1),
		MazeFlags.PassageS | MazeFlags.PassageW => (corner, 2),
		MazeFlags.PassageW | MazeFlags.PassageN => (corner, 3),

		MazeFlags.PassageAll & ~MazeFlags.PassageW => (tPassage, 0),
		MazeFlags.PassageAll & ~MazeFlags.PassageN => (tPassage, 1),
		MazeFlags.PassageAll & ~MazeFlags.PassageE => (tPassage, 2),
		MazeFlags.PassageAll & ~MazeFlags.PassageS => (tPassage, 3),

		_ => (xPassage, 0)
	};
}