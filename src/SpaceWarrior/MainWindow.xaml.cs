using System.Threading;
using System.Windows.Threading;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SpaceWarrior.ViewModels;
using SpaceWarrior.Entities;

namespace SpaceWarrior
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;
        private readonly CancellationTokenSource _cts;
        private readonly CancellationToken _ct;
        private Task _listenerTask;
        private readonly BitmapImage _bulletImg;
        private bool _runKeyListener;
        // http://stackoverflow.com/questions/19261840/how-to-achieve-a-background-image-continuous-move-in-wpf

        public MainWindow()
        {
            InitializeComponent();
            _bulletImg = new BitmapImage(new Uri(@"Images\bullet.png", UriKind.Relative));
            _runKeyListener = true;

            this.WindowStyle = WindowStyle.None;
            this.WindowState = WindowState.Maximized;

            _viewModel = new MainViewModel(
                // playerPosX
                0.0,
                // PlayerPosY
                (this.Height / 2) - (this.PlayerModelHitBox.Height / 2),
                // playerWidth
                PlayerModelHitBox.Width,
                // playerHeight
                PlayerModelHitBox.Height,
                // worldWith
                this.Width,
                // worldHeight
                this.Height,
                // playerSpeedX
                600.0,
                // playerSpeedY
                600.0,
                // movePlayerX
                (d) => Dispatcher.Invoke(() => Canvas.SetLeft(PlayerModelHitBox, d)),
                // movePayerY
                (d) => Dispatcher.Invoke(() => Canvas.SetTop(PlayerModelHitBox, d)),
                // bulletWidth
                _bulletImg.Width,
                // bulletHeigth
                _bulletImg.Height,
                // factory method for creating enemies
                GetEnemy);

            _cts = new CancellationTokenSource();
            _ct = _cts.Token;

            this.KeyDown += MainWindow_KeyDown;
            DataContext = _viewModel;

            _viewModel.RunWorker();
            RunKeyListener();
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                var menuCtrl = new MenuWindow(
                    menuWindow =>
                    {
                        // TODO: sauber beenden --> TaskCancelation
                        Application.Current.Shutdown();
                    },
                    menuWindow => menuWindow.Close()
                    );
                menuCtrl.ShowDialog();
            }
        }

        // Littel helper
        private Tuple<Action<double>, Action<double>, Action> GetNewBulletModelMoveActions()
        {
            var bullet = new Rectangle
            {
                Fill = new ImageBrush(_bulletImg),
                Height = _bulletImg.Height,
                Width = _bulletImg.Width
            };


            MainCanvas.Children.Add(bullet);
            Canvas.SetZIndex(bullet, 0);
            //Canvas.SetLeft(bullet, Canvas.GetLeft(PlayerModelHitBox));
            //Canvas.SetTop(bullet, Canvas.GetTop(PlayerModelHitBox));

            return new Tuple<Action<double>, Action<double>, Action>(
                d => Dispatcher.Invoke(() => Canvas.SetLeft(bullet, d)),
                d => Dispatcher.Invoke(() => Canvas.SetTop(bullet, d)),
                () => Dispatcher.Invoke(() => MainCanvas.Children.Remove(bullet)));
        }

        // Little helper
        private Enemy GetEnemy(double posX, double posY, double speedX, double speedY, int maxHits)
        {
            Rectangle enemy = null;
            BitmapImage enemyImage = null;
            double enemyHight = 0;
            double enemyWidth = 0;

            Dispatcher.Invoke(() => enemyImage = new BitmapImage(new Uri(@"Images\enemy.png", UriKind.Relative)));
            Dispatcher.Invoke(() => enemyHight = enemyImage.Height);
            Dispatcher.Invoke(() => enemyWidth = enemyImage.Width);

            Dispatcher.Invoke(() =>
            {
                enemy = new Rectangle
                {
                    Fill = new ImageBrush(enemyImage),
                    Height = enemyHight,
                    Width = enemyWidth
                };
            });

            Dispatcher.Invoke(() => MainCanvas.Children.Add(enemy));
            Dispatcher.Invoke(() => Canvas.SetZIndex(enemy, 0));

            Enemy enemyModel = null;
            Dispatcher.Invoke(() =>
            {
                enemyModel = new Enemy(
                    posX,
                    posY,
                    enemyImage.Width,
                    enemyImage.Height,
                    speedX,
                    speedY,
                    d => Dispatcher.Invoke(() => Canvas.SetLeft(enemy, d)),
                    d => Dispatcher.Invoke(() => Canvas.SetTop(enemy, d)),
                    () => Dispatcher.Invoke(() => MainCanvas.Children.Remove(enemy)),
                    maxHits);
            });

            return enemyModel;
        }

        public void RunKeyListener()
        {
            _listenerTask =
                new Task(() =>
                {
                    while (_runKeyListener)
                    {
                        try
                        {
                            Dispatcher.Invoke(() =>
                            {
                                _viewModel.Up = Keyboard.IsKeyDown(Key.W);
                                _viewModel.Left = Keyboard.IsKeyDown(Key.A);
                                _viewModel.Down = Keyboard.IsKeyDown(Key.S);
                                _viewModel.Right = Keyboard.IsKeyDown(Key.D);

                                /* Check if the objects (eg. Bullets) are correctyl get removed */
                                var c =
                                    MainCanvas.Children.OfType<Rectangle>()
                                        .Count();
                                inf.Content = c + " objects on Sceen";

                                if (!Keyboard.IsKeyDown(Key.Space)) return;

                                if (_viewModel.CanAddBullet)
                                {
                                    var bulletActions = GetNewBulletModelMoveActions();

                                    _viewModel.AddBulletIfPossible(bulletActions.Item1, bulletActions.Item2, bulletActions.Item3);
                                }
                            });
                        }
                        catch (TaskCanceledException)
                        {
                            // Cancelation Requested
                        }

                        Thread.Sleep(10);
                    }
                }, _ct);

            _listenerTask.Start();
        }

        private void WorldSizeChanged(object sender, SizeChangedEventArgs e)
        {
            _viewModel.WorldHeight = MainCanvas.ActualHeight;
            _viewModel.WorldWidth = MainCanvas.ActualWidth;
        }

        private void GameClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _runKeyListener = false;
            _cts.Cancel();

            try
            {
                _listenerTask.Wait(_ct);
            }
            catch (OperationCanceledException)
            {
                // _litenerTask abgebrochen
            }

            try
            {
                _viewModel.StopWorker();
            }
            catch (OperationCanceledException)
            {
                // TODO: implement better way to quit the game
            }

        }



    }
}
