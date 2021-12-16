﻿#pragma warning disable CS0219

namespace Vogen.Examples
{
    /*
        You shouldn't be allowed to `default` a Value Object as it bypasses
        any validation you might have added.
    */
    
    public class Naughty
    {
        public Naughty()
        {
            // uncomment for - error VOG009: Type 'CustomerId' cannot be constructed with default as it is prohibited.
            // CustomerId c = default;
            // var c2 = default(CustomerId);

            // VendorId v = default;
            // var v2 = default(VendorId);
        }
    }

    [ValueObject(typeof(int))]
    public partial struct CustomerId { }

    [ValueObject(typeof(int))]
    public partial class VendorId { }
}