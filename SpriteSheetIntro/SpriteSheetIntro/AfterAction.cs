using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpriteSheetIntro
{
    class AfterAction
    {
        private Action<Character> afterCall;

        public Action<Character> AfterCall
        {
            get { return afterCall; }
            set { afterCall = value; }
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

        private bool allowFastFall;

        public bool AllowFastFall
        {
            get { return allowFastFall; }
            set { allowFastFall = value; }
        }

        public AfterAction(Action<Character> afterCall)
        {
            this.afterCall = afterCall;
        }
    }
}
