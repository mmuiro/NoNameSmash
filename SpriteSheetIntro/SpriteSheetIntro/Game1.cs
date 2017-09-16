using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Xml;

namespace SpriteSheetIntro
{
    

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Character guy;
        public static Sprite Platform;
        public static Sprite MiniPlatform1;
        public static Sprite MiniPlatform2;

        public static Sprite PlatformSLLeft;
        public static Sprite PlatformSLRight;
        public static Sprite PlatformMLeft;
        public static Sprite PlatformMRight;
        public static Sprite PlatformMCenter;

        public static Sprite PlatformLLLeft;
        public static Sprite PlatformLLRight;
        public static Sprite PlatformMLLeft;
        public static Sprite PlatformMLRight;
        public static Sprite PlatformMLCenter;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 1200;
            graphics.PreferredBackBufferHeight = 600;
            graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            #region XmlLoading

            Dictionary<AnimationType, Animation> animations = new Dictionary<AnimationType, Animation>();


            List<Frame> Frames = new List<Frame>();

            XmlDocument document = new XmlDocument();
            document.Load("Character.xml");

            foreach (XmlElement animation in document.GetElementsByTagName("Animation"))
            {
                AnimationType animationName = (AnimationType)Enum.Parse(typeof(AnimationType), animation.Attributes["Name"].Value);
                List<Frame> animationFrames = new List<Frame>();
                TimeSpan animationSpeed = TimeSpan.FromMilliseconds(Convert.ToInt32(animation.Attributes["Speed"].Value));
                bool animationLooping = bool.Parse(animation.Attributes["Looping"].Value);

                foreach (XmlElement frame in animation.GetElementsByTagName("Frame"))
                {
                    XmlElement SourceRectangle = (XmlElement)frame.GetElementsByTagName("SourceRectangle")[0];
                    Rectangle sourceRectangle = new Rectangle(
                        int.Parse(SourceRectangle.GetAttribute("x")),
                        int.Parse(SourceRectangle.GetAttribute("y")),
                        int.Parse(SourceRectangle.GetAttribute("width")),
                        int.Parse(SourceRectangle.GetAttribute("height")));
                    XmlElement Origin = (XmlElement)frame.GetElementsByTagName("Origin")[0];
                    Vector2 origin = new Vector2(
                        int.Parse(Origin.GetAttribute("x")),
                        int.Parse(Origin.GetAttribute("y")));

                    animationFrames.Add(new Frame(sourceRectangle, origin));
                }
                animations.Add(animationName, new Animation(animationFrames, animationSpeed, animationLooping));

            }
            guy = new Character(new Vector2(GraphicsDevice.Viewport.Width/2, 2 * GraphicsDevice.Viewport.Height/3), Content.Load<Texture2D>("COMBO1"), Color.White, animations);
            Platform = new Sprite(new Vector2(GraphicsDevice.Viewport.Width / 2, 2 * GraphicsDevice.Viewport.Height / 3), Content.Load<Texture2D>("platform2"), Color.White);
            Platform.Origin = new Vector2(Platform.Image.Width / 2, 0);
            MiniPlatform1 = new Sprite(new Vector2(GraphicsDevice.Viewport.Width / 3, 2 * GraphicsDevice.Viewport.Height / 6), Content.Load<Texture2D>("platform3"), Color.White);
            MiniPlatform1.Origin = new Vector2(Platform.Image.Width / 2, 0);
            MiniPlatform2 = new Sprite(new Vector2(GraphicsDevice.Viewport.Width / 0.94f, 2 * GraphicsDevice.Viewport.Height / 6), Content.Load<Texture2D>("platform3"), Color.White);
            MiniPlatform2.Origin = new Vector2(Platform.Image.Width / 2, 0);

