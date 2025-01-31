﻿using System.Diagnostics;
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
using System.Windows.Forms;
using System.Net.NetworkInformation;
using static sanciyuandehundan_API.Midi;
using System.Xml;



namespace sanciyuandehundan_API
{
    public class Picture
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
        [DllImport("sanciyuandehundan_API_Cpp.dll", CharSet = CharSet.Unicode, EntryPoint = "mci_play", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint mci_play(string order);
        public enum puhao
        {
            gao=0,zhong=-10,di=-20
        }
        public static string local_all;
        private static readonly byte[] dadaio_G = { 2, 2, 1, 2, 2, 2, 1 };
        private static readonly byte[] dadaio_G_huanyuan = { 0, 2, 4, 5, 7, 9, 11, 12 };
        private static readonly byte[] dadaio_F = { 1, 2, 2, 2, 1, 2, 2 };
        private static readonly byte[] dadaio_F_huanyuan = { 0, 1, 3, 5, 7, 8, 10, 12 };
        private static readonly byte[] dadaio_C = { 2, 1, 2, 2, 2, 1, 2 };
        private static readonly byte[] dadaio_C_huanyuan = { 0, 2, 3, 5, 7, 9, 10, 12 };
        public static byte[] yingui_start_file = { 0x4d, 0x54, 0x68, 0x64, 0x00, 0x00, 0x00, 0x06 };//文件定义,要加上种类、音轨数、四分音符长度
        public static byte[] yingui_start = { 0x4d, 0x54, 0x72, 0x6b };//音轨头
        public static byte[] yingui_one = { 0x00, 0x00 };//文件定义，种类,单音轨
        public static byte[] yingui_many = { 0x00, 0x01 };//文件定义，种类,多音轨
        public static byte[] yingui_end = { 0xff, 0x2f, 0x00 };//音轨尾
        public static byte[] yingui_diaohao = { 0xff, 0x59, 0x02 };//调号头
        public static byte[] yingui_jiepai = { 0xff, 0x58, 0x04 };//节拍头
        public static byte[] yingui_speed = { 0xff, 0x51, 0x03 };//速度头
        public const float power_chang_up = 1.2F;
        public const float power_chang_down = 0.8F;
        public const float time_end = 0.1F;
        public const float time_first = 0.9F;
        public const int base_C = 60;
        public static char[] split = { '/', ',' };//分割格式
        public class Yingui
        {
            public class Hexian
            {
                public enum Hexian_xiushi
                {
                    xiushi_null = 0,
                    pp = 1,
                    p = 2,
                    mp = 3,
                    mf = 4,
                    f = 5,
                    ff = 6
                }
                public int time = 0;
                public int time_0_1 = 0;
                public int time_1_2 = 0;
                public int time_2_0 = 0;
                public int time_ret_to = 0;
                public Yingui parent;
                public Note[] note;
                public byte[] note_down = null;//按下
                public byte note_down_num = 0;
                public byte[] note_up_1 = null;//放开1
                public byte note_up_1_num = 0;
                public byte[] note_up_2 = null;//放开2
                public byte note_up_2_num = 0;
                public string sheet;
                public Hexian next;
                public Hexian last;

                public Hexian(string sheet_, Yingui parent_, Hexian last_)
                {
                    sheet = sheet_;
                    Console.WriteLine("————————————————\n创建和弦: " + sheet_ + "\n通道: " + parent_.index + "\n上个和弦: " + last_?.sheet + "\n—————————");
                    //Console.WriteLine(last?.note[0].weizhi);
                    parent = parent_;
                    string[] temp = sheet_.Split(split);
                    time = Music_stream_time(temp.Last(), parent.xiaojie_note_base, parent.xiaojie_note_long);
                    Console.WriteLine("时间: " + time + "\n—————————");
                    last = last_;
                    if (last != null) last.next = this;
                    if (temp.First().Equals('0'))
                    {
                        Console.WriteLine("此和弦为休止符");
                        if (last != null)
                        {
                            Console.WriteLine("不为开头休止符");
                            //Console.WriteLine("有东西可加");
                            if (last.note_up_2_num != 0)
                            {
                                last.time_2_0 += time;
                                //Console.WriteLine("加到最尾");
                                Console.WriteLine("加到上一个和弦的up2后，加了: " + time);
                            }
                            else if (last.note_up_1_num != 0)
                            {
                                last.time_1_2 += time;
                                //Console.WriteLine("加到尾");
                                Console.WriteLine("加到上一个和弦的up1后，加了: " + time);
                            }
                            else if (last.note_down_num != 0)
                            {
                                last.time_0_1 += time;
                                //Console.WriteLine("加到没那么尾");
                                Console.WriteLine("加到上一个和弦的down后，加了: " + time);
                            }//确认last的哪个要被加
                        }
                        else
                        {
                            Console.WriteLine("为开头休止符");
                            Music_stream_time(time, parent);
                        }
                        return;
                    }
                    else
                    {
                        if (last == null)
                        {
                            Music_stream_time((byte)0, parent);
                            Console.WriteLine("此和弦是开头和弦");
                        }
                        Console.WriteLine("此和弦不是休止符");
                    }
                    Console.WriteLine("—————————");
                    note = new Note[temp.Length - 1];
                    note_down = new byte[note.Length];
                    note_up_1 = new byte[note.Length];
                    note_up_2 = new byte[note.Length];
                    Console.WriteLine("此和弦的音符有: " + note.Length + "个\n—————————\n——————");
                    for (int i = 0; i < temp.Length - 1; i++)
                    {
                        note[i] = new Note(temp[i], this);
                    }
                    Console.WriteLine("—————————");
                    Note_event();
                    Console.WriteLine("————————————————");
                }
                public void Note_event()
                {
                    //Console.WriteLine("xiu: " + next?.note[0].note_xiushi[0].ToString());
                    Console.WriteLine("处理修饰");
                    foreach (Note n in note)
                    {
                        foreach (Note.Note_xiushi xiu in n.note_xiushi)
                        {
                            switch (xiu)
                            {
                                case Note.Note_xiushi.xiushi_null:
                                    note_down[note_down_num] = n.pinlv;
                                    note_down_num++;
                                    note_up_1[note_up_1_num] = n.pinlv;
                                    note_up_1_num++;
                                    Console.WriteLine("note_down加入" + n.pinlv);
                                    Console.WriteLine("note_up_1加入" + n.pinlv);
                                    break;
                                case Note.Note_xiushi.lianyin_first:
                                    note_down[note_down_num] = n.pinlv;
                                    note_down_num++;
                                    Console.WriteLine("note_down加入" + n.pinlv);
                                    //if (next != null) time += next.time_add();
                                    break;
                                case Note.Note_xiushi.lianyin_ing:
                                    Console.WriteLine("无加入");
                                    break;
                                case Note.Note_xiushi.lianyin_ing_noend:
                                    note_up_2[note_up_2_num] = n.pinlv;
                                    note_up_2_num++;
                                    Console.WriteLine("note_up_2加入" + n.pinlv);
                                    break;
                                case Note.Note_xiushi.lianyin_last:
                                    note_up_1[note_up_1_num] = n.pinlv;
                                    note_up_1_num++;
                                    Console.WriteLine("note_up_1加入" + n.pinlv);
                                    break;
                                case Note.Note_xiushi.lianyin_one:
                                    note_down[note_down_num] = n.pinlv;
                                    note_down_num++;
                                    Console.WriteLine("note_down加入" + n.pinlv);
                                    note_up_2[note_up_1_num] = n.pinlv;
                                    note_up_2_num++;
                                    Console.WriteLine("note_up_2加入" + n.pinlv);
                                    break;
                            }
                        }
                    }


                    if (note_up_1_num == 0)
                    {
                        //Console.WriteLine("没有up1");
                        time_0_1 += time;
                        time_1_2 += 0;
                        Console.WriteLine("没有up1，0_1即为time");
                    }
                    else
                    {
                        time_0_1 += int.Parse((time * time_first).ToString());
                        //Console.WriteLine("计算01：" + time_0_1 + " | " + (time * time_first).ToString());
                        time_1_2 += (int)(time * time_end);
                        Console.WriteLine("有up1，0_1为0.9time,1_2为0.1time");
                    }
                    if (note_down_num == 0)
                    {
                        Console.Write("没有down，将以下时间：");

                        time_ret_to += time_0_1;
                        Console.Write("0_1");
                        if (note_up_1_num == 0)
                        {
                            time_ret_to += time_1_2;
                            Console.Write("|1_2");
                        }
                        if (note_up_2_num == 0)
                        {
                            time_ret_to += time_2_0;
                            Console.Write("|2_0");
                        }//确认哪些要加到last
                        Console.WriteLine("往前加，加了: "+time_ret_to);
                        //Console.WriteLine("加多少: " + time_ret_to);
                        last?.time_add(time_ret_to);
                    }

                }//对修饰做出反应
                public void time_add(int time)
                {
                    Console.WriteLine("前面有和弦可加");
                    if (note_up_2_num != 0)
                    {
                        time_2_0 += time;
                        Console.WriteLine("加到2_0");
                    }
                    else if (note_up_1_num != 0)
                    {
                        time_1_2 += time;
                        Console.WriteLine("加到1_2");
                    }
                    else if (note_down_num != 0)
                    {
                        time_0_1 += time;
                        Console.WriteLine("加到0_1");
                    }//确认last的哪个要被加
                    else
                    {

                        Console.WriteLine("只有间隔，再往前");
                        last?.time_add(time);
                    }
                }
                public void Hexian_write(BinaryWriter writer)
                {
                    bool a = false;
                    for (int i = 0; i < note_down_num; i++)
                    {
                        writer.Write((byte)(0x90 + parent.index));
                        writer.Write(note_down[i]);
                        Console.WriteLine("按下: " + note_down[i]);
                        Music_stream_power(time, parent, a);
                        if (i != note_down_num - 1)
                        {
                            writer.Write((byte)0);
                        }//如果下一个还是按下事件

                        a = true;
                    }//按下
                    if (note_down_num != 0)
                    {
                        Console.WriteLine("写入01: " + time_0_1);
                        Music_stream_time(time_0_1, parent);
                    }

                    for (int i = 0; i < note_up_1_num; i++)
                    {
                        writer.Write((byte)(0x80 + parent.index));
                        writer.Write(note_up_1[i]);
                        Console.WriteLine("松开1: " + note_up_1[i]);
                        writer.Write((byte)0);
                        if (i != note_up_1_num - 1)
                        {
                            writer.Write((byte)0);
                        }//如果下一个还是松开事件
                    }
                    if (note_up_1_num != 0)
                    {
                        Console.WriteLine("写入12: " + time_1_2);
                        Music_stream_time(time_1_2, parent);
                    }

                    for (int i = 0; i < note_up_2_num; i++)
                    {
                        writer.Write((byte)(0x80 + parent.index));
                        writer.Write(note_up_2[i]); 
                        Console.WriteLine("松开2: " + note_up_2[i]);
                        writer.Write((byte)0);
                        if (i != note_up_2_num - 1)
                        {
                            writer.Write((byte)0);
                        }
                    }
                    if (note_up_2_num != 0)
                    {
                        Console.WriteLine("写入20: " + time_2_0);
                        Music_stream_time(time_2_0, parent);
                    }
                }//将事件写入流！！！！！！！！！！！！！！！！！！！！！！
                /*
                public byte[] Note_down(ref byte index)
                {
                    byte[] ret = new byte[note.Length];
                    index = 0;
                    foreach (Note note_ in note)
                    {
                        foreach (Note.Note_xiushi xiushi in note_.note_xiushi)
                        {
                            switch (xiushi)
                            {
                                case Note.Note_xiushi.xiushi_null:
                                case Note.Note_xiushi.lianyin_first:
                                    ret[index] = note_.pinlv;
                                    index++;
                                    break;
                            }
                        }
                    }
                    return ret;
                }//检测哪些音符是要按下的
                public byte[] Note_up(ref byte index)
                {
                    byte[] ret= new byte[0];
                    index = 0;
                    foreach (Note note_ in note)
                    {
                        foreach (Note.Note_xiushi xiushi in note_.note_xiushi)
                        {
                            switch (xiushi)
                            {
                                case Note.Note_xiushi.xiushi_null:
                                case Note.Note_xiushi.lianyin_last:
                                    ret[index] = note_.pinlv;
                                    index++;
                                    break;
                            }
                        }
                    }
                    return ret;
                }//检测哪些音符是要松开的*/
            }
            public class Note
            {
                public enum Note_xiushi
                {
                    xiushi_null = 0,//没有修饰
                    lianyin_first = 1,//连音线开头
                    lianyin_ing = 2,//连音线中间
                    lianyin_ing_noend = 3,//连音线中间但没连到连音线
                    lianyin_last = 4,//连音线尾巴
                    lianyin_one = 5,//连音线但没接到同音符
                }
                public Hexian parent;
                public byte pinlv;
                public int weizhi;
                public string note_yuan;
                public string note_xiushi_string;
                public Note_xiushi[] note_xiushi;
                public Note(string note_, Hexian parent_)
                {
                    Console.WriteLine("创建音符: " + note_);
                    note_yuan = note_.Split('(')[0];
                    Console.WriteLine("音符位置: " + note_yuan);
                    weizhi = note_yuan.Last() - '0';
                    Console.WriteLine("音符调用音阶索引: "+weizhi);
                    parent = parent_;
                    pinlv = Music_stream_note(note_yuan, parent.parent, true);
                    Console.WriteLine("音高: " + pinlv);
                    note_xiushi_string = note_.Split('(')[1];
                    Console.WriteLine("修饰: "+note_xiushi_string);
                    Note_xiushi_detect();
                    Console.WriteLine("——————");
                }
                private void Note_xiushi_detect()
                {
                    Console.WriteLine("检测修饰");
                    note_xiushi = new Note_xiushi[note_xiushi_string.Length];
                    byte index = 0;
                    foreach (char a in note_xiushi_string)
                    {
                        switch (a)
                        {
                            case Lianyin_first:
                                note_xiushi[index] = Note_xiushi.lianyin_first;
                                index++;
                                Console.WriteLine("修饰代码: " + Note_xiushi.lianyin_first);
                                break;
                            case Lianyin_ing:
                                note_xiushi[index] = Note_xiushi.lianyin_ing;
                                index++;
                                Console.WriteLine("修饰代码: " + Note_xiushi.lianyin_ing);
                                break;
                            case Lianyin_ing_noend:
                                note_xiushi[index] = Note_xiushi.lianyin_ing_noend;
                                index++;
                                Console.WriteLine("修饰代码: " + Note_xiushi.lianyin_ing_noend);
                                break;
                            case Lianyin_last:
                                note_xiushi[index] = Note_xiushi.lianyin_last;
                                index++;
                                Console.WriteLine("修饰代码: " + Note_xiushi.lianyin_last);
                                break;
                            case Lianyin_One:
                                note_xiushi[index] = Note_xiushi.lianyin_one;
                                index++;
                                Console.WriteLine("修饰代码: " + Note_xiushi.lianyin_one);
                                break;
                        }
                    }
                    if (index == 0)
                    {
                        note_xiushi = new Note_xiushi[1] { Note_xiushi.xiushi_null };
                        Console.WriteLine("修饰代码: " + Note_xiushi.xiushi_null);
                    }
                }//检测有哪些修饰
            }
            /// <summary>
            /// 跳过
            /// </summary>
            public const string Skip = "k";
            /// <summary>
            /// 该音符有连音线
            /// </summary>
            public const char Lianyin_first = 'l';
            public const char Lianyin_ing = 'L';
            public const char Lianyin_ing_noend = 'p';
            public const char Lianyin_last = ';';
            public const char Lianyin_One = 'O';

            public string help =
                "instrument，midi乐器代码，有列举\n" +
                "pinlv，一分钟几拍\n" +
                "note，一拍是几分音符\n" +
                "xiaojie，一小节几拍\n" +
                "power，力度或理解为音量，范围：0~127\n" +
                "diaoshi，音程比C大调低或高多少\n" +
                "♭♯♮";

            /// <summary>
            /// 该音轨的.mid文件地址
            /// </summary>
            public string local_1;
            /// <summary>
            /// 该音轨的暂存地址，不可播放
            /// </summary>
            public string local_2;
            /// <summary>
            /// 该音轨在mci设备中的名字
            /// </summary>
            private string mci_name;
            /// <summary>
            /// 该音轨在midi设备中的通道0~15
            /// </summary>
            public int index;
            /// <summary>
            /// 一小节有几拍
            /// </summary>
            public int xiaojie_tempo_num;
            /// <summary>
            /// 一小节可以被分成几个三拍，计算强弱拍用
            /// </summary>
            public int xiaojie_tempo_split;
            /// <summary>
            /// 一小节中哪几拍是强拍
            /// </summary>
            public int[] xiaojie_tempo_split_anchored;
            /// <summary>
            /// 基准音量
            /// </summary>
            public int power_base;
            /// <summary>
            /// 基准音量记录
            /// </summary>
            public int power_base_save;
            /// <summary>
            /// 一分钟有几拍
            /// </summary>
            public int xiaojie_tempo_minute;
            /// <summary>
            /// 一拍是几分音符
            /// </summary>
            public float xiaojie_note_base;
            /// <summary>
            /// 一拍是几tick
            /// </summary>
            public int xiaojie_note_long;
            /// <summary>
            /// 该音轨的音色
            /// </summary>
            public int instrument;
            /// <summary>
            /// 该音轨的未分割乐谱
            /// </summary>
            public string yuepu;
            /// <summary>
            /// 该音轨的时长
            /// </summary>
            public int time;
            /// <summary>
            /// 该音轨的谱号,高音谱0，低音谱-20
            /// </summary>
            public puhao diaoshi;
            /// <summary>
            /// 该音轨的谱号上有几个升（+）降（-）记号
            /// </summary>
            public int diaoshi_updpwn = 0;
            /// <summary>
            /// 标记各音符的升降半音数
            /// </summary>
            public int[] diaoshi_anchored = new int[8];
            /// <summary>
            /// key补正
            /// </summary>
            public byte keybuzheng = 0;
            /// <summary>
            /// 写入可播放的单音轨.mid文件
            /// </summary>
            public System.IO.BinaryWriter writer1;
            /// <summary>
            /// 写入不可播放的用于暂存midievent的文件
            /// </summary>
            public System.IO.BinaryWriter writer2;

            public Yingui(string sheet, int index_, int instrument_, int pinlv, int note, int xiaojie_, int power_, puhao diaoshi, int updown, int key)
            {
                keybuzheng = (byte)key;
                index = index_;
                mci_name = "yingui_" + index.ToString();
                yuepu = sheet.Replace("\n", string.Empty).Replace("\r", string.Empty);
                Music_speed(pinlv, this);
                Music_note_base(note, xiaojie_, this);
                Music_power(power_, this);
                Music_instrument(instrument_, this);
                Music_diaoshi(diaoshi, updown, this);
                Yingui_parse();
            }
            public Yingui(int index)
            {
                this.index = index;
            }
            public void Yingui_parse_()
            {
                xiaojie_ing = 1;
                Yingui_close();
                writer1 = new BinaryWriter(new FileStream(Environment.CurrentDirectory + "\\yingui" + index.ToString() + "_1.mid", FileMode.Create));//创建流，文件
                writer2 = new BinaryWriter(new FileStream(Environment.CurrentDirectory + "\\yingui" + index.ToString() + "_2.mid", FileMode.Create));//创建流，音轨
                local_1 = Environment.CurrentDirectory + "\\yingui" + index.ToString() + "_1.mid";
                local_2 = Environment.CurrentDirectory + "\\yingui" + index.ToString() + "_2.mid";



                Music_stream_file(writer1, 1);//写入文件头
                Music_stream_global(writer1);//写入全局音轨
                Music_stream_start(writer2, this);//写入音轨头

                string[] pu1 = yuepu.Split('|');//分割为一个一个的和弦
                string[][] pu2 = new string[pu1.Length][];

                int forindex_0 = 0;
                foreach (string p in pu1)
                {
                    pu2[forindex_0] = p.Split(split, StringSplitOptions.RemoveEmptyEntries);
                    forindex_0++;
                }//分割为一个一个音高和长度

                int forindex_3 = 0;
                int start_stop = 0;
                if (pu2[0][0] == "0")
                {
                    do
                    {
                        start_stop += Music_stream_time(pu2[forindex_3][1], this.xiaojie_note_base, this.xiaojie_note_long);
                        pu2[forindex_3][0] = Skip;
                        forindex_3++;
                    } while (pu2[forindex_3][0].Equals("0"));//多个休止符
                }
                Music_stream_time(start_stop, this);//开头间隔
                int forindex_1 = 0;
                int forindex_2 = 0;
                int time = 0;
                double time_end = 0.1;
                int time_stop = 0;
                bool endstop = false;
                bool xiaojie_same = false;
                for (int i = 0; i < pu1.Length; i++)
                {
                    if (pu2[i][0].Equals(Skip))
                    {
                        continue;
                    }//跳过

                    if (pu2[i][0].Equals("pp"))
                    {
                        power_base = power_base_save;
                        continue;
                    }
                    else if (pu2[i][0].Equals("p"))
                    {
                        power_base = power_base_save;
                        continue;
                    }
                    else if (pu2[i][0].Equals("mp"))
                    {
                        power_base = power_base_save;
                        continue;
                    }
                    else if (pu2[i][0].Equals("mf"))
                    {
                        power_base = power_base_save;
                        continue;
                    }
                    else if (pu2[i][0].Equals("f"))
                    {
                        power_base = power_base_save;
                        continue;
                    }
                    else if (pu2[i][0].Equals("ff"))
                    {
                        power_base = power_base_save;
                        continue;
                    }//力度符号

                    time = Music_stream_time(pu2[i].Last(), this.xiaojie_note_base, this.xiaojie_note_long);
                    time_end = 0.1;
                    if (i < pu2.GetLength(0) - 1)
                    {
                        if (pu2[i + 1][0].Equals("0"))
                        {
                            forindex_2 = i + 1;
                            do
                            {
                                time_stop += Music_stream_time(pu2[forindex_2][1], this.xiaojie_note_base, this.xiaojie_note_long);
                                pu2[forindex_2][0] = Skip;
                                forindex_2++;
                                if (forindex_2 >= pu1.Length)
                                {
                                    endstop = true;
                                    break;
                                }
                            } while (pu2[forindex_2][0].Equals("0"));//多个休止符
                        }//如果下一个是休止符
                        else if (pu2[i + 1][0].Equals("-") & pu2[i + 1].GetLength(0) == 1)
                        {
                            if (pu2[i + 2][0].Equals(pu2[i][0]))
                            {
                                time += Music_stream_time(pu2[i + 2].Last(), this.xiaojie_note_base, this.xiaojie_note_long);
                                pu2[i + 1][0] = Skip;
                                pu2[i + 2][0] = Skip;
                            }//如果连音线连接的是两相同音符
                            else
                            {
                                pu2[i + 1][0] = Skip;
                                time_end = 0;
                            }//如果连音线连接的是两不同音符
                        }//如果下一个是连音线
                    }//特殊事件
                    forindex_1 = 0;
                    //Console.WriteLine("———————————————————————");
                    foreach (string p in pu2[i])
                    {
                        if (forindex_1 != pu2[i].Length - 1)
                        {
                            writer2.Write((byte)(0x90 + index));//格式
                            writer2.Write((byte)Music_stream_note(p, this, true));//音高
                            Music_stream_power(time, this, xiaojie_same);
                            xiaojie_same = true;
                            forindex_1++;
                            if (forindex_1 != pu2[i].Length - 1)
                            {
                                writer2.Write((byte)0);
                            }//如果下一个指向的还是音符
                        }
                    }//按下
                    //Console.WriteLine("———————————————————————");
                    Music_stream_time((int)(time * (1 - time_end)), this);//分割———————————————————————————————————
                    forindex_1 = 0;
                    foreach (string p in pu2[i])
                    {
                        if (forindex_1 != pu2[i].Length - 1)
                        {
                            writer2.Write((byte)(0x80 + index));//格式
                            writer2.Write(((byte)Music_stream_note(p, this, false)));//音高
                            writer2.Write((byte)0);
                            //Music_stream_power(time, this);
                            forindex_1++;
                            if (forindex_1 != pu2[i].Length - 1)
                            {
                                writer2.Write((byte)0);
                            }//如果下一个指向的还是音符
                        }
                    }//放开
                    xiaojie_same = false;
                    //Console.WriteLine(pu2[i][0]);
                    if (i != pu1.Length - 1 | endstop) Music_stream_time((int)(time * time_end + time_stop), this);//分割——————————————————————————
                    time_stop = 0;
                    //time = 0;
                    endstop = false;
                }//音轨
                power_time = 0;
                if (!endstop) Music_stream_time(xiaojie_note_long + (int)(time * time_end), this);//结尾间隔
                Music_stream_end(writer2, this);
                writer2.Seek(0, SeekOrigin.Begin);
                writer2.BaseStream.CopyTo(writer1.BaseStream);
                writer1.Close();
                writer2.Close();
                //Console.WriteLine("time_all:" + this.time);
            }

            public void Yingui_parse()
            {
                xiaojie_ing = 1;
                Yingui_close();
                writer1 = new BinaryWriter(new FileStream(Environment.CurrentDirectory + "\\yingui" + index.ToString() + "_1.mid", FileMode.Create));//创建流，文件
                writer2 = new BinaryWriter(new FileStream(Environment.CurrentDirectory + "\\yingui" + index.ToString() + "_2.mid", FileMode.Create));//创建流，音轨
                local_1 = Environment.CurrentDirectory + "\\yingui" + index.ToString() + "_1.mid";
                local_2 = Environment.CurrentDirectory + "\\yingui" + index.ToString() + "_2.mid";



                Music_stream_file(writer1, 1);//写入文件头
                Music_stream_global(writer1);//写入全局音轨
                Music_stream_start(writer2, this);//写入音轨头

                string[] pu1 = yuepu.Split('|');//分割为一个一个的和弦
                Hexian[] hexians = new Hexian[pu1.Length];
                int hexians_index = 0;
                foreach (string pu in pu1)
                {
                    switch (pu)
                    {
                        case "pp":
                            power_base = (int)(power_base_save * 0.3);
                            break;
                        case "p":
                            power_base = (int)(power_base_save * 0.6);
                            break;
                        case "mp":
                            power_base = (int)(power_base_save * 0.7);
                            break;
                        case "mf":
                            power_base = (int)(power_base_save * 1);
                            break;
                        case "f":
                            power_base = (int)(power_base_save * 1.4);
                            break;
                        case "ff":
                            power_base = (int)(power_base_save * 1.8);
                            break;
                        default:
                            if (hexians_index - 1 != -1)
                            {
                                //Console.WriteLine("写入: " + hexians_index);
                                hexians[hexians_index] = new Hexian(pu, this, hexians[hexians_index - 1]);
                            }
                            else
                            {
                                hexians[hexians_index] = new Hexian(pu, this, null);
                            }
                            hexians_index++;
                            break;
                    }
                }
                foreach (Hexian h in hexians)
                {
                    h?.Hexian_write(writer2);
                }

                power_time = 0;
                Music_stream_end(writer2, this);
                writer2.Seek(0, SeekOrigin.Begin);
                writer2.BaseStream.CopyTo(writer1.BaseStream);
                writer1.Close();
                writer2.Close();
                //Console.WriteLine("time_all:" + this.time);
            }

            internal static int power_time = 0;
            internal static byte power_zan = 0;

            /// <summary>
            /// 打开文件
            /// </summary>
            public void Yingui_open()
            {
                Console.WriteLine("open: " + mci_play("open " + local_1 + " type sequencer Alias " + mci_name));
                //Console.WriteLine("setvolume: " + mci_play("setaudio " + mci_name + " volume to " + 100.ToString()));
            }

            /// <summary>
            /// 播放
            /// </summary>
            public void Yingui_play()
            {
                Console.WriteLine("play: " + mci_play("play " + mci_name));
            }

            /// <summary>
            /// 关闭文件
            /// </summary>
            public void Yingui_close()
            {
                Console.WriteLine("close: " + mci_play("close " + mci_name));
            }

            /// <summary>
            /// 暂停播放
            /// </summary>
            public void Yingui_pause()
            {
                Console.WriteLine("pause: " + mci_play("pause " + mci_name));
            }

            /// <summary>
            /// 继续播放
            /// </summary>
            public void Yingui_resume()
            {
                Console.WriteLine("resume: " + mci_play("play " + mci_name));
            }

            static int xiaojie_ing = 1;
            /// <summary>
            /// 写入力度
            /// </summary>
            /// <param name="time"></param>
            /// <param name="yingui"></param>
            private static void Music_stream_power(int time, Yingui yingui, bool xiaojie_same)
            {
                for (int i = 0; i < yingui.xiaojie_tempo_split; i++)
                {
                    if (power_time + 1 > yingui.xiaojie_tempo_split_anchored[i] * yingui.xiaojie_note_long & power_time + 1 <= (yingui.xiaojie_tempo_split_anchored[i] + 1) * yingui.xiaojie_note_long)
                    {
                        if (i == 0)
                        {
                            power_zan = (byte)(yingui.power_base * power_chang_up);//强
                            break;
                        }
                        else
                        {
                            power_zan = (byte)yingui.power_base;
                            break;
                        }
                    }
                    else
                    {
                        power_zan = (byte)(yingui.power_base * power_chang_down);
                    }

                }
                if (!xiaojie_same)
                {
                    power_time += time;
                    //Console.WriteLine("time_ing：" + power_time);
                }
                if (power_time >= yingui.xiaojie_note_long * yingui.xiaojie_tempo_num)
                {
                    power_time -= yingui.xiaojie_note_long * yingui.xiaojie_tempo_num;
                    //Console.WriteLine("小节：" + xiaojie_ing);
                    xiaojie_ing++;
                }
                yingui.writer2.Write(power_zan);

            }

            /// <summary>
            /// 计算时间
            /// </summary>
            /// <param name="i"></param>
            /// <returns></returns>
            public static int Music_stream_time(string i, float xiaojie_note_base, int xiaojie_note_long)
            {
                Console.Write("时间计算（"+i + "转化成: ");
                double time = 0;
                bool lianyin_bool = false;
                string[] lianyin = { };
                if (i.First().Equals('[') & i.Last().Equals(']'))//连音
                {
                    lianyin = i.TrimStart('[').TrimEnd(']').Split(';');
                    i = lianyin[1];
                    lianyin_bool = true;
                }

                //Console.WriteLine("time:" + i);
                string time_ = "";
                int point = 0;
                foreach (char a in i)
                {
                    if (a == '.')
                    {
                        point++;//有几个点
                    }
                    else if (double.TryParse(a.ToString(), out time))
                    {
                        time_ += a;
                    }
                }
                time = int.Parse(time_);
                time = (double)(1 / xiaojie_note_base) / time * (double)(xiaojie_note_long);//计算无附点的长度
                for (int j = 1; j <= point; j++)
                {
                    time += time / Math.Pow(2, j);
                }//计算附点

                if (lianyin_bool)//连音
                {
                    double zan;
                    zan = time % int.Parse(lianyin[0]);
                    time /= int.Parse(lianyin[0]);
                    //Console.WriteLine("zan:" + zan);
                    if (zan != 0)
                    {
                        if (zan >= int.Parse(lianyin[2]))
                        {
                            time++;
                        }
                    }
                }
                Console.WriteLine(time+"）");
                return ((int)time);
            }

            /// <summary>
            /// 写入时间
            /// </summary>
            /// <param name="i"></param>
            internal static void Music_stream_time(int i, Yingui yingui)
            {
                Console.Write("写入时间: "+i+"; mid: ");
                yingui.time += i;
                //Console.WriteLine("time: " + i);
                if (i > 16383)
                {
                    yingui.writer2.Write((byte)((1 << 7) + (i >> 14)));
                    Console.Write(" " + (byte)((1 << 7) + (i >> 14)));
                }
                if (i > 127)
                {
                    yingui.writer2.Write((byte)((1 << 7) + (i >> 7)));
                    Console.Write(" " + (byte)((1 << 7) + (i >> 7)));
                }
                yingui.writer2.Write((byte)(((byte)(i << 1)) >> 1));
                Console.WriteLine(" " + (byte)(((byte)(i << 1)) >> 1));
            }

            /// <summary>
            /// 计算音高
            /// </summary>
            /// <param name="note"></param>
            /// <returns></returns>
            public static byte Music_stream_note(string note, Yingui yingui, bool outnote)
            {
                //var biao =/ [0-9] g / ;
                int highdown = 0;//低或高几个八度
                int updown = 0;
                if (yingui.instrument == 24) { highdown--; }
                bool huanyuan = false;
                foreach (char a in note)
                {
                    if (a == '-')
                    {
                        highdown--;
                    }//低一个八度
                    else if (a == '+')
                    {
                        highdown++;
                    }//高一个八度
                    else if (a == '♭')
                    {
                        updown--;
                    }//降记号
                    else if (a == '♯')
                    {
                        updown++;
                    }//升记号
                    else if (a == '♮')
                    {
                        huanyuan = true;
                    }//还原记号
                }
                int note_ = note.Last() - '0';
                byte note_out = (byte)(base_C + yingui.diaoshi + yingui.diaoshi_anchored[note_ - 1] + highdown * 12 + updown);
                if (huanyuan & yingui.diaoshi == puhao.gao) note_out = (byte)(base_C + yingui.diaoshi + dadaio_G_huanyuan[note_ - 1] + highdown * 12);
                else if (huanyuan & yingui.diaoshi == puhao.di) note_out = (byte)(base_C + yingui.diaoshi + dadaio_F_huanyuan[note_ - 1] + highdown * 12);
                else if (huanyuan & yingui.diaoshi == puhao.zhong) note_out = (byte)(base_C + yingui.diaoshi + dadaio_C_huanyuan[note_ - 1] + highdown * 12);
                note_out += yingui.keybuzheng;
                //if (outnote) Console.WriteLine("note:" + note + "     note_out:" + note_out);
                return note_out;
            }//！！！！！！！！！1111111111
        }

        public Midi()
        {
            /*if (midiOutOpen(out midiOut, -1, 0, 0, 0) != 0)
            {
                MessageBox.Show("無法打開MIDI設備");
            }*/
            //midiOutShortMsg(midiOut, 0x7e << 16 | 60 << 8 | 0x90);
        }
        ~Midi()
        {
            //Music_close_all();
            /*if (midiOut != 0)
            {
                midiOutClose(midiOut);
            }*/
        }

        /// <summary>
        /// 计算一拍子几tick
        /// </summary>
        /// <param name="pinlv">
        /// 一分钟几拍
        /// </param>
        public static void Music_speed(int pinlv, Yingui yingui)
        {
            yingui.xiaojie_tempo_minute = pinlv;
            yingui.xiaojie_note_long = 57600 / pinlv;
        }

        /// <summary>
        /// 拍子单位（几分音符）,例：32分音符输入32
        /// </summary>
        public static void Music_note_base(int note, int xiaojie_, Yingui yingui)
        {
            int k = 0;
            yingui.xiaojie_tempo_num = xiaojie_;
            //Console.WriteLine("一小节几拍" + yingui.xiaojie_tempo_num);
            if (xiaojie_ != 4)
            {
                k = 3;
                yingui.xiaojie_tempo_split = xiaojie_ / 3;
                //Console.WriteLine("一小节可以被切成几个3拍" + yingui.xiaojie_tempo_split);
            }
            else
            {
                k = 2;
                yingui.xiaojie_tempo_split = xiaojie_ / 2;
                //Console.WriteLine("一小节可以被切成几个2拍" + yingui.xiaojie_tempo_split);
            }//一小节4拍是特殊的
            yingui.xiaojie_tempo_split_anchored = new int[yingui.xiaojie_tempo_split];
            yingui.xiaojie_tempo_split_anchored[0] = 0;//第一拍必定是强拍
            for (int i = 1; i < yingui.xiaojie_tempo_split; i++)
            {
                yingui.xiaojie_tempo_split_anchored[i] = i * k;
            }
            yingui.xiaojie_note_base = 1.0F / note;
        }

        /// <summary>
        /// 音量，最高0x7e
        /// </summary>
        /// <param name="power_"></param>
        /// <param name="index"></param>
        public static void Music_power(int power_, Yingui yingui)
        {
            //yingui.power = new int[sheet.Split('|').Length];
            yingui.power_base = power_;
            yingui.power_base_save = power_;
        }

        /// <summary>
        /// 设定乐器
        /// </summary>
        /// <param name="instrument_"></param>
        /// <param name="index"></param>
        public static void Music_instrument(int instrument_, Yingui yingui)
        {
            yingui.instrument = instrument_;
            //midiOutShortMsg(midiOut, instrument_ << 8 | 0xC0 + index);
        }

        /// <summary>
        /// 设置音高变化，此音轨中输入的1相较于中央C高（正）或低（负数）了多少
        /// </summary>
        /// <param name="diaoshi"></param>
        /// <param name="index"></param>
        public static void Music_diaoshi(puhao diaoshi_, int updown, Yingui yingui)
        {
            updown *= 4;
            //updown %= 7;
            byte[] zan2 = new byte[7];
            zan2[0] = 0;
            yingui.diaoshi = diaoshi_;
            yingui.diaoshi_updpwn = updown;

            int zan1 = updown % 7;
            //Console.WriteLine("updown % 7  " + updown % 7);
            //if (zan1 < 0) zan1 *= -1;
            if (zan1 > 0)
            {
                byte[] diao1 = new byte[zan1];
                byte[] diao2 = new byte[7 - zan1];

                for (int i = 7 - zan1; i < 7; i++)
                {
                    if (diaoshi_ == puhao.gao) diao1[i - 7 + zan1] = dadaio_G[i];
                    if (diaoshi_ == puhao.zhong) diao1[i - 7 + zan1] = dadaio_C[i];
                    if (diaoshi_ == puhao.di) diao1[i - 7 + zan1] = dadaio_F[i];
                }
                for (int i = 0; i < 7 - zan1; i++)
                {
                    if (diaoshi_ == puhao.gao) diao2[i] = dadaio_G[i];
                    if (diaoshi_ == puhao.zhong) diao2[i] = dadaio_C[i];
                    if (diaoshi_ == puhao.di) diao2[i] = dadaio_F[i];
                }
                diao1.CopyTo(zan2, 0);
                diao2.CopyTo(zan2, zan1);
                zan2.CopyTo(yingui.diaoshi_anchored, 1);
                //foreach (byte b in diao1) Console.WriteLine("d1:" + b);
                //foreach (byte b in diao2) Console.WriteLine("d2:" + b);
                //foreach (byte b in zan2) Console.WriteLine("z2:" + b);
                if (updown/4==2) yingui.diaoshi_anchored[0] = 1;
                for (int i = 1; i < 8; i++)
                {
                    yingui.diaoshi_anchored[i] += yingui.diaoshi_anchored[i - 1];
                }
                //Console.WriteLine("______________");
                //foreach (byte b in yingui.diaoshi_anchored) Console.WriteLine(b);
            }//升记号
            else if (zan1 == 0)
            {
                if (diaoshi_ == puhao.gao)
                {
                    dadaio_G.CopyTo(yingui.diaoshi_anchored, 1);
                }
                else if (diaoshi_ == puhao.zhong)
                {
                    dadaio_C.CopyTo(yingui.diaoshi_anchored, 1);
                }
                else if (diaoshi_ == puhao.di)
                {
                    dadaio_F.CopyTo(yingui.diaoshi_anchored, 1);
                }
                if (updown < 0)
                {
                    for (int i = 1; i < 8; i++)
                    {
                        yingui.diaoshi_anchored[i] += (byte)(yingui.diaoshi_anchored[i - 1] - 1);
                    }
                }
                else if (updown > 0)
                {
                    for (int i = 1; i < 8; i++)
                    {
                        yingui.diaoshi_anchored[i] += (byte)(yingui.diaoshi_anchored[i - 1] + 1);
                    }
                }
                else
                {
                    for (int i = 1; i < 8; i++)
                    {
                        yingui.diaoshi_anchored[i] += (byte)(yingui.diaoshi_anchored[i - 1]);
                    }
                }
                //Console.WriteLine("______________");
                //foreach (byte b in yingui.diaoshi_anchored) Console.WriteLine(b);
            }//全升或降一个半音或没有升降
            else
            {
                zan1 *= -1;
                byte[] diao1 = new byte[zan1];
                byte[] diao2 = new byte[7 - zan1];
                if (diaoshi_ == puhao.gao)
                {

                    for (int i = 0; i < zan1; i++)
                    {
                        yingui.diaoshi_anchored[8 - zan1 + i] = dadaio_G[i];
                        //Console.WriteLine("将 " + i + " 转到 " + (8 - zan1 + i));
                    }
                    for (int i = zan1+1; i < 8; i++)
                    {
                        yingui.diaoshi_anchored[i - zan1] = dadaio_G[i - 1];
                        //Console.WriteLine("将 " + (i - 1) + " 转到 " + (i - zan1));
                    }
                }
                else if (diaoshi_ == puhao.zhong)
                {
                    for (int i = 0; i < zan1; i++)
                    {
                        yingui.diaoshi_anchored[8 - zan1 + i] = dadaio_C[i];
                        //Console.WriteLine("将 " + i + " 转到 " + (8 - zan1 + i));
                    }
                    for (int i = zan1 + 1; i < 8; i++)
                    {
                        yingui.diaoshi_anchored[i - zan1] = dadaio_C[i - 1];
                        //Console.WriteLine("将 " + (i - 1) + " 转到 " + (i - zan1));
                    }
                }
                else if (diaoshi_ == puhao.di)
                {
                    for (int i = 0; i < zan1; i++)
                    {
                        yingui.diaoshi_anchored[8 - zan1 + i] = dadaio_F[i];
                        //Console.WriteLine("将 " + i + " 转到 " + (8 - zan1 + i));
                    }
                    for (int i = zan1 + 1; i < 8; i++)
                    {
                        yingui.diaoshi_anchored[i - zan1] = dadaio_F[i - 1];
                        //Console.WriteLine("将 " + (i - 1) + " 转到 " + (i - zan1));
                    }
                }
                //Console.WriteLine("______________");
                //foreach (byte b in yingui.diaoshi_anchored) Console.WriteLine(b);
                if (updown / 4 == 6) yingui.diaoshi_anchored[0] = -1;
                for (int i = 1; i < 8; i++)
                {
                    yingui.diaoshi_anchored[i] += yingui.diaoshi_anchored[i - 1];
                }
                //Console.WriteLine("______________");
                //foreach (byte b in yingui.diaoshi_anchored) Console.WriteLine(b);
            }//降记号
        }//低音E、高音C

        public static void Music_parse_hebin(string[] paths, int note_long)
        {
            BinaryReader reader;
            BinaryWriter allwriter = new BinaryWriter(new FileStream(Environment.CurrentDirectory + "\\yingui_all.mid", FileMode.Create));
            Music_stream_file(allwriter, paths.Length);
            Music_stream_global(allwriter);
            for (int i = 0; i < paths.Length; i++)
            {
                reader = new BinaryReader(new FileStream(paths[i], FileMode.Open));
                reader.BaseStream.CopyTo(allwriter.BaseStream);
                allwriter.Seek(0, SeekOrigin.End);
                reader.Close();
            }//读取音轨的暂存文件,
            local_all = Environment.CurrentDirectory + "\\yingui_all.mid";
            allwriter.Close();

        }//将多音轨合成成一文件，Environment.CurrentDirectory + "\\yingui" + index.ToString() + "_2.mid"

        /// <summary>
        /// 写入 .mid 文件的开头
        /// </summary>
        /// <param name="writer">
        /// 流
        /// </param>
        /// <param name="num">
        /// 几个音轨
        /// </param>
        /// <param name="note_long">
        /// 一个四分音符多长
        /// </param>
        private static void Music_stream_file(BinaryWriter writer, int num)
        {
            writer.Write(yingui_start_file);//写入文件定义
            writer.Write(yingui_many);//写入文件定义,文件类型
            writer.Write((byte)0); //音轨数量
            writer.Write((byte)(num + 1));//写入文件定义，音轨数量
            writer.Write((byte)0x01);//如果大于7位
            writer.Write((byte)0xe0);//一个四分音符几tick
            //该文件信息
        }

        /// <summary>
        /// 写入全局音轨
        /// </summary>
        /// <param name="writer">
        /// 流
        /// </param>
        private static void Music_stream_global(BinaryWriter writer)
        {
            writer.Write(yingui_start);//写入音轨头
            writer.Write((short)0); //音轨长度
            writer.Write((byte)0); //音轨长度
            writer.Write((byte)0x4);//写入音轨长度
            writer.Write((byte)0x00);//分隔
            writer.Write(yingui_end);//写入音轨尾
            //全局音轨
        }

        /// <summary>
        /// 写入音轨结尾
        /// </summary>
        /// <param name="writer"></param>
        private static void Music_stream_end(BinaryWriter writer, Yingui yingui)
        {
            //Yingui.Music_stream_time(yingui.xiaojie_note_long, writer);//结尾间隔
            writer.Write(yingui_end);
            writer.Seek(7, SeekOrigin.Begin);//到之前预留的空位
            writer.Write((byte)(writer.BaseStream.Length - 8));//写入音轨长度
            if (writer.BaseStream.Length > 65535)
            {
                writer.Seek(5, SeekOrigin.Begin);//到之前预留的空位
                writer.Write((byte)(writer.BaseStream.Length >> 16));//写入音轨长度
            }//写入音轨长度
            if (writer.BaseStream.Length > 255)
            {
                writer.Seek(6, SeekOrigin.Begin);//到之前预留的空位
                writer.Write((byte)(writer.BaseStream.Length >> 8));//写入音轨长度
            }//写入音轨长度
            writer.Seek(7, SeekOrigin.Begin);//到之前预留的空位
            writer.Write((byte)(writer.BaseStream.Length - 8));//写入音轨长度
        }

        /// <summary>
        /// 写入音轨开头
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="yingui"></param>
        private static void Music_stream_start(BinaryWriter writer, Yingui yingui)
        {
            writer.Write(yingui_start);//写入音轨头
            writer.Write((byte)0x00);//预留音轨长度空位
            writer.Write((byte)0x00);//预留音轨长度空位
            writer.Write((byte)0x00);//预留音轨长度空位
            writer.Write((byte)0x00);//预留音轨长度空位
            writer.Write((byte)0x00);//分隔
            writer.Write((byte)(0xc0 + yingui.index));//乐器
            writer.Write((byte)yingui.instrument);//乐器
            //writer.Write((byte)0x00);//开头分隔
            //该音轨信息
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        public static void Music_open_all()
        {
            Console.WriteLine("open_all: " + mci_play("open " + local_all + " type sequencer Alias yingui_all"));
        }

        /// <summary>
        /// 播放
        /// </summary>
        public static void Music_play_all()
        {
            Console.WriteLine("play_all: " + mci_play("play yingui_all"));
        }

        /// <summary>
        /// 关闭文件
        /// </summary>
        public static void Music_close_all()
        {
            Console.WriteLine("close_all: " + mci_play("close yingui_all"));
        }

        /// <summary>
        /// 暂停播放
        /// </summary>
        public static void Music_pause_all()
        {
            Console.WriteLine("pause_all: " + mci_play("pause yingui_all"));
        }

        /// <summary>
        /// 继续播放
        /// </summary>
        public static void Music_resume_all()
        {
            Console.WriteLine("resume_all: " + mci_play("play yingui_all"));
        }

        public static double Music_time(string i)
        {
            double time = 0;
            bool lianyin_bool = false;
            string[] lianyin = { };
            if (i.First().Equals('[') & i.Last().Equals(']'))//连音
            {
                lianyin = i.TrimStart('[').TrimEnd(']').Split(';');
                i = lianyin[1];
                lianyin_bool = true;
            }

            //Console.WriteLine("time:" + i);
            string time_ = "";
            int point = 0;
            foreach (char a in i)
            {
                if (a == '.')
                {
                    point++;//有几个点
                }
                else if (double.TryParse(a.ToString(), out time))
                {
                    time_ += a;
                }
            }
            time = int.Parse(time_);
            //time = (double)(1 / xiaojie_note_base) / time * 256;//计算无附点的长度
            time = 256 / time;
            for (int j = 1; j <= point; j++)
            {
                time += time / Math.Pow(2, j);
            }//计算附点

            if (lianyin_bool)//连音
            {
                double zan;
                zan = time % int.Parse(lianyin[0]);
                time /= int.Parse(lianyin[0]);
                //Console.WriteLine("zan:" + zan);
                if (zan != 0)
                {
                    if (zan >= int.Parse(lianyin[2]))
                    {
                        time++;
                    }
                }

            }
            //Console.WriteLine(time);
            return time;
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



        /*public int[] xiaojie=new int[16];//一小节几拍
        public int[] xiaojie_split=new int[16];//一小节可以被分成几个三拍
        public int[][] xiaojie_split_anchored=new int[16][];//哪几拍是强的
        public int[] power_base = new int[16];//每个声部基准音量 
        public int[][] power = new int[16][];//每个音符音量
        public int[] tempo_minute=new int[16];//一分钟几拍
        public float[] note_base=new float[16];//一拍是几分音符
        public int[] note_long=new int[16];//一拍几tick
        public int[] instrument = new int[16];//乐器
        public int[][,] music_zan_0 = new int[16][,];//暂存乐谱0
        public int[][,] music_zan_1 = new int[16][,];//暂存乐谱1
        public int[][,] music_zan_2 = new int[16][,];//暂存乐谱2
        public int[][] music_zan_3 = new int[16][];//暂存乐谱3
        public byte[][] music_zan_4 = new byte[16][];//暂存乐谱4
        public bool[][] stop = new bool[16][];//和下一个音符间是否有连音线
        public int[] time =new int[16];//曲子时长几秒
        public int[] stop_number=new int[16];//几个连音线
        public int[] note_number = new int[16];//乐谱的和弦最多有几个音符
        public int[] diaoshi=new int[16];//整首歌曲的调式
        //public int[] puhao=new int[16];//谱号*/
        //public System.IO.BinaryWriter writer1;//保存单音轨的mid文件，可播放
        //public System.IO.BinaryWriter writer2;//仅保存音轨数据，不可直接播放
        //public System.IO.BinaryReader reader;
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
        /*midiOutShortMsg(midiOut, 100 << 16 + 60 << 8 + 0x90);
            for (int i = 0; i < music_zan_2[index].GetLength(0); i++)
            {
                for (int j = 0; j < note_number[index]; j++)
                {
                    midiOutShortMsg(midiOut, music_zan_2[index][i, j]);
                    Console.WriteLine("play: " + music_zan_2[index][i,j]);
                }
                Thread.Sleep(music_zan_2[index][i, note_number[index]]);
                for (int j = 0; j < note_number[index]; j++)
                {
                    midiOutShortMsg(midiOut, music_zan_2[index][i, j] - 0x10);
                }
                Thread.Sleep(music_zan_2[index][i, note_number[index] + 1]);
            }*///效率还是太低
        /*
            Console.WriteLine(System.DateTime.Now);
            
            int l_0=music.GetLength(0);
            int l_1=music.GetLength(1);
            for (int i = 0; i < l_0; i++)
            {
                if (music[i, 0] != 0)
                {
                    Console.WriteLine(stop[index][i]+":"+i);
                    for (int o = 0; o < l_1 - 2; o++)
                    {
                        midiOutShortMsg(midiOut, music[i, o]);
                    }//发出这个和弦

                    if (stop[index][i]==false)//如果无连音线则停止发音，有则不停止，营造出连续的感觉
                    {
                        Thread.Sleep(music[i, l_1 - 1] - note_long[index] / 10);
                        for (int o = 0; o < l_1 - 2; o++)
                        {
                            midiOutShortMsg(midiOut, music[i, o] - (power[index][i] << 16));
                        }
                    }//无连音线
                    else
                    {
                        if (music[i, 0] == music[i + 2, 0])//两音相同
                        {
                            Console.WriteLine(i);
                            Thread.Sleep(music[i, l_1 - 1] + music[i + 1, l_1 - 1]);
                            i++;//下个音不重复发音
                            i++;
                            continue;
                        }
                        else Thread.Sleep(music[i, l_1 - 1]);//两音不同
                    }//有连音线
                }
            }
            for (int o = 0; o < l_1 - 2; o++)
            {
                midiOutShortMsg(midiOut, music[l_0-1, o] - (power[index][0] << 16));
            }//结束尾音
            Console.WriteLine(System.DateTime.Now);
            *///效率太低，被弃用，原play函数
        /// <summary>
        /// 解析简谱输入
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="index"></param>
        /// <param name="diaoshi"></param>
        /*public static void Music_parse(Yingui yingui)
        {
            string[] p1 = yingui.yuepu.Split('|');
            string[] zan1;
            int high_;
            int note;
            int saigaohe=0;
            yingui.time = 0;
            float xiaojie_paizi_now=0;//现在在第几拍
            yingui.stop_number = 0;
            bool[] v;//标记是否是被连音线连接的不同音阶的音符 

            for (int i = 0; i < yingui.xiaojie_split_anchored.GetLength(0);i++)
            {
                Console.WriteLine(yingui.xiaojie_split_anchored[i]);//哪些拍子是被标记的
            }

            for (int i = 0; i < p1.Length; i++)
            {
                if (p1[i].Split('|')[0].Split(',').Length > saigaohe) saigaohe = p1[i].Split('|')[0].Split(',').Length;
            }//检测最多一个和弦有几个音

            yingui.music_zan_0 =new int[p1.Length,saigaohe+1];
            string[] zan2;
            yingui.stop = new bool[p1.Length];
            string out_="";

            for(int i=0; i<p1.Length; i++)// -1/+1/1,4|
            {
                high_ = 0;
                note = 0;

                if (p1[i] == "-")//|-|
                {
                    yingui.stop[i-1]= true;
                    yingui.stop_number++;
                    continue;
                }//连音线

                zan1 = p1[i].Split(',');
                yingui.music_zan_0[i, saigaohe] = int.Parse(zan1[1].Replace(".",""));//取出音符
                Console.Write("音长"+yingui.music_zan_0[i,saigaohe]+" ");
                xiaojie_paizi_now += (1.0F/yingui.music_zan_0[i, saigaohe ])/yingui.note_base;//记录现在到这小节的第几拍了
                if (xiaojie_paizi_now >= yingui.xiaojie) xiaojie_paizi_now -= yingui.xiaojie;//每过一个小节重置拍子进度
                bool a=false;//是否是强拍
                for(int u = 0; u < yingui.xiaojie_split; u++)
                {
                    if ((int)(xiaojie_paizi_now-1) == (int)(yingui.xiaojie_split_anchored[u]))//检测此音符所在的拍子是否为被标记为强拍的拍子
                    {
                        a = true;
                        Console.Write(xiaojie_paizi_now - 1 + " ");
                        Console.Write((int)(yingui.xiaojie_split_anchored[u]) + " ");
                        if (u == 0)//如果是小节的第一拍则为强拍
                        {
                            yingui.power[i] = (int)(yingui.power_base * power_chang_up);//kk
                            Console.Write("1 ");
                        }
                        else//不是则为次强拍
                        {
                            yingui.power[i] = yingui.power_base;
                            Console.Write("0 ");
                        }v
                        break;
                    }
                }
                if (!a)
                {
                    yingui.power[i] = (int)(yingui.power_base * power_chang_down);
                    Console.Write(xiaojie_paizi_now - 1 + " ");
                    Console.Write((int)(yingui.xiaojie_split_anchored[yingui.xiaojie_split_anchored.GetLength(0)-1] ) + " ");
                    Console.Write("-1 ");
                }//如果没被标记为强拍则为弱拍
                //节奏，拍的强弱

                yingui.music_zan_0[i, saigaohe] = (int)(yingui.note_long * ((1.0F / yingui.music_zan_0[i, saigaohe]) / yingui.note_base));//计算该音符长度
                yingui.time += yingui.music_zan_0[i, saigaohe]- yingui.note_long / 10;//计算此乐曲时间
                for (int k = 1; k < zan1[1].Length; k++)
                {
                    if (zan1[1][k] == '.') yingui.music_zan_0[i, saigaohe] += yingui.music_zan_0[i, saigaohe] / (2 * k);
                }//附点音符
                Console.Write(yingui.music_zan_0[i, saigaohe] + " ");

                zan2 = zan1[0].Split('/');// -1/+1/1
                for(int o=0; o < zan2.Length; o++)// -1 +1 1
                {
                    foreach(char s in zan2[o])// -1
                    {
                        if (s == '+') high_+=1;
                        else if (s == '-') high_-=1;
                        else note = s - '0';//将char转为int
                    }

                    yingui.music_zan_0[i, o] = (note * 2) + (high_ * 12) + base_C-2 + yingui.diaoshi;
                    if (note * 2 + high_ > 6) yingui.music_zan_0[i, o] -= 1;//获取音阶代码，定义常量用const
                    Console.WriteLine("音高"+yingui.music_zan_0[i, o]);

                    //music_zan_0[index][i, o] = 59 + note + (12 * high_)+diaoshi[index];//获取该音符midi代码，有问题！！！！！！！！！！！！！！
                    
                    yingui.music_zan_0[i,o]= yingui.power[i] << 16 | yingui.music_zan_0[i,o] << 8 | 0x90 + yingui.index;//转换为midi输入格式
                    out_+= yingui.music_zan_0[i, o].ToString() + ',';
                }//音阶

                out_ += yingui.music_zan_0[i, saigaohe];
                Console.WriteLine(out_);
                out_ = "";

            }//暂存乐谱

            Console.WriteLine("time:" + yingui.time.ToString());
            yingui.music_zan_1 =new int[yingui.music_zan_0.GetLength(0) - yingui.stop_number,saigaohe+2];//设定暂存1乐谱,相比原暂存乐谱增加了音符间间隔时长的设置
            v = new bool[yingui.music_zan_1.GetLength(0)];
            Console.WriteLine("music:"+ yingui.music_zan_1.GetLength(0).ToString()+','+yingui.music_zan_1.GetLength(1).ToString());
            Console.WriteLine("music_zan:" + yingui.music_zan_0.GetLength(0).ToString() + ',' + yingui.music_zan_0.GetLength(1).ToString());

            for(int i = 0; i < yingui.music_zan_0.GetLength(0); i++)
            {
                Console.Write('|');
                for (int j = 0; j < yingui.music_zan_0.GetLength(1); j++)
                {
                    Console.Write(yingui.music_zan_0[i, j].ToString() + '|');
                }
                Console.WriteLine();
            }//监控

            int b = 0;//遇到了几次连音线
            int xiangtong = 0;
            for (int i = 0; i < yingui.music_zan_0.GetLength(0); i++)//清除连音线造成的空格
            {
                if (yingui.music_zan_0[i, 0] == 0)//如果这里是连音线的位置
                {
                    if (yingui.music_zan_0[i - 1, 0] == yingui.music_zan_0[i + 1, 0])//被连音线链接双方音阶是否相同，相同
                    {
                        for (int j = 0; j < yingui.music_zan_0.GetLength(1); j++)//跳过这格连音线和被连音线链接的后方的相同音符
                        {
                            yingui.music_zan_1[i - b, j] = yingui.music_zan_0[i + 2, j];
                        }
                        yingui.music_zan_1[i - b - 1, saigaohe] += yingui.music_zan_0[i + 2, saigaohe];
                        xiangtong++;
                        i++;
                        b++;
                    }
                    else//不同
                    {
                        for (int j = 0; j < yingui.music_zan_0.GetLength(1); j++)//跳过这格
                        {
                            yingui.music_zan_1[i - b, j] = yingui.music_zan_0[i + 1, j];
                        }
                        v[i - b-1] = true;
                    }
                    b++;
                    i++;
                }
                else
                {
                    for (int j = 0; j < yingui.music_zan_0.GetLength(1); j++)
                    {
                        yingui.music_zan_1[i - b, j] = yingui.music_zan_0[i, j];
                    }
                }
            }//暂存乐谱简化，连音线，两个音符不同时的情况

            Console.WriteLine("——————————————");//优雅的分隔线

            for(int i = 0; i < yingui.music_zan_1.GetLength(0); i++)
            {
                if (!v[i]) yingui.music_zan_1[i, saigaohe + 1] = yingui.music_zan_1[i, saigaohe] / 10;//按下间隔
                Console.Write('|');
                for (int j = 0; j < yingui.music_zan_1.GetLength(1); j++)
                {
                    Console.Write(yingui.music_zan_1[i, j].ToString() + '|');
                }
                Console.WriteLine();
            }//监控

            Console.WriteLine("——————————————");//优雅的分隔线

            yingui.music_zan_2 =new int[yingui.music_zan_1.GetLength(0)-xiangtong,saigaohe+2];//暂存乐谱2初始化
            for (int i = 0; i < yingui.music_zan_2.GetLength(0); i++)
            {
                Console.Write('|');
                for (int j = 0; j < yingui.music_zan_1.GetLength(1); j++)
                {
                    yingui.music_zan_2[i, j] = yingui.music_zan_1[i, j];
                    Console.Write(yingui.music_zan_2[i, j].ToString() + '|');
                }
                Console.WriteLine();
            }//暂存乐谱2
            yingui.note_number = saigaohe;

            yingui.music_zan_3 = new int[yingui.music_zan_2.GetLength(0)*(2*saigaohe+2)];
            string[] biaoji=new string[yingui.music_zan_3.Length];//标记这是什么命令
            int san_index=0;
            int s3;
            for (int i = 0; i < yingui.music_zan_2.GetLength(0); i++)
            {
                for (int j = 0;j <saigaohe; j++)
                {
                    if (yingui.music_zan_2[i, j] != 0)
                    {
                        yingui.music_zan_3[san_index] = yingui.music_zan_2[i, j];
                        biaoji[san_index] = "down";
                        Console.WriteLine("按下:" + yingui.music_zan_3[san_index]);
                        san_index++;//下一个要在哪个索引
                    }
                }//按下
                s3 = yingui.music_zan_2[i, saigaohe];//按住暂存
                yingui.music_zan_3[san_index] = yingui.music_zan_2[i,saigaohe]-s3/10;//按住
                biaoji[san_index] = "keep";
                Console.WriteLine("按住:" + yingui.music_zan_3[san_index]);
                san_index++;
                for(int j = 0;j<saigaohe; j++)
                {
                    if (yingui.music_zan_2[i, j] != 0)
                    {
                        yingui.music_zan_3[san_index] = yingui.music_zan_2[i, j] - 0x10;
                        biaoji[san_index] = "up";
                        Console.WriteLine("松开:" + yingui.music_zan_3[san_index]);
                        san_index++;
                    }
                }//松开
                yingui.music_zan_3[san_index] = yingui.music_zan_2[i, saigaohe + 1];//停顿
                biaoji[san_index] = "keep";
                Console.WriteLine("停顿:" + yingui.music_zan_3[san_index]);
                san_index++;
            }//化为命令

            yingui.writer1 = new BinaryWriter(new FileStream(Environment.CurrentDirectory + "\\yingui" + yingui.index.ToString() + "_1.mid", FileMode.Create));//创建流
            yingui.writer2 = new BinaryWriter(new FileStream(Environment.CurrentDirectory + "\\yingui" + yingui.index.ToString() + "_2.mid", FileMode.Create));//创建流
            yingui.writer1.Write(yingui_start_file);//写入文件定义
            yingui.writer1.Write(yingui_one);//写入文件定义,文件类型
            yingui.writer1.Write((byte)0); //音轨数量
            yingui.writer1.Write((byte)2);//写入文件定义，音轨数量，顺序有问题
            //writer1.Write((short)note_long[index]);//写入文件定义，四分音符长度,有问题,反了
            yingui.writer1.Write((byte)(yingui.note_long >> 8));//如果大于7位
            yingui.writer1.Write((byte)yingui.note_long);
            //该文件信息

            yingui.writer1.Write(yingui_start);//写入音轨头
            yingui.writer1.Write((short)0); //音轨长度
            yingui.writer1.Write((byte)0); //音轨长度
            yingui.writer1.Write((byte)0x4);//写入音轨长度，顺序有问题
            yingui.writer1.Write((byte)0x00);//分隔
            /*writer1.Write(yingui_diaohao);//调号头
            writer1.Write((byte)diaoshi[index]);//调号
            writer1.Write((byte)0x00);//调号
            writer1.Write((byte)0x00);//分隔
            writer1.Write(yingui_jiepai);//节拍头
            writer1.Write((byte)xiaojie[index]);//一小节几拍
            writer1.Write((byte)(1/note_base[index]));//一拍是几分音符
            writer1.Write((byte)0x18);//节拍器时钟
            writer1.Write((byte)0x08);//一四分音符几个32分音符
            writer1.Write((byte)0x00);//分隔
            //writer1.Write(yingui_speed);//写入速度头
            //writer1.Write((byte)0x00);//分隔
            yingui.writer1.Write(yingui_end);//写入音轨尾
            //全局音轨

            yingui.writer2.Write(yingui_start);//写入音轨头
            yingui.writer2.Write((byte)0x00);//预留音轨长度空位
            yingui.writer2.Write((byte)0x00);//预留音轨长度空位
            yingui.writer2.Write((byte)0x00);//预留音轨长度空位
            yingui.writer2.Write((byte)0x00);//预留音轨长度空位
            yingui.writer2.Write((byte)0x00);//分隔
            yingui.writer2.Write((byte)(0xc0+yingui.index));//乐器
            yingui.writer2.Write((byte)yingui.instrument);//乐器
            yingui.writer2.Write((byte)0x00);//分隔
            //该音轨信息

            for (int i = 0; i < yingui.music_zan_3.Length;i++)
            {
                switch (biaoji[i])
                {
                    case "keep"://最多三位元组
                        if (yingui.music_zan_3[i] > 16383)
                        {
                            yingui.writer2.Write((byte)((1 << 7) + (yingui.music_zan_3[i] >> 14)));//如果大于14位
                        }
                        if (yingui.music_zan_3[i]>127)
                        {
                            yingui.writer2.Write((byte)((1 << 7) + (yingui.music_zan_3[i]>>7)));//如果大于7位
                        }
                        byte z = (byte)yingui.music_zan_3[i];
                        yingui.writer2.Write((byte)((byte)(z<<1)>>1));
                        break;
                    case "down":
                        yingui.writer2.Write((byte)yingui.music_zan_3[i]);//模式与通道
                        yingui.writer2.Write((byte)(yingui.music_zan_3[i]>>8));//音高
                        yingui.writer2.Write((byte)(yingui.music_zan_3[i]>>16));//力度
                        if (biaoji[i + 1].Equals("down"))
                        {
                            yingui.writer2.Write((byte)0);
                        }//和弦中各音符之间的间隔
                        break;
                    case "up":
                        yingui.writer2.Write((byte)yingui.music_zan_3[i]);//模式与通道
                        yingui.writer2.Write((byte)(yingui.music_zan_3[i] >> 8));//音高
                        yingui.writer2.Write((byte)0x0);//力度
                        if (biaoji[i + 1].Equals("up"))
                        {
                            yingui.writer2.Write((byte)0);
                        }//和弦中各音符之间的间隔
                        break;
                }
            }//化为mid格式的音轨
            yingui.writer2.Seek(-1, SeekOrigin.End);
            if (yingui.note_long > 16383)
            {
                yingui.writer2.Write((byte)((1 << 7) + (byte)(yingui.note_long >> 14)));//如果大于14位
            }
            if (yingui.note_long > 127)
            {
                yingui.writer2.Write((byte)((1 << 7) + (yingui.note_long >> 7)));//如果大于7位
            }
            byte d = (byte)yingui.note_long;
            yingui.writer2.Write((byte)((byte)(d << 1) >> 1));//间隔

            yingui.writer2.Write(yingui_end);//写入音轨尾
            if (yingui.writer2.BaseStream.Length > 65535)
            {
                yingui.writer2.Seek(5, SeekOrigin.Begin);//到之前预留的空位
                yingui.writer2.Write((byte)(yingui.writer2.BaseStream.Length >> 16));
            }
            if (yingui.writer2.BaseStream.Length > 255)
            {
                yingui.writer2.Seek(6, SeekOrigin.Begin);//到之前预留的空位
                yingui.writer2.Write((byte)(yingui.writer2.BaseStream.Length >>8));
            }
            yingui.writer2.Seek(7, SeekOrigin.Begin);//到之前预留的空位
            yingui.writer2.Write((byte)(yingui.writer2.BaseStream.Length-8));//写入音轨长度

            yingui.writer2.Seek(0, SeekOrigin.Begin);//回到开头
            yingui.writer2.BaseStream.CopyTo(yingui.writer1.BaseStream);//将音轨附加到文件
            yingui.writer2.Close();

            Console.WriteLine(Environment.CurrentDirectory + "\\yingui" + yingui.index.ToString() + ".mid");//音轨保存地址
            yingui.writer1.Close();

            //writer.Write(music_zan_4[index]);
            //midiOutShortMsg(midiOut, 0x4f << 16 | 0x40 << 8 | 0x90);
            //midi_play(me[index], midiOut);
            //Console.WriteLine("a");
        }//废弃！！！！！！！！！！！！！！！！！！！！！！！
        */
        /// <summary>
        /// 将单音轨暂存合成多音轨文件
        /// </summary>
        /// <param name="num">
        /// 现在有几个音轨
        /// </param>
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

