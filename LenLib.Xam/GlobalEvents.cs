﻿using System;

namespace LenLib.Xam
{
    public class GlobalEvents
    {
        public event EventHandler BackButtonPressed;

        public void OnBackButtonPressed()
        {
            var handler = BackButtonPressed;

            handler?.Invoke(this, new EventArgs());
        }
    }
}