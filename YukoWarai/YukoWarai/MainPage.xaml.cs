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
        private int laughtime = 0;      // 笑う回数
        private int flame = 0;          // 笑いアニメフレーム番号

        private int laughkoe = 0;       // 笑い声の番号（0～3）
        private string[] koe = new string[] { "warai1", "warai2", "warai3", "warai4" };

        // 乱数発生用変数
        System.Random r = new System.Random();

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
                DependencyService.Get<IMediaPlayer>().Stop();
                laughing = true;
                laughtime = 4;
            };

            // タイマー処理
            Device.StartTimer(TimeSpan.FromMilliseconds(250), () => {
                if (laughing == false) {
                    // 笑ってなきゃ処理しない
                    return true;
                } else {
                    if (DependencyService.Get<IMediaPlayer>().NowPlaying() == true) {
                        // 笑ってて再生中ならアニメ処理
                        _anime[flame].IsVisible = false;
                        flame = (flame + 1) % 4;
                        _anime[flame].IsVisible = true;
                    } else {
                        // 笑ってて再生終わりなら
                        DependencyService.Get<IMediaPlayer>().Stop();
                        if (laughtime > 1) {
                            // まだ２回以上笑うなら別の声で笑い始める
                            laughkoe = (laughkoe + r.Next(1, 4)) % 4;
                            DependencyService.Get<IMediaPlayer>().PlayAsync(koe[laughkoe]);
                            laughtime -= 1;
                        } else {
                            if (laughtime == 1) {
                                // 最後の一回なら「へぇ」と言う
                                DependencyService.Get<IMediaPlayer>().PlayAsync("waraiend");
                                laughtime -= 1;
                            } else {
                                // 笑い終わりなら全部終了
                                _anime[flame].IsVisible = false;
                                _stay.IsVisible = true;
                                laughing = false;
                            }
                        }
                    }
                }
                return true;
            });

            // 開始の「ユウコです！」
            DependencyService.Get<IMediaPlayer>().PlayAsync("yukodesu");
        }
    }
}
