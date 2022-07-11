using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ADLParallelTest
{

    internal class Program
    {
        static ConcurrentStack<uint> cpuidList = new ConcurrentStack<uint>();

        [DllImport("kernel32.dll")]
        extern public static uint GetCurrentProcessorNumber();
        static void Main(string[] args)
        {
            //初始化计时器变量
            Stopwatch sw = new Stopwatch();
            //启动计时器
            sw.Start();
            //运行并行for循环，次数设定为60
            Parallel.For(0, 60, (num) =>
            {
                //通过win32的processthreadsapi.h获取当前线程所运行的方法的处理器编号
                var cpuid = GetCurrentProcessorNumber();
                //放入容器中，ConcurrentStack是线程安全的
                cpuidList.Push(cpuid);
                //运行二叉树代码
                BinaryTrees.Test(18);
                //打印运行时间
                Console.WriteLine($"二叉树运行时间：{sw.Elapsed}，CPUID:{cpuid}");
            });
            sw.Stop();

            //排序取得运行次数最多的cpuid
            var TopCpu = cpuidList.GroupBy(s => s)
                                  .Select(s => new { cpuid = s.Key, count = s.Count() })
                                  .OrderByDescending(s => s.count)
                                  .First();
            //按照cpuid升序，用于显示哪颗核心运行了多少次
            var orderby_cpulist = cpuidList.GroupBy(s => s)
                                           .Select(s => new { cpuid = s.Key, count = s.Count() })
                                           .OrderBy(s => s.cpuid);
            //根据编号分组，大于等于12的是小核心（因为是12900H，6P（开超线程*2）+8E）
            var pecore_count = cpuidList.GroupBy(s => s < 12)
                                        .Select(s => new { PorE = s.Key ? "大核" : "小核", count = s.Count() })
                                        .OrderBy(s => s.count).ToArray();
            Console.WriteLine($"总共计数{cpuidList.Count}次，最多的核心为{TopCpu.cpuid}，运行了{TopCpu.count}次，" +
                            $"{pecore_count[0].PorE}运行了{pecore_count[0].count}次，{pecore_count[1].PorE}运行了{pecore_count[1].count}次");
            foreach (var item in orderby_cpulist)
            {
                Console.WriteLine($"cpu{item.cpuid}运行了{item.count}次");
            }
        }
    }
}
