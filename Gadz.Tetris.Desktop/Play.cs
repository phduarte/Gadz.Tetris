﻿using Gadz.Tetris.Core.DomainModel.Pecas;
using Gadz.Tetris.Core.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Texto = Gadz.Tetris.Core.CrossCutting.Texto.Jogo;

namespace Gadz.Tetris.Desktop {
    public partial class Play : Form {

        #region fields

        Form _baseForm;
        static int _index = 0;
        readonly TaskScheduler _threadPrincipal;
        readonly GameController _controller;
        const int BLOCK_SIZE = 22;
        const string BLOCK_PREFIX = "block";

        #endregion

        public Play(Form formBase) {

            _baseForm = formBase;
            _threadPrincipal = TaskScheduler.FromCurrentSynchronizationContext();
            _controller = GameController.Create(10, 20);

            InitializeComponent();
            SetScreenText();

            if (!Program.ClassicMode) {
                mainBoardPanel.BackgroundImage = Properties.Resources.BACKGROUND_TETRIS;
                BackColor = Color.White;
            }

            Start();
        }
        
        void Start() {
            ListenEvents();
            DrawScreen();
            PaintScreen();
            PlayStartSound();
        }

        void SetScreenText() {
            label2.Text = Texto.Pontuacao.ToUpper();
            label3.Text = Texto.Tempo.ToUpper();
            label4.Text = Texto.Linhas.ToUpper();
            label5.Text = Texto.Nivel.ToUpper();
            label6.Text = Texto.Velocidade.ToUpper();
            label8.Text = Texto.Proximo.ToUpper();
            Text = Texto.Nome.ToUpper();
        }

        void PlayStartSound() {
            Program.SoundPlayer.Start();
        }

        void ListenEvents() {
            _controller.Start();
            _controller.OnRefresh += PaintScreen;
            _controller.OnRefresh += UpdateScreenTextAsync;
            _controller.OnEnd += ExitAsync;

            _controller.OnClear += PaintScreen;
            _controller.OnClear += Program.SoundPlayer.Clear;

            _controller.OnMove += Program.SoundPlayer.Move;
            _controller.OnSlide += Program.SoundPlayer.Slide;
        }

        async void UpdateScreenTextAsync() {
            await Task.Factory.StartNew(() => {
                lbLevel.Text = _controller.Level.ToString();
                lbPoints.Text = _controller.Score.ToString();
                lbTime.Text = _controller.Time.ToString(@"hh\:mm\:ss");
                lbLines.Text = _controller.Lines.ToString();
                lbSpeed.Text = _controller.Speed.ToString();
            }, CancellationToken.None, TaskCreationOptions.None, _threadPrincipal);
        }

        async void ExitAsync() {
            await Task.Factory.StartNew(() => {
                Program.SoundPlayer.End();
                Hide();
                new GameOver().ShowDialog();
                Close();
            }, CancellationToken.None, TaskCreationOptions.None, _threadPrincipal);
        }

        void DrawScreen() {

            mainBoardPanel.Controls.Clear();
            mainBoardPanel.Width = _controller.BoardWidth * BLOCK_SIZE;
            mainBoardPanel.Height = _controller.BoardHeight * BLOCK_SIZE;

            for (int y = 0; y < _controller.BoardHeight; y++) {
                for (int x = 0; x < _controller.BoardWidth; x++) {
                    mainBoardPanel.Controls.Add(CreateBlock(x, y, string.Empty));
                }
            }

            nextBlockPanel.Controls.Clear();

            for (int y = 0; y < 4; y++) {
                for (int x = 0; x < 4; x++) {
                    nextBlockPanel.Controls.Add(CreateBlock(x, y, string.Empty));
                }
            }
        }

        void PaintScreen() {
            PaintBoardAsync();
            PaintNextPieceAsync();
        }

        async void PaintNextPieceAsync() {
            await Task.Factory.StartNew(() => {
                for (int y = 0; y < 4; y++) {
                    for (int x = 0; x < 4; x++) {
                        PaintBlock(x, y, string.Empty, nextBlockPanel);
                    }
                }

                PaintBlock(_controller.GetNextBlocks(), nextBlockPanel);
            });
        }

