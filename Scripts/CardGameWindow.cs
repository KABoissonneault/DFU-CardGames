using UnityEngine;

using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using System;

namespace CardGamesMod
{
    class CardGameWindow : DaggerfallBaseWindow
    {
        const int cardPixelWidth = 35;
        const int cardPixelHeight = 47;
        const float cardTextureU = 0.0655430712f;
        const float cardTextureV = 0.25f;

        CardGame game;

        Texture2D baseCardTexture;

        public CardGameWindow(IUserInterfaceManager uiManager, CardGame inGame)
            : base(uiManager)
        {
            game = inGame;
        }

        protected override void Setup()
        {
            NativePanel.BackgroundColor = new Color(0.2f, 0.4f, 0.3f);

            baseCardTexture = CardGamesMod.Mod.GetAsset<Texture2D>("8BitDeckAssets");
        }

        int GetCardPixelX(StandardCard card)
        {
            if (card.Rank >= CardRank.Two && card.Rank <= CardRank.King)
            {
                int diff = (int)card.Rank - (int)CardRank.Two;
                return cardPixelWidth + cardPixelWidth * diff;
            }
            else if (card.Rank == CardRank.Ace)
            {
                return 455;
            }
            else if (card.Rank == CardRank.Joker)
            {
                return 0;
            }

            throw new Exception("Not expected");
        }

        int GetCardPixelY(StandardCard card)
        {
            if (card.Rank == CardRank.Joker)
            {
                if(card.Suit.IsRed())
                {
                    return 94;
                }
                else
                {
                    return 141;
                }
            }

            switch(card.Suit)
            {
                case CardSuit.Hearts:
                    return 0;

                case CardSuit.Clubs:
                    return 47;

                case CardSuit.Diamonds:
                    return 94;

                case CardSuit.Spades:
                    return 141;
            }

            throw new Exception("Not expected");
        }

        float GetCardTextureU(StandardCard card)
        {
            return (float)GetCardPixelX(card) / baseCardTexture.width;
        }

        float GetCardTextureV(StandardCard card)
        {
            return 1 - ((float)(GetCardPixelY(card) + cardPixelHeight) / baseCardTexture.height);
        }

        public override void Draw()
        {
            base.Draw();

            const int cardDistance = 70;
            if (game.Draw.GetCardCount() > 0)
            {
                int positionX = (int)NativePanel.Size.x / 2 - cardDistance / 2 - cardPixelWidth / 2;
                int positionY = (int)NativePanel.Size.y / 2 - cardPixelHeight / 2;

                GUI.DrawTextureWithTexCoords(
                    new Rect(positionX * NativePanel.LocalScale.x, positionY * NativePanel.LocalScale.y
                    , cardPixelWidth * NativePanel.LocalScale.x, cardPixelHeight * NativePanel.LocalScale.y)
                    , baseCardTexture
                    , new Rect(0, 0.75f, cardTextureU, cardTextureV)
                    );
            }

            if(game.Discard.GetCardCount() > 0)
            {
                int positionX = (int)NativePanel.Size.x / 2 + cardDistance / 2 - cardPixelWidth / 2;
                int positionY = (int)NativePanel.Size.y / 2 - cardPixelHeight / 2;

                var topCard = game.Discard.GetTopCard();
                var currentCardU = GetCardTextureU(topCard);
                var currentCardV = GetCardTextureV(topCard);

                GUI.DrawTextureWithTexCoords(
                    new Rect(positionX * NativePanel.LocalScale.x, positionY * NativePanel.LocalScale.y,
                    cardPixelWidth * NativePanel.LocalScale.x, cardPixelHeight * NativePanel.LocalScale.y)
                    , baseCardTexture
                    , new Rect(currentCardU, currentCardV, cardTextureU, cardTextureV)
                    );
            }
        }
    }
}
