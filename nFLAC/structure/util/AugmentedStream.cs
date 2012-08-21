using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace org.nflac.structure.util
{
    /// <summary>
    /// This class allows for the stream to append a sequence of bytes to the end or the beginning of the stream
    /// When bytes are added they are read before (or after the actual stream is being read)
    /// </summary>
    //class AugmentedStream : Stream
    //{
    //    private Stream realStream;

    //    private byte[] augBefore;
    //    private byte[] augAfter;

    //    private int augBeforeRead;
    //    private int augAfterRead;

    //    private bool bAugBefore;
    //    private bool bAugAfter;

    //    public AugmentedStream(Stream str)
    //    {
    //        realStream = str;
    //    }

    //    public override int Read(byte[] buffer, int offset, int count)
    //    {
    //        int augBytes = 0;
    //        int buffFill = 0;
    //        int leftRead = count;

    //        int ret = 0;

    //        if (bAugBefore)
    //        {
    //            augBytes = augBefore.Length - augBeforeRead;

    //            if (count < augBytes)
    //            {
    //                augBytes = count;
    //            }

    //            for (int i = 0; i < augBytes; i++)
    //            {
    //                buffer[offset+buffFill++] = augBefore[i + augBeforeRead];
    //                ret++;
    //            }

    //            augBeforeRead += augBytes;

    //            if (augBeforeRead == augBefore.Length)
    //            {
    //                bAugBefore = false;
    //            }

    //            leftRead -= augBytes;
    //        }

    //        if (leftRead > 0)
    //        {
    //            //if we need to read from the real stream the virtual has obviously ended
    //            bAugBefore = false;

    //            int num = realStream.Read(buffer, offset + buffFill, leftRead);

    //            if (num < leftRead)
    //            {
    //                if (bAugAfter)
    //                {
    //                    buffFill += num;
    //                    leftRead -= num;

    //                    augBytes = augAfter.Length - augAfterRead;

    //                    if (leftRead < augBytes)
    //                    {
    //                        augBytes = leftRead;
    //                    }

    //                    for (int i = 0; i < augBytes; i++)
    //                    {
    //                        buffer[offset + buffFill++] = augAfter[i + augAfterRead];
    //                        ret++;
    //                    }

    //                    augAfterRead += augBytes;

    //                    if (augAfterRead == augAfter.Length)
    //                    {
    //                        bAugAfter = false;
    //                    }
    //                }
    //                else
    //                {
    //                    if (ret==0){
    //                        ret = num;
    //                    }
    //                }
    //            }
    //            else
    //            {
    //                ret += num;
    //            }
    //        }

    //        return ret;
    //    }

    //    public void AugmentBefore(byte[] array)
    //    {
    //        if (bAugBefore)
    //        {
    //            //cannot augment augmented stream. actually i do but that would lead to inobvious result
    //        }
    //        else
    //        {
    //            bAugBefore = true;
    //            augBefore = (byte[]) array.Clone();
    //            augBeforeRead = 0;
    //        }
    //    }

    //    public void AugmentAfter(byte[] array)
    //    {
    //        if (bAugAfter)
    //        {
    //            //cannot augment augmented stream. actually i do but that would lead to inobvious result
    //        }
    //        else
    //        {
    //            bAugAfter = true;
    //            augAfter = (byte[])array.Clone();
    //            augAfterRead = 0;
    //        }
    //    }
    //}
}
