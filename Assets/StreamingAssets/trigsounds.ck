global int whichSound;
global Event triggered;




while(true)
{
    triggered => now;
    spork ~ playTrig(whichSound);
}

fun void playTrig(int whichSound)
{
    
    if (whichSound == 1)
    {
        SndBuf impactBuf => dac;
        me.dir() + "impact.wav" => impactBuf.read;
        0 => impactBuf.pos;
        
        0.5 => impactBuf.gain;
        
        impactBuf.length() =>  now;
    }
    
    if (whichSound == 2)
    {
        SndBuf impactBuf => dac;
        me.dir() + "808bd3.wav" => impactBuf.read;
        0 => impactBuf.pos;
        
        0.75 => impactBuf.gain;
        
        impactBuf.length() =>  now;
    }
    
    if (whichSound == 3)
    {
        SndBuf impactBuf => dac;
        me.dir() + "808bd3.wav" => impactBuf.read;
        0 => impactBuf.pos;
        
        0.5 => impactBuf.gain;
        
        impactBuf.length() =>  now;
    }
}
