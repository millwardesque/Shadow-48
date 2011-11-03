using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Shadow_48
{
    /// <summary>
    /// Possible player states
    /// </summary>
    public enum PlayerStateType
    {
        Idle,
        Walking,
        Running,
        Crouching,
        Jumping,
        Interacting
    };

    /// <summary>
    /// Base class for all player states
    /// </summary>
    abstract class PlayerState
    {
        protected Player _player;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="player">The player owning the state</param>
        public PlayerState(Player player)
        {
            _player = player;
        }

        /// <summary>
        /// Called when the state is enabled
        /// </summary>
        /// <param name="previousState">The previous player state</param>
        public abstract void OnEnableState(PlayerStateType previousState);

        /// <summary>
        /// Called when the state is disabled
        /// </summary>
        /// <param name="newState">The new player state</param>
        public abstract void OnDisableState(PlayerStateType newState);

        /// <summary>
        /// Called when the player is updated
        /// </summary>
        /// <param name="gameTime">Game time</param>
        /// <param name="keys">State of the keyboard</param>
        /// <param name="elapsedSeconds">Number of elapsed seconds</param>
        public abstract void OnUpdate(GameTime gameTime, KeyboardState keys, float elapsedSeconds);
    }
}
