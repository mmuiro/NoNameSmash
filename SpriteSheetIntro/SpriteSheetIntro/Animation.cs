﻿using System;
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

        public List<Frame> Frames = new List<Frame>();

        public TimeSpan AnimationSpeed { get; set; }
        protected TimeSpan animationTime;
        protected int currentFrameIndex;
        public bool Looping { get; set; }

        public event EventHandler AnimationFinished;

        public Animation(List<Frame> frames)
            :this(frames, TimeSpan.FromMilliseconds(250)){ }

        public Animation(List<Frame> frames, TimeSpan animationSpeed, bool looping = true)
        {
            Frames = frames;
            AnimationSpeed = animationSpeed;
            animationTime = TimeSpan.Zero;
            currentFrameIndex = 0;
            Looping = looping;
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
                    if (AnimationFinished != null)
                    {
                        AnimationFinished(this, EventArgs.Empty);
                    }
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