        async void PaintBoardAsync() {
            await Task.Factory.StartNew(() => {
                PaintBlock(_controller.GetActualBlocks(), mainBoardPanel);
                for (int y = 0; y < _controller.BoardHeight; y++) {
                    for (int x = 0; x < _controller.BoardWidth; x++) {
                        PaintBlock(x, y, _controller.Matrix[x, y].Cor.ToString(), mainBoardPanel);
                    }
                }
            }, CancellationToken.None, TaskCreationOptions.None, _threadPrincipal);
        }

        static PictureBox CreateBlock(int x, int y, string cor) {
            var block = new PictureBox {
                Location = new Point(x * BLOCK_SIZE, y * BLOCK_SIZE),
                BackgroundImage = GetBackgroundImage(cor),
                BackgroundImageLayout = ImageLayout.Stretch,
                Name = BLOCK_PREFIX + (_index++).ToString(),
                Size = new Size(BLOCK_SIZE, BLOCK_SIZE),
                BorderStyle = BorderStyle.None,
                TabIndex = 0,
                TabStop = false
            };

            return block;
        }

        static Image GetBackgroundImage(string cor) {

            if (!(cor == "TRANSPARENTE" || cor == string.Empty) && Program.ClassicMode) return Properties.Resources.BLOCK_CLASSIC;
            if (cor == "AMARELO") return Properties.Resources.BLOCK_YELLOW;
            if (cor == "VERMELHO") return Properties.Resources.BLOCK_RED;
            if (cor == "ROXO") return Properties.Resources.BLOCK_PURPLE;
            if (cor == "VERDE") return Properties.Resources.BLOCK_GREEN;
            if (cor == "LARANJA") return Properties.Resources.BLOCK_ORANGE;
            if (cor == "AZUL") return Properties.Resources.BLOCK_BLUE;
            if (cor == "CIANO") return Properties.Resources.BLOCK_CYAN;

            if (Program.ClassicMode) return Properties.Resources.BLOCK_CLASSIC_FADED;

            return null;
        }

        void PaintBlock(IEnumerable<Bloco> blocos, Panel panel) {
            foreach (var bloco in blocos) {
                PaintBlock(bloco.X, bloco.Y, bloco.Cor.ToString(), panel);
            }
        }

        void PaintBlock(int x, int y, string color, Panel panel) {
            foreach (Control i in panel.Controls) {
                if (i.Name.StartsWith(BLOCK_PREFIX)
                    && i.Location.X == x * BLOCK_SIZE
                    && i.Location.Y == y * BLOCK_SIZE) {
                    i.BackColor = Color.Transparent;
                    i.BackgroundImage = GetBackgroundImage(color);
                    break;
                }
            }
        }

        private void Jogo_KeyDown(object sender, KeyEventArgs e) {
            switch (e.KeyCode) {
                case Keys.Down:

                    if (e.Control)
                        _controller.SmashDown();
                    else
                        _controller.MoveDown();

                    break;

                case Keys.Left:

                    if (e.Control)
                        _controller.RunLeft();
                    else
                        _controller.MoveLeft();

                    break;
                case Keys.Right:

                    if (e.Control)
                        _controller.RunRight();
                    else
                        _controller.MoveRight();

                    break;

                case Keys.Up:
                    _controller.Rotate();
                    break;

                case Keys.Escape:
                    _controller.Exit();
                    break;

                case Keys.Enter:
                    if (_controller.Playing)
                        _controller.Pause();
                    else
                        _controller.Continue();
                    break;

                case Keys.ShiftKey:
                    Program.SoundPlayer.ToggleMute();
                    break;

                case Keys.Space:
                    _controller.SmashDown();
                    break;
            }
        }

        private void Jogo_FormClosed(object sender, FormClosedEventArgs e) {
            _baseForm.Show();
        }

        private void Jogo_Load(object sender, EventArgs e) {

            using (var pfc = new PrivateFontCollection()) {

                pfc.AddFontFile(@"Fonts\digital_counter_7.ttf");

                foreach (Control f in Controls) {
                    var actualFontSize = f.Font.Size;
                    f.Font = new Font(pfc.Families[0], actualFontSize, FontStyle.Regular);
                }
            }
        }
    }
}
