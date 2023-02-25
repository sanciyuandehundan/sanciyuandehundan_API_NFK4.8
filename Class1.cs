using System.Diagnostics;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using Microsoft.VisualBasic;
using static System.Net.WebRequestMethods;
using System.Reflection;
using System.Security.Policy;
using System.Drawing;

namespace sanciyuandehundan_API
{
    public class picture
    {
        /// <summary>
        /// 图片高
        /// </summary>
        public int picture_H;

        /// <summary>
        /// 图片宽
        /// </summary>
        public int picture_W;

        /// <summary>
        /// 图片坐标
        /// </summary>
        public Point picture_location;

        /// <summary>
        /// 根据窗口大小调整图片大小
        /// </summary>
        /// <param name="tuH"></param>
        /// <param name="tuW"></param>
        /// <param name="winH"></param>
        /// <param name="winW"></param>
        public void Tu(int tuH, int tuW, int winH, int winW)
        {
            double zany0 = new float();//最终结果
            double zany1 = new float();//form数据
            double zany2 = new float();//图片数据
            double zanx0 = new float();//最终结果
            double zanx1 = new float();//form数据
            double zanx2 = new float();//图片数据
            double bilix = new float();//屏幕比例宽
            double biliy = new float();//屏幕比例高
            bili(winH, winW);
            willreturn[1] = winW / willreturn[0];
            willreturn[0] = winH / willreturn[0];
            biliy = willreturn[0];
            bilix = willreturn[1];
            if (tuH / biliy > tuW / bilix)//高图
            {
                zany1 = winW;//将int转成double计算
                zany2 = tuW;//将int转成double计算
                zany0 = zany1 / zany2;//将int转成double计算
                picture_W = winW + 10;//增加一定大小，避免出现缝隙
                picture_H = (int)(tuH * zany0);/*缩小或放大了几倍*/
            }
            else//宽图
            {
                zanx1 = winH;
                zanx2 = tuH;
                zanx0 = zanx1 / zanx2;//将int转成double计算
                picture_H = winH + 10;//增加一定大小，避免出现缝隙
                picture_W = (int)(tuW * zanx0);/*缩小或放大了几倍*/
                //Y = 100;试错
                //X = 0;试错
            }
            picture_location = new Point(-(picture_W - winW) / 2, -((picture_H - winH) / 2));//更改图片位置，将图片置中
        }//设置图片大小与位置
        int[] willreturn = new int[] { 0, 0 };//高，宽
        public int[] bili(int a, int b)
        {
            if (a % b == 0)
            {
                willreturn[0] = a;//height高
                willreturn[1] = a;//width宽
                Debug.Print("高比" + willreturn[0].ToString());
                Debug.Print("宽比" + willreturn[1].ToString());
                return willreturn;
            }
            else
            {
                return bili(b, a % b);
            }
        }//计算屏幕比例
    }
    public class Midi
    {
        public int[] power = new int[16];//音量
        public int[] tempo_minute = new int[16];//一分钟几拍
        public float[] note_base = new float[16];//一拍是几分音符
        public int[] note_long = new int[16];//一拍几毫秒
        public int[] instrument = new int[16];//乐器
        public int[][,] music = new int[16][,];//最终乐谱（主）
        public bool[][] stop = new bool[16][];//和下一个音符间是否有连音线
        public int time=0;//曲子时长几毫秒

