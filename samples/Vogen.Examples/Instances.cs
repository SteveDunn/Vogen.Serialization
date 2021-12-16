﻿using System;

namespace Vogen.Examples
{
    
    /*
     * Instances allow us to create specific static readonly instances of this type.
     */
    
    [ValueObject(typeof(float))]
    [Instance("Freezing", 0.0f)]
    [Instance("Boiling", 100.0f)]
    [Instance("AbsoluteZero", -273.15f)]
    public readonly partial struct Centigrade
    {
        public static Validation Validate(float value) =>
            value >= Instances.Centigrade.AbsoluteZero.Value ? Validation.Ok : Validation.Invalid("Cannot be colder than absolute zero");
    }

    /*
     * Instances are the only way to avoid validation, so we can create instances
     * that nobody else can. This is useful for creating special instances
     * that represent concepts such as 'invalid' and 'unspecified'.
     */
    [ValueObject(typeof(int))]
    [Instance("Unspecified", -1)]
    [Instance("Invalid", -2)]
    public readonly partial struct Age
    {
        private static Validation Validate(int value) =>
            value > 0 ? Validation.Ok : Validation.Invalid("Must be greater than zero.");
    }

    [ValueObject(typeof(int))]
    [Instance("Unspecified", 0)]
    [Instance("Invalid", -1)]
    public partial class VendorId
    {
        private static Validation Validate(int value) =>
            value > 0 ? Validation.Ok : Validation.Invalid("Must be greater than zero.");
    }

    [ValueObject(typeof(string))]
    [Instance("Invalid", "[INVALID]")]
    public partial class VendorName
    {
    }

    public class VendorInformation
    {
        public VendorId VendorId { get; private init; } = Instances.VendorId.Unspecified;

        public static VendorInformation FromTextFile()
        {
            // image the text file is screwed...
            return new VendorInformation
            {
                VendorId = Instances.VendorId.Invalid
            };
        }
    }

    public class VendorRelatedThings
    {
        public VendorName GetVendorName(VendorId id)
        {
            if (id == Instances.VendorId.Unspecified) 
                throw new InvalidOperationException("The vendor ID was unspecified");

            // throw if invalid
            if (id == Instances.VendorId.Invalid) 
                throw new InvalidOperationException("The vendor ID was invalid");
            
            // or record it as invalid
            if (id == Instances.VendorId.Invalid) return Instances.VendorName.Invalid;

            return Instances.VendorName.From("abc");
        }
    }

    internal static class RepresentingUnspecified
    {
        public static void Run()
        {
            VendorInformation vi = new VendorInformation();
            Console.WriteLine(vi.VendorId == Instances.VendorId.Unspecified); // true
            Console.WriteLine(vi.VendorId != Instances.VendorId.Invalid); // true

            // from a text file that is screwed, we'll end up with:
            var invalidVi = VendorInformation.FromTextFile();
            
            Console.WriteLine(invalidVi.VendorId == Instances.VendorId.Invalid); // true
        }
    }
}