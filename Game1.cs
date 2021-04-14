﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System.Collections.Generic;
using System.Diagnostics;

namespace _316TermProject
{
	public class Game1 : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;

		Vector3 cameraPos;
		Vector3 playerPos;

		float playerMovSpeed;
		Vector3 playerJumpAccel;
		bool isJumping = false;

		List<Obstacle> obstacles;

		List<Vector3> alleyPos;

		Model player;
		Model alley1;
		Model trashBag, dumpster, dumpsterEmpty;

		float timer;
		bool gameOver;

		KeyboardState kState;
		public Game1()
		{
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
		}

		protected override void Initialize()
		{
			// TODO: Add your initialization logic here
			playerPos = Vector3.Zero;
			cameraPos = new Vector3(playerPos.X, 4, 12);
			playerMovSpeed = 0.1f;

			obstacles = new List<Obstacle>();
			//TEST OBSTACLE
			obstacles.Add(new Obstacle(new Vector3(0, 30, -2)));

			alleyPos = new List<Vector3>();
			alleyPos.Add(new Vector3(0, 0, -2));
			alleyPos.Add(new Vector3(0, 18, -2));

			timer = 0f;
			gameOver = false;

			base.Initialize();
		}

		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			player = Content.Load<Model>("Player");
			alley1 = Content.Load<Model>("AlleyTest");
			trashBag = Content.Load<Model>("trashBag");
			dumpster = Content.Load<Model>("dumpster");
			dumpsterEmpty = Content.Load<Model>("dumpsterEmpty");

			//gameFont = Content.Load<SpriteFont>("Font");
			//largeGameFont = Content.Load<SpriteFont>("LargeF");
		}

		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			
			if (!gameOver)
			{
				kState = Keyboard.GetState();

				// Le funny hack to convert keypresses into data similar to joystick axes
				Vector2 inpDir = new Vector2(
				((kState.IsKeyDown(Keys.A)) ? -1 : 0) + ((kState.IsKeyDown(Keys.D)) ? 1 : 0),
				((kState.IsKeyDown(Keys.W)) ? 1 : 0) + ((kState.IsKeyDown(Keys.S)) ? -1 : 0)
				);

				#region Player movement
				Vector3 moveVec = new Vector3(playerMovSpeed, 0, 0);

				// Player left/right movement
				if (kState.IsKeyDown(Keys.Left))
				{
					playerPos -= moveVec;
					cameraPos -= moveVec;
				}
				else if (kState.IsKeyDown(Keys.Right))
				{
					playerPos += moveVec;
					cameraPos += moveVec;
				}

				if (playerPos.X > 3) { playerPos.X = 3; cameraPos.X = 3; }
				else if (playerPos.X < -3) { playerPos.X = -3; cameraPos.X = -3; }

				//DEBUG LOG PLAYER POS
				//Debug.WriteLine(playerPos);

				// Player jump calculations
				Vector3 jumpVec = new Vector3(0, 0.356f, 0);
				Vector3 decelerateVec = new Vector3(0, -0.012f, 0);
				// Check for jump input, assign player velocity
				if (kState.IsKeyDown(Keys.Space) && !isJumping)
				{
					isJumping = true;
					playerJumpAccel = jumpVec;
				}
				// Accelerate player back down
				if (isJumping)
				{
					playerPos += playerJumpAccel;
					playerJumpAccel += decelerateVec;
				}
				// "Catch" the player on the floor
				// TODO: Change this system to account for being able to stand on obstacles 
				if (playerPos.Y < 0.0f)
				{
					isJumping = false;
					playerPos.Y = 0.0f;
				}
				#endregion

				//Update the obstacles
				foreach (Obstacle o in obstacles)
				{
					o.Update(gameTime);
				}

				// Update Timer
				// Is this supposed to be +=? Or should it just be =
				timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
			}

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			#region Proj/View Matrices
			Matrix proj = Matrix.CreatePerspectiveFieldOfView(
				MathHelper.ToRadians(60),
				1.5f,
				0.001f,
				1000f);

			Matrix view = Matrix.CreateLookAt(
				cameraPos,
				new Vector3(playerPos.X, 0, 0),
				new Vector3(0, 1, 0)
				);
			#endregion

			#region Draw Player
			//how the model is positioned in the world
			// SRT -> Scale*Rotation*Translation
			Matrix world = Matrix.CreateScale(0.8f)
				* Matrix.CreateTranslation(playerPos);

			foreach (ModelMesh mesh in player.Meshes)
			{
				foreach (BasicEffect effect in mesh.Effects)
				{
					effect.World = world;
					effect.View = view;
					effect.Projection = proj;
					effect.EnableDefaultLighting();
					//effect.LightingEnabled = true; // turn on the lighting subsystem.
					//effect.EmissiveColor = new Vector3(0.8f, 0.8f, 0.8f);
				}
				mesh.Draw();
			}
			#endregion

			#region Draw Obstacles
			foreach (Obstacle o in obstacles)
			{
				if (o.Type == Obstacle.ObstacleType.Dumpster)
				{
					world = Matrix.CreateScale(4f)
					* Matrix.CreateTranslation(o.Pos)
					* Matrix.CreateRotationX(MathHelper.ToRadians(-90));

					//Draw each mesh with basic effects (not sure if this is set up right)
					foreach (ModelMesh mesh in dumpster.Meshes)
					{
						foreach (BasicEffect effect in mesh.Effects)
						{
							effect.World = world;
							effect.View = view;
							effect.Projection = proj;
							effect.EnableDefaultLighting();
						}
						mesh.Draw();
					}
				}

				else if (o.Type == Obstacle.ObstacleType.DumpsterEmpty)
				{
					world = Matrix.CreateScale(4f)
					* Matrix.CreateTranslation(o.Pos)
					* Matrix.CreateRotationX(MathHelper.ToRadians(-90));

					//Draw each mesh with basic effects (not sure if this is set up right)
					foreach (ModelMesh mesh in dumpsterEmpty.Meshes)
					{
						foreach (BasicEffect effect in mesh.Effects)
						{
							effect.World = world;
							effect.View = view;
							effect.Projection = proj;
							effect.EnableDefaultLighting();
						}
						mesh.Draw();
					}
				}

				else if (o.Type == Obstacle.ObstacleType.TrashBag)
				{

					world = Matrix.CreateScale(5f)
					* Matrix.CreateTranslation(o.Pos)
					* Matrix.CreateRotationX(MathHelper.ToRadians(-90));

					//Draw each mesh with basic effects (not sure if this is set up right)
					foreach (ModelMesh mesh in trashBag.Meshes)
					{
						foreach (BasicEffect effect in mesh.Effects)
						{
							effect.World = world;
							effect.View = view;
							effect.Projection = proj;
							effect.EnableDefaultLighting();
						}
						mesh.Draw();
					}
				}
			}
			#endregion

			//Alleys should probably be drawn last, as they are the background
			#region Draw Alleys
			foreach (Vector3 pos in alleyPos)
			{
				world = Matrix.CreateScale(4f)
					* Matrix.CreateTranslation(pos)
					* Matrix.CreateRotationX(MathHelper.ToRadians(-90));

				//Draw each mesh with basic effects (not sure if this is set up right)
				foreach (ModelMesh mesh in alley1.Meshes)
				{
					foreach (BasicEffect effect in mesh.Effects)
					{
						effect.World = world;
						effect.View = view;
						effect.Projection = proj;
						effect.EnableDefaultLighting();
					}
					mesh.Draw();
				}
			}
			#endregion

			base.Draw(gameTime);
		}
	}
}