        /// <summary>
        /// 音符midi码
        /// </summary>
        public enum Music_note_collection
        {
            Rest = 0, C8 = 108, B7 = 107, A7s = 106, A7 = 105, G7s = 104, G7 = 103, F7s = 102, F7 = 101, E7 = 100,
            D7s = 99, D7 = 98, C7s = 97, C7 = 96, B6 = 95, A6s = 94, A6 = 93, G6s = 92, G6 = 91, F6s = 90, F6 = 89,
            E6 = 88, D6s = 87, D6 = 86, C6s = 85, C6 = 84, B5 = 83, A5s = 82, A5 = 81, G5s = 80, G5 = 79, F5s = 78,
            F5 = 77, E5 = 76, D5s = 75, D5 = 74, C5s = 73, C5 = 72, B4 = 71, A4s = 70, A4 = 69, G4s = 68, G4 = 67,
            F4s = 66, F4 = 65, E4 = 64, D4s = 63, D4 = 62, C4s = 61, C4 = 60, B3 = 59, A3s = 58, A3 = 57, G3s = 56,
            G3 = 55, F3s = 54, F3 = 53, E3 = 52, D3s = 51, D3 = 50, C3s = 49, C3 = 48, B2 = 47, A2s = 46, A2 = 45,
            G2s = 44, G2 = 43, F2s = 42, F2 = 41, E2 = 40, D2s = 39, D2 = 38, C2s = 37, C2 = 36, B1 = 35, A1s = 34,
            A1 = 33, G1s = 32, G1 = 31, F1s = 30, F1 = 29, E1 = 28, D1s = 27, D1 = 26, C1s = 25, C1 = 24, B0 = 23,
            A0s = 22, A0 = 21
        }
        /// <summary>
        /// 乐器表
        /// </summary>
        public enum Music_instrument_collection
        {
            AcousticGrandPiano = 1, BrightAcousticPiano = 2, ElectricGrand_Piano = 3, Honky_tonkPiano = 4, ElectricPiano_1 = 5, ElectricPiano_2 = 6, Harpsichord = 7,
            Clavinet = 8, Celesta = 9, Glockenspiel = 10, Musicalbox = 11, Vibraphone = 12, Marimba = 13, Xylophone = 14, TubularBell = 15, Dulcimer = 16,
            DrawbarOrgan = 17, PercussiveOrgan = 18, RockOrgan = 19, Churchorgan = 20, Reedorgan = 21, Accordion = 22, Harmonica = 23, TangoAccordion = 24, AcousticGuitar_nylon = 25,
            AcousticGuitar_steel = 26, ElectricGuitar_jazz = 27, ElectricGuitar_clean = 28, ElectricGuitar_muted = 29, OverdrivenGuitar = 30, DistortionGuitar = 31, Guitarharmonics = 32,
            AcousticBass = 33, ElectricBass_finger = 34, ElectricBass_pick = 35, FretlessBass = 36, SlapBass_1 = 37, SlapBass_2 = 38, SynthBass_1 = 39, SynthBass_2 = 40, Violin = 41, Viola = 42,
            Cello = 43, Contrabass = 44, TremoloStrings = 45, PizzicatoStrings = 46, OrchestralHarp = 47, Timpani = 48, StringEnsemble_1 = 49, StringEnsemble_2 = 50, SynthStrings_1 = 51,
            SynthStrings_2 = 52, Voice_Aahs = 53, Voice_Oohs = 54, SynthVoice = 55, OrchestraHit = 56, Trumpet = 57, Trombone = 58, Tuba = 59, MutedTrumpet = 60, Frenchhorn = 61, BrassSection = 62,
            SynthBrass_1 = 63, SynthBrass_2 = 64, SopranoSax = 65, AltoSax = 66, TenorSax = 67, BaritoneSax = 68, Oboe = 69, EnglishHorn = 70, Bassoon = 71, Clarinet = 72, Piccolo = 73, Flute = 74,
            Recorder = 75, PanFlute = 76, BlownBottle = 77, Shakuhachi = 78, Whistle = 79, Ocarina = 80, Lead_1_square = 81, Lead_2_sawtooth = 82, Lead_3_calliope = 83, Lead_4_chiff = 84,
            Lead_5_charang = 85, Lead_6_voice = 86, Lead_7_fifths = 87, Lead_8_bass_lead = 88, Pad_1_newage = 89, Pad_2_warm = 90, Pad_3_polysynth = 91, Pad_4_choir = 92, Pad_5_bowed = 93,
            Pad_6_metallic = 94, Pad_7_halo = 95, Pad_8_sweep = 96, FX_1_rain = 97, FX_2_soundtrack = 98, FX_3_crystal = 99, FX_4_atmosphere = 100, FX_5_brightness = 101, FX_6_goblins = 102,
            FX_7_echoes = 103, FX_8_sci_fi = 104, Sitar = 105, Banjo = 106, Shamisen = 107, Koto = 108, Kalimba = 109, Bagpipe = 110, Fiddle = 111, Shanai = 112, TinkleBell = 113, Agogo = 114,
            SteelDrums = 115, Woodblock = 116, TaikoDrum = 117, MelodicTom = 118, SynthDrum = 119, ReverseCymbal = 120
        }

