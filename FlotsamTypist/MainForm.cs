using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Configuration;

namespace FlotsamTypist
{
    public partial class MainForm : Form
    {
        public const string LETTERS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public const string NUMBERS = "1234567890";
        public const string PUNCTUATION = "`~!@#$%^&*()-_=+[{]}\\|;:'\",<.>/?";

        public const string CHARACTER_SET = LETTERS + NUMBERS + PUNCTUATION;

        public const string UNDERBAR = "_";

        public const int MIN_NUM_CHARS = 4;
        public const int MAX_NUM_CHARS = 120;

        public string FullTypewriterPhraseText { get; set; }

        public bool EscapeClosesApplication { get; set; }
        public bool ShowKeyboardHint { get; set; }
        public int NumCharactersToDisplay { get; set; }

        // rand.Next( 5, 10 ) ==> 5, 6, 7, 8, 9 at random; not "10", 10 is exclusive upper bound.
        // rand.Next( 5 ) ==> 0, 1, 2, 3, 4 at random, similarly.
        public Random rand = new Random();

        public MainForm()
        {
            InitializeComponent();

            // Having {Esc} close the app is convenient, but if you're practicing typing and
            // keep hitting {Esc} accidentally when reaching for the '~' or '1' keys, it very
            // quickly gets annoying.  Allow the user to turn it off.
            EscapeClosesApplication = AppSettings.Get< bool >( "EscapeClosesApplication", true );

            // Sometimes you can get "stuck" on a character: you repeatedly mistype it
            // and can't figure out what's wrong.  For me, the '{' '}' '-' '=' keys are
            // particularly problematic this way.  Prefer to NOT have to glance down at the
            // keyboard to reorient; instead, have an option for displaying the key you're
            // hitting.
            ShowKeyboardHint = AppSettings.Get< bool >( "ShowKeyboardHint" );
            if (ShowKeyboardHint)
            {
                const int shiftAmount = 6;      // shift down a little, make room for the keyboard hint text (aesthetics)
                ShiftDisplayTextsVerticallyBy( shiftAmount );
            }

            NumCharactersToDisplay = DetermineNumCharsToDisplay();

            // Scale the display window to correctly show that number of monospace characters.
            SetTextPhraseWidths( NumCharactersToDisplay );
            SetFormDisplayWidth( NumCharactersToDisplay );

            // Find out where the user wants the Form to appear.
            int formXOffset = AppSettings.Get< int >( "FormXPositionInPixels", 50 );
            int formYOffset = AppSettings.Get< int >( "FormYPositionInPixels", 50 );

            StartPosition = FormStartPosition.Manual;
            Location = new Point(formXOffset, formYOffset);

            DoAnotherPhrase();
        }

        private void DoAnotherPhrase()
        {
            FullTypewriterPhraseText = "";

            // int howLongTypewriterPhrase = rand.Next( 5, 11 );    // Hmm. Interesting idea, but doesn't work that well in practice.
            int howLongTypewriterPhrase = NumCharactersToDisplay;

            for (int j = 0; j < howLongTypewriterPhrase; ++j)
            {
                int randCharIndex = rand.Next( 0, CHARACTER_SET.Length );
                // char randChar = CHARACTER_SET[ 38 ];     // test with letter 'M'
                // char randChar = CHARACTER_SET[ 42 ];     // test with letter 'Q'
                // char randChar = CHARACTER_SET[ 48 ];     // test with letter 'W'
                char randChar = CHARACTER_SET[ randCharIndex ];

                FullTypewriterPhraseText += randChar;
            }

            TextOverlay.Text = "";      // reset: no "correct" text letters, yet
            TypewriterPhrase.Text = FullTypewriterPhraseText;

            SetUnderbarPosition();
        }

        private void ResetAndStartOver()
        {
            // Pause, give the user time to savor her success.
            Thread.Sleep( 200 );

            DoAnotherPhrase();
        }

