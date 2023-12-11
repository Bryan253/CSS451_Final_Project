// code adapted from
//  https://catlikecoding.com/unity/tutorials/prototypes/maze-2/

using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
public struct MazeGenerate : IJob
{
	public Maze maze;

	public int seed;

	public void Execute ()
	{
		var random = new Random((uint)seed);
		var scratchpad = new NativeArray<(int, MazeFlags, MazeFlags)>(
			4, Allocator.Temp, NativeArrayOptions.UninitializedMemory
		);

    var activeIndices = new NativeArray<int>(
			maze.Length, Allocator.Temp, NativeArrayOptions.UninitializedMemory
		);

		int firstActiveIndex = 0, lastActiveIndex = 0;
		activeIndices[firstActiveIndex] = random.NextInt(maze.Length);

    while (firstActiveIndex <= lastActiveIndex)
		{
			int index = activeIndices[lastActiveIndex];
			
			int availablePassageCount = FindAvailablePassages(index, scratchpad);
			
			if (availablePassageCount <= 1)
			{
				lastActiveIndex -= 1;
			}

			if (availablePassageCount > 0)
			{
				(int, MazeFlags, MazeFlags) passage =
					scratchpad[random.NextInt(0, availablePassageCount)];
				maze.Set(index, passage.Item2);
				maze[passage.Item1] = passage.Item3;
				activeIndices[++lastActiveIndex] = passage.Item1;
			}
		}
	}
	
	int FindAvailablePassages (
		int index, NativeArray<(int, MazeFlags, MazeFlags)> scratchpad
	)
	{
		int2 coordinates = maze.IndexToCoordinates(index);
		int count = 0;
		if (coordinates.x + 1 < maze.SizeEW)
		{
			int i = index + maze.StepE;
			if (maze[i] == MazeFlags.Empty)
			{
				scratchpad[count++] = (i, MazeFlags.PassageE, MazeFlags.PassageW);
			}
		}
		if (coordinates.x > 0)
		{
			int i = index + maze.StepW;
			if (maze[i] == MazeFlags.Empty)
			{
				scratchpad[count++] = (i, MazeFlags.PassageW, MazeFlags.PassageE);
			}
		}
		if (coordinates.y + 1 < maze.SizeNS)
		{
			int i = index + maze.StepN;
			if (maze[i] == MazeFlags.Empty)
			{
				scratchpad[count++] = (i, MazeFlags.PassageN, MazeFlags.PassageS);
			}
		}
		if (coordinates.y > 0)
		{
			int i = index + maze.StepS;
			if (maze[i] == MazeFlags.Empty)
			{
				scratchpad[count++] = (i, MazeFlags.PassageS, MazeFlags.PassageN);
			}
		}
		return count;
	}
}