        /// <summary>
        /// 跟midi设备建立链接
        /// </summary>
        /// <param name="lphMidiOut"></param>
        /// <param name="uDeviceID"></param>
        /// <param name="dwCallback"></param>
        /// <param name="dwInstance"></param>
        /// <param name="dwFlags"></param>
        /// <returns></returns>
        [DllImport("winmm.dll")]
        private extern static int midiOutOpen(out int lphMidiOut, int uDeviceID, int dwCallback, int dwInstance, int dwFlags);

        /// <summary>
        /// 跟midi设备切断链接
        /// </summary>
        /// <param name="lphMidiOut"></param>
        /// <returns></returns>
        [DllImport("winmm.dll")]
        private extern static int midiOutClose(int lphMidiOut);

        /// <summary>
        /// 发出声音
        /// </summary>
        /// <param name="lphMidiOut"></param>
        /// <param name="dwMsg"></param>
        /// <returns></returns>
        [DllImport("winmm.dll")]
        public extern static int midiOutShortMsg(int lphMidiOut, int dwMsg);

        /// <summary>
        /// midi设备的句柄
        /// </summary>
        public int midiOut;

        public Midi()
        {
            if (midiOutOpen(out midiOut, -1, 0, 0, 0) != 0)
            {
                throw new Exception("無法打開MIDI設備");
            }
        }
        ~Midi()
        {
            if (midiOut != 0)
            {
                midiOutClose(midiOut);
            }
        }

        /// <summary>
        /// 计算一拍子几秒
        /// </summary>
        /// <param name="pinlv">
        /// 一分钟几拍
        /// </param>
        public void Music_speed(int pinlv,int index)
        {
            tempo_minute[index] = pinlv;
            note_long[index] = 60000 / pinlv;
        }

        /// <summary>
        /// 拍子单位（几分音符）,例：32分音符输入32
        /// </summary>
        public void Music_note_base(int note, int index)
        {
            note_base[index] = 1.0F/note;
        }

        /// <summary>
        /// 音量，最高255
        /// </summary>
        /// <param name="power_"></param>
        /// <param name="index"></param>
        public void Music_power(int power_,int index)
        {
            power[index] = power_;
        }

        /// <summary>
        /// 设定乐器
        /// </summary>
        /// <param name="instrument_"></param>
        /// <param name="index"></param>
        public void Music_instrument(int instrument_,int index)
        {
            instrument[index] = instrument_;
            midiOutShortMsg(midiOut, instrument_ << 8 | 0xC0 + index);
        }

