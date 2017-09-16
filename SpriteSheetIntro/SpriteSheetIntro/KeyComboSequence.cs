using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace SpriteSheetIntro
{
    public class KeyComboSequence
    {
        private int indexToCheck;

        private KeyCombo[] keyCombos;

        public KeyCombo[] KeyCombos
        {
            get
            {
                return keyCombos;
            }
        }

        private TimeSpan timeToNextKeyCombo;
        private TimeSpan elaspedTime;

        public KeyComboSequence(TimeSpan timeToNextKeyCombo, params KeyCombo[] keyCombos)
        {
            this.timeToNextKeyCombo = timeToNextKeyCombo;
            this.keyCombos = keyCombos;
            indexToCheck = 0;
            elaspedTime = TimeSpan.Zero;
        }

        public bool ComboDoneUpdate(GameTime gameTime, KeyboardState keyboardState)
        {
            elaspedTime += gameTime.ElapsedGameTime;
            if (elaspedTime <= timeToNextKeyCombo)
            {
                if (keyCombos[indexToCheck].IsActive(keyboardState))
                {
                    indexToCheck++;
                    elaspedTime = TimeSpan.Zero;
                    if (indexToCheck >= keyCombos.Length)
                    {
                        indexToCheck = 0;
                        return true;
                    }
                }
            }
            else
            {
                indexToCheck = 0;
                elaspedTime = TimeSpan.Zero;
            }
            return false;
        }
    }
}
