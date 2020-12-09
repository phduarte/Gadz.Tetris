﻿namespace Gadz.Tetris.Model
{
    /// <summary>
    /// TetraminoConfiguration
    /// </summary>
    public struct TetraminoConfiguration
    {
        public PieceType Type;
        public Point Position;
        public int Rotation;
        public PieceColor Color => Piece.GetPieceColor(Type);
    }
}
