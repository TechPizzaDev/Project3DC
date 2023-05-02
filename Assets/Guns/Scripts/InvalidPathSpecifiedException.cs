using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Guns.Modifiers
{
    public class InvalidPathSpecifiedException : Exception
    {
        public InvalidPathSpecifiedException(string attributeName) : base($"{attributeName} does not excist at the provided path!") { }
    }
}

