using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

using static CardGamesMod.CardGamesMethods;

namespace CardGamesMod
{
    class CardEventArgs : EventArgs
    {
        public StandardCard Card;
    }

    class CardStack
    {
        List<StandardCard> stack = new List<StandardCard>();

        public EventHandler OnCardAdded;
        public EventHandler OnCardRemoved;

        public CardStack(IEnumerable<StandardCard> InitialCards)
        {
            stack.AddRange(InitialCards);
        }

        public void AddCard(in StandardCard card)
        {
            stack.Add(card);
            OnCardAdded?.Invoke(this, new CardEventArgs { Card = card });
        }

        public StandardCard DrawCard()
        {
            StandardCard card = GetTopCard();
            stack.RemoveAt(stack.Count - 1);
            OnCardRemoved?.Invoke(this, new CardEventArgs { Card = card });
            return card;
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

    class CardGame : MonoBehaviour
    {
        CardStack draw = new CardStack(GetStandardDeck().Concat(GetStandardDeck()));
        CardStack discard = new CardStack(Enumerable.Empty<StandardCard>());

        public CardStack Draw { get { return draw; } }
        public CardStack Discard { get { return discard; } }

        public void Start()
        {

        }

        public void Update()
        {
            if(Input.GetKeyDown(KeyCode.RightArrow))
            {
                if(draw.GetCardCount() > 0)
                    discard.AddCard(draw.DrawCard());

            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (discard.GetCardCount() > 0)
                    draw.AddCard(discard.DrawCard());
            }
        }
    }
}
