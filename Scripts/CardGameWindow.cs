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
        const float CardHandScale = 0.5f;

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

        public void DrawFacedownCardStackAt(int count, Vector2 basePosition, float cardDistance, float cardScale)
        {
            if (count == 0)
                return;

            if(cardDistance == 0.0f)
            {
                DrawCardAt(basePosition, FacedownUVRect, cardScale);
            }
            else
            {
                float distanceBuffer = 0.0f;
                float lastDistance = 0.0f;

                DrawCardAt(basePosition, FacedownUVRect, cardScale);

                for(int i = 1; i < count; ++i)
                {
                    distanceBuffer += cardDistance * cardScale;

                    float flooredBuffer = Mathf.Floor(distanceBuffer);
                    if (flooredBuffer > lastDistance)
                    {
                        var currentPosition = basePosition;
                        currentPosition.y -= flooredBuffer;

                        DrawCardAt(currentPosition, FacedownUVRect, cardScale);

                        lastDistance = flooredBuffer;
                    }
                }
            }
        }

        public void DrawFacedownCardHandAt(int count, Vector2 basePosition, float cardDistance, float cardScale)
        {
            if (count == 0)
                return;

            if (count % 2 == 0)
            {
                float x_base_offset = (count / 2) * cardDistance * cardScale;

                for (int i = 0; i < count; ++i)
                {
                    var currentPosition = basePosition;
                    currentPosition.x -= x_base_offset;
                    currentPosition.x += i * cardDistance * cardScale;

                    DrawCardAt(currentPosition, FacedownUVRect, cardScale);
                }
            }
            else
            {
                float x_base_offset = (count / 2.0f) * cardDistance * cardScale;

                for (int i = 0; i < count; ++i)
                {
                    var currentPosition = basePosition;
                    currentPosition.x -= x_base_offset;
                    currentPosition.x += i * cardDistance * cardScale;

                    DrawCardAt(currentPosition, FacedownUVRect, cardScale);
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

        public void DrawFaceupHandAt(CardHand hand, Vector2 basePosition, float cardDistance)
        {
            const float HandScale = 0.8f;

            if (hand.GetCardCount() == 0)
                return;

            if(hand.GetCardCount() % 2 == 0)
            {
                float x_base_offset = (hand.GetCardCount() / 2) * cardDistance * HandScale;

                for(int i = 0; i < hand.GetCardCount(); ++i)
                {
                    var currentPosition = basePosition;
                    currentPosition.x -= x_base_offset;
                    currentPosition.x += i * cardDistance * HandScale;

                    DrawCardAt(currentPosition, GetCardUVRect(hand.GetCard(i)), HandScale);
                }
            }
            else
            {
                float x_base_offset = (hand.GetCardCount() / 2 + 0.5f) * cardDistance * HandScale;

                for (int i = 0; i < hand.GetCardCount(); ++i)
                {
                    var currentPosition = basePosition;
                    currentPosition.x -= x_base_offset;
                    currentPosition.x += i * cardDistance * HandScale;

                    DrawCardAt(currentPosition, GetCardUVRect(hand.GetCard(i)), HandScale);
                }
            }
        }

        public override void Draw()
        {
            base.Draw();

            const int cardDistance = 32;

            DrawFacedownCardStackAt(game.Draw.GetCardCount(),
                new Vector2(
                    NativePanel.Size.x / 2
                    , NativePanel.Size.y / 2 - cardDistance / 2 - CardPixelHeight / 2
                ),
                0.1f,
                CardStackScale
            );

            float DiscardLength = NativePanel.Size.x / 4 + (NativePanel.Size.x / 4) * Mathf.Min(game.Discard.GetCardCount() / 25.0f, 1.0f);

            DrawFaceupStackAt(game.Discard,
                new Vector2(
                    (NativePanel.Size.x - DiscardLength) / 2
                    , (int)NativePanel.Size.y / 2 + cardDistance / 2 - CardPixelHeight / 2
                ),
                12.0f,
                DiscardLength
            );

            DrawFaceupHandAt(game.SouthHand,
                new Vector2(
                    NativePanel.Size.x / 2
                    , NativePanel.Size.y - (NativePanel.Size.y /16.0f) - CardPixelHeight
                ),
                12.0f
            );

            DrawFacedownCardHandAt(game.NorthHand.GetCardCount(),
                new Vector2(
                    NativePanel.Size.x / 2
                    , NativePanel.Size.y / 16.0f
                ),
                12.0f,
                CardHandScale
                );
        }
    }
}
