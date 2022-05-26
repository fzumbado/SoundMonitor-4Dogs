using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundMonitor4Dogs
{
    public class PlaySound
    {
        /// <summary>
        /// File path of file sound to play
        /// </summary>
        string Filename { get; set; }


        /// <summary>
        /// Public constructor. Can also use new Sound(string Filename).
        /// </summary>
        public PlaySound() { }
        /// <summary>
        /// Constructor declaring the filename to be played
        /// </summary>
        /// <param name="fileName"></param>
        public PlaySound(string fileName) => Filename = fileName.Trim();


        /// <summary>
        /// Plays the sound set in the propery. Can also use Play(string Filename)
        /// </summary>
        public void Play(bool sameThread = false)
        {
            if (Filename.Equals(string.Empty))
                throw new Exception("Must declare Filename property first. Can be done with constructor or Play(filename)");

            System.Media.SoundPlayer player = new System.Media.SoundPlayer();
            player.SoundLocation = Filename;

            if(sameThread)
                player.PlaySync(); //in same thread
            else
                player.Play(); //in another thread
        }

        /// <summary>
        /// Plays sound declared in parameter
        /// </summary>
        /// <param name="fileName">string for file path</param>
        public void Play(string fileName)
        {
            Filename = fileName.Trim();
            Play();
        }

    }
}
