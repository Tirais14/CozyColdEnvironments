namespace UTIRLib.Animation
{
    public class AnimatorParameterTrigger : AnimatorParameter<bool>
    {
        private int turnedOnFramesCount;

        public AnimatorParameterTrigger(string name) : base(name, false)
        { }

        public void Activate() => Set(true);

        ///<summary>
        /// Base method that observes when to turn off the trigger.
        /// Put it to update method of custom animator state machine
        /// </summary>
        public void StateObserver()
        {
            if (!value) return;

            turnedOnFramesCount++;
            if (turnedOnFramesCount >= 1) { Set(false); }
        }
    }
}