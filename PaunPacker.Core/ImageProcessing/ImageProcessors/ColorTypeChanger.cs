using System;
using System.Threading;
using PaunPacker.Core.Types;
using SkiaSharp;

namespace PaunPacker.Core.ImageProcessing.ImageProcessors
{
    /// <summary>
    /// Default implementation of <see cref="ColorTypeChangerBase"/>
    /// </summary>
    public class ColorTypeChanger : ColorTypeChangerBase
    {
        /// <summary>
        /// Constructs new ColorTypeChanger from a target ColorType (the ColorType to which the current ColorType should be changed)
        /// </summary>
        /// <param name="newColorType">The target ColorType</param>
        public ColorTypeChanger(SKColorType newColorType) : base(newColorType)
        {

        }

        /// <inheritdoc />
        public override PPImage ChangeColorType(PPImage input, CancellationToken token = default)
        {
            return ChangeColorType(input, NewColorType, token);
        }

        /// <inheritdoc />
        public override PPImage ChangeColorType(PPImage input, SKColorType type, CancellationToken token = default)
        {
            if (input == null)
            {
                throw new ArgumentNullException($"The {nameof(input)} cannot be null");
            }

            NewColorType = type;
            return new PPImage(input.Bitmap.Copy(NewColorType), input.ImagePath)
            {
                ImageName = input.ImageName
            };
        }
    }
}
