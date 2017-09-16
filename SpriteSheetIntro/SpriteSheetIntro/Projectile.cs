using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpriteSheetIntro
{
    public class Projectile : Sprite
    {
        private Vector2 speed;

        public Vector2 Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        private bool shouldDestroy;

        public bool ShouldDestroy
        {
            get { return shouldDestroy; }
            set { shouldDestroy = value; }
        }

        public Projectile(Vector2 position, Texture2D image, Vector2 speed)
            : base(position, image, Color.White)
        {
            this.speed = speed;
        }
    }
}
