namespace SpriteSheetIntro
{
    public class AnimationEventArgs : System.EventArgs
    {
        public AnimationType AnimationType { get; set; }
        public int PlayerID { get; set; }

        public AnimationEventArgs(AnimationType animationType, int playerID)
        {
            this.AnimationType = AnimationType;
            this.PlayerID = playerID;
        }
    }
}