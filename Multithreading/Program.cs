using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Multithreading;

public static class Program
{
    private static async Task Main(string[] args)
    {
        // Console.WriteLine("Hello, World!");
        // var text = "";
        //
        //
        // var t1 = new Thread(param => Console.WriteLine(param));
        // var t2 = new Thread(param => Console.WriteLine(param));
        //
        //
        // t1.Name = "thread1";
        // t2.Name = "thread2";
        //
        //
        // t1.Start("t1 started");
        // Console.WriteLine("t1 started");
        // Console.WriteLine(t1.ManagedThreadId);
        // Console.WriteLine(t1);
        //
        //
        // //
        // var process = Process.GetCurrentProcess();
        // process.PriorityClass = ProcessPriorityClass.High;
        // foreach (ProcessThread  thread in process.Threads)
        // {
        //     Console.WriteLine(thread);
        //     Console.WriteLine(thread.Id);
        // }
        //
        // Console.ReadLine();
        //
        //
        // Thread.Sleep(3000); // Pause for 1 second
        //
        //
        //
        // Console.WriteLine("state:" + t1.ThreadState);
        // t1.Join(); // Wait for completion 
        // Console.WriteLine(t1.IsThreadPoolThread);
        // Console.WriteLine(t1.ThreadState);
        // // Console.WriteLine(t.Priority);
        // Console.WriteLine(t1.ExecutionContext);
        // Console.WriteLine(t1.IsAlive);
        // Console.WriteLine(t1.Name);
        // Console.WriteLine(t1);
        //
        // text = Console.ReadLine();
        // Console.WriteLine(text);

        SimpleSample();
        Console.WriteLine("\n");
        MidSample();
        Console.WriteLine("\n");
        await AdvancedSample();
    }

    private static void SimpleSample()
    {
        Console.WriteLine("SimpleSample Began");
        var threads = new List<Thread>();

        // Create and start some threads
        for (var i = 1; i <= 3; i++)
        {
            var t = new Thread(() =>
            {
                Console.WriteLine($"Hello from managed thread {Environment.CurrentManagedThreadId}");
                Thread.Sleep(2000); // simulate work
                Console.WriteLine($"Thread {Environment.CurrentManagedThreadId} finished work");
            })
            {
                Name = $"Thread-{i}"
            };
            t.Start();
            threads.Add(t);
        }

        // Give threads a moment to start
        Thread.Sleep(100);

        Console.WriteLine("\n--- Managed Threads Info ---");
        foreach (var t in threads)
        {
            Console.WriteLine($"Name: {t.Name}, ManagedThreadId: {t.ManagedThreadId}, IsAlive: {t.IsAlive}");
        }

        Console.WriteLine("\n--- OS Threads Info ---");
        var process = Process.GetCurrentProcess();
        foreach (ProcessThread pt in process.Threads)
        {
            Console.WriteLine($"OS Thread Id: {pt.Id}, State: {pt.ThreadState}, Priority: {pt.BasePriority}");
        }

        // Wait for threads to finish
        foreach (var t in threads)
            t.Join();

        Console.WriteLine("All threads finished.");
        Console.WriteLine("SimpleSample Finished");
    }

    private static void MidSample()
    {
        Console.WriteLine("MidSample Began");


        var threads = new List<Thread>();

        for (var i = 1; i <= 3; i++)
        {
            var t = new Thread(() =>
            {
                Console.WriteLine(
                    $"Managed thread {Environment.CurrentManagedThreadId} (Name: {Thread.CurrentThread.Name}) started, OS Thread ID: {GetCurrentThreadId()}");
                Thread.Sleep(2000); // simulate work
                Console.WriteLine(
                    $"Managed thread {Thread.CurrentThread.ManagedThreadId} finished, OS Thread ID: {GetCurrentThreadId()}");
            })
            {
                Name = $"Thread-{i}"
            };
            t.Start();
            threads.Add(t);
        }

        // Print all threads info from the process
        Thread.Sleep(100); // give threads time to start

        Console.WriteLine("\n--- All OS Threads in the Process ---");
        var process = Process.GetCurrentProcess();
        foreach (ProcessThread pt in process.Threads)
        {
            Console.WriteLine($"OS Thread Id: {pt.Id}, State: {pt.ThreadState}, Priority: {pt.BasePriority}");
        }

        // Wait for threads to finish
        foreach (var t in threads)
            t.Join();

        Console.WriteLine("All threads finished.");
        Console.WriteLine("MidSample Finished");
        return;

        // P/Invoke to get OS thread ID
        [DllImport("kernel32.dll")]
        static extern uint GetCurrentThreadId();
    }

    private static async Task AdvancedSample([System.Runtime.CompilerServices.CallerMemberName] string methodName = "")
    {
        
       // Console.WriteLine(MethodBase.GetCurrentMethod()!.Name + " has started");
        Console.WriteLine(methodName + " started");


        Console.WriteLine(
            $"Main method starts on ManagedThreadId={Thread.CurrentThread.ManagedThreadId}, OS ThreadId={GetCurrentThreadId()}");

        // Launch ThreadPool tasks using Task.Run
        var tasks = new Task[3];
        for (var i = 0; i < 3; i++)
        {
            var idx = i + 1;
            tasks[i] = Task.Run(() =>
            {
                Console.WriteLine(
                    $"Task.Run {idx} starts on ManagedThreadId={Thread.CurrentThread.ManagedThreadId}, OS ThreadId={GetCurrentThreadId()}");
                Thread.Sleep(1500); // simulate CPU-bound work
                Console.WriteLine(
                    $"Task.Run {idx} ends on ManagedThreadId={Thread.CurrentThread.ManagedThreadId}, OS ThreadId={GetCurrentThreadId()}");
            });
        }

        // Launch async method using await and Task.Delay (non-blocking)
        var asyncTask = MyAsyncMethod();

        // Wait for all tasks
        await Task.WhenAll(tasks);
        await asyncTask;

        Console.WriteLine("All work finished.");
        //Console.WriteLine(MethodBase.GetCurrentMethod()!.Name + " has completed.");
        Console.WriteLine(methodName + " ended");

        return;

        // P/Invoke to get the current OS thread ID
        [DllImport("kernel32.dll")]
        static extern uint GetCurrentThreadId();
    }

    private static async Task MyAsyncMethod()
    {
        Console.WriteLine(
            $"Async method starts on ManagedThreadId={Thread.CurrentThread.ManagedThreadId}, OS ThreadId={GetCurrentThreadId()}");
        await Task.Delay(2000); // non-blocking wait
        Console.WriteLine(
            $"Async method resumes on ManagedThreadId={Thread.CurrentThread.ManagedThreadId}, OS ThreadId={GetCurrentThreadId()}");
        return;

        // P/Invoke to get the current OS thread ID
        [DllImport("kernel32.dll")]
        static extern uint GetCurrentThreadId();
    }
    
    static void StartSample()
    {
        // create a new thread
        Thread t = new Thread(Worker);

        // start the thread
        t.Start();

        // do some other work in the main thread
        for (int i = 1; i < 5; i++)
        {
            Console.WriteLine("Main thread doing some work");
            Thread.Sleep(100);
        }

        // wait for the worker thread to complete
        t.Join();

        Console.WriteLine("Done");
    }

    static void Worker()
    {
        for (int i = 1; i < 3; i++)
        {
            Console.WriteLine("Worker thread doing some work");
            Thread.Sleep(100);
        }
    }
}