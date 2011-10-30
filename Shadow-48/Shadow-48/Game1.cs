using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Shadow_48
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Renderer _renderer;     // Game Renderer
        Background _background; // Game background
        SceneNode _scene;       // Scene graph root node
        Player _player;         // Player
        Enemy _enemy;           // Enemy        
        
        /// <summary>
        /// Constructor
        /// </summary>
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            _renderer = new Renderer(graphics);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load the background
            _background = new Background(new Sprite(Content.Load<Texture2D>("background-1")));

            // Create the scenegraph
            _scene = new SceneNode(null, "_root_");

            // Load the player
            // Load the player animations
            Dictionary<String, FrameSet> playerAnimations = new Dictionary<string, FrameSet>();
            List<int> walkFrames = new List<int>();
            walkFrames.Add(0);
            playerAnimations.Add("walk", new FrameSet(walkFrames, false));
 
            List<int> crouchFrames = new List<int>();
            crouchFrames.Add(1);
            playerAnimations.Add("crouch", new FrameSet(crouchFrames, false));

            List<int> jumpFrames = new List<int>();
            jumpFrames.Add(2);
            playerAnimations.Add("jump", new FrameSet(jumpFrames, false));

            List<int> interactionFrames = new List<int>();
            interactionFrames.Add(2);
            playerAnimations.Add("interact", new FrameSet(interactionFrames, false));

            AnimatedSprite playerSprite = new AnimatedSprite(Content.Load<Texture2D>("player-ss"), 32, 64, playerAnimations);
            _player = new Player(_scene, "Player", playerSprite, 100.0f);
            
            // Load the enemy
            Sprite enemySprite = new Sprite(Content.Load<Texture2D>("enemy"));
            _enemy = new Enemy(_scene, "Enemy-1", enemySprite, 20.0f);
            _enemy.Position = new Vector2(300.0f, 20.0f);

            // Load the crate
            WorldObject crate = new WorldObject(_scene, "Crate-1", new Sprite(Content.Load<Texture2D>("crate")));
            crate.Position = new Vector2(200.0f, 200.0f);
            crate.IsElevated = true;

            WorldObject crate2 = new WorldObject(_scene, "Crate-2", new Sprite(Content.Load<Texture2D>("crate")));
            crate2.Position = new Vector2(400.0f, 400.0f);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keys = Keyboard.GetState();

            // Allows the game to exit
            if (keys.IsKeyDown(Keys.Escape) || GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // Update the objects
            List<WorldObject> updatables = new List<WorldObject>();
            _scene.FindUpdatables(ref updatables);
            for (int i = 0; i < updatables.Count; ++i)
            {
                updatables[i].Update(gameTime);
            }

            // Process collisions
            List<WorldObject> collidables = new List<WorldObject>();
            _scene.FindCollidables(ref collidables);

            for (int i = 0; i < collidables.Count; ++i)
            {
                // Check for a collision with all the other collidable objects
                // We can start with the (i + 1) object rather than from the 0th object
                // because we've already compared the 0 - (i-1)'th objects against this object 
                // when this was the j'th object
                for (int j = i + 1; j < collidables.Count; ++j)
                {
                    if (i != j && collidables[i].CollisionAABB.DoesCollide(collidables[j].CollisionAABB))
                    {
                        collidables[i].ProcessCollision(collidables[j]);
                        collidables[j].ProcessCollision(collidables[i]);
                    }
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            _renderer.Render(gameTime);
            
            spriteBatch.Begin();
            _background.Render(gameTime, spriteBatch, _renderer);

            // Process renderables
            List<WorldObject> renderables = new List<WorldObject>();
            _scene.FindRenderables(ref renderables);
            for (int i = 0; i < renderables.Count; ++i)
            {
                renderables[i].Render(gameTime, spriteBatch, _renderer);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
