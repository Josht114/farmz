using Microsoft.Xna.Framework;

namespace FarmGame.Systems
{
    public enum Season { Spring, Summer, Autumn, Winter }

    public class TimeSystem
    {
        public const float MINUTES_PER_REAL_SECOND = 1f;
        public const int   DAYS_PER_SEASON         = 28;

        public int    Day    { get; private set; } = 1;
        public int    Hour   { get; private set; } = 6;
        public int    Minute { get; private set; } = 0;
        public Season Season { get; private set; } = Season.Spring;
        public int    Year   { get; private set; } = 1;

        public float TimeScale { get; set; } = 1f;

        private float _accumulator;

        // 0.0 = full night, 1.0 = full day
        public float DaylightAmount
        {
            get
            {
                float timeOfDay = Hour + Minute / 60f;

                // Dawn: 5→7, full day: 7→18, dusk: 18→21, night: 21→5
                if (timeOfDay >= 7f  && timeOfDay < 18f) return 1f;
                if (timeOfDay >= 5f  && timeOfDay < 7f)  return (timeOfDay - 5f) / 2f;
                if (timeOfDay >= 18f && timeOfDay < 21f) return 1f - (timeOfDay - 18f) / 3f;
                return 0f;
            }
        }

        public bool IsDay   => Hour >= 7 && Hour < 18;
        public bool IsNight => !IsDay;

        public string TimeString =>
            $"{Hour:D2}:{Minute:D2}  Day {Day}  {Season}  Year {Year}";

        public void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _accumulator += delta * MINUTES_PER_REAL_SECOND * TimeScale;

            while (_accumulator >= 1f)
            {
                _accumulator -= 1f;
                AdvanceMinute();
            }

            // Auto-sleep at midnight
            if (Hour == 0 && Minute == 0)
                Sleep();
        }

        // Call when player presses sleep key
        public void Sleep()
        {
            AdvanceDay();
            Hour   = 6;
            Minute = 0;
            _accumulator = 0f;
        }

        private void AdvanceMinute()
        {
            Minute++;
            if (Minute < 60) return;
            Minute = 0;
            Hour++;
            if (Hour < 24) return;
            Hour = 0;
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