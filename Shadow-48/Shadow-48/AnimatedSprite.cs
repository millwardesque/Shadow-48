using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Shadow_48
{
    /// <summary>
    /// Animated sprite
    /// </summary>
    class AnimatedSprite : Sprite
    {
        private int _cellWidth;     // Width of a single animation cell
        private int _cellHeight;    // Height of a single animation cell
        private Dictionary<String, FrameSet> _animations;   // Map of animations
        private String _activeAnimation;    // Active animation
        private int _currentFrameIndex;   // Index of the current frame of animation

        /// <summary>
        /// Width of the sprite
        /// </summary>
        public override int Width
        {
            get { return _cellWidth; }
        }

        /// <summary>
        /// Height of the sprite
        /// </summary>
        public override int Height
        {
            get { return _cellHeight; }
        }

        /// <summary>
        /// Active animation
        /// </summary>
        public String ActiveAnimation
        {
            get { return _activeAnimation; }
            set
            {
                _activeAnimation = value;
                _currentFrameIndex = 0;
            }
        }

        /// <summary>
        /// Current frame index
        /// </summary>
        public int CurrentFrameIndex
        {
            get { return _currentFrameIndex; }
        }
        
        /// <summary>
        /// Gets the current set of animation frames
        /// </summary>
        public FrameSet CurrentFrames
        {
            get
            {
                FrameSet frames;
                _animations.TryGetValue(ActiveAnimation, out frames);
                return frames;
            }
        }

        /// <summary>
        /// Gets the current animation frame
        /// </summary>
        private int CurrentFrame
        {
            get
            {
                FrameSet frames;
                _animations.TryGetValue(ActiveAnimation, out frames);
                return frames.GetFrame(CurrentFrameIndex);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="texture">Texture of the sprite</param>
        /// <param name="cellWidth">Width of a single cell</param>
        /// <param name="cellHeight">Height of a single cell</param>
        public AnimatedSprite(Texture2D texture, int cellWidth, int cellHeight, Dictionary<String, FrameSet> animations)
            : base(texture)
        {
            _cellWidth = cellWidth;
            _cellHeight = cellHeight;
            _animations = animations;

            ActiveAnimation = _animations.Keys.First();
        }

        /// <summary>
        /// Renders the sprite
        /// </summary>
        /// <param name="gameTime">Game time</param>
        /// <param name="batch">SpriteBatch to draw the sprite</param>
        /// <param name="renderer">The system Renderer</param>
        /// <param name="position">The position </param>
        public override void Render(GameTime gameTime, SpriteBatch batch, Renderer renderer, Vector2 position)
        {
            Rectangle destination = new Rectangle((int)position.X, (int)position.Y, Width, Height);
            Rectangle source = new Rectangle(Width * CurrentFrame, 0, Width, Height);
            batch.Draw(Texture, destination, source, Color.White);
        }
    }
}
