using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace YukoWarai
{
    // 音声再生のためのインターフェースの作成
    public interface IMediaPlayer
    {
        Task PlayAsync(string title);
        void Stop();
        bool NowPlaying();
    }

    public partial class MainPage : ContentPage
    {
        private Image _banner;          // バナーイメージ
        private ExImage _stay;          // 静止時イメージ（タップ対応）
        private Image[] _anime;         // 笑い時イメージ

        private bool laughing;          // 今笑っているよフラグ
        private int flame = 0;          // 笑いアニメフレーム番号

        public MainPage()
        {
            int i;                      // 有象無象

            // おおもとの初期化
            InitializeComponent();
            laughing = false;

            // グリッドへのアクセス準備
            Grid grid;
            grid = g;

            // バナーの表示
            _banner = new Image();
            _banner.Source = ImageSource.FromResource("YukoWarai.Image.Banner.png");
            grid.Children.Add(_banner, 1, 0);

            // 静止時イメージの格納
            _stay = new ExImage();
            _stay.Source = ImageSource.FromResource("YukoWarai.Image.Stay.png");
            _stay.IsVisible = true;
            grid.Children.Add(_stay, 1, 1);

            // 笑い時イメージの格納
            _anime = new Image[4];
            for(i=0; i<4; i++) {
                _anime[i] = new Image();
            }
            _anime[0].Source = ImageSource.FromResource("YukoWarai.Image.Laughing1.png");
            _anime[1].Source = ImageSource.FromResource("YukoWarai.Image.Laughing2.png");
            _anime[2].Source = ImageSource.FromResource("YukoWarai.Image.Laughing3.png");
            _anime[3].Source = ImageSource.FromResource("YukoWarai.Image.Stay.png");
            for(i=0; i<4; i++) {
                grid.Children.Add(_anime[i], 1, 1);
                _anime[i].IsVisible = false;
            }

            // タッチイベントの実装
            _stay.Down += (sender, a) => {
                laughing = true;
                DependencyService.Get<IMediaPlayer>().Stop();
                DependencyService.Get<IMediaPlayer>().PlayAsync("warai1");
            };

            // タイマー処理
            Device.StartTimer(TimeSpan.FromMilliseconds(200), () => {
                if (laughing == false) {
                    // 笑ってなきゃ処理しない
                    return true;
                } else if (DependencyService.Get<IMediaPlayer>().NowPlaying() == true) {
                    // 笑ってて再生中ならアニメ処理
                    _anime[flame].IsVisible = false;
                    flame = (flame + 1) % 4;
                    _anime[flame].IsVisible = true;
                } else {
                    // 笑ってて再生終わりなら全て終了
                    _anime[flame].IsVisible = false;
                    _stay.IsVisible = true;
                    DependencyService.Get<IMediaPlayer>().Stop();
                    laughing = false;
                }
                return true;
            });

            // 開始の効果音
            DependencyService.Get<IMediaPlayer>().PlayAsync("yukodesu");
        }
    }
}
