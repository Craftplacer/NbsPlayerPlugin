namespace NbsPlayerPlugin
{
    public static class Constants
    {
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

        public static readonly char[] ColorValues = new char[]
        {
            '9', //dirt, harp
            'a', //wood, bass / double bass
            'c',
            'e',
            'd', //glass, click / hat
            '4', //wool, guitar
            '6', //clay, flute
            '5', //gold block, bell
            '3', //packed ice, chime
            '7', //bone block, xylophone
        };

        public static readonly string[] NoteValues = new string[]
        {
            "F#3",
            "G3",
            "G#3",
            "A3",
            "A#3",
            "B3",
            "C4",
            "C#4",
            "D4",
            "D#4",
            "E4",
            "F4",
            "F#4",
            "G4",
            "G#4",
            "A4",
            "A#4",
            "B4",
            "C5",
            "C#5",
            "D5",
            "D#5",
            "E5",
            "F5",
            "F#5"
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