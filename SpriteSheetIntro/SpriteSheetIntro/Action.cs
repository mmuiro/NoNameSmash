using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace SpriteSheetIntro
{
    public class Action : KeyComboSequence
    {
        private PreAction preAction;

        public PreAction PreAction
        {
            get { return preAction; }
            set { preAction = value; }
        }

        private Action<Character> call;
        
        public Action<Character> Call
        {
            get
            {
                return call;
            }
        }

        private Action<Character> afterAction;

        public Action<Character> AfterAction
        {
            get { return afterAction; }
            set { afterAction = value; }
        }

        private bool allowMovement;

        public bool AllowMovement
        {
            get { return allowMovement; }
            set { allowMovement = value; }
        }

        private bool allowJump;

        public bool AllowJump
        {
            get { return allowJump; }
            set { allowJump = value; }
        }
        private bool allowFlip;
        public bool AllowFlip
        {
            get { return allowFlip; }
            set { allowFlip = value; }
        }

        private bool allowFastFall;

        public bool AllowFastFall
        {
            get { return allowFastFall; }
            set { allowFastFall = value; }
        }
        
        private bool allowCancel;

        public bool AllowCancel
        {
            get { return allowCancel; }
            set { allowCancel = value; }
        }

        private bool actionDone;

        public bool ActionDone
        {
            get { return actionDone; }
            set { actionDone = value; }
        }

        public Action(Action<Character> call, TimeSpan timeToNextKeyCombo, params KeyCombo[] keyCombos)
            : this(null, call, null, timeToNextKeyCombo, keyCombos)
        { }

        public Action(PreAction preAction, Action<Character> call, TimeSpan timeToNextKeyCombo, params KeyCombo[] keyCombos)
            : this(preAction, call, null, timeToNextKeyCombo, keyCombos)
        {
        }

        public Action(Action<Character> call, Action<Character> afterAction, TimeSpan timeToNextKeyCombo, params KeyCombo[] keyCombos)
            : this(null, call, afterAction, timeToNextKeyCombo, keyCombos)
        {
        }
            
        public Action(PreAction preAction, Action<Character> call, Action<Character> afterAction, TimeSpan timeToNextKeyCombo, params KeyCombo[] keyCombos)
            : base(timeToNextKeyCombo, keyCombos)
        {
            this.preAction = preAction;
            this.call = call;
            this.afterAction = afterAction;
            allowMovement = false;
            allowJump = false;
            allowFastFall = false;
            allowCancel = false;
            actionDone = false;
            allowFlip = false;
        }
//Maybe Remove this Stuff

    }
}
