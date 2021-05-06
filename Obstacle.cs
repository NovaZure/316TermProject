using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;

public class Obstacle
{
	Random rand = new Random();

	// Position of the obstacle

	// Lane of the Object
	sbyte lanePosition = -1;
	// Position of the object relative to each lane
	// Displacing a model becomes uniform no matter what lane its in
	Vector3 relativePosition;
	// Position of the object relative to the player
	// Is what is decreased every frame
	Vector3 depthPosition;
	// Total of all previous positions
	Vector3 absolutePosition;

	readonly Vector3 OBSTACLE_MOVEMENT_SPEED = new Vector3(0, 0.6f, 0);
	readonly Vector3 OBSTACLE_POSITION_CONVERSION = new Vector3(2.5f, 0, 0);
	readonly Vector3 OBSTACLE_RESET_DEPTHPOSITION = new Vector3(0, 30, 0);
	public enum ObstacleType { TrashBag, Dumpster, DumpsterEmpty };
	ObstacleType type;

	//Getters/Setters
	public Vector3 RelativePosition
	{
		get { return relativePosition; }
		set { relativePosition = value; }
	}
	public sbyte LanePosition
	{
		get { return lanePosition; }
		set { lanePosition = value; }
	}
	public Vector3 AbsolutePosition
	{
		get { return absolutePosition; }
		set { absolutePosition = value; }
	}
	public ObstacleType Type
	{
		get { return type; }
		set { type = value; }
	}

	public Obstacle(
		Vector3 relativePosition,
		sbyte lanePosition = -1,
		ObstacleType t = ObstacleType.Dumpster
	)
	{
        RelativePosition = relativePosition;
		depthPosition = OBSTACLE_RESET_DEPTHPOSITION;
		lanePosition = LanePosition;
		type = t;
	}
	public Obstacle(List<Obstacle> pickable)
	{
		this = pickable[rand.Next(0, pickable.Count)];
		this.LanePosition = (sbyte)(rand.Next(-1, 2));
		this.depthPosition = OBSTACLE_RESET_DEPTHPOSITION;
	}
	public void Update(GameTime gameTime)
	{
		// Move the obstacle forward
		depthPosition -= OBSTACLE_MOVEMENT_SPEED;
		// Total all positional deltas into the AbsolutePosition
		AbsolutePosition = (LanePosition * OBSTACLE_POSITION_CONVERSION) + RelativePosition + depthPosition;
		//Debug.WriteLine(AbsolutePosition + " | " + LanePosition + " | " + RelativePosition + " | " + depthPosition);
	}

	public bool checkForOOB()
    {
		return (depthPosition.Y < -10);
	}

}
