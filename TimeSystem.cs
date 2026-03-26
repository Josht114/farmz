using Microsoft.Xna.Framework;

namespace FarmGame.Systems
{
    public enum Season { Spring, Summer, Autumn, Winter }

    /// <summary>
    /// Tracks in-game time.
    /// One real second  = 1 in-game minute  (configurable via TimeScale).
    /// One in-game day  = 24 in-game minutes.
    /// One in-game week = 7 days, one season = 4 weeks.
    /// </summary>
    public class TimeSystem
    {
        // ------------------------------------------------------------------ constants

        public const float MINUTES_PER_REAL_SECOND = 1f;
        public const int   MINUTES_PER_DAY         = 1440;  // 24 × 60
        public const int   DAYS_PER_SEASON         = 28;

        // ------------------------------------------------------------------ state

        public int    Day     { get; private set; } = 1;
        public int    Hour    { get; private set; } = 6;    // start at 6 AM
        public int    Minute  { get; private set; } = 0;
        public Season Season  { get; private set; } = Season.Spring;
        public int    Year    { get; private set; } = 1;

        private float _accumulator; // fractional in-game minutes

        public float TimeScale { get; set; } = 1f;

        // ------------------------------------------------------------------ derived

        public bool IsDay   => Hour >= 6 && Hour < 20;
        public bool IsNight => !IsDay;

        public string TimeString =>
            $"{Hour:D2}:{Minute:D2}  Day {Day}  {Season}  Year {Year}";

        // ------------------------------------------------------------------ update

        public void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _accumulator += delta * MINUTES_PER_REAL_SECOND * TimeScale;

            while (_accumulator >= 1f)
            {
                _accumulator -= 1f;
                AdvanceMinute();
            }
        }

        // ------------------------------------------------------------------ private

        private void AdvanceMinute()
        {
            Minute++;
            if (Minute < 60) return;

            Minute = 0;
            Hour++;
            if (Hour < 24) return;

            Hour = 0;
            AdvanceDay();
        }

        private void AdvanceDay()
        {
            Day++;
            if (Day <= DAYS_PER_SEASON) return;

            Day = 1;
            Season = (Season)(((int)Season + 1) % 4);
            if (Season == Season.Spring) Year++;
        }
    }
}
