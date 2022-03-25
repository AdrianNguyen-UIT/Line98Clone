using System;

public static class Timer {
    private static float currentTime = 0.0f;
    private static bool stop = false;
    public static void Reset() => currentTime = 0.0f; 

    public static void Stop() => stop = true;
    public static void Resume() => stop = false;

    public static void Update(float time)
    {
        if (!stop)
            currentTime += time;
    }

    public static string Format() => TimeSpan.FromSeconds(currentTime).ToString("hh':'mm':'ss");

    public static void SetTime(float time) => currentTime = time;
    public static float GetTime() => currentTime;
}
