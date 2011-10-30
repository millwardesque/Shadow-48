using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Shadow_48
{
    class WorldObject : SceneNode
    {
        protected Sprite _sprite;     // Sprite for the object
        protected Vector2 _previousPosition;  // Previous position of the object
        protected AABB _aabb;         // Collision volume for the object
        protected AABB _previousAABB; // Previous AABB for the object
        protected bool _isHigh = true;       // If true, this object is high and can't be jumped over, but can act as standing cover
        protected bool _isElevated = false;  // If true, this object is elevated and can be crouched under
        protected bool _isFixed = true;      // If true, this object is fixed in place and shouldn't move in the case of a collision
        protected bool _isInteractive = false;   // If true, this object can be interacted with

        /// <summary>
        /// Position of the object
        /// </summary>
        public override Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;

                // Update the centre of the AABB
                _aabb.Centre = this.Centre;
            }
        }

        /// <summary>
        /// Sprite for the object
        /// </summary>
        public Sprite ObjectSprite
        {
            get { return _sprite; }
            set
            {
                _sprite = value;
                _aabb = new AABB(Centre, new Vector2(_sprite.Width / 2.0f, _sprite.Height / 2.0f));
            }
        }

        /// <summary>
        /// Collision volume for the object
        /// </summary>
        public AABB CollisionAABB
        {
            get { return _aabb; }
        }

        /// <summary>
        /// Centre of the object
        /// </summary>
        public Vector2 Centre
        {
            get { return CalculateCentre(_position, _sprite.Width, _sprite.Height); }
        }

        /// <summary>
        /// Displacement of the object from the previous frame
        /// </summary>
        public Vector2 Displacement
        {
            get { return (_position - _previousPosition); }
        }

        /// <summary>
        /// Is the object high or not?
        /// </summary>
        public bool IsHigh
        {
            get { return _isHigh; }
            set { _isHigh = value; }
        }

        /// <summary>
        /// Is the object elevated or not?
        /// </summary>
        public bool IsElevated
        {
            get { return _isElevated; }
            set { _isElevated = value; }
        }

        /// <summary>
        /// Is the object fixed in place
        /// </summary>
        public bool IsFixed
        {
            get { return _isFixed; }
            set { _isFixed = value; }
        }

        /// <summary>
        /// Is the object interactive
        /// </summary>
        public bool IsInteractive
        {
            get { return _isInteractive; }
            set { _isInteractive = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parent">Parent of the object</param>
        /// <param name="name">Name of the object</param>
        /// <param name="sprite">Sprite representing the object</param>
        public WorldObject(SceneNode parent, String name, Sprite sprite)
            : base (parent, name)
        {
            _sprite = sprite;
            _position = Vector2.Zero;
            _aabb = new AABB(Centre, new Vector2(sprite.Width / 2.0f, sprite.Height / 2.0f));  
        }

        /// <summary>
        /// Calculate the centre of the object
        /// </summary>
        /// <param name="position">Bottom-left corner of the object</param>
        /// <param name="width">Width of the object</param>
        /// <param name="height">Height of the object</param>
        /// <returns>Returns a Vector2 containing the centre of the rectangle</returns>
        protected Vector2 CalculateCentre(Vector2 position, float width, float height)
        {
            Vector2 centre = new Vector2(position.X + width / 2.0f, position.Y + height / 2.0f);
            return centre;
        }

        /// <summary>
        /// Adjust the world object's position
        /// </summary>
        /// <param name="adjustment">The amount to adjust the </param>
        public virtual void Adjust(Vector2 adjustment)
        {
            Position += adjustment;
        }

        /// <summary>
        /// Renders the world object
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
        /// Updates the world object
        /// </summary>
        /// <param name="gameTime">Game Time</param>
        public override void Update(GameTime gameTime)
        {
            _previousPosition = _position;
            _previousAABB = _aabb;

            base.Update(gameTime);
        }

        /// <summary>
        /// Processes a collision with the given collider
        /// </summary>
        /// <param name="collider"></param>
        public virtual void ProcessCollision(WorldObject collider)
        {
            this.Position -= Displacement;
        }

        /// <summary>
        /// Processes an interaction from another object
        /// </summary>
        /// <param name="interactor"></param>
        public virtual void OnInteract(WorldObject interactor)
        {
        }
    }
}
