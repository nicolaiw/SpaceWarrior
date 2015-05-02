using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace SpaceWarrior.Entities
{
    public class Player : ModelBase
    {
       
        public Player(
            double posX,
            double posY,
            double width,
            double height,
            double speedX,
            double speedY,
            Action<double> moveHitBoxX,
            Action<double> moveHitBoxY) : base(posX, posY, width, height, speedX, speedY, moveHitBoxX, moveHitBoxY)
        { }

        public void Update(
            double timeSinceLastFrame,
            bool up,
            bool down,
            bool left,
            bool right,
            double worldSizeX,
            double worldSizeY)
        {
            if (up) PosY -= SpeedY * timeSinceLastFrame;
            if (down) PosY += SpeedY * timeSinceLastFrame;
            if (left) PosX -= SpeedX * timeSinceLastFrame;
            if (right) PosX += SpeedX * timeSinceLastFrame;

            if (PosX < 0)PosX = 0;
            if (PosY < 0) PosY = 0;
            if (PosX > worldSizeX - Width) PosX = worldSizeX - Width;
            if (PosY > worldSizeY - Height) PosY = worldSizeY - Height;
        }
    }
}
