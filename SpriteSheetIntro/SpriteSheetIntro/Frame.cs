using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SpriteSheetIntro
{
    public class Frame
    {
        private Rectangle sourceRectangle;

        public Rectangle SourceRectangle
        {
            get { return sourceRectangle; }
            set { sourceRectangle = value; }
        }

        private Vector2 origin;

        public Vector2 Origin
        {
            get { return origin; }
            set { origin = value; }
        }

        private Rectangle damageRectangle;

        /// <summary>
        /// Damage HitBox:
        /// In XML: The HitBox element describes where the hitbox is RELATIVE TO THE PLAYER'S ORIGIN
        /// </summary>
        public Rectangle DamageRectangle
        {
            get { return damageRectangle; }
            set { damageRectangle = value; }
        }
           

        public Frame(Rectangle sourceRectangle, Rectangle damageRectangle)
            : this(sourceRectangle, Vector2.Zero, damageRectangle)
        {}

        public Frame(Rectangle sourceRectangle, Vector2 origin = default(Vector2)) { }
        
        public Frame(Rectangle sourceRectangle, Vector2 origin, Rectangle damageRectangle)
        {
            this.sourceRectangle = sourceRectangle;
            this.origin = origin;
            this.damageRectangle = damageRectangle;
        }
    }
}
