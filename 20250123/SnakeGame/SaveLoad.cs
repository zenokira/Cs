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

        List<Point>[] lbl_List = new List<Point>[3];
        List<Point> useList = new List<Point>();
        int[] state = new int[4]; // LV Length LVPoint Jumsu
        SnakeVector snakeVector = SnakeVector.LEFT;

        bool flag;

        public SaveLoad()
        {
            flag = false;
        }
        public SaveLoad(List<Point> snake, List<Point> obstacle, List<Point> feed, List<Point> usepoint, int lv, int length, int lvpoint, int jumsu, SnakeVector vector)
        {
            lbl_List[0] = snake;
            lbl_List[1] = obstacle;
            lbl_List[2] = feed;
            useList = usepoint;
            this.state = new int[4]{ lv, length, lvpoint, jumsu};
            this.snakeVector = vector;
        }

        public string getFileName()
        {
            return fileName;
        }

        public List<Point> getSnakeList()
        {
            return lbl_List[0];
        }
        public List<Point> getObstacleList()
        {
            return lbl_List[1];
        }
        public List<Point> getFeedList()
        {
            return lbl_List[2];
        }

        public int[] getState()
        {
            return state;
        }
        public void Save()
        {
            using (Stream ws = new FileStream(fileName, FileMode.Create))
            {
                string jsonString =JsonSerializer.Serialize<List<Point>>(lbl_List[0]) + "\n"
                    + JsonSerializer.Serialize<List<Point>>(lbl_List[1]) + "\n"
                    + JsonSerializer.Serialize<List<Point>>(lbl_List[2]) + "\n"
                    + JsonSerializer.Serialize<List<Point>>(useList) + "\n"
                    + $"{state[0]}\n" + $"{state[1]}\n" + $"{state[2]}\n" + $"{state[3]}\n"
                    + $"{snakeVector}";


                byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonString);
                ws.Write(jsonBytes, 0, jsonBytes.Length);
                ws.Close();
            }
        }

        public void Load()
        {  
            using (Stream rs = new FileStream(fileName, FileMode.Open))
            {
                byte[] jsonBytes = new byte[rs.Length];
                rs.Read(jsonBytes, 0, jsonBytes.Length);
                string jsonString = System.Text.Encoding.UTF8.GetString(jsonBytes);

                string[] str = jsonString.Split('\n');

                for(int i = 0; i < 3; i++)
                {
                    var imsiv = JsonSerializer.Deserialize<List<Point>>(str[i]);
                    lbl_List[i] = (List<Point>)imsiv;
                }

                var v = JsonSerializer.Deserialize<List<Point>>(str[3]);
                useList = (List<Point>) v;


                for (int i = 0; i < 4; i++)
                    state[i] = Convert.ToInt32(str[4+i]);

                if (str[8].Equals("UP"))
                    snakeVector = SnakeVector.UP;
                else if (str[8].Equals("DOWN"))
                    snakeVector = SnakeVector.DOWN;
                else if (str[8].Equals("LEFT"))
                    snakeVector = SnakeVector.LEFT;
                else if (str[8].Equals("RIGHT"))
                    snakeVector = SnakeVector.RIGHT;
  
                flag = true;
                rs.Close();
            }
        }

        public SnakeVector GetSnakeVector()
        {
            return snakeVector;
        }
        public int getLV()
        {
            return state[0];
        }
        public int getLength()
        {
            return state[1];
        }
        public int getLVPoint()
        {
            return state[2];
        }

        public List<Point> getUseList()
        {
            return useList;
        }
        public int getJumsu()
        {
            return state[3];
        }
    }
}
