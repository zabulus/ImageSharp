// Copyright (c) Six Labors.
// Licensed under the Six Labors Split License.

using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Pbm;

/// <summary>
/// Provides PBM specific metadata information for the image.
/// </summary>
public class PbmMetadata : IFormatMetadata<PbmMetadata>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PbmMetadata"/> class.
    /// </summary>
    public PbmMetadata() =>
        this.ComponentType = this.ColorType == PbmColorType.BlackAndWhite ? PbmComponentType.Bit : PbmComponentType.Byte;

    /// <summary>
    /// Initializes a new instance of the <see cref="PbmMetadata"/> class.
    /// </summary>
    /// <param name="other">The metadata to create an instance from.</param>
    private PbmMetadata(PbmMetadata other)
    {
        this.Encoding = other.Encoding;
        this.ColorType = other.ColorType;
        this.ComponentType = other.ComponentType;
    }

    /// <summary>
    /// Gets or sets the encoding of the pixels.
    /// </summary>
    public PbmEncoding Encoding { get; set; } = PbmEncoding.Plain;

    /// <summary>
    /// Gets or sets the color type.
    /// </summary>
    public PbmColorType ColorType { get; set; } = PbmColorType.Grayscale;

    /// <summary>
    /// Gets or sets the data type of the pixel components.
    /// </summary>
    public PbmComponentType ComponentType { get; set; } = PbmComponentType.Byte;

    /// <inheritdoc/>
    public static PbmMetadata FromFormatConnectingMetadata(FormatConnectingMetadata metadata)
    {
        PbmColorType color;
        PixelColorType colorType = metadata.PixelTypeInfo.ColorType ?? PixelColorType.Luminance;

        switch (colorType)
        {
            case PixelColorType.Binary:
                color = PbmColorType.BlackAndWhite;
                break;
            case PixelColorType.Luminance:
                color = PbmColorType.Grayscale;
                break;
            default:
                if (colorType.HasFlag(PixelColorType.RGB) || colorType.HasFlag(PixelColorType.BGR))
                {
                    color = PbmColorType.Rgb;
                }
                else
                {
                    color = PbmColorType.Grayscale;
                }

                break;
        }

        PbmComponentType componentType = PbmComponentType.Short;
        int bpp = metadata.PixelTypeInfo.BitsPerPixel;
        if (bpp == 1)
        {
            componentType = PbmComponentType.Bit;
        }
        else if (bpp <= 8)
        {
            componentType = PbmComponentType.Byte;
        }

        return new PbmMetadata
        {
            ColorType = color,
            ComponentType = componentType
        };
    }

    /// <inheritdoc/>
    public FormatConnectingMetadata ToFormatConnectingMetadata()
    {
        int bpp;
        PixelColorType colorType;
        PixelComponentInfo info;
        switch (this.ColorType)
        {
            case PbmColorType.BlackAndWhite:
                bpp = 1;
                colorType = PixelColorType.Binary;
                info = PixelComponentInfo.Create(1, bpp, 1);
                break;
            case PbmColorType.Grayscale:
                bpp = 8;
                colorType = PixelColorType.Luminance;
                info = PixelComponentInfo.Create(1, bpp, 8);
                break;
            case PbmColorType.Rgb:
                bpp = 24;
                colorType = PixelColorType.RGB;
                info = PixelComponentInfo.Create(3, bpp, 8, 8, 8);
                break;
            default:
                bpp = 8;
                colorType = PixelColorType.Luminance;
                info = PixelComponentInfo.Create(1, bpp, 8);
                break;
        }

        return new FormatConnectingMetadata
        {
            PixelTypeInfo = new PixelTypeInfo(bpp)
            {
                AlphaRepresentation = PixelAlphaRepresentation.None,
                ColorType = colorType,
                ComponentInfo = info,
            },
        };
    }

    /// <inheritdoc/>
    IDeepCloneable IDeepCloneable.DeepClone() => this.DeepClone();

    /// <inheritdoc/>
    public PbmMetadata DeepClone() => new(this);
}
