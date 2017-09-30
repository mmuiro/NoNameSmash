using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpriteSheetIntro
{
    public class Sprite
    {
        protected static Texture2D pixel;

        protected Vector2 _jump;
        protected Vector2 _position;
        public Vector2 Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
            }
        }
        public float X
        {
            get
            {
                return _position.X;
            }
            set
            {
                _position.X = value;
            }
        }
        public float Y
        {
            get
            {
                return _position.Y;
            }
            set
            {
                _position.Y = value;
            }
        }

        protected Color _color;
        public Color Color
        {
            get
            {
                return _color;
            }
        }

        protected Rectangle _sourceRectangle;
        public Rectangle SourceRectangle
        {
            get
            {
                return _sourceRectangle;
            }
            set
            {
                _sourceRectangle = value;
            }
        }

        protected Texture2D _image;
        public Texture2D Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
            }
        }
        public int Width
        {
            get
            {
                return _sourceRectangle.Width;
            }
        }
        public int Height
        {
            get
            {
                return _sourceRectangle.Height;
            }
        }

        protected Rectangle _boundingBox;
        public Rectangle BoundingBox
        {
            get
            {
                return _boundingBox;
            }
        }

        protected float _rotation;
        public float Rotation
        {
            get
            {
                return _rotation;
            }
            set
            {
                _rotation = value;
            }

        }

        protected Vector2 _origin;
        public Vector2 Origin
        {
            get
            {
                return _origin;
            }
            set
            {
                _origin = value;
            }

        }
        protected Rectangle _damageRectangle;
            public Rectangle DamageRectangle
        {
            get
            {
                return _damageRectangle;
            }
            set
            {
                _damageRectangle = value;
            }
        }

        protected Vector2 _scale;
        public Vector2 Scale
        {
            get
            {
                return _scale;
            }
            set
            {
                _scale = value;
            }

        }

        protected SpriteEffects _effects;
        public SpriteEffects Effects
        {
            get
            {
                return _effects;
            }
            set
            {
                _effects = value;
            }

        }

        protected float _layerDepth;
        public float Layerdepth
        {
            get
            {
                return _layerDepth;
            }
            set
            {
                _layerDepth = value;
            }

        }

        #region move to other class
        /*
        protected Vector2 _speed;
        protected Vector2 _jump;
        public Vector2 Speed
        {
            get
            {
                return _speed;
            }
            set
            {
                _speed = value;
            }
        }

        public float X
        {
            get
            {
                return _speed.X;
            }
            set
            {
                _speed.X = value;
            }
        }
        public float Y
        {
            get
            {
                return _speed.Y;
            }
            set
            {
                _speed.Y = value;
            }
        }
        */
        #endregion


        public Sprite(Vector2 location, Texture2D image, Color color)
        {
            _position = location;
            _image = image;
            _color = color;
            _scale = new Vector2(1, 1);
            _effects = SpriteEffects.None;
            _rotation = 0;
            _sourceRectangle = new Rectangle(0, 0, image.Width, image.Height);
            _damageRectangle = new Rectangle(0, 0, 0, 0);
            _boundingBox = new Rectangle((int)_position.X, (int)_position.Y, _sourceRectangle.Width, _sourceRectangle.Height);
            if (pixel == null)
            {
                pixel = new Texture2D(image.GraphicsDevice, 1, 1);
                pixel.SetData<Color>(new Color[] { Color.White });
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_image,_position, _sourceRectangle,_color, _rotation, _origin, _scale, _effects, _layerDepth);
        }

        public virtual void DrawHitbox(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            Texture2D pixel = new Texture2D(graphicsDevice, 1, 1);
            spriteBatch.Draw(pixel, _boundingBox, Color.Lerp(Color.Red, Color.Transparent, .5f));
        }

        public virtual void DrawHitbox(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Color color)
        {
            Texture2D pixel = new Texture2D(graphicsDevice, 1, 1);
            pixel.SetData<Color>(new Color[] { Color.White });

            spriteBatch.Draw(pixel, _boundingBox, Color.Lerp(color, Color.Transparent, .5f));
        }

        public virtual void Update(GameTime gameTime)
        {
            _boundingBox.X = (int)(_position.X - _origin.X * _scale.X);
            _boundingBox.Y = (int)(_position.Y - _origin.Y * _scale.Y);
            _boundingBox.Width = (int)(_sourceRectangle.Width * _scale.X);
            _boundingBox.Height = (int)(_sourceRectangle.Height * _scale.Y);

            if (_effects == SpriteEffects.FlipVertically)
            {
                _origin = new Vector2(_origin.X, _sourceRectangle.Height - _origin.Y);
            }
            else if(_effects == SpriteEffects.FlipHorizontally)
            {
                _origin = new Vector2(_sourceRectangle.Width - _origin.X, _origin.Y);
            }
        }
    }
}
