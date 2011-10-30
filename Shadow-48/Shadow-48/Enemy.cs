using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Shadow_48
{
    /// <summary>
    /// States the enemy can assume
    /// </summary>
    enum EnemyState
    {
        Idle,
        Seeking,
        Pursuing
    }

    /// <summary>
    /// Enemy character
    /// </summary>
    class Enemy : WorldObject
    {
        private float _walkSpeed = 20.0f;   // Walk-speed of the enemy
        private Player _player = null;      // Reference to the player
        private EnemyState _state;          // State of the enemy
        private delegate void UpdateUsingState(GameTime gameTime, float elapsedSeconds);    // Function for updating the enemy in a given state.  Changed depending on enemy state
        UpdateUsingState _updateFunction;   // Update function to run
        
        /// <summary>
        /// Walking speed of the enemy
        /// </summary>
        public float WalkSpeed
        {
            get { return _walkSpeed; }
            set { _walkSpeed = value; }
        }

        /// <summary>
        /// State of the enemy
        /// </summary>
        public EnemyState State
        {
            get { return _state; }
            set
            {
                _state = value;

                // Process the new state
                switch (_state)
                {
                    case EnemyState.Idle:
                        _updateFunction = this.UpdateIdle;
                        break;
                    case EnemyState.Seeking:
                        _updateFunction = this.UpdateSeeking;
                        break;
                    case EnemyState.Pursuing:
                        _updateFunction = this.UpdatePursuing;
                        break;
                    default:
                        break;
                }
            }
        }
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parent">Parent of the object</param>
        /// <param name="name">Name of the object</param>
        /// <param name="sprite">The sprite representing the enemy</param>
        /// <param name="walkSpeed">Walking speed of the enemy</param>
        public Enemy(SceneNode parent, String name, Sprite sprite, float walkSpeed)
            : base(parent, name, sprite)
        {
            WalkSpeed = walkSpeed;
            State = EnemyState.Idle;
            IsFixed = false;
        }

        /// <summary>
        /// Updates the state of the enemy
        /// </summary>
        /// <param name="gameTime">Game time</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            float elapsedSeconds = gameTime.ElapsedGameTime.Milliseconds / 1000.0f;   // Elapsed seconds, since the value passed only deals in whole numbers

            // If we don't have a reference already, try to find the player
            if (null == _player)
            {
                _player = (Player)Root.FindInGraph("Player");
            }

            float sightThreshold = 100.0f;  // Distance at which the enemy can see the player
            float soundThreshold = 200.0f;  // Distance at which the enemy can hear the player
            float distanceToPlayer = (_player.Position - this.Position).Length();

            // Update the state of the enemy
            switch (_state)
            {
                case EnemyState.Idle:
                    if (CanSeePlayer(distanceToPlayer, sightThreshold))
                    {
                        State = EnemyState.Pursuing;
                    }
                    else if (CanHearPlayer(distanceToPlayer, soundThreshold))
                    {
                        State = EnemyState.Seeking;
                    }               
                    break;
                case EnemyState.Seeking:
                    if (CanSeePlayer(distanceToPlayer, sightThreshold))
                    {
                        State = EnemyState.Pursuing;
                    }
                    else if (!CanHearPlayer(distanceToPlayer, soundThreshold))
                    {
                        State = EnemyState.Idle;
                    }
                    break;
                case EnemyState.Pursuing:
                    if (!CanSeePlayer(distanceToPlayer, sightThreshold))
                    {
                        if (CanHearPlayer(distanceToPlayer, soundThreshold))
                        {
                            State = EnemyState.Seeking;
                        }
                        else
                        {
                            State = EnemyState.Idle;
                        }
                    }
                    break;
                default:
                    break;
            }

            // Update the enemy
            _updateFunction(gameTime, elapsedSeconds);
        }

        /// <summary>
        /// Checks to see if the enemy can hear the player
        /// </summary>
        /// <returns>True if the player can be heard, else false</returns>
        protected bool CanHearPlayer(float distanceToPlayer, float soundThreshold)
        {
            return (_player.NoiseLevel > 0.3f && distanceToPlayer < soundThreshold);
        }

        /// <summary>
        /// Checks to see if the enemy can see the player
        /// </summary>
        /// <returns>True if the player can be seen, else false</returns>
        protected bool CanSeePlayer(float distanceToPlayer, float sightThreshold)
        {
            return (distanceToPlayer < sightThreshold);
        }

        /// <summary>
        /// Updates the enemy when in an idle state
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="elapsedSeconds"></param>
        protected void UpdateIdle(GameTime gameTime, float elapsedSeconds)
        {
        }

        /// <summary>
        /// Updates the enemy when seeking the player
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="elapsedSeconds"></param>
        protected void UpdateSeeking(GameTime gameTime, float elapsedSeconds)
        {
            if ((null != _player))
            {
                Vector2 direction = _player.Position - this.Position;
                direction.Normalize();
                this.Position += direction * WalkSpeed * elapsedSeconds;
            }
        }

        /// <summary>
        /// Updates the enemy when pursuing the player
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="elapsedSeconds"></param>
        protected void UpdatePursuing(GameTime gameTime, float elapsedSeconds)
        {
            if ((null != _player))
            {
                float runMultiplier = 2.0f;
                Vector2 direction = _player.Position - this.Position;
                direction.Normalize();
                this.Position += direction * runMultiplier * WalkSpeed * elapsedSeconds;
            }
        }
    }
}
