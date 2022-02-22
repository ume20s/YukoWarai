using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace YukoWarai
{
    public class ExImage : Image
    {
        // 押下イベント取得のためにImageクラスを拡張
        public event EventHandler Down;
        public bool OnDown()
        {
            if (Down != null)
            {
                Down(this, new EventArgs());
            }
            return true;
        }
    }
}
