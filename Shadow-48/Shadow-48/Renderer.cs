using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Shadow_48
{
    /// <summary>
    /// Renderer for the engine
    /// </summary>
    class Renderer
    {
        private GraphicsDeviceManager _graphics = null;    // Graphics Device Manager used in the system
        private Color _clearColour = Color.Black;  // Color used to clear the screen

        /// <summary>
        /// Width of the render surface
        /// </summary>
        public int Width
        {
            get { return _graphics.GraphicsDevice.PresentationParameters.BackBufferWidth; }
        }

        /// <summary>
        /// Height of the render surface
        /// </summary>
        public int Height
        {
            get { return _graphics.GraphicsDevice.PresentationParameters.BackBufferHeight; }
        }

        /// <summary>
        /// Color used to clear the screen
        /// </summary>
        public Color ClearColour
        {
            get { return _clearColour; }
            set { _clearColour = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="graphics">Graphics device manager</param>
        public Renderer(GraphicsDeviceManager graphics)
        {
            _graphics = graphics;
        }

        /// <summary>
        /// Renders the game
        /// </summary>
        /// <param name="gameTime"></param>
        public void Render(GameTime gameTime)
        {
            _graphics.GraphicsDevice.Clear(_clearColour);
        }

        /// <summary>
        /// Converts a point from a coordinate space where (0, 0) is at the top-left to one where (0, 0) is at the bottom-left
        /// </summary>
        /// <param name="point">The point to convert</param>
        /// <returns>The converted point</returns>
        public Vector2 YCoordinateFlip(Vector2 point)
        {
            Vector2 flipped = new Vector2(point.X, this.Height - point.Y);
            return flipped;
        }
    }
}