        /// <summary>
        /// 演奏音乐
        /// </summary>
        /// <param name="music">
        /// 谱子
        /// </param>
        public void Music_play(int[,] music,int index)
        {
            int l_0=music.GetLength(0);
            int l_1=music.GetLength(1);
            for (int i = 0; i < l_0; i++)
            {
                if (music[i, 0] != 0)
                {
                    for (int o = 0; o < l_1 - 2; o++)
                    {
                        midiOutShortMsg(midiOut, music[i, o]);
                    }//发出这个和弦

                    if (!stop[index][i])//如果无连音线则停止发音，有则不停止，营造出连续的感觉
                    {
                        Thread.Sleep(music[i, l_1 - 1] - note_long[index] / 10);
                        for (int o = 0; o < l_1 - 2; o++)
                        {
                            midiOutShortMsg(midiOut, music[i, o] - (power[index] << 16));
                        }
                    }//无连音线
                    else
                    {
                        if (music[i, 0] == music[i + 1, 0])//两音相同
                        {
                            Thread.Sleep(music[i, l_1 - 1] + music[i + 1, l_1 - 1]);
                            i++;//下个音不重复发音
                        }
                        else Thread.Sleep(music[i, l_1 - 1]);//两音不同
                    }//有连音线
                }
            }
            for (int o = 0; o < l_1 - 2; o++)
            {
                midiOutShortMsg(midiOut, music[l_0-1, o] - (power[index] << 16));
            }//结束尾音
        }

        /// <summary>
        /// 封装后音乐播放器
        /// </summary>
        /// <param name="tempo_minute">
        /// 一分钟几拍
        /// </param>
        /// <param name="index">
        /// 通道
        /// </param>
        /// <param name="note_base">
        /// 一拍子是几分音符，例如：四分音符输入4
        /// </param>
        /// <param name="instrument">
        /// 乐器
        /// </param>
        /// <param name="music">
        /// 乐谱
        /// </param>
        public void Music_Play(Midi midi,int power,int tempo_minute, int index, int note_base, int instrument, int[,] music)
        {
            midi.Music_speed(tempo_minute, index);
            midi.Music_note_base(note_base,index);
            midi.Music_instrument(instrument,index);
            midi.Music_power(power, index);
            midi.Music_play(music,index);
        }

        /// <summary>
        /// 解析简谱输入
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="index"></param>
        /// <param name="diaoshi"></param>
        public void Music_parse(string p0,int index,int diaoshi)
        {
            string[] p1 = p0.Split('|');
            string[] zan1;
            int high_;
            int note;
            int saigaohe=0;

            for(int i = 0; i < p1.Length; i++)
            {
                if (p1[i].Split('|')[0].Split(',').Length > saigaohe) saigaohe = p1[i].Split('|')[0].Split(',').Length;
            }//检测最多一个和弦有几个音

            music[index]=new int[p1.Length,saigaohe+1];
            string[] zan2 = new string[saigaohe];
            stop[index] = new bool[p1.Length];
            string out_="";

            for(int i=0; i<p1.Length; i++)// -1/+1/1,4|
            {
                high_ = 0;
                note = 0;

                if (p1[i] == "-")//|-|
                {
                    stop[index][i-1]= true;
                    continue;
                }

                zan1 = p1[i].Split(',');
                music[index][i, saigaohe-1] = int.Parse(zan1[1][0].ToString());//取出音符
                for (int k = 1; k < zan1[1].Length; k++)
                {
                    music[index][i, saigaohe-1] += music[index][i, saigaohe-1] / (2 * k);
                }//附点音符

                /*Console.WriteLine(zan1[1]);
                Console.WriteLine(note_base[index]);
                Console.WriteLine((1.0F / music[index][i, saigaohe - 1]) / note_base[index]);
                Console.WriteLine(note_long[index]);*/
                music[index][i, saigaohe] = (int)(note_long[index] * ((1.0F / music[index][i, saigaohe - 1]) / note_base[index]));//计算该音符长度

                zan2 = zan1[0].Split('/');// -1/+1/1
                for(int o=0; o < zan2.Length; o++)// -1 +1 1
                {
                    foreach(char s in zan2[o])// -1
                    {
                        if (s == '+') high_++;
                        else if (s == '-') high_--;
                        else note = s - '0';
                    }
                    music[index][i,o]= 59 + note + (12 * high_) + diaoshi;//获取该音符midi代码
                    music[index][i,o]= power[index] << 16 | music[index][i,o] << 8 | 0x90 + index;//转换为midi输入格式
                    out_+= music[index][i, o].ToString() + ',';
                }
                out_ += music[index][i, saigaohe];
                Console.WriteLine(out_);
                out_ = "";
                time += music[index][i, saigaohe];
            }
            Console.WriteLine("time:" + time.ToString());
            /*for (int i=0; i<p1.Length; i++)
            {
                stop[index][i] = true;

                zan = p1[i].Split(',');
                p[i] = zan[0];//音阶
                l[i] = zan[1];//音符
                high_ = 0;
                //Console.WriteLine(p[i]+" "+l[i]);

                if (p[i].Contains("/"))
                {
                    string[] zan_h= p[i].Split('/');
                    p[i]= zan_h[0];
                    p_h[i,0]= zan_h[1];
                    if (zan_h.Length > 2) p_h[i, 1] = zan_h[2];

                    //music_he[index][i,0]=p[i].Split('/')[0];
                }

                for (int k = 0; k < p[i].Length; k++)
                {
                    if (p[i][k] == '-')
                    {
                        high_--;
                    }
                    else if (p[i][k]=='+')
                    {
                        high_++;
                    }
                    else
                    {
                        note = int.Parse(p[i][k].ToString());
                    }
                }//计算单个音符的音高
                sheets[index][i, 0] = 59 + note + (12 * high_) + diaoshi;

                sheets[index][i, 1] = (int)(note_long[index] * ((1.0F / (l[i][0] - '0')) / note_base[index]));//音符基本长度
                for (int k=1; k < l[i].Length;k++)
                {
                    sheets[index][i, 1] += sheets[index][i, 1] / 2*k;
                }//附点音符

                music[index][i, 0] = power[index] << 16 | sheets[index][i, 0] << 8 | 0x90 + index;
                music[index][i, 4] = sheets[index][i, 1];
                Console.WriteLine(music[index][i, 0].ToString() + '/' + music[index][i,1].ToString());
            }//将每个音符分别存储*/

        }

