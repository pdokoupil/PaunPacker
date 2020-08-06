using System;
using System.Threading;
using PaunPacker.Core.Types;

namespace PaunPacker.Core.ImageProcessing.ImageProcessors
{
    /// <summary>
    /// Represents the Crop image processor
    /// </summary>
    /// <remarks>
    /// Same as Trim image processor <see cref="Trimmer"/> except that the modified size is also stored in the metadata
    /// Current implementation does not "keep position" (as they call it in TexturePacker) therefore the transparent pixels are simply removed and the offset stays at 0,0
    /// </remarks>
    public class CroppingTrimmer : TrimmerBase
    {
        /// <summary>
        /// Constructs a new CroppingTrimmer with alphaTolerance equal to 0
        /// </summary>
        public CroppingTrimmer()
        {
            trimmer = new Trimmer(0);
        }

        /// <summary>
        /// Constructs a new CroppingTrimmer with a given <paramref name="alphaTolerance"/>
        /// </summary>
        /// <param name="alphaTolerance">The alpha tolerance to be used when cropping</param>
        public CroppingTrimmer(int alphaTolerance)
        {
            trimmer = new Trimmer(alphaTolerance);
        }

        /// <summary>
        /// Performs the "Cropping" Trim operation (i.e. the Crop)
        /// </summary>
        /// <param name="input">Removes transparent pixels from border of the image permamently</param>
        /// <param name="token">The cancellation token</param>
        /// <remarks>The cropped pixels are reflected in the metadata</remarks>
        /// <returns>Cropped texture, unmodified <paramref name="input"/> if the cancellation was requested</returns>
        public override PPImage Trim(PPImage input, CancellationToken token = default)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            var result = trimmer.Trim(input, token);
            result.OriginalWidth = result.Bitmap.Width;
            result.OriginalHeight = result.Bitmap.Height;
            result.OffsetX = result.OffsetY = 0; //does not keep position
            return result;

        }

        private readonly Trimmer trimmer;
    }
}
