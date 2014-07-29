﻿using HearthstoneMemorySearchCLR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HearthstoneBot
{
    public class PlayTracker : Singleton<PlayTracker>
    {
        public enum GameState
        {
            Mulliganing,
            MyTurn,
            OpponentTurn,
            Idle,

            NotInitialized
        }

        private HearthstoneMemorySearchWrapper searcher = new HearthstoneMemorySearchWrapper();
        private List<CardWrapper> lastTurnHand = null;
        private List<CardWrapper> handShouldBe = null;

        public int Mana
        {
            get;
            set;
        }

        public int MaxMana
        {
            get;
            set;
        }

        public GameState State
        {
            get;
            set;
        }

        public GameCards Cards
        {
            get;
            set;
        }

        public PlayTracker()
        {
            this.State = GameState.NotInitialized;
            this.Mana = 0;
            this.MaxMana = 0;
        }

        public void Update()
        {
            List<CardWrapper> cards = searcher.GetCardList();

            this.Cards = new GameCards();
            this.Cards.Init(cards, this.handShouldBe);

            // No cards, must be idle
            if (this.Cards.PlayerHero == null)
            {
                this.State = GameState.Idle;
                return;
            }

            // We make assummptions that we are in the idle start when the app starts
            if(this.State == GameState.NotInitialized)
            {
                throw new Exception("Start Hearthstone-Bot before entering a game!");
                this.State = GameState.Idle;
            }

            if(this.State == GameState.Idle)
            {
                // Figure out if we have the starting turn or not
                if(this.Cards.PlayerZonedCards[(int)GameCards.Zones.HAND].FirstOrDefault(c => c.Name == "The Coin") == null)
                {
                    this.State = GameState.MyTurn;
                    this.MaxMana = 1;
                    this.Mana = 1;
                }
                else
                {
                    this.State = GameState.OpponentTurn;
                    this.lastTurnHand = this.Cards.PlayerZonedCards[(int)GameCards.Zones.HAND];
                }
            }

            if(this.State == GameState.OpponentTurn)
            {
                if(this.lastTurnHand.Count < this.Cards.PlayerZonedCards[(int)GameCards.Zones.HAND].Count)
                {
                    this.State = GameState.MyTurn;
                    this.MaxMana++;
                    this.Mana = this.MaxMana;
                }
            }
        }

        public void CardPlayedFromHand(int zonePos)
        {
            this.handShouldBe = new List<CardWrapper>();

            foreach(CardWrapper card in this.Cards.PlayerZonedCards[(int)GameCards.Zones.HAND])
            {
                CardWrapper newCard = new CardWrapper();

                newCard.CardId = card.CardId;
                newCard.Id = card.Id;
                newCard.Name = card.Name;
                newCard.Zone = card.Zone;
                newCard.ZonePos = card.ZonePos;

                this.handShouldBe.Add(newCard);
            }

            this.handShouldBe.Sort((a, b) => a.ZonePos - b.ZonePos);

            foreach(CardWrapper card in this.handShouldBe)
            {
                if(card.ZonePos > zonePos)
                {
                    card.ZonePos--;
                }
            }
        }

        public void PassedTurn()
        {
            if(this.State != GameState.MyTurn)
            {
                throw new Exception("Passing turn when it isn't mine!");
            }
            else
            {
                this.State = GameState.OpponentTurn;
                this.lastTurnHand = this.Cards.PlayerZonedCards[(int)GameCards.Zones.HAND];
            }
        }
    }
}
