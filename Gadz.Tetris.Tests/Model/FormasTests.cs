﻿using Gadz.Tetris.Model.Pieces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Gadz.Tetris.Model
{
    /// <summary>
    /// Defines the <see cref="FormasTests" />
    /// </summary>
    [TestClass]
    public class FormasTests
    {
        private PieceBuilder _pieceBuilder;

        [TestInitialize]
        public void Setup()
        {
            var game = GameController.Create(10, 10);
            _pieceBuilder = new PieceBuilder()
                .OnBoard(game.Board);
        }

        /// <summary>
        /// The DeveDesenharFormaI
        /// </summary>
        [TestMethod]
        public void DeveDesenharFormaI()
        {
            var piece = _pieceBuilder.OfType(PieceType.I).Build();
            Assert.AreEqual(4, piece.Blocks.Count());
            Assert.AreEqual("0+0,0+1,0+2,0+3", piece.ToString());
        }

        /// <summary>
        /// The DeveDesenharFormaJ
        /// </summary>
        [TestMethod]
        public void DeveDesenharFormaJ()
        {
            var forma = _pieceBuilder.OfType(PieceType.J).Build();
            Assert.AreEqual(4, forma.Blocks.Count());
            Assert.AreEqual("1+0,1+1,1+2,0+2", forma.ToString());
        }

        /// <summary>
        /// The DeveDesenharFormaL
        /// </summary>
        [TestMethod]
        public void DeveDesenharFormaL()
        {
            var forma = _pieceBuilder.OfType(PieceType.L).Build();
            Assert.AreEqual("0+0,0+1,0+2,1+2", forma.ToString());
        }

        /// <summary>
        /// The DeveDesenharFormaO
        /// </summary>
        [TestMethod]
        public void DeveDesenharFormaO()
        {
            var piece = _pieceBuilder.OfType(PieceType.O).Build();
            Assert.AreEqual(4, piece.Blocks.Count());
            Assert.AreEqual("0+0,0+1,1+0,1+1", piece.ToString());
        }

        /// <summary>
        /// The DeveDesenharFormaS
        /// </summary>
        [TestMethod]
        public void DeveDesenharFormaS()
        {
            var piece = _pieceBuilder.OfType(PieceType.S).Build();
            Assert.AreEqual("0+0,0+1,1+1,1+2", piece.ToString());
        }

        /// <summary>
        /// The DeveDesenharFormaT
        /// </summary>
        [TestMethod]
        public void DeveDesenharFormaT()
        {
            var piece = _pieceBuilder.OfType(PieceType.T).Build();
            Assert.AreEqual(4, piece.Blocks.Count());
            Assert.AreEqual("0+0,0+1,0+2,1+1", piece.ToString());
        }

        /// <summary>
        /// The DeveDesenharFormaZ
        /// </summary>
        [TestMethod]
        public void DeveDesenharFormaZ()
        {
            var piece = _pieceBuilder.OfType(PieceType.Z).Build();
            Assert.AreEqual(4, piece.Blocks.Count());
            Assert.AreEqual("1+0,1+1,0+1,0+2", piece.ToString());
        }

        /// <summary>
        /// The NaoDeveDesenharNenhumaForma
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void NaoDeveDesenharNenhumaForma()
        {
            _pieceBuilder.OfType((PieceType)8).Build();
        }
    }
}
