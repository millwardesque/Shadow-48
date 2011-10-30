using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Shadow_48
{
    /// <summary>
    /// Axis-aligned bounding box
    /// </summary>
    class AABB
    {
        private Vector2 _centre;        // Centre of the box
        private Vector2 _halfWidths;    // Half-widths of the box

        /// <summary>
        /// Centre of the box
        /// </summary>
        public Vector2 Centre
        {
            get { return _centre; }
            set { _centre = value; }
        }

        /// <summary>
        /// Half-widths of the box
        /// </summary>
        public Vector2 HalfWidths
        {
            get { return _halfWidths; }
            set { _halfWidths = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="centre">CEntre of the AABB</param>
        /// <param name="halfWidths">Halfwidths of the AABB</param>
        public AABB(Vector2 centre, Vector2 halfWidths)
        {
            Centre = centre;
            HalfWidths = halfWidths;
        }

        /// <summary>
        /// Checks for a collision between 2 AABBs
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public bool DoesCollide(AABB box)
        {
            // If there is no overlap in either the x or the y direction, there can't possible be a collision
            if (Math.Abs(_centre.X - box.Centre.X) > (_halfWidths.X + box.HalfWidths.X))
            {
                return false;
            }
          
            if (Math.Abs(_centre.Y - box.Centre.Y) > (_halfWidths.Y + box.HalfWidths.Y)) {
                return false;
            }

            // Overlaps were detected in both directions, there must be a collision
            return true;
        }
    }
}
