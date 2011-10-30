using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Shadow_48
{
    class Background
    {
        Sprite _sprite; // Sprite for the background

        /// <summary>
        /// Background image
        /// </summary>
        public Sprite Sprite
        {
            get { return _sprite; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sprite">Sprite for the background</param>
        public Background(Sprite sprite)
        {
            _sprite = sprite;
        }

        /// <summary>
        /// Renders the background
        /// </summary>
        /// <param name="gameTime">Game Time</param>
        /// <param name="batch">Batch used to render the sprite</param>
        public void Render(GameTime gameTime, SpriteBatch batch, Renderer renderer)
        {
            Vector2 renderPosition = renderer.YCoordinateFlip(new Vector2(0.0f, 0.0f));
            renderPosition.Y -= _sprite.Height;
            batch.Draw(_sprite.Texture, new Rectangle((int)renderPosition.X, (int)renderPosition.Y, _sprite.Width, _sprite.Height), Color.White);
        }
    }
}
