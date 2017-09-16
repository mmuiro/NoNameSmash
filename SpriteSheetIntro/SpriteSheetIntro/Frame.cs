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

        public Frame(Rectangle sourceRectangle)
            : this(sourceRectangle, Vector2.Zero)
        {}

        //public Frame(Rectangle sourceRectangle, Vector2 origin = default(Vector2))
        
        public Frame(Rectangle sourceRectangle, Vector2 origin)
        {
            this.sourceRectangle = sourceRectangle;
            this.origin = origin;
        }
    }
}
