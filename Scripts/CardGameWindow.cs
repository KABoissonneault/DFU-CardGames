using UnityEngine;

using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using System;

namespace CardGamesMod
{
    class CardGameWindow : DaggerfallBaseWindow
    {
        const int CardPixelWidth = 35;
        const int CardPixelHeight = 47;
        const float CardTextureU = 0.0655430712f;
        const float CardTextureV = 0.25f;

        const float CardStackScale = 0.5f;

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
                return CardPixelWidth + CardPixelWidth * diff;
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
            return 1 - ((float)(GetCardPixelY(card) + CardPixelHeight) / baseCardTexture.height);
        }

        static readonly Rect FacedownUVRect = new Rect(0, 0.75f, CardTextureU, CardTextureV);

        Rect GetCardUVRect(StandardCard card)
        {
            var currentCardU = GetCardTextureU(card);
            var currentCardV = GetCardTextureV(card);

            return new Rect(currentCardU, currentCardV, CardTextureU, CardTextureV);
        }

        public void DrawCardAt(Vector2 position, Rect UVRect, float scale = 1.0f)
        {
            GUI.DrawTextureWithTexCoords(
                       new Rect(position.x * NativePanel.LocalScale.x, position.y * NativePanel.LocalScale.y
                       , CardPixelWidth * NativePanel.LocalScale.x * scale, CardPixelHeight * NativePanel.LocalScale.y * scale)
                       , baseCardTexture
                       , UVRect
                       );
        }

        public void DrawFacedownStackAt(CardStack stack, Vector2 basePosition, float cardDistance = 0.0f)
        {
            if (stack.GetCardCount() == 0)
                return;

            if(cardDistance == 0.0f)
            {
                DrawCardAt(basePosition, FacedownUVRect, CardStackScale);
            }
            else
            {
                float distanceBuffer = 0.0f;
                float lastDistance = 0.0f;

                DrawCardAt(basePosition, FacedownUVRect, CardStackScale);

                for(int i = 1; i < stack.GetCardCount(); ++i)
                {
                    distanceBuffer += cardDistance * CardStackScale;

                    float flooredBuffer = Mathf.Floor(distanceBuffer);
                    if (flooredBuffer > lastDistance)
                    {
                        var currentPosition = basePosition;
                        currentPosition.y -= flooredBuffer;

                        DrawCardAt(currentPosition, FacedownUVRect, CardStackScale);

                        lastDistance = flooredBuffer;
                    }
                }
            }
        }

        public void DrawFaceupStackAt(CardStack stack, Vector2 basePosition, float cardDistance = 0.0f, float maxDistance = 0.0f)
        {
            if (stack.GetCardCount() == 0)
                return;

            if (cardDistance == 0.0f)
            {
                DrawCardAt(basePosition, GetCardUVRect(stack.GetTopCard()), CardStackScale);
            }
            else
            {
                float distanceBuffer = 0.0f;
                float verticalOffset = 0.0f;

                for (int i = 0; i < stack.GetCardCount(); ++i)
                {
                    float flooredBuffer = Mathf.Floor(distanceBuffer);
                    var currentPosition = basePosition;
                    currentPosition.x += flooredBuffer;
                    currentPosition.y += verticalOffset;

                    DrawCardAt(currentPosition, GetCardUVRect(stack.GetCard(i)), CardStackScale);

                    distanceBuffer += cardDistance * CardStackScale;

                    if(maxDistance != 0.0f && distanceBuffer >= maxDistance)
                    {
                        distanceBuffer = 0.0f;
                        verticalOffset += CardPixelHeight * 0.8f * CardStackScale;
                    }
                }
            }
        }

        public override void Draw()
        {
            base.Draw();

            const int cardDistance = 32;

            DrawFacedownStackAt(game.Draw,
                new Vector2(
                    (int)NativePanel.Size.x / 2
                    , (int)NativePanel.Size.y / 2 - cardDistance / 2 - CardPixelHeight / 2
                ),
                0.1f
            );

            DrawFaceupStackAt(game.Discard,
                new Vector2(
                    3 * (int)NativePanel.Size.x / 8
                    , (int)NativePanel.Size.y / 2 + cardDistance / 2 - CardPixelHeight / 2
                ),
                12.0f,
                NativePanel.Size.x / 4
            );
        }
    }
}
