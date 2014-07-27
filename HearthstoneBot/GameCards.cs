﻿using HearthstoneMemorySearchCLR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HearthstoneBot
{
    public class GameCards
    {
        public enum Zones
        {
            PLAY,
            HAND,
            GRAVEYARD,
            SETASIDE,
            DECK,

            COUNT
        }

        public List<CardWrapper>[] PlayerZonedCards = new List<CardWrapper>[(int)Zones.COUNT];
        public List<CardWrapper>[] OpponentZonedCards = new List<CardWrapper>[(int)Zones.COUNT];

        public CardWrapper PlayerHero
        {
            get
            {
                return PlayerZonedCards[(int)Zones.PLAY].First(c => c.Id == 4);
            }
        }
        public CardWrapper OpponentHero
        {
            get
            {
                return OpponentZonedCards[(int)Zones.PLAY].First(c => c.Id == 36);
            }
        }

        private void AddCardToZone(Zones zone, CardWrapper card)
        {
            if(card.Id <= 35)
            {
                this.PlayerZonedCards[(int)zone].Add(card);
            }
            else
            {
                this.OpponentZonedCards[(int)zone].Add(card);
            }
        }
        public void Init(List<CardWrapper> cards)
        {
            for (int i = (int)Zones.PLAY; i < (int)Zones.COUNT; ++i)
            {
                PlayerZonedCards[i] = new List<CardWrapper>();
                OpponentZonedCards[i] = new List<CardWrapper>();
            }

            cards.Sort((x, y) => { return x.ZonePos - y.ZonePos; });

            foreach (CardWrapper card in cards)
            {
                if (card.Zone == "PLAY")
                {
                    this.AddCardToZone(Zones.PLAY, card);
                }
                else if (card.Zone == "HAND")
                {
                    this.AddCardToZone(Zones.HAND, card);
                }
                else if (card.Zone == "GRAVEYARD")
                {
                    this.AddCardToZone(Zones.GRAVEYARD, card);
                }
                else if (card.Zone == "SETASIDE")
                {
                    this.AddCardToZone(Zones.SETASIDE, card);
                }
                else if (card.Zone == "DECK")
                {
                    this.AddCardToZone(Zones.DECK, card);
                }
                else
                {
                    throw new Exception("A NEW ZONE");
                }
            }
        }
    }
}
