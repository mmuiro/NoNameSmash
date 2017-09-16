using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace SpriteSheetIntro
{
    public enum ComboType
    {
        Pressed,
        Released
    }

    public class KeyCombo
    {
        private Keys[] keys;

        public Keys[] Keys
        {
            get { return keys; }
        }

        private ComboType comboType;

        public ComboType ComboType
        {
            get { return comboType; }
        }


        public KeyCombo(ComboType comboType, params Keys[] keys)
        {
            this.comboType = comboType;
            this.keys = keys;
        }

        public bool IsActive(KeyboardState keyboardState)
        {
            if (comboType == SpriteSheetIntro.ComboType.Pressed)
            {
                return IsKeysDown(keyboardState);
            }
            else
            {
                return IsKeysUp(keyboardState);
            }
        }

        private bool IsKeysDown(KeyboardState keyboardState)
        {
            foreach (Keys key in Keys)
            {
                if (!keyboardState.IsKeyDown(key))
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsKeysUp(KeyboardState keyboardState)
        {
            foreach (Keys key in Keys)
            {
                if (!keyboardState.IsKeyUp(key))
                {
                    return false;
                }
            }
            return true;
        }

    }
}
