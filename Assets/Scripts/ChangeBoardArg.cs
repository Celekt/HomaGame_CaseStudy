using System;

public class ChangeBoardArg : EventArgs
{
    public int boardNumber;

    public ChangeBoardArg(int boardNumber)
    {
        this.boardNumber = boardNumber;
    }
}
