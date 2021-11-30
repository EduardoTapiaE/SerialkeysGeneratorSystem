using System;
using System.Collections.Generic;
using System.Text;

namespace KeysLibrary.Services
{
    public interface IKeysServices
    {
        string GeneratePublicKey();
        string GenerateSerialKey();
    }
}
