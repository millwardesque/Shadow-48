using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shadow_48
{
    /// <summary>
    /// Set of frames for a single animation
    /// </summary>
    class FrameSet
    {
        List<int> _frames;  // Set of frames of animation
        bool _loop = false; // True if this animation should loop

        /// <summary>
        /// Frames of animation
        /// </summary>
        public List<int> Frames
        {
            get { return _frames; }
            set { _frames = value; }
        }

        /// <summary>
        /// Whether or not the animation should loop
        /// </summary>
        public bool Loop
        {
            get { return _loop; }
            set { _loop = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="frames"></param>
        /// <param name="loop"></param>
        public FrameSet(List<int> frames, bool loop)
        {
            _frames = frames;
            _loop = loop;
        }

        /// <summary>
        /// Gets a specific frame of animation
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int GetFrame(int index)
        {
            if (index < 0 && index >= Frames.Count)
            {
                throw new IndexOutOfRangeException("Unable to get animation frame using index " + index);
            }

            return _frames[index];
        }
    }
}
