1️⃣ Definition of Thread Safety

A piece of code (method, class, or variable) is thread-safe if it can be safely invoked by multiple threads at the same time without causing:

Race conditions – unpredictable results due to concurrent access.

Data corruption – inconsistent or invalid state.

Crashes or deadlocks – unexpected exceptions or hangs.

In short:

Thread-safe code behaves correctly regardless of how many threads access it concurrently.

2️⃣ Examples
Unsafe (not thread-safe)
class Counter
{
public int Value = 0;

    public void Increment()
    {
        Value++; // Not atomic!
    }
}

var counter = new Counter();
Parallel.For(0, 1000, i => counter.Increment());
Console.WriteLine(counter.Value); // Likely < 1000 due to race conditions


Problem: Value++ is not atomic. Two threads can read the same value before incrementing → lost updates.

Safe (thread-safe)

Option 1: Using lock

class Counter
{
private object _lock = new object();
public int Value = 0;

    public void Increment()
    {
        lock (_lock)
        {
            Value++;
        }
    }
}


lock ensures only one thread at a time can execute the critical section.

Option 2: Using Interlocked

class Counter
{
private int _value = 0;

    public void Increment()
    {
        Interlocked.Increment(ref _value);
    }

    public int Value => _value;
}


Interlocked.Increment is atomic and much faster than lock for simple operations.

3️⃣ Key Principles of Thread Safety

Immutable objects – If data never changes after creation, it’s inherently thread-safe.

class Person
{
public string Name { get; }
public int Age { get; }
public Person(string name, int age) { Name = name; Age = age; }
}


Synchronization – Protect shared state with lock, Monitor, Mutex, or SemaphoreSlim.

Atomic operations – Use Interlocked for simple numeric updates.

Avoid shared state if possible – Use local variables or thread-local storage.

Thread-safe collections – Use ConcurrentDictionary, ConcurrentQueue, BlockingCollection, etc.

4️⃣ Thread Safety vs Concurrency

Concurrency: Multiple threads run simultaneously.

Thread safety: Code works correctly when accessed concurrently.

Concurrency does not automatically mean thread-safe. You must design for safety.

5️⃣ Summary

Thread-safe = “safe to use by multiple threads at the same time without errors or corruption.”

Achieved by synchronization, atomic operations, or immutable data.

Non-thread-safe code can cause race conditions, data corruption, and unpredictable behavior.