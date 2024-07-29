using Microsoft.Xna.Framework;

namespace Xnb.Exporter
{

    public class AnimationClip
    {
        /**
        * @brief Gets or sets the total number of frames in the animation clip.
        *
        * This property represents the total number of frames available in the animation clip.
        */
        public int FrameCount { get; set; }

        /**
        * @brief Retrieves the transformation matrix for a specific bone at a given frame.
        *
        * This method returns the transformation matrix for the specified bone at the specified frame index.
        * 
        * @param boneIndex The index of the bone for which the transformation matrix is being retrieved.
        * @param frameIndex The index of the frame for which the transformation matrix is being retrieved.
        * 
        * @return A Matrix representing the transformation of the bone at the given frame index.
        */
        public Matrix GetTransform(int boneIndex, int frameIndex)
        {

            return Matrix.Identity;
        }
    }
}
