using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SpriteSheetIntro
{
    public class Animation
    {
        public Frame CurrentFrame
        {
            get
            {
                return Frames[currentFrameIndex];
            }
        }

        //This might need to be moved into an "AttackAnimation" class
        
        
        public AnimationType AnimationType { get; private set; }
        public int PlayerID { get; set;  }

        public List<Frame> Frames = new List<Frame>();

        public TimeSpan AnimationSpeed { get; set; }
        protected TimeSpan animationTime;
        protected int currentFrameIndex;
        public bool Looping { get; set; }

        public event EventHandler<AnimationEventArgs> AnimationFinished;

        public Animation(List<Frame> frames, AnimationType animationType)
            : this(frames, TimeSpan.FromMilliseconds(250), animationType) { }

        public Animation(List<Frame> frames, TimeSpan animationSpeed, AnimationType animationType, bool looping = true)
        {
            Frames = frames;
            AnimationSpeed = animationSpeed;
            animationTime = TimeSpan.Zero;
            currentFrameIndex = 0;
            Looping = looping;

            this.AnimationType = animationType;
        }

        public virtual void Update(GameTime gameTime)
        {
            animationTime += gameTime.ElapsedGameTime;
            if (animationTime >= AnimationSpeed)
            {
                animationTime = TimeSpan.Zero;
                currentFrameIndex++;
                if (currentFrameIndex >= Frames.Count)
                {
                    currentFrameIndex = Looping ? 0 : Frames.Count - 1;
                    AnimationFinished?.Invoke(this, new AnimationEventArgs(this.AnimationType, PlayerID));
                }

                
            }
        }

        public void Reset()
        {
            currentFrameIndex = 0;
            animationTime = TimeSpan.Zero;
        }
    }
}
