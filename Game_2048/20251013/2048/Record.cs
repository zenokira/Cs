using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace _2048
{
    public partial class Record : Form
    {
        public Record()
        {
            InitializeComponent();
        }

        private void Record_Load(object sender, EventArgs e)
        {
            Init_ListView();
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

            // 🔹 폼 중앙 배치
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        public System.Windows.Forms.ListView GetListView()
        {
            return lv_record;
        }
    }
    
}
