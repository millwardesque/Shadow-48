using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Shadow_48
{
    /// <summary>
    /// A node in the scene-graph
    /// </summary>
    class SceneNode
    {
        private SceneNode _parent = null;   // Parent node
        private List<SceneNode> _children;  // Child nodes
        private String _name;   // Name of the node;
        protected Vector2 _position;  // Position of the node

        /// <summary>
        /// Position of the object
        /// </summary>
        public virtual Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;
            }
        }

        /// <summary>
        /// Parent node
        /// </summary>
        public SceneNode Parent
        {
            get { return _parent; }
            set {
                // Remove myself from my current parent's list of children
                if (null != _parent)
                {
                    _parent.Children.Remove(this);
                }

                // Add myself to my new parent's list of children
                _parent = value;
                if (null != _parent)
                {
                    _parent.Children.Add(this);
                }
            }
        }

        /// <summary>
        /// Gets the list of children
        /// </summary>
        public List<SceneNode> Children
        {
            get { return _children; }
        }

        /// <summary>
        /// Gets the root of the graph
        /// </summary>
        public SceneNode Root
        {
            get
            {
                if (Parent != null)
                {
                    return Parent.Root;
                }
                else
                {
                    return this;
                }
            }
        }

        /// <summary>
        /// Name of the node
        /// </summary>
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parent">Parent of the node</param>
        /// <param name="name">Name of the node</param>
        public SceneNode(SceneNode parent, String name)
        {
            Parent = parent;

            Name = name;
            _children = new List<SceneNode>();
        }

        /// <summary>
        /// Finds a node in the scene graph
        /// </summary>
        /// <param name="startNode">Root node to start searching from</param>
        /// <param name="name">Name of the node to find</param>
        public SceneNode FindInGraph(String name) {
            if (Name == name)   // If I'm the node, stop searching and return me
            {
                return this;
            }
            else if (_children.Count > 0)   // Otherwise, check my children
            {
                for (int i = 0; i < _children.Count; ++i)
                {
                    SceneNode result = _children[i].FindInGraph(name);
                    if (null != result)
                    {
                        return result;
                    }
                }
            }

            // If I have no children or they can't find the node either, return null
            return null;
        }

        /// <summary>
        /// Finds a set of all collidables
        /// </summary>
        /// <param name="collidables">List to add the collidables to</param>
        public void FindCollidables(ref List<WorldObject> collidables)
        {
            // Check to see if I'm a world object
            if (this is WorldObject)
            {
                collidables.Add((WorldObject)this);
            }

            // Check children for collidables
            if (_children.Count > 0)
            {
                for (int i = 0; i < _children.Count; ++i)
                {
                    _children[i].FindCollidables(ref collidables);
                }
            }
        }

        /// <summary>
        /// Finds a set of all renderables
        /// </summary>
        /// <param name="renderables">List to add the renderables to</param>
        public void FindRenderables(ref List<WorldObject> renderables)
        {
            // Check to see if I'm a world object
            if (this is WorldObject)
            {
                renderables.Add((WorldObject)this);
            }

            // Check children for renderables
            if (_children.Count > 0)
            {
                for (int i = 0; i < _children.Count; ++i)
                {
                    _children[i].FindRenderables(ref renderables);
                }
            }
        }

        /// <summary>
        /// Finds a set of all updatables
        /// </summary>
        /// <param name="updatables">List to add the updatables to</param>
        public void FindUpdatables(ref List<WorldObject> updatables)
        {
            // Check to see if I'm a world object
            if (this is WorldObject)
            {
                updatables.Add((WorldObject)this);
            }

            // Check children for updateables
            if (_children.Count > 0)
            {
                for (int i = 0; i < _children.Count; ++i)
                {
                    _children[i].FindUpdatables(ref updatables);
                }
            }
        }

        /// <summary>
        /// Finds all nearby objects
        /// </summary>
        /// <param name="sourceObject">Source object</param>
        /// <param name="distanceThreshold">The distance from position within which an object is considered close</param>
        /// <param name="nearbyObjects">The list to add nearby objects to</param>
        public void FindNearbyObjects(SceneNode sourceObject, float distanceThreshold, ref List<WorldObject> nearbyObjects)
        {
            // Check if I'm close to the source object
            if (this != sourceObject)
            {
                if (distanceThreshold >= (sourceObject.Position - this.Position).Length())
                {
                    nearbyObjects.Add((WorldObject)this);
                }
            }

            // Check children for nearby objects
            if (_children.Count > 0)
            {
                for (int i = 0; i < _children.Count; ++i)
                {
                    _children[i].FindNearbyObjects(sourceObject, distanceThreshold, ref nearbyObjects);
                }
            }
        }

        /// <summary>
        /// Updates the node
        /// </summary>
        /// <param name="gameTime">Game time</param>
        public virtual void Update(GameTime gameTime)
        {
        }

        /// <summary>
        /// Renders the node
        /// </summary>
        /// <param name="gameTime">Game Time</param>
        /// <param name="batch">Batch used to render the sprite if desired</param>
        /// <param name="renderer">System renderer</param>
        public virtual void Render(GameTime gameTime, SpriteBatch batch, Renderer renderer)
        {
        }
    }
}
