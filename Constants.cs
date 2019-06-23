namespace NbsPlayerPlugin
{
    public static class Constants
    {
        public const string Prefix = "§a";

        public static readonly string[] InstrumentValues = new string[]
        {
            "block.note_block.harp", //dirt, harp
            "block.note_block.basedrum",
            "block.note_block.bass", //wood, bass / double bass
            "block.note_block.snare", //sand, snare
            "block.note_block.hat", //glass, click / hat
            "block.note_block.guitar", //wool, guitar
            "block.note_block.flute", //clay, flute
            "block.note_block.bell", //gold block, bell
            "block.note_block.chime", //packed ice, chime
            "block.note_block.xylophone", //bone block, xylophone
            //108, //???, pling
        };

        public static readonly float[] PitchValues = new float[]
        {
            0.5f,
            0.529732f,
            0.561234f,
            0.594604f,
            0.629961f,
            0.667420f,
            0.707107f,
            0.749154f,
            0.793701f,
            0.840896f,
            0.890899f,
            0.943874f,
            1f,
            1.059463f,
            1.122462f,
            1.189207f,
            1.259921f,
            1.334840f,
            1.414214f,
            1.498307f,
            1.587401f,
            1.681793f,
            1.781797f,
            1.887749f,
            2f
        };
    }
}