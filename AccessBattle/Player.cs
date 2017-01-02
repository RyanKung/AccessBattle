﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AccessBattle
{
    public class Player : PropChangeNotifier
    {
        string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value) return;
                _name = value;
                OnPropertyChanged();
            }
        }
        public int Points { get; set; }

        public bool DidVirusCheck { get; set; }
        public bool Did404NotFound { get; set; }

        public Player()
        {
        }
    }
}
