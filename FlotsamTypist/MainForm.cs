using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace FlotsamTypist
{
    public partial class MainForm : Form
    {
        public const string LETTERS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public const string NUMBERS = "1234567890";
        public const string PUNCTUATION = "`~!@#$%^&*()-_=+[{]}\\|;:'\",<.>/?";

        public string FullTypewriterPhraseText { get; set; }

        public const string UNDERBAR = "_";

        public readonly int NumLetters;
        public readonly int NumNumbers;
        public readonly int NumPuncts;

        // rand.Next( 5, 10 ) ==> 5, 6, 7, 8, 9 at random; not "10", 10 is exclusive upper bound.
        // rand.Next( 5 ) ==> 0, 1, 2, 3, 4 at random, similarly.
        public Random rand = new Random();

        public MainForm()
        {
            InitializeComponent();

            NumLetters = LETTERS.Length;
            NumNumbers = NUMBERS.Length;
            NumPuncts = PUNCTUATION.Length;

            DoAnotherPhrase();
        }

        private void DoAnotherPhrase()
        {
            int numCharsAvailable = NumLetters + NumNumbers + NumPuncts;

            FullTypewriterPhraseText = "";

            // int howLongTypewriterPhrase = rand.Next( 5, 11 );
            int howLongTypewriterPhrase = 14;

            for (int j = 0; j < howLongTypewriterPhrase; ++j)
            {
                int randCharIndex = rand.Next( 0, numCharsAvailable );
                char randChar = (LETTERS + NUMBERS + PUNCTUATION)[ randCharIndex ];

                FullTypewriterPhraseText += randChar;
            }

            TextOverlay.Text = "";      // no text
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
            //
            Underliner.Text = string.Join( "", Enumerable.Repeat( ' ', TextOverlay.Text.Length )) + UNDERBAR;
        }

        private void MainForm_KeyPress( object sender, KeyPressEventArgs e )
        {
            switch (e.KeyChar)
            {
                case (char) Keys.Escape:
                    e.Handled = true;
                    this.Close();       // close the Form, end the application
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

            // Then revert back to the original background color.
            TextOverlay.Text = originalTextOverlayText;
            TypewriterPhrase.Text = originalTypewritePhraseText;
            TypewriterPhrase.ForeColor = originalTypewritePhraseColor;

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
            //
            TypewriterPhrase.Text = string.Join( "", Enumerable.Repeat( ' ', TextOverlay.Text.Length )) 
                + FullTypewriterPhraseText.Substring( numCorrectChars, FullTypewriterPhraseText.Length - numCorrectChars );
            
            SetUnderbarPosition();

            bool anyCharsRemaining = (Underliner.Text.Length <= FullTypewriterPhraseText.Length);
            return anyCharsRemaining;
        }
    }
}
