using com.TheDisappointedProgrammer.IOCC;

[Bean]
public class Level1
{
    [BeanReference]
    private Level12a childOne;
    [BeanReference]
    private Level12b childTwo;
}

[Bean]
public class Level12a
{
    [BeanReference]
    private Level12a3a childOne;
    [BeanReference]
    private Level12a3b childTwo;
}

[Bean]
public class Level12a3a
{
}

[Bean]
public class Level12a3b
{
}

[Bean]
public class Level12b
{
    [BeanReference]
    private Level12b3a childOne;
    [BeanReference]
    private Level12b3b childTwo;
}

[Bean]
public class Level12b3a
{
}

[Bean]
public class Level12b3b
{
}

public class Vanilla
{
}