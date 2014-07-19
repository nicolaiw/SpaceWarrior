using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceWarrior.Entities
{
    public abstract class ModelBase
    {
        protected Action<double> MoveHitBoxX;
        protected Action<double> MoveHitBoxY;

        private double _posX;
        public double PosX
        {
            get { return _posX; }
            protected set
            {
                _posX = value;
                MoveHitBoxX(_posX);
            }
        }

        private double _posY;
        public double PosY
        {
            get { return _posY; }
            protected set
            {
                _posY = value;
                MoveHitBoxY(_posY);
            }
        }

        public double Width
        {
            get;
            protected set;
        }

        public double Height
        {
            get;
            protected set;
        }

        public double SpeedX
        {
            get;
            protected set;
        }

        public double SpeedY
        {
            get;
            protected set;
        }

        protected ModelBase(
            double posX,
            double posY,
            double width,
            double height,
            double speedX,
            double speedY,
            Action<double> moveHitBoxX,
            Action<double> moveHitBoxY)
        {
            MoveHitBoxX = moveHitBoxX;
            MoveHitBoxY = moveHitBoxY;

            PosX = posX;
            PosY = posY;
            Width = width;
            Height = height;
            SpeedX = speedX;
            SpeedY = speedY;
        }
    }
}
