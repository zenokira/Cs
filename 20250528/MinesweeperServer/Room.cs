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
        GameOver
    }

    public enum MatchResult
    {
        NoResultYet,
        WinForPlayer1,
        WinForPlayer2,
        Draw
    }

    class Room
    {
        readonly int[] MINE_COUNT = { 10, 40, 99 };
        readonly int[,] GAMEPAN = { { 9, 9 }, { 16, 16 }, { 30, 16 } };

        public int RoomId { get; set; }
        public int Player1_Id { get; set; }
        public int Player2_Id { get; set; }
        public int Level { get; set; }

        public List<Point> MineMap { get; private set; }
        public Point FirstClick { get; private set; }

        private Dictionary<int, PlayerState> playerStates = new Dictionary<int, PlayerState>();
        private Dictionary<int, bool> playerClicked = new Dictionary<int, bool>();

        private Random rand = new Random();

        public Room(int roomId, int p1, int p2, int level)
        {
            RoomId = roomId;
            Player1_Id = p1;
            Player2_Id = p2;
            Level = level;

            MineMap = new List<Point>();

            // 초기 상태는 Alive, 클릭 안한 상태
            playerStates[Player1_Id] = PlayerState.Alive;
            playerStates[Player2_Id] = PlayerState.Alive;
            playerClicked[Player1_Id] = false;
            playerClicked[Player2_Id] = false;
        }

        public void InitGame()
        {
            SetFirstClickPoint();
            SetRandomMinePosition();

            // 게임 초기화 시 상태도 초기화
            playerStates[Player1_Id] = PlayerState.Alive;
            playerStates[Player2_Id] = PlayerState.Alive;
            playerClicked[Player1_Id] = false;
            playerClicked[Player2_Id] = false;
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

        // 플레이어 상태 업데이트 및 턴 종료 체크, 승부 판정까지 수행
        // 승부 판정 결과를 반환 (NoResultYet: 아직 판정 못함)
        public MatchResult UpdatePlayerState(int playerId, PlayerState state)
        {
            if (!playerStates.ContainsKey(playerId) || !playerClicked.ContainsKey(playerId))
                throw new ArgumentException("Unknown playerId");

            playerStates[playerId] = state;
            playerClicked[playerId] = true;

            // 양쪽 다 클릭(턴 종료) 했는지 확인
            if (playerClicked.Values.All(clicked => clicked))
            {
                MatchResult result = JudgeMatch(playerStates[Player1_Id], playerStates[Player2_Id]);

                // 다음 턴을 위해 클릭 상태 초기화
                var keys = playerClicked.Keys.ToList();
                foreach (var key in keys)
                {
                    playerClicked[key] = false;
                }

                return result;
            }

            return MatchResult.NoResultYet;
        }

        private MatchResult JudgeMatch(PlayerState p1State, PlayerState p2State)
        {
            if (p1State == PlayerState.Clear && p2State == PlayerState.Clear)
                return MatchResult.Draw;

            if (p1State == PlayerState.Clear && p2State != PlayerState.Clear)
                return MatchResult.WinForPlayer1;

            if (p2State == PlayerState.Clear && p1State != PlayerState.Clear)
                return MatchResult.WinForPlayer2;

            if (p1State == PlayerState.GameOver && p2State != PlayerState.GameOver)
                return MatchResult.WinForPlayer2;

            if (p2State == PlayerState.GameOver && p1State != PlayerState.GameOver)
                return MatchResult.WinForPlayer1;

            // 둘 다 살아있거나 아직 판정 불가 상태
            return MatchResult.NoResultYet;
        }

        public int GetOppPlayer(int playerId)
        {
            return playerId == Player1_Id ? Player2_Id : Player1_Id;
        }

        // 마인 위치 문자열 변환 (예: x*y x*y ...)
        public string MineConvertString()
        {
            var sb = new System.Text.StringBuilder();
            foreach (Point p in MineMap)
            {
                sb.Append($"{p.X}*{p.Y} ");
            }
            return sb.ToString().Trim();
        }
    }
}
