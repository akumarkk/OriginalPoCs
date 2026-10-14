using System;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    // ThreadStatic binds the value strictly to the current physical thread
    [ThreadStatic]
    private static string _requestId;

    static async Task Main(string[] args)
    {
        // Simulate two concurrent requests running at the same time
        Task task1 = HandleRequestAsync("REQ-AAA");
        Task task2 = HandleRequestAsync("REQ-BBB");

        await Task.WhenAll(task1, task2);
    }

    private static async Task HandleRequestAsync(string requestId)
    {
        // Set the thread-local value on whatever thread executes this first line
        _requestId = requestId;

        Console.WriteLine($"[Thread {Environment.CurrentManagedThreadId}] Start '{requestId}' | _requestId is: {_requestId}");

        // Simulate an asynchronous I/O wait (e.g., calling a database).
        // This releases the thread and introduces an await boundary.
        //await Task.Delay(100);

        // --- THE THREAD HOP HAPPENS HERE ---
        // When we resume, we are likely on a different thread, 
        // or the thread pool has reused this thread for something else.
        
        Console.WriteLine($"[Thread {Environment.CurrentManagedThreadId}] After await for '{requestId}' | _requestId is now: {_requestId ?? "(null/lost)"}");
    }
}