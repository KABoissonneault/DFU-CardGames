using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using System;
using DaggerfallConnect;
using DaggerfallWorkshop;

namespace CardGamesMod
{
    public enum CardSuit
    {
        Hearts,
        Diamonds,
        Clubs,
        Spades,
    }

    public enum CardRank
    {
        Ace,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King,
        Joker,
    }

    public struct StandardCard
    {
        public CardSuit Suit;
        public CardRank Rank;
    }

    public static class CardGamesMethods
    {
        public static IEnumerable<StandardCard> GetStandardDeck()
        {
            foreach (CardSuit suit in Enum.GetValues(typeof(CardSuit)))
            {
                foreach (CardRank rank in Enum.GetValues(typeof(CardRank)))
                {
                    if (rank != CardRank.Joker)
                    {
                        yield return new StandardCard { Suit = suit, Rank = rank };
                    }
                }
            }

            // For Jokers, we only care about "red" or "black"
            yield return new StandardCard { Suit = CardSuit.Hearts, Rank = CardRank.Joker };
            yield return new StandardCard { Suit = CardSuit.Spades, Rank = CardRank.Joker };
        }

        public static bool IsRed(this CardSuit suit)
        {
            return suit == CardSuit.Hearts || suit == CardSuit.Diamonds;
        }

        public static bool IsBlack(this CardSuit suit)
        {
            return suit == CardSuit.Spades || suit == CardSuit.Clubs;
        }
    }

    public class CardGamesMod : MonoBehaviour
    {
        private static Mod mod;
        private static CardGamesMod instance;
        static GameObject modObject;

        CardGame cardGame;
        CardGameWindow gameWindow;

        public static Mod Mod { get { return mod; } }

        [Invoke(StateManager.StateTypes.Start, 0)]
        public static void Init(InitParams initParams)
        {
            mod = initParams.Mod;

            modObject = new GameObject(mod.Title);
            instance = modObject.AddComponent<CardGamesMod>();
            instance.cardGame = modObject.AddComponent<CardGame>();
            instance.cardGame.enabled = false;
            modObject.AddComponent<CardGameController>();

            // Tables
            PlayerActivate.RegisterCustomActivation(mod, 41108, OnTableActivated);
            PlayerActivate.RegisterCustomActivation(mod, 41109, OnTableActivated);
            PlayerActivate.RegisterCustomActivation(mod, 41110, OnTableActivated);
            PlayerActivate.RegisterCustomActivation(mod, 41111, OnTableActivated);
            PlayerActivate.RegisterCustomActivation(mod, 41112, OnTableActivated);
            PlayerActivate.RegisterCustomActivation(mod, 41130, OnTableActivated);
            PlayerActivate.RegisterCustomActivation(mod, 51103, OnTableActivated);
            PlayerActivate.RegisterCustomActivation(mod, 51104, OnTableActivated);

            mod.IsReady = true;
        }

        private void Start()
        {
            
        }

        static void OnTableActivated(RaycastHit hit)
        {
            if (instance.cardGame.enabled)
                return;

            var PlayerEnterExit = GameManager.Instance.PlayerEnterExit;
            if (PlayerEnterExit.IsPlayerInsideBuilding && PlayerEnterExit­.BuildingType == DFLocation.BuildingTypes.Tavern)
            {
                instance.cardGame.enabled = true;
                
                instance.gameWindow = new CardGameWindow(DaggerfallUI.UIManager, instance.cardGame);
                instance.gameWindow.OnClose += GameWindow_OnClose;
                DaggerfallUI.UIManager.PushWindow(instance.gameWindow);
            }
        }

        private static void GameWindow_OnClose()
        {
            instance.cardGame.enabled = false;
        }
    }
}
