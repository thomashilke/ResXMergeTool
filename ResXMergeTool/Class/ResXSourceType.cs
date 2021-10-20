using System;
using System.Resources;

namespace ResXMergeTool
{
    [Flags()]
    public enum ResXSourceType
    {
        UNKOWN = -1,
        CONFLICT = 0,
        BASE = 1,
        LOCAL = 2,
        REMOTE = 4,
        BASE_LOCAL = BASE | LOCAL,
        BASE_REMOTE = BASE | REMOTE,
        LOCAL_REMOTE = LOCAL | REMOTE,
        ALL = BASE | LOCAL | REMOTE,
        MANUAL = 8,
    }
}
