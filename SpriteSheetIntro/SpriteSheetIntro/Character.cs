using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace SpriteSheetIntro
{
    public enum AnimationType
    {
        SideAerial,
        DownAerial,
        UpAerial,
        NeutralAerial,
        SideSmash,
        DownSmash,
        UpSmash,
        NeutralSmash,
        UpSpecial,
        SideSpecial,
        DownSpecial,
        DownSpecialProjectile,
        NeutralSpecial,
        NeutralSpecial2,
        NeutralSpecial3,
        Disabled,
        ShieldStun,
        SpotDodge,
        Dodge,
        Shield,
        Idle,
        Running,
        Walking,
        Stunned
    }

    public enum ActionType
    {
       
        DownSmash=0,
        UpSmash,
        SideSmash,        
        NeutralSmash,
        DownAerial,
        SideAerial,
        UpAerial,
        NeutralAerial,
        UpSpecial,
        SideSpecial,
        DownSpecial,
        DownSpecialProjectile,
        NeutralSpecial,
        AirDodge,
        Dodge,
        Shield,
        ShieldStun,

        //Disabled,
        FastFall,
        Jump,
        Idle,
        Running,
        Walking,
        Stunned
    }

    public enum State
    {
        Ground,
        Air
    }

    public class Character : Sprite
    {

        public Color[,] pixelData = new Color[40, 40];
        public int PlayerID { get; private set; }

        public State state = State.Air;
        private Dictionary<State, List<Action>> actions;

        public AnimationType currentAnimation = AnimationType.Idle;
        Dictionary<AnimationType, Animation> animations;

        List<Projectile> projectiles = new List<Projectile>();

        TimeSpan sidespecialtimer = TimeSpan.Zero;
        TimeSpan downspecialtimer = TimeSpan.Zero;
        TimeSpan elapsedAttackDelay = TimeSpan.Zero;
        public TimeSpan attackDelay = TimeSpan.FromMilliseconds(250);
        TimeSpan StunDelay = TimeSpan.FromMilliseconds(0);
        TimeSpan elapsedStunDelay = TimeSpan.Zero;

        public bool delaying = false;
        public bool stunned = false;
        bool insidespecial = false;
        bool indownspecial = false;

        Vector2 attackVelocity;
        Vector2 attackDragSeconds;

        public Rectangle NeutralSpecialHBox;
        public bool HitStun = false;
        
        public Keys KeyUp = Keys.W;
        public Keys KeyJump = Keys.Space;
        public Keys KeyDown = Keys.S;
        public Keys KeyRight = Keys.D;
        public Keys KeyLeft = Keys.A;
        public Keys KeySpecial = Keys.R;
        public Keys KeySmash = Keys.T;
        public Keys KeyShield = Keys.Y;
        public int hitcount = 0;
        public int attacknumber = 0;
        public TimeSpan AttackLength = new TimeSpan();
        public Rectangle feethitbox;
        public Rectangle floorhitbox;
        public bool hitbyatk = false;
        bool shieldbroken = false;
        bool dodging = false;
        bool jumping = false;
        public bool knockedleft = false;
        public bool knockedright = false;
        bool airCancel = false;
        bool falling = false;
        bool inupspecial = false;
        bool NS = false;
        bool ResetNSTimer = false;
        bool inmovement = false;
        bool walkingleft = false;
        bool walkingright = false;
        public bool knockedup = false;
        public bool knockeddiagonal = false;
        public float knockbackX = 0f;
        public float knockbackY = 0f;
        public float knockback = 0f;
        public int DamageCounter = 0;
        public int Lives = 3;       
        int maxJumps = 2;
        int jumpsLeft;
        int NScounter = 0;
        public int multihitcounter = 0;
        public bool OmniDirectionalAttack = false;
        public TimeSpan Airtime = new TimeSpan();
        public TimeSpan UpSpecAttack = new TimeSpan();
        public TimeSpan Shieldtime = new TimeSpan();
        public TimeSpan AfterShieldTime = new TimeSpan();
        public TimeSpan DodgeTimer = new TimeSpan();
        public TimeSpan NSActiontimer = new TimeSpan();
        public TimeSpan MovementDragTimer = new TimeSpan();
        private string _name;
        private int _numberOfUpdates = 0;
        public int NumberOfUpdates
        {
            get
            {
                return _numberOfUpdates;
            }
            set
            {
                _numberOfUpdates = 0;
            }
        }

        Action lastAction;
        public float gravity = 9.81f;

        public Character(string name, Vector2 location, Texture2D image, Color color, Dictionary<AnimationType, Animation> animations, int playerID)
            : base(location, image, color)
        {
            PlayerID = playerID;

            _name = name;
            //# of junmps assignment
            jumpsLeft = maxJumps;

            //Jump height
            _jump = new Vector2(6);

            //Assign animations
            this.animations = animations;
            foreach (AnimationType animationType in animations.Keys)
            {
                animations[animationType].PlayerID = playerID;
                animations[animationType].AnimationFinished += AnimationFinished;
            }

            //Finds correct set of frames
            SourceRectangle = animations[currentAnimation].CurrentFrame.SourceRectangle;
            Origin = animations[currentAnimation].CurrentFrame.Origin;
            DamageRectangle = animations[currentAnimation].CurrentFrame.DamageRectangle;

            //Assign hardcoded values
            _boundingBox.Width = 16;//_boundingBox.Width / 6 - 24;
            _boundingBox.Height = 21;//_boundingBox.Height / 14 - 19;
            feethitbox = new Rectangle((int)_position.X - 3, (int)_position.Y - 1, 7, 1);
            floorhitbox = new Rectangle((int)_position.X - 3, (int)_position.Y, 7, 1);
        }
        public void createAction()
        {
            //Add States
            actions = new Dictionary<State, List<Action>>();
            actions.Add(State.Ground, new List<Action>());
            actions.Add(State.Air, new List<Action>());

            //Possible basic moves only in the air state
            actions[State.Air].Add(new Action(AirDodge, TimeSpan.MaxValue, new KeyCombo(ComboType.Pressed, KeyShield)));
            actions[State.Air].Add(new Action(UpAerial, TimeSpan.MaxValue, new KeyCombo(ComboType.Released, KeyUp, KeySmash), new KeyCombo(ComboType.Pressed, KeyUp, KeySmash)) { AllowFastFall = true });
            actions[State.Air].Add(new Action(SideAerial, TimeSpan.MaxValue, new KeyCombo(ComboType.Released, KeySmash), new KeyCombo(ComboType.Pressed, KeyLeft, KeySmash)) { AllowFastFall = true });
            actions[State.Air].Add(new Action(SideAerial, TimeSpan.MaxValue, new KeyCombo(ComboType.Released, KeySmash), new KeyCombo(ComboType.Pressed, KeyRight, KeySmash)) { AllowFastFall = true });
            actions[State.Air].Add(new Action(NeutralAerial, TimeSpan.MaxValue, new KeyCombo(ComboType.Released, KeySmash), new KeyCombo(ComboType.Pressed, KeySmash)) { AllowFastFall = true });
            actions[State.Air].Add(new Action(DownAerial, TimeSpan.MaxValue, new KeyCombo(ComboType.Released, KeySmash), new KeyCombo(ComboType.Pressed, KeyDown, KeySmash))/* { AllowFastFall = true }*/);
            actions[State.Air].Add(new Action(
                           new PreAction(PreFastFall) { AllowFastFall = true },
                           null, TimeSpan.MaxValue, new KeyCombo(ComboType.Released, KeyDown), new KeyCombo(ComboType.Pressed, KeyDown)));

            //Possible basic movies only in the ground state
            actions[State.Ground].Add(new Action(Dodge, TimeSpan.MaxValue, new KeyCombo(ComboType.Pressed, KeyShield, KeyLeft)));
            actions[State.Ground].Add(new Action(Dodge, TimeSpan.MaxValue, new KeyCombo(ComboType.Pressed, KeyShield, KeyRight)));
            actions[State.Ground].Add(new Action(UpSmash, TimeSpan.MaxValue, new KeyCombo(ComboType.Pressed, KeyUp, KeySmash)));
            actions[State.Ground].Add(new Action(
               new PreAction(PreShield) { AllowCancel = false },
               Shield, TimeSpan.MaxValue, new KeyCombo(ComboType.Pressed, KeyShield))
            { AllowCancel = true, AllowFlip = true });
            actions[State.Ground].Add(new Action(SideSmash, TimeSpan.MaxValue, new KeyCombo(ComboType.Pressed, KeyLeft, KeySmash)));
            actions[State.Ground].Add(new Action(SideSmash, TimeSpan.MaxValue, new KeyCombo(ComboType.Pressed, KeyRight, KeySmash)));
            actions[State.Ground].Add(new Action(DownSmash, TimeSpan.MaxValue, new KeyCombo(ComboType.Pressed, KeyDown, KeySmash)));
            actions[State.Ground].Add(new Action(NeutralSmash, TimeSpan.MaxValue, new KeyCombo(ComboType.Pressed, KeySmash)));

            //Special moves that can be done in the ground and the air
            Action neutralSpecialAction = new Action(NeutralSpecial, TimeSpan.MaxValue, new KeyCombo(ComboType.Released, KeySpecial), new KeyCombo(ComboType.Pressed, KeySpecial));

                Action stunnedAction = new Action(Stunned, TimeSpan.MaxValue, new KeyCombo(ComboType.Pressed)) { AllowCancel = false };
                Action stunnedAction2 = new Action(Stunned, TimeSpan.MaxValue, new KeyCombo(ComboType.Released)) { AllowCancel = false };
            

            actions[State.Air].Add(neutralSpecialAction);
            actions[State.Ground].Add(neutralSpecialAction);
            actions[State.Air].Add(stunnedAction);
            actions[State.Ground].Add(stunnedAction);
            actions[State.Air].Add(stunnedAction2);
            actions[State.Ground].Add(stunnedAction2);

            Action preWalkLeftAction = new Action(
                            new PreAction(PreWalking) { AllowMovement = true },
                            Walking, TimeSpan.MaxValue, new KeyCombo(ComboType.Pressed, KeyLeft))
            { AllowMovement = true, AllowJump = true, AllowFastFall = true, AllowCancel = true, AllowFlip = true };

            actions[State.Ground].Add(preWalkLeftAction);
            actions[State.Air].Add(preWalkLeftAction);


            Action preWalkRightAction = new Action(
                            new PreAction(PreWalking) { AllowMovement = true },
                            Walking, TimeSpan.MaxValue, new KeyCombo(ComboType.Pressed, KeyRight))
            { AllowMovement = true, AllowJump = true, AllowFastFall = true, AllowCancel = true, AllowFlip = true };

            actions[State.Ground].Add(preWalkRightAction);
            actions[State.Air].Add(preWalkRightAction);


            Action downSpecialAction = new Action(DownSpecial, TimeSpan.MaxValue, new KeyCombo(ComboType.Pressed, KeyDown, KeySpecial));
            actions[State.Ground].Add(downSpecialAction);
            actions[State.Air].Add(downSpecialAction);

            Action upSpecialAction = new Action(UpSpecial, ResetFall, TimeSpan.MaxValue, new KeyCombo(ComboType.Pressed, KeyUp, KeySpecial));
            actions[State.Ground].Add(upSpecialAction);
            actions[State.Air].Add(upSpecialAction);


            Action sideSpecialLeftAction = new Action(SideSpecial, TimeSpan.MaxValue, new KeyCombo(ComboType.Pressed, KeyLeft, KeySpecial)) { AllowFlip = true };

            actions[State.Ground].Add(sideSpecialLeftAction);
            actions[State.Air].Add(sideSpecialLeftAction);

            Action sideSpecialRightAction = new Action(SideSpecial, TimeSpan.MaxValue, new KeyCombo(ComboType.Pressed, KeyRight, KeySpecial)) { AllowFlip = true };

            actions[State.Ground].Add(sideSpecialRightAction);
            actions[State.Air].Add(sideSpecialRightAction);


            Action jumpAction = new Action(
                new PreAction(PreJump) { AllowJump = true },
                null, TimeSpan.MaxValue, new KeyCombo(ComboType.Released, KeyJump), new KeyCombo(ComboType.Pressed, KeyJump))
            { AllowMovement = true, AllowJump = true, AllowFastFall = true };

            actions[State.Ground].Add(jumpAction);
            actions[State.Air].Add(jumpAction);


            Action idleAction = new Action(Idle, TimeSpan.MaxValue, new KeyCombo(ComboType.Released, KeyLeft, KeyRight)) { AllowMovement = true, AllowJump = true, AllowFastFall = true, AllowCancel = true, AllowFlip = true };
            Action idle2Action = new Action(Idle, TimeSpan.MaxValue, new KeyCombo(ComboType.Pressed, KeyLeft, KeyRight)) { AllowMovement = true, AllowJump = true, AllowFastFall = true, AllowCancel = true, AllowFlip = true };
            actions[State.Ground].Add(idleAction);
            actions[State.Air].Add(idleAction);
            actions[State.Ground].Add(idle2Action);
            actions[State.Air].Add(idle2Action);
        }
        
        void Idle(Character character)
        {
            character.ChangeAnimation(AnimationType.Idle);
            character.attackDelay = TimeSpan.Zero;
        }
        static public void Stunned(Character character)
        {
            character.ChangeAnimation(AnimationType.Stunned);
            
            character.insidespecial = false;
            character.attackDelay = TimeSpan.FromMilliseconds(550);
            character.stunned = true;
        }
        void Shield(Character character)
        {
            //if (shieldbroken == true)
            //{
            //    character.ChangeAnimation(AnimationType.ShieldStun);
            //}
            if (falling == true || jumping == true)
            {
                character.ChangeAnimation(AnimationType.Dodge);
                dodging = true;
            }
            else
            {
                character.ChangeAnimation(AnimationType.Shield);
            }
            character.attackDelay = TimeSpan.FromMilliseconds(0);




        }
        void PreShield(Character character)
        {
            if (shieldbroken == true)
            {
                character.ChangeAnimation(AnimationType.ShieldStun);
            }

        }
        void Dodge(Character character)
        {
            character.ChangeAnimation(AnimationType.Dodge);
            character.attackVelocity = new Vector2(12f * (_effects == SpriteEffects.None ? 1 : -1), 0f);
            character.attackDragSeconds = new Vector2(10f, 0f);
            character.attackDelay = TimeSpan.FromMilliseconds(100);
            dodging = true;
        }
        void AirDodge(Character character)
        {
            character.ChangeAnimation(AnimationType.Dodge);

            character.attackDelay = TimeSpan.FromMilliseconds(200);
            dodging = true;
        }
        void PreWalking(Character character)
        {
            if (!stunned)
            { 
            if (walkingright == true)
            {
                character._position += Vector2.UnitX * 6 * (_effects == SpriteEffects.None ? 1 : -1);
            }
            else if (walkingleft == true)
            {
                character._position += Vector2.UnitX * 6 * (_effects == SpriteEffects.None ? 1 : -1);
            }
        }
        }

        void Walking(Character character)
        {
            character.ChangeAnimation(AnimationType.Walking);
            character.attackDelay = TimeSpan.Zero;


        }
        //void PreRunning(Character character)
        //{
        //    character._position += Vector2.UnitX * 7 * (_effects == SpriteEffects.None ? 1 : -1);
        //}

        //void Running(Character character)
        //{
        //    character.ChangeAnimation(AnimationType.Running);
        //    character.attackDelay = TimeSpan.Zero;
        //}

        void SideSmash(Character character)
        {
            character.ChangeAnimation(AnimationType.SideSmash);
            if (inmovement == true)
            {

            }
            character.attackDelay = TimeSpan.FromMilliseconds(250);
        }
        void SideAerial(Character character)
        {
            character.ChangeAnimation(AnimationType.SideAerial);
            if (inmovement == true)
            {
                character.attackVelocity = new Vector2(9f * (_effects == SpriteEffects.None ? 1 : -1), 0f);
                character.attackDragSeconds = new Vector2(10f, 0f);
            }
            character.attackDelay = TimeSpan.FromMilliseconds(100);

        }
        void SideSpecial(Character character)
        {
            character.ChangeAnimation(AnimationType.SideSpecial);
            insidespecial = true;
            jumping = false;
            falling = false;
            gravity = 0f;

            //rip harambe - he didn't deserve this

            character.attackDelay = TimeSpan.FromMilliseconds(300);
        }

        void DownSmash(Character character)
        {
            character.ChangeAnimation(AnimationType.DownSmash);
            character.attackDelay = TimeSpan.FromMilliseconds(250);
        }
        void DownAerial(Character character)
        {
            character.ChangeAnimation(AnimationType.DownAerial);
            if (inmovement == true)
            {

            }
            character.attackDelay = TimeSpan.FromMilliseconds(150);
        }

        void DownSpecial(Character character)
        {
            character.ChangeAnimation(AnimationType.DownSpecial);
            indownspecial = true;
            jumping = false;
            falling = false;
            gravity = 0f;
            character.projectiles.Add(new AnimatingProjectile(character.Position, character.Image, Vector2.Zero,
                new Animation(character.animations[AnimationType.DownSpecialProjectile].Frames, character.animations[AnimationType.DownSpecialProjectile].AnimationSpeed, AnimationType.DownSpecialProjectile) { PlayerID = character.PlayerID }));
            character.projectiles[projectiles.Count - 1].Speed *= (character._effects == SpriteEffects.None ? 1 : -1);
            character.projectiles[projectiles.Count - 1].Effects = character._effects;
            character.projectiles[projectiles.Count - 1].Position += Vector2.UnitX * (character._effects == SpriteEffects.None ? character._sourceRectangle.Width / 2 : -character._sourceRectangle.Width / 2);
            
            character.attackDelay = TimeSpan.FromMilliseconds(250);

        }
        public void FollowCharacter(Character character)
        {
            character.projectiles[projectiles.Count - 1].X = character.Position.X;
            character.projectiles[projectiles.Count - 1].Y = character.Position.Y;



        }

        void UpSmash(Character character)
        {
            character.ChangeAnimation(AnimationType.UpSmash);
            if (inmovement == true)
            {

            }
            character.attackDelay = TimeSpan.FromMilliseconds(150);
        }
        void UpAerial(Character character)
        {
            character.ChangeAnimation(AnimationType.UpAerial);
            if (inmovement == true)
            {
                character.attackVelocity = new Vector2(5f * (_effects == SpriteEffects.None ? 1 : -1), 0f);
                character.attackDragSeconds = new Vector2(10f, 0f);
            }
            character.attackDelay = TimeSpan.FromMilliseconds(100);
        }

        void UpSpecial(Character character)//,GameTime gameTime)
        {
            inupspecial = true;
            character.ChangeAnimation(AnimationType.UpSpecial);
            //int i = 0;
            //for (i = 0; i < 7; i++)
            //{
            //    UpSpecAttack += gameTime.ElapsedGameTime;
            //    if (UpSpecAttack >= TimeSpan.FromMilliseconds(100))
            //    {
            //        UpSpecAttack = new TimeSpan();
            //        character._position += Vector2.UnitY * 5;
            //        character._position += Vector2.UnitX * 4 * (_effects == SpriteEffects.None ? 1 : -1); ;

            //    }
            //        }
            character.attackVelocity = new Vector2(10f * (_effects == SpriteEffects.None ? 1 : -1), -15f);
            character.attackDragSeconds = new Vector2(10f, 3f);
            character.airCancel = true;
            character.jumpsLeft = 0;
            ResetFall(character);
        }

        void ResetFall(Character character)
        {
            character.Airtime = new TimeSpan();
            character.jumping = false;
            character.falling = true;
        }

        /*void Disabled(Character character)
        {
            character.ChangeAnimation(AnimationType.Disabled);
        }*/


        void NeutralSpecial(Character character)
        {
            if (NScounter == 0)
            {
                character.ChangeAnimation(AnimationType.NeutralSpecial);
                character.attackDelay = TimeSpan.FromMilliseconds(10);
                NScounter++;
            }
            else if (NScounter == 1)
            {
                character.ChangeAnimation(AnimationType.NeutralSpecial2);
                character.attackDelay = TimeSpan.FromMilliseconds(10);
                NScounter++;
            }
            else if (NScounter == 2)
            {
                character.ChangeAnimation(AnimationType.NeutralSpecial3);
                character.attackDelay = TimeSpan.FromMilliseconds(250);
                NScounter = 0;
                NS = false;
                NSActiontimer = new TimeSpan();
            }
            if (inmovement == true)
            {

            }
        }
        void NeutralAerial(Character character)
        {
            character.ChangeAnimation(AnimationType.NeutralAerial);
            if (inmovement == true)
            {
                character.attackVelocity = new Vector2(3f * (_effects == SpriteEffects.None ? 1 : -1), 0f);
                character.attackDragSeconds = new Vector2(10f, 0f);
            }
            character.attackDelay = TimeSpan.FromMilliseconds(50);
        }
        void NeutralSmash(Character character)
        {
            character.ChangeAnimation(AnimationType.NeutralSmash);
            if (inmovement == true)
            {

            }
            character.attackDelay = TimeSpan.FromMilliseconds(300);
        }

        void PreJump(Character character)
        {
            if (stunned == false)
            { 
            if (jumpsLeft > 0)
            {
                character.Airtime = new TimeSpan();
                character.jumping = true;
                character.falling = false;
                character.jumpsLeft--;
                character.attackDelay = TimeSpan.Zero;
            }
            }
        }

        void PreFastFall(Character character)
        {
            if (character.jumping || character.falling)
            {
                character.Airtime += TimeSpan.FromMilliseconds(200);
                character.falling = true;
                character.jumping = false;
            }
        }


        void AnimationFinished(object sender, AnimationEventArgs e)
        {
            delaying = true;
            gravity = 9.8f;
            
        }

        void ChangeAnimation(AnimationType animationType)
        {
            if (currentAnimation != animationType)
            {
                animations[currentAnimation].Reset();
                currentAnimation = animationType;
            }
        }

        public void Update(GameTime gameTime, KeyboardState ks, KeyboardState lks)
        {

            _numberOfUpdates++;
            if (currentAnimation == AnimationType.Walking || currentAnimation == AnimationType.Running)
            {
                inmovement = true;
                MovementDragTimer = new TimeSpan();
            }
            else
            {
                if (inmovement == true)
                {
                    MovementDragTimer += gameTime.ElapsedGameTime;
                    if (MovementDragTimer >= TimeSpan.FromMilliseconds(250))
                    {
                        inmovement = false;
                    }
                }
            }
            if (ks.IsKeyDown(KeyLeft) && ks.IsKeyUp(KeyRight))
            {
                walkingleft = true;


            }
            else
            {
                walkingleft = false;
            }
            if (ks.IsKeyDown(KeyRight) && ks.IsKeyUp(KeyLeft))
            {
                walkingright = true;


            }
            else
            {
                walkingright = false;
            }
            if (NScounter == 1)
            {
                ResetNSTimer = true;
                NS = true;
            }
            if (NScounter == 2)
            {
                if (ResetNSTimer == true)
                {
                    NSActiontimer = new TimeSpan();
                    ResetNSTimer = false;
                }
                NS = true;
            }
            if (NS)
            {
                NSActiontimer += gameTime.ElapsedGameTime;
                if (NSActiontimer >= TimeSpan.FromMilliseconds(750))
                {
                    NScounter = 0;
                    NSActiontimer = new TimeSpan();
                    NS = false;
                    ResetNSTimer = false;
                }
            }
            if (ks.IsKeyDown(KeyShield) && shieldbroken == false && dodging == false && ks.IsKeyUp(KeyLeft) && ks.IsKeyUp(KeyRight) && falling == false && jumping == false)
            {

                Shieldtime += gameTime.ElapsedGameTime;

            }


            if (dodging == true)
            {
                DodgeTimer += gameTime.ElapsedGameTime;
                if (DodgeTimer >= TimeSpan.FromMilliseconds(475))
                {
                    dodging = false;
                    DodgeTimer = new TimeSpan();
                }
            }
            if (Shieldtime >= TimeSpan.FromMilliseconds(3000))
            {
                shieldbroken = true;
                animations[currentAnimation].Reset();
                Shieldtime = new TimeSpan();

            }
            if (shieldbroken == true)
            {
                //lastAction = null;
                //if (lastAction != null)
                //{
                //    lastAction = null;
                //}
                //animations[currentAnimation].Reset();
                ChangeAnimation(AnimationType.ShieldStun);
                AfterShieldTime += gameTime.ElapsedGameTime;

                attackDelay = TimeSpan.FromMilliseconds(0);
                if (AfterShieldTime >= TimeSpan.FromMilliseconds(3000))
                {
                    shieldbroken = false;
                    animations[currentAnimation].Reset();
                    AfterShieldTime = new TimeSpan();
                }

            }
            if (_position.X > 1225 || _position.X < -25 || _position.Y < -25 || _position.Y > 625)
            {
                _position.Y = 400;
                _position.X = 600;
                if (Lives > 0)
                {
                Lives--;
            }
                DamageCounter = 0;
                knockback = 0;
                knockbackX = 0;
                knockbackY = 0;
                Airtime = new TimeSpan();
                falling = false;
                jumping = false;
                gravity = 9.8f;
                insidespecial = false;
                inupspecial = false;
                state = State.Ground;
                sidespecialtimer = new TimeSpan();
                ChangeAnimation(AnimationType.Idle);

            }
            if (indownspecial)
            {
                downspecialtimer += gameTime.ElapsedGameTime;
            }
            else
            {
                downspecialtimer = new TimeSpan();

            }
            if (downspecialtimer >= TimeSpan.FromMilliseconds(400))
            {
                Airtime = new TimeSpan();
                gravity = 9.81f;
                jumping = false;
                falling = true;
                state = State.Air;
                downspecialtimer = new TimeSpan();
                indownspecial = false;
            }
            if (insidespecial)
            {
                sidespecialtimer += gameTime.ElapsedGameTime;
                if (delaying == false)
                {
                    _position += Vector2.UnitX * 6 * (_effects == SpriteEffects.None ? 1 : -1);
                }
            }
            else
            {
                sidespecialtimer = new TimeSpan();

            }
            if (sidespecialtimer >= TimeSpan.FromMilliseconds(1040))
            {
                Airtime = new TimeSpan();
                gravity = 9.81f;
                jumping = false;
                falling = true;
                state = State.Air;
                sidespecialtimer = new TimeSpan();
                insidespecial = false;
            }

            if (floorhitbox.Intersects(Game1.Platform.BoundingBox))
            {

                if (_boundingBox.Intersects(Game1.PlatformSLLeft.BoundingBox))
                {
                    if (_boundingBox.Intersects(Game1.PlatformLLLeft.BoundingBox))
                    {
                        Airtime = new TimeSpan();

                        state = State.Air;
                        falling = true;
                        jumping = false;

                        _position.Y = Game1.PlatformLLLeft.BoundingBox.Bottom + 21 + 1;
                    }
                }
                else if (_boundingBox.Intersects(Game1.PlatformSLRight.BoundingBox))
                {
                    if (_boundingBox.Intersects(Game1.PlatformLLRight.BoundingBox))
                    {
                        Airtime = new TimeSpan();

                        state = State.Air;
                        falling = true;
                        jumping = false;

                        _position.Y = Game1.PlatformLLRight.BoundingBox.Bottom + 21 + 1;
                    }
                }
                else if (_boundingBox.Intersects(Game1.PlatformMLeft.BoundingBox))
                {
                    if (_boundingBox.Intersects(Game1.PlatformMLLeft.BoundingBox))
                    {
                        Airtime = new TimeSpan();

                        state = State.Air;
                        falling = true;
                        jumping = false;

                        _position.Y = Game1.PlatformMLLeft.BoundingBox.Bottom + 21 + 1;
                    }
                }
                else if (_boundingBox.Intersects(Game1.PlatformMRight.BoundingBox))
                {
                    if (_boundingBox.Intersects(Game1.PlatformMLRight.BoundingBox))
                    {
                        Airtime = new TimeSpan();
                        state = State.Air;
                        falling = true;
                        jumping = false;

                        _position.Y = Game1.PlatformMLRight.BoundingBox.Bottom + 21 + 1;
                    }
                }

                //else if (_boundingBox.Intersects(Game1.PlatformMLCenter.BoundingBox))
                //{
                //    Airtime = new TimeSpan();

                //    falling = true;
                //    jumping = false;

                //    _position.Y = Game1.PlatformMLCenter.BoundingBox.Bottom + 22;
                //} GO A LITTLE FURTHER FOR ACTUAL CODE
                //I was going to put in "at this moment it is 4/20" but the time changed to 4/21 just as i clicked because of the stupid errors :((((( 
                else if (feethitbox.Intersects(Game1.Platform.BoundingBox) && inupspecial == true)
                {
                    Airtime = new TimeSpan();

                    falling = true;
                    state = State.Air;
                    jumping = false;

                    _position.Y = Game1.PlatformMLRight.BoundingBox.Bottom + 21 + 6;
                }
                else if (feethitbox.Intersects(Game1.Platform.BoundingBox) && inupspecial == false)
                {
                    jumping = false;
                    falling = false;
                    state = State.Ground;
                    airCancel = false;
                    jumpsLeft = maxJumps;
                    Airtime = new TimeSpan();
                    _position.Y = Game1.Platform.BoundingBox.Top;// + _origin.Y;
                }


            }
            else if (floorhitbox.Intersects(Game1.MiniPlatform2.BoundingBox))
            {

                if (feethitbox.Intersects(Game1.MiniPlatform2.BoundingBox) && jumping == false && falling == true)
                {
                    jumping = false;
                    falling = false;
                    state = State.Ground;
                    airCancel = false;
                    jumpsLeft = maxJumps;
                    Airtime = new TimeSpan();
                    inupspecial = false;
                    _position.Y = Game1.MiniPlatform2.BoundingBox.Top;// - BoundingBox.Height + _origin.Y;
                }
            }
            else if (floorhitbox.Intersects(Game1.MiniPlatform1.BoundingBox))
            {

                if (feethitbox.Intersects(Game1.MiniPlatform1.BoundingBox) && jumping == false && falling == true)
                {
                    jumping = false;
                    falling = false;
                    airCancel = false;
                    state = State.Ground;
                    jumpsLeft = maxJumps;
                    Airtime = new TimeSpan();
                    inupspecial = false;
                    _position.Y = Game1.MiniPlatform1.BoundingBox.Top; //- BoundingBox.Height + _origin.Y;
                }
            }

            else if (!jumping)
            {
                falling = true;
                state = State.Air;
            }


            //GO HERE FOR THE CENTER PLATFORM BOTTOM STOPPER
            if (_boundingBox.Intersects(Game1.PlatformMLCenter.BoundingBox))
            {
                Airtime = new TimeSpan();

                falling = true;
                state = State.Air;
                jumping = false;

                _position.Y = Game1.PlatformMLCenter.BoundingBox.Bottom + 22;
            }

            if (jumping)
            {
                Airtime += gameTime.ElapsedGameTime;
                float movementAmount = _jump.Y - gravity * (float)Airtime.TotalMilliseconds / 1000f;
                _position.Y -= movementAmount;
                state = State.Air;

                if (movementAmount <= 0)
                {
                    Airtime = TimeSpan.Zero;
                    falling = true;
                    jumping = false;
                }
            }
            else if (falling)
            {
                state = State.Air;
                Airtime += gameTime.ElapsedGameTime;
                float movementAmount = -gravity * (float)Airtime.TotalMilliseconds / 1000f;
                _position.Y -= movementAmount;
            }
            else
            {
                state = State.Ground;
            }

            ActionType currentActionType = ActionType.Walking;
            Action currentAction = null;
            List<PreAction> preActions = new List<PreAction>();
            foreach (Action action in actions[state])
            {
                if (action.ComboDoneUpdate(gameTime, ks))
                {
                    if (action.PreAction != null)
                    {
                        preActions.Add(action.PreAction);
                    }
                    if (action.Call != null && shieldbroken == false)
                    {
                        ActionType actionType = (ActionType)Enum.Parse(typeof(ActionType), action.Call.Method.Name);
                        if ((int)currentActionType >= (int)actionType)
                        {
                            currentAction = action;
                            currentActionType = actionType;
                        }
                    }
                }
            }





            if (delaying)
            {
                elapsedAttackDelay += gameTime.ElapsedGameTime;
                //           if(currentAnimation == AnimationType.SideAerial && 
                //ChangeAnimation(AnimationType.Idle);
                if (elapsedAttackDelay >= attackDelay)
                {

                    elapsedAttackDelay = TimeSpan.Zero;
                    /* if (disabled)
                     {
                         currentActionType = ActionType.Disabled;
                     }
                     else
                     {*/
                    stunned = false;
                    animations[currentAnimation].Reset();
                    
                    delaying = false;
                    if (lastAction != null)
                    {

                        if (lastAction.AfterAction != null)
                        {
                            lastAction.AfterAction(this);
                        }
                        lastAction.ActionDone = true;
                        attackVelocity = Vector2.Zero;
                    }

                    //}
                }
            }

            if (lastAction != null && lastAction.ActionDone)
            {
                lastAction.ActionDone = false;
                lastAction = null;
                //if (shieldbroken)
                //{
                //    shieldbroken = false;
                //    ChangeAnimation(AnimationType.ShieldStun);
                //}
                if (airCancel)
                {
                    ChangeAnimation(AnimationType.Disabled);
                    inupspecial = false;
                }
                else if (lastAction == null && shieldbroken == false)
                {
                    ChangeAnimation(AnimationType.Idle);
                }
                else if(lastAction==null && stunned == true)
                {
                }
                else if (lastAction == null && shieldbroken == true)
                {
                    ChangeAnimation(AnimationType.ShieldStun);
                }
            }


            if (currentAction != null)
            {
                if (lastAction == null || lastAction.AllowFlip)
                {
                    if (ks.IsKeyDown(KeyLeft))
                    {
                        _effects = SpriteEffects.FlipHorizontally;
                    }
                    else if (ks.IsKeyDown(KeyRight))
                    {
                        _effects = SpriteEffects.None;
                    }
                }
            }

            if (currentAction != null && !airCancel && !stunned)
            {
                if (lastAction == null || lastAction.AllowCancel)
                {
                    if (shieldbroken == false && !stunned)
                    {
                        AnimationType newAnimation;
                        if (Enum.TryParse(currentActionType.ToString(), out newAnimation))
                        {
                            currentAnimation = newAnimation;
                        }
                        if (currentAnimation != AnimationType.Idle && currentAnimation != AnimationType.Walking && currentAnimation == AnimationType.ShieldStun)
                        {
                            //Debugger.Break();
                        }
                        currentAction.Call(this);
                        lastAction = currentAction;
                    }
                }
            }
            if (shieldbroken == false)
            {
                foreach (PreAction preAction in preActions)
                {
                    if ((lastAction == null) ||
                        (lastAction.AllowFastFall && preAction.AllowFastFall == lastAction.AllowFastFall) ||
                        (lastAction.AllowMovement && preAction.AllowMovement == lastAction.AllowMovement) ||
                        (lastAction.AllowJump && preAction.AllowJump == lastAction.AllowJump))
                    {
                        preAction.PreCall(this);
                    }
                }
            }

            animations[currentAnimation].Update(gameTime);

            for (int i = 0; i < projectiles.Count; i++)
            {
                projectiles[i].Update(gameTime);
                if (projectiles[i].ShouldDestroy)
                {
                    projectiles.RemoveAt(i);
                    i--;
                }
            }


            Position += attackVelocity;
            attackVelocity.X = MathHelper.Lerp(attackVelocity.X, 0, (float)gameTime.ElapsedGameTime.TotalSeconds * attackDragSeconds.X);
            attackVelocity.Y = MathHelper.Lerp(attackVelocity.Y, 0, (float)gameTime.ElapsedGameTime.TotalSeconds * attackDragSeconds.Y);
            if (Math.Abs(attackVelocity.X) < float.Epsilon)
            {
                attackVelocity.X = 0;
            }
            if (Math.Abs(attackVelocity.Y) < float.Epsilon)
            {
                attackVelocity.Y = 0;
            }


            SourceRectangle = animations[currentAnimation].CurrentFrame.SourceRectangle;
            Origin = animations[currentAnimation].CurrentFrame.Origin;
            DamageRectangle = animations[currentAnimation].CurrentFrame.DamageRectangle;
            


            Color[] temp = new Color[SourceRectangle.Width * SourceRectangle.Height];
            Image.GetData<Color>(0, SourceRectangle, temp, 0, temp.Length);
            for (int i = 0; i < SourceRectangle.Width; i++)
            {
                for (int j = 0; j < SourceRectangle.Height; j++)
                {
                    pixelData[i, j] = temp[i + j * SourceRectangle.Width];
                }
            }            

            //base.Update(gameTime);

            if (_effects == SpriteEffects.FlipVertically)
            {
                _origin = new Vector2(_origin.X, _sourceRectangle.Height - _origin.Y);
            }
            else if (_effects == SpriteEffects.FlipHorizontally)
            {
                _origin = new Vector2(_sourceRectangle.Width - _origin.X, _origin.Y);
            }

            _boundingBox = new Rectangle((int)(Position.X - 8), (int)(Position.Y - 21), 16, 21);

            feethitbox = new Rectangle((int)_position.X - 3, (int)_position.Y - 1, 7, 1);
            floorhitbox = new Rectangle((int)_position.X - 3, (int)_position.Y, 7, 1);

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            foreach (Projectile projectile in projectiles)
            {
                projectile.Draw(spriteBatch);
            }
            
            //spriteBatch.Draw(pixel, Game1.Platform.BoundingBox, Color.Red);
            //spriteBatch.Draw(pixel, Game1.PlatformMCenter.BoundingBox, Color.Green);
        }
        public override void DrawHitbox(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Color color)
        {
            base.DrawHitbox(spriteBatch, graphicsDevice, color);
        }
        public Rectangle dhitbox;
        public void DrawDamageHitbox(SpriteBatch spriteBatch, GraphicsDevice graphics, Color color)
        {
            Texture2D pixel = new Texture2D(graphics, 1, 1);
            pixel.SetData<Color>(new Color[] { Color.White });

            

            if (_effects == SpriteEffects.FlipHorizontally)
            {
                dhitbox = new Rectangle((int)X - DamageRectangle.X- DamageRectangle.Width, (int)Y +((SourceRectangle.Height- (int)Origin.Y)) - ((SourceRectangle.Height - (int)Origin.Y) - DamageRectangle.Y), DamageRectangle.Width, DamageRectangle.Height);
            }
            else
            {
                dhitbox = new Rectangle((int)X + DamageRectangle.X, (int)Y + ((SourceRectangle.Height - (int)Origin.Y)) - ((SourceRectangle.Height - (int)Origin.Y) - DamageRectangle.Y), DamageRectangle.Width, DamageRectangle.Height);
            }

            spriteBatch.Draw(pixel, dhitbox, color);
        }
    }
}
