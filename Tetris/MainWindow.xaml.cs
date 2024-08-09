using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Tetris
{
    public partial class MainWindow : Window
    {
        private readonly ImageSource[] tileImages = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/TileEmpty.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileCyan.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileBlue.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileOrange.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileYellow.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileGreen.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TilePurple.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileRed.png", UriKind.Relative))
        };

        private readonly ImageSource[] blockImages = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/Block-Empty.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-I.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-J.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-L.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-O.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-S.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-T.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-Z.png", UriKind.Relative))
        };

        public static bool flag = false;
        public Image[,] imageControls;
        public int maxDelay = 1000;
        public int minDelay = 75;
        public int delayDecrease = 25;
        public Records records = new Records();
        private GameState gameState = new GameState();

        public MainWindow()
        {
            InitializeComponent();
            imageControls = SetupGameCanvas(gameState.GameGrid);
        }

        public Image[,] SetupGameCanvas(GameGrid grid)
        {
            Image[,] imageControls = new Image[grid.Rows, grid.Columns];
            int cellSize = 25;
            for (int r = 0; r < grid.Rows; r++)
            {
                for (int c = 0; c < grid.Columns; c++)
                {
                    Image imageControl = new Image
                    {
                        Width = cellSize,
                        Height = cellSize
                    };

                    Canvas.SetTop(imageControl, (r - 2) * cellSize + 10);
                    Canvas.SetLeft(imageControl, c * cellSize);
                    GameCanvas.Children.Add(imageControl);
                    imageControls[r, c] = imageControl;
                }
            }

            return imageControls;
        }

        public void DrawGrid(GameGrid grid)
        {
            for (int r = 0; r < grid.Rows; r++)
            {
                for (int c = 0; c < grid.Columns; c++)
                {
                    int id = grid[r, c];
                    imageControls[r, c].Opacity = 1;
                    imageControls[r, c].Source = tileImages[id];
                }
            }
        }

        public void DrawBlock(Block block)
        {
            foreach (Position p in block.TilePositions())
            {
                imageControls[p.Row, p.Column].Opacity = 1;
                imageControls[p.Row, p.Column].Source = tileImages[block.Id];
            }
        }

        public void DrawNextBlock(BlockQueue blockQueue)
        {
            Block next = blockQueue.NextBlock;
            NextImage.Source = blockImages[next.Id];
        }



        public void DrawGhostBlock(Block block)
        {
            int dropDistance = gameState.BlockDropDistance();

            foreach (Position p in block.TilePositions())
            {
                imageControls[p.Row + dropDistance, p.Column].Opacity = 0.25;
                imageControls[p.Row + dropDistance, p.Column].Source = tileImages[block.Id];
            }
        }

        public void Draw(GameState gameState)
        {

            DrawGrid(gameState.GameGrid);
            DrawGhostBlock(gameState.CurrentBlock);
            DrawBlock(gameState.CurrentBlock);
            DrawNextBlock(gameState.BlockQueue);
            ScoreText.Text = $"Счёт: {gameState.Score}";
        }

        private async Task GameLoop()
        {
            Draw(gameState);
            while (!gameState.GameOver)
            {
                int delay = Math.Max(minDelay, maxDelay - (gameState.Score * delayDecrease));
                await Task.Delay(delay);
                if (flag == true)
                gameState.MoveBlockDown();
                Draw(gameState);
            }
            GameOverMenu.Visibility = Visibility.Visible;
            FinalScoreText.Text = $"Счёт: {gameState.Score}";
            records.saveScore(gameState.Score);
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameState.GameOver)
            {
                return;
            }
            if (flag == true)
            {
                switch (e.Key)
                {
                    case Key.Left:
                        gameState.MoveBlockLeft();
                        break;
                    case Key.Right:
                        gameState.MoveBlockRight();
                        break;
                    case Key.Down:
                        gameState.MoveBlockDown();
                        break;
                    case Key.Up:
                        gameState.RotateBlockCW();
                        break;
                    case Key.Z:
                        gameState.RotateBlockCCW();
                        break;
                    case Key.Space:
                        gameState.DropBlock();
                        break;
                    case Key.Enter:
                        {
                            flag = false;
                            PauseMod.Visibility = Visibility.Visible;
                            break;
                        }
                    default:
                        return;
                }
            }

            Draw(gameState);
        }

        private async void GameCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            await GameLoop();
        }

        private async void PlayAgain_Click(object sender, RoutedEventArgs e)
        {
            gameState = new GameState();
            GameOverMenu.Visibility = Visibility.Hidden;
            await GameLoop();
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            flag = true;
            GameOverMenu.Visibility = Visibility.Hidden;
            MainMenu.Visibility = Visibility.Hidden;
            pause.Visibility = Visibility.Visible;
        }

        private void Records_Click(object sender, RoutedEventArgs e)
        {
            records.getRecords();
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            flag= false;
            PauseMod.Visibility = Visibility.Visible;
        }

        private void Resume_Click(object sender, RoutedEventArgs e)
        {
            flag = true;
            PauseMod.Visibility = Visibility.Hidden;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            gameState = new GameState();
            flag = false;
            PauseMod.Visibility = Visibility.Hidden;
            MainMenu.Visibility = Visibility.Visible;
            pause.Visibility = Visibility.Hidden;
        }

        private void mainExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            Helping.Visibility= Visibility.Visible;
            help.Visibility= Visibility.Visible;
        }

        private void help_Click_1(object sender, RoutedEventArgs e)
        {
            Helping.Visibility = Visibility.Hidden;
            help.Visibility = Visibility.Hidden;
        }
    }
}
