using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace SpaceWarrior.Entities
{
    public class Bullet : ModelBase
    {
        public event EventHandler<EventArgs> BulletOutOfScope;
        private Action RemoveBullet;

        public Bullet(
            double posX,
            double posY,
            double speedX,
            double speedY,
            double width,
            double height,
            //Rectangle hitBox,
            Action<double> moveHitBoxX,
            Action<double> moveHitBoxY,
            Action removeBullet)
            : base(posX, posY, width, height, speedX, speedY, moveHitBoxX, moveHitBoxY)
        {
            RemoveBullet = removeBullet;
        }

        public void Update(double timeSinceLastFrame, double wolrdWidth)
        {
            PosX += SpeedX * timeSinceLastFrame;

            if (PosX > wolrdWidth)
            {
                RemoveBullet();
                this.MoveHitBoxX = null;
                this.MoveHitBoxY = null;

                var ev = BulletOutOfScope;
                if(ev != null)
                    ev(this, new EventArgs());
            }
        }
    }

}
