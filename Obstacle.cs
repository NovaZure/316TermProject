using System;

using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class Obstacle
{
	Random rand = new Random();
	//Position of the obstacle
	Vector3 pos;
	public enum ObstacleType { TrashBag, Dumpster, DumpsterEmpty };
	ObstacleType type;

	//Getters/Setters
	public Vector3 Pos
	{
		get { return pos; }
		set { pos = value; }
	}

	public ObstacleType Type
	{
		get { return type; }
		set { type = value; }
	}


	public Obstacle(Vector3 position, ObstacleType t=ObstacleType.Dumpster)
	{
		Pos = position;
		type = t;
	}

	public void Update(GameTime gameTime)
	{
		Vector3 moveVec = new Vector3(0, 0.3f, 0);

		//Move the obstacle forward
		Pos = Pos - moveVec;

		//If the obstacle is behind the player, despawn it (?).
		//We could instead move the obstacle back in front of the player, possibly changing its type and lane as well.
		//This means we would have a finite number of obstacle instances
		if (Pos.Y < -10)
		{
			switch(rand.Next(1,3)) {
				case 1:
					Pos = new Vector3(-2.5f, 30, -2);
					break;
				case 2:
					Pos = new Vector3(0, 30, -2);
					break;
				case 3:
					Pos = new Vector3(2.5f, 30, -2);
					break;
			}
		}
	}
}