            PlatformSLLeft = new Sprite(new Vector2(GraphicsDevice.Viewport.Width / 3.425f, .716f * GraphicsDevice.Viewport.Height), Content.Load<Texture2D>("PlatformblockLarge"), Color.White);
            PlatformSLLeft.Origin = new Vector2(PlatformSLLeft.Image.Width / 2, 0);
            PlatformSLRight = new Sprite(new Vector2(GraphicsDevice.Viewport.Width / 1.4125f, .716f * GraphicsDevice.Viewport.Height), Content.Load<Texture2D>("PlatformblockLarge"), Color.White);
            PlatformSLRight.Origin = new Vector2(PlatformSLRight.Image.Width / 2, 0);
            PlatformMLeft = new Sprite(new Vector2(GraphicsDevice.Viewport.Width / 2.4f, .74f * GraphicsDevice.Viewport.Height), Content.Load<Texture2D>("PlatformBlockMedium"), Color.White);
            PlatformMLeft.Origin = new Vector2(PlatformMLeft.Image.Width / 2, 0);
            PlatformMRight = new Sprite(new Vector2(GraphicsDevice.Viewport.Width / 1.7145f, .74f * GraphicsDevice.Viewport.Height), Content.Load<Texture2D>("PlatformBlockMedium"), Color.White);
            PlatformMRight.Origin = new Vector2(PlatformMRight.Image.Width / 2, 0);
            PlatformMCenter = new Sprite(new Vector2(GraphicsDevice.Viewport.Width / 2, .7659f * GraphicsDevice.Viewport.Height), Content.Load<Texture2D>("PlatformBlockMedium"), Color.White);
            PlatformMCenter.Origin = new Vector2(PlatformMCenter.Image.Width / 2, 0);

            PlatformLLLeft = new Sprite(new Vector2(GraphicsDevice.Viewport.Width / 3.425f, .705f * GraphicsDevice.Viewport.Height), Content.Load<Texture2D>("PlatformSLlayer"), Color.White);
            PlatformLLLeft.Origin = new Vector2(PlatformSLLeft.Image.Width / 2, 0);
            PlatformLLRight = new Sprite(new Vector2(GraphicsDevice.Viewport.Width / 1.4125f, .705f * GraphicsDevice.Viewport.Height), Content.Load<Texture2D>("PlatformSLlayer"), Color.White);
            PlatformLLRight.Origin = new Vector2(PlatformSLRight.Image.Width / 2, 0);
            PlatformMLLeft = new Sprite(new Vector2(GraphicsDevice.Viewport.Width / 2.4f, .729f * GraphicsDevice.Viewport.Height), Content.Load<Texture2D>("PlatformMBlock"), Color.White);
            PlatformMLLeft.Origin = new Vector2(PlatformMLeft.Image.Width / 2, 0);
            PlatformMLRight = new Sprite(new Vector2(GraphicsDevice.Viewport.Width / 1.7145f, .729f * GraphicsDevice.Viewport.Height), Content.Load<Texture2D>("PlatformMBlock"), Color.White);
            PlatformMLRight.Origin = new Vector2(PlatformMRight.Image.Width / 2, 0);
            PlatformMLCenter = new Sprite(new Vector2(GraphicsDevice.Viewport.Width / 2, .7549f * GraphicsDevice.Viewport.Height), Content.Load<Texture2D>("PlatformMBlock"), Color.White);
            PlatformMLCenter.Origin = new Vector2(PlatformMCenter.Image.Width / 2, 0);
            //^^^^ Same ^^^^
            //for (int i = 0; i < list.Count; i++)
            //{
            //    XmlNode node = list[i];
            //}




            #endregion

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }



        protected override void Update(GameTime gameTime)
        {
            guy.Update(gameTime);
            Platform.Update(gameTime);
            MiniPlatform1.Update(gameTime);
            MiniPlatform2.Update(gameTime);
            PlatformSLLeft.Update(gameTime);
            PlatformSLRight.Update(gameTime);
            PlatformMLeft.Update(gameTime);
            PlatformMRight.Update(gameTime);
            PlatformMCenter.Update(gameTime);

            PlatformLLLeft.Update(gameTime);
            PlatformLLRight.Update(gameTime);
            PlatformMLLeft.Update(gameTime);
            PlatformMLRight.Update(gameTime);
            PlatformMLCenter.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);


            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);

            Platform.Draw(spriteBatch);
            MiniPlatform1.Draw(spriteBatch);
            MiniPlatform2.Draw(spriteBatch);
            //PlatformSLLeft.Draw(spriteBatch);
            //PlatformSLRight.Draw(spriteBatch);
            PlatformMLeft.Draw(spriteBatch);
            PlatformMRight.Draw(spriteBatch);
            PlatformMCenter.Draw(spriteBatch);

            PlatformLLLeft.Draw(spriteBatch);
            PlatformLLRight.Draw(spriteBatch);
            PlatformMLLeft.Draw(spriteBatch);
            PlatformMLRight.Draw(spriteBatch);
            PlatformMLCenter.Draw(spriteBatch);
            guy.Draw(spriteBatch);

            Texture2D pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData<Color>(new Color[] { Color.Red });

            //spriteBatch.Draw(pixel, MiniPlatform1.BoundingBox, Color.Red);

            //spriteBatch.DrawString(Content.Load<SpriteFont>("font"), guy.currentAnimation.ToString(), Vector2.Zero, Color.Black);

            spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
