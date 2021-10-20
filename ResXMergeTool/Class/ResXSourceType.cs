using System;

namespace ResXMergeTool
{
    [Flags()]
    public enum ResXSourceType
    {
        UNKOWN = -1,
        BASE = 1,
        LOCAL = 2,
        REMOTE = 4,
        LOCAL_REMOTE = LOCAL | REMOTE,
        ALL = BASE | LOCAL | REMOTE,
        MANUAL = 8,
    }
}
