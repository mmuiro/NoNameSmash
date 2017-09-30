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
    //IS DONE IS DONE I IS DONE

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics; 
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;
        Character Player1;
        Character Player2;
        bool GameActive = true;
        int hitstun = 10000;
        public static Sprite Platform;
        public static Sprite MiniPlatform1;
        public static Sprite MiniPlatform2;
        public string LivesPlayer1 = "";
        public string LivesPlayer2 = "";
        string LInd1 = "Lives:";
        string LInd2 = "Lives:";
        string Damage1 = "";
        string Damage2 = "";
        string HitCounter = "";
        string finalmsg = "";
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

        public static Texture2D pixel;

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
            spriteFont = Content.Load<SpriteFont>("font");
            #region XmlLoading

            Dictionary<AnimationType, Animation> animations = new Dictionary<AnimationType, Animation>();

            Dictionary<AnimationType, Animation> animations2 = new Dictionary<AnimationType, Animation>();


            //List<Frame> Frames = new List<Frame>();

            XmlDocument document = new XmlDocument();
            document.Load("Character.xml");

            int count = 0;
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
                    XmlElement DamageRectangle = (XmlElement)frame.GetElementsByTagName("Hitbox")[0];
                    Rectangle damageRectangle = new Rectangle(
                        int.Parse(DamageRectangle.GetAttribute("x")),
                        int.Parse(DamageRectangle.GetAttribute("y")),
                        int.Parse(DamageRectangle.GetAttribute("width")),
                        int.Parse(DamageRectangle.GetAttribute("height")));

                    animationFrames.Add(new Frame(sourceRectangle, origin, damageRectangle));
                    count++;
                }
                animations.Add(animationName, new Animation(animationFrames, animationSpeed, animationName, animationLooping));
                animations2.Add(animationName, new Animation(animationFrames, animationSpeed, animationName, animationLooping));
            }

            Player1 = new Character("Player1", new Vector2(GraphicsDevice.Viewport.Width / 3, 2 * GraphicsDevice.Viewport.Height / 3), Content.Load<Texture2D>("COMBO1"), Color.White, animations, 1);
            Player2 = new Character("Player2", new Vector2(2 * GraphicsDevice.Viewport.Width / 3, 2 * GraphicsDevice.Viewport.Height / 3), Content.Load<Texture2D>("COMBO2"), Color.White, animations2, 2);

            Player2.KeyUp = Keys.Up;
            Player2.KeyJump = Keys.NumPad0;
            Player2.KeyDown = Keys.Down;
            Player2.KeyRight = Keys.Right;
            Player2.KeyLeft = Keys.Left;
            Player2.KeySpecial = Keys.NumPad1;
            Player2.KeySmash = Keys.NumPad2;
            Player2.KeyShield = Keys.NumPad3;

            Player1.createAction();
            Player2.createAction();

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
            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData<Color>(new Color[] { Color.HotPink });
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

        TimeSpan elapsedTime = TimeSpan.Zero;
        int player1UpdateCount = 0;
        KeyboardState lastKeyboardState;

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            Player1.Update(gameTime, keyboardState, lastKeyboardState);
            Player2.Update(gameTime, keyboardState, lastKeyboardState);

            elapsedTime += gameTime.ElapsedGameTime;
            if (elapsedTime >= TimeSpan.FromSeconds(1))
            {
                player1UpdateCount = Player1.NumberOfUpdates;
                Player1.NumberOfUpdates = 0;
                elapsedTime = TimeSpan.Zero;
            }


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

            lastKeyboardState = keyboardState;
            LivesPlayer1 = Player1.Lives.ToString();
            LivesPlayer2 = Player2.Lives.ToString();
            HitCounter = Player2.multihitcounter.ToString();
            Damage1 = Player1.DamageCounter.ToString();
            Damage2 = Player2.DamageCounter.ToString();
            if (GameActive)
                if (Player1.Lives <= 0)
                {
                    finalmsg = "Player 2 is the Winner!";
                    GameActive = false;
                }
            if (Player2.Lives <= 0)
            {
                finalmsg = "Player 1 is the Winner!";
                GameActive = false;
            }

            MouseState ms = Mouse.GetState();

            IsMouseVisible = true;
            if (new Rectangle((int)(Player1.X - Player1.Origin.X), (int)(Player1.Y - Player1.Origin.Y), Player1.pixelData.GetLength(0), Player1.pixelData.GetLength(1)).Contains(ms.X, ms.Y))
            {
                if (Player1.pixelData[(int)(ms.X - (Player1.X - Player1.Origin.X)), (int)(ms.Y - (Player1.Y - Player1.Origin.Y))].A != 0f)
                {
                    ;
                }
            }

            if (new Rectangle((int)(Player2.X - Player2.Origin.X), (int)(Player2.Y - Player2.Origin.Y), Player2.pixelData.GetLength(0), Player2.pixelData.GetLength(1)).Contains(ms.X, ms.Y))
            {
                if (Player2.pixelData[(int)(ms.X - (Player2.X - Player2.Origin.X)), (int)(ms.Y - (Player2.Y - Player2.Origin.Y))].A != 0f)
                {
                    ;
                }
            }
            //fix player2 damage systems

            //All Damaging systems for player 1 are here h

            #region NeutralSpecial
            if (Player2.currentAnimation == AnimationType.NeutralSpecial && Player1.hitcount == 0 && Player2.delaying == false)
            {

                if (Player1.hitcount == 0)
                {
                    Player1.AttackLength = new TimeSpan();
                    Player1.attacknumber = 1;
                    if (Player1.BoundingBox.Intersects(Player2.dhitbox))
                    {
                        Player1.knockeddiagonal = false;
                        Player2.OmniDirectionalAttack = false;
                        Player1.knockedup = true;
                        if (Player1.currentAnimation != AnimationType.Shield && Player1.currentAnimation != AnimationType.Dodge && Player1.currentAnimation != AnimationType.SpotDodge)
                        {
                            Player1.DamageCounter += 5;
                            Player1.hitcount++;
                            Player1.knockback = -(float)3;
                            Player1.attackDelay = TimeSpan.FromMilliseconds(hitstun);
                            Player1.delaying = true;
                        }


                    }

                }


            }

            else if (Player2.currentAnimation == AnimationType.NeutralSpecial2 && Player1.hitcount == 0 && Player2.delaying == false)
            {

                if (Player1.hitcount == 0)
                {
                    Player1.AttackLength = new TimeSpan();
                    Player1.attacknumber = 2;
                    if (Player1.BoundingBox.Intersects(Player2.dhitbox))
                    {
                        Player1.knockeddiagonal = false;
                        Player2.OmniDirectionalAttack = false;
                        Player1.knockedup = true;
                        if (Player1.currentAnimation != AnimationType.Shield && Player1.currentAnimation != AnimationType.Dodge && Player1.currentAnimation != AnimationType.SpotDodge)
                        {
                            Player1.DamageCounter += 5;
                            Player1.hitcount++;
                            Player1.knockback = -(float)3;
                            Player1.attackDelay = TimeSpan.FromMilliseconds(hitstun);
                            Player1.delaying = true;
                        }
                    }


                }


            }

            else if (Player2.currentAnimation == AnimationType.NeutralSpecial3 && Player1.hitcount == 0 && Player2.delaying == false)
            {

                if (Player1.hitcount == 0)
                {
                    Player1.AttackLength = new TimeSpan();
                    Player1.attacknumber = 3;
                    if (Player1.BoundingBox.Intersects(Player2.dhitbox))
                    {
                        Player1.knockeddiagonal = false;
                        Player2.OmniDirectionalAttack = false;
                        Player1.knockedup = false;
                        if (Player1.currentAnimation != AnimationType.Shield && Player1.currentAnimation != AnimationType.Dodge && Player1.currentAnimation != AnimationType.SpotDodge)
                        {


                            Player1.DamageCounter += 10;
                            Player1.hitcount++;
                            Player1.knockback = Player1.DamageCounter / 5;
                            Player1.attackDelay = TimeSpan.FromMilliseconds(hitstun);
                            Player1.delaying = true;
                        }
                    }
                }


            }

            #endregion
            #region SideSmash
            else if (Player2.currentAnimation == AnimationType.SideSmash && Player1.hitcount == 0 && Player2.delaying == false)
            {

                if (Player1.hitcount == 0)
                {
                    Player1.AttackLength = new TimeSpan();
                    Player1.attacknumber = 4;
                    if (Player1.BoundingBox.Intersects((Player2.dhitbox)))
                    {
                        Player2.OmniDirectionalAttack = false;
                        Player1.knockeddiagonal = true;
                        Player1.knockedup = false;
                        if (Player1.currentAnimation != AnimationType.Shield && Player1.currentAnimation != AnimationType.Dodge && Player1.currentAnimation != AnimationType.SpotDodge)
                        {
                            Player1.DamageCounter += 22;
                            Player1.hitcount++;
                            Player1.knockback = Player1.DamageCounter / 5;
                            Player1.knockbackX = Player1.DamageCounter / 5;
                            Player1.knockbackY = -1 * Player1.DamageCounter / 8;
                        }
                    }
                }

            }
            #endregion
            #region DownSmash
            else if (Player2.currentAnimation == AnimationType.DownSmash && Player1.hitcount == 0 && Player2.delaying == false)
            {
                if (Player1.hitcount == 0)
                {
                    Player1.AttackLength = new TimeSpan();
                    Player1.attacknumber = 5;
                    Player1.knockedup = false;
                    Player2.multihitcounter++;
                    if (Player2.multihitcounter <= 4)
                    {
                        if (Player1.BoundingBox.Intersects(Player2.dhitbox))
                        {
                            Player1.knockeddiagonal = false;
                            Player2.OmniDirectionalAttack = true;
                            if (Player1.currentAnimation != AnimationType.Shield && Player1.currentAnimation != AnimationType.Dodge && Player1.currentAnimation != AnimationType.SpotDodge)
                            {
                                Player1.hitcount++;
                                Player1.DamageCounter += 4;
                                Player1.knockback = -(float)2;
                            }
                        }
                    }
                    else
                    {

                        //Player2.mulithitcounter++;
                        if (Player1.BoundingBox.Intersects(Player2.dhitbox))
                        {
                            Player1.knockedup = false;
                            if (Player1.currentAnimation != AnimationType.Shield && Player1.currentAnimation != AnimationType.Dodge && Player1.currentAnimation != AnimationType.SpotDodge)
                            {
                                Player1.hitcount++;
                                Player1.DamageCounter += 13;
                                Player1.knockback = Player1.DamageCounter / 5;
                            }

                        }
                    }
                }
            }
            #endregion
            #region UpSmash
            else if (Player2.currentAnimation == AnimationType.UpSmash && Player1.hitcount == 0 && Player2.delaying == false)
            {

                if (Player1.hitcount == 0)
                {
                    Player1.AttackLength = new TimeSpan();
                    Player1.attacknumber = 6;
                    if (Player1.BoundingBox.Intersects((Player2.dhitbox)))
                    {
                        Player1.knockeddiagonal = false;
                        Player2.OmniDirectionalAttack = false;
                        Player1.knockedup = true;
                        if (Player1.currentAnimation != AnimationType.Shield && Player1.currentAnimation != AnimationType.Dodge && Player1.currentAnimation != AnimationType.SpotDodge)
                        {
                            Player1.DamageCounter += 16;
                            Player1.Airtime = new TimeSpan();
                            Player1.hitcount++;
                            Player1.knockback = -(Player1.DamageCounter / 6
                                );
                        }
                    }
                }
            }
            #endregion
            #region SideSpecial
            else if (Player2.currentAnimation == AnimationType.SideSpecial && Player1.hitcount == 0 && Player2.delaying == false)
            {

                if (Player1.hitcount == 0)
                {
                    Player1.AttackLength = new TimeSpan();
                    Player1.attacknumber = 7;
                    if (Player1.BoundingBox.Intersects((Player2.dhitbox)))
                    {
                        Player2.OmniDirectionalAttack = false;
                        Player1.knockedup = false;
                        Player1.knockeddiagonal = true;
                        if (Player1.currentAnimation != AnimationType.Shield && Player1.currentAnimation != AnimationType.Dodge && Player1.currentAnimation != AnimationType.SpotDodge)
                        {
                            Player1.DamageCounter += 2;
                            Player1.hitcount++;
                            Player1.Airtime = TimeSpan.FromMilliseconds(470);
                            Player1.gravity = 0f;
                            Player1.knockbackY = -(float)0;
                            Player1.knockbackX = (float)9;
                        }

                    }
                }
            }
            #endregion
            #region DownSpecial
            else if (Player2.currentAnimation == AnimationType.DownSpecial && Player1.hitcount == 0 && Player2.delaying == false)
            {

                if (Player1.hitcount == 0)
                {
                    Player1.AttackLength = new TimeSpan();
                    Player1.attacknumber = 8;
                    Player2.multihitcounter++;
                    if (Player2.multihitcounter <= 9)
                    {
                        if (Player1.BoundingBox.Intersects((Player2.dhitbox)))
                        {
                            Player2.OmniDirectionalAttack = false;
                            Player1.knockeddiagonal = true;
                            Player1.knockedup = false;
                            if (Player1.currentAnimation != AnimationType.Shield && Player1.currentAnimation != AnimationType.Dodge && Player1.currentAnimation != AnimationType.SpotDodge)
                            {
                                Player1.DamageCounter += 6;
                                Player1.hitcount++;
                                Player1.knockbackX = (float)4;
                                Player1.knockbackY = (float)-5;
                            }
                        }
                    }
                    else
                    {
                        if (Player1.BoundingBox.Intersects((Player2.dhitbox)))
                        {
                            Player2.OmniDirectionalAttack = false;
                            Player1.knockeddiagonal = true;
                            Player1.knockedup = false;
                            if (Player1.currentAnimation != AnimationType.Shield && Player1.currentAnimation != AnimationType.Dodge && Player1.currentAnimation != AnimationType.SpotDodge)
                            {
                                Player1.hitcount++;
                                Player1.DamageCounter += 8;
                                Player1.knockbackX = Player1.DamageCounter / 9;
                                Player1.knockbackY = -(Player1.DamageCounter / 9);
                            }
                        }
                    }
                }

            }
            #endregion
            #region NeutralSmash
            else if (Player2.currentAnimation == AnimationType.NeutralSmash && Player1.hitcount == 0 && Player2.delaying == false)
            {
                if (Player1.hitcount == 0)
                {
                    Player1.AttackLength = new TimeSpan();
                    Player1.attacknumber = 9;
                    if (Player1.BoundingBox.Intersects((Player2.dhitbox)))
                    {
                        Player2.OmniDirectionalAttack = false;
                        Player1.knockeddiagonal = false;
                        Player1.knockedup = false;
                        if (Player1.currentAnimation != AnimationType.Shield && Player1.currentAnimation != AnimationType.Dodge && Player1.currentAnimation != AnimationType.SpotDodge)
                        {
                            Player1.DamageCounter += 10;
                            Player1.hitcount++;
                            Player1.knockback = Player1.DamageCounter / 10;
                        }
                    }
                }

            }

            #endregion
            #region UpSpecial/Reco
            else if (Player2.currentAnimation == AnimationType.UpSpecial && Player1.hitcount == 0 && Player2.delaying == false)
            {

                if (Player1.hitcount == 0)
                {
                    Player1.AttackLength = new TimeSpan();
                    Player1.attacknumber = 10;
                    if (Player1.BoundingBox.Intersects((Player2.dhitbox)))
                    {
                        Player2.OmniDirectionalAttack = false;
                        Player1.knockeddiagonal = true;
                        Player1.knockedup = false;
                        if (Player1.currentAnimation != AnimationType.Shield && Player1.currentAnimation != AnimationType.Dodge && Player1.currentAnimation != AnimationType.SpotDodge)
                        {
                            Player1.DamageCounter += 20;

                            Player1.hitcount++;
                            Player1.knockbackX = (Player1.DamageCounter / 7);
                            Player1.knockbackY = -(Player1.DamageCounter / 6);
                        }
                    }
                }

            }
            #endregion
            #region UpAerial
            else if (Player2.currentAnimation == AnimationType.UpAerial && Player1.hitcount == 0 && Player2.delaying == false)
            {

                if (Player1.hitcount == 0)
                {
                    Player1.AttackLength = new TimeSpan();
                    Player1.attacknumber = 11;
                    if (Player1.BoundingBox.Intersects((Player2.dhitbox)))
                    {
                        Player2.OmniDirectionalAttack = false;
                        Player1.knockeddiagonal = false;
                        Player1.knockedup = true;
                        if (Player1.currentAnimation != AnimationType.Shield && Player1.currentAnimation != AnimationType.Dodge && Player1.currentAnimation != AnimationType.SpotDodge)
                        {
                            Player1.DamageCounter += 13;
                            Player1.Airtime = new TimeSpan();
                            Player1.hitcount++;
                            Player1.knockback = -(Player1.DamageCounter / (13 / 2));
                        }
                    }
                }

            }
            #endregion
            #region SideAerial
            else if (Player2.currentAnimation == AnimationType.SideAerial && Player1.hitcount == 0 && Player2.delaying == false)
            {

                if (Player1.hitcount == 0)
                {
                    Player1.AttackLength = new TimeSpan();
                    Player1.attacknumber = 12;
                    if (Player1.BoundingBox.Intersects((Player2.dhitbox)))
                    {
                        Player2.OmniDirectionalAttack = false;
                        Player1.knockeddiagonal = false;
                        Player1.knockedup = false;
                        if (Player1.currentAnimation != AnimationType.Shield && Player1.currentAnimation != AnimationType.Dodge && Player1.currentAnimation != AnimationType.SpotDodge)
                        {
                            Player1.DamageCounter += 16;

                            Player1.hitcount++;
                            Player1.knockback = (Player1.DamageCounter / 6);
                        }
                    }
                }

            }
            #endregion
            #region DownAerial
            else if (Player2.currentAnimation == AnimationType.DownAerial && Player1.hitcount == 0 && Player2.delaying == false)
            {
                if (Player1.hitcount == 0)
                {
                    Player1.AttackLength = new TimeSpan();
                    Player1.attacknumber = 13;
                    if (Player1.BoundingBox.Intersects((Player2.dhitbox)))
                    {
                        {
                            Player2.OmniDirectionalAttack = false;
                            Player1.knockeddiagonal = false;
                            Player1.knockedup = true;
                            if (Player1.currentAnimation != AnimationType.Shield && Player1.currentAnimation != AnimationType.Dodge && Player1.currentAnimation != AnimationType.SpotDodge)
                            {
                                Player1.DamageCounter += 18;

                                Player1.hitcount++;
                                Player1.knockback = (Player1.DamageCounter / 5);
                            }
                        }
                    }

                }
            }
            #endregion
            #region NeutralAerial
            else if (Player2.currentAnimation == AnimationType.NeutralAerial && Player1.hitcount == 0 && Player2.delaying == false)
            {

                if (Player1.hitcount == 0)
                {
                    Player1.AttackLength = new TimeSpan();
                    Player1.attacknumber = 14;
                    if (Player1.BoundingBox.Intersects((Player2.dhitbox)))
                    {
                        Player2.OmniDirectionalAttack = false;
                        Player1.knockeddiagonal = false;
                        Player1.knockedup = false;
                        if (Player1.currentAnimation != AnimationType.Shield && Player1.currentAnimation != AnimationType.Dodge && Player1.currentAnimation != AnimationType.SpotDodge)
                        {
                            Player1.DamageCounter += 11;

                            Player1.hitcount++;
                            Player1.knockback = (Player1.DamageCounter / 8);
                        }
                    }
                }

            }
            #endregion

            if (Player1.BoundingBox.Intersects(Player2.dhitbox) && Player2.stunned == false)
            {
                if (Player1.currentAnimation != AnimationType.Shield && Player1.currentAnimation != AnimationType.Dodge && Player1.currentAnimation != AnimationType.SpotDodge)
                {
                    if (Player2.Effects == SpriteEffects.FlipHorizontally)
                    {
                        Player1.knockedleft = true;
                        Player1.knockedright = false;
                    }
                    else
                    {
                        Player1.knockedright = true;
                        Player1.knockedleft = false;
                    }
                    Character.Stunned(Player1);
                }
            }



            #region AttackSelectionTimer
            if (Player1.attacknumber == 1)
            {
                Player1.AttackLength += gameTime.ElapsedGameTime;
                if (Player1.AttackLength >= TimeSpan.FromMilliseconds(240))
                {
                    Player1.hitcount = 0;
                    Player1.AttackLength = new TimeSpan();
                }
            }
            else if (Player1.attacknumber == 2)
            {
                Player1.AttackLength += gameTime.ElapsedGameTime;
                if (Player1.AttackLength >= TimeSpan.FromMilliseconds(240))
                {
                    Player1.hitcount = 0;
                    Player1.AttackLength = new TimeSpan();
                }
            }
            else if (Player1.attacknumber == 3)
            {
                Player1.AttackLength += gameTime.ElapsedGameTime;
                if (Player1.AttackLength >= TimeSpan.FromMilliseconds(480))
                {
                    Player1.hitcount = 0;
                    Player1.AttackLength = new TimeSpan();
                }
            }
            else if (Player1.attacknumber == 4)
            {
                Player1.AttackLength += gameTime.ElapsedGameTime;
                if (Player1.AttackLength >= TimeSpan.FromMilliseconds(504))
                {
                    Player1.hitcount = 0;
                    Player1.AttackLength = new TimeSpan();
                }
            }
            else if (Player1.attacknumber == 5)
            {
                Player1.AttackLength += gameTime.ElapsedGameTime;
                if (Player2.multihitcounter < 5)
                {
                    if (Player1.AttackLength >= TimeSpan.FromMilliseconds(50))
                    {
                        Player1.hitcount = 0;
                        Player1.AttackLength = new TimeSpan();
                    }
                }
                else
                {
                    if (Player1.AttackLength >= TimeSpan.FromMilliseconds(280))
                    {
                        Player1.hitcount = 0;
                        Player1.AttackLength = new TimeSpan();
                        Player2.multihitcounter = 0;


                    }
                }
            }
            else if (Player1.attacknumber == 6)
            {

                Player1.AttackLength += gameTime.ElapsedGameTime;
                if(Player1.AttackLength >= TimeSpan.FromMilliseconds(450))
                {
                    Player1.hitcount = 0;
                    Player1.AttackLength = new TimeSpan();
                }
            }
            else if(Player1.attacknumber == 7)
            {
                Player1.AttackLength += gameTime.ElapsedGameTime;
                if(Player1.AttackLength >= TimeSpan.FromMilliseconds(65))
                {
                    Player1.hitcount = 0;
                    Player1.AttackLength = new TimeSpan();
                }
            }
            else if (Player1.attacknumber == 8)
            {
                Player1.AttackLength += gameTime.ElapsedGameTime;
                if (Player2.multihitcounter <10)
                {
                    if (Player1.AttackLength >= TimeSpan.FromMilliseconds(60))
                    {
                        Player1.hitcount = 0;
                        Player1.AttackLength = new TimeSpan();
                    }
                }
                else
                {
                    if (Player1.AttackLength >= TimeSpan.FromMilliseconds(280))
                    {
                        Player1.hitcount = 0;
                        Player1.AttackLength = new TimeSpan();
                        Player2.multihitcounter = 0;


                    }
                }
            }
            else if (Player1.attacknumber == 9)
            {
                Player1.AttackLength += gameTime.ElapsedGameTime;
                if (Player1.AttackLength >= TimeSpan.FromMilliseconds(370))
                {
                    Player1.hitcount = 0;
                    Player1.AttackLength = new TimeSpan();
                }
            }
            else if (Player1.attacknumber == 10)
            {
                Player1.AttackLength += gameTime.ElapsedGameTime;
                if (Player1.AttackLength >= TimeSpan.FromMilliseconds(370))
                {
                    Player1.hitcount = 0;
                    Player1.AttackLength = new TimeSpan();
                }
            }
            else if (Player1.attacknumber == 11)
            {
                Player1.AttackLength += gameTime.ElapsedGameTime;
                if (Player1.AttackLength >= TimeSpan.FromMilliseconds(350))
                {
                    Player1.hitcount = 0;
                    Player1.AttackLength = new TimeSpan();
                }
            }
            else if (Player1.attacknumber == 12)
            {
                Player1.AttackLength += gameTime.ElapsedGameTime;
                if (Player1.AttackLength >= TimeSpan.FromMilliseconds(425))
                {
                    Player1.hitcount = 0;
                    Player1.AttackLength = new TimeSpan();
                }
            }
            else if (Player1.attacknumber == 13)
            {
                Player1.AttackLength += gameTime.ElapsedGameTime;
                if (Player1.AttackLength >= TimeSpan.FromMilliseconds(540))
                {
                    Player1.hitcount = 0;
                    Player1.AttackLength = new TimeSpan();
                }
            }
            else if (Player1.attacknumber == 14)
            {
                Player1.AttackLength += gameTime.ElapsedGameTime;
                if (Player1.AttackLength >= TimeSpan.FromMilliseconds(280))
                {
                    Player1.hitcount = 0;
                    Player1.AttackLength = new TimeSpan();
                }
            }


            #endregion
            //Knockback Displacement calculation is here
            #region KnockbackCalculator
            #region DiagonalKnockback
            if (Player1.knockeddiagonal == true)
            {
                if (Player1.knockedleft)
                {
                    if (Player1.knockbackX > 0.1f)
                    {
                        Player1.X += -1 * Player1.knockbackX;
                        Player1.knockbackX--;
                        Math.Floor(Player1.knockbackX);
                        if (Player1.knockbackY > 0f)
                        {
                            Player1.Y += Player1.knockbackY / 2;
                            //Player1.knockbackY++;
                            Math.Floor(Player1.knockbackY);
                        }
                        else if (Player1.knockbackY < 0f)
                        {
                            Player1.Y += Player1.knockbackY / 2;
                            //Player1.knockbackY--;
                            Math.Floor(Player1.knockbackY);
                        }
                    }

                    else if (Player1.knockbackX < -0.1f)
                    {
                        Player1.X += -1 * Player1.knockbackX;
                        Player1.knockbackX++;
                        Math.Floor(Player1.knockbackY);

                        if (Player1.knockbackY > 0f)
                        {
                            Player1.Y += Player1.knockbackY / 2;
                            //Player1.knockbackY++;
                            Math.Floor(Player1.knockbackY);
                        }
                        else if (Player1.knockbackY < 0f)
                        {
                            Player1.Y += Player1.knockbackY / 2;
                            //Player1.knockbackY--;
                            Math.Floor(Player1.knockbackY);
                        }

                    }
                }

                else if (Player1.knockedright)
                {

                        if (Player1.knockbackX > 0.1f)
                        {
                            Player1.X += 1 * Player1.knockbackX;
                            Player1.knockbackX--;
                            Math.Floor(Player1.knockbackX);
                            if (Player1.knockbackY > 0f)
                            {
                                Player1.Y += Player1.knockbackY / 2;
                                //Player1.knockbackY++;
                                Math.Floor(Player1.knockbackY);
                            }
                            else if (Player1.knockbackY < -0f)
                            {
                                Player1.Y += Player1.knockbackY / 2;
                                //Player1.knockbackY--;
                                Math.Floor(Player1.knockbackY);
                            }
                        }
                        else if (Player1.knockbackX < -0.1f)
                        {
                            Player1.X += 1 * Player1.knockbackX;
                            Player1.knockbackX++;
                            Math.Floor(Player1.knockbackY);

                            if (Player1.knockbackY > 0f)
                            {
                                Player1.Y += Player1.knockbackY / 2;
                                //Player1.knockbackY++;
                                Math.Floor(Player1.knockbackY);
                            }
                            else if (Player1.knockbackY < 0f)
                            {
                                Player1.Y += Player1.knockbackY / 2;
                                //Player1.knockbackY--;
                                Math.Floor(Player1.knockbackY);
                            }



                        }

                }
            }
            #endregion
            #region OmniDirectional
            if (Player2.OmniDirectionalAttack == true)
            {
                if (Player1.knockedup == false)
                {
                if (Player1.X <= Player2.X)
                {
                    
                   
                        if (Player1.knockback > .01f)
                        {
                            Player1.X += -1 * Player1.knockback;
                            Player1.knockback--;
                            Math.Floor(Player1.knockback);
                        }
                        else if (Player1.knockback < -.01f)
                        {
                            Player1.X += 1 * Player1.knockback;
                            Player1.knockback++;
                            Math.Floor(Player1.knockback);
                        }
                        else { Player1.knockback = 0f; }
                    }
                    else if (Player1.X >= Player2.X)
                    {

                            if (Player1.knockback > .01f)
                            {
                                Player1.X += 1 * Player1.knockback;
                                Player1.knockback--;
                                Math.Floor(Player1.knockback);
                            }
                            else if (Player1.knockback < -.01f)
                            {
                                Player1.X += -1 * Player1.knockback;
                                Player1.knockback++;
                                Math.Floor(Player1.knockback);
                            }
                            else { Player1.knockback = 0f; }
                        
                    }
                }
                else if (Player1.knockedup == true)
                {
                    if (Player1.knockback > .05f)
                    {
                        Player1.Y += Player1.knockback;
                        Player1.knockback--;
                    }
                    else if (Player1.knockback < -.05f)
                    {
                        Player1.Y += Player1.knockback;
                        Player1.knockback++;
                    }
                }
            }
            #endregion
            #region NormalKnockback
            else if (Player1.knockback < .01f || Player1.knockback> -0.1f)
            {
                if (Player1.knockedup == false)
                {
                    if (Player1.knockedleft)
                    {
                        if (Player1.knockback > .01f)
                        {
                            Player1.X += -1 * Player1.knockback;
                            Player1.knockback--;
                            Math.Floor(Player1.knockback);
                        }
                        else if (Player1.knockback < -.01f)
                        {
                            Player1.X += -1 * Player1.knockback;
                            Player1.knockback++;
                            Math.Floor(Player1.knockback);
                        }
                        else { Player1.knockback = 0f; }
                    }
                    else if (Player1.knockedright)
                    {
                        if (Player1.knockback > .01f)
                        {
                            Player1.X += 1 * Player1.knockback;
                            Player1.knockback--;
                            Math.Floor(Player1.knockback);
                        }
                        else if (Player1.knockback < -.01f)
                        {
                            Player1.X += 1 * Player1.knockback;
                            Player1.knockback++;
                            Math.Floor(Player1.knockback);
                        }
                        else { Player1.knockback = 0f; }
                    }
                }
                else if (Player1.knockedup == true)
                {
                    if (Player1.knockback > .05f)
                    {
                        Player1.Y += Player1.knockback;
                        Player1.knockback--;
                        Player1.feethitbox.Y += 1;
                        if(Player1.feethitbox.Intersects(Platform.BoundingBox))
                        {
                            Player1.Y -= Player1.knockback;
                            Player1.knockback = -1 * (Player1.knockback);
                            Player1.Y--;
                            Math.Floor(Player1.knockback);
                        }
                    }
                    else if (Player1.knockback < -.05f)
                    {
                        Player1.Y += Player1.knockback;
                        Player1.knockback++;
                        if (Player1.feethitbox.Intersects(Platform.BoundingBox))
                        {
                            //Player1.Y  -= Player1.knockback;
                            

                        }


                    }
                    else { Player1.knockback = 0f; }
                }
            }
            #endregion
            #endregion
            //Damage systems for player 2 are here

            #region NeutralSpecial
            if (Player1.currentAnimation == AnimationType.NeutralSpecial && Player2.hitcount == 0 && Player1.delaying == false)
            {

                if (Player2.hitcount == 0)
                {
                    Player2.AttackLength = new TimeSpan();
                    Player2.attacknumber = 1;
                    if (Player2.BoundingBox.Intersects(Player1.dhitbox))
                    {
                        Player2.knockeddiagonal = false;
                        Player1.OmniDirectionalAttack = false;
                        Player2.knockedup = true;
                        if (Player2.currentAnimation != AnimationType.Shield && Player2.currentAnimation != AnimationType.Dodge && Player2.currentAnimation != AnimationType.SpotDodge)
                        {
                            Player2.DamageCounter += 5;
                            Player2.hitcount++;
                            Player2.knockback = -(float)3;
                            Player2.attackDelay = TimeSpan.FromMilliseconds(hitstun);
                            Player2.delaying = true;
                        }


                    }

                }


            }

            else if (Player1.currentAnimation == AnimationType.NeutralSpecial2 && Player2.hitcount == 0 && Player1.delaying == false)
            {

                if (Player2.hitcount == 0)
                {
                    Player2.AttackLength = new TimeSpan();
                    Player2.attacknumber = 2;
                    if (Player2.BoundingBox.Intersects(Player1.dhitbox))
                    {
                        Player2.knockeddiagonal = false;
                        Player1.OmniDirectionalAttack = false;
                        Player2.knockedup = true;
                        if (Player2.currentAnimation != AnimationType.Shield && Player2.currentAnimation != AnimationType.Dodge && Player2.currentAnimation != AnimationType.SpotDodge)
                        {
                            Player2.DamageCounter += 5;
                            Player2.hitcount++;
                            Player2.knockback = -(float)3;
                            Player2.attackDelay = TimeSpan.FromMilliseconds(hitstun);
                            Player2.delaying = true;
                        }
                    }


                }


            }

            else if (Player1.currentAnimation == AnimationType.NeutralSpecial3 && Player2.hitcount == 0 && Player1.delaying == false)
            {

                if (Player2.hitcount == 0)
                {
                    Player2.AttackLength = new TimeSpan();
                    Player2.attacknumber = 3;
                    if (Player2.BoundingBox.Intersects(Player1.dhitbox))
                    {
                        Player2.knockeddiagonal = false;
                        Player1.OmniDirectionalAttack = false;
                        Player2.knockedup = false;
                        if (Player2.currentAnimation != AnimationType.Shield && Player2.currentAnimation != AnimationType.Dodge && Player2.currentAnimation != AnimationType.SpotDodge)
                        {


                            Player2.DamageCounter += 10;
                            Player2.hitcount++;
                            Player2.knockback = Player2.DamageCounter / 5;
                            Player2.attackDelay = TimeSpan.FromMilliseconds(hitstun);
                            Player2.delaying = true;
                        }
                    }
                }


            }

            #endregion
            #region SideSmash
            else if (Player1.currentAnimation == AnimationType.SideSmash && Player2.hitcount == 0 && Player1.delaying == false)
            {

                if (Player2.hitcount == 0)
                {
                    Player2.AttackLength = new TimeSpan();
                    Player2.attacknumber = 4;
                    if (Player2.BoundingBox.Intersects((Player1.dhitbox)))
                    {
                        Player1.OmniDirectionalAttack = false;
                        Player2.knockeddiagonal = true;
                        Player2.knockedup = false;
                        if (Player2.currentAnimation != AnimationType.Shield && Player2.currentAnimation != AnimationType.Dodge && Player2.currentAnimation != AnimationType.SpotDodge)
                        {
                            Player2.DamageCounter += 22;
                            Player2.hitcount++;
                            Player2.knockback = Player2.DamageCounter / 5;
                            Player2.knockbackX = Player2.DamageCounter / 5;
                            Player2.knockbackY = -1 * Player2.DamageCounter / 8;
                        }
                    }
                }

            }
            #endregion
            #region DownSmash
            else if (Player1.currentAnimation == AnimationType.DownSmash && Player2.hitcount == 0 && Player1.delaying == false)
            {
                if (Player2.hitcount == 0)
                {
                    Player2.AttackLength = new TimeSpan();
                    Player2.attacknumber = 5;
                    Player2.knockedup = false;
                    Player1.multihitcounter++;
                    if (Player1.multihitcounter <= 4)
                    {
                        if (Player2.BoundingBox.Intersects(Player1.dhitbox))
                        {
                            Player2.knockeddiagonal = false;
                            Player1.OmniDirectionalAttack = true;
                            if (Player2.currentAnimation != AnimationType.Shield && Player2.currentAnimation != AnimationType.Dodge && Player2.currentAnimation != AnimationType.SpotDodge)
                            {
                                Player2.hitcount++;
                                Player2.DamageCounter += 4;
                                Player2.knockback = -(float)2;
                            }
                        }
                    }
                    else
                    {

                        //Player1.mulithitcounter++;
                        if (Player2.BoundingBox.Intersects(Player1.dhitbox))
                        {
                            Player2.knockedup = false;
                            if (Player2.currentAnimation != AnimationType.Shield && Player2.currentAnimation != AnimationType.Dodge && Player2.currentAnimation != AnimationType.SpotDodge)
                            {
                                Player2.hitcount++;
                                Player2.DamageCounter += 13;
                                Player2.knockback = Player2.DamageCounter / 5;
                            }

                        }
                    }
                }
            }
            #endregion
            #region UpSmash
            else if (Player1.currentAnimation == AnimationType.UpSmash && Player2.hitcount == 0 && Player1.delaying == false)
            {

                if (Player2.hitcount == 0)
                {
                    Player2.AttackLength = new TimeSpan();
                    Player2.attacknumber = 6;
                    if (Player2.BoundingBox.Intersects((Player1.dhitbox)))
                    {
                        Player2.knockeddiagonal = false;
                        Player1.OmniDirectionalAttack = false;
                        Player2.knockedup = true;
                        if (Player2.currentAnimation != AnimationType.Shield && Player2.currentAnimation != AnimationType.Dodge && Player2.currentAnimation != AnimationType.SpotDodge)
                        {
                            Player2.DamageCounter += 16;
                            Player2.Airtime = new TimeSpan();
                            Player2.hitcount++;
                            Player2.knockback = -(Player2.DamageCounter / 6
                                );
                        }
                    }
                }
            }
            #endregion
            #region SideSpecial
            else if (Player1.currentAnimation == AnimationType.SideSpecial && Player2.hitcount == 0 && Player1.delaying == false)
            {

                if (Player2.hitcount == 0)
                {
                    Player2.AttackLength = new TimeSpan();
                    Player2.attacknumber = 7;
                    if (Player2.BoundingBox.Intersects((Player1.dhitbox)))
                    {
                        Player1.OmniDirectionalAttack = false;
                        Player2.knockedup = false;
                        Player2.knockeddiagonal = true;
                        if (Player2.currentAnimation != AnimationType.Shield && Player2.currentAnimation != AnimationType.Dodge && Player2.currentAnimation != AnimationType.SpotDodge)
                        {
                            Player2.DamageCounter += 2;
                            Player2.hitcount++;
                            Player2.Airtime = TimeSpan.FromMilliseconds(470);
                            Player2.gravity = 0f;
                            Player2.knockbackY = -(float)0;
                            Player2.knockbackX = (float)9;
                        }

                    }
                }
            }
            #endregion
            #region DownSpecial
            else if (Player1.currentAnimation == AnimationType.DownSpecial && Player2.hitcount == 0 && Player1.delaying == false)
            {

                if (Player2.hitcount == 0)
                {
                    Player2.AttackLength = new TimeSpan();
                    Player2.attacknumber = 8;
                    Player1.multihitcounter++;
                    if (Player1.multihitcounter <= 9)
                    {
                        if (Player2.BoundingBox.Intersects((Player1.dhitbox)))
                        {
                            Player1.OmniDirectionalAttack = false;
                            Player2.knockeddiagonal = true;
                            Player2.knockedup = false;
                            if (Player2.currentAnimation != AnimationType.Shield && Player2.currentAnimation != AnimationType.Dodge && Player2.currentAnimation != AnimationType.SpotDodge)
                            {
                                Player2.DamageCounter += 6;
                                Player2.hitcount++;
                                Player2.knockbackX = (float)4;
                                Player2.knockbackY = (float)-5;
                            }
                        }
                    }
                    else
                    {
                        if (Player2.BoundingBox.Intersects((Player1.dhitbox)))
                        {
                            Player1.OmniDirectionalAttack = false;
                            Player2.knockeddiagonal = true;
                            Player2.knockedup = false;
                            if (Player2.currentAnimation != AnimationType.Shield && Player2.currentAnimation != AnimationType.Dodge && Player2.currentAnimation != AnimationType.SpotDodge)
                            {
                                Player2.hitcount++;
                                Player2.DamageCounter += 8;
                                Player2.knockbackX = Player2.DamageCounter / 9;
                                Player2.knockbackY = -(Player2.DamageCounter / 9);
                            }
                        }
                    }
                }

            }
            #endregion
            #region NeutralSmash
            else if (Player1.currentAnimation == AnimationType.NeutralSmash && Player2.hitcount == 0 && Player1.delaying == false)
            {
                if (Player2.hitcount == 0)
                {
                    Player2.AttackLength = new TimeSpan();
                    Player2.attacknumber = 9;
                    if (Player2.BoundingBox.Intersects((Player1.dhitbox)))
                    {
                        Player1.OmniDirectionalAttack = false;
                        Player2.knockeddiagonal = false;
                        Player2.knockedup = false;
                        if (Player2.currentAnimation != AnimationType.Shield && Player2.currentAnimation != AnimationType.Dodge && Player2.currentAnimation != AnimationType.SpotDodge)
                        {
                            Player2.DamageCounter += 10;
                            Player2.hitcount++;
                            Player2.knockback = Player2.DamageCounter / 10;
                        }
                    }
                }

            }

            #endregion
            #region UpSpecial/Reco
            else if (Player1.currentAnimation == AnimationType.UpSpecial && Player2.hitcount == 0 && Player1.delaying == false)
            {

                if (Player2.hitcount == 0)
                {
                    Player2.AttackLength = new TimeSpan();
                    Player2.attacknumber = 10;
                    if (Player2.BoundingBox.Intersects((Player1.dhitbox)))
                    {
                        Player1.OmniDirectionalAttack = false;
                        Player2.knockeddiagonal = true;
                        Player2.knockedup = false;
                        if (Player2.currentAnimation != AnimationType.Shield && Player2.currentAnimation != AnimationType.Dodge && Player2.currentAnimation != AnimationType.SpotDodge)
                        {
                            Player2.DamageCounter += 20;

                            Player2.hitcount++;
                            Player2.knockbackX = (Player2.DamageCounter / 7);
                            Player2.knockbackY = -(Player2.DamageCounter / 6);
                        }
                    }
                }

            }
            #endregion
            #region UpAerial
            else if (Player1.currentAnimation == AnimationType.UpAerial && Player2.hitcount == 0 && Player1.delaying == false)
            {

                if (Player2.hitcount == 0)
                {
                    Player2.AttackLength = new TimeSpan();
                    Player2.attacknumber = 11;
                    if (Player2.BoundingBox.Intersects((Player1.dhitbox)))
                    {
                        Player1.OmniDirectionalAttack = false;
                        Player2.knockeddiagonal = false;
                        Player2.knockedup = true;
                        if (Player2.currentAnimation != AnimationType.Shield && Player2.currentAnimation != AnimationType.Dodge && Player2.currentAnimation != AnimationType.SpotDodge)
                        {
                            Player2.DamageCounter += 13;
                            Player2.Airtime = new TimeSpan();
                            Player2.hitcount++;
                            Player2.knockback = -(Player2.DamageCounter / (13 / 2));
                        }
                    }
                }

            }
            #endregion
            #region SideAerial
            else if (Player1.currentAnimation == AnimationType.SideAerial && Player2.hitcount == 0 && Player1.delaying == false)
            {

                if (Player2.hitcount == 0)
                {
                    Player2.AttackLength = new TimeSpan();
                    Player2.attacknumber = 12;
                    if (Player2.BoundingBox.Intersects((Player1.dhitbox)))
                    {
                        Player1.OmniDirectionalAttack = false;
                        Player2.knockeddiagonal = false;
                        Player2.knockedup = false;
                        if (Player2.currentAnimation != AnimationType.Shield && Player2.currentAnimation != AnimationType.Dodge && Player2.currentAnimation != AnimationType.SpotDodge)
                        {
                            Player2.DamageCounter += 16;

                            Player2.hitcount++;
                            Player2.knockback = (Player2.DamageCounter / 6);
                        }
                    }
                }

            }
            #endregion
            #region DownAerial
            else if (Player1.currentAnimation == AnimationType.DownAerial && Player2.hitcount == 0 && Player1.delaying == false)
            {
                if (Player2.hitcount == 0)
                {
                    Player2.AttackLength = new TimeSpan();
                    Player2.attacknumber = 13;
                    if (Player2.BoundingBox.Intersects((Player1.dhitbox)))
                    {
                        {
                            Player1.OmniDirectionalAttack = false;
                            Player2.knockeddiagonal = false;
                            Player2.knockedup = true;
                            if (Player2.currentAnimation != AnimationType.Shield && Player2.currentAnimation != AnimationType.Dodge && Player2.currentAnimation != AnimationType.SpotDodge)
                            {
                                Player2.DamageCounter += 18;

                                Player2.hitcount++;
                                Player2.knockback = (Player2.DamageCounter / 5);
                            }
                        }
                    }

                }
            }
            #endregion
            #region NeutralAerial
            else if (Player1.currentAnimation == AnimationType.NeutralAerial && Player2.hitcount == 0 && Player1.delaying == false)
            {

                if (Player2.hitcount == 0)
                {
                    Player2.AttackLength = new TimeSpan();
                    Player2.attacknumber = 14;
                    if (Player2.BoundingBox.Intersects((Player1.dhitbox)))
                    {
                        Player1.OmniDirectionalAttack = false;
                        Player2.knockeddiagonal = false;
                        Player2.knockedup = false;
                        if (Player2.currentAnimation != AnimationType.Shield && Player2.currentAnimation != AnimationType.Dodge && Player2.currentAnimation != AnimationType.SpotDodge)
                        {
                            Player2.DamageCounter += 11;

                            Player2.hitcount++;
                            Player2.knockback = (Player2.DamageCounter / 8);
                        }
                    }
                }

            }
            #endregion

            if (Player2.BoundingBox.Intersects(Player1.dhitbox)&& Player1.stunned == false)
            {
                if (Player2.currentAnimation != AnimationType.Shield && Player2.currentAnimation != AnimationType.Dodge && Player2.currentAnimation != AnimationType.SpotDodge)
                {
                    if (Player1.Effects == SpriteEffects.FlipHorizontally)
                    {
                        Player2.knockedleft = true;
                        Player2.knockedright = false;
                    }
                    else
                    {
                        Player2.knockedright = true;
                        Player2.knockedleft = false;
                    }
                    Character.Stunned(Player2);
                }
            }
            #region AttackSelectionTimer
            if (Player2.attacknumber == 1)
            {
                Player2.AttackLength += gameTime.ElapsedGameTime;
                if (Player2.AttackLength >= TimeSpan.FromMilliseconds(240))
                {
                    Player2.hitcount = 0;
                    Player2.AttackLength = new TimeSpan();
                }
            }
            else if (Player2.attacknumber == 2)
            {
                Player2.AttackLength += gameTime.ElapsedGameTime;
                if (Player2.AttackLength >= TimeSpan.FromMilliseconds(240))
                {
                    Player2.hitcount = 0;
                    Player2.AttackLength = new TimeSpan();
                }
            }
            else if (Player2.attacknumber == 3)
            {
                Player2.AttackLength += gameTime.ElapsedGameTime;
                if (Player2.AttackLength >= TimeSpan.FromMilliseconds(480))
                {
                    Player2.hitcount = 0;
                    Player2.AttackLength = new TimeSpan();
                }
            }
            else if (Player2.attacknumber == 4)
            {
                Player2.AttackLength += gameTime.ElapsedGameTime;
                if (Player2.AttackLength >= TimeSpan.FromMilliseconds(504))
                {
                    Player2.hitcount = 0;
                    Player2.AttackLength = new TimeSpan();
                }
            }
            else if (Player2.attacknumber == 5)
            {
                Player2.AttackLength += gameTime.ElapsedGameTime;
                if (Player1.multihitcounter < 5)
                {
                    if (Player2.AttackLength >= TimeSpan.FromMilliseconds(50))
                    {
                        Player2.hitcount = 0;
                        Player2.AttackLength = new TimeSpan();
                    }
                }
                else
                {
                    if (Player2.AttackLength >= TimeSpan.FromMilliseconds(280))
                    {
                        Player2.hitcount = 0;
                        Player2.AttackLength = new TimeSpan();
                        Player1.multihitcounter = 0;


                    }
                }
            }
            else if (Player2.attacknumber == 6)
            {

                Player2.AttackLength += gameTime.ElapsedGameTime;
                if (Player2.AttackLength >= TimeSpan.FromMilliseconds(450))
                {
                    Player2.hitcount = 0;
                    Player2.AttackLength = new TimeSpan();
                }
            }
            else if (Player2.attacknumber == 7)
            {
                Player2.AttackLength += gameTime.ElapsedGameTime;
                if (Player2.AttackLength >= TimeSpan.FromMilliseconds(65))
                {
                    Player2.hitcount = 0;
                    Player2.AttackLength = new TimeSpan();
                }
            }
            else if (Player2.attacknumber == 8)
            {
                Player2.AttackLength += gameTime.ElapsedGameTime;
                if (Player1.multihitcounter < 10)
                {
                    if (Player2.AttackLength >= TimeSpan.FromMilliseconds(60))
                    {
                        Player2.hitcount = 0;
                        Player2.AttackLength = new TimeSpan();
                    }
                }
                else
                {
                    if (Player2.AttackLength >= TimeSpan.FromMilliseconds(280))
                    {
                        Player2.hitcount = 0;
                        Player2.AttackLength = new TimeSpan();
                        Player1.multihitcounter = 0;


                    }
                }
            }
            else if (Player2.attacknumber == 9)
            {
                Player2.AttackLength += gameTime.ElapsedGameTime;
                if (Player2.AttackLength >= TimeSpan.FromMilliseconds(370))
                {
                    Player2.hitcount = 0;
                    Player2.AttackLength = new TimeSpan();
                }
            }
            else if (Player2.attacknumber == 10)
            {
                Player2.AttackLength += gameTime.ElapsedGameTime;
                if (Player2.AttackLength >= TimeSpan.FromMilliseconds(370))
                {
                    Player2.hitcount = 0;
                    Player2.AttackLength = new TimeSpan();
                }
            }
            else if (Player2.attacknumber == 11)
            {
                Player2.AttackLength += gameTime.ElapsedGameTime;
                if (Player2.AttackLength >= TimeSpan.FromMilliseconds(350))
                {
                    Player2.hitcount = 0;
                    Player2.AttackLength = new TimeSpan();
                }
            }
            else if (Player2.attacknumber == 12)
            {
                Player2.AttackLength += gameTime.ElapsedGameTime;
                if (Player2.AttackLength >= TimeSpan.FromMilliseconds(425))
                {
                    Player2.hitcount = 0;
                    Player2.AttackLength = new TimeSpan();
                }
            }
            else if (Player2.attacknumber == 13)
            {
                Player2.AttackLength += gameTime.ElapsedGameTime;
                if (Player2.AttackLength >= TimeSpan.FromMilliseconds(540))
                {
                    Player2.hitcount = 0;
                    Player2.AttackLength = new TimeSpan();
                }
            }
            else if (Player2.attacknumber == 14)
            {
                Player2.AttackLength += gameTime.ElapsedGameTime;
                if (Player2.AttackLength >= TimeSpan.FromMilliseconds(280))
                {
                    Player2.hitcount = 0;
                    Player2.AttackLength = new TimeSpan();
                }
            }


            #endregion
            //Knockback Displacement calculation is here
            #region KnockbackCalculator
            #region DiagonalKnockback
            if (Player2.knockeddiagonal == true)
            {
                if (Player2.knockedleft)
                {
                    if (Player2.knockbackX > 0.1f)
                    {
                        Player2.X += -1 * Player2.knockbackX;
                        Player2.knockbackX--;
                        Math.Floor(Player2.knockbackX);
                        if (Player2.knockbackY > 0f)
                        {
                            Player2.Y += Player2.knockbackY / 2;
                            //Player2.knockbackY++;
                            Math.Floor(Player2.knockbackY);
                        }
                        else if (Player2.knockbackY < 0f)
                        {
                            Player2.Y += Player2.knockbackY / 2;
                            //Player2.knockbackY--;
                            Math.Floor(Player2.knockbackY);
                        }
                    }

                    else if (Player2.knockbackX < -0.1f)
                    {
                        Player2.X += -1 * Player2.knockbackX;
                        Player2.knockbackX++;
                        Math.Floor(Player2.knockbackY);

                        if (Player2.knockbackY > 0f)
                        {
                            Player2.Y += Player2.knockbackY / 2;
                            //Player2.knockbackY++;
                            Math.Floor(Player2.knockbackY);
                        }
                        else if (Player2.knockbackY < 0f)
                        {
                            Player2.Y += Player2.knockbackY / 2;
                            //Player2.knockbackY--;
                            Math.Floor(Player2.knockbackY);
                        }

                    }
                }

                else if (Player2.knockedright)
                {

                    if (Player2.knockbackX > 0.1f)
                    {
                        Player2.X += 1 * Player2.knockbackX;
                        Player2.knockbackX--;
                        Math.Floor(Player2.knockbackX);
                        if (Player2.knockbackY > 0f)
                        {
                            Player2.Y += Player2.knockbackY / 2;
                            //Player2.knockbackY++;
                            Math.Floor(Player2.knockbackY);
                        }
                        else if (Player2.knockbackY < -0f)
                        {
                            Player2.Y += Player2.knockbackY / 2;
                            //Player2.knockbackY--;
                            Math.Floor(Player2.knockbackY);
                        }
                    }
                    else if (Player2.knockbackX < -0.1f)
                    {
                        Player2.X += 1 * Player2.knockbackX;
                        Player2.knockbackX++;
                        Math.Floor(Player2.knockbackY);

                        if (Player2.knockbackY > 0f)
                        {
                            Player2.Y += Player2.knockbackY / 2;
                            //Player2.knockbackY++;
                            Math.Floor(Player2.knockbackY);
                        }
                        else if (Player2.knockbackY < 0f)
                        {
                            Player2.Y += Player2.knockbackY / 2;
                            //Player2.knockbackY--;
                            Math.Floor(Player2.knockbackY);
                        }



                    }

                }
            }
            #endregion
            #region OmniDirectional
            if (Player1.OmniDirectionalAttack == true)
            {
                if (Player2.knockedup == false)
                {
                    if (Player2.X <= Player1.X)
                    {


                        if (Player2.knockback > .01f)
                        {
                            Player2.X += -1 * Player2.knockback;
                            Player2.knockback--;
                            Math.Floor(Player2.knockback);
                        }
                        else if (Player2.knockback < -.01f)
                        {
                            Player2.X += 1 * Player2.knockback;
                            Player2.knockback++;
                            Math.Floor(Player2.knockback);
                        }
                        else { Player2.knockback = 0f; }
                    }
                    else if (Player2.X >= Player1.X)
                    {

                        if (Player2.knockback > .01f)
                        {
                            Player2.X += 1 * Player2.knockback;
                            Player2.knockback--;
                            Math.Floor(Player2.knockback);
                        }
                        else if (Player2.knockback < -.01f)
                        {
                            Player2.X += -1 * Player2.knockback;
                            Player2.knockback++;
                            Math.Floor(Player2.knockback);
                        }
                        else { Player2.knockback = 0f; }

                    }
                }
                else if (Player2.knockedup == true)
                {
                    if (Player2.knockback > .05f)
                    {
                        Player2.Y += Player2.knockback;
                        Player2.knockback--;
                    }
                    else if (Player2.knockback < -.05f)
                    {
                        Player2.Y += Player2.knockback;
                        Player2.knockback++;
                    }
                }
            }
            #endregion
            #region NormalKnockback
            else if (Player2.knockback < .01f || Player2.knockback > -0.1f)
            {
                if (Player2.knockedup == false)
                {
                    if (Player2.knockedleft)
                    {
                        if (Player2.knockback > .01f)
                        {
                            Player2.X += -1 * Player2.knockback;
                            Player2.knockback--;
                            Math.Floor(Player2.knockback);
                        }
                        else if (Player2.knockback < -.01f)
                        {
                            Player2.X += -1 * Player2.knockback;
                            Player2.knockback++;
                            Math.Floor(Player2.knockback);
                        }
                        else { Player2.knockback = 0f; }
                    }
                    else if (Player2.knockedright)
                    {
                        if (Player2.knockback > .01f)
                        {
                            Player2.X += 1 * Player2.knockback;
                            Player2.knockback--;
                            Math.Floor(Player2.knockback);
                        }
                        else if (Player2.knockback < -.01f)
                        {
                            Player2.X += 1 * Player2.knockback;
                            Player2.knockback++;
                            Math.Floor(Player2.knockback);
                        }
                        else { Player2.knockback = 0f; }
                    }
                }
                else if (Player2.knockedup == true)
                {
                    if (Player2.knockback > .05f)
                    {
                        Player2.Y += Player2.knockback;
                        Player2.knockback--;
                        Player2.feethitbox.Y += 1;
                        if (Player2.feethitbox.Intersects(Platform.BoundingBox))
                        {
                            Player2.Y -= Player2.knockback;
                            Player2.knockback = -1 * (Player2.knockback);
                            Player2.Y--;
                            Math.Floor(Player2.knockback);
                        }
                    }
                    else if (Player2.knockback < -.05f)
                    {
                        Player2.Y += Player2.knockback;
                        Player2.knockback++;
                        if (Player2.feethitbox.Intersects(Platform.BoundingBox))
                        {
                            //Player2.Y  -= Player2.knockback;


                        }
                    }
                }
            }

                    #endregion
                    #endregion

                    base.Update(gameTime);
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

            lastKeyboardState = keyboardState;
            LivesPlayer1 = Player1.Lives.ToString();
            LivesPlayer2 = Player2.Lives.ToString();
            HitCounter = Player2.multihitcounter.ToString();
            Damage1 = Player1.DamageCounter.ToString();
            Damage2 = Player2.DamageCounter.ToString();
        }

        






      
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);


            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            if (GameActive)
            {
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

                Player1.Draw(spriteBatch);
                Player2.Draw(spriteBatch);
                Player1.DrawDamageHitbox(spriteBatch, GraphicsDevice,  new Color(0, 0, 0, 0));
                Player2.DrawDamageHitbox(spriteBatch, GraphicsDevice, new Color(0, 0, 0, 0));
                Player1.DrawHitbox(spriteBatch, GraphicsDevice, new Color(0, 0, 0, 0));
                Player2.DrawHitbox(spriteBatch, GraphicsDevice, new Color(0, 0, 0, 0));
            }

            spriteBatch.DrawString(spriteFont, LivesPlayer1, new Vector2(275, 500), Color.White);
            spriteBatch.DrawString(spriteFont, LivesPlayer2, new Vector2(975, 500), Color.White);
            spriteBatch.DrawString(spriteFont, LInd1, new Vector2(200, 500), Color.Red);
            spriteBatch.DrawString(spriteFont, LInd2, new Vector2(900, 500), Color.Blue);
            spriteBatch.DrawString(spriteFont, Damage1, new Vector2(125, 500), Color.White);
            spriteBatch.DrawString(spriteFont, Damage2, new Vector2(1050, 500), Color.White);
            spriteBatch.DrawString(spriteFont, finalmsg, new Vector2(475, 300), Color.White);
            //spriteBatch.DrawString(spriteFont, HitCounter, new Vector2(600, 50), Color.White);

            //spriteBatch.Draw(pixel, new Rectangle((int)(Player1.X - Player1.Origin.X), (int)(Player1.Y - Player1.Origin.Y), pixelData.GetLength(0), pixelData.GetLength(1)), Color.White);

            //spriteBatch.Draw(pixel, MiniPlatform1.BoundingBox, Color.Red);

            //spriteBatch.DrawString(Content.Load<SpriteFont>("font"), guy.currentAnimation.ToString(), Vector2.Zero, Color.Black);

            spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
