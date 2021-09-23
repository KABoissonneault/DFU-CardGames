using UnityEngine;

namespace CardGamesMod
{
    [RequireComponent(typeof(CardGame))]
    class CardGameController : MonoBehaviour
    {
        const float KeyRepeatDelay = 1.0f;
        const float KeyRepeatPeriod = 0.25f;

        CardGame game;

        KeyCode heldKey;
        float keyRepeatBuffer;

        CardGameController()
        {

        }

        private void Start()
        {
            game = GetComponent<CardGame>();
        }

        private void Update()
        {
            if (!game.enabled)
                return;

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (game.Draw.GetCardCount() > 0)
                    game.Discard.AddCard(game.Draw.DrawCard());

                heldKey = KeyCode.RightArrow;
                keyRepeatBuffer = KeyRepeatDelay;

            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (game.Discard.GetCardCount() > 0)
                    game.Draw.AddCard(game.Discard.DrawCard());

                heldKey = KeyCode.LeftArrow;
                keyRepeatBuffer = KeyRepeatDelay;
            }
            else if(Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (game.Draw.GetCardCount() > 0)
                    game.SouthHand.AddCard(game.Draw.DrawCard());

                heldKey = KeyCode.DownArrow;
                keyRepeatBuffer = KeyRepeatDelay;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (game.SouthHand.GetCardCount() > 0)
                    game.Discard.AddCard(game.SouthHand.RemoveCard(game.SouthHand.GetCardCount() - 1));

                heldKey = KeyCode.UpArrow;
                keyRepeatBuffer = KeyRepeatDelay;
            }

            if (heldKey != KeyCode.None)
            {
                keyRepeatBuffer -= Time.unscaledDeltaTime;
                if(keyRepeatBuffer <= 0.0f)
                {
                    switch(heldKey)
                    {
                        case KeyCode.RightArrow:
                            if (game.Draw.GetCardCount() > 0)
                                game.Discard.AddCard(game.Draw.DrawCard());
                            break;

                        case KeyCode.LeftArrow:
                            if (game.Discard.GetCardCount() > 0)
                                game.Draw.AddCard(game.Discard.DrawCard());
                            break;

                        case KeyCode.DownArrow:
                            if (game.Draw.GetCardCount() > 0)
                                game.SouthHand.AddCard(game.Draw.DrawCard());
                            break;

                        case KeyCode.UpArrow:
                            if (game.SouthHand.GetCardCount() > 0)
                                game.Discard.AddCard(game.SouthHand.RemoveCard(game.SouthHand.GetCardCount() - 1));
                            break;
                    }

                    keyRepeatBuffer += KeyRepeatPeriod;
                }
            }

            if(!Input.GetKey(heldKey))
            {
                heldKey = KeyCode.None;
            }
        }
    }
}