        private void SetUnderbarPosition()
        {
            // Repeating number of characters in a string.  See:
            //
            // http://stackoverflow.com/questions/4115064/is-there-a-built-in-function-to-repeat-string-or-char-in-net
            // https://stackoverflow.com/questions/22617486/is-there-a-spacen-method-in-c-net
            //
            // Underliner.Text = string.Join( "", Enumerable.Repeat( ' ', TextOverlay.Text.Length )) + UNDERBAR;
            Underliner.Text = new string( ' ', TextOverlay.Text.Length ) + UNDERBAR;
        }

        private void MainForm_KeyPress( object sender, KeyPressEventArgs e )
        {
            if (ShowKeyboardHint && !string.IsNullOrWhiteSpace(KeyboardHint.Text))
            {
                const int asciiSpace = 32;
                DisplayKeyboardHint( asciiSpace );
            }

            switch (e.KeyChar)
            {
                case (char) Keys.Escape:
                    e.Handled = true;
                    if (EscapeClosesApplication)
                    {
                        this.Close();   // close the Form, end the application
                    }
                    break;

                case (char) Keys.Back:  // Backspace
                    e.Handled = true;
                    AdvanceCorrectCharacter( -1 );
                    break;

                default:
                    e.Handled = true;
                    if (IsCorrectNextKey( e.KeyChar ))
                    {
                        bool anyCharsRemaining = AdvanceCorrectCharacter();
                        if (!anyCharsRemaining)
                        {
                            ResetAndStartOver();
                        }
                    }
                    else
                    {
                        FlashFormBackground( 100 );

                        if (ShowKeyboardHint)
                        {
                            DisplayKeyboardHint( e.KeyChar );
                        }
                    }
                    break;
            }
        }

        private bool IsCorrectNextKey( int keyChar )
        {
            int currentPosn = TextOverlay.Text.Length;

            int correctKey = (int) TypewriterPhrase.Text[ currentPosn ];

            bool isCorrectKey = (keyChar == correctKey);

            return isCorrectKey;
        }

        private void FlashFormBackground( int milliseconds )
        {
            // Change the UI...
            string originalTextOverlayText = TextOverlay.Text;
            string originalTypewritePhraseText = TypewriterPhrase.Text;

            string fullText = originalTextOverlayText + originalTypewritePhraseText.Trim();

            TextOverlay.Text = "";    // no text
            TypewriterPhrase.Text = fullText;

            Color originalTypewritePhraseColor = TypewriterPhrase.ForeColor;
            TypewriterPhrase.ForeColor = Color.Black;

            Color originalKeyboardHintColor = KeyboardHint.ForeColor;
            KeyboardHint.ForeColor = Color.Black;

            Color originalFormBackgroundColor = this.BackColor;

            BackColor = Color.White;

            // ... and immediately update the screen to show it.  See:
            //
            // http://stackoverflow.com/questions/1360944/force-gui-update-from-ui-thread
            //
            Invalidate();
            Update();
            Refresh();
            Application.DoEvents();     // necessary! or the TransparentLabel's don't redraw.

            // Pause, give the user time to see and ponder the color update.
            Thread.Sleep( milliseconds );

            // Then revert back to the original Form background, and text foreground, colors.
            TextOverlay.Text = originalTextOverlayText;
            TypewriterPhrase.Text = originalTypewritePhraseText;
            TypewriterPhrase.ForeColor = originalTypewritePhraseColor;
            KeyboardHint.ForeColor = originalKeyboardHintColor;

            BackColor = originalFormBackgroundColor;
        }

