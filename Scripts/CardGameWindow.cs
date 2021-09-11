using UnityEngine;

using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using System;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game;

namespace CardGamesMod
{
    class CardGameWindow : DaggerfallBaseWindow
    {
        const int cardPixelWidth = 35;
        const int cardPixelHeight = 47;


        CardGame game;

        Panel drawPanel;
        Panel discardPanel;

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

            const int cardDistance = 70;

            {
                int positionX = (int)NativePanel.Size.x / 2 - cardDistance / 2 - cardPixelWidth / 2;
                int positionY = (int)NativePanel.Size.y / 2 - cardPixelHeight / 2;

                drawPanel = DaggerfallUI.AddPanel(
                    new Rect
                    {
                        position = new Vector2(positionX, positionY),
                        size = new Vector2(cardPixelWidth, cardPixelHeight)
                    },
                    NativePanel
                    );

                if (game.Draw.GetCardCount() > 0)
                {
                    drawPanel.BackgroundTexture = ImageReader.GetSubTexture(baseCardTexture, new Rect { position = new Vector2 { x = 0, y = 0 }, size = new Vector2 { x = cardPixelWidth, y = cardPixelHeight } });
                }
                else
                {
                    drawPanel.Enabled = false;
                }

                game.Draw.OnCardAdded += OnCardDraw;
                game.Draw.OnCardRemoved += OnCardDraw;
            }

            {
                int positionX = (int)NativePanel.Size.x / 2 + cardDistance / 2 - cardPixelWidth / 2;
                int positionY = (int)NativePanel.Size.y / 2 - cardPixelHeight / 2;

                discardPanel = DaggerfallUI.AddPanel(
                    new Rect
                    {
                        position = new Vector2(positionX, positionY),
                        size = new Vector2(cardPixelWidth, cardPixelHeight)
                    },
                    NativePanel
                    );

                if (game.Discard.GetCardCount() > 0)
                {
                    UpdateCardTexture(discardPanel, game.Discard.GetTopCard());
                }
                else
                {
                    discardPanel.Enabled = false;
                }
                game.Discard.OnCardAdded += OnCardDraw;
                game.Discard.OnCardRemoved += OnCardDraw;
            }
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

        void OnCardDraw(object stack, EventArgs e)
        {
            var cardStack = stack as CardStack;
            if (cardStack == game.Draw)
            {
                if (cardStack.GetCardCount() == 0)
                {
                    drawPanel.Enabled = false;
                }
                else
                {
                    drawPanel.Enabled = true;
                }
            }
            else if(cardStack == game.Discard)
            {
                if (cardStack.GetCardCount() == 0)
                {
                    discardPanel.Enabled = false;
                }
                else
                {
                    discardPanel.Enabled = true;
                    UpdateCardTexture(discardPanel, cardStack.GetTopCard());
                }
            }
        }

        void UpdateCardTexture(Panel panel, StandardCard card)
        {
            panel.BackgroundTexture = ImageReader.GetSubTexture(baseCardTexture,
                new Rect { position = new Vector2 { x = GetCardPixelX(card), y = GetCardPixelY(card) }, size = new Vector2 { x = cardPixelWidth, y = cardPixelHeight } }
                );
        }
    }
}
