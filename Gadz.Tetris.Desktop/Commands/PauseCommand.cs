﻿using System.Windows.Forms;

namespace Gadz.Tetris.Desktop.Commands
{
    internal class PauseCommand : Command
    {
        public PauseCommand(GameController game) 
            : base(game)
        {
        }

        public override Keys Key => Keys.Enter;

        public override bool Control => false;

        public override void Execute() => _gameController.Pause();

        public override bool Match(Keys key, bool control) => key == Key && _gameController.Playing;
    }
}