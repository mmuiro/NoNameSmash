using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpriteSheetIntro
{
    public class PreAction
    {
        private Action<Character> preCall;

        public Action<Character> PreCall
        {
            get { return preCall; }
            set { preCall = value; }
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

        private bool allowCancel;

        public bool AllowCancel
        {
            get { return allowCancel; }
            set { allowCancel = value; }
        }

        public PreAction(Action<Character> preCall)
        {
            this.preCall = preCall;
        }
    }
}
