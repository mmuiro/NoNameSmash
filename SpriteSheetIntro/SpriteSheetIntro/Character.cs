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

        SideSmash,
        DownSmash,
        UpSmash,
        NeutralSmash,
        UpSpecial,
        SideSpecial,
        DownSpecial,
        DownSpecialProjectile,
        NeutralSpecial,
        Disabled,
        ShieldStun,
        SpotDodge,
        Dodge,
        Shield,
        Idle,
        Walking
    }

    public enum ActionType
    {

        SideSmash = 0,
        DownSmash,
        UpSmash,
        NeutralSmash,
        UpSpecial,
        SideSpecial,
        DownSpecial,
        DownSpecialProjectile,
        NeutralSpecial,
        Dodge,
            Shield,
        ShieldStun,
        
        //Disabled,
        FastFall,
        Jump,
        Idle,
        Walking
    }

    public class Character : Sprite
    {
        List<Action> actions;

        public AnimationType currentAnimation = AnimationType.Idle;
        Dictionary<AnimationType, Animation> animations;

        List<Projectile> projectiles = new List<Projectile>();

        TimeSpan sidespecialtimer = TimeSpan.Zero;
        TimeSpan elapsedAttackDelay = TimeSpan.Zero;
        TimeSpan attackDelay = TimeSpan.FromMilliseconds(250);
        bool delaying = false;
        bool insidespecial = false;
        Vector2 attackVelocity;
        Vector2 attackDragSeconds;
        
        public Rectangle feethitbox;
        public Rectangle floorhitbox;
        bool shieldbroken = false;
        bool jumping = false;
        bool airCancel = false;
        bool falling = false;
        int maxJumps = 2;
        int jumpsLeft;
        public TimeSpan Airtime = new TimeSpan();
        public TimeSpan UpSpecAttack = new TimeSpan();
        public TimeSpan Shieldtime = new TimeSpan();
        public TimeSpan AfterShieldTime = new TimeSpan();
        Action lastAction;

        KeyboardState lks;
        public float gravity = 9.81f;

        public Character(Vector2 location, Texture2D image, Color color, Dictionary<AnimationType, Animation> animations)
            : base(location, image, color)
        {
            jumpsLeft = maxJumps;
            _jump = new Vector2(6);
            this.animations = animations;
            foreach (AnimationType animationType in animations.Keys)
            {
                animations[animationType].AnimationFinished += AnimationFinished;
            }
            SourceRectangle = animations[currentAnimation].CurrentFrame.SourceRectangle;
            Origin = animations[currentAnimation].CurrentFrame.Origin;
            _boundingBox.Width = 16;//_boundingBox.Width / 6 - 24;
            _boundingBox.Height = 21;//_boundingBox.Height / 14 - 19;
            feethitbox = new Rectangle((int)_position.X - 3, (int)_position.Y - 1, 7, 1);
            floorhitbox = new Rectangle((int)_position.X - 3, (int)_position.Y, 7, 1);

            actions = new List<Action>();
            actions.Add(new Action(Idle, TimeSpan.MaxValue, new KeyCombo(ComboType.Released, Keys.A, Keys.D)) { AllowMovement = true, AllowJump = true, AllowFastFall = true, AllowCancel = true, AllowFlip = true });

            actions.Add(new Action(
                            new PreAction(PreWalking) { AllowMovement = true },
                            Walking, TimeSpan.MaxValue, new KeyCombo(ComboType.Pressed, Keys.A))
            { AllowMovement = true, AllowJump = true, AllowFastFall = true, AllowCancel = true, AllowFlip = true });

            actions.Add(new Action(
                            new PreAction(PreWalking) { AllowMovement = true },
                            Walking, TimeSpan.MaxValue, new KeyCombo(ComboType.Pressed, Keys.D))
            { AllowMovement = true, AllowJump = true, AllowFastFall = true, AllowCancel = true, AllowFlip = true });

            //actions.Add(new Action(Shield, TimeSpan.MaxValue, new KeyCombo(ComboType.Pressed, Keys.Y)) {AllowCancel = true});
            actions.Add(new Action(
                new PreAction(PreShield) { AllowCancel = false },
                Shield, TimeSpan.MaxValue, new KeyCombo(ComboType.Pressed, Keys.Y))
            { AllowCancel = true });
            actions.Add(new Action(Dodge, TimeSpan.MaxValue, new KeyCombo(ComboType.Pressed, Keys.Y, Keys.A)));
            actions.Add(new Action(Dodge, TimeSpan.MaxValue, new KeyCombo(ComboType.Pressed, Keys.Y, Keys.D)));

            actions.Add(new Action(SideSmash, TimeSpan.MaxValue, new KeyCombo(ComboType.Pressed, Keys.A, Keys.T)));
            actions.Add(new Action(SideSmash, TimeSpan.MaxValue, new KeyCombo(ComboType.Pressed, Keys.D, Keys.T)));

            actions.Add(new Action(SideSpecial, TimeSpan.MaxValue, new KeyCombo(ComboType.Pressed, Keys.A, Keys.R)) { AllowMovement = true, AllowFlip = true });
            actions.Add(new Action(SideSpecial, TimeSpan.MaxValue, new KeyCombo(ComboType.Pressed, Keys.D, Keys.R)) { AllowMovement = true, AllowFlip = true });


            actions.Add(new Action(DownSmash, TimeSpan.MaxValue, new KeyCombo(ComboType.Pressed, Keys.S, Keys.T)));
            actions.Add(new Action(DownSpecial, TimeSpan.MaxValue, new KeyCombo(ComboType.Pressed, Keys.S, Keys.R)));

            actions.Add(new Action(UpSpecial, ResetFall, TimeSpan.MaxValue, new KeyCombo(ComboType.Pressed, Keys.W, Keys.R)));

            actions.Add(new Action(UpSmash, TimeSpan.MaxValue, new KeyCombo(ComboType.Pressed, Keys.W, Keys.T)));
            actions.Add(new Action(
                            new PreAction(PreJump) { AllowJump = true },
                            null, TimeSpan.MaxValue, new KeyCombo(ComboType.Released, Keys.W), new KeyCombo(ComboType.Pressed, Keys.W))
            { AllowMovement = true, AllowJump = true, AllowFastFall = true });

            actions.Add(new Action(
                            new PreAction(PreFastFall) { AllowFastFall = true },
                            null, TimeSpan.MaxValue, new KeyCombo(ComboType.Released, Keys.S), new KeyCombo(ComboType.Pressed, Keys.S)));

            actions.Add(new Action(NeutralSpecial, TimeSpan.MaxValue, new KeyCombo(ComboType.Pressed, Keys.R)));

            actions.Add(new Action(NeutralSmash, TimeSpan.MaxValue, new KeyCombo(ComboType.Pressed, Keys.T)));
        }

        void Idle(Character character)
        {
            character.ChangeAnimation(AnimationType.Idle);
            character.attackDelay = TimeSpan.Zero;
        }                                              
        void Shield(Character character)
        {
            //if (shieldbroken == true)
            //{
            //    character.ChangeAnimation(AnimationType.ShieldStun);
            //}
                  
                character.ChangeAnimation(AnimationType.Shield);
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
        }
        void PreWalking(Character character)
        {
            character._position += Vector2.UnitX * 5 * (_effects == SpriteEffects.None ? 1 : -1);
        }

        void Walking(Character character)
        {
            character.ChangeAnimation(AnimationType.Walking);
            character.attackDelay = TimeSpan.Zero;
        }

        void SideSmash(Character character)
        {
            character.ChangeAnimation(AnimationType.SideSmash);
            character.attackDelay = TimeSpan.FromMilliseconds(250);
        }

        void SideSpecial(Character character)
        {
            character.ChangeAnimation(AnimationType.SideSpecial);
            insidespecial = true;
            jumping = false;
            falling = false;
            gravity = 0f;


            character.attackDelay = TimeSpan.FromMilliseconds(100);
            character._position += Vector2.UnitX * 1 * (_effects == SpriteEffects.None ? 1 : -1);
        }

        void DownSmash(Character character)
        {
            character.ChangeAnimation(AnimationType.DownSmash);
            character.attackDelay = TimeSpan.FromMilliseconds(250);
        }

        void DownSpecial(Character character)
        {
            character.ChangeAnimation(AnimationType.DownSpecial);
            character.projectiles.Add(new AnimatingProjectile(character.Position, character.Image, Vector2.Zero,
                new Animation(character.animations[AnimationType.DownSpecialProjectile].Frames, character.animations[AnimationType.DownSpecialProjectile].AnimationSpeed)));
            character.projectiles[projectiles.Count - 1].Speed *= (character._effects == SpriteEffects.None ? 1 : -1);
            character.projectiles[projectiles.Count - 1].Effects = character._effects;
            character.projectiles[projectiles.Count - 1].Position += Vector2.UnitX * (character._effects == SpriteEffects.None ? character._sourceRectangle.Width / 2 : -character._sourceRectangle.Width / 2);
            character.attackDelay = TimeSpan.FromMilliseconds(250);
        }

        void UpSmash(Character character)
        {
            character.ChangeAnimation(AnimationType.UpSmash);
            character.attackDelay = TimeSpan.FromMilliseconds(250);
           
        }

        void UpSpecial(Character character)//,GameTime gameTime)
        {
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
            character.ChangeAnimation(AnimationType.NeutralSpecial);
            character.attackDelay = TimeSpan.FromMilliseconds(250);
        }

        void NeutralSmash(Character character)
        {
            character.ChangeAnimation(AnimationType.NeutralSmash);
            character.attackDelay = TimeSpan.FromMilliseconds(100);
        }

        void PreJump(Character character)
        {
            if (character.jumpsLeft > 0)
            {
                character.Airtime = new TimeSpan();
                character.jumping = true;
                character.falling = false;
                character.jumpsLeft--;
                character.attackDelay = TimeSpan.Zero;
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


        void AnimationFinished(object sender, EventArgs e)
        {
            delaying = true;
            
        }

        void ChangeAnimation(AnimationType animationType)
        {
            if (currentAnimation != animationType)
            {
                animations[currentAnimation].Reset();
                currentAnimation = animationType;
            }
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            KeyboardState ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.Y) && shieldbroken == false)
            {
                
                Shieldtime += gameTime.ElapsedGameTime;

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
                Airtime = new TimeSpan();
                falling = false;
                jumping = false;
                ChangeAnimation(AnimationType.Idle);

            }
            if (insidespecial)
            {
                sidespecialtimer += gameTime.ElapsedGameTime;
            }
            if (sidespecialtimer >= TimeSpan.FromMilliseconds(1040))
            {
                Airtime = new TimeSpan();
                gravity = 9.81f;
                jumping = false;
                falling = true;
                sidespecialtimer = new TimeSpan();
                insidespecial = false;
            }

            ActionType currentActionType = ActionType.Walking;
            Action currentAction = null;
            List<PreAction> preActions = new List<PreAction>();
            foreach (Action action in actions)
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

            if (floorhitbox.Intersects(Game1.Platform.BoundingBox))
            {

                if (_boundingBox.Intersects(Game1.PlatformSLLeft.BoundingBox))
                {
                    if (_boundingBox.Intersects(Game1.PlatformLLLeft.BoundingBox))
                    {
                        Airtime = new TimeSpan();

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
                else if (feethitbox.Intersects(Game1.Platform.BoundingBox))
                {
                    jumping = false;
                    falling = false;
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
                    airCancel = false;
                    jumpsLeft = maxJumps;
                    Airtime = new TimeSpan();
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
                    jumpsLeft = maxJumps;
                    Airtime = new TimeSpan();
                    _position.Y = Game1.MiniPlatform1.BoundingBox.Top; //- BoundingBox.Height + _origin.Y;
                }
            }
           
            else if (!jumping)
            {
                falling = true;
            }
            

            //GO HERE FOR THE CENTER PLATFORM BOTTOM STOPPER
            if (_boundingBox.Intersects(Game1.PlatformMLCenter.BoundingBox))
            {
                Airtime = new TimeSpan();

                falling = true;
                jumping = false;

                _position.Y = Game1.PlatformMLCenter.BoundingBox.Bottom + 22;
            }



            if (delaying)
            {
                elapsedAttackDelay += gameTime.ElapsedGameTime;

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
                    animations[currentAnimation].Reset();
                    delaying = false;
                    if (lastAction != null)
                    {
      
                        if(lastAction.AfterAction != null)
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
                }
                else if (lastAction == null && shieldbroken == false)
                {
                    ChangeAnimation(AnimationType.Idle);
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
                    if (ks.IsKeyDown(Keys.A))
                    {
                        _effects = SpriteEffects.FlipHorizontally;
                    }
                    else if (ks.IsKeyDown(Keys.D))
                    {
                        _effects = SpriteEffects.None;
                    }
                }
            }

            if (currentAction != null && !airCancel)
            {
                if (lastAction == null || lastAction.AllowCancel)
                {
                    if (shieldbroken == false)
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
            if(shieldbroken == false)
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
            if(Math.Abs(attackVelocity.X) < float.Epsilon)
            {
                attackVelocity.X = 0;
            }
            if(Math.Abs(attackVelocity.Y) < float.Epsilon)
            {
                attackVelocity.Y = 0;
            }

            if (jumping)
            {
                Airtime += gameTime.ElapsedGameTime;
                float movementAmount = _jump.Y - gravity * (float)Airtime.TotalMilliseconds / 1000f;
                _position.Y -= movementAmount;

                if (movementAmount <= 0)
                {
                    Airtime = TimeSpan.Zero;
                    falling = true;
                    jumping = false;
                }
            }
            else if (falling)
            {
                Airtime += gameTime.ElapsedGameTime;
                float movementAmount = -gravity * (float)Airtime.TotalMilliseconds / 1000f;
                _position.Y -= movementAmount;
            }

            SourceRectangle = animations[currentAnimation].CurrentFrame.SourceRectangle;
            Origin = animations[currentAnimation].CurrentFrame.Origin;

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
            lks = ks;
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
    }
}
