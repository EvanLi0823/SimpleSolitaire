using System;
using System.Threading.Tasks;
using UnityEngine;

namespace SimpleSolitaire
{
    public static class TaskUtils
    {
        public static async Task Wait(float seconds)
        {
            var startTime = Time.time;
            while (startTime + seconds > Time.time)
            {
                await Task.Yield();
            }
        }

        public static async Task WaitUntil(Func<bool> predicate)
        {
            while (!predicate())
            {
                await Task.Yield();
            }
        }
    }
}