        public int[,] p = new int[10, 2]
        {
            {65,750},
            {64,750},
            {67,750},
            {65,750},
            {60,750},
            {65,750},
            {64,750},
            {67,750},
            {65,750},
            {60,750}
        };
        /*
            60  62  64  65  67   69  71
            1   2   3   4   5    6   7
            C   D   E   F   G	 A	 B
            Do  Re  Mi  Fa  Sol	 La	 Si 
         */
        /*int yingliang=50;
Midi midi=new Midi();
string[] yue;
int[] yue_jisuanji;
public Form1()
{
   InitializeComponent();
}

private void button1_Click(object sender, EventArgs e)
{
   if(openFileDialog1.ShowDialog() == DialogResult.OK)
   {
       yue=File.ReadAllText(openFileDialog1.FileName).Split('|');
   }
   yue_jisuanji = new int[yue.Length];
   for(int i = 0; i < yue.Length; i++)
   {
       yue_jisuanji[i] = int.Parse(yue[i]);
   }//转换谱子形式
}//载入谱子

int yue_index = 0;
private void timer1_Tick(object sender, EventArgs e)
{
   midi.Play(yue_jisuanji[yue_index], yingliang);
   yue_index++;
}

private void trackBar1_Scroll(object sender, EventArgs e)
{
   yingliang= trackBar1.Value;
}//调节音量


}
class Midi
{
[DllImport("winmm.dll")]
private extern static int midiOutOpen(out int lphMidiOut, int uDeviceID, int dwCallback, int dwInstance, int dwFlags);
[DllImport("winmm.dll")]
private extern static int midiOutClose(int lphMidiOut);
[DllImport("winmm.dll")]
public extern static int midiOutShortMsg(int lphMidiOut, int dwMsg);
public int midiOut;
public Midi()
{
   if (midiOutOpen(out midiOut, -1, 0, 0, 0) != 0)
   {
       throw new Exception("無法打開MIDI設備");
   }
}
~Midi()
{
   if (midiOut != 0)
   {
       midiOutClose(midiOut);
   }
}
public void Instrument(int instrument)
{
   midiOutShortMsg(midiOut, instrument << 8 | 0xC0);
}
public void Play(int note, int yinliang)
{
   midiOutShortMsg(midiOut, yinliang << 16 | note << 8 | 0x90);
}
public void Stop(int note)
{
   midiOutShortMsg(midiOut, note << 8 | 0x90);
}

    }
    

    

}
/*
#include <iostream>
#include <Windows.h>
#pragma comment(lib,"winmm.lib")
using namespace std;

enum Scale
{
  Rest = 0, C8 = 108, B7 = 107, A7s = 106, A7 = 105, G7s = 104, G7 = 103, F7s = 102, F7 = 101, E7 = 100,
  D7s = 99, D7 = 98, C7s = 97, C7 = 96, B6 = 95, A6s = 94, A6 = 93, G6s = 92, G6 = 91, F6s = 90, F6 = 89,
  E6 = 88, D6s = 87, D6 = 86, C6s = 85, C6 = 84, B5 = 83, A5s = 82, A5 = 81, G5s = 80, G5 = 79, F5s = 78,
  F5 = 77, E5 = 76, D5s = 75, D5 = 74, C5s = 73, C5 = 72, B4 = 71, A4s = 70, A4 = 69, G4s = 68, G4 = 67,
  F4s = 66, F4 = 65, E4 = 64, D4s = 63, D4 = 62, C4s = 61, C4 = 60, B3 = 59, A3s = 58, A3 = 57, G3s = 56,
  G3 = 55, F3s = 54, F3 = 53, E3 = 52, D3s = 51, D3 = 50, C3s = 49, C3 = 48, B2 = 47, A2s = 46, A2 = 45,
  G2s = 44, G2 = 43, F2s = 42, F2 = 41, E2 = 40, D2s = 39, D2 = 38, C2s = 37, C2 = 36, B1 = 35, A1s = 34,
  A1 = 33, G1s = 32, G1 = 31, F1s = 30, F1 = 29, E1 = 28, D1s = 27, D1 = 26, C1s = 25, C1 = 24, B0 = 23,
  A0s = 22, A0 = 21
};

enum Voice
{
  X1 = C2, X2 = D2, X3 = E2, X4 = F2, X5 = G2, X6 = A2, X7 = B2,
  L1 = C3, L2 = D3, L3 = E3, L4 = F3, L5 = G3, L6 = A3, L7 = B3,
  M1 = C4, M2 = D4, M3 = E4, M4 = F4, M5 = G4, M6 = A4, M7 = B4,
  H1 = C5, H2 = D5, H3 = E5, H4 = F5, H5 = G5, H6 = A5, H7 = B5,
  LOW_SPEED = 500, MIDDLE_SPEED = 400, HIGH_SPEED = 300,
  _ = 0XFF
};

void Wind()
{
  HMIDIOUT handle;
  midiOutOpen(&handle, 0, 0, 0, CALLBACK_NULL);
  // midiOutShortMsg(handle, 2 << 8 | 0xC0);
  int volume = 0x7f;
  int voice = 0x0;
  int sleep = 350;
  int wind[] =
  {
   400,0,L7,M1,M2,M3,300,L3,0,M5,M3,300,L2,L5,2,_,0,L7,M1,M2,M3,300,L2,0,M5,M3,M2,M3,M1,M2,L7,M1,300,L5,0,L7,M1,M2,M3,300,L3,0,M5,M3,300,L2,L5,2,_,0,L7,M1,M2,M3,300,L2,0,M5,M3,M2,M3,M1,M2,L7,M1,300,L5,
   0,L7,M1,M2,M3,300,L3,0,M5,M3,300,L2,L5,2,_,0,L7,M1,M2,M3,300,L2,0,M5,M3,M2,M3,M1,M2,L7,M1,300,L5,0,L7,M1,M2,M3,300,L3,0,M5,M3,300,L2,L5,2,_,
   0,M6,M3,M2,L6,M3,L6,M2,M3,L6,_,_,_,
   M2,700,0,M1,300,M2,700,0,M1,300,M2,M3,M5,0,M3,700,300,M2,700,0,M1,300,M2,700,0,M1,M2,M3,M2,M1,300,L5,_,
   M2,700,0,M1,300,M2,700,0,M1,300,M2,M3,M5,0,M3,700,300,M2,700,0,M3,300,M2,0,M1,700,300,M2,_,_,_,
   M2,700,0,M1,300,M2,700,0,M1,300,M2,M3,M5,0,M3,700,300,M2,700,0,M3,300,M2,0,M1,700,300,L6,_,
   0,M3,M2,M1,M2,300,M1,_,0,M3,M2,M1,M2,300,M1,700,0,L5,M3,M2,M1,M2,300,M1,_,_,_,
   M1,M2,M3,M1,M6,0,M5,M6,300,_,700,0,M1,300,M7,0,M6,M7,300,_,_,M7,0,M6,M7,300,_,M3,0,H1,H2,H1,M7,300,M6,M5,M6,0,M5,M6,_,M5,M6,M5,300,M6,0,M5,M2,300,_,0,M5,700,300,M3,_,_,_,
   M1,M2,M3,M1,M6,0,M5,M6,300,_,700,0,M1,300,M7,0,M6,M7,300,_,_,M7,0,M6,M7,300,_,M3,0,H1,H2,H1,M7,300,M6,M5,M6,0,H3,H3,300,_,M5,M6,0,H3,H3,300,_,0,M5,700,300,M6,_,_,_,_,_,
   H1,H2,H3,0,H6,H5,300,_,0,H6,H5,300,_,0,H6,H5,300,_,0,H2,H3,300,H3,0,H6,H5,300,_,0,H6,H5,300,_,0,H6,H5,300,_,0,H2,H3,300,H2,0,H1,M6,300,_,0,H1,H1,300,H2,0,H1,300,M6,700,0,_,300,H1,700,H3,_,0,H3,H4,H3,H2,H3,300,H2,700,
   H1,H2,H3,0,H6,H5,_,H6,H5,_,H6,H5,300,_,H3,H3,0,H6,H5,_,H6,H5,_,H6,H5,700,300,H3,700,H2,0,H1,M6,700,300,
   H3,700,H2,0,H1,300,M6,700,H1,H1,_,_,_,_,_,
   0,M6,300,H3,700,H2,0,H1,M6,700,300,H3,H2,700,300,0,H1,M6,300,700,H1,H1,_,_,
   0,L7,M1,M2,M3,300,L3,0,M5,M3,300,L2,L5,2,_,0,L7,M1,M2,M3,300,L2,0,M5,M3,M2,M3,M1,M2,L7,M1,300,L5,0,L7,M1,M2,M3,300,L3,0,M5,M3,300,L2,L5,2,_,
   0,M6,M3,M2,L6,M3,L6,M2,M3,L6,_,_,_,
   M2,700,0,M1,300,M2,700,0,M1,300,M2,M3,M5,0,M3,700,300,M2,700,0,M1,300,M2,700,0,M1,M2,M3,M2,M1,300,L5,_,
   M2,700,0,M1,300,M2,700,0,M1,300,M2,M3,M5,0,M3,700,300,M2,700,0,M3,300,M2,0,M1,700,300,M2,_,_,_,
   M2,700,0,M1,300,M2,700,0,M1,300,M2,M3,M5,0,M3,700,300,M2,700,0,M3,300,M2,0,M1,700,300,L6,_,
   0,M3,M2,M1,M2,300,M1,_,0,M3,M2,M1,M2,300,M1,700,0,L5,M3,M2,M1,M2,300,M1,_,_,_,
   M1,M2,M3,M1,M6,0,M5,M6,300,_,700,0,M1,300,M7,0,M6,M7,300,_,_,M7,0,M6,M7,300,_,M3,0,H1,H2,H1,M7,300,M6,M5,M6,0,M5,M6,_,M5,M6,M5,300,M6,0,M5,M2,300,_,0,M5,700,300,M3,_,_,_,
   M1,M2,M3,M1,M6,0,M5,M6,300,_,700,0,M1,300,M7,0,M6,M7,300,_,_,M7,0,M6,M7,300,_,M3,0,H1,H2,H1,M7,300,M6,M5,M6,0,H3,H3,300,_,M5,M6,0,H3,H3,300,_,0,M5,700,300,M6,_,_,_,_,_,
   H1,H2,H3,0,H6,H5,300,_,0,H6,H5,300,_,0,H6,H5,300,_,0,H2,H3,300,H3,0,H6,H5,300,_,0,H6,H5,300,_,0,H6,H5,300,_,0,H2,H3,300,H2,0,H1,M6,300,_,0,H1,H1,300,H2,0,H1,300,M6,700,0,_,300,H1,700,H3,_,0,H3,H4,H3,H2,H3,300,H2,700,
   H1,H2,H3,0,H6,H5,_,H6,H5,_,H6,H5,300,_,H3,H3,0,H6,H5,_,H6,H5,_,H6,H5,700,300,H3,700,H2,0,H1,M6,700,300,
   H3,700,H2,0,H1,300,M6,700,H1,H1,_,_,_,_,_,
   H1,H2,H3,0,H6,H5,300,_,0,H6,H5,300,_,0,H6,H5,300,_,0,H2,H3,300,H3,0,H6,H5,300,_,0,H6,H5,300,_,0,H6,H5,300,_,0,H2,H3,300,H2,0,H1,M6,300,_,0,H1,H1,300,H2,0,H1,300,M6,700,0,_,300,H1,700,H3,_,0,H3,H4,H3,H2,H3,300,H2,700,
   H1,H2,H3,0,H6,H5,_,H6,H5,_,H6,H5,300,_,H3,H3,0,H6,H5,_,H6,H5,_,H6,H5,700,300,H3,700,H2,0,H1,M6,700,300,
   H3,700,H2,0,H1,300,M6,700,H1,H1,_,_,_,_,_,
   H1,H2,H3,0,H6,H5,300,_,0,H6,H5,300,_,0,H6,H5,300,_,0,H2,H3,300,H3,0,H6,H5,300,_,0,H6,H5,300,_,0,H6,H5,300,_,0,H2,H3,300,H2,0,H1,M6,300,_,0,H1,H1,300,H2,0,H1,300,M6,700,0,_,300,H1,700,H3,_,0,H3,H4,H3,H2,H3,300,H2,700,
   H1,H2,H3,0,H6,H5,_,H6,H5,_,H6,H5,300,_,H3,H3,0,H6,H5,_,H6,H5,_,H6,H5,700,300,H3,700,H2,0,H1,M6,700,300,
   H3,700,H2,0,H1,300,M6,700,H1,H1,_,_,_,_,_,
   0,M6,300,H3,700,H2,0,H1,M6,700,300,H3,H2,700,300,0,H1,M6,300,700,H1,H1,_,_,_,_,_,_,_,
  };

  for (auto i : wind) {

  if (i == 0) { sleep = 175;continue; }
  if (i == 700) { Sleep(175);continue; }
  if (i == 300) { sleep = 350;continue; }
  if (i == _) { Sleep(350);continue; }
  // if (i == 900) volume += 100;
  voice = (volume << 16) + ((i) << 8) + 0x90;
  midiOutShortMsg(handle, voice);
  cout << voice << endl;
  Sleep(sleep);
  // midiOutShortMsg(handle, 0x7BB0);
  }
  midiOutClose(handle);
}
int main()
{
  Wind();
  return 0;
}
 */
    }

}

