

using System.Collections;

namespace _2048
{
    public partial class Record : Form
    {
        public UserStore userStore { get; private set; }
        public Record(UserStore store)
        {
            InitializeComponent();
            userStore = store;
            userStore.Changed += OnStoreChanged;
        }

        private void Record_Load(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.CenterParent;
            Init_ListView();
            UpdateListView();
        }

        private void Init_ListView()
        {
            lv_record.View = View.Details;          // 상세 보기 모드
            lv_record.FullRowSelect = true;         // 한 행 전체 선택
            lv_record.GridLines = true;             // 격자선 표시
            lv_record.MultiSelect = false;          // 하나만 선택
            lv_record.HideSelection = false;        // 포커스 잃어도 선택 유지

            // 기존 컬럼이 있으면 초기화
            lv_record.Columns.Clear();

            // 컬럼 추가
            lv_record.Columns.Add("순위", 60, HorizontalAlignment.Center);
            lv_record.Columns.Add("이름", 100, HorizontalAlignment.Center);
            lv_record.Columns.Add("움직인 횟수", 80, HorizontalAlignment.Center);

            // 🔹 ListView 전체 크기 맞추기
            int totalWidth = lv_record.Columns.Cast<ColumnHeader>().Sum(c => c.Width) + 10;
            lv_record.Width = totalWidth;

            // 🔹 폼 크기를 리스트뷰에 맞춰 조정
            this.ClientSize = new Size(lv_record.Width + 20, this.ClientSize.Height);
        }
        private void OnStoreChanged(object? sender, EventArgs e)
        {
            UpdateListView();
        }
        public ListView GetListView()
        {
            return lv_record;
        }

        public void ListView_Clear()
        {
            var lv = GetListView();
            lv.Items.Clear();
        }



        public int GetListViewCount() => lv_record.Items.Count;


        public void UpdateListView()
        {
            ListView_Clear();
            int rank = 1;
            int sameCount = 0;
            int? prevCount = null;

            foreach (var user in userStore)
            {
                int moves = int.TryParse(user.Moves, out int val) ? val : 0;

                if (prevCount is null)
                {
                    prevCount = moves;
                    sameCount = 1;
                }
                else if (prevCount == moves)
                {
                    sameCount++;
                }
                else
                {
                    rank += sameCount;   // 동점자 수만큼 순위 점프
                    sameCount = 1;
                    prevCount = moves;
                }

                var item = new ListViewItem(rank.ToString())
                {
                    UseItemStyleForSubItems = false
                };

                var nameSub = new ListViewItem.ListViewSubItem(item, user.Name);
                var countSub = new ListViewItem.ListViewSubItem(item, user.Moves);

                item.SubItems.Add(nameSub);
                item.SubItems.Add(countSub);
                lv_record.Items.Add(item);
            }
        }

    }
   
    
}
