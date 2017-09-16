using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpriteSheetIntro
{
    public class AnimatingProjectile : Projectile
    {
        private Animation animation;

        public Animation Animation
        {
            get { return animation; }
            set { animation = value; }
        }

        public AnimatingProjectile(Vector2 position, Texture2D image, Vector2 speed, Animation animation)
            : base(position, image, speed)
        {
            this.animation = animation;
            SourceRectangle = animation.CurrentFrame.SourceRectangle;
            Origin = animation.CurrentFrame.Origin;
            animation.AnimationFinished += new EventHandler(animation_AnimationFinished);
        }

        void animation_AnimationFinished(object sender, EventArgs e)
        {
            ShouldDestroy = true;
        }

        public override void Update(GameTime gameTime)
        {
            animation.Update(gameTime);
            SourceRectangle = animation.CurrentFrame.SourceRectangle;
            Origin = animation.CurrentFrame.Origin;
            base.Update(gameTime);
        }
    }
}
