using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Shadow_48
{
    /// <summary>
    /// A sprite
    /// </summary>
    class Sprite
    {
        protected Texture2D _texture; // Sprite texture

        /// <summary>
        /// Width of the sprite
        /// </summary>
        public virtual int Width
        {
            get { return _texture.Bounds.Width; }
        }

        /// <summary>
        /// Height of the sprite
        /// </summary>
        public virtual int Height
        {
            get { return _texture.Bounds.Height; }
        }

        /// <summary>
        /// Gets the texture of the sprite
        /// </summary>
        public Texture2D Texture
        {
            get { return _texture; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="texture">Texture of the sprite</param>
        public Sprite(Texture2D texture)
        {
            _texture = texture;
        }

        /// <summary>
        /// Renders the sprite
        /// </summary>
        /// <param name="gameTime">Game time</param>
        /// <param name="batch">SpriteBatch to draw the sprite</param>
        /// <param name="renderer">The system Renderer</param>
        /// <param name="position">The position </param>
        public virtual void Render(GameTime gameTime, SpriteBatch batch, Renderer renderer, Vector2 position)
        {
            batch.Draw(Texture, new Rectangle((int)position.X, (int)position.Y, Width, Height), Color.White);
        }
    }
}
