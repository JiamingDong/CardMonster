using System;

public class RandomUtils
{
    public static int seed;
    private static Random random;

    public static void Init()
    {
        random = new Random(seed);
    }

    /// <summary>
    /// 获取随机值，包括上下限
    /// </summary>
    public static int GetRandomNumber(int lowerLimit, int upperLimit)
    {
        return random.Next(lowerLimit, upperLimit + 1);
    }
}
