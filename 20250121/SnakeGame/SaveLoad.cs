using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace SnakeGame
{
    // 뱀게임에서 저장할 내용
    // 머리, 몸통, 먹이, 장애물, 레벨 점수 몸통길이

    class SaveLoad
    {
        const string fileName = "save.json";


        public void Save(Snake snake, Obstacle obstacle, Feed feed, int[] state)
        {
            List<Point> snake_lbl_List = ExtractionPointToLabel(snake.GetSnakeAllLabelList());
            List<Point> obstacle_lbl_List = ExtractionPointToLabel(obstacle.getObstacleList());
            List<Point> feed_lbl_List = ExtractionPointToLabel(feed.getFeedList());

            using (Stream ws = new FileStream(fileName, FileMode.Create))
            {
                string jsonString = $"{snake_lbl_List.Count}\n"
                    + JsonSerializer.Serialize<List<Point>>(snake_lbl_List) + "\n"
                    + $"{obstacle_lbl_List.Count}\n"
                    + JsonSerializer.Serialize<List<Point>>(obstacle_lbl_List) + "\n"
                    + $"{feed_lbl_List.Count}\n"
                    + JsonSerializer.Serialize<List<Point>>(feed_lbl_List) + "\n"
                    + $"{state[0]}\n" + $"{state[1]}\n" + $"{state[2]}\n";


                byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonString);
                ws.Write(jsonBytes, 0, jsonBytes.Length);
            }
        }

        public List<Point> ExtractionPointToLabel(List<Label> list)
        {
            List<Point> points = new List<Point>();
            foreach (Label label in list)
            {
                points.Add(label.Location);
            }
            return points;
        }

        public void Load(out List<Point> snake, out List<Point> obstacle, out List<Point> feed, out int[] state)
        {
            using (Stream rs = new FileStream(fileName, FileMode.Open))
            {
                byte[] jsonBytes = new byte[rs.Length];
                rs.Read(jsonBytes, 0, jsonBytes.Length);
                string jsonString = System.Text.Encoding.UTF8.GetString(jsonBytes);

                List<Point>[] points = new List<Point>[3];
                int[] counts = new int[3];

                string[] str = jsonString.Split('\n');
                for (int i = 0; i < 3; i++)
                {
                    counts[i] = Convert.ToInt32(str[i * 2]);
                    var v = JsonSerializer.Deserialize<List<Point>>(str[i * 2 + 1]);
                    points[i] = (List<Point>)v;
                }

                snake = points[0]; obstacle = points[1]; feed = points[2];
                state = new int[3];
                for(int i = 0; i < 3; i++)
                    state[i] = Convert.ToInt32(str[6+i]);
            }
        }
    }
}
