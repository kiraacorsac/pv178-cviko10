﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Threads
{
    /// <summary>
    /// Úkolem bude napsat program countdown, tedy nějaký odpočet.
    /// Countdown bude na začátku mít počet tiků, a rozmezí mezi tiky v sekundách.
    /// Za každý tik se zavolá funkce, specifikovaná v konstruktoru.
    /// Na konci se zavolá jiná funkce, také specifikovaná v konstruktoru.
    /// Countdown poběží na samostatném vláknu.
    /// </summary>
    public class Countdown
    {
        private readonly int duration;
        private int ticks;
        private readonly Action endFunction;
        private readonly Action tickFunction;

        /// <summary>
        /// Konstruktor třídy Countdown.
        /// </summary>
        /// <param name="duration">Trvání (v ticích)</param>
        /// <param name="secondsPerTick">Rozmezí mezi tiky (v sekundách)</param>
        /// <param name="endAct">Funkce na konci</param>
        /// <param name="tickAct">Funkce při tiku</param>
        public Countdown(int duration, int secondsPerTick, Action endAct, Action tickAct)
        {
            this.duration = duration;
            SecondsPerTick = secondsPerTick;
            endFunction = endAct;
            tickFunction = tickAct;
            ticks = 0;
        }

        /// <summary>
        /// Jeden tik
        /// </summary>
        public void Tick()
        {
            ticks++;
            tickFunction();
            if (ticks == duration)
            {
                endFunction();
                Stop();
            }
        }

        /// <summary>
        /// Zapnutí odpočtu.
        /// </summary>
        public void Start()
        {
            Enabled = true;
        }

        /// <summary>
        /// Vypnutí odpočtu.
        /// </summary>
        private void Stop()
        {
            Enabled = false;
        }

        /// <summary>
        /// Vlastnost popisující, zda odpočet běží.
        /// </summary>
        public bool Enabled { get; set; }

        public int SecondsPerTick { get; }
    }
}
