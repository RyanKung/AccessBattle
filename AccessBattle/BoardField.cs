﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessBattle
{
    /// <summary>
    /// Represents a field on the board
    /// </summary>
    public class BoardField : PropChangeNotifier
    {
        Card _card;
        /// <summary>
        /// Card that was placed on this field.
        /// </summary>
        public Card Card
        {
            get { return _card; }
            set { SetProp(ref _card, value); }
        }

        /// <summary>X Location of this field.</summary>
        public ushort X { get; private set; }
        /// <summary>Y Location of this field.</summary>
        public ushort Y { get; private set; }


        /// <summary>Checks the coordinates to determine if this field in an server area field.</summary>
        public bool IsServerArea => Y == 10 && (X == 4 || X == 5);
        /// <summary>Checks the coordinates to determine if this field in an exit field.</summary>
        public bool IsExit => X >= 3 && X <= 4 && (Y == 0 || Y == 7);
        /// <summary>Checks the coordinates to determine if this field in a stack field.</summary>
        public bool IsStack => Y >= 8 && Y <= 9;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x">X Location of this field.</param>
        /// <param name="y">Y Location of this field.</param>
        public BoardField(ushort x, ushort y)
        {
            X = x;
            Y = y;
        }

        public void Update(Sync sync, PlayerState[] players)
        {
            // Do not check the coordinates! They might require transformation for player 2!
            Card card;
            if (sync.Card.IsFirewall)
            {
                card = new FirewallCard
                {
                    Owner = sync.Card.Owner == 1 ? players[0] : players[1]
                };
            }
            else
            {
                card = new OnlineCard
                {
                    HasBoost = sync.Card.HasBoost,
                    IsFaceUp = sync.Card.IsFaceUp,
                    Owner = sync.Card.Owner == 1 ? players[0] : players[1],
                    Type = sync.Card.Type
                };
            }
            Card = card;
        }

        public Sync GetSync()
        {
            return new Sync
            {
                Card = Card?.GetSync(),
                X = X,
                Y = Y,
            };
        }

        public class Sync
        {
            public Card.Sync Card;
            public ushort X, Y;
        }
    }
}
