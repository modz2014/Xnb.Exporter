using Microsoft.Xna.Framework;

namespace Xnb.Exporter
{
    public class AnimationClip
    {
        public int FrameCount { get; set; }

        public Matrix GetTransform(int boneIndex, int frameIndex)
        {

            return Matrix.Identity;
        }
    }
}
