using System;
using System.Collections.Generic;
using System.Text;

namespace LubaEngine.Types
{
    public interface IScene {
        void OnLoad();
        void Start();
        void Update();

        void OnUnload();
    }
}
