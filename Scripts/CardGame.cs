using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

using static CardGamesMod.CardGamesMethods;

namespace CardGamesMod
{
    class CardEventArgs : EventArgs
    {
        public IEnumerable<StandardCard> Cards;
    }

    class CardStack
    {
        List<StandardCard> stack = new List<StandardCard>();

        public EventHandler OnCardAdded;
        public EventHandler OnCardRemoved;

        public void AddCard(in StandardCard card)
        {
            stack.Add(card);
            OnCardAdded?.Invoke(this, new CardEventArgs { Cards = new StandardCard[1] { card } });
        }

        public void AddCards(IEnumerable<StandardCard> cards)
        {
            stack.AddRange(cards);
            OnCardAdded?.Invoke(this, new CardEventArgs { Cards = cards });
        }

        public StandardCard DrawCard()
        {
            StandardCard card = GetTopCard();
            stack.RemoveAt(stack.Count - 1);
            OnCardRemoved?.Invoke(this, new CardEventArgs { Cards = new StandardCard[1] { card } });
            return card;
        }

        public IEnumerable<StandardCard> DrawCards(int count)
        {
            if (stack.Count < count)
                throw new Exception("Overdrew stack");

            List<StandardCard> result = new List<StandardCard>(stack.Skip(stack.Count - count));
            stack.RemoveRange(stack.Count - count, count);
            OnCardRemoved?.Invoke(this, new CardEventArgs { Cards = result });
            return result;
        }

        public StandardCard GetCard(int i)
        {
            return stack[i];
        }

        public StandardCard GetTopCard()
        {
            return stack[stack.Count - 1];
        }

        public int GetCardCount()
        {
            return stack.Count;
        }
    }

    class CardHand
    {
        List<StandardCard> hand = new List<StandardCard>();

        public EventHandler OnCardAdded;
        public EventHandler OnCardRemoved;

        public void AddCard(StandardCard card)
        {
            hand.Add(card);
            OnCardAdded?.Invoke(this, new CardEventArgs { Cards = new StandardCard[1] { card } });
        }

        public void AddCards(IEnumerable<StandardCard> cards)
        {
            hand.AddRange(cards);
            OnCardAdded?.Invoke(this, new CardEventArgs { Cards = cards });
        }

        public StandardCard GetCard(int i)
        {
            return hand[i];
        }

        public int GetCardCount()
        {
            return hand.Count;
        }

        public StandardCard RemoveCard(int i)
        {
            StandardCard card = hand[i];
            hand.RemoveAt(i);
            return card;
        }
    }

    class CardGame : MonoBehaviour
    {
        CardStack draw = new CardStack();
        CardStack discard = new CardStack();

        CardHand northHand = new CardHand();
        CardHand westHand = new CardHand();
        CardHand southHand = new CardHand();
        CardHand eastHand = new CardHand();

        public CardStack Draw { get { return draw; } }
        public CardStack Discard { get { return discard; } }
        public CardHand NorthHand { get { return northHand; } }
        public CardHand WestHand { get { return westHand; } }
        public CardHand SouthHand { get { return southHand; } }
        public CardHand EastHand { get { return eastHand; } }

        public void Start()
        {
            System.Random rng = new System.Random();
            draw.AddCards(GetStandardDeck().Concat(GetStandardDeck()).OrderBy(_ => rng.Next()));
        }

        public void Update()
        {
            
        }
    }
}
