﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Shadow_48
{
    /// <summary>
    /// A player in the game
    /// </summary>
    class Player : WorldObject
    {
        private Dictionary<String, SoundEffect> _soundFX;   // Sound effects to use with the player
        private float _walkSpeed = 20.0f;    // Speed in units per second
        private float _runMultiplier = 2.0f;    // Multiplier used when the player is running
        private PlayerStateType _state = PlayerStateType.Idle;  // State of the player
        private delegate void UpdateUsingState(GameTime gameTime, KeyboardState keys, float elapsedSeconds);    // Function for updating the player.  Changed depending on player state
        UpdateUsingState _updateFunction;   // Update function to run
        private WorldObject _objectUnder = null;   // Object that the player is ducking under
        private SoundEffectInstance _runSound = null;   // Instance of the run sound
        private SoundEffectInstance _walkSound = null;   // Instance of the walk sound
        private SoundEffectInstance _crouchSound = null;    // Instance of the crouch sound
        private float _noiseLevel = 0.0f;   // Level of noise emitted by the player

        /// <summary>
        /// Walk-speed property
        /// </summary>
        public float WalkSpeed
        {
            get { return _walkSpeed; }
            set { _walkSpeed = value; }
        }

        /// <summary>
        /// Run-speed of the player
        /// </summary>
        public float RunSpeed
        {
            get { return _walkSpeed * _runMultiplier; }
        }

        /// <summary>
        /// State of the player
        /// </summary>
        public PlayerStateType State
        {
            get { return _state; }
            set
            {
                PlayerStateType oldState = _state;
                _state = value;

                // Process the state we're changing from
                switch (oldState)
                {
                    case PlayerStateType.Running:
                        if (null != _runSound)
                        {
                            _runSound.Stop();
                            _runSound = null;
                        }
                        break;

                    case PlayerStateType.Crouching:
                        if (null != _crouchSound)
                        {
                            _crouchSound.Stop();
                            _crouchSound = null;
                        }
                        break;
                    case PlayerStateType.Walking:
                        if (null != _walkSound)
                        {
                            _walkSound.Stop();
                            _walkSound = null;
                        }
                        break;

                    default:
                        break;
                }

                // Adjust the state of the player
                switch (_state)
                {
                    case PlayerStateType.Idle:
                        _updateFunction = this.UpdateIdle;
                        ((AnimatedSprite)_sprite).ActiveAnimation = "idle";
                        break;
                    case PlayerStateType.Walking:
                        _updateFunction = this.UpdateWalking;
                        ((AnimatedSprite)_sprite).ActiveAnimation = "walk";
                        SoundEffect walkSound;
                        _soundFX.TryGetValue("walk", out walkSound);
                        _walkSound = walkSound.CreateInstance();
                        _walkSound.IsLooped = true;
                        _walkSound.Play();
                        break;
                    case PlayerStateType.Running:
                        _updateFunction = this.UpdateRunning;
                        ((AnimatedSprite)_sprite).ActiveAnimation = "walk";
                        SoundEffect runSound;
                        _soundFX.TryGetValue("run", out runSound);
                        _runSound = runSound.CreateInstance();
                        _runSound.IsLooped = true;
                        _runSound.Play();
                        break;
                    case PlayerStateType.Crouching:
                        _updateFunction = this.UpdateCrouching;
                        ((AnimatedSprite)_sprite).ActiveAnimation = "crouch";
                        break;
                    case PlayerStateType.Jumping:
                        _updateFunction = this.UpdateCrouching;
                        ((AnimatedSprite)_sprite).ActiveAnimation = "jump";

                        SoundEffect jumpSound = null;
                        _soundFX.TryGetValue("jump", out jumpSound);
                        jumpSound.Play();

                        break;
                    case PlayerStateType.Interacting:
                        _updateFunction = this.UpdateInteract;
                        ((AnimatedSprite)_sprite).ActiveAnimation = "interact";
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Noise level of the player
        /// </summary>
        public float NoiseLevel
        {
            get { return _noiseLevel; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parent">Parent of the object</param>
        /// <param name="name">Name of the object</param>
        /// <param name="sprite">The sprite representing the player</param>
        /// <param name="walkSpeed">Walking speed of the player</param>
        /// <param name="soundFX">Sound effects for the player</param>
        public Player(SceneNode parent, String name, Sprite sprite, float walkSpeed, Dictionary<String, SoundEffect> soundFX)
            : base(parent, name, sprite)
        {
            WalkSpeed = walkSpeed;
            _updateFunction = this.UpdateWalking;
            _soundFX = soundFX;
            IsFixed = false;
        }

        /// <summary>
        /// Overrides the update function to allow for player control
        /// </summary>
        /// <param name="gameTime">Game time</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            KeyboardState keys = Keyboard.GetState();   // State of the keyboard
            float elapsedSeconds = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;   // Elapsed seconds, since the value passed only deals in whole numbers

            // Update the state of the player based on the user input
            switch (State)
            {
                case PlayerStateType.Idle:
                    if (IsMovementRequested(keys))
                    {
                        if (IsRunningRequested(keys))
                        {
                            State = PlayerStateType.Running;
                        }
                        else
                        {
                            State = PlayerStateType.Walking;
                        }
                    }
                    else if (IsCrouchingRequested(keys))
                    {
                        State = PlayerStateType.Crouching;
                    }
                    else if (IsJumpingRequested(keys))
                    {
                        State = PlayerStateType.Jumping;
                    }
                    else if (IsInteractRequested(keys) && CanInteract())
                    {
                        State = PlayerStateType.Interacting;
                    }
                    break;
                case PlayerStateType.Walking:
                    if (!IsMovementRequested(keys))
                    {
                        State = PlayerStateType.Idle;
                    }
                    else if (IsRunningRequested(keys))
                    {
                        State = PlayerStateType.Running;
                    }
                    else if (IsCrouchingRequested(keys))
                    {
                        State = PlayerStateType.Crouching;
                    }
                    else if (IsJumpingRequested(keys))
                    {
                        State = PlayerStateType.Jumping;
                    }
                    else if (IsInteractRequested(keys) && CanInteract())
                    {
                        State = PlayerStateType.Interacting;
                    }
                    break;
                case PlayerStateType.Running:
                    if (!IsMovementRequested(keys))
                    {
                        State = PlayerStateType.Idle;
                    }
                    else if (!IsRunningRequested(keys))
                    {
                        State = PlayerStateType.Walking;
                    }
                    else if (IsCrouchingRequested(keys))
                    {
                        State = PlayerStateType.Crouching;
                    }
                    else if (IsJumpingRequested(keys))
                    {
                        State = PlayerStateType.Jumping;
                    }
                    break;
                case PlayerStateType.Crouching:
                    if (!IsCrouchingRequested(keys) && _objectUnder == null)
                    {
                        if (IsMovementRequested(keys))
                        {
                            if (IsRunningRequested(keys))
                            {
                                State = PlayerStateType.Running;
                            }
                            else
                            {
                                State = PlayerStateType.Walking;
                            }
                        }
                        else
                        {
                            State = PlayerStateType.Idle;
                        }
                    }
                    break;
                case PlayerStateType.Jumping:
                    if (!IsJumpingRequested(keys))
                    {
                        if (IsMovementRequested(keys))
                        {
                            if (IsRunningRequested(keys))
                            {
                                State = PlayerStateType.Running;
                            }
                            else
                            {
                                State = PlayerStateType.Walking;
                            }
                        }
                        else
                        {
                            State = PlayerStateType.Idle;
                        }
                    }
                    break;
                case PlayerStateType.Interacting:
                    if (!IsInteractRequested(keys))
                    {
                        if (IsMovementRequested(keys))
                        {
                            if (IsRunningRequested(keys))
                            {
                                State = PlayerStateType.Running;
                            }
                            else
                            {
                                State = PlayerStateType.Walking;
                            }
                        }
                        else
                        {
                            State = PlayerStateType.Idle;
                        }
                    }
                    break;
                default:
                    break;
            }
            
            // Update the player based on their current state
            _updateFunction(gameTime, keys, elapsedSeconds);           
        }

        /// <summary>
        /// Renders the player
        /// </summary>
        /// <param name="gameTime">Game Time</param>
        /// <param name="batch">Batch used to render the sprite</param>
        public override void Render(GameTime gameTime, SpriteBatch batch, Renderer renderer)
        {
            Vector2 renderPosition = renderer.YCoordinateFlip(_position);
            renderPosition.Y -= _sprite.Height;
            _sprite.Render(gameTime, batch, renderer, renderPosition);
            base.Render(gameTime, batch, renderer);
        }

        /// <summary>
        /// Checks to see if movement is requested from the user
        /// </summary>
        /// <param name="keys">The state of the keyboard</param>
        /// <returns>True if the user has requested movement, else false</returns>
        protected bool IsMovementRequested(KeyboardState keys)
        {
            return (keys.IsKeyDown(Keys.Up) || keys.IsKeyDown(Keys.Down) || keys.IsKeyDown(Keys.Left) || keys.IsKeyDown(Keys.Right));
        }

        /// <summary>
        /// Checks to see if an interaction is requested from the user
        /// </summary>
        /// <param name="keys">The state of the keyboard</param>
        /// <returns>True if the user has requested movement, else false</returns>
        protected bool IsInteractRequested(KeyboardState keys)
        {
            return (keys.IsKeyDown(Keys.F));
        }

        /// <summary>
        /// Checks to see if the player can interact with any nearby objects
        /// </summary>
        /// <returns></returns>
        protected bool CanInteract()
        {
            List<WorldObject> nearbyObjects = new List<WorldObject>();
            List<WorldObject> interactiveObjects = new List<WorldObject>();
            this.Root.FindNearbyObjects(this, 50.0f, ref nearbyObjects);
            foreach(WorldObject nearby in nearbyObjects)
            {
                if (nearby.IsInteractive)
                {
                    interactiveObjects.Add(nearby);
                }
            }

            return (interactiveObjects.Count != 0);
        }

        /// <summary>
        /// Checks to see if running is requested from the user
        /// </summary>
        /// <param name="keys">The state of the keyboard</param>
        /// <returns>True if the user has requested to run, else false</returns>
        protected bool IsRunningRequested(KeyboardState keys)
        {
            return (keys.IsKeyDown(Keys.LeftShift) || keys.IsKeyDown(Keys.RightShift));
        }

        /// <summary>
        /// Checks to see if crouching is requested from the user
        /// </summary>
        /// <param name="keys">The state of the keyboard</param>
        /// <returns>True if the user has requested to crouch, else false</returns>
        protected bool IsCrouchingRequested(KeyboardState keys)
        {
            return (keys.IsKeyDown(Keys.C));
        }

        /// <summary>
        /// Checks to see if jumping is requested from the user
        /// </summary>
        /// <param name="keys">The state of the keyboard</param>
        /// <returns>True if the user has requested to jump, else false</returns>
        protected bool IsJumpingRequested(KeyboardState keys)
        {
            return (keys.IsKeyDown(Keys.Space));
        }

        /// <summary>
        /// Updates the player when the player is idle
        /// </summary>
        /// <param name="gameTime">Game time</param>
        /// <param name="keys">The state of the keyboard</param>
        /// <param name="elapsedSeconds">Number of elapsed seconds</param>
        protected void UpdateIdle(GameTime gameTime, KeyboardState keys, float elapsedSeconds)
        {
            _noiseLevel = 0.0f;
        }

        /// <summary>
        /// Updates the player when the player is walking
        /// </summary>
        /// <param name="gameTime">Game time</param>
        /// <param name="keys">The state of the keyboard</param>
        /// <param name="elapsedSeconds">Number of elapsed seconds</param>
        protected void UpdateWalking(GameTime gameTime, KeyboardState keys, float elapsedSeconds)
        {
            // Move the player
            Vector2 distance = Vector2.Zero;
            if (keys.IsKeyDown(Keys.Up))
            {
                distance.Y += 1.0f;
            }
            if (keys.IsKeyDown(Keys.Down))
            {
                distance.Y -= 1.0f;
            }
            if (keys.IsKeyDown(Keys.Left))
            {
                distance.X -= 1.0f;
            }
            if (keys.IsKeyDown(Keys.Right))
            {
                distance.X += 1.0f;
            }

            // If the player moved, adjust the player's position
            if (distance != Vector2.Zero)
            {
                distance.Normalize();   // Normalize the distance to the player into just a direction
                distance *= WalkSpeed * elapsedSeconds;
                this.Adjust(distance);
                _noiseLevel = 0.4f;
            }
            else
            {
                _noiseLevel = 0.0f;
            }
        }

        /// <summary>
        /// Updates the player when the player is running
        /// </summary>
        /// <param name="gameTime">Game time</param>
        /// <param name="keys">The state of the keyboard</param>
        /// <param name="elapsedSeconds">Number of elapsed seconds</param>
        protected void UpdateRunning(GameTime gameTime, KeyboardState keys, float elapsedSeconds)
        {
            // Move the player
            Vector2 distance = Vector2.Zero;
            if (keys.IsKeyDown(Keys.Up))
            {
                distance.Y += 1.0f;
            }
            if (keys.IsKeyDown(Keys.Down))
            {
                distance.Y -= 1.0f;
            }
            if (keys.IsKeyDown(Keys.Left))
            {
                distance.X -= 1.0f;
            }
            if (keys.IsKeyDown(Keys.Right))
            {
                distance.X += 1.0f;
            }

            // If the player moved, adjust the player's position
            if (distance != Vector2.Zero)
            {
                distance.Normalize();   // Normalize the distance to the player into just a direction
                distance *= RunSpeed * elapsedSeconds;
                this.Adjust(distance);
                _noiseLevel = 0.6f;
            }
            else
            {
                _noiseLevel = 0.0f;
            }
        }

        /// <summary>
        /// Updates the player when the player is running
        /// </summary>
        /// <param name="gameTime">Game time</param>
        /// <param name="keys">The state of the keyboard</param>
        /// <param name="elapsedSeconds">Number of elapsed seconds</param>
        protected void UpdateCrouching(GameTime gameTime, KeyboardState keys, float elapsedSeconds)
        {
            float crouchSlowdown = 0.5f;    // Slowdow factor to use when the player is crouching

            // Move the player
            Vector2 distance = Vector2.Zero;
            if (keys.IsKeyDown(Keys.Up))
            {
                distance.Y += 1.0f;
            }
            if (keys.IsKeyDown(Keys.Down))
            {
                distance.Y -= 1.0f;
            }
            if (keys.IsKeyDown(Keys.Left))
            {
                distance.X -= 1.0f;
            }
            if (keys.IsKeyDown(Keys.Right))
            {
                distance.X += 1.0f;
            }

            // If the player moved, adjust the player's position
            if (distance != Vector2.Zero)
            {
                distance.Normalize();   // Normalize the distance to the player into just a direction
                distance *= crouchSlowdown * WalkSpeed * elapsedSeconds;
                this.Adjust(distance);
                _noiseLevel = 0.3f;

                // If we're not playing the looping crouch noise, play it now
                if (null == _crouchSound)
                {
                    SoundEffect crouchSound;
                    _soundFX.TryGetValue("walk", out crouchSound);
                    _crouchSound = crouchSound.CreateInstance();
                    _crouchSound.IsLooped = true;
                    _crouchSound.Play();
                    _crouchSound.Volume /= 4.0f;
                }
            }
            else
            {
                // If we're not playing the looping crouch noise, play it now
                if (null != _crouchSound)
                {
                    _crouchSound.Stop();
                    _crouchSound = null;
                }
                _noiseLevel = 0.0f;
            }

            // Check to see if we're crouching under anything
            if (_objectUnder != null && !this.CollisionAABB.DoesCollide(_objectUnder.CollisionAABB))
            {
                _objectUnder = null;
            }
        }

        /// <summary>
        /// Updates the player when the player is jumping
        /// </summary>
        /// <param name="gameTime">Game time</param>
        /// <param name="keys">The state of the keyboard</param>
        /// <param name="elapsedSeconds">Number of elapsed seconds</param>
        protected void UpdateJumping(GameTime gameTime, KeyboardState keys, float elapsedSeconds)
        {
            float jumpSlowdown = 0.7f;    // Slowdow factor to use when the player is crouching

            // Move the player
            Vector2 distance = Vector2.Zero;
            if (keys.IsKeyDown(Keys.Up))
            {
                distance.Y += 1.0f;
            }
            if (keys.IsKeyDown(Keys.Down))
            {
                distance.Y -= 1.0f;
            }
            if (keys.IsKeyDown(Keys.Left))
            {
                distance.X -= 1.0f;
            }
            if (keys.IsKeyDown(Keys.Right))
            {
                distance.X += 1.0f;
            }

            // If the player moved, adjust the player's position
            if (distance != Vector2.Zero)
            {
                distance.Normalize();   // Normalize the distance to the player into just a direction
                distance *= jumpSlowdown * WalkSpeed * elapsedSeconds;
                this.Adjust(distance);
                _noiseLevel = 0.6f;
            }
            else
            {
                _noiseLevel = 0.5f;
            }
        }

        /// <summary>
        /// Updates the player when the player is interacting with an object in the world (button, etc.)
        /// </summary>
        /// <param name="gameTime">Game time</param>
        /// <param name="keys">The state of the keyboard</param>
        /// <param name="elapsedSeconds">Number of elapsed seconds</param>
        protected void UpdateInteract(GameTime gameTime, KeyboardState keys, float elapsedSeconds)
        {
        }

        /// <summary>
        /// Processes a collision with the given collider
        /// </summary>
        /// <param name="collider"></param>
        public override void ProcessCollision(WorldObject collider)
        {
            // Ignore collisions in certain cases
            if (collider.IsElevated && State == PlayerStateType.Crouching)
            {
                _objectUnder = collider;
                return;
            }
            else if (!collider.IsHigh && State == PlayerStateType.Jumping)
            {
                _objectUnder = null;
                return;
            }
            else
            {
                _objectUnder = null;
                this.Position -= Displacement;
            }
        }
    }
}
