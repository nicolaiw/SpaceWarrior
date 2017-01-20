using System;

namespace SpaceWarrior.Entities
{
    public class Enemy : ModelBase
    {
        private int Hits { get; set; }
        private int MaxHits { get; set; }
        private Action _removeEnemy;

        public Enemy(
            double posX,
            double posY,
            double width,
            double height,
            double speedX,
            double speedY,
            Action<double> moveHitBoxX,
            Action<double> moveHitBoxY,
            Action removeEnemy,
            int maxHits) : base(posX, posY, width, height, speedX, speedY, moveHitBoxX, moveHitBoxY)
        {
            _removeEnemy = removeEnemy;
            MaxHits = maxHits;
            var rnd = new Random();
            var rndVal = (rnd.NextDouble()*100);
            SpeedX += rndVal;
            SpeedY += rndVal;
        }

        public void Update(double timeSinceLastFrame, double playerPosX, double playerPosY)
        {
            // Vektor basiertes movement http://www.youtube.com/watch?v=hznpuFrAaEc&index=18&list=PL2C21DE50640DBD4D

            // Vektor der in richtung des Spielers zeigt
            var speedX = playerPosX - PosX;
            var speedY = playerPosY - PosY;

            // Vekorlänge (Pythagoras)
            var speed = Math.Sqrt(Math.Pow(speedX,2)+Math.Pow(speedY,2));

            // Vektor normalisieren, indem man Ihn durch seine länge teilt
            speedX /= speed;
            speedY /= speed;

            // somit ist der speed nicht mehr von der Länge des Vektors abhängig,
            // sondern von den von uns vorgegebenen Werten
            speedX *= SpeedX*timeSinceLastFrame;
            speedY *= SpeedY*timeSinceLastFrame;

            PosX += speedX;
            PosY += speedY;
        }
    }
}
