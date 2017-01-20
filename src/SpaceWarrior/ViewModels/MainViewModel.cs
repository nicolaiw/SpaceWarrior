using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SpaceWarrior.Entities;

namespace SpaceWarrior.ViewModels
{
    public class MainViewModel : PropertyChangedImplementation
    {
        private Player Player { get; set; }
        private readonly List<Bullet> _bullets;
        private readonly List<Enemy> _enemies;
        //                    posX,   posY,   speedX, speedY, MaxHits
        private readonly Func<double, double, double, double, int, Enemy> _createEnemy;
        private bool _runWorkerLoop;
        private Task _workerTask;
        private readonly CancellationTokenSource _cts;
        private readonly CancellationToken _ct;

        private const double ShotFrequency = 0.1; // Frquenz ist eigentlich das falsche Wort hier
        private double _timeSinceLastShot = 0;

        public double BulletWidth { get; private set; }
        public double BulletHeight { get; private set; }

        public bool Up { get; set; }
        public bool Down { get; set; }
        public bool Left { get; set; }
        public bool Right { get; set; }
        public bool Space { get; set; }

        public bool CanAddBullet
        {
            get { return _timeSinceLastShot >= ShotFrequency; }
        }

        public MainViewModel(
            double playerPosX,
            double playerPosY,
            double playerWidth,
            double playerHeight,
            double worldWidth,
            double worldHeight,
            double playerSpeedX,
            double playerSpeedY,
            Action<double> movePlayerX,
            Action<double> movePlayerY,
            double bulletWidth,
            double bulletHeigth,
            //   posX,   posY,   speedX, speedY, MaxHits
            Func<double, double, double, double, int, Enemy> createEnemy)
        {
            Player = new Player(playerPosX, playerPosY, playerWidth, playerHeight, playerSpeedX, playerSpeedY, movePlayerX, movePlayerY);

            WorldWidth = worldWidth;
            WorldHeight = worldHeight;
            _bullets = new List<Bullet>();
            _enemies = new List<Enemy>();
            BulletWidth = bulletWidth;
            BulletHeight = bulletHeigth;
            _createEnemy = createEnemy;
            _runWorkerLoop = true;
            _cts = new CancellationTokenSource();
            _ct = _cts.Token;
        }

        // Wrapper um die createEnemyFunc etwas leserlicher zu machen
        private Enemy CreateEnemy(double posX, double posY, double speedX, double speedY, int maxHits)
        {
            return _createEnemy(posX, posY, speedX, speedY, maxHits);
        }

        private static double GetCurrentMilli()
        {
            var jan1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var span = DateTime.UtcNow - jan1970;
            return span.TotalMilliseconds;
        }

        
        public void AddBulletIfPossible(Action<double> moveBulletX, Action<double> moveBulletY, Action removeBullet)
        {
            if (!CanAddBullet) return;

            var bulletPosX = Player.PosX + Player.Width - BulletWidth;
            var bulletPosY = Player.PosY + (Player.Height / 2) - (BulletHeight / 2);

            var bullet = new Bullet(bulletPosX, bulletPosY, Player.SpeedX + 300.0, 0, BulletWidth, BulletHeight, moveBulletX, moveBulletY, removeBullet);
            _bullets.Add(bullet);
            bullet.BulletOutOfScope += RemoveBullet;
            _timeSinceLastShot = 0;
        }

        public void StopWorker()
        {
            _runWorkerLoop = false;
            _cts.Cancel();
            _workerTask.Wait(_ct);
        }

        public void RunWorker()
        {
            _workerTask =
                new Task(() =>
                {
                    try
                    {
                       // Berechnung anhand der Zeit die der Rechner für das Berechnenn und Zeichnen braucht.
                       // Dadurch bewegt sich der player auf jeder Hardware gleich schnell (wenn auch vll. ruckeliger)
                       var lastFrame = GetCurrentMilli();

                       // Testgegner
                       _enemies.Add(CreateEnemy(0, 10, 100, 200, 1));
                        _enemies.Add(CreateEnemy(WorldWidth / 2, 10, 20, 50, 1));
                        _enemies.Add(CreateEnemy(WorldWidth, 10, 50, 30, 1));

                       /*
                        *        GAME LOOP 
                        */
                        while (_runWorkerLoop)
                        {
                            _ct.ThrowIfCancellationRequested();

                            var thisFrame = GetCurrentMilli();
                            var timeSinceLastFrame = (thisFrame - lastFrame) / 1000.0;

                           /*
                                 What if the player does not hit space for a bullet ?
                                 --> potentionally double overflow !!
                                 So just increase if !CanAddBullet
                            */
                            if (!CanAddBullet)
                            {
                                 _timeSinceLastShot += timeSinceLastFrame;
                            }
                            lastFrame = thisFrame;

                            Player.Update(timeSinceLastFrame, Up, Down, Left, Right, WorldWidth, WorldHeight);
                            UpdateBullets(timeSinceLastFrame);
                            UpdateEnemies(timeSinceLastFrame);

                            Thread.Sleep(15);
                        }
                    }
                    catch (TaskCanceledException)
                    {
                       // Canelation requested
                   }
                }, _ct);

            _workerTask.Start();
        }

        private void RemoveBullet(object sender, EventArgs e)
        {
            _bullets.Remove(sender as Bullet);
        }

        public void UpdateBullets(double timeSinceLastFrame)
        {
            //TODO: was passiert wenn während der loop neue bullets hinzukommen ?
            //foreach läuft auf Exception
            for (int i = 0; i < _bullets.Count; i++)
            {
                _bullets[i].Update(timeSinceLastFrame, WorldWidth);
            }
        }

        public void UpdateEnemies(double timeSinceLastFrame)
        {
            for (int i = 0; i < _enemies.Count; i++)
            {
                _enemies[i].Update(timeSinceLastFrame, Player.PosX, Player.PosY);
            }
        }

        //public void UpdateBullets(double timeSinceLastFrame)
        //{
        //    var bulletsToRemove = new List<Bullet>();
        //    for (int i = 0; i < _bullets.Count; i++)
        //    {
        //        _bullets[i].UpdateBullet(timeSinceLastFrame);
        //        if (_bullets[i].PosX > this.WorldWidth) bulletsToRemove.Add(_bullets[i]);
        //    }


        //    foreach (var bullet in bulletsToRemove)
        //    {
        //        _bullets.Remove(bullet);
        //    }
        //}

        private double _worldWidth;
        public double WorldWidth
        {
            get { return _worldWidth; }
            set
            {
                _worldWidth = value;
                OnPropertyChanged();
            }
        }

        private double _worlHeight;
        public double WorldHeight
        {
            get { return _worlHeight; }
            set
            {
                _worlHeight = value;
                OnPropertyChanged();
            }
        }

    }
}
