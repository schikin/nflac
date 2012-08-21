using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace org.nflac.structure.data
{
    enum ChannelAssignment
    {
        MONO,
        LEFT_RIGHT,
        LEFT_RIGHT_CENTER,
        LEFT_RIGHT_BLEFT_BRIGHT,                //left; right; back-left; back-right
        LEFT_RIGHT_CENTER_BLEFT_BRIGHT,         //left; right; center; back/surround left; back/surround right
        LEFT_RIGHT_CENTER_LFE_BLEFT_BRIGHT,     //left; right; center; LFE; back/surround left; back/surround right
        CH7,                                    //7 channels (undefined)
        CH8,                                    //8 channels (undefined)
        LEFT_SIDE,                              //left; side (difference)
        RIGHT_SIDE,                             //side (difference); right
        MID_SIDE,                               //mid(average); side(difference)
    }
}