        private bool AdvanceCorrectCharacter( int numCharsToAdvance = 1 )
        {
            int numCorrectChars = TextOverlay.Text.Length + numCharsToAdvance;

            // Lower bound check.
            if (numCorrectChars < 0)
            {
                numCorrectChars = 0;
            }

            // Upper bound check.
            if (numCorrectChars > FullTypewriterPhraseText.Length)
            {
                numCorrectChars = FullTypewriterPhraseText.Length;
            }

            TextOverlay.Text = FullTypewriterPhraseText.Substring( 0, numCorrectChars );

            // Repeating number of characters in a string.  See:
            //
            // http://stackoverflow.com/questions/4115064/is-there-a-built-in-function-to-repeat-string-or-char-in-net
            // https://stackoverflow.com/questions/22617486/is-there-a-spacen-method-in-c-net
            //
            // TypewriterPhrase.Text = string.Join( "", Enumerable.Repeat( ' ', TextOverlay.Text.Length )) 
            //     + FullTypewriterPhraseText.Substring( numCorrectChars, FullTypewriterPhraseText.Length - numCorrectChars );
            TypewriterPhrase.Text = new string( ' ', TextOverlay.Text.Length )
                + FullTypewriterPhraseText.Substring( numCorrectChars, FullTypewriterPhraseText.Length - numCorrectChars );
            
            SetUnderbarPosition();

            bool anyCharsRemaining = (Underliner.Text.Length <= FullTypewriterPhraseText.Length);
            return anyCharsRemaining;
        }

        private void DisplayKeyboardHint( int mistypedKey )
        {
            // In the discussion below, "X" is some character, or potentially string, that
            // acts as a hint to tell the user how they mistyped.  "X" might be a regular
            // ASCII character, but it might also be a Unicode graphic symbol like \u2b60 for
            // "left arrow" or something semi-exotic like that.  I'm not sure exactly what
            // I'll end up doing.
            //
            // So the goal here is to display some character "X", at the same horizontal
            // position, as the character the user mistyped.  If the font being used
            // to render the "X" was identical to the (monospaced) font used in the
            // TypewriterPhrase, we could position the "X" horizontally simply by counting
            // how many characters in the TypewriterPhrase the user had correctly typed so
            // far.  Easy-peasy.  That is the strategy used to position the underline which
            // moves along as the user types.
            // 
            // But that doesn't work if the font used for "X" is different than the
            // TypewriterPhrase font.  In that case we have to graphically measure the
            // position of the current character (where the user mistyped) and use that to
            // decide where to display the "X".
            //
            // Surprsingly this is not very hard to do.  The approach for measuring the
            // graphical display size of a rendered string is described here:
            //
            //      https://stackoverflow.com/questions/6704923/textrenderer-measuretext-and-graphics-measurestring-mismatch-in-size
            //
            SizeF typewriterTextDisplaySize;
            using( Graphics g = TypewriterPhrase.CreateGraphics() )
            {
                // The Underliner text (underscore) is already correctly positioned, so it's a
                // convenient reference for figuring out where to render the "X".
                typewriterTextDisplaySize = g.MeasureString(Underliner.Text, Underliner.Font);
            }

            float mistypedCharacterDisplayOffset = typewriterTextDisplaySize.Width;
            const int offsetFudgeFactor = 8;    // determined by experiment
            int newXLocation = offsetFudgeFactor + (int) Math.Round( mistypedCharacterDisplayOffset, 0, MidpointRounding.AwayFromZero );

            KeyboardHint.Location = new Point( newXLocation, KeyboardHint.Location.Y );
            KeyboardHint.Text = ((char) mistypedKey).ToString();    // show what the user mistyped
        }

        private void ShiftDisplayTextsVerticallyBy( int shiftAmount )
        {
            // DEV: I must be misunderstanding 'Point' operations because I could not for
            // the life of me get the .Offset() method to do anything.  Finally resorted to
            // the approach below.  Is there a better way to do this?
            //
            // [Later]: it seems, .Location cannot be updated via its .X and .Y properties;
            // can only replace the Location with a new one.  I guess that's why Offset() was
            // having no effect.  Not entirely clear.
            TextOverlay.Location = new Point( TextOverlay.Location.X, TextOverlay.Location.Y + shiftAmount );
            TypewriterPhrase.Location = new Point( TypewriterPhrase.Location.X, TypewriterPhrase.Location.Y + shiftAmount );
            Underliner.Location = new Point( Underliner.Location.X, Underliner.Location.Y + shiftAmount );
            KeyboardHint.Location = new Point( KeyboardHint.Location.X, KeyboardHint.Location.Y + shiftAmount );
        }

