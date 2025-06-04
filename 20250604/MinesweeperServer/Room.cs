using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace MinesweeperServer
{
    public enum PlayerState
    {
        Alive,
        Clear,
        Dead
    }

    public enum MatchResult
    {
        NotDecided,
        Decided,
        Draw
    }

    class Room
    {
        readonly int[] MINE_COUNT = { 10, 40, 99 };
        readonly int[,] GAMEPAN = { { 9, 9 }, { 16, 16 }, { 30, 16 } };

        public int RoomId { get; private set; }
        public int Level { get; private set; }

        public Player Player1 { get; private set; }
        public Player Player2 { get; private set; }

        public List<Point> MineMap { get; private set; }
        public Point FirstClick { get; private set; }

        public int Winner { get; private set; } = -1;
        public int Loser { get; private set; } = -1;

        public Player FirstTurnPlayer;

        private Random rand = new Random();

        public bool Player1_Reset { get; set; } = false;
        public bool Player2_Reset { get; set; } = false;

        public Room(int roomId, Player p1, Player p2, int level)
        {
            RoomId = roomId;
            Level = level;

            Player1 = p1;
            Player2 = p2;

            MineMap = new List<Point>();
        }

        public void InitGame()
        {
            SetFirstClickPoint();
            SetRandomMinePosition();

            Player1.Reset();
            Player2.Reset();
            Winner = -1;
            Loser = -1;
            FirstTurnPlayer = RandomTurn();
            Player1_Reset = false;
            Player2_Reset = false;
        }

        private Player RandomTurn()
        {
            int r = rand.Next(10);
            if (r % 2 == 0) return Player1;
            else return Player2;

        }
        private void SetFirstClickPoint()
        {
            int n = rand.Next(GAMEPAN[Level, 0] * GAMEPAN[Level, 1]);
            int y = n / GAMEPAN[Level, 0];
            int x = n % GAMEPAN[Level, 0];
            FirstClick = new Point(x, y);
        }

        private void SetRandomMinePosition()
        {
            MineMap.Clear();
            int mineCount = MINE_COUNT[Level];
            int width = GAMEPAN[Level, 0];
            int height = GAMEPAN[Level, 1];

            for (int i = 0; i < mineCount;)
            {
                int n = rand.Next(width * height);
                int y = n / width;
                int x = n % width;
                Point pt = new Point(x, y);

                if (FirstClick.Equals(pt)) continue;
                if (MineMap.Contains(pt)) continue;

                MineMap.Add(pt);
                i++;
            }
        }

        public MatchResult UpdatePlayerState(Player player, PlayerState state)
        {
            if (player == null)
                throw new ArgumentException("Unknown player");

            player.State = state;
            player.HasClicked = true;

            if (Player1.HasClicked && Player2.HasClicked)
            {
                MatchResult result = JudgeMatch(Player1.State, Player2.State);
                Player1.HasClicked = false;
                Player2.HasClicked = false;
                return result;
            }

            return MatchResult.NotDecided;
        }

        private MatchResult JudgeMatch(PlayerState p1State, PlayerState p2State)
        {
            if (p1State == PlayerState.Clear && p2State == PlayerState.Clear)
            {
                if (Player1.ClearTime == Player2.ClearTime)
                {
                    return MatchResult.Draw;
                }
                else if (Player1.ClearTime < Player2.ClearTime)
                {
                    Winner = Player1.Id;
                    Loser = Player2.Id;
                }
                else
                {
                    Winner = Player2.Id;
                    Loser = Player1.Id;
                }
                return MatchResult.Decided;
            }

            if (p1State == PlayerState.Clear && p2State != PlayerState.Clear)
            {
                Winner = Player1.Id;
                Loser = Player2.Id;
                return MatchResult.Decided;
            }

            if (p2State == PlayerState.Clear && p1State != PlayerState.Clear)
            {
                Winner = Player2.Id;
                Loser = Player1.Id;
                return MatchResult.Decided;
            }

            if (p1State == PlayerState.Dead && p2State != PlayerState.Dead)
            {
                Winner = Player2.Id;
                Loser = Player1.Id;
                return MatchResult.Decided;
            }

            if (p2State == PlayerState.Dead && p1State != PlayerState.Dead)
            {
                Winner = Player1.Id;
                Loser = Player2.Id;
                return MatchResult.Decided;
            }

            return MatchResult.NotDecided;
        }

        public int GetOppPlayer(int playerId)
        {
            if (Player1.Id == playerId) return Player2.Id;
            if (Player2.Id == playerId) return Player1.Id;
            throw new ArgumentException("Unknown playerId");
        }

        public string MineConvertString()
        {
            var sb = new System.Text.StringBuilder();
            foreach (Point p in MineMap)
            {
                sb.Append($"{p.X}*{p.Y} ");
            }
            return sb.ToString().Trim();
        }

        private Player GetPlayerById(int id)
        {
            if (Player1.Id == id) return Player1;
            if (Player2.Id == id) return Player2;
            return null;
        }

        void Parsing_ClearTime(string data, out int time)
        {
            string[] parts = data.Split(' ');

            if (parts.Length > 1 && int.TryParse(parts[1], out time))
            {

            }
            else
            {
                time = -1;
                Console.WriteLine("숫자 추출 실패");
            }
        }

        public void SetResetRequest(Player p)
        {
            if ( p.Id == Player1.Id)
            {
                Player1_Reset = true;
            }
            else
            {
                Player2_Reset = true;
            }
        }

        public bool CanResetGame()
        {
            return Player1_Reset && Player2_Reset;
        }
    }
}