        private int DetermineNumCharsToDisplay()
        {
            int numCharactersToDisplay = AppSettings.Get< int >( "NumCharactersToDisplay" );

            // Lower bound check.
            if (numCharactersToDisplay < MIN_NUM_CHARS)
            {
                numCharactersToDisplay = MIN_NUM_CHARS;
            }

            // Upper bound check.
            if (numCharactersToDisplay > MAX_NUM_CHARS)
            {
                numCharactersToDisplay = MAX_NUM_CHARS;
            }

            return numCharactersToDisplay;
        }


        /// <summary>
        /// Scale the Text labels, proportionally to the number of monospace characters
        /// being displayed.
        /// </summary>
        private void SetTextPhraseWidths( int numCharactersToDisplay )
        {
            // Temporarily: use a wide letter (eg. 'W') and construct a display string of
            // the user-requested length.  For a monospace font it actually shouldn't matter
            // which letter we use, but 'W' clearly displays the width (during debugging).
            TypewriterPhrase.Text = new string( 'W', NumCharactersToDisplay );

            // Measure the width of the string.
            SizeF typewriterTextDisplaySize;
            using( Graphics g = TypewriterPhrase.CreateGraphics() )
            {
                typewriterTextDisplaySize = g.MeasureString(TypewriterPhrase.Text, TypewriterPhrase.Font);
            }

            int characterDisplayWidth = (int) Math.Round( typewriterTextDisplaySize.Width, MidpointRounding.AwayFromZero );

            // Use the width to scale all the Text overlay labels.
            TypewriterPhrase.Size = new Size( characterDisplayWidth, TypewriterPhrase.Size.Height );
            TextOverlay.Size = new Size( characterDisplayWidth, TextOverlay.Size.Height );
            Underliner.Size = new Size( characterDisplayWidth, Underliner.Size.Height );
        }

        private void SetFormDisplayWidth( int numCharactersToDisplay )
        {
            // Hijack the 'TypewriterPhrase' control, to figure out the width to use to set
            // the MainForm to.  This is somewhat of a kluge, but other more sane attempts to
            // set the MainForm width were not particularly reliable.  It was hard to find
            // an approach that worked properly across the entire range of MIN_NUM_CHARS and
            // MAX_NUM_CHARS without looking wonky.  The approach I finally settled on here,
            // at least works well.
            TypewriterPhrase.Text = new string( 'M', NumCharactersToDisplay );      // artifically set the display width

            // Figure out how wide the TypewriterPhrase.Text really is.
            SizeF typewriterTextDisplaySize;
            using( Graphics g = TypewriterPhrase.CreateGraphics() )
            {
                typewriterTextDisplaySize = g.MeasureString(TypewriterPhrase.Text, TypewriterPhrase.Font);
            }

            // Set the MainForm width to be as wide as the TypewriterPhrase display width,
            // plus some padding (margin) on each side.
            int margin = TypewriterPhrase.Location.X;

            int mainFormClientWidth = 
                (int) Math.Round( typewriterTextDisplaySize.Width + 2 * margin, 0, MidpointRounding.AwayFromZero );     // same margin on both sides

            this.ClientSize = new Size( mainFormClientWidth, this.ClientSize.Height );

            TypewriterPhrase.Text = "";   // reset
        }

        private void MainForm_Load( object sender, EventArgs e )
        {

        }
    }

    // Read a value from App.config. See:
    //
    //      https://stacktoheap.com/blog/2013/01/20/using-typeconverters-to-get-appsettings-in-net/
    //
    // Usage: bool debugSetting = AppSettings.Get<bool>("Debug");
    //
    public static class AppSettings
    {
        public static T Get< T >( string key, T defaultValue = default( T ))
        {
            string appSetting = ConfigurationManager.AppSettings[ key ];
            if (appSetting == null)     // not found
            {
                return defaultValue;
            }
            
            var converter = TypeDescriptor.GetConverter( typeof( T ));
            return (T) (converter.ConvertFromInvariantString( appSetting ));
        }
    }
}
